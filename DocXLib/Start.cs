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
using Font = Xceed.Document.NET.Font;

namespace DocXLib
{
    // TODO:
    // The year TOC only includes what is in the document.... Should include whole year
    // The first page number is missing if document starts half way through the year
    public static class Start
    {
        private const bool IncludePictures = false;
        private const bool CompareLocalAndHostImages = false;

        private const bool UseLicensedVersion = true;
        private const int STARTATCHUNKIDX = 1;
        private readonly static Color HeadingTitleColor = Color.FromArgb(103, 88, 65);
        private readonly static Color DateColor = Color.FromArgb(173, 165, 107);
        public readonly static Color PageNumberColor = Color.FromArgb(237, 125, 49);

        public const string DocXDirectory = @"C:\Users\Slop\Desktop\docx\";
        public const string ChapterImageDirectory = DocXDirectory + @"Chapters\";
        private const string DiaryXmlPath = @"C:\Users\Slop\AppData\Roaming\res\xml\diary.xml";
        private const int KatiePersonId = 502;
        private readonly static float[] HeadingColumnWidths = new[] { 400f, 100f };
        private readonly static float[] TocColumnWidths = new[] { 450f, 50f };
        private static DocumentSectionManager documentSectionManager;
        private const float ResizeChapterPics = 1f;
        public const int pageNumberJumpYear = 2011;
        public const int pageNumberJumpYearPN = 700;

        //public readonly static List<int> ChunkStartIdx = new List<int> { 
        //    /*  0 */ 0, 
        //    /*  1 */ 800, 
        //    /*  2 */ 950, 
        //    /*  3 */ 1200, 
        //    /*  4 */ 1410, 
        //    /*  5 */ 1650, 
        //    /*  6 */ 1800, 
        //    /*  7 */ 2000, 
        //    /*  8 */ 2250, 
        //    /*  9 */ 2600};

        public readonly static List<DocumentSlice> ChunkLength = new List<DocumentSlice>
        {
            /*  0 */ new DocumentSlice(800),
            /*  1 */ new DocumentSlice(150),
            /*  2 */ new DocumentSlice(250),
            /*  3 */ new DocumentSlice(150),
            /*  4 */ new DocumentSlice(260),
            /*  5 */ new DocumentSlice(120),
            /*  6 */ new DocumentSlice(170),
            /*  7 */ new DocumentSlice(100),
            /*  8 */ new DocumentSlice(150),
            /*  9 */ new DocumentSlice(350)
        };  

        public static void Run(int? idx = null)
        {
            var diary = Load.LoadXml(DiaryXmlPath);

            var startAtChunkIdx = idx ?? STARTATCHUNKIDX;
            var startAtAllKatieEntriesIdx = GetStartIndex(startAtChunkIdx);
            var takeEntries = ChunkLength[startAtChunkIdx].DiaryEntriesCount;

            if (IncludePictures == false)
            {
                startAtAllKatieEntriesIdx = 0;
                takeEntries = 100;
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
            string filePath;
            if (IncludePictures)
            {
                var documentFromPostfix = documentEntries.First().DateEntry.GetShortDate().Replace(" ", "-");
                var documentLastPostfix = documentEntries.Last().DateEntry.GetShortDate().Replace(" ", "-");
                filePath = Path.Combine(DocXDirectory, $"diary{startAtChunkIdx} {documentFromPostfix} to {documentLastPostfix}.docx");
            }
            else
            {
                filePath = Path.Combine(DocXDirectory, $"diaryNoPics.docx");
            }

            var document = DocX.Create(filePath);
            documentSectionManager = new DocumentSectionManager(document);

            var font = new Font("Calibri (Body)");
            document.SetDefaultFont(font, 11d, Color.Black);
            document.DifferentOddAndEvenPages = true;
            document.DifferentFirstPage = true;
            ApplyStandardMargins(document);

            var counter = 0;
            var chunkedEntries = documentEntries.ToList();
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
                Console.WriteLine($"{counter} / {chunkedEntriesLength} - {entryContext.Entry.DateEntry.GetShortDate()}  ({startAtAllKatieEntriesIdx + counter})");

                if (entry.DateEntry.Year != previousYear)
                {
                    // Add a blank page introducing the year.
                    CreateYearPages(document, entry.DateEntry.Year, allKatieEntries.Where(e => e.DateEntry.Year == entry.DateEntry.Year));
                    previousYear = entry.DateEntry.Year;
                    previousMonth = 0;
                }

                if (entry.DateEntry.Month != previousMonth)
                {
                    // put the month and year as a mini heading
                    var monthParagraph = document.InsertParagraph(entry.DateEntry.GetLongMonthAndYear());
                    monthParagraph.Color(PageNumberColor)
                        .FontSize(16)
                        .Bold(true)
                        .CapsStyle(CapsStyle.caps)
                        .SpacingBefore(16);

                    previousMonth = entry.DateEntry.Month.Value;
                }

                document.InsertParagraph("").SpacingBefore(10);
                CreateDiaryHeader(document, entry, startAtAllKatieEntriesIdx + counter);
                CreateDiaryContent(entryContext);

                AddPictures(entryContext);
            }

            HeadersAndFooters.AddSectionBits(document.Sections, documentSectionManager, chunkedEntries);

            document.Save();
            document.Dispose();
        }



