' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class FileChooser

    Private Shared defaultPath As String

    ''' <summary>
    ''' File name chosen
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public ReadOnly Property Filename() As String
        Get
            Return Me.FileTextBox.Text
        End Get
    End Property

    ''' <summary>
    ''' The initial directory to show in the FileDialog
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public Property InitialDirectory() As String
        Get
            If defaultPath Is Nothing Then
                defaultPath = My.Computer.FileSystem.CurrentDirectory
            End If
            Return defaultPath
        End Get
        Set(ByVal value As String)
            defaultPath = value
        End Set
    End Property

    ''' <summary>
    ''' Reset the control back to its initial value
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Reset()
        Me.FileTextBox.Text = String.Empty
    End Sub

    ''' <summary>
    ''' Populate the TextBox with the value selected 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FileBrowseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileBrowseButton.Click
        Dim fileDialog As New OpenFileDialog()
        fileDialog.InitialDirectory = Me.InitialDirectory
        If (fileDialog.ShowDialog() = DialogResult.OK) Then
            Me.FileTextBox.Text = fileDialog.FileName
            Me.InitialDirectory = System.IO.Path.GetDirectoryName(Filename)
        End If
    End Sub

End Class
