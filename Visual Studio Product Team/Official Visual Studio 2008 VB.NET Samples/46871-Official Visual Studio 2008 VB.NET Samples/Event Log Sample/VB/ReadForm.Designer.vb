' Copyright (c) Microsoft Corporation. All rights reserved.
Partial Public Class ReadForm
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
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.lstEntryType = New System.Windows.Forms.ListBox
        Me.btnViewLogEntries = New System.Windows.Forms.Button
        Me.lblEntryType = New System.Windows.Forms.Label
        Me.rchEventLogOutput = New System.Windows.Forms.RichTextBox
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        Me.CloseToolStripMenuItem.Size = New System.Drawing.Size(51, 20)
        Me.CloseToolStripMenuItem.Text = "&Close"
        '
        'lstEntryType
        '
        Me.lstEntryType.AccessibleDescription = "List of logs"
        Me.lstEntryType.AccessibleName = "Log List"
        Me.lstEntryType.FormattingEnabled = True
        Me.lstEntryType.Location = New System.Drawing.Point(12, 77)
        Me.lstEntryType.Name = "lstEntryType"
        Me.lstEntryType.Size = New System.Drawing.Size(184, 43)
        Me.lstEntryType.TabIndex = 22
        '
        'btnViewLogEntries
        '
        Me.btnViewLogEntries.AccessibleDescription = "View Button"
        Me.btnViewLogEntries.AccessibleName = "View Button"
        Me.btnViewLogEntries.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnViewLogEntries.Location = New System.Drawing.Point(12, 126)
        Me.btnViewLogEntries.Name = "btnViewLogEntries"
        Me.btnViewLogEntries.Size = New System.Drawing.Size(184, 23)
        Me.btnViewLogEntries.TabIndex = 23
        Me.btnViewLogEntries.Text = "&View Log Entries"
        '
        'lblEntryType
        '
        Me.lblEntryType.AccessibleDescription = "Choose a log label"
        Me.lblEntryType.AccessibleName = "log label"
        Me.lblEntryType.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblEntryType.Location = New System.Drawing.Point(12, 48)
        Me.lblEntryType.Name = "lblEntryType"
        Me.lblEntryType.Size = New System.Drawing.Size(184, 23)
        Me.lblEntryType.TabIndex = 21
        Me.lblEntryType.Text = "&Choose a Log:"
        '
        'rchEventLogOutput
        '
        Me.rchEventLogOutput.AccessibleDescription = "Contents of log"
        Me.rchEventLogOutput.AccessibleName = "Log Contents"
        Me.rchEventLogOutput.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.rchEventLogOutput.Location = New System.Drawing.Point(219, 77)
        Me.rchEventLogOutput.Name = "rchEventLogOutput"
        Me.rchEventLogOutput.ReadOnly = True
        Me.rchEventLogOutput.Size = New System.Drawing.Size(465, 467)
        Me.rchEventLogOutput.TabIndex = 24
        Me.rchEventLogOutput.Text = ""
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CloseToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(696, 24)
        Me.MenuStrip1.TabIndex = 25
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'ReadForm
        '
        Me.ClientSize = New System.Drawing.Size(696, 562)
        Me.Controls.Add(Me.btnViewLogEntries)
        Me.Controls.Add(Me.lblEntryType)
        Me.Controls.Add(Me.rchEventLogOutput)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.lstEntryType)
        Me.Name = "ReadForm"
        Me.Text = "Read Event Log Entries"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents CloseToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lstEntryType As System.Windows.Forms.ListBox
    Friend WithEvents btnViewLogEntries As System.Windows.Forms.Button
    Friend WithEvents lblEntryType As System.Windows.Forms.Label
    Friend WithEvents rchEventLogOutput As System.Windows.Forms.RichTextBox
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
End Class
