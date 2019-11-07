Imports Microsoft.SharePoint.Client

''' <summary>
''' This Silverlight application creates a new item in the Annoucements list
''' in the current SharePoint site.
''' </summary>
''' <remarks>
''' To use this application, compile it then upload the XAP file to a SharePoint 
''' library such as Site Assets. Add a Silverlight Web Part to a page in the UI and
''' configure it to display the SILVERLIGHT_CreateListItem.xap file in the SharePoint
''' library.
''' </remarks>
Partial Public Class MainPage
    Inherits UserControl

    Private annoucementsList As List

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub createAnnouncementButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        'Only create the item if there is a title
        If titleTextbox.Text <> String.Empty Then
            'Get the current context
            Dim context As ClientContext = ClientContext.Current
            'Get the Announcements list and add a new item
            annoucementsList = context.Web.Lists.GetByTitle("Announcements")
            Dim newItem As ListItem = annoucementsList.AddItem(New ListItemCreationInformation())
            'Set the new item's properties
            newItem("Title") = titleTextbox.Text
            newItem("Body") = bodyTextbox.Text + vbLf + vbTab + "This announcement was created by Silverlight code!"
            newItem.Update()
            'Load the list
            context.Load(annoucementsList, Function(list) list.Title)
            'Execute the query to create the new item
            context.ExecuteQueryAsync(AddressOf onQuerySucceeded, AddressOf onQueryFailed)
        End If
    End Sub

    Private Sub onQuerySucceeded(ByVal sender As Object, ByVal args As ClientRequestSucceededEventArgs)
        'The query succeeded but this method does not run in the UI thread.
        'We must use the Dispatcher to begin another method in the UI thread.
        Dim updateUI As UpdateUIMethod = AddressOf DisplayInfo
        Me.Dispatcher.BeginInvoke(updateUI)
    End Sub

    Private Sub onQueryFailed(ByVal sender As Object, ByVal args As ClientRequestFailedEventArgs)
        'The query failed, display the reason
        resultLabel.Content = "Request Failed: " + args.Message
    End Sub

    Private Sub DisplayInfo()
        'The item was successfully created so let the user know
        resultLabel.Content = "New item successfully created"
    End Sub

    Private Delegate Sub UpdateUIMethod()

End Class