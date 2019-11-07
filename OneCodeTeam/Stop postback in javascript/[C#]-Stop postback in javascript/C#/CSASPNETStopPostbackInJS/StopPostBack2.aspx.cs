/****************************** Module Header ******************************\
* Module Name: StopPostBack2.aspx.cs
* Project:     CSASPNETStopPostbackInJS
* Copyright (c) Microsoft Corporation
*
* This page contains a client button and a hidden button. 
* Use JavasCript code to invoke hidden button onclick event or not.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/


using System;

namespace CSASPNETStopPostbackInJS
{
    public partial class StopPostBack2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCausePostback_Click(object sender, EventArgs e)
        {
            // Postbacks code
            textDisplay.Value += " This is a server click";
        }
    }
}