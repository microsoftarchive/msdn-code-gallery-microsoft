/****************************** Module Header ******************************\
* Module Name:  BHOIEContextMenu.cs
* Project:	    CSBrowserHelperObject
* Copyright (c) Microsoft Corporation.
* 
* The class BHOIEContextMenu is a Browser Helper Object which runs within Internet
* Explorer and offers additional services.
* 
* A BHO is a dynamic-link library (DLL) capable of attaching itself to any new 
* instance of Internet Explorer or Windows Explorer. Such a module can get in touch 
* with the browser through the container's site. In general, a site is an intermediate
* object placed in the middle of the container and each contained object. When the
* container is Internet Explorer (or Windows Explorer), the object is now required 
* to implement a simpler and lighter interface called IObjectWithSite. 
* It provides just two methods SetSite and GetSite. 
* 
* This class is used to disable the default context menu in IE. It also supplies 
* functions to register this BHO to IE.
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
using Microsoft.Win32;
using SHDocVw;
using mshtml;

namespace CSBrowserHelperObject
{
    /// <summary>
    /// Set the GUID of this class and specify that this class is ComVisible.
    /// A BHO must implement the interface IObjectWithSite. 
    /// </summary>
    [ComVisible(true),
    ClassInterface(ClassInterfaceType.None),
   Guid("C42D40F0-BEBF-418D-8EA1-18D99AC2AB17")]
    public class BHOIEContextMenu : IObjectWithSite
    {
        // Current IE instance. For IE7 or later version, an IE Tab is just 
        // an IE instance.
        private InternetExplorer ieInstance;

        // To register a BHO, a new key should be created under this key.
        private const string BHORegistryKey =
            "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Browser Helper Objects";



        #region Com Register/UnRegister Methods
        /// <summary>
        /// When this class is registered to COM, add a new key to the BHORegistryKey 
        /// to make IE use this BHO.
        /// On 64bit machine, if the platform of this assembly and the installer is x86,
        /// 32 bit IE can use this BHO. If the platform of this assembly and the installer
        /// is x64, 64 bit IE can use this BHO.
        /// </summary>
        [ComRegisterFunction]
        public static void RegisterBHO(Type t)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(BHORegistryKey, true);
            if (key == null)
            {
                key = Registry.LocalMachine.CreateSubKey(BHORegistryKey);
            }

            // 32 digits separated by hyphens, enclosed in braces: 
            // {00000000-0000-0000-0000-000000000000}
            string bhoKeyStr = t.GUID.ToString("B");
            
            RegistryKey bhoKey = key.OpenSubKey(bhoKeyStr, true);

            // Create a new key.
            if (bhoKey == null)
            {
                bhoKey = key.CreateSubKey(bhoKeyStr);
            }

            // NoExplorer:dword = 1 prevents the BHO to be loaded by Explorer
            string name = "NoExplorer";
            object value = (object)1;
            bhoKey.SetValue(name, value);
            key.Close();
            bhoKey.Close();
        }

        /// <summary>
        /// When this class is unregistered from COM, delete the key.
        /// </summary>
        [ComUnregisterFunction]
        public static void UnregisterBHO(Type t)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(BHORegistryKey, true);
            string guidString = t.GUID.ToString("B");
            if (key != null)
            {
                key.DeleteSubKey(guidString, false);
            }
        }
        #endregion

        #region IObjectWithSite Members
        /// <summary>
        /// This method is called when the BHO is instantiated and when
        /// it is destroyed. The site is an object implemented the 
        /// interface InternetExplorer.
        /// </summary>
        /// <param name="site"></param>
        public void SetSite(Object site)
        {
            if (site != null)
            {
                ieInstance = (InternetExplorer)site;

                // Register the DocumentComplete event.
                ieInstance.DocumentComplete +=
                    new DWebBrowserEvents2_DocumentCompleteEventHandler(
                        ieInstance_DocumentComplete);
            }
        }

        /// <summary>
        /// Retrieves and returns the specified interface from the last site
        /// set through SetSite(). The typical implementation will query the
        /// previously stored pUnkSite pointer for the specified interface.
        /// </summary>
        public void GetSite(ref Guid guid, out Object ppvSite)
        {
            IntPtr punk = Marshal.GetIUnknownForObject(ieInstance);
            ppvSite = new object();
            IntPtr ppvSiteIntPtr = Marshal.GetIUnknownForObject(ppvSite);
            int hr = Marshal.QueryInterface(punk, ref guid, out ppvSiteIntPtr);
            Marshal.ThrowExceptionForHR(hr);
            Marshal.Release(punk);
            Marshal.Release(ppvSiteIntPtr);
        }
        #endregion

        #region event handler

        /// <summary>
        /// Handle the DocumentComplete event.
        /// </summary>
        /// <param name="pDisp">
        /// The pDisp is an an object implemented the interface InternetExplorer.
        /// By default, this object is the same as the ieInstance, but if the page 
        /// contains many frames, each frame has its own document.
        /// </param>
        void ieInstance_DocumentComplete(object pDisp, ref object URL)
        {
            string url = URL as string;

            if (string.IsNullOrEmpty(url) 
                || url.Equals("about:blank", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            InternetExplorer explorer = pDisp as InternetExplorer;

            // Set the handler of the document in InternetExplorer.
            if (explorer != null)
            {
                SetHandler(explorer);
            }
        }


        void SetHandler(InternetExplorer explorer)
        {
            try
            {

                // Register the oncontextmenu event of the  document in InternetExplorer.
                HTMLDocumentEventHelper helper =
                    new HTMLDocumentEventHelper(explorer.Document as HTMLDocument);
                helper.oncontextmenu += new HtmlEvent(oncontextmenuHandler);
            }
            catch { }
        }

        /// <summary>
        /// Handle the oncontextmenu event.
        /// </summary>
        /// <param name="e"></param>
        void oncontextmenuHandler(IHTMLEventObj e)
        {

            // To cancel the default behavior, set the returnValue property of the event
            // object to false.
            e.returnValue = false;

        }

        #endregion




    }
}
