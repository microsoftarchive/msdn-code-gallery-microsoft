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
        Me.DataValues = New System.Windows.Forms.GroupBox
        Me.txtZ = New System.Windows.Forms.MaskedTextBox
        Me.txtY = New System.Windows.Forms.MaskedTextBox
        Me.txtX = New System.Windows.Forms.MaskedTextBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.txtZAfter = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtYAfter = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.txtXAfter = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.lblz = New System.Windows.Forms.Label
        Me.lbly = New System.Windows.Forms.Label
        Me.lblx = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.cmdStandardDeserializationBinary = New System.Windows.Forms.Button
        Me.grpDefaultSerializationBinary = New System.Windows.Forms.GroupBox
        Me.cmdStandardSerializationBinary = New System.Windows.Forms.Button
        Me.cmdViewClass2 = New System.Windows.Forms.Button
        Me.txtView = New System.Windows.Forms.TextBox
        Me.cmdViewClass1 = New System.Windows.Forms.Button
        Me.cmdCustomSerialization = New System.Windows.Forms.Button
        Me.cmdStandardDeserializationSoap = New System.Windows.Forms.Button
        Me.cmdStandardSerializationSoap = New System.Windows.Forms.Button
        Me.cmdCustomDeserialization = New System.Windows.Forms.Button
        Me.grpCustomSerialization = New System.Windows.Forms.GroupBox
        Me.grpDefaultSerializationSoap = New System.Windows.Forms.GroupBox
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.DataValues.SuspendLayout()
        Me.grpDefaultSerializationBinary.SuspendLayout()
        Me.grpCustomSerialization.SuspendLayout()
        Me.grpDefaultSerializationSoap.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'DataValues
        '
        Me.DataValues.Controls.Add(Me.txtZ)
        Me.DataValues.Controls.Add(Me.txtY)
        Me.DataValues.Controls.Add(Me.txtX)
        Me.DataValues.Controls.Add(Me.Label5)
        Me.DataValues.Controls.Add(Me.txtZAfter)
        Me.DataValues.Controls.Add(Me.Label1)
        Me.DataValues.Controls.Add(Me.txtYAfter)
        Me.DataValues.Controls.Add(Me.Label2)
        Me.DataValues.Controls.Add(Me.txtXAfter)
        Me.DataValues.Controls.Add(Me.Label3)
        Me.DataValues.Controls.Add(Me.lblz)
        Me.DataValues.Controls.Add(Me.lbly)
        Me.DataValues.Controls.Add(Me.lblx)
        Me.DataValues.Controls.Add(Me.Label4)
        Me.DataValues.Location = New System.Drawing.Point(24, 47)
        Me.DataValues.Name = "DataValues"
        Me.DataValues.Size = New System.Drawing.Size(469, 126)
        Me.DataValues.TabIndex = 29
        Me.DataValues.TabStop = False
        Me.DataValues.Text = "Da&ta Values for Instance"
        '
        'txtZ
        '
        Me.txtZ.Location = New System.Drawing.Point(192, 71)
        Me.txtZ.Mask = "00000"
        Me.txtZ.Name = "txtZ"
        Me.txtZ.Size = New System.Drawing.Size(40, 20)
        Me.txtZ.TabIndex = 20
        Me.txtZ.Text = "3"
        Me.txtZ.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals
        '
        'txtY
        '
        Me.txtY.Location = New System.Drawing.Point(192, 44)
        Me.txtY.Mask = "00000"
        Me.txtY.Name = "txtY"
        Me.txtY.Size = New System.Drawing.Size(40, 20)
        Me.txtY.TabIndex = 19
        Me.txtY.Text = "2"
        Me.txtY.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals
        '
        'txtX
        '
        Me.txtX.Location = New System.Drawing.Point(192, 19)
        Me.txtX.Mask = "00000"
        Me.txtX.Name = "txtX"
        Me.txtX.Size = New System.Drawing.Size(40, 20)
        Me.txtX.TabIndex = 18
        Me.txtX.Text = "1"
        Me.txtX.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals
        '
        'Label5
        '
        Me.Label5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label5.Location = New System.Drawing.Point(253, 50)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(32, 24)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "&After"
        '
        'txtZAfter
        '
        Me.txtZAfter.Location = New System.Drawing.Point(406, 70)
        Me.txtZAfter.MaxLength = 3
        Me.txtZAfter.Name = "txtZAfter"
        Me.txtZAfter.ReadOnly = True
        Me.txtZAfter.Size = New System.Drawing.Size(40, 20)
        Me.txtZAfter.TabIndex = 17
        '
        'Label1
        '
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(285, 74)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(140, 16)
        Me.Label1.TabIndex = 16
        Me.Label1.Text = "NonSerializedVariable:"
        '
        'txtYAfter
        '
        Me.txtYAfter.Location = New System.Drawing.Point(406, 46)
        Me.txtYAfter.MaxLength = 3
        Me.txtYAfter.Name = "txtYAfter"
        Me.txtYAfter.ReadOnly = True
        Me.txtYAfter.Size = New System.Drawing.Size(40, 20)
        Me.txtYAfter.TabIndex = 15
        '
        'Label2
        '
        Me.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label2.Location = New System.Drawing.Point(285, 50)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(140, 16)
        Me.Label2.TabIndex = 14
        Me.Label2.Text = "PublicProperty:"
        '
        'txtXAfter
        '
        Me.txtXAfter.Location = New System.Drawing.Point(406, 22)
        Me.txtXAfter.MaxLength = 3
        Me.txtXAfter.Name = "txtXAfter"
        Me.txtXAfter.ReadOnly = True
        Me.txtXAfter.Size = New System.Drawing.Size(40, 20)
        Me.txtXAfter.TabIndex = 13
        '
        'Label3
        '
        Me.Label3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label3.Location = New System.Drawing.Point(285, 26)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(140, 16)
        Me.Label3.TabIndex = 12
        Me.Label3.Text = "PublicVariable:"
        '
        'lblz
        '
        Me.lblz.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblz.Location = New System.Drawing.Point(51, 74)
        Me.lblz.Name = "lblz"
        Me.lblz.Size = New System.Drawing.Size(124, 16)
        Me.lblz.TabIndex = 9
        Me.lblz.Text = "&NonSerializedVariable:"
        '
        'lbly
        '
        Me.lbly.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lbly.Location = New System.Drawing.Point(51, 50)
        Me.lbly.Name = "lbly"
        Me.lbly.Size = New System.Drawing.Size(124, 16)
        Me.lbly.TabIndex = 7
        Me.lbly.Text = "P&ublicProperty"
        '
        'lblx
        '
        Me.lblx.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblx.Location = New System.Drawing.Point(51, 26)
        Me.lblx.Name = "lblx"
        Me.lblx.Size = New System.Drawing.Size(124, 16)
        Me.lblx.TabIndex = 5
        Me.lblx.Text = "&PublicVariable:"
        '
        'Label4
        '
        Me.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label4.Location = New System.Drawing.Point(8, 48)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(48, 24)
        Me.Label4.TabIndex = 4
        Me.Label4.Text = "&Before"
        '
        'cmdStandardDeserializationBinary
        '
        Me.cmdStandardDeserializationBinary.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdStandardDeserializationBinary.Enabled = False
        Me.cmdStandardDeserializationBinary.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cmdStandardDeserializationBinary.Location = New System.Drawing.Point(16, 56)
        Me.cmdStandardDeserializationBinary.Name = "cmdStandardDeserializationBinary"
        Me.cmdStandardDeserializationBinary.Size = New System.Drawing.Size(200, 23)
        Me.cmdStandardDeserializationBinary.TabIndex = 20
        Me.cmdStandardDeserializationBinary.Text = "Deser&ialize SerializableClass"
        '
        'grpDefaultSerializationBinary
        '
        Me.grpDefaultSerializationBinary.AccessibleRole = System.Windows.Forms.AccessibleRole.None
        Me.grpDefaultSerializationBinary.Controls.Add(Me.cmdStandardDeserializationBinary)
        Me.grpDefaultSerializationBinary.Controls.Add(Me.cmdStandardSerializationBinary)
        Me.grpDefaultSerializationBinary.Location = New System.Drawing.Point(21, 190)
        Me.grpDefaultSerializationBinary.Name = "grpDefaultSerializationBinary"
        Me.grpDefaultSerializationBinary.Size = New System.Drawing.Size(232, 126)
        Me.grpDefaultSerializationBinary.TabIndex = 30
        Me.grpDefaultSerializationBinary.TabStop = False
        Me.grpDefaultSerializationBinary.Text = "Default Serialization with Bi&nary Formatter"
        '
        'cmdStandardSerializationBinary
        '
        Me.cmdStandardSerializationBinary.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdStandardSerializationBinary.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cmdStandardSerializationBinary.Location = New System.Drawing.Point(16, 24)
        Me.cmdStandardSerializationBinary.Name = "cmdStandardSerializationBinary"
        Me.cmdStandardSerializationBinary.Size = New System.Drawing.Size(200, 23)
        Me.cmdStandardSerializationBinary.TabIndex = 19
        Me.cmdStandardSerializationBinary.Text = "Se&rialize SerializableClass"
        '
        'cmdViewClass2
        '
        Me.cmdViewClass2.Enabled = False
        Me.cmdViewClass2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cmdViewClass2.Location = New System.Drawing.Point(16, 85)
        Me.cmdViewClass2.Name = "cmdViewClass2"
        Me.cmdViewClass2.Size = New System.Drawing.Size(200, 24)
        Me.cmdViewClass2.TabIndex = 33
        Me.cmdViewClass2.Text = "Vie&w CustomSerializableClass File"
        '
        'txtView
        '
        Me.txtView.Location = New System.Drawing.Point(24, 331)
        Me.txtView.Multiline = True
        Me.txtView.Name = "txtView"
        Me.txtView.ReadOnly = True
        Me.txtView.Size = New System.Drawing.Size(709, 232)
        Me.txtView.TabIndex = 34
        '
        'cmdViewClass1
        '
        Me.cmdViewClass1.Enabled = False
        Me.cmdViewClass1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cmdViewClass1.Location = New System.Drawing.Point(16, 85)
        Me.cmdViewClass1.Name = "cmdViewClass1"
        Me.cmdViewClass1.Size = New System.Drawing.Size(200, 24)
        Me.cmdViewClass1.TabIndex = 32
        Me.cmdViewClass1.Text = "&View SerializableClass File"
        '
        'cmdCustomSerialization
        '
        Me.cmdCustomSerialization.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdCustomSerialization.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cmdCustomSerialization.Location = New System.Drawing.Point(16, 24)
        Me.cmdCustomSerialization.Name = "cmdCustomSerialization"
        Me.cmdCustomSerialization.Size = New System.Drawing.Size(200, 23)
        Me.cmdCustomSerialization.TabIndex = 22
        Me.cmdCustomSerialization.Text = "Seria&lize CustomSerializableClass"
        '
        'cmdStandardDeserializationSoap
        '
        Me.cmdStandardDeserializationSoap.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdStandardDeserializationSoap.Enabled = False
        Me.cmdStandardDeserializationSoap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cmdStandardDeserializationSoap.Location = New System.Drawing.Point(16, 56)
        Me.cmdStandardDeserializationSoap.Name = "cmdStandardDeserializationSoap"
        Me.cmdStandardDeserializationSoap.Size = New System.Drawing.Size(200, 23)
        Me.cmdStandardDeserializationSoap.TabIndex = 2
        Me.cmdStandardDeserializationSoap.Text = "&Deserialize SerializableClass"
        '
        'cmdStandardSerializationSoap
        '
        Me.cmdStandardSerializationSoap.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdStandardSerializationSoap.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cmdStandardSerializationSoap.Location = New System.Drawing.Point(16, 24)
        Me.cmdStandardSerializationSoap.Name = "cmdStandardSerializationSoap"
        Me.cmdStandardSerializationSoap.Size = New System.Drawing.Size(200, 23)
        Me.cmdStandardSerializationSoap.TabIndex = 1
        Me.cmdStandardSerializationSoap.Text = "S&erialize SerializableClass"
        '
        'cmdCustomDeserialization
        '
        Me.cmdCustomDeserialization.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdCustomDeserialization.Enabled = False
        Me.cmdCustomDeserialization.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.cmdCustomDeserialization.Location = New System.Drawing.Point(16, 56)
        Me.cmdCustomDeserialization.Name = "cmdCustomDeserialization"
        Me.cmdCustomDeserialization.Size = New System.Drawing.Size(200, 23)
        Me.cmdCustomDeserialization.TabIndex = 23
        Me.cmdCustomDeserialization.Text = "Deseriali&ze CustomSerializableClass"
        '
        'grpCustomSerialization
        '
        Me.grpCustomSerialization.AccessibleRole = System.Windows.Forms.AccessibleRole.None
        Me.grpCustomSerialization.Controls.Add(Me.cmdCustomDeserialization)
        Me.grpCustomSerialization.Controls.Add(Me.cmdCustomSerialization)
        Me.grpCustomSerialization.Controls.Add(Me.cmdViewClass2)
        Me.grpCustomSerialization.Location = New System.Drawing.Point(514, 49)
        Me.grpCustomSerialization.Name = "grpCustomSerialization"
        Me.grpCustomSerialization.Size = New System.Drawing.Size(232, 124)
        Me.grpCustomSerialization.TabIndex = 31
        Me.grpCustomSerialization.TabStop = False
        Me.grpCustomSerialization.Text = "&Custom Serialization"
        '
        'grpDefaultSerializationSoap
        '
        Me.grpDefaultSerializationSoap.AccessibleRole = System.Windows.Forms.AccessibleRole.None
        Me.grpDefaultSerializationSoap.Controls.Add(Me.cmdStandardDeserializationSoap)
        Me.grpDefaultSerializationSoap.Controls.Add(Me.cmdStandardSerializationSoap)
        Me.grpDefaultSerializationSoap.Controls.Add(Me.cmdViewClass1)
        Me.grpDefaultSerializationSoap.Location = New System.Drawing.Point(277, 190)
        Me.grpDefaultSerializationSoap.Name = "grpDefaultSerializationSoap"
        Me.grpDefaultSerializationSoap.Size = New System.Drawing.Size(232, 126)
        Me.grpDefaultSerializationSoap.TabIndex = 28
        Me.grpDefaultSerializationSoap.TabStop = False
        Me.grpDefaultSerializationSoap.Text = "Default Serialization with &Soap Formatter"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(6, 2, 0, 2)
        Me.MenuStrip1.TabIndex = 35
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'fileToolStripMenuItem
        '
        Me.fileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.exitToolStripMenuItem})
        Me.fileToolStripMenuItem.Name = "fileToolStripMenuItem"
        
        Me.fileToolStripMenuItem.Text = "&File"
        '
        'exitToolStripMenuItem
        '
        Me.exitToolStripMenuItem.Name = "exitToolStripMenuItem"
        
        Me.exitToolStripMenuItem.Text = "E&xit"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 590)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(774, 18)
        Me.StatusStrip1.TabIndex = 36
        Me.StatusStrip1.Text = "FileLocation"
        '
        'Form1
        '
        Me.ClientSize = New System.Drawing.Size(774, 608)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.grpDefaultSerializationBinary)
        Me.Controls.Add(Me.txtView)
        Me.Controls.Add(Me.grpCustomSerialization)
        Me.Controls.Add(Me.grpDefaultSerializationSoap)
        Me.Controls.Add(Me.DataValues)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.DataValues.ResumeLayout(False)
        Me.DataValues.PerformLayout()
        Me.grpDefaultSerializationBinary.ResumeLayout(False)
        Me.grpCustomSerialization.ResumeLayout(False)
        Me.grpDefaultSerializationSoap.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents DataValues As System.Windows.Forms.GroupBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtZAfter As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtYAfter As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtXAfter As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents lblz As System.Windows.Forms.Label
    Friend WithEvents lbly As System.Windows.Forms.Label
    Friend WithEvents lblx As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents cmdStandardDeserializationBinary As System.Windows.Forms.Button
    Friend WithEvents grpDefaultSerializationBinary As System.Windows.Forms.GroupBox
    Friend WithEvents cmdStandardSerializationBinary As System.Windows.Forms.Button
    Friend WithEvents cmdViewClass2 As System.Windows.Forms.Button
    Friend WithEvents txtView As System.Windows.Forms.TextBox
    Friend WithEvents cmdViewClass1 As System.Windows.Forms.Button
    Friend WithEvents cmdCustomSerialization As System.Windows.Forms.Button
    Friend WithEvents cmdStandardDeserializationSoap As System.Windows.Forms.Button
    Friend WithEvents cmdStandardSerializationSoap As System.Windows.Forms.Button
    Friend WithEvents cmdCustomDeserialization As System.Windows.Forms.Button
    Friend WithEvents grpCustomSerialization As System.Windows.Forms.GroupBox
    Friend WithEvents grpDefaultSerializationSoap As System.Windows.Forms.GroupBox
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents txtZ As System.Windows.Forms.MaskedTextBox
    Friend WithEvents txtY As System.Windows.Forms.MaskedTextBox
    Friend WithEvents txtX As System.Windows.Forms.MaskedTextBox

End Class
