/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    class MockWebBrowsingService : IVsWebBrowsingService
    {
        public class NavigateEventArgs : EventArgs
        {
            public readonly string Url;
            public NavigateEventArgs(string url)
            {
                Url = url;
            }
        }

        public event EventHandler<NavigateEventArgs> OnNavigate;

        #region IVsWebBrowsingService Members

        public int CreateExternalWebBrowser(uint dwCreateFlags, VSPREVIEWRESOLUTION dwResolution, string lpszURL)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int CreateWebBrowser(uint dwCreateFlags, ref Guid rguidOwner, string lpszBaseCaption, string lpszStartURL, IVsWebBrowserUser pUser, out IVsWebBrowser ppBrowser, out IVsWindowFrame ppFrame)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int CreateWebBrowserEx(uint dwCreateFlags, ref Guid rguidPersistenceSlot, uint dwId, string lpszBaseCaption, string lpszStartURL, IVsWebBrowserUser pUser, out IVsWebBrowser ppBrowser, out IVsWindowFrame ppFrame)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetFirstWebBrowser(ref Guid rguidPersistenceSlot, out IVsWindowFrame ppFrame, out IVsWebBrowser ppBrowser)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetWebBrowserEnum(ref Guid rguidPersistenceSlot, out IEnumWindowFrames ppenum)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Navigate(string lpszURL, uint dwNaviageFlags, out IVsWindowFrame ppFrame)
        {
            if (OnNavigate != null)
            {
                OnNavigate(this, new NavigateEventArgs(lpszURL));
            }

            ppFrame = new MockWindowFrame();
            return VSConstants.S_OK;
        }

        #endregion
    }
}
