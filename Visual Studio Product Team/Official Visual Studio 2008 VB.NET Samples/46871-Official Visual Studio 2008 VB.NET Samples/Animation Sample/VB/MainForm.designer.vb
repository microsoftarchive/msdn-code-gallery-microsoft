Partial Public Class MainForm
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
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
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.Label1 = New System.Windows.Forms.Label
        Me.optWink = New System.Windows.Forms.RadioButton
        Me.optBall = New System.Windows.Forms.RadioButton
        Me.optText = New System.Windows.Forms.RadioButton
        Me.tmrAnimation = New System.Windows.Forms.Timer(Me.components)
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(20, 42)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(152, 23)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "Animation Examples:"
        '
        'optWink
        '
        Me.optWink.AccessibleDescription = "RadioButton with text ""Winking Eye"""
        Me.optWink.AccessibleName = "Winking Eye"
        Me.optWink.Checked = True
        Me.optWink.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.optWink.Location = New System.Drawing.Point(178, 42)
        Me.optWink.Name = "optWink"
        Me.optWink.Size = New System.Drawing.Size(113, 24)
        Me.optWink.TabIndex = 11
        Me.optWink.Text = "&Winking Eye"
        '
        'optBall
        '
        Me.optBall.AccessibleDescription = "RadioButton with text ""Bouncing Ball"""
        Me.optBall.AccessibleName = "Bouncing Ball"
        Me.optBall.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.optBall.Location = New System.Drawing.Point(278, 42)
        Me.optBall.Name = "optBall"
        Me.optBall.Size = New System.Drawing.Size(103, 24)
        Me.optBall.TabIndex = 13
        Me.optBall.Text = "&Bouncing Ball"
        '
        'optText
        '
        Me.optText.AccessibleDescription = "RadioButton with text ""Animated Text"""
        Me.optText.AccessibleName = "Animated Text"
        Me.optText.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.optText.Location = New System.Drawing.Point(387, 42)
        Me.optText.Name = "optText"
        Me.optText.Size = New System.Drawing.Size(117, 24)
        Me.optText.TabIndex = 14
        Me.optText.Text = "&Animated Text"
        '
        'tmrAnimation
        '
        Me.tmrAnimation.Enabled = True
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(566, 24)
        Me.MenuStrip1.TabIndex = 15
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
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.ClientSize = New System.Drawing.Size(566, 410)
        Me.Controls.Add(Me.optBall)
        Me.Controls.Add(Me.optText)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.optWink)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Name = "MainForm"
        Me.Text = "GDI Animation Sample"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents optWink As System.Windows.Forms.RadioButton
    Friend WithEvents optBall As System.Windows.Forms.RadioButton
    Friend WithEvents optText As System.Windows.Forms.RadioButton
    Friend WithEvents tmrAnimation As System.Windows.Forms.Timer
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
