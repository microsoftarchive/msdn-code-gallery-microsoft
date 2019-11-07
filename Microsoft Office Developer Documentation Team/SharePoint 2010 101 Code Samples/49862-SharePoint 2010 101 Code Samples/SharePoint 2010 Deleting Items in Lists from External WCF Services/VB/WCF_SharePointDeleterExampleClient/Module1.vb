'This is the main WCF namespace
Imports System.ServiceModel

Module Module1

    Sub Main()
        'Before you run this WCF Client, ensure that the 
        'WCF_ItemDeleterService project has been started

        'Create the WCF Client
        Console.WriteLine("Connecting to the WCF SharePoint Item Deleter Service")
        Dim sharePointClient As SharePointItemDeleterClient = New SharePointItemDeleterClient()
        'Get the list entries from the WCF service
        Console.WriteLine("Deleting the item...")
        Console.WriteLine()
        'Change this line to pass the title of an item that exists in your list
        Dim sharepointResults As Boolean = sharePointClient.DeleteItem("Test Item")
        If sharepointResults Then
            Console.WriteLine("The item was successfully deleted")
        Else
            Console.WriteLine("The operation did not complete properly. Have you edited the service code to work with your SharePoint farm and list name?")
        End If
        Console.WriteLine("Press any key to close the client application")
        Console.ReadKey()
    End Sub

End Module
