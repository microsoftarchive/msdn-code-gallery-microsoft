/****************************** Module Header ******************************\
* Module Name:  HTMLEventHandler.cs
* Project:	    CSBrowserHelperObject
* Copyright (c) Microsoft Corporation.
* 
* This ComVisible class HTMLEventHandler can be assigned to the event properties
* of DispHTMLDocument interfaces, like oncontextmenu, onclick and so on. It can
* also be used in other HTMLElements.
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

using System.Runtime.InteropServices;

namespace CSBrowserHelperObject
{

    // The delegate of the handler method.
    public delegate void HtmlEvent(mshtml.IHTMLEventObj e);

    [ComVisible(true)]
    public class HTMLEventHandler
    {

        private mshtml.HTMLDocument htmlDocument;

        public event HtmlEvent eventHandler;

        public HTMLEventHandler(mshtml.HTMLDocument htmlDocument)
        {
            this.htmlDocument = htmlDocument;
        }

        [DispId(0)]
        public void FireEvent()
        {
            this.eventHandler(this.htmlDocument.parentWindow.@event);
        }
    }
}
