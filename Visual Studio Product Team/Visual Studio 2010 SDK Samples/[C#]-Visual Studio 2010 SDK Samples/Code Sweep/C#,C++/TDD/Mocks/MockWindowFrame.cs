/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    class MockWindowFrame : IVsWindowFrame
    {
        public MockTextLines TextLines = null;

        #region IVsWindowFrame Members

        public int CloseFrame(uint grfSaveOptions)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetFramePos(VSSETFRAMEPOS[] pdwSFP, out Guid pguidRelativeTo, out int px, out int py, out int pcx, out int pcy)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetGuidProperty(int propid, out Guid pguid)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetProperty(int propid, out object pvar)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Hide()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int IsOnScreen(out int pfOnScreen)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int IsVisible()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int QueryViewInterface(ref Guid riid, out IntPtr ppv)
        {
            if (riid == typeof(IVsTextLines).GUID)
            {
                ppv = Marshal.GetIUnknownForObject(TextLines);
                return VSConstants.S_OK;
            }
            else
            {
                ppv = IntPtr.Zero;
                return VSConstants.E_NOINTERFACE;
            }
        }

        public int SetFramePos(VSSETFRAMEPOS dwSFP, ref Guid rguidRelativeTo, int x, int y, int cx, int cy)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetGuidProperty(int propid, ref Guid rguid)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetProperty(int propid, object var)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Show()
        {
            return VSConstants.S_OK;
        }

        public int ShowNoActivate()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
