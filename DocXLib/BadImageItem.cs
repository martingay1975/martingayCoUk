using DocXLib.Image;
using System;
using System.IO;

namespace DocXLib
{
    public class BadImageItem
    {
        public enum MissingStateEnum
        {
            None,
            FoundOnHDNotInBestOfPath,
            FoundOnWebNotInBestOfPath
        }

        public BadImageItem(string year, string filename, string originalFullPath)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            Year = year;
            Filename = filename;
            OriginalFullPath = originalFullPath;
            FoundLocations = null;
        }

        public string Year;
        public string Filename;
        public string[] FoundLocations;
        public MissingStateEnum MissingState;
        public string OriginalFullPath;

        public void FindFilename()
        {
            var yearSearchPath = Path.Combine(PictureHelper.BaseImagePath, Year);
            FoundLocations = Directory.GetFiles(yearSearchPath, Filename, SearchOption.AllDirectories);
            if (FoundLocations.Length == 0)
            {
                FoundLocations = Directory.GetFiles(PictureHelper.BaseImagePath, Filename, SearchOption.AllDirectories);
            }
        }
    }
}
