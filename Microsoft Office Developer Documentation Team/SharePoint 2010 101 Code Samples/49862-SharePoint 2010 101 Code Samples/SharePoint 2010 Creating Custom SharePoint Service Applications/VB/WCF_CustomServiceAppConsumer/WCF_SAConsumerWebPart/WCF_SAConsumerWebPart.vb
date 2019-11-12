Imports System
Imports System.ComponentModel
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls
Imports WCF_CustomServiceApplication.Client


<ToolboxItemAttribute(false)> _
Public Class WCF_SAConsumerWebPart
    Inherits WebPart

    'This simple web part calls the Day Namer service application
    'Make sure you have deployed the WCF_CustomServiceApplication
    'add created an instance of the service application, either
    'by using Central Administration or the PowerShell scripts,
    'before you try to use this Web Part

    'Controls
    Private instructionsLabel As Label
    Private getTodayButton As Button
    Private resultLabel As Label

    Protected Overrides Sub CreateChildControls()
        'Setup instructions label
        instructionsLabel = New Label()
        instructionsLabel.Text = "Push the button to call the Day Namer service application"
        Me.Controls.Add(instructionsLabel)
        'Setup the Get Today button
        getTodayButton = New Button()
        getTodayButton.Text = "Get Today"
        AddHandler getTodayButton.Click, AddressOf getTodayButton_Click
        Me.Controls.Add(getTodayButton)
        'Setup the results label
        resultLabel = New Label()
        resultLabel.Text = String.Empty
        Me.Controls.Add(resultLabel)
    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        'Render all three controls
        instructionsLabel.RenderControl(writer)
        writer.Write("<br />")
        getTodayButton.RenderControl(writer)
        writer.Write("<br />")
        resultLabel.RenderControl(writer)
    End Sub

    Private Sub getTodayButton_Click(ByVal sender As Object, ByVal args As EventArgs)
        'This is where we call the custom service application
        'Notice that we only need the class of the client -
        'this is the portion of the service application that runs on
        'web front-end servers.
        Dim dayNamerClient As DayNamerServiceClient = New DayNamerServiceClient(SPServiceContext.Current)
        'Call a method and display the result
        Dim today As String = dayNamerClient.TodayIs()
        resultLabel.Text = today
    End Sub


End Class
