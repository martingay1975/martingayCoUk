//using Xceed.Words.NET;

namespace DocXLib
{
    public class DocumentSlice
    {
        public DocumentSlice(int diaryEntriesCount)
        {
            DiaryEntriesCount = diaryEntriesCount;
        }

        public int DiaryEntriesCount { get; }
    }
}