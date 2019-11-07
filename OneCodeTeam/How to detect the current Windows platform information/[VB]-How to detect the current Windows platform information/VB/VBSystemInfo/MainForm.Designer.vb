<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
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
        Me.groupBox1 = New System.Windows.Forms.GroupBox()
        Me.label6 = New System.Windows.Forms.Label()
        Me.textBox6 = New System.Windows.Forms.TextBox()
        Me.textBox5 = New System.Windows.Forms.TextBox()
        Me.textBox3 = New System.Windows.Forms.TextBox()
        Me.label4 = New System.Windows.Forms.Label()
        Me.textBox4 = New System.Windows.Forms.TextBox()
        Me.label5 = New System.Windows.Forms.Label()
        Me.textBox2 = New System.Windows.Forms.TextBox()
        Me.textBox1 = New System.Windows.Forms.TextBox()
        Me.label3 = New System.Windows.Forms.Label()
        Me.label2 = New System.Windows.Forms.Label()
        Me.label1 = New System.Windows.Forms.Label()
        Me.groupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'groupBox1
        '
        Me.groupBox1.Controls.Add(Me.label6)
        Me.groupBox1.Controls.Add(Me.textBox6)
        Me.groupBox1.Controls.Add(Me.textBox5)
        Me.groupBox1.Controls.Add(Me.textBox3)
        Me.groupBox1.Controls.Add(Me.label4)
        Me.groupBox1.Controls.Add(Me.textBox4)
        Me.groupBox1.Controls.Add(Me.label5)
        Me.groupBox1.Controls.Add(Me.textBox2)
        Me.groupBox1.Controls.Add(Me.textBox1)
        Me.groupBox1.Controls.Add(Me.label3)
        Me.groupBox1.Controls.Add(Me.label2)
        Me.groupBox1.Controls.Add(Me.label1)
        Me.groupBox1.Location = New System.Drawing.Point(27, 42)
        Me.groupBox1.Name = "groupBox1"
        Me.groupBox1.Size = New System.Drawing.Size(443, 290)
        Me.groupBox1.TabIndex = 1
        Me.groupBox1.TabStop = False
        Me.groupBox1.Text = "System Information"
        '
        'label6
        '
        Me.label6.AutoSize = True
        Me.label6.Location = New System.Drawing.Point(25, 213)
        Me.label6.Name = "label6"
        Me.label6.Size = New System.Drawing.Size(91, 13)
        Me.label6.TabIndex = 15
        Me.label6.Text = "Process Bitness : "
        '
        'textBox6
        '
        Me.textBox6.Location = New System.Drawing.Point(153, 213)
        Me.textBox6.Name = "textBox6"
        Me.textBox6.Size = New System.Drawing.Size(107, 20)
        Me.textBox6.TabIndex = 14
        '
        'textBox5
        '
        Me.textBox5.Location = New System.Drawing.Point(153, 175)
        Me.textBox5.Name = "textBox5"
        Me.textBox5.Size = New System.Drawing.Size(107, 20)
        Me.textBox5.TabIndex = 13
        '
        'textBox3
        '
        Me.textBox3.Location = New System.Drawing.Point(153, 93)
        Me.textBox3.Name = "textBox3"
        Me.textBox3.Size = New System.Drawing.Size(231, 20)
        Me.textBox3.TabIndex = 12
        '
        'label4
        '
        Me.label4.AutoSize = True
        Me.label4.Location = New System.Drawing.Point(23, 96)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(109, 13)
        Me.label4.TabIndex = 11
        Me.label4.Text = "Service Pack Level : "
        '
        'textBox4
        '
        Me.textBox4.Location = New System.Drawing.Point(153, 248)
        Me.textBox4.Name = "textBox4"
        Me.textBox4.Size = New System.Drawing.Size(231, 20)
        Me.textBox4.TabIndex = 10
        '
        'label5
        '
        Me.label5.AutoSize = True
        Me.label5.Location = New System.Drawing.Point(23, 248)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(52, 13)
        Me.label5.TabIndex = 9
        Me.label5.Text = "Domain : "
        '
        'textBox2
        '
        Me.textBox2.Location = New System.Drawing.Point(153, 133)
        Me.textBox2.Name = "textBox2"
        Me.textBox2.Size = New System.Drawing.Size(231, 20)
        Me.textBox2.TabIndex = 5
        '
        'textBox1
        '
        Me.textBox1.Location = New System.Drawing.Point(153, 51)
        Me.textBox1.Name = "textBox1"
        Me.textBox1.Size = New System.Drawing.Size(231, 20)
        Me.textBox1.TabIndex = 4
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.Location = New System.Drawing.Point(23, 175)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(94, 13)
        Me.label3.TabIndex = 2
        Me.label3.Text = "Machine Bitness : "
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(23, 133)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(92, 13)
        Me.label2.TabIndex = 1
        Me.label2.Text = "Computer Name : "
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(23, 58)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(96, 13)
        Me.label1.TabIndex = 0
        Me.label1.Text = "Operating System :"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(496, 382)
        Me.Controls.Add(Me.groupBox1)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.groupBox1.ResumeLayout(False)
        Me.groupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents groupBox1 As System.Windows.Forms.GroupBox
    Private WithEvents label6 As System.Windows.Forms.Label
    Private WithEvents textBox6 As System.Windows.Forms.TextBox
    Private WithEvents textBox5 As System.Windows.Forms.TextBox
    Private WithEvents textBox3 As System.Windows.Forms.TextBox
    Private WithEvents label4 As System.Windows.Forms.Label
    Private WithEvents textBox4 As System.Windows.Forms.TextBox
    Private WithEvents label5 As System.Windows.Forms.Label
    Private WithEvents textBox2 As System.Windows.Forms.TextBox
    Private WithEvents textBox1 As System.Windows.Forms.TextBox
    Private WithEvents label3 As System.Windows.Forms.Label
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents label1 As System.Windows.Forms.Label

End Class
