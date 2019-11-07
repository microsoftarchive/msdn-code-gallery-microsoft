'This is the main WCF namespace
Imports System.ServiceModel

Module Module1

    Sub Main()
        'Before you run this WCF Client, ensure that the 
        'WCF_SharePointCallingService project has been started

        'Create the WCF Client
        Console.WriteLine("Connecting to the WCF SharePoint List Reader Service")
        Dim sharePointClient As SharePointListReaderClient = New SharePointListReaderClient()
        'Alter this to the name of the list you want to access
        Dim listName As String = "Announcements"
        'Get the list entries from the WCF service
        Console.WriteLine("Getting list items...")
        Console.WriteLine()
        Dim sharepointResults As Dictionary(Of String, String) = sharePointClient.GetList(listName)
        For Each pair As KeyValuePair(Of String, String) In sharepointResults
            Console.WriteLine("Item:")
            Console.WriteLine("Title: {0}", pair.Key)
            Console.WriteLine("Body: {0}", pair.Value)
            Console.WriteLine()
        Next
        'This line keeps the client open until the user presses a key
        Console.WriteLine("Press any key to close the client")
        Console.ReadKey()
    End Sub

End Module
