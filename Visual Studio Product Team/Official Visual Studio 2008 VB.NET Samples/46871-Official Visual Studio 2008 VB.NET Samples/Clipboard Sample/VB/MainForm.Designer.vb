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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.PasteImageButton = New System.Windows.Forms.Button
        Me.CopyImageButton = New System.Windows.Forms.Button
        Me.PictureBox2 = New System.Windows.Forms.PictureBox
        Me.PictureBox1 = New System.Windows.Forms.PictureBox
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.PasteTextButton = New System.Windows.Forms.Button
        Me.CopyTextButton = New System.Windows.Forms.Button
        Me.TextBox2 = New System.Windows.Forms.TextBox
        Me.TextBox1 = New System.Windows.Forms.TextBox
        Me.PasteRTFButton = New System.Windows.Forms.Button
        Me.CopyRTFButton = New System.Windows.Forms.Button
        Me.RichTextBox2 = New System.Windows.Forms.RichTextBox
        Me.RichTextBox1 = New System.Windows.Forms.RichTextBox
        Me.PasteHTMLButton = New System.Windows.Forms.Button
        Me.CopyHTMLButton = New System.Windows.Forms.Button
        Me.WebBrowser2 = New System.Windows.Forms.WebBrowser
        Me.WebBrowser1 = New System.Windows.Forms.WebBrowser
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.PasteObjectButton = New System.Windows.Forms.Button
        Me.CopyObjectButton = New System.Windows.Forms.Button
        Me.TextBox4 = New System.Windows.Forms.TextBox
        Me.TextBox3 = New System.Windows.Forms.TextBox
        Me.GroupBox4 = New System.Windows.Forms.GroupBox
        Me.PasteFilesButton = New System.Windows.Forms.Button
        Me.ListBox1 = New System.Windows.Forms.ListBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.WindowsExplorerLinkLabel = New System.Windows.Forms.LinkLabel
        Me.Label1 = New System.Windows.Forms.Label
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.clearClipboardToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.GroupBox1.SuspendLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.PasteImageButton)
        Me.GroupBox1.Controls.Add(Me.CopyImageButton)
        Me.GroupBox1.Controls.Add(Me.PictureBox2)
        Me.GroupBox1.Controls.Add(Me.PictureBox1)
        Me.GroupBox1.Location = New System.Drawing.Point(13, 26)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(554, 108)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Working with Images"
        '
        'PasteImageButton
        '
        Me.PasteImageButton.Location = New System.Drawing.Point(307, 63)
        Me.PasteImageButton.Name = "PasteImageButton"
        Me.PasteImageButton.TabIndex = 3
        Me.PasteImageButton.Text = "Paste Image"
        '
        'CopyImageButton
        '
        Me.CopyImageButton.Location = New System.Drawing.Point(175, 63)
        Me.CopyImageButton.Name = "CopyImageButton"
        Me.CopyImageButton.TabIndex = 2
        Me.CopyImageButton.Text = "Copy Image"
        '
        'PictureBox2
        '
        Me.PictureBox2.BackColor = System.Drawing.SystemColors.ControlDark
        Me.PictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.PictureBox2.Location = New System.Drawing.Point(389, 20)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(140, 55)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox2.TabIndex = 1
        Me.PictureBox2.TabStop = False
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.SystemColors.ControlDark
        Me.PictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.PictureBox1.Location = New System.Drawing.Point(28, 20)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(140, 56)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.PasteTextButton)
        Me.GroupBox2.Controls.Add(Me.CopyTextButton)
        Me.GroupBox2.Controls.Add(Me.TextBox2)
        Me.GroupBox2.Controls.Add(Me.TextBox1)
        Me.GroupBox2.Controls.Add(Me.PasteRTFButton)
        Me.GroupBox2.Controls.Add(Me.CopyRTFButton)
        Me.GroupBox2.Controls.Add(Me.RichTextBox2)
        Me.GroupBox2.Controls.Add(Me.RichTextBox1)
        Me.GroupBox2.Controls.Add(Me.PasteHTMLButton)
        Me.GroupBox2.Controls.Add(Me.CopyHTMLButton)
        Me.GroupBox2.Controls.Add(Me.WebBrowser2)
        Me.GroupBox2.Controls.Add(Me.WebBrowser1)
        Me.GroupBox2.Location = New System.Drawing.Point(12, 141)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(555, 296)
        Me.GroupBox2.TabIndex = 1
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Working with Text"
        '
        'PasteTextButton
        '
        Me.PasteTextButton.Location = New System.Drawing.Point(308, 237)
        Me.PasteTextButton.Name = "PasteTextButton"
        Me.PasteTextButton.TabIndex = 13
        Me.PasteTextButton.Text = "Paste Text"
        '
        'CopyTextButton
        '
        Me.CopyTextButton.Location = New System.Drawing.Point(176, 237)
        Me.CopyTextButton.Name = "CopyTextButton"
        Me.CopyTextButton.TabIndex = 12
        Me.CopyTextButton.Text = "Copy Text"
        '
        'TextBox2
        '
        Me.TextBox2.Location = New System.Drawing.Point(392, 211)
        Me.TextBox2.Multiline = True
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(139, 75)
        Me.TextBox2.TabIndex = 11
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(30, 211)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(139, 75)
        Me.TextBox1.TabIndex = 10
        '
        'PasteRTFButton
        '
        Me.PasteRTFButton.Location = New System.Drawing.Point(306, 139)
        Me.PasteRTFButton.Name = "PasteRTFButton"
        Me.PasteRTFButton.TabIndex = 9
        Me.PasteRTFButton.Text = "Paste RTF"
        '
        'CopyRTFButton
        '
        Me.CopyRTFButton.Location = New System.Drawing.Point(174, 139)
        Me.CopyRTFButton.Name = "CopyRTFButton"
        Me.CopyRTFButton.TabIndex = 8
        Me.CopyRTFButton.Text = "Copy RTF"
        '
        'RichTextBox2
        '
        Me.RichTextBox2.Location = New System.Drawing.Point(391, 117)
        Me.RichTextBox2.Name = "RichTextBox2"
        Me.RichTextBox2.Size = New System.Drawing.Size(139, 77)
        Me.RichTextBox2.TabIndex = 7
        Me.RichTextBox2.Text = ""
        '
        'RichTextBox1
        '
        Me.RichTextBox1.Location = New System.Drawing.Point(30, 117)
        Me.RichTextBox1.Name = "RichTextBox1"
        Me.RichTextBox1.Size = New System.Drawing.Size(139, 76)
        Me.RichTextBox1.TabIndex = 6
        Me.RichTextBox1.Text = ""
        '
        'PasteHTMLButton
        '
        Me.PasteHTMLButton.Location = New System.Drawing.Point(306, 52)
        Me.PasteHTMLButton.Name = "PasteHTMLButton"
        Me.PasteHTMLButton.TabIndex = 5
        Me.PasteHTMLButton.Text = "Paste HTML"
        '
        'CopyHTMLButton
        '
        Me.CopyHTMLButton.Location = New System.Drawing.Point(174, 52)
        Me.CopyHTMLButton.Name = "CopyHTMLButton"
        Me.CopyHTMLButton.TabIndex = 4
        Me.CopyHTMLButton.Text = "Copy HTML"
        '
        'WebBrowser2
        '
        Me.WebBrowser2.Location = New System.Drawing.Point(392, 20)
        Me.WebBrowser2.Name = "WebBrowser2"
        Me.WebBrowser2.Size = New System.Drawing.Size(138, 76)
        Me.WebBrowser2.TabIndex = 3
        '
        'WebBrowser1
        '
        Me.WebBrowser1.Location = New System.Drawing.Point(29, 20)
        Me.WebBrowser1.Name = "WebBrowser1"
        Me.WebBrowser1.Size = New System.Drawing.Size(139, 78)
        Me.WebBrowser1.TabIndex = 2
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.PasteObjectButton)
        Me.GroupBox3.Controls.Add(Me.CopyObjectButton)
        Me.GroupBox3.Controls.Add(Me.TextBox4)
        Me.GroupBox3.Controls.Add(Me.TextBox3)
        Me.GroupBox3.Location = New System.Drawing.Point(12, 444)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(554, 111)
        Me.GroupBox3.TabIndex = 2
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Working with Objects"
        '
        'PasteObjectButton
        '
        Me.PasteObjectButton.Location = New System.Drawing.Point(308, 47)
        Me.PasteObjectButton.Name = "PasteObjectButton"
        Me.PasteObjectButton.TabIndex = 17
        Me.PasteObjectButton.Text = "Paste Object"
        '
        'CopyObjectButton
        '
        Me.CopyObjectButton.Location = New System.Drawing.Point(176, 47)
        Me.CopyObjectButton.Name = "CopyObjectButton"
        Me.CopyObjectButton.TabIndex = 16
        Me.CopyObjectButton.Text = "Copy Object"
        '
        'TextBox4
        '
        Me.TextBox4.BackColor = System.Drawing.SystemColors.MenuBar
        Me.TextBox4.Location = New System.Drawing.Point(392, 21)
        Me.TextBox4.Multiline = True
        Me.TextBox4.Name = "TextBox4"
        Me.TextBox4.Size = New System.Drawing.Size(139, 75)
        Me.TextBox4.TabIndex = 15
        '
        'TextBox3
        '
        Me.TextBox3.BackColor = System.Drawing.SystemColors.MenuBar
        Me.TextBox3.Location = New System.Drawing.Point(30, 21)
        Me.TextBox3.Multiline = True
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.Size = New System.Drawing.Size(139, 75)
        Me.TextBox3.TabIndex = 14
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.PasteFilesButton)
        Me.GroupBox4.Controls.Add(Me.ListBox1)
        Me.GroupBox4.Controls.Add(Me.Label2)
        Me.GroupBox4.Controls.Add(Me.WindowsExplorerLinkLabel)
        Me.GroupBox4.Controls.Add(Me.Label1)
        Me.GroupBox4.Location = New System.Drawing.Point(13, 562)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(551, 134)
        Me.GroupBox4.TabIndex = 3
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Working with Files"
        '
        'PasteFilesButton
        '
        Me.PasteFilesButton.Location = New System.Drawing.Point(175, 34)
        Me.PasteFilesButton.Name = "PasteFilesButton"
        Me.PasteFilesButton.TabIndex = 18
        Me.PasteFilesButton.Text = "Paste Files"
        '
        'ListBox1
        '
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.Location = New System.Drawing.Point(307, 16)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(222, 108)
        Me.ListBox1.TabIndex = 3
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(32, 39)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(124, 51)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Copy some files and click on the Paste Files button"
        '
        'WindowsExplorerLinkLabel
        '
        Me.WindowsExplorerLinkLabel.AutoSize = True
        Me.WindowsExplorerLinkLabel.Location = New System.Drawing.Point(61, 18)
        Me.WindowsExplorerLinkLabel.Name = "WindowsExplorerLinkLabel"
        Me.WindowsExplorerLinkLabel.Size = New System.Drawing.Size(95, 14)
        Me.WindowsExplorerLinkLabel.TabIndex = 1
        Me.WindowsExplorerLinkLabel.TabStop = True
        Me.WindowsExplorerLinkLabel.Text = "Windows Explorer"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(31, 18)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(32, 14)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Open"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem, Me.clearClipboardToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(6, 2, 0, 2)
        'Me.MenuStrip1.Raft = System.Windows.Forms.RaftingSides.Top
        Me.MenuStrip1.TabIndex = 4
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
        'clearClipboardToolStripMenuItem
        '
        Me.clearClipboardToolStripMenuItem.Name = "clearClipboardToolStripMenuItem"
        Me.clearClipboardToolStripMenuItem.Text = "&Clear Clipboard"
        '
        'Form1
        '
        Me.ClientSize = New System.Drawing.Size(586, 712)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "Form1"
        Me.Text = "Clipboard Sample"
        Me.GroupBox1.ResumeLayout(False)
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents PasteImageButton As System.Windows.Forms.Button
    Friend WithEvents CopyImageButton As System.Windows.Forms.Button
    Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents WebBrowser1 As System.Windows.Forms.WebBrowser
    Friend WithEvents RichTextBox1 As System.Windows.Forms.RichTextBox
    Friend WithEvents PasteHTMLButton As System.Windows.Forms.Button
    Friend WithEvents CopyHTMLButton As System.Windows.Forms.Button
    Friend WithEvents WebBrowser2 As System.Windows.Forms.WebBrowser
    Friend WithEvents PasteTextButton As System.Windows.Forms.Button
    Friend WithEvents CopyTextButton As System.Windows.Forms.Button
    Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents PasteRTFButton As System.Windows.Forms.Button
    Friend WithEvents CopyRTFButton As System.Windows.Forms.Button
    Friend WithEvents RichTextBox2 As System.Windows.Forms.RichTextBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents PasteObjectButton As System.Windows.Forms.Button
    Friend WithEvents CopyObjectButton As System.Windows.Forms.Button
    Friend WithEvents TextBox4 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox3 As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents WindowsExplorerLinkLabel As System.Windows.Forms.LinkLabel
    Friend WithEvents PasteFilesButton As System.Windows.Forms.Button
    Friend WithEvents ListBox1 As System.Windows.Forms.ListBox
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents clearClipboardToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
