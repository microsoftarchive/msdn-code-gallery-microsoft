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
        Me.components = New System.ComponentModel.Container
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ViewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ModulesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.RefreshToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer
        Me.lvProcesses = New System.Windows.Forms.ListView
        Me.chProcessName = New System.Windows.Forms.ColumnHeader
        Me.chPID = New System.Windows.Forms.ColumnHeader
        Me.cdProcessorTime = New System.Windows.Forms.ColumnHeader
        Me.cdPriv = New System.Windows.Forms.ColumnHeader
        Me.chUser = New System.Windows.Forms.ColumnHeader
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ModuleContextMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.RefreshContextMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.lvProcessDetail = New System.Windows.Forms.ListView
        Me.chItem = New System.Windows.Forms.ColumnHeader
        Me.chValue = New System.Windows.Forms.ColumnHeader
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.lvThreads = New System.Windows.Forms.ListView
        Me.chThreads = New System.Windows.Forms.ColumnHeader
        Me.chBasePri = New System.Windows.Forms.ColumnHeader
        Me.chCurrentPri = New System.Windows.Forms.ColumnHeader
        Me.chPriBoostEnabled = New System.Windows.Forms.ColumnHeader
        Me.chPriLevel = New System.Windows.Forms.ColumnHeader
        Me.chPrivProcTime = New System.Windows.Forms.ColumnHeader
        Me.chStartAddress = New System.Windows.Forms.ColumnHeader
        Me.chTotalProcTime = New System.Windows.Forms.ColumnHeader
        Me.chUserProcTime = New System.Windows.Forms.ColumnHeader
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuStrip1.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem, Me.ViewToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(560, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'fileToolStripMenuItem
        '
        Me.fileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.exitToolStripMenuItem})
        Me.fileToolStripMenuItem.Name = "fileToolStripMenuItem"
        Me.fileToolStripMenuItem.Size = New System.Drawing.Size(40, 20)
        Me.fileToolStripMenuItem.Text = "&File"
        '
        'exitToolStripMenuItem
        '
        Me.exitToolStripMenuItem.Name = "exitToolStripMenuItem"
        Me.exitToolStripMenuItem.Size = New System.Drawing.Size(109, 22)
        Me.exitToolStripMenuItem.Text = "E&xit"
        '
        'ViewToolStripMenuItem
        '
        Me.ViewToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ModulesToolStripMenuItem, Me.RefreshToolStripMenuItem})
        Me.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem"
        Me.ViewToolStripMenuItem.Size = New System.Drawing.Size(48, 20)
        Me.ViewToolStripMenuItem.Text = "&View"
        '
        'ModulesToolStripMenuItem
        '
        Me.ModulesToolStripMenuItem.Name = "ModulesToolStripMenuItem"
        Me.ModulesToolStripMenuItem.Size = New System.Drawing.Size(136, 22)
        Me.ModulesToolStripMenuItem.Text = "&Modules"
        '
        'RefreshToolStripMenuItem
        '
        Me.RefreshToolStripMenuItem.Name = "RefreshToolStripMenuItem"
        Me.RefreshToolStripMenuItem.Size = New System.Drawing.Size(136, 22)
        Me.RefreshToolStripMenuItem.Text = "&Refresh"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 24)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.SplitContainer2)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.StatusStrip1)
        Me.SplitContainer1.Panel2.Controls.Add(Me.lvThreads)
        Me.SplitContainer1.Size = New System.Drawing.Size(560, 314)
        Me.SplitContainer1.SplitterDistance = 199
        Me.SplitContainer1.TabIndex = 1
        Me.SplitContainer1.Text = "SplitContainer1"
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.lvProcesses)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.lvProcessDetail)
        Me.SplitContainer2.Size = New System.Drawing.Size(560, 199)
        Me.SplitContainer2.SplitterDistance = 330
        Me.SplitContainer2.TabIndex = 0
        Me.SplitContainer2.Text = "SplitContainer2"
        '
        'lvProcesses
        '
        Me.lvProcesses.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chProcessName, Me.chPID, Me.cdProcessorTime, Me.cdPriv, Me.chUser})
        Me.lvProcesses.ContextMenuStrip = Me.ContextMenuStrip1
        Me.lvProcesses.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvProcesses.FullRowSelect = True
        Me.lvProcesses.Location = New System.Drawing.Point(0, 0)
        Me.lvProcesses.Name = "lvProcesses"
        Me.lvProcesses.Size = New System.Drawing.Size(330, 199)
        Me.lvProcesses.TabIndex = 0
        Me.lvProcesses.View = System.Windows.Forms.View.Details
        '
        'chProcessName
        '
        Me.chProcessName.Name = "chProcessName"
        Me.chProcessName.Text = "Process Name"
        Me.chProcessName.Width = 90
        '
        'chPID
        '
        Me.chPID.Name = "chPID"
        Me.chPID.Text = "PID"
        '
        'cdProcessorTime
        '
        Me.cdProcessorTime.Name = "cdProcessorTime"
        Me.cdProcessorTime.Text = "Processor Time"
        Me.cdProcessorTime.Width = 100
        '
        'cdPriv
        '
        Me.cdPriv.Name = "cdPriv"
        Me.cdPriv.Text = "Priv"
        '
        'chUser
        '
        Me.chUser.Name = "chUser"
        Me.chUser.Text = "User"
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ModuleContextMenuItem, Me.RefreshContextMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(137, 48)
        '
        'ModuleContextMenuItem
        '
        Me.ModuleContextMenuItem.Name = "ModuleContextMenuItem"
        Me.ModuleContextMenuItem.Size = New System.Drawing.Size(136, 22)
        Me.ModuleContextMenuItem.Text = "&Modules"
        '
        'RefreshContextMenuItem
        '
        Me.RefreshContextMenuItem.Name = "RefreshContextMenuItem"
        Me.RefreshContextMenuItem.Size = New System.Drawing.Size(136, 22)
        Me.RefreshContextMenuItem.Text = "&Refresh"
        '
        'lvProcessDetail
        '
        Me.lvProcessDetail.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chItem, Me.chValue})
        Me.lvProcessDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvProcessDetail.FullRowSelect = True
        Me.lvProcessDetail.Location = New System.Drawing.Point(0, 0)
        Me.lvProcessDetail.Name = "lvProcessDetail"
        Me.lvProcessDetail.Size = New System.Drawing.Size(226, 199)
        Me.lvProcessDetail.TabIndex = 0
        Me.lvProcessDetail.View = System.Windows.Forms.View.Details
        '
        'chItem
        '
        Me.chItem.Name = "chItem"
        Me.chItem.Text = "Item"
        '
        'chValue
        '
        Me.chValue.Name = "chValue"
        Me.chValue.Text = "Value"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 89)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(560, 22)
        Me.StatusStrip1.TabIndex = 1
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'lvThreads
        '
        Me.lvThreads.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chThreads, Me.chBasePri, Me.chCurrentPri, Me.chPriBoostEnabled, Me.chPriLevel, Me.chPrivProcTime, Me.chStartAddress, Me.chTotalProcTime, Me.chUserProcTime})
        Me.lvThreads.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvThreads.FullRowSelect = True
        Me.lvThreads.Location = New System.Drawing.Point(0, 0)
        Me.lvThreads.Name = "lvThreads"
        Me.lvThreads.Size = New System.Drawing.Size(560, 111)
        Me.lvThreads.TabIndex = 0
        Me.lvThreads.View = System.Windows.Forms.View.Details
        '
        'chThreads
        '
        Me.chThreads.Name = "chThreads"
        Me.chThreads.Text = "Threads"
        '
        'chBasePri
        '
        Me.chBasePri.Name = "chBasePri"
        Me.chBasePri.Text = "Base Pri"
        '
        'chCurrentPri
        '
        Me.chCurrentPri.Name = "chCurrentPri"
        Me.chCurrentPri.Text = "Current Pri"
        '
        'chPriBoostEnabled
        '
        Me.chPriBoostEnabled.Name = "chPriBoostEnabled"
        Me.chPriBoostEnabled.Text = "Boost Enabled"
        '
        'chPriLevel
        '
        Me.chPriLevel.Name = "chPriLevel"
        Me.chPriLevel.Text = "Pri Level"
        '
        'chPrivProcTime
        '
        Me.chPrivProcTime.Name = "chPrivProcTime"
        Me.chPrivProcTime.Text = "PrivProcTime"
        '
        'chStartAddress
        '
        Me.chStartAddress.Name = "chStartAddress"
        Me.chStartAddress.Text = "Start Address"
        '
        'chTotalProcTime
        '
        Me.chTotalProcTime.Name = "chTotalProcTime"
        Me.chTotalProcTime.Text = "Total Proc Time"
        '
        'chUserProcTime
        '
        Me.chUserProcTime.Name = "chUserProcTime"
        Me.chUserProcTime.Text = "User Proc Time"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.BackColor = System.Drawing.SystemColors.Window
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(32, 19)
        Me.ToolStripMenuItem2.Text = "ContextMenuStrip"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.BackColor = System.Drawing.SystemColors.Window
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(32, 19)
        Me.ToolStripMenuItem1.Text = "ContextMenuStrip"
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(560, 338)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Name = "MainForm"
        Me.Text = "Process Viewer Sample"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        Me.SplitContainer2.ResumeLayout(False)
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ViewToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ModulesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RefreshToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
    Friend WithEvents lvProcesses As System.Windows.Forms.ListView
    Friend WithEvents lvProcessDetail As System.Windows.Forms.ListView
    Friend WithEvents lvThreads As System.Windows.Forms.ListView
    Friend WithEvents chThreads As System.Windows.Forms.ColumnHeader
    Friend WithEvents chBasePri As System.Windows.Forms.ColumnHeader
    Friend WithEvents chCurrentPri As System.Windows.Forms.ColumnHeader
    Friend WithEvents chPriBoostEnabled As System.Windows.Forms.ColumnHeader
    Friend WithEvents chPriLevel As System.Windows.Forms.ColumnHeader
    Friend WithEvents chPrivProcTime As System.Windows.Forms.ColumnHeader
    Friend WithEvents chStartAddress As System.Windows.Forms.ColumnHeader
    Friend WithEvents chTotalProcTime As System.Windows.Forms.ColumnHeader
    Friend WithEvents chUserProcTime As System.Windows.Forms.ColumnHeader
    Friend WithEvents chProcessName As System.Windows.Forms.ColumnHeader
    Friend WithEvents chPID As System.Windows.Forms.ColumnHeader
    Friend WithEvents cdProcessorTime As System.Windows.Forms.ColumnHeader
    Friend WithEvents cdPriv As System.Windows.Forms.ColumnHeader
    Friend WithEvents chUser As System.Windows.Forms.ColumnHeader
    Friend WithEvents chItem As System.Windows.Forms.ColumnHeader
    Friend WithEvents chValue As System.Windows.Forms.ColumnHeader
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ModuleContextMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RefreshContextMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripMenuItem
End Class
