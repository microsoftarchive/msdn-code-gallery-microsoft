/****************************** Module Header ******************************\
* Module Name:    Default.aspx.cs
* Project:        CSASPNETInfiniteLoading
* Copyright (c) Microsoft Corporation
*
* This project illustrates how to scroll down to load new page content based
* on the AJAX technology. 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;
using System.Text;

namespace CSASPNETInfiniteLoading
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        [WebMethod]
        public static string Foo()
        {
            StringBuilder getPostsText = new StringBuilder();
            using (DataSet ds = new DataSet())
            {
                ds.ReadXml(HttpContext.Current.Server.MapPath("App_Data/books.xml"));
                DataView dv = ds.Tables[0].DefaultView;

                foreach (DataRowView myDataRow in dv)
                {
                    getPostsText.AppendFormat("<p>author: {0}</br>", myDataRow["author"]);
                    getPostsText.AppendFormat("genre: {0}</br>", myDataRow["genre"]);
                    getPostsText.AppendFormat("price: {0}</br>", myDataRow["price"]);
                    getPostsText.AppendFormat("publish date: {0}</br>", myDataRow["publish_date"]);
                    getPostsText.AppendFormat("description: {0}</br></p>", myDataRow["description"]);
                }
                getPostsText.AppendFormat("<div style='height:15px;'></div>");

            }
            return getPostsText.ToString();
        }
    }
}
