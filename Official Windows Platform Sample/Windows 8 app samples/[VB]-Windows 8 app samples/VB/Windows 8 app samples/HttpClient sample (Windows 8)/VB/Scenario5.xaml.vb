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
Imports System.Net.Http
Imports System.IO
Imports SDKTemplate

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario5
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Private Async Sub Start_Click(sender As Object, e As RoutedEventArgs)
        If Not rootPage.TryUpdateBaseAddress() Then
            Return
        End If

        rootPage.NotifyUser("In progress", NotifyType.StatusMessage)
        OutputField.Text = String.Empty

        Dim resourceAddress As String = AddressField.Text.Trim()

        Try
            Dim stream As Stream = GenerateSampleStream(1000)
            Dim streamContent As New StreamContent(stream)

            Dim request As New HttpRequestMessage(HttpMethod.Post, resourceAddress)
            request.Content = streamContent
            request.Headers.TransferEncodingChunked = True
            ' Assume we do not know the content length
            Dim response As HttpResponseMessage = Await rootPage.httpClient.SendAsync(request)

            ' The above lines could be replaced by the following if you knew the content-length in advance:
            ' streamContent.Headers.ContentLength = 1000;
            ' HttpResponseMessage response = rootPage.httpClient.PostAsync(resourceAddress, streamContent);

            Await Helpers.DisplayTextResult(response, OutputField)

            rootPage.NotifyUser("Completed", NotifyType.StatusMessage)
        Catch hre As HttpRequestException
            rootPage.NotifyUser("Error", NotifyType.ErrorMessage)
            OutputField.Text = hre.ToString()
        Catch ex As Exception
            ' For debugging
            rootPage.NotifyUser("Error", NotifyType.ErrorMessage)
            OutputField.Text = ex.ToString()
        End Try
    End Sub

    Private Sub Cancel_Click(sender As Object, e As RoutedEventArgs)
        rootPage.NotifyUser("Cancelling", NotifyType.StatusMessage)
        rootPage.httpClient.CancelPendingRequests()
    End Sub

    Private Shared Function GenerateSampleStream(size As Integer) As MemoryStream
        ' Generate sample data
        Dim stream As New MemoryStream(size)
        Dim subData As Byte() = New Byte(size - 1) {}
        For i As Integer = 0 To subData.Length - 1
            ' '@'
            subData(i) = 64
        Next
        stream.Write(subData, 0, subData.Length)
        stream.Seek(0, SeekOrigin.Begin)
        Return stream
    End Function
End Class
