namespace CSWMIEnableDisableNetworkAdapter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.grpNetworkAdapters = new System.Windows.Forms.GroupBox();
            this.stsMessage = new System.Windows.Forms.StatusStrip();
            this.tsslbResult = new System.Windows.Forms.ToolStripStatusLabel();
            this.stsMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpNetworkAdapters
            // 
            this.grpNetworkAdapters.BackColor = System.Drawing.SystemColors.ControlLightLight;
            resources.ApplyResources(this.grpNetworkAdapters, "grpNetworkAdapters");
            this.grpNetworkAdapters.Name = "grpNetworkAdapters";
            this.grpNetworkAdapters.TabStop = false;
            // 
            // stsMessage
            // 
            this.stsMessage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslbResult});
            resources.ApplyResources(this.stsMessage, "stsMessage");
            this.stsMessage.Name = "stsMessage";
            // 
            // tsslbResult
            // 
            this.tsslbResult.BackColor = System.Drawing.SystemColors.Control;
            this.tsslbResult.Name = "tsslbResult";
            resources.ApplyResources(this.tsslbResult, "tsslbResult");
            // 
            // EnableDisableNetworkAdapterForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.stsMessage);
            this.Controls.Add(this.grpNetworkAdapters);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "EnableDisableNetworkAdapterForm";
            this.stsMessage.ResumeLayout(false);
            this.stsMessage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpNetworkAdapters;
        private System.Windows.Forms.StatusStrip stsMessage;
        private System.Windows.Forms.ToolStripStatusLabel tsslbResult;
    }
}

