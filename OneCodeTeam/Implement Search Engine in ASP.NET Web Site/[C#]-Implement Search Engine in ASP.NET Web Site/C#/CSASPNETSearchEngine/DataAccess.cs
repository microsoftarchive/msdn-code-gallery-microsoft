/****************************** Module Header ******************************\
* Module Name:    DataAccess.cs
* Project:        CSASPNETSearchEngine
* Copyright (c) Microsoft Corporation
*
* This class is a Data Access Layer which does database operations.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

namespace CSASPNETSearchEngine
{
    /// <summary>
    /// This class is used to access database.
    /// </summary>
    public class DataAccess
    {
        /// <summary>
        /// Retrieve an individual record from database.
        /// </summary>
        /// <param name="id">Record id</param>
        /// <returns>A found record</returns>
        public Article GetArticle(long id)
        {
            List<Article> articles = QueryList("select * from [Articles] where [ID] = " + id.ToString());

            // Only return the first record.
            if (articles.Count > 0)
            {
                return articles[0];
            }
            return null;
        }

        /// <summary>
        /// Retrieve all records from database.
        /// </summary>
        /// <returns></returns>
        public List<Article> GetAll()
        {
            return QueryList("select * from [Articles]");
        }

        /// <summary>
        /// Search records from database.
        /// </summary>
        /// <param name="keywords">the list of keywords</param>
        /// <returns>all found records</returns>
        public List<Article> Search(List<string> keywords)
        {
            // Generate a complex Sql command.
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("select * from [Articles] where ");
            foreach (string item in keywords)
            {
                sqlBuilder.AppendFormat("([Title] like '%{0}%' or [Content] like '%{0}%') and ", item);
            }

            // Remove unnecessary string " and " at the end of the command.
            string sql = sqlBuilder.ToString(0, sqlBuilder.Length - 5);

            return QueryList(sql);
        }

        #region Helpers

        /// <summary>
        /// Create a connected SqlCommand object.
        /// </summary>
        /// <param name="cmdText">Command text</param>
        /// <returns>SqlCommand object</returns>
        protected SqlCommand GenerateSqlCommand(string cmdText)
        {
            // Read Connection String from web.config file.
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDatabaseConnectionString"].ConnectionString);
            SqlCommand cmd = new SqlCommand(cmdText, conn);
            cmd.Connection.Open();
            return cmd;
        }

        /// <summary>
        /// Create an Article object from a SqlDataReader object.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected Article ReadArticle(SqlDataReader reader)
        {
            Article article = new Article();

            article.ID = (long)reader["ID"];
            article.Title = (string)reader["Title"];
            article.Content = (string)reader["Content"];

            return article;
        }

        /// <summary>
        /// Excute a Sql command.
        /// </summary>
        /// <param name="cmdText">Command text</param>
        /// <returns></returns>
        protected List<Article> QueryList(string cmdText)
        {
            List<Article> articles = new List<Article>();

            SqlCommand cmd = GenerateSqlCommand(cmdText);
            using (cmd.Connection)
            {
                SqlDataReader reader = cmd.ExecuteReader();

                // Transform records to a list.
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        articles.Add(ReadArticle(reader));
                    }
                }
            }
            return articles;
        }

        #endregion
    }
}