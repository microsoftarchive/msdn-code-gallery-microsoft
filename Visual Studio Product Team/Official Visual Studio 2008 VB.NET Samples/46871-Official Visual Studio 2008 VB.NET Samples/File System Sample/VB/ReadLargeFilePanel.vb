' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.IO

Public Class ReadLargeFilePanel
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
    Friend WithEvents FileContentsListBox As System.Windows.Forms.ListBox


    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.FileContentsListBox = New System.Windows.Forms.ListBox
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'DescriptionTextBox
        '
        Me.DescriptionTextBox.AutoSize = False
        Me.DescriptionTextBox.Location = New System.Drawing.Point(7, 20)
        Me.DescriptionTextBox.Multiline = True
        Me.DescriptionTextBox.Size = New System.Drawing.Size(568, 47)
        Me.DescriptionTextBox.Text = "When reading very large files, it is often necessary to read the file iteratively" & _
            " rather than all at once.  My.Computer.FileSystem.OpenTextFileReader provides ac" & _
            "cess to a Stream object that enables more granular file access."
        '
        'ExececuteMethodButton
        '
        '
        'ResetValuesButton
        '
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.FileContentsListBox)
        Me.GroupBox2.Controls.SetChildIndex(Me.FileContentsListBox, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.EndParenLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.ExececuteMethodButton, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.ResetValuesButton, 0)
        '
        'FileContentsListBox
        '
        Me.FileContentsListBox.FormattingEnabled = True
        Me.FileContentsListBox.Location = New System.Drawing.Point(25, 187)
        Me.FileContentsListBox.Name = "FileContentsListBox"
        Me.FileContentsListBox.SelectionMode = System.Windows.Forms.SelectionMode.None
        Me.FileContentsListBox.Size = New System.Drawing.Size(538, 186)
        Me.FileContentsListBox.TabIndex = 4
        '
        'ReadLargeFilePanel
        '
        Me.Name = "ReadLargeFilePanel"
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Shared panelInstance As ReadLargeFilePanel
    Friend WithEvents readFileFileChooser As New FileChooser()
    Friend WithEvents encodingComboBox As New ComboBox()

    ''' <summary>
    ''' Get a global instance of this panel
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As ReadLargeFilePanel
        If (panelInstance Is Nothing) Then
            panelInstance = New ReadLargeFilePanel
        End If
        Return panelInstance
    End Function

    ''' <summary>
    ''' Add the necessary controls to the panel
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ReadFile_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeUserControls()
        MyBase.AddParameter("filename", readFileFileChooser)
        MyBase.AddParameter("encoding", encodingComboBox)
    End Sub

    ''' <summary>
    ''' Set the controls to their default values
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeUserControls()
        MyBase.MethodNameLabel.Text = "My.Computer.FileSystem.OpenTextFileReader("
        readFileFileChooser.Reset()
        encodingComboBox.Items.AddRange(New String() {"ASCII", "BigEndianUnicode", "Unicode", "UTF7", "UTF8", "UTF32"})
        encodingComboBox.AutoSize = True
        encodingComboBox.SelectedItem = "ASCII"
    End Sub

    ''' <summary>
    ''' Read in the file, one line at a time until the end of the file is reached
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ExececuteMethodButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExececuteMethodButton.Click
        Try
            ' Open the stream reader and append each read line to the list box.  Stop when we reach the end of the stream.
            Using reader As StreamReader = My.Computer.FileSystem.OpenTextFileReader( _
                                                            file:=readFileFileChooser.Filename, _
                                                            encoding:=ParseEncoding(CType(Me.encodingComboBox.SelectedItem, String)))
                FileContentsListBox.Items.Clear()
                While (Not reader.EndOfStream)
                    Me.FileContentsListBox.Items.Add(reader.ReadLine())
                End While
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Reset the controls to their default values
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ResetValuesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetValuesButton.Click
        InitializeUserControls()
    End Sub

End Class
