using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace DocXWinform
{
    public partial class CompareImagesUserControl : UserControl
    {
        public CompareImagesUserControl(string localFile, string hostFile, int score)
        {
            InitializeComponent();

            this.lblLocal.Text = localFile;
            this.pbLocal.ImageLocation = localFile;
            this.pbLocal.Load();

            this.lblHost.Text = hostFile;
            this.pbHost.ImageLocation = hostFile;
            this.pbHost.Load();

            this.lblScore.Text = score.ToString();
        }

        private void lblHost_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(this.lblHost.Text);
        }

        private void lblLocal_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var dirInfo = new FileInfo(this.lblLocal.Text);
            Process.Start(dirInfo.Directory.FullName);
        }
    }
}
