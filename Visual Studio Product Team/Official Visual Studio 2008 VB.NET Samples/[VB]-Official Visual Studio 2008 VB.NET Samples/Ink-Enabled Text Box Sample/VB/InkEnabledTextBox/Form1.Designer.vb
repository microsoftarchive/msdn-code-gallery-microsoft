<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        If disposing Then
            If Not myInkOverlay Is Nothing Then
                myInkOverlay.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer


    Friend WithEvents textBox1 As System.Windows.Forms.TextBox
    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.textBox1 = New System.Windows.Forms.TextBox
        Me.richTextBoxWithTipCorrection = New System.Windows.Forms.RichTextBox
        Me.basicTextBox = New System.Windows.Forms.TextBox
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'textBox1
        '
        Me.textBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.textBox1.Location = New System.Drawing.Point(13, 12)
        Me.textBox1.Name = "textBox1"
        Me.textBox1.Size = New System.Drawing.Size(606, 35)
        Me.textBox1.TabIndex = 0
        '
        'richTextBoxWithTipCorrection
        '
        Me.richTextBoxWithTipCorrection.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.richTextBoxWithTipCorrection.Location = New System.Drawing.Point(12, 53)
        Me.richTextBoxWithTipCorrection.Multiline = False
        Me.richTextBoxWithTipCorrection.Name = "richTextBoxWithTipCorrection"
        Me.richTextBoxWithTipCorrection.Size = New System.Drawing.Size(606, 31)
        Me.richTextBoxWithTipCorrection.TabIndex = 1
        Me.richTextBoxWithTipCorrection.Text = ""
        '
        'basicTextBox
        '
        Me.basicTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.basicTextBox.Location = New System.Drawing.Point(12, 90)
        Me.basicTextBox.Name = "basicTextBox"
        Me.basicTextBox.Size = New System.Drawing.Size(607, 35)
        Me.basicTextBox.TabIndex = 2
        '
        'Timer1
        '
        Me.Timer1.Interval = 5000
        '
        'Form1
        '
        Me.ClientSize = New System.Drawing.Size(631, 167)
        Me.Controls.Add(Me.basicTextBox)
        Me.Controls.Add(Me.richTextBoxWithTipCorrection)
        Me.Controls.Add(Me.textBox1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents richTextBoxWithTipCorrection As System.Windows.Forms.RichTextBox
    Friend WithEvents basicTextBox As System.Windows.Forms.TextBox
    Friend WithEvents Timer1 As System.Windows.Forms.Timer

End Class
