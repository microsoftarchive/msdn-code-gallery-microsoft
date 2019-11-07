' Copyright (c) Microsoft Corporation. All rights reserved.
Partial Public Class MainForm
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()> _
    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

    End Sub

    'Form overrides dispose to clean up the component list.
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
        Me.btnCreateDelete = New System.Windows.Forms.Button
        Me.btnRead = New System.Windows.Forms.Button
        Me.btnWrite = New System.Windows.Forms.Button
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnCreateDelete
        '
        Me.btnCreateDelete.AccessibleDescription = "Click to open the form for creating and deleting event logs."
        Me.btnCreateDelete.AccessibleName = "ButtonCreateDelete"
        Me.btnCreateDelete.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnCreateDelete.Location = New System.Drawing.Point(70, 162)
        Me.btnCreateDelete.Name = "btnCreateDelete"
        Me.btnCreateDelete.Size = New System.Drawing.Size(152, 23)
        Me.btnCreateDelete.TabIndex = 8
        Me.btnCreateDelete.Text = "&Create and Delete Logs"
        '
        'btnRead
        '
        Me.btnRead.AccessibleDescription = "Click to open the form for reading from the event log."
        Me.btnRead.AccessibleName = "ButtonRead"
        Me.btnRead.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnRead.Location = New System.Drawing.Point(70, 122)
        Me.btnRead.Name = "btnRead"
        Me.btnRead.Size = New System.Drawing.Size(152, 23)
        Me.btnRead.TabIndex = 7
        Me.btnRead.Text = "&Read from the Event Log"
        '
        'btnWrite
        '
        Me.btnWrite.AccessibleDescription = "Click to open the form for writting to the event log."
        Me.btnWrite.AccessibleName = "ButtonWrite"
        Me.btnWrite.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnWrite.Location = New System.Drawing.Point(70, 82)
        Me.btnWrite.Name = "btnWrite"
        Me.btnWrite.Size = New System.Drawing.Size(152, 23)
        Me.btnWrite.TabIndex = 6
        Me.btnWrite.Text = "&Write to the Event Log"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(6, 2, 0, 2)
        Me.MenuStrip1.TabIndex = 9
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'fileToolStripMenuItem
        '
        Me.fileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.exitToolStripMenuItem})
        Me.fileToolStripMenuItem.Name = "fileToolStripMenuItem"

        Me.fileToolStripMenuItem.Text = "&File"
        '
        'exitToolStripMenuItem
        '
        Me.exitToolStripMenuItem.Name = "exitToolStripMenuItem"

        Me.exitToolStripMenuItem.Text = "E&xit"
        '
        'Form1
        '
        Me.ClientSize = New System.Drawing.Size(292, 266)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.btnRead)
        Me.Controls.Add(Me.btnWrite)
        Me.Controls.Add(Me.btnCreateDelete)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.MenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnCreateDelete As System.Windows.Forms.Button
    Friend WithEvents btnRead As System.Windows.Forms.Button
    Friend WithEvents btnWrite As System.Windows.Forms.Button
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
