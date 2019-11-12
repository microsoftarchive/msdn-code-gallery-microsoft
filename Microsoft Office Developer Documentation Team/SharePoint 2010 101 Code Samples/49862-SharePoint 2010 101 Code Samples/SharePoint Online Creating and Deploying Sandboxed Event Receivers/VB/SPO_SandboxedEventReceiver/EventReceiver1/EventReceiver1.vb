Option Explicit On

Imports System
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Security
Imports Microsoft.SharePoint.Utilities
Imports Microsoft.SharePoint.Workflow

Public Class EventReceiver1 
    Inherits SPItemEventReceiver

	''' <summary>
	''' An item was added.
	''' </summary>
	Public Overrides Sub ItemAdded(properties as SPItemEventProperties)
        'This example illustrates a sandboxed event receiver
        'Because it works in the sandbox, you can use it in 
        'SharePoint Online by deploying its .wsp file to the 
        'solutions gallery. The event receiver modifies items
        'added to the Announcements list.

        'To test this solution before deployment, set the Site URL 
        'property of the project to match your test SharePoint farm, then
        'use F5

        'To deploy this project to your SharePoint Online site, upload
        'the SPO_SandboxedWebPart.wsp solution file from the bin/debug
        'folder to your solution gallery. Then activate the solution.

        'Get the item that was added
        Dim addItem As SPListItem = properties.ListItem
        If checkSandbox() Then
            'This is running in the Sandbox
            addItem("Body") += "This item was modified by an event receiver running in the sandbox"
        Else
            'This is running outside the Sandbox
            addItem("Body") += "This item was modified by an event receiver running outside the sandbox"
        End If
        'Save the changes
        addItem.Update()
	End Sub

    Private Function checkSandbox() As Boolean
        'This method returns true only if the code is running in the sandbox
        If System.AppDomain.CurrentDomain.FriendlyName.Contains("Sandbox") Then
            Return True
        Else
            Return False
        End If
    End Function

End Class
