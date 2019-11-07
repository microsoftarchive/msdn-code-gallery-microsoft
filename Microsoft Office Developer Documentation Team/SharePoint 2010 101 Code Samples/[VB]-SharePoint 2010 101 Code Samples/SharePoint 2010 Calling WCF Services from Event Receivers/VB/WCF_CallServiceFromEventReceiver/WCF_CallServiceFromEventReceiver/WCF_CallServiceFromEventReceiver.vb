Option Explicit On
Option Strict Off

Imports System
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Security
Imports Microsoft.SharePoint.Utilities
Imports Microsoft.SharePoint.Workflow
'This is the main WCF namespace
Imports System.ServiceModel

Public Class WCF_CallServiceFromEventReceiver 
    Inherits SPItemEventReceiver

	''' <summary>
	''' An item was added.
	''' </summary>
    Public Overrides Sub ItemAdded(ByVal properties As SPItemEventProperties)
        'This sample demonstrates how to call a WCF service 
        'from an event receiver. Note that this is only possible in
        'server-side code when the event receiver is deployed OUTSIDE
        'the sandbox. The WCF Service is in the WCF_ExampleService
        'project in this solution.

        'To use this sample, configure the WCF_CallServiceFromEventReceiver
        'project to deploy to your SharePoint site (the default is
        'http://intranet.contoso.com) then start that project for 
        'debugging. Run the WCF_ExampleService project and wait until the 
        'prompt tells you the service is ready. Then add an item to the 
        'Announcements list. The day name will be added to the bottom 
        'of the body.

        'I used svcutil.exe to generate the proxy class for the service
        'in the generatedDayNamerProxy.cs file. I'm going to configure this
        'in code by using a channel factory.

        'Get the item that was added
        Dim addedItem As SPListItem = properties.ListItem
        'Create the channel factory with a Uri, binding and endpoint
        Dim serviceUri As Uri = New Uri("http://localhost:8088/WCF_ExampleService/Service/DayNamerService")
        Dim serviceBinding As WSHttpBinding = New WSHttpBinding()
        Dim dayNamerEndPoint As EndpointAddress = New EndpointAddress(serviceUri)
        Dim channelFactory As ChannelFactory(Of IDayNamer) = New ChannelFactory(Of IDayNamer)(serviceBinding, dayNamerEndPoint)
        'Create a channel
        Dim dayNamer As IDayNamer = channelFactory.CreateChannel()
        'Now we can call the TodayIs method
        Dim today As String = dayNamer.TodayIs()
        'Update the item
        addedItem("Body") += "This item was added on " + today
        addedItem.Update()
        'close the factory with all its channels
        channelFactory.Close()
    End Sub

End Class
