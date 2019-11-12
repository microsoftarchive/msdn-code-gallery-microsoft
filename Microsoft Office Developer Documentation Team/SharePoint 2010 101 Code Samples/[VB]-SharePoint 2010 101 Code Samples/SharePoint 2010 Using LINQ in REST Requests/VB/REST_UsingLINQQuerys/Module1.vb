'Needed for the LINQ query
Imports System.Linq
'Needed System.Net for the CredentialsCache class
Imports System.Net
'This includes the Team Site Service Reference
Imports REST_UsingLINQQuerys.TeamSiteServiceReference

Module Module1

    Sub Main()
        'This example returns items from the Announcements list
        'For your own lists, first add a Service Reference to the SharePoint site you want to query
        'Then change the objects below.

        'Formulate the URL to the List Data RESTful service 
        Dim Url As String = "http://intranet.contoso.com/_vti_bin/listdata.svc"
        'Create a dataContext object for the service
        Dim dataContext As TeamSiteDataContext = New TeamSiteDataContext(New Uri(Url))
        'Authenticate as the currently logged on user
        dataContext.Credentials = CredentialCache.DefaultCredentials
        'Query for items beginning with 'A' by using a LINQ expression to select items with a specific title
        Dim results = From announce In dataContext.Announcements
                   Where announce.Title.StartsWith("A")
                   Order By announce.Title
                   Select announce.Title, announce.Id, announce.Body
        'Tell the user what will be displayed
        Console.WriteLine("Items in the Announcements list that begin with an A, ordered by Title:")
        Console.WriteLine()
        If results.Count > 0 Then
            'Loop through the results
            For Each result In results
                Console.WriteLine("Title: " & result.Title)
                Console.WriteLine("ID: " & result.Id)
                Console.WriteLine("Body: " & result.Body)
                Console.WriteLine()
            Next
        Else
            'There is no announcement with the first letter 'A'
            Console.WriteLine("An announcement with a title beginning with 'A' was not found.")
        End If
        'This line prevents the console disappearing before you can read the result
        'Alternatively, remove this line a run the project without debugging (CTRL-F5)
        Console.ReadKey()
    End Sub

End Module
