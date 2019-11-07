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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.hpPlainHTML = New System.Windows.Forms.HelpProvider
        Me.btnLink3 = New System.Windows.Forms.Button
        Me.hpAdvancedCHM = New System.Windows.Forms.HelpProvider
        Me.btnLink2 = New System.Windows.Forms.Button
        Me.btnLink1 = New System.Windows.Forms.Button
        Me.ErrorProvider1 = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.btnExecute = New System.Windows.Forms.Button
        Me.txtPrice = New System.Windows.Forms.TextBox
        Me.txtProductName = New System.Windows.Forms.TextBox
        Me.tcMain = New System.Windows.Forms.TabControl
        Me.tpToolTip = New System.Windows.Forms.TabPage
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.tpPopUpHelp = New System.Windows.Forms.TabPage
        Me.Label5 = New System.Windows.Forms.Label
        Me.btnClear = New System.Windows.Forms.Button
        Me.Label4 = New System.Windows.Forms.Label
        Me.btnSave = New System.Windows.Forms.Button
        Me.rtbTextEntry = New System.Windows.Forms.RichTextBox
        Me.tpHTMLHelp = New System.Windows.Forms.TabPage
        Me.Label10 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.tpErrorHelp = New System.Windows.Forms.TabPage
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.txtNumberValue = New System.Windows.Forms.TextBox
        Me.btnSubmit = New System.Windows.Forms.Button
        Me.txtTextValue = New System.Windows.Forms.TextBox
        Me.HelpProvider1 = New System.Windows.Forms.HelpProvider
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.contentsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.indexToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.searchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        CType(Me.ErrorProvider1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tcMain.SuspendLayout()
        Me.tpToolTip.SuspendLayout()
        Me.tpPopUpHelp.SuspendLayout()
        Me.tpHTMLHelp.SuspendLayout()
        Me.tpErrorHelp.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'hpPlainHTML
        '
        Me.hpPlainHTML.HelpNamespace = "..\..\help.htm"
        '
        'btnLink3
        '
        Me.btnLink3.AccessibleDescription = "Link to basic HTML help file"
        Me.btnLink3.AccessibleName = "Link to basic HTML help file"
        Me.hpPlainHTML.SetHelpKeyword(Me.btnLink3, "help.htm")
        Me.btnLink3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnLink3.Location = New System.Drawing.Point(16, 168)
        Me.btnLink3.Name = "btnLink3"
        Me.hpPlainHTML.SetShowHelp(Me.btnLink3, True)
        Me.btnLink3.Size = New System.Drawing.Size(376, 23)
        Me.btnLink3.TabIndex = 4
        Me.btnLink3.Text = "Link to Basic &HTML help file"
        '
        'hpAdvancedCHM
        '
        Me.hpAdvancedCHM.HelpNamespace = "..\..\htmlhelp.chm"
        '
        'btnLink2
        '
        Me.btnLink2.AccessibleDescription = "Link to compiling keyword indexes"
        Me.btnLink2.AccessibleName = "Link to compiling keyword indexes"
        Me.hpAdvancedCHM.SetHelpKeyword(Me.btnLink2, "compiling keyword indexes")
        Me.hpAdvancedCHM.SetHelpNavigator(Me.btnLink2, System.Windows.Forms.HelpNavigator.KeywordIndex)
        Me.btnLink2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnLink2.Location = New System.Drawing.Point(16, 88)
        Me.btnLink2.Name = "btnLink2"
        Me.hpAdvancedCHM.SetShowHelp(Me.btnLink2, True)
        Me.btnLink2.Size = New System.Drawing.Size(376, 23)
        Me.btnLink2.TabIndex = 1
        Me.btnLink2.Text = "Link To ""&compiling keyword indexes"""
        '
        'btnLink1
        '
        Me.btnLink1.AccessibleDescription = "Link to Compiling a help project"
        Me.btnLink1.AccessibleName = "Link to Compiling a help project"
        Me.hpAdvancedCHM.SetHelpKeyword(Me.btnLink1, "about compiling a help project")
        Me.btnLink1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnLink1.Location = New System.Drawing.Point(17, 56)
        Me.btnLink1.Name = "btnLink1"
        Me.hpAdvancedCHM.SetShowHelp(Me.btnLink1, True)
        Me.btnLink1.Size = New System.Drawing.Size(376, 24)
        Me.btnLink1.TabIndex = 0
        Me.btnLink1.Text = "Link To ""&about compiling a help project"""
        '
        'ErrorProvider1
        '
        Me.ErrorProvider1.ContainerControl = Me
        Me.ErrorProvider1.Icon = CType(resources.GetObject("ErrorProvider1.Icon"), System.Drawing.Icon)
        '
        'btnExecute
        '
        Me.btnExecute.AccessibleDescription = "Execute button"
        Me.btnExecute.AccessibleName = "Execute button"
        Me.btnExecute.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnExecute.Location = New System.Drawing.Point(24, 120)
        Me.btnExecute.Name = "btnExecute"
        Me.btnExecute.Size = New System.Drawing.Size(75, 23)
        Me.btnExecute.TabIndex = 3
        Me.btnExecute.Text = "Execute"
        Me.ToolTip1.SetToolTip(Me.btnExecute, "Execute the Query.")
        '
        'txtPrice
        '
        Me.txtPrice.AccessibleDescription = "Product Price"
        Me.txtPrice.AccessibleName = "Product Price"
        Me.txtPrice.Location = New System.Drawing.Point(216, 96)
        Me.txtPrice.Name = "txtPrice"
        Me.txtPrice.Size = New System.Drawing.Size(72, 20)
        Me.txtPrice.TabIndex = 2
        Me.ToolTip1.SetToolTip(Me.txtPrice, "Enter a price.")
        '
        'txtProductName
        '
        Me.txtProductName.AccessibleDescription = "Product Name"
        Me.txtProductName.AccessibleName = "Product Name"
        Me.txtProductName.Location = New System.Drawing.Point(24, 96)
        Me.txtProductName.Name = "txtProductName"
        Me.txtProductName.Size = New System.Drawing.Size(184, 20)
        Me.txtProductName.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.txtProductName, "Enter a product name.")
        '
        'tcMain
        '
        Me.tcMain.AccessibleDescription = "Tab Control"
        Me.tcMain.AccessibleName = "Tab Control"
        Me.tcMain.Controls.Add(Me.tpToolTip)
        Me.tcMain.Controls.Add(Me.tpPopUpHelp)
        Me.tcMain.Controls.Add(Me.tpHTMLHelp)
        Me.tcMain.Controls.Add(Me.tpErrorHelp)
        Me.tcMain.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.tcMain.ItemSize = New System.Drawing.Size(76, 18)
        Me.tcMain.Location = New System.Drawing.Point(0, 25)
        Me.tcMain.Name = "tcMain"
        Me.tcMain.SelectedIndex = 0
        Me.tcMain.ShowToolTips = True
        Me.tcMain.Size = New System.Drawing.Size(416, 310)
        Me.tcMain.TabIndex = 2
        '
        'tpToolTip
        '
        Me.tpToolTip.AccessibleDescription = "Tab Page Tool Tip"
        Me.tpToolTip.AccessibleName = "Tab Page Tool Tip"
        Me.tpToolTip.Controls.Add(Me.Label3)
        Me.tpToolTip.Controls.Add(Me.Label2)
        Me.tpToolTip.Controls.Add(Me.btnExecute)
        Me.tpToolTip.Controls.Add(Me.txtPrice)
        Me.tpToolTip.Controls.Add(Me.txtProductName)
        Me.tpToolTip.Controls.Add(Me.Label1)
        Me.tpToolTip.Location = New System.Drawing.Point(4, 22)
        Me.tpToolTip.Name = "tpToolTip"
        Me.tpToolTip.Size = New System.Drawing.Size(408, 284)
        Me.tpToolTip.TabIndex = 0
        Me.tpToolTip.Text = "Tool Tip Help"
        Me.tpToolTip.ToolTipText = "Tab One"
        '
        'Label3
        '
        Me.Label3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label3.Location = New System.Drawing.Point(216, 80)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(64, 16)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Price"
        '
        'Label2
        '
        Me.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label2.Location = New System.Drawing.Point(24, 80)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(168, 16)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Product Name"
        '
        'Label1
        '
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(16, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(376, 56)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = resources.GetString("Label1.Text")
        '
        'tpPopUpHelp
        '
        Me.tpPopUpHelp.Controls.Add(Me.Label5)
        Me.tpPopUpHelp.Controls.Add(Me.btnClear)
        Me.tpPopUpHelp.Controls.Add(Me.Label4)
        Me.tpPopUpHelp.Controls.Add(Me.btnSave)
        Me.tpPopUpHelp.Controls.Add(Me.rtbTextEntry)
        Me.tpPopUpHelp.Location = New System.Drawing.Point(4, 22)
        Me.tpPopUpHelp.Name = "tpPopUpHelp"
        Me.tpPopUpHelp.Size = New System.Drawing.Size(408, 284)
        Me.tpPopUpHelp.TabIndex = 1
        Me.tpPopUpHelp.Text = "PopUp Help"
        '
        'Label5
        '
        Me.Label5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label5.Location = New System.Drawing.Point(16, 88)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(100, 16)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Text Entry"
        '
        'btnClear
        '
        Me.btnClear.AccessibleDescription = "Clear button"
        Me.btnClear.AccessibleName = "Clear button"
        Me.HelpProvider1.SetHelpString(Me.btnClear, "Clear text.")
        Me.btnClear.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnClear.Location = New System.Drawing.Point(96, 208)
        Me.btnClear.Name = "btnClear"
        Me.HelpProvider1.SetShowHelp(Me.btnClear, True)
        Me.btnClear.Size = New System.Drawing.Size(75, 23)
        Me.btnClear.TabIndex = 3
        Me.btnClear.Text = "Clear"
        '
        'Label4
        '
        Me.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label4.Location = New System.Drawing.Point(16, 16)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(384, 72)
        Me.Label4.TabIndex = 2
        Me.Label4.Text = resources.GetString("Label4.Text")
        '
        'btnSave
        '
        Me.btnSave.AccessibleDescription = "Save Button"
        Me.btnSave.AccessibleName = "Save Button"
        Me.HelpProvider1.SetHelpString(Me.btnSave, "Save text.")
        Me.btnSave.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnSave.Location = New System.Drawing.Point(16, 208)
        Me.btnSave.Name = "btnSave"
        Me.HelpProvider1.SetShowHelp(Me.btnSave, True)
        Me.btnSave.Size = New System.Drawing.Size(75, 23)
        Me.btnSave.TabIndex = 1
        Me.btnSave.Text = "Save"
        '
        'rtbTextEntry
        '
        Me.rtbTextEntry.AccessibleDescription = "Test Text Entry form"
        Me.rtbTextEntry.AccessibleName = "Test Text Entry form"
        Me.HelpProvider1.SetHelpString(Me.rtbTextEntry, "Enter rich text here.")
        Me.rtbTextEntry.Location = New System.Drawing.Point(16, 104)
        Me.rtbTextEntry.Name = "rtbTextEntry"
        Me.HelpProvider1.SetShowHelp(Me.rtbTextEntry, True)
        Me.rtbTextEntry.Size = New System.Drawing.Size(376, 96)
        Me.rtbTextEntry.TabIndex = 0
        Me.rtbTextEntry.Text = ""
        '
        'tpHTMLHelp
        '
        Me.tpHTMLHelp.Controls.Add(Me.Label10)
        Me.tpHTMLHelp.Controls.Add(Me.btnLink3)
        Me.tpHTMLHelp.Controls.Add(Me.Label9)
        Me.tpHTMLHelp.Controls.Add(Me.btnLink2)
        Me.tpHTMLHelp.Controls.Add(Me.btnLink1)
        Me.tpHTMLHelp.Location = New System.Drawing.Point(4, 22)
        Me.tpHTMLHelp.Name = "tpHTMLHelp"
        Me.tpHTMLHelp.Size = New System.Drawing.Size(408, 284)
        Me.tpHTMLHelp.TabIndex = 2
        Me.tpHTMLHelp.Text = "HTML Help"
        '
        'Label10
        '
        Me.Label10.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label10.Location = New System.Drawing.Point(16, 120)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(376, 40)
        Me.Label10.TabIndex = 5
        Me.Label10.Text = "Left clicking the button below with the question mark will bring up a basic HTML " & _
            "page that could have information about this control and how it is used."
        '
        'Label9
        '
        Me.Label9.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label9.Location = New System.Drawing.Point(16, 16)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(376, 40)
        Me.Label9.TabIndex = 2
        Me.Label9.Text = "Left click the question mark on the top right of the window then left click one o" & _
            "f the buttons to just to a help topic in the help file."
        '
        'tpErrorHelp
        '
        Me.tpErrorHelp.Controls.Add(Me.Label8)
        Me.tpErrorHelp.Controls.Add(Me.Label7)
        Me.tpErrorHelp.Controls.Add(Me.Label6)
        Me.tpErrorHelp.Controls.Add(Me.txtNumberValue)
        Me.tpErrorHelp.Controls.Add(Me.btnSubmit)
        Me.tpErrorHelp.Controls.Add(Me.txtTextValue)
        Me.tpErrorHelp.Location = New System.Drawing.Point(4, 22)
        Me.tpErrorHelp.Name = "tpErrorHelp"
        Me.tpErrorHelp.Size = New System.Drawing.Size(408, 284)
        Me.tpErrorHelp.TabIndex = 3
        Me.tpErrorHelp.Text = "Error Help"
        '
        'Label8
        '
        Me.Label8.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label8.Location = New System.Drawing.Point(16, 16)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(368, 56)
        Me.Label8.TabIndex = 1
        Me.Label8.Text = resources.GetString("Label8.Text")
        '
        'Label7
        '
        Me.Label7.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label7.Location = New System.Drawing.Point(16, 120)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(72, 16)
        Me.Label7.TabIndex = 4
        Me.Label7.Text = "Number here"
        '
        'Label6
        '
        Me.Label6.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label6.Location = New System.Drawing.Point(16, 80)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(56, 16)
        Me.Label6.TabIndex = 2
        Me.Label6.Text = "Text here"
        '
        'txtNumberValue
        '
        Me.txtNumberValue.AccessibleDescription = "Number Input"
        Me.txtNumberValue.AccessibleName = "Number Input"
        Me.txtNumberValue.Location = New System.Drawing.Point(16, 136)
        Me.txtNumberValue.Name = "txtNumberValue"
        Me.txtNumberValue.Size = New System.Drawing.Size(64, 20)
        Me.txtNumberValue.TabIndex = 5
        '
        'btnSubmit
        '
        Me.btnSubmit.AccessibleDescription = "Submit Button for Form"
        Me.btnSubmit.AccessibleName = "Submit Button"
        Me.btnSubmit.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnSubmit.Location = New System.Drawing.Point(16, 160)
        Me.btnSubmit.Name = "btnSubmit"
        Me.btnSubmit.Size = New System.Drawing.Size(75, 23)
        Me.btnSubmit.TabIndex = 5
        Me.btnSubmit.Text = "&Submit"
        '
        'txtTextValue
        '
        Me.txtTextValue.AccessibleDescription = "Text Input"
        Me.txtTextValue.AccessibleName = "Text Input"
        Me.txtTextValue.Location = New System.Drawing.Point(16, 96)
        Me.txtTextValue.Name = "txtTextValue"
        Me.txtTextValue.Size = New System.Drawing.Size(160, 20)
        Me.txtTextValue.TabIndex = 3
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem, Me.ToolStripMenuItem1})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(416, 24)
        Me.MenuStrip1.TabIndex = 3
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
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.contentsToolStripMenuItem, Me.indexToolStripMenuItem, Me.searchToolStripMenuItem})
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Text = "&Help"
        '
        'contentsToolStripMenuItem
        '
        Me.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem"
        Me.contentsToolStripMenuItem.Text = "&Contents"
        '
        'indexToolStripMenuItem
        '
        Me.indexToolStripMenuItem.Name = "indexToolStripMenuItem"
        Me.indexToolStripMenuItem.Text = "&Index"
        '
        'searchToolStripMenuItem
        '
        Me.searchToolStripMenuItem.Name = "searchToolStripMenuItem"
        Me.searchToolStripMenuItem.Text = "&Search"
        '
        'Form1
        '
        Me.ClientSize = New System.Drawing.Size(416, 335)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.tcMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.HelpButton = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.ErrorProvider1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tcMain.ResumeLayout(False)
        Me.tpToolTip.ResumeLayout(False)
        Me.tpToolTip.PerformLayout()
        Me.tpPopUpHelp.ResumeLayout(False)
        Me.tpHTMLHelp.ResumeLayout(False)
        Me.tpErrorHelp.ResumeLayout(False)
        Me.tpErrorHelp.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents hpPlainHTML As System.Windows.Forms.HelpProvider
    Friend WithEvents hpAdvancedCHM As System.Windows.Forms.HelpProvider
    Friend WithEvents ErrorProvider1 As System.Windows.Forms.ErrorProvider
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents tcMain As System.Windows.Forms.TabControl
    Friend WithEvents tpToolTip As System.Windows.Forms.TabPage
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnExecute As System.Windows.Forms.Button
    Friend WithEvents txtPrice As System.Windows.Forms.TextBox
    Friend WithEvents txtProductName As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents tpPopUpHelp As System.Windows.Forms.TabPage
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents btnClear As System.Windows.Forms.Button
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents rtbTextEntry As System.Windows.Forms.RichTextBox
    Friend WithEvents tpHTMLHelp As System.Windows.Forms.TabPage
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents btnLink3 As System.Windows.Forms.Button
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents btnLink2 As System.Windows.Forms.Button
    Friend WithEvents btnLink1 As System.Windows.Forms.Button
    Friend WithEvents tpErrorHelp As System.Windows.Forms.TabPage
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtNumberValue As System.Windows.Forms.TextBox
    Friend WithEvents btnSubmit As System.Windows.Forms.Button
    Friend WithEvents txtTextValue As System.Windows.Forms.TextBox
    Friend WithEvents HelpProvider1 As System.Windows.Forms.HelpProvider
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents helpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents contentsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents indexToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents searchToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
End Class
