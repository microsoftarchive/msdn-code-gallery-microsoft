/****************************** Module Header ******************************\
* Module Name:  HTMLDocumentEventHelper.cs
* Project:	    CSBrowserHelperObject
* Copyright (c) Microsoft Corporation.
* 
* This ComVisible class HTMLDocumentEventHelper is used to set the event handler
* of the HTMLDocument. The interface DispHTMLDocument defines many events like 
* oncontextmenu, onclick and so on, and these events could be set to an
* HTMLEventHandler instance.
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
using mshtml;

namespace CSBrowserHelperObject
{
    [ComVisible(true)]
    public class HTMLDocumentEventHelper
    {
        private HTMLDocument document;

        public HTMLDocumentEventHelper(HTMLDocument document)
        {
            this.document = document;
        }

        public event HtmlEvent oncontextmenu
        {
            add
            {
                DispHTMLDocument dispDoc = this.document as DispHTMLDocument;

                object existingHandler = dispDoc.oncontextmenu;
                HTMLEventHandler handler = existingHandler is HTMLEventHandler ?
                    existingHandler as HTMLEventHandler : 
                    new HTMLEventHandler(this.document);

                // Set the handler to the oncontextmenu event.
                dispDoc.oncontextmenu = handler;

                handler.eventHandler += value;
            }
            remove
            {
                DispHTMLDocument dispDoc = this.document as DispHTMLDocument;
                object existingHandler = dispDoc.oncontextmenu;

                HTMLEventHandler handler = existingHandler is HTMLEventHandler ?
                    existingHandler as HTMLEventHandler : null;

                if (handler != null)
                    handler.eventHandler -= value;
            }
        }
    }

}
