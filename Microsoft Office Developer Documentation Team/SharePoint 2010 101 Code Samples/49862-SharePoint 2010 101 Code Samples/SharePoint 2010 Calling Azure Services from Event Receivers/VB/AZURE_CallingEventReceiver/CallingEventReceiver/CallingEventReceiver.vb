Option Explicit On

Imports System
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Security
Imports Microsoft.SharePoint.Utilities
Imports Microsoft.SharePoint.Workflow
Imports System.ServiceModel

Public Class CallingEventReceiver
    Inherits SPItemEventReceiver

    Public Overrides Sub ItemAdded(ByVal properties As SPItemEventProperties)
        'This sample demonstrates how to call a WCF service hosted in Azure
        'from an event receiver. Note that this is only possible in
        'server-side code when the event receiver is deployed OUTSIDE
        'the sandbox. The WCF Service is in the WCFServiceWebRole1
        'project in this solution.

        'This event receiver calls the DayInfoService.svc WCF service in Windows Azure
        'Make sure you package and publish the service in your Window Azure account
        'before you run this event receiver

        'Get the item that was added
        Dim addedItem As SPListItem = properties.ListItem
        'Create the channel factory with a Uri, binding and endpoint
        Dim serviceUri As Uri = New Uri("http://daynamervb.cloudapp.net/dayinfoservice.svc")
        Dim serviceBinding As BasicHttpBinding = New BasicHttpBinding()
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
