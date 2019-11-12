Imports System
Imports System.ComponentModel
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls
'Import the WCF Namespace
Imports System.ServiceModel

<ToolboxItemAttribute(false)> _
Public Class WCF_CallServiceFromWebPart
    Inherits WebPart

    'This sample demonstrates how to call a WCF service 
    'from a web part. Note that this is only possible in
    'server-side code when the Web Part is deployed OUTSIDE
    'the sandbox. The WCF Service is in the WCF_ExampleService
    'project in this solution.

    'To use this sample, configure the WCF_CallServiceFromWebPart
    'project to deploy to your SharePoint site (the default is
    'http://intranet.contoso.com) then start that project for 
    'debugging. Add the custom web part to any page. Before 
    'you click the"Today" button, run the WCF_ExampleService
    'project and wait until the prompt tells you the service is
    'ready. 

    'UI Controls
    Private instructionsLabel As Label
    Private resultLabel As Label
    Private buttonToday As Button

    Protected Overrides Sub CreateChildControls()
        'Set up the instructions label
        instructionsLabel = New Label()
        instructionsLabel.Text = "Make sure the WCF Service is running, then push the Today button to call it."
        Me.Controls.Add(instructionsLabel)
        'Set up the button
        buttonToday = New Button()
        buttonToday.Text = "Today"
        AddHandler buttonToday.Click, AddressOf buttonToday_Click
        Me.Controls.Add(buttonToday)
        'Set up the results label
        resultLabel = New Label()
        resultLabel.Text = String.Empty
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

    Private Sub buttonToday_Click(ByVal sender As Object, ByVal args As EventArgs)
        'I used svcutil.exe to generate the proxy class for the service
        'in the generatedDayNamerProxy.cs file. I'm going to configure this
        'in code by using a channel factory.

        'Create the channel factory with a Uri, binding and endpoint
        Dim serviceUri As Uri = New Uri("http://localhost:8088/WCF_ExampleService/Service/DayNamerService")
        Dim serviceBinding As WSHttpBinding = New WSHttpBinding()
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
