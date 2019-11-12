Public Partial Class TaskManager
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
        Me.btnModules = New System.Windows.Forms.Button
        Me.txtTotalProcessorTime = New System.Windows.Forms.TextBox
        Me.txtStartTime = New System.Windows.Forms.TextBox
        Me.txtMinWorkingSet = New System.Windows.Forms.TextBox
        Me.txtMaxWorkingSet = New System.Windows.Forms.TextBox
        Me.txtNumberOfThreads = New System.Windows.Forms.TextBox
        Me.txtPriority = New System.Windows.Forms.TextBox
        Me.label7 = New System.Windows.Forms.Label
        Me.label6 = New System.Windows.Forms.Label
        Me.label5 = New System.Windows.Forms.Label
        Me.label4 = New System.Windows.Forms.Label
        Me.label3 = New System.Windows.Forms.Label
        Me.label2 = New System.Windows.Forms.Label
        Me.label1 = New System.Windows.Forms.Label
        Me.cboCurrentProcesses = New System.Windows.Forms.ComboBox
        Me.SuspendLayout()
        '
        'btnModules
        '
        Me.btnModules.Location = New System.Drawing.Point(220, 276)
        Me.btnModules.Name = "btnModules"
        Me.btnModules.TabIndex = 29
        Me.btnModules.Text = "Modules"
        '
        'txtTotalProcessorTime
        '
        Me.txtTotalProcessorTime.Location = New System.Drawing.Point(116, 228)
        Me.txtTotalProcessorTime.Name = "txtTotalProcessorTime"
        Me.txtTotalProcessorTime.Size = New System.Drawing.Size(184, 20)
        Me.txtTotalProcessorTime.TabIndex = 28
        Me.txtTotalProcessorTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtStartTime
        '
        Me.txtStartTime.Location = New System.Drawing.Point(116, 196)
        Me.txtStartTime.Name = "txtStartTime"
        Me.txtStartTime.Size = New System.Drawing.Size(184, 20)
        Me.txtStartTime.TabIndex = 26
        Me.txtStartTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtMinWorkingSet
        '
        Me.txtMinWorkingSet.Location = New System.Drawing.Point(116, 164)
        Me.txtMinWorkingSet.Name = "txtMinWorkingSet"
        Me.txtMinWorkingSet.Size = New System.Drawing.Size(184, 20)
        Me.txtMinWorkingSet.TabIndex = 24
        Me.txtMinWorkingSet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtMaxWorkingSet
        '
        Me.txtMaxWorkingSet.Location = New System.Drawing.Point(116, 132)
        Me.txtMaxWorkingSet.Name = "txtMaxWorkingSet"
        Me.txtMaxWorkingSet.Size = New System.Drawing.Size(184, 20)
        Me.txtMaxWorkingSet.TabIndex = 22
        Me.txtMaxWorkingSet.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtNumberOfThreads
        '
        Me.txtNumberOfThreads.Location = New System.Drawing.Point(116, 100)
        Me.txtNumberOfThreads.Name = "txtNumberOfThreads"
        Me.txtNumberOfThreads.Size = New System.Drawing.Size(184, 20)
        Me.txtNumberOfThreads.TabIndex = 20
        Me.txtNumberOfThreads.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'txtPriority
        '
        Me.txtPriority.Location = New System.Drawing.Point(116, 68)
        Me.txtPriority.Name = "txtPriority"
        Me.txtPriority.Size = New System.Drawing.Size(184, 20)
        Me.txtPriority.TabIndex = 18
        Me.txtPriority.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'label7
        '
        Me.label7.Location = New System.Drawing.Point(4, 228)
        Me.label7.Name = "label7"
        Me.label7.Size = New System.Drawing.Size(104, 32)
        Me.label7.TabIndex = 27
        Me.label7.Text = "Total Processor Time"
        '
        'label6
        '
        Me.label6.Location = New System.Drawing.Point(4, 196)
        Me.label6.Name = "label6"
        Me.label6.TabIndex = 25
        Me.label6.Text = "Start Time"
        '
        'label5
        '
        Me.label5.Location = New System.Drawing.Point(4, 164)
        Me.label5.Name = "label5"
        Me.label5.TabIndex = 23
        Me.label5.Text = "Min Working Set"
        '
        'label4
        '
        Me.label4.Location = New System.Drawing.Point(4, 132)
        Me.label4.Name = "label4"
        Me.label4.TabIndex = 21
        Me.label4.Text = "Max Working Set"
        '
        'label3
        '
        Me.label3.Location = New System.Drawing.Point(4, 100)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(104, 23)
        Me.label3.TabIndex = 19
        Me.label3.Text = "Number of Threads"
        '
        'label2
        '
        Me.label2.Location = New System.Drawing.Point(4, 68)
        Me.label2.Name = "label2"
        Me.label2.TabIndex = 17
        Me.label2.Text = "Priority"
        '
        'label1
        '
        Me.label1.Location = New System.Drawing.Point(4, 20)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(104, 23)
        Me.label1.TabIndex = 15
        Me.label1.Text = "Current Processes:"
        '
        'cboCurrentProcesses
        '
        Me.cboCurrentProcesses.Location = New System.Drawing.Point(116, 20)
        Me.cboCurrentProcesses.Name = "cboCurrentProcesses"
        Me.cboCurrentProcesses.Size = New System.Drawing.Size(192, 21)
        Me.cboCurrentProcesses.TabIndex = 16
        '
        'TaskManager
        '
        Me.ClientSize = New System.Drawing.Size(312, 318)
        Me.Controls.Add(Me.btnModules)
        Me.Controls.Add(Me.txtTotalProcessorTime)
        Me.Controls.Add(Me.txtStartTime)
        Me.Controls.Add(Me.txtMinWorkingSet)
        Me.Controls.Add(Me.txtMaxWorkingSet)
        Me.Controls.Add(Me.txtNumberOfThreads)
        Me.Controls.Add(Me.txtPriority)
        Me.Controls.Add(Me.label7)
        Me.Controls.Add(Me.label6)
        Me.Controls.Add(Me.label5)
        Me.Controls.Add(Me.label4)
        Me.Controls.Add(Me.label3)
        Me.Controls.Add(Me.label2)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.cboCurrentProcesses)
        Me.Name = "TaskManager"
        Me.Text = "TaskManager"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnModules As System.Windows.Forms.Button
    Friend WithEvents txtTotalProcessorTime As System.Windows.Forms.TextBox
    Friend WithEvents txtStartTime As System.Windows.Forms.TextBox
    Friend WithEvents txtMinWorkingSet As System.Windows.Forms.TextBox
    Friend WithEvents txtMaxWorkingSet As System.Windows.Forms.TextBox
    Friend WithEvents txtNumberOfThreads As System.Windows.Forms.TextBox
    Friend WithEvents txtPriority As System.Windows.Forms.TextBox
    Friend WithEvents label7 As System.Windows.Forms.Label
    Friend WithEvents label6 As System.Windows.Forms.Label
    Friend WithEvents label5 As System.Windows.Forms.Label
    Friend WithEvents label4 As System.Windows.Forms.Label
    Friend WithEvents label3 As System.Windows.Forms.Label
    Friend WithEvents label2 As System.Windows.Forms.Label
    Friend WithEvents label1 As System.Windows.Forms.Label
    Friend WithEvents cboCurrentProcesses As System.Windows.Forms.ComboBox
End Class
