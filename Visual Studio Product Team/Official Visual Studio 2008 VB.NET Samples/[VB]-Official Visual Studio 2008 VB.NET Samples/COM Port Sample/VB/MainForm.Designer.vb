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
        Me.ClearButton = New System.Windows.Forms.Button
        Me.tmrReadCommPort = New System.Windows.Forms.Timer(Me.components)
        Me.UserCommandTextbox = New System.Windows.Forms.TextBox
        Me.SendUserCommandButton = New System.Windows.Forms.Button
        Me.SendATCommandButton = New System.Windows.Forms.Button
        Me.SelectedModemTextbox = New System.Windows.Forms.TextBox
        Me.CheckModemsButton = New System.Windows.Forms.Button
        Me.PortsList = New System.Windows.Forms.CheckedListBox
        Me.StatusTextbox = New System.Windows.Forms.TextBox
        Me.CheckForPortsButton = New System.Windows.Forms.Button
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ClearButton
        '
        Me.ClearButton.AccessibleDescription = "Button to clear the status text box."
        Me.ClearButton.AccessibleName = "Clear Button"
        Me.ClearButton.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.ClearButton.Location = New System.Drawing.Point(416, 299)
        Me.ClearButton.Name = "ClearButton"
        Me.ClearButton.Size = New System.Drawing.Size(104, 24)
        Me.ClearButton.TabIndex = 26
        Me.ClearButton.Text = "&Clear"
        '
        'tmrReadCommPort
        '
        '
        'UserCommandTextbox
        '
        Me.UserCommandTextbox.AccessibleDescription = "TextBox to enter user command."
        Me.UserCommandTextbox.AccessibleName = "User Command TextBox"
        Me.UserCommandTextbox.Enabled = False
        Me.UserCommandTextbox.Location = New System.Drawing.Point(8, 299)
        Me.UserCommandTextbox.Name = "UserCommandTextbox"
        Me.UserCommandTextbox.Size = New System.Drawing.Size(168, 20)
        Me.UserCommandTextbox.TabIndex = 25
        '
        'SendUserCommandButton
        '
        Me.SendUserCommandButton.AccessibleDescription = "Button to send an user command to the modem."
        Me.SendUserCommandButton.AccessibleName = "Send User Command Button"
        Me.SendUserCommandButton.Enabled = False
        Me.SendUserCommandButton.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.SendUserCommandButton.Location = New System.Drawing.Point(8, 267)
        Me.SendUserCommandButton.Name = "SendUserCommandButton"
        Me.SendUserCommandButton.Size = New System.Drawing.Size(168, 23)
        Me.SendUserCommandButton.TabIndex = 24
        Me.SendUserCommandButton.Text = "Send &User Command"
        '
        'SendATCommandButton
        '
        Me.SendATCommandButton.AccessibleDescription = "Button to send an AT command to the modem."
        Me.SendATCommandButton.AccessibleName = "Send AT Command Button"
        Me.SendATCommandButton.Enabled = False
        Me.SendATCommandButton.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.SendATCommandButton.Location = New System.Drawing.Point(8, 235)
        Me.SendATCommandButton.Name = "SendATCommandButton"
        Me.SendATCommandButton.Size = New System.Drawing.Size(168, 23)
        Me.SendATCommandButton.TabIndex = 23
        Me.SendATCommandButton.Text = "Send &AT Command"
        '
        'SelectedModemTextbox
        '
        Me.SelectedModemTextbox.AccessibleDescription = "TextBox displaying Selected modem."
        Me.SelectedModemTextbox.AccessibleName = "Selected Modem CheckBox"
        Me.SelectedModemTextbox.Enabled = False
        Me.SelectedModemTextbox.Location = New System.Drawing.Point(8, 195)
        Me.SelectedModemTextbox.Name = "SelectedModemTextbox"
        Me.SelectedModemTextbox.Size = New System.Drawing.Size(168, 20)
        Me.SelectedModemTextbox.TabIndex = 22
        Me.SelectedModemTextbox.Text = "No Modem Selected"
        '
        'CheckModemsButton
        '
        Me.CheckModemsButton.AccessibleDescription = "Button to check available modems."
        Me.CheckModemsButton.AccessibleName = "Check Modems Button"
        Me.CheckModemsButton.Enabled = False
        Me.CheckModemsButton.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckModemsButton.Location = New System.Drawing.Point(8, 163)
        Me.CheckModemsButton.Name = "CheckModemsButton"
        Me.CheckModemsButton.Size = New System.Drawing.Size(168, 23)
        Me.CheckModemsButton.TabIndex = 21
        Me.CheckModemsButton.Text = "Check for &Modems"
        '
        'PortsList
        '
        Me.PortsList.AccessibleDescription = "Checked ListBox to display the 4 main COM ports."
        Me.PortsList.AccessibleName = "ComPorts CheckedListBox"
        Me.PortsList.Enabled = False
        Me.PortsList.FormattingEnabled = True
        Me.PortsList.Items.AddRange(New Object() {"COM1", "COM2", "COM3", "COM4"})
        Me.PortsList.Location = New System.Drawing.Point(8, 67)
        Me.PortsList.Name = "PortsList"
        Me.PortsList.Size = New System.Drawing.Size(168, 49)
        Me.PortsList.TabIndex = 20
        Me.PortsList.TabStop = False
        '
        'StatusTextbox
        '
        Me.StatusTextbox.AccessibleDescription = "TextBox to display information and status."
        Me.StatusTextbox.AccessibleName = "Status TextBox"
        Me.StatusTextbox.Location = New System.Drawing.Point(216, 35)
        Me.StatusTextbox.Multiline = True
        Me.StatusTextbox.Name = "StatusTextbox"
        Me.StatusTextbox.ReadOnly = True
        Me.StatusTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.StatusTextbox.Size = New System.Drawing.Size(304, 256)
        Me.StatusTextbox.TabIndex = 19
        '
        'CheckForPortsButton
        '
        Me.CheckForPortsButton.AccessibleDescription = "Button to check available ports."
        Me.CheckForPortsButton.AccessibleName = "Check Ports Button"
        Me.CheckForPortsButton.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.CheckForPortsButton.Location = New System.Drawing.Point(8, 35)
        Me.CheckForPortsButton.Name = "CheckForPortsButton"
        Me.CheckForPortsButton.Size = New System.Drawing.Size(168, 23)
        Me.CheckForPortsButton.TabIndex = 18
        Me.CheckForPortsButton.Text = "Check for &Ports"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(528, 24)
        Me.MenuStrip1.TabIndex = 27
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
        Me.ClientSize = New System.Drawing.Size(528, 338)
        Me.Controls.Add(Me.SendUserCommandButton)
        Me.Controls.Add(Me.SendATCommandButton)
        Me.Controls.Add(Me.SelectedModemTextbox)
        Me.Controls.Add(Me.CheckModemsButton)
        Me.Controls.Add(Me.PortsList)
        Me.Controls.Add(Me.StatusTextbox)
        Me.Controls.Add(Me.CheckForPortsButton)
        Me.Controls.Add(Me.ClearButton)
        Me.Controls.Add(Me.UserCommandTextbox)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Name = "MainForm"
        Me.Text = "Main Form Sample"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ClearButton As System.Windows.Forms.Button
    Friend WithEvents tmrReadCommPort As System.Windows.Forms.Timer
    Friend WithEvents UserCommandTextbox As System.Windows.Forms.TextBox
    Friend WithEvents SendUserCommandButton As System.Windows.Forms.Button
    Friend WithEvents SendATCommandButton As System.Windows.Forms.Button
    Friend WithEvents SelectedModemTextbox As System.Windows.Forms.TextBox
    Friend WithEvents CheckModemsButton As System.Windows.Forms.Button
    Friend WithEvents PortsList As System.Windows.Forms.CheckedListBox
    Friend WithEvents StatusTextbox As System.Windows.Forms.TextBox
    Friend WithEvents CheckForPortsButton As System.Windows.Forms.Button
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
