<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
        Me.ShowVisualizerButton = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'ShowVisualizerButton
        '
        Me.ShowVisualizerButton.Location = New System.Drawing.Point(12, 28)
        Me.ShowVisualizerButton.Name = "ShowVisualizerButton"
        Me.ShowVisualizerButton.Size = New System.Drawing.Size(177, 23)
        Me.ShowVisualizerButton.TabIndex = 0
        Me.ShowVisualizerButton.Text = "Show Visualizer"
        Me.ShowVisualizerButton.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(200, 79)
        Me.Controls.Add(Me.ShowVisualizerButton)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ShowVisualizerButton As System.Windows.Forms.Button

End Class
