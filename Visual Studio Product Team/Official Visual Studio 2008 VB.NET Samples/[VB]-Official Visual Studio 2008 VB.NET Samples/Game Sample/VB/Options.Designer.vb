' Copyright (c) Microsoft Corporation. All rights reserved.
Public Partial Class Options
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
        Me.isSoundOn = New System.Windows.Forms.CheckBox
        Me.resetScores = New System.Windows.Forms.Button
        Me.DataGrid1 = New System.Windows.Forms.DataGrid
        CType(Me.DataGrid1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'isSoundOn
        '
        Me.isSoundOn.Location = New System.Drawing.Point(26, 138)
        Me.isSoundOn.Name = "isSoundOn"
        Me.isSoundOn.TabIndex = 6
        Me.isSoundOn.Text = "Sound"
        '
        'resetScores
        '
        Me.resetScores.Location = New System.Drawing.Point(330, 18)
        Me.resetScores.Name = "resetScores"
        Me.resetScores.TabIndex = 5
        Me.resetScores.Text = "Reset"
        '
        'DataGrid1
        '
        Me.DataGrid1.CaptionText = "High Scores"
        Me.DataGrid1.DataMember = ""
        Me.DataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText
        Me.DataGrid1.Location = New System.Drawing.Point(26, 18)
        Me.DataGrid1.Name = "DataGrid1"
        Me.DataGrid1.Size = New System.Drawing.Size(288, 104)
        Me.DataGrid1.TabIndex = 4
        '
        'Options
        '
        Me.ClientSize = New System.Drawing.Size(430, 180)
        Me.Controls.Add(Me.isSoundOn)
        Me.Controls.Add(Me.resetScores)
        Me.Controls.Add(Me.DataGrid1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "Options"
        Me.Text = "Options"
        CType(Me.DataGrid1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents isSoundOn As System.Windows.Forms.CheckBox
    Friend WithEvents resetScores As System.Windows.Forms.Button
    Friend WithEvents DataGrid1 As System.Windows.Forms.DataGrid
End Class
