using System.Diagnostics;
using System.IO;
using Xceed.Document.NET;
using System.Collections.Generic;

namespace DocXLib
{
    public static class PictureHelper
    {
        public readonly static List<BadImageItem> NotInBestOfImages;
        public const string BaseImagePath = @"L:\images";
        
        static PictureHelper()
        {
            NotInBestOfImages = new List<BadImageItem>();
        }

        public static Picture CreateImage(in EntryContext entryContext, string imagePath, string caption)
        {
            // go find the image from the _BestOf directory. Full Res image.
            var sourceImagePath = GetFullImagePath(imagePath);
            using (var fs = new FileStream(sourceImagePath, FileMode.Open, FileAccess.Read))
            {
                var documentImage = entryContext.Document.AddImage(fs);
                var documentPicture = documentImage.CreatePicture();
                documentPicture.Name = caption;
                return documentPicture;
            }
        }

        public static (string year, string filename) GetImagePathParts(string imagePath)
        {
            var pathParts = imagePath.Split(new char[] { '\\', '/' });

            var year = pathParts[pathParts.Length - 2];
            var filename = pathParts[pathParts.Length - 1];

            return (year, filename);
        }

        private static string GetFullImagePath(string imagePath)
        {
            // e.g file path is images\years\2020\2020_09_12-08-Fars50thWeddingAnniversary.jpeg
            // need to turn into L:\images\2020\_BestOf\2020_09_12-08-Fars50thWeddingAnniversary.jpeg

            var (year, filename) = GetImagePathParts(imagePath);

            var convertedPath = Path.Combine(BaseImagePath, year, "_BestOf", filename);

            if (!File.Exists(convertedPath))
            {
                NotInBestOfImages.Add(new BadImageItem(year, filename, imagePath));
                Debug.WriteLine($"'{convertedPath,50}'. Original '{imagePath}'");
                //throw new Exception($"Cannot find file '{convertedPath}'. Original '{imagePath}'");
            }

            return convertedPath;
        }
    }
}

