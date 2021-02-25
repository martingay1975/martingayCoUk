using System;
using System.Collections.Generic;
using System.Linq;
using Xceed.Document.NET;

namespace DocXLib
{
    public class DocumentSectionManager
    {
        private readonly Document document;
        //private readonly PageSetup pageSetup;

        public Dictionary<int, SectionInfo> SectionInfos { get; }

        public DocumentSectionManager(Document document)
        {
            this.document = document ?? throw new ArgumentNullException(nameof(document));
            this.SectionInfos = new Dictionary<int, SectionInfo>();
        }

        public bool HaveAddedSections()
        {
            return this.SectionInfos.ContainsKey(0);
        }

        public Section AddSection(SectionInfo sectionInfo)
        {
            if (HaveAddedSections())
            {
                document.InsertSectionPageBreak();
            }

            var section = document.Sections.Last();
            section.PageLayout.Orientation = Orientation.Portrait;
            var sectionIdx = document.Sections.Count - 1;
            this.SectionInfos[sectionIdx] = sectionInfo;
            return section;
        }
    }

}