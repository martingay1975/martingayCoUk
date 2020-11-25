using System.Linq;
using Xceed.Document.NET;

namespace DocXLib
{
    internal static class TableHelper
    {
        private static int TotalWidth = 450;

        // return false to stop visiting
        public delegate bool VisitCellFunc(int rowIndex, int columnIndex, float columnWidth, Cell cell);

        /// <summary>
        /// Creates/inserts a table with no borders
        /// </summary>
        public static Table CreateTable(EntryContext entryContext, int rowCount, int columnCount, VisitCellFunc visitCellFunc = null)
        {
            var columnWidths = Enumerable.Repeat((float)TotalWidth / (float)columnCount, columnCount).ToArray();

            // small gap before the table
            var paragraph = entryContext.Document.InsertParagraph("");
            paragraph.Spacing(7);

            var table = entryContext.Document.InsertTable(rowCount, columnCount);

            // Set the table's column width and background 
            table.SetWidths(columnWidths);
            table.AutoFit = AutoFit.Contents;
            table.Design = TableDesign.None;

            if (visitCellFunc != null)
            {
                for (var rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                {
                    var row = table.Rows[rowIndex];
                    for (var columnIndex = 0; columnIndex < row.Cells.Count; columnIndex++)
                    {
                        var cell = row.Cells[columnIndex];
                        if (!visitCellFunc(rowIndex, columnIndex, columnWidths[columnIndex], cell))
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
