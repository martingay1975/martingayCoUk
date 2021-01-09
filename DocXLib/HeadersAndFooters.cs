using System;
using System.Collections.Generic;
using Xceed.Document.NET;

namespace DocXLib
{
    public static class HeadersAndFooters
    {
        public static void AddSectionBits(IList<Section> documentSections, DocumentSectionManager documentSectionManager, int? newDocumentPageStart)
        {
            for (var sectionNumber = 0; sectionNumber < documentSections.Count; sectionNumber ++)
            {
                if (documentSectionManager.SectionInfos.TryGetValue(sectionNumber, out var sectionInfo))
                {
                    Console.WriteLine($"Section[{sectionNumber}] = {sectionInfo?.Type.ToString()} - {sectionInfo?.Year.ToString()}");

                    switch (sectionInfo.Type)
                    {
                        case SectionInfo.SectionInfoType.ChapterEntries:
                        case SectionInfo.SectionInfoType.ChapterTOC:
                        {
                            AddPageFooters(documentSections[sectionNumber], sectionInfo, newDocumentPageStart);
                            break;
                        }
                    }

                    
                }
            }
        }
        private static void AddPageFooters(Section section, SectionInfo sectionInfo, int? pageStart = null)
        {
            section.AddFooters();
            section.DifferentFirstPage = false;

            if (pageStart.HasValue && sectionInfo.Type == SectionInfo.SectionInfoType.ChapterTOC)
            {
                section.PageNumberStart = pageStart.Value;
            }

            var footers = section.Footers;
            var year = sectionInfo.Year.ToString();

            // Page number to the left for even
            var pEven = footers.Even.Paragraphs[0];
            AddFooterTable(pEven, true, year);

            // Page number to the right for odd
            var pOdd = footers.Odd.Paragraphs[0];
            AddFooterTable(pOdd, false, year);
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

            TableHelper.CreateTable(null, paragraph, 1, Start.pageSetup, options);
        }
    }
}
