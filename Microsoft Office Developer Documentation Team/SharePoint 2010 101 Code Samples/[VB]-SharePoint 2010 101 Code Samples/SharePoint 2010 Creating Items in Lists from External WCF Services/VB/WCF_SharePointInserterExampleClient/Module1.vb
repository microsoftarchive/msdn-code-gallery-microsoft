'This is the main WCF namespace
Imports System.ServiceModel

Module Module1

    Sub Main()
        'Before you run this WCF Client, ensure that the 
        'WCF_ItemCreatorService project has been started

        'Create the WCF Client
        Console.WriteLine("Connecting to the WCF SharePoint Item Inserter Service")
        Dim sharePointClient As SharePointItemInserterClient = New SharePointItemInserterClient()
        'Get the list entries from the WCF service
        Console.WriteLine("Inserting the item...")
        Console.WriteLine()
        Dim sharepointResults As Boolean = sharePointClient.InsertItem("Test Item", "This item was created by a WCF service")
        If sharepointResults Then
            Console.WriteLine("The item was successfully created and saved")
        Else
            Console.WriteLine("The operation did not complete properly. Have you edited the service code to work with your SharePoint farm and list name?")
        End If
        Console.WriteLine("Press any key to close the client application")
        Console.ReadKey()
    End Sub

End Module
