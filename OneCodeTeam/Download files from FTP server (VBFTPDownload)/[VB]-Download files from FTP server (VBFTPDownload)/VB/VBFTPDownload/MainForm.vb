'*************************** Module Header ******************************'
' Module Name:  MainForm.vb
' Project:	    VBFTPDownload
' Copyright (c) Microsoft Corporation.
' 
' This is the main form of this application. It is used to initialize the UI and 
' handle the events.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.Linq
Imports System.Net

Partial Public Class MainForm
    Inherits Form

    Private _client As FTPClientManager = Nothing

    Private _currentCredentials As NetworkCredential = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

#Region "URL navigation"

    ''' <summary>
    ''' Handle the Click event of btnConnect.
    ''' </summary>
    Private Sub btnConnect_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnConnect.Click

        ' Connect to server specified by tbFTPServer.Text.
        Connect(Me.tbFTPServer.Text.Trim())

    End Sub

    Private Sub Connect(ByVal urlStr As String)
        Try
            Dim url As New Uri(urlStr)

            ' The schema of url must be ftp. 
            If Not url.Scheme.Equals("ftp", StringComparison.OrdinalIgnoreCase) Then
                Throw New ApplicationException("The schema of url must be ftp. ")
            End If

            ' Set the url to the folder that contains this file. 
            If url.IsFile Then
                url = New Uri(url, "..")
            End If

            ' Show the Form UICredentialsProvider to get new Credentials.
            Using provider As New UICredentialsProvider(Me._currentCredentials)

                ' Show the Form UICredentialsProvider as a dialog.
                Dim result = provider.ShowDialog()

                ' If user typed the Credentials and pressed the "OK" button.
                If result = System.Windows.Forms.DialogResult.OK Then

                    ' Reset the current Credentials.
                    Me._currentCredentials = provider.Credentials

                Else
                    Return
                End If
            End Using

            ' Initialize the FTPClient instance.
            _client = New FTPClientManager(url, _currentCredentials)

            AddHandler _client.UrlChanged, AddressOf client_UrlChanged
            AddHandler _client.StatusChanged, AddressOf client_StatusChanged
            AddHandler _client.ErrorOccurred, AddressOf client_ErrorOccurred
            AddHandler _client.FileDownloadCompleted, AddressOf client_FileDownloadCompleted
            AddHandler _client.NewMessageArrived, AddressOf client_NewMessageArrived

            ' List the sub directories and files.
            RefreshSubDirectoriesAndFiles()


        Catch webEx As System.Net.WebException
            If (TryCast(webEx.Response, FtpWebResponse)).StatusCode = FtpStatusCode.NotLoggedIn Then
                ' Reconnect the server.
                Connect(urlStr)

                Return
            Else
                MessageBox.Show(webEx.Message)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Log the message of FTPClient.
    ''' </summary>
    Private Sub client_NewMessageArrived(ByVal sender As Object,
                                         ByVal e As NewMessageEventArg)
        Me.Invoke(New EventHandler(Of NewMessageEventArg)(
                AddressOf client_NewMessageArrivedHandler), sender, e)
    End Sub

    Private Sub client_NewMessageArrivedHandler(ByVal sender As Object,
                                         ByVal e As NewMessageEventArg)
        Dim log As String = String.Format("{0} {1}", Date.Now, e.NewMessage)
        Me.lstLog.Items.Add(log)
        Me.lstLog.SelectedIndex = Me.lstLog.Items.Count - 1
    End Sub

    ''' <summary>
    ''' Log the FileDownloadCompleted event when a file was downloaded.
    ''' </summary>
    Private Sub client_FileDownloadCompleted(ByVal sender As Object,
                                             ByVal e As FileDownloadCompletedEventArgs)
        Me.Invoke(New EventHandler(Of FileDownloadCompletedEventArgs)(
                AddressOf client_FileDownloadCompletedHandler), sender, e)
    End Sub

    Private Sub client_FileDownloadCompletedHandler(ByVal sender As Object,
                                            ByVal e As FileDownloadCompletedEventArgs)
        Dim log As String =
            String.Format("{0} Download from {1} to {2} is completed. Length: {3}. ",
                          Date.Now, e.ServerPath,
                          e.LocalFile.FullName,
                          e.LocalFile.Length)

        Me.lstLog.Items.Add(log)
        Me.lstLog.SelectedIndex = Me.lstLog.Items.Count - 1
    End Sub

    ''' <summary>
    ''' Log the ErrorOccurred event if there was an error.
    ''' </summary>
    Private Sub client_ErrorOccurred(ByVal sender As Object, ByVal e As ErrorEventArgs)
        Me.Invoke(New EventHandler(Of ErrorEventArgs)(
               AddressOf client_ErrorOccurredHandler), sender, e)
    End Sub

    Private Sub client_ErrorOccurredHandler(ByVal sender As Object, ByVal e As ErrorEventArgs)
        Me.lstLog.Items.Add(String.Format("{0} {1} ", Date.Now, e.ErrorException.Message))

        Dim innerException = e.ErrorException.InnerException

        ' Log all the innerException.
        Do While innerException IsNot Nothing
            Me.lstLog.Items.Add(String.Format(vbTab & vbTab & vbTab & " {0} ",
                                              innerException.Message))
            innerException = innerException.InnerException
        Loop

        Me.lstLog.SelectedIndex = Me.lstLog.Items.Count - 1
    End Sub

    ''' <summary>
    ''' Refresh the UI if the Status of the FTPClient changed.
    ''' </summary>
    Private Sub client_StatusChanged(ByVal sender As Object, ByVal e As EventArgs)
        Me.Invoke(New EventHandler(AddressOf client_StatusChangedHandler), sender, e)
    End Sub

    Private Sub client_StatusChangedHandler(ByVal sender As Object, ByVal e As EventArgs)

        ' Disable all the buttons if the client is downloading file.
        If _client.Status = FTPClientManagerStatus.Downloading Then
            btnBrowseDownloadPath.Enabled = False
            btnConnect.Enabled = False
            btnDownload.Enabled = False
            btnNavigateParentFolder.Enabled = False
            lstFileExplorer.Enabled = False
        Else
            btnBrowseDownloadPath.Enabled = True
            btnConnect.Enabled = True
            btnDownload.Enabled = True
            btnNavigateParentFolder.Enabled = True
            lstFileExplorer.Enabled = True
        End If

        Dim log As String = String.Format("{0} FTPClient status changed to {1}. ",
                                          Date.Now, _client.Status.ToString())

        Me.lstLog.Items.Add(log)
        Me.lstLog.SelectedIndex = Me.lstLog.Items.Count - 1
    End Sub

    ''' <summary>
    ''' Handle the UrlChanged event of the FTPClient.
    ''' </summary>
    Private Sub client_UrlChanged(ByVal sender As Object, ByVal e As EventArgs)
        Me.Invoke(New EventHandler(AddressOf client_UrlChangedHandler), sender, e)
    End Sub

    Private Sub client_UrlChangedHandler(ByVal sender As Object, ByVal e As EventArgs)
        RefreshSubDirectoriesAndFiles()

        Dim log As String = String.Format("{0} The current url changed to {1}. ",
                                          Date.Now, _client.Url)

        Me.lstLog.Items.Add(log)
        Me.lstLog.SelectedIndex = Me.lstLog.Items.Count - 1
    End Sub

    ''' <summary>
    ''' Handle the DoubleClick event of lstFileExplorer.
    ''' </summary>
    Private Sub lstFileExplorer_DoubleClick(ByVal sender As Object, ByVal e As EventArgs) _
        Handles lstFileExplorer.DoubleClick
        ' if only one item is selected and the item represents a folder, then navigate
        ' to a subDirectory.
        If lstFileExplorer.SelectedItems.Count = 1 _
            AndAlso (TryCast(lstFileExplorer.SelectedItem, FTPFileSystem)).IsDirectory Then
            Me._client.Naviagte((TryCast(lstFileExplorer.SelectedItem, FTPFileSystem)).Url)
        End If
    End Sub

    ''' <summary>
    ''' Handle the Click event of btnNavigateParentFolder.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnNavigateParentFolder_Click(ByVal sender As Object,
                                              ByVal e As EventArgs) _
                                          Handles btnNavigateParentFolder.Click

        ' Navigate to the parent folder.
        Me._client.NavigateParent()
    End Sub

    ''' <summary>
    ''' List the sub directories and files.
    ''' </summary>
    Private Sub RefreshSubDirectoriesAndFiles()
        lbCurrentUrl.Text = String.Format("Current Path: {0}", _client.Url)

        Dim subDirs = _client.GetSubDirectoriesAndFiles()

        ' Sort the list.
        Dim orderedDirs = From dir In subDirs
                          Order By dir.IsDirectory Descending, dir.Name
                          Select dir

        lstFileExplorer.Items.Clear()
        For Each subdir In orderedDirs
            lstFileExplorer.Items.Add(subdir)
        Next subdir
    End Sub


