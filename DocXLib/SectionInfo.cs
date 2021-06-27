namespace DocXLib
{
    public class SectionInfo
    {
        public int Year { get; set; }

        public SectionInfoType Type { get; set; }

        public enum SectionInfoType
        {
            Eof,
            DocumentCover,
            DocumentTOC,
            ChapterImage,
            ChapterTOC,
            ChapterEntries,
            FamilyTree
        }
    }

}
