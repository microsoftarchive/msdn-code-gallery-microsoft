/****************************** Module Header ******************************\
* Module Name:    MessageList.aspx.cs
* Project:        CSASPNETPrivateMessagesModule
* Copyright (c) Microsoft Corporation
*
* This page is used to show the message list
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Data.SqlClient;
using System.Data;
using System.Web.Security;

namespace CSASPNETPrivateMessagesModule
{
    public partial class MessageList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MembershipUser currentUser = Membership.GetUser();// Current User          

            string strFlag = Request.QueryString["flag"];
            if (!IsPostBack)
            {
                // Query string
                string queryString = "SELECT [MessageID], [FromUserId], [ToUserId], [Message], "
                    + "[CreateDate], [MessageTitle], [State] FROM [MessageTable]";

                // The status field has two values, the normal message is 1 while a draft is 0.
                switch (strFlag)
                {
                    case "0": // Draft
                        queryString += " where state=0 and FromUserId=@UserId";
                        break;
                    case "1": // Inbox
                        queryString += " where state=1 and ToUserId=@UserId";
                        break;
                    case "2": // Outbox
                        queryString += " where state=1 and FromUserId=@UserId";
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(strFlag))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    DataSet sqlData = new DataSet();

                    using (SqlConnection connection = new SqlConnection(Common.ConnetionString))
                    {
                        // Set query string
                        adapter.SelectCommand = new SqlCommand(queryString, connection);
                        SqlParameter para = new SqlParameter("UserId", currentUser.ProviderUserKey);
                        adapter.SelectCommand.Parameters.Add(para);

                        // Open connection
                        connection.Open();

                        // Sql data is stored DataSet.
                        adapter.Fill(sqlData, "MessageTable");

                        // Close connection
                        connection.Close();
                    }

                    // Bind datatable to GridView
                    gdvView.DataSource = sqlData.Tables[0];
                    gdvView.DataBind();
                }
            }
        }

    }
}
