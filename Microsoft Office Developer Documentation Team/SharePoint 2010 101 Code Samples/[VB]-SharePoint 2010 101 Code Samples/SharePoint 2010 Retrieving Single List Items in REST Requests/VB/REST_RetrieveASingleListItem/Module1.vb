'Needed for the LINQ query
Imports System.Linq
'Needed System.Net for the CredentialsCache class
Imports System.Net
'This includes the Team Site Service Reference
Imports REST_RetrieveASingleListItem.TeamSiteServiceReference

Module Module1

    Sub Main()
        'This example returns an item called "Test Item" from the Announcements list
        'For your own lists, first add a Service Reference to the SharePoint site you want to query
        'Then change the item object below.

        'Formulate the URL to the List Data RESTful service 
        Dim Url As String = "http://intranet.contoso.com/_vti_bin/listdata.svc"
        'Create a dataContext object for the service
        Dim dataContext As TeamSiteDataContext = New TeamSiteDataContext(New Uri(Url))
        'Authenticate as the currently logged on user
        dataContext.Credentials = CredentialCache.DefaultCredentials
        Try
            'Query for a single item by using a LINQ expression to select items with a specific title
            Dim item = (From announce In dataContext.Announcements
                       Where announce.Title = "Test Item"
                       Select announce).ToList()(0)
            'Return the item
            Console.WriteLine("Title: " & item.Title)
            Console.WriteLine("Body: " & item.Body)
        Catch e As Exception
            'There is no announcement with that title
            Console.WriteLine("An announcement with the title 'Test Title' was not found.")
        End Try
        'This line prevents the console disappearing before you can read the result
        'Alternatively, remove this line a run the project without debugging (CTRL-F5)
        Console.ReadKey()
    End Sub

End Module
