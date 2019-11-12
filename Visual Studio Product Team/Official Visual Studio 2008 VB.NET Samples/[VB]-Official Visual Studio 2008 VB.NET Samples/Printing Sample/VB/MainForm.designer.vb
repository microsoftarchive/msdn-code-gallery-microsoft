' Copyright (c) Microsoft Corporation. All rights reserved.
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class MainForm
    Inherits System.Windows.Forms.Form

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.txtDocument = New System.Windows.Forms.TextBox
        Me.btnPageSetup = New System.Windows.Forms.Button
        Me.btnPrintDialog = New System.Windows.Forms.Button
        Me.btnPrintPreview = New System.Windows.Forms.Button
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
        Me.PrintDocument1 = New System.Drawing.Printing.PrintDocument
        Me.PrintDialog1 = New System.Windows.Forms.PrintDialog
        Me.PrintPreviewDialog1 = New System.Windows.Forms.PrintPreviewDialog
        Me.PageSetupDialog1 = New System.Windows.Forms.PageSetupDialog
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtDocument
        '
        Me.txtDocument.AccessibleDescription = "TextBox to contain text for printing"
        Me.txtDocument.AccessibleName = "TextBox for printing"
        Me.txtDocument.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.txtDocument.Location = New System.Drawing.Point(12, 66)
        Me.txtDocument.Multiline = True
        Me.txtDocument.Name = "txtDocument"
        Me.txtDocument.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtDocument.Size = New System.Drawing.Size(400, 272)
        Me.txtDocument.TabIndex = 8
        '
        'btnPageSetup
        '
        Me.btnPageSetup.AccessibleDescription = "Button with text ""Page Setup"""
        Me.btnPageSetup.AccessibleName = "Page Setup button"
        Me.btnPageSetup.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnPageSetup.Location = New System.Drawing.Point(204, 34)
        Me.btnPageSetup.Name = "btnPageSetup"
        Me.btnPageSetup.Size = New System.Drawing.Size(88, 23)
        Me.btnPageSetup.TabIndex = 7
        Me.btnPageSetup.Text = "Page &Setup"
        '
        'btnPrintDialog
        '
        Me.btnPrintDialog.AccessibleDescription = "Button with text ""Print Dialog"""
        Me.btnPrintDialog.AccessibleName = "Print Dialog button"
        Me.btnPrintDialog.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnPrintDialog.Location = New System.Drawing.Point(108, 34)
        Me.btnPrintDialog.Name = "btnPrintDialog"
        Me.btnPrintDialog.Size = New System.Drawing.Size(88, 23)
        Me.btnPrintDialog.TabIndex = 6
        Me.btnPrintDialog.Text = "Print &Dialog"
        '
        'btnPrintPreview
        '
        Me.btnPrintPreview.AccessibleDescription = "Button with text ""Print Preview"""
        Me.btnPrintPreview.AccessibleName = "Print Preview button"
        Me.btnPrintPreview.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnPrintPreview.Location = New System.Drawing.Point(12, 34)
        Me.btnPrintPreview.Name = "btnPrintPreview"
        Me.btnPrintPreview.Size = New System.Drawing.Size(88, 23)
        Me.btnPrintPreview.TabIndex = 5
        Me.btnPrintPreview.Text = "Print &Preview"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(429, 24)
        Me.MenuStrip1.TabIndex = 9
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'fileToolStripMenuItem
        '
        Me.fileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.exitToolStripMenuItem})
        Me.fileToolStripMenuItem.Name = "fileToolStripMenuItem"
        Me.fileToolStripMenuItem.Size = New System.Drawing.Size(40, 20)
        Me.fileToolStripMenuItem.Text = "&File"
        '
        'exitToolStripMenuItem
        '
        Me.exitToolStripMenuItem.Name = "exitToolStripMenuItem"
        Me.exitToolStripMenuItem.Size = New System.Drawing.Size(109, 22)
        Me.exitToolStripMenuItem.Text = "E&xit"
        '
        'PrintDocument1
        '
        '
        'PrintPreviewDialog1
        '
        Me.PrintPreviewDialog1.AutoScrollMargin = New System.Drawing.Size(0, 0)
        Me.PrintPreviewDialog1.AutoScrollMinSize = New System.Drawing.Size(0, 0)
        Me.PrintPreviewDialog1.ClientSize = New System.Drawing.Size(400, 300)
        Me.PrintPreviewDialog1.Enabled = True
        Me.PrintPreviewDialog1.Icon = CType(resources.GetObject("PrintPreviewDialog1.Icon"), System.Drawing.Icon)
        Me.PrintPreviewDialog1.Name = "PrintPreviewDialog1"
        Me.PrintPreviewDialog1.Visible = False
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(429, 357)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.txtDocument)
        Me.Controls.Add(Me.btnPageSetup)
        Me.Controls.Add(Me.btnPrintDialog)
        Me.Controls.Add(Me.btnPrintPreview)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "MainForm"
        Me.Text = "Printing Sample"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtDocument As System.Windows.Forms.TextBox
    Friend WithEvents btnPageSetup As System.Windows.Forms.Button
    Friend WithEvents btnPrintDialog As System.Windows.Forms.Button
    Friend WithEvents btnPrintPreview As System.Windows.Forms.Button
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents PrintDocument1 As System.Drawing.Printing.PrintDocument
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PrintDialog1 As System.Windows.Forms.PrintDialog
    Friend WithEvents PrintPreviewDialog1 As System.Windows.Forms.PrintPreviewDialog
    Friend WithEvents PageSetupDialog1 As System.Windows.Forms.PageSetupDialog

End Class
