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
        Me.Explorer = New System.Windows.Forms.Button
        Me.Simple = New System.Windows.Forms.Button
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Explorer
        '
        Me.Explorer.AccessibleDescription = "Button with text ""Launch Explorer-Style Viewer"""
        Me.Explorer.AccessibleName = "Launch Explorer-Style Viewer"
        Me.Explorer.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Explorer.Location = New System.Drawing.Point(93, 67)
        Me.Explorer.Name = "Explorer"
        Me.Explorer.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Explorer.Size = New System.Drawing.Size(232, 23)
        Me.Explorer.TabIndex = 5
        Me.Explorer.Text = "Launch &Explorer-Style Viewer"
        '
        'Simple
        '
        Me.Simple.AccessibleDescription = "Button with text ""Launch Directory Scanner"""
        Me.Simple.AccessibleName = "Launch Directory Scanner"
        Me.Simple.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Simple.Location = New System.Drawing.Point(93, 35)
        Me.Simple.Name = "Simple"
        Me.Simple.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Simple.Size = New System.Drawing.Size(232, 23)
        Me.Simple.TabIndex = 4
        Me.Simple.Text = "Launch &Directory Scanner"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(421, 26)
        Me.MenuStrip1.TabIndex = 6
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
        Me.ClientSize = New System.Drawing.Size(421, 102)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.Simple)
        Me.Controls.Add(Me.Explorer)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "Form1"
        Me.Text = "Explorer Application Sample"
        Me.MenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Explorer As System.Windows.Forms.Button
    Friend WithEvents Simple As System.Windows.Forms.Button
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
