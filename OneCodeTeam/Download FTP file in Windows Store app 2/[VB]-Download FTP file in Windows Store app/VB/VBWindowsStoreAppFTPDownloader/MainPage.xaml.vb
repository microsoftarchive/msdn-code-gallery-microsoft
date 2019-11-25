'*************************** Module Header ******************************\
' Module Name:  MainPage.xaml.vb
' Project:	    VBWindowsStoreAppFTPDownloader
' Copyright (c) Microsoft Corporation.
' 
' The main UI of this app.
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports System.Linq
Imports System.Net
Imports System.Text
Imports Windows.UI.Notifications
Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation


''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class MainPage
    Inherits Common.LayoutAwarePage
    Private serverUrl As String
    Private pathStack As Stack(Of String)
    Private credential As NetworkCredential

    Public Sub New()
        Me.InitializeComponent()
    End Sub


#Region "Page methods"

    ''' <summary>
    '''  Subscribe download completed event.
    ''' </summary>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        MyBase.OnNavigatedTo(e)

        AddHandler FTP.FTPDownloadManager.Instance.DownloadCompleted, AddressOf FTPItem_DownloadCompleted
        FTP.FTPDownloadManager.Instance.Initialize()
    End Sub

    ''' <summary>
    ''' Unsubscribe download completed event.
    ''' </summary>
    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        MyBase.OnNavigatedFrom(e)
        RemoveHandler FTP.FTPDownloadManager.Instance.DownloadCompleted, AddressOf FTPItem_DownloadCompleted
    End Sub

#End Region

