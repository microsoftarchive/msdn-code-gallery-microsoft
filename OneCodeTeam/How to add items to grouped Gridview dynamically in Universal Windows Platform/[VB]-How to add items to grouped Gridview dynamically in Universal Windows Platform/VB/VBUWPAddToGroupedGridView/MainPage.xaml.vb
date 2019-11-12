'***************************** Module Header ******************************\
' Module Name:  MainPage.xaml.vb
' Project:      VBUWPAddToGroupedGridView
' Copyright (c) Microsoft Corporation.
' 
' This sample demonstrates how to add item to a grouped GridView in UWP.
'  
'  
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
'  
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/
Imports System.Globalization
Imports System.Threading

Public NotInheritable Class MainPage
    Inherits Page
    Private Shared _source As ObservableCollection(Of GroupInfoCollection(Of Item))

    ''' <summary>
    ''' The variable is used to share information between the handler of the 
    ''' click handler of the add item button and the layout updated
    ''' handler of the grid-view. As both event handlers are executed 
    ''' potentially concurrently, the access must be interlocked. 
    ''' </summary>
    Private _itemAdded As Integer

    Public Sub New()
        Me.InitializeComponent()

        _source = (New StoreData()).GetGroupsByCategory()

        CollectionViewSource.Source = _source

        Dim pictureOptions As New ObservableCollection(Of String)() From {
            "Banana",
            "Lemon",
            "Mint",
            "Orange",
            "SauceCaramel",
            "SauceChocolate",
            "SauceStrawberry",
            "SprinklesChocolate",
            "SprinklesRainbow",
            "SprinklesVanilla",
            "Strawberry",
            "Vanilla"
        }
        PictureComboBox.ItemsSource = pictureOptions
        PictureComboBox.SelectedIndex = 0

        Dim groupOptions As New ObservableCollection(Of String)()
        For Each groupInfoList As GroupInfoCollection(Of Item) In _source
            groupOptions.Add(groupInfoList.Key)
        Next

        GroupComboBox.ItemsSource = groupOptions
        GroupComboBox.SelectedIndex = 0
    End Sub

    Private Async Sub FooterClick(sender As Object, e As RoutedEventArgs)
        Await Windows.System.Launcher.LaunchUriAsync(New Uri(TryCast(sender, HyperlinkButton).Tag.ToString()))
    End Sub

    Private Sub AddItemClick(sender As Object, e As Windows.UI.Xaml.RoutedEventArgs)
        Dim path As String = String.Format(CultureInfo.InvariantCulture, "SampleData/Images/60{0}.png", PictureComboBox.SelectedItem)

        Dim item As New Item
        item.Title = TitleTextBox.Text
        item.Category = DirectCast(GroupComboBox.SelectedItem, String)
        item.SetImage(StoreData.BaseUri, path)
        Dim group As GroupInfoCollection(Of Item) = _source.[Single](Function(groupInfoList) groupInfoList.Key = item.Category)
        group.Add(item)
        Interlocked.Increment(_itemAdded)
    End Sub
    ''' <summary>
    ''' If the selection in the combo-box for the group-selection changes,
    ''' the grouped grid view scrolls the selected group into view. This is 
    ''' especially useful In narrow views. 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub GroupComboBoxSelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim key As String = TryCast(GroupComboBox.SelectedItem, String)
        Dim group As GroupInfoCollection(Of Item) = _source.[Single](Function(groupInfoList) groupInfoList.Key = key)
        ItemsByCategory.ScrollIntoView(group)
    End Sub

    Private Sub ItemsByCategoryLayoutUpdated(sender As Object, e As Object)
        Dim needsToScroll As Integer = Interlocked.Exchange(_itemAdded, 0)

        If needsToScroll <> 0 Then
            Dim key As String = TryCast(GroupComboBox.SelectedItem, String)
            Dim group As GroupInfoCollection(Of Item) = _source.[Single](Function(groupInfoList) groupInfoList.Key = key)
            ItemsByCategory.ScrollIntoView(group)
        End If
    End Sub
End Class
