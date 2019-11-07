/****************************** Module Header ******************************\
* Module Name:  Login.aspx.cs
* Project:      CSASPNETAutoLogin
* Copyright (c) Microsoft Corporation.
* 
* This page is used to display the user's login information.
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

namespace CSASPNETAutoLogin
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Write("You have login the website." + "<br>");
            Response.Write("Your userName：" + Request.Form["UserName"].ToString() + "<br>");
            Response.Write("Your passWord：" + Request.Form["Password"].ToString() + "<br>");
            Response.End();

        }
    }
}
