namespace CSWMIEnableDisableNetworkAdapter
{
    partial class UcNetworkAdapter
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
            this.pctNetworkAdapter = new System.Windows.Forms.PictureBox();
            this.lbProductName = new System.Windows.Forms.Label();
            this.lbConnectionStatus = new System.Windows.Forms.Label();
            this.btnEnableDisable = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pctNetworkAdapter)).BeginInit();
            this.SuspendLayout();
            // 
            // pctNetworkAdapter
            // 
            this.pctNetworkAdapter.Location = new System.Drawing.Point(5, 8);
            this.pctNetworkAdapter.Name = "pctNetworkAdapter";
            this.pctNetworkAdapter.Size = new System.Drawing.Size(15, 15);
            this.pctNetworkAdapter.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pctNetworkAdapter.TabIndex = 0;
            this.pctNetworkAdapter.TabStop = false;
            // 
            // lbProductName
            // 
            this.lbProductName.Location = new System.Drawing.Point(25, 4);
            this.lbProductName.Name = "lbProductName";
            this.lbProductName.Size = new System.Drawing.Size(419, 22);
            this.lbProductName.TabIndex = 1;
            this.lbProductName.Text = "label1";
            this.lbProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbConnectionStatus
            // 
            this.lbConnectionStatus.Location = new System.Drawing.Point(450, 5);
            this.lbConnectionStatus.Name = "lbConnectionStatus";
            this.lbConnectionStatus.Size = new System.Drawing.Size(132, 22);
            this.lbConnectionStatus.TabIndex = 2;
            this.lbConnectionStatus.Text = "label2";
            this.lbConnectionStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnEnableDisable
            // 
            this.btnEnableDisable.Location = new System.Drawing.Point(601, 3);
            this.btnEnableDisable.Name = "btnEnableDisable";
            this.btnEnableDisable.Size = new System.Drawing.Size(60, 22);
            this.btnEnableDisable.TabIndex = 3;
            this.btnEnableDisable.Text = "button1";
            this.btnEnableDisable.UseVisualStyleBackColor = true;
            // 
            // UCNetworkAdapter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnEnableDisable);
            this.Controls.Add(this.lbConnectionStatus);
            this.Controls.Add(this.lbProductName);
            this.Controls.Add(this.pctNetworkAdapter);
            this.Name = "UcNetworkAdapter";
            this.Size = new System.Drawing.Size(670, 30);
            ((System.ComponentModel.ISupportInitialize)(this.pctNetworkAdapter)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pctNetworkAdapter;
        private System.Windows.Forms.Label lbProductName;
        private System.Windows.Forms.Label lbConnectionStatus;
        private System.Windows.Forms.Button btnEnableDisable;

    }
}
