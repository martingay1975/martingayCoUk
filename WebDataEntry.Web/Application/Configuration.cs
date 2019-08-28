using System.Configuration;
using System.IO;

namespace WebDataEntry.Web.Application
{
    public class Configuration : IConfiguration
    {
        public string DiaryXmlFilePath { get; set; }
        public string JsonDirectoryPath { get { return Path.Combine(BasePath, @"res\json"); } }
        public string BasePath { get { return @"c:\www.martingay.f2s.com"; } }
        public string DiaryImagesPath { get { return Path.Combine(BasePath, @"images\years"); } }
        public string FtpHost { get { return ConfigurationManager.AppSettings["FtpHost"];  }  }
        public string FtpLogin { get { return ConfigurationManager.AppSettings["FtpLogin"]; } }
        public string FtpPassword { get { return ConfigurationManager.AppSettings["FtpPassword"]; } }
    }
}