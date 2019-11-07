Partial Public Class Form1
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
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
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.lblStep6 = New System.Windows.Forms.Label
        Me.btnDisplay = New System.Windows.Forms.Button
        Me.lblArrow5 = New System.Windows.Forms.Label
        Me.lblStep5 = New System.Windows.Forms.Label
        Me.btnPopulate = New System.Windows.Forms.Button
        Me.lblArrow4 = New System.Windows.Forms.Label
        Me.lblStep4 = New System.Windows.Forms.Label
        Me.btnCreateView = New System.Windows.Forms.Button
        Me.lblArrow3 = New System.Windows.Forms.Label
        Me.lblStep3 = New System.Windows.Forms.Label
        Me.btnCreateSP = New System.Windows.Forms.Button
        Me.lblArrow2 = New System.Windows.Forms.Label
        Me.lblStep2 = New System.Windows.Forms.Label
        Me.btnCreateTable = New System.Windows.Forms.Button
        Me.lblArrow1 = New System.Windows.Forms.Label
        Me.lblStep1 = New System.Windows.Forms.Label
        Me.btnCreateDB = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.DataGridView1 = New System.Windows.Forms.DataGridView
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblStep6
        '
        Me.lblStep6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblStep6.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblStep6.Location = New System.Drawing.Point(350, 143)
        Me.lblStep6.Name = "lblStep6"
        Me.lblStep6.Size = New System.Drawing.Size(16, 23)
        Me.lblStep6.TabIndex = 55
        Me.lblStep6.Text = "6."
        '
        'btnDisplay
        '
        Me.btnDisplay.BackColor = System.Drawing.SystemColors.InactiveCaptionText
        Me.btnDisplay.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnDisplay.Location = New System.Drawing.Point(366, 135)
        Me.btnDisplay.Name = "btnDisplay"
        Me.btnDisplay.Size = New System.Drawing.Size(104, 23)
        Me.btnDisplay.TabIndex = 54
        Me.btnDisplay.Text = "D&isplay Data"
        Me.btnDisplay.UseVisualStyleBackColor = False
        '
        'lblArrow5
        '
        Me.lblArrow5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblArrow5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblArrow5.Location = New System.Drawing.Point(320, 140)
        Me.lblArrow5.Name = "lblArrow5"
        Me.lblArrow5.Size = New System.Drawing.Size(31, 23)
        Me.lblArrow5.TabIndex = 53
        Me.lblArrow5.Text = ">>"
        '
        'lblStep5
        '
        Me.lblStep5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblStep5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblStep5.Location = New System.Drawing.Point(190, 143)
        Me.lblStep5.Margin = New System.Windows.Forms.Padding(1, 3, 2, 3)
        Me.lblStep5.Name = "lblStep5"
        Me.lblStep5.Size = New System.Drawing.Size(16, 23)
        Me.lblStep5.TabIndex = 52
        Me.lblStep5.Text = "5."
        '
        'btnPopulate
        '
        Me.btnPopulate.BackColor = System.Drawing.SystemColors.InactiveCaptionText
        Me.btnPopulate.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnPopulate.Location = New System.Drawing.Point(210, 135)
        Me.btnPopulate.Margin = New System.Windows.Forms.Padding(2, 3, 3, 3)
        Me.btnPopulate.Name = "btnPopulate"
        Me.btnPopulate.Size = New System.Drawing.Size(104, 23)
        Me.btnPopulate.TabIndex = 51
        Me.btnPopulate.Text = "&Populate Table"
        Me.btnPopulate.UseVisualStyleBackColor = False
        '
        'lblArrow4
        '
        Me.lblArrow4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblArrow4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblArrow4.Location = New System.Drawing.Point(157, 140)
        Me.lblArrow4.Margin = New System.Windows.Forms.Padding(3, 3, 1, 3)
        Me.lblArrow4.Name = "lblArrow4"
        Me.lblArrow4.Size = New System.Drawing.Size(31, 23)
        Me.lblArrow4.TabIndex = 50
        Me.lblArrow4.Text = ">>"
        '
        'lblStep4
        '
        Me.lblStep4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblStep4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblStep4.Location = New System.Drawing.Point(26, 143)
        Me.lblStep4.Name = "lblStep4"
        Me.lblStep4.Size = New System.Drawing.Size(16, 23)
        Me.lblStep4.TabIndex = 49
        Me.lblStep4.Text = "4."
        '
        'btnCreateView
        '
        Me.btnCreateView.BackColor = System.Drawing.SystemColors.InactiveCaptionText
        Me.btnCreateView.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnCreateView.Location = New System.Drawing.Point(46, 136)
        Me.btnCreateView.Name = "btnCreateView"
        Me.btnCreateView.Size = New System.Drawing.Size(104, 23)
        Me.btnCreateView.TabIndex = 48
        Me.btnCreateView.Text = "Create &View"
        Me.btnCreateView.UseVisualStyleBackColor = False
        '
        'lblArrow3
        '
        Me.lblArrow3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblArrow3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblArrow3.Location = New System.Drawing.Point(477, 100)
        Me.lblArrow3.Name = "lblArrow3"
        Me.lblArrow3.Size = New System.Drawing.Size(31, 23)
        Me.lblArrow3.TabIndex = 47
        Me.lblArrow3.Text = ">>"
        '
        'lblStep3
        '
        Me.lblStep3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblStep3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblStep3.Location = New System.Drawing.Point(350, 104)
        Me.lblStep3.Name = "lblStep3"
        Me.lblStep3.Size = New System.Drawing.Size(16, 23)
        Me.lblStep3.TabIndex = 46
        Me.lblStep3.Text = "3."
        '
        'btnCreateSP
        '
        Me.btnCreateSP.BackColor = System.Drawing.SystemColors.InactiveCaptionText
        Me.btnCreateSP.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnCreateSP.Location = New System.Drawing.Point(366, 96)
        Me.btnCreateSP.Name = "btnCreateSP"
        Me.btnCreateSP.Size = New System.Drawing.Size(104, 23)
        Me.btnCreateSP.TabIndex = 45
        Me.btnCreateSP.Text = "Create &SPROC"
        Me.btnCreateSP.UseVisualStyleBackColor = False
        '
        'lblArrow2
        '
        Me.lblArrow2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblArrow2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblArrow2.Location = New System.Drawing.Point(320, 101)
        Me.lblArrow2.Name = "lblArrow2"
        Me.lblArrow2.Size = New System.Drawing.Size(31, 23)
        Me.lblArrow2.TabIndex = 44
        Me.lblArrow2.Text = ">>"
        '
        'lblStep2
        '
        Me.lblStep2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblStep2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblStep2.Location = New System.Drawing.Point(189, 104)
        Me.lblStep2.Margin = New System.Windows.Forms.Padding(0, 3, 2, 3)
        Me.lblStep2.Name = "lblStep2"
        Me.lblStep2.Size = New System.Drawing.Size(16, 23)
        Me.lblStep2.TabIndex = 43
        Me.lblStep2.Text = "2."
        '
        'btnCreateTable
        '
        Me.btnCreateTable.BackColor = System.Drawing.SystemColors.InactiveCaptionText
        Me.btnCreateTable.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnCreateTable.Location = New System.Drawing.Point(209, 96)
        Me.btnCreateTable.Margin = New System.Windows.Forms.Padding(2, 3, 3, 3)
        Me.btnCreateTable.Name = "btnCreateTable"
        Me.btnCreateTable.Size = New System.Drawing.Size(104, 23)
        Me.btnCreateTable.TabIndex = 42
        Me.btnCreateTable.Text = "Create &Table"
        Me.btnCreateTable.UseVisualStyleBackColor = False
        '
        'lblArrow1
        '
        Me.lblArrow1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblArrow1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblArrow1.Location = New System.Drawing.Point(157, 100)
        Me.lblArrow1.Margin = New System.Windows.Forms.Padding(3, 3, 1, 3)
        Me.lblArrow1.Name = "lblArrow1"
        Me.lblArrow1.Size = New System.Drawing.Size(31, 23)
        Me.lblArrow1.TabIndex = 41
        Me.lblArrow1.Text = ">>"
        '
        'lblStep1
        '
        Me.lblStep1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblStep1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblStep1.Location = New System.Drawing.Point(26, 103)
        Me.lblStep1.Name = "lblStep1"
        Me.lblStep1.Size = New System.Drawing.Size(16, 23)
        Me.lblStep1.TabIndex = 40
        Me.lblStep1.Text = "1."
        '
        'btnCreateDB
        '
        Me.btnCreateDB.BackColor = System.Drawing.SystemColors.InactiveCaptionText
        Me.btnCreateDB.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnCreateDB.Location = New System.Drawing.Point(46, 96)
        Me.btnCreateDB.Name = "btnCreateDB"
        Me.btnCreateDB.Size = New System.Drawing.Size(104, 23)
        Me.btnCreateDB.TabIndex = 39
        Me.btnCreateDB.Text = "Create &Database"
        Me.btnCreateDB.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.SystemColors.Desktop
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(22, 64)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(136, 23)
        Me.Label1.TabIndex = 38
        Me.Label1.Text = "Database Demo"
        '
        'DataGridView1
        '
        Me.DataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None
        Me.DataGridView1.Location = New System.Drawing.Point(21, 201)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(470, 188)
        Me.DataGridView1.TabIndex = 57
        Me.DataGridView1.Visible = False
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(529, 24)
        Me.MenuStrip1.TabIndex = 58
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
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(529, 410)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.lblStep6)
        Me.Controls.Add(Me.btnDisplay)
        Me.Controls.Add(Me.lblArrow5)
        Me.Controls.Add(Me.lblStep5)
        Me.Controls.Add(Me.btnPopulate)
        Me.Controls.Add(Me.lblArrow4)
        Me.Controls.Add(Me.lblStep4)
        Me.Controls.Add(Me.btnCreateView)
        Me.Controls.Add(Me.lblArrow3)
        Me.Controls.Add(Me.lblStep3)
        Me.Controls.Add(Me.btnCreateSP)
        Me.Controls.Add(Me.lblArrow2)
        Me.Controls.Add(Me.lblStep2)
        Me.Controls.Add(Me.btnCreateTable)
        Me.Controls.Add(Me.lblArrow1)
        Me.Controls.Add(Me.lblStep1)
        Me.Controls.Add(Me.btnCreateDB)
        Me.Controls.Add(Me.Label1)
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend Shared ReadOnly Property GetInstance() As Form1
        Get
            If m_DefaultInstance Is Nothing OrElse m_DefaultInstance.IsDisposed() Then
                SyncLock m_SyncObject
                    If m_DefaultInstance Is Nothing OrElse m_DefaultInstance.IsDisposed() Then
                        m_DefaultInstance = New Form1
                    End If
                End SyncLock
            End If
            Return m_DefaultInstance
        End Get
    End Property

    Private Shared m_DefaultInstance As Form1
    Private Shared m_SyncObject As New Object
    Friend WithEvents lblStep6 As System.Windows.Forms.Label
    Friend WithEvents btnDisplay As System.Windows.Forms.Button
    Friend WithEvents lblArrow5 As System.Windows.Forms.Label
    Friend WithEvents lblStep5 As System.Windows.Forms.Label
    Friend WithEvents btnPopulate As System.Windows.Forms.Button
    Friend WithEvents lblArrow4 As System.Windows.Forms.Label
    Friend WithEvents lblStep4 As System.Windows.Forms.Label
    Friend WithEvents btnCreateView As System.Windows.Forms.Button
    Friend WithEvents lblArrow3 As System.Windows.Forms.Label
    Friend WithEvents lblStep3 As System.Windows.Forms.Label
    Friend WithEvents btnCreateSP As System.Windows.Forms.Button
    Friend WithEvents lblArrow2 As System.Windows.Forms.Label
    Friend WithEvents lblStep2 As System.Windows.Forms.Label
    Friend WithEvents btnCreateTable As System.Windows.Forms.Button
    Friend WithEvents lblArrow1 As System.Windows.Forms.Label
    Friend WithEvents lblStep1 As System.Windows.Forms.Label
    Friend WithEvents btnCreateDB As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
