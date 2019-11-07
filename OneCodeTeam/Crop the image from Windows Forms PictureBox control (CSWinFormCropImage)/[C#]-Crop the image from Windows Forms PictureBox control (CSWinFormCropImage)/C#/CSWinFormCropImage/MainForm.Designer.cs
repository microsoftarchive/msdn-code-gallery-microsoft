namespace CSWinformCropImage
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
            this.SrcPicBox = new System.Windows.Forms.PictureBox();
            this.TargetPicBox = new System.Windows.Forms.PictureBox();
            this.BtnCrop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbCordinates = new System.Windows.Forms.Label();
            this.chkCropCordinates = new System.Windows.Forms.CheckBox();
            this.tbCordinates = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.SrcPicBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TargetPicBox)).BeginInit();
            this.SuspendLayout();
            // 
            // SrcPicBox
            // 
            this.SrcPicBox.Cursor = System.Windows.Forms.Cursors.Cross;
            this.SrcPicBox.Location = new System.Drawing.Point(45, 28);
            this.SrcPicBox.Name = "SrcPicBox";
            this.SrcPicBox.Size = new System.Drawing.Size(125, 125);
            this.SrcPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.SrcPicBox.TabIndex = 0;
            this.SrcPicBox.TabStop = false;
            this.SrcPicBox.Paint += new System.Windows.Forms.PaintEventHandler(this.SrcPicBox_Paint);
            this.SrcPicBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SrcPicBox_MouseDown);
            this.SrcPicBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SrcPicBox_MouseMove);
            this.SrcPicBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SrcPicBox_MouseUp);
            // 
            // TargetPicBox
            // 
            this.TargetPicBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TargetPicBox.Location = new System.Drawing.Point(45, 207);
            this.TargetPicBox.Name = "TargetPicBox";
            this.TargetPicBox.Size = new System.Drawing.Size(250, 250);
            this.TargetPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.TargetPicBox.TabIndex = 1;
            this.TargetPicBox.TabStop = false;
            // 
            // BtnCrop
            // 
            this.BtnCrop.Location = new System.Drawing.Point(233, 28);
            this.BtnCrop.Name = "BtnCrop";
            this.BtnCrop.Size = new System.Drawing.Size(87, 24);
            this.BtnCrop.TabIndex = 2;
            this.BtnCrop.Text = "Crop";
            this.BtnCrop.UseVisualStyleBackColor = true;
            this.BtnCrop.Click += new System.EventHandler(this.BtnCrop_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(42, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 3;
            this.label1.Text = "Source Picture Box";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(42, 179);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "Destination Picture Box";
            // 
            // lbCordinates
            // 
            this.lbCordinates.AutoSize = true;
            this.lbCordinates.Location = new System.Drawing.Point(230, 69);
            this.lbCordinates.Name = "lbCordinates";
            this.lbCordinates.Size = new System.Drawing.Size(66, 13);
            this.lbCordinates.TabIndex = 5;
            this.lbCordinates.Text = "Coordinates:";
            // 
            // chkCropCordinates
            // 
            this.chkCropCordinates.AutoSize = true;
            this.chkCropCordinates.Location = new System.Drawing.Point(233, 96);
            this.chkCropCordinates.Name = "chkCropCordinates";
            this.chkCropCordinates.Size = new System.Drawing.Size(202, 17);
            this.chkCropCordinates.TabIndex = 6;
            this.chkCropCordinates.Text = "Crop using coordinates ( x1,y1,x2,y2 )";
            this.chkCropCordinates.UseVisualStyleBackColor = true;
            this.chkCropCordinates.CheckedChanged += new System.EventHandler(this.chkCropCordinates_CheckedChanged);
            // 
            // tbCordinates
            // 
            this.tbCordinates.Location = new System.Drawing.Point(233, 133);
            this.tbCordinates.Name = "tbCordinates";
            this.tbCordinates.Size = new System.Drawing.Size(140, 20);
            this.tbCordinates.TabIndex = 7;
            this.tbCordinates.Visible = false;
            this.tbCordinates.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbCordinates_KeyPress);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 476);
            this.Controls.Add(this.tbCordinates);
            this.Controls.Add(this.chkCropCordinates);
            this.Controls.Add(this.lbCordinates);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnCrop);
            this.Controls.Add(this.TargetPicBox);
            this.Controls.Add(this.SrcPicBox);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "CSWinformCropImage";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.SrcPicBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TargetPicBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox SrcPicBox;
        private System.Windows.Forms.PictureBox TargetPicBox;
        private System.Windows.Forms.Button BtnCrop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbCordinates;
        private System.Windows.Forms.CheckBox chkCropCordinates;
        private System.Windows.Forms.TextBox tbCordinates;
    }
}

