using DocXLib.Image;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace DocXLib
{
    public static class BadImageManager
    {
        private static readonly string FoundOnHDNotInBestOfPath = Path.Combine(PictureHelper.BaseImagePath, "_FoundOnHDNotInBestOf");
        private static readonly string FoundOnWebNotInBestOfPath = Path.Combine(PictureHelper.BaseImagePath, "_FoundOnWebNotInBestOfPath");

        public static void Process(in List<BadImageItem> badImages)
        {
            Debug.WriteLine($"There are {badImages.Count} bad images.");
            int count = 1;
            foreach (var badImage in badImages)
            {
                badImage.FindFilename();

                var value = badImage.FoundLocations == null ? "-" : string.Join(",", badImage.FoundLocations);
                Debug.WriteLine($"{count} - {badImage.Filename} - {value}");

                if (badImage.FoundLocations != null && badImage.FoundLocations.Length == 1)
                {
                    // found the image - just not in bestOf directory
                    File.Copy(badImage.FoundLocations[0], Path.Combine(FoundOnHDNotInBestOfPath, badImage.Filename), true);
                    badImage.MissingState = BadImageItem.MissingStateEnum.FoundOnHDNotInBestOfPath;
                }
                else
                {
                    using (WebClient client = new WebClient())
                    {
                        var url = $"http://www.martingay.co.uk/images/years/{badImage.Year}/{badImage.Filename}";
                        client.DownloadFile(new Uri(url), Path.Combine(FoundOnWebNotInBestOfPath, badImage.Filename));
                        badImage.MissingState = BadImageItem.MissingStateEnum.FoundOnWebNotInBestOfPath;
                    }
                }
                count++;
            }
        }
    }
}
