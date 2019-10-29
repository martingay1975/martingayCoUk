using System;
using System.Configuration;
using System.IO;

namespace WebDataEntry.Web.Application
{
    public class Configuration : IConfiguration
    {
        public string DiaryXmlFilePath { get; set; }
        public string JsonDirectoryPath => Path.Combine(BasePath, @"res\json");
        public string BasePath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public string DiaryImagesPath => Path.Combine(BasePath, @"images\years");
        public string FtpHost => ConfigurationManager.AppSettings["FtpHost"];
        public string FtpLogin => ConfigurationManager.AppSettings["FtpLogin"];
        public string FtpPassword => ConfigurationManager.AppSettings["FtpPassword"];
    }
}