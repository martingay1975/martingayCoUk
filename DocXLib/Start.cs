using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using DocXLib.Model.Data.Xml;
using HtmlAgilityPack;
using Xceed.Document.NET;
using Xceed.Words.NET;
using Font = Xceed.Document.NET.Font;

namespace DocXLib
{
    public static class Start
    {
        public const string OutputDocXDirectory = @"C:\temp\docx\";
        private const string BaseImagePath = @"L:\images";
        private const string DiaryXmlPath = @"C:\Users\Slop\AppData\Roaming\res\xml\diary.xml";
        const int KatiePersonId = 502;
        private readonly static List<BadImageItem> BadImages;
        private readonly static Size MaxSize;

        static Start()
        {
            BadImages = new List<BadImageItem>();
            MaxSize = new Size(width: 250, height: 300);
        }

        public static void Run()
        {
            var diary = Load.LoadXml(DiaryXmlPath);
            var document = CreateDocument();

            try
            {
                foreach (var entry in diary.Entries.Where(entry => entry.People.Contains(KatiePersonId)))
                {
                    // loop around each diary entry
                    CreateDiaryHeader(document, entry.DateEntry, entry.Title.Value);
                    CreateDiaryContent(document, entry.Info.OriginalContent);
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                document.Save();
                ProcessBadImages();
            }
        }

        public class BadImageItem
        {
            public BadImageItem(string year, string filename)
            {
                Year = year;
                Filename = filename;
                FoundLocations = null;
            }

            public string Year;
            public string Filename;
            public string[] FoundLocations;

            public void FindFilename()
            {
                var yearSearchPath = Path.Combine(BaseImagePath, Year);
                FoundLocations = Directory.GetFiles(yearSearchPath, Filename, SearchOption.AllDirectories);
                if (FoundLocations.Length == 0)
                {
                    FoundLocations = Directory.GetFiles(BaseImagePath, Filename, SearchOption.AllDirectories);
                }
            }
        }

        private static void ProcessBadImages()
        {
            Debug.WriteLine($"There are {BadImages.Count} bad images.");
            int count = 1;
            foreach (var badImage in BadImages)
            {
                badImage.FindFilename();
                
                var value = badImage.FoundLocations == null ? "-" : string.Join(",", badImage.FoundLocations);
                Debug.WriteLine($"{count} - {badImage.Filename} - {value}");

                if (badImage.FoundLocations != null && badImage.FoundLocations.Length == 1)
                {
                    File.Copy(badImage.FoundLocations[0], Path.Combine(BaseImagePath, badImage.Filename));
                } else
                {
                    using (WebClient client = new WebClient())
                    {
                        var url = $"http://www.martingay.co.uk/images/years/{badImage.Year}/{badImage.Filename}";
                        client.DownloadFile(new Uri(url), $@"l:\images\{badImage.Filename}");
                    }
                }
                count++;
            }
        }

        private static void CreateDiaryHeader(in DocX document, in DateEntry dateX, in string title)
        {
            // Add a table in a document of 1 row and 3 columns.
            var columnWidths = new [] { 480f, 120f };
            var table = document.InsertTable(1, columnWidths.Length);

            // Set the table's column width and background 
            table.SetWidths(columnWidths);
            table.AutoFit = AutoFit.Contents;

            var row = table.Rows.First();

            // Fill in the columns of the first row in the table.
            var titleParagraph = row.Cells[0].Paragraphs.First();
            titleParagraph.Append(title)
                .Bold()
                .FontSize(13);

            var dateParagraph = row.Cells[1].Paragraphs.First();
            dateParagraph.Append(dateX.GetLongDate())
                .FontSize(9);

            dateParagraph.Alignment = Alignment.right;
        }

        private static void CreateDiaryContent(in DocX document, in string contentHtml)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(contentHtml);

            var childNodes = htmlDocument.DocumentNode.ChildNodes;
            foreach (var childNode in childNodes)
            {
                NodeHandler(document, childNode);
            }

            document.InsertParagraph("")
                .SpacingAfter(20);
        }

        private static void CreateImage(in DocX document, Paragraph paragraph, string imagePath, string caption)
        {
            // go find the image from the _BestOf directory. Full Res image.

            var sourceImagePath = GetFullImagePath(imagePath);
            //var streamImage = document.AddImage(new FileStream(sourceImagePath, FileMode.Open, FileAccess.Read));
            //var pictureStream = streamImage.CreatePicture();

            //var newSize = ImageExtension.CalculateNewSize(MaxSize, new Size((int)pictureStream.Width, (int)pictureStream.Height));

            //pictureStream.Width = newSize.Width;
            //pictureStream.Height = newSize.Height;
            //paragraph.AppendPicture(pictureStream);
            //paragraph.InsertCaptionAfterSelf(caption);
            //paragraph.SpacingAfter(20);
        }

        private static string GetFullImagePath(string imagePath)
        {
            // e.g file path is images\years\2020\2020_09_12-08-Fars50thWeddingAnniversary.jpeg
            // need to turn into L:\images\2020\_BestOf\2020_09_12-08-Fars50thWeddingAnniversary.jpeg

            var pathParts = imagePath.Split(new char[] {'\\', '/'});

            var year = pathParts[pathParts.Length - 2];
            var filename = pathParts[pathParts.Length - 1];
            var convertedPath = Path.Combine(BaseImagePath, year, "_BestOf", filename);

            if (!File.Exists(convertedPath))
            {
                BadImages.Add(new BadImageItem(year, filename));
                Debug.WriteLine($"'{convertedPath, 50}'. Original '{imagePath}'");
                //throw new Exception($"Cannot find file '{convertedPath}'. Original '{imagePath}'");
            }

            return convertedPath;
        }

        private static string GetText(HtmlNode htmlNode)
        {
            // need to unescape html eg. &gt; to >
            return WebUtility.HtmlDecode(htmlNode.InnerText);
        }

        private static void NodeHandler(in DocX document, HtmlNode htmlNode)
        {
            switch (htmlNode.Name)
            {
                case "p":
                {
                    var text = GetText(htmlNode);
                    document.InsertParagraph(text);
                    foreach (var paragraphChildNode in htmlNode.ChildNodes)
                    {
                        NodeHandler(document, paragraphChildNode);
                    }

                    break;
                }
                case "ul":
                {
                    var list = document.AddList();
                    foreach (var listItemNode in htmlNode.ChildNodes)
                    {
                        document.AddListItem(list, GetText(listItemNode), 0, ListItemType.Numbered);
                    }

                    document.InsertList(list);
                    break;
                }
                case "image":
                {
                    var paragraph = document.InsertParagraph();
                    CreateImage(document, paragraph, GetChildNodeValue(htmlNode, "src"), GetChildNodeValue(htmlNode, "caption"));
                    break;
                }
                case "#text":
                {
                    // do nothing - already handled.
                    break;
                }
            }
        }

        static string GetChildNodeValue(in HtmlNode htmlNode, string nodeName)
        {
            var childNode = htmlNode.ChildNodes.First(node => nodeName == node.Name);
            return childNode.InnerText;
        }

        static DocX CreateDocument()
        {
            var filePath = Path.Combine(OutputDocXDirectory, "diary.docx");
            var document = DocX.Create(filePath);
            document.SetDefaultFont(new Font("Calibri (Body)"), 11d, Color.Black);

            return document;
        }
    }
}
