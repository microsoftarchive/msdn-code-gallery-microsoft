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
        Me.DatePanelCheckBox = New System.Windows.Forms.CheckBox
        Me.CapsLockpanelCheckBox = New System.Windows.Forms.CheckBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.ShowTextPanelCheckBox = New System.Windows.Forms.CheckBox
        Me.NumLockPanelCheckBox = New System.Windows.Forms.CheckBox
        Me.ProgressBarPanelCheckBox = New System.Windows.Forms.CheckBox
        Me.ShowPanelsCheckBox = New System.Windows.Forms.CheckBox
        Me.TextBox1 = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.status = New System.Windows.Forms.ToolStripStatusLabel
        Me.numLock = New System.Windows.Forms.ToolStripStatusLabel
        Me.capsLock = New System.Windows.Forms.ToolStripStatusLabel
        Me.dateLabel = New System.Windows.Forms.ToolStripStatusLabel
        Me.progressBarStrip = New System.Windows.Forms.ToolStripProgressBar
        Me.GroupBox1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'DatePanelCheckBox
        '
        Me.DatePanelCheckBox.Checked = True
        Me.DatePanelCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.DatePanelCheckBox.Location = New System.Drawing.Point(10, 117)
        Me.DatePanelCheckBox.Name = "DatePanelCheckBox"
        Me.DatePanelCheckBox.Size = New System.Drawing.Size(349, 17)
        Me.DatePanelCheckBox.TabIndex = 7
        Me.DatePanelCheckBox.Text = "Date - Autosized based on the contents of this panel."
        '
        'CapsLockpanelCheckBox
        '
        Me.CapsLockpanelCheckBox.Checked = True
        Me.CapsLockpanelCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CapsLockpanelCheckBox.Location = New System.Drawing.Point(10, 94)
        Me.CapsLockpanelCheckBox.Name = "CapsLockpanelCheckBox"
        Me.CapsLockpanelCheckBox.Size = New System.Drawing.Size(334, 17)
        Me.CapsLockpanelCheckBox.TabIndex = 6
        Me.CapsLockpanelCheckBox.Text = "Caps Locked - Fixed sized."
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.ShowTextPanelCheckBox)
        Me.GroupBox1.Controls.Add(Me.DatePanelCheckBox)
        Me.GroupBox1.Controls.Add(Me.CapsLockpanelCheckBox)
        Me.GroupBox1.Controls.Add(Me.NumLockPanelCheckBox)
        Me.GroupBox1.Controls.Add(Me.ProgressBarPanelCheckBox)
        Me.GroupBox1.Enabled = False
        Me.GroupBox1.Location = New System.Drawing.Point(8, 96)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(364, 140)
        Me.GroupBox1.TabIndex = 20
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Statusbar Panels"
        '
        'ShowTextPanelCheckBox
        '
        Me.ShowTextPanelCheckBox.Checked = True
        Me.ShowTextPanelCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ShowTextPanelCheckBox.Location = New System.Drawing.Point(10, 24)
        Me.ShowTextPanelCheckBox.Name = "ShowTextPanelCheckBox"
        Me.ShowTextPanelCheckBox.Size = New System.Drawing.Size(341, 17)
        Me.ShowTextPanelCheckBox.TabIndex = 8
        Me.ShowTextPanelCheckBox.Text = "Status Text - AutoSize property is set to spring."
        '
        'NumLockPanelCheckBox
        '
        Me.NumLockPanelCheckBox.Checked = True
        Me.NumLockPanelCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.NumLockPanelCheckBox.Location = New System.Drawing.Point(9, 71)
        Me.NumLockPanelCheckBox.Name = "NumLockPanelCheckBox"
        Me.NumLockPanelCheckBox.Size = New System.Drawing.Size(341, 17)
        Me.NumLockPanelCheckBox.TabIndex = 5
        Me.NumLockPanelCheckBox.Text = "Num Lock - Fixed sized."
        '
        'ProgressBarPanelCheckBox
        '
        Me.ProgressBarPanelCheckBox.Location = New System.Drawing.Point(10, 46)
        Me.ProgressBarPanelCheckBox.Name = "ProgressBarPanelCheckBox"
        Me.ProgressBarPanelCheckBox.Size = New System.Drawing.Size(251, 18)
        Me.ProgressBarPanelCheckBox.TabIndex = 4
        Me.ProgressBarPanelCheckBox.Text = "Progress - Ownerdrawn via the progressbar."
        '
        'ShowPanelsCheckBox
        '
        Me.ShowPanelsCheckBox.Location = New System.Drawing.Point(12, 71)
        Me.ShowPanelsCheckBox.Name = "ShowPanelsCheckBox"
        Me.ShowPanelsCheckBox.Size = New System.Drawing.Size(246, 18)
        Me.ShowPanelsCheckBox.TabIndex = 19
        Me.ShowPanelsCheckBox.Text = "Show Statusbar Panels"
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(97, 38)
        Me.TextBox1.MaxLength = 256
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(275, 20)
        Me.TextBox1.TabIndex = 21
        Me.TextBox1.Text = "This sample shows how to use various features of the statusbar control."
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(8, 40)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(85, 18)
        Me.Label1.TabIndex = 22
        Me.Label1.Text = "Statusbar Text:"
        '
        'Timer1
        '
        Me.Timer1.Interval = 200
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(395, 24)
        Me.MenuStrip1.TabIndex = 23
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
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.status, Me.numLock, Me.capsLock, Me.dateLabel, Me.progressBarStrip})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 305)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(395, 25)
        Me.StatusStrip1.TabIndex = 24
        '
        'status
        '
        Me.status.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.status.Name = "status"
        Me.status.Size = New System.Drawing.Size(73, 20)
        Me.status.Text = "Status Text"
        '
        'numLock
        '
        Me.numLock.AutoSize = False
        Me.numLock.Name = "numLock"
        Me.numLock.Size = New System.Drawing.Size(50, 20)
        Me.numLock.Text = "NumLock"
        '
        'capsLock
        '
        Me.capsLock.AutoSize = False
        Me.capsLock.Name = "capsLock"
        Me.capsLock.Size = New System.Drawing.Size(50, 20)
        Me.capsLock.Text = "CapsLock"
        '
        'dateLabel
        '
        Me.dateLabel.Name = "dateLabel"
        Me.dateLabel.Size = New System.Drawing.Size(34, 20)
        Me.dateLabel.Text = "Date"
        '
        'progressBarStrip
        '
        Me.progressBarStrip.Name = "progressBarStrip"
        Me.progressBarStrip.Size = New System.Drawing.Size(100, 18)
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(395, 330)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.ShowPanelsCheckBox)
        Me.Name = "MainForm"
        Me.Text = "Status Strip Sample"
        Me.GroupBox1.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DatePanelCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents CapsLockpanelCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents ShowTextPanelCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents NumLockPanelCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents ProgressBarPanelCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents ShowPanelsCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents status As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents numLock As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents capsLock As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents dateLabel As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents progressBarStrip As System.Windows.Forms.ToolStripProgressBar

End Class
