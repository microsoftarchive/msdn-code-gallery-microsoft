/****************************** Module Header ******************************\
* Module Name:  ImageListExplorerBar.cs
* Project:	    CSIEExplorerBar
* Copyright (c) Microsoft Corporation.
* 
* The class ImageListExplorerBar inherits the class System.Windows.Forms.UserControl, 
* and implements the IObjectWithSite, IDeskBand, IDockingWindow, IOleWindow and 
* IInputObject interfaces.
* 
* To add an Explorer Bar in IE, you can follow these steps:
* 
* 1. Create a valid GUID for this ComVisible class. 
* 
* 2. Implement the IObjectWithSite, IDeskBand, IDockingWindow, IOleWindow and 
*    IInputObject interfaces.
*    
* 3. Register this assembly to COM.
* 
* 4. 4.Create a new key using the category identifier (CATID) of the type of 
*    Explorer Bar you are creating as the name of the key. This can be one of
*    the following values: 
*    {00021494-0000-0000-C000-000000000046} Defines a horizontal Explorer Bar. 
*    {00021493-0000-0000-C000-000000000046} Defines a vertical Explorer Bar. 
*    
*    The result should look like:
*
*    HKEY_CLASSES_ROOT\CLSID\<Your GUID>\Implemented Categories\{00021494-0000-0000-C000-000000000046}
*    or  
*    HKEY_CLASSES_ROOT\CLSID\<Your GUID>\Implemented Categories\{00021493-0000-0000-C000-000000000046}
*    
* 5. Delete following registry keys because they cache the ExplorerBar enum.
* 
*    HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Discardable\PostSetup\
*    Component Categories\{00021493-0000-0000-C000-000000000046}\Enum
*    or
*    HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Discardable\PostSetup\
*    Component Categories\{00021494-0000-0000-C000-000000000046}\Enum
*
*
* 6. Set the size of the Explorer Bar in the registry.
* 
*    HKEY_LOCAL_MACHINE\Software\Microsoft\Internet Explorer\Explorer Bars\<Your GUID>\BarSize
*
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using SHDocVw;

namespace CSIEExplorerBar
{
    [ComVisible(true)]
    [Guid("5802D092-1784-4908-8CDB-99B6842D353F")]
    public partial class ImageListExplorerBar :
        UserControl,
        NativeMethods.IObjectWithSite,
        NativeMethods.IDeskBand,
        NativeMethods.IDockingWindow,
        NativeMethods.IOleWindow,
        NativeMethods.IInputObject
    {

        // The title of the explorer bar.
        private const string imageListExplorerBarTitle =
            "Image List Explorer Bar";

        // Defines a vertical Explorer Bar. 
        private const string verticalExplorerBarCATID =
            "{00021493-0000-0000-C000-000000000046}";

        // The IInputObjectSite object.
        private NativeMethods.IInputObjectSite site;

        // The Internet Explorer instance.
        private InternetExplorer explorer;

        public ImageListExplorerBar()
        {
            InitializeComponent();
        }

        #region NativeMethods.IObjectWithSite

        public void SetSite(object pUnkSite)
        {

            // Release previous COM objects.
            if (this.site != null)
            {
                Marshal.ReleaseComObject(this.site);
            }
            if (this.explorer != null)
            {
                Marshal.ReleaseComObject(this.explorer);
                this.explorer = null;
            }

            // pUnkSite is a pointer to object that implements IOleWindowSite. 
            this.site = pUnkSite as NativeMethods.IInputObjectSite;

            if (this.site != null)
            {

                // The site implements IServiceProvider interface and can be used to 
                // get InternetExplorer instance.
                var provider = this.site as NativeMethods._IServiceProvider;
                Guid guid = new Guid("{0002DF05-0000-0000-C000-000000000046}");
                Guid riid = new Guid("{00000000-0000-0000-C000-000000000046}");
                try
                {
                    object webBrowser;
                    provider.QueryService(ref guid, ref riid, out webBrowser);
                    this.explorer = webBrowser as InternetExplorer;
                }
                catch (COMException)
                {
                }
            }

        }


        public void GetSite(ref Guid riid, out object ppvSite)
        {
            ppvSite = this.site;
        }

        #endregion

        #region NativeMethods.IDeskBand

        /// <summary>
        /// Gets the information for a band object.
        /// </summary>
        public void GetBandInfo(uint dwBandID, uint dwViewMode,
            ref NativeMethods.DESKBANDINFO pdbi)
        {
            pdbi.wszTitle = ImageListExplorerBar.imageListExplorerBarTitle;
            pdbi.ptActual.X = base.Size.Width;
            pdbi.ptActual.Y = base.Size.Height;
            pdbi.ptMaxSize.X = -1;
            pdbi.ptMaxSize.Y = -1;
            pdbi.ptMinSize.X = -1;
            pdbi.ptMinSize.Y = -1;
            pdbi.ptIntegral.X = -1;
            pdbi.ptIntegral.Y = -1;
            pdbi.dwModeFlags = NativeMethods.DBIM.NORMAL 
                | NativeMethods.DBIM.VARIABLEHEIGHT;
        }

        public void ShowDW(bool fShow)
        {
            if (fShow)
                this.Show();
            else
                this.Hide();
        }

        public void CloseDW(uint dwReserved)
        {
            this.Dispose(true);
        }

        public void ResizeBorderDW(IntPtr prcBorder, object punkToolbarSite, bool fReserved)
        {
        }

        public void GetWindow(out IntPtr hwnd)
        {
            hwnd = this.Handle;
        }

        public void ContextSensitiveHelp(bool fEnterMode)
        {
        }

        #endregion


        #region NativeMethods.IInputObject

        /// <summary>
        /// UI-activates or deactivates the object.
        /// </summary>
        /// <param name="fActivate">
        /// Indicates if the object is being activated or deactivated. If this value is 
        /// nonzero, the object is being activated. If this value is zero, the object is
        /// being deactivated.
        /// </param>
        public void UIActivateIO(int fActivate, ref NativeMethods.MSG msg)
        {
            if (fActivate != 0)
            {
                Control nextControl = base.GetNextControl(this, true);
                if (Control.ModifierKeys == Keys.Shift)
                {
                    nextControl = base.GetNextControl(nextControl, false);
                }
                if (nextControl != null)
                {
                    nextControl.Select();
                }
                base.Focus();
            }

        }

        public int HasFocusIO()
        {
            if (!base.ContainsFocus)
            {
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Enables the object to process keyboard accelerators.
        /// </summary>
        public int TranslateAcceleratorIO(ref NativeMethods.MSG msg)
        {
            if ((msg.message == 256) && ((msg.wParam == 9) || (msg.wParam == 117)))
                if (base.SelectNextControl(
                    base.ActiveControl,
                    Control.ModifierKeys != Keys.Shift,
                    true,
                    true,
                    false))
                {
                    return 0;
                }
            return 1;

        }
        #endregion

        #region ComRegister Functions

        /// <summary>
        /// Called when derived class is registered as a COM server.
        /// </summary>
        [ComRegisterFunctionAttribute]
        public static void Register(Type t)
        {

            // Add the category identifier for a vertical Explorer Bar and set other values.
            RegistryKey clsidkey =
                Registry.ClassesRoot.CreateSubKey(@"CLSID\" + t.GUID.ToString("B"));
            clsidkey.SetValue(null, ImageListExplorerBar.imageListExplorerBarTitle);
            clsidkey.SetValue("MenuText", ImageListExplorerBar.imageListExplorerBarTitle);
            clsidkey.SetValue("HelpText", "See Readme.txt for detailed help!");
            clsidkey.CreateSubKey("Implemented Categories")
                .CreateSubKey(ImageListExplorerBar.verticalExplorerBarCATID);
            clsidkey.Close();

            // Set Bar size.
            string explorerBarKeyPath =
@"SOFTWARE\Microsoft\Internet Explorer\Explorer Bars\" + t.GUID.ToString("B");
            RegistryKey explorerBarKey =
                Registry.LocalMachine.CreateSubKey(explorerBarKeyPath);
            explorerBarKey.SetValue("BarSize",
                new byte[] { 06, 01, 00, 00, 00, 00, 00, 00 },
                RegistryValueKind.Binary);
            explorerBarKey.Close();


            Registry.CurrentUser.CreateSubKey(explorerBarKeyPath).SetValue("BarSize", new byte[] { 06, 01, 00, 00, 00, 00, 00, 00 }, RegistryValueKind.Binary);

            // Remove the cache.
            try
            {
                string catPath =
@"Software\Microsoft\Windows\CurrentVersion\Explorer\Discardable\PostSetup\Component Categories\"
+ ImageListExplorerBar.verticalExplorerBarCATID;

                Registry.CurrentUser.OpenSubKey(catPath, true).DeleteSubKey("Enum");
            }
            catch { }

            try
            {
                string catPath =
@"Software\Microsoft\Windows\CurrentVersion\Explorer\Discardable\PostSetup\Component Categories64\"
+ ImageListExplorerBar.verticalExplorerBarCATID;

                Registry.CurrentUser.OpenSubKey(catPath, true).DeleteSubKey("Enum");
            }
            catch { }
        }

        /// <summary>
        /// Called when derived class is unregistered as a COM server.
        /// </summary>
        [ComUnregisterFunctionAttribute]
        public static void Unregister(Type t)
        {
            string clsidkeypath = t.GUID.ToString("B");
            Registry.ClassesRoot.OpenSubKey("CLSID", true).DeleteSubKeyTree(clsidkeypath);

            string explorerBarsKeyPath =
@"SOFTWARE\Microsoft\Internet Explorer\Explorer Bars";

            Registry.LocalMachine.OpenSubKey(explorerBarsKeyPath, true).DeleteSubKey(t.GUID.ToString("B"));
            Registry.CurrentUser.OpenSubKey(explorerBarsKeyPath, true).DeleteSubKey(t.GUID.ToString("B"));


        }


        #endregion

        private void btnGetImg_Click(object sender, EventArgs e)
        {
            lstImg.Items.Clear();

            mshtml.HTMLDocument doc = explorer.Document as mshtml.HTMLDocument;

            var imgs = doc.getElementsByTagName("img");

            foreach (var img in imgs)
            {
                mshtml.IHTMLImgElement imgElement = img as mshtml.IHTMLImgElement;
                if (imgElement != null && !lstImg.Items.Contains(imgElement.src))
                {
                    lstImg.Items.Add(imgElement.src);
                }
            }
        }

        private void lstImg_DoubleClick(object sender, EventArgs e)
        {
            if (lstImg.SelectedItem != null)
            {
                string url = lstImg.SelectedItem as string;
                mshtml.HTMLDocument doc = explorer.Document as mshtml.HTMLDocument;
                doc.parentWindow.open(url);
            }
        }
    }
}
