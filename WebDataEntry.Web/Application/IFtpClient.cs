namespace WebDataEntry.Web.Application
{
    public interface IFtpClient
    {
        void Download(string filePath, string destinationFilePath);
        void UploadAsync(string relativePath);
    }
}