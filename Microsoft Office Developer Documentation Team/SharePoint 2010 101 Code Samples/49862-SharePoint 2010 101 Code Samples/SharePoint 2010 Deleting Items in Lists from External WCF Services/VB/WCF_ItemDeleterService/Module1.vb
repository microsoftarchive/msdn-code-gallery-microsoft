'This is needed for the credentials classes
Imports System.Net
'This is the main WCF namespace
Imports System.ServiceModel
'This is needed for self-hosting the service
Imports System.ServiceModel.Description
'This is the service reference for the SharePoint RESTful list service
Imports WCF_ItemDeleterService.TeamSiteServiceReference

Namespace WCF_ItemDeleterService

    Module Module1
        'Define the service contract
        <ServiceContract(Namespace:="http://WCF_SharePointCallingService")>
        Public Interface ISharePointItemDeleter
            'One method that inserts an item and returns true or false
            <OperationContract()> _
            Function DeleteItem(ByVal Title As String) As Boolean
        End Interface

        Public Class SharePointItemDeleterService
            Implements ISharePointItemDeleter

            Public Function DeleteItem(ByVal Title As String) As Boolean Implements ISharePointItemDeleter.DeleteItem
                'This example finds the first item in the Announcements list
                'with the specified title.
                'For your own lists, first add a Service Reference to the 
                'SharePoint site you want to query, then change the item object below.

                'Tell the user what's going on
                Console.WriteLine("Received a DeleteItem call. About to query SharePoint...")
                'Formulate the URL to the List Data RESTful service.
                'You must correct this path to point to your own SharePoint farm
                Dim Url As String = "http://intranet.contoso.com/_vti_bin/listdata.svc"
                'Create a dataContext object for the service
                Dim dataContext As TeamSiteDataContext = New TeamSiteDataContext(New Uri(Url))
                'Authenticate as administrator. 
                Dim myCredential As NetworkCredential = New NetworkCredential("Administrator", "pass@word1")
                dataContext.Credentials = myCredential
                Try
                    'Use a lambda expression to locate an item with a matching title
                    Dim announcement As AnnouncementsItem = dataContext.Announcements.Where(Function(i) i.Title = Title).FirstOrDefault()
                    'Delete the announcement
                    dataContext.DeleteObject(announcement)
                    dataContext.SaveChanges()
                    Return True
                Catch e As Exception
                    Console.WriteLine("An error occurred: {0}", e.Message)
                    Return False
                End Try
            End Function
        End Class

        Sub Main()
            'This example WCF service is self-hosted in
            'the command console. This procedure runs the
            'service until the user presses a key

            'This is the address for the WCF service
            Dim baseAddress As Uri = New Uri("http://localhost:8088/WCF_SharePointItemDeleterService/Service")
            Dim selfHost As ServiceHost = New ServiceHost(GetType(SharePointItemDeleterService), baseAddress)
            Try
                'Create an endpoint
                selfHost.AddServiceEndpoint(GetType(ISharePointItemDeleter), New WSHttpBinding(), "SharePointItemDeleterService")
                'Enable the service to exchange its metadata
                Dim smb As ServiceMetadataBehavior = New ServiceMetadataBehavior()
                smb.HttpGetEnabled = True
                selfHost.Description.Behaviors.Add(smb)
                'Open the service and tell the user
                selfHost.Open()
                Console.WriteLine("The SharePoint Item Deleter Service is ready")
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

