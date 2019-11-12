' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System.Linq
Imports System.Collections.Generic
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.Graphics.Display
Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation
Imports Windows.Security.Cryptography.Certificates
Imports System.Net
Imports System.Text
Imports System.IO
Imports System.Threading.Tasks
Imports System.Xml
Imports Windows.Data.Xml.Dom
Imports SDKTemplate

Partial Public NotInheritable Class ScenarioInput1
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing
    Private certificateRequest As String

    Public Sub New()
        InitializeComponent()
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content.
        AddHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        RemoveHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub
#End Region

#Region "Use this code if you need access to elements in the output frame - otherwise delete"
    Private Sub rootPage_OutputFrameLoaded(sender As Object, e As Object)
        ' At this point, we know that the Output Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Output Frame.

        ' Get a pointer to the content within the OutputFrame.
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)

        ' Go find the elements that we need for this scenario.
        ' ex: flipView1 = TryCast(outputFrame.FindName("FlipView1"), FlipView)
    End Sub
#End Region

#Region "Sample Click Handlers - modify if you need them, delete them otherwise"
    ''' <summary>
    ''' This is the click handler for the 'Default' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub CreateRequest_Click(sender As Object, e As RoutedEventArgs)
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)

        Dim outputTextBlock As TextBlock = TryCast(outputFrame.FindName("Scenario1Result"), TextBlock)
        outputTextBlock.Text = "Creating certificate request..."

        Try
            'call the default constructor of CertificateRequestProperties
            Dim reqProp As New CertificateRequestProperties
            reqProp.Subject = "Toby"
            reqProp.FriendlyName = "Toby's Cert"

            'call Certificate Enrollment function createRequest to create a certificate request
            certificateRequest = Await CertificateEnrollmentManager.CreateRequestAsync(reqProp)
            outputTextBlock.Text &= vbCrLf & "Request created, content:" & vbCrLf & certificateRequest
        Catch ex As Exception
            outputTextBlock.Text &= vbCrLf & vbCrLf & "Certificate request creation failed with error: " & ex.Message
        End Try
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Other' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub InstallCertifiate_Click(sender As Object, e As RoutedEventArgs)
        Dim response As String = ""

        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        Dim outputTextBlock As TextBlock = TryCast(outputFrame.FindName("Scenario1Result"), TextBlock)

        If String.IsNullOrEmpty(certificateRequest) Then
            outputTextBlock.Text &= vbCrLf & "You need to create a certificate request first"
            Exit Sub
        End If

        ' A valid z need to be provided, or else this will not work.
        Dim url As String = ""
        ' Add code here to initialize url

        If String.IsNullOrEmpty(url) Then
            outputTextBlock.Text = vbCrLf & "To submit a request, please update the code provide a valid URL."
            Exit Sub
        End If

        Try
            outputTextBlock.Text = "Submitting request to server..."
            response = Await SubmitCertificateRequestAndGetResponse(certificateRequest, url)

            If String.IsNullOrEmpty(response) Then
                outputTextBlock.Text &= vbCrLf & "Submit request succeeded but the returned response is empty"
            End If

            outputTextBlock.Text &= vbCrLf & "Response received, content: " & vbCrLf & response

            ' install
            outputTextBlock.Text &= vbCrLf & "Installing certificate ..."
            Await CertificateEnrollmentManager.InstallCertificateAsync(response, InstallOptions.None)
            outputTextBlock.Text &= vbCrLf & "The certificate response is installed sucessfully." & vbCrLf
        Catch ex As Exception
            outputTextBlock.Text &= vbCrLf & vbCrLf & "Certificate Installation failed with error: " & ex.Message & vbCrLf
        End Try
    End Sub

    ''' <summary>
    ''' Submit a certificate request to a Certificate Services
    ''' The request is encapsulated in an XML and sent to server over http, the XML format is defined by the server.
    ''' </summary>
    ''' <param name="certificateRequest"></param>
    ''' <param name="url"></param>
    ''' <returns></returns>
    Private Shared Async Function SubmitCertificateRequestAndGetResponse(certificateRequest As String, url As String) As Task(Of String)
        Dim webRequest__1 As WebRequest = WebRequest.Create(url)
        webRequest__1.ContentType = "text/xml;charset=utf-8"
        webRequest__1.Method = "POST"

        Dim body As String = "<SubmitRequest xmlns=""http://tempuri.org/""><strRequest>" & certificateRequest & "</strRequest></SubmitRequest>"
        Dim writeBuffer() As Byte = Encoding.UTF8.GetBytes(body)
        Using stream As Stream = Await webRequest__1.GetRequestStreamAsync
            stream.Write(writeBuffer, 0, writeBuffer.Length)
        End Using

        ' response
        Dim respAsyncResult = Await webRequest__1.GetResponseAsync
        Dim rawServerResponse As String = Nothing

        Using response As WebResponse = Await webRequest__1.GetResponseAsync
            Using responseStream As Stream = response.GetResponseStream
                Using reader As New StreamReader(responseStream)
                    ' read response data
                    rawServerResponse = reader.ReadToEnd
                End Using
            End Using
        End Using

        Dim xDoc As New XmlDocument
        xDoc.LoadXml(rawServerResponse)
        Dim nodeList = xDoc.GetElementsByTagName("SubmitRequestResult")
        If nodeList.Length <> 1 Then
            Throw New XmlException("The certificate response is not in expected XML format.")
        End If
        Return nodeList.ElementAt(0).InnerText
    End Function
#End Region

End Class
