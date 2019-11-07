Imports System
Imports System.ComponentModel
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls
Imports System.ServiceModel

<ToolboxItemAttribute(false)> _
Public Class CallingWebPart
    Inherits WebPart

    'UI Controls
    Private instructionsLabel As Label
    Private resultLabel As Label
    Private buttonToday As Button

    Protected Overrides Sub CreateChildControls()
        'Set up the instructions label
        instructionsLabel = New Label()
        instructionsLabel.Text = "Click the Get Today button to call the Azure service"
        Me.Controls.Add(instructionsLabel)
        'Set up the button
        buttonToday = New Button()
        buttonToday.Text = "Get Today"
        AddHandler buttonToday.Click, AddressOf buttonGetToday_Click
        Me.Controls.Add(buttonToday)
        'Set up the results label
        resultLabel = New Label()
        Me.Controls.Add(resultLabel)
    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        'Render the controls
        instructionsLabel.RenderControl(writer)
        writer.Write("<br />")
        buttonToday.RenderControl(writer)
        writer.Write("<br />")
        resultLabel.RenderControl(writer)
    End Sub

    Private Sub buttonGetToday_Click(ByVal sender As Object, ByVal args As EventArgs)
        'I used svcutil.exe to generate the proxy class for the service
        'in the generatedDayNamerProxy.cs file. I'm going to configure this
        'in code by using a channel factory.

        'Create the channel factory with a Uri, binding and endpoint
        'The Uri is to the hosted service in Windows Azure
        'Edit this Uri to match the location where you published the service.
        Dim serviceUri As Uri = New Uri("http://daynamervb.cloudapp.net/dayinfoservice.svc")
        Dim serviceBinding As BasicHttpBinding = New BasicHttpBinding()
        Dim dayNamerEndPoint As EndpointAddress = New EndpointAddress(serviceUri)
        Dim channelFactory As ChannelFactory(Of IDayNamer) = New ChannelFactory(Of IDayNamer)(serviceBinding, dayNamerEndPoint)
        'Create a channel
        Dim dayNamer As IDayNamer = channelFactory.CreateChannel()
        'Now we can call the TodayIs method
        Dim today As String = dayNamer.TodayIs()
        resultLabel.Text = "Today is: " + today
        'close the factory with all its channels
        channelFactory.Close()
    End Sub

End Class
