<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Frm_Client
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.tableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel()
        Me.cbx_Container = New System.Windows.Forms.ComboBox()
        Me.label1 = New System.Windows.Forms.Label()
        Me.panel1 = New System.Windows.Forms.Panel()
        Me.flowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.tableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.lbl_ContainerName = New System.Windows.Forms.Label()
        Me.txt_ContainerName = New System.Windows.Forms.TextBox()
        Me.tableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.btn_SelectFile = New System.Windows.Forms.Button()
        Me.txt_FileName = New System.Windows.Forms.TextBox()
        Me.btn_UnZipAndUpload = New System.Windows.Forms.Button()
        Me.tableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.label6 = New System.Windows.Forms.Label()
        Me.label7 = New System.Windows.Forms.Label()
        Me.label5 = New System.Windows.Forms.Label()
        Me.label4 = New System.Windows.Forms.Label()
        Me.label3 = New System.Windows.Forms.Label()
        Me.label2 = New System.Windows.Forms.Label()
        Me.label8 = New System.Windows.Forms.Label()
        Me.tableLayoutPanel4.SuspendLayout()
        Me.panel1.SuspendLayout()
        Me.tableLayoutPanel2.SuspendLayout()
        Me.tableLayoutPanel3.SuspendLayout()
        Me.tableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tableLayoutPanel4
        '
        Me.tableLayoutPanel4.ColumnCount = 3
        Me.tableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130.0!))
        Me.tableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400.0!))
        Me.tableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tableLayoutPanel4.Controls.Add(Me.cbx_Container, 1, 0)
        Me.tableLayoutPanel4.Controls.Add(Me.label1, 0, 0)
        Me.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tableLayoutPanel4.Location = New System.Drawing.Point(5, 42)
        Me.tableLayoutPanel4.Margin = New System.Windows.Forms.Padding(5)
        Me.tableLayoutPanel4.Name = "tableLayoutPanel4"
        Me.tableLayoutPanel4.RowCount = 1
        Me.tableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tableLayoutPanel4.Size = New System.Drawing.Size(799, 39)
        Me.tableLayoutPanel4.TabIndex = 0
        '
        'cbx_Container
        '
        Me.cbx_Container.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.cbx_Container.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cbx_Container.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cbx_Container.FormattingEnabled = True
        Me.cbx_Container.Location = New System.Drawing.Point(135, 5)
        Me.cbx_Container.Margin = New System.Windows.Forms.Padding(5)
        Me.cbx_Container.Name = "cbx_Container"
        Me.cbx_Container.Size = New System.Drawing.Size(390, 24)
        Me.cbx_Container.TabIndex = 5
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.label1.Location = New System.Drawing.Point(5, 0)
        Me.label1.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(120, 39)
        Me.label1.TabIndex = 6
        Me.label1.Text = "Container Name:"
        '
        'panel1
        '
        Me.panel1.Controls.Add(Me.label7)
        Me.panel1.Controls.Add(Me.label5)
        Me.panel1.Controls.Add(Me.label4)
        Me.panel1.Controls.Add(Me.label3)
        Me.panel1.Controls.Add(Me.label2)
        Me.panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.panel1.Location = New System.Drawing.Point(4, 324)
        Me.panel1.Margin = New System.Windows.Forms.Padding(4)
        Me.panel1.Name = "panel1"
        Me.panel1.Size = New System.Drawing.Size(801, 142)
        Me.panel1.TabIndex = 0
        '
        'flowLayoutPanel1
        '
        Me.flowLayoutPanel1.AutoScroll = True
        Me.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown
        Me.flowLayoutPanel1.Location = New System.Drawing.Point(5, 91)
        Me.flowLayoutPanel1.Margin = New System.Windows.Forms.Padding(5)
        Me.flowLayoutPanel1.Name = "flowLayoutPanel1"
        Me.flowLayoutPanel1.Size = New System.Drawing.Size(799, 224)
        Me.flowLayoutPanel1.TabIndex = 8
        '
        'tableLayoutPanel2
        '
        Me.tableLayoutPanel2.ColumnCount = 3
        Me.tableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 129.0!))
        Me.tableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 416.0!))
        Me.tableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tableLayoutPanel2.Controls.Add(Me.lbl_ContainerName, 0, 0)
        Me.tableLayoutPanel2.Controls.Add(Me.txt_ContainerName, 1, 0)
        Me.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tableLayoutPanel2.Location = New System.Drawing.Point(5, 475)
        Me.tableLayoutPanel2.Margin = New System.Windows.Forms.Padding(5)
        Me.tableLayoutPanel2.Name = "tableLayoutPanel2"
        Me.tableLayoutPanel2.RowCount = 1
        Me.tableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tableLayoutPanel2.Size = New System.Drawing.Size(799, 34)
        Me.tableLayoutPanel2.TabIndex = 11
        '
        'lbl_ContainerName
        '
        Me.lbl_ContainerName.AutoSize = True
        Me.lbl_ContainerName.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbl_ContainerName.Location = New System.Drawing.Point(5, 0)
        Me.lbl_ContainerName.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lbl_ContainerName.Name = "lbl_ContainerName"
        Me.lbl_ContainerName.Size = New System.Drawing.Size(119, 34)
        Me.lbl_ContainerName.TabIndex = 2
        Me.lbl_ContainerName.Text = "Container Name:"
        '
        'txt_ContainerName
        '
        Me.txt_ContainerName.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txt_ContainerName.Location = New System.Drawing.Point(134, 5)
        Me.txt_ContainerName.Margin = New System.Windows.Forms.Padding(5)
        Me.txt_ContainerName.Name = "txt_ContainerName"
        Me.txt_ContainerName.Size = New System.Drawing.Size(406, 23)
        Me.txt_ContainerName.TabIndex = 3
        '
        'tableLayoutPanel3
        '
        Me.tableLayoutPanel3.ColumnCount = 3
        Me.tableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 129.0!))
        Me.tableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 417.0!))
        Me.tableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tableLayoutPanel3.Controls.Add(Me.label8, 0, 0)
        Me.tableLayoutPanel3.Controls.Add(Me.txt_FileName, 1, 0)
        Me.tableLayoutPanel3.Controls.Add(Me.btn_SelectFile, 2, 0)
        Me.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tableLayoutPanel3.Location = New System.Drawing.Point(5, 519)
        Me.tableLayoutPanel3.Margin = New System.Windows.Forms.Padding(5)
        Me.tableLayoutPanel3.Name = "tableLayoutPanel3"
        Me.tableLayoutPanel3.RowCount = 1
        Me.tableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tableLayoutPanel3.Size = New System.Drawing.Size(799, 39)
        Me.tableLayoutPanel3.TabIndex = 12
        '
        'btn_SelectFile
        '
        Me.btn_SelectFile.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.btn_SelectFile.Location = New System.Drawing.Point(551, 5)
        Me.btn_SelectFile.Margin = New System.Windows.Forms.Padding(5)
        Me.btn_SelectFile.Name = "btn_SelectFile"
        Me.btn_SelectFile.Size = New System.Drawing.Size(83, 28)
        Me.btn_SelectFile.TabIndex = 0
        Me.btn_SelectFile.Text = "Browse "
        Me.btn_SelectFile.UseVisualStyleBackColor = True
        '
        'txt_FileName
        '
        Me.txt_FileName.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txt_FileName.Location = New System.Drawing.Point(134, 5)
        Me.txt_FileName.Margin = New System.Windows.Forms.Padding(5)
        Me.txt_FileName.Name = "txt_FileName"
        Me.txt_FileName.Size = New System.Drawing.Size(407, 23)
        Me.txt_FileName.TabIndex = 1
        '
        'btn_UnZipAndUpload
        '
        Me.btn_UnZipAndUpload.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btn_UnZipAndUpload.Location = New System.Drawing.Point(256, 568)
        Me.btn_UnZipAndUpload.Margin = New System.Windows.Forms.Padding(5)
        Me.btn_UnZipAndUpload.Name = "btn_UnZipAndUpload"
        Me.btn_UnZipAndUpload.Size = New System.Drawing.Size(297, 30)
        Me.btn_UnZipAndUpload.TabIndex = 9
        Me.btn_UnZipAndUpload.Text = "UnZip FIles and Upload to Blob"
        Me.btn_UnZipAndUpload.UseVisualStyleBackColor = True
        '
        'tableLayoutPanel1
        '
        Me.tableLayoutPanel1.ColumnCount = 1
        Me.tableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tableLayoutPanel1.Controls.Add(Me.flowLayoutPanel1, 0, 2)
        Me.tableLayoutPanel1.Controls.Add(Me.panel1, 0, 3)
        Me.tableLayoutPanel1.Controls.Add(Me.tableLayoutPanel4, 0, 1)
        Me.tableLayoutPanel1.Controls.Add(Me.tableLayoutPanel2, 0, 4)
        Me.tableLayoutPanel1.Controls.Add(Me.tableLayoutPanel3, 0, 5)
        Me.tableLayoutPanel1.Controls.Add(Me.btn_UnZipAndUpload, 0, 6)
        Me.tableLayoutPanel1.Controls.Add(Me.label6, 0, 0)
        Me.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.tableLayoutPanel1.Margin = New System.Windows.Forms.Padding(5)
        Me.tableLayoutPanel1.Name = "tableLayoutPanel1"
        Me.tableLayoutPanel1.RowCount = 7
        Me.tableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37.0!))
        Me.tableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49.0!))
        Me.tableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150.0!))
        Me.tableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44.0!))
        Me.tableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49.0!))
        Me.tableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
        Me.tableLayoutPanel1.Size = New System.Drawing.Size(809, 603)
        Me.tableLayoutPanel1.TabIndex = 12
        '
        'label6
        '
        Me.label6.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.label6.AutoSize = True
        Me.label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.label6.Location = New System.Drawing.Point(4, 10)
        Me.label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.label6.Name = "label6"
        Me.label6.Size = New System.Drawing.Size(413, 17)
        Me.label6.TabIndex = 13
        Me.label6.Text = "Select the container of which you want to view all blobs "
        '
        'label7
        '
        Me.label7.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.label7.AutoSize = True
        Me.label7.Location = New System.Drawing.Point(11, 96)
        Me.label7.Name = "label7"
        Me.label7.Size = New System.Drawing.Size(344, 17)
        Me.label7.TabIndex = 14
        Me.label7.Text = "(4) The name can't contain two consecutive hyphens."
        '
        'label5
        '
        Me.label5.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.label5.AutoSize = True
        Me.label5.Location = New System.Drawing.Point(12, 76)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(777, 17)
        Me.label5.TabIndex = 15
        Me.label5.Text = "(3) Container names can contain only lowercase letters, numbers, and hyphens, and" & _
    " must begin with a letter or a number. "
        '
        'label4
        '
        Me.label4.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.label4.AutoSize = True
        Me.label4.Location = New System.Drawing.Point(12, 55)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(383, 17)
        Me.label4.TabIndex = 16
        Me.label4.Text = "(2) The name should be between 3 and 63 characters long."
        '
        'label3
        '
        Me.label3.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.label3.AutoSize = True
        Me.label3.Location = New System.Drawing.Point(12, 33)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(431, 17)
        Me.label3.TabIndex = 17
        Me.label3.Text = "(1) You can input a new container name or existed container name."
        '
        'label2
        '
        Me.label2.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.label2.AutoSize = True
        Me.label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.label2.Location = New System.Drawing.Point(12, 12)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(477, 17)
        Me.label2.TabIndex = 18
        Me.label2.Text = "Input the name of the container to which you want to upload files"
        '
        'label8
        '
        Me.label8.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.label8.AutoSize = True
        Me.label8.Location = New System.Drawing.Point(15, 11)
        Me.label8.Name = "label8"
        Me.label8.Size = New System.Drawing.Size(99, 17)
        Me.label8.TabIndex = 14
        Me.label8.Text = "Zip File Name:"
        '
        'Frm_Client
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(809, 603)
        Me.Controls.Add(Me.tableLayoutPanel1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.Name = "Frm_Client"
        Me.Text = "Client "
        Me.tableLayoutPanel4.ResumeLayout(False)
        Me.tableLayoutPanel4.PerformLayout()
        Me.panel1.ResumeLayout(False)
        Me.panel1.PerformLayout()
        Me.tableLayoutPanel2.ResumeLayout(False)
        Me.tableLayoutPanel2.PerformLayout()
        Me.tableLayoutPanel3.ResumeLayout(False)
        Me.tableLayoutPanel3.PerformLayout()
        Me.tableLayoutPanel1.ResumeLayout(False)
        Me.tableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents tableLayoutPanel4 As System.Windows.Forms.TableLayoutPanel
    Private WithEvents cbx_Container As System.Windows.Forms.ComboBox
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents panel1 As System.Windows.Forms.Panel
    Private WithEvents flowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel
    Private WithEvents tableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Private WithEvents lbl_ContainerName As System.Windows.Forms.Label
    Private WithEvents txt_ContainerName As System.Windows.Forms.TextBox
    Private WithEvents tableLayoutPanel3 As System.Windows.Forms.TableLayoutPanel
    Private WithEvents btn_SelectFile As System.Windows.Forms.Button
    Private WithEvents txt_FileName As System.Windows.Forms.TextBox
    Private WithEvents btn_UnZipAndUpload As System.Windows.Forms.Button
    Private WithEvents tableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Private WithEvents label6 As System.Windows.Forms.Label
    Private WithEvents label7 As System.Windows.Forms.Label
    Private WithEvents label5 As System.Windows.Forms.Label
    Private WithEvents label4 As System.Windows.Forms.Label
    Private WithEvents label3 As System.Windows.Forms.Label
    Private WithEvents label2 As System.Windows.Forms.Label
    Private WithEvents label8 As System.Windows.Forms.Label

End Class
