using System.IO;

namespace WebDataEntry.Web.Application
{
    public class ConfigurationFactory
    {
        private static IConfiguration _configuration;

        public IConfiguration Create()
        {
            if (_configuration == null)
            {
                var configuration = new Configuration();
                configuration.DiaryXmlFilePath = Path.Combine(configuration.BasePath, @"res\xml\diary.xml");
                _configuration = configuration;
            }

            return _configuration;
        }
    }
}