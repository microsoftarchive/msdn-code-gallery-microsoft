namespace CSIEExplorerBar
{
    partial class ImageListExplorerBar
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
            this.pnlCmd = new System.Windows.Forms.Panel();
            this.btnGetImg = new System.Windows.Forms.Button();
            this.pnlImgList = new System.Windows.Forms.Panel();
            this.lstImg = new System.Windows.Forms.ListBox();
            this.pnlCmd.SuspendLayout();
            this.pnlImgList.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCmd
            // 
            this.pnlCmd.Controls.Add(this.btnGetImg);
            this.pnlCmd.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCmd.Location = new System.Drawing.Point(0, 0);
            this.pnlCmd.Name = "pnlCmd";
            this.pnlCmd.Size = new System.Drawing.Size(366, 36);
            this.pnlCmd.TabIndex = 0;
            // 
            // btnGetImg
            // 
            this.btnGetImg.Location = new System.Drawing.Point(16, 4);
            this.btnGetImg.Name = "btnGetImg";
            this.btnGetImg.Size = new System.Drawing.Size(129, 23);
            this.btnGetImg.TabIndex = 0;
            this.btnGetImg.Text = "Get all images";
            this.btnGetImg.UseVisualStyleBackColor = true;
            this.btnGetImg.Click += new System.EventHandler(this.btnGetImg_Click);
            // 
            // pnlImgList
            // 
            this.pnlImgList.Controls.Add(this.lstImg);
            this.pnlImgList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlImgList.Location = new System.Drawing.Point(0, 36);
            this.pnlImgList.Name = "pnlImgList";
            this.pnlImgList.Size = new System.Drawing.Size(366, 412);
            this.pnlImgList.TabIndex = 1;
            // 
            // lstImg
            // 
            this.lstImg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstImg.FormattingEnabled = true;
            this.lstImg.Location = new System.Drawing.Point(0, 0);
            this.lstImg.Name = "lstImg";
            this.lstImg.Size = new System.Drawing.Size(366, 412);
            this.lstImg.TabIndex = 0;
            this.lstImg.DoubleClick += new System.EventHandler(this.lstImg_DoubleClick);
            // 
            // BookmarkExplorerBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlImgList);
            this.Controls.Add(this.pnlCmd);
            this.Name = "BookmarkExplorerBar";
            this.Size = new System.Drawing.Size(366, 448);
            this.pnlCmd.ResumeLayout(false);
            this.pnlImgList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlCmd;
        private System.Windows.Forms.Button btnGetImg;
        private System.Windows.Forms.Panel pnlImgList;
        private System.Windows.Forms.ListBox lstImg;

    }
}
