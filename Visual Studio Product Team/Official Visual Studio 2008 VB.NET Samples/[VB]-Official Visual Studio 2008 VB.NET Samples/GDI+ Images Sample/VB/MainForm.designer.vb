' Copyright (c) Microsoft Corporation. All rights reserved.
Partial Public Class MainForm
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
        Me.MainImage = New System.Windows.Forms.PictureBox
        Me.XLabel = New System.Windows.Forms.Label
        Me.YLabel = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Cropping = New System.Windows.Forms.GroupBox
        Me.TestButton = New System.Windows.Forms.Button
        Me.CropButton = New System.Windows.Forms.Button
        Me.HeightInput = New System.Windows.Forms.TextBox
        Me.WidthInput = New System.Windows.Forms.TextBox
        Me.LeftInput = New System.Windows.Forms.TextBox
        Me.TopInput = New System.Windows.Forms.TextBox
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog
        Me.Resizing = New System.Windows.Forms.GroupBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.ResizeButton = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtPercent = New System.Windows.Forms.TextBox
        Me.ZoomLabel = New System.Windows.Forms.Label
        Me.WidthLabel = New System.Windows.Forms.Label
        Me.HeightLabel = New System.Windows.Forms.Label
        Me.ImageInfo = New System.Windows.Forms.GroupBox
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FileMenu = New System.Windows.Forms.ToolStripMenuItem
        Me.OpenImage = New System.Windows.Forms.ToolStripMenuItem
        Me.SaveImageAs = New System.Windows.Forms.ToolStripMenuItem
        Me.SaveThumbnailAs = New System.Windows.Forms.ToolStripMenuItem
        Me.ExitApplication = New System.Windows.Forms.ToolStripMenuItem
        Me.EditMenu = New System.Windows.Forms.ToolStripMenuItem
        Me.Undo = New System.Windows.Forms.ToolStripMenuItem
        Me.ImageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ZoomMenu = New System.Windows.Forms.ToolStripMenuItem
        Me.Zoom25 = New System.Windows.Forms.ToolStripMenuItem
        Me.Zoom50 = New System.Windows.Forms.ToolStripMenuItem
        Me.Zoom100 = New System.Windows.Forms.ToolStripMenuItem
        Me.Zoom150 = New System.Windows.Forms.ToolStripMenuItem
        Me.Zoom200 = New System.Windows.Forms.ToolStripMenuItem
        Me.RotateMenu = New System.Windows.Forms.ToolStripMenuItem
        Me.Rotate90 = New System.Windows.Forms.ToolStripMenuItem
        Me.Rotate180 = New System.Windows.Forms.ToolStripMenuItem
        Me.Rotate270 = New System.Windows.Forms.ToolStripMenuItem
        Me.FlipMenu = New System.Windows.Forms.ToolStripMenuItem
        Me.HorizontalFlip = New System.Windows.Forms.ToolStripMenuItem
        Me.VerticalFlip = New System.Windows.Forms.ToolStripMenuItem
        Me.Negative = New System.Windows.Forms.ToolStripMenuItem
        Me.Grayscale = New System.Windows.Forms.ToolStripMenuItem
        Me.Revert = New System.Windows.Forms.ToolStripMenuItem
        CType(Me.MainImage, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Cropping.SuspendLayout()
        Me.Resizing.SuspendLayout()
        Me.ImageInfo.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MainImage
        '
        Me.MainImage.BackColor = System.Drawing.Color.White
        Me.MainImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.MainImage.Enabled = False
        Me.MainImage.Location = New System.Drawing.Point(133, 29)
        Me.MainImage.Name = "MainImage"
        Me.MainImage.Size = New System.Drawing.Size(450, 360)
        Me.MainImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.MainImage.TabIndex = 4
        Me.MainImage.TabStop = False
        '
        'XLabel
        '
        Me.XLabel.AutoSize = True
        Me.XLabel.Location = New System.Drawing.Point(9, 17)
        Me.XLabel.Name = "XLabel"
        Me.XLabel.Size = New System.Drawing.Size(24, 14)
        Me.XLabel.TabIndex = 5
        Me.XLabel.Text = "X: 0"
        '
        'YLabel
        '
        Me.YLabel.AutoSize = True
        Me.YLabel.Location = New System.Drawing.Point(55, 17)
        Me.YLabel.Name = "YLabel"
        Me.YLabel.Size = New System.Drawing.Size(24, 14)
        Me.YLabel.TabIndex = 6
        Me.YLabel.Text = "Y: 0"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(8, 103)
        Me.Label3.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(40, 14)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "Height:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(21, 24)
        Me.Label4.Margin = New System.Windows.Forms.Padding(3, 3, 3, 1)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(27, 14)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "Top:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(22, 49)
        Me.Label5.Margin = New System.Windows.Forms.Padding(3, 1, 3, 3)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(26, 14)
        Me.Label5.TabIndex = 9
        Me.Label5.Text = "Left:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(12, 76)
        Me.Label6.Margin = New System.Windows.Forms.Padding(3, 1, 3, 3)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(36, 14)
        Me.Label6.TabIndex = 10
        Me.Label6.Text = "Width:"
        '
        'Cropping
        '
        Me.Cropping.Controls.Add(Me.TestButton)
        Me.Cropping.Controls.Add(Me.CropButton)
        Me.Cropping.Controls.Add(Me.HeightInput)
        Me.Cropping.Controls.Add(Me.WidthInput)
        Me.Cropping.Controls.Add(Me.LeftInput)
        Me.Cropping.Controls.Add(Me.TopInput)
        Me.Cropping.Controls.Add(Me.Label4)
        Me.Cropping.Controls.Add(Me.Label6)
        Me.Cropping.Controls.Add(Me.Label5)
        Me.Cropping.Controls.Add(Me.Label3)
        Me.Cropping.Enabled = False
        Me.Cropping.Location = New System.Drawing.Point(10, 116)
        Me.Cropping.Name = "Cropping"
        Me.Cropping.Size = New System.Drawing.Size(112, 192)
        Me.Cropping.TabIndex = 11
        Me.Cropping.TabStop = False
        Me.Cropping.Text = "Cropping"
        '
        'TestButton
        '
        Me.TestButton.Location = New System.Drawing.Point(12, 127)
        Me.TestButton.Name = "TestButton"
        Me.TestButton.Size = New System.Drawing.Size(88, 23)
        Me.TestButton.TabIndex = 6
        Me.TestButton.Text = "Test"
        '
        'CropButton
        '
        Me.CropButton.Location = New System.Drawing.Point(12, 157)
        Me.CropButton.Name = "CropButton"
        Me.CropButton.Size = New System.Drawing.Size(88, 23)
        Me.CropButton.TabIndex = 6
        Me.CropButton.Text = "Crop"
        '
        'HeightInput
        '
        Me.HeightInput.AutoSize = False
        Me.HeightInput.Location = New System.Drawing.Point(55, 99)
        Me.HeightInput.Name = "HeightInput"
        Me.HeightInput.Size = New System.Drawing.Size(45, 20)
        Me.HeightInput.TabIndex = 5
        Me.HeightInput.Tag = "Height"
        Me.HeightInput.Text = "0"
        '
        'WidthInput
        '
        Me.WidthInput.AutoSize = False
        Me.WidthInput.Location = New System.Drawing.Point(55, 73)
        Me.WidthInput.Name = "WidthInput"
        Me.WidthInput.Size = New System.Drawing.Size(45, 20)
        Me.WidthInput.TabIndex = 4
        Me.WidthInput.Tag = "Width"
        Me.WidthInput.Text = "0"
        '
        'LeftInput
        '
        Me.LeftInput.AutoSize = False
        Me.LeftInput.Location = New System.Drawing.Point(55, 47)
        Me.LeftInput.Name = "LeftInput"
        Me.LeftInput.Size = New System.Drawing.Size(45, 20)
        Me.LeftInput.TabIndex = 3
        Me.LeftInput.Tag = "Left"
        Me.LeftInput.Text = "0"
        '
        'TopInput
        '
        Me.TopInput.AutoSize = False
        Me.TopInput.Location = New System.Drawing.Point(55, 21)
        Me.TopInput.Name = "TopInput"
        Me.TopInput.Size = New System.Drawing.Size(45, 20)
        Me.TopInput.TabIndex = 2
        Me.TopInput.Tag = "Top"
        Me.TopInput.Text = "0"
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.Filter = "All Image Formats (*.bmp;*.jpg;*.jpeg;*.gif;*.tif)|*.bmp;*.jpg;*.jpeg;*.gif;*.tif" & _
         "|Bitmaps (*.bmp)|*.bmp|GIFs (*.gif)|*.gif|JPEGs (*.jpg)|*.jpg;*.jpeg|TIFs (*.tif" & _
         ")|*.tif"
        '
        'SaveFileDialog1
        '
        Me.SaveFileDialog1.Filter = "Bitmap (*.bmp)|*.bmp|GIF (*.gif)|*.gif|JPEG (*.jpg)|*.jpg;*.jpeg|TIF (*.tif)|*.ti" & _
         "f"
        '
        'Resizing
        '
        Me.Resizing.Controls.Add(Me.Label2)
        Me.Resizing.Controls.Add(Me.ResizeButton)
        Me.Resizing.Controls.Add(Me.Label1)
        Me.Resizing.Controls.Add(Me.txtPercent)
        Me.Resizing.Enabled = False
        Me.Resizing.Location = New System.Drawing.Point(10, 29)
        Me.Resizing.Name = "Resizing"
        Me.Resizing.Size = New System.Drawing.Size(112, 80)
        Me.Resizing.TabIndex = 16
        Me.Resizing.TabStop = False
        Me.Resizing.Text = "Resizing"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(88, 23)
        Me.Label2.Margin = New System.Windows.Forms.Padding(0, 1, 3, 3)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(14, 14)
        Me.Label2.TabIndex = 22
        Me.Label2.Text = "%"
        '
        'ResizeButton
        '
        Me.ResizeButton.Location = New System.Drawing.Point(12, 47)
        Me.ResizeButton.Name = "ResizeButton"
        Me.ResizeButton.Size = New System.Drawing.Size(88, 23)
        Me.ResizeButton.TabIndex = 1
        Me.ResizeButton.Text = "Resize"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 23)
        Me.Label1.Margin = New System.Windows.Forms.Padding(3, 1, 1, 3)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(46, 14)
        Me.Label1.TabIndex = 20
        Me.Label1.Text = "Percent:"
        '
        'txtPercent
        '
        Me.txtPercent.AutoSize = False
        Me.txtPercent.Location = New System.Drawing.Point(55, 20)
        Me.txtPercent.Margin = New System.Windows.Forms.Padding(2, 3, 1, 3)
        Me.txtPercent.Name = "txtPercent"
        Me.txtPercent.Size = New System.Drawing.Size(32, 20)
        Me.txtPercent.TabIndex = 0
        Me.txtPercent.Tag = "Resize Percentage"
        Me.txtPercent.Text = "100"
        '
        'ZoomLabel
        '
        Me.ZoomLabel.AutoSize = True
        Me.ZoomLabel.Location = New System.Drawing.Point(9, 80)
        Me.ZoomLabel.Name = "ZoomLabel"
        Me.ZoomLabel.Size = New System.Drawing.Size(68, 14)
        Me.ZoomLabel.TabIndex = 21
        Me.ZoomLabel.Text = "Zoom: 100%"
        '
        'WidthLabel
        '
        Me.WidthLabel.AutoSize = True
        Me.WidthLabel.Location = New System.Drawing.Point(9, 38)
        Me.WidthLabel.Name = "WidthLabel"
        Me.WidthLabel.Size = New System.Drawing.Size(46, 14)
        Me.WidthLabel.TabIndex = 26
        Me.WidthLabel.Text = "Width: 0"
        '
        'HeightLabel
        '
        Me.HeightLabel.AutoSize = True
        Me.HeightLabel.Location = New System.Drawing.Point(9, 59)
        Me.HeightLabel.Name = "HeightLabel"
        Me.HeightLabel.Size = New System.Drawing.Size(49, 14)
        Me.HeightLabel.TabIndex = 27
        Me.HeightLabel.Text = "Height: 0"
        '
        'ImageInfo
        '
        Me.ImageInfo.Controls.Add(Me.HeightLabel)
        Me.ImageInfo.Controls.Add(Me.YLabel)
        Me.ImageInfo.Controls.Add(Me.WidthLabel)
        Me.ImageInfo.Controls.Add(Me.ZoomLabel)
        Me.ImageInfo.Controls.Add(Me.XLabel)
        Me.ImageInfo.Enabled = False
        Me.ImageInfo.Location = New System.Drawing.Point(10, 315)
        Me.ImageInfo.Name = "ImageInfo"
        Me.ImageInfo.Size = New System.Drawing.Size(112, 100)
        Me.ImageInfo.TabIndex = 28
        Me.ImageInfo.TabStop = False
        Me.ImageInfo.Text = "Image Info"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileMenu, Me.EditMenu, Me.ImageToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(6, 2, 0, 2)
        Me.MenuStrip1.TabIndex = 32
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileMenu
        '
        Me.FileMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenImage, Me.SaveImageAs, Me.SaveThumbnailAs, Me.ExitApplication})
        Me.FileMenu.Name = "FileMenu"
        Me.FileMenu.Text = "&File"
        '
        'OpenImage
        '
        Me.OpenImage.Name = "OpenImage"
        Me.OpenImage.Text = "&Open Image"
        '
        'SaveImageAs
        '
        Me.SaveImageAs.Enabled = False
        Me.SaveImageAs.Name = "SaveImageAs"
        Me.SaveImageAs.Text = "&Save Image As ..."
        '
        'SaveThumbnailAs
        '
        Me.SaveThumbnailAs.Enabled = False
        Me.SaveThumbnailAs.Name = "SaveThumbnailAs"
        Me.SaveThumbnailAs.Text = "Save &Thumbnail As ..."
        '
        'ExitApplication
        '
        Me.ExitApplication.Name = "ExitApplication"
        Me.ExitApplication.Text = "E&xit"
        '
        'EditMenu
        '
        Me.EditMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.Undo})
        Me.EditMenu.Name = "EditMenu"
        Me.EditMenu.Text = "&Edit"
        Me.EditMenu.Visible = False
        '
        'Undo
        '
        Me.Undo.Enabled = False
        Me.Undo.Name = "Undo"
        Me.Undo.Text = "&Undo"
        '
        'ImageToolStripMenuItem
        '
        Me.ImageToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ZoomMenu, Me.RotateMenu, Me.FlipMenu, Me.Negative, Me.Grayscale, Me.Revert})
        Me.ImageToolStripMenuItem.Name = "ImageToolStripMenuItem"
        Me.ImageToolStripMenuItem.Text = "&Image"
        Me.ImageToolStripMenuItem.Visible = False
        '
        'ZoomMenu
        '
        Me.ZoomMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.Zoom25, Me.Zoom50, Me.Zoom100, Me.Zoom150, Me.Zoom200})
        Me.ZoomMenu.Name = "ZoomMenu"
        Me.ZoomMenu.Text = "&Zoom"
        '
        'Zoom25
        '
        Me.Zoom25.Name = "Zoom25"
        Me.Zoom25.Text = "25%"
        '
        'Zoom50
        '
        Me.Zoom50.Name = "Zoom50"
        Me.Zoom50.Text = "50%"
        '
        'Zoom100
        '
        Me.Zoom100.Checked = True
        Me.Zoom100.CheckState = System.Windows.Forms.CheckState.Checked
        Me.Zoom100.Name = "Zoom100"
        Me.Zoom100.Text = "100%"
        '
        'Zoom150
        '
        Me.Zoom150.Name = "Zoom150"
        Me.Zoom150.Text = "150%"
        '
        'Zoom200
        '
        Me.Zoom200.Name = "Zoom200"
        Me.Zoom200.Text = "200%"
        '
        'RotateMenu
        '
        Me.RotateMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.Rotate90, Me.Rotate180, Me.Rotate270})
        Me.RotateMenu.Name = "RotateMenu"
        Me.RotateMenu.Text = "&Rotate"
        '
        'Rotate90
        '
        Me.Rotate90.Name = "Rotate90"
        Me.Rotate90.Text = "90°"
        '
        'Rotate180
        '
        Me.Rotate180.Name = "Rotate180"
        Me.Rotate180.Text = "180°"
        '
        'Rotate270
        '
        Me.Rotate270.Name = "Rotate270"
        Me.Rotate270.Text = "270°"
        '
        'FlipMenu
        '
        Me.FlipMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.HorizontalFlip, Me.VerticalFlip})
        Me.FlipMenu.Name = "FlipMenu"
        Me.FlipMenu.Text = "&Flip"
        '
        'HorizontalFlip
        '
        Me.HorizontalFlip.Name = "HorizontalFlip"
        Me.HorizontalFlip.Text = "&Horizontal"
        '
        'VerticalFlip
        '
        Me.VerticalFlip.Name = "VerticalFlip"
        Me.VerticalFlip.Text = "&Vertical"
        '
        'Negative
        '
        Me.Negative.Name = "Negative"
        Me.Negative.Text = "&Negative"
        '
        'Grayscale
        '
        Me.Grayscale.Name = "Grayscale"
        Me.Grayscale.Text = "&Grayscale"
        '
        'Revert
        '
        Me.Revert.Name = "Revert"
        Me.Revert.Text = "&Revert"
        '
        'GDIImagesSampleForm
        '
        Me.ClientSize = New System.Drawing.Size(592, 433)
        Me.Controls.Add(Me.ImageInfo)
        Me.Controls.Add(Me.Resizing)
        Me.Controls.Add(Me.Cropping)
        Me.Controls.Add(Me.MainImage)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(600, 460)
        Me.MinimumSize = New System.Drawing.Size(600, 460)
        Me.Name = "GDIImagesSampleForm"
        Me.Text = "Form1"
        CType(Me.MainImage, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Cropping.ResumeLayout(False)
        Me.Cropping.PerformLayout()
        Me.Resizing.ResumeLayout(False)
        Me.Resizing.PerformLayout()
        Me.ImageInfo.ResumeLayout(False)
        Me.ImageInfo.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MainImage As System.Windows.Forms.PictureBox
    Friend WithEvents XLabel As System.Windows.Forms.Label
    Friend WithEvents YLabel As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Cropping As System.Windows.Forms.GroupBox
    Friend WithEvents HeightInput As System.Windows.Forms.TextBox
    Friend WithEvents WidthInput As System.Windows.Forms.TextBox
    Friend WithEvents LeftInput As System.Windows.Forms.TextBox
    Friend WithEvents TopInput As System.Windows.Forms.TextBox
    Friend WithEvents CropButton As System.Windows.Forms.Button
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents TestButton As System.Windows.Forms.Button
    Friend WithEvents Resizing As System.Windows.Forms.GroupBox
    Friend WithEvents ResizeButton As System.Windows.Forms.Button
    Friend WithEvents txtPercent As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ZoomLabel As System.Windows.Forms.Label
    Friend WithEvents WidthLabel As System.Windows.Forms.Label
    Friend WithEvents HeightLabel As System.Windows.Forms.Label
    Friend WithEvents ImageInfo As System.Windows.Forms.GroupBox
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenImage As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveImageAs As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveThumbnailAs As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitApplication As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EditMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Undo As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ImageToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ZoomMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Zoom25 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Zoom50 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Zoom100 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Zoom150 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Zoom200 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RotateMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Rotate90 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Rotate180 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Rotate270 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FlipMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HorizontalFlip As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents VerticalFlip As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Negative As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Grayscale As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Revert As System.Windows.Forms.ToolStripMenuItem

End Class
