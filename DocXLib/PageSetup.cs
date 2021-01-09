namespace DocXLib
{
    public class PageSetup
    {
        public PageSetup(int pageColumns, bool isPortrait)
        {
            this.pageColumns = pageColumns;
            this.pageWidth = IsPortrait ? PortraitWidth : LandscapeWidth;
            this.isPortrait = isPortrait;
        }

        //private readonly static float[] EntryHeadingColumnWidths = new[] { 200f, 50f };
        //private readonly static float[] TocColumnWidths = new[] { 450f, 50f };
        private readonly int pageColumns;

        private const int LandscapeWidth = 700;
        private const int PortraitWidth = 500;
        private int pageWidth;
        private bool isPortrait;

        public bool IsPortrait => isPortrait;

        public int PageColumnWidth => pageWidth / pageColumns;

        public int GetTableColumnCountForPictures(int picCount)
        {
            // 500 up to 3 columns max
            // <300 one column max
            // >300 & < 500 2 column max

            if (PageColumnWidth > 300 && PageColumnWidth < 500)
            {
                return picCount >= 2 ? 2 : 1;
            }
            if (PageColumnWidth >= 500)
            {
                return picCount >= 3 ? 3 : 2;
            }

            return 1;
        }

        public float[] GetEntryHeadingColumnWidths()
        {
            return new float[] { (float)0.8 * PageColumnWidth, (float)0.2 * PageColumnWidth };
        }

        public float[] GetTocColumnWidths()
        {
            return new float[] { (float)0.8 * pageWidth, (float)0.2 * pageWidth };
        }
    }
}
