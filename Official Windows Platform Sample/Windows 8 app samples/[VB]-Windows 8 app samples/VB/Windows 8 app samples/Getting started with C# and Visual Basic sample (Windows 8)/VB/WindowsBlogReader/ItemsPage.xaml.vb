'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

' The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

''' <summary>
''' A page that displays a collection of item previews.  In the Split Application this page
''' is used to display and select one of the available groups.
''' </summary>
Public NotInheritable Class ItemsPage
    Inherits Common.LayoutAwarePage

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
        ' TODO: Assign a bindable collection of items to Me.DefaultViewModel("Items")
        Dim feedDataSource = DirectCast(App.Current.Resources("feedDataSource"), FeedDataSource)
        If feedDataSource IsNot Nothing Then
            Me.DefaultViewModel("Items") = feedDataSource.Feeds
        End If

    End Sub

    Private Sub ItemView_ItemClick(sender As Object, e As ItemClickEventArgs) 
        ' Navigate to the split page, configuring the new page
        ' by passing the title of the clicked item as a navigation parameter
        If e.ClickedItem IsNot Nothing Then
            Dim title = (DirectCast(e.ClickedItem, FeedData)).Title
            Me.Frame.Navigate(GetType(SplitPage), title)
        End If
    End Sub
End Class
