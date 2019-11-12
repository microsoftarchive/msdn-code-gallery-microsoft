Public Partial Class ViewSourceForm
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
        Me.SourceTextbox = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'SourceTextbox
        '
        Me.SourceTextbox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SourceTextbox.Font = New System.Drawing.Font("Courier New", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SourceTextbox.Location = New System.Drawing.Point(0, 0)
        Me.SourceTextbox.Multiline = True
        Me.SourceTextbox.Name = "SourceTextbox"
        Me.SourceTextbox.ReadOnly = True
        Me.SourceTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.SourceTextbox.Size = New System.Drawing.Size(550, 571)
        Me.SourceTextbox.TabIndex = 0
        Me.SourceTextbox.WordWrap = False
        '
        'ViewSourceForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(5, 13)
        Me.ClientSize = New System.Drawing.Size(550, 571)
        Me.Controls.Add(Me.SourceTextbox)
        Me.Name = "ViewSourceForm"
        Me.Text = "ViewSourceForm"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents SourceTextbox As System.Windows.Forms.TextBox
End Class
