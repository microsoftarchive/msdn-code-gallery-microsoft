namespace CSWMIEnableDisableNetworkAdapter
{
    partial class ProgressInfoForm
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
            this.lbProgressInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbProgressInfo
            // 
            this.lbProgressInfo.Location = new System.Drawing.Point(12, 9);
            this.lbProgressInfo.Name = "lbProgressInfo";
            this.lbProgressInfo.Size = new System.Drawing.Size(366, 26);
            this.lbProgressInfo.TabIndex = 0;
            this.lbProgressInfo.Text = "label1";
            this.lbProgressInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ProgressInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Lime;
            this.ClientSize = new System.Drawing.Size(390, 45);
            this.Controls.Add(this.lbProgressInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ProgressInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.ProgressInfoLoad);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbProgressInfo;
    }
}