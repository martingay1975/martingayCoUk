using System.Linq;
using Xceed.Document.NET;

namespace DocXLib
{
    internal static class TableHelper
    {
        public readonly static float[] TocColumnWidths = new[] { 490f, 70f };
        private static int TotalWidth = DocumentSetup.GetLivePageWidthPoints();

        // return false to stop visiting
        public delegate bool VisitCellFunc(int rowIndex, int columnIndex, float columnWidth, Cell cell);

        internal class Options
        {
            public VisitCellFunc VisitCellFunc { get; set; }
            public float[] ColumnWidths { get; set; }
            public int ColumnCountIfNoWidths { get; set; }
        }

        /// <summary>
        /// Creates/inserts a table with no borders
        /// </summary>
        public static Table CreateTable(Section section, Paragraph paragraph, int rowCount, Options options)
        {
            var columnWidths = options.ColumnWidths 
                ?? Enumerable.Repeat(TotalWidth / (float)options.ColumnCountIfNoWidths, options.ColumnCountIfNoWidths).ToArray();

            Table table;
            if (paragraph != null)
            {
                table = paragraph.InsertTableAfterSelf(rowCount, columnWidths.Length);
            }
            else {
                table = section.InsertTable(rowCount, columnWidths.Length);
            }

            // Set the table's column width and background 

            table.SetWidths(columnWidths);

            table.AutoFit = AutoFit.Contents;
            table.Design = TableDesign.None;

            if (options?.VisitCellFunc != null)
            {
                for (var rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                {
                    var row = table.Rows[rowIndex];
                    for (var columnIndex = 0; columnIndex < row.Cells.Count; columnIndex++)
                    {
                        var cell = row.Cells[columnIndex];
                        if (!options.VisitCellFunc(rowIndex, columnIndex, columnWidths[columnIndex], cell))
                        {
                            return table;
                        }
                    }
                }
            }
            return table;
        }
    }
}
