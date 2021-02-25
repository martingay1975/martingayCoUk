using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using DocXLib.Image;
using DocXLib.Model.Data.Xml;
using HtmlAgilityPack;
using WebDataEntry.Web.Application;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace DocXLib
{
    //TODO: Need to make a contents page:
    // Page Cover and TOC on document 2
    // Need to tie the header table with the contents table.
    public static class Start
    {
        private const bool IncludePictures = true;
        private const bool CompareLocalAndHostImages = false;

        private const int STARTATCHUNKIDX = 1;
        private readonly static Color HeadingTitleColor = Color.FromArgb(103, 88, 65);
        public readonly static Color RedColor = Color.FromArgb(238, 48, 48);
        public readonly static Color GreenColor = Color.FromArgb(98, 238, 48);
        public readonly static Color YellowColor = Color.FromArgb(233, 238, 48);
        public readonly static Color OrangeColor = Color.FromArgb(237, 125, 49);

        public readonly static Color PinkColor = Color.FromArgb(229, 129, 196);
        public readonly static Color PurpleColor = Color.FromArgb(160, 83, 203);

        public readonly static Color PageNumberColor = PinkColor;

        public const string DocXDirectory = @"C:\Users\Slop\Desktop\docx\";
        public const string ChapterImageDirectory = DocXDirectory + @"Chapters\";
        private const string DiaryXmlPath = @"C:\Users\Slop\AppData\Roaming\res\xml\diary.xml";
        private const int KatiePersonId = 502;
        private readonly static float[] HeadingColumnWidths = new[] { 400f, 100f };
        public const float ResizeChapterPics = 1.03f;

        public const string ResourcesDirectory = DocXDirectory + @"Resources\";
        public const string BirthdayIcon = ResourcesDirectory + @"birthdayCake.png";
        public const string BoyIcon = ResourcesDirectory + @"blueBottle.png";
        public const string GirlIcon = ResourcesDirectory + @"pinkBottle.png";
        public const string XmasIcon = ResourcesDirectory + @"xmasTree.png";

        public static void Run(int? idx = null)
        {
            var diary = Load.LoadXml(DiaryXmlPath);

            var startAtChunkIdx = idx ?? STARTATCHUNKIDX;
            var startAtAllKatieEntriesIdx = DocumentSlices.GetStartIndex(startAtChunkIdx);
            var takeEntries = DocumentSlices.DocumentList[startAtChunkIdx].DiaryEntriesCount;

            if (IncludePictures == false)
            {
                startAtAllKatieEntriesIdx = 0;
                takeEntries = 3000;
            }

            var allKatieEntries = diary.Entries.Where(entry => entry.People.Contains(KatiePersonId)).ToList();
            var documentEntries = allKatieEntries.Skip(startAtAllKatieEntriesIdx).Take(takeEntries);

            var previousYear = 0;
            var previousMonth = (int?)0;
            if (startAtAllKatieEntriesIdx > 0)
            {
                var previousEntry = allKatieEntries[startAtAllKatieEntriesIdx - 1];
                previousMonth = previousEntry.DateEntry.Month;
                previousYear = previousEntry.DateEntry.Year;
            }

            CreateDocument(allKatieEntries, documentEntries, startAtChunkIdx, startAtAllKatieEntriesIdx, previousYear, previousMonth);
        }

        private static void CreateDocument(in IEnumerable<Entry> allKatieEntries, in IEnumerable<Entry> documentEntries, int startAtChunkIdx, int startAtAllKatieEntriesIdx, int previousYear, int? previousMonth)
        {
            DocX document = DocumentSetup.CreateAndSetupDocument(documentEntries, startAtChunkIdx, IncludePictures, DocXDirectory);

            var chunkedEntries = documentEntries.ToList();
            var chunkedEntriesLength = chunkedEntries.Count;
            var srcs = new List<string>();
            
            if (DocumentSlices.TryGetStartOfDocumentSlice(startAtChunkIdx, out var documentSlice))
            {
                if (!DocumentSetup.IncludeBleeding)
                {
                    DocumentSetup.InsertDocumentFrontPage(document, documentSlice.BookNumber.Value);
                }

                DocumentYears.InsertDocumentTOC(DocumentSetup.DocumentSectionManager);
            }

            if (CompareLocalAndHostImages)
            {
                CompareImages.Run(chunkedEntries, DocXDirectory, document);
                return;
            }

            // Loop around each of the entries
            for (var entryCounter = 0; entryCounter < chunkedEntries.Count; entryCounter ++)
            {
                var entry = chunkedEntries[entryCounter];
                var entryContext = new EntryContext(document, entry);

                Console.WriteLine($"{entryCounter} / {chunkedEntriesLength} - {entryContext.Entry.DateEntry.GetShortDate()}  ({startAtAllKatieEntriesIdx + entryCounter})");

                if (entry.DateEntry.Year != previousYear)
                {
                    if (IncludePictures)
                    {
                        var chapterPageAndTOCSection = DocumentSetup.DocumentSectionManager.AddSection(new SectionInfo() { Type = SectionInfo.SectionInfoType.ChapterImage, Year = entry.DateEntry.Year });
                        InsertChapterImagePage(document, chapterPageAndTOCSection, entry.DateEntry.Year);

                        DocumentSetup.DocumentSectionManager.AddSection(new SectionInfo() { Type = SectionInfo.SectionInfoType.ChapterTOC, Year = entry.DateEntry.Year });
                        InsertYearTOC(document, allKatieEntries.Where(e => e.DateEntry.Year == entry.DateEntry.Year).ToList());
                    }
                    
                    // Add a blank page introducing the year.
                    CreateYearEntryPages(entry.DateEntry.Year);
                    previousYear = entry.DateEntry.Year;
                    previousMonth = 0;
                }

                if (entry.DateEntry.Month != previousMonth)
                {
                    // put the month and year as a mini heading
                    var monthParagraph = document.InsertParagraph();
                    monthParagraph.InsertHorizontalLine(HorizontalBorderPosition.top, BorderStyle.Tcbs_single, 10, 1, PageNumberColor);
                    monthParagraph.Append(entry.DateEntry.GetLongMonthAndYear());
                    monthParagraph.Color(PageNumberColor)
                        .FontSize(48)
                        .Bold(true)
                        .CapsStyle(CapsStyle.caps)
                        .SpacingBefore(16);

                    previousMonth = entry.DateEntry.Month.Value;
                }

                if (!DocumentSetup.DocumentSectionManager.HaveAddedSections())
                {
                    // if the document starts half way through the entries for the year, need to create the section
                    CreateYearEntryPages(entry.DateEntry.Year);
                }

                document.InsertParagraph("").SpacingBefore(10);
                CreateDiaryHeader(document, entry, startAtAllKatieEntriesIdx + entryCounter);
                CreateDiaryContent(entryContext);
                AddPictures(entryContext);

                try
                {
                    var lookForwardYear = chunkedEntries[entryCounter + 1].DateEntry.Year;
                    if (lookForwardYear != entry.DateEntry.Year)
                    {
                        DocumentSetup.DocumentSectionManager.AddSection(new SectionInfo() { Type = SectionInfo.SectionInfoType.Eof });
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }

            if (DocumentSlices.IsLastSliceInBook(startAtChunkIdx))
            {
                DocumentSetup.DocumentSectionManager.AddSection(new SectionInfo() { Type = SectionInfo.SectionInfoType.Eof });
            }

            var startPageNum = documentSlice?.StartPageNumber;
            HeadersAndFooters.AddSectionBits(document.Sections, DocumentSetup.DocumentSectionManager, startPageNum);

            try
            {
                document.Save();
                document.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("Please close the Word document down so we can overwrite it. Then press any key.");
                Console.ReadKey();
                document.Save();
                document.Dispose();
            }
        }


        private static Section CreateYearEntryPages(int year)
        {
            return DocumentSetup.DocumentSectionManager.AddSection(new SectionInfo() { Type = SectionInfo.SectionInfoType.ChapterEntries, Year = year });
        }

        private static void InsertChapterImagePage(Document document, Section section, int year)
        {
            var yearParagraph = section.InsertParagraph();
            var chapterPicture = PictureHelper.CreatePicture(document, Path.Combine(ChapterImageDirectory, $"{year}.jpg"));
            chapterPicture.Width *= ResizeChapterPics;
            chapterPicture.Height *= ResizeChapterPics;
            yearParagraph.AppendPicture(chapterPicture);
        }

        private static void InsertYearTOC(Document document, List<Entry> yearEntries)
        {
            var months = yearEntries.GroupBy(e => e.DateEntry.Month).Select((m, en) => m.Key.Value);
            var entryCounter = 0;
            var previousMonth = 0;
            var paragraph = document.InsertParagraph(yearEntries.First().DateEntry.Year.ToString())
                .FontSize(14)
                .Bold(true);

            var options = new TableHelper.Options
            {
                ColumnWidths = TableHelper.TocColumnWidths,
                VisitCellFunc = (int rowIndex, int columnIndex, float columnWidth, Cell cell) =>
                {
                    var cellParagraph = cell.Paragraphs[0];

                    switch (columnIndex)
                    {
                        case 0:
                            {
                                var entry = yearEntries[entryCounter];
                                if (entry.DateEntry.Month.Value != previousMonth)
                                {
                                    // new month
                                    var monthName = DateEntry.GetMonthName(entry.DateEntry.Month.Value);
                                    cellParagraph.Append(monthName).ApplyDateFormatting();
                                    previousMonth = entry.DateEntry.Month.Value;
                                }
                                else
                                {
                                    AddSpecialIcon(document, entry, cellParagraph);
                                    // just add the title
                                    cellParagraph.Append($"\t{entry.Title.Value}").FontSize(9);
                                    entryCounter++;
                                }
                                break;
                            }
                        case 1:
                            {
                                cellParagraph.Alignment = Alignment.right;
                                break;
                            }
                    }

                    cell.VerticalAlignment = VerticalAlignment.Center;
                    return true;
                }
            };

            var rowCount = yearEntries.Count + months.Count();
            TableHelper.CreateTable(null, paragraph, rowCount, options);
        }

        private static void AddSpecialIcon(Document document, Entry entry, Paragraph paragraph)
        {
            if ((entry.DateEntry.Month.Value == 7 && entry.DateEntry.Day.Value == 24 && entry.DateEntry.Year == 2003) 
                || (entry.DateEntry.Month.Value == 9 && entry.DateEntry.Day.Value == 11 && entry.DateEntry.Year == 2007))
            {
                InsertPicture(GirlIcon);
            }

            if (entry.DateEntry.Month.Value == 8 && entry.DateEntry.Day.Value == 5 && entry.DateEntry.Year == 2005)
            {
                InsertPicture(BoyIcon);
            }

            if (entry.DateEntry.Month.Value == 7 && entry.DateEntry.Day.Value == 24)
            {
                InsertPicture(BirthdayIcon);
            }

            if (entry.DateEntry.Month.Value == 12 && (entry.DateEntry.Day.Value == 24 || entry.DateEntry.Day.Value == 25))
            {
                InsertPicture(XmasIcon);
            }

            void InsertPicture(string imagePath)
            {
                var picture = PictureHelper.CreatePicture(document, imagePath);
                SizePicture(picture, new Size(18, 18));
                paragraph.InsertPicture(picture);
                paragraph.Append("  ");
            }
        }

        private static void CreateDiaryHeader(in Document document, in Entry entry, in int allKatieEntriesIdx)
        {
            // Add a table in a document of 1 row and 3 columns.
            
            var table = document.InsertTable(1, HeadingColumnWidths.Length);
            
            // Set the table's column width and background 
            table.SetWidths(HeadingColumnWidths);
            table.AutoFit = AutoFit.Contents;
            table.Design = TableDesign.None;

            var row = table.Rows.First();

            // Fill in the columns of the first row in the table.
            // Title
            var titleParagraph = row.Cells[0].Paragraphs.First();

            titleParagraph.Heading(HeadingType.Heading3);
            AddSpecialIcon(document, entry, titleParagraph);
            titleParagraph.Append(entry.Title.Value)
                .CapsStyle(CapsStyle.caps)
                .FontSize(16)
                .Spacing(3)
                .Bold(true)
                .Color(PurpleColor);

            row.Cells[0].VerticalAlignment = VerticalAlignment.Center;

            // Date
            var dateParagraph = row.Cells[1].Paragraphs.First();
            dateParagraph.Append(entry.DateEntry.GetLongDate())
                .ApplyDateFormatting();

            dateParagraph.Alignment = Alignment.right;

            // entry number
            row.Cells[1].InsertParagraph(allKatieEntriesIdx.ToString()).ApplyDateFormatting().FontSize(6).Italic(true).Alignment = Alignment.right;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Center;
        }

        public static Paragraph ApplyDateFormatting(this Paragraph dateParagraph)
        {
            return dateParagraph.CapsStyle(CapsStyle.caps)
                .FontSize(9)
                .Color(PinkColor);
        }

        private static void CreateDiaryContent(in EntryContext entryContext)
        {
            // Create a table with 1 row and 1 column - serves as a container
            var paragraph = entryContext.Document.InsertParagraph("");
            var section = entryContext.Document.Sections.Last();
            entryContext.Container = TableHelper.CreateTable(section, null, 1, new TableHelper.Options() { ColumnCountIfNoWidths = 1 });
            HtmlHelper.ReadOriginalInfoContent(entryContext, NodeHandler);
        }

        private static void NodeHandler(in EntryContext entryContext, HtmlNode htmlNode)
        {
            var wesawAttributeValue = htmlNode.GetAttributeValue("wesaw", "");
            if (wesawAttributeValue.Length > 0 && wesawAttributeValue.IndexOf(KatiePersonId.ToString()) == -1)
            {
                // there are "wesaw" values, but did not include the target person
                return;
            }

            switch (htmlNode.Name)
            {

                case "woops":
                {
                    foreach (var paragraphChildNode in htmlNode.ChildNodes)
                    {
                        NodeHandler(entryContext, paragraphChildNode);
                    }
                    break;
                }
                case "p":
                case "first":
                {
                    var text = HtmlHelper.GetText(htmlNode);
                    entryContext.SetContentParagraph(text);
                    foreach (var paragraphChildNode in htmlNode.ChildNodes)
                    {
                        NodeHandler(entryContext, paragraphChildNode);
                    }

                    break;
                }
                case "ul":
                {
                    var list = entryContext.Document.AddList();
                    foreach (var listItemNode in htmlNode.ChildNodes)
                    {
                        var text = "\t•  " + HtmlHelper.GetText(listItemNode);
                        entryContext.SetContentParagraph(text, true);
                    }

                    break;
                }
                case "image":
                {
                    if (IncludePictures)
                    {
                        var picture = PictureHelper.CreateEntryPicture(entryContext, HtmlHelper.GetChildNodeValue(htmlNode, "src"), HtmlHelper.GetChildNodeValue(htmlNode, "caption"));
                        if (picture != null)
                        {
                            entryContext.Pictures.Add(picture);
                        }
                    }
                    break;
                }
                case "#text":
                {
                    // do nothing - already handled.
                    break;
                }
                case "div" when htmlNode.Attributes.Any(a => a.Value == "youtube"):
                {
                    break;
                    // do nothing for now. Although maybe take a screen grab
                }
                case "a":
                {
                    //entryContext.Paragraph.Append(GetText(htmlNode));
                    break;
                }
                case "googlelink":
                {
                    entryContext.Paragraph.Append(" " + HtmlHelper.GetText(htmlNode) + " ");
                    break;
                }
                default:
                {
                    var msg = $"'{htmlNode.Name}' not processed at '{entryContext.Entry.DateEntry.GetShortDate()}' == '{entryContext.Entry.Title?.Value}'";
                    throw new Exception(msg);
                }
            }
        }

        private static void AddPictures(EntryContext entryContext)
        {
            var picCount = entryContext.Pictures.Count;
            if (picCount == 0)
            {
                return; // no pictures so need to continue
            }

            if (picCount == 1 )
            {
                // insert a picture before any text
                var firstParagraph = entryContext.Document.Tables.Last().Rows.Last().Cells.Last().Paragraphs.First();
                var picture = entryContext.Pictures[0];
                var xml = picture.Xml.ToString();
                // UseLicensedVersion - START
                picture.WrappingStyle = PictureWrappingStyle.WrapTight;
                picture.WrapText = PictureWrapText.right;
                picture.VerticalAlignment = WrappingVerticalAlignment.TopRelativeToLine;
                picture.DistanceFromTextLeft = 7;
                picture.DistanceFromTextRight = 7;
                picture.DistanceFromTextBottom = 7;
                // UseLicensedVersion - END

                SizePicture(picture, new Size(225, 300));
                
                firstParagraph.SpacingBefore(0);
                firstParagraph.KeepLinesTogether(false);
                firstParagraph.InsertPicture(picture);
                
                return;
            }

            var columnCount = (picCount % 3 == 0 || picCount > 4) ? 3 : 2;
            var rowCount = Math.Ceiling(picCount / (float)columnCount);

            var pictureEnumerator = entryContext.Pictures.GetEnumerator();

            // placing mulitple pictures in a table
            var options = new TableHelper.Options
            {
                ColumnCountIfNoWidths = columnCount,
                VisitCellFunc = (int rowIndex, int columnIndex, float columnWidth, Cell cell) =>
                {
                    // visitor for each cell in the table.

                    if (!pictureEnumerator.MoveNext())
                    {
                        // no more pictures to display
                        return false;
                    }

                    var picture = pictureEnumerator.Current;

                    var columnMaxSize = new Size((int)columnWidth - 10, (int)(columnWidth * 1.25));
                    SizePicture(picture, columnMaxSize);

                    var cellParagraph = cell.Paragraphs.First();
                    cellParagraph.AppendPicture(picture);
                    cellParagraph.InsertCaptionAfterSelf(picture.Name);

                    return true;
                }
            };

            var paragraph = entryContext.Document.Paragraphs.Last();
            TableHelper.CreateTable(null, paragraph, (int)rowCount, options);
        }

        private static void SizePicture(Picture picture, Size pictureMaxSize)
        {
            var pictureSize = new Size((int)picture.Width, (int)picture.Height);
            var newSize = ImageExtension.CalculateNewSize(pictureMaxSize, pictureSize, (maxSize, _) => maxSize);
            picture.Width = newSize.Width;
            picture.Height = newSize.Height;
        }
    }
}