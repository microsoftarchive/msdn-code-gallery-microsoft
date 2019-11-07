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

Partial Public NotInheritable Class ScenarioInput4
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    ' Helps iterating through feeds.
    Private feedIterator As New SyndicationItemIterator

    ' Controls from the output frame.
    Private outputField As TextBox
    Private indexField As TextBlock
    Private titleField As TextBox
    Private contentField As TextBox

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

        outputField = TryCast(outputFrame.FindName("OutputField"), TextBox)
        titleField = TryCast(outputFrame.FindName("TitleBox"), TextBox)
        contentField = TryCast(outputFrame.FindName("ContentBox"), TextBox)
        indexField = TryCast(FindName("IndexFieldTextBox"), TextBlock)
    End Sub

    ' Download a feed.
    Private Async Sub GetFeed_Click(sender As Object, e As RoutedEventArgs)
        Try
            ' If we retrieve the feed via the Edit uri then we will be logged in and be
            ' able to modify/delete the resource.
            Dim resourceUri As New Uri(ServiceAddressField.Text.Trim & CommonData.EditUri)

            outputField.Text = "Fetching feed: " & resourceUri.ToString & vbCr & vbCrLf

            feedIterator.AttachFeed(Await CommonData.GetClient.RetrieveFeedAsync(resourceUri))

            If feedIterator.HasElements Then
                outputField.Text &= "Got feed" & vbCr & vbCrLf
                outputField.Text &= "Title: " & feedIterator.GetTitle & vbCr & vbCrLf
                outputField.Text &= "EditUri: " & feedIterator.GetEditUri.ToString & vbCr & vbCrLf

                DisplayItem()
            Else
                outputField.Text &= "Got empty feed" & vbCr & vbCrLf
            End If

            outputField.Text &= "Complete" & vbCr & vbCrLf
        Catch ex As Exception
            Dim status = WebError.GetStatus(ex.HResult)
            If status = WebErrorStatus.Unauthorized Then
                outputField.Text &= "Wrong username or password!" & vbCr & vbCrLf
            End If
            outputField.Text &= ex.ToString & vbCr & vbCrLf
        End Try
    End Sub

    ' Update the current item.
    Private Async Sub UpdateItem_Click(sender As Object, e As RoutedEventArgs)
        Try
            If Not feedIterator.HasElements Then
                outputField.Text &= "No item currently displayed, please download a feed first." & vbCr & vbCrLf
                Exit Sub
            End If

            outputField.Text &= "Updating item: " & feedIterator.GetEditUri.ToString & vbCr & vbCrLf

            ' Update the item
            Dim updatedItem As New SyndicationItem
            updatedItem.Title = New SyndicationText(titleField.Text, SyndicationTextType.Text)
            updatedItem.Content = New SyndicationContent(contentField.Text, SyndicationTextType.Html)

            Await CommonData.GetClient().UpdateResourceAsync(feedIterator.GetEditUri(), updatedItem)

            outputField.Text &= "Complete" & vbCr & vbCrLf
        Catch ex As Exception
            Dim status As WebErrorStatus = WebError.GetStatus(ex.HResult)
            If status = WebErrorStatus.Unauthorized Then
                outputField.Text &= "Wrong user name or password!" & vbCr & vbCrLf
            End If
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
        contentField.Text = feedIterator.GetContent
    End Sub
End Class
