
Partial Public Class MainForm
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        If disposing AndAlso (demoForm IsNot Nothing) Then
            demoForm.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.chkExtendFrame = New System.Windows.Forms.CheckBox()
        Me.tbLeft = New System.Windows.Forms.TextBox()
        Me.tbRight = New System.Windows.Forms.TextBox()
        Me.tbTop = New System.Windows.Forms.TextBox()
        Me.tbBottom = New System.Windows.Forms.TextBox()
        Me.lbLeft = New System.Windows.Forms.Label()
        Me.lbTop = New System.Windows.Forms.Label()
        Me.lbRight = New System.Windows.Forms.Label()
        Me.lbBottom = New System.Windows.Forms.Label()
        Me.chkBlurBehindWindow = New System.Windows.Forms.CheckBox()
        Me.tbX = New System.Windows.Forms.TextBox()
        Me.tbWidth = New System.Windows.Forms.TextBox()
        Me.tbY = New System.Windows.Forms.TextBox()
        Me.tbHeight = New System.Windows.Forms.TextBox()
        Me.lbX = New System.Windows.Forms.Label()
        Me.lbWidth = New System.Windows.Forms.Label()
        Me.lbY = New System.Windows.Forms.Label()
        Me.lbHeight = New System.Windows.Forms.Label()
        Me.btnApply = New System.Windows.Forms.Button()
        Me.lbAeroGlassStyleSupported = New System.Windows.Forms.Label()
        Me.chkEntendToEntireClientArea = New System.Windows.Forms.CheckBox()
        Me.chkEnableEntireFormBlurEffect = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'chkExtendFrame
        '
        Me.chkExtendFrame.AutoSize = True
        Me.chkExtendFrame.Location = New System.Drawing.Point(11, 47)
        Me.chkExtendFrame.Name = "chkExtendFrame"
        Me.chkExtendFrame.Size = New System.Drawing.Size(91, 17)
        Me.chkExtendFrame.TabIndex = 0
        Me.chkExtendFrame.Text = "Extend Frame"
        Me.chkExtendFrame.UseVisualStyleBackColor = True
        '
        'tbLeft
        '
        Me.tbLeft.Location = New System.Drawing.Point(52, 70)
        Me.tbLeft.Name = "tbLeft"
        Me.tbLeft.Size = New System.Drawing.Size(100, 20)
        Me.tbLeft.TabIndex = 1
        Me.tbLeft.Text = "30"
        '
        'tbRight
        '
        Me.tbRight.Location = New System.Drawing.Point(214, 70)
        Me.tbRight.Name = "tbRight"
        Me.tbRight.Size = New System.Drawing.Size(100, 20)
        Me.tbRight.TabIndex = 1
        Me.tbRight.Text = "60"
        '
        'tbTop
        '
        Me.tbTop.Location = New System.Drawing.Point(52, 96)
        Me.tbTop.Name = "tbTop"
        Me.tbTop.Size = New System.Drawing.Size(100, 20)
        Me.tbTop.TabIndex = 1
        Me.tbTop.Text = "40"
        '
        'tbBottom
        '
        Me.tbBottom.Location = New System.Drawing.Point(214, 96)
        Me.tbBottom.Name = "tbBottom"
        Me.tbBottom.Size = New System.Drawing.Size(100, 20)
        Me.tbBottom.TabIndex = 1
        Me.tbBottom.Text = "80"
        '
        'lbLeft
        '
        Me.lbLeft.AutoSize = True
        Me.lbLeft.Location = New System.Drawing.Point(11, 70)
        Me.lbLeft.Name = "lbLeft"
        Me.lbLeft.Size = New System.Drawing.Size(25, 13)
        Me.lbLeft.TabIndex = 2
        Me.lbLeft.Text = "Left"
        '
        'lbTop
        '
        Me.lbTop.AutoSize = True
        Me.lbTop.Location = New System.Drawing.Point(10, 96)
        Me.lbTop.Name = "lbTop"
        Me.lbTop.Size = New System.Drawing.Size(26, 13)
        Me.lbTop.TabIndex = 2
        Me.lbTop.Text = "Top"
        '
        'lbRight
        '
        Me.lbRight.AutoSize = True
        Me.lbRight.Location = New System.Drawing.Point(174, 73)
        Me.lbRight.Name = "lbRight"
        Me.lbRight.Size = New System.Drawing.Size(32, 13)
        Me.lbRight.TabIndex = 2
        Me.lbRight.Text = "Right"
        '
        'lbBottom
        '
        Me.lbBottom.AutoSize = True
        Me.lbBottom.Location = New System.Drawing.Point(173, 99)
        Me.lbBottom.Name = "lbBottom"
        Me.lbBottom.Size = New System.Drawing.Size(40, 13)
        Me.lbBottom.TabIndex = 2
        Me.lbBottom.Text = "Bottom"
        '
        'chkBlurBehindWindow
        '
        Me.chkBlurBehindWindow.AutoSize = True
        Me.chkBlurBehindWindow.Location = New System.Drawing.Point(11, 143)
        Me.chkBlurBehindWindow.Name = "chkBlurBehindWindow"
        Me.chkBlurBehindWindow.Size = New System.Drawing.Size(233, 17)
        Me.chkBlurBehindWindow.TabIndex = 0
        Me.chkBlurBehindWindow.Text = "Enable Blur Behind Window (Set the region)"
        Me.chkBlurBehindWindow.UseVisualStyleBackColor = True
        '
        'tbX
        '
        Me.tbX.Location = New System.Drawing.Point(77, 166)
        Me.tbX.Name = "tbX"
        Me.tbX.Size = New System.Drawing.Size(75, 20)
        Me.tbX.TabIndex = 1
        Me.tbX.Text = "100"
        '
        'tbWidth
        '
        Me.tbWidth.Location = New System.Drawing.Point(77, 192)
        Me.tbWidth.Name = "tbWidth"
        Me.tbWidth.Size = New System.Drawing.Size(75, 20)
        Me.tbWidth.TabIndex = 1
        Me.tbWidth.Text = "200"
        '
        'tbY
        '
        Me.tbY.Location = New System.Drawing.Point(236, 166)
        Me.tbY.Name = "tbY"
        Me.tbY.Size = New System.Drawing.Size(78, 20)
        Me.tbY.TabIndex = 1
        Me.tbY.Text = "100"
        '
        'tbHeight
        '
        Me.tbHeight.Location = New System.Drawing.Point(236, 192)
        Me.tbHeight.Name = "tbHeight"
        Me.tbHeight.Size = New System.Drawing.Size(78, 20)
        Me.tbHeight.TabIndex = 1
        Me.tbHeight.Text = "120"
        '
        'lbX
        '
        Me.lbX.AutoSize = True
        Me.lbX.Location = New System.Drawing.Point(11, 166)
        Me.lbX.Name = "lbX"
        Me.lbX.Size = New System.Drawing.Size(41, 13)
        Me.lbX.TabIndex = 2
        Me.lbX.Text = "Point.X"
        '
        'lbWidth
        '
        Me.lbWidth.AutoSize = True
        Me.lbWidth.Location = New System.Drawing.Point(10, 192)
        Me.lbWidth.Name = "lbWidth"
        Me.lbWidth.Size = New System.Drawing.Size(58, 13)
        Me.lbWidth.TabIndex = 2
        Me.lbWidth.Text = "Size.Width"
        '
        'lbY
        '
        Me.lbY.AutoSize = True
        Me.lbY.Location = New System.Drawing.Point(174, 169)
        Me.lbY.Name = "lbY"
        Me.lbY.Size = New System.Drawing.Size(41, 13)
        Me.lbY.TabIndex = 2
        Me.lbY.Text = "Point.Y"
        '
        'lbHeight
        '
        Me.lbHeight.AutoSize = True
        Me.lbHeight.Location = New System.Drawing.Point(173, 195)
        Me.lbHeight.Name = "lbHeight"
        Me.lbHeight.Size = New System.Drawing.Size(61, 13)
        Me.lbHeight.TabIndex = 2
        Me.lbHeight.Text = "Size.Height"
        '
        'btnApply
        '
        Me.btnApply.Location = New System.Drawing.Point(236, 245)
        Me.btnApply.Name = "btnApply"
        Me.btnApply.Size = New System.Drawing.Size(75, 23)
        Me.btnApply.TabIndex = 3
        Me.btnApply.Text = "Apply"
        Me.btnApply.UseVisualStyleBackColor = True
        '
        'lbAeroGlassStyleSupported
        '
        Me.lbAeroGlassStyleSupported.AutoSize = True
        Me.lbAeroGlassStyleSupported.Location = New System.Drawing.Point(11, 12)
        Me.lbAeroGlassStyleSupported.Name = "lbAeroGlassStyleSupported"
        Me.lbAeroGlassStyleSupported.Size = New System.Drawing.Size(136, 13)
        Me.lbAeroGlassStyleSupported.TabIndex = 0
        Me.lbAeroGlassStyleSupported.Text = "Aero Glass Style Supported"
        '
        'chkEntendToEntireClientArea
        '
        Me.chkEntendToEntireClientArea.AutoSize = True
        Me.chkEntendToEntireClientArea.Location = New System.Drawing.Point(52, 122)
        Me.chkEntendToEntireClientArea.Name = "chkEntendToEntireClientArea"
        Me.chkEntendToEntireClientArea.Size = New System.Drawing.Size(202, 17)
        Me.chkEntendToEntireClientArea.TabIndex = 0
        Me.chkEntendToEntireClientArea.Text = "Extend Frame to the entire client area"
        Me.chkEntendToEntireClientArea.UseVisualStyleBackColor = True
        '
        'chkEnableEntireFormBlurEffect
        '
        Me.chkEnableEntireFormBlurEffect.AutoSize = True
        Me.chkEnableEntireFormBlurEffect.Location = New System.Drawing.Point(48, 218)
        Me.chkEnableEntireFormBlurEffect.Name = "chkEnableEntireFormBlurEffect"
        Me.chkEnableEntireFormBlurEffect.Size = New System.Drawing.Size(243, 17)
        Me.chkEnableEntireFormBlurEffect.TabIndex = 0
        Me.chkEnableEntireFormBlurEffect.Text = "Enable Blur Behind Window on the entire form"
        Me.chkEnableEntireFormBlurEffect.UseVisualStyleBackColor = True
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(325, 280)
        Me.Controls.Add(Me.btnApply)
        Me.Controls.Add(Me.lbHeight)
        Me.Controls.Add(Me.lbBottom)
        Me.Controls.Add(Me.lbY)
        Me.Controls.Add(Me.lbRight)
        Me.Controls.Add(Me.lbWidth)
        Me.Controls.Add(Me.lbX)
        Me.Controls.Add(Me.lbTop)
        Me.Controls.Add(Me.tbHeight)
        Me.Controls.Add(Me.lbLeft)
        Me.Controls.Add(Me.tbY)
        Me.Controls.Add(Me.tbBottom)
        Me.Controls.Add(Me.tbWidth)
        Me.Controls.Add(Me.tbRight)
        Me.Controls.Add(Me.tbX)
        Me.Controls.Add(Me.tbTop)
        Me.Controls.Add(Me.chkEnableEntireFormBlurEffect)
        Me.Controls.Add(Me.chkBlurBehindWindow)
        Me.Controls.Add(Me.tbLeft)
        Me.Controls.Add(Me.lbAeroGlassStyleSupported)
        Me.Controls.Add(Me.chkEntendToEntireClientArea)
        Me.Controls.Add(Me.chkExtendFrame)
        Me.Name = "MainForm"
        Me.Text = "VBWinFormExAeroToClient"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private chkExtendFrame As CheckBox
    Private tbLeft As TextBox
    Private tbRight As TextBox
    Private tbTop As TextBox
    Private tbBottom As TextBox
    Private lbLeft As Label
    Private lbTop As Label
    Private lbRight As Label
    Private lbBottom As Label
    Private chkBlurBehindWindow As CheckBox
    Private tbX As TextBox
    Private tbWidth As TextBox
    Private tbY As TextBox
    Private tbHeight As TextBox
    Private lbX As Label
    Private lbWidth As Label
    Private lbY As Label
    Private lbHeight As Label
    Private WithEvents btnApply As Button
    Private lbAeroGlassStyleSupported As Label
    Private chkEntendToEntireClientArea As CheckBox
    Private chkEnableEntireFormBlurEffect As CheckBox

End Class
