using HtmlAgilityPack;
using System.Linq;
using System.Net;

namespace DocXLib
{
    static class HtmlHelper
    {
        public delegate void NodeHandlerDelegate(in EntryContext entryContext, HtmlNode htmlNode);

        public static HtmlDocument ReadOriginalInfoContent(EntryContext entryContext, NodeHandlerDelegate nodeHandlerDelegate)
        {
            var contentHtml = entryContext.Entry.Info.OriginalContent;
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(contentHtml);
            var childNodes = htmlDocument.DocumentNode.ChildNodes;
            foreach (var childNode in childNodes)
            {
                nodeHandlerDelegate(entryContext, childNode);
            }

            return htmlDocument;
        }

        public static string GetChildNodeValue(in HtmlNode htmlNode, string nodeName)
        {
            var childNode = htmlNode.ChildNodes.First(node => nodeName == node.Name);
            return childNode.InnerText;
        }

        public static string GetText(HtmlNode htmlNode)
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


    }
}
