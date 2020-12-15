using DocXLib;
using DocXLib.Image;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace DocXWinform
{
    public partial class ImageOnWebAndDiaryXmlButNotInBestOf : Form
    {
        public ImageOnWebAndDiaryXmlButNotInBestOf()
        {
            InitializeComponent();
        }

        private void ImageOnWebAndDiaryXmlButNotInBestOf_Load(object sender, EventArgs e)
        {
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            NextPage();
        }

        private void NextPage()
        {
            this.panel1.SuspendLayout();

            panel1.Controls.Clear();
            Start.Run();

            var imageOnWebAndDiaryXmlButNotInBestOfUserControls = PictureHelper.NotInBestOfImages
                .Select((badImageItem, idx) => new ImageOnWebAndDiaryXmlButNotInBestOfUserControl(badImageItem, idx));

            panel1.Controls.AddRange(imageOnWebAndDiaryXmlButNotInBestOfUserControls.ToArray());

            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
        }
    }
}
