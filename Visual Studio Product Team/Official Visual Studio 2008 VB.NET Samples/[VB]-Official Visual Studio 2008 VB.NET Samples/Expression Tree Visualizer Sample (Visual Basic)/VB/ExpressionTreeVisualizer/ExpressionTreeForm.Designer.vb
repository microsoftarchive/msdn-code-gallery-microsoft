<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ExpressionTreeForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ErrorMessageBox = New System.Windows.Forms.TextBox
        Me.TreeBrowser1 = New TreeBrowser
        Me.SuspendLayout()
        '
        'ErrorMessageBox
        '
        Me.ErrorMessageBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ErrorMessageBox.Location = New System.Drawing.Point(6, 8)
        Me.ErrorMessageBox.Multiline = True
        Me.ErrorMessageBox.Name = "ErrorMessageBox"
        Me.ErrorMessageBox.ReadOnly = True
        Me.ErrorMessageBox.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.ErrorMessageBox.Size = New System.Drawing.Size(580, 56)
        Me.ErrorMessageBox.TabIndex = 1
        Me.ErrorMessageBox.TabStop = False
        '
        'TreeBrowser1
        '
        Me.TreeBrowser1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TreeBrowser1.Location = New System.Drawing.Point(6, 72)
        Me.TreeBrowser1.Name = "TreeBrowser1"
        Me.TreeBrowser1.Size = New System.Drawing.Size(580, 689)
        Me.TreeBrowser1.TabIndex = 2
        '
        'ExpressionTreeForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(592, 773)
        Me.Controls.Add(Me.TreeBrowser1)
        Me.Controls.Add(Me.ErrorMessageBox)
        Me.Name = "ExpressionTreeForm"
        Me.Text = "Expression Tree Viewer"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents ErrorMessageBox As System.Windows.Forms.TextBox
    Friend WithEvents TreeBrowser1 As TreeBrowser

End Class
