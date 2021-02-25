//using Xceed.Words.NET;

namespace DocXLib
{
    public class DocumentSlice
    {
        public DocumentSlice(int diaryEntriesCount, int? bookNumber = null, int? startPageNumber = null)
        {
            DiaryEntriesCount = diaryEntriesCount;
            BookNumber = bookNumber;
            StartPageNumber = startPageNumber;
        }

        public int DiaryEntriesCount { get; }
        public int? BookNumber { get; }
        public int? StartPageNumber { get; }
    }
}