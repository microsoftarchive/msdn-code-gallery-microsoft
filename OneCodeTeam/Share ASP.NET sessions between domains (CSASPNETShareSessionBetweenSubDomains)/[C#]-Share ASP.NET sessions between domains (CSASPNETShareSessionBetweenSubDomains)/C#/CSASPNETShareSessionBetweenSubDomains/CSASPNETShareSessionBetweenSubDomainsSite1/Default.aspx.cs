/****************************** Module Header ******************************\
* Module Name:    Default.aspx.cs
* Project:        CSASPNETShareSessionBetweenSubDomainsSite1
* Copyright (c) Microsoft Corporation
*
* This project demonstrates how to configure a SQL Server as SessionState and 
* make a module to share Session between two Web Sites with the same root domain.
* 
* This page is used to set value to Session and read value from Session to test 
* if Session value has been change by Web Site 2 or not.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/

using System;

namespace CSASPNETShareSessionBetweenSubDomainsSite1
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Display Session in Web Site 1.
            lbMsg.Text = (string)Session["MySession"];
        }

        protected void btnSetSession_Click(object sender, EventArgs e)
        {
            // Set Session in Web Site 1.
            Session["MySession"] = "The Session content from Site1.";
        }
    }
}