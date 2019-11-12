'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

' The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

''' <summary>
''' A basic page that provides characteristics common to most applications.
''' </summary>
Public NotInheritable Class DetailPage
    Inherits Common.LayoutAwarePage

    ''' <summary>
    ''' Populates the page with content passed during navigation.  Any saved state is also
    ''' provided when recreating a page from a prior session.
    ''' </summary>
    ''' <param name="navigationParameter">The parameter value passed to
    ''' <see cref="Frame.Navigate"/> when this page was initially requested.
    ''' </param>
    ''' <param name="pageState">A dictionary of state preserved by this page during an earlier
    ''' session.  This will be null the first time a page is visited.</param>
    Protected Overrides Sub LoadState(navigationParameter As Object, pageState As Dictionary(Of String, Object))
        ' Run the PopInThemeAnimation 
        Dim sb As Windows.UI.Xaml.Media.Animation.Storyboard = Me.FindName("PopInStoryboard")
        If sb IsNot Nothing Then
            sb.Begin()
        End If

        ' Add this code to navigate the web view to the selected blog post.
        Dim itemTitle = DirectCast(navigationParameter, String)
        Dim feedItem = FeedDataSource.GetItem(itemTitle)
        If feedItem IsNot Nothing Then
            Me.contentView.Navigate(feedItem.Link)
            Me.DataContext = feedItem
        End If
    End Sub

    ''' <summary>
    ''' Preserves state associated with this page in case the application is suspended or the
    ''' page is discarded from the navigation cache.  Values must conform to the serialization
    ''' requirements of <see cref="Common.SuspensionManager.SessionState"/>.
    ''' </summary>
    ''' <param name="pageState">An empty dictionary to be populated with serializable state.</param>
    Protected Overrides Sub SaveState(pageState As Dictionary(Of String, Object))

    End Sub

    Private Sub contentView_NavigationFailed(sender As Object, e As WebViewNavigationFailedEventArgs)
        Dim errorString = "<p>Page could not be loaded.</p><p>Error is: " & e.WebErrorStatus.ToString() & "</p>"
        Me.contentView.NavigateToString(errorString)
    End Sub

End Class
