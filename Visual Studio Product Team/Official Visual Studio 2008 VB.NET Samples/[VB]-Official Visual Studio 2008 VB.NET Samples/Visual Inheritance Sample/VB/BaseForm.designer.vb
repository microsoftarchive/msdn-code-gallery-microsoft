' Copyright (c) Microsoft Corporation. All rights reserved.
Partial Public Class BaseForm
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()> _
    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

    End Sub

    'Form overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BaseForm))
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.lblPrivate = New System.Windows.Forms.Label
        Me.lblProtected = New System.Windows.Forms.Label
        Me.lblTitle = New System.Windows.Forms.Label
        Me.btnClose = New System.Windows.Forms.Button
        Me.lblDateTime = New System.Windows.Forms.Label
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = InheritForm.My.Resources.ConsolidatedInsurance2
        Me.PictureBox1.Location = New System.Drawing.Point(531, 13)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(151, 102)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 11
        Me.PictureBox1.TabStop = False
        '
        'lblPrivate
        '
        Me.lblPrivate.AutoSize = True
        Me.lblPrivate.Location = New System.Drawing.Point(274, 347)
        Me.lblPrivate.Name = "lblPrivate"
        Me.lblPrivate.Size = New System.Drawing.Size(67, 14)
        Me.lblPrivate.TabIndex = 10
        Me.lblPrivate.Text = "Private label"
        '
        'lblProtected
        '
        Me.lblProtected.AutoSize = True
        Me.lblProtected.Location = New System.Drawing.Point(275, 322)
        Me.lblProtected.Name = "lblProtected"
        Me.lblProtected.Size = New System.Drawing.Size(80, 14)
        Me.lblProtected.TabIndex = 9
        Me.lblProtected.Text = "Protected label"
        '
        'lblTitle
        '
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(14, 17)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(457, 23)
        Me.lblTitle.TabIndex = 8
        Me.lblTitle.Text = "Title"
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(607, 326)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(75, 41)
        Me.btnClose.TabIndex = 7
        Me.btnClose.Text = "&Close"
        '
        'lblDateTime
        '
        Me.lblDateTime.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblDateTime.Location = New System.Drawing.Point(13, 330)
        Me.lblDateTime.Name = "lblDateTime"
        Me.lblDateTime.Size = New System.Drawing.Size(254, 23)
        Me.lblDateTime.TabIndex = 6
        Me.lblDateTime.Text = "Date and Time"
        Me.lblDateTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BaseForm
        '
        Me.ClientSize = New System.Drawing.Size(694, 379)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.lblPrivate)
        Me.Controls.Add(Me.lblProtected)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.lblDateTime)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "BaseForm"
        Me.Text = "Base Form"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents lblPrivate As System.Windows.Forms.Label
    Protected WithEvents lblProtected As System.Windows.Forms.Label
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents lblDateTime As System.Windows.Forms.Label
End Class




