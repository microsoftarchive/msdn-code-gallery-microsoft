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
        Me.WriteMessageButton = New System.Windows.Forms.Button
        Me.InformationRadioButton = New System.Windows.Forms.RadioButton
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.MessageTextBox = New System.Windows.Forms.TextBox
        Me.CriticalRadioButton = New System.Windows.Forms.RadioButton
        Me.ErrorRadioButton = New System.Windows.Forms.RadioButton
        Me.WarningRadioButton = New System.Windows.Forms.RadioButton
        Me.Label1 = New System.Windows.Forms.Label
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.DataGridView1 = New System.Windows.Forms.DataGridView
        Me.DataGridViewTextBoxColumn3 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.DataGridViewTextBoxColumn1 = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'WriteMessageButton
        '
        Me.WriteMessageButton.Location = New System.Drawing.Point(314, 225)
        Me.WriteMessageButton.Name = "WriteMessageButton"
        Me.WriteMessageButton.Size = New System.Drawing.Size(112, 31)
        Me.WriteMessageButton.TabIndex = 0
        Me.WriteMessageButton.Text = "Write Log Message"
        '
        'InformationRadioButton
        '
        Me.InformationRadioButton.AutoSize = True
        Me.InformationRadioButton.Checked = True
        Me.InformationRadioButton.Location = New System.Drawing.Point(14, 231)
        Me.InformationRadioButton.Name = "InformationRadioButton"
        Me.InformationRadioButton.Size = New System.Drawing.Size(76, 17)
        Me.InformationRadioButton.TabIndex = 1
        Me.InformationRadioButton.Text = " Information"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.MessageTextBox)
        Me.GroupBox2.Controls.Add(Me.CriticalRadioButton)
        Me.GroupBox2.Controls.Add(Me.ErrorRadioButton)
        Me.GroupBox2.Controls.Add(Me.WarningRadioButton)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Controls.Add(Me.InformationRadioButton)
        Me.GroupBox2.Controls.Add(Me.WriteMessageButton)
        Me.GroupBox2.Location = New System.Drawing.Point(28, 253)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(438, 269)
        Me.GroupBox2.TabIndex = 3
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Writing to the  Log"
        '
        'MessageTextBox
        '
        Me.MessageTextBox.Location = New System.Drawing.Point(11, 25)
        Me.MessageTextBox.Multiline = True
        Me.MessageTextBox.Name = "MessageTextBox"
        Me.MessageTextBox.Size = New System.Drawing.Size(415, 176)
        Me.MessageTextBox.TabIndex = 6
        '
        'CriticalRadioButton
        '
        Me.CriticalRadioButton.AutoSize = True
        Me.CriticalRadioButton.Location = New System.Drawing.Point(235, 232)
        Me.CriticalRadioButton.Margin = New System.Windows.Forms.Padding(0, 3, 3, 3)
        Me.CriticalRadioButton.Name = "CriticalRadioButton"
        Me.CriticalRadioButton.Size = New System.Drawing.Size(55, 17)
        Me.CriticalRadioButton.TabIndex = 5
        Me.CriticalRadioButton.Text = " Critical"
        '
        'ErrorRadioButton
        '
        Me.ErrorRadioButton.AutoSize = True
        Me.ErrorRadioButton.Location = New System.Drawing.Point(176, 231)
        Me.ErrorRadioButton.Margin = New System.Windows.Forms.Padding(3, 3, 1, 3)
        Me.ErrorRadioButton.Name = "ErrorRadioButton"
        Me.ErrorRadioButton.Size = New System.Drawing.Size(46, 17)
        Me.ErrorRadioButton.TabIndex = 4
        Me.ErrorRadioButton.Text = " Error"
        '
        'WarningRadioButton
        '
        Me.WarningRadioButton.AutoSize = True
        Me.WarningRadioButton.Location = New System.Drawing.Point(96, 231)
        Me.WarningRadioButton.Name = "WarningRadioButton"
        Me.WarningRadioButton.Size = New System.Drawing.Size(64, 17)
        Me.WarningRadioButton.TabIndex = 3
        Me.WarningRadioButton.Text = " Warning"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(87, 212)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(88, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Message Type"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.DataGridView1)
        Me.GroupBox1.Location = New System.Drawing.Point(28, 52)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(438, 170)
        Me.GroupBox1.TabIndex = 4
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Log Cofiguration"
        '
        'DataGridView1
        '
        Me.DataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None
        Me.DataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        Me.DataGridView1.ColumnHeadersHeight = 21
        Me.DataGridView1.Columns.Add(Me.DataGridViewTextBoxColumn3)
        Me.DataGridView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridView1.Location = New System.Drawing.Point(3, 16)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.RowHeadersVisible = False
        Me.DataGridView1.Size = New System.Drawing.Size(432, 151)
        Me.DataGridView1.TabIndex = 0
        '
        'DataGridViewTextBoxColumn3
        '
        Me.DataGridViewTextBoxColumn3.HeaderText = "Listener Name"
        Me.DataGridViewTextBoxColumn3.Name = "ListenerColumn"
        Me.DataGridViewTextBoxColumn3.ReadOnly = True
        Me.DataGridViewTextBoxColumn3.Width = 429
        '
        'DataGridViewTextBoxColumn1
        '
        Me.DataGridViewTextBoxColumn1.HeaderText = "Listener Name"
        Me.DataGridViewTextBoxColumn1.Name = "ListenerColumn2"
        Me.DataGridViewTextBoxColumn1.ReadOnly = True
        Me.DataGridViewTextBoxColumn1.Width = 300
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(496, 24)
        Me.MenuStrip1.TabIndex = 5
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
        'Form1
        '
        Me.ClientSize = New System.Drawing.Size(496, 546)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.GroupBox2)
        Me.Name = "Form1"
        Me.Text = "My.Application.Log Sample"
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents WriteMessageButton As System.Windows.Forms.Button
    Friend WithEvents InformationRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents WarningRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents ErrorRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents CriticalRadioButton As System.Windows.Forms.RadioButton
    Friend WithEvents MessageTextBox As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents DataGridViewTextBoxColumn1 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents DataGridViewTextBoxColumn3 As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
