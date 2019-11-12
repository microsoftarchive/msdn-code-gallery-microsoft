'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

' The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234

''' <summary>
''' A page that displays a group title, a list of items within the group, and details for
''' the currently selected item.
''' </summary>
Public NotInheritable Class SplitPage
    Inherits Common.LayoutAwarePage

    Private Sub ViewDetail_Click(sender As Object, e As RoutedEventArgs)
        Dim selectedItem = DirectCast(Me.itemListView.SelectedItem, FeedItem)
        If selectedItem IsNot Nothing AndAlso Me.Frame IsNot Nothing Then
            Dim itemTitle = selectedItem.Title
            Me.Frame.Navigate(GetType(DetailPage), itemTitle)
        End If
    End Sub

#Region "Page state management"

    ''' <summary>
    ''' Populates the page with content passed during navigation.  Any saved state is also
    ''' provided when recreating a page from a prior session.
    ''' </summary>
    ''' <param name="navigationParameter">The parameter value passed to <see cref="Frame.Navigate"/>
    ''' when this page was initially requested.
    ''' </param>
    ''' <param name="pageState">A dictionary of state preserved by this page during an earlier
    ''' session.  This will be null the first time a page is visited.</param>
    Protected Overrides Sub LoadState(navigationParameter As Object, pageState As Dictionary(Of String, Object))
        ' Run the PopInThemeAnimation 
        Dim sb As Windows.UI.Xaml.Media.Animation.Storyboard = Me.FindName("PopInStoryboard")
        If sb IsNot Nothing Then
            sb.Begin()
        End If

        ' TODO: Assign a bindable group to Me.DefaultViewModel("Group")
        ' TODO: Assign a collection of bindable items to Me.DefaultViewModel("Items")
        Dim feedTitle = DirectCast(navigationParameter, String)

        Dim feedData = FeedDataSource.GetFeed(feedTitle)
        If feedData IsNot Nothing Then
            Me.DefaultViewModel("Feed") = feedData
            Me.DefaultViewModel("Items") = feedData.Items
        End If


        If pageState Is Nothing Then

            ' When this is a new page, select the first item automatically unless logical page
            ' navigation is being used (see the logical page navigation #region below.)
            If Not Me.UsingLogicalPageNavigation() AndAlso Me.itemsViewSource.View IsNot Nothing Then
                Me.itemsViewSource.View.MoveCurrentToFirst()
            Else
                Me.itemsViewSource.View.MoveCurrentToPosition(-1)
            End If
        Else

            ' Restore the previously saved state associated with this page
            If pageState.ContainsKey("SelectedItem") AndAlso Me.itemsViewSource.View IsNot Nothing Then
                ' TODO: Invoke Me.itemsViewSource.View.MoveCurrentTo() with the selected
                '       item as specified by the value of pageState("SelectedItem")
                Dim itemTitle = DirectCast(pageState("SelectedItem"), String)
                Dim selectedItem = FeedDataSource.GetItem(itemTitle)
                Me.itemsViewSource.View.MoveCurrentTo(selectedItem)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Preserves state associated with this page in case the application is suspended or the
    ''' page is discarded from the navigation cache.  Values must conform to the serialization
    ''' requirements of <see cref="Common.SuspensionManager.SessionState"/>.
    ''' </summary>
    ''' <param name="pageState">An empty dictionary to be populated with serializable state.</param>
    Protected Overrides Sub SaveState(pageState As Dictionary(Of String, Object))
        If Me.itemsViewSource.View IsNot Nothing Then
            Dim selectedItem As Object = Me.itemsViewSource.View.CurrentItem
            ' TODO: Derive a serializable navigation parameter and assign it to
            '       pageState("SelectedItem")
            If selectedItem IsNot Nothing Then
                Dim itemTitle = (DirectCast(selectedItem, FeedItem)).Title
                pageState("SelectedItem") = itemTitle
            End If

        End If
    End Sub

#End Region

#Region "Logical page navigation"

    ' Visual state management typically reflects the four application view states directly
    ' (full screen landscape and portrait plus snapped and filled views.)  The split page is
    ' designed so that the snapped and portrait view states each have two distinct sub-states:
    ' either the item list or the details are displayed, but not both at the same time.
    '
    ' This is all implemented with a single physical page that can represent two logical
    ' pages.  The code below achieves this goal without making the user aware of the
    ' distinction.

    ''' <summary>
    ''' Invoked to determine whether the page should act as one logical page or two.
    ''' </summary>
    ''' <param name="viewState">The view state for which the question is being posed, or null
    ''' for the current view state.  This parameter is optional with null as the default
    ''' value.</param>
    ''' <returns>True when the view state in question is portrait or snapped, false
    ''' otherwise.</returns>
    Private Function UsingLogicalPageNavigation(Optional viewState As ApplicationViewState? = Nothing) As Boolean
        If Not viewState.HasValue Then viewState = ApplicationView.Value
        Return viewState.Value = ApplicationViewState.FullScreenPortrait OrElse
            viewState.Value = ApplicationViewState.Snapped
    End Function

    ''' <summary>
    ''' Invoked when an item within the list is selected.
    ''' </summary>
    ''' <param name="sender">The GridView (or ListView when the application is Snapped)
    ''' displaying the selected item.</param>
    ''' <param name="e">Event data that describes how the selection was changed.</param>
    Private Sub ItemListView_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

        ' Invalidate the view state when logical page navigation is in effect, as a change in
        ' selection may cause a corresponding change in the current logical page.  When an item
        ' is selected this has the effect of changing from displaying the item list to showing
        ' the selected item's details.  When the selection is cleared this has the opposite effect.
        If Me.UsingLogicalPageNavigation Then Me.InvalidateVisualState()

        ' Add this code to populate the web view
        ' with the content of the selected blog post.
        Dim list = DirectCast(sender, Selector)
        Dim selectedItem = DirectCast(list.SelectedItem, FeedItem)
        If selectedItem IsNot Nothing Then
            Me.contentView.NavigateToString(selectedItem.Content)
        Else
            Me.contentView.NavigateToString("")
        End If

    End Sub

    ''' <summary>
    ''' Invoked when the page's back button is pressed.
    ''' </summary>
    ''' <param name="sender">The back button instance.</param>
    ''' <param name="e">Event data that describes how the back button was clicked.</param>
    Protected Overrides Sub GoBack(sender As Object, e As RoutedEventArgs)
        If Me.UsingLogicalPageNavigation() AndAlso Me.itemListView.SelectedItem IsNot Nothing Then

            ' When logical page navigation is in effect and there's a selected item that item's
            ' details are currently displayed.  Clearing the selection will return to the item
            ' list.  From the user's point of view this is a logical backward navigation.
            Me.itemListView.SelectedItem = Nothing
        Else

            ' When logical page navigation is not in effect, or when there is no selected item,
            ' use the default back button behavior.
            MyBase.GoBack(sender, e)
        End If
    End Sub

    ''' <summary>
    ''' Invoked to determine the name of the visual state that corresponds to an application
    ''' view state.
    ''' </summary>
    ''' <param name="viewState">The view state for which the question is being posed.</param>
    ''' <returns>The name of the desired visual state.  This is the same as the name of the
    ''' view state except when there is a selected item in portrait and snapped views where
    ''' this additional logical page is represented by adding a suffix of _Detail.</returns>
    Protected Overrides Function DetermineVisualState(viewState As ApplicationViewState) As String

        ' Update the back button's enabled state when the view state changes
        Dim logicalPageBack As Boolean = Me.UsingLogicalPageNavigation(viewState) AndAlso Me.itemListView.SelectedItem IsNot Nothing
        Dim physicalPageBack As Boolean = Me.Frame IsNot Nothing AndAlso Me.Frame.CanGoBack
        Me.DefaultViewModel("CanGoBack") = logicalPageBack OrElse physicalPageBack

        ' Determine visual states for landscape layouts based not on the view state, but
        ' on the width of the window.  This page has one layout that is appropriate for
        ' 1366 virtual pixels or wider, and another for narrower displays or when a snapped
        ' application reduces the horizontal space available to less than 1366.
        If viewState = ApplicationViewState.Filled OrElse
            viewState = ApplicationViewState.FullScreenLandscape Then

            Dim windowWidth As Double = Window.Current.Bounds.Width
            If windowWidth >= 1366 Then Return "FullScreenLandscapeOrWide"
            Return "FilledOrNarrow"
        End If

        ' When in portrait or snapped start with the default visual state name, then add a
        ' suffix when viewing details instead of the list
        Dim defaultStateName As String = MyBase.DetermineVisualState(viewState)
        If logicalPageBack Then Return defaultStateName + "_Detail"
        Return defaultStateName
    End Function

#End Region

    Private Sub ContentView_NavigationFailed(sender As Object, e As WebViewNavigationFailedEventArgs) _
        Handles contentView.NavigationFailed
        Dim errorString = "<p>Page could not be loaded.</p><p>Error is: " & e.WebErrorStatus.ToString() & "</p>"
        Me.contentView.NavigateToString(errorString)
    End Sub
End Class
