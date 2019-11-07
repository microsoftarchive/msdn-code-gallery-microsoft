Partial Public Class DirectoryChooser
    Inherits System.Windows.Forms.UserControl

    <System.Diagnostics.DebuggerNonUserCode()> _
    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

    End Sub

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.FileBrowseButton = New System.Windows.Forms.Button
        Me.FileTextBox = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'FileBrowseButton
        '
        Me.FileBrowseButton.Location = New System.Drawing.Point(143, 1)
        Me.FileBrowseButton.Name = "FileBrowseButton"
        Me.FileBrowseButton.Size = New System.Drawing.Size(37, 20)
        Me.FileBrowseButton.TabIndex = 3
        Me.FileBrowseButton.Text = "..."
        '
        'FileTextBox
        '
        Me.FileTextBox.Location = New System.Drawing.Point(2, 1)
        Me.FileTextBox.Name = "FileTextBox"
        Me.FileTextBox.Size = New System.Drawing.Size(134, 20)
        Me.FileTextBox.TabIndex = 2
        '
        'DirectoryChooser
        '
        Me.Controls.Add(Me.FileBrowseButton)
        Me.Controls.Add(Me.FileTextBox)
        Me.Name = "DirectoryChooser"
        Me.Size = New System.Drawing.Size(183, 22)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public WithEvents FileBrowseButton As System.Windows.Forms.Button
    Friend WithEvents FileTextBox As System.Windows.Forms.TextBox

End Class
