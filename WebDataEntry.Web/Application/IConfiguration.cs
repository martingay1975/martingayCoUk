namespace WebDataEntry.Web.Application
{
    public interface IConfiguration
    {
        string DiaryXmlFilePath { get; set; }
        string JsonDirectoryPath { get; }
        string BasePath { get; }
        string DiaryImagesPath { get; }
        string FtpHost { get; }
        string FtpLogin { get; }
        string FtpPassword { get; }
    }
}