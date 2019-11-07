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
        Dim TreeNode1 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Copy a Directory")
        Dim TreeNode2 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Delete a Directory")
        Dim TreeNode3 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Move a Directory")
        Dim TreeNode4 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("View Directory Properties")
        Dim TreeNode5 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("View Drive Properties")
        Dim TreeNode6 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Directory Actions", New System.Windows.Forms.TreeNode() {TreeNode1, TreeNode2, TreeNode3, TreeNode4, TreeNode5})
        Dim TreeNode7 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Copy a File")
        Dim TreeNode8 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Delete a File")
        Dim TreeNode9 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Move a File")
        Dim TreeNode10 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Read a Text File")
        Dim TreeNode11 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Read a Large Text File")
        Dim TreeNode12 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Write a Text File")
        Dim TreeNode13 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Write a Large Text File")
        Dim TreeNode14 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Parse a File")
        Dim TreeNode15 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Find Files")
        Dim TreeNode16 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Search File Contents For Specific Text")
        Dim TreeNode17 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("View File Properties")
        Dim TreeNode18 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("File Actions", New System.Windows.Forms.TreeNode() {TreeNode7, TreeNode8, TreeNode9, TreeNode10, TreeNode11, TreeNode12, TreeNode13, TreeNode14, TreeNode15, TreeNode16, TreeNode17})
        Me.SplitContainer = New System.Windows.Forms.SplitContainer
        Me.TreeView1 = New System.Windows.Forms.TreeView
        Me.SplitContainer.Panel1.SuspendLayout()
        Me.SplitContainer.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer
        '
        Me.SplitContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer.Name = "SplitContainer"
        '
        'SplitContainer.Panel1
        '
        Me.SplitContainer.Panel1.Controls.Add(Me.TreeView1)
        '
        'SplitContainer.Panel2
        '
        Me.SplitContainer.Panel2.AutoScroll = True
        Me.SplitContainer.Size = New System.Drawing.Size(1104, 658)
        Me.SplitContainer.SplitterDistance = 370
        Me.SplitContainer.TabIndex = 0
        '
        'TreeView1
        '
        Me.TreeView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TreeView1.Location = New System.Drawing.Point(0, 0)
        Me.TreeView1.Name = "TreeView1"
        TreeNode1.Name = "Node4"
        TreeNode1.Text = "Copy a Directory"
        TreeNode2.Name = "Node5"
        TreeNode2.Text = "Delete a Directory"
        TreeNode3.Name = "Node6"
        TreeNode3.Text = "Move a Directory"
        TreeNode4.Name = "Node15"
        TreeNode4.Text = "View Directory Properties"
        TreeNode5.Name = "Node16"
        TreeNode5.Text = "View Drive Properties"
        TreeNode6.Name = "Node1"
        TreeNode6.Text = "Directory Actions"
        TreeNode7.Name = "Node3"
        TreeNode7.Text = "Copy a File"
        TreeNode8.Name = "Node4"
        TreeNode8.Text = "Delete a File"
        TreeNode9.Name = "Node5"
        TreeNode9.Text = "Move a File"
        TreeNode10.Name = "Node7"
        TreeNode10.Text = "Read a Text File"
        TreeNode11.Name = "Node8"
        TreeNode11.Text = "Read a Large Text File"
        TreeNode12.Name = "Node10"
        TreeNode12.Text = "Write a Text File"
        TreeNode13.Name = "Node9"
        TreeNode13.Text = "Write a Large Text File"
        TreeNode14.Name = "Node11"
        TreeNode14.Text = "Parse a File"
        TreeNode15.Name = "Node12"
        TreeNode15.Text = "Find Files"
        TreeNode16.Name = "Node13"
        TreeNode16.Text = "Search File Contents For Specific Text"
        TreeNode17.Name = "Node14"
        TreeNode17.Text = "View File Properties"
        TreeNode18.Name = "Node2"
        TreeNode18.Text = "File Actions"
        Me.TreeView1.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode6, TreeNode18})
        Me.TreeView1.Size = New System.Drawing.Size(370, 658)
        Me.TreeView1.TabIndex = 0
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1104, 658)
        Me.Controls.Add(Me.SplitContainer)
        Me.Name = "MainForm"
        Me.Text = "MainForm"
        Me.SplitContainer.Panel1.ResumeLayout(False)
        Me.SplitContainer.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer As System.Windows.Forms.SplitContainer
    Friend WithEvents TreeView1 As System.Windows.Forms.TreeView
End Class
