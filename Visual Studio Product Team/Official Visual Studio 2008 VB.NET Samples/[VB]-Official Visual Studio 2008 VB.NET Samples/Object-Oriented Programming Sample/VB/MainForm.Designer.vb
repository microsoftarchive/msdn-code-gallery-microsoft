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
        Me.cmdSharedMembers = New System.Windows.Forms.Button
        Me.cmdParamProperties = New System.Windows.Forms.Button
        Me.cmdPropertySyntax = New System.Windows.Forms.Button
        Me.cmdConstructors = New System.Windows.Forms.Button
        Me.cmdOverloads = New System.Windows.Forms.Button
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmdSharedMembers
        '
        Me.cmdSharedMembers.Location = New System.Drawing.Point(12, 208)
        Me.cmdSharedMembers.Name = "cmdSharedMembers"
        Me.cmdSharedMembers.Size = New System.Drawing.Size(336, 25)
        Me.cmdSharedMembers.TabIndex = 9
        Me.cmdSharedMembers.Text = "Shared &Members"
        '
        'cmdParamProperties
        '
        Me.cmdParamProperties.Location = New System.Drawing.Point(12, 168)
        Me.cmdParamProperties.Name = "cmdParamProperties"
        Me.cmdParamProperties.Size = New System.Drawing.Size(336, 25)
        Me.cmdParamProperties.TabIndex = 8
        Me.cmdParamProperties.Text = "&Parameterized Properties"
        '
        'cmdPropertySyntax
        '
        Me.cmdPropertySyntax.Location = New System.Drawing.Point(12, 128)
        Me.cmdPropertySyntax.Name = "cmdPropertySyntax"
        Me.cmdPropertySyntax.Size = New System.Drawing.Size(336, 25)
        Me.cmdPropertySyntax.TabIndex = 7
        Me.cmdPropertySyntax.Text = "Property &Syntax"
        '
        'cmdConstructors
        '
        Me.cmdConstructors.Location = New System.Drawing.Point(12, 88)
        Me.cmdConstructors.Name = "cmdConstructors"
        Me.cmdConstructors.Size = New System.Drawing.Size(336, 25)
        Me.cmdConstructors.TabIndex = 6
        Me.cmdConstructors.Text = "&Constructors"
        '
        'cmdOverloads
        '
        Me.cmdOverloads.Location = New System.Drawing.Point(12, 48)
        Me.cmdOverloads.Name = "cmdOverloads"
        Me.cmdOverloads.Size = New System.Drawing.Size(336, 25)
        Me.cmdOverloads.TabIndex = 5
        Me.cmdOverloads.Text = "&Overloads"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(371, 24)
        Me.MenuStrip1.TabIndex = 10
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
        Me.ClientSize = New System.Drawing.Size(371, 250)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.cmdParamProperties)
        Me.Controls.Add(Me.cmdPropertySyntax)
        Me.Controls.Add(Me.cmdConstructors)
        Me.Controls.Add(Me.cmdOverloads)
        Me.Controls.Add(Me.cmdSharedMembers)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "MainForm"
        Me.Text = "Object-Oriented Programming With VB.NET Sample"
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
    Friend WithEvents cmdSharedMembers As System.Windows.Forms.Button
    Friend WithEvents cmdParamProperties As System.Windows.Forms.Button
    Friend WithEvents cmdPropertySyntax As System.Windows.Forms.Button
    Friend WithEvents cmdConstructors As System.Windows.Forms.Button
    Friend WithEvents cmdOverloads As System.Windows.Forms.Button
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
