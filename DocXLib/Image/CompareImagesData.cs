using CsvHelper.Configuration.Attributes;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DocXLib.Image
{
    public class CompareImagesData
    {
        [Index(2)]
        public string Src { get; set; }
        
        [Index(3)]
        public string LocalFile { get; set; }
        
        [Index(4)]
        public string HostFile { get; set; }
        
        [Index(1)]
        public int Score { get; set; }

        [Index(0)]
        public FileExist Exists { get; set; }

        public bool IsBad()
        {
            return Exists != FileExist.OK || Score != 0;
        }

        public override string ToString()
        {
            var ret = $"{Exists},{Score,3},{Src,30}";
            if (IsBad())
            {
                ret += $",{LocalFile,50},{ HostFile,50}";
            }

            return ret;
        }

        public bool Download()
        {
            var downloadDestinationPath = Path.Combine(Path.GetDirectoryName(LocalFile), $"_DELETE_{Path.GetFileName(LocalFile)}");
            if (!File.Exists(downloadDestinationPath))
            {
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile(new Uri(HostFile), downloadDestinationPath);
                }
                return true;
            }

            return false;
        }
    }
}
