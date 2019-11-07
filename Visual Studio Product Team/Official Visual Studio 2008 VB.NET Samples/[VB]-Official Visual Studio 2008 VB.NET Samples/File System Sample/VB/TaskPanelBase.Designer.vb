Partial Public Class TaskPanelBase
    Inherits System.Windows.Forms.UserControl

    <System.Diagnostics.DebuggerNonUserCode()> _
    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

    End Sub

    'UserControl overrides dispose to clean up the component list.
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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.DescriptionTextBox = New System.Windows.Forms.TextBox
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.ResetValuesButton = New System.Windows.Forms.Button
        Me.ExececuteMethodButton = New System.Windows.Forms.Button
        Me.EndParenLabel = New System.Windows.Forms.Label
        Me.MethodNameLabel = New System.Windows.Forms.Label
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.DescriptionTextBox)
        Me.GroupBox1.Location = New System.Drawing.Point(27, 495)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(593, 120)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Description"
        '
        'DescriptionTextBox
        '
        Me.DescriptionTextBox.BackColor = System.Drawing.SystemColors.Control
        Me.DescriptionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DescriptionTextBox.Location = New System.Drawing.Point(14, 23)
        Me.DescriptionTextBox.Name = "DescriptionTextBox"
        Me.DescriptionTextBox.Size = New System.Drawing.Size(568, 13)
        Me.DescriptionTextBox.TabIndex = 0
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.ResetValuesButton)
        Me.GroupBox2.Controls.Add(Me.ExececuteMethodButton)
        Me.GroupBox2.Controls.Add(Me.EndParenLabel)
        Me.GroupBox2.Controls.Add(Me.MethodNameLabel)
        Me.GroupBox2.Location = New System.Drawing.Point(27, 15)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(592, 437)
        Me.GroupBox2.TabIndex = 2
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Method Signature"
        '
        'ResetValuesButton
        '
        Me.ResetValuesButton.Location = New System.Drawing.Point(300, 394)
        Me.ResetValuesButton.Name = "ResetValuesButton"
        Me.ResetValuesButton.Size = New System.Drawing.Size(108, 28)
        Me.ResetValuesButton.TabIndex = 3
        Me.ResetValuesButton.Text = "Reset Values"
        '
        'ExececuteMethodButton
        '
        Me.ExececuteMethodButton.Location = New System.Drawing.Point(172, 394)
        Me.ExececuteMethodButton.Name = "ExececuteMethodButton"
        Me.ExececuteMethodButton.Size = New System.Drawing.Size(108, 28)
        Me.ExececuteMethodButton.TabIndex = 2
        Me.ExececuteMethodButton.Text = "Execute"
        '
        'EndParenLabel
        '
        Me.EndParenLabel.AutoSize = True
        Me.EndParenLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.EndParenLabel.Location = New System.Drawing.Point(14, 68)
        Me.EndParenLabel.Name = "EndParenLabel"
        Me.EndParenLabel.Size = New System.Drawing.Size(19, 25)
        Me.EndParenLabel.TabIndex = 1
        Me.EndParenLabel.Text = ")"
        '
        'MethodNameLabel
        '
        Me.MethodNameLabel.AutoSize = True
        Me.MethodNameLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MethodNameLabel.Location = New System.Drawing.Point(14, 36)
        Me.MethodNameLabel.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.MethodNameLabel.Name = "MethodNameLabel"
        Me.MethodNameLabel.Size = New System.Drawing.Size(393, 25)
        Me.MethodNameLabel.TabIndex = 0
        Me.MethodNameLabel.Text = "My.Computer.FileSystem.MethodName(" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'TaskPanelBase
        '
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "TaskPanelBase"
        Me.Size = New System.Drawing.Size(650, 650)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Protected Friend WithEvents DescriptionTextBox As System.Windows.Forms.TextBox
    Friend WithEvents MethodNameLabel As System.Windows.Forms.Label
    Protected Friend WithEvents EndParenLabel As System.Windows.Forms.Label
    Protected Friend WithEvents ExececuteMethodButton As System.Windows.Forms.Button
    Protected Friend WithEvents ResetValuesButton As System.Windows.Forms.Button
    Protected Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox

End Class
