﻿using DocXLib.Model.Data.Xml;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Words.NET;
using System.Collections.Concurrent;
using System.Threading;

namespace DocXLib.Image
{
    public class CompareImages
    {
        public static void Run(IEnumerable<Entry> chunkedEntries, string docXDirectory, DocX document)
        {
            var chunkedEntriesLength = chunkedEntries.ToList().Count;
            int counter = 0;
            var compareImages = new CompareImages();
            var start = DateTime.UtcNow;
            Console.WriteLine($"Start: {start}");
            Parallel.ForEach(chunkedEntries, new ParallelOptions { MaxDegreeOfParallelism = 3 }, entry =>
            {
                var res = Interlocked.Increment(ref counter);
                var entryContext = new EntryContext(document, entry);
                var message = $"{res} / {chunkedEntriesLength} - {entryContext.Entry.DateEntry.GetShortDate()}";
                Console.WriteLine(message);

                try
                {
                    HtmlHelper.ReadOriginalInfoContent(entryContext, compareImages.NodeHandler);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ERROR: '{e.Message}'. {message}");
                }
            });

            var end = DateTime.UtcNow;
            var badReport = compareImages.Report();
            Console.WriteLine(badReport);
            File.WriteAllText(Path.Combine(docXDirectory, "BadReport.txt"), badReport);
            Console.WriteLine($"End: {end}. {(end - start).TotalMinutes}mins taken");
        }

        private ConcurrentBag<CompareImagesData> CompareImagesDatas { get; }

        public CompareImages()
        {
            CompareImagesDatas = new ConcurrentBag<CompareImagesData>();
        }

        public void NodeHandler(in EntryContext entryContext, HtmlNode htmlNode)
        {
            switch (htmlNode.Name)
            {
                case "image":
                    {
                        var src = HtmlHelper.GetChildNodeValue(htmlNode, "src");
                        var compareImagesData = PictureHelper.CompareWithHostVersion(src);
                        CompareImagesDatas.Add(compareImagesData);
                        if (compareImagesData.IsBad())
                        {
                            Console.WriteLine("  " + compareImagesData.ToString());
                        }
                        break;
                    }
                default:
                    {
                        foreach (var childNode in htmlNode.ChildNodes)
                        {
                            NodeHandler(entryContext, childNode);
                        }
                        break;
                    }
            }
        }

        public string Report()
        {
            var stringBuilder = new StringBuilder();
            CompareImagesDatas.OrderBy(cid => cid.Exists).ThenByDescending(cid => cid.Score);
            foreach (var compareImagesData in CompareImagesDatas)
            {
                if (compareImagesData.IsBad())
                {
                    stringBuilder.AppendLine(compareImagesData.ToString());
                }
            }

            var badCount = CompareImagesDatas.Where(cid => cid.IsBad()).Count();
            stringBuilder.AppendLine($"Bad: {badCount} / {CompareImagesDatas.Count}");

            return stringBuilder.ToString();
        }
    }
}
