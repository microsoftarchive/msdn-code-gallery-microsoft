'This is needed for the credentials classes
Imports System.Net
'This is the main WCF namespace
Imports System.ServiceModel
'This is needed for self-hosting the service
Imports System.ServiceModel.Description
'This is the service reference for the SharePoint RESTful list service
Imports WCF_SharePointCallingService.TeamSiteServiceReference

Namespace WCF_SharePointCallingService

    Module Module1
        'Define the service contract
        <ServiceContract(Namespace:="http://WCF_SharePointCallingService")>
        Public Interface ISharePointListReader
            'One method that returns a dictionary of item Titles and Bodies.
            <OperationContract()> _
            Function GetList(ByVal Name As String) As Dictionary(Of String, String)
        End Interface

        'This class implements the service contract defined in ISharePointListReader
        Public Class SharePointListReaderService
            Implements ISharePointListReader

            Public Function GetList(ByVal Name As String) As Dictionary(Of String, String) Implements ISharePointListReader.GetList
                'Run a query against the SharePoint list
                Console.WriteLine("Received a GetList call. About to query SharePoint...")
                Dim results As Dictionary(Of String, String) = queryListServer(Name)
                'Let the user know what happened
                Console.WriteLine()
                Console.WriteLine("Query complete. Returning results to client.")
                Return results
            End Function

            Private Function queryListServer(ByVal listName As String) As Dictionary(Of String, String)
                'This example returns all items from the Announcements list
                'For your own lists, first add a Service Reference to the 
                'SharePoint site you want to query, then change the item object below.

                Dim listEntries As Dictionary(Of String, String) = New Dictionary(Of String, String)
                'Formulate the URL to the List Data RESTful service 
                Dim Url As String = "http://intranet.contoso.com/_vti_bin/listdata.svc"
                'Create a dataContext object for the service
                Dim dataContext As TeamSiteDataContext = New TeamSiteDataContext(New Uri(Url))
                'Authenticate as administrator. 
                Dim myCredential As NetworkCredential = New NetworkCredential("Administrator", "pass@word1")
                dataContext.Credentials = myCredential
                'As this is running in the console, we can display the results
                Console.WriteLine("Items in the Announcements list:")
                Console.WriteLine()
                'Loop through the announcements
                For Each item As AnnouncementsItem In dataContext.Announcements
                    'Display the item's properties
                    Console.WriteLine("Title: " + item.Title)
                    listEntries.Add(item.Title, item.Body)
                Next
                'Return the dictionary
                Return listEntries
            End Function
        End Class

        Sub Main()
            'This example WCF service is self-hosted in
            'the command console. This procedure runs the
            'service until the user presses a key

            'This is the address for the WCF service
            Dim baseAddress As Uri = New Uri("http://localhost:8088/WCF_SharePointCallingService/Service")
            Dim selfHost As ServiceHost = New ServiceHost(GetType(SharePointListReaderService), baseAddress)
            Try
                'Create an endpoint
                selfHost.AddServiceEndpoint(GetType(ISharePointListReader), New WSHttpBinding(), "SharePointListReaderService")
                'Enable the service to exchange its metadata
                Dim smb As ServiceMetadataBehavior = New ServiceMetadataBehavior()
                smb.HttpGetEnabled = True
                selfHost.Description.Behaviors.Add(smb)
                'Open the service and tell the user
                selfHost.Open()
                Console.WriteLine("The SharePoint List Reader Service is ready")
                Console.WriteLine("Press any key to close the service")
                'Wait for the user to press a key
                Console.ReadKey()
                'Close the service
                selfHost.Close()
            Catch e As CommunicationException
                Console.WriteLine("A communication exception occurred: {0}", e.Message)
                selfHost.Abort()
            End Try
        End Sub

    End Module
End Namespace
