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
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.resetDisplayMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.xmlTasks = New System.Windows.Forms.CheckedListBox
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer
        Me.xmlDisplay = New System.Windows.Forms.TextBox
        Me.xmlEdit = New System.Windows.Forms.TextBox
        Me.MenuStrip1.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 459)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(643, 22)
        Me.StatusStrip1.TabIndex = 0
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem, Me.resetDisplayMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(643, 24)
        Me.MenuStrip1.TabIndex = 1
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
        'resetDisplayMenuItem
        '
        Me.resetDisplayMenuItem.Name = "resetDisplayMenuItem"
        Me.resetDisplayMenuItem.Size = New System.Drawing.Size(96, 20)
        Me.resetDisplayMenuItem.Text = "&Reset Display"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 24)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.xmlTasks)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer2)
        Me.SplitContainer1.Size = New System.Drawing.Size(643, 435)
        Me.SplitContainer1.SplitterDistance = 215
        Me.SplitContainer1.TabIndex = 2
        Me.SplitContainer1.Text = "SplitContainer1"
        '
        'xmlTasks
        '
        Me.xmlTasks.Dock = System.Windows.Forms.DockStyle.Fill
        Me.xmlTasks.FormattingEnabled = True
        Me.xmlTasks.Location = New System.Drawing.Point(0, 0)
        Me.xmlTasks.Name = "xmlTasks"
        Me.xmlTasks.Size = New System.Drawing.Size(215, 424)
        Me.xmlTasks.TabIndex = 0
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.xmlDisplay)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.xmlEdit)
        Me.SplitContainer2.Size = New System.Drawing.Size(424, 435)
        Me.SplitContainer2.SplitterDistance = 142
        Me.SplitContainer2.TabIndex = 0
        Me.SplitContainer2.Text = "SplitContainer2"
        '
        'xmlDisplay
        '
        Me.xmlDisplay.Dock = System.Windows.Forms.DockStyle.Fill
        Me.xmlDisplay.Location = New System.Drawing.Point(0, 0)
        Me.xmlDisplay.Multiline = True
        Me.xmlDisplay.Name = "xmlDisplay"
        Me.xmlDisplay.Size = New System.Drawing.Size(424, 142)
        Me.xmlDisplay.TabIndex = 0
        '
        'xmlEdit
        '
        Me.xmlEdit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.xmlEdit.Location = New System.Drawing.Point(0, 0)
        Me.xmlEdit.Multiline = True
        Me.xmlEdit.Name = "xmlEdit"
        Me.xmlEdit.Size = New System.Drawing.Size(424, 289)
        Me.xmlEdit.TabIndex = 0
        '
        'Form1
        '
        Me.ClientSize = New System.Drawing.Size(643, 481)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.PerformLayout()
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        Me.SplitContainer2.Panel2.PerformLayout()
        Me.SplitContainer2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
    Friend WithEvents xmlTasks As System.Windows.Forms.CheckedListBox
    Friend WithEvents xmlDisplay As System.Windows.Forms.TextBox
    Friend WithEvents xmlEdit As System.Windows.Forms.TextBox
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents resetDisplayMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
