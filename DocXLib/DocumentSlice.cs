namespace DocXLib
{
    public class DocumentSlice
    {
        public DocumentSlice(int diaryEntriesCount, bool lastInDocument, DocumentProperties documentProperties= null)
        {
            DiaryEntriesCount = diaryEntriesCount;
            this.LastInDocument = lastInDocument;
            DocumentProperties = documentProperties;
        }

        public bool FirstInDocument => this.DocumentProperties != null; 
        public int DiaryEntriesCount { get; }
        public bool LastInDocument { get; }
        public DocumentProperties DocumentProperties { get; }
    }
}