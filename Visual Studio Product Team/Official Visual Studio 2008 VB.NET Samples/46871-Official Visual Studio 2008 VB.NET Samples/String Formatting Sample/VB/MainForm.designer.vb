Partial Public Class MainForm
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
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
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.cboCultureInfoNumeric = New System.Windows.Forms.ComboBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.cboCultureInfoDateTime = New System.Windows.Forms.ComboBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.optCustomNumeric = New System.Windows.Forms.RadioButton
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.optCustomDateTime = New System.Windows.Forms.RadioButton
        Me.txtEnum = New System.Windows.Forms.TextBox
        Me.optStandardDateTime = New System.Windows.Forms.RadioButton
        Me.txtDateTime = New System.Windows.Forms.TextBox
        Me.pgeEnum = New System.Windows.Forms.TabPage
        Me.pgeDateTime = New System.Windows.Forms.TabPage
        Me.tabExamples = New System.Windows.Forms.TabControl
        Me.pgeNumeric = New System.Windows.Forms.TabPage
        Me.optStandardNumeric = New System.Windows.Forms.RadioButton
        Me.txtNumeric = New System.Windows.Forms.TextBox
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.pgeEnum.SuspendLayout()
        Me.pgeDateTime.SuspendLayout()
        Me.tabExamples.SuspendLayout()
        Me.pgeNumeric.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'cboCultureInfoNumeric
        '
        Me.cboCultureInfoNumeric.AccessibleDescription = "CultureInfo ComboBox for Numeric example"
        Me.cboCultureInfoNumeric.AccessibleName = "CultureInfo ComboBox for Numeric example"
        Me.cboCultureInfoNumeric.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboCultureInfoNumeric.FormattingEnabled = True
        Me.cboCultureInfoNumeric.ItemHeight = 13
        Me.cboCultureInfoNumeric.Location = New System.Drawing.Point(344, 8)
        Me.cboCultureInfoNumeric.Name = "cboCultureInfoNumeric"
        Me.cboCultureInfoNumeric.Size = New System.Drawing.Size(192, 21)
        Me.cboCultureInfoNumeric.TabIndex = 0
        '
        'Label4
        '
        Me.Label4.AccessibleDescription = "Label with text ""CultureInfo"""
        Me.Label4.AccessibleName = "CultureInfo label for Numeric example"
        Me.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label4.Location = New System.Drawing.Point(283, 11)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(64, 23)
        Me.Label4.TabIndex = 10
        Me.Label4.Text = "CultureInfo:"
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = "Label with text ""Numeric Format Strings"""
        Me.Label1.AccessibleName = "Numeric Format Strings title"
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(8, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(176, 23)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Numeric Format Strings"
        '
        'cboCultureInfoDateTime
        '
        Me.cboCultureInfoDateTime.AccessibleDescription = "CultureInfo ComboBox for Date-Time example"
        Me.cboCultureInfoDateTime.AccessibleName = "CultureInfo ComboBox for Date-Time example"
        Me.cboCultureInfoDateTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboCultureInfoDateTime.FormattingEnabled = True
        Me.cboCultureInfoDateTime.ItemHeight = 13
        Me.cboCultureInfoDateTime.Location = New System.Drawing.Point(344, 8)
        Me.cboCultureInfoDateTime.Name = "cboCultureInfoDateTime"
        Me.cboCultureInfoDateTime.Size = New System.Drawing.Size(192, 21)
        Me.cboCultureInfoDateTime.TabIndex = 0
        '
        'Label3
        '
        Me.Label3.AccessibleName = "CultureInfo label for DateTime example"
        Me.Label3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label3.Location = New System.Drawing.Point(283, 11)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(64, 23)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "CultureInfo:"
        '
        'optCustomNumeric
        '
        Me.optCustomNumeric.AccessibleDescription = "Custom Format String Example"
        Me.optCustomNumeric.AccessibleName = "Custom Format String Example"
        Me.optCustomNumeric.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.optCustomNumeric.Location = New System.Drawing.Point(476, 32)
        Me.optCustomNumeric.Name = "optCustomNumeric"
        Me.optCustomNumeric.Size = New System.Drawing.Size(72, 24)
        Me.optCustomNumeric.TabIndex = 2
        Me.optCustomNumeric.Text = "&Custom"
        '
        'Label2
        '
        Me.Label2.AccessibleDescription = "Label with text ""Date-Time Format Strings"""
        Me.Label2.AccessibleName = "Date-Time Format Strings title"
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label2.Location = New System.Drawing.Point(8, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(176, 23)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Date-Time Format Strings"
        '
        'Label5
        '
        Me.Label5.AccessibleDescription = "Label with text ""Enumeration Format Strings"""
        Me.Label5.AccessibleName = "Enumeration Format Strings title"
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.Label5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label5.Location = New System.Drawing.Point(8, 16)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(176, 23)
        Me.Label5.TabIndex = 8
        Me.Label5.Text = "Enumeration Format Strings"
        '
        'optCustomDateTime
        '
        Me.optCustomDateTime.AccessibleDescription = "Custom Format String Example"
        Me.optCustomDateTime.AccessibleName = "Custom Format String Example"
        Me.optCustomDateTime.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.optCustomDateTime.Location = New System.Drawing.Point(476, 32)
        Me.optCustomDateTime.Name = "optCustomDateTime"
        Me.optCustomDateTime.Size = New System.Drawing.Size(72, 24)
        Me.optCustomDateTime.TabIndex = 2
        Me.optCustomDateTime.Text = "&Custom"
        '
        'txtEnum
        '
        Me.txtEnum.AccessibleDescription = "TextBox to display Enumeration formatting"
        Me.txtEnum.AccessibleName = "Enumeration TextBox"
        Me.txtEnum.Location = New System.Drawing.Point(16, 56)
        Me.txtEnum.Multiline = True
        Me.txtEnum.Name = "txtEnum"
        Me.txtEnum.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtEnum.Size = New System.Drawing.Size(520, 216)
        Me.txtEnum.TabIndex = 1
        '
        'optStandardDateTime
        '
        Me.optStandardDateTime.AccessibleDescription = "Standard Format String Example"
        Me.optStandardDateTime.AccessibleName = "Standard Format String Example"
        Me.optStandardDateTime.Checked = True
        Me.optStandardDateTime.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.optStandardDateTime.Location = New System.Drawing.Point(388, 32)
        Me.optStandardDateTime.Name = "optStandardDateTime"
        Me.optStandardDateTime.Size = New System.Drawing.Size(72, 24)
        Me.optStandardDateTime.TabIndex = 1
        Me.optStandardDateTime.Text = "&Standard"
        '
        'txtDateTime
        '
        Me.txtDateTime.AccessibleDescription = "TextBox to display Date-Time formatting"
        Me.txtDateTime.AccessibleName = "Date-Time TextBox"
        Me.txtDateTime.Location = New System.Drawing.Point(16, 56)
        Me.txtDateTime.Multiline = True
        Me.txtDateTime.Name = "txtDateTime"
        Me.txtDateTime.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtDateTime.Size = New System.Drawing.Size(520, 216)
        Me.txtDateTime.TabIndex = 4
        '
        'pgeEnum
        '
        Me.pgeEnum.AccessibleDescription = "TabPage with text ""Enumeration"""
        Me.pgeEnum.AccessibleName = "Enumeration tab"
        Me.pgeEnum.Controls.Add(Me.Label5)
        Me.pgeEnum.Controls.Add(Me.txtEnum)
        Me.pgeEnum.Location = New System.Drawing.Point(4, 22)
        Me.pgeEnum.Name = "pgeEnum"
        Me.pgeEnum.Size = New System.Drawing.Size(552, 294)
        Me.pgeEnum.TabIndex = 2
        Me.pgeEnum.Text = "Enumeration"
        Me.pgeEnum.UseVisualStyleBackColor = False
        '
        'pgeDateTime
        '
        Me.pgeDateTime.AccessibleDescription = "TabPage with text ""Date-Time"""
        Me.pgeDateTime.AccessibleName = "Date-Time tab"
        Me.pgeDateTime.Controls.Add(Me.cboCultureInfoDateTime)
        Me.pgeDateTime.Controls.Add(Me.Label3)
        Me.pgeDateTime.Controls.Add(Me.Label2)
        Me.pgeDateTime.Controls.Add(Me.optCustomDateTime)
        Me.pgeDateTime.Controls.Add(Me.optStandardDateTime)
        Me.pgeDateTime.Controls.Add(Me.txtDateTime)
        Me.pgeDateTime.Location = New System.Drawing.Point(4, 22)
        Me.pgeDateTime.Name = "pgeDateTime"
        Me.pgeDateTime.Size = New System.Drawing.Size(552, 294)
        Me.pgeDateTime.TabIndex = 1
        Me.pgeDateTime.Text = "Date-Time"
        Me.pgeDateTime.UseVisualStyleBackColor = False
        '
        'tabExamples
        '
        Me.tabExamples.AccessibleDescription = "TabControl for application"
        Me.tabExamples.AccessibleName = "TabControl"
        Me.tabExamples.Controls.Add(Me.pgeNumeric)
        Me.tabExamples.Controls.Add(Me.pgeDateTime)
        Me.tabExamples.Controls.Add(Me.pgeEnum)
        Me.tabExamples.ItemSize = New System.Drawing.Size(51, 18)
        Me.tabExamples.Location = New System.Drawing.Point(12, 47)
        Me.tabExamples.Name = "tabExamples"
        Me.tabExamples.SelectedIndex = 0
        Me.tabExamples.Size = New System.Drawing.Size(560, 320)
        Me.tabExamples.TabIndex = 2
        '
        'pgeNumeric
        '
        Me.pgeNumeric.AccessibleDescription = "TabPage with text ""Numeric"""
        Me.pgeNumeric.AccessibleName = "Numeric tab"
        Me.pgeNumeric.Controls.Add(Me.cboCultureInfoNumeric)
        Me.pgeNumeric.Controls.Add(Me.Label4)
        Me.pgeNumeric.Controls.Add(Me.Label1)
        Me.pgeNumeric.Controls.Add(Me.optCustomNumeric)
        Me.pgeNumeric.Controls.Add(Me.optStandardNumeric)
        Me.pgeNumeric.Controls.Add(Me.txtNumeric)
        Me.pgeNumeric.Location = New System.Drawing.Point(4, 22)
        Me.pgeNumeric.Name = "pgeNumeric"
        Me.pgeNumeric.Size = New System.Drawing.Size(552, 294)
        Me.pgeNumeric.TabIndex = 0
        Me.pgeNumeric.Text = "Numeric"
        Me.pgeNumeric.UseVisualStyleBackColor = False
        '
        'optStandardNumeric
        '
        Me.optStandardNumeric.AccessibleDescription = "Standard Format String Example"
        Me.optStandardNumeric.AccessibleName = "Standard Format String Example"
        Me.optStandardNumeric.Checked = True
        Me.optStandardNumeric.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.optStandardNumeric.Location = New System.Drawing.Point(388, 32)
        Me.optStandardNumeric.Name = "optStandardNumeric"
        Me.optStandardNumeric.Size = New System.Drawing.Size(72, 24)
        Me.optStandardNumeric.TabIndex = 1
        Me.optStandardNumeric.Text = "&Standard"
        '
        'txtNumeric
        '
        Me.txtNumeric.AccessibleDescription = "TextBox to display numeric formatting"
        Me.txtNumeric.AccessibleName = "Numeric TextBox"
        Me.txtNumeric.Location = New System.Drawing.Point(16, 56)
        Me.txtNumeric.Multiline = True
        Me.txtNumeric.Name = "txtNumeric"
        Me.txtNumeric.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtNumeric.Size = New System.Drawing.Size(520, 216)
        Me.txtNumeric.TabIndex = 0
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(599, 24)
        Me.MenuStrip1.TabIndex = 3
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'fileToolStripMenuItem
        '
        Me.fileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.exitToolStripMenuItem})
        Me.fileToolStripMenuItem.Name = "fileToolStripMenuItem"
        Me.fileToolStripMenuItem.Size = New System.Drawing.Size(40, 20)
        Me.fileToolStripMenuItem.Text = "&File"
        '
        'exitToolStripMenuItem
        '
        Me.exitToolStripMenuItem.Name = "exitToolStripMenuItem"
        Me.exitToolStripMenuItem.Size = New System.Drawing.Size(109, 22)
        Me.exitToolStripMenuItem.Text = "E&xit"
        '
        'MainForm
        '
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(599, 388)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.tabExamples)
        Me.Name = "MainForm"
        Me.Text = "String Formatting Sample"
        Me.pgeEnum.ResumeLayout(False)
        Me.pgeEnum.PerformLayout()
        Me.pgeDateTime.ResumeLayout(False)
        Me.pgeDateTime.PerformLayout()
        Me.tabExamples.ResumeLayout(False)
        Me.pgeNumeric.ResumeLayout(False)
        Me.pgeNumeric.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cboCultureInfoNumeric As System.Windows.Forms.ComboBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cboCultureInfoDateTime As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents optCustomNumeric As System.Windows.Forms.RadioButton
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents optCustomDateTime As System.Windows.Forms.RadioButton
    Friend WithEvents txtEnum As System.Windows.Forms.TextBox
    Friend WithEvents optStandardDateTime As System.Windows.Forms.RadioButton
    Friend WithEvents txtDateTime As System.Windows.Forms.TextBox
    Friend WithEvents pgeEnum As System.Windows.Forms.TabPage
    Friend WithEvents pgeDateTime As System.Windows.Forms.TabPage
    Friend WithEvents tabExamples As System.Windows.Forms.TabControl
    Friend WithEvents pgeNumeric As System.Windows.Forms.TabPage
    Friend WithEvents optStandardNumeric As System.Windows.Forms.RadioButton
    Friend WithEvents txtNumeric As System.Windows.Forms.TextBox
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
