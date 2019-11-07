Option Explicit On
Option Strict On

Imports System
Imports System.Text
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Security
Imports Microsoft.SharePoint.Utilities
Imports Microsoft.SharePoint.Workflow

Public Class WebEventReceiver
    Inherits SPWebEventReceiver

    ''' <summary>
    ''' A site collection was deleted.
    ''' </summary>
    Public Overrides Sub SiteDeleted(ByVal properties As SPWebEventProperties)
        Me.LogWebEventProperties(properties)
    End Sub

    ''' <summary>
    ''' A site was deleted.
    ''' </summary>
    Public Overrides Sub WebDeleted(ByVal properties As SPWebEventProperties)
        Me.LogWebEventProperties(properties)
    End Sub

    ''' <summary>
    ''' A site was moved.
    ''' </summary>
    Public Overrides Sub WebMoved(ByVal properties As SPWebEventProperties)
        Me.LogWebEventProperties(properties)
    End Sub

    ''' <summary>
    ''' A site was provisioned.
    ''' </summary>
    Public Overrides Sub WebProvisioned(ByVal properties As SPWebEventProperties)
        Me.LogWebEventProperties(properties)
    End Sub

    Private Sub LogWebEventProperties(ByVal properties As SPWebEventProperties)
        ' Specify the log list name.
        Dim listName As String = "WebEventLogger"

        ' Create string builder object.
        Dim sb As StringBuilder = New StringBuilder()

        ' Add properties that do not throw exceptions.
        sb.AppendFormat("Cancel: {0}" & vbLf, properties.Cancel)
        sb.AppendFormat("ErrorMessage: {0}" & vbLf, properties.ErrorMessage)
        sb.AppendFormat("EventType: {0}" & vbLf, properties.EventType)
        sb.AppendFormat("FullUrl: {0}" & vbLf, properties.FullUrl)
        sb.AppendFormat("NewServerRelativeUrl: {0}" & vbLf, properties.NewServerRelativeUrl)
        sb.AppendFormat("ParentWebId: {0}" & vbLf, properties.ParentWebId)
        sb.AppendFormat("ReceiverData: {0}" & vbLf, properties.ReceiverData)
        sb.AppendFormat("RedirectUrl: {0}" & vbLf, properties.RedirectUrl)
        sb.AppendFormat("ServerRelativeUrl: {0}" & vbLf, properties.ServerRelativeUrl)
        sb.AppendFormat("SiteId: {0}" & vbLf, properties.SiteId)
        sb.AppendFormat("Status: {0}" & vbLf, properties.Status)
        sb.AppendFormat("UserDisplayName: {0}" & vbLf, properties.UserDisplayName)
        sb.AppendFormat("UserLoginName: {0}" & vbLf, properties.UserLoginName)
        sb.AppendFormat("WebId: {0}" & vbLf, properties.WebId)
        sb.AppendFormat("Web: {0}" & vbLf, properties.Web)

        ' Log the event to the list.
        Me.EventFiringEnabled = False
        LogEvent(properties.Web, listName, properties.EventType, sb.ToString())
        Me.EventFiringEnabled = True
    End Sub

    ''' <summary>
    ''' Log the event to the specified list.
    ''' </summary>
    Public Shared Sub LogEvent(ByVal web As SPWeb, _
        ByVal listName As String, _
        ByVal eventType As SPEventReceiverType, _
        ByVal details As String)

        Dim logList As SPList = EnsureLogList(web.Site.RootWeb, listName)
        Dim logItem As SPListItem = logList.Items.Add()
        logItem("Title") = String.Format("{0} triggered at {1}", eventType, DateTime.Now)
        logItem("Event") = eventType.ToString()
        'logItem("Before") = IsBeforeEvent(eventType)
        logItem("Date") = DateTime.Now
        logItem("Details") = details
        logItem.Update()
    End Sub

    ''' <summary>
    ''' Ensures that the Logs list with the specified name is created.
    ''' </summary>
    Private Shared Function EnsureLogList(ByVal web As SPWeb, ByVal listName As String) As SPList
        Dim list As SPList = Nothing
        Try
            'See if the list already exists
            list = web.Lists(listName)
        Catch ex As Exception
            ' Create list.
            Dim listGuid As Guid = web.Lists.Add(listName, listName, SPListTemplateType.GenericList)
            list = web.Lists(listGuid)
            list.OnQuickLaunch = True
            'Add the fields to the list.
            'No need to add "Title" because it is added by default.
            'We use it to set the event name.
            list.Fields.Add("Event", SPFieldType.Text, True)
            list.Fields.Add("Before", SPFieldType.Boolean, True)
            list.Fields.Add("Date", SPFieldType.DateTime, True)
            list.Fields.Add("Details", SPFieldType.Note, False)
            'Specify which fields to view.
            Dim view As SPView = list.DefaultView
            view.ViewFields.Add("Event")
            view.ViewFields.Add("Before")
            view.ViewFields.Add("Date")
            view.ViewFields.Add("Details")
            view.Update()
            'Save the changes
            list.Update()
        End Try
        'Return the list
        Return list
    End Function

End Class
