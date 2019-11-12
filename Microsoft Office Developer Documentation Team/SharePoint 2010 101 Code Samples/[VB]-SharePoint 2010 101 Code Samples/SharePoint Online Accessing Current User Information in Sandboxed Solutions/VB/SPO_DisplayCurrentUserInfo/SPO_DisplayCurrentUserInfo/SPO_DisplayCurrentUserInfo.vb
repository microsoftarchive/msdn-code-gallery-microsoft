Imports System
Imports System.ComponentModel
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls

<ToolboxItemAttribute(false)> _
Public Class SPO_DisplayCurrentUserInfo
    Inherits WebPart

    'Note: The Visual Studio 2010 Visual Web Part project template
    'is not compatible with the sandbox. To create a Web Part for
    'SharePoint Online, create an empty SharePoint project and ensure 
    'that it runs in the sandbox (Check the Sandboxed Solution 
    'property of the project). Then add a Web Part item to the 
    'Project

    'To test this solution before deployment, set the Site URL 
    'property of the project to match your test SharePoint farm, then
    'use F5

    'To deploy this project to your SharePoint Online site, upload
    'the SPO_SandboxedWebPart.wsp solution file from the bin/debug
    'folder to your solution gallery. Activate the solution and add
    'the web part to a page. 

    'Web part controls
    Private userNameLabel As Label
    Private userEmailAddressLabel As Label
    Private labelSandboxCheck As Label

    Protected Overrides Sub CreateChildControls()
        'Get the current SharePoint web
        Dim currentWeb As SPWeb = SPContext.Current.Web
        'Display the username
        userNameLabel = New Label()
        userNameLabel.Text = "Current User Name: " + currentWeb.CurrentUser.Name
        Me.Controls.Add(userNameLabel)
        'Display the user email address
        userEmailAddressLabel = New Label()
        userEmailAddressLabel.Text = "Current User Email: " + currentWeb.CurrentUser.Email
        Me.Controls.Add(userEmailAddressLabel)
        'Check if this is in the Sandbox
        labelSandboxCheck = New Label()
        If checkSandbox() Then
            labelSandboxCheck.Text = "This Web Part is sandboxed"
        Else
            labelSandboxCheck.Text = "This Web Part is NOT sandboxed"
        End If
        Me.Controls.Add(labelSandboxCheck)
    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        EnsureChildControls()
        userNameLabel.RenderControl(writer)
        writer.Write("<br />")
        userEmailAddressLabel.RenderControl(writer)
        writer.Write("<br />")
        labelSandboxCheck.RenderControl(writer)
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