        private static void ApplyStandardMargins(Document document)
        {
            document.MarginBottom = 40;
            document.MarginTop = 40;
            document.MarginLeft = 55;
            document.MarginRight = 55;
            document.MirrorMargins = true;
        }

        private static void ApplyStandardMargins(Section section)
        {
            section.MarginBottom = 40;
            section.MarginTop = 40;
            section.MarginLeft = 55;
            section.MarginRight = 55;
            section.MirrorMargins = true;
        }

        private static void CreateYearPages(Document document, int year, IEnumerable<Entry> yearEntries)
        {
            var chapterPageAndTOCSection = documentSectionManager.AddSection(new SectionInfo() { Type = SectionInfo.SectionInfoType.ChapterImage , Year = year});
            InsertChapterImagePage(document, chapterPageAndTOCSection, year);
            
            var restOfYearSection = documentSectionManager.AddSection(new SectionInfo() { Type = SectionInfo.SectionInfoType.ChapterEntries, Year = year });

            ApplyStandardMargins(restOfYearSection);
            InsertYearTOC(document, yearEntries.ToList());
            restOfYearSection.InsertParagraph("").InsertPageBreakAfterSelf();
        }

        private static void InsertChapterImagePage(Document document, Section section, int year)
        {
            section.MarginBottom = 10;
            section.MarginTop = 10;
            section.MarginLeft = 25;
            section.MarginRight = 25;

            var yearParagraph = section.InsertParagraph();
            var chapterPicture = PictureHelper.CreatePicture(document, Path.Combine(ChapterImageDirectory, $"{year}.jpg"));
            chapterPicture.Width = chapterPicture.Width * ResizeChapterPics;
            chapterPicture.Height = chapterPicture.Height * ResizeChapterPics;
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

            var rowCount = yearEntries.Count + months.Count();
            TableHelper.CreateTable(null, paragraph, rowCount, options);
        }

        private static void InsertDocumentTOC(Document document)
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

            var paragraph = document.InsertParagraph("");
            TableHelper.CreateTable(null, paragraph, 18, options);
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

            // entry number
            row.Cells[1].InsertParagraph(allKatieEntriesIdx.ToString()).ApplyDateFormatting().FontSize(6).Italic(true).Alignment = Alignment.right;

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
            var paragraph = entryContext.Document.InsertParagraph("");
            var section = entryContext.Document.Sections.Last();
            entryContext.Container = TableHelper.CreateTable(section, null, 1, new TableHelper.Options() { ColumnCountIfNoWidths = 1 });
            HtmlHelper.ReadOriginalInfoContent(entryContext, NodeHandler);
            //entryContext.Container.Paragraphs.Last().InsertHorizontalLine(HorizontalBorderPosition.bottom, BorderStyle.Tcbs_single, 25);
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

            var paragraph = entryContext.Document.InsertParagraph();
            TableHelper.CreateTable(null, paragraph, (int)rowCount, options);
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
                startIndex += ChunkLength[i].DiaryEntriesCount;
            }

            return startIndex;
        }
    }

    public class DocumentSectionManager
    {
        private readonly Document document;

        public Dictionary<int, SectionInfo> SectionInfos { get; }

        public DocumentSectionManager(Document document)
        {
            this.document = document ?? throw new ArgumentNullException(nameof(document));
            this.SectionInfos = new Dictionary<int, SectionInfo>();
        }

        public Section AddSection(SectionInfo sectionInfo)
        {
            var section = document.InsertSectionPageBreak();
            var sectionIdx = document.Sections.Count - 1;
            this.SectionInfos[sectionIdx] = sectionInfo;
            return section;
        }
    }

}