#Region "UI event handlers"

    ''' <summary>
    ''' Connect to FTP server and list the sub folders and file.
    ''' </summary>
    Private Sub ConnectFTPServerButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim serverUri As Uri = Nothing
        Dim serverUriStr As String = serverName.Text.Trim()

        If Uri.TryCreate(serverUriStr, UriKind.Absolute, serverUri) Then
            serverUrl = serverUri.ToString()
            pathStack = New Stack(Of String)()

            If (Not String.IsNullOrEmpty(userName.Text.Trim())) AndAlso (Not String.IsNullOrEmpty(password.Password.Trim())) Then
                credential = New System.Net.NetworkCredential(userName.Text.Trim(), password.Password.Trim())

            Else
                credential = Nothing
            End If

            ' List the sub folders and file.
            ListDirectory()
        Else
            NotifyUser(serverUriStr & " is not a valid FTP server")
        End If

    End Sub

    ''' <summary>
    ''' When use click an item:
    ''' 1. If it is a directory, navigate to the sub folder.
    ''' 2. If it is a file, select it.
    ''' </summary>
    Private Sub itemsView_ItemClick(ByVal sender As Object, ByVal e As ItemClickEventArgs)
        Dim clickedItem = CType(e.ClickedItem, DataModel.SampleDataItem)
        If clickedItem.Content.IsDirectory Then
            Me.pathStack.Push(clickedItem.Content.Name)
            Me.ListDirectory()
        Else
            TryCast(sender, ListViewBase).SelectedItems.Clear()
            TryCast(sender, ListViewBase).SelectedItems.Add((TryCast(sender, ListViewBase)).Items.First(Function(i) i Is clickedItem))
        End If
    End Sub

    ''' <summary>
    ''' Back to parent folder.
    ''' </summary>
    Private Sub backButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If Me.pathStack.Count > 0 Then
            Me.pathStack.Pop()
            Me.ListDirectory()
        End If
    End Sub

    Private isSyncing As Boolean = False

    ''' <summary>
    ''' When selection changed
    ''' 1. Sync the selection of GridView and ListView.
    ''' 2. Show the app bar.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub itemsView_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
        If isSyncing Then
            Return
        End If

        isSyncing = True
        If sender Is itemGridView Then
            itemListView.SelectedItems.Clear()
            For Each item As Object In itemGridView.SelectedItems
                itemListView.SelectedItems.Add(itemListView.Items.First(Function(i) i Is item))
            Next item
        ElseIf sender Is itemListView Then
            itemGridView.SelectedItems.Clear()
            For Each item As Object In itemListView.SelectedItems
                itemGridView.SelectedItems.Add(itemGridView.Items.First(Function(i) i Is item))
            Next item
        End If
        isSyncing = False

        downloadButton.Visibility = If((TryCast(sender, ListViewBase)).SelectedItems.Count > 0, Visibility.Visible, Visibility.Collapsed)
        manageAppBar.IsOpen = True
    End Sub

    ''' <summary>
    ''' Download the selected items.
    ''' </summary>
    Private Async Sub downloadButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)

        Dim unsnapped As Boolean = ((ApplicationView.Value <> ApplicationViewState.Snapped) OrElse ApplicationView.TryUnsnap())

        If Not unsnapped Then
            NotifyUser("Cannot unsnap the sample.")
            Return
        End If


        ' Select a folder as target.
        Dim picker As New Windows.Storage.Pickers.FolderPicker()
        picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary

        picker.FileTypeFilter.Add("*")

        Dim folder = Await picker.PickSingleFolderAsync()

        If folder IsNot Nothing Then

            ' Download the items.
            Dim itemsToDownload = itemGridView.SelectedItems.Select(Function(i) (TryCast(i, DataModel.SampleDataItem)).Content)
            FTP.FTPDownloadManager.Instance.DownloadFTPItemsAsync(itemsToDownload, folder, credential)
        End If

        ' Clear the selection.
        itemGridView.SelectedItems.Clear()
        itemListView.SelectedItems.Clear()
    End Sub

    ''' <summary>
    ''' Refresh the explorer.
    ''' </summary>
    Private Sub refreshButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)

        If Not String.IsNullOrEmpty(serverUrl) And Not pathStack Is Nothing Then
            Me.ListDirectory()
        End If

    End Sub

#End Region

    ''' <summary>
    ''' Show toast notification when a file is downloaded.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub FTPItem_DownloadCompleted(ByVal sender As Object, ByVal e As FTP.DownloadCompletedEventArgs)

        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                                  Sub()
                                      If e.DownloadError Is Nothing Then
                                          progressInfo.Items.Insert(0, String.Format("{0} is completed.",
                                                                                     e.RequestFile.ToString()))
                                      Else
                                          progressInfo.Items.Insert(0, String.Format("{0} is not completed: {1}.",
                                                                                     e.RequestFile.ToString(),
                                                                                     e.DownloadError.Message))
                                      End If
                                  End Sub)

    End Sub



#Region "FTP methods"

    ''' <summary>
    '''  List the sub folders and file.
    '''  Generate the data source and then bind to the Controls.
    ''' </summary>
    Private Async Sub ListDirectory()
        NotifyUser("")
        getDataProgress.Visibility = Windows.UI.Xaml.Visibility.Visible
        Dim relativePath As String = GenerateRelativePath(pathStack)

        Try
            Dim items As IEnumerable(Of FTP.FTPFileSystem) = Await FTP.FTPDownloadManager.Instance.ListFtpContentAsync(serverUrl & relativePath, credential)
            Dim source As New DataModel.FTPFileDataSource(items)
            Me.DefaultViewModel("Groups") = source.AllGroups
        Catch ex As Exception
            Me.DefaultViewModel("Groups") = Nothing
            NotifyUser(ex.Message)
        End Try

        Me.getDataProgress.Visibility = Windows.UI.Xaml.Visibility.Collapsed
        Me.backButton.IsEnabled = Me.pathStack.Count > 0
        Me.ftpPath.Text = relativePath
    End Sub

    ''' <summary>
    ''' Generate the current relative path.
    ''' </summary>
    ''' <param name="pathStack"></param>
    ''' <returns></returns>
    Private Function GenerateRelativePath(ByVal pathStack As IEnumerable(Of String)) As String
        Dim relativePath As New StringBuilder("/")
        For Each Path As String In pathStack.Reverse()
            Dim encodedUrl As String = System.Net.WebUtility.UrlEncode(Path)
            encodedUrl = encodedUrl.Replace("+", "%20")
            encodedUrl = encodedUrl.Replace("%2b", "+")
            relativePath.AppendFormat("{0}/", encodedUrl)
        Next Path

        Return relativePath.ToString()
    End Function

#End Region

#Region "Common methods"

    Private Async Sub Footer_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Await Windows.System.Launcher.LaunchUriAsync(New Uri((TryCast(sender, HyperlinkButton)).Tag.ToString()))
    End Sub

    Public Sub NotifyUser(ByVal message As String)
        textStatus.Text = message
    End Sub

#End Region

End Class

