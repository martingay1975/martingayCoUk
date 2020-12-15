using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using DocXLib.Model.Data.Xml;
using HtmlAgilityPack;
using WebDataEntry.Web.Application;
using Xceed.Document.NET;
using Xceed.Words.NET;
using Font = Xceed.Document.NET.Font;

namespace DocXLib
{
    public static class Start
    {
        private const bool AutoTOC = false;
        private const bool UseLicensedVersion = false;
        private const int STARTATCHUNKIDX = 1;
        private const bool IncludePictures = false;
        private const bool CompareLocalAndHostImages = true;
        private readonly static Color HeadingTitleColor = Color.FromArgb(103, 88, 65);
        private readonly static Color DateColor = Color.FromArgb(173, 165, 107);
        public const string DocXDirectory = @"C:\Users\Slop\Desktop\docx\";
        public const string ChapterImageDirectory = DocXDirectory + @"Chapters\";
        private const string DiaryXmlPath = @"C:\Users\Slop\AppData\Roaming\res\xml\diary.xml";
        private const int KatiePersonId = 502;
        private readonly static float[] HeadingColumnWidths = new[] { 480f, 120f };
        private readonly static float[] TocColumnWidths = new[] { 540f, 60f };
        private const float ResizeChapterPics = 0.9f;

        //public readonly static List<int> ChunkStartIdx = new List<int> { 
        //    /*  0 */ 0, 
        //    /*  1 */ 800, 
        //    /*  2 */ 950, 
        //    /*  3 */ 1200, 
        //    /*  4 */ 1400, 
        //    /*  5 */ 1650, 
        //    /*  6 */ 1800, 
        //    /*  7 */ 2000, 
        //    /*  8 */ 2250, 
        //    /*  9 */ 2600};

        public readonly static List<int> ChunkLength = new List<int>
        {
            /*  0 */ 800,
            /*  1 */ 150,
            /*  2 */ 250,
            /*  3 */ 150,
            /*  4 */ 250,
            /*  5 */ 150,
            /*  6 */ 150,
            /*  7 */ 100,
            /*  8 */ 150,
            /*  9 */ 350
        };  

        public static void Run(int? idx = null)
        {
            ImageSimilarity.Create();

            var diary = Load.LoadXml(DiaryXmlPath);

            var startAtChunkIdx = idx ?? STARTATCHUNKIDX;
            var startAt = GetStartIndex(startAtChunkIdx);
            var takeEntries = ChunkLength[startAtChunkIdx];

            if (IncludePictures == false)
            {
                startAt = 0;
                takeEntries = 3000;
            }

            var entries = diary.Entries.Where(entry => entry.People.Contains(KatiePersonId)).ToList();
            var chunkedEntries = entries.Skip(startAt).Take(takeEntries);

            var previousYear = 0;
            var previousMonth = (int?)0;
            if (startAt > 0)
            {
                var previousEntry = entries[startAt - 1];
                previousMonth = previousEntry.DateEntry.Month;
                previousYear = previousEntry.DateEntry.Year;
            }

            CreateDocument(chunkedEntries, startAtChunkIdx, previousYear, previousMonth);
        }

