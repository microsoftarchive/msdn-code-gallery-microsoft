// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

namespace FormRegionOutlookAddIn
{
    [System.ComponentModel.ToolboxItemAttribute(false)]
    partial class RssPane : Microsoft.Office.Tools.Outlook.FormRegionBase
    {
        public RssPane(Microsoft.Office.Interop.Outlook.FormRegion formRegion)
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
            this.webBrowserRss = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webBrowserRss
            // 
            this.webBrowserRss.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserRss.Location = new System.Drawing.Point(0, 0);
            this.webBrowserRss.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserRss.Name = "webBrowserRss";
            this.webBrowserRss.Size = new System.Drawing.Size(725, 525);
            this.webBrowserRss.TabIndex = 0;
            // 
            // RssPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.webBrowserRss);
            this.Name = "RssPane";
            this.Size = new System.Drawing.Size(725, 525);
            this.FormRegionClosed += new System.EventHandler(this.RssPane_FormRegionClosed);
            this.FormRegionShowing += new System.EventHandler(this.RssPane_FormRegionShowing);
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
            manifest.FormRegionName = "RSS Article";
            manifest.FormRegionType = Microsoft.Office.Tools.Outlook.FormRegionType.Adjoining;
            manifest.ShowInspectorCompose = false;
            manifest.ShowInspectorRead = false;

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowserRss;

        public partial class RssPaneFactory : Microsoft.Office.Tools.Outlook.IFormRegionFactory
        {
            public event Microsoft.Office.Tools.Outlook.FormRegionInitializingEventHandler FormRegionInitializing;

            private Microsoft.Office.Tools.Outlook.FormRegionManifest _Manifest;

            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public RssPaneFactory()
            {
                this._Manifest = Globals.Factory.CreateFormRegionManifest();
                RssPane.InitializeManifest(this._Manifest);
                this.FormRegionInitializing += new Microsoft.Office.Tools.Outlook.FormRegionInitializingEventHandler(this.RssPaneFactory_FormRegionInitializing);
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
                RssPane form = new RssPane(formRegion);
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
        internal RssPane RssPane
        {
            get
            {
                foreach (var item in this)
                {
                    if (item.GetType() == typeof (RssPane))
                        return (RssPane)item;
                }
                return null;
            }
        }
    }
}
