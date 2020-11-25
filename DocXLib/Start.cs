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
        private const bool UseLicensedVersion = false;
        private const int STARTATCHUNKIDX = 1;
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

        private static int GetStartIndex(int chunkIdx)
        {
            int startIndex = 0;
            for (var i=0; i<chunkIdx; i++)
            {
                startIndex += ChunkLength[i];
            }

            return startIndex;
        }

        public const string DocXDirectory = @"C:\temp\docx\";
        private const string DiaryXmlPath = @"C:\Users\Slop\AppData\Roaming\res\xml\diary.xml";
        private const int KatiePersonId = 502;

        public static void Run(int? idx = null)
        {
            var diary = Load.LoadXml(DiaryXmlPath);

            var startAtChunkIdx = idx ?? STARTATCHUNKIDX;
            var startAt = GetStartIndex(startAtChunkIdx);
            var takeEntries = ChunkLength[startAtChunkIdx];
            var entries = diary.Entries.Where(entry => entry.People.Contains(KatiePersonId));
            var chunkedEntries = entries.Skip(startAt).Take(takeEntries);
            CreateDocument(chunkedEntries, startAtChunkIdx);
        }

        private static void CreateYearPage(DocX document, int year)
        {
            document.InsertSectionPageBreak();
            var yearParagraph = document.InsertParagraph(year.ToString()).FontSize(50);
            yearParagraph.Heading(HeadingType.Heading1);
            document.InsertParagraph("Image to follow");
            document.InsertSectionPageBreak();
        }

        private static void CreateDocument(in IEnumerable<Entry> entries, int idx)
        {
            var documentFromPostfix = entries.First().DateEntry.GetShortDate().Replace(" ", "-");
            var documentLastPostfix = entries.Last().DateEntry.GetShortDate().Replace(" ", "-");
            var filePath = Path.Combine(DocXDirectory, $"diary{idx} {documentFromPostfix} to {documentLastPostfix}.docx");
            var document = DocX.Create(filePath);
            var font = new Font("Calibri (Body)");
            document.SetDefaultFont(font, 11d, Color.Black);

            var counter = 0;
            var chunkedEntries = entries.ToList();
            var chunkedEntriesLength = chunkedEntries.Count;
            var previousYear = 0;
            var previousMonth = 0;

            // InsertTOC(document);

            foreach (var entry in chunkedEntries)
            {
                var entryContext = new EntryContext(document, entry);
                
                counter++;
                Console.WriteLine($"{counter} / {chunkedEntriesLength} - {entryContext.Entry.DateEntry.GetShortDate()}");

                if (entry.DateEntry.Year != previousYear)
                {
                    // Add a blank page introducing the year.
                    CreateYearPage(document, entry.DateEntry.Year);
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

        private static void InsertTOC(DocX document)
        {
            var switches = new Dictionary<TableOfContentsSwitches, string>() {
                { TableOfContentsSwitches.O, "1-3" },
                { TableOfContentsSwitches.U, "" } };

            document.InsertTableOfContents("", switches);
        }

        private static void CreateDiaryHeader(in DocX document, in Entry entry)
        {
            // Add a table in a document of 1 row and 3 columns.
            var columnWidths = new [] { 480f, 120f };
            var table = document.InsertTable(1, columnWidths.Length);
            
            // Set the table's column width and background 
            table.SetWidths(columnWidths);
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
                .Color(Color.DarkBlue);

            // Date
            var dateParagraph = row.Cells[1].Paragraphs.First();
            dateParagraph.Append(entry.DateEntry.GetLongDate())
                .CapsStyle(CapsStyle.caps)
                .FontSize(9)
                .Color(Color.DeepPink);

            dateParagraph.Alignment = Alignment.right;
        }

        private static void CreateDiaryContent(in EntryContext entryContext)
        {
            // Create a table with 1 row and 1 column - serves as a container
            entryContext.Container = TableHelper.CreateTable(entryContext, 1, 1);

            var contentHtml = entryContext.Entry.Info.OriginalContent;
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(contentHtml);
            
            var childNodes = htmlDocument.DocumentNode.ChildNodes;
            foreach (var childNode in childNodes)
            {
                NodeHandler(entryContext, childNode);
            }

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
                    var picture = PictureHelper.CreateImage(entryContext, GetChildNodeValue(htmlNode, "src"), GetChildNodeValue(htmlNode, "caption"));
                    if (picture != null)
                    {
                        entryContext.Pictures.Add(picture);
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

        static string GetChildNodeValue(in HtmlNode htmlNode, string nodeName)
        {
            var childNode = htmlNode.ChildNodes.First(node => nodeName == node.Name);
            return childNode.InnerText;
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

            TableHelper.CreateTable(entryContext, (int)rowCount, columnCount,
                (int rowIndex, int columnIndex, float columnWidth, Cell cell) =>
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
                });
        }

        static void SizePicture(Picture picture, Size pictureMaxSize)
        {
            var pictureSize = new Size((int)picture.Width, (int)picture.Height);
            var newSize = ImageExtension.CalculateNewSize(pictureMaxSize, pictureSize, (maxSize, _) => maxSize);
            picture.Width = newSize.Width;
            picture.Height = newSize.Height;
        }
    }
}