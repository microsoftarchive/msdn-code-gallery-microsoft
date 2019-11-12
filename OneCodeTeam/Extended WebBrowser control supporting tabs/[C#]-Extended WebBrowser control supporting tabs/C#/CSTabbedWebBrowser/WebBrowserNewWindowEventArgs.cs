/****************************** Module Header ******************************\
* Module Name:  WebBrowserNavigateErrorEventArgs.cs
* Project:	    CSTabbedWebBrowser
* Copyright (c) Microsoft Corporation.
* 
* The class WebBrowserNavigateErrorEventArgs defines the event arguments used
* by WebBrowserEx.NewWindow3 event.
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

namespace CSTabbedWebBrowser
{

    public class WebBrowserNewWindowEventArgs : EventArgs
    {
        public String Url { get; set; }

        public Boolean Cancel { get; set; }

        public WebBrowserNewWindowEventArgs(String url, Boolean cancel)
        {
            this.Url = url;
            this.Cancel = cancel;
        }

    }
}
