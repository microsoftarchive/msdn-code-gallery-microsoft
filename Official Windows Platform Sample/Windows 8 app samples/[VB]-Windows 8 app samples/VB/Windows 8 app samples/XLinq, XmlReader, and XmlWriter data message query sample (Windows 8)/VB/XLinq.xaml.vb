'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System
Imports System.Threading.Tasks
Imports System.Net.Http
Imports System.IO
Imports System.Text
Imports System.Xml.Linq
Imports System.Linq

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class XLinqScenario
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'DoSomething' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub DoSomething_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            rootPage.NotifyUser("You clicked the " & b.Content.ToString & " button", NotifyType.StatusMessage)
        End If

        Try
            Dim cities As String = Await ProcessWithXLinq()
            OutputTextBlock1.Text = cities
        Catch ex As System.Xml.XmlException
            OutputTextBlock1.Text = "Exception happened, Message:" & ex.Message
        Catch netEx As System.Net.Http.HttpRequestException
            OutputTextBlock1.Text = "Exception happend, Message:" & netEx.Message & " Have you updated the bing map key in function ProcessWithXLinq()?"
        End Try
    End Sub

    Private Async Function ProcessWithXLinq() As Task(Of String)
        ' you need to acquire a Bing Maps key. See http://www.bingmapsportal.com/
        Dim bingMapKey As String = "INSERT_YOUR_BING_MAPS_KEY"
        ' the following uri will returns a response with xml content
        Dim uri As New Uri(String.Format("http://dev.virtualearth.net/REST/v1/Locations?q=manchester&o=xml&key={0}", bingMapKey))

        Dim client As New HttpClient()
        Dim response As HttpResponseMessage = Await client.GetAsync(uri)
        response.EnsureSuccessStatusCode()

        ' ReadAsStreamAsync() returns when the whole message is downloaded
        Dim stream As Stream = Await response.Content.ReadAsStreamAsync()

        Dim xdoc As XDocument = XDocument.Load(stream)
        Dim xns As XNamespace = "http://schemas.microsoft.com/search/local/ws/rest/v1"

        ' query node named "Address"
        ' where CountryRegion contains "United States"

        Dim addresses = From node In xdoc.Descendants(xns + "Address")
                         Where node.Element(xns + "CountryRegion").Value.Contains("United States")
                         Select node.Element(xns + "FormattedAddress").Value


        ' select the FormattedAddress node's value
        Dim stringBuilder As New StringBuilder("Manchester in US: ")
        For Each name As String In addresses
            stringBuilder.Append(name & "; ")
        Next

        Return stringBuilder.ToString()
    End Function

End Class
