' Copyright (c) Microsoft Corporation. All rights reserved.
Partial Public Class CreateDeleteForm
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
        Me.lblLogFileToDelete = New System.Windows.Forms.Label
        Me.lblLogFileToCreate = New System.Windows.Forms.Label
        Me.txtLogNameToDelete = New System.Windows.Forms.TextBox
        Me.txtLogNameToCreate = New System.Windows.Forms.TextBox
        Me.btnDeleteLog = New System.Windows.Forms.Button
        Me.btnCreateLog = New System.Windows.Forms.Button
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblLogFileToDelete
        '
        Me.lblLogFileToDelete.AccessibleDescription = "Name of log to delete label"
        Me.lblLogFileToDelete.AccessibleName = "Delete Log Label"
        Me.lblLogFileToDelete.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblLogFileToDelete.Location = New System.Drawing.Point(255, 42)
        Me.lblLogFileToDelete.Name = "lblLogFileToDelete"
        Me.lblLogFileToDelete.Size = New System.Drawing.Size(168, 23)
        Me.lblLogFileToDelete.TabIndex = 16
        Me.lblLogFileToDelete.Text = "Name of Log to D&elete:"
        '
        'lblLogFileToCreate
        '
        Me.lblLogFileToCreate.AccessibleDescription = "Name of log to create label"
        Me.lblLogFileToCreate.AccessibleName = "Create Log Label"
        Me.lblLogFileToCreate.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblLogFileToCreate.Location = New System.Drawing.Point(47, 42)
        Me.lblLogFileToCreate.Name = "lblLogFileToCreate"
        Me.lblLogFileToCreate.Size = New System.Drawing.Size(160, 23)
        Me.lblLogFileToCreate.TabIndex = 13
        Me.lblLogFileToCreate.Text = "Name of Log to C&reate:"
        '
        'txtLogNameToDelete
        '
        Me.txtLogNameToDelete.AccessibleDescription = "Name of log to delete"
        Me.txtLogNameToDelete.AccessibleName = "Delete Log Name"
        Me.txtLogNameToDelete.Location = New System.Drawing.Point(255, 66)
        Me.txtLogNameToDelete.Name = "txtLogNameToDelete"
        Me.txtLogNameToDelete.Size = New System.Drawing.Size(176, 20)
        Me.txtLogNameToDelete.TabIndex = 17
        '
        'txtLogNameToCreate
        '
        Me.txtLogNameToCreate.AccessibleDescription = "Name of Log to Create"
        Me.txtLogNameToCreate.AccessibleName = "Create Log Name"
        Me.txtLogNameToCreate.Location = New System.Drawing.Point(47, 66)
        Me.txtLogNameToCreate.Name = "txtLogNameToCreate"
        Me.txtLogNameToCreate.Size = New System.Drawing.Size(168, 20)
        Me.txtLogNameToCreate.TabIndex = 14
        '
        'btnDeleteLog
        '
        Me.btnDeleteLog.AccessibleDescription = "Click to delete the log"
        Me.btnDeleteLog.AccessibleName = "Delete Log Button"
        Me.btnDeleteLog.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnDeleteLog.Location = New System.Drawing.Point(255, 106)
        Me.btnDeleteLog.Name = "btnDeleteLog"
        Me.btnDeleteLog.Size = New System.Drawing.Size(176, 23)
        Me.btnDeleteLog.TabIndex = 18
        Me.btnDeleteLog.Text = "&Delete an Event Log"
        '
        'btnCreateLog
        '
        Me.btnCreateLog.AccessibleDescription = "Click to create the log"
        Me.btnCreateLog.AccessibleName = "Create Log Button"
        Me.btnCreateLog.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnCreateLog.Location = New System.Drawing.Point(47, 106)
        Me.btnCreateLog.Name = "btnCreateLog"
        Me.btnCreateLog.Size = New System.Drawing.Size(168, 23)
        Me.btnCreateLog.TabIndex = 15
        Me.btnCreateLog.Text = "&Create an Event Log"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CloseToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(6, 2, 0, 2)
        Me.MenuStrip1.TabIndex = 21
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        Me.CloseToolStripMenuItem.Text = "&Close"
        '
        'CreateDeleteForm
        '
        Me.ClientSize = New System.Drawing.Size(496, 167)
        Me.Controls.Add(Me.lblLogFileToDelete)
        Me.Controls.Add(Me.lblLogFileToCreate)
        Me.Controls.Add(Me.txtLogNameToDelete)
        Me.Controls.Add(Me.txtLogNameToCreate)
        Me.Controls.Add(Me.btnDeleteLog)
        Me.Controls.Add(Me.btnCreateLog)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Name = "CreateDeleteForm"
        Me.Text = "Create or Delete Log"
        Me.MenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblLogFileToDelete As System.Windows.Forms.Label
    Friend WithEvents lblLogFileToCreate As System.Windows.Forms.Label
    Friend WithEvents txtLogNameToDelete As System.Windows.Forms.TextBox
    Friend WithEvents txtLogNameToCreate As System.Windows.Forms.TextBox
    Friend WithEvents btnDeleteLog As System.Windows.Forms.Button
    Friend WithEvents btnCreateLog As System.Windows.Forms.Button
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents CloseToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
