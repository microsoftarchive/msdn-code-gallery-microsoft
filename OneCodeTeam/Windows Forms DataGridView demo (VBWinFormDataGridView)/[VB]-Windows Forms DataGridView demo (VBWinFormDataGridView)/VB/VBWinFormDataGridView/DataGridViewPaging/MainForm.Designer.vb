Namespace VBWinFormDataGridView.DataGridViewPaging

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class MainForm
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
            Me.dataGridView1 = New System.Windows.Forms.DataGridView
            Me.toolStripButtonNext = New System.Windows.Forms.ToolStripButton
            Me.toolStripButtonPrev = New System.Windows.Forms.ToolStripButton
            Me.toolStripButtonFirst = New System.Windows.Forms.ToolStripButton
            Me.toolStrip1 = New System.Windows.Forms.ToolStrip
            Me.toolStripButtonLast = New System.Windows.Forms.ToolStripButton
            Me.groupBox1 = New System.Windows.Forms.GroupBox
            CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.toolStrip1.SuspendLayout()
            Me.groupBox1.SuspendLayout()
            Me.SuspendLayout()
            '
            'dataGridView1
            '
            Me.dataGridView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
            Me.dataGridView1.Location = New System.Drawing.Point(3, 44)
            Me.dataGridView1.Name = "dataGridView1"
            Me.dataGridView1.Size = New System.Drawing.Size(632, 425)
            Me.dataGridView1.TabIndex = 0
            '
            'toolStripButtonNext
            '
            Me.toolStripButtonNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.toolStripButtonNext.Image = CType(resources.GetObject("toolStripButtonNext.Image"), System.Drawing.Image)
            Me.toolStripButtonNext.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.toolStripButtonNext.Name = "toolStripButtonNext"
            Me.toolStripButtonNext.Size = New System.Drawing.Size(64, 22)
            Me.toolStripButtonNext.Text = "Next Page"
            '
            'toolStripButtonPrev
            '
            Me.toolStripButtonPrev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.toolStripButtonPrev.Image = CType(resources.GetObject("toolStripButtonPrev.Image"), System.Drawing.Image)
            Me.toolStripButtonPrev.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.toolStripButtonPrev.Name = "toolStripButtonPrev"
            Me.toolStripButtonPrev.Size = New System.Drawing.Size(85, 22)
            Me.toolStripButtonPrev.Text = "Previous Page"
            '
            'toolStripButtonFirst
            '
            Me.toolStripButtonFirst.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.toolStripButtonFirst.Image = CType(resources.GetObject("toolStripButtonFirst.Image"), System.Drawing.Image)
            Me.toolStripButtonFirst.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.toolStripButtonFirst.Name = "toolStripButtonFirst"
            Me.toolStripButtonFirst.Size = New System.Drawing.Size(62, 22)
            Me.toolStripButtonFirst.Text = "First Page"
            '
            'toolStrip1
            '
            Me.toolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripButtonFirst, Me.toolStripButtonPrev, Me.toolStripButtonNext, Me.toolStripButtonLast})
            Me.toolStrip1.Location = New System.Drawing.Point(3, 16)
            Me.toolStrip1.Name = "toolStrip1"
            Me.toolStrip1.Size = New System.Drawing.Size(632, 25)
            Me.toolStrip1.TabIndex = 1
            Me.toolStrip1.Text = "toolStrip1"
            '
            'toolStripButtonLast
            '
            Me.toolStripButtonLast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.toolStripButtonLast.Image = CType(resources.GetObject("toolStripButtonLast.Image"), System.Drawing.Image)
            Me.toolStripButtonLast.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.toolStripButtonLast.Name = "toolStripButtonLast"
            Me.toolStripButtonLast.Size = New System.Drawing.Size(61, 22)
            Me.toolStripButtonLast.Text = "Last Page"
            '
            'groupBox1
            '
            Me.groupBox1.Controls.Add(Me.toolStrip1)
            Me.groupBox1.Controls.Add(Me.dataGridView1)
            Me.groupBox1.Location = New System.Drawing.Point(12, 17)
            Me.groupBox1.Name = "groupBox1"
            Me.groupBox1.Size = New System.Drawing.Size(638, 472)
            Me.groupBox1.TabIndex = 2
            Me.groupBox1.TabStop = False
            Me.groupBox1.Text = "groupBox1"
            Me.groupBox1.UseCompatibleTextRendering = True
            '
            'MainForm
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(662, 507)
            Me.Controls.Add(Me.groupBox1)
            Me.Name = "MainForm"
            Me.Text = "MainForm"
            CType(Me.dataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.toolStrip1.ResumeLayout(False)
            Me.toolStrip1.PerformLayout()
            Me.groupBox1.ResumeLayout(False)
            Me.groupBox1.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents dataGridView1 As System.Windows.Forms.DataGridView
        Private WithEvents toolStripButtonNext As System.Windows.Forms.ToolStripButton
        Private WithEvents toolStripButtonPrev As System.Windows.Forms.ToolStripButton
        Private WithEvents toolStripButtonFirst As System.Windows.Forms.ToolStripButton
        Private WithEvents toolStrip1 As System.Windows.Forms.ToolStrip
        Private WithEvents toolStripButtonLast As System.Windows.Forms.ToolStripButton
        Private WithEvents groupBox1 As System.Windows.Forms.GroupBox
    End Class

End Namespace