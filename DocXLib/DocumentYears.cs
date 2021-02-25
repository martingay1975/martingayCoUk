using System.Collections.Generic;
using Xceed.Document.NET;
using System;
using System.Drawing;

namespace DocXLib
{
    public class Years
    {
        public int Year { get; set; }
        public int Page { get; set; }
        public int Book { get; set; }
        public int EntryNo { get; set; }
    }

    public static class DocumentYears
    {
        public readonly static float[] DocumentTocColumnWidths = new[] { 300f, 90f, 70f };

        public static List<Years> YearsToPages { get; private set; }

        static DocumentYears()
        {
            YearsToPages = new List<Years>
            {
                new Years { Year = 2003, Book = 1, Page = 1, EntryNo = 1},
                new Years { Year = 2004, Book = 1, Page = 36, EntryNo = 86},
                new Years { Year = 2005, Book = 1, Page = 121, EntryNo = 247},
                new Years { Year = 2006, Book = 1, Page = 207, EntryNo = 400},
                new Years { Year = 2007, Book = 1, Page = 309, EntryNo = 566},
                new Years { Year = 2008, Book = 1, Page = 405, EntryNo = 728},
                new Years { Year = 2009, Book = 1, Page = 486, EntryNo = 851},
                new Years { Year = 2010, Book = 1, Page = 572, EntryNo = 992},
                new Years { Year = 2011, Book = 2, Page = 663, EntryNo = 1126},
                new Years { Year = 2012, Book = 2, Page = 766, EntryNo = 1261 },
                new Years { Year = 2013, Book = 2, Page = 849, EntryNo = 1368},
                new Years { Year = 2014, Book = 2, Page = 913, EntryNo = 1461},
                new Years { Year = 2015, Book = 2, Page = 1001, EntryNo = 1593},
                new Years { Year = 2016, Book = 3, Page = 1106, EntryNo = 1718},
                new Years { Year = 2017, Book = 3, Page = 1216, EntryNo = 1858},
                new Years { Year = 2018, Book = 3, Page = 1327, EntryNo = 1987},
                new Years { Year = 2019, Book = 3, Page = 1434, EntryNo = 2116},
                new Years { Year = 2020, Book = 3, Page = 1569, EntryNo = 2285},
                new Years { Year = 2021, Book = 3, Page = 1682, EntryNo = 2427},
            };
        }

        public static void InsertDocumentTOC(DocumentSectionManager documentSectionManager)
        {
            // TOC for whole document
            var options = new TableHelper.Options
            {
                ColumnWidths = DocumentTocColumnWidths,
                VisitCellFunc = (int rowIndex, int columnIndex, float columnWidth, Cell cell) =>
                {
                    var cellParagraph = cell.Paragraphs[0];
                    if (rowIndex == YearsToPages.Count)
                    {
                        switch (columnIndex)
                        {
                            case 0:
                                {
                                    cellParagraph.Append("Family Tree");
                                    cellParagraph.FontSize(14);
                                    break;
                                }
                            case 1:
                                {
                                    cellParagraph.Append($"Book 3").Color(Start.PurpleColor);
                                    cellParagraph.Alignment = Alignment.right;
                                    break;
                                }
                            case 2:
                                {
                                    cellParagraph.Append("1681").Color(Start.PurpleColor);
                                    cellParagraph.Alignment = Alignment.right;
                                    break;
                                }
                        }
                        return true;
                    }

                    var years = YearsToPages[rowIndex];
                    FormatCell(years.Book, cell, cellParagraph);
                    
                    switch (columnIndex)
                    {
                        case 0:
                            {
                                cellParagraph.Append(years.Year.ToString());
                                cellParagraph.FontSize(14);
                                break;
                            }

                        case 1:
                            {
                                cellParagraph.Append($"Book {years.Book}");
                                cellParagraph.Alignment = Alignment.right;
                                break;
                            }
                        case 2:
                            {
                                cellParagraph.Append(years.Page.ToString());
                                cellParagraph.Alignment = Alignment.right;
                                break;
                            }
                    }

                    cell.VerticalAlignment = VerticalAlignment.Center;
                    return true;
                }
            };

            var documentTocSection = documentSectionManager.AddSection(new SectionInfo() { Type = SectionInfo.SectionInfoType.DocumentTOC });
            var paragraph1 = documentTocSection.InsertParagraph();
            paragraph1.AppendLine("Hello... ")
                .FontSize(14)
                .Bold(true);

            var paragraph2 = documentTocSection.InsertParagraph();
            documentTocSection.InsertParagraph("The childhood story of the beautiful Katie Gay is so large, it had to be spread over three books. It is a copy of the blogsite - martingay.co.uk - in all its glory.");
            documentTocSection.InsertParagraph("");

            //documentTocSection.InsertParagraph("The childhood life of 'Katie Clara Uhrskov Gay'. The most beautiful person inside and out you will ever meet.");
            //documentTocSection.InsertParagraph("The book contains the mundane and exciting activities that the family have been involved in whilst Katie has been growing up. There are three books covering her life from 0 to 18 years. Have fun looking back.");

            TableHelper.CreateTable(documentTocSection, null, YearsToPages.Count + 1, options);
        }

        private static void FormatCell(int bookNumber, Cell cell, Paragraph paragraph)
        {
            switch (bookNumber)
            {
                case 1:
                    cell.FillColor = Start.RedColor;
                    paragraph.Color(Color.White);
                    break;
                case 2:
                    cell.FillColor = Start.YellowColor;
                    paragraph.Color(Color.Black);
                    break;
                case 3:
                    cell.FillColor = Start.GreenColor;
                    paragraph.Color(Color.Black);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}