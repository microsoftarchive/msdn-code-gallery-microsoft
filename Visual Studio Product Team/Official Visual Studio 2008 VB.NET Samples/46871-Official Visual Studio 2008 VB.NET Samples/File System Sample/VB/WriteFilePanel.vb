' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class WriteFilePanel
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
    Friend WithEvents FileContentsTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.FileContentsTextBox = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'DescriptionTextBox
        '
        Me.DescriptionTextBox.Text = "When writing files infrequently, the My.Computer.WriteAllText can be used to writ" & _
            "e the file in a single of code!"
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
        Me.GroupBox2.Controls.Add(Me.FileContentsTextBox)
        Me.GroupBox2.Controls.SetChildIndex(Me.EndParenLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.ExececuteMethodButton, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.ResetValuesButton, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.FileContentsTextBox, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.Label1, 0)
        '
        'FileContentsTextBox
        '
        Me.FileContentsTextBox.AutoSize = False
        Me.FileContentsTextBox.Location = New System.Drawing.Point(28, 209)
        Me.FileContentsTextBox.Multiline = True
        Me.FileContentsTextBox.Name = "FileContentsTextBox"
        Me.FileContentsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.FileContentsTextBox.Size = New System.Drawing.Size(539, 178)
        Me.FileContentsTextBox.TabIndex = 5
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(28, 188)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(99, 14)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Text to write to file:"
        '
        'WriteFilePanel
        '
        Me.Name = "WriteFilePanel"
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Shared panelInstance As WriteFilePanel
    Friend WithEvents writeFileFileChooser As New FileChooser()
    Friend WithEvents appendComboBox As New ComboBox()
    Friend WithEvents encodingComboBox As New ComboBox()

    ''' <summary>
    ''' Get a global instance of the panel
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As WriteFilePanel
        If (panelInstance Is Nothing) Then
            panelInstance = New WriteFilePanel
        End If
        Return panelInstance
    End Function

    ''' <summary>
    ''' Load the panel, adding one control for each parameter to WriteAllText
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ReadFile_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        InitializeUserControls()
        MyBase.AddParameter("filename", writeFileFileChooser)
        MyBase.AddParameter("append", appendComboBox)
        MyBase.AddParameter("encoding", encodingComboBox)
    End Sub

    ''' <summary>
    ''' Initialize each control to its default value
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeUserControls()
        MyBase.MethodNameLabel.Text = "My.Computer.FileSystem.WriteAllText("
        writeFileFileChooser.Reset()
        appendComboBox.Items.AddRange(New String() {"True", "False"})
        appendComboBox.AutoSize = True
        appendComboBox.SelectedItem = "True"

        encodingComboBox.Items.AddRange(New String() {"ASCII", "BigEndianUnicode", "Unicode", "UTF7", "UTF8", "UTF32"})
        encodingComboBox.AutoSize = True
        encodingComboBox.SelectedItem = "ASCII"
    End Sub

    ''' <summary>
    ''' Write the text in the TextBox out to a file
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ExececuteMethodButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExececuteMethodButton.Click
        Try
            My.Computer.FileSystem.WriteAllText( _
                                            file:=writeFileFileChooser.Filename, _
                                            text:=Me.FileContentsTextBox.Text, _
                                            append:=Boolean.Parse(CType(appendComboBox.SelectedItem, String)), _
                                            encoding:=ParseEncoding(CType(Me.encodingComboBox.SelectedItem, String)))
            Me.FileContentsTextBox.Text = String.Empty
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Reset the control values to their original state
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ResetValuesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetValuesButton.Click
        InitializeUserControls()
    End Sub

End Class
