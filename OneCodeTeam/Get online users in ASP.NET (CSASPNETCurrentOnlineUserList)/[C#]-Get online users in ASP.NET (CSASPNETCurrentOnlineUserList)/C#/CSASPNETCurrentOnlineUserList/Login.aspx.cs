/**************************** Module Header ********************************\
* Module Name:    Login.aspx.cs
* Project:        CSASPNETCurrentOnlineUserList
* Copyright (c) Microsoft Corporation
*
* The Membership.GetNumberOfUsersOnline Method can get the number of online
* users,however many asp.net projects are not using membership.This project
* shows how to display a list of current online users' information without 
* using membership provider.

* This page is used to let user sign in. 

* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\***************************************************************************/

using System;
using System.Web.UI.WebControls;

namespace CSASPNETCurrentOnlineUserList
{
    public partial class Login : System.Web.UI.Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string _error;

            // Check the value of user's input data.
            if (check_text(out _error))
            {
                // Initialize the datatable which used to store the
                // information of current online user.
                DataTableForCurrentOnlineUser onLineTable = new DataTableForCurrentOnlineUser();

                // An instance of user's entity.
                UserEntity _user = new UserEntity();
                _user.Ticket = DateTime.Now.ToString("yyyyMMddHHmmss");
                _user.UserName = tbUserName.Text.Trim();
                _user.TrueName = tbTrueName.Text.Trim();
                _user.ClientIP = this.Request.UserHostAddress;
                _user.RoleID = "MingXuGroup";

                // Use session variable to store the ticket.
                this.Session["Ticket"] = _user.Ticket;

                // Log in.
                onLineTable.Login(_user, true);
                Response.Redirect("CurrentOnlineUserList.aspx");
            }
            else
            {
                this.lbMessage.Visible = true;
                this.lbMessage.Text = _error;
            }
        }
        public bool check_text(out string error)
        {
            error = "";
            if (this.tbUserName.Text.Trim() == "")
            {
                error = "Please enter the username";
                return false;
            }
            if (this.tbTrueName.Text.Trim() == "")
            {
                error = "Please enter the truename";
                return false;
            }
            return true;
        }
    }
}