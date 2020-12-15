using DocXLib.Model.Data.Xml;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Words.NET;
using Xceed.Document.NET;
using System.Collections.Concurrent;
using System.Threading;

namespace DocXLib
{

    public class CompareImagesData
    {
        public string Src { get; set; }
        public string LocalFile { get; set; }
        public string HostFile { get; set; }
        public int Score { get; set; }
        public FileExist Exists { get; set; }

        public bool IsBad()
        {
            return Exists != FileExist.OK || Score != 0;
        }

        public override string ToString()
        {
            var ret = $"{Exists},{Score,3},{Src,30}";
            if (IsBad())
            {
                ret += $",{LocalFile,50},{ HostFile,50}";
            }

            return ret;
        }
    }

    public enum FileExist
    {
        OK,
        NotOnClient,
        NotOnHost
    }

    public class CompareImages
    {
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

        public static void Run(IEnumerable<Entry> chunkedEntries, string docXDirectory, DocX document)
        {
            var chunkedEntriesLength = chunkedEntries.ToList().Count;
            int counter = 0;
            var compareImages = new CompareImages();
            Parallel.ForEach(chunkedEntries, entry =>
            {
                var res = Interlocked.Increment(ref counter);
                var entryContext = new EntryContext(document, entry);
                Console.WriteLine($"{res} / {chunkedEntriesLength} - {entryContext.Entry.DateEntry.GetShortDate()}");
                HtmlHelper.ReadOriginalInfoContent(entryContext, compareImages.NodeHandler);
            });

            var badReport = compareImages.Report();
            Console.WriteLine(badReport);
            File.WriteAllText(Path.Combine(docXDirectory, "BadReport.txt"), badReport);
        }
    }
}
