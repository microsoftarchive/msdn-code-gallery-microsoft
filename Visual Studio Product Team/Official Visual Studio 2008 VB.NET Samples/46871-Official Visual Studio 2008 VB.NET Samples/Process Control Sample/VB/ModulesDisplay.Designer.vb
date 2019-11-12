Public Partial Class ModulesDisplay
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
        Me.rchText = New System.Windows.Forms.RichTextBox
        Me.SuspendLayout()
        '
        'rchText
        '
        Me.rchText.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.rchText.Location = New System.Drawing.Point(14, 17)
        Me.rchText.Name = "rchText"
        Me.rchText.Size = New System.Drawing.Size(264, 232)
        Me.rchText.TabIndex = 1
        Me.rchText.Text = ""
        '
        'ModulesDisplay
        '
        Me.ClientSize = New System.Drawing.Size(292, 266)
        Me.Controls.Add(Me.rchText)
        Me.Name = "ModulesDisplay"
        Me.Text = "ModulesDisplay"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents rchText As System.Windows.Forms.RichTextBox
End Class
