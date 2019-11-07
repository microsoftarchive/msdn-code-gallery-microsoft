' Copyright (c) Microsoft Corporation. All rights reserved.
Partial Public Class Modules
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
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.lvModules = New System.Windows.Forms.ListView
        Me.chModName = New System.Windows.Forms.ColumnHeader("")
        Me.lvModDetails = New System.Windows.Forms.ListView
        Me.chItem = New System.Windows.Forms.ColumnHeader("")
        Me.chValue = New System.Windows.Forms.ColumnHeader("")
        Me.MenuStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(6, 2, 0, 2)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem2})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Text = "&File"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Text = "&Close"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1})
        Me.StatusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 246)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(533, 20)
        Me.StatusStrip1.TabIndex = 1
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Text = "ToolStripStatusLabel1"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 24)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.lvModules)
        '
        'Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.lvModDetails)
        Me.SplitContainer1.Size = New System.Drawing.Size(533, 222)
        Me.SplitContainer1.SplitterDistance = 208
        Me.SplitContainer1.TabIndex = 2
        Me.SplitContainer1.Text = "SplitContainer1"
        '
        'lvModules
        '
        Me.lvModules.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chModName})
        Me.lvModules.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvModules.Location = New System.Drawing.Point(0, 0)
        Me.lvModules.Name = "lvModules"
        Me.lvModules.Size = New System.Drawing.Size(208, 223)
        Me.lvModules.TabIndex = 0
        Me.lvModules.View = System.Windows.Forms.View.Details
        '
        'chModName
        '
        Me.chModName.Text = "ModName"
        Me.chModName.Width = 100
        '
        'lvModDetails
        '
        Me.lvModDetails.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chItem, Me.chValue})
        Me.lvModDetails.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvModDetails.Location = New System.Drawing.Point(0, 0)
        Me.lvModDetails.Name = "lvModDetails"
        Me.lvModDetails.Size = New System.Drawing.Size(321, 222)
        Me.lvModDetails.TabIndex = 0
        Me.lvModDetails.View = System.Windows.Forms.View.Details
        '
        'chItem
        '
        Me.chItem.Text = "Item"
        '
        'chValue
        '
        Me.chValue.Text = "Value"
        '
        'Modules
        '
        Me.ClientSize = New System.Drawing.Size(533, 266)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Name = "Modules"
        Me.Text = "Modules"
        Me.MenuStrip1.ResumeLayout(False)
        Me.StatusStrip1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents lvModules As System.Windows.Forms.ListView
    Friend WithEvents lvModDetails As System.Windows.Forms.ListView
    Friend WithEvents chModName As System.Windows.Forms.ColumnHeader
    Friend WithEvents chItem As System.Windows.Forms.ColumnHeader
    Friend WithEvents chValue As System.Windows.Forms.ColumnHeader
End Class
