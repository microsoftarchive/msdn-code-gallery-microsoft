namespace CSWebDownloader
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.prgDownload = new System.Windows.Forms.ProgressBar();
            this.lbUrl = new System.Windows.Forms.Label();
            this.tbURL = new System.Windows.Forms.TextBox();
            this.btnDownload = new System.Windows.Forms.Button();
            this.lbPath = new System.Windows.Forms.Label();
            this.tbPath = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lbStatus = new System.Windows.Forms.Label();
            this.btnPause = new System.Windows.Forms.Button();
            this.lbSummary = new System.Windows.Forms.Label();
            this.btnCheckUrl = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // prgDownload
            // 
            this.prgDownload.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.prgDownload.Location = new System.Drawing.Point(0, 147);
            this.prgDownload.Name = "prgDownload";
            this.prgDownload.Size = new System.Drawing.Size(704, 23);
            this.prgDownload.TabIndex = 0;
            // 
            // lbUrl
            // 
            this.lbUrl.AutoSize = true;
            this.lbUrl.Location = new System.Drawing.Point(12, 40);
            this.lbUrl.Name = "lbUrl";
            this.lbUrl.Size = new System.Drawing.Size(29, 13);
            this.lbUrl.TabIndex = 1;
            this.lbUrl.Text = "URL";
            // 
            // tbURL
            // 
            this.tbURL.Location = new System.Drawing.Point(75, 36);
            this.tbURL.Name = "tbURL";
            this.tbURL.Size = new System.Drawing.Size(538, 20);
            this.tbURL.TabIndex = 2;
            // 
            // btnDownload
            // 
            this.btnDownload.Enabled = false;
            this.btnDownload.Location = new System.Drawing.Point(365, 89);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(75, 23);
            this.btnDownload.TabIndex = 3;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // lbPath
            // 
            this.lbPath.AutoSize = true;
            this.lbPath.Location = new System.Drawing.Point(12, 67);
            this.lbPath.Name = "lbPath";
            this.lbPath.Size = new System.Drawing.Size(58, 13);
            this.lbPath.TabIndex = 1;
            this.lbPath.Text = "Local Path";
            // 
            // tbPath
            // 
            this.tbPath.Enabled = false;
            this.tbPath.Location = new System.Drawing.Point(76, 63);
            this.tbPath.Name = "tbPath";
            this.tbPath.Size = new System.Drawing.Size(537, 20);
            this.tbPath.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(539, 89);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Location = new System.Drawing.Point(12, 131);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(0, 13);
            this.lbStatus.TabIndex = 1;
            // 
            // btnPause
            // 
            this.btnPause.Enabled = false;
            this.btnPause.Location = new System.Drawing.Point(454, 89);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 23);
            this.btnPause.TabIndex = 4;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // lbSummary
            // 
            this.lbSummary.AutoSize = true;
            this.lbSummary.Location = new System.Drawing.Point(114, 131);
            this.lbSummary.Name = "lbSummary";
            this.lbSummary.Size = new System.Drawing.Size(0, 13);
            this.lbSummary.TabIndex = 1;
            // 
            // btnCheckUrl
            // 
            this.btnCheckUrl.Location = new System.Drawing.Point(619, 35);
            this.btnCheckUrl.Name = "btnCheckUrl";
            this.btnCheckUrl.Size = new System.Drawing.Size(75, 23);
            this.btnCheckUrl.TabIndex = 5;
            this.btnCheckUrl.Text = "CheckUrl";
            this.btnCheckUrl.UseVisualStyleBackColor = true;
            this.btnCheckUrl.Click += new System.EventHandler(this.btnCheckUrl_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(704, 170);
            this.Controls.Add(this.btnCheckUrl);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.tbPath);
            this.Controls.Add(this.tbURL);
            this.Controls.Add(this.lbSummary);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.lbPath);
            this.Controls.Add(this.lbUrl);
            this.Controls.Add(this.prgDownload);
            this.Name = "MainForm";
            this.Text = "CSWebDownloader";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar prgDownload;
        private System.Windows.Forms.Label lbUrl;
        private System.Windows.Forms.TextBox tbURL;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Label lbPath;
        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Label lbSummary;
        private System.Windows.Forms.Button btnCheckUrl;
    }
}

