'This namespace is used for the SPJobDefinition class
Imports Microsoft.SharePoint.Administration

'To create a custom timer job, first add a class to your SharePoint project and 
'inherit from SPJobDefinition. Implement the constructors and override the Execute
'method as shown below. To install your timer job, and set the schedule, you must 
'add a Feature and a Feature receiver. 
Public Class GENERAL_CustomTimerJob
    Inherits SPJobDefinition

#Region "Constructors"

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal jobName As String, ByVal service As SPService, ByVal server As SPServer, ByVal targetType As SPJobLockType)
        MyBase.New(jobName, service, server, targetType)
    End Sub

    Public Sub New(ByVal jobName As String, ByVal webApplication As SPWebApplication)
        MyBase.New(jobName, webApplication, Nothing, SPJobLockType.ContentDatabase)

        'Set the title of the job, which will be shown in the Central Admin UI
        Me.Title = "Simple Example Timer Job"
    End Sub

#End Region

    Public Overrides Sub Execute(ByVal targetInstanceId As System.Guid)
        'Get the Web Application in which this Timer Job runs
        Dim webApp As SPWebApplication = DirectCast(Me.Parent, SPWebApplication)
        'Get the site collection
        Dim timerSiteCollection As SPSiteCollection = webApp.ContentDatabases(targetInstanceId).Sites
        'Get the Announcements list in the RootWeb of each SPSite
        Dim timerJobList As SPList = Nothing
        For Each site As SPSite In timerSiteCollection
            timerJobList = site.RootWeb.Lists.TryGetList("Announcements")
            If timerJobList IsNot Nothing Then
                Dim newItem As SPListItem = timerJobList.Items.Add()
                newItem("Title") = "Today is " + DateTime.Today.ToLongDateString()
                newItem.Update()
            End If
        Next
    End Sub

End Class
