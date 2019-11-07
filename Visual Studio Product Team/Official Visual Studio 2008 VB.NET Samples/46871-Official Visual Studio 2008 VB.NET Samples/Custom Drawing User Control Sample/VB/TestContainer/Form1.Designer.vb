' Copyright (c) Microsoft Corporation. All rights reserved.
Partial Public Class Form1
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
        Me.ShowZerosCheckBox = New System.Windows.Forms.CheckBox
        Me.ColorDialog1 = New System.Windows.Forms.ColorDialog
        Me.ChangeFillColorButton = New System.Windows.Forms.Button
        Me.ChangeOutlineColorButton = New System.Windows.Forms.Button
        Me.ShowOutlineCheckBox = New System.Windows.Forms.CheckBox
        Me.ColorDialog2 = New System.Windows.Forms.ColorDialog
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.TrackBar1 = New System.Windows.Forms.TrackBar
        Me.TrackBar2 = New System.Windows.Forms.TrackBar
        Me.TrackBar3 = New System.Windows.Forms.TrackBar
        Me.Label3 = New System.Windows.Forms.Label
        Me.AddOneButton = New System.Windows.Forms.Button
        Me.SubtractOneButton = New System.Windows.Forms.Button
        Me.BeadedScoreBoard1 = New Scoreboard.BeadedScoreBoard
        Me.ScoreBoard1 = New Scoreboard.DigitalScoreBoard
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TrackBar2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TrackBar3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ShowZerosCheckBox
        '
        Me.ShowZerosCheckBox.AutoSize = True
        Me.ShowZerosCheckBox.Checked = True
        Me.ShowZerosCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ShowZerosCheckBox.Location = New System.Drawing.Point(185, 257)
        Me.ShowZerosCheckBox.Margin = New System.Windows.Forms.Padding(3, 1, 3, 3)
        Me.ShowZerosCheckBox.Name = "ShowZerosCheckBox"
        Me.ShowZerosCheckBox.Size = New System.Drawing.Size(124, 17)
        Me.ShowZerosCheckBox.TabIndex = 3
        Me.ShowZerosCheckBox.Text = "Show Leading Zeros"
        '
        'ChangeFillColorButton
        '
        Me.ChangeFillColorButton.Location = New System.Drawing.Point(11, 282)
        Me.ChangeFillColorButton.Name = "ChangeFillColorButton"
        Me.ChangeFillColorButton.Size = New System.Drawing.Size(149, 22)
        Me.ChangeFillColorButton.TabIndex = 4
        Me.ChangeFillColorButton.Text = "Change Fill Color"
        '
        'ChangeOutlineColorButton
        '
        Me.ChangeOutlineColorButton.Location = New System.Drawing.Point(11, 254)
        Me.ChangeOutlineColorButton.Name = "ChangeOutlineColorButton"
        Me.ChangeOutlineColorButton.Size = New System.Drawing.Size(149, 22)
        Me.ChangeOutlineColorButton.TabIndex = 6
        Me.ChangeOutlineColorButton.Text = "Change Outline Color"
        '
        'ShowOutlineCheckBox
        '
        Me.ShowOutlineCheckBox.AutoSize = True
        Me.ShowOutlineCheckBox.Checked = True
        Me.ShowOutlineCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ShowOutlineCheckBox.Location = New System.Drawing.Point(185, 285)
        Me.ShowOutlineCheckBox.Margin = New System.Windows.Forms.Padding(3, 1, 3, 3)
        Me.ShowOutlineCheckBox.Name = "ShowOutlineCheckBox"
        Me.ShowOutlineCheckBox.Size = New System.Drawing.Size(89, 17)
        Me.ShowOutlineCheckBox.TabIndex = 7
        Me.ShowOutlineCheckBox.Text = "Show Outline"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(11, 170)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(85, 13)
        Me.Label1.TabIndex = 10
        Me.Label1.Text = "Number of Digits"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(11, 210)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(35, 13)
        Me.Label2.TabIndex = 11
        Me.Label2.Text = "Score"
        '
        'TrackBar1
        '
        Me.TrackBar1.Location = New System.Drawing.Point(104, 170)
        Me.TrackBar1.Maximum = 8
        Me.TrackBar1.Minimum = 1
        Me.TrackBar1.Name = "TrackBar1"
        Me.TrackBar1.Size = New System.Drawing.Size(222, 50)
        Me.TrackBar1.TabIndex = 12
        Me.TrackBar1.Value = 4
        '
        'TrackBar2
        '
        Me.TrackBar2.Location = New System.Drawing.Point(52, 210)
        Me.TrackBar2.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.TrackBar2.Maximum = 9999
        Me.TrackBar2.Name = "TrackBar2"
        Me.TrackBar2.Size = New System.Drawing.Size(275, 50)
        Me.TrackBar2.TabIndex = 13
        Me.TrackBar2.TickFrequency = 0
        Me.TrackBar2.TickStyle = System.Windows.Forms.TickStyle.None
        Me.TrackBar2.Value = 123
        '
        'TrackBar3
        '
        Me.TrackBar3.Location = New System.Drawing.Point(105, 454)
        Me.TrackBar3.Margin = New System.Windows.Forms.Padding(1, 3, 3, 3)
        Me.TrackBar3.Maximum = 25
        Me.TrackBar3.Minimum = 1
        Me.TrackBar3.Name = "TrackBar3"
        Me.TrackBar3.Size = New System.Drawing.Size(223, 50)
        Me.TrackBar3.TabIndex = 15
        Me.TrackBar3.Value = 3
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 454)
        Me.Label3.Margin = New System.Windows.Forms.Padding(3, 3, 0, 3)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(89, 13)
        Me.Label3.TabIndex = 16
        Me.Label3.Text = "Number of Beads"
        '
        'AddOneButton
        '
        Me.AddOneButton.Location = New System.Drawing.Point(12, 425)
        Me.AddOneButton.Name = "AddOneButton"
        Me.AddOneButton.Size = New System.Drawing.Size(75, 23)
        Me.AddOneButton.TabIndex = 17
        Me.AddOneButton.Text = "Move Left"
        '
        'SubtractOneButton
        '
        Me.SubtractOneButton.Location = New System.Drawing.Point(239, 425)
        Me.SubtractOneButton.Name = "SubtractOneButton"
        Me.SubtractOneButton.Size = New System.Drawing.Size(89, 23)
        Me.SubtractOneButton.TabIndex = 18
        Me.SubtractOneButton.Text = "Move Right"
        '
        'BeadedScoreBoard1
        '
        Me.BeadedScoreBoard1.BeadColor = System.Drawing.Color.Black
        Me.BeadedScoreBoard1.BeadCount = 3
        Me.BeadedScoreBoard1.BeadOutlineColor = System.Drawing.Color.Black
        Me.BeadedScoreBoard1.Clickable = True
        Me.BeadedScoreBoard1.Location = New System.Drawing.Point(12, 319)
        Me.BeadedScoreBoard1.Name = "BeadedScoreBoard1"
        Me.BeadedScoreBoard1.Outline = True
        Me.BeadedScoreBoard1.Score = 0
        Me.BeadedScoreBoard1.Size = New System.Drawing.Size(314, 109)
        Me.BeadedScoreBoard1.TabIndex = 14
        '
        'ScoreBoard1
        '
        Me.ScoreBoard1.Digits = 4
        Me.ScoreBoard1.LeadingZeros = True
        Me.ScoreBoard1.Location = New System.Drawing.Point(28, 31)
        Me.ScoreBoard1.Margin = New System.Windows.Forms.Padding(3, 3, 3, 2)
        Me.ScoreBoard1.Name = "ScoreBoard1"
        Me.ScoreBoard1.NumberColor = System.Drawing.Color.Black
        Me.ScoreBoard1.Outline = True
        Me.ScoreBoard1.OutlineColor = System.Drawing.Color.Black
        Me.ScoreBoard1.Score = CType(123, Long)
        Me.ScoreBoard1.Size = New System.Drawing.Size(277, 135)
        Me.ScoreBoard1.Spacing = Scoreboard.DigitalScoreBoard.SpaceSize.Medium
        Me.ScoreBoard1.TabIndex = 0
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(338, 24)
        Me.MenuStrip1.TabIndex = 19
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(40, 20)
        Me.FileToolStripMenuItem.Text = "&File"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(96, 22)
        Me.ExitToolStripMenuItem.Text = "E&xit"
        '
        'Form1
        '
        Me.ClientSize = New System.Drawing.Size(338, 497)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.SubtractOneButton)
        Me.Controls.Add(Me.AddOneButton)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TrackBar3)
        Me.Controls.Add(Me.BeadedScoreBoard1)
        Me.Controls.Add(Me.TrackBar2)
        Me.Controls.Add(Me.TrackBar1)
        Me.Controls.Add(Me.ScoreBoard1)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.ChangeOutlineColorButton)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ShowOutlineCheckBox)
        Me.Controls.Add(Me.ChangeFillColorButton)
        Me.Controls.Add(Me.ShowZerosCheckBox)
        Me.Name = "Form1"
        Me.Text = "User Control Custom Drawing Sample"
        CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TrackBar2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TrackBar3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ShowZerosCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents ColorDialog1 As System.Windows.Forms.ColorDialog
    Friend WithEvents ChangeFillColorButton As System.Windows.Forms.Button
    Friend WithEvents ChangeOutlineColorButton As System.Windows.Forms.Button
    Friend WithEvents ShowOutlineCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents ColorDialog2 As System.Windows.Forms.ColorDialog
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents TrackBar1 As System.Windows.Forms.TrackBar
    Friend WithEvents TrackBar2 As System.Windows.Forms.TrackBar
    Friend WithEvents TrackBar3 As System.Windows.Forms.TrackBar
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents AddOneButton As System.Windows.Forms.Button
    Friend WithEvents SubtractOneButton As System.Windows.Forms.Button
    Friend WithEvents BeadedScoreBoard1 As ScoreBoard.BeadedScoreBoard
    Friend WithEvents ScoreBoard1 As ScoreBoard.DigitalScoreBoard
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
