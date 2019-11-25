/**************************** Module Header ********************************\
* Module Name:    CheckUserOnlinePage.aspx
* Project:        CSASPNETCurrentOnlineUserList
* Copyright (c) Microsoft Corporation
*
* The Membership.GetNumberOfUsersOnline Method can get the number of online
* users,however many asp.net projects are not using membership.This project
* shows how to display a list of current online users' information without 
* using membership provider.

* This page is used to get request from other pages which contains the 
* CheckUserOnline custom control and delete the records of user who is off 
* line from CurrentOnlineUser table.

* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\***************************************************************************/

using System;
using System.Web.UI.WebControls;

namespace CSASPNETCurrentOnlineUserList
{
    public partial class CheckUserOnlinePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Check();
        }
        public virtual string SessionName
        {
            get
            {
                object _obj1 = this.ViewState["SessionName"];
                if (_obj1 != null) { return ((string)_obj1).Trim(); }
                return "Ticket";
            }
            set
            {
                this.ViewState["SessionName"] = value;
            }
        }
        protected void Check()
        {
            string _myTicket = "";
            if (System.Web.HttpContext.Current.Session[this.SessionName] != null)
            {
                _myTicket = System.Web.HttpContext.Current.Session[this.SessionName].ToString();
            }
            if (_myTicket != "")
            {
                // Initialize the datatable which used to store the information of
                // current online user.
                DataTableForCurrentOnlineUser _onlinetable = new DataTableForCurrentOnlineUser();

                // Update the time when the page refresh or the page get a request.
                _onlinetable.RefreshTime(_myTicket);
                Response.Write("OK：" + DateTime.Now.ToString());
            }
            else
            {
                Response.Write("Sorry：" + DateTime.Now.ToString());
            }
        }
    }
}