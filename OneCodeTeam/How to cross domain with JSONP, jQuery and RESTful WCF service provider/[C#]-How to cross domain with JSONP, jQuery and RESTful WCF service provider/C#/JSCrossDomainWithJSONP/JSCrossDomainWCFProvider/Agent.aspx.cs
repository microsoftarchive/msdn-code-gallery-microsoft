/****************************** Module Header ******************************\
Module Name:  Agent.cs
Project:      JSCrossDomainWCFProvider
Copyright (c) Microsoft Corporation.
	 
Agent page to invoke WCF service for client invoker
	 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx.
All other rights reserved.
	 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.IO;
using System.Net;

namespace JSCrossDomainWCFProvider
{
    /// <summary>
    /// Agent page to invoke WCF service
    /// </summary>
    public partial class Agent : System.Web.UI.Page
    {
        #region Constructor

        public Agent()
        {
            userRequestUri = "http://localhost:50500/UserService.svc/user";
        }

        #endregion

        #region Fields

        private readonly string userRequestUri = string.Empty;

        #endregion

        #region Methods

        /// <summary>
        /// Page load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Page_Load(object sender, EventArgs e)
        {
            string allUsersJson = GetAllUsers();
            if (string.IsNullOrEmpty(allUsersJson))
            {
                Response.Write(string.Empty);
                return;
            }

            Response.Write(allUsersJson);
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>Return client callBack name and json string</returns>
        private string GetAllUsers()
        {
            string allUsersUri = string.Format("{0}/all", userRequestUri);
            string callBack = Request.QueryString["callBack"];

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(allUsersUri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream webStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(webStream);
            string allUsersJson = reader.ReadToEnd();

            reader.Close();
            response.Close();

            return string.Format("{0}({1});", callBack, allUsersJson);
        }

        #endregion Methods
    }
}