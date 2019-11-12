/****************************** Module Header ******************************\
* Module Name: DefaultPage.ascx.cs
* Project:     CSASPNETPreventMultipleWindows
* Copyright (c) Microsoft Corporation
*
* This is a user-control for start page.It will get a random  
* string and assign it to window.name.At last,jump to 
* Main.aspx page.
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
    public partial class DefaultPage : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This method will get a random string when it been invoked,
        /// and stored it in session
        /// </summary>
        /// <returns>return this random string</returns>
        public string GetWindowName()
        {
            string WindowName = Guid.NewGuid().ToString().Replace("-", "");
            Session["WindowName"] = WindowName;
            return WindowName;
        }
    }
}