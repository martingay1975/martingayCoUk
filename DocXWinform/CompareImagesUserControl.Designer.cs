
namespace DocXWinform
{
    partial class CompareImagesUserControl
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
            this.pbLocal = new System.Windows.Forms.PictureBox();
            this.pbHost = new System.Windows.Forms.PictureBox();
            this.lblScore = new System.Windows.Forms.Label();
            this.lblHost = new System.Windows.Forms.LinkLabel();
            this.lblLocal = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pbLocal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbHost)).BeginInit();
            this.SuspendLayout();
            // 
            // pbLocal
            // 
            this.pbLocal.Location = new System.Drawing.Point(3, 47);
            this.pbLocal.Name = "pbLocal";
            this.pbLocal.Size = new System.Drawing.Size(249, 100);
            this.pbLocal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbLocal.TabIndex = 0;
            this.pbLocal.TabStop = false;
            // 
            // pbHost
            // 
            this.pbHost.Location = new System.Drawing.Point(288, 47);
            this.pbHost.Name = "pbHost";
            this.pbHost.Size = new System.Drawing.Size(249, 100);
            this.pbHost.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbHost.TabIndex = 2;
            this.pbHost.TabStop = false;
            // 
            // lblScore
            // 
            this.lblScore.AutoSize = true;
            this.lblScore.Location = new System.Drawing.Point(543, 134);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(33, 13);
            this.lblScore.TabIndex = 4;
            this.lblScore.Text = "score";
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(285, 16);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(55, 13);
            this.lblHost.TabIndex = 7;
            this.lblHost.TabStop = true;
            this.lblHost.Text = "linkLabel1";
            this.lblHost.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblHost_LinkClicked);
            // 
            // lblLocal
            // 
            this.lblLocal.AutoSize = true;
            this.lblLocal.Location = new System.Drawing.Point(33, 16);
            this.lblLocal.Name = "lblLocal";
            this.lblLocal.Size = new System.Drawing.Size(55, 13);
            this.lblLocal.TabIndex = 8;
            this.lblLocal.TabStop = true;
            this.lblLocal.Text = "linkLabel1";
            this.lblLocal.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblLocal_LinkClicked);
            // 
            // CompareImagesUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblLocal);
            this.Controls.Add(this.lblHost);
            this.Controls.Add(this.lblScore);
            this.Controls.Add(this.pbHost);
            this.Controls.Add(this.pbLocal);
            this.Name = "CompareImagesUserControl";
            this.Size = new System.Drawing.Size(579, 150);
            ((System.ComponentModel.ISupportInitialize)(this.pbLocal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbHost)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbLocal;
        private System.Windows.Forms.PictureBox pbHost;
        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.LinkLabel lblHost;
        private System.Windows.Forms.LinkLabel lblLocal;
    }
}