        private static void CreateDocument(in IEnumerable<Entry> entries, int idx, int previousYear, int? previousMonth)
        {
            string filePath;
            if (IncludePictures)
            {
                var documentFromPostfix = entries.First().DateEntry.GetShortDate().Replace(" ", "-");
                var documentLastPostfix = entries.Last().DateEntry.GetShortDate().Replace(" ", "-");
                filePath = Path.Combine(DocXDirectory, $"diary{idx} {documentFromPostfix} to {documentLastPostfix}.docx");
            }
            else
            {
                filePath = Path.Combine(DocXDirectory, $"diaryNoPics.docx");
            }

            var document = DocX.Create(filePath);
            var font = new Font("Calibri (Body)");
            document.SetDefaultFont(font, 11d, Color.Black);
            //document.DifferentOddAndEvenPages = true;

            var counter = 0;
            var chunkedEntries = entries.ToList();
            var chunkedEntriesLength = chunkedEntries.Count;
            var srcs = new List<string>();

            if (chunkedEntries.First().DateEntry.Year <= 2003)
            {
                InsertDocumentTOC(document);
            }

            if (CompareLocalAndHostImages)
            {
                CompareImages.Run(chunkedEntries, DocXDirectory, document);
                return;
            }

            foreach (var entry in chunkedEntries)
            {
                var entryContext = new EntryContext(document, entry);

                counter++;
                Console.WriteLine($"{counter} / {chunkedEntriesLength} - {entryContext.Entry.DateEntry.GetShortDate()}");

                if (entry.DateEntry.Year != previousYear)
                {
                    // Add a blank page introducing the year.
                    CreateYearPages(document, entry.DateEntry.Year, chunkedEntries.Where(e => e.DateEntry.Year == entry.DateEntry.Year));
                    previousYear = entry.DateEntry.Year;
                    previousMonth = 0;
                }

                if (entry.DateEntry.Month != previousMonth)
                {
                    var month = document.InsertParagraph(entry.DateEntry.GetLongMonthAndYear());
                    month.Heading(HeadingType.Heading2);
                    previousMonth = entry.DateEntry.Month.Value;
                }

                document.InsertParagraph("").SpacingBefore(10);
                CreateDiaryHeader(document, entry);
                CreateDiaryContent(entryContext);

                AddPictures(entryContext);
            }

            document.Save();
            document.Dispose();
        }

        private static void CreateYearPages(DocX document, int year, IEnumerable<Entry> yearEntries)
        {
            document.InsertSectionPageBreak();

            Paragraph yearParagraph;
            if (AutoTOC)
            {
                yearParagraph = document.InsertParagraph(year.ToString()).FontSize(50);
                yearParagraph.Heading(HeadingType.Heading1);
                document.InsertParagraph("TODO Following image.... please make Wrap over text to cover up this Heading text");
            }
            else
            {
                yearParagraph = document.InsertParagraph();
            }

            InsertChapterImagePage(document, year, yearParagraph);
            InsertYearTOC(document, yearEntries.ToList());
            document.InsertParagraph("");

            document.InsertSectionPageBreak();

            var yearSection = document.Sections.Last();
            yearSection.AddFooters();
            yearSection.SectionBreakType = SectionBreakType.evenPage;
            yearSection.Footers.Even.InsertParagraph($"E #\t{year}")
                .AppendPageNumber(PageNumberFormat.normal);

            yearSection.Footers.Even.Paragraphs[0].Alignment = Alignment.left;

            yearSection.Footers.Odd.InsertParagraph($"\t{year}\tO #")
                .AppendPageNumber(PageNumberFormat.normal);
        }

        private static void InsertChapterImagePage(Document document, int year, Paragraph yearParagraph)
        {
            var chapterPicture = PictureHelper.CreatePicture(document, Path.Combine(ChapterImageDirectory, $"{year}.jpg"));
            chapterPicture.Width = chapterPicture.Width * ResizeChapterPics;
            chapterPicture.Height = chapterPicture.Height * ResizeChapterPics;
            yearParagraph.AppendPicture(chapterPicture);
            document.InsertSectionPageBreak();
        }

