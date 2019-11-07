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
        Me.FileMenu = New System.Windows.Forms.ToolStripMenuItem
        Me.NewMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.Option1 = New System.Windows.Forms.ToolStripMenuItem
        Me.MoreOptions = New System.Windows.Forms.ToolStripMenuItem
        Me.MoreOptions1 = New System.Windows.Forms.ToolStripMenuItem
        Me.MoreOptions2 = New System.Windows.Forms.ToolStripMenuItem
        Me.MoreOptions3 = New System.Windows.Forms.ToolStripMenuItem
        Me.Option2 = New System.Windows.Forms.ToolStripMenuItem
        Me.Option3 = New System.Windows.Forms.ToolStripMenuItem
        Me.OpenMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ViewToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.StatusStripOption = New System.Windows.Forms.ToolStripMenuItem
        Me.CheckedListMenu = New System.Windows.Forms.ToolStripMenuItem
        Me.AddOptionMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.RemoveOptionMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripBar = New System.Windows.Forms.ToolStripSeparator
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.MainStatusStrip = New System.Windows.Forms.StatusStrip
        Me.SelectedItem = New System.Windows.Forms.ToolStripButton
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.MainStatusStrip.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'FileMenu
        '
        Me.FileMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NewMenuItem, Me.OpenMenuItem, Me.ToolStripSeparator1, Me.ExitToolStripMenuItem})
        Me.FileMenu.MergeIndex = 1
        Me.FileMenu.Name = "FileMenu"
        Me.FileMenu.Size = New System.Drawing.Size(40, 20)
        Me.FileMenu.Text = "&File"
        Me.FileMenu.ToolTipText = "File Menu"
        '
        'NewMenuItem
        '
        Me.NewMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.Option1, Me.Option2, Me.Option3})
        Me.NewMenuItem.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.NewMenuItem.Image = Global.Menus.My.Resources.Resources.SampleImageNew
        Me.NewMenuItem.Name = "NewMenuItem"
        Me.NewMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.N), System.Windows.Forms.Keys)
        Me.NewMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.NewMenuItem.Text = "&New"
        Me.NewMenuItem.ToolTipText = "New"
        '
        'Option1
        '
        Me.Option1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MoreOptions, Me.MoreOptions1, Me.MoreOptions2, Me.MoreOptions3})
        Me.Option1.Name = "Option1"
        Me.Option1.Size = New System.Drawing.Size(132, 22)
        Me.Option1.Text = "Option 1"
        '
        'MoreOptions
        '
        Me.MoreOptions.Name = "MoreOptions"
        Me.MoreOptions.Size = New System.Drawing.Size(160, 22)
        Me.MoreOptions.Text = "More Options"
        '
        'MoreOptions1
        '
        Me.MoreOptions1.Name = "MoreOptions1"
        Me.MoreOptions1.Size = New System.Drawing.Size(160, 22)
        Me.MoreOptions1.Text = "."
        '
        'MoreOptions2
        '
        Me.MoreOptions2.Name = "MoreOptions2"
        Me.MoreOptions2.Size = New System.Drawing.Size(160, 22)
        Me.MoreOptions2.Text = ".."
        '
        'MoreOptions3
        '
        Me.MoreOptions3.Name = "MoreOptions3"
        Me.MoreOptions3.Size = New System.Drawing.Size(160, 22)
        Me.MoreOptions3.Text = "..."
        '
        'Option2
        '
        Me.Option2.Name = "Option2"
        Me.Option2.Size = New System.Drawing.Size(132, 22)
        Me.Option2.Text = "Option 2"
        '
        'Option3
        '
        Me.Option3.Name = "Option3"
        Me.Option3.Size = New System.Drawing.Size(132, 22)
        Me.Option3.Text = "Option 3"
        '
        'OpenMenuItem
        '
        Me.OpenMenuItem.Enabled = False
        Me.OpenMenuItem.Name = "OpenMenuItem"
        Me.OpenMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.OpenMenuItem.Text = "&Open"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(151, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(154, 22)
        Me.ExitToolStripMenuItem.Text = "E&xit"
        '
        'ViewToolStripMenuItem1
        '
        Me.ViewToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusStripOption})
        Me.ViewToolStripMenuItem1.Name = "ViewToolStripMenuItem1"
        Me.ViewToolStripMenuItem1.Size = New System.Drawing.Size(48, 20)
        Me.ViewToolStripMenuItem1.Text = "&View"
        '
        'StatusStripOption
        '
        Me.StatusStripOption.Checked = True
        Me.StatusStripOption.CheckOnClick = True
        Me.StatusStripOption.CheckState = System.Windows.Forms.CheckState.Checked
        Me.StatusStripOption.Name = "StatusStripOption"
        Me.StatusStripOption.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.StatusStripOption.Size = New System.Drawing.Size(200, 22)
        Me.StatusStripOption.Text = "Status Strip"
        '
        'CheckedListMenu
        '
        Me.CheckedListMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddOptionMenuItem, Me.RemoveOptionMenuItem, Me.ToolStripBar})
        Me.CheckedListMenu.Name = "CheckedListMenu"
        Me.CheckedListMenu.Size = New System.Drawing.Size(91, 20)
        Me.CheckedListMenu.Text = "&Checked List"
        '
        'AddOptionMenuItem
        '
        Me.AddOptionMenuItem.Name = "AddOptionMenuItem"
        Me.AddOptionMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.A), System.Windows.Forms.Keys)
        Me.AddOptionMenuItem.Size = New System.Drawing.Size(220, 22)
        Me.AddOptionMenuItem.Text = "Add Option"
        Me.AddOptionMenuItem.ToolTipText = "Add Option"
        '
        'RemoveOptionMenuItem
        '
        Me.RemoveOptionMenuItem.Name = "RemoveOptionMenuItem"
        Me.RemoveOptionMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.R), System.Windows.Forms.Keys)
        Me.RemoveOptionMenuItem.Size = New System.Drawing.Size(220, 22)
        Me.RemoveOptionMenuItem.Text = "Remove Option"
        Me.RemoveOptionMenuItem.ToolTipText = "Remove Option"
        '
        'ToolStripBar
        '
        Me.ToolStripBar.Name = "ToolStripBar"
        Me.ToolStripBar.Size = New System.Drawing.Size(217, 6)
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(24, 20)
        Me.ToolStripMenuItem1.Text = " "
        '
        'MainStatusStrip
        '
        Me.MainStatusStrip.BackColor = System.Drawing.SystemColors.Control
        Me.MainStatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SelectedItem})
        Me.MainStatusStrip.Location = New System.Drawing.Point(0, 325)
        Me.MainStatusStrip.Name = "MainStatusStrip"
        Me.MainStatusStrip.Size = New System.Drawing.Size(480, 22)
        Me.MainStatusStrip.SizingGrip = False
        Me.MainStatusStrip.TabIndex = 5
        Me.MainStatusStrip.Text = "MainStatusStrip"
        '
        'SelectedItem
        '
        Me.SelectedItem.Name = "SelectedItem"
        Me.SelectedItem.Size = New System.Drawing.Size(23, 15)
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileMenu, Me.ViewToolStripMenuItem1, Me.CheckedListMenu, Me.ToolStripMenuItem1})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.ShowItemToolTips = True
        Me.MenuStrip1.Size = New System.Drawing.Size(480, 24)
        Me.MenuStrip1.TabIndex = 4
        Me.MenuStrip1.Text = "MenuStrip"
        '
        'MainForm
        '
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(480, 347)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.MainStatusStrip)
        Me.Name = "MainForm"
        Me.Text = "Menu Sample"
        Me.MainStatusStrip.ResumeLayout(False)
        Me.MainStatusStrip.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NewMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Option1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MoreOptions As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MoreOptions1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MoreOptions2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MoreOptions3 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Option2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Option3 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ViewToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusStripOption As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CheckedListMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AddOptionMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RemoveOptionMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripBar As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MainStatusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents SelectedItem As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
