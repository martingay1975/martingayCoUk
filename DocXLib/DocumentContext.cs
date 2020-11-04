using DocXLib.Model.Data.Xml;
using System.Collections.Generic;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace DocXLib
{
    public class DocumentContext
    {
        public DocX Document { get; }
        public Paragraph Paragraph { get; private set; }
        public Entry Entry { get; private set; }
        public List<Picture> Pictures { get; private set; }

        public DocumentContext(DocX document)
        {
            Document = document;
        }

        public void SetNewEntry(Entry entry)
        {
            this.Entry = entry;
            Pictures = new List<Picture>();
        }

        public Paragraph SetNewParagraph(string value)
        {
            this.Paragraph = this.Document.InsertParagraph(value);
            return this.Paragraph;
        }
    }
}