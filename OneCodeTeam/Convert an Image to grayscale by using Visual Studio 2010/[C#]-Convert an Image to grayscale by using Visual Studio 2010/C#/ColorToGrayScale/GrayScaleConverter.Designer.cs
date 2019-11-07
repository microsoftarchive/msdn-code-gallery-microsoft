namespace ColorToGrayScale
{
    partial class GrayScaleConverterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GrayScaleConverterForm));
            this.mnuActionMenu = new System.Windows.Forms.MenuStrip();
            this.mnuChooseFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuConvert = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSaveOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReset = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuActionMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuActionMenu
            // 
            this.mnuActionMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuChooseFile,
            this.mnuConvert,
            this.mnuSaveOutput,
            this.mnuReset});
            this.mnuActionMenu.Location = new System.Drawing.Point(0, 0);
            this.mnuActionMenu.Name = "mnuActionMenu";
            this.mnuActionMenu.Size = new System.Drawing.Size(302, 24);
            this.mnuActionMenu.TabIndex = 0;
            this.mnuActionMenu.Text = "menuStrip1";
            // 
            // mnuChooseFile
            // 
            this.mnuChooseFile.Name = "mnuChooseFile";
            this.mnuChooseFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
            this.mnuChooseFile.Size = new System.Drawing.Size(89, 20);
            this.mnuChooseFile.Text = "Choose File...";
            this.mnuChooseFile.Click += new System.EventHandler(this.mnuChooseFile_Click);
            // 
            // mnuConvert
            // 
            this.mnuConvert.Name = "mnuConvert";
            this.mnuConvert.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
            this.mnuConvert.Size = new System.Drawing.Size(61, 20);
            this.mnuConvert.Text = "Convert";
            this.mnuConvert.Click += new System.EventHandler(this.mnuConvert_Click);
            // 
            // mnuSaveOutput
            // 
            this.mnuSaveOutput.Name = "mnuSaveOutput";
            this.mnuSaveOutput.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.mnuSaveOutput.Size = new System.Drawing.Size(84, 20);
            this.mnuSaveOutput.Text = "Save Output";
            this.mnuSaveOutput.Click += new System.EventHandler(this.mnuSaveOutput_Click);
            // 
            // mnuReset
            // 
            this.mnuReset.Name = "mnuReset";
            this.mnuReset.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.R)));
            this.mnuReset.Size = new System.Drawing.Size(47, 20);
            this.mnuReset.Text = "Reset";
            this.mnuReset.Click += new System.EventHandler(this.mnuReset_Click);
            // 
            // GrayScaleConverterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 261);
            this.Controls.Add(this.mnuActionMenu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mnuActionMenu;
            this.Name = "GrayScaleConverterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Image Converter";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.GrayScaleConverterForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.GrayScaleConverterForm_Paint);
            this.mnuActionMenu.ResumeLayout(false);
            this.mnuActionMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuActionMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuChooseFile;
        private System.Windows.Forms.ToolStripMenuItem mnuConvert;
        private System.Windows.Forms.ToolStripMenuItem mnuSaveOutput;
        private System.Windows.Forms.ToolStripMenuItem mnuReset;
    }
}

