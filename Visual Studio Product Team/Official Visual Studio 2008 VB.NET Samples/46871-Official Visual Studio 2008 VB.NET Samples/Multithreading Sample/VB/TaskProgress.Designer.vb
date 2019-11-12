Public Partial Class TaskProgress
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
        Me.ProgressIndicator = New System.Windows.Forms.ProgressBar
        Me.label1 = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'ThreadID
        '
        Me.ThreadID.AutoSize = True
        Me.ThreadID.Location = New System.Drawing.Point(13, 13)
        Me.ThreadID.Name = "ThreadID"
        Me.ThreadID.Size = New System.Drawing.Size(207, 14)
        Me.ThreadID.TabIndex = 0
        Me.ThreadID.Text = "This window is being serviced by thread: "
        '
        'ProgressIndicator
        '
        Me.ProgressIndicator.Location = New System.Drawing.Point(13, 34)
        Me.ProgressIndicator.Name = "ProgressIndicator"
        Me.ProgressIndicator.Size = New System.Drawing.Size(227, 23)
        Me.ProgressIndicator.TabIndex = 1
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.label1.Location = New System.Drawing.Point(34, 75)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(164, 14)
        Me.label1.TabIndex = 2
        Me.label1.Text = "Executing some lengthy task..."
        '
        'TaskProgress
        '
        Me.ClientSize = New System.Drawing.Size(247, 110)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.ProgressIndicator)
        Me.Controls.Add(Me.ThreadID)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "TaskProgress"
        Me.Text = "TaskProgress"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ThreadID As System.Windows.Forms.Label
    Friend WithEvents ProgressIndicator As System.Windows.Forms.ProgressBar
    Friend WithEvents label1 As System.Windows.Forms.Label
End Class
