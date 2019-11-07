namespace CSSetDesktopWallpaper
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
            this.components = new System.ComponentModel.Container();
            this.tbWallpaperFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowseWallpaper = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radFill = new System.Windows.Forms.RadioButton();
            this.radFit = new System.Windows.Forms.RadioButton();
            this.radStretch = new System.Windows.Forms.RadioButton();
            this.radCenter = new System.Windows.Forms.RadioButton();
            this.radTile = new System.Windows.Forms.RadioButton();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.pctPreview = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSetWallpaper = new System.Windows.Forms.Button();
            this.wallpaperOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pctPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // tbWallpaperFileName
            // 
            this.tbWallpaperFileName.Location = new System.Drawing.Point(77, 16);
            this.tbWallpaperFileName.Name = "tbWallpaperFileName";
            this.tbWallpaperFileName.ReadOnly = true;
            this.tbWallpaperFileName.Size = new System.Drawing.Size(272, 20);
            this.tbWallpaperFileName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Wallpaper";
            // 
            // btnBrowseWallpaper
            // 
            this.btnBrowseWallpaper.Location = new System.Drawing.Point(355, 14);
            this.btnBrowseWallpaper.Name = "btnBrowseWallpaper";
            this.btnBrowseWallpaper.Size = new System.Drawing.Size(88, 23);
            this.btnBrowseWallpaper.TabIndex = 3;
            this.btnBrowseWallpaper.Text = "Browse...";
            this.btnBrowseWallpaper.UseVisualStyleBackColor = true;
            this.btnBrowseWallpaper.Click += new System.EventHandler(this.btnBrowseWallpaper_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radFill);
            this.groupBox1.Controls.Add(this.radFit);
            this.groupBox1.Controls.Add(this.radStretch);
            this.groupBox1.Controls.Add(this.radCenter);
            this.groupBox1.Controls.Add(this.radTile);
            this.groupBox1.Location = new System.Drawing.Point(355, 44);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(88, 146);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Style";
            // 
            // radFill
            // 
            this.radFill.AutoSize = true;
            this.radFill.Location = new System.Drawing.Point(7, 116);
            this.radFill.Name = "radFill";
            this.radFill.Size = new System.Drawing.Size(37, 17);
            this.radFill.TabIndex = 4;
            this.radFill.Text = "Fill";
            this.radFill.UseVisualStyleBackColor = true;
            // 
            // radFit
            // 
            this.radFit.AutoSize = true;
            this.radFit.Location = new System.Drawing.Point(7, 92);
            this.radFit.Name = "radFit";
            this.radFit.Size = new System.Drawing.Size(36, 17);
            this.radFit.TabIndex = 3;
            this.radFit.Text = "Fit";
            this.radFit.UseVisualStyleBackColor = true;
            // 
            // radStretch
            // 
            this.radStretch.AutoSize = true;
            this.radStretch.Checked = true;
            this.radStretch.Location = new System.Drawing.Point(7, 68);
            this.radStretch.Name = "radStretch";
            this.radStretch.Size = new System.Drawing.Size(59, 17);
            this.radStretch.TabIndex = 2;
            this.radStretch.TabStop = true;
            this.radStretch.Text = "Stretch";
            this.radStretch.UseVisualStyleBackColor = true;
            // 
            // radCenter
            // 
            this.radCenter.AutoSize = true;
            this.radCenter.Location = new System.Drawing.Point(7, 44);
            this.radCenter.Name = "radCenter";
            this.radCenter.Size = new System.Drawing.Size(56, 17);
            this.radCenter.TabIndex = 1;
            this.radCenter.Text = "Center";
            this.radCenter.UseVisualStyleBackColor = true;
            // 
            // radTile
            // 
            this.radTile.AutoSize = true;
            this.radTile.Location = new System.Drawing.Point(7, 20);
            this.radTile.Name = "radTile";
            this.radTile.Size = new System.Drawing.Size(42, 17);
            this.radTile.TabIndex = 0;
            this.radTile.Text = "Tile";
            this.radTile.UseVisualStyleBackColor = true;
            // 
            // pctPreview
            // 
            this.pctPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pctPreview.Location = new System.Drawing.Point(16, 74);
            this.pctPreview.Name = "pctPreview";
            this.pctPreview.Size = new System.Drawing.Size(333, 239);
            this.pctPreview.TabIndex = 5;
            this.pctPreview.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Image Preview:";
            // 
            // btnSetWallpaper
            // 
            this.btnSetWallpaper.Location = new System.Drawing.Point(355, 289);
            this.btnSetWallpaper.Name = "btnSetWallpaper";
            this.btnSetWallpaper.Size = new System.Drawing.Size(88, 23);
            this.btnSetWallpaper.TabIndex = 7;
            this.btnSetWallpaper.Text = "Set Wallpaper";
            this.btnSetWallpaper.UseVisualStyleBackColor = true;
            this.btnSetWallpaper.Click += new System.EventHandler(this.btnSetWallpaper_Click);
            // 
            // wallpaperOpenFileDialog
            // 
            this.wallpaperOpenFileDialog.Filter = "\"All Picture Files |*.bmp;*.gif;*.jpg;*.png;*.tif\"";
            this.wallpaperOpenFileDialog.Title = "Select Wallpaper";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 325);
            this.Controls.Add(this.btnSetWallpaper);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pctPreview);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnBrowseWallpaper);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbWallpaperFileName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "CSSetDesktopWallpaper";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pctPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbWallpaperFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowseWallpaper;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radTile;
        private System.Windows.Forms.RadioButton radCenter;
        private System.Windows.Forms.RadioButton radStretch;
        private System.Windows.Forms.RadioButton radFit;
        private System.Windows.Forms.RadioButton radFill;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox pctPreview;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSetWallpaper;
        private System.Windows.Forms.OpenFileDialog wallpaperOpenFileDialog;
    }
}

