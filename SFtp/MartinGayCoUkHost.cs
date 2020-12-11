using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SFtp
{
    public class MartinGayCoUkHost
    {
        private readonly string login;
        private readonly string password;
        private readonly string host;
        private readonly string baseHostPath;
        private readonly string baseClientPath;

        public MartinGayCoUkHost(string baseClientPath, string baseHostPath)
        {
            this.baseClientPath = baseClientPath ?? throw new ArgumentNullException(nameof(baseClientPath));
            this.baseHostPath = baseHostPath ?? throw new ArgumentNullException(nameof(baseHostPath));
            this.login = "u54145433";
            this.password = "weston2184";
            this.host = "home292042675.1and1-data.host";
        }

        public bool Exists(string relativePath)
        {
            var hostPath = GetFullHostPath(relativePath);
            using (var stfpBatch = new SFtpBatch(login, password, host))
            {
                return stfpBatch.Exists(hostPath);
            }
        }

        public void UploadBatch(IEnumerable<string> relativePaths)
        {
            using (var stfpBatch = new SFtpBatch(login, password, host))
            {
                Parallel.ForEach(relativePaths, relativePath =>
                {
                    var clientPath = GetFullClientPath(relativePath);
                    var hostPath = GetFullHostPath(relativePath);
                    if (stfpBatch.Upload(clientPath, hostPath))
                    {
                        Console.WriteLine($"{relativePath} Uploaded");
                    }
                });
            }
        }

        public bool Upload(string relativePath)
        {
            using (var stfpBatch = new SFtpBatch(login, password, host))
            {
                var clientPath = GetFullClientPath(relativePath);
                var hostPath = GetFullHostPath(relativePath);
                return stfpBatch.Upload(clientPath, hostPath);
            }
        }

        public string GetRelativePath(string clientPath)
        {
            if (clientPath.IndexOf(baseClientPath) != 0)
            {
                throw new ArgumentException($"The {nameof(clientPath)} must start with {baseClientPath}");
            }

            var relativePath = clientPath.Substring(baseClientPath.Length);

            if (relativePath[0] == '\\')
            {
                relativePath = relativePath.Substring(1);
            }

            return relativePath;
        }

        public string GetFullClientPath(string relativePathFromBase)
        {
            return Path.Combine(baseClientPath, relativePathFromBase.Replace("/", "\\"));
        }

        public string GetFullHostPath(string relativePathFromBase)
        {
            return $"{baseHostPath}{relativePathFromBase.Replace('\\', '/')}";
        }
    }
}