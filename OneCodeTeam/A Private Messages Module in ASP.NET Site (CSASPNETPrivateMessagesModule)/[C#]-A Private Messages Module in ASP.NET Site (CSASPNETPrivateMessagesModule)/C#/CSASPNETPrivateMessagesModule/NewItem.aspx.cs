/****************************** Module Header ******************************\
* Module Name:    NewItem.aspx.cs
* Project:        CSASPNETPrivateMessagesModule
* Copyright (c) Microsoft Corporation
*
* This page is used to reply or send new item
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
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Data.SqlClient;


namespace CSASPNETPrivateMessagesModule
{
    public partial class NewItem : System.Web.UI.Page
    {
        int rid = 0;                                    // The message to reply
        int state = 1;                                  // The status of the sent message 
        string strFrom = string.Empty;                  // fromuser
        string strTo = string.Empty;                    // touser
        string strTitle = string.Empty;                 // title
        string strContent = string.Empty;               // content
        string strTime = string.Empty;                  // createtime
        string queryString = string.Empty;              // Query string
        SqlCommand command = null;                             // sql commmand
        SqlParameter para1 = null;                             // SqlParameter
        SqlParameter para2 = null;

        SqlDataReader reader = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["rid"]))
            {
                rid = Convert.ToInt32(Request.QueryString["rid"]);

                // The flag of reply and reply all.
                string flag = Request.QueryString["flag"];

                // According to the flag, get the recipient list
                switch (flag)
                {
                    case "1":   // reply
                        GetBasicInfo();
                        strTo = Common.getUserNameByProviderUserKey(strFrom);
                        break;
                    case "2":   // replyall
                        GetBasicInfo();
                        GetReplyAllTo();
                        break;
                    default:
                        break;
                }

                strContent = "From: " + Common.getUserNameByProviderUserKey(strFrom)
                    + "\nSent: " + strTime
                    + "\nTo: " + strTo
                    + "\nTitle: " + strTitle
                    + "\nContent: " + strContent;

                strTitle = "Re:" + strTitle;
            }

            if (!IsPostBack)
            {
                BindUser();

                #region bind to control
                txtContent.Text = strContent;
                txtTo.Text = strTo;
                txtTitle.Text = strTitle;
                #endregion
            }
        }

        /// <summary>
        /// Get recipient list of reply all
        /// </summary>
        private void GetReplyAllTo()
        {
            using (SqlConnection connection = new SqlConnection(Common.ConnetionString))
            {
                queryString = "SELECT ToUserId from MessageTable where CreateDate=@create and FromUserId=@FromUserId";
                command = new SqlCommand(queryString, connection);
                para1 = new SqlParameter("create", strTime);
                para2 = new SqlParameter("FromUserId", strFrom);
                command.Parameters.Clear();
                command.CommandText = queryString;
                command.Parameters.Add(para1);
                command.Parameters.Add(para2);

                connection.Open();
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
                connection.Close();
            }
        }

        /// <summary>
        /// Get the basic infomation of the message
        /// </summary>
        private void GetBasicInfo()
        {
            MembershipUser currentUser; // Current User
            currentUser = Membership.GetUser();

            // Query string
            queryString = "SELECT [MessageID], [FromUserId], [ToUserId], [Message], [CreateDate],"
                + " [MessageTitle], [State] FROM [MessageTable] "
                + "where MessageID=@id and (ToUserId=@userid or FromUserId=@userid)";
            using (SqlConnection connection = new SqlConnection(Common.ConnetionString))
            {
                command = new SqlCommand(queryString, connection);
                para1 = new SqlParameter("id", rid);
                para2 = new SqlParameter("userId", currentUser.ProviderUserKey);
                command.Parameters.Add(para1);
                command.Parameters.Add(para2);
                connection.Open();
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    strFrom = reader["FromUserId"].ToString();
                    strTitle = reader["MessageTitle"].ToString();
                    strContent = reader["Message"].ToString();
                    strTime = reader["CreateDate"].ToString();
                }
                reader.Close();
                connection.Close();
            }
        }

        /// <summary>
        /// Get all MembershipUser
        /// </summary>
        private void BindUser()
        {
            MembershipUserCollection muc = Membership.GetAllUsers();
            chlUser.DataSource = muc;
            chlUser.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string strTo = string.Empty;

            // Loop the users and add the selected user to recipient list
            foreach (ListItem item in this.chlUser.Items)
            {
                if (item.Selected)
                {
                    strTo += ";" + item.Text;
                }
            }

            if (strTo.Length > 0)
            {
                txtTo.Text = strTo.Substring(1);
            }
        }

        // Send message
        protected void btnEnter_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                SaveToDB();
            }
        }

        /// <summary>
        /// Save the message to Database 
        /// </summary>
        private void SaveToDB()
        {
            string strSql = "";
            // Get recipient list
            string[] tos = txtTo.Text.Split(';');

            // Save message for everyone in the recipient list
            for (int i = 0; i < tos.Length; i++)
            {
                strSql += "insert into MessageTable(FromUserId,ToUserId,Message,MessageTitle,CreateDate,state)"
                    + " values('" + Membership.GetUser().ProviderUserKey + "','"
                    + Common.getUserKeyByUserName(tos[i]) + "','" + txtContent.Text.Trim()
                    + "','" + txtTitle.Text.Trim() + "','" + DateTime.Now.ToString() + "'," + state + ");";
            }

            using (SqlConnection connection = new SqlConnection(Common.ConnetionString))
            {
                connection.Open();
                SqlTransaction tran = connection.BeginTransaction();
                try
                {

                    SqlCommand cmd = new SqlCommand(strSql, connection, tran);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (Exception e)
                {
                    tran.Rollback();
                }
                finally
                {
                    connection.Close();
                    Response.Redirect("messagelist.aspx?flag=2");
                }
            }
        }

        /// <summary>
        /// Save draft
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDraft_Click(object sender, EventArgs e)
        {
            state = 0; // This is the value of draft
            SaveToDB();
        }
    }
}
