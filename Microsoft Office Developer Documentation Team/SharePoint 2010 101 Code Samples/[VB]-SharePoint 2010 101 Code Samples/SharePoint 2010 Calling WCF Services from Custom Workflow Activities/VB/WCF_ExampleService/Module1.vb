Imports System
'This is the main WCF namespace
Imports System.ServiceModel
'This namespace is used for self-hosting the service
Imports System.ServiceModel.Description

Namespace WCF_ExampleService
    Module Module1
        'Define the service contract
        <ServiceContract(Namespace:="http://WCF_ExampleService")>
        Public Interface IDayNamer
            <OperationContract()> _
            Function TodayIs() As String
            <OperationContract()> _
            Function TodayAdd(ByVal daysToAdd As Integer) As String
        End Interface

        'This class implements the WCF service
        Public Class DayNamerService
            Implements IDayNamer

            Public Function TodayAdd(ByVal daysToAdd As Integer) As String Implements IDayNamer.TodayAdd
                'Add the requested number to today
                Dim requestedDateTime As DateTime = DateTime.Today.AddDays(daysToAdd)
                Dim requestedDay As DayOfWeek = requestedDateTime.DayOfWeek
                'output to the console
                Console.WriteLine("Received a TodayAdd call.:")
                Console.WriteLine("Days to add: {0}", daysToAdd.ToString())
                Console.WriteLine("Returned {0}", requestedDay.ToString())
                'Return today's name
                Return requestedDay.ToString()
            End Function

            Public Function TodayIs() As String Implements IDayNamer.TodayIs
                'Find out what today is
                Dim today As DayOfWeek = DateTime.Today.DayOfWeek
                'output to the console
                Console.WriteLine("Received a TodayIs call.")
                Console.WriteLine("Returned {0}", today.ToString())
                'Return today's name
                Return today.ToString()
            End Function
        End Class

        Sub Main()
            'This example WCF service is self-hosted in
            'the command console. This procedure runs the
            'service until the user presses a key

            'This is the address for the WCF service
            Dim baseAddress As Uri = New Uri("http://localhost:8088/WCF_ExampleService/Service")
            Dim selfHost As ServiceHost = New ServiceHost(GetType(DayNamerService), baseAddress)
            Try
                'Create an endpoint
                selfHost.AddServiceEndpoint(GetType(IDayNamer), New WSHttpBinding(), "DayNamerService")
                'Enable the service to exchange its metadata
                Dim smb As ServiceMetadataBehavior = New ServiceMetadataBehavior()
                smb.HttpGetEnabled = True
                selfHost.Description.Behaviors.Add(smb)
                'Open the service and tell the user
                selfHost.Open()
                Console.WriteLine("The Day Namer Service is ready")
                Console.WriteLine("Press any key to close the service")
                'Wait for the user to press a key
                Console.ReadKey()
                'Close the service
                selfHost.Close()
            Catch ex As CommunicationException
                Console.WriteLine("A communication exception occurred: {0}", ex.Message)
                selfHost.Abort()
            End Try
        End Sub
    End Module

End Namespace
