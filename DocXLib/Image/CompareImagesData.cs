namespace DocXLib.Image
{
    public class CompareImagesData
    {
        public string Src { get; set; }
        public string LocalFile { get; set; }
        public string HostFile { get; set; }
        public int Score { get; set; }
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
    }
}
