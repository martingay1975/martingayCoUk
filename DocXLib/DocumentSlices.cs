using System.Collections.Generic;

namespace DocXLib
{
    public static class DocumentSlices
    {
        public readonly static List<DocumentSlice> DocumentList = new List<DocumentSlice>
        {
            // Start of 2003 - Book 1 (565 page no - Reports 654 pages - 173,652 words)
            /*  0 */ new DocumentSlice(700, 1, 1),
            /*  1 */ new DocumentSlice(155),
            /*  2 */ new DocumentSlice(136),
            // Start of 2010 - Book 2 - 1127 (536 pages)
            /*  3 */ new DocumentSlice(135, 2, 572),
            /*  4 */ new DocumentSlice(135),
            /*  5 */ new DocumentSlice(200),
            /*  6 */ new DocumentSlice(132),
            /*  7 */ new DocumentSlice(125),
            // Start of 2018 - Book 3 - 1988 (575 pages - Reports 580 pages - 125,352 words)
            /*  8 */ new DocumentSlice(145, 3, 1106),
            /*  9 */ new DocumentSlice(147),
            /*  10 */ new DocumentSlice(138),
            /*  11 */ new DocumentSlice(156),
            /*  12 */ new DocumentSlice(165)
        };

        public static bool TryGetStartOfDocumentSlice(int documentSliceIndex, out DocumentSlice documentSlice)
        {
            if (DocumentList[documentSliceIndex].BookNumber != null)
            {
                documentSlice = DocumentList[documentSliceIndex];
                return true;
            }

            documentSlice = null;
            return false;
        }

        public static bool IsLastSliceInBook(int documentSliceIndex)
        {
            return documentSliceIndex + 1 == DocumentList.Count || TryGetStartOfDocumentSlice(documentSliceIndex + 1, out _);
        }

        public static int GetStartIndex(int documentSliceIndex)
        {
            int startIndex = 0;
            for (var i = 0; i < documentSliceIndex; i++)
            {
                startIndex += DocumentList[i].DiaryEntriesCount;
            }

            return startIndex;
        }
    }
}