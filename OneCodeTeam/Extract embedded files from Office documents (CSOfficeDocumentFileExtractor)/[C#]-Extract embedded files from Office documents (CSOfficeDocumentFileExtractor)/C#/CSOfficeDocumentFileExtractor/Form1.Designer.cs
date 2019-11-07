namespace CSOfficeDocumentFileExtractor
{
    partial class Form1
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
            this.chkdLstEmbeddedFiles = new System.Windows.Forms.CheckedListBox();
            this.btnExtractSelectedFiles = new System.Windows.Forms.Button();
            this.lblSourceFile = new System.Windows.Forms.Label();
            this.txtSourceFile = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnScan = new System.Windows.Forms.Button();
            this.lblExtractTo = new System.Windows.Forms.Label();
            this.txtDestinationFolder = new System.Windows.Forms.TextBox();
            this.btnBrowseFolder = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkdLstEmbeddedFiles
            // 
            this.chkdLstEmbeddedFiles.CheckOnClick = true;
            this.chkdLstEmbeddedFiles.FormattingEnabled = true;
            this.chkdLstEmbeddedFiles.Location = new System.Drawing.Point(199, 15);
            this.chkdLstEmbeddedFiles.Name = "chkdLstEmbeddedFiles";
            this.chkdLstEmbeddedFiles.Size = new System.Drawing.Size(215, 244);
            this.chkdLstEmbeddedFiles.TabIndex = 0;
            // 
            // btnExtractSelectedFiles
            // 
            this.btnExtractSelectedFiles.Location = new System.Drawing.Point(265, 431);
            this.btnExtractSelectedFiles.Name = "btnExtractSelectedFiles";
            this.btnExtractSelectedFiles.Size = new System.Drawing.Size(102, 42);
            this.btnExtractSelectedFiles.TabIndex = 1;
            this.btnExtractSelectedFiles.Text = "Extract Selected Files";
            this.btnExtractSelectedFiles.UseVisualStyleBackColor = true;
            this.btnExtractSelectedFiles.Click += new System.EventHandler(this.btnExtractSelectedFiles_Click);
            // 
            // lblSourceFile
            // 
            this.lblSourceFile.AutoSize = true;
            this.lblSourceFile.Location = new System.Drawing.Point(15, 22);
            this.lblSourceFile.Name = "lblSourceFile";
            this.lblSourceFile.Size = new System.Drawing.Size(60, 13);
            this.lblSourceFile.TabIndex = 2;
            this.lblSourceFile.Text = "Source File";
            // 
            // txtSourceFile
            // 
            this.txtSourceFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSourceFile.Location = new System.Drawing.Point(81, 17);
            this.txtSourceFile.Name = "txtSourceFile";
            this.txtSourceFile.Size = new System.Drawing.Size(446, 21);
            this.txtSourceFile.TabIndex = 3;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(539, 16);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(0);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(30, 25);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = ". . .";
            this.btnBrowse.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkdLstEmbeddedFiles);
            this.groupBox1.Location = new System.Drawing.Point(12, 93);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(595, 265);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(265, 45);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(75, 42);
            this.btnScan.TabIndex = 6;
            this.btnScan.Text = "Scan";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // lblExtractTo
            // 
            this.lblExtractTo.AutoSize = true;
            this.lblExtractTo.Location = new System.Drawing.Point(15, 391);
            this.lblExtractTo.Name = "lblExtractTo";
            this.lblExtractTo.Size = new System.Drawing.Size(56, 13);
            this.lblExtractTo.TabIndex = 7;
            this.lblExtractTo.Text = "Extract To";
            // 
            // txtDestinationFolder
            // 
            this.txtDestinationFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDestinationFolder.Location = new System.Drawing.Point(81, 387);
            this.txtDestinationFolder.Name = "txtDestinationFolder";
            this.txtDestinationFolder.Size = new System.Drawing.Size(446, 21);
            this.txtDestinationFolder.TabIndex = 8;
            this.txtDestinationFolder.Text = "C:\\Temp";
            // 
            // btnBrowseFolder
            // 
            this.btnBrowseFolder.Location = new System.Drawing.Point(539, 384);
            this.btnBrowseFolder.Name = "btnBrowseFolder";
            this.btnBrowseFolder.Size = new System.Drawing.Size(30, 25);
            this.btnBrowseFolder.TabIndex = 9;
            this.btnBrowseFolder.Text = ". . .";
            this.btnBrowseFolder.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnBrowseFolder.UseVisualStyleBackColor = true;
            this.btnBrowseFolder.Click += new System.EventHandler(this.btnBrowseFolder_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 521);
            this.Controls.Add(this.btnBrowseFolder);
            this.Controls.Add(this.txtDestinationFolder);
            this.Controls.Add(this.lblExtractTo);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtSourceFile);
            this.Controls.Add(this.lblSourceFile);
            this.Controls.Add(this.btnExtractSelectedFiles);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox chkdLstEmbeddedFiles;
        private System.Windows.Forms.Button btnExtractSelectedFiles;
        private System.Windows.Forms.Label lblSourceFile;
        private System.Windows.Forms.TextBox txtSourceFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.Label lblExtractTo;
        private System.Windows.Forms.TextBox txtDestinationFolder;
        private System.Windows.Forms.Button btnBrowseFolder;
    }
}

