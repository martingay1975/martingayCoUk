using System;
using System.IO;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;

namespace SFtp
{
    public class SFtpBatch : IDisposable
    {
        private readonly SftpClient sftp;

        public SFtpBatch(string login, string password, string host)
        {
            var authenticationMethod = new AuthenticationMethod[] { new PasswordAuthenticationMethod(login, password) };
            var connectionInfo = new ConnectionInfo(host, login, authenticationMethod);
            sftp = new SftpClient(connectionInfo);
            sftp.OperationTimeout = TimeSpan.FromMinutes(1);
            sftp.BufferSize = 4096;
            sftp.Connect();
        }

        public void Download(string hostRelativePath, string clientPath)
        {
            if (string.IsNullOrWhiteSpace(hostRelativePath))
            {
                throw new ArgumentException(nameof(hostRelativePath));
            }

            if (string.IsNullOrWhiteSpace(clientPath))
            {
                throw new ArgumentException(nameof(clientPath));
            }

            var destinationFileInfo = new FileInfo(clientPath);
            if (!Directory.Exists(destinationFileInfo.DirectoryName))
            {
                Directory.CreateDirectory(destinationFileInfo.DirectoryName);
            }

            using (var fileStream = new FileStream(clientPath, FileMode.Create))
            {
                sftp.DownloadFile(hostRelativePath, fileStream);
            }
        }
        public void UploadFile(string clientPath, string hostPath)
        {
            using (var fileStream = new FileStream(clientPath, FileMode.Open))
            {
                sftp.UploadFile(fileStream, hostPath, canOverride: true);
            }
        }

        public bool Exists(string hostPath)
        {
            return sftp.Exists(hostPath);
        }

        public bool Upload(string clientPath, string hostPath)
        {
            var fileInfo = new FileInfo(clientPath);
            SftpFileAttributes status;

            try
            {
                status = sftp.GetAttributes(hostPath);
            }
            catch (SftpPathNotFoundException)
            {
                // file does not exist, saves calling exists()
                UploadFile(clientPath, hostPath);
                return true;
            }

            if (status.Size != fileInfo.Length)
            {
                UploadFile(clientPath, hostPath);
                return true;
            }

            // file exists and same size, so not going upload
            return false;
        }

        public void DeleteFile(string hostPath)
        {
            sftp.DeleteFile(hostPath);
        }

        public void Dispose()
        {
            sftp.Disconnect();
            sftp.Dispose();
        }
    }
}
