using System.Collections.Generic;

namespace DocXLib
{
    public static class DocumentSlices
    {
        public readonly static List<DocumentSlice> DocumentList = new List<DocumentSlice>
        {
            // Start of 2003 - Book 1
            /*  0 */ new DocumentSlice(700, 1, 1),
            /*  1 */ new DocumentSlice(158),
            /*  2 */ new DocumentSlice(142),
            /*  3 */ new DocumentSlice(138),
            /*  4 */ new DocumentSlice(138),
            /*  5 */ new DocumentSlice(108),
            // Start of 2013 - Book 2
            /*  6 */ new DocumentSlice(186, 2, 659),
            /*  7 */ new DocumentSlice(145),
            /*  8 */ new DocumentSlice(131),
            /*  9 */ new DocumentSlice(140),
            /*  10 */ new DocumentSlice(148),
            /*  11 */ new DocumentSlice(140),
            /*  12 */ new DocumentSlice(158),
            /*  13 */ new DocumentSlice(165)
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