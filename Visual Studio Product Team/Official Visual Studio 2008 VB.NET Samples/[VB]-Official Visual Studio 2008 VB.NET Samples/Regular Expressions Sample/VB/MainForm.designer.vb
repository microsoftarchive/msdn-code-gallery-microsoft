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
        Me.chkSingleLine = New System.Windows.Forms.CheckBox
        Me.pgeRegExTester = New System.Windows.Forms.TabPage
        Me.txtResults = New System.Windows.Forms.TextBox
        Me.txtSource = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.btnReplace = New System.Windows.Forms.Button
        Me.btnSplit = New System.Windows.Forms.Button
        Me.btnFind = New System.Windows.Forms.Button
        Me.txtReplace = New System.Windows.Forms.TextBox
        Me.txtRegEx = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.lblResultCount = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.chkMultiline = New System.Windows.Forms.CheckBox
        Me.chkIgnoreCase = New System.Windows.Forms.CheckBox
        Me.chkShowCaptures = New System.Windows.Forms.CheckBox
        Me.chkShowGroups = New System.Windows.Forms.CheckBox
        Me.lvcUrl = New System.Windows.Forms.ColumnHeader
        Me.optImages = New System.Windows.Forms.RadioButton
        Me.optLinks = New System.Windows.Forms.RadioButton
        Me.btnScrape = New System.Windows.Forms.Button
        Me.lvcSrc = New System.Windows.Forms.ColumnHeader
        Me.lvcWidth = New System.Windows.Forms.ColumnHeader
        Me.lvcHeight = New System.Windows.Forms.ColumnHeader
        Me.lvcAlt = New System.Windows.Forms.ColumnHeader
        Me.OpenFileDialog2 = New System.Windows.Forms.OpenFileDialog
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
        Me.txtURL = New System.Windows.Forms.TextBox
        Me.lblResults = New System.Windows.Forms.Label
        Me.btnFindNumber = New System.Windows.Forms.Button
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.Label12 = New System.Windows.Forms.Label
        Me.txtFindNumber = New System.Windows.Forms.TextBox
        Me.lblValid = New System.Windows.Forms.Label
        Me.grpValidation = New System.Windows.Forms.GroupBox
        Me.Label11 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.btnValidate = New System.Windows.Forms.Button
        Me.txtEmail = New System.Windows.Forms.TextBox
        Me.txtDate = New System.Windows.Forms.TextBox
        Me.txtZip = New System.Windows.Forms.TextBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.TabControl1 = New System.Windows.Forms.TabControl
        Me.pgeSimple = New System.Windows.Forms.TabPage
        Me.pgeScreenScrape = New System.Windows.Forms.TabPage
        Me.Label10 = New System.Windows.Forms.Label
        Me.lvwImages = New System.Windows.Forms.ListView
        Me.lvwLinks = New System.Windows.Forms.ListView
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.pgeRegExTester.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.grpValidation.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.pgeSimple.SuspendLayout()
        Me.pgeScreenScrape.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'chkSingleLine
        '
        Me.chkSingleLine.AccessibleDescription = "CheckBox with text ""Singleline"""
        Me.chkSingleLine.AccessibleName = "Singleline"
        Me.chkSingleLine.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.chkSingleLine.Location = New System.Drawing.Point(373, 93)
        Me.chkSingleLine.Name = "chkSingleLine"
        Me.chkSingleLine.Size = New System.Drawing.Size(83, 24)
        Me.chkSingleLine.TabIndex = 2
        Me.chkSingleLine.Text = "&Singleline"
        '
        'pgeRegExTester
        '
        Me.pgeRegExTester.Controls.Add(Me.chkSingleLine)
        Me.pgeRegExTester.Controls.Add(Me.txtResults)
        Me.pgeRegExTester.Controls.Add(Me.txtSource)
        Me.pgeRegExTester.Controls.Add(Me.Label3)
        Me.pgeRegExTester.Controls.Add(Me.btnReplace)
        Me.pgeRegExTester.Controls.Add(Me.btnSplit)
        Me.pgeRegExTester.Controls.Add(Me.btnFind)
        Me.pgeRegExTester.Controls.Add(Me.txtReplace)
        Me.pgeRegExTester.Controls.Add(Me.txtRegEx)
        Me.pgeRegExTester.Controls.Add(Me.Label1)
        Me.pgeRegExTester.Controls.Add(Me.Label2)
        Me.pgeRegExTester.Controls.Add(Me.lblResultCount)
        Me.pgeRegExTester.Controls.Add(Me.Label4)
        Me.pgeRegExTester.Controls.Add(Me.chkMultiline)
        Me.pgeRegExTester.Controls.Add(Me.chkIgnoreCase)
        Me.pgeRegExTester.Controls.Add(Me.chkShowCaptures)
        Me.pgeRegExTester.Controls.Add(Me.chkShowGroups)
        Me.pgeRegExTester.Location = New System.Drawing.Point(4, 22)
        Me.pgeRegExTester.Name = "pgeRegExTester"
        Me.pgeRegExTester.Size = New System.Drawing.Size(720, 490)
        Me.pgeRegExTester.TabIndex = 3
        Me.pgeRegExTester.Text = "RegEx Tester"
        Me.pgeRegExTester.UseVisualStyleBackColor = False
        '
        'txtResults
        '
        Me.txtResults.AcceptsReturn = True
        Me.txtResults.AcceptsTab = True
        Me.txtResults.AccessibleDescription = "TextBox for viewing results"
        Me.txtResults.AccessibleName = "results"
        Me.txtResults.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtResults.Location = New System.Drawing.Point(16, 331)
        Me.txtResults.Multiline = True
        Me.txtResults.Name = "txtResults"
        Me.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtResults.Size = New System.Drawing.Size(673, 134)
        Me.txtResults.TabIndex = 9
        '
        'txtSource
        '
        Me.txtSource.AcceptsReturn = True
        Me.txtSource.AcceptsTab = True
        Me.txtSource.AccessibleDescription = "TextBox for entering Text to be parsed"
        Me.txtSource.AccessibleName = "Text to be parsed"
        Me.txtSource.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSource.Location = New System.Drawing.Point(16, 198)
        Me.txtSource.Multiline = True
        Me.txtSource.Name = "txtSource"
        Me.txtSource.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtSource.Size = New System.Drawing.Size(673, 54)
        Me.txtSource.TabIndex = 8
        '
        'Label3
        '
        Me.Label3.AccessibleDescription = "Label with text ""Text to be parsed"""
        Me.Label3.AccessibleName = "Text to be parsed label"
        Me.Label3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label3.Location = New System.Drawing.Point(11, 173)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(93, 24)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "Text to be parsed"
        '
        'btnReplace
        '
        Me.btnReplace.AccessibleDescription = "Button with text ""Replace"""
        Me.btnReplace.AccessibleName = "Replace"
        Me.btnReplace.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnReplace.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnReplace.Location = New System.Drawing.Point(126, 270)
        Me.btnReplace.Name = "btnReplace"
        Me.btnReplace.Size = New System.Drawing.Size(80, 32)
        Me.btnReplace.TabIndex = 7
        Me.btnReplace.Text = "&Replace"
        '
        'btnSplit
        '
        Me.btnSplit.AccessibleDescription = "Button with text ""Split"""
        Me.btnSplit.AccessibleName = "Split"
        Me.btnSplit.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSplit.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnSplit.Location = New System.Drawing.Point(16, 270)
        Me.btnSplit.Name = "btnSplit"
        Me.btnSplit.Size = New System.Drawing.Size(80, 32)
        Me.btnSplit.TabIndex = 5
        Me.btnSplit.Text = "S&plit"
        '
        'btnFind
        '
        Me.btnFind.AccessibleDescription = "Button with text ""Find"""
        Me.btnFind.AccessibleName = "Find"
        Me.btnFind.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnFind.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnFind.Location = New System.Drawing.Point(1056, 88)
        Me.btnFind.Name = "btnFind"
        Me.btnFind.Size = New System.Drawing.Size(80, 32)
        Me.btnFind.TabIndex = 4
        Me.btnFind.Text = "&Find"
        '
        'txtReplace
        '
        Me.txtReplace.AccessibleDescription = "TextBox for entering replace pattern"
        Me.txtReplace.AccessibleName = "replace pattern"
        Me.txtReplace.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtReplace.Font = New System.Drawing.Font("Courier New", 8.25!)
        Me.txtReplace.Location = New System.Drawing.Point(16, 123)
        Me.txtReplace.Name = "txtReplace"
        Me.txtReplace.Size = New System.Drawing.Size(673, 20)
        Me.txtReplace.TabIndex = 6
        '
        'txtRegEx
        '
        Me.txtRegEx.AccessibleDescription = "TextBox for entering Regular Expression Pattern"
        Me.txtRegEx.AccessibleName = "Regular Expression Pattern"
        Me.txtRegEx.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtRegEx.Font = New System.Drawing.Font("Courier New", 8.25!)
        Me.txtRegEx.Location = New System.Drawing.Point(16, 37)
        Me.txtRegEx.Multiline = True
        Me.txtRegEx.Name = "txtRegEx"
        Me.txtRegEx.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtRegEx.Size = New System.Drawing.Size(673, 43)
        Me.txtRegEx.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = "Label with text ""Regular Expression Pattern"""
        Me.Label1.AccessibleName = "Regular Expression Pattern label"
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(12, 13)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(248, 24)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "Regular Expression Pattern"
        '
        'Label2
        '
        Me.Label2.AccessibleDescription = "Label with text ""Replace Pattern"""
        Me.Label2.AccessibleName = "Replace Pattern label"
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label2.Location = New System.Drawing.Point(16, 99)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(96, 24)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Replace Pattern"
        '
        'lblResultCount
        '
        Me.lblResultCount.AccessibleDescription = "Label for showing results on RegEx Tester tab"
        Me.lblResultCount.AccessibleName = "results label on RegEx Tester tab"
        Me.lblResultCount.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.lblResultCount.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblResultCount.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblResultCount.Location = New System.Drawing.Point(78, 308)
        Me.lblResultCount.Name = "lblResultCount"
        Me.lblResultCount.Size = New System.Drawing.Size(170, 24)
        Me.lblResultCount.TabIndex = 10
        Me.lblResultCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label4
        '
        Me.Label4.AccessibleDescription = "Label with text ""Results"""
        Me.Label4.AccessibleName = "Results"
        Me.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label4.Location = New System.Drawing.Point(13, 313)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(72, 24)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "Results"
        '
        'chkMultiline
        '
        Me.chkMultiline.AccessibleDescription = "Checkbox with text ""Multiline"""
        Me.chkMultiline.AccessibleName = "Multiline"
        Me.chkMultiline.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.chkMultiline.Location = New System.Drawing.Point(461, 93)
        Me.chkMultiline.Name = "chkMultiline"
        Me.chkMultiline.Size = New System.Drawing.Size(88, 24)
        Me.chkMultiline.TabIndex = 3
        Me.chkMultiline.Text = "&Multiline"
        '
        'chkIgnoreCase
        '
        Me.chkIgnoreCase.AccessibleDescription = "CheckBox with text ""Ignore case"""
        Me.chkIgnoreCase.AccessibleName = "Ignore case"
        Me.chkIgnoreCase.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.chkIgnoreCase.Location = New System.Drawing.Point(272, 93)
        Me.chkIgnoreCase.Name = "chkIgnoreCase"
        Me.chkIgnoreCase.Size = New System.Drawing.Size(96, 24)
        Me.chkIgnoreCase.TabIndex = 1
        Me.chkIgnoreCase.Text = "&Ignore case"
        '
        'chkShowCaptures
        '
        Me.chkShowCaptures.AccessibleDescription = "CheckBox with text ""Show Captures"""
        Me.chkShowCaptures.AccessibleName = "Show Captures"
        Me.chkShowCaptures.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkShowCaptures.CheckAlign = System.Drawing.ContentAlignment.TopLeft
        Me.chkShowCaptures.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.chkShowCaptures.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.chkShowCaptures.Location = New System.Drawing.Point(1112, 312)
        Me.chkShowCaptures.Name = "chkShowCaptures"
        Me.chkShowCaptures.Size = New System.Drawing.Size(112, 24)
        Me.chkShowCaptures.TabIndex = 21
        Me.chkShowCaptures.Text = "Show &Captures"
        Me.chkShowCaptures.TextAlign = System.Drawing.ContentAlignment.TopLeft
        '
        'chkShowGroups
        '
        Me.chkShowGroups.AccessibleDescription = "CheckBox with text ""Show Groups"""
        Me.chkShowGroups.AccessibleName = "Show Groups"
        Me.chkShowGroups.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkShowGroups.CheckAlign = System.Drawing.ContentAlignment.TopLeft
        Me.chkShowGroups.Checked = True
        Me.chkShowGroups.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkShowGroups.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.chkShowGroups.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.chkShowGroups.Location = New System.Drawing.Point(1015, 312)
        Me.chkShowGroups.Name = "chkShowGroups"
        Me.chkShowGroups.Size = New System.Drawing.Size(112, 24)
        Me.chkShowGroups.TabIndex = 22
        Me.chkShowGroups.Text = "Show &Groups"
        Me.chkShowGroups.TextAlign = System.Drawing.ContentAlignment.TopLeft
        '
        'lvcUrl
        '
        Me.lvcUrl.Name = "lvcUrl"
        Me.lvcUrl.Text = "Url"
        Me.lvcUrl.Width = 696
        '
        'optImages
        '
        Me.optImages.AccessibleDescription = "RadioButton with text ""Images"""
        Me.optImages.AccessibleName = "Images"
        Me.optImages.Checked = True
        Me.optImages.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.optImages.Location = New System.Drawing.Point(526, 13)
        Me.optImages.Name = "optImages"
        Me.optImages.Size = New System.Drawing.Size(64, 24)
        Me.optImages.TabIndex = 3
        Me.optImages.Text = "&Images"
        '
        'optLinks
        '
        Me.optLinks.AccessibleDescription = "RadioButton with text ""Links"""
        Me.optLinks.AccessibleName = "Links"
        Me.optLinks.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.optLinks.Location = New System.Drawing.Point(457, 12)
        Me.optLinks.Name = "optLinks"
        Me.optLinks.Size = New System.Drawing.Size(61, 24)
        Me.optLinks.TabIndex = 2
        Me.optLinks.Text = "&Links"
        '
        'btnScrape
        '
        Me.btnScrape.AccessibleDescription = "Button with text ""Scrape!"""
        Me.btnScrape.AccessibleName = "Scrape!"
        Me.btnScrape.BackColor = System.Drawing.SystemColors.Control
        Me.btnScrape.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.btnScrape.ForeColor = System.Drawing.SystemColors.ControlText
        Me.btnScrape.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnScrape.Location = New System.Drawing.Point(600, 13)
        Me.btnScrape.Name = "btnScrape"
        Me.btnScrape.Size = New System.Drawing.Size(104, 23)
        Me.btnScrape.TabIndex = 1
        Me.btnScrape.Text = "&Scrape!"
        Me.btnScrape.UseVisualStyleBackColor = False
        '
        'lvcSrc
        '
        Me.lvcSrc.Name = "lvcSrc"
        Me.lvcSrc.Text = "Src"
        Me.lvcSrc.Width = 300
        '
        'lvcWidth
        '
        Me.lvcWidth.Name = "lvcWidth"
        Me.lvcWidth.Text = "Width"
        Me.lvcWidth.Width = 50
        '
        'lvcHeight
        '
        Me.lvcHeight.Name = "lvcHeight"
        Me.lvcHeight.Text = "Height"
        Me.lvcHeight.Width = 50
        '
        'lvcAlt
        '
        Me.lvcAlt.Name = "lvcAlt"
        Me.lvcAlt.Text = "Alt"
        Me.lvcAlt.Width = 296
        '
        'txtURL
        '
        Me.txtURL.AccessibleDescription = "TextBox for entering Web address"
        Me.txtURL.AccessibleName = "Web address TextBox"
        Me.txtURL.Location = New System.Drawing.Point(67, 13)
        Me.txtURL.Name = "txtURL"
        Me.txtURL.Size = New System.Drawing.Size(373, 20)
        Me.txtURL.TabIndex = 0
        Me.txtURL.Text = "http://www.microsoft.com/net"
        '
        'lblResults
        '
        Me.lblResults.AccessibleDescription = "Label for displaying results of Find a Number"
        Me.lblResults.AccessibleName = "Find a Number results"
        Me.lblResults.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblResults.Location = New System.Drawing.Point(23, 89)
        Me.lblResults.Name = "lblResults"
        Me.lblResults.Size = New System.Drawing.Size(397, 23)
        Me.lblResults.TabIndex = 14
        '
        'btnFindNumber
        '
        Me.btnFindNumber.AccessibleDescription = "Button with text ""Find the Number!"""
        Me.btnFindNumber.AccessibleName = "Find the Number!"
        Me.btnFindNumber.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnFindNumber.Location = New System.Drawing.Point(292, 56)
        Me.btnFindNumber.Name = "btnFindNumber"
        Me.btnFindNumber.Size = New System.Drawing.Size(128, 23)
        Me.btnFindNumber.TabIndex = 1
        Me.btnFindNumber.Text = "&Find the Number!"
        '
        'GroupBox1
        '
        Me.GroupBox1.AccessibleDescription = "GroupBox with text ""Find a Number"""
        Me.GroupBox1.AccessibleName = "Find a Number UI elements"
        Me.GroupBox1.Controls.Add(Me.lblResults)
        Me.GroupBox1.Controls.Add(Me.btnFindNumber)
        Me.GroupBox1.Controls.Add(Me.Label12)
        Me.GroupBox1.Controls.Add(Me.txtFindNumber)
        Me.GroupBox1.Location = New System.Drawing.Point(32, 48)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(440, 136)
        Me.GroupBox1.TabIndex = 11
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Find a Number"
        '
        'Label12
        '
        Me.Label12.AccessibleDescription = "Label with text stating to type in a string with a number"
        Me.Label12.AccessibleName = "Find a Number instructional text"
        Me.Label12.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label12.Location = New System.Drawing.Point(20, 32)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(264, 23)
        Me.Label12.TabIndex = 12
        Me.Label12.Text = "Type in a string with an embedded number:"
        '
        'txtFindNumber
        '
        Me.txtFindNumber.AccessibleDescription = "TextBox with sample text for Find a Number example"
        Me.txtFindNumber.AccessibleName = "Find a Number sample text"
        Me.txtFindNumber.Location = New System.Drawing.Point(20, 56)
        Me.txtFindNumber.Name = "txtFindNumber"
        Me.txtFindNumber.Size = New System.Drawing.Size(264, 20)
        Me.txtFindNumber.TabIndex = 0
        Me.txtFindNumber.Text = "akjsdi sd8902jdklsqpoeou"
        '
        'lblValid
        '
        Me.lblValid.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblValid.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblValid.Location = New System.Drawing.Point(24, 32)
        Me.lblValid.Name = "lblValid"
        Me.lblValid.Size = New System.Drawing.Size(216, 23)
        Me.lblValid.TabIndex = 15
        Me.lblValid.Text = "All fields are currently validated."
        Me.lblValid.Visible = False
        '
        'grpValidation
        '
        Me.grpValidation.AccessibleDescription = "GroupBox with Text ""Validation"""
        Me.grpValidation.Controls.Add(Me.lblValid)
        Me.grpValidation.Controls.Add(Me.Label11)
        Me.grpValidation.Controls.Add(Me.Label9)
        Me.grpValidation.Controls.Add(Me.Label8)
        Me.grpValidation.Controls.Add(Me.btnValidate)
        Me.grpValidation.Controls.Add(Me.txtEmail)
        Me.grpValidation.Controls.Add(Me.txtDate)
        Me.grpValidation.Controls.Add(Me.txtZip)
        Me.grpValidation.Controls.Add(Me.Label5)
        Me.grpValidation.Controls.Add(Me.Label7)
        Me.grpValidation.Controls.Add(Me.Label6)
        Me.grpValidation.Location = New System.Drawing.Point(32, 224)
        Me.grpValidation.Name = "grpValidation"
        Me.grpValidation.Size = New System.Drawing.Size(656, 232)
        Me.grpValidation.TabIndex = 6
        Me.grpValidation.TabStop = False
        Me.grpValidation.Text = "Validation"
        '
        'Label11
        '
        Me.Label11.AccessibleDescription = "Label with instructional text for e-Mail address"
        Me.Label11.AccessibleName = "e-Mail address instructions"
        Me.Label11.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label11.Location = New System.Drawing.Point(288, 150)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(328, 40)
        Me.Label11.TabIndex = 14
        Me.Label11.Text = "An e-Mail address in the form someone@example.com. Name and DomainName can have m" & _
            "ultiple names separated by a period."
        '
        'Label9
        '
        Me.Label9.AccessibleDescription = "Label with instructional text for date"
        Me.Label9.AccessibleName = "date instructions"
        Me.Label9.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label9.Location = New System.Drawing.Point(205, 117)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(426, 23)
        Me.Label9.TabIndex = 13
        Me.Label9.Text = "A date in mm-dd-yyyy or mm-dd-yy format. A slash can also be used as a separator." & _
            ""
        '
        'Label8
        '
        Me.Label8.AccessibleDescription = "Label with instructional text for Zip code"
        Me.Label8.AccessibleName = "zip code instructions"
        Me.Label8.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label8.Location = New System.Drawing.Point(205, 76)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(426, 23)
        Me.Label8.TabIndex = 12
        Me.Label8.Text = "A 5-digit or 5+4 digit zip code."
        '
        'btnValidate
        '
        Me.btnValidate.AccessibleDescription = "Button with text ""Validate!"""
        Me.btnValidate.AccessibleName = "Validate!"
        Me.btnValidate.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnValidate.Location = New System.Drawing.Point(96, 184)
        Me.btnValidate.Name = "btnValidate"
        Me.btnValidate.Size = New System.Drawing.Size(75, 23)
        Me.btnValidate.TabIndex = 3
        Me.btnValidate.Text = "&Validate!"
        '
        'txtEmail
        '
        Me.txtEmail.AccessibleDescription = "TextBox with sample e-Mail address"
        Me.txtEmail.AccessibleName = "sample e-Mail address"
        Me.txtEmail.Location = New System.Drawing.Point(96, 152)
        Me.txtEmail.Name = "txtEmail"
        Me.txtEmail.Size = New System.Drawing.Size(184, 20)
        Me.txtEmail.TabIndex = 2
        Me.txtEmail.Text = "someone@example.com"
        '
        'txtDate
        '
        Me.txtDate.AccessibleDescription = "TextBox with sample date"
        Me.txtDate.AccessibleName = "sample date"
        Me.txtDate.Location = New System.Drawing.Point(96, 112)
        Me.txtDate.Name = "txtDate"
        Me.txtDate.Size = New System.Drawing.Size(100, 20)
        Me.txtDate.TabIndex = 1
        Me.txtDate.Text = "7/12/2002"
        '
        'txtZip
        '
        Me.txtZip.AccessibleDescription = "TextBox with sample Zip code"
        Me.txtZip.AccessibleName = "sample Zip code"
        Me.txtZip.Location = New System.Drawing.Point(96, 72)
        Me.txtZip.Name = "txtZip"
        Me.txtZip.Size = New System.Drawing.Size(100, 20)
        Me.txtZip.TabIndex = 0
        Me.txtZip.Text = "99999-9999"
        '
        'Label5
        '
        Me.Label5.AccessibleDescription = "Label with text ""Email"""
        Me.Label5.AccessibleName = "Email label"
        Me.Label5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label5.Location = New System.Drawing.Point(58, 155)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(40, 23)
        Me.Label5.TabIndex = 8
        Me.Label5.Text = "e-Mail:"
        '
        'Label7
        '
        Me.Label7.AccessibleDescription = "Label with text ""Zip Code"""
        Me.Label7.AccessibleName = "Zip Code label"
        Me.Label7.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label7.Location = New System.Drawing.Point(42, 74)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(56, 23)
        Me.Label7.TabIndex = 10
        Me.Label7.Text = "Zip Code:"
        '
        'Label6
        '
        Me.Label6.AccessibleDescription = "Label with text ""Date"""
        Me.Label6.AccessibleName = "Date label"
        Me.Label6.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label6.Location = New System.Drawing.Point(64, 112)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(40, 23)
        Me.Label6.TabIndex = 9
        Me.Label6.Text = "Date:"
        '
        'TabControl1
        '
        Me.TabControl1.AccessibleDescription = "TabControl for the How To demo"
        Me.TabControl1.AccessibleName = "Demo TabControl"
        Me.TabControl1.Controls.Add(Me.pgeSimple)
        Me.TabControl1.Controls.Add(Me.pgeScreenScrape)
        Me.TabControl1.Controls.Add(Me.pgeRegExTester)
        Me.TabControl1.ItemSize = New System.Drawing.Size(91, 18)
        Me.TabControl1.Location = New System.Drawing.Point(16, 32)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(728, 516)
        Me.TabControl1.TabIndex = 1
        '
        'pgeSimple
        '
        Me.pgeSimple.Controls.Add(Me.GroupBox1)
        Me.pgeSimple.Controls.Add(Me.grpValidation)
        Me.pgeSimple.Location = New System.Drawing.Point(4, 22)
        Me.pgeSimple.Name = "pgeSimple"
        Me.pgeSimple.Size = New System.Drawing.Size(720, 490)
        Me.pgeSimple.TabIndex = 0
        Me.pgeSimple.Text = "Simple Examples"
        Me.pgeSimple.UseVisualStyleBackColor = False
        '
        'pgeScreenScrape
        '
        Me.pgeScreenScrape.Controls.Add(Me.optImages)
        Me.pgeScreenScrape.Controls.Add(Me.optLinks)
        Me.pgeScreenScrape.Controls.Add(Me.btnScrape)
        Me.pgeScreenScrape.Controls.Add(Me.txtURL)
        Me.pgeScreenScrape.Controls.Add(Me.Label10)
        Me.pgeScreenScrape.Controls.Add(Me.lvwImages)
        Me.pgeScreenScrape.Controls.Add(Me.lvwLinks)
        Me.pgeScreenScrape.Location = New System.Drawing.Point(4, 22)
        Me.pgeScreenScrape.Name = "pgeScreenScrape"
        Me.pgeScreenScrape.Size = New System.Drawing.Size(720, 490)
        Me.pgeScreenScrape.TabIndex = 2
        Me.pgeScreenScrape.Text = "Screen Scrape"
        Me.pgeScreenScrape.UseVisualStyleBackColor = False
        '
        'Label10
        '
        Me.Label10.AccessibleDescription = "Label with text ""Address"""
        Me.Label10.AccessibleName = "Address label"
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.Label10.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label10.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label10.Location = New System.Drawing.Point(18, 16)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(53, 15)
        Me.Label10.TabIndex = 28
        Me.Label10.Text = "Address:"
        '
        'lvwImages
        '
        Me.lvwImages.AccessibleDescription = "ListView for image attributes"
        Me.lvwImages.AccessibleName = "image attributes"
        Me.lvwImages.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.lvcSrc, Me.lvcAlt, Me.lvcHeight, Me.lvcWidth})
        Me.lvwImages.FullRowSelect = True
        Me.lvwImages.GridLines = True
        Me.lvwImages.Location = New System.Drawing.Point(11, 45)
        Me.lvwImages.Name = "lvwImages"
        Me.lvwImages.Size = New System.Drawing.Size(700, 427)
        Me.lvwImages.TabIndex = 29
        Me.lvwImages.View = System.Windows.Forms.View.Details
        '
        'lvwLinks
        '
        Me.lvwLinks.AccessibleDescription = "ListView for Link Urls"
        Me.lvwLinks.AccessibleName = "Link Urls"
        Me.lvwLinks.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.lvcUrl})
        Me.lvwLinks.FullRowSelect = True
        Me.lvwLinks.GridLines = True
        Me.lvwLinks.Location = New System.Drawing.Point(11, 45)
        Me.lvwLinks.Name = "lvwLinks"
        Me.lvwLinks.Size = New System.Drawing.Size(700, 427)
        Me.lvwLinks.TabIndex = 32
        Me.lvwLinks.View = System.Windows.Forms.View.Details
        Me.lvwLinks.Visible = False
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(760, 24)
        Me.MenuStrip1.TabIndex = 2
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
        Me.ClientSize = New System.Drawing.Size(760, 581)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.TabControl1)
        Me.Name = "MainForm"
        Me.Text = "Regular Expression Sample"
        Me.pgeRegExTester.ResumeLayout(False)
        Me.pgeRegExTester.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.grpValidation.ResumeLayout(False)
        Me.grpValidation.PerformLayout()
        Me.TabControl1.ResumeLayout(False)
        Me.pgeSimple.ResumeLayout(False)
        Me.pgeScreenScrape.ResumeLayout(False)
        Me.pgeScreenScrape.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents chkSingleLine As System.Windows.Forms.CheckBox
    Friend WithEvents pgeRegExTester As System.Windows.Forms.TabPage
    Friend WithEvents txtResults As System.Windows.Forms.TextBox
    Friend WithEvents txtSource As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents btnReplace As System.Windows.Forms.Button
    Friend WithEvents btnSplit As System.Windows.Forms.Button
    Friend WithEvents btnFind As System.Windows.Forms.Button
    Friend WithEvents txtReplace As System.Windows.Forms.TextBox
    Friend WithEvents txtRegEx As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblResultCount As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents chkMultiline As System.Windows.Forms.CheckBox
    Friend WithEvents chkIgnoreCase As System.Windows.Forms.CheckBox
    Friend WithEvents chkShowCaptures As System.Windows.Forms.CheckBox
    Friend WithEvents chkShowGroups As System.Windows.Forms.CheckBox
    Friend WithEvents lvcUrl As System.Windows.Forms.ColumnHeader
    Friend WithEvents optImages As System.Windows.Forms.RadioButton
    Friend WithEvents optLinks As System.Windows.Forms.RadioButton
    Friend WithEvents btnScrape As System.Windows.Forms.Button
    Friend WithEvents lvcSrc As System.Windows.Forms.ColumnHeader
    Friend WithEvents lvcWidth As System.Windows.Forms.ColumnHeader
    Friend WithEvents lvcHeight As System.Windows.Forms.ColumnHeader
    Friend WithEvents lvcAlt As System.Windows.Forms.ColumnHeader
    Friend WithEvents OpenFileDialog2 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents txtURL As System.Windows.Forms.TextBox
    Friend WithEvents lblResults As System.Windows.Forms.Label
    Friend WithEvents btnFindNumber As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents txtFindNumber As System.Windows.Forms.TextBox
    Friend WithEvents lblValid As System.Windows.Forms.Label
    Friend WithEvents grpValidation As System.Windows.Forms.GroupBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents btnValidate As System.Windows.Forms.Button
    Friend WithEvents txtEmail As System.Windows.Forms.TextBox
    Friend WithEvents txtDate As System.Windows.Forms.TextBox
    Friend WithEvents txtZip As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents pgeSimple As System.Windows.Forms.TabPage
    Friend WithEvents pgeScreenScrape As System.Windows.Forms.TabPage
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents lvwImages As System.Windows.Forms.ListView
    Friend WithEvents lvwLinks As System.Windows.Forms.ListView
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
