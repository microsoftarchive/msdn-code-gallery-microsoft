Partial Public Class MainForm
    Inherits System.Windows.Forms.Form


    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub





    Friend WithEvents cboGradientMode As System.Windows.Forms.ComboBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents nudGradientBlend As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents nudRotation As System.Windows.Forms.NumericUpDown
    Friend WithEvents cboHatchStyle As System.Windows.Forms.ComboBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents cboWrapMode As System.Windows.Forms.ComboBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents cboBrushSize As System.Windows.Forms.ComboBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents btnSetColor2 As System.Windows.Forms.Button
    Friend WithEvents txtColor2 As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents cboDrawing As System.Windows.Forms.ComboBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents btnSetColor1 As System.Windows.Forms.Button
    Friend WithEvents txtColor1 As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents cboBrushType As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents picDemoArea As System.Windows.Forms.PictureBox



    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.cboGradientMode = New System.Windows.Forms.ComboBox
        Me.Label10 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.nudGradientBlend = New System.Windows.Forms.NumericUpDown
        Me.Label9 = New System.Windows.Forms.Label
        Me.nudRotation = New System.Windows.Forms.NumericUpDown
        Me.cboHatchStyle = New System.Windows.Forms.ComboBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.cboWrapMode = New System.Windows.Forms.ComboBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.cboBrushSize = New System.Windows.Forms.ComboBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.btnSetColor2 = New System.Windows.Forms.Button
        Me.txtColor2 = New System.Windows.Forms.TextBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.cboDrawing = New System.Windows.Forms.ComboBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.btnSetColor1 = New System.Windows.Forms.Button
        Me.txtColor1 = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.cboBrushType = New System.Windows.Forms.ComboBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.picDemoArea = New System.Windows.Forms.PictureBox
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.sbrDrawingStatus = New System.Windows.Forms.StatusStrip
        CType(Me.nudGradientBlend, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudRotation, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picDemoArea, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'cboGradientMode
        '
        Me.cboGradientMode.AccessibleDescription = "ComboBox selecting the Hatch Style to use."
        Me.cboGradientMode.AccessibleName = "Hatch Style Combo"
        Me.cboGradientMode.Enabled = False
        Me.cboGradientMode.FormattingEnabled = True
        Me.cboGradientMode.ItemHeight = 13
        Me.cboGradientMode.Location = New System.Drawing.Point(112, 333)
        Me.cboGradientMode.Name = "cboGradientMode"
        Me.cboGradientMode.Size = New System.Drawing.Size(176, 21)
        Me.cboGradientMode.TabIndex = 53
        Me.cboGradientMode.Text = "Horizontal"
        '
        'Label10
        '
        Me.Label10.AccessibleDescription = "Label with text ""Gradient Mode"""
        Me.Label10.AccessibleName = "Gradient Mode Label"
        Me.Label10.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label10.Location = New System.Drawing.Point(8, 333)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(104, 23)
        Me.Label10.TabIndex = 52
        Me.Label10.Text = "Gradient Mode:"
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = "Label with text ""Gradient Blend"""
        Me.Label1.AccessibleName = "Gradient Blend Label"
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(8, 301)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(104, 23)
        Me.Label1.TabIndex = 50
        Me.Label1.Text = "Gradient Blend:"
        '
        'nudGradientBlend
        '
        Me.nudGradientBlend.AccessibleDescription = "Up Down Numeric to detrmine Gradient Blend of brush."
        Me.nudGradientBlend.AccessibleName = "Gradient Blend UpDownNumeric"
        Me.nudGradientBlend.DecimalPlaces = 2
        Me.nudGradientBlend.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        Me.nudGradientBlend.Location = New System.Drawing.Point(112, 301)
        Me.nudGradientBlend.Maximum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudGradientBlend.Name = "nudGradientBlend"
        Me.nudGradientBlend.Size = New System.Drawing.Size(176, 20)
        Me.nudGradientBlend.TabIndex = 51
        Me.nudGradientBlend.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'Label9
        '
        Me.Label9.AccessibleDescription = "Label with text ""Rotation"""
        Me.Label9.AccessibleName = "Rotation Label"
        Me.Label9.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label9.Location = New System.Drawing.Point(8, 269)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(96, 23)
        Me.Label9.TabIndex = 48
        Me.Label9.Text = "Rotatation:"
        '
        'nudRotation
        '
        Me.nudRotation.AccessibleDescription = "Up Down Numeric to detrmine rotation angle of brush."
        Me.nudRotation.AccessibleName = "Rotation UpDownNumeric"
        Me.nudRotation.Increment = New Decimal(New Integer() {5, 0, 0, 0})
        Me.nudRotation.Location = New System.Drawing.Point(112, 269)
        Me.nudRotation.Maximum = New Decimal(New Integer() {180, 0, 0, 0})
        Me.nudRotation.Name = "nudRotation"
        Me.nudRotation.Size = New System.Drawing.Size(176, 20)
        Me.nudRotation.TabIndex = 49
        '
        'cboHatchStyle
        '
        Me.cboHatchStyle.AccessibleDescription = "ComboBox selecting the Hatch Style to use."
        Me.cboHatchStyle.AccessibleName = "Hatch Style Combo"
        Me.cboHatchStyle.FormattingEnabled = True
        Me.cboHatchStyle.ItemHeight = 13
        Me.cboHatchStyle.Location = New System.Drawing.Point(112, 237)
        Me.cboHatchStyle.Name = "cboHatchStyle"
        Me.cboHatchStyle.Size = New System.Drawing.Size(176, 21)
        Me.cboHatchStyle.TabIndex = 47
        Me.cboHatchStyle.Text = "Horizontal"
        '
        'Label8
        '
        Me.Label8.AccessibleDescription = "Label with text ""Hatch Style"""
        Me.Label8.AccessibleName = "Hatch Style Label"
        Me.Label8.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label8.Location = New System.Drawing.Point(8, 237)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(96, 23)
        Me.Label8.TabIndex = 46
        Me.Label8.Text = "Hatch Style:"
        '
        'cboWrapMode
        '
        Me.cboWrapMode.AccessibleDescription = "ComboBox selecting the Wrap Mode to use."
        Me.cboWrapMode.AccessibleName = "Wrap Mode Combo"
        Me.cboWrapMode.FormattingEnabled = True
        Me.cboWrapMode.ItemHeight = 13
        Me.cboWrapMode.Location = New System.Drawing.Point(112, 205)
        Me.cboWrapMode.Name = "cboWrapMode"
        Me.cboWrapMode.Size = New System.Drawing.Size(176, 21)
        Me.cboWrapMode.TabIndex = 45
        Me.cboWrapMode.Text = "Tile"
        '
        'Label7
        '
        Me.Label7.AccessibleDescription = "Label with text ""Wrap Mode"""
        Me.Label7.AccessibleName = "Wrap Mode Label"
        Me.Label7.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label7.Location = New System.Drawing.Point(8, 205)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(96, 23)
        Me.Label7.TabIndex = 44
        Me.Label7.Text = "Wrap Mode:"
        '
        'cboBrushSize
        '
        Me.cboBrushSize.AccessibleDescription = "ComboBox selecting the size of Brush to use."
        Me.cboBrushSize.AccessibleName = "Brush Size Combo"
        Me.cboBrushSize.FormattingEnabled = True
        Me.cboBrushSize.ItemHeight = 13
        Me.cboBrushSize.Items.AddRange(New Object() {"Large", "Medium", "Small"})
        Me.cboBrushSize.Location = New System.Drawing.Point(112, 173)
        Me.cboBrushSize.Name = "cboBrushSize"
        Me.cboBrushSize.Size = New System.Drawing.Size(176, 21)
        Me.cboBrushSize.TabIndex = 43
        '
        'Label6
        '
        Me.Label6.AccessibleDescription = "Label with text ""Brush Size"""
        Me.Label6.AccessibleName = "Brush Size Label"
        Me.Label6.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label6.Location = New System.Drawing.Point(8, 173)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(96, 23)
        Me.Label6.TabIndex = 42
        Me.Label6.Text = "Brush Size:"
        '
        'btnSetColor2
        '
        Me.btnSetColor2.AccessibleDescription = "Command that calls the ColorDialog for Color 2"
        Me.btnSetColor2.AccessibleName = "Command ColorDialog2t"
        Me.btnSetColor2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnSetColor2.Location = New System.Drawing.Point(262, 127)
        Me.btnSetColor2.Name = "btnSetColor2"
        Me.btnSetColor2.Size = New System.Drawing.Size(32, 20)
        Me.btnSetColor2.TabIndex = 41
        Me.btnSetColor2.Text = "&2..."
        '
        'txtColor2
        '
        Me.txtColor2.AccessibleDescription = "TextBox with name of selected color2"
        Me.txtColor2.AccessibleName = "Color2 TextBox"
        Me.txtColor2.Location = New System.Drawing.Point(112, 127)
        Me.txtColor2.Name = "txtColor2"
        Me.txtColor2.Size = New System.Drawing.Size(144, 20)
        Me.txtColor2.TabIndex = 40
        Me.txtColor2.Text = "Color [White]"
        '
        'Label5
        '
        Me.Label5.AccessibleDescription = "Label with text ""Color"""
        Me.Label5.AccessibleName = "Color2 Label"
        Me.Label5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label5.Location = New System.Drawing.Point(8, 125)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(96, 23)
        Me.Label5.TabIndex = 39
        Me.Label5.Text = "Color 2:"
        '
        'cboDrawing
        '
        Me.cboDrawing.AccessibleDescription = "ComboBox selecting the type of drawing to make."
        Me.cboDrawing.AccessibleName = "Drawing Type Combo"
        Me.cboDrawing.FormattingEnabled = True
        Me.cboDrawing.ItemHeight = 13
        Me.cboDrawing.Items.AddRange(New Object() {"Fill", "Ellipses", "Lines"})
        Me.cboDrawing.Location = New System.Drawing.Point(112, 61)
        Me.cboDrawing.Name = "cboDrawing"
        Me.cboDrawing.Size = New System.Drawing.Size(176, 21)
        Me.cboDrawing.TabIndex = 35
        '
        'Label4
        '
        Me.Label4.AccessibleDescription = "Label with text ""Drawing"""
        Me.Label4.AccessibleName = "Drawing Label"
        Me.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label4.Location = New System.Drawing.Point(8, 61)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(96, 23)
        Me.Label4.TabIndex = 34
        Me.Label4.Text = "Drawing:"
        '
        'btnSetColor1
        '
        Me.btnSetColor1.AccessibleDescription = "Command that calls the ColorDialog for Color 1"
        Me.btnSetColor1.AccessibleName = "Command ColorDialog1"
        Me.btnSetColor1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btnSetColor1.ImageIndex = 0
        Me.btnSetColor1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnSetColor1.Location = New System.Drawing.Point(262, 97)
        Me.btnSetColor1.Name = "btnSetColor1"
        Me.btnSetColor1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.btnSetColor1.Size = New System.Drawing.Size(32, 20)
        Me.btnSetColor1.TabIndex = 38
        Me.btnSetColor1.Text = "&1..."
        '
        'txtColor1
        '
        Me.txtColor1.AccessibleDescription = "TextBox with name of selected color1"
        Me.txtColor1.AccessibleName = "Color1 TextBox"
        Me.txtColor1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.txtColor1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.txtColor1.Location = New System.Drawing.Point(112, 98)
        Me.txtColor1.Name = "txtColor1"
        Me.txtColor1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.txtColor1.Size = New System.Drawing.Size(144, 20)
        Me.txtColor1.TabIndex = 37
        Me.txtColor1.Text = "Color [Blue]"
        Me.txtColor1.WordWrap = False
        '
        'Label3
        '
        Me.Label3.AccessibleDescription = "Label with text ""Color"""
        Me.Label3.AccessibleName = "Color1 Label"
        Me.Label3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label3.Location = New System.Drawing.Point(8, 101)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(96, 23)
        Me.Label3.TabIndex = 36
        Me.Label3.Text = "Color 1:"
        '
        'cboBrushType
        '
        Me.cboBrushType.AccessibleDescription = "ComboBox selecting the type of Brush to use."
        Me.cboBrushType.AccessibleName = "Brush Type Combo"
        Me.cboBrushType.FormattingEnabled = True
        Me.cboBrushType.ItemHeight = 13
        Me.cboBrushType.Items.AddRange(New Object() {"Solid", "Hatch", "Texture", "LinearGradient", "PathGradient"})
        Me.cboBrushType.Location = New System.Drawing.Point(112, 37)
        Me.cboBrushType.Name = "cboBrushType"
        Me.cboBrushType.Size = New System.Drawing.Size(176, 21)
        Me.cboBrushType.TabIndex = 33
        '
        'Label2
        '
        Me.Label2.AccessibleDescription = "Label with text ""Brush Type"""
        Me.Label2.AccessibleName = "Brush Type Label"
        Me.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label2.Location = New System.Drawing.Point(8, 37)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(96, 23)
        Me.Label2.TabIndex = 32
        Me.Label2.Text = "Brush Type:"
        '
        'picDemoArea
        '
        Me.picDemoArea.AccessibleDescription = "Area where the brushes are demonstrated"
        Me.picDemoArea.AccessibleName = "Demo Area PictureBox"
        Me.picDemoArea.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.picDemoArea.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picDemoArea.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.picDemoArea.Location = New System.Drawing.Point(306, 37)
        Me.picDemoArea.Name = "picDemoArea"
        Me.picDemoArea.Size = New System.Drawing.Size(312, 317)
        Me.picDemoArea.TabIndex = 31
        Me.picDemoArea.TabStop = False
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(626, 24)
        Me.MenuStrip1.TabIndex = 55
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
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(109, 22)
        Me.ExitToolStripMenuItem.Text = "E&xit"
        '
        'sbrDrawingStatus
        '
        Me.sbrDrawingStatus.Location = New System.Drawing.Point(0, 374)
        Me.sbrDrawingStatus.Name = "sbrDrawingStatus"
        Me.sbrDrawingStatus.Size = New System.Drawing.Size(626, 22)
        Me.sbrDrawingStatus.TabIndex = 56
        Me.sbrDrawingStatus.Text = "StatusStrip1"
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(626, 396)
        Me.Controls.Add(Me.sbrDrawingStatus)
        Me.Controls.Add(Me.cboGradientMode)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.nudGradientBlend)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.nudRotation)
        Me.Controls.Add(Me.cboHatchStyle)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.cboWrapMode)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.cboBrushSize)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.btnSetColor2)
        Me.Controls.Add(Me.txtColor2)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.cboDrawing)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.btnSetColor1)
        Me.Controls.Add(Me.txtColor1)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.cboBrushType)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.picDemoArea)
        Me.Controls.Add(Me.MenuStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "GDI Brushes Sample"
        CType(Me.nudGradientBlend, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudRotation, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picDemoArea, System.ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents sbrDrawingStatus As System.Windows.Forms.StatusStrip



End Class
