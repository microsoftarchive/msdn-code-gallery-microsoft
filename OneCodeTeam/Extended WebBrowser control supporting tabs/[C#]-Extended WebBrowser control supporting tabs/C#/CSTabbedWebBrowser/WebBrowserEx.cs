/****************************** Module Header ******************************\
* Module Name:  WebBrowserEx.cs
* Project:	    CSTabbedWebBrowser
* Copyright (c) Microsoft Corporation.
* 
* This WebBrowserEx class inherits WebBrowser class and can handle NewWindow3 event.
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
using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.Win32;


namespace CSTabbedWebBrowser
{
    public partial class WebBrowserEx : WebBrowser
    {

        AxHost.ConnectionPointCookie cookie;

        DWebBrowserEvent2Helper helper;

        public event EventHandler<WebBrowserNewWindowEventArgs> NewWindow3;

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public WebBrowserEx()
        {
        }

        /// <summary>
        /// Associates the underlying ActiveX control with a client that can 
        /// handle control events including NewWindow3 event.
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void CreateSink()
        {
            base.CreateSink();

            helper = new DWebBrowserEvent2Helper(this);
            cookie = new AxHost.ConnectionPointCookie(
                this.ActiveXInstance, helper, typeof(DWebBrowserEvents2));
        }


        /// <summary>
        /// Releases the event-handling client attached in the CreateSink method
        /// from the underlying ActiveX control
        /// </summary>
        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void DetachSink()
        {
            if (cookie != null)
            {
                cookie.Disconnect();
                cookie = null;
            }
            base.DetachSink();
        }


        /// <summary>
        ///  Raises the NewWindow3 event.
        /// </summary>
        protected virtual void OnNewWindow3(WebBrowserNewWindowEventArgs e)
        {
            if (this.NewWindow3 != null)
            {
                this.NewWindow3(this, e);
            }
        }
    }
}
