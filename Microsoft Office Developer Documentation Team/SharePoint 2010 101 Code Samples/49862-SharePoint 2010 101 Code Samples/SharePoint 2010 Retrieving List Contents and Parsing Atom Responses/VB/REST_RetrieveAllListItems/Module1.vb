'Needed System.Net for the CredentialsCache class
Imports System.Net
'This includes the Team Site Service Reference
Imports REST_RetrieveAllListItems.TeamSiteServiceReference

Module Module1

    Sub Main()
        'This example returns all items from the Announcements list
        'For your own lists, first add a Service Reference to the SharePoint site you want to query
        'Then change the item object below.

        'Formulate the URL to the List Data RESTful service 
        Dim Url As String = "http://intranet.contoso.com/_vti_bin/listdata.svc"
        'Create a dataContext object for the service
        Dim dataContext As TeamSiteDataContext = New TeamSiteDataContext(New Uri(Url))
        'Authenticate as the currently logged on user
        dataContext.Credentials = CredentialCache.DefaultCredentials
        'Tell the user what will be displayed
        Console.WriteLine("Items in the Announcements list:")
        Console.WriteLine()
        For Each item As AnnouncementsItem In dataContext.Announcements
            'Display the item's properties
            Console.WriteLine("Title: " + item.Title)
            Console.WriteLine("ID: " + item.Id.ToString())
            Console.WriteLine("Body: " + item.Body)
            Console.WriteLine()
        Next
        'This line prevents the console disappearing before you can read the result
        'Alternatively, remove this line a run the project without debugging (CTRL-F5)
        Console.ReadKey()
    End Sub

End Module
