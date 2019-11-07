/****************************** Module Header ******************************\
* Module Name: NextPage.ascx.cs
* Project:     CSASPNETPreventMultipleWindows
* Copyright (c) Microsoft Corporation
*
* This is a user-control for other page.It will get window name 
* and check whether allow this jump request
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
    public partial class NextPage : System.Web.UI.UserControl
    {
        public const string InvalidPage = "InvalidPage";
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This method can get window name from Default.aspx
        /// </summary>
        /// <returns></returns>
        public string GetWindowName()
        {
            if (Session["WindowName"] != null)
            {
                string WindowName = Session["WindowName"].ToString();
                return WindowName;
            }
            else
            {
                return InvalidPage;
            }
        }
    }
}