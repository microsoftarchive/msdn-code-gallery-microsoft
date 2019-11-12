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
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub





    Friend WithEvents btnCycle As System.Windows.Forms.Button
    Friend WithEvents comboTransform As System.Windows.Forms.ComboBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents updownMiterLimit As System.Windows.Forms.NumericUpDown
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents radioBrush As System.Windows.Forms.RadioButton
    Friend WithEvents radioColor As System.Windows.Forms.RadioButton
    Friend WithEvents comboLineStyle As System.Windows.Forms.ComboBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents comboLineJoin As System.Windows.Forms.ComboBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents comboEndCap As System.Windows.Forms.ComboBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents comboStartCap As System.Windows.Forms.ComboBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents updownWidth As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents comboShape As System.Windows.Forms.ComboBox
    Friend WithEvents pbLines As System.Windows.Forms.PictureBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnSetColor As System.Windows.Forms.Button
    Friend WithEvents txtColor As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents comboAlignment As System.Windows.Forms.ComboBox
    Friend WithEvents comboDashCap As System.Windows.Forms.ComboBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents comboBrush As System.Windows.Forms.ComboBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents timerCycle As System.Windows.Forms.Timer



    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.btnCycle = New System.Windows.Forms.Button
        Me.comboTransform = New System.Windows.Forms.ComboBox
        Me.Label12 = New System.Windows.Forms.Label
        Me.Label11 = New System.Windows.Forms.Label
        Me.updownMiterLimit = New System.Windows.Forms.NumericUpDown
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.radioBrush = New System.Windows.Forms.RadioButton
        Me.radioColor = New System.Windows.Forms.RadioButton
        Me.comboLineStyle = New System.Windows.Forms.ComboBox
        Me.Label9 = New System.Windows.Forms.Label
        Me.comboLineJoin = New System.Windows.Forms.ComboBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.comboEndCap = New System.Windows.Forms.ComboBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.comboStartCap = New System.Windows.Forms.ComboBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.updownWidth = New System.Windows.Forms.NumericUpDown
        Me.Label5 = New System.Windows.Forms.Label
        Me.comboShape = New System.Windows.Forms.ComboBox
        Me.pbLines = New System.Windows.Forms.PictureBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.btnSetColor = New System.Windows.Forms.Button
        Me.txtColor = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.comboAlignment = New System.Windows.Forms.ComboBox
        Me.comboDashCap = New System.Windows.Forms.ComboBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.comboBrush = New System.Windows.Forms.ComboBox
        Me.Label10 = New System.Windows.Forms.Label
        Me.timerCycle = New System.Windows.Forms.Timer(Me.components)
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        CType(Me.updownMiterLimit, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        CType(Me.updownWidth, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbLines, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnCycle
        '
        Me.btnCycle.AccessibleDescription = "Button to cause line to animate."
        Me.btnCycle.AccessibleName = "Animate Button"
        Me.btnCycle.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.btnCycle.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnCycle.Location = New System.Drawing.Point(217, 97)
        Me.btnCycle.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.btnCycle.Name = "btnCycle"
        Me.btnCycle.Size = New System.Drawing.Size(72, 23)
        Me.btnCycle.TabIndex = 31
        Me.btnCycle.Text = "&Animate"
        '
        'comboTransform
        '
        Me.comboTransform.AccessibleDescription = "ComboBox to determine Transform property."
        Me.comboTransform.AccessibleName = "Transform Combo"
        Me.comboTransform.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.comboTransform.FormattingEnabled = True
        Me.comboTransform.ItemHeight = 13
        Me.comboTransform.Items.AddRange(New Object() {"None", "Scale", "Rotate", "Translate"})
        Me.comboTransform.Location = New System.Drawing.Point(113, 465)
        Me.comboTransform.Name = "comboTransform"
        Me.comboTransform.Size = New System.Drawing.Size(176, 21)
        Me.comboTransform.TabIndex = 54
        Me.comboTransform.Text = "None"
        '
        'Label12
        '
        Me.Label12.AccessibleDescription = "Label with text ""Transform"""
        Me.Label12.AccessibleName = "Transform Label"
        Me.Label12.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Label12.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label12.Location = New System.Drawing.Point(9, 465)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(104, 23)
        Me.Label12.TabIndex = 53
        Me.Label12.Text = "&Transform:"
        '
        'Label11
        '
        Me.Label11.AccessibleDescription = "Label with text ""Miter Limit"""
        Me.Label11.AccessibleName = "Miter Limit Label"
        Me.Label11.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Label11.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label11.Location = New System.Drawing.Point(9, 385)
        Me.Label11.Margin = New System.Windows.Forms.Padding(3, 1, 3, 3)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(96, 23)
        Me.Label11.TabIndex = 49
        Me.Label11.Text = "&Miter Limit:"
        '
        'updownMiterLimit
        '
        Me.updownMiterLimit.AccessibleDescription = "Up Down Numeric to detrmine MiterLimit attribute."
        Me.updownMiterLimit.AccessibleName = "MiterLimit UpDownNumeric"
        Me.updownMiterLimit.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.updownMiterLimit.DecimalPlaces = 2
        Me.updownMiterLimit.Increment = New Decimal(New Integer() {25, 0, 0, 131072})
        Me.updownMiterLimit.Location = New System.Drawing.Point(113, 385)
        Me.updownMiterLimit.Margin = New System.Windows.Forms.Padding(3, 2, 3, 3)
        Me.updownMiterLimit.Maximum = New Decimal(New Integer() {15, 0, 0, 0})
        Me.updownMiterLimit.Name = "updownMiterLimit"
        Me.updownMiterLimit.Size = New System.Drawing.Size(176, 20)
        Me.updownMiterLimit.TabIndex = 50
        Me.updownMiterLimit.Value = New Decimal(New Integer() {4, 0, 0, 0})
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.GroupBox1.Controls.Add(Me.radioBrush)
        Me.GroupBox1.Controls.Add(Me.radioColor)
        Me.GroupBox1.Location = New System.Drawing.Point(112, 155)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(177, 40)
        Me.GroupBox1.TabIndex = 46
        Me.GroupBox1.TabStop = False
        '
        'radioBrush
        '
        Me.radioBrush.AccessibleDescription = "RadioButton to select Brush instead of Color."
        Me.radioBrush.AccessibleName = "Brush Radio"
        Me.radioBrush.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.radioBrush.Location = New System.Drawing.Point(85, 8)
        Me.radioBrush.Name = "radioBrush"
        Me.radioBrush.Size = New System.Drawing.Size(76, 24)
        Me.radioBrush.TabIndex = 12
        Me.radioBrush.Text = "Use Brush"
        '
        'radioColor
        '
        Me.radioColor.AccessibleDescription = "RadioButton to select Color instead of Brush."
        Me.radioColor.AccessibleName = "Color Radio"
        Me.radioColor.Checked = True
        Me.radioColor.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.radioColor.Location = New System.Drawing.Point(8, 8)
        Me.radioColor.Name = "radioColor"
        Me.radioColor.Size = New System.Drawing.Size(75, 24)
        Me.radioColor.TabIndex = 11
        Me.radioColor.Text = "Use Color"
        '
        'comboLineStyle
        '
        Me.comboLineStyle.AccessibleDescription = "Combo to accept style of line"
        Me.comboLineStyle.AccessibleName = "Line Style Combo"
        Me.comboLineStyle.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.comboLineStyle.FormattingEnabled = True
        Me.comboLineStyle.ItemHeight = 13
        Me.comboLineStyle.Location = New System.Drawing.Point(113, 97)
        Me.comboLineStyle.Name = "comboLineStyle"
        Me.comboLineStyle.Size = New System.Drawing.Size(104, 21)
        Me.comboLineStyle.TabIndex = 30
        Me.comboLineStyle.Text = "Solid"
        '
        'Label9
        '
        Me.Label9.AccessibleDescription = "Label with text ""line style"""
        Me.Label9.AccessibleName = "Line Style Label"
        Me.Label9.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Label9.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label9.Location = New System.Drawing.Point(9, 97)
        Me.Label9.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(104, 23)
        Me.Label9.TabIndex = 29
        Me.Label9.Text = "&Line Style:"
        '
        'comboLineJoin
        '
        Me.comboLineJoin.AccessibleDescription = "ComboBox to determine LineJoin property."
        Me.comboLineJoin.AccessibleName = "Line Join Combo"
        Me.comboLineJoin.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.comboLineJoin.FormattingEnabled = True
        Me.comboLineJoin.ItemHeight = 13
        Me.comboLineJoin.Location = New System.Drawing.Point(113, 361)
        Me.comboLineJoin.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.comboLineJoin.Name = "comboLineJoin"
        Me.comboLineJoin.Size = New System.Drawing.Size(176, 21)
        Me.comboLineJoin.TabIndex = 48
        Me.comboLineJoin.Text = "Miter"
        '
        'Label8
        '
        Me.Label8.AccessibleDescription = "Label with text ""Line Join"""
        Me.Label8.AccessibleName = "Line Join Label"
        Me.Label8.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Label8.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label8.Location = New System.Drawing.Point(9, 361)
        Me.Label8.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(96, 23)
        Me.Label8.TabIndex = 47
        Me.Label8.Text = "&Line Join:"
        '
        'comboEndCap
        '
        Me.comboEndCap.AccessibleDescription = "ComboBox to determine EndCap property."
        Me.comboEndCap.AccessibleName = "End Cap Combo"
        Me.comboEndCap.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.comboEndCap.FormattingEnabled = True
        Me.comboEndCap.ItemHeight = 13
        Me.comboEndCap.Location = New System.Drawing.Point(113, 297)
        Me.comboEndCap.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.comboEndCap.Name = "comboEndCap"
        Me.comboEndCap.Size = New System.Drawing.Size(176, 21)
        Me.comboEndCap.TabIndex = 43
        Me.comboEndCap.Text = "Flat"
        '
        'Label6
        '
        Me.Label6.AccessibleDescription = "Label with text ""End Cap"""
        Me.Label6.AccessibleName = "End Cap Label"
        Me.Label6.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Label6.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label6.Location = New System.Drawing.Point(9, 297)
        Me.Label6.Margin = New System.Windows.Forms.Padding(3, 1, 3, 1)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(96, 23)
        Me.Label6.TabIndex = 42
        Me.Label6.Text = "&End Cap:"
        '
        'comboStartCap
        '
        Me.comboStartCap.AccessibleDescription = "ComboBox to determine StartCap property."
        Me.comboStartCap.AccessibleName = "Start Cap combo"
        Me.comboStartCap.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.comboStartCap.FormattingEnabled = True
        Me.comboStartCap.ItemHeight = 13
        Me.comboStartCap.Location = New System.Drawing.Point(113, 273)
        Me.comboStartCap.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.comboStartCap.Name = "comboStartCap"
        Me.comboStartCap.Size = New System.Drawing.Size(176, 21)
        Me.comboStartCap.TabIndex = 41
        Me.comboStartCap.Text = "Flat"
        '
        'Label4
        '
        Me.Label4.AccessibleDescription = "Label with text 'width'"
        Me.Label4.AccessibleName = "Width label"
        Me.Label4.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label4.Location = New System.Drawing.Point(9, 121)
        Me.Label4.Margin = New System.Windows.Forms.Padding(3, 1, 3, 3)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(96, 23)
        Me.Label4.TabIndex = 32
        Me.Label4.Text = "&Width:"
        '
        'updownWidth
        '
        Me.updownWidth.AccessibleDescription = "Up Down Numeric to detrmine width of the pen"
        Me.updownWidth.AccessibleName = "Width UpDownNumeric"
        Me.updownWidth.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.updownWidth.Location = New System.Drawing.Point(113, 121)
        Me.updownWidth.Margin = New System.Windows.Forms.Padding(3, 1, 3, 3)
        Me.updownWidth.Maximum = New Decimal(New Integer() {50, 0, 0, 0})
        Me.updownWidth.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.updownWidth.Name = "updownWidth"
        Me.updownWidth.Size = New System.Drawing.Size(176, 20)
        Me.updownWidth.TabIndex = 33
        Me.updownWidth.Value = New Decimal(New Integer() {25, 0, 0, 0})
        '
        'Label5
        '
        Me.Label5.AccessibleDescription = "Label with text 'Drawing'"
        Me.Label5.AccessibleName = "Drawing Label"
        Me.Label5.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Label5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label5.Location = New System.Drawing.Point(9, 57)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(96, 23)
        Me.Label5.TabIndex = 27
        Me.Label5.Text = "&Drawing:"
        '
        'comboShape
        '
        Me.comboShape.AccessibleDescription = "Combo Box determining what types of lines should be drawn"
        Me.comboShape.AccessibleName = "Lines Combo"
        Me.comboShape.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.comboShape.FormattingEnabled = True
        Me.comboShape.ItemHeight = 13
        Me.comboShape.Items.AddRange(New Object() {"Lines", "Intersecting Lines", "Circles and Curves"})
        Me.comboShape.Location = New System.Drawing.Point(113, 57)
        Me.comboShape.Name = "comboShape"
        Me.comboShape.Size = New System.Drawing.Size(176, 21)
        Me.comboShape.TabIndex = 28
        Me.comboShape.Text = "Lines"
        '
        'pbLines
        '
        Me.pbLines.AccessibleDescription = "PictureBox where the Pen is used."
        Me.pbLines.AccessibleName = "Picture"
        Me.pbLines.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.pbLines.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pbLines.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.pbLines.Location = New System.Drawing.Point(322, 65)
        Me.pbLines.Name = "pbLines"
        Me.pbLines.Size = New System.Drawing.Size(345, 432)
        Me.pbLines.TabIndex = 34
        Me.pbLines.TabStop = False
        '
        'Label3
        '
        Me.Label3.AccessibleDescription = "Label with text ""Start Cap"""
        Me.Label3.AccessibleName = "Start Cap Label"
        Me.Label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Label3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label3.Location = New System.Drawing.Point(9, 273)
        Me.Label3.Margin = New System.Windows.Forms.Padding(3, 3, 3, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(104, 23)
        Me.Label3.TabIndex = 40
        Me.Label3.Text = "&Start Cap:"
        '
        'Label2
        '
        Me.Label2.AccessibleDescription = "Label with text ""Alignment"""
        Me.Label2.AccessibleName = "Alignment Label"
        Me.Label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label2.Location = New System.Drawing.Point(9, 425)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(96, 23)
        Me.Label2.TabIndex = 51
        Me.Label2.Text = "&Alignment:"
        '
        'btnSetColor
        '
        Me.btnSetColor.AccessibleDescription = "Command that calls the ColorDialog"
        Me.btnSetColor.AccessibleName = "Command Color Select"
        Me.btnSetColor.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.btnSetColor.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnSetColor.Location = New System.Drawing.Point(263, 209)
        Me.btnSetColor.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.btnSetColor.Name = "btnSetColor"
        Me.btnSetColor.Size = New System.Drawing.Size(32, 23)
        Me.btnSetColor.TabIndex = 38
        Me.btnSetColor.Text = "&..."
        '
        'txtColor
        '
        Me.txtColor.AccessibleDescription = "TextBox with name of selected color"
        Me.txtColor.AccessibleName = "Color TextBox"
        Me.txtColor.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.txtColor.Location = New System.Drawing.Point(113, 209)
        Me.txtColor.Name = "txtColor"
        Me.txtColor.Size = New System.Drawing.Size(144, 20)
        Me.txtColor.TabIndex = 36
        Me.txtColor.Text = "Color [BurleyWood]"
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = "Label with text ""Color"""
        Me.Label1.AccessibleName = "Color Label"
        Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(9, 209)
        Me.Label1.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(96, 23)
        Me.Label1.TabIndex = 35
        Me.Label1.Text = "&Color:"
        '
        'comboAlignment
        '
        Me.comboAlignment.AccessibleDescription = "ComboBox to determine Alignment property."
        Me.comboAlignment.AccessibleName = "Alignment Combo"
        Me.comboAlignment.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.comboAlignment.FormattingEnabled = True
        Me.comboAlignment.ItemHeight = 13
        Me.comboAlignment.Location = New System.Drawing.Point(113, 425)
        Me.comboAlignment.Name = "comboAlignment"
        Me.comboAlignment.Size = New System.Drawing.Size(176, 21)
        Me.comboAlignment.TabIndex = 52
        Me.comboAlignment.Text = "Center"
        '
        'comboDashCap
        '
        Me.comboDashCap.AccessibleDescription = "ComboBox to determine DashCap property."
        Me.comboDashCap.AccessibleName = "Dash Cap Combo"
        Me.comboDashCap.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.comboDashCap.FormattingEnabled = True
        Me.comboDashCap.ItemHeight = 13
        Me.comboDashCap.Location = New System.Drawing.Point(113, 321)
        Me.comboDashCap.Margin = New System.Windows.Forms.Padding(3, 1, 3, 3)
        Me.comboDashCap.Name = "comboDashCap"
        Me.comboDashCap.Size = New System.Drawing.Size(176, 21)
        Me.comboDashCap.TabIndex = 45
        Me.comboDashCap.Text = "Flat"
        '
        'Label7
        '
        Me.Label7.AccessibleDescription = "Label with text ""Dash Cap"""
        Me.Label7.AccessibleName = "Dash Cap Label"
        Me.Label7.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Label7.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label7.Location = New System.Drawing.Point(9, 321)
        Me.Label7.Margin = New System.Windows.Forms.Padding(3, 0, 3, 3)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(96, 23)
        Me.Label7.TabIndex = 44
        Me.Label7.Text = "&Dash Cap:"
        '
        'comboBrush
        '
        Me.comboBrush.AccessibleDescription = "ComboBox to determine which brush to use."
        Me.comboBrush.AccessibleName = "Brush Combo"
        Me.comboBrush.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.comboBrush.FormattingEnabled = True
        Me.comboBrush.ItemHeight = 13
        Me.comboBrush.Items.AddRange(New Object() {"Solid", "Hatch", "Texture", "Gradient"})
        Me.comboBrush.Location = New System.Drawing.Point(113, 233)
        Me.comboBrush.Margin = New System.Windows.Forms.Padding(3, 0, 3, 3)
        Me.comboBrush.Name = "comboBrush"
        Me.comboBrush.Size = New System.Drawing.Size(176, 21)
        Me.comboBrush.TabIndex = 39
        '
        'Label10
        '
        Me.Label10.AccessibleDescription = "Label with text ""Brush"""
        Me.Label10.AccessibleName = "Brush Label"
        Me.Label10.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Label10.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label10.Location = New System.Drawing.Point(9, 233)
        Me.Label10.Margin = New System.Windows.Forms.Padding(3, 0, 3, 3)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(104, 23)
        Me.Label10.TabIndex = 37
        Me.Label10.Text = "&Brush:"
        '
        'timerCycle
        '
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(676, 24)
        Me.MenuStrip1.TabIndex = 55
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
        Me.ClientSize = New System.Drawing.Size(676, 507)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.comboBrush)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.comboDashCap)
        Me.Controls.Add(Me.comboAlignment)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.comboTransform)
        Me.Controls.Add(Me.btnSetColor)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.comboStartCap)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.comboEndCap)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.comboLineJoin)
        Me.Controls.Add(Me.updownMiterLimit)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.txtColor)
        Me.Controls.Add(Me.btnCycle)
        Me.Controls.Add(Me.comboLineStyle)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.updownWidth)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.comboShape)
        Me.Controls.Add(Me.pbLines)
        Me.Controls.Add(Me.MenuStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "GDI Pens Sample"
        CType(Me.updownMiterLimit, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        CType(Me.updownWidth, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbLines, System.ComponentModel.ISupportInitialize).EndInit()
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
