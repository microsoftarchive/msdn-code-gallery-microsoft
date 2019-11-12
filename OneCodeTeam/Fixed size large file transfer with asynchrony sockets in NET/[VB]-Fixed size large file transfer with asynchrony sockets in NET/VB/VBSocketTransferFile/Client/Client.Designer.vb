Namespace Client
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class Client
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
            Me.gbxServerInfo = New System.Windows.Forms.GroupBox()
            Me.tbxPort = New System.Windows.Forms.TextBox()
            Me.lblPort = New System.Windows.Forms.Label()
            Me.tbxAddress = New System.Windows.Forms.TextBox()
            Me.lblAddress = New System.Windows.Forms.Label()
            Me.btnSavePath = New System.Windows.Forms.Button()
            Me.tbxSavePath = New System.Windows.Forms.TextBox()
            Me.progressBar = New System.Windows.Forms.ProgressBar()
            Me.btnConnect = New System.Windows.Forms.Button()
            Me.gbxServerInfo.SuspendLayout()
            Me.SuspendLayout()
            '
            'gbxServerInfo
            '
            Me.gbxServerInfo.Controls.Add(Me.tbxPort)
            Me.gbxServerInfo.Controls.Add(Me.lblPort)
            Me.gbxServerInfo.Controls.Add(Me.tbxAddress)
            Me.gbxServerInfo.Controls.Add(Me.lblAddress)
            Me.gbxServerInfo.Location = New System.Drawing.Point(15, 28)
            Me.gbxServerInfo.Name = "gbxServerInfo"
            Me.gbxServerInfo.Size = New System.Drawing.Size(260, 88)
            Me.gbxServerInfo.TabIndex = 6
            Me.gbxServerInfo.TabStop = False
            Me.gbxServerInfo.Text = "Server"
            '
            'tbxPort
            '
            Me.tbxPort.Location = New System.Drawing.Point(59, 52)
            Me.tbxPort.Name = "tbxPort"
            Me.tbxPort.Size = New System.Drawing.Size(100, 20)
            Me.tbxPort.TabIndex = 3
            Me.tbxPort.Text = "11000"
            '
            'lblPort
            '
            Me.lblPort.AutoSize = True
            Me.lblPort.Location = New System.Drawing.Point(6, 52)
            Me.lblPort.Name = "lblPort"
            Me.lblPort.Size = New System.Drawing.Size(26, 13)
            Me.lblPort.TabIndex = 2
            Me.lblPort.Text = "Port"
            '
            'tbxAddress
            '
            Me.tbxAddress.Location = New System.Drawing.Point(59, 20)
            Me.tbxAddress.Name = "tbxAddress"
            Me.tbxAddress.Size = New System.Drawing.Size(100, 20)
            Me.tbxAddress.TabIndex = 1
            '
            'lblAddress
            '
            Me.lblAddress.AutoSize = True
            Me.lblAddress.Location = New System.Drawing.Point(6, 20)
            Me.lblAddress.Name = "lblAddress"
            Me.lblAddress.Size = New System.Drawing.Size(45, 13)
            Me.lblAddress.TabIndex = 0
            Me.lblAddress.Text = "Address"
            '
            'btnSavePath
            '
            Me.btnSavePath.Location = New System.Drawing.Point(200, 132)
            Me.btnSavePath.Name = "btnSavePath"
            Me.btnSavePath.Size = New System.Drawing.Size(75, 23)
            Me.btnSavePath.TabIndex = 9
            Me.btnSavePath.Text = "Save To..."
            Me.btnSavePath.UseVisualStyleBackColor = True
            '
            'tbxSavePath
            '
            Me.tbxSavePath.Enabled = False
            Me.tbxSavePath.Location = New System.Drawing.Point(15, 134)
            Me.tbxSavePath.Name = "tbxSavePath"
            Me.tbxSavePath.Size = New System.Drawing.Size(179, 20)
            Me.tbxSavePath.TabIndex = 8
            '
            'progressBar
            '
            Me.progressBar.Location = New System.Drawing.Point(15, 161)
            Me.progressBar.Name = "progressBar"
            Me.progressBar.Size = New System.Drawing.Size(260, 23)
            Me.progressBar.TabIndex = 7
            '
            'btnConnect
            '
            Me.btnConnect.Location = New System.Drawing.Point(99, 190)
            Me.btnConnect.Name = "btnConnect"
            Me.btnConnect.Size = New System.Drawing.Size(75, 23)
            Me.btnConnect.TabIndex = 5
            Me.btnConnect.Text = "Connect"
            Me.btnConnect.UseVisualStyleBackColor = True
            '
            'Client
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(291, 240)
            Me.Controls.Add(Me.gbxServerInfo)
            Me.Controls.Add(Me.btnSavePath)
            Me.Controls.Add(Me.tbxSavePath)
            Me.Controls.Add(Me.progressBar)
            Me.Controls.Add(Me.btnConnect)
            Me.Name = "Client"
            Me.Text = "Client"
            Me.gbxServerInfo.ResumeLayout(False)
            Me.gbxServerInfo.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents gbxServerInfo As System.Windows.Forms.GroupBox
        Private WithEvents tbxPort As System.Windows.Forms.TextBox
        Private WithEvents lblPort As System.Windows.Forms.Label
        Private WithEvents tbxAddress As System.Windows.Forms.TextBox
        Private WithEvents lblAddress As System.Windows.Forms.Label
        Private WithEvents btnSavePath As System.Windows.Forms.Button
        Private WithEvents tbxSavePath As System.Windows.Forms.TextBox
        Private WithEvents progressBar As System.Windows.Forms.ProgressBar
        Private WithEvents btnConnect As System.Windows.Forms.Button

    End Class
End Namespace

