namespace IEProxyModifier
{
    partial class IEProxyModifierForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IEProxyModifierForm));
            this.grpProxyDetails = new System.Windows.Forms.GroupBox();
            this.cmbProxyStatusInfo = new System.Windows.Forms.ComboBox();
            this.lbProxyStatus = new System.Windows.Forms.Label();
            this.tbProxyByPass = new System.Windows.Forms.TextBox();
            this.tbProxyServer = new System.Windows.Forms.TextBox();
            this.cmbAccessType = new System.Windows.Forms.ComboBox();
            this.lbProxyByPass = new System.Windows.Forms.Label();
            this.lbProxyServer = new System.Windows.Forms.Label();
            this.lbAccessType = new System.Windows.Forms.Label();
            this.btnGetProxy = new System.Windows.Forms.Button();
            this.btnSetProxy = new System.Windows.Forms.Button();
            this.grpProxyDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpProxyDetails
            // 
            this.grpProxyDetails.Controls.Add(this.cmbProxyStatusInfo);
            this.grpProxyDetails.Controls.Add(this.lbProxyStatus);
            this.grpProxyDetails.Controls.Add(this.tbProxyByPass);
            this.grpProxyDetails.Controls.Add(this.tbProxyServer);
            this.grpProxyDetails.Controls.Add(this.cmbAccessType);
            this.grpProxyDetails.Controls.Add(this.lbProxyByPass);
            this.grpProxyDetails.Controls.Add(this.lbProxyServer);
            this.grpProxyDetails.Controls.Add(this.lbAccessType);
            this.grpProxyDetails.Location = new System.Drawing.Point(13, 13);
            this.grpProxyDetails.Name = "grpProxyDetails";
            this.grpProxyDetails.Size = new System.Drawing.Size(237, 220);
            this.grpProxyDetails.TabIndex = 0;
            this.grpProxyDetails.TabStop = false;
            this.grpProxyDetails.Text = "Proxy Details";
            // 
            // cmbProxyStatusInfo
            // 
            this.cmbProxyStatusInfo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProxyStatusInfo.FormattingEnabled = true;
            this.cmbProxyStatusInfo.Items.AddRange(new object[] {
            "FALSE",
            "TRUE"});
            this.cmbProxyStatusInfo.Location = new System.Drawing.Point(102, 180);
            this.cmbProxyStatusInfo.Name = "cmbProxyStatusInfo";
            this.cmbProxyStatusInfo.Size = new System.Drawing.Size(115, 21);
            this.cmbProxyStatusInfo.TabIndex = 7;
            // 
            // lbProxyStatus
            // 
            this.lbProxyStatus.AutoSize = true;
            this.lbProxyStatus.Location = new System.Drawing.Point(17, 183);
            this.lbProxyStatus.Name = "lbProxyStatus";
            this.lbProxyStatus.Size = new System.Drawing.Size(66, 13);
            this.lbProxyStatus.TabIndex = 6;
            this.lbProxyStatus.Text = "Proxy Status";
            // 
            // tbProxyByPass
            // 
            this.tbProxyByPass.Location = new System.Drawing.Point(102, 129);
            this.tbProxyByPass.Name = "tbProxyByPass";
            this.tbProxyByPass.Size = new System.Drawing.Size(115, 20);
            this.tbProxyByPass.TabIndex = 5;
            // 
            // tbProxyServer
            // 
            this.tbProxyServer.Location = new System.Drawing.Point(102, 78);
            this.tbProxyServer.Name = "tbProxyServer";
            this.tbProxyServer.Size = new System.Drawing.Size(115, 20);
            this.tbProxyServer.TabIndex = 4;
            // 
            // cmbAccessType
            // 
            this.cmbAccessType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccessType.FormattingEnabled = true;
            this.cmbAccessType.Items.AddRange(new object[] {
            "PRECONFIG",
            "DIRECT",
            "PROXY"});
            this.cmbAccessType.Location = new System.Drawing.Point(102, 27);
            this.cmbAccessType.Name = "cmbAccessType";
            this.cmbAccessType.Size = new System.Drawing.Size(115, 21);
            this.cmbAccessType.TabIndex = 3;
            // 
            // lbProxyByPass
            // 
            this.lbProxyByPass.AutoSize = true;
            this.lbProxyByPass.Location = new System.Drawing.Point(17, 132);
            this.lbProxyByPass.Name = "lbProxyByPass";
            this.lbProxyByPass.Size = new System.Drawing.Size(71, 13);
            this.lbProxyByPass.TabIndex = 2;
            this.lbProxyByPass.Text = "Proxy ByPass";
            // 
            // lbProxyServer
            // 
            this.lbProxyServer.AutoSize = true;
            this.lbProxyServer.Location = new System.Drawing.Point(17, 81);
            this.lbProxyServer.Name = "lbProxyServer";
            this.lbProxyServer.Size = new System.Drawing.Size(67, 13);
            this.lbProxyServer.TabIndex = 1;
            this.lbProxyServer.Text = "Proxy Server";
            // 
            // lbAccessType
            // 
            this.lbAccessType.AutoSize = true;
            this.lbAccessType.Location = new System.Drawing.Point(17, 30);
            this.lbAccessType.Name = "lbAccessType";
            this.lbAccessType.Size = new System.Drawing.Size(69, 13);
            this.lbAccessType.TabIndex = 0;
            this.lbAccessType.Text = "Access Type";
            // 
            // btnGetProxy
            // 
            this.btnGetProxy.Location = new System.Drawing.Point(45, 248);
            this.btnGetProxy.Name = "btnGetProxy";
            this.btnGetProxy.Size = new System.Drawing.Size(75, 23);
            this.btnGetProxy.TabIndex = 1;
            this.btnGetProxy.Text = "Get Proxy";
            this.btnGetProxy.UseVisualStyleBackColor = true;
            this.btnGetProxy.Click += new System.EventHandler(this.btnGetProxy_Click);
            // 
            // btnSetProxy
            // 
            this.btnSetProxy.Location = new System.Drawing.Point(150, 248);
            this.btnSetProxy.Name = "btnSetProxy";
            this.btnSetProxy.Size = new System.Drawing.Size(75, 23);
            this.btnSetProxy.TabIndex = 2;
            this.btnSetProxy.Text = "Set Proxy";
            this.btnSetProxy.UseVisualStyleBackColor = true;
            this.btnSetProxy.Click += new System.EventHandler(this.btnSetProxy_Click);
            // 
            // IEProxyModifierForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(265, 285);
            this.Controls.Add(this.btnSetProxy);
            this.Controls.Add(this.btnGetProxy);
            this.Controls.Add(this.grpProxyDetails);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "IEProxyModifierForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Proxy Modifier";
            this.Load += new System.EventHandler(this.IEProxyModifierForm_Load);
            this.grpProxyDetails.ResumeLayout(false);
            this.grpProxyDetails.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpProxyDetails;
        private System.Windows.Forms.TextBox tbProxyByPass;
        private System.Windows.Forms.TextBox tbProxyServer;
        private System.Windows.Forms.ComboBox cmbAccessType;
        private System.Windows.Forms.Label lbProxyByPass;
        private System.Windows.Forms.Label lbProxyServer;
        private System.Windows.Forms.Label lbAccessType;
        private System.Windows.Forms.Button btnGetProxy;
        private System.Windows.Forms.Button btnSetProxy;
        private System.Windows.Forms.Label lbProxyStatus;
        private System.Windows.Forms.ComboBox cmbProxyStatusInfo;

    }
}

