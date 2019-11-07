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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.Label1 = New System.Windows.Forms.Label
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.currentDateMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.timeZoneMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.restartMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.frameworkMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.osMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.Label2 = New System.Windows.Forms.Label
        Me.btnTray = New System.Windows.Forms.Button
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ContextMenuStrip1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = "Label for form title"
        Me.Label1.AccessibleName = "Title Label"
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold)
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(62, 47)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(376, 48)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "System Information Tray Icon"
        '
        'NotifyIcon1
        '
        Me.NotifyIcon1.BalloonTipText = Nothing
        Me.NotifyIcon1.BalloonTipTitle = Nothing
        Me.NotifyIcon1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.NotifyIcon1.Icon = CType(resources.GetObject("NotifyIcon1.Icon"), System.Drawing.Icon)
        Me.NotifyIcon1.Text = "NotifyIcon1"
        Me.NotifyIcon1.Visible = True
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.currentDateMenuItem, Me.timeZoneMenuItem, Me.restartMenuItem, Me.frameworkMenuItem, Me.osMenuItem, Me.exitMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(226, 136)
        '
        'currentDateMenuItem
        '
        Me.currentDateMenuItem.Name = "currentDateMenuItem"
        Me.currentDateMenuItem.Size = New System.Drawing.Size(225, 22)
        Me.currentDateMenuItem.Text = "Current &Date"
        '
        'timeZoneMenuItem
        '
        Me.timeZoneMenuItem.Name = "timeZoneMenuItem"
        Me.timeZoneMenuItem.Size = New System.Drawing.Size(225, 22)
        Me.timeZoneMenuItem.Text = "Current &Time Zone"
        '
        'restartMenuItem
        '
        Me.restartMenuItem.Name = "restartMenuItem"
        Me.restartMenuItem.Size = New System.Drawing.Size(225, 22)
        Me.restartMenuItem.Text = "Time Since Last &Restart"
        '
        'frameworkMenuItem
        '
        Me.frameworkMenuItem.Name = "frameworkMenuItem"
        Me.frameworkMenuItem.Size = New System.Drawing.Size(225, 22)
        Me.frameworkMenuItem.Text = "&Framework Version"
        '
        'osMenuItem
        '
        Me.osMenuItem.Name = "osMenuItem"
        Me.osMenuItem.Size = New System.Drawing.Size(225, 22)
        Me.osMenuItem.Text = "Current &OS Version"
        '
        'exitMenuItem
        '
        Me.exitMenuItem.Name = "exitMenuItem"
        Me.exitMenuItem.Size = New System.Drawing.Size(225, 22)
        Me.exitMenuItem.Text = "E&xit"
        '
        'Label2
        '
        Me.Label2.AccessibleDescription = "Label showing the instructions for using the form."
        Me.Label2.AccessibleName = "Instructions Label"
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label2.Location = New System.Drawing.Point(62, 111)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(368, 48)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Click the ""Tray"" button to place an icon in the system tray.  The user can then d" & _
            "ouble-click the tray icon to re-show this form, or right-click on the tray icon " & _
            "to get system information."
        '
        'btnTray
        '
        Me.btnTray.AccessibleDescription = "Press this button to to place an icon in the system tray."
        Me.btnTray.AccessibleName = "Tray Button"
        Me.btnTray.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnTray.Location = New System.Drawing.Point(390, 215)
        Me.btnTray.Name = "btnTray"
        Me.btnTray.Size = New System.Drawing.Size(75, 23)
        Me.btnTray.TabIndex = 8
        Me.btnTray.Text = "&Tray"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(527, 24)
        Me.MenuStrip1.TabIndex = 9
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
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(527, 284)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.btnTray)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "MainForm"
        Me.Text = "Tray Application Sample"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents NotifyIcon1 As System.Windows.Forms.NotifyIcon
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnTray As System.Windows.Forms.Button
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContextMenuStrip1 As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents currentDateMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents timeZoneMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents restartMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents frameworkMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents osMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
