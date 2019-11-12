' Copyright (c) Microsoft Corporation. All rights reserved.
Public Partial Class PictureForm
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
        Me.MediaPictureBox = New System.Windows.Forms.PictureBox
        CType(Me.MediaPictureBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'MediaPictureBox
        '
        Me.MediaPictureBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.MediaPictureBox.Location = New System.Drawing.Point(0, 0)
        Me.MediaPictureBox.Name = "MediaPictureBox"
        Me.MediaPictureBox.Size = New System.Drawing.Size(497, 521)
        Me.MediaPictureBox.TabIndex = 0
        Me.MediaPictureBox.TabStop = False
        '
        'PictureForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(5, 13)
        Me.ClientSize = New System.Drawing.Size(497, 521)
        Me.Controls.Add(Me.MediaPictureBox)
        Me.Name = "PictureForm"
        Me.Text = "PictureForm"
        CType(Me.MediaPictureBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents MediaPictureBox As System.Windows.Forms.PictureBox
End Class
