using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DocXLib;

namespace DocXWinform
{
    public partial class ImageOnWebAndDiaryXmlButNotInBestOfUserControl : UserControl
    {
        private const string DiaryXmlPath = @"C:\Users\Slop\AppData\Roaming\res\xml\diary.xml";
        private const string UploadImageUrl = "http://localhost:53000/api/rpc/UploadImages";
        private const string DeleteImageUrl = "http://localhost:53000/api/rpc/DeleteImage/{0}/{1}";

        private BadImageItem BadImageItem { get; }

        public ImageOnWebAndDiaryXmlButNotInBestOfUserControl(BadImageItem badImageItem, int index)
        {
            InitializeComponent();
            BadImageItem = badImageItem;
            Initialize();
            this.Top = this.Height * index;
        }

        private void Initialize()
        {
            this.lblOriginalFilename.Text = BadImageItem.Filename;
            this.picOriginal.Load($"http://www.martingay.co.uk/images/years/{BadImageItem.Year}/{BadImageItem.Filename}");
        }

        private void btnFindLocalImage_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.InitialDirectory = Path.Combine(PictureHelper.BaseImagePath, BadImageItem.Year);
            this.openFileDialog1.Multiselect = false;
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.lblNewFilePath.Text = this.openFileDialog1.FileName;
                this.picNew.Load(this.openFileDialog1.FileName);
            }
            else
            {
                this.lblNewFilePath.Text = "";
            }
        }

        private void btnChangeWeb_Click(object sender, EventArgs e)
        {
            // Load the diary and search and replace the original file with the new one.
            var diaryXmlContents = File.ReadAllText(DiaryXmlPath);

            var (_, newFilename) = PictureHelper.GetImagePathParts(this.lblNewFilePath.Text);
            var year = BadImageItem.Year;

            // Copy file to _BestOf folder (if not already there)
            var bestOfDestination = Path.Combine(PictureHelper.BaseImagePath, year, "_BestOf", newFilename);
            File.Copy(this.lblNewFilePath.Text, bestOfDestination, true);
            Debug.WriteLine($"Copied '{this.lblNewFilePath.Text}' to '{bestOfDestination}'");

            // Now upload the image.
            var fileContent = File.ReadAllBytes(this.lblNewFilePath.Text);
            var targetFilePath = $"images/years/{year}/{newFilename}";
            UploadMultipart(fileContent, 
                targetFilePath,
                "image/jpeg", 
                UploadImageUrl);
            Debug.WriteLine($"Uploaded new image: {targetFilePath}");

            var newDiaryXmlContents = diaryXmlContents.Replace(BadImageItem.Filename, newFilename);
            File.WriteAllText(DiaryXmlPath, newDiaryXmlContents);
            Debug.WriteLine("Change diary.xml");

            if (BadImageItem.Filename != newFilename)
            {
                var deleteUrl = string.Format(DeleteImageUrl, year, BadImageItem.Filename);
                using (var webClient = new System.Net.WebClient())
                {
                    webClient.DownloadString(deleteUrl);
                }
                Debug.WriteLine($"Removed old file from web: {BadImageItem.Filename}");
            }
        }

        public void UploadMultipart(byte[] file, string filename, string contentType, string url)
        {
            using (var webClient = new System.Net.WebClient())
            {
                string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
                webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
                var fileData = webClient.Encoding.GetString(file);
                var package = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n{3}\r\n--{0}--\r\n", boundary, filename, contentType, fileData);

                var nfile = webClient.Encoding.GetBytes(package);

                byte[] resp = webClient.UploadData(url, "POST", nfile);
                var response = Encoding.UTF8.GetString(resp);
            }
        }
    }
}
