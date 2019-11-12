Namespace Microsoft.ServiceModel.Samples
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
                participant.Leave(member)
                participant.Close()
                'factory.Close()
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
            Me.RichTextBox1 = New System.Windows.Forms.RichTextBox
            Me.RichTextBox2 = New System.Windows.Forms.RichTextBox
            Me.Button1 = New System.Windows.Forms.Button
            Me.Connect = New System.Windows.Forms.Button
            Me.TextBox1 = New System.Windows.Forms.TextBox
            Me.SuspendLayout()
            '
            'RichTextBox1
            '
            Me.RichTextBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.RichTextBox1.Location = New System.Drawing.Point(12, 3)
            Me.RichTextBox1.Name = "RichTextBox1"
            Me.RichTextBox1.Size = New System.Drawing.Size(260, 189)
            Me.RichTextBox1.TabIndex = 0
            Me.RichTextBox1.Text = ""
            Me.RichTextBox1.Visible = False
            '
            'RichTextBox2
            '
            Me.RichTextBox2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.RichTextBox2.Location = New System.Drawing.Point(13, 209)
            Me.RichTextBox2.Name = "RichTextBox2"
            Me.RichTextBox2.Size = New System.Drawing.Size(212, 34)
            Me.RichTextBox2.TabIndex = 1
            Me.RichTextBox2.Text = ""
            Me.RichTextBox2.Visible = False
            '
            'Button1
            '
            Me.Button1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Button1.Location = New System.Drawing.Point(231, 209)
            Me.Button1.Name = "Button1"
            Me.Button1.Size = New System.Drawing.Size(41, 34)
            Me.Button1.TabIndex = 2
            Me.Button1.Text = "Send"
            Me.Button1.UseVisualStyleBackColor = True
            Me.Button1.Visible = False
            '
            'Connect
            '
            Me.Connect.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.Connect.Location = New System.Drawing.Point(110, 160)
            Me.Connect.Name = "Connect"
            Me.Connect.Size = New System.Drawing.Size(60, 32)
            Me.Connect.TabIndex = 3
            Me.Connect.Text = "Connect"
            Me.Connect.UseVisualStyleBackColor = True
            '
            'TextBox1
            '
            Me.TextBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.TextBox1.Location = New System.Drawing.Point(74, 85)
            Me.TextBox1.Name = "TextBox1"
            Me.TextBox1.Size = New System.Drawing.Size(151, 20)
            Me.TextBox1.TabIndex = 4
            Me.TextBox1.Text = "DefaultName"
            '
            'Form1
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(284, 264)
            Me.Controls.Add(Me.TextBox1)
            Me.Controls.Add(Me.Connect)
            Me.Controls.Add(Me.Button1)
            Me.Controls.Add(Me.RichTextBox2)
            Me.Controls.Add(Me.RichTextBox1)
            Me.Name = "Form1"
            Me.Text = "Chat Instance"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents RichTextBox1 As System.Windows.Forms.RichTextBox
        Friend WithEvents RichTextBox2 As System.Windows.Forms.RichTextBox
        Friend WithEvents Button1 As System.Windows.Forms.Button
        Friend WithEvents Connect As System.Windows.Forms.Button
        Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    End Class
End Namespace
