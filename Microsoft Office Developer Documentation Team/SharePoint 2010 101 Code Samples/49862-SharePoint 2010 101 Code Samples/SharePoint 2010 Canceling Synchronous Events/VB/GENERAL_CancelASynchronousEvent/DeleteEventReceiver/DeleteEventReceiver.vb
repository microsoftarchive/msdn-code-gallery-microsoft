Option Explicit On

Imports System
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Security
Imports Microsoft.SharePoint.Utilities
Imports Microsoft.SharePoint.Workflow

''' <summary>
''' This example cancels the synchronous ItemDeleting event and logs
''' it's actions to the Tasks list.
''' </summary>
Public Class DeleteEventReceiver
    Inherits SPItemEventReceiver

    ''' <summary>
    ''' An item is being deleted.
    ''' </summary>
    Public Overrides Sub ItemDeleting(ByVal properties As SPItemEventProperties)
        'Prevent the item being deleted by cancelling the event
        properties.Cancel = True
        properties.ErrorMessage = "You cannot delete any items from this list"
        'Get the item
        Dim updatedItem As SPListItem = properties.ListItem
        Using currentWeb As SPWeb = properties.Web
            'Get the Tasks list in the same SPWeb
            Dim tasksList As SPList = currentWeb.Lists("Tasks")
            'Create an item to log the update
            Dim newTask As SPListItem = tasksList.Items.Add()
            newTask("Title") = "Educate User"
            newTask("Body") = "The following user tried to delete an announcement: " + properties.UserDisplayName
            newTask.Update()
        End Using
    End Sub

End Class
