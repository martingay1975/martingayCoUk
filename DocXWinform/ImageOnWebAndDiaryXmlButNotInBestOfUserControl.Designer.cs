namespace DocXWinform
{
    partial class ImageOnWebAndDiaryXmlButNotInBestOfUserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.picOriginal = new System.Windows.Forms.PictureBox();
            this.btnFindLocalImage = new System.Windows.Forms.Button();
            this.lblOriginalFilename = new System.Windows.Forms.Label();
            this.picNew = new System.Windows.Forms.PictureBox();
            this.lblNewFilePath = new System.Windows.Forms.Label();
            this.btnChangeWeb = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picOriginal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNew)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // picOriginal
            // 
            this.picOriginal.Location = new System.Drawing.Point(3, 18);
            this.picOriginal.Name = "picOriginal";
            this.picOriginal.Size = new System.Drawing.Size(259, 126);
            this.picOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picOriginal.TabIndex = 0;
            this.picOriginal.TabStop = false;
            // 
            // btnFindLocalImage
            // 
            this.btnFindLocalImage.Location = new System.Drawing.Point(268, 18);
            this.btnFindLocalImage.Name = "btnFindLocalImage";
            this.btnFindLocalImage.Size = new System.Drawing.Size(75, 48);
            this.btnFindLocalImage.TabIndex = 1;
            this.btnFindLocalImage.Text = "Find Local Image";
            this.btnFindLocalImage.UseVisualStyleBackColor = true;
            this.btnFindLocalImage.Click += new System.EventHandler(this.btnFindLocalImage_Click);
            // 
            // lblOriginalFilename
            // 
            this.lblOriginalFilename.AutoSize = true;
            this.lblOriginalFilename.Location = new System.Drawing.Point(3, 2);
            this.lblOriginalFilename.Name = "lblOriginalFilename";
            this.lblOriginalFilename.Size = new System.Drawing.Size(94, 13);
            this.lblOriginalFilename.TabIndex = 2;
            this.lblOriginalFilename.Tag = " ";
            this.lblOriginalFilename.Text = "lblOriginalFilename";
            // 
            // picNew
            // 
            this.picNew.Location = new System.Drawing.Point(349, 18);
            this.picNew.Name = "picNew";
            this.picNew.Size = new System.Drawing.Size(262, 126);
            this.picNew.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picNew.TabIndex = 3;
            this.picNew.TabStop = false;
            // 
            // lblNewFilePath
            // 
            this.lblNewFilePath.AutoSize = true;
            this.lblNewFilePath.Location = new System.Drawing.Point(346, 2);
            this.lblNewFilePath.Name = "lblNewFilePath";
            this.lblNewFilePath.Size = new System.Drawing.Size(77, 13);
            this.lblNewFilePath.TabIndex = 4;
            this.lblNewFilePath.Tag = " ";
            this.lblNewFilePath.Text = "lblNewFilePath";
            // 
            // btnChangeWeb
            // 
            this.btnChangeWeb.Location = new System.Drawing.Point(617, 18);
            this.btnChangeWeb.Name = "btnChangeWeb";
            this.btnChangeWeb.Size = new System.Drawing.Size(75, 48);
            this.btnChangeWeb.TabIndex = 5;
            this.btnChangeWeb.Text = "Change For Web";
            this.btnChangeWeb.UseVisualStyleBackColor = true;
            this.btnChangeWeb.Click += new System.EventHandler(this.btnChangeWeb_Click);
            // 
            // ImageOnWebAndDiaryXmlButNotInBestOfUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnChangeWeb);
            this.Controls.Add(this.lblNewFilePath);
            this.Controls.Add(this.picNew);
            this.Controls.Add(this.lblOriginalFilename);
            this.Controls.Add(this.btnFindLocalImage);
            this.Controls.Add(this.picOriginal);
            this.Name = "ImageOnWebAndDiaryXmlButNotInBestOfUserControl";
            this.Size = new System.Drawing.Size(786, 152);
            ((System.ComponentModel.ISupportInitialize)(this.picOriginal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNew)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.PictureBox picOriginal;
        private System.Windows.Forms.Button btnFindLocalImage;
        private System.Windows.Forms.Label lblOriginalFilename;
        private System.Windows.Forms.PictureBox picNew;
        private System.Windows.Forms.Label lblNewFilePath;
        private System.Windows.Forms.Button btnChangeWeb;
    }
}
