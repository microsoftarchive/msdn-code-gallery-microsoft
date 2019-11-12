/****************************** Module Header ******************************\
* Module Name: AutoRedirect.ascx.cs
* Project:     CSASPNETAutoRedirectLoginPage
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to develop an asp.net code-sample that will 
* automatically redirect to login page when page Session is expired or 
* time out. It will ask the user to extend the Session at 1 minutes before
* expiring. If the user doesn't has any actions, the web page will redirect 
* to login page automatically, and also note that it need to work in one or
* more browser's tabs. 
* 
* The user control use to check the user session is expired or time out,
* and update the expired time when users has new behaviour. 
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
using System.Globalization;

namespace CSASPNETAutoRedirectLoginPage.UserControl
{
    public partial class AutoRedirect : System.Web.UI.UserControl
    {
        public string LoginDate;
        public string ExpressDate;
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check session is expire or timeout.
            if (Session["username"] == null)
            {
                Response.Redirect("LoginPage.aspx?info=0");
            }

            // Get user login time or last activity time.
            DateTime date = DateTime.Now;
            LoginDate = date.ToString("u", DateTimeFormatInfo.InvariantInfo).Replace("Z", "");
            int sessionTimeout = Session.Timeout;
            DateTime dateExpress = date.AddMinutes(sessionTimeout);
            ExpressDate = dateExpress.ToString("u", DateTimeFormatInfo.InvariantInfo).Replace("Z", "");
        }
    }
}