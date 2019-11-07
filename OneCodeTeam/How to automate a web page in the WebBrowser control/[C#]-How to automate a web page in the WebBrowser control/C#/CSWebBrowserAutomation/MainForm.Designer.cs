namespace CSWebBrowserAutomation
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.tbUrl = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.pnlNavigate = new System.Windows.Forms.Panel();
            this.lbUrl = new System.Windows.Forms.Label();
            this.btnAutoComplete = new System.Windows.Forms.Button();
            this.webBrowser = new CSWebBrowserAutomation.WebBrowserEx();
            this.pnlHeader.SuspendLayout();
            this.pnlNavigate.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.tbUrl);
            this.pnlHeader.Controls.Add(this.btnGo);
            this.pnlHeader.Controls.Add(this.pnlNavigate);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1057, 32);
            this.pnlHeader.TabIndex = 0;
            // 
            // tbUrl
            // 
            this.tbUrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbUrl.Location = new System.Drawing.Point(152, 0);
            this.tbUrl.Name = "tbUrl";
            this.tbUrl.Size = new System.Drawing.Size(830, 27);
            this.tbUrl.TabIndex = 3;
            this.tbUrl.Text = "https://www.codeplex.com/site/login?RedirectUrl=https%3a%2f%2fwww.codeplex.com%2f" +
    "site%2fusers%2fupdate";
            // 
            // btnGo
            // 
            this.btnGo.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnGo.Location = new System.Drawing.Point(982, 0);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(75, 32);
            this.btnGo.TabIndex = 4;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // pnlNavigate
            // 
            this.pnlNavigate.Controls.Add(this.lbUrl);
            this.pnlNavigate.Controls.Add(this.btnAutoComplete);
            this.pnlNavigate.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlNavigate.Location = new System.Drawing.Point(0, 0);
            this.pnlNavigate.Name = "pnlNavigate";
            this.pnlNavigate.Size = new System.Drawing.Size(152, 32);
            this.pnlNavigate.TabIndex = 1;
            // 
            // lbUrl
            // 
            this.lbUrl.AutoSize = true;
            this.lbUrl.Location = new System.Drawing.Point(128, 9);
            this.lbUrl.Name = "lbUrl";
            this.lbUrl.Size = new System.Drawing.Size(20, 13);
            this.lbUrl.TabIndex = 1;
            this.lbUrl.Text = "Url";
            // 
            // btnAutoComplete
            // 
            this.btnAutoComplete.Enabled = false;
            this.btnAutoComplete.Location = new System.Drawing.Point(12, 4);
            this.btnAutoComplete.Name = "btnAutoComplete";
            this.btnAutoComplete.Size = new System.Drawing.Size(110, 23);
            this.btnAutoComplete.TabIndex = 0;
            this.btnAutoComplete.Text = "Auto Complete";
            this.btnAutoComplete.UseVisualStyleBackColor = true;
            this.btnAutoComplete.Click += new System.EventHandler(this.btnAutoComplete_Click);
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 32);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(1057, 483);
            this.webBrowser.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1057, 515);
            this.Controls.Add(this.webBrowser);
            this.Controls.Add(this.pnlHeader);
            this.Name = "MainForm";
            this.Text = "WebBrowserAutomation";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlNavigate.ResumeLayout(false);
            this.pnlNavigate.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.TextBox tbUrl;
        private System.Windows.Forms.Panel pnlNavigate;
        private System.Windows.Forms.Label lbUrl;
        private WebBrowserEx webBrowser;
        private System.Windows.Forms.Button btnAutoComplete;
        private System.Windows.Forms.Button btnGo;
    }
}

