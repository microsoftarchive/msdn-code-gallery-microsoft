/****************************** Module Header ******************************\
* Module Name:    Default.aspx.cs
* Project:        CSASPNETRatingControlSelectCurrentValue
* Copyright (c) Microsoft Corporation
*
* This sample will demonstrate you how to solve the problem that using the Ajax 
* Rating control to select the currently selected option. Because the OnChanged 
* event doesn't trigger resulted in the user cannot select the currently selected 
* items. The sample will load a list of books, when the user clicks the link in 
* one of the records, we can see the rating corresponds to the current record books. 
* When the user clicks on the current rating, the database will use the current 
* rating to insert a new record.
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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace CSASPNETRatingControlSelectCurrentValue
{
    public partial class _Default : System.Web.UI.Page
    {
        // SQL connection.
        static string connetionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        SqlConnection conn = new SqlConnection(connetionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            // Bind data.
            BindData();
        }

        /// <summary>
        /// Bind data to gdvBooks.
        /// </summary>
        private void BindData()
        {
            SqlDataAdapter sda = new SqlDataAdapter("select * from bookInfo", conn);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            gdvBooks.DataSource = ds;
            gdvBooks.DataBind();
        }

        /// <summary>
        /// Get the rating of the selected item and then insert it into the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Store the rating of the selected item.
            int intRate = 0;

            switch (Rating1.CurrentRating)
            {
                case 1:
                    intRate = 1;
                    break;
                case 2:
                    intRate = 2;
                    break;
                case 3:
                    intRate = 3;
                    break;
                case 4:
                    intRate = 4;
                    break;
                case 5:
                    intRate = 5;
                    break;
            }

            try
            {
                // Insert a new record.
                insertdataintosql("test1", "Microsoft", intRate);
            }
            catch (DataException ee)
            {
                lbResponse.Text = ee.Message;
                lbResponse.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                // Bind data.
                BindData();
            }
        }

        /// <summary>
        /// Bind the rating of the currently selected item to control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gdvBooks_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("RateDetail", StringComparison.OrdinalIgnoreCase))
            {
                LinkButton lb = (LinkButton)e.CommandSource; // Get control according to CommandSource

                string s = lb.CommandArgument;
                Rating1.CurrentRating = Convert.ToInt32(lb.CommandArgument);
            }
        }

        /// <summary>
        /// Insert data to database.
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="Author">Author</param>
        /// <param name="Rate">Rate</param>
        public void insertdataintosql(string name, string Author, int Rate)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "insert into bookInfo(name,Author,Rate) values(@name,@Author,@Rate)";
            cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
            cmd.Parameters.Add("@Author", SqlDbType.NVarChar).Value = Author;
            cmd.Parameters.Add("@Rate", SqlDbType.Int).Value = Rate;
            cmd.CommandType = CommandType.Text;
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}


