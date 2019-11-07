Namespace Server
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class Server
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
            Me.tbxPort = New System.Windows.Forms.TextBox()
            Me.lblPort = New System.Windows.Forms.Label()
            Me.btnSend = New System.Windows.Forms.Button()
            Me.lbxServer = New System.Windows.Forms.ListBox()
            Me.btnSelectFile = New System.Windows.Forms.Button()
            Me.tbxFile = New System.Windows.Forms.TextBox()
            Me.lblFile = New System.Windows.Forms.Label()
            Me.btnStartup = New System.Windows.Forms.Button()
            Me.OpenFileDialog = New System.Windows.Forms.OpenFileDialog()
            Me.lblServer = New System.Windows.Forms.Label()
            Me.SuspendLayout()
            '
            'tbxPort
            '
            Me.tbxPort.Location = New System.Drawing.Point(77, 20)
            Me.tbxPort.Name = "tbxPort"
            Me.tbxPort.Size = New System.Drawing.Size(100, 20)
            Me.tbxPort.TabIndex = 15
            Me.tbxPort.Text = "11000"
            '
            'lblPort
            '
            Me.lblPort.AutoSize = True
            Me.lblPort.Location = New System.Drawing.Point(10, 20)
            Me.lblPort.Name = "lblPort"
            Me.lblPort.Size = New System.Drawing.Size(26, 13)
            Me.lblPort.TabIndex = 14
            Me.lblPort.Text = "Port"
            '
            'btnSend
            '
            Me.btnSend.Location = New System.Drawing.Point(226, 147)
            Me.btnSend.Name = "btnSend"
            Me.btnSend.Size = New System.Drawing.Size(75, 26)
            Me.btnSend.TabIndex = 13
            Me.btnSend.Text = "Send"
            Me.btnSend.UseVisualStyleBackColor = True
            '
            'lbxServer
            '
            Me.lbxServer.FormattingEnabled = True
            Me.lbxServer.Location = New System.Drawing.Point(13, 97)
            Me.lbxServer.Name = "lbxServer"
            Me.lbxServer.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple
            Me.lbxServer.Size = New System.Drawing.Size(207, 134)
            Me.lbxServer.TabIndex = 12
            '
            'btnSelectFile
            '
            Me.btnSelectFile.Location = New System.Drawing.Point(226, 45)
            Me.btnSelectFile.Name = "btnSelectFile"
            Me.btnSelectFile.Size = New System.Drawing.Size(75, 23)
            Me.btnSelectFile.TabIndex = 11
            Me.btnSelectFile.Text = "Select"
            Me.btnSelectFile.UseVisualStyleBackColor = True
            '
            'tbxFile
            '
            Me.tbxFile.Enabled = False
            Me.tbxFile.Location = New System.Drawing.Point(77, 45)
            Me.tbxFile.Name = "tbxFile"
            Me.tbxFile.Size = New System.Drawing.Size(143, 20)
            Me.tbxFile.TabIndex = 10
            '
            'lblFile
            '
            Me.lblFile.AutoSize = True
            Me.lblFile.Location = New System.Drawing.Point(10, 48)
            Me.lblFile.Name = "lblFile"
            Me.lblFile.Size = New System.Drawing.Size(61, 13)
            Me.lblFile.TabIndex = 9
            Me.lblFile.Text = "File to send"
            '
            'btnStartup
            '
            Me.btnStartup.Location = New System.Drawing.Point(226, 179)
            Me.btnStartup.Name = "btnStartup"
            Me.btnStartup.Size = New System.Drawing.Size(75, 52)
            Me.btnStartup.TabIndex = 8
            Me.btnStartup.Text = "Startup"
            Me.btnStartup.UseVisualStyleBackColor = True
            '
            'OpenFileDialog
            '
            Me.OpenFileDialog.FileName = "OpenFileDialog"
            '
            'lblServer
            '
            Me.lblServer.AutoSize = True
            Me.lblServer.Location = New System.Drawing.Point(13, 78)
            Me.lblServer.Name = "lblServer"
            Me.lblServer.Size = New System.Drawing.Size(93, 13)
            Me.lblServer.TabIndex = 16
            Me.lblServer.Text = "Connected Clients"
            '
            'Server
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(311, 251)
            Me.Controls.Add(Me.lblServer)
            Me.Controls.Add(Me.tbxPort)
            Me.Controls.Add(Me.lblPort)
            Me.Controls.Add(Me.btnSend)
            Me.Controls.Add(Me.lbxServer)
            Me.Controls.Add(Me.btnSelectFile)
            Me.Controls.Add(Me.tbxFile)
            Me.Controls.Add(Me.lblFile)
            Me.Controls.Add(Me.btnStartup)
            Me.Name = "Server"
            Me.Text = "Server"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents tbxPort As System.Windows.Forms.TextBox
        Private WithEvents lblPort As System.Windows.Forms.Label
        Private WithEvents btnSend As System.Windows.Forms.Button
        Private WithEvents lbxServer As System.Windows.Forms.ListBox
        Private WithEvents btnSelectFile As System.Windows.Forms.Button
        Private WithEvents tbxFile As System.Windows.Forms.TextBox
        Private WithEvents lblFile As System.Windows.Forms.Label
        Private WithEvents btnStartup As System.Windows.Forms.Button
        Friend WithEvents OpenFileDialog As System.Windows.Forms.OpenFileDialog
        Friend WithEvents lblServer As System.Windows.Forms.Label

    End Class
End Namespace
