'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

' The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Async Sub OnNavigatedTo(e As Navigation.NavigationEventArgs)
        Dim feedDataSource = DirectCast(App.Current.Resources("feedDataSource"), FeedDataSource)

        If feedDataSource IsNot Nothing Then
            If feedDataSource.Feeds.Count = 0 Then
                Await feedDataSource.GetFeedsAsync()
            End If
            Me.DataContext = (feedDataSource.Feeds).First()
        End If
    End Sub

    Private Sub ItemListView_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles ItemListView.SelectionChanged
        If e.AddedItems.Count > 0 Then
            Dim feedItem = DirectCast(e.AddedItems(0), FeedItem)
            If feedItem IsNot Nothing Then
                ' Navigate the WebView to the blog post content HTML string.
                ContentView.NavigateToString(feedItem.Content)
            End If
        Else
            ' If the item was de-selected, clear the WebView.
            ContentView.NavigateToString("")
        End If
    End Sub

End Class
