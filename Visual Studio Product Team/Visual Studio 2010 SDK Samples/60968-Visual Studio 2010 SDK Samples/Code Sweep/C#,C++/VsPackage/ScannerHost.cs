/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Samples.VisualStudio.CodeSweep.Scanner;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Samples.VisualStudio.CodeSweep.BuildTask;

namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
{
    class ScannerHost : IScannerHost
    {
        IServiceProvider _serviceProvider;

        public ScannerHost(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #region IScannerHost Members

        public void AddResult(IScanResult result, string projectFile)
        {
            Factory.GetTaskProvider().AddResult(result, projectFile);
        }

        public string GetTextOfFileIfOpenInIde(string filePath)
        {
            IVsRunningDocumentTable rdt = _serviceProvider.GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;

            IVsHierarchy hierarchy = null;
            uint itemid = 0;
            IntPtr docDataUnk = IntPtr.Zero;
            uint lockCookie = 0;

            int hr = rdt.FindAndLockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, filePath, out hierarchy, out itemid, out docDataUnk, out lockCookie);
            try
            {
                if (hr == VSConstants.S_OK)
                {
                    IVsTextLines textLines = Marshal.GetUniqueObjectForIUnknown(docDataUnk) as IVsTextLines;

                    if (textLines != null)
                    {
                        string text = null;
                        int endLine = 0;
                        int endIndex = 0;

                        hr = textLines.GetLastLineIndex(out endLine, out endIndex);
                        Debug.Assert(hr == VSConstants.S_OK, "GetLastLineIndex did not return S_OK.");

                        hr = textLines.GetLineText(0, 0, endLine, endIndex, out text);
                        Debug.Assert(hr == VSConstants.S_OK, "GetLineText did not return S_OK.");

                        return text;
                    }
                }

                return null;
            }
            finally
            {
                if (lockCookie != 0)
                {
                    rdt.UnlockDocument((uint)_VSRDTFLAGS.RDT_ReadLock, lockCookie);
                }
            }
        }

        #endregion
    }
}
