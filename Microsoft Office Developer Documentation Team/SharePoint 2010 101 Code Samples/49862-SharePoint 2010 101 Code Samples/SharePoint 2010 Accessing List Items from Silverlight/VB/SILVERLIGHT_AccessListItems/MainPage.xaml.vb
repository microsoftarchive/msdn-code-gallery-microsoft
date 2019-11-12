Imports Microsoft.SharePoint.Client

''' <summary>
''' This Silverlight application queries all the items in the Tasks list in the
''' current SharePoint site and displays them in a text box control. It assumes 
''' there is a Tasks list.
''' </summary>
''' <remarks>
''' To use this application, compile it then upload the XAP file to a SharePoint 
''' library such as Site Assets. Add a Silverlight Web Part to a page in the UI and
''' configure it to display the SILVERLIGHT_AccessListItems.xap file in the SharePoint
''' library.
''' </remarks>
Partial Public Class MainPage
    Inherits UserControl

    'Internal objects
    Dim currentWebSite As Web
    Dim tasksList As List
    Dim taskItems As ListItemCollection

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub showTaskListItemsButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        'Start but getting the current SharePoint site
        Dim context As ClientContext = ClientContext.Current
        currentWebSite = context.Web
        context.Load(currentWebSite)
        'Get the Tasks list
        tasksList = context.Web.Lists.GetByTitle("Tasks")
        'Query the Task list for all items. We can use a blank CamlQuery object for this
        Dim query As CamlQuery = New CamlQuery()
        taskItems = tasksList.GetItems(query)
        context.Load(taskItems)
        'Run the query asynchronously
        context.ExecuteQueryAsync(AddressOf onQuerySucceeded, AddressOf onQueryFailed)
    End Sub

    Private Sub onQuerySucceeded(ByVal sender As Object, ByVal args As ClientRequestSucceededEventArgs)
        'The query succeeded but this method does not run in the UI thread.
        'We must use the Dispatcher to begin another method in the UI thread.
        Dim updateUI As UpdateUIMethod = AddressOf DisplayInfo
        Me.Dispatcher.BeginInvoke(updateUI)
    End Sub

    Private Sub onQueryFailed(ByVal sender As Object, ByVal args As ClientRequestFailedEventArgs)
        'The query failed, display the reason
        MessageBox.Show("Request Failed: " + args.Message)
    End Sub

    Private Sub DisplayInfo()
        'We add text to the displayTextBlock to show Task items
        displayTextBlock.Text = "Items in the Tasks List: " + taskItems.Count.ToString()
        For Each currentItem As ListItem In taskItems
            displayTextBlock.Text += vbLf + vbTab + "Item ID: " + currentItem.Id.ToString()
            displayTextBlock.Text += vbLf + vbTab + "Item Title: " + currentItem("Title")
        Next
    End Sub

    Private Delegate Sub UpdateUIMethod()
End Class