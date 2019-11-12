namespace Client
{
    partial class Client
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
            this.btnConnect = new System.Windows.Forms.Button();
            this.gbxServerInfo = new System.Windows.Forms.GroupBox();
            this.tbxPort = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.tbxAddress = new System.Windows.Forms.TextBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.tbxSavePath = new System.Windows.Forms.TextBox();
            this.btnSavePath = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.gbxServerInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(96, 186);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // gbxServerInfo
            // 
            this.gbxServerInfo.Controls.Add(this.tbxPort);
            this.gbxServerInfo.Controls.Add(this.lblPort);
            this.gbxServerInfo.Controls.Add(this.tbxAddress);
            this.gbxServerInfo.Controls.Add(this.lblAddress);
            this.gbxServerInfo.Location = new System.Drawing.Point(12, 24);
            this.gbxServerInfo.Name = "gbxServerInfo";
            this.gbxServerInfo.Size = new System.Drawing.Size(260, 88);
            this.gbxServerInfo.TabIndex = 1;
            this.gbxServerInfo.TabStop = false;
            this.gbxServerInfo.Text = "Server";
            // 
            // tbxPort
            // 
            this.tbxPort.Location = new System.Drawing.Point(59, 52);
            this.tbxPort.Name = "tbxPort";
            this.tbxPort.Size = new System.Drawing.Size(100, 20);
            this.tbxPort.TabIndex = 3;
            this.tbxPort.Text = "11000";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(6, 52);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(26, 13);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "Port";
            // 
            // tbxAddress
            // 
            this.tbxAddress.Location = new System.Drawing.Point(59, 20);
            this.tbxAddress.Name = "tbxAddress";
            this.tbxAddress.Size = new System.Drawing.Size(100, 20);
            this.tbxAddress.TabIndex = 1;
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(6, 20);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(45, 13);
            this.lblAddress.TabIndex = 0;
            this.lblAddress.Text = "Address";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 157);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(260, 23);
            this.progressBar.TabIndex = 2;
            // 
            // tbxSavePath
            // 
            this.tbxSavePath.Enabled = false;
            this.tbxSavePath.Location = new System.Drawing.Point(12, 130);
            this.tbxSavePath.Name = "tbxSavePath";
            this.tbxSavePath.Size = new System.Drawing.Size(179, 20);
            this.tbxSavePath.TabIndex = 3;
            // 
            // btnSavePath
            // 
            this.btnSavePath.Location = new System.Drawing.Point(197, 128);
            this.btnSavePath.Name = "btnSavePath";
            this.btnSavePath.Size = new System.Drawing.Size(75, 23);
            this.btnSavePath.TabIndex = 4;
            this.btnSavePath.Text = "Save To...";
            this.btnSavePath.UseVisualStyleBackColor = true;
            this.btnSavePath.Click += new System.EventHandler(this.btnSavePath_Click);
            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 231);
            this.Controls.Add(this.btnSavePath);
            this.Controls.Add(this.tbxSavePath);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.gbxServerInfo);
            this.Controls.Add(this.btnConnect);
            this.Name = "Client";
            this.Text = "Client";
            this.gbxServerInfo.ResumeLayout(false);
            this.gbxServerInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.GroupBox gbxServerInfo;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox tbxPort;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox tbxAddress;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox tbxSavePath;
        private System.Windows.Forms.Button btnSavePath;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

