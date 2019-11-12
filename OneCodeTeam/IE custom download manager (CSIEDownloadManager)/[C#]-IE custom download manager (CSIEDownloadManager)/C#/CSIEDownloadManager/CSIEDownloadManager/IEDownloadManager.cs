/****************************** Module Header ******************************\
 * Module Name:  IEDownloadManager.cs
 * Project:      CSIEDownloadManager
 * Copyright (c) Microsoft Corporation.
 * 
 * The ability to implement a custom download manager was introduced in Microsoft 
 * Internet Explorer 5.5. This feature enables you to extend the functionality of 
 * Windows Internet Explorer and WebBrowser applications by implementing a Component
 * Object Model (COM) object to handle the file download process.
 * 
 * A download manager is implemented as a COM object that exposes the IUnknown and
 * IDownloadManager interface. IDownloadManager has only one method, 
 * IDownloadManager::Download, which is called by Internet Explorer or a WebBrowser
 * application to download a file. 
 * 
 * For Internet Explorer 6 and later, if the WebBrowser application does not implement
 * the IServiceProvider::QueryService method, or when using Internet Explorer itself 
 * for which IServiceProvider::QueryService cannot be implemented, the application 
 * checks for the presence of a registry key containing the class identifier (CLSID) 
 * of the download manager COM object. The CLSID can be provided in either of the 
 * following registry values.
 * 
 *     HKEY_LOCAL_MACHINE
 *          Software
 *               Microsoft
 *                    Internet Explorer
 *                         DownloadUI
 *     HKEY_CURRENT_USER
 *          Software
 *               Microsoft
 *                    Internet Explorer
 *                         DownloadUI
 * 
 * DownloadUI is not a key, it is a REG_SZ value under Software\Microsoft\Internet Explorer.
 * 
 * If the application cannot locate a custom download manager the default download user 
 * interface is used.
 * 
 * The IEDownloadManager class implements the IDownloadManager interface. When IE starts to 
 * download a file, the Download method of this class will be called, and then the 
 * CSWebDownloader.exe will be launched to download the file.
 * 
 * This class will also set the registry values when this assembly is registered to COM.
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Win32;

namespace CSIEDownloadManager
{
    [System.Runtime.InteropServices.ComVisible(true)]
    [System.Runtime.InteropServices.Guid("bdb9c34c-d0ca-448e-b497-8de62e709744")]
    public class IEDownloadManager : NativeMethods.IDownloadManager
    {
        /// <summary>
        /// Return S_OK (0) so that IE will stop to download the file itself. 
        /// Else the default download user interface is used.
        /// </summary>
        public int Download(IMoniker pmk, IBindCtx pbc, uint dwBindVerb, int grfBINDF, 
            IntPtr pBindInfo, string pszHeaders, string pszRedir, uint uiCP)
        {

            // Get the display name of the pointer to an IMoniker interface that specifies
            // the object to be downloaded.
            string name = string.Empty;
            pmk.GetDisplayName(pbc, null, out name);

            if (!string.IsNullOrEmpty(name))
            {
                Uri url = null;
                bool result = Uri.TryCreate(name, UriKind.Absolute, out url);

                if (result)
                {

                    // Launch CSWebDownloader.exe to download the file. 
                    FileInfo assemblyFile = 
                        new FileInfo(Assembly.GetExecutingAssembly().Location);
                    ProcessStartInfo start = new ProcessStartInfo
                    {
                        Arguments = name,
                        FileName =
                        string.Format("{0}\\CSWebDownloader.exe", assemblyFile.DirectoryName)
                    };
                    Process.Start(start);
                    return 0;
                }              
            }
            return 1;
        }



        #region ComRegister Functions

        /// <summary>
        /// Called when derived class is registered as a COM server.
        /// </summary>
        [System.Runtime.InteropServices.ComRegisterFunction]
        public static void Register(Type t)
        {
            string ieKeyPath = @"SOFTWARE\Microsoft\Internet Explorer\";
            using (RegistryKey ieKey = Registry.CurrentUser.CreateSubKey(ieKeyPath))
            {
                ieKey.SetValue("DownloadUI", t.GUID.ToString("B"));
            }
        }

        /// <summary>
        /// Called when derived class is unregistered as a COM server.
        /// </summary>
        [System.Runtime.InteropServices.ComUnregisterFunction]
        public static void Unregister(Type t)
        {
            string ieKeyPath = @"SOFTWARE\Microsoft\Internet Explorer\";
            using (RegistryKey ieKey = Registry.CurrentUser.OpenSubKey(ieKeyPath, true))
            {
                ieKey.DeleteValue("DownloadUI");
            }
        }
        #endregion
    }
}
