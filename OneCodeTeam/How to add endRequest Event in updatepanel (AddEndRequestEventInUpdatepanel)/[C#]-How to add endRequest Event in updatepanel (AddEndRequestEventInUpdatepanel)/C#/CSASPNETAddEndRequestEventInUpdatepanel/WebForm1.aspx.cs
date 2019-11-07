/****************************** Module Header ******************************\
* Module Name:    WebForm1.aspx.cs
* Project:        CSASPNETAddEndRequestEventInUpdatepanel
* Copyright (c) Microsoft Corporation
*
* The WebForm1 page shows the scene using the MasterPage, 
* which already has the ScriptManager control. This page 
* use the ScriptManagerProxy control.
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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CSASPNETAddEndRequestEventInUpdatepanel
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            grid1.BorderWidth = 2;
        }
    }
}
