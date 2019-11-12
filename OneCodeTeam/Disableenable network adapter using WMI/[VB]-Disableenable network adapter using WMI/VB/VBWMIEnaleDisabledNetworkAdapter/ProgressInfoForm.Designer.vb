<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ProgressInfoForm
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
        Me.lbProgressInfo = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lbProgressInfo
        '
        Me.lbProgressInfo.Location = New System.Drawing.Point(12, 9)
        Me.lbProgressInfo.Name = "lbProgressInfo"
        Me.lbProgressInfo.Size = New System.Drawing.Size(366, 26)
        Me.lbProgressInfo.TabIndex = 1
        Me.lbProgressInfo.Text = "label1"
        Me.lbProgressInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ProgressInfo
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Lime
        Me.ClientSize = New System.Drawing.Size(390, 45)
        Me.Controls.Add(Me.lbProgressInfo)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "ProgressInfo"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "ProgressInfo"
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents lbProgressInfo As System.Windows.Forms.Label
End Class
