/****************************** Module Header ******************************\
Module Name:  ContentPage.aspx.cs
Project:      CSASPNETMasterPage
Copyright (c) Microsoft Corporation.

This page is the content page of Master.master page.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL
All other rights reserved.
 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CSASPNETMasterPage
{
    public partial class ContentPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Label lbMasterPageHello = Master.FindControl("lbHello") as Label;

            if (lbMasterPageHello != null)
            {
                lbMasterPageHello.Text = "Hello, " + txtName.Text + "!";
            }
        }
    }
}