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
        Me.ThreadButton = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.UIButton = New System.Windows.Forms.Button
        Me.Label2 = New System.Windows.Forms.Label
        Me.StartTimer = New System.Windows.Forms.Button
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ThreadButton
        '
        Me.ThreadButton.Location = New System.Drawing.Point(50, 94)
        Me.ThreadButton.Name = "ThreadButton"
        Me.ThreadButton.Size = New System.Drawing.Size(202, 23)
        Me.ThreadButton.TabIndex = 0
        Me.ThreadButton.Text = "Update Using New Thread"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(50, 65)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(41, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Update"
        '
        'UIButton
        '
        Me.UIButton.Location = New System.Drawing.Point(50, 123)
        Me.UIButton.Name = "UIButton"
        Me.UIButton.Size = New System.Drawing.Size(202, 23)
        Me.UIButton.TabIndex = 2
        Me.UIButton.Text = "Update Using UI Thread"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(50, 45)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(132, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "The UI Thread Number is: "
        '
        'StartTimer
        '
        Me.StartTimer.Location = New System.Drawing.Point(50, 152)
        Me.StartTimer.Name = "StartTimer"
        Me.StartTimer.Size = New System.Drawing.Size(202, 48)
        Me.StartTimer.TabIndex = 4
        Me.StartTimer.Text = "Update UI Using a Timer " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "5 Times Every 2 Seconds"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(314, 26)
        Me.MenuStrip1.TabIndex = 5
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
        Me.ClientSize = New System.Drawing.Size(314, 215)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.StartTimer)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.UIButton)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ThreadButton)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "Form1"
        Me.Text = "UI Changes from Thread Sample"
        Me.MenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ThreadButton As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents UIButton As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents StartTimer As System.Windows.Forms.Button
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
