using DocXLib;
using DocXLib.Image;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace DocXWinform
{
    public partial class ContainerOfControls : Form
    {
        private IReadOnlyList<CompareImagesData> CompareImagesData;
        private int start = 0;
        private const int controlsPerPage = 6;
        private CompareImages compareImages;
        public ContainerOfControls()
        {
            InitializeComponent();
            compareImages = new CompareImages();

            var config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            var startIndex = config.AppSettings.Settings["StartIndex"];
            if (!int.TryParse(startIndex?.ToString(), out start))
            {
                start = 0;
            }
        }

        private void SetIndexLabel()
        {
            lblIndex.Text = $"{start} / {CompareImagesData.Count} (+{controlsPerPage})";
        }

        private void ImageOnWebAndDiaryXmlButNotInBestOf_Load(object sender, EventArgs e)
        {
            CompareImagesData = LoadCompareData().ToList();
            foreach (var c in CompareImagesData)
            {
                c.Download();
            }

            SetIndexLabel();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            SetIndexLabel();

            var nextImagesData = CompareImagesData.Skip(start).Take(controlsPerPage);
            var nextPageControls = nextImagesData.Select(compareImageData => 
                new CompareImagesUserControl(compareImageData.LocalFile, compareImageData.HostFile, compareImageData.Score)).ToList();
            
            for (var nextPageControlIdx = 0; nextPageControlIdx < nextPageControls.Count; nextPageControlIdx ++)
            {
                var nextPageControl = nextPageControls[nextPageControlIdx];
                nextPageControl.Top = nextPageControl.Height * nextPageControlIdx;

            }

            NextPage(nextPageControls);
            start += controlsPerPage;
        }

        private void NextPage(IReadOnlyList<UserControl> userControls)
        {
            this.panel1.SuspendLayout();
            panel1.Controls.Clear();

            panel1.Controls.AddRange(userControls.ToArray());

            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
        }

        private void GetBestOf()
        {
            Start.Run();

            var imageOnWebAndDiaryXmlButNotInBestOfUserControls = PictureHelper.NotInBestOfImages
                .Select((badImageItem, idx) => new ImageOnWebAndDiaryXmlButNotInBestOfUserControl(badImageItem, idx));

            panel1.Controls.AddRange(imageOnWebAndDiaryXmlButNotInBestOfUserControls.ToArray());
        }

        private IReadOnlyList<CompareImagesData> LoadCompareData()
        {
            return compareImages.LoadCompareImagesData(Start.DocXDirectory)
                .Where(c => c.Score > 13)
                .OrderBy(c => c.HostFile.ToLowerInvariant()).ToList();
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            config.AppSettings.Settings.Add("StartIndex", start.ToString());
            config.Save(ConfigurationSaveMode.Minimal);
        }
    }
}
