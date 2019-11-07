' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System.Net
Imports Windows.Security.Credentials
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.Web
Imports Windows.Web.AtomPub
Imports Windows.Web.Syndication
Imports SDKTemplate

Partial Public NotInheritable Class ScenarioInput1
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    ' Helps iterating through feeds.
    Private feedIterator As New SyndicationItemIterator

    ' Controls from the output frame.
    Private outputField As TextBox
    Private indexField As TextBlock
    Private titleField As TextBlock
    Private contentWebView As WebView

    Public Sub New()
        InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, Global.SDKTemplate.MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content.
        AddHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded

        CommonData.Restore(Me)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        RemoveHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

    Protected Overrides Sub OnNavigatingFrom(e As NavigatingCancelEventArgs)
        CommonData.Save(Me)
    End Sub

    Private Sub rootPage_OutputFrameLoaded(sender As Object, e As Object)
        ' At this point, we know that the Output Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Output Frame.

        ' Get a pointer to the content within the OutputFrame.
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)

        indexField = TryCast(FindName("IndexFieldTextBox"), TextBlock)
        outputField = TryCast(outputFrame.FindName("OutputField"), TextBox)
        titleField = TryCast(outputFrame.FindName("TitleField"), TextBlock)
        contentWebView = TryCast(outputFrame.FindName("ContentWebView"), WebView)
    End Sub

    Private Async Sub GetFeed_Click(sender As Object, e As RoutedEventArgs)
        Try
            ' Note that this feed is public by default and will not require authentication.
            ' We will only get back a limited use feed, without information about editing.
            Dim resourceUri As New Uri(ServiceAddressField.Text.Trim & CommonData.FeedUri.ToString)

            outputField.Text = "Fetching resource: " & resourceUri.ToString & vbCr & vbCrLf

            feedIterator.AttachFeed(Await CommonData.GetClient.RetrieveFeedAsync(resourceUri))

            outputField.Text &= "Complete" & vbCr & vbCrLf
            DisplayItem()
        Catch ex As Exception
            outputField.Text &= ex.ToString & vbCr & vbCrLf
        End Try
    End Sub

    Private Sub PreviousItem_Click(sender As Object, e As RoutedEventArgs)
        feedIterator.MovePrevious()
        DisplayItem()
    End Sub

    Private Sub NextItem_Click(sender As Object, e As RoutedEventArgs)
        feedIterator.MoveNext()
        DisplayItem()
    End Sub

    Private Sub UserNameField_TextChanged(sender As Object, e As TextChangedEventArgs)
        CommonData.Save(Me)
    End Sub

    Private Sub PasswordField_PasswordChanged(sender As Object, e As RoutedEventArgs)
        CommonData.Save(Me)
    End Sub

    Private Sub DisplayItem()
        indexField.Text = feedIterator.GetIndexDescription
        titleField.Text = feedIterator.GetTitle
        contentWebView.NavigateToString(feedIterator.GetContent)
    End Sub
End Class
