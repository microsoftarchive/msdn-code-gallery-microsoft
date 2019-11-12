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
        Me.ThreadID = New System.Windows.Forms.Label
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.BackgroundWorker = New System.Windows.Forms.Button
        Me.ThreadPool = New System.Windows.Forms.Button
        Me.SameThread = New System.Windows.Forms.Button
        Me.Worker = New System.ComponentModel.BackgroundWorker
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.GroupBox1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ThreadID
        '
        Me.ThreadID.AutoSize = True
        Me.ThreadID.Location = New System.Drawing.Point(13, 35)
        Me.ThreadID.Name = "ThreadID"
        Me.ThreadID.Size = New System.Drawing.Size(201, 13)
        Me.ThreadID.TabIndex = 4
        Me.ThreadID.Text = "This window is being serviced by thread: "
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.BackgroundWorker)
        Me.GroupBox1.Controls.Add(Me.ThreadPool)
        Me.GroupBox1.Controls.Add(Me.SameThread)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 56)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(242, 128)
        Me.GroupBox1.TabIndex = 5
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Execute a long running task"
        '
        'BackgroundWorker
        '
        Me.BackgroundWorker.Location = New System.Drawing.Point(16, 59)
        Me.BackgroundWorker.Name = "BackgroundWorker"
        Me.BackgroundWorker.Size = New System.Drawing.Size(213, 23)
        Me.BackgroundWorker.TabIndex = 2
        Me.BackgroundWorker.Text = "Run using &Background Worker"
        '
        'ThreadPool
        '
        Me.ThreadPool.Location = New System.Drawing.Point(16, 89)
        Me.ThreadPool.Name = "ThreadPool"
        Me.ThreadPool.Size = New System.Drawing.Size(213, 23)
        Me.ThreadPool.TabIndex = 1
        Me.ThreadPool.Text = "Run on &worker pool thread"
        '
        'SameThread
        '
        Me.SameThread.Location = New System.Drawing.Point(16, 29)
        Me.SameThread.Name = "SameThread"
        Me.SameThread.Size = New System.Drawing.Size(213, 23)
        Me.SameThread.TabIndex = 0
        Me.SameThread.Text = "Run on &same thread"
        '
        'Worker
        '
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(270, 24)
        Me.MenuStrip1.TabIndex = 6
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
        Me.ClientSize = New System.Drawing.Size(270, 199)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.ThreadID)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Threading Sample"
        Me.GroupBox1.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ThreadID As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents SameThread As System.Windows.Forms.Button
    Friend WithEvents ThreadPool As System.Windows.Forms.Button
    Friend WithEvents BackgroundWorker As System.Windows.Forms.Button
    Friend WithEvents Worker As System.ComponentModel.BackgroundWorker
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
