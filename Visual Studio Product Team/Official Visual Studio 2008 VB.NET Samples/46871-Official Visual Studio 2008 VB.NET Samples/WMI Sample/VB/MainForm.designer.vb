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
        Me.mnuMain = New System.Windows.Forms.MainMenu(Me.components)
        Me.tpWMIClasses = New System.Windows.Forms.TabPage
        Me.chkIncludeSubclasses = New System.Windows.Forms.CheckBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.btnClassEnum = New System.Windows.Forms.Button
        Me.lstWMIClasses = New System.Windows.Forms.ListBox
        Me.tpWMIQueries = New System.Windows.Forms.TabPage
        Me.lblInstructions = New System.Windows.Forms.Label
        Me.btnTimeZone = New System.Windows.Forms.Button
        Me.btnBios = New System.Windows.Forms.Button
        Me.btnProcessor = New System.Windows.Forms.Button
        Me.btnComputerSystem = New System.Windows.Forms.Button
        Me.btnOperatingSytem = New System.Windows.Forms.Button
        Me.txtOutput = New System.Windows.Forms.TextBox
        Me.MainTabControl = New System.Windows.Forms.TabControl
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.tpWMIClasses.SuspendLayout()
        Me.tpWMIQueries.SuspendLayout()
        Me.MainTabControl.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tpWMIClasses
        '
        Me.tpWMIClasses.Controls.Add(Me.chkIncludeSubclasses)
        Me.tpWMIClasses.Controls.Add(Me.Label1)
        Me.tpWMIClasses.Controls.Add(Me.btnClassEnum)
        Me.tpWMIClasses.Controls.Add(Me.lstWMIClasses)
        Me.tpWMIClasses.Location = New System.Drawing.Point(4, 22)
        Me.tpWMIClasses.Name = "tpWMIClasses"
        Me.tpWMIClasses.Size = New System.Drawing.Size(568, 286)
        Me.tpWMIClasses.TabIndex = 2
        Me.tpWMIClasses.Text = "WMI Classes"
        Me.tpWMIClasses.UseVisualStyleBackColor = False
        '
        'chkIncludeSubclasses
        '
        Me.chkIncludeSubclasses.AccessibleDescription = "Include subclasses checkbox"
        Me.chkIncludeSubclasses.AccessibleName = "Include subclasses checkbox"
        Me.chkIncludeSubclasses.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.chkIncludeSubclasses.Location = New System.Drawing.Point(16, 240)
        Me.chkIncludeSubclasses.Name = "chkIncludeSubclasses"
        Me.chkIncludeSubclasses.Size = New System.Drawing.Size(128, 24)
        Me.chkIncludeSubclasses.TabIndex = 9
        Me.chkIncludeSubclasses.Text = "Include Subclasses"
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.Label1.ForeColor = System.Drawing.Color.Blue
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(16, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(536, 32)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "This demonstrates how to get a list of all WMI classes through use of WMI classes" & _
            " ManagementClass and EnumerationOptions:"
        '
        'btnClassEnum
        '
        Me.btnClassEnum.AccessibleDescription = "ClassEnum Button"
        Me.btnClassEnum.AccessibleName = "ClassEnum Button"
        Me.btnClassEnum.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnClassEnum.Location = New System.Drawing.Point(168, 232)
        Me.btnClassEnum.Name = "btnClassEnum"
        Me.btnClassEnum.Size = New System.Drawing.Size(168, 32)
        Me.btnClassEnum.TabIndex = 2
        Me.btnClassEnum.Text = "&Start Enumeration"
        '
        'lstWMIClasses
        '
        Me.lstWMIClasses.FormattingEnabled = True
        Me.lstWMIClasses.Location = New System.Drawing.Point(16, 56)
        Me.lstWMIClasses.Name = "lstWMIClasses"
        Me.lstWMIClasses.Size = New System.Drawing.Size(536, 160)
        Me.lstWMIClasses.TabIndex = 0
        '
        'tpWMIQueries
        '
        Me.tpWMIQueries.AccessibleDescription = "WMI Queries Tab Page"
        Me.tpWMIQueries.AccessibleName = "WMI Queries Tab Page"
        Me.tpWMIQueries.Controls.Add(Me.lblInstructions)
        Me.tpWMIQueries.Controls.Add(Me.btnTimeZone)
        Me.tpWMIQueries.Controls.Add(Me.btnBios)
        Me.tpWMIQueries.Controls.Add(Me.btnProcessor)
        Me.tpWMIQueries.Controls.Add(Me.btnComputerSystem)
        Me.tpWMIQueries.Controls.Add(Me.btnOperatingSytem)
        Me.tpWMIQueries.Controls.Add(Me.txtOutput)
        Me.tpWMIQueries.Location = New System.Drawing.Point(4, 22)
        Me.tpWMIQueries.Name = "tpWMIQueries"
        Me.tpWMIQueries.Size = New System.Drawing.Size(568, 286)
        Me.tpWMIQueries.TabIndex = 0
        Me.tpWMIQueries.Text = "WMI Queries"
        Me.tpWMIQueries.UseVisualStyleBackColor = False
        '
        'lblInstructions
        '
        Me.lblInstructions.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.lblInstructions.ForeColor = System.Drawing.Color.Blue
        Me.lblInstructions.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblInstructions.Location = New System.Drawing.Point(8, 8)
        Me.lblInstructions.Name = "lblInstructions"
        Me.lblInstructions.Size = New System.Drawing.Size(536, 32)
        Me.lblInstructions.TabIndex = 6
        Me.lblInstructions.Text = "This demonstrates how to use the WMI class ManagementObjectSearcher to get system" & _
            " information using queries."
        '
        'btnTimeZone
        '
        Me.btnTimeZone.AccessibleDescription = "TimeZone button"
        Me.btnTimeZone.AccessibleName = "TimeZone button"
        Me.btnTimeZone.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnTimeZone.Location = New System.Drawing.Point(456, 240)
        Me.btnTimeZone.Name = "btnTimeZone"
        Me.btnTimeZone.Size = New System.Drawing.Size(104, 32)
        Me.btnTimeZone.TabIndex = 5
        Me.btnTimeZone.Text = "&Time Zone"
        '
        'btnBios
        '
        Me.btnBios.AccessibleDescription = "Bios button"
        Me.btnBios.AccessibleName = "Bios button"
        Me.btnBios.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnBios.Location = New System.Drawing.Point(344, 240)
        Me.btnBios.Name = "btnBios"
        Me.btnBios.Size = New System.Drawing.Size(104, 32)
        Me.btnBios.TabIndex = 4
        Me.btnBios.Text = "&Bios"
        '
        'btnProcessor
        '
        Me.btnProcessor.AccessibleDescription = "Processor button"
        Me.btnProcessor.AccessibleName = "Processor button"
        Me.btnProcessor.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnProcessor.Location = New System.Drawing.Point(232, 240)
        Me.btnProcessor.Name = "btnProcessor"
        Me.btnProcessor.Size = New System.Drawing.Size(104, 32)
        Me.btnProcessor.TabIndex = 3
        Me.btnProcessor.Text = "&Processor"
        '
        'btnComputerSystem
        '
        Me.btnComputerSystem.AccessibleDescription = "Computer System button"
        Me.btnComputerSystem.AccessibleName = "Computer System button"
        Me.btnComputerSystem.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnComputerSystem.Location = New System.Drawing.Point(120, 240)
        Me.btnComputerSystem.Name = "btnComputerSystem"
        Me.btnComputerSystem.Size = New System.Drawing.Size(104, 32)
        Me.btnComputerSystem.TabIndex = 2
        Me.btnComputerSystem.Text = "&Computer System"
        '
        'btnOperatingSytem
        '
        Me.btnOperatingSytem.AccessibleDescription = "OperatingSystem Button"
        Me.btnOperatingSytem.AccessibleName = "OperatingSystem Button"
        Me.btnOperatingSytem.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnOperatingSytem.Location = New System.Drawing.Point(8, 240)
        Me.btnOperatingSytem.Name = "btnOperatingSytem"
        Me.btnOperatingSytem.Size = New System.Drawing.Size(104, 32)
        Me.btnOperatingSytem.TabIndex = 1
        Me.btnOperatingSytem.Text = "&Operating System"
        '
        'txtOutput
        '
        Me.txtOutput.AccessibleDescription = "Output TextBox"
        Me.txtOutput.AccessibleName = "Output TextBox"
        Me.txtOutput.Location = New System.Drawing.Point(8, 48)
        Me.txtOutput.Multiline = True
        Me.txtOutput.Name = "txtOutput"
        Me.txtOutput.Size = New System.Drawing.Size(552, 176)
        Me.txtOutput.TabIndex = 0
        '
        'MainTabControl
        '
        Me.MainTabControl.AccessibleDescription = "Main Tab Control"
        Me.MainTabControl.AccessibleName = "Main Tab Control"
        Me.MainTabControl.Controls.Add(Me.tpWMIQueries)
        Me.MainTabControl.Controls.Add(Me.tpWMIClasses)
        Me.MainTabControl.ItemSize = New System.Drawing.Size(74, 18)
        Me.MainTabControl.Location = New System.Drawing.Point(12, 36)
        Me.MainTabControl.Name = "MainTabControl"
        Me.MainTabControl.SelectedIndex = 0
        Me.MainTabControl.Size = New System.Drawing.Size(576, 312)
        Me.MainTabControl.TabIndex = 1
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(594, 24)
        Me.MenuStrip1.TabIndex = 2
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(40, 20)
        Me.FileToolStripMenuItem.Text = "&File"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(109, 22)
        Me.ExitToolStripMenuItem.Text = "E&xit"
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(594, 354)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.MainTabControl)
        Me.Menu = Me.mnuMain
        Me.Name = "MainForm"
        Me.Text = "Windows Management Interface (WMI) Sample"
        Me.tpWMIClasses.ResumeLayout(False)
        Me.tpWMIQueries.ResumeLayout(False)
        Me.tpWMIQueries.PerformLayout()
        Me.MainTabControl.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents mnuMain As System.Windows.Forms.MainMenu
    Friend WithEvents tpWMIClasses As System.Windows.Forms.TabPage
    Friend WithEvents chkIncludeSubclasses As System.Windows.Forms.CheckBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnClassEnum As System.Windows.Forms.Button
    Friend WithEvents lstWMIClasses As System.Windows.Forms.ListBox
    Friend WithEvents tpWMIQueries As System.Windows.Forms.TabPage
    Friend WithEvents lblInstructions As System.Windows.Forms.Label
    Friend WithEvents btnTimeZone As System.Windows.Forms.Button
    Friend WithEvents btnBios As System.Windows.Forms.Button
    Friend WithEvents btnProcessor As System.Windows.Forms.Button
    Friend WithEvents btnComputerSystem As System.Windows.Forms.Button
    Friend WithEvents btnOperatingSytem As System.Windows.Forms.Button
    Friend WithEvents txtOutput As System.Windows.Forms.TextBox
    Friend WithEvents MainTabControl As System.Windows.Forms.TabControl
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
