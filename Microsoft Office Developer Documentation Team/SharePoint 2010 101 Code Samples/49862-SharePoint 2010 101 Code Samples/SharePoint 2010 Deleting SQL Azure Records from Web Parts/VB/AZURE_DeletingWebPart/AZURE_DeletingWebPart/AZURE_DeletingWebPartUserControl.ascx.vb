Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts

'This class uses standard ADO.NET classes to delete a record
'from SQL Azure. The only difference between this and the
'same operation on-premise is the Connection string. 
'Before you use this example, ensure that you have a database
'in your SQL Azure server and edit the connection string to 
'match it.
Partial Public Class AZURE_DeletingWebPartUserControl
    Inherits UserControl

    'This is the query that selects data
    Const QUERY_STRING As String = "SELECT * FROM Products.dbo.Products;"
    'This is the connection string. Make sure the servername include the tcp
    'protocol, and the name of your SQL Azure server, including ".database.windows.net". 
    'Edit the username and password.
    'Also note the longer connection timeout.
    Const CONNECTION_STRING As String = "Server=tcp:myserver.database.windows.net;Database=Products;User ID=administrator;Password=password;Trusted_Connection=False;Encrypt=True;Connection Timeout=30"

    Private sqlConnection As SqlConnection
    Private productsDataset As DataSet = New DataSet()
    Private dataAdapter As SqlDataAdapter
    Private commandBuilder As SqlCommandBuilder

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        'Create a connection to SQL Azure
        sqlConnection = New SqlConnection(CONNECTION_STRING)
        dataAdapter = New SqlDataAdapter()
        'Specify the select query
        dataAdapter.SelectCommand = New SqlCommand(QUERY_STRING, sqlConnection)
        'Create the command builder so we can use an auto-generated INSERT command
        commandBuilder = New SqlCommandBuilder(dataAdapter)
        'Get the data
        dataAdapter.Fill(productsDataset)
        'Bind the data adaptor to the datagrid control
        datagridProducts.DataSource = productsDataset
        datagridProducts.DataBind()
    End Sub

    Protected Sub lnkbtnDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkbtnDelete.Click
        'Get the Products table.
        Dim productsTable As DataTable = productsDataset.Tables(0)
        'Delete the first record
        productsTable.Rows(0).Delete()
        Try
            'Execute the command against the database
            dataAdapter.Update(productsTable)
            'Refresh the datagrid
            datagridProducts.DataBind()
            labelResults.Text = "First record deleted successfully"
        Catch ex As Exception
            labelResults.Text = "Record deletion caused an error: " + ex.Message
        End Try
    End Sub
End Class
