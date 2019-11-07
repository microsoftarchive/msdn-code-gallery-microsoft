/****************************** Module Header ******************************\
* Module Name:    SessionExpired.ascx.cs
* Project:        CSASPNETAlertSessionExpired
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to design a simple user control, which is used to 
* alert the user when the session is about to expired. 
* 
* In this file, we register the CallServer method to the client side.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.

* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Web.UI;
using CSASPNETAlertSessionExpired.Util;

namespace CSASPNETAlertSessionExpired.Controls
{
    /// <summary>
    /// Inherit the ICallbackEventHandler interface to realize async request.
    /// </summary>
    public partial class SessionExpired : System.Web.UI.UserControl,ICallbackEventHandler
    {
        /// <summary>
        /// Register the javascript codes to the client, which is benefit to the client to request
        /// the server for async.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["SessionForTest"] = "SessionForTest";
            }

            // Set the client method "client_ReceiveServerData" to receive the value
            // from the server.
            string reference = Page.ClientScript.GetCallbackEventReference(this, "", "clientReceiveServerData", "");
            
            // Register a client method "CallServer" to request the server for async.
            string callbackScript = "function clientCallServer()" +
                "{" + reference + ";}";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "clientCallServer", callbackScript, true);

            // Set the Session's timeout value to the client side. It will be assign to a HiddenField that 
            // it will not affect user use.
            hfTimeOut.Value = Session.Timeout.ToString();
            
        }


        #region The Callbackhandler

        /// <summary>
        /// You do not need to get any value from the server side.
        /// </summary>
        /// <returns></returns>
        public string GetCallbackResult()
        {
            return String.Empty;
        }

        /// <summary>
        /// Do not add any code for you just need to interactive with the server.
        /// </summary>
        /// <param name="eventArgument"></param>
        public void RaiseCallbackEvent(string eventArgument)
        {
           
        }

        #endregion

       
    }
}