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
        Me.components = New System.ComponentModel.Container
        Me.btnRefreshCategories = New System.Windows.Forms.Button
        Me.lblAddingACustomControl = New System.Windows.Forms.Label
        Me.btnDecrementCounter = New System.Windows.Forms.Button
        Me.btnIncrementCounter = New System.Windows.Forms.Button
        Me.txtBuiltInOrCustom = New System.Windows.Forms.TextBox
        Me.lblBuiltInOrCustom = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.txtCounterHelp = New System.Windows.Forms.TextBox
        Me.lblCounterType = New System.Windows.Forms.Label
        Me.txtCounterType = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.tmrUpdateUI = New System.Windows.Forms.Timer(Me.components)
        Me.txtCounterValue = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.cboCategories = New System.Windows.Forms.ComboBox
        Me.lblSelectTimer = New System.Windows.Forms.Label
        Me.cboCounters = New System.Windows.Forms.ComboBox
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.StatusStrip1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnRefreshCategories
        '
        Me.btnRefreshCategories.AccessibleDescription = "Button to refresh Categories."
        Me.btnRefreshCategories.AccessibleName = "Refresh Categories Button"
        Me.btnRefreshCategories.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnRefreshCategories.Location = New System.Drawing.Point(196, 357)
        Me.btnRefreshCategories.Name = "btnRefreshCategories"
        Me.btnRefreshCategories.Size = New System.Drawing.Size(192, 23)
        Me.btnRefreshCategories.TabIndex = 42
        Me.btnRefreshCategories.Text = "&Refresh Categories"
        '
        'lblAddingACustomControl
        '
        Me.lblAddingACustomControl.AccessibleDescription = "Label describing how to build a custom counter"
        Me.lblAddingACustomControl.AccessibleName = "Build a Custom Counter Label"
        Me.lblAddingACustomControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblAddingACustomControl.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblAddingACustomControl.Location = New System.Drawing.Point(28, 213)
        Me.lblAddingACustomControl.Name = "lblAddingACustomControl"
        Me.lblAddingACustomControl.Size = New System.Drawing.Size(360, 136)
        Me.lblAddingACustomControl.TabIndex = 34
        Me.lblAddingACustomControl.Text = "Only Custom counters can be incremented or decremented.  You must first create a " & _
            "Custom Performance Counter manually.  To do this, open the ""Server Explorer"" in " & _
            "Visual Studio .NET, open up Servers -> your computer name -> Performance Counter" & _
            "s.  Right click ""Performance Counters"" and click ""Create New Category"". Create a" & _
            " category and a new counter.  Press ""Refresh"" to reload the categories in this f" & _
            "orm."
        '
        'btnDecrementCounter
        '
        Me.btnDecrementCounter.AccessibleDescription = "Button that will decrement a custom counter."
        Me.btnDecrementCounter.AccessibleName = "Decrement Counter Button"
        Me.btnDecrementCounter.Enabled = False
        Me.btnDecrementCounter.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnDecrementCounter.Location = New System.Drawing.Point(220, 173)
        Me.btnDecrementCounter.Name = "btnDecrementCounter"
        Me.btnDecrementCounter.Size = New System.Drawing.Size(168, 23)
        Me.btnDecrementCounter.TabIndex = 33
        Me.btnDecrementCounter.Text = "&Decrement Counter"
        '
        'btnIncrementCounter
        '
        Me.btnIncrementCounter.AccessibleDescription = "Button that will increment a custom counter."
        Me.btnIncrementCounter.AccessibleName = "Increment Counter Button"
        Me.btnIncrementCounter.Enabled = False
        Me.btnIncrementCounter.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnIncrementCounter.Location = New System.Drawing.Point(28, 173)
        Me.btnIncrementCounter.Name = "btnIncrementCounter"
        Me.btnIncrementCounter.Size = New System.Drawing.Size(168, 23)
        Me.btnIncrementCounter.TabIndex = 32
        Me.btnIncrementCounter.Text = "&Increment  Counter"
        '
        'txtBuiltInOrCustom
        '
        Me.txtBuiltInOrCustom.AccessibleDescription = "TextBox displaying the value for a custom counter"
        Me.txtBuiltInOrCustom.AccessibleName = "CustomCounter TextBox"
        Me.txtBuiltInOrCustom.Enabled = False
        Me.txtBuiltInOrCustom.Location = New System.Drawing.Point(260, 125)
        Me.txtBuiltInOrCustom.Name = "txtBuiltInOrCustom"
        Me.txtBuiltInOrCustom.Size = New System.Drawing.Size(128, 20)
        Me.txtBuiltInOrCustom.TabIndex = 31
        '
        'lblBuiltInOrCustom
        '
        Me.lblBuiltInOrCustom.AccessibleDescription = "Label with text 'Built-In or Custom Counter'"
        Me.lblBuiltInOrCustom.AccessibleName = "Custom Counter Label"
        Me.lblBuiltInOrCustom.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblBuiltInOrCustom.Location = New System.Drawing.Point(20, 125)
        Me.lblBuiltInOrCustom.Name = "lblBuiltInOrCustom"
        Me.lblBuiltInOrCustom.Size = New System.Drawing.Size(232, 23)
        Me.lblBuiltInOrCustom.TabIndex = 30
        Me.lblBuiltInOrCustom.Text = "Built-In or Custom Counter:"
        '
        'Label3
        '
        Me.Label3.AccessibleDescription = "Label with text 'Counter Help'"
        Me.Label3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label3.Location = New System.Drawing.Point(420, 173)
        Me.Label3.Name = "Label3"
        Me.Label3.TabIndex = 40
        Me.Label3.Text = "Counter Help:"
        '
        'txtCounterHelp
        '
        Me.txtCounterHelp.AccessibleDescription = "TextBox displaying the counter help or description."
        Me.txtCounterHelp.AccessibleName = "Counter Help TextBox"
        Me.txtCounterHelp.Location = New System.Drawing.Point(420, 197)
        Me.txtCounterHelp.Multiline = True
        Me.txtCounterHelp.Name = "txtCounterHelp"
        Me.txtCounterHelp.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtCounterHelp.Size = New System.Drawing.Size(160, 176)
        Me.txtCounterHelp.TabIndex = 41
        Me.txtCounterHelp.TabStop = False
        '
        'lblCounterType
        '
        Me.lblCounterType.AccessibleDescription = "Label with text 'Counter Type'"
        Me.lblCounterType.AccessibleName = "Counter Type Label"
        Me.lblCounterType.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblCounterType.Location = New System.Drawing.Point(420, 117)
        Me.lblCounterType.Name = "lblCounterType"
        Me.lblCounterType.TabIndex = 38
        Me.lblCounterType.Text = "Counter Type:"
        '
        'txtCounterType
        '
        Me.txtCounterType.AccessibleDescription = "TextBox displaying the counter type."
        Me.txtCounterType.AccessibleName = "Counter Type TextBox"
        Me.txtCounterType.Location = New System.Drawing.Point(420, 141)
        Me.txtCounterType.Name = "txtCounterType"
        Me.txtCounterType.Size = New System.Drawing.Size(160, 20)
        Me.txtCounterType.TabIndex = 39
        Me.txtCounterType.TabStop = False
        '
        'Label2
        '
        Me.Label2.AccessibleDescription = "Label with text 'Counter Value'"
        Me.Label2.AccessibleName = "Counter Value Label"
        Me.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label2.Location = New System.Drawing.Point(420, 61)
        Me.Label2.Name = "Label2"
        Me.Label2.TabIndex = 35
        Me.Label2.Text = "Counter Value:"
        '
        'tmrUpdateUI
        '
        '
        'txtCounterValue
        '
        Me.txtCounterValue.AccessibleDescription = "TextBox displaying the counter value."
        Me.txtCounterValue.AccessibleName = "Counter Value TextBox"
        Me.txtCounterValue.Location = New System.Drawing.Point(420, 85)
        Me.txtCounterValue.Name = "txtCounterValue"
        Me.txtCounterValue.Size = New System.Drawing.Size(160, 20)
        Me.txtCounterValue.TabIndex = 37
        Me.txtCounterValue.TabStop = False
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = "Label with text 'Select Category'"
        Me.Label1.AccessibleName = "Select Category Label"
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(20, 53)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(120, 23)
        Me.Label1.TabIndex = 26
        Me.Label1.Text = "Select Category:"
        '
        'cboCategories
        '
        Me.cboCategories.AccessibleDescription = "Combo Box allowing user to select a category."
        Me.cboCategories.AccessibleName = "Select Category ComboBox"
        Me.cboCategories.FormattingEnabled = True
        Me.cboCategories.ItemHeight = 13
        Me.cboCategories.Location = New System.Drawing.Point(148, 53)
        Me.cboCategories.Name = "cboCategories"
        Me.cboCategories.Size = New System.Drawing.Size(240, 21)
        Me.cboCategories.TabIndex = 27
        '
        'lblSelectTimer
        '
        Me.lblSelectTimer.AccessibleDescription = "Label with text 'Select Counter'"
        Me.lblSelectTimer.AccessibleName = "Select Counter Label"
        Me.lblSelectTimer.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblSelectTimer.Location = New System.Drawing.Point(20, 85)
        Me.lblSelectTimer.Name = "lblSelectTimer"
        Me.lblSelectTimer.Size = New System.Drawing.Size(120, 23)
        Me.lblSelectTimer.TabIndex = 28
        Me.lblSelectTimer.Text = "Select Counter:"
        '
        'cboCounters
        '
        Me.cboCounters.AccessibleDescription = "Combo Box allowing user to select a counter."
        Me.cboCounters.AccessibleName = "Select Counter ComboBox"
        Me.cboCounters.FormattingEnabled = True
        Me.cboCounters.ItemHeight = 13
        Me.cboCounters.Location = New System.Drawing.Point(148, 85)
        Me.cboCounters.Name = "cboCounters"
        Me.cboCounters.Size = New System.Drawing.Size(240, 21)
        Me.cboCounters.TabIndex = 29
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1})
        Me.StatusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 441)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.TabIndex = 43
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(6, 2, 0, 2)
        Me.MenuStrip1.TabIndex = 44
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
        Me.ClientSize = New System.Drawing.Size(654, 463)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.btnDecrementCounter)
        Me.Controls.Add(Me.btnIncrementCounter)
        Me.Controls.Add(Me.txtBuiltInOrCustom)
        Me.Controls.Add(Me.lblBuiltInOrCustom)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtCounterHelp)
        Me.Controls.Add(Me.lblCounterType)
        Me.Controls.Add(Me.txtCounterType)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtCounterValue)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cboCategories)
        Me.Controls.Add(Me.lblSelectTimer)
        Me.Controls.Add(Me.cboCounters)
        Me.Controls.Add(Me.btnRefreshCategories)
        Me.Controls.Add(Me.lblAddingACustomControl)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.StatusStrip1.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnRefreshCategories As System.Windows.Forms.Button
    Friend WithEvents lblAddingACustomControl As System.Windows.Forms.Label
    Friend WithEvents btnDecrementCounter As System.Windows.Forms.Button
    Friend WithEvents btnIncrementCounter As System.Windows.Forms.Button
    Friend WithEvents txtBuiltInOrCustom As System.Windows.Forms.TextBox
    Friend WithEvents lblBuiltInOrCustom As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtCounterHelp As System.Windows.Forms.TextBox
    Friend WithEvents lblCounterType As System.Windows.Forms.Label
    Friend WithEvents txtCounterType As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents tmrUpdateUI As System.Windows.Forms.Timer
    Friend WithEvents txtCounterValue As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cboCategories As System.Windows.Forms.ComboBox
    Friend WithEvents lblSelectTimer As System.Windows.Forms.Label
    Friend WithEvents cboCounters As System.Windows.Forms.ComboBox
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
End Class
