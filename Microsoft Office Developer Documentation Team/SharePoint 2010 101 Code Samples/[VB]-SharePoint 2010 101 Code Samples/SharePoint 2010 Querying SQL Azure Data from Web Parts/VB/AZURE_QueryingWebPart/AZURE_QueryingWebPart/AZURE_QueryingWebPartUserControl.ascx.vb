Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts

Partial Public Class AZURE_QueryingWebPartUserControl
    Inherits UserControl

    'This is the query to run. 
    Const QUERY_STRING As String = "SELECT * FROM Products.dbo.Products;"
    'This is the connection string. Make sure the servername include the tcp
    'protocol, and the name of your SQL Azure server, including ".database.windows.net". 
    'Edit the username and password.
    'Also note the longer connection timeout.
    Const CONNECTION_STRING As String = "Server=tcp:myservername.database.windows.net;Database=Products;User ID=administrator;Password=password;Trusted_Connection=False;Encrypt=True;Connection Timeout=30"

    Private productsDataset As DataSet = New DataSet()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

    End Sub

    Protected Sub lnkbtnGetProducts_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkbtnGetStoreInformation.Click
        'Create a connection to SQL Azure
        Using sqlConnection As SqlConnection = New SqlConnection(CONNECTION_STRING)
            Dim dataAdapter As SqlDataAdapter = New SqlDataAdapter()
            'Specify the select query
            dataAdapter.SelectCommand = New SqlCommand(QUERY_STRING, sqlConnection)
            'Get the data
            dataAdapter.Fill(productsDataset)
            'Bind the data adaptor to the datagrid control
            datagridProducts.DataSource = productsDataset
            datagridProducts.DataBind()
        End Using
    End Sub
End Class
