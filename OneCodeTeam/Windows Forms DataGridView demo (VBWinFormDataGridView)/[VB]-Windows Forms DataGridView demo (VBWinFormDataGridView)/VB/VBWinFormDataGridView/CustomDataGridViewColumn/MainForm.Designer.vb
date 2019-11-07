Namespace VBWinFormDataGridView.CustomDataGridViewColumn

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
            Me.label2 = New System.Windows.Forms.Label
            Me.employeesDataGridView = New System.Windows.Forms.DataGridView
            CType(Me.employeesDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'label2
            '
            Me.label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.label2.Location = New System.Drawing.Point(27, 24)
            Me.label2.Name = "label2"
            Me.label2.Size = New System.Drawing.Size(580, 87)
            Me.label2.TabIndex = 5
            Me.label2.Text = resources.GetString("label2.Text")
            '
            'employeesDataGridView
            '
            Me.employeesDataGridView.Location = New System.Drawing.Point(30, 124)
            Me.employeesDataGridView.Name = "employeesDataGridView"
            Me.employeesDataGridView.RowTemplate.Height = 21
            Me.employeesDataGridView.Size = New System.Drawing.Size(577, 299)
            Me.employeesDataGridView.TabIndex = 4
            '
            'MainForm
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(634, 446)
            Me.Controls.Add(Me.label2)
            Me.Controls.Add(Me.employeesDataGridView)
            Me.Name = "MainForm"
            Me.Text = "DataGridViewCustomColumn Sample"
            CType(Me.employeesDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents label2 As System.Windows.Forms.Label
        Private WithEvents employeesDataGridView As System.Windows.Forms.DataGridView

    End Class

End Namespace
