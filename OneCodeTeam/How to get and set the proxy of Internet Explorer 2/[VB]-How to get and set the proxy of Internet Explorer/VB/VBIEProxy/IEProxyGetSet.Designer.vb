<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IEProxyGetSet
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
        Me.grpProxyDetails = New System.Windows.Forms.GroupBox()
        Me.cmbProxyStatusInfo = New System.Windows.Forms.ComboBox()
        Me.lbProxyStatus = New System.Windows.Forms.Label()
        Me.tbProxyByPass = New System.Windows.Forms.TextBox()
        Me.tbProxyServer = New System.Windows.Forms.TextBox()
        Me.cmbAccessType = New System.Windows.Forms.ComboBox()
        Me.lbProxyByPass = New System.Windows.Forms.Label()
        Me.lbProxyServer = New System.Windows.Forms.Label()
        Me.lbAccessType = New System.Windows.Forms.Label()
        Me.btnSetProxy = New System.Windows.Forms.Button()
        Me.btnGetProxy = New System.Windows.Forms.Button()
        Me.grpProxyDetails.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpProxyDetails
        '
        Me.grpProxyDetails.Controls.Add(Me.cmbProxyStatusInfo)
        Me.grpProxyDetails.Controls.Add(Me.lbProxyStatus)
        Me.grpProxyDetails.Controls.Add(Me.tbProxyByPass)
        Me.grpProxyDetails.Controls.Add(Me.tbProxyServer)
        Me.grpProxyDetails.Controls.Add(Me.cmbAccessType)
        Me.grpProxyDetails.Controls.Add(Me.lbProxyByPass)
        Me.grpProxyDetails.Controls.Add(Me.lbProxyServer)
        Me.grpProxyDetails.Controls.Add(Me.lbAccessType)
        Me.grpProxyDetails.Location = New System.Drawing.Point(22, 18)
        Me.grpProxyDetails.Name = "grpProxyDetails"
        Me.grpProxyDetails.Size = New System.Drawing.Size(237, 220)
        Me.grpProxyDetails.TabIndex = 3
        Me.grpProxyDetails.TabStop = False
        Me.grpProxyDetails.Text = "Proxy Details"
        '
        'cmbProxyStatusInfo
        '
        Me.cmbProxyStatusInfo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbProxyStatusInfo.FormattingEnabled = True
        Me.cmbProxyStatusInfo.Items.AddRange(New Object() {"FALSE", "TRUE"})
        Me.cmbProxyStatusInfo.Location = New System.Drawing.Point(100, 197)
        Me.cmbProxyStatusInfo.Name = "cmbProxyStatusInfo"
        Me.cmbProxyStatusInfo.Size = New System.Drawing.Size(115, 21)
        Me.cmbProxyStatusInfo.TabIndex = 7
        '
        'lbProxyStatus
        '
        Me.lbProxyStatus.AutoSize = True
        Me.lbProxyStatus.Location = New System.Drawing.Point(15, 200)
        Me.lbProxyStatus.Name = "lbProxyStatus"
        Me.lbProxyStatus.Size = New System.Drawing.Size(66, 13)
        Me.lbProxyStatus.TabIndex = 6
        Me.lbProxyStatus.Text = "Proxy Status"
        '
        'tbProxyByPass
        '
        Me.tbProxyByPass.Location = New System.Drawing.Point(100, 146)
        Me.tbProxyByPass.Name = "tbProxyByPass"
        Me.tbProxyByPass.Size = New System.Drawing.Size(115, 20)
        Me.tbProxyByPass.TabIndex = 5
        '
        'tbProxyServer
        '
        Me.tbProxyServer.Location = New System.Drawing.Point(100, 95)
        Me.tbProxyServer.Name = "tbProxyServer"
        Me.tbProxyServer.Size = New System.Drawing.Size(115, 20)
        Me.tbProxyServer.TabIndex = 4
        '
        'cmbAccessType
        '
        Me.cmbAccessType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbAccessType.FormattingEnabled = True
        Me.cmbAccessType.Items.AddRange(New Object() {"PRECONFIG", "DIRECT", "PROXY"})
        Me.cmbAccessType.Location = New System.Drawing.Point(100, 44)
        Me.cmbAccessType.Name = "cmbAccessType"
        Me.cmbAccessType.Size = New System.Drawing.Size(115, 21)
        Me.cmbAccessType.TabIndex = 3
        '
        'lbProxyByPass
        '
        Me.lbProxyByPass.AutoSize = True
        Me.lbProxyByPass.Location = New System.Drawing.Point(15, 149)
        Me.lbProxyByPass.Name = "lbProxyByPass"
        Me.lbProxyByPass.Size = New System.Drawing.Size(71, 13)
        Me.lbProxyByPass.TabIndex = 2
        Me.lbProxyByPass.Text = "Proxy ByPass"
        '
        'lbProxyServer
        '
        Me.lbProxyServer.AutoSize = True
        Me.lbProxyServer.Location = New System.Drawing.Point(15, 98)
        Me.lbProxyServer.Name = "lbProxyServer"
        Me.lbProxyServer.Size = New System.Drawing.Size(67, 13)
        Me.lbProxyServer.TabIndex = 1
        Me.lbProxyServer.Text = "Proxy Server"
        '
        'lbAccessType
        '
        Me.lbAccessType.AutoSize = True
        Me.lbAccessType.Location = New System.Drawing.Point(15, 47)
        Me.lbAccessType.Name = "lbAccessType"
        Me.lbAccessType.Size = New System.Drawing.Size(69, 13)
        Me.lbAccessType.TabIndex = 0
        Me.lbAccessType.Text = "Access Type"
        '
        'btnSetProxy
        '
        Me.btnSetProxy.Location = New System.Drawing.Point(159, 253)
        Me.btnSetProxy.Name = "btnSetProxy"
        Me.btnSetProxy.Size = New System.Drawing.Size(75, 23)
        Me.btnSetProxy.TabIndex = 5
        Me.btnSetProxy.Text = "Set Proxy"
        Me.btnSetProxy.UseVisualStyleBackColor = True
        '
        'btnGetProxy
        '
        Me.btnGetProxy.Location = New System.Drawing.Point(54, 253)
        Me.btnGetProxy.Name = "btnGetProxy"
        Me.btnGetProxy.Size = New System.Drawing.Size(75, 23)
        Me.btnGetProxy.TabIndex = 4
        Me.btnGetProxy.Text = "Get Proxy"
        Me.btnGetProxy.UseVisualStyleBackColor = True
        '
        'IEProxyGetSet
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(288, 311)
        Me.Controls.Add(Me.grpProxyDetails)
        Me.Controls.Add(Me.btnSetProxy)
        Me.Controls.Add(Me.btnGetProxy)
        Me.Name = "IEProxyGetSet"
        Me.Text = "IEProxyGetSet"
        Me.grpProxyDetails.ResumeLayout(False)
        Me.grpProxyDetails.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents grpProxyDetails As System.Windows.Forms.GroupBox
    Private WithEvents cmbProxyStatusInfo As System.Windows.Forms.ComboBox
    Private WithEvents lbProxyStatus As System.Windows.Forms.Label
    Private WithEvents tbProxyByPass As System.Windows.Forms.TextBox
    Private WithEvents tbProxyServer As System.Windows.Forms.TextBox
    Private WithEvents cmbAccessType As System.Windows.Forms.ComboBox
    Private WithEvents lbProxyByPass As System.Windows.Forms.Label
    Private WithEvents lbProxyServer As System.Windows.Forms.Label
    Private WithEvents lbAccessType As System.Windows.Forms.Label
    Private WithEvents btnSetProxy As System.Windows.Forms.Button
    Private WithEvents btnGetProxy As System.Windows.Forms.Button
End Class
