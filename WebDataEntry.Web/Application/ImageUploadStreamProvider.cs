using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebDataEntry.Web.Application
{
    public class ImageUploadStreamProvider : MultipartFormDataStreamProvider
    {
        private List<string> _fileTempPaths;
        private readonly IConfiguration _configuration;

        public List<string> RelativeFilePaths { get; private set; }
        public List<string> FullPaths { get; private set; }

        public ImageUploadStreamProvider(string path)
            : base(path)
        {
            _configuration = new ConfigurationFactory().Create();
			_fileTempPaths = new List<string>();
			RelativeFilePaths = new List<string>();
			FullPaths = new List<string>();
		}


        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            var browserFilePath = headers.ContentDisposition.FileName;
            browserFilePath = browserFilePath.Replace("\"", string.Empty);
            _fileTempPaths.Add(Path.Combine(RootPath, new FileInfo(browserFilePath).Name));
            return browserFilePath;
        }

        public override System.Threading.Tasks.Task ExecutePostProcessingAsync()
		{
			foreach (var fileTempPath in _fileTempPaths)
			{
				ProcessFile(fileTempPath);
			}

			return base.ExecutePostProcessingAsync();
		}

		private readonly static Size maxDimensions = new Size(700, 1200);

		private void ProcessFile(string fileTempPath)
		{
			using (var image = Image.FromFile(fileTempPath))
			using (var newImage = image.ResizeImage(maxDimensions))
			{
				var filePath = MakeDirPath(fileTempPath);
				newImage.SaveJPeg(filePath, 90);
				RelativeFilePaths.Add(filePath.Substring(_configuration.BasePath.Length + 1));
				FullPaths.Add(filePath);
			}

			// remove the temp file.
			File.Delete(fileTempPath);
		}

		private string MakeDirPath(string fileTempPath)
        {
            var fileInfo = new FileInfo(fileTempPath);
            var filename = fileInfo.Name;
            var yearString = filename.Substring(0, 4);

            var yearInFilename = "XXXX";
            int year;
            if (Int32.TryParse(yearString, out year))
            {
                yearInFilename = year.ToString(CultureInfo.InvariantCulture);
            }

            var temp1 = _configuration.DiaryImagesPath;
            var ret1 = Path.Combine(temp1, yearInFilename);
            var ret = Path.Combine(ret1, fileInfo.Name);

            var directoryInfo = new DirectoryInfo(ret);
            if (!directoryInfo.Parent.Exists)
                directoryInfo.Parent.Create();

            return ret;
        }
    }
}