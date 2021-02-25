using System;
using System.Collections.Generic;
using Xceed.Document.NET;

namespace DocXLib
{
    public static class HeadersAndFooters
    {
        public static void AddSectionBits(IList<Section> documentSections, DocumentSectionManager documentSectionManager, int? newDocumentPageStart)
        {
            bool setStartPage = newDocumentPageStart.HasValue;

            for (var sectionNumber = 0; sectionNumber < documentSections.Count; sectionNumber ++)
            {
                if (documentSectionManager.SectionInfos.TryGetValue(sectionNumber, out var sectionInfo))
                {
                    Console.WriteLine($"Section[{sectionNumber}] = {sectionInfo?.Type.ToString()} - {sectionInfo?.Year.ToString()}");
                    var section = documentSections[sectionNumber];

                    switch (sectionInfo.Type)
                    {
                        case SectionInfo.SectionInfoType.ChapterImage:
                        case SectionInfo.SectionInfoType.DocumentCover:
                        {
                            DocumentSetup.ApplyZeroMargins(section);
                            break;
                        }
                        case SectionInfo.SectionInfoType.ChapterEntries:
                        case SectionInfo.SectionInfoType.ChapterTOC:
                        {
                            DocumentSetup.ApplyStandardMargins(section);
                            AddPageFooters(section, sectionInfo, setStartPage, newDocumentPageStart);
                            setStartPage = false;
                            break;
                        }
                        case SectionInfo.SectionInfoType.DocumentTOC:
                        {
                            DocumentSetup.ApplyStandardMargins(section);
                            break;
                        }
                    }
                }
            }
        }

        private static void AddPageFooters(Section section, SectionInfo sectionInfo, bool setStartPage, int? pageStart = null)
        {
            section.AddFooters();
            section.DifferentFirstPage = false;

            if (setStartPage && sectionInfo.Type == SectionInfo.SectionInfoType.ChapterTOC)
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
                                    cellParagraph.FontSize(18).Color(Start.PageNumberColor);
                                    cellParagraph.AppendPageNumber(PageNumberFormat.normal);
                                }
                                break;
                            }
                        case 1:
                            {
                                cellParagraph.InsertText($"Katie Gay - {year}");
                                cellParagraph.Color(Start.PageNumberColor);
                                cellParagraph.Alignment = Alignment.center;
                                break;
                            }
                        case 2:
                            {
                                if (!isEven)
                                {
                                    cellParagraph.FontSize(18).Color(Start.PageNumberColor);
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
