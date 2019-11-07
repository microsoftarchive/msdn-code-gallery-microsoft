using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;


namespace AZURE_UpdatingWebPart.AZURE_UpdatingWebPart
{
    //This class uses standard ADO.NET classes to update a record
    //in SQL Azure. The only difference between this and the
    //same operation on-premise is the Connection string. 
    //Before you use this example, ensure that you have a database
    //in your SQL Azure server and edit the connection string to 
    //match it.
    public partial class AZURE_UpdatingWebPartUserControl : UserControl
    {
        //This is the query that selects data
        const string QUERY_STRING = "SELECT * FROM Products.dbo.Products;";
        //This is the connection string. Make sure the servername include the tcp
        //protocol, and the name of your SQL Azure server, including ".database.windows.net". 
        //Edit the username and password.
        //Also note the longer connection timeout.
        const string CONNECTION_STRING = "Server=tcp:myserver.database.windows.net;Database=Products;User ID=administrator;Password=password;Trusted_Connection=False;Encrypt=True;Connection Timeout=30";

        private SqlConnection sqlConnection;
        private DataSet productsDataset = new DataSet();
        private SqlDataAdapter dataAdapter;
        private SqlCommandBuilder commandBuilder;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Create a connection to SQL Azure
            sqlConnection = new SqlConnection(CONNECTION_STRING);
            dataAdapter = new SqlDataAdapter();
            //Specify the select query
            dataAdapter.SelectCommand = new SqlCommand(QUERY_STRING, sqlConnection);
            //Create the command builder so we can use an auto-generated INSERT command
            commandBuilder = new SqlCommandBuilder(dataAdapter);
            //Get the data
            dataAdapter.Fill(productsDataset);
            //Bind the data adaptor to the datagrid control
            datagridProducts.DataSource = productsDataset;
            datagridProducts.DataBind();
        }

        protected void lnkbtnDelete_Click(object sender, EventArgs e)
        {
            //Get the Products table.
            DataTable productsTable = productsDataset.Tables[0];
            //Get the first record
            DataRow firstProduct = productsTable.Rows[0];
            firstProduct["ProductDescription"] = "This product is out of stock";
            try
            {
                //Execute the command against the database
                dataAdapter.Update(productsTable);
                //Refresh the datagrid
                datagridProducts.DataBind();
                labelResults.Text = "First record updated successfully";
            }
            catch (Exception ex)
            {
                labelResults.Text = "Record update caused an error: " + ex.Message;
            }
        }
    }
}
