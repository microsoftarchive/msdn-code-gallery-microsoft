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
Imports SDKTemplate

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario4
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

    Private Async Sub Start_Click(sender As Object, e As RoutedEventArgs)
        If Not rootPage.TryUpdateBaseAddress() Then
            Return
        End If

        rootPage.NotifyUser("In progress", NotifyType.StatusMessage)
        OutputField.Text = String.Empty

        Dim resourceAddress As String = AddressField.Text.Trim()

        Try
            Dim response As HttpResponseMessage = Await rootPage.httpClient.PostAsync(resourceAddress, New StringContent(RequestBodyField.Text))
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
End Class
