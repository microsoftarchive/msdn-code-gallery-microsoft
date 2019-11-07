Partial Public Class MainForm
    Inherits System.Windows.Forms.Form


#Region " Windows Form Designer generated code "

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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents optInsert As System.Windows.Forms.RadioButton
    Friend WithEvents optRemove As System.Windows.Forms.RadioButton
    Friend WithEvents optReplace As System.Windows.Forms.RadioButton
    Friend WithEvents optSubstring As System.Windows.Forms.RadioButton
    Friend WithEvents optPadRight As System.Windows.Forms.RadioButton
    Friend WithEvents optPadLeft As System.Windows.Forms.RadioButton
    Friend WithEvents optTrim As System.Windows.Forms.RadioButton
    Friend WithEvents optToUpper As System.Windows.Forms.RadioButton
    Friend WithEvents optToLower As System.Windows.Forms.RadioButton
    Friend WithEvents optTrimStart As System.Windows.Forms.RadioButton
    Friend WithEvents optTrimEnd As System.Windows.Forms.RadioButton
    Friend WithEvents lblPrm1 As System.Windows.Forms.Label
    Friend WithEvents lblPrm2 As System.Windows.Forms.Label
    Friend WithEvents lblPrm3 As System.Windows.Forms.Label
    Friend WithEvents txtPrm1 As System.Windows.Forms.TextBox
    Friend WithEvents txtPrm2 As System.Windows.Forms.TextBox
    Friend WithEvents txtPrm3 As System.Windows.Forms.TextBox
    Friend WithEvents txtSample As System.Windows.Forms.TextBox
    Friend WithEvents lblResultsLabel As System.Windows.Forms.Label
    Friend WithEvents lblResults As System.Windows.Forms.Label
    Friend WithEvents btnRecalc As System.Windows.Forms.Button
    Friend WithEvents btnRefresh As System.Windows.Forms.Button
    Friend WithEvents ttpStrings As System.Windows.Forms.ToolTip
    Friend WithEvents pnlDemo As System.Windows.Forms.Panel
    Friend WithEvents tabStringDemo As System.Windows.Forms.TabControl
    Friend WithEvents pagReturnStrings As System.Windows.Forms.TabPage
    Friend WithEvents pagInfo As System.Windows.Forms.TabPage
    Friend WithEvents optEndsWith As System.Windows.Forms.RadioButton
    Friend WithEvents optStartsWith As System.Windows.Forms.RadioButton
    Friend WithEvents optLastIndexOfAny As System.Windows.Forms.RadioButton
    Friend WithEvents optIndexOfAny As System.Windows.Forms.RadioButton
    Friend WithEvents optIndexOf As System.Windows.Forms.RadioButton
    Friend WithEvents optLastIndexOf As System.Windows.Forms.RadioButton
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents pagShared As System.Windows.Forms.TabPage
    Friend WithEvents optFormat As System.Windows.Forms.RadioButton
    Friend WithEvents optConcat As System.Windows.Forms.RadioButton
    Friend WithEvents optCompareOrdinal As System.Windows.Forms.RadioButton
    Friend WithEvents optCompare As System.Windows.Forms.RadioButton
    Friend WithEvents optJoin As System.Windows.Forms.RadioButton
    Friend WithEvents grpResults As System.Windows.Forms.GroupBox
    Friend WithEvents grpParameters As System.Windows.Forms.GroupBox
    Friend WithEvents grpSample As System.Windows.Forms.GroupBox
    Friend WithEvents optSplit As System.Windows.Forms.RadioButton
    Friend WithEvents btnStringWriter As System.Windows.Forms.Button
    Friend WithEvents btnStringBuilder As System.Windows.Forms.Button
    Friend WithEvents pagOther As System.Windows.Forms.TabPage
    Friend WithEvents txtResults As System.Windows.Forms.TextBox
    Friend WithEvents chkDebug As System.Windows.Forms.CheckBox
    Friend WithEvents btnClear As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.tabStringDemo = New System.Windows.Forms.TabControl
        Me.pagReturnStrings = New System.Windows.Forms.TabPage
        Me.optTrimStart = New System.Windows.Forms.RadioButton
        Me.optTrimEnd = New System.Windows.Forms.RadioButton
        Me.optTrim = New System.Windows.Forms.RadioButton
        Me.optToUpper = New System.Windows.Forms.RadioButton
        Me.optToLower = New System.Windows.Forms.RadioButton
        Me.optSubstring = New System.Windows.Forms.RadioButton
        Me.optPadRight = New System.Windows.Forms.RadioButton
        Me.optPadLeft = New System.Windows.Forms.RadioButton
        Me.optReplace = New System.Windows.Forms.RadioButton
        Me.optRemove = New System.Windows.Forms.RadioButton
        Me.optInsert = New System.Windows.Forms.RadioButton
        Me.pnlDemo = New System.Windows.Forms.Panel
        Me.grpResults = New System.Windows.Forms.GroupBox
        Me.lblResultsLabel = New System.Windows.Forms.Label
        Me.lblResults = New System.Windows.Forms.Label
        Me.btnRecalc = New System.Windows.Forms.Button
        Me.grpParameters = New System.Windows.Forms.GroupBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblPrm2 = New System.Windows.Forms.Label
        Me.lblPrm3 = New System.Windows.Forms.Label
        Me.txtPrm1 = New System.Windows.Forms.TextBox
        Me.txtPrm2 = New System.Windows.Forms.TextBox
        Me.lblPrm1 = New System.Windows.Forms.Label
        Me.txtPrm3 = New System.Windows.Forms.TextBox
        Me.grpSample = New System.Windows.Forms.GroupBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.txtSample = New System.Windows.Forms.TextBox
        Me.btnRefresh = New System.Windows.Forms.Button
        Me.pagInfo = New System.Windows.Forms.TabPage
        Me.optSplit = New System.Windows.Forms.RadioButton
        Me.optEndsWith = New System.Windows.Forms.RadioButton
        Me.optStartsWith = New System.Windows.Forms.RadioButton
        Me.optLastIndexOfAny = New System.Windows.Forms.RadioButton
        Me.optLastIndexOf = New System.Windows.Forms.RadioButton
        Me.optIndexOfAny = New System.Windows.Forms.RadioButton
        Me.optIndexOf = New System.Windows.Forms.RadioButton
        Me.pagShared = New System.Windows.Forms.TabPage
        Me.optJoin = New System.Windows.Forms.RadioButton
        Me.optFormat = New System.Windows.Forms.RadioButton
        Me.optConcat = New System.Windows.Forms.RadioButton
        Me.optCompareOrdinal = New System.Windows.Forms.RadioButton
        Me.optCompare = New System.Windows.Forms.RadioButton
        Me.pagOther = New System.Windows.Forms.TabPage
        Me.btnClear = New System.Windows.Forms.Button
        Me.chkDebug = New System.Windows.Forms.CheckBox
        Me.txtResults = New System.Windows.Forms.TextBox
        Me.btnStringWriter = New System.Windows.Forms.Button
        Me.btnStringBuilder = New System.Windows.Forms.Button
        Me.ttpStrings = New System.Windows.Forms.ToolTip(Me.components)
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.tabStringDemo.SuspendLayout()
        Me.pagReturnStrings.SuspendLayout()
        Me.pnlDemo.SuspendLayout()
        Me.grpResults.SuspendLayout()
        Me.grpParameters.SuspendLayout()
        Me.grpSample.SuspendLayout()
        Me.pagInfo.SuspendLayout()
        Me.pagShared.SuspendLayout()
        Me.pagOther.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabStringDemo
        '
        Me.tabStringDemo.Controls.Add(Me.pagReturnStrings)
        Me.tabStringDemo.Controls.Add(Me.pagInfo)
        Me.tabStringDemo.Controls.Add(Me.pagShared)
        Me.tabStringDemo.Controls.Add(Me.pagOther)
        Me.tabStringDemo.Location = New System.Drawing.Point(8, 50)
        Me.tabStringDemo.Name = "tabStringDemo"
        Me.tabStringDemo.SelectedIndex = 0
        Me.tabStringDemo.Size = New System.Drawing.Size(640, 387)
        Me.tabStringDemo.TabIndex = 0
        '
        'pagReturnStrings
        '
        Me.pagReturnStrings.Controls.Add(Me.optTrimStart)
        Me.pagReturnStrings.Controls.Add(Me.optTrimEnd)
        Me.pagReturnStrings.Controls.Add(Me.optTrim)
        Me.pagReturnStrings.Controls.Add(Me.optToUpper)
        Me.pagReturnStrings.Controls.Add(Me.optToLower)
        Me.pagReturnStrings.Controls.Add(Me.optSubstring)
        Me.pagReturnStrings.Controls.Add(Me.optPadRight)
        Me.pagReturnStrings.Controls.Add(Me.optPadLeft)
        Me.pagReturnStrings.Controls.Add(Me.optReplace)
        Me.pagReturnStrings.Controls.Add(Me.optRemove)
        Me.pagReturnStrings.Controls.Add(Me.optInsert)
        Me.pagReturnStrings.Controls.Add(Me.pnlDemo)
        Me.pagReturnStrings.Location = New System.Drawing.Point(4, 22)
        Me.pagReturnStrings.Name = "pagReturnStrings"
        Me.pagReturnStrings.Size = New System.Drawing.Size(632, 361)
        Me.pagReturnStrings.TabIndex = 0
        Me.pagReturnStrings.Text = "Methods that Return Strings"
        Me.pagReturnStrings.UseVisualStyleBackColor = False
        '
        'optTrimStart
        '
        Me.optTrimStart.Appearance = System.Windows.Forms.Appearance.Button
        Me.optTrimStart.Location = New System.Drawing.Point(8, 248)
        Me.optTrimStart.Name = "optTrimStart"
        Me.optTrimStart.Size = New System.Drawing.Size(104, 22)
        Me.optTrimStart.TabIndex = 10
        Me.optTrimStart.Text = "TrimStart"
        Me.optTrimStart.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optTrimEnd
        '
        Me.optTrimEnd.Appearance = System.Windows.Forms.Appearance.Button
        Me.optTrimEnd.Location = New System.Drawing.Point(8, 224)
        Me.optTrimEnd.Name = "optTrimEnd"
        Me.optTrimEnd.Size = New System.Drawing.Size(104, 22)
        Me.optTrimEnd.TabIndex = 9
        Me.optTrimEnd.Text = "TrimEnd"
        Me.optTrimEnd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optTrim
        '
        Me.optTrim.Appearance = System.Windows.Forms.Appearance.Button
        Me.optTrim.Location = New System.Drawing.Point(8, 200)
        Me.optTrim.Name = "optTrim"
        Me.optTrim.Size = New System.Drawing.Size(104, 22)
        Me.optTrim.TabIndex = 8
        Me.optTrim.Text = "Trim"
        Me.optTrim.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optToUpper
        '
        Me.optToUpper.Appearance = System.Windows.Forms.Appearance.Button
        Me.optToUpper.Location = New System.Drawing.Point(8, 176)
        Me.optToUpper.Name = "optToUpper"
        Me.optToUpper.Size = New System.Drawing.Size(104, 22)
        Me.optToUpper.TabIndex = 7
        Me.optToUpper.Text = "ToUpper"
        Me.optToUpper.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optToLower
        '
        Me.optToLower.Appearance = System.Windows.Forms.Appearance.Button
        Me.optToLower.Location = New System.Drawing.Point(8, 152)
        Me.optToLower.Name = "optToLower"
        Me.optToLower.Size = New System.Drawing.Size(104, 22)
        Me.optToLower.TabIndex = 6
        Me.optToLower.Text = "ToLower"
        Me.optToLower.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optSubstring
        '
        Me.optSubstring.Appearance = System.Windows.Forms.Appearance.Button
        Me.optSubstring.Location = New System.Drawing.Point(8, 128)
        Me.optSubstring.Name = "optSubstring"
        Me.optSubstring.Size = New System.Drawing.Size(104, 22)
        Me.optSubstring.TabIndex = 5
        Me.optSubstring.Text = "Substring"
        Me.optSubstring.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optPadRight
        '
        Me.optPadRight.Appearance = System.Windows.Forms.Appearance.Button
        Me.optPadRight.Location = New System.Drawing.Point(8, 104)
        Me.optPadRight.Name = "optPadRight"
        Me.optPadRight.Size = New System.Drawing.Size(104, 22)
        Me.optPadRight.TabIndex = 4
        Me.optPadRight.Text = "PadRight"
        Me.optPadRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optPadLeft
        '
        Me.optPadLeft.Appearance = System.Windows.Forms.Appearance.Button
        Me.optPadLeft.Location = New System.Drawing.Point(8, 80)
        Me.optPadLeft.Name = "optPadLeft"
        Me.optPadLeft.Size = New System.Drawing.Size(104, 22)
        Me.optPadLeft.TabIndex = 3
        Me.optPadLeft.Text = "PadLeft"
        Me.optPadLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optReplace
        '
        Me.optReplace.Appearance = System.Windows.Forms.Appearance.Button
        Me.optReplace.Location = New System.Drawing.Point(8, 56)
        Me.optReplace.Name = "optReplace"
        Me.optReplace.Size = New System.Drawing.Size(104, 22)
        Me.optReplace.TabIndex = 2
        Me.optReplace.Text = "Replace"
        Me.optReplace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optRemove
        '
        Me.optRemove.Appearance = System.Windows.Forms.Appearance.Button
        Me.optRemove.Location = New System.Drawing.Point(8, 32)
        Me.optRemove.Name = "optRemove"
        Me.optRemove.Size = New System.Drawing.Size(104, 22)
        Me.optRemove.TabIndex = 1
        Me.optRemove.Text = "Remove"
        Me.optRemove.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optInsert
        '
        Me.optInsert.Appearance = System.Windows.Forms.Appearance.Button
        Me.optInsert.Checked = True
        Me.optInsert.Location = New System.Drawing.Point(8, 8)
        Me.optInsert.Name = "optInsert"
        Me.optInsert.Size = New System.Drawing.Size(104, 22)
        Me.optInsert.TabIndex = 0
        Me.optInsert.Text = "Insert"
        Me.optInsert.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlDemo
        '
        Me.pnlDemo.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlDemo.Controls.Add(Me.grpResults)
        Me.pnlDemo.Controls.Add(Me.grpParameters)
        Me.pnlDemo.Controls.Add(Me.grpSample)
        Me.pnlDemo.Location = New System.Drawing.Point(120, 8)
        Me.pnlDemo.Name = "pnlDemo"
        Me.pnlDemo.Size = New System.Drawing.Size(504, 347)
        Me.pnlDemo.TabIndex = 11
        '
        'grpResults
        '
        Me.grpResults.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpResults.Controls.Add(Me.lblResultsLabel)
        Me.grpResults.Controls.Add(Me.lblResults)
        Me.grpResults.Controls.Add(Me.btnRecalc)
        Me.grpResults.Location = New System.Drawing.Point(8, 192)
        Me.grpResults.Name = "grpResults"
        Me.grpResults.Size = New System.Drawing.Size(488, 139)
        Me.grpResults.TabIndex = 2
        Me.grpResults.TabStop = False
        '
        'lblResultsLabel
        '
        Me.lblResultsLabel.AutoSize = True
        Me.lblResultsLabel.Location = New System.Drawing.Point(8, 0)
        Me.lblResultsLabel.Name = "lblResultsLabel"
        Me.lblResultsLabel.Size = New System.Drawing.Size(42, 13)
        Me.lblResultsLabel.TabIndex = 0
        Me.lblResultsLabel.Text = "Results"
        Me.lblResultsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblResults
        '
        Me.lblResults.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblResults.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblResults.Location = New System.Drawing.Point(16, 24)
        Me.lblResults.Name = "lblResults"
        Me.lblResults.Size = New System.Drawing.Size(376, 107)
        Me.lblResults.TabIndex = 1
        '
        'btnRecalc
        '
        Me.btnRecalc.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRecalc.Location = New System.Drawing.Point(400, 24)
        Me.btnRecalc.Name = "btnRecalc"
        Me.btnRecalc.Size = New System.Drawing.Size(75, 23)
        Me.btnRecalc.TabIndex = 2
        Me.btnRecalc.Text = "&Recalc"
        Me.ttpStrings.SetToolTip(Me.btnRecalc, "Recalculate the results")
        '
        'grpParameters
        '
        Me.grpParameters.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpParameters.Controls.Add(Me.Label1)
        Me.grpParameters.Controls.Add(Me.lblPrm2)
        Me.grpParameters.Controls.Add(Me.lblPrm3)
        Me.grpParameters.Controls.Add(Me.txtPrm1)
        Me.grpParameters.Controls.Add(Me.txtPrm2)
        Me.grpParameters.Controls.Add(Me.lblPrm1)
        Me.grpParameters.Controls.Add(Me.txtPrm3)
        Me.grpParameters.Location = New System.Drawing.Point(8, 80)
        Me.grpParameters.Name = "grpParameters"
        Me.grpParameters.Size = New System.Drawing.Size(488, 104)
        Me.grpParameters.TabIndex = 1
        Me.grpParameters.TabStop = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(8, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(60, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Parameters"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblPrm2
        '
        Me.lblPrm2.Location = New System.Drawing.Point(16, 48)
        Me.lblPrm2.Name = "lblPrm2"
        Me.lblPrm2.Size = New System.Drawing.Size(120, 23)
        Me.lblPrm2.TabIndex = 3
        Me.lblPrm2.Text = "Param2"
        Me.lblPrm2.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblPrm3
        '
        Me.lblPrm3.Location = New System.Drawing.Point(16, 72)
        Me.lblPrm3.Name = "lblPrm3"
        Me.lblPrm3.Size = New System.Drawing.Size(120, 23)
        Me.lblPrm3.TabIndex = 5
        Me.lblPrm3.Text = "Param3"
        Me.lblPrm3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtPrm1
        '
        Me.txtPrm1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPrm1.Location = New System.Drawing.Point(136, 24)
        Me.txtPrm1.Name = "txtPrm1"
        Me.txtPrm1.Size = New System.Drawing.Size(200, 20)
        Me.txtPrm1.TabIndex = 2
        '
        'txtPrm2
        '
        Me.txtPrm2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPrm2.Location = New System.Drawing.Point(136, 48)
        Me.txtPrm2.Name = "txtPrm2"
        Me.txtPrm2.Size = New System.Drawing.Size(200, 20)
        Me.txtPrm2.TabIndex = 4
        '
        'lblPrm1
        '
        Me.lblPrm1.Location = New System.Drawing.Point(16, 24)
        Me.lblPrm1.Name = "lblPrm1"
        Me.lblPrm1.Size = New System.Drawing.Size(120, 23)
        Me.lblPrm1.TabIndex = 1
        Me.lblPrm1.Text = "Param1"
        Me.lblPrm1.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'txtPrm3
        '
        Me.txtPrm3.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPrm3.Location = New System.Drawing.Point(136, 72)
        Me.txtPrm3.Name = "txtPrm3"
        Me.txtPrm3.Size = New System.Drawing.Size(200, 20)
        Me.txtPrm3.TabIndex = 6
        '
        'grpSample
        '
        Me.grpSample.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpSample.Controls.Add(Me.Label2)
        Me.grpSample.Controls.Add(Me.txtSample)
        Me.grpSample.Controls.Add(Me.btnRefresh)
        Me.grpSample.Location = New System.Drawing.Point(8, 8)
        Me.grpSample.Name = "grpSample"
        Me.grpSample.Size = New System.Drawing.Size(488, 64)
        Me.grpSample.TabIndex = 0
        Me.grpSample.TabStop = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(8, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(42, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Sample"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'txtSample
        '
        Me.txtSample.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSample.Location = New System.Drawing.Point(16, 24)
        Me.txtSample.Name = "txtSample"
        Me.txtSample.Size = New System.Drawing.Size(376, 20)
        Me.txtSample.TabIndex = 1
        '
        'btnRefresh
        '
        Me.btnRefresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRefresh.Location = New System.Drawing.Point(400, 24)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(75, 23)
        Me.btnRefresh.TabIndex = 2
        Me.btnRefresh.Text = "Re&fresh"
        Me.ttpStrings.SetToolTip(Me.btnRefresh, "Refresh the sample text")
        '
        'pagInfo
        '
        Me.pagInfo.Controls.Add(Me.optSplit)
        Me.pagInfo.Controls.Add(Me.optEndsWith)
        Me.pagInfo.Controls.Add(Me.optStartsWith)
        Me.pagInfo.Controls.Add(Me.optLastIndexOfAny)
        Me.pagInfo.Controls.Add(Me.optLastIndexOf)
        Me.pagInfo.Controls.Add(Me.optIndexOfAny)
        Me.pagInfo.Controls.Add(Me.optIndexOf)
        Me.pagInfo.Location = New System.Drawing.Point(4, 22)
        Me.pagInfo.Name = "pagInfo"
        Me.pagInfo.Size = New System.Drawing.Size(632, 361)
        Me.pagInfo.TabIndex = 1
        Me.pagInfo.Text = "Methods that Return Information"
        Me.pagInfo.UseVisualStyleBackColor = False
        '
        'optSplit
        '
        Me.optSplit.Appearance = System.Windows.Forms.Appearance.Button
        Me.optSplit.Location = New System.Drawing.Point(8, 152)
        Me.optSplit.Name = "optSplit"
        Me.optSplit.Size = New System.Drawing.Size(104, 22)
        Me.optSplit.TabIndex = 6
        Me.optSplit.Text = "Split"
        Me.optSplit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optEndsWith
        '
        Me.optEndsWith.Appearance = System.Windows.Forms.Appearance.Button
        Me.optEndsWith.Location = New System.Drawing.Point(8, 128)
        Me.optEndsWith.Name = "optEndsWith"
        Me.optEndsWith.Size = New System.Drawing.Size(104, 22)
        Me.optEndsWith.TabIndex = 5
        Me.optEndsWith.Text = "EndsWith"
        Me.optEndsWith.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optStartsWith
        '
        Me.optStartsWith.Appearance = System.Windows.Forms.Appearance.Button
        Me.optStartsWith.Location = New System.Drawing.Point(8, 104)
        Me.optStartsWith.Name = "optStartsWith"
        Me.optStartsWith.Size = New System.Drawing.Size(104, 22)
        Me.optStartsWith.TabIndex = 4
        Me.optStartsWith.Text = "StartsWith"
        Me.optStartsWith.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optLastIndexOfAny
        '
        Me.optLastIndexOfAny.Appearance = System.Windows.Forms.Appearance.Button
        Me.optLastIndexOfAny.Location = New System.Drawing.Point(8, 80)
        Me.optLastIndexOfAny.Name = "optLastIndexOfAny"
        Me.optLastIndexOfAny.Size = New System.Drawing.Size(104, 22)
        Me.optLastIndexOfAny.TabIndex = 3
        Me.optLastIndexOfAny.Text = "LastIndexOfAny"
        Me.optLastIndexOfAny.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optLastIndexOf
        '
        Me.optLastIndexOf.Appearance = System.Windows.Forms.Appearance.Button
        Me.optLastIndexOf.Location = New System.Drawing.Point(8, 56)
        Me.optLastIndexOf.Name = "optLastIndexOf"
        Me.optLastIndexOf.Size = New System.Drawing.Size(104, 22)
        Me.optLastIndexOf.TabIndex = 2
        Me.optLastIndexOf.Text = "LastIndexOf"
        Me.optLastIndexOf.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optIndexOfAny
        '
        Me.optIndexOfAny.Appearance = System.Windows.Forms.Appearance.Button
        Me.optIndexOfAny.Location = New System.Drawing.Point(8, 32)
        Me.optIndexOfAny.Name = "optIndexOfAny"
        Me.optIndexOfAny.Size = New System.Drawing.Size(104, 22)
        Me.optIndexOfAny.TabIndex = 1
        Me.optIndexOfAny.Text = "IndexOfAny"
        Me.optIndexOfAny.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optIndexOf
        '
        Me.optIndexOf.Appearance = System.Windows.Forms.Appearance.Button
        Me.optIndexOf.Checked = True
        Me.optIndexOf.Location = New System.Drawing.Point(8, 8)
        Me.optIndexOf.Name = "optIndexOf"
        Me.optIndexOf.Size = New System.Drawing.Size(104, 22)
        Me.optIndexOf.TabIndex = 0
        Me.optIndexOf.Text = "IndexOf"
        Me.optIndexOf.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pagShared
        '
        Me.pagShared.Controls.Add(Me.optJoin)
        Me.pagShared.Controls.Add(Me.optFormat)
        Me.pagShared.Controls.Add(Me.optConcat)
        Me.pagShared.Controls.Add(Me.optCompareOrdinal)
        Me.pagShared.Controls.Add(Me.optCompare)
        Me.pagShared.Location = New System.Drawing.Point(4, 22)
        Me.pagShared.Name = "pagShared"
        Me.pagShared.Size = New System.Drawing.Size(632, 361)
        Me.pagShared.TabIndex = 2
        Me.pagShared.Text = "Shared Methods"
        Me.pagShared.UseVisualStyleBackColor = False
        '
        'optJoin
        '
        Me.optJoin.Appearance = System.Windows.Forms.Appearance.Button
        Me.optJoin.Location = New System.Drawing.Point(8, 104)
        Me.optJoin.Name = "optJoin"
        Me.optJoin.Size = New System.Drawing.Size(104, 22)
        Me.optJoin.TabIndex = 4
        Me.optJoin.Text = "Join"
        Me.optJoin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optFormat
        '
        Me.optFormat.Appearance = System.Windows.Forms.Appearance.Button
        Me.optFormat.Location = New System.Drawing.Point(8, 80)
        Me.optFormat.Name = "optFormat"
        Me.optFormat.Size = New System.Drawing.Size(104, 22)
        Me.optFormat.TabIndex = 3
        Me.optFormat.Text = "Format"
        Me.optFormat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optConcat
        '
        Me.optConcat.Appearance = System.Windows.Forms.Appearance.Button
        Me.optConcat.Location = New System.Drawing.Point(8, 56)
        Me.optConcat.Name = "optConcat"
        Me.optConcat.Size = New System.Drawing.Size(104, 22)
        Me.optConcat.TabIndex = 2
        Me.optConcat.Text = "Concat"
        Me.optConcat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optCompareOrdinal
        '
        Me.optCompareOrdinal.Appearance = System.Windows.Forms.Appearance.Button
        Me.optCompareOrdinal.Location = New System.Drawing.Point(8, 32)
        Me.optCompareOrdinal.Name = "optCompareOrdinal"
        Me.optCompareOrdinal.Size = New System.Drawing.Size(104, 22)
        Me.optCompareOrdinal.TabIndex = 1
        Me.optCompareOrdinal.Text = "CompareOrdinal"
        Me.optCompareOrdinal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'optCompare
        '
        Me.optCompare.Appearance = System.Windows.Forms.Appearance.Button
        Me.optCompare.Checked = True
        Me.optCompare.Location = New System.Drawing.Point(8, 8)
        Me.optCompare.Name = "optCompare"
        Me.optCompare.Size = New System.Drawing.Size(104, 22)
        Me.optCompare.TabIndex = 0
        Me.optCompare.Text = "Compare"
        Me.optCompare.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pagOther
        '
        Me.pagOther.Controls.Add(Me.btnClear)
        Me.pagOther.Controls.Add(Me.chkDebug)
        Me.pagOther.Controls.Add(Me.txtResults)
        Me.pagOther.Controls.Add(Me.btnStringWriter)
        Me.pagOther.Controls.Add(Me.btnStringBuilder)
        Me.pagOther.Location = New System.Drawing.Point(4, 22)
        Me.pagOther.Name = "pagOther"
        Me.pagOther.Size = New System.Drawing.Size(632, 361)
        Me.pagOther.TabIndex = 3
        Me.pagOther.Text = "Other Classes"
        Me.pagOther.UseVisualStyleBackColor = False
        '
        'btnClear
        '
        Me.btnClear.Location = New System.Drawing.Point(8, 104)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(104, 22)
        Me.btnClear.TabIndex = 4
        Me.btnClear.Text = "Clear Output"
        '
        'chkDebug
        '
        Me.chkDebug.Location = New System.Drawing.Point(8, 80)
        Me.chkDebug.Name = "chkDebug"
        Me.chkDebug.Size = New System.Drawing.Size(112, 24)
        Me.chkDebug.TabIndex = 3
        Me.chkDebug.Text = "Step through code"
        '
        'txtResults
        '
        Me.txtResults.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtResults.Location = New System.Drawing.Point(128, 8)
        Me.txtResults.Multiline = True
        Me.txtResults.Name = "txtResults"
        Me.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtResults.Size = New System.Drawing.Size(496, 319)
        Me.txtResults.TabIndex = 2
        '
        'btnStringWriter
        '
        Me.btnStringWriter.Location = New System.Drawing.Point(8, 32)
        Me.btnStringWriter.Name = "btnStringWriter"
        Me.btnStringWriter.Size = New System.Drawing.Size(104, 22)
        Me.btnStringWriter.TabIndex = 1
        Me.btnStringWriter.Text = "StringWriter"
        '
        'btnStringBuilder
        '
        Me.btnStringBuilder.Location = New System.Drawing.Point(8, 8)
        Me.btnStringBuilder.Name = "btnStringBuilder"
        Me.btnStringBuilder.Size = New System.Drawing.Size(104, 22)
        Me.btnStringBuilder.TabIndex = 0
        Me.btnStringBuilder.Text = "StringBuilder"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(656, 24)
        Me.MenuStrip1.TabIndex = 1
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
        'Form1
        '
        Me.AcceptButton = Me.btnRecalc
        Me.ClientSize = New System.Drawing.Size(656, 467)
        Me.Controls.Add(Me.tabStringDemo)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MaximizeBox = False
        Me.MinimumSize = New System.Drawing.Size(664, 380)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Working With Strings Sample"
        Me.tabStringDemo.ResumeLayout(False)
        Me.pagReturnStrings.ResumeLayout(False)
        Me.pnlDemo.ResumeLayout(False)
        Me.grpResults.ResumeLayout(False)
        Me.grpResults.PerformLayout()
        Me.grpParameters.ResumeLayout(False)
        Me.grpParameters.PerformLayout()
        Me.grpSample.ResumeLayout(False)
        Me.grpSample.PerformLayout()
        Me.pagInfo.ResumeLayout(False)
        Me.pagShared.ResumeLayout(False)
        Me.pagOther.ResumeLayout(False)
        Me.pagOther.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

#End Region



End Class
