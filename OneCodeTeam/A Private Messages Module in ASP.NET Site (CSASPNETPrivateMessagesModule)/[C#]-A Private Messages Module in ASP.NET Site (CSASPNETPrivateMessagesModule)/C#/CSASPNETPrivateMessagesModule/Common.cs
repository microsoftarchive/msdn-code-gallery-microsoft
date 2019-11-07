/****************************** Module Header ******************************\
* Module Name:    Common.cs
* Project:        CSASPNETPrivateMessagesModule
* Copyright (c) Microsoft Corporation
*
* This is the helper class
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System.Configuration;
using System.Data.SqlClient;


public class Common
{
    // Sql connection.
    public static string ConnetionString = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;    

    /// <summary>
    /// Get UserName By ProviderUserKey
    /// </summary>
    /// <param name="id">ProviderUserKey</param>
    /// <returns>UserName</returns>
    public static string getUserNameByProviderUserKey(string id)
    {
        // UserName
        string strName = string.Empty;

        // Query string
        string queryString = "SELECT UserName FROM [aspnet_Users] where UserID=@id";

        #region database Operation
        using (SqlConnection connection = new SqlConnection(ConnetionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            SqlParameter para2 = new SqlParameter("id", id);
            command.Parameters.Add(para2);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                strName = reader[0].ToString();
            }
            reader.Close();
            connection.Close();
        }
        #endregion

        return strName;
    }

    /// <summary>
    /// Get ProviderUserKey By UserName
    /// </summary>
    /// <param name="strName">UserName</param>
    /// <returns>ProviderUserKey</returns>
    public static string getUserKeyByUserName(string strName)
    {
        // ProviderUserKey
        string strUserKey = string.Empty;

        // Query string
        string queryString = "SELECT UserID FROM [aspnet_Users] where UserName=@Name";

        #region database Operation
        using (SqlConnection connection = new SqlConnection(ConnetionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            SqlParameter para2 = new SqlParameter("Name", strName);
            command.Parameters.Add(para2);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Read();
                strUserKey = reader[0].ToString();
            }
            reader.Close();
            connection.Close();
        }
        #endregion

        return strUserKey;
    }
}

