' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class FileSystemInfoBasePanel
    Inherits FileSystemSample.TaskPanelBase

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Protected Friend WithEvents sizeLabel As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Protected Friend WithEvents locationLabel As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Protected Friend WithEvents nameLabel As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Protected Friend WithEvents createdLabel As System.Windows.Forms.Label
    Protected Friend WithEvents modifiedLabel As System.Windows.Forms.Label
    Protected Friend WithEvents accessedLabel As System.Windows.Forms.Label
    Protected Friend WithEvents readOnlyCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Protected Friend WithEvents hiddenCheckBox As System.Windows.Forms.CheckBox


    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.sizeLabel = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.locationLabel = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.nameLabel = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.createdLabel = New System.Windows.Forms.Label
        Me.modifiedLabel = New System.Windows.Forms.Label
        Me.accessedLabel = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.readOnlyCheckBox = New System.Windows.Forms.CheckBox
        Me.hiddenCheckBox = New System.Windows.Forms.CheckBox
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'ExececuteMethodButton
        '
        Me.ExececuteMethodButton.Location = New System.Drawing.Point(170, 380)
        '
        'ResetValuesButton
        '
        Me.ResetValuesButton.Location = New System.Drawing.Point(300, 380)
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.hiddenCheckBox)
        Me.GroupBox2.Controls.Add(Me.readOnlyCheckBox)
        Me.GroupBox2.Controls.Add(Me.Label7)
        Me.GroupBox2.Controls.Add(Me.accessedLabel)
        Me.GroupBox2.Controls.Add(Me.modifiedLabel)
        Me.GroupBox2.Controls.Add(Me.createdLabel)
        Me.GroupBox2.Controls.Add(Me.Label6)
        Me.GroupBox2.Controls.Add(Me.Label5)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.sizeLabel)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Controls.Add(Me.locationLabel)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Controls.Add(Me.nameLabel)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Size = New System.Drawing.Size(592, 431)
        Me.GroupBox2.Controls.SetChildIndex(Me.ResetValuesButton, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.ExececuteMethodButton, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.EndParenLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.Label1, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.nameLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.Label2, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.locationLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.Label3, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.sizeLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.Label4, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.Label5, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.Label6, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.createdLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.modifiedLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.accessedLabel, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.Label7, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.readOnlyCheckBox, 0)
        Me.GroupBox2.Controls.SetChildIndex(Me.hiddenCheckBox, 0)
        '
        'sizeLabel
        '
        Me.sizeLabel.AutoSize = True
        Me.sizeLabel.Location = New System.Drawing.Point(285, 212)
        Me.sizeLabel.Name = "sizeLabel"
        Me.sizeLabel.Size = New System.Drawing.Size(38, 14)
        Me.sizeLabel.TabIndex = 21
        Me.sizeLabel.Text = "<size>"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(170, 212)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(26, 14)
        Me.Label3.TabIndex = 20
        Me.Label3.Text = "Size "
        '
        'locationLabel
        '
        Me.locationLabel.AutoSize = True
        Me.locationLabel.Location = New System.Drawing.Point(285, 191)
        Me.locationLabel.Name = "locationLabel"
        Me.locationLabel.Size = New System.Drawing.Size(57, 14)
        Me.locationLabel.TabIndex = 19
        Me.locationLabel.Text = "<location>"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(170, 191)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(50, 14)
        Me.Label2.TabIndex = 18
        Me.Label2.Text = "Location: "
        '
        'nameLabel
        '
        Me.nameLabel.AutoSize = True
        Me.nameLabel.Location = New System.Drawing.Point(285, 170)
        Me.nameLabel.Name = "nameLabel"
        Me.nameLabel.Size = New System.Drawing.Size(46, 14)
        Me.nameLabel.TabIndex = 17
        Me.nameLabel.Text = "<name>"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(170, 170)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(38, 14)
        Me.Label1.TabIndex = 16
        Me.Label1.Text = "Name: "
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(170, 255)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(48, 14)
        Me.Label4.TabIndex = 22
        Me.Label4.Text = "Created:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(170, 276)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(50, 14)
        Me.Label5.TabIndex = 23
        Me.Label5.Text = "Modified:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(170, 297)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(56, 14)
        Me.Label6.TabIndex = 24
        Me.Label6.Text = "Accessed:"
        '
        'createdLabel
        '
        Me.createdLabel.AutoSize = True
        Me.createdLabel.Location = New System.Drawing.Point(285, 255)
        Me.createdLabel.Name = "createdLabel"
        Me.createdLabel.Size = New System.Drawing.Size(55, 14)
        Me.createdLabel.TabIndex = 25
        Me.createdLabel.Text = "<created>"
        '
        'modifiedLabel
        '
        Me.modifiedLabel.AutoSize = True
        Me.modifiedLabel.Location = New System.Drawing.Point(285, 276)
        Me.modifiedLabel.Name = "modifiedLabel"
        Me.modifiedLabel.Size = New System.Drawing.Size(60, 14)
        Me.modifiedLabel.TabIndex = 26
        Me.modifiedLabel.Text = "<modified>"
        '
        'accessedLabel
        '
        Me.accessedLabel.AutoSize = True
        Me.accessedLabel.Location = New System.Drawing.Point(285, 297)
        Me.accessedLabel.Name = "accessedLabel"
        Me.accessedLabel.Size = New System.Drawing.Size(65, 14)
        Me.accessedLabel.TabIndex = 27
        Me.accessedLabel.Text = "<accessed>"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(170, 334)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(55, 14)
        Me.Label7.TabIndex = 28
        Me.Label7.Text = "Attributes:"
        '
        'readOnlyCheckBox
        '
        Me.readOnlyCheckBox.AutoSize = True
        Me.readOnlyCheckBox.Location = New System.Drawing.Point(285, 331)
        Me.readOnlyCheckBox.Name = "readOnlyCheckBox"
        Me.readOnlyCheckBox.Size = New System.Drawing.Size(70, 17)
        Me.readOnlyCheckBox.TabIndex = 29
        Me.readOnlyCheckBox.Text = "Read-only"
        '
        'hiddenCheckBox
        '
        Me.hiddenCheckBox.AutoSize = True
        Me.hiddenCheckBox.Location = New System.Drawing.Point(364, 332)
        Me.hiddenCheckBox.Name = "hiddenCheckBox"
        Me.hiddenCheckBox.Size = New System.Drawing.Size(56, 17)
        Me.hiddenCheckBox.TabIndex = 30
        Me.hiddenCheckBox.Text = "Hidden"
        '
        'FileSystemInfoBasePanel
        '
        Me.Name = "FileSystemInfoBasePanel"
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

End Class
