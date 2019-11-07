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
        Me.btnCommandLine = New System.Windows.Forms.Button
        Me.btnShellExecute = New System.Windows.Forms.Button
        Me.DisplayText = New System.Windows.Forms.RichTextBox
        Me.btnTaskManager = New System.Windows.Forms.Button
        Me.btnCurrentProcessInfo = New System.Windows.Forms.Button
        Me.btnProcessStartInfo = New System.Windows.Forms.Button
        Me.btnStartProcess = New System.Windows.Forms.Button
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnCommandLine
        '
        Me.btnCommandLine.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnCommandLine.Location = New System.Drawing.Point(24, 209)
        Me.btnCommandLine.Name = "btnCommandLine"
        Me.btnCommandLine.Size = New System.Drawing.Size(160, 23)
        Me.btnCommandLine.TabIndex = 13
        Me.btnCommandLine.Text = "Command Line Arguments"
        '
        'btnShellExecute
        '
        Me.btnShellExecute.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnShellExecute.Location = New System.Drawing.Point(24, 177)
        Me.btnShellExecute.Name = "btnShellExecute"
        Me.btnShellExecute.Size = New System.Drawing.Size(160, 23)
        Me.btnShellExecute.TabIndex = 12
        Me.btnShellExecute.Text = "Shell Execute"
        '
        'DisplayText
        '
        Me.DisplayText.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DisplayText.Location = New System.Drawing.Point(208, 49)
        Me.DisplayText.Name = "DisplayText"
        Me.DisplayText.Size = New System.Drawing.Size(239, 220)
        Me.DisplayText.TabIndex = 11
        Me.DisplayText.Text = ""
        '
        'btnTaskManager
        '
        Me.btnTaskManager.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnTaskManager.Location = New System.Drawing.Point(24, 145)
        Me.btnTaskManager.Name = "btnTaskManager"
        Me.btnTaskManager.Size = New System.Drawing.Size(160, 23)
        Me.btnTaskManager.TabIndex = 10
        Me.btnTaskManager.Text = "Task Manager"
        '
        'btnCurrentProcessInfo
        '
        Me.btnCurrentProcessInfo.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnCurrentProcessInfo.Location = New System.Drawing.Point(24, 49)
        Me.btnCurrentProcessInfo.Name = "btnCurrentProcessInfo"
        Me.btnCurrentProcessInfo.Size = New System.Drawing.Size(160, 23)
        Me.btnCurrentProcessInfo.TabIndex = 9
        Me.btnCurrentProcessInfo.Text = "CurrentProcessInfo"
        '
        'btnProcessStartInfo
        '
        Me.btnProcessStartInfo.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnProcessStartInfo.Location = New System.Drawing.Point(24, 113)
        Me.btnProcessStartInfo.Name = "btnProcessStartInfo"
        Me.btnProcessStartInfo.Size = New System.Drawing.Size(160, 23)
        Me.btnProcessStartInfo.TabIndex = 8
        Me.btnProcessStartInfo.Text = "Process StartInfo"
        '
        'btnStartProcess
        '
        Me.btnStartProcess.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnStartProcess.Location = New System.Drawing.Point(24, 81)
        Me.btnStartProcess.Name = "btnStartProcess"
        Me.btnStartProcess.Size = New System.Drawing.Size(160, 23)
        Me.btnStartProcess.TabIndex = 7
        Me.btnStartProcess.Text = "Starting a Process"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(473, 24)
        Me.MenuStrip1.TabIndex = 14
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
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(473, 281)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.btnCommandLine)
        Me.Controls.Add(Me.btnShellExecute)
        Me.Controls.Add(Me.DisplayText)
        Me.Controls.Add(Me.btnTaskManager)
        Me.Controls.Add(Me.btnCurrentProcessInfo)
        Me.Controls.Add(Me.btnProcessStartInfo)
        Me.Controls.Add(Me.btnStartProcess)
        Me.Name = "MainForm"
        Me.Text = "Process Sample"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend Shared ReadOnly Property GetInstance() As MainForm
        Get
            If m_DefaultInstance Is Nothing OrElse m_DefaultInstance.IsDisposed() Then
                SyncLock m_SyncObject
                    If m_DefaultInstance Is Nothing OrElse m_DefaultInstance.IsDisposed() Then
                        m_DefaultInstance = New MainForm
                    End If
                End SyncLock
            End If
            Return m_DefaultInstance
        End Get
    End Property

    Private Shared m_DefaultInstance As MainForm
    Private Shared m_SyncObject As New Object
    Friend WithEvents btnCommandLine As System.Windows.Forms.Button
    Friend WithEvents btnShellExecute As System.Windows.Forms.Button
    Friend WithEvents DisplayText As System.Windows.Forms.RichTextBox
    Friend WithEvents btnTaskManager As System.Windows.Forms.Button
    Friend WithEvents btnCurrentProcessInfo As System.Windows.Forms.Button
    Friend WithEvents btnProcessStartInfo As System.Windows.Forms.Button
    Friend WithEvents btnStartProcess As System.Windows.Forms.Button
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
