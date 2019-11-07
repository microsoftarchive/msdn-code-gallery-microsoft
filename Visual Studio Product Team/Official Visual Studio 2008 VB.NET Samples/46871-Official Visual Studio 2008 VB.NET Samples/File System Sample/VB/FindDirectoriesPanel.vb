' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class FindDirectoriesPanel
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
    Friend WithEvents DirectoriesListBox As System.Windows.Forms.ListBox
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.DirectoriesListBox = New System.Windows.Forms.ListBox
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'DescriptionTextBox
        '
        Me.DescriptionTextBox.Text = "Search a directory for subdirectories matching the wildcards provided.  The defau" & _
            "lt wildcard is ""*"""
        '
        'ExececuteMethodButton
        '
        '
        'ResetValuesButton
        '
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.DirectoriesListBox)
        Me.GroupBox2.Controls.SetChildIndex(Me.EndParenLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.ExececuteMethodButton, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.ResetValuesButton, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.DirectoriesListBox, 0)
        '
        'DirectoriesListBox
        '
        Me.DirectoriesListBox.FormattingEnabled = True
        Me.DirectoriesListBox.Location = New System.Drawing.Point(14, 202)
        Me.DirectoriesListBox.Name = "DirectoriesListBox"
        Me.DirectoriesListBox.Size = New System.Drawing.Size(558, 173)
        Me.DirectoriesListBox.TabIndex = 5
        '
        'BackgroundWorker1
        '
        Me.BackgroundWorker1.WorkerReportsProgress = False
        Me.BackgroundWorker1.WorkerSupportsCancellation = False
        '
        'FindDirectoriesPanel
        '
        Me.Name = "FindDirectoriesPanel"
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Shared panelInstance As FindDirectoriesPanel
    Friend WithEvents dirChooser As New DirectoryChooser
    Friend WithEvents recurseComboBox As New ComboBox()
    Friend WithEvents wildCardsTextBox As New TextBox()

    Private searchResults As System.Collections.ObjectModel.ReadOnlyCollection(Of String)
    Private directory As String
    Private recurse As Boolean
    Private wildCards As String()

    ''' <summary>
    ''' Return a global instance of the panel
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As FindDirectoriesPanel
        If (panelInstance Is Nothing) Then
            panelInstance = New FindDirectoriesPanel
        End If
        Return panelInstance
    End Function

    ''' <summary>
    ''' Load the panel, creating a control for each parameter to My.Computer.FileSystem.GetDirectories
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub FindDirectories_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeUserControls()
        MyBase.AddParameter("directory", dirChooser)
        MyBase.AddParameter("recurse", recurseComboBox)
        MyBase.AddParameter("wildCards", wildCardsTextBox)
    End Sub


    ''' <summary>
    ''' Set up the panel with the necessary controls.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeUserControls()
        MyBase.MethodNameLabel.Text = "My.Computer.FileSystem.GetDirectories("
        dirChooser.Reset()
        recurseComboBox.Items.AddRange(New String() {"True", "False"})
        recurseComboBox.AutoSize = True
        recurseComboBox.SelectedItem = "False"

        wildCardsTextBox.AutoSize = True
        wildCardsTextBox.Text = "*"
    End Sub

    ''' <summary>
    ''' Get all directories that match the wildcards provided.  Because this may take a long time to execute, we execute this in the background using the 
    ''' background worker.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ExececuteMethodButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExececuteMethodButton.Click
        Me.DirectoriesListBox.Items.Clear()
        Me.DirectoriesListBox.Items.Add("Searching " & Me.dirChooser.Directory & "...")

        directory = Me.dirChooser.Directory
        recurse = Boolean.Parse(CType(Me.recurseComboBox.SelectedItem, String))
        wildCards = New String() {Me.wildCardsTextBox.Text}

        Me.BackgroundWorker1.RunWorkerAsync()
    End Sub

    ''' <summary>
    ''' Reset the controls back to their default values.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ResetValuesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetValuesButton.Click
        InitializeUserControls()
    End Sub

    ''' <summary>
    ''' Search all sub directories for directory names that match the provided wildcards.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BackgroundWorker1_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Try
            If (recurse) Then
                searchResults = My.Computer.FileSystem.GetDirectories(directory, FileIO.SearchOption.SearchAllSubDirectories, wildCards)
            Else
                searchResults = My.Computer.FileSystem.GetDirectories(directory, FileIO.SearchOption.SearchTopLevelOnly, wildCards)
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' All done, so update the UI with the results.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        Me.DirectoriesListBox.Items.Clear()
        If (searchResults Is Nothing Or searchResults.Count < 1) Then
            Me.DirectoriesListBox.Items.Add("<No directories found.>")
        Else
            For Each searchResult As String In searchResults
                Me.DirectoriesListBox.Items.Add(searchResult)
            Next
        End If
    End Sub

End Class
