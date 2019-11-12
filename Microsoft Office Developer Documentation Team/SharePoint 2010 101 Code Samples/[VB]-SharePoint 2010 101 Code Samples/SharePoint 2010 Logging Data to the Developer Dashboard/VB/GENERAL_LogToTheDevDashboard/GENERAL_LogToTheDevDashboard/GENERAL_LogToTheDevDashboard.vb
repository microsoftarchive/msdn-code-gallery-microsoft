Imports System
Imports System.ComponentModel
Imports System.Threading
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Utilities
Imports Microsoft.SharePoint.WebControls

'This web part simply displays text in a label after pausing for one second
'Because the pause in done in a monitored scope, information appears in the
'developer dashboard when you display it. Look for "Long Procedure" in the 
'dashboard for timings and other information. Use the two PowerShell scripts
'in this project to enable and disable the dashboard.
<ToolboxItemAttribute(false)> _
Public Class GENERAL_LogToTheDevDashboard
    Inherits WebPart

    'Controls
    Private feedbackLabel As Label

    Protected Overrides Sub CreateChildControls()
        'Set up the feedback label
        feedbackLabel = New Label()
        Me.Controls.Add(feedbackLabel)
    End Sub

    Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
        'To log to the developer dashboard, set up a monitored scope
        Using myScope As SPMonitoredScope = New SPMonitoredScope("Long Procedure")
            feedbackLabel.Text = longProcedure()
        End Using
        MyBase.OnPreRender(e)
    End Sub

    Private Function longProcedure() As String
        'Wait for one second. This should more-or-less match the info in the developer dashboard
        Thread.Sleep(1000)
        Return "The Long Procedure has completed!"
    End Function

End Class
