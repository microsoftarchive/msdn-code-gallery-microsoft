namespace NetworkSniffer
{
    partial class NetworkSnifferForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetworkSnifferForm));
            this.cbIpAddressList = new System.Windows.Forms.ComboBox();
            this.btnStartCapture = new System.Windows.Forms.Button();
            this.treeView = new System.Windows.Forms.TreeView();
            this.lbIpAddress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbIpAddressList
            // 
            this.cbIpAddressList.BackColor = System.Drawing.SystemColors.Info;
            this.cbIpAddressList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbIpAddressList.FormattingEnabled = true;
            this.cbIpAddressList.Location = new System.Drawing.Point(197, 207);
            this.cbIpAddressList.Name = "cbIpAddressList";
            this.cbIpAddressList.Size = new System.Drawing.Size(105, 21);
            this.cbIpAddressList.TabIndex = 0;
            // 
            // btnStartCapture
            // 
            this.btnStartCapture.BackColor = System.Drawing.Color.LightBlue;
            this.btnStartCapture.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStartCapture.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnStartCapture.Location = new System.Drawing.Point(12, 207);
            this.btnStartCapture.Name = "btnStartCapture";
            this.btnStartCapture.Size = new System.Drawing.Size(83, 21);
            this.btnStartCapture.TabIndex = 1;
            this.btnStartCapture.Text = "Start Capture";
            this.btnStartCapture.UseVisualStyleBackColor = false;
            this.btnStartCapture.Click += new System.EventHandler(this.btnStartCapture_Click);
            // 
            // treeView
            // 
            this.treeView.BackColor = System.Drawing.Color.DarkKhaki;
            this.treeView.Location = new System.Drawing.Point(12, 12);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(290, 179);
            this.treeView.TabIndex = 2;
            // 
            // lbIpAddress
            // 
            this.lbIpAddress.AutoSize = true;
            this.lbIpAddress.Location = new System.Drawing.Point(131, 211);
            this.lbIpAddress.Name = "lbIpAddress";
            this.lbIpAddress.Size = new System.Drawing.Size(64, 13);
            this.lbIpAddress.TabIndex = 3;
            this.lbIpAddress.Text = "IP Address :";
            // 
            // NetworkSnifferForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 244);
            this.Controls.Add(this.lbIpAddress);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.btnStartCapture);
            this.Controls.Add(this.cbIpAddressList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "NetworkSnifferForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Network Sniffer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NetworkSnifferForm_FormClosing);
            this.Load += new System.EventHandler(this.NetworkSnifferForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbIpAddressList;
        private System.Windows.Forms.Button btnStartCapture;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.Label lbIpAddress;
    }
}

