' Copyright (c) Microsoft Corporation. All rights reserved.
Partial Public Class Form1
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
        Me.cmdResume = New System.Windows.Forms.Button
        Me.cmdPause = New System.Windows.Forms.Button
        Me.cmdStop = New System.Windows.Forms.Button
        Me.cmdStart = New System.Windows.Forms.Button
        Me.chSvcType = New System.Windows.Forms.ColumnHeader("")
        Me.chStatus = New System.Windows.Forms.ColumnHeader("")
        Me.lvServices = New System.Windows.Forms.ListView
        Me.chName = New System.Windows.Forms.ColumnHeader("")
        Me.pnlCommands = New System.Windows.Forms.Panel
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ActionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem
        Me.RefreshToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.pnlCommands.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmdResume
        '
        Me.cmdResume.Location = New System.Drawing.Point(248, 8)
        Me.cmdResume.Name = "cmdResume"
        Me.cmdResume.TabIndex = 3
        Me.cmdResume.Text = "&Resume"
        '
        'cmdPause
        '
        Me.cmdPause.Location = New System.Drawing.Point(168, 8)
        Me.cmdPause.Name = "cmdPause"
        Me.cmdPause.TabIndex = 2
        Me.cmdPause.Text = "&Pause"
        '
        'cmdStop
        '
        Me.cmdStop.Location = New System.Drawing.Point(88, 8)
        Me.cmdStop.Name = "cmdStop"
        Me.cmdStop.TabIndex = 1
        Me.cmdStop.Text = "S&top"
        '
        'cmdStart
        '
        Me.cmdStart.Location = New System.Drawing.Point(8, 8)
        Me.cmdStart.Name = "cmdStart"
        Me.cmdStart.TabIndex = 0
        Me.cmdStart.Text = "&Start"
        '
        'chSvcType
        '
        Me.chSvcType.Text = "Service Type"
        Me.chSvcType.Width = 225
        '
        'chStatus
        '
        Me.chStatus.Text = "Status"
        Me.chStatus.Width = 100
        '
        'lvServices
        '
        Me.lvServices.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chName, Me.chStatus, Me.chSvcType})
        Me.lvServices.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvServices.FullRowSelect = True
        Me.lvServices.HideSelection = False
        Me.lvServices.Location = New System.Drawing.Point(0, 0)
        Me.lvServices.MultiSelect = False
        Me.lvServices.Name = "lvServices"
        Me.lvServices.Size = New System.Drawing.Size(574, 267)
        Me.lvServices.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lvServices.TabIndex = 3
        Me.lvServices.View = System.Windows.Forms.View.Details
        '
        'chName
        '
        Me.chName.Text = "Name"
        Me.chName.Width = 200
        '
        'pnlCommands
        '
        Me.pnlCommands.Controls.Add(Me.cmdResume)
        Me.pnlCommands.Controls.Add(Me.cmdPause)
        Me.pnlCommands.Controls.Add(Me.cmdStop)
        Me.pnlCommands.Controls.Add(Me.cmdStart)
        Me.pnlCommands.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlCommands.Location = New System.Drawing.Point(0, 0)
        Me.pnlCommands.Name = "pnlCommands"
        Me.pnlCommands.Size = New System.Drawing.Size(574, 51)
        Me.pnlCommands.TabIndex = 4
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem, Me.ActionsToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(6, 2, 0, 2)
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
        'ActionsToolStripMenuItem
        '
        Me.ActionsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem2, Me.RefreshToolStripMenuItem})
        Me.ActionsToolStripMenuItem.Name = "ActionsToolStripMenuItem"
        Me.ActionsToolStripMenuItem.Text = "&Actions"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Text = "Relist &All Services"
        '
        'RefreshToolStripMenuItem
        '
        Me.RefreshToolStripMenuItem.Name = "RefreshToolStripMenuItem"
        Me.RefreshToolStripMenuItem.Text = "&Refresh"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1})
        Me.StatusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 343)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.TabIndex = 7
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
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 21)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.lvServices)
        '
        'Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlCommands)
        Me.SplitContainer1.Size = New System.Drawing.Size(574, 322)
        Me.SplitContainer1.SplitterDistance = 267
        Me.SplitContainer1.TabIndex = 8
        Me.SplitContainer1.Text = "SplitContainer1"
        '
        'Form1
        '
        Me.ClientSize = New System.Drawing.Size(574, 365)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.pnlCommands.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.StatusStrip1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cmdResume As System.Windows.Forms.Button
    Friend WithEvents cmdPause As System.Windows.Forms.Button
    Friend WithEvents cmdStop As System.Windows.Forms.Button
    Friend WithEvents cmdStart As System.Windows.Forms.Button
    Friend WithEvents chSvcType As System.Windows.Forms.ColumnHeader
    Friend WithEvents chStatus As System.Windows.Forms.ColumnHeader
    Friend WithEvents lvServices As System.Windows.Forms.ListView
    Friend WithEvents chName As System.Windows.Forms.ColumnHeader
    Friend WithEvents pnlCommands As System.Windows.Forms.Panel
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents ActionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RefreshToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
