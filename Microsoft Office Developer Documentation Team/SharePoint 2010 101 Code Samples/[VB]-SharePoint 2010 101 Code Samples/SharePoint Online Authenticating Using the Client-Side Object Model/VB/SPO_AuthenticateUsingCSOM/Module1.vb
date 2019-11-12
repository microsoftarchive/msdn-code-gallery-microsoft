Imports Microsoft.SharePoint.Client

Module Module1

    Sub Main()
        'This example is based on an MSDN Code Sample by Robert Bogue
        'You can download the original sample here: http://code.msdn.microsoft.com/Remote-Authentication-in-b7b6f43c
        'A full discussion of the coding techniques is here: http://msdn.microsoft.com/en-us/library/hh147177.aspx

        'Adjust this string to point to your site on Office 365
        Dim siteURL As String = "http://www.contoso.com/teamsite"
        Console.WriteLine("Opening Site: " + siteURL)

        'Call the ClaimClientContext class to do claims mode authentication
        Using context As ClientContext = ClaimsClientContext.GetAuthenticatedContext(siteURL)
            If Not context Is Nothing Then
                'We have the client context object so claims-based authentication is complete
                'Find out about the SP.Web object
                context.Load(context.Web)
                context.ExecuteQuery()
                'Display the name of the SharePoint site
                Console.WriteLine(context.Web.Title)
            End If
        End Using
        'This line prevents the console disappearing before you can read the result
        'Alternatively, remove this line a run the project without debugging (CTRL-F5)
        Console.ReadKey()

    End Sub

End Module
