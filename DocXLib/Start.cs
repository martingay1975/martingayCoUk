﻿using System;
using System.Collections.Generic;
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
        
        private const string DiaryXmlPath = @"C:\Users\Slop\AppData\Roaming\res\xml\diary.xml";
        private const int chunkingLength = 20;
        private const int KatiePersonId = 502;

        public static void Run()
        {
            var diary = Load.LoadXml(DiaryXmlPath);
            var entries = diary.Entries.Where(entry => entry.People.Contains(KatiePersonId));
            var chunkingStart = 0;

            while (true)
            {
                var chunkedEntries = entries.Skip(chunkingStart).Take(chunkingLength);
                if (!Create1Document(chunkedEntries))
                {
                    return;
                }
                chunkingStart += chunkingLength;
                return;
            }
        }

        private static bool Create1Document(in IEnumerable<Entry> entries)
        {
            var document = CreateDocument(entries.First().DateEntry.GetShortDate());
            var documentContext = new DocumentContext(document);

            var counter = 0;
            var chunkedEntries = entries.ToList();
            var chunkedEntriesLength = chunkedEntries.Count;
            foreach (var entry in chunkedEntries)
            {
                counter++;
                Console.WriteLine($"{counter} / {chunkedEntriesLength}");

                documentContext.SetNewEntry(entry);
                CreateDiaryHeader(documentContext);
                CreateDiaryContent(documentContext);

                AddPictures(documentContext);
                documentContext.SetNewParagraph("")
                    .SpacingAfter(40);
            }

            document.Save();
            return chunkedEntriesLength == chunkingLength;
        }

        private static void AddPictures(DocumentContext documentContext)
        {
            var picCount = documentContext.Pictures.Count;
            if (picCount == 0)
            {
                return; // no pictures so need to continue
            }

            var isThreeColumn = picCount % 3 == 0 || picCount > 4;
            var columnWidths = isThreeColumn ? new[] { 250f, 250f, 250f } : new[] { 350f, 350f };
            var columnCount = columnWidths.Length;

            var rowCount = Math.Ceiling(picCount / (float)columnCount);

            var table = documentContext.Document.InsertTable((int)rowCount, columnCount);

            // Set the table's column width and background 
            table.SetWidths(columnWidths);
            table.AutoFit = AutoFit.Contents;
            table.Design = TableDesign.None;

            var pictureEnumerator = documentContext.Pictures.GetEnumerator();

            foreach (var row in table.Rows)
            {
                foreach (var cell in row.Cells)
                {
                    if (!pictureEnumerator.MoveNext())
                    {
                        // no more pictures to display
                        return;
                    }

                    var picture = pictureEnumerator.Current;
                    var paragraph = cell.Paragraphs.First();
                    paragraph.AppendPicture(picture);
                    paragraph.InsertCaptionAfterSelf(picture.Name);
                }
            }
        }

        private static void CreateDiaryHeader(in DocumentContext documentContext)
        {
            // Add a table in a document of 1 row and 3 columns.
            var columnWidths = new [] { 480f, 120f };
            var table = documentContext.Document.InsertTable(1, columnWidths.Length);

            // Set the table's column width and background 
            table.SetWidths(columnWidths);
            table.AutoFit = AutoFit.Contents;
            table.Design = TableDesign.None;

            var row = table.Rows.First();

            // Fill in the columns of the first row in the table.
            // Title
            var titleParagraph = row.Cells[0].Paragraphs.First();
            titleParagraph.Append(documentContext.Entry.Title.Value)
                .CapsStyle(CapsStyle.caps)
                .FontSize(12)
                .Spacing(5)
                .Color(Color.DarkBlue);

            // Date
            var dateParagraph = row.Cells[1].Paragraphs.First();
            dateParagraph.Append(documentContext.Entry.DateEntry.GetLongDate())
                .CapsStyle(CapsStyle.caps)
                .FontSize(9)
                .Color(Color.DeepPink);

            dateParagraph.Alignment = Alignment.right;
        }

        private static void CreateDiaryContent(in DocumentContext documentContext)
        {
            var contentHtml = documentContext.Entry.Info.OriginalContent;
            
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(contentHtml);

            var childNodes = htmlDocument.DocumentNode.ChildNodes;

            foreach (var childNode in childNodes)
            {
                NodeHandler(documentContext, childNode);
            }
        }

        private static string GetText(HtmlNode htmlNode)
        {
            // need to unescape html eg. &gt; to >
            return WebUtility.HtmlDecode(htmlNode.InnerText);
        }

        private static void NodeHandler(in DocumentContext documentContext, HtmlNode htmlNode)
        {
            switch (htmlNode.Name)
            {
                case "p":
                {
                    var text = GetText(htmlNode);
                        var paragraph = documentContext.SetNewParagraph(text);
                        paragraph.Alignment = Alignment.left;
                        paragraph.KeepLinesTogether();

                    foreach (var paragraphChildNode in htmlNode.ChildNodes)
                    {
                        NodeHandler(documentContext, paragraphChildNode);
                    }

                    break;
                }
                case "ul":
                {
                    var list = documentContext.Document.AddList();
                    foreach (var listItemNode in htmlNode.ChildNodes)
                    {
                            documentContext.Document.AddListItem(list, GetText(listItemNode), 0, ListItemType.Numbered);
                    }

                    documentContext.Document.InsertList(list);
                    break;
                }
                case "image":
                {
                    var picture = PictureHelper.CreateImage(documentContext, GetChildNodeValue(htmlNode, "src"), GetChildNodeValue(htmlNode, "caption"));
                    documentContext.Pictures.Add(picture);
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

        static DocX CreateDocument(string documentPostfix)
        {
            var filePath = Path.Combine(OutputDocXDirectory, $"diary{documentPostfix}.docx");
            var document = DocX.Create(filePath);
            document.SetDefaultFont(new Font("Calibri (Body)"), 11d, Color.Black);

            return document;
        }
    }
}