' Copyright (c) Microsoft Corporation. All rights reserved.
''' <summary>
''' A simple control for selecting a directory
''' </summary>
''' <remarks></remarks>
Public Class DirectoryChooser

    ''' <summary>
    ''' What directory was chosen?
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public ReadOnly Property Directory() As String
        Get
            Return Me.FileTextBox.Text
        End Get
    End Property


    ''' <summary>
    ''' Populate the TextBox with the directory selected by the user/
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FileBrowseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileBrowseButton.Click
        Dim folderDialog As New FolderBrowserDialog
        folderDialog.RootFolder = Environment.SpecialFolder.MyComputer
        If (folderDialog.ShowDialog() = DialogResult.OK) Then
            Me.FileTextBox.Text = folderDialog.SelectedPath
        End If
    End Sub

    ''' <summary>
    ''' Reset the chosen directory
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Reset()
        Me.FileTextBox.Text = String.Empty
    End Sub
End Class
