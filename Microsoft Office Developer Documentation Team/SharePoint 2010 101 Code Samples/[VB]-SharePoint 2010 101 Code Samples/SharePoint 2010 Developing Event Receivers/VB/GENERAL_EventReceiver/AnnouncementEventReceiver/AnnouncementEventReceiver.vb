Option Explicit On

Imports System
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Security
Imports Microsoft.SharePoint.Utilities
Imports Microsoft.SharePoint.Workflow

''' <summary>
''' List Item Events. If you add other event handlers to this receiver, ensure
''' that you also add corresponding Receiver elements to Elements.xml
''' </summary>
Public Class AnnouncementEventReceiver
    Inherits SPItemEventReceiver

    ''' <summary>
    ''' This event fires before an item is modified.
    ''' This is a synchronous event.
    ''' </summary>
    Public Overrides Sub ItemUpdating(ByVal properties As SPItemEventProperties)
        'Get the item and the parent list
        Dim updatedItem As SPListItem = properties.ListItem
        Using currentWeb As SPWeb = properties.Web
            'Get the Tasks list in the same SPWeb
            Dim tasksList As SPList = currentWeb.Lists("Tasks")
            'Create an item to log the update
            Dim newAnnouncement As SPListItem = tasksList.Items.Add()
            newAnnouncement("Title") = "Review an updated item"
            newAnnouncement("Body") = "Please review an announcement modification. The updated item's title was: " + updatedItem.Title
            newAnnouncement.Update()
        End Using
    End Sub

    ''' <summary>
    ''' This event fires after an item is added. It's a good place to modify the item.
    ''' This is an asynchronous event
    ''' </summary>
    Public Overrides Sub ItemAdded(ByVal properties As SPItemEventProperties)
        'Get the item
        Dim addedItem As SPListItem = properties.ListItem
        'Add a creation date
        addedItem("Body") += "This item was added on: " + CStr(DateTime.Today)
        addedItem.Update()
    End Sub


End Class
