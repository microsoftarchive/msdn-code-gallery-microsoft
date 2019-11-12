
Partial Public Class AutoUpdate
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.dgvWatch = New System.Windows.Forms.DataGridView()
        Me.btnGetData = New System.Windows.Forms.Button()
        Me.splitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.label6 = New System.Windows.Forms.Label()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.proBar = New System.Windows.Forms.ProgressBar()
        Me.txtInterval = New System.Windows.Forms.TextBox()
        Me.label5 = New System.Windows.Forms.Label()
        Me.rabtnRegUpdate = New System.Windows.Forms.RadioButton()
        Me.rabtnImUpdate = New System.Windows.Forms.RadioButton()
        Me.txtHighPrice = New System.Windows.Forms.TextBox()
        Me.txtLowPrice = New System.Windows.Forms.TextBox()
        Me.label4 = New System.Windows.Forms.Label()
        Me.label3 = New System.Windows.Forms.Label()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.txtNewPrice = New System.Windows.Forms.TextBox()
        Me.txtId = New System.Windows.Forms.TextBox()
        Me.label2 = New System.Windows.Forms.Label()
        Me.label1 = New System.Windows.Forms.Label()
        CType(Me.dgvWatch, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.splitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitContainer1.Panel1.SuspendLayout()
        Me.splitContainer1.Panel2.SuspendLayout()
        Me.splitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'dgvWatch
        '
        Me.dgvWatch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvWatch.Location = New System.Drawing.Point(12, 187)
        Me.dgvWatch.Name = "dgvWatch"
        Me.dgvWatch.Size = New System.Drawing.Size(443, 191)
        Me.dgvWatch.TabIndex = 0
        '
        'btnGetData
        '
        Me.btnGetData.Location = New System.Drawing.Point(109, 147)
        Me.btnGetData.Name = "btnGetData"
        Me.btnGetData.Size = New System.Drawing.Size(75, 23)
        Me.btnGetData.TabIndex = 1
        Me.btnGetData.Text = "Get Data"
        Me.btnGetData.UseVisualStyleBackColor = True
        '
        'splitContainer1
        '
        Me.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.splitContainer1.Name = "splitContainer1"
        '
        'splitContainer1.Panel1
        '
        Me.splitContainer1.Panel1.Controls.Add(Me.label6)
        Me.splitContainer1.Panel1.Controls.Add(Me.btnStop)
        Me.splitContainer1.Panel1.Controls.Add(Me.proBar)
        Me.splitContainer1.Panel1.Controls.Add(Me.txtInterval)
        Me.splitContainer1.Panel1.Controls.Add(Me.label5)
        Me.splitContainer1.Panel1.Controls.Add(Me.rabtnRegUpdate)
        Me.splitContainer1.Panel1.Controls.Add(Me.rabtnImUpdate)
        Me.splitContainer1.Panel1.Controls.Add(Me.txtHighPrice)
        Me.splitContainer1.Panel1.Controls.Add(Me.txtLowPrice)
        Me.splitContainer1.Panel1.Controls.Add(Me.label4)
        Me.splitContainer1.Panel1.Controls.Add(Me.label3)
        Me.splitContainer1.Panel1.Controls.Add(Me.dgvWatch)
        Me.splitContainer1.Panel1.Controls.Add(Me.btnGetData)
        '
        'splitContainer1.Panel2
        '
        Me.splitContainer1.Panel2.Controls.Add(Me.btnUpdate)
        Me.splitContainer1.Panel2.Controls.Add(Me.txtNewPrice)
        Me.splitContainer1.Panel2.Controls.Add(Me.txtId)
        Me.splitContainer1.Panel2.Controls.Add(Me.label2)
        Me.splitContainer1.Panel2.Controls.Add(Me.label1)
        Me.splitContainer1.Size = New System.Drawing.Size(740, 414)
        Me.splitContainer1.SplitterDistance = 463
        Me.splitContainer1.TabIndex = 4
        '
        'label6
        '
        Me.label6.AutoSize = True
        Me.label6.Enabled = False
        Me.label6.Location = New System.Drawing.Point(424, 52)
        Me.label6.Name = "label6"
        Me.label6.Size = New System.Drawing.Size(18, 13)
        Me.label6.TabIndex = 12
        Me.label6.Text = "(s)"
        '
        'btnStop
        '
        Me.btnStop.Enabled = False
        Me.btnStop.Location = New System.Drawing.Point(250, 147)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(75, 23)
        Me.btnStop.TabIndex = 11
        Me.btnStop.Text = "Stop"
        Me.btnStop.UseVisualStyleBackColor = True
        '
        'proBar
        '
        Me.proBar.Enabled = False
        Me.proBar.Location = New System.Drawing.Point(12, 88)
        Me.proBar.Name = "proBar"
        Me.proBar.Size = New System.Drawing.Size(439, 23)
        Me.proBar.TabIndex = 10
        '
        'txtInterval
        '
        Me.txtInterval.Enabled = False
        Me.txtInterval.Location = New System.Drawing.Point(376, 49)
        Me.txtInterval.Name = "txtInterval"
        Me.txtInterval.Size = New System.Drawing.Size(41, 20)
        Me.txtInterval.TabIndex = 9
        '
        'label5
        '
        Me.label5.AutoSize = True
        Me.label5.Enabled = False
        Me.label5.Location = New System.Drawing.Point(333, 52)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(34, 13)
        Me.label5.TabIndex = 8
        Me.label5.Text = "Every"
        '
        'rabtnRegUpdate
        '
        Me.rabtnRegUpdate.AutoSize = True
        Me.rabtnRegUpdate.Location = New System.Drawing.Point(337, 12)
        Me.rabtnRegUpdate.Name = "rabtnRegUpdate"
        Me.rabtnRegUpdate.Size = New System.Drawing.Size(107, 17)
        Me.rabtnRegUpdate.TabIndex = 7
        Me.rabtnRegUpdate.Text = "Regularly Update"
        Me.rabtnRegUpdate.UseVisualStyleBackColor = True
        '
        'rabtnImUpdate
        '
        Me.rabtnImUpdate.AutoSize = True
        Me.rabtnImUpdate.Checked = True
        Me.rabtnImUpdate.Location = New System.Drawing.Point(201, 12)
        Me.rabtnImUpdate.Name = "rabtnImUpdate"
        Me.rabtnImUpdate.Size = New System.Drawing.Size(118, 17)
        Me.rabtnImUpdate.TabIndex = 6
        Me.rabtnImUpdate.TabStop = True
        Me.rabtnImUpdate.Text = "Immediately Update"
        Me.rabtnImUpdate.UseVisualStyleBackColor = True
        '
        'txtHighPrice
        '
        Me.txtHighPrice.Location = New System.Drawing.Point(84, 49)
        Me.txtHighPrice.Name = "txtHighPrice"
        Me.txtHighPrice.Size = New System.Drawing.Size(100, 20)
        Me.txtHighPrice.TabIndex = 5
        '
        'txtLowPrice
        '
        Me.txtLowPrice.Location = New System.Drawing.Point(84, 13)
        Me.txtLowPrice.Name = "txtLowPrice"
        Me.txtLowPrice.Size = New System.Drawing.Size(100, 20)
        Me.txtLowPrice.TabIndex = 4
        '
        'label4
        '
        Me.label4.AutoSize = True
        Me.label4.Location = New System.Drawing.Point(12, 49)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(59, 13)
        Me.label4.TabIndex = 3
        Me.label4.Text = "High Price:"
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.Location = New System.Drawing.Point(12, 17)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(57, 13)
        Me.label3.TabIndex = 2
        Me.label3.Text = "Low Price:"
        '
        'btnUpdate
        '
        Me.btnUpdate.Enabled = False
        Me.btnUpdate.Location = New System.Drawing.Point(88, 88)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(75, 23)
        Me.btnUpdate.TabIndex = 4
        Me.btnUpdate.Text = "Update"
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'txtNewPrice
        '
        Me.txtNewPrice.CausesValidation = False
        Me.txtNewPrice.Enabled = False
        Me.txtNewPrice.Location = New System.Drawing.Point(129, 37)
        Me.txtNewPrice.Name = "txtNewPrice"
        Me.txtNewPrice.Size = New System.Drawing.Size(100, 20)
        Me.txtNewPrice.TabIndex = 3
        '
        'txtId
        '
        Me.txtId.Enabled = False
        Me.txtId.Location = New System.Drawing.Point(129, 10)
        Me.txtId.Name = "txtId"
        Me.txtId.Size = New System.Drawing.Size(100, 20)
        Me.txtId.TabIndex = 2
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(31, 44)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(99, 13)
        Me.label2.TabIndex = 1
        Me.label2.Text = "Product New Price:"
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(31, 13)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(59, 13)
        Me.label1.TabIndex = 0
        Me.label1.Text = "Product Id:"
        '
        'AutoUpdate
        '
        Me.ClientSize = New System.Drawing.Size(740, 414)
        Me.Controls.Add(Me.splitContainer1)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "AutoUpdate"
        Me.Text = "Auto Update"
        CType(Me.dgvWatch, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitContainer1.Panel1.ResumeLayout(False)
        Me.splitContainer1.Panel1.PerformLayout()
        Me.splitContainer1.Panel2.ResumeLayout(False)
        Me.splitContainer1.Panel2.PerformLayout()
        CType(Me.splitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private WithEvents dgvWatch As DataGridView
    Private WithEvents btnGetData As Button
    Private splitContainer1 As SplitContainer
    Private WithEvents txtNewPrice As TextBox
    Private WithEvents txtId As TextBox
    Private label2 As Label
    Private label1 As Label
    Private WithEvents btnUpdate As Button
    Private label6 As Label
    Private WithEvents btnStop As Button
    Private proBar As ProgressBar
    Private WithEvents txtInterval As TextBox
    Private label5 As Label
    Private WithEvents rabtnRegUpdate As RadioButton
    Private rabtnImUpdate As RadioButton
    Private WithEvents txtHighPrice As TextBox
    Private WithEvents txtLowPrice As TextBox
    Private label4 As Label
    Private label3 As Label
End Class

