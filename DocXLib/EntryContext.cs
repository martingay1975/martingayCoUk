using DocXLib.Model.Data.Xml;
using System.Collections.Generic;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace DocXLib
{
    public class EntryContext
    {
        public DocX Document { get; }
        public Paragraph Paragraph => this.Container.Paragraphs[0];
        public Entry Entry { get; private set; }
        public List<Picture> Pictures { get; private set; }
        public Table Container { get; set; }
        private bool firstContents;

        public EntryContext(DocX document, Entry entry)
        {
            Document = document;
            Entry = entry;
            Pictures = new List<Picture>();
            firstContents = true;
        }

        public void SetContentParagraph(string value, bool noExtraLFCR = false)
        {
            if (this.firstContents || noExtraLFCR)
            {
                this.Paragraph.InsertTabStopPosition(Alignment.left, 10);
                this.Paragraph.Append(value);
                this.firstContents = false;
            }
            else
            {
                this.Paragraph.AppendLine(value);
            }

            this.Paragraph.AppendLine();
        }
    }
}