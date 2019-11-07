' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.IO

Public Class WriteLargeFilePanel
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
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents FileContentsListBox As System.Windows.Forms.ListBox


    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label
        Me.FileContentsListBox = New System.Windows.Forms.ListBox
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'DescriptionTextBox
        '
        Me.DescriptionTextBox.AutoSize = False
        Me.DescriptionTextBox.Multiline = True
        Me.DescriptionTextBox.Size = New System.Drawing.Size(568, 44)
        Me.DescriptionTextBox.Text = "When writing large files or writing small segments of files repeatedly, it is oft" & _
            "en more efficient to write the file iteratively.  This can be achieved using a S" & _
            "treamReader, available from My.Computer.OpenTextFileReader."
        '
        'ExececuteMethodButton
        '
        '
        'ResetValuesButton
        '
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Controls.Add(Me.FileContentsListBox)
        Me.GroupBox2.Controls.SetChildIndex(Me.FileContentsListBox, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.Label1, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.EndParenLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.ExececuteMethodButton, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.ResetValuesButton, 0)
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(26, 188)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(99, 14)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "Text to write to file:"
        '
        'FileContentsListBox
        '
        Me.FileContentsListBox.FormattingEnabled = True
        Me.FileContentsListBox.Location = New System.Drawing.Point(26, 209)
        Me.FileContentsListBox.Name = "FileContentsListBox"
        Me.FileContentsListBox.Size = New System.Drawing.Size(538, 160)
        Me.FileContentsListBox.TabIndex = 4
        '
        'WriteLargeFilePanel
        '
        Me.Name = "WriteLargeFilePanel"
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Shared panelInstance As WriteLargeFilePanel
    Friend WithEvents writeFileFileChooser As New FileChooser()
    Friend WithEvents appendComboBox As New ComboBox()
    Friend WithEvents encodingComboBox As New ComboBox()

    ''' <summary>
    ''' Get a global instance of this panel
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As WriteLargeFilePanel
        If (panelInstance Is Nothing) Then
            panelInstance = New WriteLargeFilePanel
        End If
        Return panelInstance
    End Function

    ''' <summary>
    ''' Create the panel and add a control for each parameter to OpenTextFileWriter()
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub WriteFile_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeUserControls()
        MyBase.AddParameter("filename", writeFileFileChooser)
        MyBase.AddParameter("append", appendComboBox)
        MyBase.AddParameter("encoding", encodingComboBox)
    End Sub

    ''' <summary>
    ''' Set the default value for each control
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeUserControls()
        MyBase.MethodNameLabel.Text = "My.Computer.FileSystem.OpenTextFileWriter("

        writeFileFileChooser.Reset()

        appendComboBox.Items.AddRange(New String() {"True", "False"})
        appendComboBox.AutoSize = True
        appendComboBox.SelectedItem = "True"

        encodingComboBox.Items.AddRange(New String() {"ASCII", "BigEndianUnicode", "Unicode", "UTF7", "UTF8", "UTF32"})
        encodingComboBox.AutoSize = True
        encodingComboBox.SelectedItem = "ASCII"
    End Sub

    ''' <summary>
    ''' Open a text writer and write each line of the list box out to the file
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ExececuteMethodButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExececuteMethodButton.Click
        Try

            Using writer As StreamWriter = My.Computer.FileSystem.OpenTextFileWriter( _
                                                        file:=writeFileFileChooser.Filename, _
                                                        append:=Boolean.Parse(CType(appendComboBox.SelectedItem, String)), _
                                                        encoding:=ParseEncoding(CType(Me.encodingComboBox.SelectedItem, String)))
                For Each line As String In Me.FileContentsListBox.Items
                    writer.WriteLine(line)
                Next
            End Using
            Me.FileContentsListBox.Items.Clear()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Reset the control values back to their original state
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ResetValuesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetValuesButton.Click
        InitializeUserControls()
    End Sub
End Class
