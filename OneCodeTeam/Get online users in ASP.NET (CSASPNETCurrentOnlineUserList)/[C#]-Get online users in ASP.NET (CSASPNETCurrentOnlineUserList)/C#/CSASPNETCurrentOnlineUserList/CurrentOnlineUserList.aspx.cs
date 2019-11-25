/**************************** Module Header ********************************\
* Module Name:    CurrentOnlineUserList.aspx.cs
* Project:        CSASPNETCurrentOnlineUserList
* Copyright (c) Microsoft Corporation
*
* The Membership.GetNumberOfUsersOnline Method can get the number of online
* users,however many asp.net projects are not using membership.This project
* shows how to display a list of current online users' information without 
* using membership provider.

* This page is used to display the current online user's information. 

* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\***************************************************************************/

using System;
using System.Web.UI.WebControls;

namespace CSASPNETCurrentOnlineUserList
{
    public partial class CurrentOnlineUserList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check whether the user is login.
            CheckLogin();
        }
        public void CheckLogin()
        {
            string _userticket = "";
            if (Session["Ticket"] != null)
            {
                _userticket = Session["Ticket"].ToString();
            }
            if (_userticket != "")
            {
                // Initialize the datatable which used to store the information
                // of current online user.
                DataTableForCurrentOnlineUser _onlinetable = new DataTableForCurrentOnlineUser();

                // Check whether the user is online by using ticket.
                if (_onlinetable.IsOnline_byTicket(this.Session["Ticket"].ToString()))
                {
                    // Update the last active time.
                    _onlinetable.ActiveTime(Session["Ticket"].ToString());

                    // Bind the datatable which used to store the information of 
                    // current online user to gridview control.
                    gvUserList.DataSource = _onlinetable.ActiveUsers;
                    gvUserList.DataBind();
                }
                else
                {
                    // If the current User is not exist in the table,then redirect
                    // the page to LogoOut.
                    Response.Redirect("LogOut.aspx");
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
    }
}