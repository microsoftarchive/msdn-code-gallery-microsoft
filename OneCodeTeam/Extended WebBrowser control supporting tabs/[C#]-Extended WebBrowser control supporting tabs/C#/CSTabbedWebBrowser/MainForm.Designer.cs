namespace CSTabbedWebBrowser
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
            this.pnlNavigate = new System.Windows.Forms.Panel();
            this.lbUrl = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnForward = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.pnlTabCommand = new System.Windows.Forms.Panel();
            this.lbGo = new System.Windows.Forms.Label();
            this.chkEnableTab = new System.Windows.Forms.CheckBox();
            this.btnCloseTab = new System.Windows.Forms.Button();
            this.btnNewTab = new System.Windows.Forms.Button();
            this.webBrowserContainer = new CSTabbedWebBrowser.TabbedWebBrowserContainer();
            this.pnlHeader.SuspendLayout();
            this.pnlNavigate.SuspendLayout();
            this.pnlTabCommand.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.tbUrl);
            this.pnlHeader.Controls.Add(this.pnlNavigate);
            this.pnlHeader.Controls.Add(this.pnlTabCommand);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(928, 30);
            this.pnlHeader.TabIndex = 0;
            // 
            // tbUrl
            // 
            this.tbUrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbUrl.Location = new System.Drawing.Point(193, 0);
            this.tbUrl.Name = "tbUrl";
            this.tbUrl.Size = new System.Drawing.Size(342, 27);
            this.tbUrl.TabIndex = 2;
            this.tbUrl.Text = "http://1code.codeplex.com/";
            this.tbUrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbUrl_KeyDown);
            // 
            // pnlNavigate
            // 
            this.pnlNavigate.Controls.Add(this.lbUrl);
            this.pnlNavigate.Controls.Add(this.btnRefresh);
            this.pnlNavigate.Controls.Add(this.btnForward);
            this.pnlNavigate.Controls.Add(this.btnBack);
            this.pnlNavigate.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlNavigate.Location = new System.Drawing.Point(0, 0);
            this.pnlNavigate.Name = "pnlNavigate";
            this.pnlNavigate.Size = new System.Drawing.Size(193, 30);
            this.pnlNavigate.TabIndex = 0;
            // 
            // lbUrl
            // 
            this.lbUrl.AutoSize = true;
            this.lbUrl.Location = new System.Drawing.Point(171, 8);
            this.lbUrl.Name = "lbUrl";
            this.lbUrl.Size = new System.Drawing.Size(20, 13);
            this.lbUrl.TabIndex = 1;
            this.lbUrl.Text = "Url";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(113, 4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(52, 23);
            this.btnRefresh.TabIndex = 0;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnForward
            // 
            this.btnForward.Location = new System.Drawing.Point(54, 4);
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(53, 23);
            this.btnForward.TabIndex = 0;
            this.btnForward.Text = "Forward";
            this.btnForward.UseVisualStyleBackColor = true;
            this.btnForward.Click += new System.EventHandler(this.btnForward_Click);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(3, 4);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(45, 23);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // pnlTabCommand
            // 
            this.pnlTabCommand.Controls.Add(this.lbGo);
            this.pnlTabCommand.Controls.Add(this.chkEnableTab);
            this.pnlTabCommand.Controls.Add(this.btnCloseTab);
            this.pnlTabCommand.Controls.Add(this.btnNewTab);
            this.pnlTabCommand.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlTabCommand.Location = new System.Drawing.Point(535, 0);
            this.pnlTabCommand.Name = "pnlTabCommand";
            this.pnlTabCommand.Size = new System.Drawing.Size(393, 30);
            this.pnlTabCommand.TabIndex = 3;
            // 
            // lbGo
            // 
            this.lbGo.AutoSize = true;
            this.lbGo.Location = new System.Drawing.Point(4, 9);
            this.lbGo.Name = "lbGo";
            this.lbGo.Size = new System.Drawing.Size(137, 13);
            this.lbGo.TabIndex = 3;
            this.lbGo.Text = "Press Enter to visit the URL";
            // 
            // chkEnableTab
            // 
            this.chkEnableTab.AutoSize = true;
            this.chkEnableTab.Location = new System.Drawing.Point(147, 8);
            this.chkEnableTab.Name = "chkEnableTab";
            this.chkEnableTab.Size = new System.Drawing.Size(81, 17);
            this.chkEnableTab.TabIndex = 1;
            this.chkEnableTab.Text = "Enable Tab";
            this.chkEnableTab.UseVisualStyleBackColor = true;
            // 
            // btnCloseTab
            // 
            this.btnCloseTab.Location = new System.Drawing.Point(315, 4);
            this.btnCloseTab.Name = "btnCloseTab";
            this.btnCloseTab.Size = new System.Drawing.Size(75, 23);
            this.btnCloseTab.TabIndex = 0;
            this.btnCloseTab.Text = "Close Tab";
            this.btnCloseTab.UseVisualStyleBackColor = true;
            this.btnCloseTab.Click += new System.EventHandler(this.btnCloseTab_Click);
            // 
            // btnNewTab
            // 
            this.btnNewTab.Location = new System.Drawing.Point(234, 5);
            this.btnNewTab.Name = "btnNewTab";
            this.btnNewTab.Size = new System.Drawing.Size(75, 23);
            this.btnNewTab.TabIndex = 0;
            this.btnNewTab.Text = "New Tab";
            this.btnNewTab.UseVisualStyleBackColor = true;
            this.btnNewTab.Click += new System.EventHandler(this.btnNewTab_Click);
            // 
            // webBrowserContainer
            // 
            this.webBrowserContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserContainer.Location = new System.Drawing.Point(0, 30);
            this.webBrowserContainer.Name = "webBrowserContainer";
            this.webBrowserContainer.Size = new System.Drawing.Size(928, 539);
            this.webBrowserContainer.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 569);
            this.Controls.Add(this.webBrowserContainer);
            this.Controls.Add(this.pnlHeader);
            this.Name = "MainForm";
            this.Text = "TabbedWebBrowser";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlNavigate.ResumeLayout(false);
            this.pnlNavigate.PerformLayout();
            this.pnlTabCommand.ResumeLayout(false);
            this.pnlTabCommand.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.TextBox tbUrl;
        private System.Windows.Forms.Panel pnlNavigate;
        private System.Windows.Forms.Label lbUrl;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnForward;
        private System.Windows.Forms.Button btnBack;
        private TabbedWebBrowserContainer webBrowserContainer;
        private System.Windows.Forms.Panel pnlTabCommand;
        private System.Windows.Forms.Button btnCloseTab;
        private System.Windows.Forms.Button btnNewTab;
        private System.Windows.Forms.CheckBox chkEnableTab;
        internal System.Windows.Forms.Label lbGo;
    }
}

