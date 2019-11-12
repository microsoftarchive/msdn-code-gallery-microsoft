/****************************** Module Header ******************************\
* Module Name: LoginPage.aspx.cs
* Project:     CSASPNETAutoRedirectLoginPage
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to develop an asp.net code-sample that will be 
* redirect to login page when page Session is expired or time out automatically. 
* It will ask the user to extend the Session at one minutes before
* expiring. If the user does not has any actions, the web page will be redirected
* to login page automatically, and also note that it need to work in one or
* more browser's tabs. 
* 
* The login page use to login in and prevent users who want to skip the login
* step by visit specified pages.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
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

namespace CSASPNETAutoRedirectLoginPage
{
    public partial class LoginPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Prevent the users who try to skip the login step by visit specified page.
            if (!Page.IsPostBack)
            {
                Session.Abandon();
            }
            if (Request.QueryString["info"] != null)
            {
                string message = Request.QueryString["info"].ToString();
                if (message == "0")
                {
                    Response.Write("<strong>you need login first to visit user page.</strong>");
                }
            }
        }

        /// <summary>
        /// User login method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = tbUserName.Text.Trim();
            if (tbUserName.Text.Trim() == "username" && tbPassword.Text.Trim() == "password")
            {
                Session["username"] = username;
                Response.Redirect("UserPage2.aspx");
            }
            else
            {
                Response.Write("<strong>User name or pass word error.</strong>");
            }
        }
    }
}