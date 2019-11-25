
Partial Public Class UICredentialsProvider
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.lbUserName = New System.Windows.Forms.Label()
        Me.lbPassword = New System.Windows.Forms.Label()
        Me.tbPassword = New System.Windows.Forms.TextBox()
        Me.tbUserName = New System.Windows.Forms.TextBox()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.lbDomain = New System.Windows.Forms.Label()
        Me.tbDomain = New System.Windows.Forms.TextBox()
        Me.chkAnonymous = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'lbUserName
        '
        Me.lbUserName.AutoSize = True
        Me.lbUserName.Location = New System.Drawing.Point(21, 9)
        Me.lbUserName.Name = "lbUserName"
        Me.lbUserName.Size = New System.Drawing.Size(60, 13)
        Me.lbUserName.TabIndex = 0
        Me.lbUserName.Text = "User Name"
        '
        'lbPassword
        '
        Me.lbPassword.AutoSize = True
        Me.lbPassword.Location = New System.Drawing.Point(21, 40)
        Me.lbPassword.Name = "lbPassword"
        Me.lbPassword.Size = New System.Drawing.Size(53, 13)
        Me.lbPassword.TabIndex = 0
        Me.lbPassword.Text = "Password"
        '
        'tbPassword
        '
        Me.tbPassword.Location = New System.Drawing.Point(98, 36)
        Me.tbPassword.Name = "tbPassword"
        Me.tbPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.tbPassword.Size = New System.Drawing.Size(171, 20)
        Me.tbPassword.TabIndex = 2
        '
        'tbUserName
        '
        Me.tbUserName.Location = New System.Drawing.Point(98, 5)
        Me.tbUserName.Name = "tbUserName"
        Me.tbUserName.Size = New System.Drawing.Size(171, 20)
        Me.tbUserName.TabIndex = 1
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(98, 121)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 4
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(194, 121)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 5
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'lbDomain
        '
        Me.lbDomain.AutoSize = True
        Me.lbDomain.Location = New System.Drawing.Point(21, 72)
        Me.lbDomain.Name = "lbDomain"
        Me.lbDomain.Size = New System.Drawing.Size(43, 13)
        Me.lbDomain.TabIndex = 0
        Me.lbDomain.Text = "Domain"
        '
        'tbDomain
        '
        Me.tbDomain.Location = New System.Drawing.Point(98, 68)
        Me.tbDomain.Name = "tbDomain"
        Me.tbDomain.Size = New System.Drawing.Size(171, 20)
        Me.tbDomain.TabIndex = 3
        '
        'chkAnonymous
        '
        Me.chkAnonymous.AutoSize = True
        Me.chkAnonymous.Location = New System.Drawing.Point(24, 98)
        Me.chkAnonymous.Name = "chkAnonymous"
        Me.chkAnonymous.Size = New System.Drawing.Size(123, 17)
        Me.chkAnonymous.TabIndex = 7
        Me.chkAnonymous.Text = "Log on anonymously"
        Me.chkAnonymous.UseVisualStyleBackColor = True
        '
        'UICredentialsProvider
        '
        Me.AcceptButton = Me.btnOK
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(319, 156)
        Me.ControlBox = False
        Me.Controls.Add(Me.chkAnonymous)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.tbUserName)
        Me.Controls.Add(Me.tbDomain)
        Me.Controls.Add(Me.tbPassword)
        Me.Controls.Add(Me.lbDomain)
        Me.Controls.Add(Me.lbPassword)
        Me.Controls.Add(Me.lbUserName)
        Me.Name = "UICredentialsProvider"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Credentials Provider"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private lbUserName As Label
    Private lbPassword As Label
    Private WithEvents tbPassword As TextBox
    Private WithEvents tbUserName As TextBox
    Private WithEvents btnOK As Button
    Private WithEvents btnCancel As Button
    Private lbDomain As Label
    Private WithEvents tbDomain As TextBox
    Private WithEvents chkAnonymous As System.Windows.Forms.CheckBox
End Class