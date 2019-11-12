Imports Microsoft.SharePoint.Administration
Imports System.ServiceModel

'This timer job calls the DayInfoService.svc WCF service in Windows Azure
'Make sure you package and publish the service in your Window Azure account
'before you run this web part.s
Public Class CallingTimerJob
    Inherits SPJobDefinition

    'Constructors
    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal jobName As String, ByVal service As SPService, ByVal server As SPServer, ByVal targetType As SPJobLockType)
        MyBase.New(jobName, service, server, targetType)
    End Sub

    Public Sub New(ByVal jobName As String, ByVal webApplication As SPWebApplication)
        MyBase.New(jobName, webApplication, Nothing, SPJobLockType.ContentDatabase)
        Me.Title = "AZURE Calling Timer Job"
    End Sub

    Public Overrides Sub Execute(ByVal targetInstanceId As System.Guid)
        'Get the Web Application in which this Timer Job runs
        Dim webApp As SPWebApplication = DirectCast(Me.Parent, SPWebApplication)
        Dim contentDB As SPContentDatabase = webApp.ContentDatabases(targetInstanceId)
        Dim timerSiteCollection As SPSiteCollection = webApp.ContentDatabases(targetInstanceId).Sites
        Dim timerJobList As SPList = Nothing
        For Each site As SPSite In timerSiteCollection
            timerJobList = site.RootWeb.Lists.TryGetList("Announcements")
            If Not timerJobList Is Nothing Then
                Dim newItem As SPListItem = timerJobList.Items.Add()
                newItem("Title") = "Today is " + getToday()
                newItem.Update()
            End If
        Next
    End Sub

    'This function calls the WCF service hosted in Azure
    Private Function getToday() As String
        'I used svcutil.exe to generate the proxy class for the service
        'in the generatedDayNamerProxy.cs file. I'm going to configure this
        'in code by using a channel factory.

        Dim today As String = String.Empty
        'Create the channel factory with a Uri, binding and endpoint
        Dim serviceUri As Uri = New Uri("http://daynamervb.cloudapp.net/dayinfoservice.svc")
        Dim serviceBinding As BasicHttpBinding = New BasicHttpBinding()
        Dim dayNamerEndPoint As EndpointAddress = New EndpointAddress(serviceUri)
        Dim channelFactory As ChannelFactory(Of IDayNamer) = New ChannelFactory(Of IDayNamer)(serviceBinding, dayNamerEndPoint)
        'Create a channel
        Dim dayNamer As IDayNamer = channelFactory.CreateChannel()
        'Now we can call the TodayIs method
        today = dayNamer.TodayIs()
        'close the factory with all its channels
        channelFactory.Close()
        'Return the name
        Return today
    End Function
End Class
