using Xceed.Document.NET;
using System;
using System.Linq;

namespace DocXLib
{
    public static class DocumentYears
    {
        public static readonly float[] DocumentTocColumnWidths = new[] { 100f, 140f, 120f, 120f };

        //public static List<Years> YearsToPages { get; private set; }

        //static DocumentYears()
        //{
        //    YearsToPages = new List<Years>
        //    {
        //        new Years { Year = 2003, Book = 1, Page = 1, EntryNo = 1},
        //        new Years { Year = 2004, Book = 1, Page = 37, EntryNo = 86},
        //        new Years { Year = 2005, Book = 1, Page = 122, EntryNo = 247},
        //        new Years { Year = 2006, Book = 1, Page = 204, EntryNo = 401},
        //        new Years { Year = 2007, Book = 1, Page = 311, EntryNo = 569},
        //        new Years { Year = 2008, Book = 1, Page = 409, EntryNo = 731},
        //        new Years { Year = 2009, Book = 1, Page = 491, EntryNo = 855},
        //        new Years { Year = 2010, Book = 2, Page = 569, EntryNo = 1000},
        //        new Years { Year = 2011, Book = 2, Page = 663, EntryNo = 1135},
        //        new Years { Year = 2012, Book = 2, Page = 758, EntryNo = 1261 },
        //        new Years { Year = 2013, Book = 2, Page = 833, EntryNo = 1383},
        //        new Years { Year = 2014, Book = 2, Page = 879, EntryNo = 1461},
        //        new Years { Year = 2015, Book = 2, Page = 962, EntryNo = 1617},
        //        new Years { Year = 2016, Book = 3, Page = 1037, EntryNo = 1718},
        //        new Years { Year = 2017, Book = 3, Page = 1206, EntryNo = 1858},
        //        new Years { Year = 2018, Book = 3, Page = 1327, EntryNo = 1987},
        //        new Years { Year = 2019, Book = 3, Page = 1434, EntryNo = 2116},
        //        new Years { Year = 2020, Book = 3, Page = 1569, EntryNo = 2285},
        //        new Years { Year = 2021, Book = 3, Page = 1682, EntryNo = 2427},
        //    };
        //}

        public static void InsertDocumentTOC(DocumentSectionManager documentSectionManager, int totalEntryCount)
        {
            // TOC for whole document
            var options = new TableHelper.Options
            {
                ColumnWidths = DocumentTocColumnWidths,
                VisitCellFunc = (int rowIndex, int columnIndex, float columnWidth, Cell cell) =>
                {
                    var cellParagraph = cell.Paragraphs[0];

                    // if we are the last year in the contents - then add an extra bit for the family tree

                    var documentSlice = DocumentSlices.DocumentList.First(docSlice => docSlice.BookNumber == rowIndex + 1);
                    var startEntryIndex = DocumentSlices.GetStartIndex(documentSlice) + 1;
                    var endEntryIndex = -1;

                    var nextSlice = DocumentSlices.DocumentList.FirstOrDefault(docSlice => docSlice.BookNumber == rowIndex + 2);
                    if (nextSlice == null)
                    {
                        // we're on book 2
                        endEntryIndex = totalEntryCount;
                    } 
                    else
                    {
                        endEntryIndex = DocumentSlices.GetStartIndex(nextSlice) + 1;
                    }
                    var bookEntryCount = endEntryIndex - startEntryIndex;

                    switch (columnIndex)
                    {
                        case 0:
                            {
                                cellParagraph.Append($"Book {documentSlice.BookNumber}");
                                cellParagraph.FontSize(14);
                                break;
                            }

                        case 1:
                        {
                            var yearRange = "";
                                switch (documentSlice.BookNumber)
                                {
                                    case 1:
                                        yearRange = "Years: 2003 - 2012";
                                        break;
                                    case 2:
                                        yearRange = "Years: 2013 - 2021";
                                        break;
                                    default:
                                        throw new IndexOutOfRangeException(documentSlice.BookNumber?.ToString());
                                }

                                cellParagraph.Append(yearRange);
                                cellParagraph.Alignment = Alignment.left;
                                break;
                            }
                        case 2:
                            {
                                cellParagraph.Append($"Page: {documentSlice.StartPageNumber}");
                                cellParagraph.Alignment = Alignment.left;
                                break;
                            }
                        case 3:
                        {
                            cellParagraph.Append($"Entries: {bookEntryCount}");
                            cellParagraph.Alignment = Alignment.left;
                            break;
                        }
                    }

                    cell.VerticalAlignment = VerticalAlignment.Center;
                    cellParagraph.Color(Start.PurpleColor);
                    return true;
                }
            };

            var documentTocSection = documentSectionManager.AddSection(new SectionInfo() { Type = SectionInfo.SectionInfoType.DocumentTOC });
            var paragraph1 = documentTocSection.InsertParagraph();
            paragraph1.AppendLine("Hello... ")
                .FontSize(14)
                .Color(Start.PinkColor)
                .Bold(true);

            var paragraph2 = documentTocSection.InsertParagraph();
            documentTocSection.InsertParagraph("This series of two books plots the childhood story of the beautiful Katie Clara Uhrskov Gay. The text and pictures are all on the blogsite - martingay.co.uk. There was an attempt to filter out bits that were not related to Katie. ");
            documentTocSection.InsertParagraph("");
            documentTocSection.InsertParagraph("Katie is a very much loved person, treasured by her family. You will not find a more giving, kind and generous person. Hopefully Katie will dip into the book now and again and look back at her upbringing.");
            documentTocSection.InsertParagraph("");

            //documentTocSection.InsertParagraph("The childhood life of 'Katie Clara Uhrskov Gay'. The most beautiful person inside and out you will ever meet.");
            //documentTocSection.InsertParagraph("The book contains the mundane and exciting activities that the family have been involved in whilst Katie has been growing up. There are three books covering her life from 0 to 18 years. Have fun looking back.");

            TableHelper.CreateTable(documentTocSection, null, 2, options);
        }
    }
}