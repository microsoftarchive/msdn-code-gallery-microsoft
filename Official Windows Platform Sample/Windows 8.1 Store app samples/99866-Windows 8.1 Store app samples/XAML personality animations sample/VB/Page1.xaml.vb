' Copyright (c) Microsoft. All rights reserved.

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Page1
    Inherits Page

    Private messageData As MessageData = Nothing
    Public Sub New()
        Me.InitializeComponent()

        messageData = New MessageData()
        GridView1.ItemsSource = messageData.Collection
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub


End Class

