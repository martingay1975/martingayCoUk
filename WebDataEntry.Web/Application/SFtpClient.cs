using Renci.SshNet;
using System.IO;
using System;
using System.Diagnostics;
using log4net;


namespace WebDataEntry.Web.Application
{
	public class SFtpBatch : IDisposable
	{
		private SftpClient sftp;
		private IConfiguration configuration;
		private ILog log = LogManager.GetLogger("SFTPBatch");

		public SFtpBatch(IConfiguration configuration)
		{
			this.configuration = configuration;
			var authenticationMethod = new AuthenticationMethod[] { new PasswordAuthenticationMethod(configuration.FtpLogin, configuration.FtpPassword) };
			var connectionInfo = new ConnectionInfo(configuration.FtpHost, configuration.FtpLogin, authenticationMethod);
			this.sftp = new SftpClient(connectionInfo);
			this.sftp.OperationTimeout = TimeSpan.FromMinutes(1);
			sftp.BufferSize = 4096;

			try
			{
				sftp.Connect();
			}
			catch (Exception e)
			{
				log.Error(e);
				throw;
			}
		}

		public void Download(string filePath, string destinationFilePath)
		{
			destinationFilePath = destinationFilePath.Replace("/", "\\");
            var destinationFileInfo = new FileInfo(destinationFilePath);
            if (!Directory.Exists(destinationFileInfo.DirectoryName))
            {
                Directory.CreateDirectory(destinationFileInfo.DirectoryName);
            }

            Debug.WriteLine("Downloading " + filePath);
			using (var fileStream = new FileStream(destinationFilePath, FileMode.Create))
			{
				sftp.DownloadFile("/martingay/" + filePath, fileStream);
			}
		}

		internal void Upload(string relativePath)
		{
			(var sourcePath, var destinationPath) = GetPaths(this.configuration.BasePath, relativePath);

			using (var fileStream = new FileStream(sourcePath, FileMode.Open))
			{
				sftp.UploadFile(fileStream, destinationPath, canOverride: true);
			}
		}

		public static (string sourcePath, string destinationPath) GetPaths(string basePath, string relativePath)
		{
			var sourcePath = Path.Combine(basePath, relativePath);
			sourcePath = sourcePath.Replace("/", "\\");
			var destinationPath = "/martingay/" + relativePath.Replace("\\", "/");
			return (sourcePath, destinationPath);
		}

		public void Dispose()
		{
			sftp.Disconnect();
			sftp.Dispose();
		}
	}
}
