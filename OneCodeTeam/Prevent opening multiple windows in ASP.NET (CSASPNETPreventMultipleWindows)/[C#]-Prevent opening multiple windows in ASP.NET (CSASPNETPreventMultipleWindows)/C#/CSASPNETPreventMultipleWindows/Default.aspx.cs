/****************************** Module Header ******************************\
* Module Name: Default.aspx.cs
* Project:     CSASPNETPreventMultipleWindows
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to detect and prevent multiple windows or 
* tab usage in Web Applications.When user want to open this link 
* in a new window or tab, erb application will reject these requests.This 
* approach will solve many security problems like sharing sessions,
* protect dupicated login,data concurrency,etc.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
\*****************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CSASPNETPreventMultipleWindows
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

    }
}