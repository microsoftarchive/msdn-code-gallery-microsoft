namespace CSOpenXmlInsertImageToPPT
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
            this.grbPowerPoint = new System.Windows.Forms.GroupBox();
            this.btnOpenPPt = new System.Windows.Forms.Button();
            this.txbPPtPath = new System.Windows.Forms.TextBox();
            this.lblSourcePpt = new System.Windows.Forms.Label();
            this.grbImage = new System.Windows.Forms.GroupBox();
            this.btnInsert = new System.Windows.Forms.Button();
            this.btnSelectImg = new System.Windows.Forms.Button();
            this.txbImagePath = new System.Windows.Forms.TextBox();
            this.lblSourceImg = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.grbPowerPoint.SuspendLayout();
            this.grbImage.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbPowerPoint
            // 
            this.grbPowerPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbPowerPoint.Controls.Add(this.btnOpenPPt);
            this.grbPowerPoint.Controls.Add(this.txbPPtPath);
            this.grbPowerPoint.Controls.Add(this.lblSourcePpt);
            this.grbPowerPoint.Location = new System.Drawing.Point(13, 13);
            this.grbPowerPoint.Name = "grbPowerPoint";
            this.grbPowerPoint.Size = new System.Drawing.Size(432, 71);
            this.grbPowerPoint.TabIndex = 0;
            this.grbPowerPoint.TabStop = false;
            this.grbPowerPoint.Text = "Source PowerPoint File";
            // 
            // btnOpenPPt
            // 
            this.btnOpenPPt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenPPt.Location = new System.Drawing.Point(361, 23);
            this.btnOpenPPt.Name = "btnOpenPPt";
            this.btnOpenPPt.Size = new System.Drawing.Size(65, 23);
            this.btnOpenPPt.TabIndex = 2;
            this.btnOpenPPt.Text = "Open";
            this.btnOpenPPt.UseVisualStyleBackColor = true;
            this.btnOpenPPt.Click += new System.EventHandler(this.btnOpenPPt_Click);
            // 
            // txbPPtPath
            // 
            this.txbPPtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbPPtPath.Location = new System.Drawing.Point(126, 26);
            this.txbPPtPath.Name = "txbPPtPath";
            this.txbPPtPath.Size = new System.Drawing.Size(229, 20);
            this.txbPPtPath.TabIndex = 1;
            // 
            // lblSourcePpt
            // 
            this.lblSourcePpt.AutoSize = true;
            this.lblSourcePpt.Location = new System.Drawing.Point(7, 30);
            this.lblSourcePpt.Name = "lblSourcePpt";
            this.lblSourcePpt.Size = new System.Drawing.Size(115, 13);
            this.lblSourcePpt.TabIndex = 0;
            this.lblSourcePpt.Text = "PPTX Document Path:";
            // 
            // grbImage
            // 
            this.grbImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbImage.Controls.Add(this.btnInsert);
            this.grbImage.Controls.Add(this.btnSelectImg);
            this.grbImage.Controls.Add(this.txbImagePath);
            this.grbImage.Controls.Add(this.lblSourceImg);
            this.grbImage.Controls.Add(this.label2);
            this.grbImage.Location = new System.Drawing.Point(13, 102);
            this.grbImage.Name = "grbImage";
            this.grbImage.Size = new System.Drawing.Size(432, 103);
            this.grbImage.TabIndex = 1;
            this.grbImage.TabStop = false;
            this.grbImage.Text = "Source Image";
            // 
            // btnInsert
            // 
            this.btnInsert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnInsert.Location = new System.Drawing.Point(10, 66);
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.Size = new System.Drawing.Size(64, 23);
            this.btnInsert.TabIndex = 4;
            this.btnInsert.Text = "Insert";
            this.btnInsert.UseVisualStyleBackColor = true;
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // btnSelectImg
            // 
            this.btnSelectImg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectImg.Location = new System.Drawing.Point(361, 30);
            this.btnSelectImg.Name = "btnSelectImg";
            this.btnSelectImg.Size = new System.Drawing.Size(64, 23);
            this.btnSelectImg.TabIndex = 3;
            this.btnSelectImg.Text = "Select";
            this.btnSelectImg.UseVisualStyleBackColor = true;
            this.btnSelectImg.Click += new System.EventHandler(this.btnSelectImg_Click);
            // 
            // txbImagePath
            // 
            this.txbImagePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbImagePath.Location = new System.Drawing.Point(122, 32);
            this.txbImagePath.Name = "txbImagePath";
            this.txbImagePath.Size = new System.Drawing.Size(233, 20);
            this.txbImagePath.TabIndex = 2;
            // 
            // lblSourceImg
            // 
            this.lblSourceImg.AutoSize = true;
            this.lblSourceImg.Location = new System.Drawing.Point(7, 35);
            this.lblSourceImg.Name = "lblSourceImg";
            this.lblSourceImg.Size = new System.Drawing.Size(109, 13);
            this.lblSourceImg.TabIndex = 1;
            this.lblSourceImg.Text = "Inserted Picture Path:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(457, 240);
            this.Controls.Add(this.grbImage);
            this.Controls.Add(this.grbPowerPoint);
            this.MinimumSize = new System.Drawing.Size(400, 260);
            this.Name = "MainForm";
            this.Text = "Insert Image into PowerPoint Form";
            this.grbPowerPoint.ResumeLayout(false);
            this.grbPowerPoint.PerformLayout();
            this.grbImage.ResumeLayout(false);
            this.grbImage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbPowerPoint;
        private System.Windows.Forms.Button btnOpenPPt;
        private System.Windows.Forms.TextBox txbPPtPath;
        private System.Windows.Forms.Label lblSourcePpt;
        private System.Windows.Forms.GroupBox grbImage;
        private System.Windows.Forms.Button btnInsert;
        private System.Windows.Forms.Button btnSelectImg;
        private System.Windows.Forms.TextBox txbImagePath;
        private System.Windows.Forms.Label lblSourceImg;
        private System.Windows.Forms.Label label2;
    }
}

