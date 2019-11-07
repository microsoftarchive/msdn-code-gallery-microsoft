/****************************** Module Header ******************************\
Module Name:  CWin32WindowWrapper.cs
Project:      CSOneNoteRibbonAddIn
Copyright (c) Microsoft Corporation.

wrapper Win32 HWND handles

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Imports directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms; 
#endregion

namespace CSOneNoteRibbonAddIn
{
    internal class CWin32WindowWrapper : IWin32Window
    {
        // handle field
        private IntPtr _windowHandle;

        /// <summary>
        ///     CWin32WindowWrapper construct method  
        /// </summary>
        /// <param name="windowHandle">window handle</param>
        public CWin32WindowWrapper(IntPtr windowHandle)
        {
            this._windowHandle = windowHandle;
        }

        // Summary:
        //     Gets the handle to the window represented by the implementer.
        //
        // Returns:
        //     A handle to the window represented by the implementer.
        public IntPtr Handle
        {
            get
            {
                return this._windowHandle;
            }
        }
    }
}
