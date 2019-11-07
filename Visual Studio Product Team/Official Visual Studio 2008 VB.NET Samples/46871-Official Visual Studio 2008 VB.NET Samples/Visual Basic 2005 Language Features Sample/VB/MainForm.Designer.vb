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
        Me.TabControl1 = New System.Windows.Forms.TabControl
        Me.TabPage1 = New System.Windows.Forms.TabPage
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.vsResultTextbox = New System.Windows.Forms.TextBox
        Me.JoinStringsButton = New System.Windows.Forms.Button
        Me.vsBTextBox = New System.Windows.Forms.TextBox
        Me.vsATextBox = New System.Windows.Forms.TextBox
        Me.TabPage2 = New System.Windows.Forms.TabPage
        Me.FillListboxButton = New System.Windows.Forms.Button
        Me.Label6 = New System.Windows.Forms.Label
        Me.ListBox1 = New System.Windows.Forms.ListBox
        Me.TabPage3 = New System.Windows.Forms.TabPage
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label11 = New System.Windows.Forms.Label
        Me.pair2bTextbox = New System.Windows.Forms.TextBox
        Me.pair2aTextbox = New System.Windows.Forms.TextBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label10 = New System.Windows.Forms.Label
        Me.pair1bTextBox = New System.Windows.Forms.TextBox
        Me.pair1aTextBox = New System.Windows.Forms.TextBox
        Me.pairResultTextbox = New System.Windows.Forms.TextBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.pairMatchButton = New System.Windows.Forms.Button
        Me.TabPage4 = New System.Windows.Forms.TabPage
        Me.Label15 = New System.Windows.Forms.Label
        Me.webStreamTextbox = New System.Windows.Forms.TextBox
        Me.RequestWebStreamButton = New System.Windows.Forms.Button
        Me.TabPage5 = New System.Windows.Forms.TabPage
        Me.Label14 = New System.Windows.Forms.Label
        Me.LoadValuesButton = New System.Windows.Forms.Button
        Me.TreeView1 = New System.Windows.Forms.TreeView
        Me.Label13 = New System.Windows.Forms.Label
        Me.SelectedIDTextbox = New System.Windows.Forms.TextBox
        Me.SelectedValueTextbox = New System.Windows.Forms.TextBox
        Me.Label12 = New System.Windows.Forms.Label
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.TabPage4.SuspendLayout()
        Me.TabPage5.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Controls.Add(Me.TabPage4)
        Me.TabControl1.Controls.Add(Me.TabPage5)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.TabControl1.Location = New System.Drawing.Point(0, 33)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(610, 403)
        Me.TabControl1.TabIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.Label5)
        Me.TabPage1.Controls.Add(Me.Label4)
        Me.TabPage1.Controls.Add(Me.Label3)
        Me.TabPage1.Controls.Add(Me.Label2)
        Me.TabPage1.Controls.Add(Me.Label1)
        Me.TabPage1.Controls.Add(Me.vsResultTextbox)
        Me.TabPage1.Controls.Add(Me.JoinStringsButton)
        Me.TabPage1.Controls.Add(Me.vsBTextBox)
        Me.TabPage1.Controls.Add(Me.vsATextBox)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(602, 377)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "1. Operator Overloading"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(57, 279)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(395, 13)
        Me.Label5.TabIndex = 8
        Me.Label5.Text = "Validated strings can be joined using the custom && operator just like ordinary s" & _
            "trings"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(57, 147)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(381, 13)
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "A and B are ValidatedString types, and are valid if the character count is <= 255" & _
            ""
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(57, 226)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(36, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Result"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(57, 95)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(13, 13)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "B"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(57, 44)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(13, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "A"
        '
        'vsResultTextbox
        '
        Me.vsResultTextbox.Location = New System.Drawing.Point(57, 242)
        Me.vsResultTextbox.Name = "vsResultTextbox"
        Me.vsResultTextbox.ReadOnly = True
        Me.vsResultTextbox.Size = New System.Drawing.Size(221, 20)
        Me.vsResultTextbox.TabIndex = 3
        '
        'JoinStringsButton
        '
        Me.JoinStringsButton.Location = New System.Drawing.Point(57, 191)
        Me.JoinStringsButton.Name = "JoinStringsButton"
        Me.JoinStringsButton.Size = New System.Drawing.Size(100, 23)
        Me.JoinStringsButton.TabIndex = 2
        Me.JoinStringsButton.Text = "Join Strings"
        '
        'vsBTextBox
        '
        Me.vsBTextBox.Location = New System.Drawing.Point(57, 111)
        Me.vsBTextBox.Name = "vsBTextBox"
        Me.vsBTextBox.Size = New System.Drawing.Size(100, 20)
        Me.vsBTextBox.TabIndex = 1
        '
        'vsATextBox
        '
        Me.vsATextBox.Location = New System.Drawing.Point(57, 60)
        Me.vsATextBox.Name = "vsATextBox"
        Me.vsATextBox.Size = New System.Drawing.Size(100, 20)
        Me.vsATextBox.TabIndex = 0
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.FillListboxButton)
        Me.TabPage2.Controls.Add(Me.Label6)
        Me.TabPage2.Controls.Add(Me.ListBox1)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(602, 380)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "2. Generics Consumption"
        '
        'FillListboxButton
        '
        Me.FillListboxButton.Location = New System.Drawing.Point(45, 46)
        Me.FillListboxButton.Name = "FillListboxButton"
        Me.FillListboxButton.Size = New System.Drawing.Size(75, 23)
        Me.FillListboxButton.TabIndex = 2
        Me.FillListboxButton.Text = "Fill ListBox"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(45, 20)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(291, 13)
        Me.Label6.TabIndex = 1
        Me.Label6.Text = "This list box is filled by a generic List Of ValidatedString types"
        '
        'ListBox1
        '
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.Location = New System.Drawing.Point(45, 75)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(385, 290)
        Me.ListBox1.TabIndex = 0
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.GroupBox2)
        Me.TabPage3.Controls.Add(Me.GroupBox1)
        Me.TabPage3.Controls.Add(Me.pairResultTextbox)
        Me.TabPage3.Controls.Add(Me.Label7)
        Me.TabPage3.Controls.Add(Me.pairMatchButton)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(602, 380)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "3. Generics Creation"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label8)
        Me.GroupBox2.Controls.Add(Me.Label11)
        Me.GroupBox2.Controls.Add(Me.pair2bTextbox)
        Me.GroupBox2.Controls.Add(Me.pair2aTextbox)
        Me.GroupBox2.Location = New System.Drawing.Point(224, 63)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(155, 70)
        Me.GroupBox2.TabIndex = 16
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Pair 2"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(70, 20)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(12, 13)
        Me.Label8.TabIndex = 12
        Me.Label8.Text = "b"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(3, 20)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(12, 13)
        Me.Label11.TabIndex = 11
        Me.Label11.Text = "a"
        '
        'pair2bTextbox
        '
        Me.pair2bTextbox.Location = New System.Drawing.Point(70, 36)
        Me.pair2bTextbox.Name = "pair2bTextbox"
        Me.pair2bTextbox.Size = New System.Drawing.Size(57, 20)
        Me.pair2bTextbox.TabIndex = 9
        '
        'pair2aTextbox
        '
        Me.pair2aTextbox.Location = New System.Drawing.Point(3, 36)
        Me.pair2aTextbox.Name = "pair2aTextbox"
        Me.pair2aTextbox.Size = New System.Drawing.Size(53, 20)
        Me.pair2aTextbox.TabIndex = 8
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label9)
        Me.GroupBox1.Controls.Add(Me.Label10)
        Me.GroupBox1.Controls.Add(Me.pair1bTextBox)
        Me.GroupBox1.Controls.Add(Me.pair1aTextBox)
        Me.GroupBox1.Location = New System.Drawing.Point(39, 63)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(155, 70)
        Me.GroupBox1.TabIndex = 15
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Pair 1"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(70, 20)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(12, 13)
        Me.Label9.TabIndex = 12
        Me.Label9.Text = "b"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(3, 20)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(12, 13)
        Me.Label10.TabIndex = 11
        Me.Label10.Text = "a"
        '
        'pair1bTextBox
        '
        Me.pair1bTextBox.Location = New System.Drawing.Point(70, 36)
        Me.pair1bTextBox.Name = "pair1bTextBox"
        Me.pair1bTextBox.Size = New System.Drawing.Size(57, 20)
        Me.pair1bTextBox.TabIndex = 9
        '
        'pair1aTextBox
        '
        Me.pair1aTextBox.Location = New System.Drawing.Point(3, 36)
        Me.pair1aTextBox.Name = "pair1aTextBox"
        Me.pair1aTextBox.Size = New System.Drawing.Size(53, 20)
        Me.pair1aTextBox.TabIndex = 8
        '
        'pairResultTextbox
        '
        Me.pairResultTextbox.Location = New System.Drawing.Point(41, 195)
        Me.pairResultTextbox.Name = "pairResultTextbox"
        Me.pairResultTextbox.ReadOnly = True
        Me.pairResultTextbox.Size = New System.Drawing.Size(100, 20)
        Me.pairResultTextbox.TabIndex = 14
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(41, 47)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(183, 13)
        Me.Label7.TabIndex = 13
        Me.Label7.Text = "Define the two Pair container objects:"
        '
        'pairMatchButton
        '
        Me.pairMatchButton.Location = New System.Drawing.Point(41, 149)
        Me.pairMatchButton.Name = "pairMatchButton"
        Me.pairMatchButton.Size = New System.Drawing.Size(153, 23)
        Me.pairMatchButton.TabIndex = 10
        Me.pairMatchButton.Text = "Do the Pair objects match?"
        '
        'TabPage4
        '
        Me.TabPage4.Controls.Add(Me.Label15)
        Me.TabPage4.Controls.Add(Me.webStreamTextbox)
        Me.TabPage4.Controls.Add(Me.RequestWebStreamButton)
        Me.TabPage4.Location = New System.Drawing.Point(4, 22)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage4.Size = New System.Drawing.Size(602, 380)
        Me.TabPage4.TabIndex = 3
        Me.TabPage4.Text = "4. Using Statement"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(36, 27)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(390, 13)
        Me.Label15.TabIndex = 3
        Me.Label15.Text = "The Web request stream will be properly disposed after the Using block executes."
        '
        'webStreamTextbox
        '
        Me.webStreamTextbox.Location = New System.Drawing.Point(36, 106)
        Me.webStreamTextbox.Multiline = True
        Me.webStreamTextbox.Name = "webStreamTextbox"
        Me.webStreamTextbox.ReadOnly = True
        Me.webStreamTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.webStreamTextbox.Size = New System.Drawing.Size(531, 243)
        Me.webStreamTextbox.TabIndex = 2
        '
        'RequestWebStreamButton
        '
        Me.RequestWebStreamButton.Location = New System.Drawing.Point(36, 62)
        Me.RequestWebStreamButton.Name = "RequestWebStreamButton"
        Me.RequestWebStreamButton.Size = New System.Drawing.Size(148, 23)
        Me.RequestWebStreamButton.TabIndex = 1
        Me.RequestWebStreamButton.Text = "Read Web stream"
        '
        'TabPage5
        '
        Me.TabPage5.Controls.Add(Me.Label14)
        Me.TabPage5.Controls.Add(Me.LoadValuesButton)
        Me.TabPage5.Controls.Add(Me.TreeView1)
        Me.TabPage5.Controls.Add(Me.Label13)
        Me.TabPage5.Controls.Add(Me.SelectedIDTextbox)
        Me.TabPage5.Controls.Add(Me.SelectedValueTextbox)
        Me.TabPage5.Controls.Add(Me.Label12)
        Me.TabPage5.Location = New System.Drawing.Point(4, 22)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage5.Size = New System.Drawing.Size(602, 380)
        Me.TabPage5.TabIndex = 4
        Me.TabPage5.Text = "5. TryCast and IsNot"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(69, 39)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(504, 13)
        Me.Label14.TabIndex = 7
        Me.Label14.Text = "Load values and select tree nodes to view Value and ID for each.  ID is stored in" & _
            " the node's Tag property."
        '
        'LoadValuesButton
        '
        Me.LoadValuesButton.Location = New System.Drawing.Point(230, 85)
        Me.LoadValuesButton.Name = "LoadValuesButton"
        Me.LoadValuesButton.Size = New System.Drawing.Size(140, 23)
        Me.LoadValuesButton.TabIndex = 6
        Me.LoadValuesButton.Text = "Load values"
        '
        'TreeView1
        '
        Me.TreeView1.Location = New System.Drawing.Point(69, 85)
        Me.TreeView1.Name = "TreeView1"
        Me.TreeView1.Size = New System.Drawing.Size(121, 97)
        Me.TreeView1.TabIndex = 5
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(69, 265)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(17, 13)
        Me.Label13.TabIndex = 4
        Me.Label13.Text = "ID"
        '
        'SelectedIDTextbox
        '
        Me.SelectedIDTextbox.Location = New System.Drawing.Point(69, 281)
        Me.SelectedIDTextbox.Name = "SelectedIDTextbox"
        Me.SelectedIDTextbox.ReadOnly = True
        Me.SelectedIDTextbox.Size = New System.Drawing.Size(100, 20)
        Me.SelectedIDTextbox.TabIndex = 3
        '
        'SelectedValueTextbox
        '
        Me.SelectedValueTextbox.Location = New System.Drawing.Point(69, 228)
        Me.SelectedValueTextbox.Name = "SelectedValueTextbox"
        Me.SelectedValueTextbox.ReadOnly = True
        Me.SelectedValueTextbox.Size = New System.Drawing.Size(100, 20)
        Me.SelectedValueTextbox.TabIndex = 2
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(69, 211)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(33, 13)
        Me.Label12.TabIndex = 1
        Me.Label12.Text = "Value"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(610, 26)
        Me.MenuStrip1.TabIndex = 1
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
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(610, 436)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.TabControl1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "MainForm"
        Me.Text = "VB Language Sample"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage3.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.TabPage4.ResumeLayout(False)
        Me.TabPage4.PerformLayout()
        Me.TabPage5.ResumeLayout(False)
        Me.TabPage5.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents JoinStringsButton As System.Windows.Forms.Button
    Friend WithEvents vsBTextBox As System.Windows.Forms.TextBox
    Friend WithEvents vsATextBox As System.Windows.Forms.TextBox
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
    Friend WithEvents vsResultTextbox As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents FillListboxButton As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents ListBox1 As System.Windows.Forms.ListBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents pairMatchButton As System.Windows.Forms.Button
    Friend WithEvents pair1bTextBox As System.Windows.Forms.TextBox
    Friend WithEvents pair1aTextBox As System.Windows.Forms.TextBox
    Friend WithEvents pairResultTextbox As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents pair2bTextbox As System.Windows.Forms.TextBox
    Friend WithEvents pair2aTextbox As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents RequestWebStreamButton As System.Windows.Forms.Button
    Friend WithEvents webStreamTextbox As System.Windows.Forms.TextBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents SelectedIDTextbox As System.Windows.Forms.TextBox
    Friend WithEvents SelectedValueTextbox As System.Windows.Forms.TextBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents TreeView1 As System.Windows.Forms.TreeView
    Friend WithEvents LoadValuesButton As System.Windows.Forms.Button
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
