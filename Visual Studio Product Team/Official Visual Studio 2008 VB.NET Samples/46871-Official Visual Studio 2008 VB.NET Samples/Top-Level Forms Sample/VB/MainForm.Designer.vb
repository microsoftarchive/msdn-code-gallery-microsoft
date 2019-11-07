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
        Me.mnuExit = New System.Windows.Forms.MenuItem
        Me.MenuItem2 = New System.Windows.Forms.MenuItem
        Me.mnuSave = New System.Windows.Forms.MenuItem
        Me.sbDocInfo = New System.Windows.Forms.StatusBar
        Me.mnuFile = New System.Windows.Forms.MenuItem
        Me.mnuNew = New System.Windows.Forms.MenuItem
        Me.mnuClose = New System.Windows.Forms.MenuItem
        Me.MenuItem1 = New System.Windows.Forms.MenuItem
        Me.mnuMain = New System.Windows.Forms.MainMenu(Me.components)
        Me.txtData = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'mnuExit
        '
        Me.mnuExit.Index = 5
        Me.mnuExit.Text = "E&xit"
        '
        'MenuItem2
        '
        Me.MenuItem2.Index = 4
        Me.MenuItem2.Text = "-"
        '
        'mnuSave
        '
        Me.mnuSave.Index = 3
        Me.mnuSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS
        Me.mnuSave.Text = "&Save"
        '
        'sbDocInfo
        '
        Me.sbDocInfo.Location = New System.Drawing.Point(9, 222)
        Me.sbDocInfo.Name = "sbDocInfo"
        Me.sbDocInfo.Size = New System.Drawing.Size(438, 22)
        Me.sbDocInfo.TabIndex = 2
        Me.sbDocInfo.Text = "Ready"
        '
        'mnuFile
        '
        Me.mnuFile.Index = 0
        Me.mnuFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuNew, Me.mnuClose, Me.MenuItem1, Me.mnuSave, Me.MenuItem2, Me.mnuExit})
        Me.mnuFile.Text = "&File"
        '
        'mnuNew
        '
        Me.mnuNew.Index = 0
        Me.mnuNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN
        Me.mnuNew.Text = "&New"
        '
        'mnuClose
        '
        Me.mnuClose.Index = 1
        Me.mnuClose.Text = "&Close"
        '
        'MenuItem1
        '
        Me.MenuItem1.Index = 2
        Me.MenuItem1.Text = "-"
        '
        'mnuMain
        '
        Me.mnuMain.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuFile})
        '
        'txtData
        '
        Me.txtData.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtData.Location = New System.Drawing.Point(9, 9)
        Me.txtData.Multiline = True
        Me.txtData.Name = "txtData"
        Me.txtData.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtData.Size = New System.Drawing.Size(438, 213)
        Me.txtData.TabIndex = 3
        '
        'MainForm
        '
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(456, 253)
        Me.Controls.Add(Me.txtData)
        Me.Controls.Add(Me.sbDocInfo)
        Me.Menu = Me.mnuMain
        Me.Name = "MainForm"
        Me.Padding = New System.Windows.Forms.Padding(9)
        Me.Text = "Managing Top-Level Forms Sample"
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
    Friend WithEvents mnuExit As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem2 As System.Windows.Forms.MenuItem
    Friend WithEvents mnuSave As System.Windows.Forms.MenuItem
    Friend WithEvents sbDocInfo As System.Windows.Forms.StatusBar
    Friend WithEvents mnuFile As System.Windows.Forms.MenuItem
    Friend WithEvents mnuNew As System.Windows.Forms.MenuItem
    Friend WithEvents mnuClose As System.Windows.Forms.MenuItem
    Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
    Friend WithEvents mnuMain As System.Windows.Forms.MainMenu
    Friend WithEvents txtData As System.Windows.Forms.TextBox

End Class
