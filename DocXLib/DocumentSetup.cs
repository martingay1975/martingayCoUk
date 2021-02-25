using DocXLib.Image;
using DocXLib.Model.Data.Xml;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace DocXLib
{
    class DocumentSetup
    {
        private const string FontName = "Adobe Fangsong Std R";
        public const bool IncludeBleeding = false;

        public const double marginInches = 0.5;     // minimum margin where text should not appear. Minimum 0.5"
        public const double bleedInches = 0.25;     // increase the size of the page and margin. Minimum 0.25"
        public const double gutterSpineMarginInches = 0.2;  // allow for extra margin where the spine is. Minimum 0.2"

        public const double A4PageHeightInches = 11.7d;
        public const double A4PageWidthInches = 8.3d;

        public static DocumentSectionManager DocumentSectionManager;

        public static DocX CreateAndSetupDocument(IEnumerable<Entry> documentEntries, int startAtChunkIdx, bool includePictures, string docxDirectory)
        {
            string filePath;

            if (includePictures)
            {
                var documentFromPostfix = documentEntries.First().DateEntry.GetShortDate().Replace(" ", "-");
                var documentLastPostfix = documentEntries.Last().DateEntry.GetShortDate().Replace(" ", "-");
                filePath = Path.Combine(docxDirectory, $"diary{startAtChunkIdx} {documentFromPostfix} to {documentLastPostfix}.docx");
            }
            else
            {
                filePath = Path.Combine(docxDirectory, $"diaryNoPics.docx");
            }

            var document = DocX.Create(filePath);
            DocumentSectionManager = new DocumentSectionManager(document);

            var font = new Xceed.Document.NET.Font(FontName);
            document.SetDefaultFont(font, 10d, Color.Black);
            document.DifferentOddAndEvenPages = true;
            document.PageHeight = InchesToPoints(A4PageHeightInches) + (GetBleedingPoints() * 2);
            document.PageWidth = InchesToPoints(A4PageWidthInches) + (GetBleedingPoints() * 2);

            return document;
        }

        public static void InsertDocumentFrontPage(Document document, int documentNo)
        {
            var documentCoverSection = DocumentSectionManager.AddSection(new SectionInfo() { Type = SectionInfo.SectionInfoType.DocumentCover });
            InsertFrontPageImagePage(document, documentCoverSection, documentNo);
        }

        private static void InsertFrontPageImagePage(Document document, Section section, int documentNo)
        {
            var paragraph = section.InsertParagraph();
            var frontPageImage = PictureHelper.CreatePicture(document, Path.Combine(Start.ChapterImageDirectory, $"FrontCover{documentNo}.jpg"));
            frontPageImage.Width = frontPageImage.Width * Start.ResizeChapterPics;
            frontPageImage.Height = frontPageImage.Height * Start.ResizeChapterPics;
            paragraph.AppendPicture(frontPageImage);
        }


        public static void ApplyStandardMargins(Section section)
        {
            section.MarginBottom = InchesToPoints(marginInches) + GetBleedingPoints();
            section.MarginTop = InchesToPoints(marginInches) + GetBleedingPoints();
            
            section.MarginLeft = InchesToPoints(marginInches + gutterSpineMarginInches) + GetBleedingPoints();
            section.MarginRight = InchesToPoints(marginInches) + GetBleedingPoints();

            section.MirrorMargins = true;
        }

        private static int GetBleedingPoints()
        {
            return IncludeBleeding ? InchesToPoints(bleedInches) : 0;
        }

        /// <summary>
        /// Gets the width of the live part of the page.
        /// </summary>
        public static int GetLivePageWidthPoints()
        {
            return InchesToPoints(A4PageWidthInches - (2 * marginInches));
        }

        private static int InchesToPoints(double inches)
        {
            return (int)(inches * 72);
        }

        public static void ApplyZeroMargins(Section section)
        {
            section.MarginBottom = GetBleedingPoints();
            section.MarginTop = GetBleedingPoints();
            section.MarginLeft = GetBleedingPoints();
            section.MarginRight = GetBleedingPoints();
            section.MirrorMargins = false;
        }
    }
}
