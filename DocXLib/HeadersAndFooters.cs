using DocXLib.Model.Data.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using Xceed.Document.NET;

namespace DocXLib
{
    public static class HeadersAndFooters
    {
        public static void AddSectionBits(IList<Section> documentSections, DocumentSectionManager documentSectionManager, List<Entry> chunkedEntries)
        {
            var hasDocumentTOC = chunkedEntries.First().DateEntry.Year == 2003;
            var sectionNumberStart = hasDocumentTOC ? 1 : 0;

            for (var sectionNumber = sectionNumberStart; sectionNumber < documentSections.Count; sectionNumber = sectionNumber + 1)
            {
                if (documentSectionManager.SectionInfos.TryGetValue(sectionNumber, out var sectionInfo))
                {
                    Console.WriteLine($"Section[{sectionNumber}] = {sectionInfo?.Type.ToString()} - {sectionInfo?.Year.ToString()}");
                }
                else
                {
                    Console.WriteLine($"Section[{sectionNumber}] = nothing");
                }

            }


            for (var sectionNumber = sectionNumberStart; sectionNumber < documentSections.Count; sectionNumber = sectionNumber + 2)
            {
                var section = documentSections[sectionNumber];

                var yearSection = (sectionNumber - (hasDocumentTOC ? 1 : 0)) / 2;
                var year = chunkedEntries.First().DateEntry.Year + yearSection;

                int? startPageNo = null;
                if (year == Start.pageNumberJumpYear)
                {
                    startPageNo = Start.pageNumberJumpYearPN;
                }
                AddPageFooters(section, year, startPageNo);
            }
        }
        private static void AddPageFooters(Section section, int year, int? pageStart = null)
        {
            section.AddFooters();
            section.DifferentFirstPage = true;

            if (pageStart.HasValue)
            {
                section.PageNumberStart = pageStart.Value;
            }

            var footers = section.Footers;

            // Page number to the left for even
            var pEven = footers.Even.Paragraphs[0];
            AddFooterTable(pEven, true, year.ToString());

            // Page number to the right for odd
            var pOdd = footers.Odd.Paragraphs[0];
            AddFooterTable(pOdd, false, year.ToString());
        }

        private static void AddFooterTable(Paragraph paragraph, bool isEven, string year)
        {
            var options = new TableHelper.Options
            {
                ColumnCountIfNoWidths = 3,
                VisitCellFunc = (int rowIndex, int columnIndex, float columnWidth, Cell cell) =>
                {
                    var cellParagraph = cell.Paragraphs[0];

                    switch (columnIndex)
                    {
                        case 0:
                            {
                                if (isEven)
                                {
                                    cellParagraph.FontSize(18).Bold(true).Color(Start.PageNumberColor);
                                    cellParagraph.AppendPageNumber(PageNumberFormat.normal).FontSize(25).Bold(true);
                                }
                                break;
                            }
                        case 1:
                            {
                                cellParagraph.InsertText($"Katie Gay - {year}");
                                cellParagraph.Alignment = Alignment.center;
                                break;
                            }
                        case 2:
                            {

                                if (!isEven)
                                {
                                    cellParagraph.FontSize(18).Bold(true).Color(Start.PageNumberColor);
                                    cellParagraph.AppendPageNumber(PageNumberFormat.normal);
                                    cellParagraph.Alignment = Alignment.right;
                                }

                                break;
                            }
                    }

                    cell.VerticalAlignment = VerticalAlignment.Center;
                    return true;
                }
            };

            TableHelper.CreateTable(null, paragraph, 1, options);
        }

    }
}
