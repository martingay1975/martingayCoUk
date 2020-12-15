﻿using System.Diagnostics;
using System.IO;
using Xceed.Document.NET;
using System.Collections.Generic;
using System;
using SFtp;

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

        public static Picture CreateEntryPicture(in EntryContext entryContext, string imagePath, string caption)
        {
            // go find the image from the _BestOf directory. Full Res image.
            var sourceImagePath = GetFullImagePath(imagePath);
            var documentPicture = CreatePicture(entryContext.Document, sourceImagePath);
            documentPicture.Name = caption;
            return documentPicture;
        }

        public static Picture CreatePicture(Document document, string sourceImagePath)
        {
            using (var fs = new FileStream(sourceImagePath, FileMode.Open, FileAccess.Read))
            {
                var documentImage = document.AddImage(fs);
                var documentPicture = documentImage.CreatePicture();
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
                throw new Exception($"Cannot find file '{convertedPath}'. Original '{imagePath}'");
            }

            return convertedPath;
        }

        /// <summary>
        /// Takes a src as set in the xml
        /// </summary>
        public static CompareImagesData CompareWithHostVersion(string src)
        {
            var compareImagesData = new CompareImagesData() { Src = src };
            var hostUrl = $"http://martingay.co.uk/{src.Replace('\\', '/')}";
            compareImagesData.HostFile = hostUrl;

            try
            {
                compareImagesData.LocalFile = GetFullImagePath(src);
            }
            catch
            {
                compareImagesData.Exists = FileExist.NotOnClient;
                return compareImagesData;
            }

            try
            {
                compareImagesData.Score = ImageSimilarity.GetSimilarScore(compareImagesData.LocalFile, hostUrl);
                compareImagesData.Exists = FileExist.OK;
            }
            catch (Exception)
            {
                compareImagesData.Exists = FileExist.NotOnHost;
            }

            return compareImagesData;
        }
    }
}

