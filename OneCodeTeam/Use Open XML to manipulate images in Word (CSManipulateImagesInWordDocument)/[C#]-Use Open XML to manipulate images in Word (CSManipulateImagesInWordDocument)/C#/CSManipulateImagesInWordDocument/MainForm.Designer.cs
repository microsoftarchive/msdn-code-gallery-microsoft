namespace CSManipulateImagesInWordDocument
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

            if (disposing && (documentManipulator != null))
            {
                documentManipulator.Dispose();
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
            this.pnlBrowse = new System.Windows.Forms.Panel();
            this.lbFileName = new System.Windows.Forms.Label();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.pnlImageList = new System.Windows.Forms.Panel();
            this.lstImage = new System.Windows.Forms.ListBox();
            this.pnlOperation = new System.Windows.Forms.Panel();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnReplace = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.picView = new System.Windows.Forms.PictureBox();
            this.pnlBrowse.SuspendLayout();
            this.pnlImageList.SuspendLayout();
            this.pnlOperation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picView)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlBrowse
            // 
            this.pnlBrowse.Controls.Add(this.lbFileName);
            this.pnlBrowse.Controls.Add(this.btnOpenFile);
            this.pnlBrowse.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBrowse.Location = new System.Drawing.Point(0, 0);
            this.pnlBrowse.Name = "pnlBrowse";
            this.pnlBrowse.Size = new System.Drawing.Size(783, 37);
            this.pnlBrowse.TabIndex = 0;
            // 
            // lbFileName
            // 
            this.lbFileName.AutoSize = true;
            this.lbFileName.Location = new System.Drawing.Point(143, 13);
            this.lbFileName.Name = "lbFileName";
            this.lbFileName.Size = new System.Drawing.Size(0, 13);
            this.lbFileName.TabIndex = 2;
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(12, 8);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(124, 23);
            this.btnOpenFile.TabIndex = 1;
            this.btnOpenFile.Text = "Open the word doc";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // pnlImageList
            // 
            this.pnlImageList.Controls.Add(this.lstImage);
            this.pnlImageList.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlImageList.Location = new System.Drawing.Point(0, 37);
            this.pnlImageList.Name = "pnlImageList";
            this.pnlImageList.Size = new System.Drawing.Size(269, 431);
            this.pnlImageList.TabIndex = 1;
            // 
            // lstImage
            // 
            this.lstImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstImage.FormattingEnabled = true;
            this.lstImage.Location = new System.Drawing.Point(0, 0);
            this.lstImage.Name = "lstImage";
            this.lstImage.Size = new System.Drawing.Size(269, 431);
            this.lstImage.TabIndex = 0;
            this.lstImage.SelectedIndexChanged += new System.EventHandler(this.lstImage_SelectedIndexChanged);
            // 
            // pnlOperation
            // 
            this.pnlOperation.Controls.Add(this.btnExport);
            this.pnlOperation.Controls.Add(this.btnReplace);
            this.pnlOperation.Controls.Add(this.btnDelete);
            this.pnlOperation.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlOperation.Location = new System.Drawing.Point(269, 37);
            this.pnlOperation.Name = "pnlOperation";
            this.pnlOperation.Size = new System.Drawing.Size(514, 35);
            this.pnlOperation.TabIndex = 2;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(33, 5);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnReplace
            // 
            this.btnReplace.Location = new System.Drawing.Point(218, 5);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(75, 23);
            this.btnReplace.TabIndex = 1;
            this.btnReplace.Text = "Replace";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(125, 5);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 0;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // picView
            // 
            this.picView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picView.Location = new System.Drawing.Point(269, 72);
            this.picView.Name = "picView";
            this.picView.Size = new System.Drawing.Size(514, 396);
            this.picView.TabIndex = 3;
            this.picView.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(783, 468);
            this.Controls.Add(this.picView);
            this.Controls.Add(this.pnlOperation);
            this.Controls.Add(this.pnlImageList);
            this.Controls.Add(this.pnlBrowse);
            this.Name = "MainForm";
            this.Text = "CSManipulateImagesInWordDocument";
            this.pnlBrowse.ResumeLayout(false);
            this.pnlBrowse.PerformLayout();
            this.pnlImageList.ResumeLayout(false);
            this.pnlOperation.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlBrowse;
        private System.Windows.Forms.Label lbFileName;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.Panel pnlImageList;
        private System.Windows.Forms.ListBox lstImage;
        private System.Windows.Forms.Panel pnlOperation;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.PictureBox picView;
        private System.Windows.Forms.Button btnExport;
    }
}

