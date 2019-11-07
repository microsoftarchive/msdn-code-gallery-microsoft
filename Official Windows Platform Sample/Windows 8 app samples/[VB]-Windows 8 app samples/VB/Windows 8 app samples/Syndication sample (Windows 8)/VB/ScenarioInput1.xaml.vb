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
Imports Windows.Web.Syndication

Partial Public NotInheritable Class ScenarioInput1
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As Global.SDKTemplate.MainPage = Nothing
    Private outputField As TextBox = Nothing
    Private feedTitleField As TextBlock = Nothing
    Private itemTitleField As TextBlock = Nothing
    Private linkField As HyperlinkButton = Nothing
    Private contentWebView As WebView = Nothing

    Private currentFeed As SyndicationFeed = Nothing
    Private currentItemIndex As Integer

    Public Sub New()
        InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, Global.SDKTemplate.MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content.
        AddHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        RemoveHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

    Private Sub rootPage_OutputFrameLoaded(sender As Object, e As Object)
        ' At this point, we know that the Output Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Output Frame.

        ' Get a pointer to the content within the OutputFrame.
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)

        ' Make the WebView fills the output frame.
        Dim contentRoot = TryCast(rootPage.FindName("ContentRoot"), Grid)
        If contentRoot IsNot Nothing AndAlso contentRoot.Children.Count >= 2 Then
            Dim scrollViewer = TryCast(contentRoot.Children(1), ScrollViewer)
            If scrollViewer IsNot Nothing Then
                Dim grid = TryCast(scrollViewer.Content, Grid)
                If grid IsNot Nothing Then
                    grid.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch
                End If
            End If
        End If
        outputFrame.Frame.MaxWidth = 2000

        outputField = TryCast(outputFrame.FindName("OutputField"), TextBox)
        feedTitleField = TryCast(outputFrame.FindName("FeedTitleField"), TextBlock)
        itemTitleField = TryCast(outputFrame.FindName("ItemTitleField"), TextBlock)
        linkField = TryCast(outputFrame.FindName("LinkField"), HyperlinkButton)
        contentWebView = TryCast(outputFrame.FindName("ContentWebView"), WebView)
    End Sub

    Private Async Sub GetFeed_Click(sender As Object, e As RoutedEventArgs)
        Dim client As New SyndicationClient
        client.BypassCacheOnRetrieve = True

        ' Some servers require a user-agent.
        client.SetRequestHeader("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)")

        Try
            Dim uri As New Uri(feedUri.Text.Trim)
            outputField.Text = "Downloading feed: " & uri.ToString & vbCr & VBCrLf

            currentFeed = Await client.RetrieveFeedAsync(uri)

            DisplayFeed()
        Catch ex As Exception
            outputField.Text &= ex.Message

            ' Match HResult with a SyndicationErrorStatus value. It is also available WebErrorStatus for
            ' HTTP status error codes.
            Dim status As SyndicationErrorStatus = SyndicationError.GetStatus(ex.HResult)
            If status = SyndicationErrorStatus.InvalidXml Then
                outputField.Text &= "An invalid XML exception was thrown. " & "Are you sure you are pointing to a RSS or Atom feed?"
            End If
        End Try
    End Sub

    Private Sub DisplayFeed()
        outputField.Text &= "Download Complete" & vbCr & VBCrLf

        Dim title As ISyndicationText = currentFeed.Title
        feedTitleField.Text = If(title IsNot Nothing, title.Text, "(no title)")

        currentItemIndex = 0
        If currentFeed.Items.Count > 0 Then
            DisplayCurrentItem()
        End If

        ' List the items.
        outputField.Text &= "Items: " & currentFeed.Items.Count.ToString & vbCrLf
    End Sub

    Private Sub PreviousItem_Click(sender As Object, e As RoutedEventArgs)
        If currentFeed IsNot Nothing AndAlso currentItemIndex > 0 Then
            currentItemIndex -= 1
            DisplayCurrentItem()
        End If
    End Sub

    Private Sub NextItem_Click(sender As Object, e As RoutedEventArgs)
        If currentFeed IsNot Nothing AndAlso currentItemIndex < (currentFeed.Items.Count - 1) Then
            currentItemIndex += 1
            DisplayCurrentItem()
        End If
    End Sub

    Private Sub DisplayCurrentItem()
        Dim item As SyndicationItem = currentFeed.Items(currentItemIndex)

        ' Display item number.
        IndexField.Text = String.Format("{0} of {1}", currentItemIndex + 1, currentFeed.Items.Count)

        ' Title
        itemTitleField.Text = If(item.Title IsNot Nothing, item.Title.Text, "(no title)")

        ' Display the main link.
        Dim link As String = String.Empty
        If item.Links.Count > 0 Then
            link = item.Links(0).Uri.AbsoluteUri
        End If
        linkField.Content = link

        ' Display the body as HTML.
        Dim content As String = "(no content)"
        If item.Content IsNot Nothing Then
            content = item.Content.Text
        ElseIf item.Summary IsNot Nothing Then
            content = item.Summary.Text
        End If
        contentWebView.NavigateToString(content)
    End Sub
End Class
