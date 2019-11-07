Public Partial Class ImageViewer
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
        Me.btnClose = New System.Windows.Forms.Button
        Me.lblImageFilename = New System.Windows.Forms.Label
        Me.picImage = New System.Windows.Forms.PictureBox
        CType(Me.picImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnClose
        '
        Me.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnClose.Location = New System.Drawing.Point(330, 313)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.TabIndex = 5
        Me.btnClose.Text = "Close"
        '
        'lblImageFilename
        '
        Me.lblImageFilename.Location = New System.Drawing.Point(8, 271)
        Me.lblImageFilename.Name = "lblImageFilename"
        Me.lblImageFilename.Size = New System.Drawing.Size(400, 32)
        Me.lblImageFilename.TabIndex = 4
        Me.lblImageFilename.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'picImage
        '
        Me.picImage.BackColor = System.Drawing.SystemColors.Window
        Me.picImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.picImage.Location = New System.Drawing.Point(13, 15)
        Me.picImage.Name = "picImage"
        Me.picImage.Size = New System.Drawing.Size(392, 248)
        Me.picImage.TabIndex = 3
        Me.picImage.TabStop = False
        '
        'ImageViewer
        '
        Me.ClientSize = New System.Drawing.Size(416, 350)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.lblImageFilename)
        Me.Controls.Add(Me.picImage)
        Me.Name = "ImageViewer"
        Me.Text = "ImageViewer"
        CType(Me.picImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents lblImageFilename As System.Windows.Forms.Label
    Friend WithEvents picImage As System.Windows.Forms.PictureBox
End Class
