' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class ReadFilePanel
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


    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.FileContentsTextBox = New System.Windows.Forms.TextBox
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'DescriptionTextBox
        '
        Me.DescriptionTextBox.Text = "When reading the entire contents of a file, the My.Computer.FileSystem.ReadAllTex" & _
            "t method can be used.  "
        '
        'ExececuteMethodButton
        '
        '
        'ResetValuesButton
        '
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.FileContentsTextBox)
        Me.GroupBox2.Controls.SetChildIndex(Me.FileContentsTextBox, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.EndParenLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.ExececuteMethodButton, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.ResetValuesButton, 0)
        '
        'FileContentsTextBox
        '
        Me.FileContentsTextBox.AutoSize = False
        Me.FileContentsTextBox.Location = New System.Drawing.Point(27, 193)
        Me.FileContentsTextBox.Multiline = True
        Me.FileContentsTextBox.Name = "FileContentsTextBox"
        Me.FileContentsTextBox.ReadOnly = True
        Me.FileContentsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.FileContentsTextBox.Size = New System.Drawing.Size(539, 178)
        Me.FileContentsTextBox.TabIndex = 4
        '
        'ReadFilePanel
        '
        Me.Name = "ReadFilePanel"
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Shared panelInstance As ReadFilePanel
    Friend WithEvents readFileFileChooser As New FileChooser()
    Friend WithEvents encodingComboBox As New ComboBox()

    ''' <summary>
    ''' Get a global instance of this panel
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As ReadFilePanel
        If (panelInstance Is Nothing) Then
            panelInstance = New ReadFilePanel
        End If
        Return panelInstance
    End Function

    ''' <summary>
    ''' Load the panel, adding a control for each parameter to ReadAllText()
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
    ''' Initialize the controls to their default values.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeUserControls()
        Me.readFileFileChooser.Reset()
        MyBase.MethodNameLabel.Text = "My.Computer.FileSystem.ReadAllText("
        encodingComboBox.Items.AddRange(New String() {"ASCII", "BigEndianUnicode", "Unicode", "UTF7", "UTF8", "UTF32"})
        encodingComboBox.AutoSize = True
        encodingComboBox.SelectedItem = "ASCII"
    End Sub

    ''' <summary>
    ''' Execute the ReadAllText() method with the paramter values provided.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ExececuteMethodButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExececuteMethodButton.Click
        Try
            Me.FileContentsTextBox.Text = My.Computer.FileSystem.ReadAllText( _
                                                            file:=readFileFileChooser.Filename, _
                                                            encoding:=ParseEncoding(CType(Me.encodingComboBox.SelectedItem, String)))
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