        private static void InsertYearTOC(Document document, List<Entry> yearEntries)
        {
            var months = yearEntries.GroupBy(e => e.DateEntry.Month).Select((m, en) => m.Key.Value);
            var entryCounter = 0;
            var previousMonth = 0;
            document.InsertParagraph(yearEntries.First().DateEntry.Year.ToString())
                .FontSize(14)
                .Bold(true);

            var options = new TableHelper.Options
            {
                ColumnWidths = TocColumnWidths,
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

            TableHelper.CreateTable(document, yearEntries.Count + months.Count(), options);
        }

        private static void InsertDocumentTOC(DocX document)
        {
            if (AutoTOC)
            {

                var switches = new Dictionary<TableOfContentsSwitches, string>() {
                { TableOfContentsSwitches.O, "1-3" },
                { TableOfContentsSwitches.U, "" } };

                document.InsertTableOfContents("", switches);
            }
            else
            {
                // TOC for whole document
                var options = new TableHelper.Options
                {
                    ColumnWidths = TocColumnWidths,
                    VisitCellFunc = (int rowIndex, int columnIndex, float columnWidth, Cell cell) =>
                    {
                        var cellParagraph = cell.Paragraphs[0];
                        switch (columnIndex)
                        {
                            case 0:
                                {
                                    cellParagraph.Append($"{rowIndex + 2003}");
                                    cellParagraph.ApplyDateFormatting();
                                    cellParagraph.FontSize(14);
                                    break;
                                }

                            case 1:
                                {
                                    cellParagraph.Append("TODO");
                                    cellParagraph.Alignment = Alignment.right;
                                    break;
                                }
                        }
                    
                        cell.VerticalAlignment = VerticalAlignment.Center;
                        return true;
                    }
                };

                TableHelper.CreateTable(document, 18, options);
            }
        }

        private static void CreateDiaryHeader(in DocX document, in Entry entry)
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
            titleParagraph.Append(entry.Title.Value)
                .CapsStyle(CapsStyle.caps)
                .FontSize(14)
                .Spacing(3)
                .Bold(true)
                .Color(HeadingTitleColor);

            row.Cells[0].VerticalAlignment = VerticalAlignment.Center;

            // Date
            var dateParagraph = row.Cells[1].Paragraphs.First();
            dateParagraph.Append(entry.DateEntry.GetLongDate())
                .ApplyDateFormatting();

            dateParagraph.Alignment = Alignment.right;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Center;
        }

        public static Paragraph ApplyDateFormatting(this Paragraph dateParagraph)
        {
            return dateParagraph.CapsStyle(CapsStyle.caps)
                .FontSize(9)
                .Color(DateColor);
        }

        private static void CreateDiaryContent(in EntryContext entryContext)
        {
            // Create a table with 1 row and 1 column - serves as a container
            entryContext.Container = TableHelper.CreateTable(entryContext.Document, 1, new TableHelper.Options() { ColumnCountIfNoWidths = 1 });

            HtmlDocument htmlDocument = HtmlHelper.ReadOriginalInfoContent(entryContext, NodeHandler);

            htmlDocument = null;
            GC.Collect();
        }

        private static string GetText(HtmlNode htmlNode)
        {
            // need to unescape html eg. &gt; to >
            var ret = WebUtility.HtmlDecode(htmlNode.InnerText)
                .Replace('\n', ' ')
                .Trim();

            while (ret.IndexOf("  ") > -1)
            {
                ret = ret.Replace("  ", " ");
            }

            return ret;
        }

        private static void NodeHandler(in EntryContext entryContext, HtmlNode htmlNode)
        {
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
                    var text = GetText(htmlNode);
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
                        var text = "\t•  " + GetText(listItemNode);
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
                    entryContext.Paragraph.Append(" " + GetText(htmlNode) + " ");
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
                var firstParagraph = entryContext.Paragraph;
                var picture = entryContext.Pictures[0];

                // UseLicensedVersion - START
                //picture.WrappingStyle = PictureWrappingStyle.WrapTight;
                //picture.WrapText = PictureWrapText.right;
                //picture.VerticalAlignment = WrappingVerticalAlignment.TopRelativeToLine;
                //picture.DistanceFromTextLeft = 7;
                //picture.DistanceFromTextRight = 7;
                //picture.DistanceFromTextBottom = 7;
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

            // placing the pictures in a table
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

            TableHelper.CreateTable(entryContext.Document, (int)rowCount, options);
        }

        private static void SizePicture(Picture picture, Size pictureMaxSize)
        {
            var pictureSize = new Size((int)picture.Width, (int)picture.Height);
            var newSize = ImageExtension.CalculateNewSize(pictureMaxSize, pictureSize, (maxSize, _) => maxSize);
            picture.Width = newSize.Width;
            picture.Height = newSize.Height;
        }

        private static int GetStartIndex(int chunkIdx)
        {
            int startIndex = 0;
            for (var i = 0; i < chunkIdx; i++)
            {
                startIndex += ChunkLength[i];
            }

            return startIndex;
        }

    }
}