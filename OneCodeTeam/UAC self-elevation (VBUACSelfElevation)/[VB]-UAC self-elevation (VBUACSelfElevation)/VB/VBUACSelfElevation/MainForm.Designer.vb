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
        Me.lbIsElevated = New System.Windows.Forms.Label
        Me.label3 = New System.Windows.Forms.Label
        Me.lbIsRunAsAdmin = New System.Windows.Forms.Label
        Me.label2 = New System.Windows.Forms.Label
        Me.lbInAdminGroup = New System.Windows.Forms.Label
        Me.label1 = New System.Windows.Forms.Label
        Me.btnElevate = New System.Windows.Forms.Button
        Me.Label4 = New System.Windows.Forms.Label
        Me.lbIntegrityLevel = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lbIsElevated
        '
        Me.lbIsElevated.AutoSize = True
        Me.lbIsElevated.Location = New System.Drawing.Point(140, 63)
        Me.lbIsElevated.Name = "lbIsElevated"
        Me.lbIsElevated.Size = New System.Drawing.Size(0, 13)
        Me.lbIsElevated.TabIndex = 13
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.Location = New System.Drawing.Point(13, 63)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(98, 13)
        Me.label3.TabIndex = 12
        Me.label3.Text = "IsProcessElevated:"
        '
        'lbIsRunAsAdmin
        '
        Me.lbIsRunAsAdmin.AutoSize = True
        Me.lbIsRunAsAdmin.Location = New System.Drawing.Point(140, 38)
        Me.lbIsRunAsAdmin.Name = "lbIsRunAsAdmin"
        Me.lbIsRunAsAdmin.Size = New System.Drawing.Size(0, 13)
        Me.lbIsRunAsAdmin.TabIndex = 11
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(13, 38)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(79, 13)
        Me.label2.TabIndex = 10
        Me.label2.Text = "IsRunAsAdmin:"
        '
        'lbInAdminGroup
        '
        Me.lbInAdminGroup.AutoSize = True
        Me.lbInAdminGroup.Location = New System.Drawing.Point(140, 13)
        Me.lbInAdminGroup.Name = "lbInAdminGroup"
        Me.lbInAdminGroup.Size = New System.Drawing.Size(0, 13)
        Me.lbInAdminGroup.TabIndex = 9
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(13, 13)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(107, 13)
        Me.label1.TabIndex = 8
        Me.label1.Text = "IsUserInAdminGroup:"
        '
        'btnElevate
        '
        Me.btnElevate.Location = New System.Drawing.Point(15, 113)
        Me.btnElevate.Name = "btnElevate"
        Me.btnElevate.Size = New System.Drawing.Size(181, 26)
        Me.btnElevate.TabIndex = 7
        Me.btnElevate.Text = "Self-elevate"
        Me.btnElevate.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(13, 88)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(76, 13)
        Me.Label4.TabIndex = 14
        Me.Label4.Text = "Integrity Level:"
        '
        'lbIntegrityLevel
        '
        Me.lbIntegrityLevel.AutoSize = True
        Me.lbIntegrityLevel.Location = New System.Drawing.Point(140, 88)
        Me.lbIntegrityLevel.Name = "lbIntegrityLevel"
        Me.lbIntegrityLevel.Size = New System.Drawing.Size(0, 13)
        Me.lbIntegrityLevel.TabIndex = 15
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(244, 151)
        Me.Controls.Add(Me.lbIntegrityLevel)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.lbIsElevated)
        Me.Controls.Add(Me.label3)
        Me.Controls.Add(Me.lbIsRunAsAdmin)
        Me.Controls.Add(Me.label2)
        Me.Controls.Add(Me.lbInAdminGroup)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.btnElevate)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "VBUACSelfElevation"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents lbIsElevated As System.Windows.Forms.Label
    Private WithEvents label3 As System.Windows.Forms.Label
    Private WithEvents lbIsRunAsAdmin As System.Windows.Forms.Label
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents lbInAdminGroup As System.Windows.Forms.Label
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents btnElevate As System.Windows.Forms.Button
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lbIntegrityLevel As System.Windows.Forms.Label

End Class
