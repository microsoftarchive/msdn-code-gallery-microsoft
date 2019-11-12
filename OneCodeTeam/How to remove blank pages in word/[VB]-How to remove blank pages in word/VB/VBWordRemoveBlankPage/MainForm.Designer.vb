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
        Me.label1 = New System.Windows.Forms.Label()
        Me.txbWordPath = New System.Windows.Forms.TextBox()
        Me.btnOpenWord = New System.Windows.Forms.Button()
        Me.btnRemove = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(13, 13)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(88, 13)
        Me.label1.TabIndex = 1
        Me.label1.Text = "Word Document:"
        '
        'txbWordPath
        '
        Me.txbWordPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txbWordPath.Location = New System.Drawing.Point(107, 10)
        Me.txbWordPath.Name = "txbWordPath"
        Me.txbWordPath.Size = New System.Drawing.Size(219, 20)
        Me.txbWordPath.TabIndex = 2
        '
        'btnOpenWord
        '
        Me.btnOpenWord.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOpenWord.Location = New System.Drawing.Point(332, 8)
        Me.btnOpenWord.Name = "btnOpenWord"
        Me.btnOpenWord.Size = New System.Drawing.Size(75, 23)
        Me.btnOpenWord.TabIndex = 3
        Me.btnOpenWord.Text = "Select"
        Me.btnOpenWord.UseVisualStyleBackColor = True
        '
        'btnRemove
        '
        Me.btnRemove.Location = New System.Drawing.Point(13, 48)
        Me.btnRemove.Name = "btnRemove"
        Me.btnRemove.Size = New System.Drawing.Size(130, 23)
        Me.btnRemove.TabIndex = 4
        Me.btnRemove.Text = "Remove Blank Page"
        Me.btnRemove.UseVisualStyleBackColor = True
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(438, 125)
        Me.Controls.Add(Me.btnRemove)
        Me.Controls.Add(Me.btnOpenWord)
        Me.Controls.Add(Me.txbWordPath)
        Me.Controls.Add(Me.label1)
        Me.MinimumSize = New System.Drawing.Size(400, 120)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "RemoveBlankPage"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents txbWordPath As System.Windows.Forms.TextBox
    Private WithEvents btnOpenWord As System.Windows.Forms.Button
    Private WithEvents btnRemove As System.Windows.Forms.Button

End Class
