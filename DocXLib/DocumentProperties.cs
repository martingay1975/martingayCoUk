//using Xceed.Words.NET;

using System;

namespace DocXLib
{
    public class DocumentProperties
    {
        public DocumentProperties(Tuple<int, int> yearRange, int? startDocumentPage = null)
        {
            YearRange = yearRange;
            this.StartDocumentPage = startDocumentPage;
        }

        public Tuple<int, int> YearRange { get; }
        public int? StartDocumentPage { get; }
    }
}