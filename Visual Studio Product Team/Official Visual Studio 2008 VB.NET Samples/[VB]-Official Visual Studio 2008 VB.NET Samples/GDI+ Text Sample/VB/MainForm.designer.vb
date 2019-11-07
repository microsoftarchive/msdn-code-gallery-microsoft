Partial Public Class MainForm
    Inherits System.Windows.Forms.Form



    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
     Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub



    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents singleLinePage As System.Windows.Forms.TabPage


    Friend WithEvents picDemoArea As System.Windows.Forms.PictureBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents lstEffects As System.Windows.Forms.ListBox
    Friend WithEvents txtShortText As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents nudSkew As System.Windows.Forms.NumericUpDown
    Friend WithEvents effectDepth As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents optGradient As System.Windows.Forms.RadioButton
    Friend WithEvents optHatch As System.Windows.Forms.RadioButton
    Friend WithEvents nudFontSize As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents multiLinePage As System.Windows.Forms.TabPage
    Friend WithEvents multilineSize As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents picMultiLine As System.Windows.Forms.PictureBox
    Friend WithEvents txtLongText As System.Windows.Forms.TextBox



    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.TabControl1 = New System.Windows.Forms.TabControl
        Me.singleLinePage = New System.Windows.Forms.TabPage
        Me.picDemoArea = New System.Windows.Forms.PictureBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.lstEffects = New System.Windows.Forms.ListBox
        Me.txtShortText = New System.Windows.Forms.TextBox
        Me.Label11 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.nudSkew = New System.Windows.Forms.NumericUpDown
        Me.effectDepth = New System.Windows.Forms.NumericUpDown
        Me.Label12 = New System.Windows.Forms.Label
        Me.optGradient = New System.Windows.Forms.RadioButton
        Me.optHatch = New System.Windows.Forms.RadioButton
        Me.nudFontSize = New System.Windows.Forms.NumericUpDown
        Me.Label1 = New System.Windows.Forms.Label
        Me.multiLinePage = New System.Windows.Forms.TabPage
        Me.multilineSize = New System.Windows.Forms.NumericUpDown
        Me.Label2 = New System.Windows.Forms.Label
        Me.picMultiLine = New System.Windows.Forms.PictureBox
        Me.txtLongText = New System.Windows.Forms.TextBox
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.TabControl1.SuspendLayout()
        Me.singleLinePage.SuspendLayout()
        CType(Me.picDemoArea, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudSkew, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.effectDepth, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudFontSize, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.multiLinePage.SuspendLayout()
        CType(Me.multilineSize, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picMultiLine, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.singleLinePage)
        Me.TabControl1.Controls.Add(Me.multiLinePage)
        Me.TabControl1.ItemSize = New System.Drawing.Size(107, 21)
        Me.TabControl1.Location = New System.Drawing.Point(17, 47)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(728, 400)
        Me.TabControl1.TabIndex = 51
        '
        'singleLinePage
        '
        Me.singleLinePage.Controls.Add(Me.picDemoArea)
        Me.singleLinePage.Controls.Add(Me.Label6)
        Me.singleLinePage.Controls.Add(Me.Label5)
        Me.singleLinePage.Controls.Add(Me.lstEffects)
        Me.singleLinePage.Controls.Add(Me.txtShortText)
        Me.singleLinePage.Controls.Add(Me.Label11)
        Me.singleLinePage.Controls.Add(Me.Label4)
        Me.singleLinePage.Controls.Add(Me.nudSkew)
        Me.singleLinePage.Controls.Add(Me.effectDepth)
        Me.singleLinePage.Controls.Add(Me.Label12)
        Me.singleLinePage.Controls.Add(Me.optGradient)
        Me.singleLinePage.Controls.Add(Me.optHatch)
        Me.singleLinePage.Controls.Add(Me.nudFontSize)
        Me.singleLinePage.Controls.Add(Me.Label1)
        Me.singleLinePage.Location = New System.Drawing.Point(4, 25)
        Me.singleLinePage.Name = "singleLinePage"
        Me.singleLinePage.Size = New System.Drawing.Size(720, 371)
        Me.singleLinePage.TabIndex = 0
        Me.singleLinePage.Text = "Single Line Text"
        Me.singleLinePage.UseVisualStyleBackColor = False
        '
        'picDemoArea
        '
        Me.picDemoArea.AccessibleDescription = "Area where the text is demonstrated"
        Me.picDemoArea.AccessibleName = "Demo Area PictureBox"
        Me.picDemoArea.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.picDemoArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picDemoArea.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.picDemoArea.Location = New System.Drawing.Point(356, 8)
        Me.picDemoArea.Name = "picDemoArea"
        Me.picDemoArea.Size = New System.Drawing.Size(350, 350)
        Me.picDemoArea.TabIndex = 50
        Me.picDemoArea.TabStop = False
        '
        'Label6
        '
        Me.Label6.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label6.Location = New System.Drawing.Point(16, 200)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(100, 23)
        Me.Label6.TabIndex = 29
        Me.Label6.Text = "Effect"
        '
        'Label5
        '
        Me.Label5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label5.Location = New System.Drawing.Point(16, 88)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(100, 23)
        Me.Label5.TabIndex = 28
        Me.Label5.Text = "Effect"
        '
        'lstEffects
        '
        Me.lstEffects.FormattingEnabled = True
        Me.lstEffects.Items.AddRange(New Object() {"Brush", "Shadow", "Embossed", "Block", "Shear", "Reflect"})
        Me.lstEffects.Location = New System.Drawing.Point(144, 88)
        Me.lstEffects.Name = "lstEffects"
        Me.lstEffects.Size = New System.Drawing.Size(120, 95)
        Me.lstEffects.TabIndex = 27
        '
        'txtShortText
        '
        Me.txtShortText.AccessibleDescription = "TextBox where user can type in short text to be displayed."
        Me.txtShortText.AccessibleName = "Short Text TextBox"
        Me.txtShortText.Location = New System.Drawing.Point(144, 16)
        Me.txtShortText.MaxLength = 20
        Me.txtShortText.Name = "txtShortText"
        Me.txtShortText.Size = New System.Drawing.Size(168, 20)
        Me.txtShortText.TabIndex = 2
        Me.txtShortText.Text = "Sample"
        '
        'Label11
        '
        Me.Label11.AccessibleDescription = "Label with text ""Short Text"""
        Me.Label11.AccessibleName = "Short Text Label"
        Me.Label11.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label11.Location = New System.Drawing.Point(16, 16)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(100, 23)
        Me.Label11.TabIndex = 1
        Me.Label11.Text = "Text: "
        '
        'Label4
        '
        Me.Label4.AccessibleDescription = "Label with text ""Shear"""
        Me.Label4.AccessibleName = "Shear  Label"
        Me.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label4.Location = New System.Drawing.Point(16, 280)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(56, 24)
        Me.Label4.TabIndex = 25
        Me.Label4.Text = "Shear:"
        '
        'nudSkew
        '
        Me.nudSkew.AccessibleDescription = "Numeric UpDown allowing user to enter amount of shear."
        Me.nudSkew.AccessibleName = "Shear Numeric Up Down"
        Me.nudSkew.DecimalPlaces = 1
        Me.nudSkew.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        Me.nudSkew.Location = New System.Drawing.Point(152, 280)
        Me.nudSkew.Maximum = New Decimal(New Integer() {2, 0, 0, 0})
        Me.nudSkew.Minimum = New Decimal(New Integer() {2, 0, 0, -2147483648})
        Me.nudSkew.Name = "nudSkew"
        Me.nudSkew.Size = New System.Drawing.Size(56, 20)
        Me.nudSkew.TabIndex = 26
        Me.nudSkew.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'effectDepth
        '
        Me.effectDepth.AccessibleDescription = "Numeric UpDown allowing user to enter shadow depth."
        Me.effectDepth.AccessibleName = "Shadow Depth Numeric Up Down"
        Me.effectDepth.Location = New System.Drawing.Point(152, 240)
        Me.effectDepth.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.effectDepth.Name = "effectDepth"
        Me.effectDepth.Size = New System.Drawing.Size(56, 20)
        Me.effectDepth.TabIndex = 14
        Me.effectDepth.Value = New Decimal(New Integer() {8, 0, 0, 0})
        '
        'Label12
        '
        Me.Label12.AccessibleDescription = "Label with text ""Depth"""
        Me.Label12.AccessibleName = "Shadow Depth Label"
        Me.Label12.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label12.Location = New System.Drawing.Point(16, 240)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(56, 24)
        Me.Label12.TabIndex = 13
        Me.Label12.Text = "Depth:"
        '
        'optGradient
        '
        Me.optGradient.AccessibleDescription = "Radio Button to specify a Linear Gradient Brush."
        Me.optGradient.AccessibleName = "Gradient Brush Option"
        Me.optGradient.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.optGradient.Location = New System.Drawing.Point(208, 200)
        Me.optGradient.Name = "optGradient"
        Me.optGradient.Size = New System.Drawing.Size(88, 32)
        Me.optGradient.TabIndex = 10
        Me.optGradient.Text = "&Gradient"
        '
        'optHatch
        '
        Me.optHatch.AccessibleDescription = "Radio Button to specify a Hatch Brush."
        Me.optHatch.AccessibleName = "Hatch Brush Option"
        Me.optHatch.Checked = True
        Me.optHatch.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.optHatch.Location = New System.Drawing.Point(144, 200)
        Me.optHatch.Name = "optHatch"
        Me.optHatch.Size = New System.Drawing.Size(64, 32)
        Me.optHatch.TabIndex = 8
        Me.optHatch.Text = "&Hatch"
        '
        'nudFontSize
        '
        Me.nudFontSize.AccessibleDescription = "Numeric UpDown allowing user to enter font size"
        Me.nudFontSize.AccessibleName = "Font Size Numeric Up Down"
        Me.nudFontSize.Increment = New Decimal(New Integer() {5, 0, 0, 0})
        Me.nudFontSize.Location = New System.Drawing.Point(144, 48)
        Me.nudFontSize.Maximum = New Decimal(New Integer() {75, 0, 0, 0})
        Me.nudFontSize.Minimum = New Decimal(New Integer() {5, 0, 0, 0})
        Me.nudFontSize.Name = "nudFontSize"
        Me.nudFontSize.Size = New System.Drawing.Size(72, 20)
        Me.nudFontSize.TabIndex = 4
        Me.nudFontSize.Value = New Decimal(New Integer() {50, 0, 0, 0})
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = "Label with text ""Font Size"""
        Me.Label1.AccessibleName = "Font Size Label"
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(16, 48)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(100, 23)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Font Size:"
        '
        'multiLinePage
        '
        Me.multiLinePage.Controls.Add(Me.multilineSize)
        Me.multiLinePage.Controls.Add(Me.Label2)
        Me.multiLinePage.Controls.Add(Me.picMultiLine)
        Me.multiLinePage.Controls.Add(Me.txtLongText)
        Me.multiLinePage.Location = New System.Drawing.Point(4, 25)
        Me.multiLinePage.Name = "multiLinePage"
        Me.multiLinePage.Size = New System.Drawing.Size(720, 371)
        Me.multiLinePage.TabIndex = 1
        Me.multiLinePage.Text = "Multiline Text"
        Me.multiLinePage.UseVisualStyleBackColor = False
        '
        'multilineSize
        '
        Me.multilineSize.AccessibleDescription = "Numeric UpDown allowing user to enter font size"
        Me.multilineSize.AccessibleName = "Font Size Numeric Up Down"
        Me.multilineSize.Increment = New Decimal(New Integer() {5, 0, 0, 0})
        Me.multilineSize.Location = New System.Drawing.Point(144, 128)
        Me.multilineSize.Maximum = New Decimal(New Integer() {75, 0, 0, 0})
        Me.multilineSize.Minimum = New Decimal(New Integer() {5, 0, 0, 0})
        Me.multilineSize.Name = "multilineSize"
        Me.multilineSize.Size = New System.Drawing.Size(72, 20)
        Me.multilineSize.TabIndex = 53
        Me.multilineSize.Value = New Decimal(New Integer() {50, 0, 0, 0})
        '
        'Label2
        '
        Me.Label2.AccessibleDescription = "Label with text ""Font Size"""
        Me.Label2.AccessibleName = "Font Size Label"
        Me.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label2.Location = New System.Drawing.Point(16, 128)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(100, 23)
        Me.Label2.TabIndex = 52
        Me.Label2.Text = "Font Size:"
        '
        'picMultiLine
        '
        Me.picMultiLine.AccessibleDescription = "Area where the text is demonstrated"
        Me.picMultiLine.AccessibleName = "Demo Area PictureBox"
        Me.picMultiLine.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.picMultiLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picMultiLine.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.picMultiLine.Location = New System.Drawing.Point(319, 8)
        Me.picMultiLine.Name = "picMultiLine"
        Me.picMultiLine.Size = New System.Drawing.Size(350, 350)
        Me.picMultiLine.TabIndex = 51
        Me.picMultiLine.TabStop = False
        '
        'txtLongText
        '
        Me.txtLongText.AccessibleDescription = "TextBox where user can type in short text to be displayed"
        Me.txtLongText.AccessibleName = "Long Text TextBox"
        Me.txtLongText.Location = New System.Drawing.Point(16, 16)
        Me.txtLongText.Multiline = True
        Me.txtLongText.Name = "txtLongText"
        Me.txtLongText.Size = New System.Drawing.Size(248, 88)
        Me.txtLongText.TabIndex = 30
        Me.txtLongText.Text = "This is sample text that can be displayed in the box to the right. It contains mu" & _
            "ltiple lines."
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(762, 24)
        Me.MenuStrip1.TabIndex = 52
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
        Me.ClientSize = New System.Drawing.Size(762, 468)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "GDI Text Sample"
        Me.TabControl1.ResumeLayout(False)
        Me.singleLinePage.ResumeLayout(False)
        Me.singleLinePage.PerformLayout()
        CType(Me.picDemoArea, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudSkew, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.effectDepth, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudFontSize, System.ComponentModel.ISupportInitialize).EndInit()
        Me.multiLinePage.ResumeLayout(False)
        Me.multiLinePage.PerformLayout()
        CType(Me.multilineSize, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picMultiLine, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend Shared ReadOnly Property GetInstance() As MainForm
        Get
            If m_DefaultInstance Is Nothing OrElse m_DefaultInstance.IsDisposed() Then
                SyncLock m_SyncObject
                    If m_DefaultInstance Is Nothing OrElse m_DefaultInstance.IsDisposed() Then
                        m_DefaultInstance = New MainForm
                    End If
                End SyncLock
            End If
            Return m_DefaultInstance
        End Get
    End Property

    Private Shared m_DefaultInstance As MainForm
    Private Shared m_SyncObject As New Object
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem


End Class
