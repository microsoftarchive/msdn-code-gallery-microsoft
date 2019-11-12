<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UCNetworkAdapter
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.btnEnableDisable = New System.Windows.Forms.Button()
        Me.lbConnectionStatus = New System.Windows.Forms.Label()
        Me.lbProductName = New System.Windows.Forms.Label()
        Me.pctNetworkAdapter = New System.Windows.Forms.PictureBox()
        CType(Me.pctNetworkAdapter, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnEnableDisable
        '
        Me.btnEnableDisable.Location = New System.Drawing.Point(603, 3)
        Me.btnEnableDisable.Name = "btnEnableDisable"
        Me.btnEnableDisable.Size = New System.Drawing.Size(60, 22)
        Me.btnEnableDisable.TabIndex = 7
        Me.btnEnableDisable.Text = "button1"
        Me.btnEnableDisable.UseVisualStyleBackColor = True
        '
        'lbConnectionStatus
        '
        Me.lbConnectionStatus.Location = New System.Drawing.Point(452, 5)
        Me.lbConnectionStatus.Name = "lbConnectionStatus"
        Me.lbConnectionStatus.Size = New System.Drawing.Size(132, 22)
        Me.lbConnectionStatus.TabIndex = 6
        Me.lbConnectionStatus.Text = "label2"
        Me.lbConnectionStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lbProductName
        '
        Me.lbProductName.Location = New System.Drawing.Point(27, 4)
        Me.lbProductName.Name = "lbProductName"
        Me.lbProductName.Size = New System.Drawing.Size(419, 22)
        Me.lbProductName.TabIndex = 5
        Me.lbProductName.Text = "label1"
        Me.lbProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pctNetworkAdapter
        '
        Me.pctNetworkAdapter.Location = New System.Drawing.Point(7, 8)
        Me.pctNetworkAdapter.Name = "pctNetworkAdapter"
        Me.pctNetworkAdapter.Size = New System.Drawing.Size(15, 15)
        Me.pctNetworkAdapter.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pctNetworkAdapter.TabIndex = 4
        Me.pctNetworkAdapter.TabStop = False
        '
        'UCNetworkAdapter
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.btnEnableDisable)
        Me.Controls.Add(Me.lbConnectionStatus)
        Me.Controls.Add(Me.lbProductName)
        Me.Controls.Add(Me.pctNetworkAdapter)
        Me.Name = "UCNetworkAdapter"
        Me.Size = New System.Drawing.Size(670, 30)
        CType(Me.pctNetworkAdapter, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents btnEnableDisable As System.Windows.Forms.Button
    Private WithEvents lbConnectionStatus As System.Windows.Forms.Label
    Private WithEvents lbProductName As System.Windows.Forms.Label
    Private WithEvents pctNetworkAdapter As System.Windows.Forms.PictureBox

End Class
