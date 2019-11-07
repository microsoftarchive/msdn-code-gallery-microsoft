' Copyright (c) Microsoft Corporation. All rights reserved.
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class MainForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.tabWebServices = New System.Windows.Forms.TabControl
        Me.pgeTime = New System.Windows.Forms.TabPage
        Me.btnGetTime = New System.Windows.Forms.Button
        Me.lblTime = New System.Windows.Forms.Label
        Me.Label18 = New System.Windows.Forms.Label
        Me.txtZipCodeForTime = New System.Windows.Forms.TextBox
        Me.Label17 = New System.Windows.Forms.Label
        Me.Label15 = New System.Windows.Forms.Label
        Me.pgeCurrency = New System.Windows.Forms.TabPage
        Me.btnConvert = New System.Windows.Forms.Button
        Me.lblConvertedAmount = New System.Windows.Forms.Label
        Me.Label16 = New System.Windows.Forms.Label
        Me.txtAmount = New System.Windows.Forms.TextBox
        Me.cboConvertTo = New System.Windows.Forms.ComboBox
        Me.Label11 = New System.Windows.Forms.Label
        Me.Label13 = New System.Windows.Forms.Label
        Me.Label14 = New System.Windows.Forms.Label
        Me.pgeBooks = New System.Windows.Forms.TabPage
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.txtISBN = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.btnGetBookData = New System.Windows.Forms.Button
        Me.lvwBooks = New System.Windows.Forms.ListView
        Me.chISBN = New System.Windows.Forms.ColumnHeader
        Me.chBNPrice = New System.Windows.Forms.ColumnHeader
        Me.pgeDilbert = New System.Windows.Forms.TabPage
        Me.btnCartoon = New System.Windows.Forms.Button
        Me.Label5 = New System.Windows.Forms.Label
        Me.picDilbert = New System.Windows.Forms.PictureBox
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.tabWebServices.SuspendLayout()
        Me.pgeTime.SuspendLayout()
        Me.pgeCurrency.SuspendLayout()
        Me.pgeBooks.SuspendLayout()
        Me.pgeDilbert.SuspendLayout()
        CType(Me.picDilbert, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabWebServices
        '
        Me.tabWebServices.AccessibleDescription = "TabControl for all Web Service Tab Pages"
        Me.tabWebServices.AccessibleName = "Tab Control"
        Me.tabWebServices.Controls.Add(Me.pgeTime)
        Me.tabWebServices.Controls.Add(Me.pgeCurrency)
        Me.tabWebServices.Controls.Add(Me.pgeBooks)
        Me.tabWebServices.Controls.Add(Me.pgeDilbert)
        Me.tabWebServices.ItemSize = New System.Drawing.Size(42, 18)
        Me.tabWebServices.Location = New System.Drawing.Point(12, 60)
        Me.tabWebServices.Name = "tabWebServices"
        Me.tabWebServices.SelectedIndex = 0
        Me.tabWebServices.Size = New System.Drawing.Size(656, 376)
        Me.tabWebServices.TabIndex = 2
        '
        'pgeTime
        '
        Me.pgeTime.AccessibleDescription = "TabPage for the Local Time by Zip Code Web service demo."
        Me.pgeTime.AccessibleName = "Local Time By ZipCode"
        Me.pgeTime.Controls.Add(Me.btnGetTime)
        Me.pgeTime.Controls.Add(Me.lblTime)
        Me.pgeTime.Controls.Add(Me.Label18)
        Me.pgeTime.Controls.Add(Me.txtZipCodeForTime)
        Me.pgeTime.Controls.Add(Me.Label17)
        Me.pgeTime.Controls.Add(Me.Label15)
        Me.pgeTime.Location = New System.Drawing.Point(4, 22)
        Me.pgeTime.Name = "pgeTime"
        Me.pgeTime.Size = New System.Drawing.Size(648, 350)
        Me.pgeTime.TabIndex = 4
        Me.pgeTime.Text = "Local Time By Zip"
        Me.pgeTime.UseVisualStyleBackColor = False
        '
        'btnGetTime
        '
        Me.btnGetTime.AccessibleDescription = "Button stating ""Get Time!"""
        Me.btnGetTime.AccessibleName = "Get Time!"
        Me.btnGetTime.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnGetTime.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.btnGetTime.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnGetTime.Location = New System.Drawing.Point(244, 64)
        Me.btnGetTime.Name = "btnGetTime"
        Me.btnGetTime.Size = New System.Drawing.Size(75, 23)
        Me.btnGetTime.TabIndex = 11
        Me.btnGetTime.Text = "&Get Time!"
        '
        'lblTime
        '
        Me.lblTime.AccessibleDescription = "label to display local time returned by the Web service"
        Me.lblTime.AccessibleName = "Local Time resulting value"
        Me.lblTime.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.lblTime.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblTime.Location = New System.Drawing.Point(132, 100)
        Me.lblTime.Name = "lblTime"
        Me.lblTime.Size = New System.Drawing.Size(286, 23)
        Me.lblTime.TabIndex = 10
        Me.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label18
        '
        Me.Label18.AccessibleDescription = "Label stating ""Local Time"""
        Me.Label18.AccessibleName = "Local Time"
        Me.Label18.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.Label18.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label18.Location = New System.Drawing.Point(30, 103)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(96, 23)
        Me.Label18.TabIndex = 9
        Me.Label18.Text = "Local Time:"
        '
        'txtZipCodeForTime
        '
        Me.txtZipCodeForTime.AccessibleDescription = "TextBox to enter Zip code to get local time."
        Me.txtZipCodeForTime.AccessibleName = "Zip code for local time."
        Me.txtZipCodeForTime.Location = New System.Drawing.Point(132, 66)
        Me.txtZipCodeForTime.Name = "txtZipCodeForTime"
        Me.txtZipCodeForTime.Size = New System.Drawing.Size(100, 20)
        Me.txtZipCodeForTime.TabIndex = 8
        Me.txtZipCodeForTime.Text = "98103"
        '
        'Label17
        '
        Me.Label17.AccessibleDescription = "Label stating ""Zip Code"""
        Me.Label17.AccessibleName = "Zip Code for obtaining local time"
        Me.Label17.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.Label17.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label17.Location = New System.Drawing.Point(42, 64)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(72, 23)
        Me.Label17.TabIndex = 7
        Me.Label17.Text = "Zip Code:"
        '
        'Label15
        '
        Me.Label15.AccessibleDescription = "Label that displays the tab page title."
        Me.Label15.AccessibleName = "Local Time TabPage Title"
        Me.Label15.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label15.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label15.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label15.Location = New System.Drawing.Point(16, 16)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(200, 23)
        Me.Label15.TabIndex = 6
        Me.Label15.Text = "Local Time by Zip Code"
        '
        'pgeCurrency
        '
        Me.pgeCurrency.AccessibleDescription = "TabPage for the Currency Converter Web service demo."
        Me.pgeCurrency.AccessibleName = "Euro Currency Converter"
        Me.pgeCurrency.Controls.Add(Me.btnConvert)
        Me.pgeCurrency.Controls.Add(Me.lblConvertedAmount)
        Me.pgeCurrency.Controls.Add(Me.Label16)
        Me.pgeCurrency.Controls.Add(Me.txtAmount)
        Me.pgeCurrency.Controls.Add(Me.cboConvertTo)
        Me.pgeCurrency.Controls.Add(Me.Label11)
        Me.pgeCurrency.Controls.Add(Me.Label13)
        Me.pgeCurrency.Controls.Add(Me.Label14)
        Me.pgeCurrency.Location = New System.Drawing.Point(4, 22)
        Me.pgeCurrency.Name = "pgeCurrency"
        Me.pgeCurrency.Size = New System.Drawing.Size(648, 350)
        Me.pgeCurrency.TabIndex = 3
        Me.pgeCurrency.Text = "Currency Converter"
        Me.pgeCurrency.UseVisualStyleBackColor = False
        '
        'btnConvert
        '
        Me.btnConvert.AccessibleDescription = "Button with Text ""Convert!"""
        Me.btnConvert.AccessibleName = "Convert!"
        Me.btnConvert.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnConvert.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.btnConvert.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnConvert.Location = New System.Drawing.Point(226, 120)
        Me.btnConvert.Name = "btnConvert"
        Me.btnConvert.Size = New System.Drawing.Size(75, 23)
        Me.btnConvert.TabIndex = 2
        Me.btnConvert.Text = "&Convert!"
        '
        'lblConvertedAmount
        '
        Me.lblConvertedAmount.AccessibleDescription = "Label to display resulting conversion amount"
        Me.lblConvertedAmount.AccessibleName = "Conversion Result"
        Me.lblConvertedAmount.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblConvertedAmount.Location = New System.Drawing.Point(226, 156)
        Me.lblConvertedAmount.Name = "lblConvertedAmount"
        Me.lblConvertedAmount.Size = New System.Drawing.Size(100, 23)
        Me.lblConvertedAmount.TabIndex = 10
        '
        'Label16
        '
        Me.Label16.AccessibleDescription = "Label with text ""Resulting Value:"""
        Me.Label16.AccessibleName = "Resulting Value title"
        Me.Label16.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.Label16.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label16.Location = New System.Drawing.Point(128, 156)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(96, 23)
        Me.Label16.TabIndex = 9
        Me.Label16.Text = "Resulting Value:"
        '
        'txtAmount
        '
        Me.txtAmount.AccessibleDescription = "TextBox to enter Euro amount to convert"
        Me.txtAmount.AccessibleName = "Euro Amount TextBox"
        Me.txtAmount.Location = New System.Drawing.Point(224, 56)
        Me.txtAmount.Name = "txtAmount"
        Me.txtAmount.Size = New System.Drawing.Size(56, 20)
        Me.txtAmount.TabIndex = 0
        Me.txtAmount.Text = "20"
        '
        'cboConvertTo
        '
        Me.cboConvertTo.AccessibleDescription = "ComboBox containing Euro currencies"
        Me.cboConvertTo.AccessibleName = "Euro Currency ComboBox"
        Me.cboConvertTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboConvertTo.FormattingEnabled = True
        Me.cboConvertTo.ItemHeight = 13
        Me.cboConvertTo.Location = New System.Drawing.Point(224, 88)
        Me.cboConvertTo.Name = "cboConvertTo"
        Me.cboConvertTo.Size = New System.Drawing.Size(160, 21)
        Me.cboConvertTo.TabIndex = 1
        '
        'Label11
        '
        Me.Label11.AccessibleDescription = "Label with text ""Currency Converter"""
        Me.Label11.AccessibleName = "Currency TabPage tTitle"
        Me.Label11.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label11.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label11.Location = New System.Drawing.Point(16, 16)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(176, 23)
        Me.Label11.TabIndex = 0
        Me.Label11.Text = "Currency Converter"
        '
        'Label13
        '
        Me.Label13.AccessibleDescription = "Label with text ""Convert to:"""
        Me.Label13.AccessibleName = "Convert To:"
        Me.Label13.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.Label13.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label13.Location = New System.Drawing.Point(147, 91)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(73, 23)
        Me.Label13.TabIndex = 6
        Me.Label13.Text = "Convert To:"
        '
        'Label14
        '
        Me.Label14.AccessibleDescription = "Label with text ""Amount to Convert in Euros"""
        Me.Label14.AccessibleName = "Amount to convert Label"
        Me.Label14.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.Label14.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label14.Location = New System.Drawing.Point(61, 59)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(160, 23)
        Me.Label14.TabIndex = 7
        Me.Label14.Text = "Amount to Convert in Euros:"
        '
        'pgeBooks
        '
        Me.pgeBooks.AccessibleDescription = "TabPage for the SalesNPriceInfo Web service demo."
        Me.pgeBooks.AccessibleName = "Sales & Price Info for Books"
        Me.pgeBooks.BackColor = System.Drawing.SystemColors.Control
        Me.pgeBooks.Controls.Add(Me.Label4)
        Me.pgeBooks.Controls.Add(Me.Label3)
        Me.pgeBooks.Controls.Add(Me.txtISBN)
        Me.pgeBooks.Controls.Add(Me.Label2)
        Me.pgeBooks.Controls.Add(Me.btnGetBookData)
        Me.pgeBooks.Controls.Add(Me.lvwBooks)
        Me.pgeBooks.Location = New System.Drawing.Point(4, 22)
        Me.pgeBooks.Name = "pgeBooks"
        Me.pgeBooks.Size = New System.Drawing.Size(648, 350)
        Me.pgeBooks.TabIndex = 1
        Me.pgeBooks.Text = "Book Info By ISBN"
        Me.pgeBooks.UseVisualStyleBackColor = False
        '
        'Label4
        '
        Me.Label4.AccessibleDescription = "Label containing instructional text regarding the Books Web service."
        Me.Label4.AccessibleName = "Instructional Text for Books"
        Me.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label4.Location = New System.Drawing.Point(24, 40)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(592, 48)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = resources.GetString("Label4.Text")
        '
        'Label3
        '
        Me.Label3.AccessibleDescription = "Label for TabPage title."
        Me.Label3.AccessibleName = "Books Tab Page title"
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label3.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label3.Location = New System.Drawing.Point(16, 16)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(152, 23)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Book Info by ISBN"
        '
        'txtISBN
        '
        Me.txtISBN.AccessibleDescription = "TextBox to enter ISBN number."
        Me.txtISBN.AccessibleName = "ISBN"
        Me.txtISBN.Location = New System.Drawing.Point(88, 96)
        Me.txtISBN.Name = "txtISBN"
        Me.txtISBN.Size = New System.Drawing.Size(104, 20)
        Me.txtISBN.TabIndex = 0
        Me.txtISBN.Text = "0735620598 "
        '
        'Label2
        '
        Me.Label2.AccessibleDescription = "Label stating ""ISBN"" to indicate what to enter into the TextBox."
        Me.Label2.AccessibleName = "ISBN"
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label2.Location = New System.Drawing.Point(40, 96)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(48, 23)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "ISBN:"
        '
        'btnGetBookData
        '
        Me.btnGetBookData.AccessibleDescription = "Button with text ""Get Data!"""
        Me.btnGetBookData.AccessibleName = "Get Data!"
        Me.btnGetBookData.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btnGetBookData.BackColor = System.Drawing.SystemColors.Control
        Me.btnGetBookData.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnGetBookData.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.btnGetBookData.ImageIndex = 0
        Me.btnGetBookData.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnGetBookData.Location = New System.Drawing.Point(200, 95)
        Me.btnGetBookData.Name = "btnGetBookData"
        Me.btnGetBookData.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.btnGetBookData.Size = New System.Drawing.Size(75, 23)
        Me.btnGetBookData.TabIndex = 1
        Me.btnGetBookData.Text = "&Get Data!!"
        Me.btnGetBookData.UseVisualStyleBackColor = False
        '
        'lvwBooks
        '
        Me.lvwBooks.AccessibleDescription = "ListView to display data from Web service."
        Me.lvwBooks.AccessibleName = "Book Data"
        Me.lvwBooks.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chISBN, Me.chBNPrice})
        Me.lvwBooks.FullRowSelect = True
        Me.lvwBooks.GridLines = True
        Me.lvwBooks.Location = New System.Drawing.Point(32, 128)
        Me.lvwBooks.Name = "lvwBooks"
        Me.lvwBooks.Size = New System.Drawing.Size(590, 190)
        Me.lvwBooks.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lvwBooks.TabIndex = 2
        Me.lvwBooks.View = System.Windows.Forms.View.Details
        '
        'chISBN
        '
        Me.chISBN.Name = "chISBN"
        Me.chISBN.Text = "ISBN"
        Me.chISBN.Width = 186
        '
        'chBNPrice
        '
        Me.chBNPrice.Name = "chBNPrice"
        Me.chBNPrice.Text = "B&N Price"
        Me.chBNPrice.Width = 100
        '
        'pgeDilbert
        '
        Me.pgeDilbert.AccessibleDescription = "A TabPage for the DailyDilbert Web service demo."
        Me.pgeDilbert.AccessibleName = "Dilbert (Async Demo)"
        Me.pgeDilbert.BackColor = System.Drawing.SystemColors.Control
        Me.pgeDilbert.Controls.Add(Me.btnCartoon)
        Me.pgeDilbert.Controls.Add(Me.Label5)
        Me.pgeDilbert.Controls.Add(Me.picDilbert)
        Me.pgeDilbert.Location = New System.Drawing.Point(4, 22)
        Me.pgeDilbert.Name = "pgeDilbert"
        Me.pgeDilbert.Size = New System.Drawing.Size(648, 350)
        Me.pgeDilbert.TabIndex = 0
        Me.pgeDilbert.Text = "Dilbert (Async Demo)"
        Me.pgeDilbert.UseVisualStyleBackColor = False
        '
        'btnCartoon
        '
        Me.btnCartoon.Location = New System.Drawing.Point(180, 15)
        Me.btnCartoon.Name = "btnCartoon"
        Me.btnCartoon.Size = New System.Drawing.Size(75, 23)
        Me.btnCartoon.TabIndex = 6
        Me.btnCartoon.Text = "Get Cartoon"
        '
        'Label5
        '
        Me.Label5.AccessibleDescription = "Label that displays the tab page title."
        Me.Label5.AccessibleName = "Dilbert TabPage Title"
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label5.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label5.Location = New System.Drawing.Point(16, 16)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(112, 23)
        Me.Label5.TabIndex = 5
        Me.Label5.Text = "Daily Diblert"
        '
        'picDilbert
        '
        Me.picDilbert.AccessibleDescription = "A PictureBox that displays the Dilbert cartoon."
        Me.picDilbert.AccessibleName = "Cartoon PictureBox"
        Me.picDilbert.BackColor = System.Drawing.SystemColors.Menu
        Me.picDilbert.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.picDilbert.Location = New System.Drawing.Point(16, 63)
        Me.picDilbert.Name = "picDilbert"
        Me.picDilbert.Size = New System.Drawing.Size(608, 248)
        Me.picDilbert.TabIndex = 0
        Me.picDilbert.TabStop = False
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(684, 24)
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
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(684, 448)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.tabWebServices)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "MainForm"
        Me.Text = "Web Service Sample"
        Me.tabWebServices.ResumeLayout(False)
        Me.pgeTime.ResumeLayout(False)
        Me.pgeTime.PerformLayout()
        Me.pgeCurrency.ResumeLayout(False)
        Me.pgeCurrency.PerformLayout()
        Me.pgeBooks.ResumeLayout(False)
        Me.pgeBooks.PerformLayout()
        Me.pgeDilbert.ResumeLayout(False)
        CType(Me.picDilbert, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tabWebServices As System.Windows.Forms.TabControl
    Friend WithEvents pgeTime As System.Windows.Forms.TabPage
    Friend WithEvents btnGetTime As System.Windows.Forms.Button
    Friend WithEvents lblTime As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents txtZipCodeForTime As System.Windows.Forms.TextBox
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents pgeCurrency As System.Windows.Forms.TabPage
    Friend WithEvents btnConvert As System.Windows.Forms.Button
    Friend WithEvents lblConvertedAmount As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents txtAmount As System.Windows.Forms.TextBox
    Friend WithEvents cboConvertTo As System.Windows.Forms.ComboBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents pgeBooks As System.Windows.Forms.TabPage
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtISBN As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnGetBookData As System.Windows.Forms.Button
    Friend WithEvents lvwBooks As System.Windows.Forms.ListView
    Friend WithEvents chISBN As System.Windows.Forms.ColumnHeader
    Friend WithEvents chBNPrice As System.Windows.Forms.ColumnHeader
    Friend WithEvents pgeDilbert As System.Windows.Forms.TabPage
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents picDilbert As System.Windows.Forms.PictureBox
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnCartoon As System.Windows.Forms.Button

End Class
