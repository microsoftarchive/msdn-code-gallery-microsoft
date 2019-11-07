/****************************** Module Header ******************************\
* Module Name:  WebBrowser2EventHelper.cs
* Project:	    CSTabbedWebBrowser
* Copyright (c) Microsoft Corporation.
* 
* The class WebBrowser2EventHelper is used to handle the NewWindow3 event 
* from the underlying ActiveX control by raising the NewWindow3 event 
* defined in class WebBrowserEx. 
* 
* Because of the protected method WebBrowserEx.OnNewWindow3, this
* class is defined inside the class WebBrowserEx.
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
using System.Runtime.InteropServices;

namespace CSTabbedWebBrowser
{
    public partial class WebBrowserEx
    {
        private class DWebBrowserEvent2Helper
            : StandardOleMarshalObject, DWebBrowserEvents2
        {
            private WebBrowserEx parent;

            public DWebBrowserEvent2Helper(WebBrowserEx parent)
            {
                this.parent = parent;
            }

            /// <summary>
            /// Raise the NewWindow3 event.
            /// If an instance of WebBrowser2EventHelper is associated with the underlying
            /// ActiveX control, this method will be called When the NewWindow3 event was
            /// fired in the ActiveX control.
            /// </summary>
            public void NewWindow3(ref object ppDisp, ref bool Cancel, uint dwFlags,
                string bstrUrlContext, string bstrUrl)
            {
                var e = new WebBrowserNewWindowEventArgs(bstrUrl, Cancel);
                this.parent.OnNewWindow3(e);
                Cancel = e.Cancel;
            }
        }
    }
}
