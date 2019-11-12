'*************************** Module Header ******************************'
' Module Name:  MainForm.vb
' Project:	    VBFTPUpload
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
Imports System.Text
Imports System.IO

Partial Public Class MainForm
    Inherits Form

    Private _client As FTPClientManager = Nothing

    Private _currentCredentials As NetworkCredential = Nothing

    Public Sub New()
        InitializeComponent()

        RefreshUI()
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
            AddHandler _client.FileUploadCompleted, AddressOf client_FileUploadCompleted
            AddHandler _client.NewMessageArrived, AddressOf client_NewMessageArrived

            ' Refresh the UI and list the sub directories and files.
            RefreshUI()


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
    ''' Log the FileUploadCompleted event when a file was uploaded.
    ''' </summary>
    Private Sub client_FileUploadCompleted(ByVal sender As Object,
                                             ByVal e As FileUploadCompletedEventArgs)
        Me.Invoke(New EventHandler(Of FileUploadCompletedEventArgs)(
                   AddressOf client_FileUploadCompletedHandler), sender, e)
    End Sub

    Private Sub client_FileUploadCompletedHandler(ByVal sender As Object,
                                            ByVal e As FileUploadCompletedEventArgs)
        Dim log As String = String.Format(
            "{0} Upload from {1} to {2} is completed. Length: {3}. ",
            Date.Now, e.LocalFile.FullName, e.ServerPath, e.LocalFile.Length)

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
        RefreshUI()

        Dim log As String = String.Format("{0} FTPClient status changed to {1}. ",
                                          Date.Now, _client.Status.ToString())

        Me.lstLog.Items.Add(log)
        Me.lstLog.SelectedIndex = Me.lstLog.Items.Count - 1
    End Sub

    Private Sub RefreshUI()
        ' Disable all the buttons if the client is uploading file.
        If _client Is Nothing OrElse _client.Status <> FTPClientManagerStatus.Idle Then

            btnBrowseLocalFolder.Enabled = False
            btnUploadFolder.Enabled = False

            btnBrowseLocalFile.Enabled = False
            btnUploadFile.Enabled = False

            btnDelete.Enabled = False

            btnNavigateParentFolder.Enabled = False
            lstFileExplorer.Enabled = False
        Else

            btnBrowseLocalFolder.Enabled = True
            btnUploadFolder.Enabled = True

            btnBrowseLocalFile.Enabled = True
            btnUploadFile.Enabled = True

            btnDelete.Enabled = True

            btnNavigateParentFolder.Enabled = True
            lstFileExplorer.Enabled = True
        End If

        btnConnect.Enabled = _client Is Nothing _
            OrElse _client.Status = FTPClientManagerStatus.Idle

        RefreshSubDirectoriesAndFiles()

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
    Private Sub btnNavigateParentFolder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnNavigateParentFolder.Click

        ' Navigate to the parent folder.
        Me._client.NavigateParent()
    End Sub

    ''' <summary>
    ''' List the sub directories and files.
    ''' </summary>
    Private Sub RefreshSubDirectoriesAndFiles()
        If _client Is Nothing Then
            Return
        End If

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

#Region "Upload a Folder"

    ''' <summary>
    ''' Handle the Click event of btnBrowseLocalFolder.
    ''' </summary>
    Private Sub btnBrowseLocalFolder_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnBrowseLocalFolder.Click
        BrowserLocalFolder()
    End Sub

    ''' <summary>
    ''' Handle the Click event of btnUploadFolder.
    ''' </summary>
    Private Sub btnUploadFolder_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnUploadFolder.Click

        ' If the tbLocalFolder.Text is empty, then show a FolderBrowserDialog.
        If String.IsNullOrWhiteSpace(tbLocalFolder.Text) _
            AndAlso BrowserLocalFolder() <> DialogResult.OK Then
            Return
        End If

        Try
            Dim dir As New DirectoryInfo(tbLocalFolder.Text)

            If Not dir.Exists Then
                Throw New ApplicationException(
                    String.Format(" The folder {0} does not exist!", dir.FullName))
            End If

            ' Upload the selected items.
            _client.UploadFolder(dir, _client.Url, chkCreateFolder.Checked)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Show a FolderBrowserDialog.
    ''' </summary>
    Private Function BrowserLocalFolder() As DialogResult
        Using folderBrowser As New FolderBrowserDialog()
            If Not String.IsNullOrWhiteSpace(tbLocalFolder.Text) Then
                folderBrowser.SelectedPath = tbLocalFolder.Text
            End If
            Dim result = folderBrowser.ShowDialog()
            If result = DialogResult.OK Then
                tbLocalFolder.Text = folderBrowser.SelectedPath
            End If
            Return result
        End Using
    End Function

#End Region

#Region "Upload files"

    Private Sub btnBrowseLocalFile_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnBrowseLocalFile.Click
        BrowserLocalFiles()
    End Sub

    Private Sub btnUploadFile_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnUploadFile.Click
        If tbLocalFile.Tag Is Nothing AndAlso BrowserLocalFiles() <> DialogResult.OK Then
            Return
        End If

        Try
            Dim files As New List(Of FileInfo)()
            Dim selectedFiles() As String = TryCast(tbLocalFile.Tag, String())

            For Each selectedFile In selectedFiles
                Dim fileInfo_Renamed As New FileInfo(selectedFile)
                If Not fileInfo_Renamed.Exists Then
                    Throw New ApplicationException(
                        String.Format(" The file {0} does not exist!", selectedFile))
                Else
                    files.Add(fileInfo_Renamed)
                End If
            Next selectedFile

            If files.Count > 0 Then
                _client.UploadFoldersAndFiles(files, _client.Url)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Show a FolderBrowserDialog.
    ''' </summary>
    Private Function BrowserLocalFiles() As DialogResult
        Using fileBrowser As New OpenFileDialog()
            fileBrowser.Multiselect = True
            Dim result = fileBrowser.ShowDialog()
            If result = DialogResult.OK Then
                tbLocalFile.Tag = fileBrowser.FileNames

                Dim filesText As New StringBuilder()
                For Each file In fileBrowser.FileNames
                    filesText.Append(file & ";")
                Next file
                tbLocalFile.Text = filesText.ToString()
            End If
            Return result
        End Using
    End Function


#End Region

#Region "Delete files"

    Private Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs) _
        Handles btnDelete.Click
        If lstFileExplorer.SelectedItems.Count = 0 Then
            MessageBox.Show("Please select the items to delete in the FTP File Explorer")
        End If

        Dim itemsToDelete =
            lstFileExplorer.SelectedItems.Cast(Of FTPFileSystem)().ToList()

        Me._client.DeleteItemsOnFTPServer(itemsToDelete)

        RefreshUI()
    End Sub

#End Region

End Class