#End Region

#Region "Download File/Folders"

    ''' <summary>
    ''' Handle the Click event of btnBrowseDownloadPath.
    ''' </summary>
    Private Sub btnBrowseDownloadPath_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnBrowseDownloadPath.Click
        BrowserDownloadPath()
    End Sub

    ''' <summary>
    ''' Handle the Click event of btnDownload.
    ''' </summary>
    Private Sub btnDownload_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnDownload.Click

        ' One or more files / folders should be selected in the File Explorer.
        If lstFileExplorer.SelectedItems.Count = 0 Then
            MessageBox.Show(
                "Please select one or more files / folders in the File Explorer",
                "No file is selected")
            Return
        End If

        ' If the tbDownloadPath.Text is empty, then show a FolderBrowserDialog.
        If String.IsNullOrWhiteSpace(tbDownloadPath.Text) _
            AndAlso BrowserDownloadPath() <> DialogResult.OK Then
            Return
        End If


        Dim directoriesAndFiles =
            lstFileExplorer.SelectedItems.Cast(Of FTPFileSystem)().ToList()

        ' Download the selected items.
        _client.DownloadDirectoriesAndFiles(directoriesAndFiles, tbDownloadPath.Text)

    End Sub

    ''' <summary>
    ''' Show a FolderBrowserDialog.
    ''' </summary>
    Private Function BrowserDownloadPath() As DialogResult
        Using folderBrowser As New FolderBrowserDialog()
            If Not String.IsNullOrWhiteSpace(tbDownloadPath.Text) Then
                folderBrowser.SelectedPath = tbDownloadPath.Text
            End If
            Dim result = folderBrowser.ShowDialog()
            If result = DialogResult.OK Then
                tbDownloadPath.Text = folderBrowser.SelectedPath
            End If
            Return result
        End Using
    End Function
#End Region
End Class