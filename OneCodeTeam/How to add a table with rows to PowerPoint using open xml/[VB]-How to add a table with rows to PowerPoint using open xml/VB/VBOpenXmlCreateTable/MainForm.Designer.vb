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
        Me.lblSource = New System.Windows.Forms.Label()
        Me.txbSource = New System.Windows.Forms.TextBox()
        Me.btnOpen = New System.Windows.Forms.Button()
        Me.btnInserTable = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblSource
        '
        Me.lblSource.AutoSize = True
        Me.lblSource.Location = New System.Drawing.Point(12, 44)
        Me.lblSource.Name = "lblSource"
        Me.lblSource.Size = New System.Drawing.Size(101, 13)
        Me.lblSource.TabIndex = 1
        Me.lblSource.Text = "Source PowerPoint:"
        '
        'txbSource
        '
        Me.txbSource.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txbSource.Location = New System.Drawing.Point(119, 42)
        Me.txbSource.Name = "txbSource"
        Me.txbSource.Size = New System.Drawing.Size(137, 20)
        Me.txbSource.TabIndex = 2
        '
        'btnOpen
        '
        Me.btnOpen.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOpen.Location = New System.Drawing.Point(262, 39)
        Me.btnOpen.Name = "btnOpen"
        Me.btnOpen.Size = New System.Drawing.Size(75, 23)
        Me.btnOpen.TabIndex = 3
        Me.btnOpen.Text = "Open"
        Me.btnOpen.UseVisualStyleBackColor = True
        '
        'btnInserTable
        '
        Me.btnInserTable.Location = New System.Drawing.Point(15, 78)
        Me.btnInserTable.Name = "btnInserTable"
        Me.btnInserTable.Size = New System.Drawing.Size(75, 23)
        Me.btnInserTable.TabIndex = 4
        Me.btnInserTable.Text = "Insert Table"
        Me.btnInserTable.UseVisualStyleBackColor = True
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(349, 243)
        Me.Controls.Add(Me.btnInserTable)
        Me.Controls.Add(Me.btnOpen)
        Me.Controls.Add(Me.txbSource)
        Me.Controls.Add(Me.lblSource)
        Me.MinimumSize = New System.Drawing.Size(300, 200)
        Me.Name = "MainForm"
        Me.Text = "Create Table in PowerPoint"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents lblSource As System.Windows.Forms.Label
    Private WithEvents txbSource As System.Windows.Forms.TextBox
    Private WithEvents btnOpen As System.Windows.Forms.Button
    Private WithEvents btnInserTable As System.Windows.Forms.Button

End Class
