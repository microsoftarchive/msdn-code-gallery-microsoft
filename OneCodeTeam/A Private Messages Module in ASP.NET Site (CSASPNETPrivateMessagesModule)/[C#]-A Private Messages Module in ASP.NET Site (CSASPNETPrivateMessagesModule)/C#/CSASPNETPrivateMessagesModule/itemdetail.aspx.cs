/****************************** Module Header ******************************\
* Module Name:    itemdetail.aspx.cs
* Project:        CSASPNETPrivateMessagesModule
* Copyright (c) Microsoft Corporation
*
* This Page is used to show the detail of the message
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
using System.Web.Security;


namespace CSASPNETPrivateMessagesModule
{
    public partial class itemdetail : System.Web.UI.Page
    {
        string strFrom = string.Empty;      // fromuser
        string strTo = string.Empty;        // touser
        string strTitle = string.Empty;     // title
        string strContent = string.Empty;   // content
        string strTime = string.Empty;      // createtime
        int id = 0;                         // messageid

        protected void Page_Load(object sender, EventArgs e)
        {
            MembershipUser currentUser;     // Current User
            currentUser = Membership.GetUser();

            if (!string.IsNullOrEmpty(Request.QueryString["ID"]))
            {
                id = Convert.ToInt32(Request.QueryString["ID"]);

                // Query string
                string queryString = "SELECT [MessageID], [FromUserId], [ToUserId], [Message], "
                    + "[CreateDate], [MessageTitle], [State] FROM [MessageTable] "
                    + " where MessageID=@id and (ToUserId=@userid or FromUserId=@userid)";

                using (SqlConnection connection = new SqlConnection(Common.ConnetionString))
                {
                    // sql command
                    SqlCommand command = new SqlCommand(queryString, connection);

                    #region BasicInfo
                    SqlParameter para1 = new SqlParameter("id", id);
                    SqlParameter para2 = new SqlParameter("userId", currentUser.ProviderUserKey);
                    command.Parameters.Add(para1);
                    command.Parameters.Add(para2);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        strFrom = reader["FromUserId"].ToString();
                        strTitle = reader["MessageTitle"].ToString();
                        strContent = reader["Message"].ToString();
                        strTime = reader["CreateDate"].ToString();

                    }
                    reader.Close();
                    #endregion

                    #region Get the recipients
                    queryString = "SELECT ToUserId FROM MessageTable where CreateDate = @CreateDate"
                        + " AND FromUserId = @FromUserId";
                    para1 = new SqlParameter("CreateDate", Convert.ToDateTime(strTime));
                    para2 = new SqlParameter("FromUserId", (object)strFrom);
                    command.Parameters.Clear();
                    command.CommandText = queryString;
                    command.Parameters.Add(para1);
                    command.Parameters.Add(para2);
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            strTo += ";" + Common.getUserNameByProviderUserKey(reader[0].ToString());
                        }
                    }
                    if (strTo.Length > 0)
                    {
                        strTo = strTo.Substring(1);
                    }
                    reader.Close();
                    #endregion

                    connection.Close();
                }
            }

            #region bind to control
            ltrContent.Text = strContent;
            ltrFrom.Text = Common.getUserNameByProviderUserKey(strFrom);
            ltrTo.Text = strTo;
            ltrTime.Text = strTime;
            ltrTitle.Text = strTitle;
            #endregion
        }

        protected void btnReply_Click(object sender, EventArgs e)
        {
            Response.Redirect("NewItem.aspx?flag=1&rid=" + id);
        }

        protected void btnReplyAll_Click(object sender, EventArgs e)
        {
            Response.Redirect("NewItem.aspx?flag=2&rid=" + id);
        }
    }
}
