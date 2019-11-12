using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace AZURE_QueryingWebPart.AZURE_QueryingWebPart
{
    public partial class AZURE_QueryingWebPartUserControl : UserControl
    {
        //This is the query to run. 
        const string QUERY_STRING = "SELECT * FROM Products.dbo.Products;";
        //This is the connection string. Make sure the servername include the tcp
        //protocol, and the name of your SQL Azure server, including ".database.windows.net". 
        //Edit the username and password.
        //Also note the longer connection timeout.
        const string CONNECTION_STRING = "Server=tcp:myservername.database.windows.net;Database=Products;User ID=Administrator;Password=PASSWORD;Trusted_Connection=False;Encrypt=True;Connection Timeout=30";

        private DataSet productsDataset = new DataSet();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void lnkbtnGetProducts_Click(object sender, EventArgs e)
        {
            //Create a connection to SQL Azure
            using (SqlConnection sqlConnection = new SqlConnection(CONNECTION_STRING))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                //Specify the select query
                dataAdapter.SelectCommand = new SqlCommand(QUERY_STRING, sqlConnection);
                //Get the data
                dataAdapter.Fill(productsDataset);
                //Bind the data adaptor to the datagrid control
                datagridProducts.DataSource = productsDataset;
                datagridProducts.DataBind();
            }
        }
    }
}
