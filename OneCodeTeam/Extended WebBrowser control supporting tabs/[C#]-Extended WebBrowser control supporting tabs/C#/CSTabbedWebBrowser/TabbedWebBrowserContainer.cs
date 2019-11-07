/****************************** Module Header ******************************\
* Module Name:  TabbedWebBrowserContainer.cs
* Project:	    CSTabbedWebBrowser
* Copyright (c) Microsoft Corporation.
* 
* This is a UserControl that contains a System.Windows.Forms.TabControl. The
* TabControl does not support to create/close a tab in UI, so this UserControl 
* supplies the method to create/close the tab. 
* 
* When add a new TabPage to the TabControl, the type is WebBrowserTabPage which 
* inherits System.Windows.Forms.TabPage, and this UserControl exposes the 
* methods GoBack, GoForward and Refresh of the System.Windows.Forms.WebBrowser
* class. It also subcribes the NewWindow event of WebBrowserTabPage, when a 
* NewWindow event is fired in the WebBrowser, it will create a tab other than 
* open the page in Internet Explorer. 
* 
* The "Open in new Tab" context command is disabled in WebBorwser by default, you
* can add a value *.exe=1 (* means the process name)to the key 
* HKCU\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_TABBED_BROWSING.
* See http://msdn.microsoft.com/en-us/library/ms537636(VS.85).aspx
* 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Security.Permissions;


namespace CSTabbedWebBrowser
{
    public partial class TabbedWebBrowserContainer : UserControl
    {
        /// <summary>
        /// A static property to get or set whether the "Open in new Tab" context 
        /// menu in WebBrowser is enabled.
        /// </summary>
        public static bool IsTabEnabled
        {
            [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
            get
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_TABBED_BROWSING"))
                {
                    if (key != null)
                    {
                        string processName = Process.GetCurrentProcess().ProcessName + ".exe";
                        int keyValue = (int)key.GetValue(processName, 0);
                        return keyValue == 1;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
            set
            {
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                       @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_TABBED_BROWSING"))
                {
                    string processName = Process.GetCurrentProcess().ProcessName + ".exe";
                    int keyValue = (int)key.GetValue(processName, 0);

                    bool isEnabled = keyValue == 1;
                    if (isEnabled != value)
                    {
                        key.SetValue(processName, value ? 1 : 0);
                    }
                }
            }
        }

        /// <summary>
        /// The select tab of the tab control.
        /// </summary>
        public WebBrowserTabPage ActiveTab
        {
            get
            {
                if (tabControl.SelectedTab != null)
                {
                    return tabControl.SelectedTab as WebBrowserTabPage;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// This control should have one tab at least.
        /// </summary>
        public bool CanCloseActivePage
        {
            get
            {
                return tabControl.TabPages.Count > 1;
            }
        }


        public TabbedWebBrowserContainer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Create a new tab and navigate to "about:blank" when the control is loaded.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            NewTab("about:blank");
        }

        /// <summary>
        /// Navigate the WebBrowser control in the ActiveTab to the Url.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void Navigate(string url)
        {
            if (this.ActiveTab != null)
            {
                this.ActiveTab.WebBrowser.Navigate(url);
            }
        }

        /// <summary>
        /// Create a new WebBrowserTabPage instance, add it to the tab control, and 
        /// subscribe the its NewWindow event.
        /// </summary>
        /// <returns></returns>
        WebBrowserTabPage CreateTabPage()
        {
            WebBrowserTabPage tab = new WebBrowserTabPage();
            tab.NewWindow += new EventHandler<WebBrowserNewWindowEventArgs>(tab_NewWindow);
            this.tabControl.TabPages.Add(tab);
            this.tabControl.SelectedTab = tab;
            return tab;
        }

        /// <summary>
        /// Create a WebBrowserTabPage and then navigate to the Url.
        /// </summary>
        /// <param name="url"></param>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void NewTab(string url)
        {
            CreateTabPage();
            Navigate(url);
        }

        /// <summary>
        /// Close the active tab.
        /// </summary>
        public void CloseActiveTab()
        {
            // This control should have one tab at least.
            if (CanCloseActivePage)
            {
                var tabToClose = this.ActiveTab;
                this.tabControl.TabPages.Remove(tabToClose);
            }
        }

        /// <summary>
        /// Handle the NewWindow event of the WebBrowserTabPage. when a NewWindow event
        /// is fired in the WebBrowser, create a tab other than open the page in Internet
        /// Explorer. 
        /// </summary>
        void tab_NewWindow(object sender, WebBrowserNewWindowEventArgs e)
        {
            if (TabbedWebBrowserContainer.IsTabEnabled)
            {
                NewTab(e.Url);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Expose the GoBack method of the WebBrowser control in the ActiveTab.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void GoBack()
        {
            this.ActiveTab.WebBrowser.GoBack();
        }

        /// <summary>
        /// Expose the GoForward method of the WebBrowser control in the ActiveTab.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void GoForward()
        {
            this.ActiveTab.WebBrowser.GoForward();
        }

        /// <summary>
        /// Expose the Refresh method of the WebBrowser control in the ActiveTab.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public void RefreshWebBrowser()
        {
            this.ActiveTab.WebBrowser.Refresh();
        }
    }
}
