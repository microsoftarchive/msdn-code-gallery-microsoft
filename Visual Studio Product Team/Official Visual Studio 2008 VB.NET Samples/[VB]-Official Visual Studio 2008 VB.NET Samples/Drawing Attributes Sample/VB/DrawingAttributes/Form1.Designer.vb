<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        If disposing And myInkOverlay IsNot Nothing Then
            myInkOverlay.Dispose()
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
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.antiAliasCheckbox = New System.Windows.Forms.CheckBox
        Me.Button1 = New System.Windows.Forms.Button
        Me.pressureSensitiveCheckbox = New System.Windows.Forms.CheckBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.penTipEllipse = New System.Windows.Forms.RadioButton
        Me.penTipRectangle = New System.Windows.Forms.RadioButton
        Me.rasterOpsGroupBox = New System.Windows.Forms.GroupBox
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel
        Me.transparencyUpDown = New System.Windows.Forms.NumericUpDown
        Me.Label1 = New System.Windows.Forms.Label
        Me.heightUpDown = New System.Windows.Forms.NumericUpDown
        Me.widthUpDown = New System.Windows.Forms.NumericUpDown
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.GroupBox1.SuspendLayout()
        Me.rasterOpsGroupBox.SuspendLayout()
        CType(Me.transparencyUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.heightUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.widthUpDown, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.Panel1.Location = New System.Drawing.Point(232, 12)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(437, 239)
        Me.Panel1.TabIndex = 0
        '
        'antiAliasCheckbox
        '
        Me.antiAliasCheckbox.AutoSize = True
        Me.antiAliasCheckbox.Checked = True
        Me.antiAliasCheckbox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.antiAliasCheckbox.Location = New System.Drawing.Point(12, 12)
        Me.antiAliasCheckbox.Name = "antiAliasCheckbox"
        Me.antiAliasCheckbox.Size = New System.Drawing.Size(81, 17)
        Me.antiAliasCheckbox.TabIndex = 1
        Me.antiAliasCheckbox.Text = "Anti-Aliased"
        Me.antiAliasCheckbox.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(13, 36)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 2
        Me.Button1.Text = "Color"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'pressureSensitiveCheckbox
        '
        Me.pressureSensitiveCheckbox.AutoSize = True
        Me.pressureSensitiveCheckbox.Checked = True
        Me.pressureSensitiveCheckbox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.pressureSensitiveCheckbox.Location = New System.Drawing.Point(13, 66)
        Me.pressureSensitiveCheckbox.Name = "pressureSensitiveCheckbox"
        Me.pressureSensitiveCheckbox.Size = New System.Drawing.Size(113, 17)
        Me.pressureSensitiveCheckbox.TabIndex = 3
        Me.pressureSensitiveCheckbox.Text = "Pressure Sensitive"
        Me.pressureSensitiveCheckbox.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.penTipEllipse)
        Me.GroupBox1.Controls.Add(Me.penTipRectangle)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 89)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(101, 75)
        Me.GroupBox1.TabIndex = 4
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Pen Tip"
        '
        'penTipEllipse
        '
        Me.penTipEllipse.AutoSize = True
        Me.penTipEllipse.Location = New System.Drawing.Point(7, 44)
        Me.penTipEllipse.Name = "penTipEllipse"
        Me.penTipEllipse.Size = New System.Drawing.Size(42, 17)
        Me.penTipEllipse.TabIndex = 1
        Me.penTipEllipse.Text = "Ball"
        Me.penTipEllipse.UseVisualStyleBackColor = True
        '
        'penTipRectangle
        '
        Me.penTipRectangle.AutoSize = True
        Me.penTipRectangle.Checked = True
        Me.penTipRectangle.Location = New System.Drawing.Point(7, 20)
        Me.penTipRectangle.Name = "penTipRectangle"
        Me.penTipRectangle.Size = New System.Drawing.Size(74, 17)
        Me.penTipRectangle.TabIndex = 0
        Me.penTipRectangle.TabStop = True
        Me.penTipRectangle.Text = "Rectangle"
        Me.penTipRectangle.UseVisualStyleBackColor = True
        '
        'rasterOpsGroupBox
        '
        Me.rasterOpsGroupBox.Controls.Add(Me.FlowLayoutPanel1)
        Me.rasterOpsGroupBox.Location = New System.Drawing.Point(15, 257)
        Me.rasterOpsGroupBox.Name = "rasterOpsGroupBox"
        Me.rasterOpsGroupBox.Size = New System.Drawing.Size(654, 241)
        Me.rasterOpsGroupBox.TabIndex = 5
        Me.rasterOpsGroupBox.TabStop = False
        Me.rasterOpsGroupBox.Text = "Raster Operation"
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(3, 16)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(648, 222)
        Me.FlowLayoutPanel1.TabIndex = 0
        '
        'transparencyUpDown
        '
        Me.transparencyUpDown.Location = New System.Drawing.Point(90, 165)
        Me.transparencyUpDown.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        Me.transparencyUpDown.Name = "transparencyUpDown"
        Me.transparencyUpDown.Size = New System.Drawing.Size(120, 20)
        Me.transparencyUpDown.TabIndex = 6
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 167)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(72, 13)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "Transparency"
        '
        'heightUpDown
        '
        Me.heightUpDown.Increment = New Decimal(New Integer() {10, 0, 0, 0})
        Me.heightUpDown.Location = New System.Drawing.Point(89, 198)
        Me.heightUpDown.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.heightUpDown.Name = "heightUpDown"
        Me.heightUpDown.Size = New System.Drawing.Size(120, 20)
        Me.heightUpDown.TabIndex = 8
        Me.heightUpDown.Value = New Decimal(New Integer() {50, 0, 0, 0})
        '
        'widthUpDown
        '
        Me.widthUpDown.Increment = New Decimal(New Integer() {10, 0, 0, 0})
        Me.widthUpDown.Location = New System.Drawing.Point(89, 231)
        Me.widthUpDown.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.widthUpDown.Name = "widthUpDown"
        Me.widthUpDown.Size = New System.Drawing.Size(120, 20)
        Me.widthUpDown.TabIndex = 9
        Me.widthUpDown.Value = New Decimal(New Integer() {50, 0, 0, 0})
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 200)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(38, 13)
        Me.Label2.TabIndex = 10
        Me.Label2.Text = "Height"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 233)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(35, 13)
        Me.Label3.TabIndex = 11
        Me.Label3.Text = "Width"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(681, 510)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.widthUpDown)
        Me.Controls.Add(Me.heightUpDown)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.transparencyUpDown)
        Me.Controls.Add(Me.rasterOpsGroupBox)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.pressureSensitiveCheckbox)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.antiAliasCheckbox)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.rasterOpsGroupBox.ResumeLayout(False)
        CType(Me.transparencyUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.heightUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.widthUpDown, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents antiAliasCheckbox As System.Windows.Forms.CheckBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents pressureSensitiveCheckbox As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents penTipEllipse As System.Windows.Forms.RadioButton
    Friend WithEvents penTipRectangle As System.Windows.Forms.RadioButton
    Friend WithEvents rasterOpsGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents transparencyUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents heightUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents widthUpDown As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents FlowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel

End Class
