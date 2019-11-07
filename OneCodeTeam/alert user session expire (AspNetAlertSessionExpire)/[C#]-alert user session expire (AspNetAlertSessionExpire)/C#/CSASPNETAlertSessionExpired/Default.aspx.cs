/****************************** Module Header ******************************\
* Module Name:    Default.aspx.cs
* Project:        CSASPNETAlertSessionExpired
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to design a simple user control, which is used to 
* alert the user when the session is about to expired. 
* 
* In this file, get the session state.
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
using System.Web.UI;
using CSASPNETAlertSessionExpired.Util;

namespace CSASPNETAlertSessionExpired
{
    public partial class _Default : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        #region Get Session State

        protected void SessionState_Click(object sender, EventArgs e)
        {
            if (Session["SessionForTest"] == null)
            {

                Response.Redirect("SessionExpired.htm");
            }
            else
            {
                lbSessionState.Text = SessionStates.Exist.ToString();

            }
        }

        #endregion

      
    }
}

    
