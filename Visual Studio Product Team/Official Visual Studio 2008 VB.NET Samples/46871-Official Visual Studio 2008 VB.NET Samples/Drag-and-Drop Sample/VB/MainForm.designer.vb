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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Dim TreeNode1 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Hot Sauce")
        Dim TreeNode2 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Cod")
        Dim TreeNode3 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Salmon")
        Dim TreeNode4 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Catfish")
        Dim TreeNode5 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Fish", New System.Windows.Forms.TreeNode() {TreeNode2, TreeNode3, TreeNode4})
        Dim TreeNode6 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Crab")
        Dim TreeNode7 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Lobster")
        Dim TreeNode8 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Seafood", New System.Windows.Forms.TreeNode() {TreeNode5, TreeNode6, TreeNode7})
        Dim TreeNode9 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Spaghetti")
        Dim TreeNode10 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Fettuccini")
        Dim TreeNode11 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Pasta", New System.Windows.Forms.TreeNode() {TreeNode9, TreeNode10})
        Dim TreeNode12 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Garnishes")
        Dim TreeNode13 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Bowtie pasta")
        Dim TreeNode14 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Calamari")
        Dim TreeNode15 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Ketchup")
        Dim TreeNode16 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Spicy Brown Mustard")
        Dim TreeNode17 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Yellow Mustard")
        Dim TreeNode18 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Hot Mustard")
        Dim TreeNode19 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Mustard", New System.Windows.Forms.TreeNode() {TreeNode16, TreeNode17, TreeNode18})
        Me.picRight = New System.Windows.Forms.PictureBox
        Me.picLeft = New System.Windows.Forms.PictureBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.txtAllowDrop = New System.Windows.Forms.TextBox
        Me.txtNoDrop = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.tvwRight = New System.Windows.Forms.TreeView
        Me.Label2 = New System.Windows.Forms.Label
        Me.tvwLeft = New System.Windows.Forms.TreeView
        Me.txtSource = New System.Windows.Forms.TextBox
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        CType(Me.picRight, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picLeft, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'picRight
        '
        Me.picRight.AccessibleDescription = "Empty PictureBox as drag target"
        Me.picRight.AccessibleName = "Drag Target PictureBox control"
        Me.picRight.BackColor = System.Drawing.SystemColors.Window
        Me.picRight.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.picRight.Location = New System.Drawing.Point(325, 470)
        Me.picRight.Name = "picRight"
        Me.picRight.Size = New System.Drawing.Size(64, 56)
        Me.picRight.TabIndex = 61
        Me.picRight.TabStop = False
        '
        'picLeft
        '
        Me.picLeft.AccessibleDescription = "PictureBox with Beany.bmp as drag source"
        Me.picLeft.AccessibleName = "Drag Source PictureBox control"
        Me.picLeft.BackColor = System.Drawing.SystemColors.Window
        Me.picLeft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.picLeft.Image = CType(resources.GetObject("picLeft.Image"), System.Drawing.Image)
        Me.picLeft.Location = New System.Drawing.Point(85, 470)
        Me.picLeft.Name = "picLeft"
        Me.picLeft.Size = New System.Drawing.Size(64, 56)
        Me.picLeft.TabIndex = 60
        Me.picLeft.TabStop = False
        '
        'Label7
        '
        Me.Label7.AccessibleDescription = "Label with instructional text on dropping the picture to the other PictureBox con" & _
            "trol."
        Me.Label7.AccessibleName = "Instructional Text Label for Example 3, drag target"
        Me.Label7.Location = New System.Drawing.Point(261, 422)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(208, 48)
        Me.Label7.TabIndex = 59
        Me.Label7.Text = "Drop the image into the PictureBox control. Next, drag it back to the left Pictur" & _
            "eBox control."
        '
        'Label8
        '
        Me.Label8.AccessibleDescription = "Label with instructional text on dragging a picture to the other PictureBox contr" & _
            "ol."
        Me.Label8.AccessibleName = "Instructional Text Label for Example 3, drag source"
        Me.Label8.Location = New System.Drawing.Point(13, 422)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(216, 48)
        Me.Label8.TabIndex = 58
        Me.Label8.Text = "Click and drag the image to the right PictureBox control. The code for this is ve" & _
            "ry similar to the code for Example 1."
        '
        'Label9
        '
        Me.Label9.AccessibleDescription = "Label with text: ""Example 3:..."""
        Me.Label9.AccessibleName = "Example 3 Title Label"
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label9.Location = New System.Drawing.Point(13, 390)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(456, 23)
        Me.Label9.TabIndex = 57
        Me.Label9.Text = "Example 3: Drag-and-Drop Using PictureBox Controls"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label5
        '
        Me.Label5.AccessibleDescription = "Label with instructional text on dropping node onto TreeView"
        Me.Label5.AccessibleName = "Instructional Text Label for Example 2, drag target"
        Me.Label5.Location = New System.Drawing.Point(258, 198)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(208, 48)
        Me.Label5.TabIndex = 56
        Me.Label5.Text = "Drop the node onto an existing node. Notice that any child nodes are also dropped" & _
            "."
        '
        'Label6
        '
        Me.Label6.AccessibleDescription = "Label with instructional text on dragging a node to the other TreeView."
        Me.Label6.AccessibleName = "Instructional Text Label for Example 2, drag source"
        Me.Label6.Location = New System.Drawing.Point(10, 198)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(216, 48)
        Me.Label6.TabIndex = 55
        Me.Label6.Text = "Click a node in either TreeView and drag it to the other TreeView control."
        '
        'Label4
        '
        Me.Label4.AccessibleDescription = "Label with instructional text on dropping text onto TextBox controls"
        Me.Label4.AccessibleName = "Instructional Text Label for Example 1, drag targets"
        Me.Label4.Location = New System.Drawing.Point(258, 48)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(208, 48)
        Me.Label4.TabIndex = 53
        Me.Label4.Text = "Drop the text onto one of the TextBox controls. Only the TextBox with AllowDrop=T" & _
            "rue will receive the text."
        '
        'Label3
        '
        Me.Label3.AccessibleDescription = "Label with instructional text on dragging text to another TextBox"
        Me.Label3.AccessibleName = "Instructional Text Label for Example 1, drag source"
        Me.Label3.Location = New System.Drawing.Point(10, 47)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(216, 55)
        Me.Label3.TabIndex = 52
        Me.Label3.Text = "Click into the TextBox and drag the text to one of the TextBox controls on the ri" & _
            "ght. This will perform a Move-Drop. Press Ctrl and drag for a Copy-Drop. "
        '
        'txtAllowDrop
        '
        Me.txtAllowDrop.AccessibleDescription = "TextBox with ""AllowDrop = False"""
        Me.txtAllowDrop.AccessibleName = "Drag Target TextBox that doesn't allow drop"
        Me.txtAllowDrop.AllowDrop = True
        Me.txtAllowDrop.Location = New System.Drawing.Point(258, 102)
        Me.txtAllowDrop.Name = "txtAllowDrop"
        Me.txtAllowDrop.Size = New System.Drawing.Size(208, 20)
        Me.txtAllowDrop.TabIndex = 47
        Me.txtAllowDrop.Text = "AllowDrop = True"
        '
        'txtNoDrop
        '
        Me.txtNoDrop.AccessibleDescription = "TextBox with ""AllowDrop = True"""
        Me.txtNoDrop.AccessibleName = "Drag Target TextBox that allows drop"
        Me.txtNoDrop.Location = New System.Drawing.Point(258, 134)
        Me.txtNoDrop.Name = "txtNoDrop"
        Me.txtNoDrop.Size = New System.Drawing.Size(208, 20)
        Me.txtNoDrop.TabIndex = 48
        Me.txtNoDrop.Text = "AllowDrop = False"
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = "Label with text: ""Example 2:..."""
        Me.Label1.AccessibleName = "Example 2 Title Label"
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label1.Location = New System.Drawing.Point(10, 166)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(456, 23)
        Me.Label1.TabIndex = 54
        Me.Label1.Text = "Example 2: Drag-and-Drop Using TreeView Controls"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tvwRight
        '
        Me.tvwRight.AccessibleDescription = "Right TreeView control with various foods listed"
        Me.tvwRight.AccessibleName = "Drag Target TreeView"
        Me.tvwRight.AllowDrop = True
        Me.tvwRight.Location = New System.Drawing.Point(258, 246)
        Me.tvwRight.Name = "tvwRight"
        TreeNode1.Name = ""
        TreeNode1.Text = "Hot Sauce"
        TreeNode2.Name = ""
        TreeNode2.Text = "Cod"
        TreeNode3.Name = ""
        TreeNode3.Text = "Salmon"
        TreeNode4.Name = ""
        TreeNode4.Text = "Catfish"
        TreeNode5.Name = ""
        TreeNode5.Text = "Fish"
        TreeNode6.Name = ""
        TreeNode6.Text = "Crab"
        TreeNode7.Name = ""
        TreeNode7.Text = "Lobster"
        TreeNode8.Name = ""
        TreeNode8.Text = "Seafood"
        TreeNode9.Name = ""
        TreeNode9.Text = "Spaghetti"
        TreeNode10.Name = ""
        TreeNode10.Text = "Fettuccini"
        TreeNode11.Name = ""
        TreeNode11.Text = "Pasta"
        TreeNode12.Name = ""
        TreeNode12.Text = "Garnishes"
        Me.tvwRight.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode1, TreeNode8, TreeNode11, TreeNode12})
        Me.tvwRight.Size = New System.Drawing.Size(208, 136)
        Me.tvwRight.TabIndex = 50
        '
        'Label2
        '
        Me.Label2.AccessibleDescription = "Label with text: ""Example 1:..."""
        Me.Label2.AccessibleName = "Example 1 Title Label"
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label2.Location = New System.Drawing.Point(15, 22)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(456, 23)
        Me.Label2.TabIndex = 51
        Me.Label2.Text = "Example 1: Drag-and-Drop Using TextBox Controls"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tvwLeft
        '
        Me.tvwLeft.AccessibleDescription = "Left TreeView control with various foods listed"
        Me.tvwLeft.AccessibleName = "Drag Source TreeView"
        Me.tvwLeft.AllowDrop = True
        Me.tvwLeft.Location = New System.Drawing.Point(10, 246)
        Me.tvwLeft.Name = "tvwLeft"
        TreeNode13.Name = ""
        TreeNode13.Text = "Bowtie pasta"
        TreeNode14.Name = ""
        TreeNode14.Text = "Calamari"
        TreeNode15.Name = ""
        TreeNode15.Text = "Ketchup"
        TreeNode16.Name = ""
        TreeNode16.Text = "Spicy Brown Mustard"
        TreeNode17.Name = ""
        TreeNode17.Text = "Yellow Mustard"
        TreeNode18.Name = ""
        TreeNode18.Text = "Hot Mustard"
        TreeNode19.Name = ""
        TreeNode19.Text = "Mustard"
        Me.tvwLeft.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode13, TreeNode14, TreeNode15, TreeNode19})
        Me.tvwLeft.Size = New System.Drawing.Size(216, 136)
        Me.tvwLeft.TabIndex = 49
        '
        'txtSource
        '
        Me.txtSource.AccessibleDescription = "TextBox with ""Source Text"""
        Me.txtSource.AccessibleName = "Drag Source TextBox"
        Me.txtSource.AllowDrop = True
        Me.txtSource.Location = New System.Drawing.Point(13, 102)
        Me.txtSource.Name = "txtSource"
        Me.txtSource.Size = New System.Drawing.Size(192, 20)
        Me.txtSource.TabIndex = 46
        Me.txtSource.Text = "Source Text"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(480, 24)
        Me.MenuStrip1.TabIndex = 62
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
        Me.ClientSize = New System.Drawing.Size(480, 549)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtAllowDrop)
        Me.Controls.Add(Me.txtNoDrop)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.tvwRight)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.tvwLeft)
        Me.Controls.Add(Me.txtSource)
        Me.Controls.Add(Me.picRight)
        Me.Controls.Add(Me.picLeft)
        Me.Name = "MainForm"
        Me.Text = "Drag and Drop Sample"
        CType(Me.picRight, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picLeft, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents picRight As System.Windows.Forms.PictureBox
    Friend WithEvents picLeft As System.Windows.Forms.PictureBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtAllowDrop As System.Windows.Forms.TextBox
    Friend WithEvents txtNoDrop As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents tvwRight As System.Windows.Forms.TreeView
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents tvwLeft As System.Windows.Forms.TreeView
    Friend WithEvents txtSource As System.Windows.Forms.TextBox
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
