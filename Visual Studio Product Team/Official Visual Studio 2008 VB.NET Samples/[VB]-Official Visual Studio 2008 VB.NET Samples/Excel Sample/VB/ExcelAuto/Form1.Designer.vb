<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Me.cmdEmailOutlook = New System.Windows.Forms.Button
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.udEvery = New System.Windows.Forms.NumericUpDown
        Me.datStart = New System.Windows.Forms.DateTimePicker
        Me.Label2 = New System.Windows.Forms.Label
        Me.cboRecurrence = New System.Windows.Forms.ComboBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.cmdLoadSpreadsheet = New System.Windows.Forms.Button
        Me.grdOrder = New System.Windows.Forms.DataGridView
        Me.cmdSchedule = New System.Windows.Forms.Button
        Me.lblEmail = New System.Windows.Forms.Label
        Me.txtEmailAddress = New System.Windows.Forms.TextBox
        Me.btnRemoveRecurrence = New System.Windows.Forms.Button
        Me.lblSMTP = New System.Windows.Forms.Label
        Me.txtSMTPHost = New System.Windows.Forms.TextBox
        Me.cmdEmailSMTP = New System.Windows.Forms.Button
        Me.GroupBox1.SuspendLayout()
        CType(Me.udEvery, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.grdOrder, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'cmdEmailOutlook
        '
        Me.cmdEmailOutlook.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdEmailOutlook.Location = New System.Drawing.Point(12, 210)
        Me.cmdEmailOutlook.Name = "cmdEmailOutlook"
        Me.cmdEmailOutlook.Size = New System.Drawing.Size(203, 33)
        Me.cmdEmailOutlook.TabIndex = 2
        Me.cmdEmailOutlook.Text = "Email Spreadsheet via Outlook"
        Me.cmdEmailOutlook.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.udEvery)
        Me.GroupBox1.Controls.Add(Me.datStart)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.cboRecurrence)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Location = New System.Drawing.Point(243, 310)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(298, 100)
        Me.GroupBox1.TabIndex = 6
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Recurrence Pattern"
        '
        'udEvery
        '
        Me.udEvery.Location = New System.Drawing.Point(59, 28)
        Me.udEvery.Maximum = New Decimal(New Integer() {365, 0, 0, 0})
        Me.udEvery.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.udEvery.Name = "udEvery"
        Me.udEvery.Size = New System.Drawing.Size(47, 20)
        Me.udEvery.TabIndex = 1
        Me.udEvery.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'datStart
        '
        Me.datStart.CustomFormat = "dddd d-MMM-yyyy hh:mm tt"
        Me.datStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.datStart.Location = New System.Drawing.Point(61, 65)
        Me.datStart.MaxDate = New Date(2100, 12, 31, 0, 0, 0, 0)
        Me.datStart.MinDate = New Date(2007, 4, 14, 0, 0, 0, 0)
        Me.datStart.Name = "datStart"
        Me.datStart.Size = New System.Drawing.Size(229, 20)
        Me.datStart.TabIndex = 4
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 67)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(43, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Starting"
        '
        'cboRecurrence
        '
        Me.cboRecurrence.FormattingEnabled = True
        Me.cboRecurrence.Items.AddRange(New Object() {"Minutes", "Hours", "Days"})
        Me.cboRecurrence.Location = New System.Drawing.Point(112, 28)
        Me.cboRecurrence.Name = "cboRecurrence"
        Me.cboRecurrence.Size = New System.Drawing.Size(120, 21)
        Me.cboRecurrence.TabIndex = 2
        Me.cboRecurrence.Text = "Days"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(11, 30)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(34, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Every"
        '
        'cmdLoadSpreadsheet
        '
        Me.cmdLoadSpreadsheet.Location = New System.Drawing.Point(12, 7)
        Me.cmdLoadSpreadsheet.Name = "cmdLoadSpreadsheet"
        Me.cmdLoadSpreadsheet.Size = New System.Drawing.Size(203, 36)
        Me.cmdLoadSpreadsheet.TabIndex = 0
        Me.cmdLoadSpreadsheet.Text = "Load Spreadsheet with Data"
        Me.cmdLoadSpreadsheet.UseVisualStyleBackColor = True
        '
        'grdOrder
        '
        Me.grdOrder.AllowUserToAddRows = False
        Me.grdOrder.AllowUserToDeleteRows = False
        Me.grdOrder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.grdOrder.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.grdOrder.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.grdOrder.DefaultCellStyle = DataGridViewCellStyle2
        Me.grdOrder.Location = New System.Drawing.Point(12, 51)
        Me.grdOrder.Name = "grdOrder"
        Me.grdOrder.ReadOnly = True
        Me.grdOrder.Size = New System.Drawing.Size(529, 136)
        Me.grdOrder.TabIndex = 1
        '
        'cmdSchedule
        '
        Me.cmdSchedule.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdSchedule.Location = New System.Drawing.Point(12, 312)
        Me.cmdSchedule.Name = "cmdSchedule"
        Me.cmdSchedule.Size = New System.Drawing.Size(203, 33)
        Me.cmdSchedule.TabIndex = 5
        Me.cmdSchedule.Text = "Schedule Recurring Email via SMTP"
        Me.cmdSchedule.UseVisualStyleBackColor = True
        '
        'lblEmail
        '
        Me.lblEmail.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblEmail.AutoSize = True
        Me.lblEmail.Location = New System.Drawing.Point(240, 213)
        Me.lblEmail.Name = "lblEmail"
        Me.lblEmail.Size = New System.Drawing.Size(76, 13)
        Me.lblEmail.TabIndex = 3
        Me.lblEmail.Text = "Email Address:"
        '
        'txtEmailAddress
        '
        Me.txtEmailAddress.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtEmailAddress.Location = New System.Drawing.Point(322, 210)
        Me.txtEmailAddress.Name = "txtEmailAddress"
        Me.txtEmailAddress.Size = New System.Drawing.Size(219, 20)
        Me.txtEmailAddress.TabIndex = 4
        '
        'btnRemoveRecurrence
        '
        Me.btnRemoveRecurrence.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnRemoveRecurrence.Location = New System.Drawing.Point(415, 417)
        Me.btnRemoveRecurrence.Name = "btnRemoveRecurrence"
        Me.btnRemoveRecurrence.Size = New System.Drawing.Size(126, 36)
        Me.btnRemoveRecurrence.TabIndex = 8
        Me.btnRemoveRecurrence.Text = "Remove Recurrence"
        Me.btnRemoveRecurrence.UseVisualStyleBackColor = True
        '
        'lblSMTP
        '
        Me.lblSMTP.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblSMTP.AutoSize = True
        Me.lblSMTP.Location = New System.Drawing.Point(240, 240)
        Me.lblSMTP.Name = "lblSMTP"
        Me.lblSMTP.Size = New System.Drawing.Size(65, 13)
        Me.lblSMTP.TabIndex = 9
        Me.lblSMTP.Text = "SMTP Host:"
        '
        'txtSMTPHost
        '
        Me.txtSMTPHost.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSMTPHost.Location = New System.Drawing.Point(323, 236)
        Me.txtSMTPHost.Name = "txtSMTPHost"
        Me.txtSMTPHost.Size = New System.Drawing.Size(219, 20)
        Me.txtSMTPHost.TabIndex = 10
        '
        'cmdEmailSMTP
        '
        Me.cmdEmailSMTP.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdEmailSMTP.Location = New System.Drawing.Point(12, 249)
        Me.cmdEmailSMTP.Name = "cmdEmailSMTP"
        Me.cmdEmailSMTP.Size = New System.Drawing.Size(203, 33)
        Me.cmdEmailSMTP.TabIndex = 11
        Me.cmdEmailSMTP.Text = "Email Spreadsheet via SMTP"
        Me.cmdEmailSMTP.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(553, 462)
        Me.Controls.Add(Me.cmdEmailSMTP)
        Me.Controls.Add(Me.txtSMTPHost)
        Me.Controls.Add(Me.lblSMTP)
        Me.Controls.Add(Me.btnRemoveRecurrence)
        Me.Controls.Add(Me.txtEmailAddress)
        Me.Controls.Add(Me.lblEmail)
        Me.Controls.Add(Me.cmdSchedule)
        Me.Controls.Add(Me.grdOrder)
        Me.Controls.Add(Me.cmdLoadSpreadsheet)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.cmdEmailOutlook)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.udEvery, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.grdOrder, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cmdEmailOutlook As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents cboRecurrence As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents datStart As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents udEvery As System.Windows.Forms.NumericUpDown
    Friend WithEvents cmdLoadSpreadsheet As System.Windows.Forms.Button
    Friend WithEvents grdOrder As System.Windows.Forms.DataGridView
    Friend WithEvents cmdSchedule As System.Windows.Forms.Button
    Friend WithEvents lblEmail As System.Windows.Forms.Label
    Friend WithEvents txtEmailAddress As System.Windows.Forms.TextBox
    Friend WithEvents btnRemoveRecurrence As System.Windows.Forms.Button
    Friend WithEvents lblSMTP As System.Windows.Forms.Label
    Friend WithEvents txtSMTPHost As System.Windows.Forms.TextBox
    Friend WithEvents cmdEmailSMTP As System.Windows.Forms.Button

End Class
