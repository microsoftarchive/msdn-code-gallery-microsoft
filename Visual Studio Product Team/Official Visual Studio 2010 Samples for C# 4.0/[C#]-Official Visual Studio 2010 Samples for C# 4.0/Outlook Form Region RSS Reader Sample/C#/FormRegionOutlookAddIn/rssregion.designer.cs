// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

namespace FormRegionOutlookAddIn
{
    [System.ComponentModel.ToolboxItemAttribute(false)]
    partial class RssRegion : Microsoft.Office.Tools.Outlook.FormRegionBase
    {
        public RssRegion(Microsoft.Office.Interop.Outlook.FormRegion formRegion)
            : base(Globals.Factory, formRegion)
        {
            this.InitializeComponent();
        }

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
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
            this.RssRegionSplitContainer = new System.Windows.Forms.SplitContainer();
            this.webBrowserRss = new System.Windows.Forms.WebBrowser();
            this.viewRssToolStrip = new System.Windows.Forms.ToolStrip();
            this.searchSimilarTopicsButton = new System.Windows.Forms.ToolStripButton();
            this.viewRssBackButton = new System.Windows.Forms.ToolStripButton();
            this.viewRssProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.webBrowserSearch = new System.Windows.Forms.WebBrowser();
            this.searchResultsToolStrip = new System.Windows.Forms.ToolStrip();
            this.hideSearchResultsButton = new System.Windows.Forms.ToolStripButton();
            this.searchResultsBackButton = new System.Windows.Forms.ToolStripButton();
            this.searchWindowProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.RssRegionSplitContainer)).BeginInit();
            this.RssRegionSplitContainer.Panel1.SuspendLayout();
            this.RssRegionSplitContainer.Panel2.SuspendLayout();
            this.RssRegionSplitContainer.SuspendLayout();
            this.viewRssToolStrip.SuspendLayout();
            this.searchResultsToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // RssRegionSplitContainer
            // 
            this.RssRegionSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RssRegionSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.RssRegionSplitContainer.Name = "RssRegionSplitContainer";
            // 
            // RssRegionSplitContainer.Panel1
            // 
            this.RssRegionSplitContainer.Panel1.Controls.Add(this.webBrowserRss);
            this.RssRegionSplitContainer.Panel1.Controls.Add(this.viewRssToolStrip);
            // 
            // RssRegionSplitContainer.Panel2
            // 
            this.RssRegionSplitContainer.Panel2.Controls.Add(this.webBrowserSearch);
            this.RssRegionSplitContainer.Panel2.Controls.Add(this.searchResultsToolStrip);
            this.RssRegionSplitContainer.Size = new System.Drawing.Size(800, 525);
            this.RssRegionSplitContainer.SplitterDistance = 400;
            this.RssRegionSplitContainer.TabIndex = 0;
            // 
            // webBrowserRss
            // 
            this.webBrowserRss.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserRss.Location = new System.Drawing.Point(0, 25);
            this.webBrowserRss.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserRss.Name = "webBrowserRss";
            this.webBrowserRss.Size = new System.Drawing.Size(400, 500);
            this.webBrowserRss.TabIndex = 2;
            this.webBrowserRss.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserRss_DocumentCompleted);
            this.webBrowserRss.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowserRss_Navigating);
            this.webBrowserRss.ProgressChanged += new System.Windows.Forms.WebBrowserProgressChangedEventHandler(this.webBrowserRss_ProgressChanged);
            // 
            // viewRssToolStrip
            // 
            this.viewRssToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.viewRssToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchSimilarTopicsButton,
            this.viewRssBackButton,
            this.viewRssProgressBar});
            this.viewRssToolStrip.Location = new System.Drawing.Point(0, 0);
            this.viewRssToolStrip.Name = "viewRssToolStrip";
            this.viewRssToolStrip.Size = new System.Drawing.Size(400, 25);
            this.viewRssToolStrip.TabIndex = 0;
            this.viewRssToolStrip.Text = "ViewRssToolStrip";
            // 
            // searchSimilarTopicsButton
            // 
            this.searchSimilarTopicsButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.searchSimilarTopicsButton.Checked = true;
            this.searchSimilarTopicsButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.searchSimilarTopicsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.searchSimilarTopicsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchSimilarTopicsButton.Name = "searchSimilarTopicsButton";
            this.searchSimilarTopicsButton.Size = new System.Drawing.Size(127, 22);
            this.searchSimilarTopicsButton.Text = "Search for Similar Topics";
            this.searchSimilarTopicsButton.Click += new System.EventHandler(this.searchSimilarTopicsButton_Click);
            // 
            // viewRssBackButton
            // 
            this.viewRssBackButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.viewRssBackButton.Checked = true;
            this.viewRssBackButton.CheckOnClick = true;
            this.viewRssBackButton.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.viewRssBackButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.viewRssBackButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.viewRssBackButton.Name = "viewRssBackButton";
            this.viewRssBackButton.Size = new System.Drawing.Size(51, 22);
            this.viewRssBackButton.Text = "   Back   ";
            this.viewRssBackButton.ToolTipText = "Back";
            this.viewRssBackButton.Click += new System.EventHandler(this.viewRssBackButton_Click);
            // 
            // viewRssProgressBar
            // 
            this.viewRssProgressBar.Name = "viewRssProgressBar";
            this.viewRssProgressBar.Size = new System.Drawing.Size(100, 22);
            this.viewRssProgressBar.Step = 20;
            this.viewRssProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.viewRssProgressBar.ToolTipText = "View Article Progress Bar";
            this.viewRssProgressBar.Value = 15;
            // 
            // webBrowserSearch
            // 
            this.webBrowserSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserSearch.Location = new System.Drawing.Point(0, 25);
            this.webBrowserSearch.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserSearch.Name = "webBrowserSearch";
            this.webBrowserSearch.Size = new System.Drawing.Size(396, 500);
            this.webBrowserSearch.TabIndex = 2;
            this.webBrowserSearch.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserSearch_DocumentCompleted);
            this.webBrowserSearch.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowserSearch_Navigating);
            this.webBrowserSearch.ProgressChanged += new System.Windows.Forms.WebBrowserProgressChangedEventHandler(this.webBrowserSearch_ProgressChanged);
            // 
            // searchResultsToolStrip
            // 
            this.searchResultsToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.searchResultsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideSearchResultsButton,
            this.searchResultsBackButton,
            this.searchWindowProgressBar});
            this.searchResultsToolStrip.Location = new System.Drawing.Point(0, 0);
            this.searchResultsToolStrip.Name = "searchResultsToolStrip";
            this.searchResultsToolStrip.Size = new System.Drawing.Size(396, 25);
            this.searchResultsToolStrip.TabIndex = 0;
            this.searchResultsToolStrip.Text = "SearchResultsToolStrip";
            // 
            // hideSearchResultsButton
            // 
            this.hideSearchResultsButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.hideSearchResultsButton.Checked = true;
            this.hideSearchResultsButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hideSearchResultsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.hideSearchResultsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.hideSearchResultsButton.Name = "hideSearchResultsButton";
            this.hideSearchResultsButton.Size = new System.Drawing.Size(106, 22);
            this.hideSearchResultsButton.Text = "Hide Search Results";
            this.hideSearchResultsButton.Click += new System.EventHandler(this.hideSearchResultsButton_Click);
            // 
            // searchResultsBackButton
            // 
            this.searchResultsBackButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.searchResultsBackButton.Checked = true;
            this.searchResultsBackButton.CheckOnClick = true;
            this.searchResultsBackButton.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.searchResultsBackButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.searchResultsBackButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchResultsBackButton.Name = "searchResultsBackButton";
            this.searchResultsBackButton.Size = new System.Drawing.Size(51, 22);
            this.searchResultsBackButton.Text = "   Back   ";
            this.searchResultsBackButton.ToolTipText = "Back   ";
            this.searchResultsBackButton.Click += new System.EventHandler(this.searchResultsBackButton_Click);
            // 
            // searchWindowProgressBar
            // 
            this.searchWindowProgressBar.Name = "searchWindowProgressBar";
            this.searchWindowProgressBar.Size = new System.Drawing.Size(100, 22);
            this.searchWindowProgressBar.Step = 20;
            this.searchWindowProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.searchWindowProgressBar.ToolTipText = "Search Window Progress Bar";
            this.searchWindowProgressBar.Value = 15;
            // 
            // RssRegion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.RssRegionSplitContainer);
            this.Name = "RssRegion";
            this.Size = new System.Drawing.Size(800, 525);
            this.FormRegionShowing += new System.EventHandler(this.RssRegion_FormRegionShowing);
            this.FormRegionClosed += new System.EventHandler(this.RssRegion_FormRegionClosed);
            this.RssRegionSplitContainer.Panel1.ResumeLayout(false);
            this.RssRegionSplitContainer.Panel1.PerformLayout();
            this.RssRegionSplitContainer.Panel2.ResumeLayout(false);
            this.RssRegionSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RssRegionSplitContainer)).EndInit();
            this.RssRegionSplitContainer.ResumeLayout(false);
            this.viewRssToolStrip.ResumeLayout(false);
            this.viewRssToolStrip.PerformLayout();
            this.searchResultsToolStrip.ResumeLayout(false);
            this.searchResultsToolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        #region Form Region Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private static void InitializeManifest(Microsoft.Office.Tools.Outlook.FormRegionManifest manifest)
        {
            manifest.FormRegionName = "View Article";
            manifest.Icons.Page = global::FormRegionOutlookAddIn.Properties.Resources.RssFeed;
            manifest.ShowInspectorCompose = false;
            manifest.ShowReadingPane = false;

        }

        #endregion

        private System.Windows.Forms.SplitContainer RssRegionSplitContainer;
        private System.Windows.Forms.ToolStrip viewRssToolStrip;
        private System.Windows.Forms.ToolStrip searchResultsToolStrip;
        private System.Windows.Forms.ToolStripButton searchSimilarTopicsButton;
        private System.Windows.Forms.ToolStripButton viewRssBackButton;
        private System.Windows.Forms.ToolStripProgressBar viewRssProgressBar;
        private System.Windows.Forms.WebBrowser webBrowserRss;
        private System.Windows.Forms.ToolStripButton hideSearchResultsButton;
        private System.Windows.Forms.ToolStripButton searchResultsBackButton;
        private System.Windows.Forms.ToolStripProgressBar searchWindowProgressBar;
        private System.Windows.Forms.WebBrowser webBrowserSearch;

        public partial class RssRegionFactory : Microsoft.Office.Tools.Outlook.IFormRegionFactory
        {
            public event Microsoft.Office.Tools.Outlook.FormRegionInitializingEventHandler  FormRegionInitializing;

            private Microsoft.Office.Tools.Outlook.FormRegionManifest _Manifest;

            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public RssRegionFactory()
            {
                this._Manifest = Globals.Factory.CreateFormRegionManifest();
                RssRegion.InitializeManifest(this._Manifest);
                this.FormRegionInitializing += 
                    new Microsoft.Office.Tools.Outlook.FormRegionInitializingEventHandler(this.RssRegionFactory_FormRegionInitializing);
                    //new Microsoft.Office.Tools.Outlook.FormRegionInitializingEventArgs(this.RssRegionFactory_FormRegionInitializing);
            }

            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public Microsoft.Office.Tools.Outlook.FormRegionManifest Manifest
            {
                get
                {
                    return this._Manifest;
                }
            }

            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            Microsoft.Office.Tools.Outlook.IFormRegion Microsoft.Office.Tools.Outlook.IFormRegionFactory.CreateFormRegion(Microsoft.Office.Interop.Outlook.FormRegion formRegion)
            {
                RssRegion form = new RssRegion(formRegion);
                form.Factory = this;
                return form;
            }

            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            byte[] Microsoft.Office.Tools.Outlook.IFormRegionFactory.GetFormRegionStorage(object outlookItem, Microsoft.Office.Interop.Outlook.OlFormRegionMode formRegionMode, Microsoft.Office.Interop.Outlook.OlFormRegionSize formRegionSize)
            {
                throw new System.NotSupportedException();
            }

            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            bool Microsoft.Office.Tools.Outlook.IFormRegionFactory.IsDisplayedForItem(object outlookItem, Microsoft.Office.Interop.Outlook.OlFormRegionMode formRegionMode, Microsoft.Office.Interop.Outlook.OlFormRegionSize formRegionSize)
            {
                if (this.FormRegionInitializing != null)
                {
                    Microsoft.Office.Tools.Outlook.FormRegionInitializingEventArgs cancelArgs =
                        Globals.Factory.CreateFormRegionInitializingEventArgs(outlookItem, formRegionMode, formRegionSize, false);
                    this.FormRegionInitializing(this, cancelArgs);
                    return !cancelArgs.Cancel;
                }
                else
                {
                    return true;
                }
            }




            public Microsoft.Office.Tools.Outlook.FormRegionKindConstants Kind
            {
                get { return Microsoft.Office.Tools.Outlook.FormRegionKindConstants.WindowsForms; }
            }
        }
    }

    partial class WindowFormRegionCollection
    {
        internal RssRegion RssRegion
        {
            get
            {
                foreach (var item in this)
                {
                    if (item.GetType() == typeof(RssRegion))
                        return (RssRegion)item;
                }
                return null;
            }
        }
    }
}
