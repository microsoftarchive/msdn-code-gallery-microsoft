' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class FindInFilesPanel
    Inherits FileSystemSample.TaskPanelBase

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents FilesListBox As System.Windows.Forms.ListBox

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker
        Me.FilesListBox = New System.Windows.Forms.ListBox
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'DescriptionTextBox
        '
        Me.DescriptionTextBox.AutoSize = False
        Me.DescriptionTextBox.Multiline = True
        Me.DescriptionTextBox.Size = New System.Drawing.Size(568, 31)
        Me.DescriptionTextBox.Text = "This sample searches a collection of files for the text specified.  Widcards may " & _
            "be used to select a subset of all files in a directory."
        '
        'ExececuteMethodButton
        '
        '
        'ResetValuesButton
        '
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.FilesListBox)
        Me.GroupBox2.Controls.SetChildIndex(Me.EndParenLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.ExececuteMethodButton, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.ResetValuesButton, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.FilesListBox, 0)
        '
        'BackgroundWorker1
        '
        Me.BackgroundWorker1.WorkerReportsProgress = False
        Me.BackgroundWorker1.WorkerSupportsCancellation = False
        '
        'FilesListBox
        '
        Me.FilesListBox.FormattingEnabled = True
        Me.FilesListBox.Location = New System.Drawing.Point(14, 261)
        Me.FilesListBox.Name = "FilesListBox"
        Me.FilesListBox.Size = New System.Drawing.Size(559, 108)
        Me.FilesListBox.TabIndex = 5
        '
        'FindInFilesPanel
        '
        Me.Name = "FindInFilesPanel"
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region


    Private Shared panelInstance As FindInFilesPanel
    Friend WithEvents dirChooser As New DirectoryChooser()
    Friend WithEvents searchTextBox As New TextBox()
    Friend WithEvents recurseComboBox As New ComboBox()
    Friend WithEvents wildCardsComboBox As New ComboBox()

    Private searchResults As System.Collections.ObjectModel.ReadOnlyCollection(Of String)
    Private searchText As String
    Private directory As String
    Private recurse As Boolean
    Private wildCards As String()

    ''' <summary>
    ''' Get a global instance of the panel.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As FindInFilesPanel
        If (panelInstance Is Nothing) Then
            panelInstance = New FindInFilesPanel
        End If
        Return panelInstance
    End Function

    ''' <summary>
    ''' Load the panel, adding the necessary controls.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FindInFiles_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeUserControls()
        Me.dirChooser.Reset()
        MyBase.AddParameter("directory", dirChooser)
        MyBase.AddParameter("searchText", searchTextBox)
        MyBase.AddParameter("recurse", recurseComboBox)
        MyBase.AddParameter("wildCards", wildCardsComboBox)
    End Sub


    ''' <summary>
    ''' Reset the panel to its default state.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeUserControls()
        MyBase.MethodNameLabel.Text = "My.Computer.FileSystem.FindInFiles("
        Me.dirChooser.Reset()

        recurseComboBox.Items.AddRange(New String() {"True", "False"})
        recurseComboBox.AutoSize = True
        recurseComboBox.SelectedItem = "False"

        wildCardsComboBox.Items.AddRange(New String() {"*.*", "*.txt", "*.doc", "*.vb", "*.bmp", ".jpg"})
        wildCardsComboBox.AutoSize = True
        wildCardsComboBox.SelectedItem = "*.*"
    End Sub


    ''' <summary>
    ''' Now search all specified files for the text.  This is not a method that is supported directly by My.Computer.FileSystem,
    ''' so we need to write the logic here.  Due to the latency of the operation, a BackgroundWorker is used to perform the 
    ''' search asynchronously.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ExececuteMethodButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExececuteMethodButton.Click
        If Not Me.BackgroundWorker1.IsBusy Then
            Me.FilesListBox.Items.Clear()

            directory = Me.dirChooser.Directory
            searchText = Me.searchTextBox.Text

            recurse = Boolean.Parse(CType(Me.recurseComboBox.SelectedItem, String))
            wildCards = New String() {CType(Me.wildCardsComboBox.SelectedItem, String)}

            Me.BackgroundWorker1.WorkerReportsProgress = True
            Me.BackgroundWorker1.RunWorkerAsync()
        End If
    End Sub


    ''' <summary>
    ''' Reset the panel and its controls to their default state.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ResetValuesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetValuesButton.Click
        InitializeUserControls()
    End Sub

    ''' <summary>
    ''' Search each file specified for the text provided.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BackgroundWorker1_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim files As System.Collections.ObjectModel.ReadOnlyCollection(Of String)
            ' Look in each file matching the provided wildcards
            If (recurse) Then
            files = My.Computer.FileSystem.FindInFiles(directory, searchText, True, FileIO.SearchOption.SearchAllSubDirectories, wildCards)
        Else
            files = My.Computer.FileSystem.FindInFiles(directory, searchText, True, FileIO.SearchOption.SearchTopLevelOnly, wildCards)
            End If
        For Each foundFile As String In files
            Me.BackgroundWorker1.ReportProgress(0, foundFile)
        Next
    End Sub

    ''' <summary>
    ''' Add the found file to the ListBox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BackgroundWorker1_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        Me.FilesListBox.Items.Add(CType(e.UserState, String))
    End Sub

    ''' <summary>
    ''' The search is complete.  If no results were found, display a message in the list box.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        If Me.FilesListBox.Items.Count < 1 Then
            Me.FilesListBox.Items.Add("No files with the specified text found.")
        End If
    End Sub

End Class
