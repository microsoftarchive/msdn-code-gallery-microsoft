' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.IO
Imports Microsoft.VisualBasic.FileIO

Public Class ParseTextFilePanel
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
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker


    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'DescriptionTextBox
        '
        Me.DescriptionTextBox.AutoSize = False
        Me.DescriptionTextBox.Multiline = True
        Me.DescriptionTextBox.Size = New System.Drawing.Size(568, 51)
        Me.DescriptionTextBox.Text = "The TextFieldParser parses delimited and fixed-width files.  This example automat" & _
            "ically detects the delimiter in the file and populates a DataGridView control wi" & _
            "th the fields from the file.  "
        '
        'ExececuteMethodButton
        '
        '
        'ResetValuesButton
        '
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.DataGridView1)
        Me.GroupBox2.Controls.SetChildIndex(Me.DataGridView1, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.EndParenLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.ExececuteMethodButton, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.ResetValuesButton, 0)
        '
        'DataGridView1
        '
        Me.DataGridView1.Location = New System.Drawing.Point(22, 206)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(547, 166)
        Me.DataGridView1.TabIndex = 4
        '
        'BackgroundWorker1
        '
        Me.BackgroundWorker1.WorkerReportsProgress = False
        Me.BackgroundWorker1.WorkerSupportsCancellation = False
        '
        'ParseTextFilePanel
        '
        Me.Name = "ParseTextFilePanel"
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Shared panelInstance As ParseTextFilePanel
    Friend WithEvents readFileFileChooser As New FileChooser()
    Friend WithEvents encodingComboBox As New ComboBox()
    Private delimiters As String()
    Private filename As String

    ''' <summary>
    ''' Get a global instance of this panel
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As ParseTextFilePanel
        If (panelInstance Is Nothing) Then
            panelInstance = New ParseTextFilePanel
        End If
        Return panelInstance
    End Function

    ''' <summary>
    ''' Load the panel and add a control for each parameter to My.Computer.FileSystem.OpenTextFieldParser()
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
    ''' Add default values for the controls on this panel
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeUserControls()
        MyBase.MethodNameLabel.Text = "My.Computer.FileSystem.OpenTextFileReader("
        encodingComboBox.Items.Clear()
        readFileFileChooser.Reset()
        encodingComboBox.Items.AddRange(New String() {"ASCII", "BigEndianUnicode", "Unicode", "UTF7", "UTF8", "UTF32"})
        encodingComboBox.AutoSize = True
        encodingComboBox.SelectedItem = "ASCII"
    End Sub

    ''' <summary>
    ''' Execute the parser and place each field in the DataGridView as a column.  This is done in the background to accommodate large files.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ExececuteMethodButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExececuteMethodButton.Click
        Try
            filename = Me.readFileFileChooser.Filename
            delimiters = New String() {GuessDelimiter(filename)}

            'Add one column to the DataGridView for every field in the file
            AddColumns(filename, delimiters)

            Me.BackgroundWorker1.WorkerReportsProgress = True
            Me.BackgroundWorker1.RunWorkerAsync()

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Add one column to DataGridView for each field in the file
    ''' </summary>
    ''' <param name="filename"></param>
    ''' <param name="delimiters"></param>
    ''' <remarks></remarks>
    Private Sub AddColumns(ByVal filename As String, ByVal delimiters As String())
        Using parser As TextFieldParser = My.Computer.FileSystem.OpenTextFieldParser(filename)
            parser.Delimiters = delimiters
            Dim fields As String()
            ' Figure out how many columns the DataGridView should have:
            fields = parser.ReadFields()

            With Me.DataGridView1
                .RowHeadersVisible = False
                Dim fieldIndex As Integer = 0
                For Each field As String In fields
                    'Add a column and configure it accordingly
                    .Columns.Add(fieldIndex.ToString, field)
                    .Columns(fieldIndex).MinimumWidth = 1
                    .Columns(fieldIndex).AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
                    fieldIndex += 1
                Next
                .ReadOnly = True
                .RowHeadersVisible = False
                .ColumnHeadersVisible = False
            End With
        End Using
    End Sub

    ''' <summary>
    ''' Guess the delimiter for this file.
    ''' </summary>
    ''' <param name="filename"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GuessDelimiter(ByVal filename As String) As String
        Dim line As String
        Const numPreviewRows As Integer = 50
        Dim currentLineNumber As Integer = 1
        Dim possibleDelimiters() As String = {vbTab, " ", ";", ",", "|"}
        Dim delimiterCount(possibleDelimiters.Length) As Integer

        ' Iterate through the file and count the number of times each delimiter appears in the line.  Keep
        ' a running total of each delimiter and return the delimiter that occured the most number of times in
        ' the enture file.
        Using fileReader As StreamReader = My.Computer.FileSystem.OpenTextFileReader(Filename)
            While ((fileReader.Peek() > 0) And (currentLineNumber <= numPreviewRows))
                line = fileReader.ReadLine()
                For index As Integer = 0 To possibleDelimiters.Length - 1
                    delimiterCount(index) += line.Split(possibleDelimiters(index).ToCharArray).Length
                Next
            End While
        End Using

        Dim maxOccurIndex As Integer = 0
        ' Find out which delimiter showed up most frequently
        For index As Integer = 0 To delimiterCount.Length - 1
            If (delimiterCount(index) >= delimiterCount(maxOccurIndex)) Then
                maxOccurIndex = index
            End If
        Next
        Return possibleDelimiters(maxOccurIndex)
    End Function

    ''' <summary>
    ''' Reset the control values to their defaults
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ResetValuesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetValuesButton.Click
        InitializeUserControls()
    End Sub

    'Parse the file in the background
    Private Sub BackgroundWorker1_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        'Now populate the rows:
        Using fileParser As TextFieldParser = My.Computer.FileSystem.OpenTextFieldParser(filename)
            fileParser.Delimiters = delimiters
            While (Not fileParser.EndOfData)
                Me.BackgroundWorker1.ReportProgress(0, fileParser.ReadFields())
            End While
        End Using
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        Me.DataGridView1.Rows.Add(CType(e.UserState, String()))
    End Sub
End Class
