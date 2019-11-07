/**************************** Module Header ********************************\
* Module Name:    Default.aspx
* Project:        CSASPNETDetectBrowserCloseEvent
* Copyright (c) Microsoft Corporation
*
* As we know, HTTP is a stateless protocol, the browser doesn't keep connecting
* to the server. When user try to close the browser using alt-f4, browser close(X) 
* and right click on browser and close -> this all is done and is working fine, 
* it's not possible to tell the server that the browser is closed. The sample 
* demonstrates how to detect the browser close event with iframe.

* This page is used to display the current online client's clientID
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
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

namespace CSASPNETDetectBrowserCloseEvent
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ClientInfo client = new ClientInfo();
            List<ClientInfo> clientList = (List<ClientInfo>)Application["ClientInfo"];
            client = ClientInfo.GetClinetInfoByClientID(clientList, this.Session.SessionID);

            // Update the ActiveTime
            client.ActiveTime = DateTime.Now;
 
            for (int i = 0; i < clientList.Count; i++)
            {
                Response.Write(clientList[i].ClientID.ToString());
                Response.Write("<br />");
            }
        }

    }
}