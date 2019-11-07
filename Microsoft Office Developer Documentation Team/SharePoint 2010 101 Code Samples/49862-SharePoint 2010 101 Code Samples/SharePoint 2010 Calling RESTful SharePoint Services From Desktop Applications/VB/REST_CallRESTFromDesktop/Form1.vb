' This references ServiceReference1 which connects to SharePoint
Imports REST_CallRESTFromDesktop.ServiceReference1
' We need the System.Net namespace for the CredentialCache class
Imports System.Net

Public Class Form1
    'This code sample connects to a team site at http://intranet.contoso.com
    'To connect to your own sites, remove ServiceReference1 from the 
    'project, then add a new data source that connects to the right URL

    'This varible holds the data context for the REST service
    'Note: The class name TeamSiteDataContext was created by Visual Studio when the ServiceReference1 was added
    'If your site is not called "Team Site" the class will be called something else.
    'The URI must point to the REST service you want to call. This example calls the List Data service
    Private dataContext As TeamSiteDataContext = New TeamSiteDataContext(New Uri("http://intranet.contoso.com/_vti_bin/listdata.svc"))

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' In the Form_Load event handler, pass credentials to the data context object
        ' CredentialCache is part of the System.Net namespace. It's DefaultCredientials
        ' property passes the credentials of the current user.
        dataContext.Credentials = CredentialCache.DefaultCredentials
        'Set the DataSource to bond to the Announcements list.
        AnnouncementsBindingSource.DataSource = dataContext.Announcements
        'When you load the form, the items in the Annoncements list are displayed.
    End Sub
End Class
