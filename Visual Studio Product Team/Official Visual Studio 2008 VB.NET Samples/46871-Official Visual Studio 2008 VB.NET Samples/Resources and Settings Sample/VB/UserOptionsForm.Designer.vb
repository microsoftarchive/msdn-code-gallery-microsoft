
Partial Public Class UserOptionsForm
    Inherits System.Windows.Forms.Form

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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.Ok_Button = New System.Windows.Forms.Button
        Me.Cancel_Button = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.ColorDialog1 = New System.Windows.Forms.ColorDialog
        Me.Label4 = New System.Windows.Forms.Label
        Me.BackColorLabel = New System.Windows.Forms.Label
        Me.SelectBackColorButton = New System.Windows.Forms.Button
        Me.ForeColorLabel = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.FontSelectButton = New System.Windows.Forms.Button
        Me.FontNameTextBox = New System.Windows.Forms.Label
        Me.SelectForeGroundColorButton = New System.Windows.Forms.Button
        Me.OptionsPreviewTextBox = New System.Windows.Forms.TextBox
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.Ok_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(136, 216)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(166, 32)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'Ok_Button
        '
        Me.Ok_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Ok_Button.Location = New System.Drawing.Point(4, 3)
        Me.Ok_Button.Name = "Ok_Button"
        Me.Ok_Button.Size = New System.Drawing.Size(75, 25)
        Me.Ok_Button.TabIndex = 0
        Me.Ok_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(87, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(75, 25)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Cancel"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(45, 16)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Font: "
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 74)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(124, 16)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "Background Color:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(12, 134)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(62, 16)
        Me.Label4.TabIndex = 11
        Me.Label4.Text = "Sample: "
        '
        'BackColorLabel
        '
        Me.BackColorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.BackColorLabel.Location = New System.Drawing.Point(11, 91)
        Me.BackColorLabel.Name = "BackColorLabel"
        Me.BackColorLabel.Size = New System.Drawing.Size(92, 22)
        Me.BackColorLabel.TabIndex = 13
        '
        'SelectBackColorButton
        '
        Me.SelectBackColorButton.Location = New System.Drawing.Point(109, 89)
        Me.SelectBackColorButton.Name = "SelectBackColorButton"
        Me.SelectBackColorButton.Size = New System.Drawing.Size(27, 22)
        Me.SelectBackColorButton.TabIndex = 14
        Me.SelectBackColorButton.Text = "..."
        '
        'ForeColorLabel
        '
        Me.ForeColorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.ForeColorLabel.Location = New System.Drawing.Point(184, 93)
        Me.ForeColorLabel.Name = "ForeColorLabel"
        Me.ForeColorLabel.Size = New System.Drawing.Size(87, 21)
        Me.ForeColorLabel.TabIndex = 16
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(182, 74)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(122, 16)
        Me.Label6.TabIndex = 15
        Me.Label6.Text = "Foreground Color:"
        '
        'FontSelectButton
        '
        Me.FontSelectButton.Location = New System.Drawing.Point(280, 33)
        Me.FontSelectButton.Name = "FontSelectButton"
        Me.FontSelectButton.Size = New System.Drawing.Size(24, 22)
        Me.FontSelectButton.TabIndex = 20
        Me.FontSelectButton.Text = "..."
        '
        'FontNameTextBox
        '
        Me.FontNameTextBox.BackColor = System.Drawing.SystemColors.Window
        Me.FontNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.FontNameTextBox.Location = New System.Drawing.Point(13, 34)
        Me.FontNameTextBox.Name = "FontNameTextBox"
        Me.FontNameTextBox.Size = New System.Drawing.Size(261, 21)
        Me.FontNameTextBox.TabIndex = 19
        '
        'SelectForeGroundColorButton
        '
        Me.SelectForeGroundColorButton.Location = New System.Drawing.Point(277, 93)
        Me.SelectForeGroundColorButton.Name = "SelectForeGroundColorButton"
        Me.SelectForeGroundColorButton.Size = New System.Drawing.Size(27, 22)
        Me.SelectForeGroundColorButton.TabIndex = 21
        Me.SelectForeGroundColorButton.Text = "..."
        '
        'OptionsPreviewTextBox
        '
        Me.OptionsPreviewTextBox.Location = New System.Drawing.Point(11, 151)
        Me.OptionsPreviewTextBox.Multiline = True
        Me.OptionsPreviewTextBox.Name = "OptionsPreviewTextBox"
        Me.OptionsPreviewTextBox.Size = New System.Drawing.Size(289, 51)
        Me.OptionsPreviewTextBox.TabIndex = 22
        Me.OptionsPreviewTextBox.Text = "Sample Text"
        '
        'UserOptionsForm1
        '
        Me.AcceptButton = Me.Ok_Button
        Me.BackColor = SettingsAndResources.Settings.Default.BackColor
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(314, 261)
        Me.Controls.Add(Me.OptionsPreviewTextBox)
        Me.Controls.Add(Me.SelectForeGroundColorButton)
        Me.Controls.Add(Me.FontSelectButton)
        Me.Controls.Add(Me.FontNameTextBox)
        Me.Controls.Add(Me.ForeColorLabel)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.SelectBackColorButton)
        Me.Controls.Add(Me.BackColorLabel)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.DataBindings.Add(New System.Windows.Forms.Binding("Font", SettingsAndResources.Settings.Default, "Font", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.DataBindings.Add(New System.Windows.Forms.Binding("ForeColor", SettingsAndResources.Settings.Default, "ForeColor", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.DataBindings.Add(New System.Windows.Forms.Binding("BackColor", SettingsAndResources.Settings.Default, "BackColor", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.Font = SettingsAndResources.Settings.Default.Font
        Me.ForeColor = SettingsAndResources.Settings.Default.ForeColor
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "UserOptionsForm1"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Options"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents Ok_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents ColorDialog1 As System.Windows.Forms.ColorDialog
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents BackColorLabel As System.Windows.Forms.Label
    Friend WithEvents SelectBackColorButton As System.Windows.Forms.Button
    Friend WithEvents ForeColorLabel As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents FontSelectButton As System.Windows.Forms.Button
    Friend WithEvents FontNameTextBox As System.Windows.Forms.Label
    Friend WithEvents SelectForeGroundColorButton As System.Windows.Forms.Button
    Friend WithEvents OptionsPreviewTextBox As System.Windows.Forms.TextBox

End Class
