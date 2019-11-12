' Copyright (c) Microsoft Corporation. All rights reserved.
Partial Public Class WriteForm
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
        Me.rdoError = New System.Windows.Forms.RadioButton
        Me.txtEntry = New System.Windows.Forms.TextBox
        Me.lblEntryText = New System.Windows.Forms.Label
        Me.groEventEntry = New System.Windows.Forms.GroupBox
        Me.rdoWarning = New System.Windows.Forms.RadioButton
        Me.rdoInfo = New System.Windows.Forms.RadioButton
        Me.txtEventID = New System.Windows.Forms.TextBox
        Me.lblEventID = New System.Windows.Forms.Label
        Me.btnWriteEntry = New System.Windows.Forms.Button
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.CloseToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.groEventEntry.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'rdoError
        '
        Me.rdoError.AccessibleDescription = "Error event option"
        Me.rdoError.AccessibleName = "Error Event Option"
        Me.rdoError.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rdoError.Location = New System.Drawing.Point(16, 88)
        Me.rdoError.Name = "rdoError"
        Me.rdoError.Size = New System.Drawing.Size(152, 24)
        Me.rdoError.TabIndex = 2
        Me.rdoError.Text = "E&rror"
        '
        'txtEntry
        '
        Me.txtEntry.AccessibleDescription = "Type the Entry Text"
        Me.txtEntry.AccessibleName = "Entry Text"
        Me.txtEntry.Location = New System.Drawing.Point(41, 65)
        Me.txtEntry.Name = "txtEntry"
        Me.txtEntry.Size = New System.Drawing.Size(176, 20)
        Me.txtEntry.TabIndex = 14
        '
        'lblEntryText
        '
        Me.lblEntryText.AccessibleDescription = "Label for Entry Text"
        Me.lblEntryText.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblEntryText.Location = New System.Drawing.Point(41, 49)
        Me.lblEntryText.Name = "lblEntryText"
        Me.lblEntryText.Size = New System.Drawing.Size(176, 23)
        Me.lblEntryText.TabIndex = 13
        Me.lblEntryText.Text = "&Entry Text:"
        '
        'groEventEntry
        '
        Me.groEventEntry.AccessibleDescription = "Event Log Entry type option box"
        Me.groEventEntry.AccessibleName = "Log type Option Box"
        Me.groEventEntry.Controls.Add(Me.rdoError)
        Me.groEventEntry.Controls.Add(Me.rdoWarning)
        Me.groEventEntry.Controls.Add(Me.rdoInfo)
        Me.groEventEntry.Location = New System.Drawing.Point(249, 49)
        Me.groEventEntry.Name = "groEventEntry"
        Me.groEventEntry.Size = New System.Drawing.Size(176, 120)
        Me.groEventEntry.TabIndex = 17
        Me.groEventEntry.TabStop = False
        Me.groEventEntry.Text = "Eve&nt Log Entry Type:"
        '
        'rdoWarning
        '
        Me.rdoWarning.AccessibleDescription = "Warning event option"
        Me.rdoWarning.AccessibleName = "Warning Event Option"
        Me.rdoWarning.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rdoWarning.Location = New System.Drawing.Point(16, 56)
        Me.rdoWarning.Name = "rdoWarning"
        Me.rdoWarning.Size = New System.Drawing.Size(152, 24)
        Me.rdoWarning.TabIndex = 1
        Me.rdoWarning.Text = "W&arning"
        '
        'rdoInfo
        '
        Me.rdoInfo.AccessibleDescription = "Informational event option"
        Me.rdoInfo.AccessibleName = "Informational Event Option"
        Me.rdoInfo.Checked = True
        Me.rdoInfo.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rdoInfo.Location = New System.Drawing.Point(16, 24)
        Me.rdoInfo.Name = "rdoInfo"
        Me.rdoInfo.Size = New System.Drawing.Size(152, 24)
        Me.rdoInfo.TabIndex = 0
        Me.rdoInfo.Text = "&Informational"
        '
        'txtEventID
        '
        Me.txtEventID.AccessibleDescription = "Type the event ID"
        Me.txtEventID.AccessibleName = "Event ID"
        Me.txtEventID.Location = New System.Drawing.Point(41, 129)
        Me.txtEventID.Name = "txtEventID"
        Me.txtEventID.Size = New System.Drawing.Size(176, 20)
        Me.txtEventID.TabIndex = 16
        '
        'lblEventID
        '
        Me.lblEventID.AccessibleDescription = "Label for Event ID"
        Me.lblEventID.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblEventID.Location = New System.Drawing.Point(41, 113)
        Me.lblEventID.Name = "lblEventID"
        Me.lblEventID.Size = New System.Drawing.Size(176, 16)
        Me.lblEventID.TabIndex = 15
        Me.lblEventID.Text = "E&vent ID:"
        '
        'btnWriteEntry
        '
        Me.btnWriteEntry.AccessibleDescription = "Click to write the entery to the application event log"
        Me.btnWriteEntry.AccessibleName = "Write Button"
        Me.btnWriteEntry.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnWriteEntry.Location = New System.Drawing.Point(193, 185)
        Me.btnWriteEntry.Name = "btnWriteEntry"
        Me.btnWriteEntry.Size = New System.Drawing.Size(232, 23)
        Me.btnWriteEntry.TabIndex = 18
        Me.btnWriteEntry.Text = "&Write an Entry to the Application Event Log"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CloseToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(6, 2, 0, 2)
        Me.MenuStrip1.TabIndex = 19
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'CloseToolStripMenuItem
        '
        Me.CloseToolStripMenuItem.Name = "CloseToolStripMenuItem"
        Me.CloseToolStripMenuItem.Text = "&Close"
        '
        'WriteForm
        '
        Me.ClientSize = New System.Drawing.Size(485, 237)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.groEventEntry)
        Me.Controls.Add(Me.txtEntry)
        Me.Controls.Add(Me.lblEntryText)
        Me.Controls.Add(Me.txtEventID)
        Me.Controls.Add(Me.lblEventID)
        Me.Controls.Add(Me.btnWriteEntry)
        Me.Name = "WriteForm"
        Me.Text = "Write to Log"
        Me.groEventEntry.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents rdoError As System.Windows.Forms.RadioButton
    Friend WithEvents txtEntry As System.Windows.Forms.TextBox
    Friend WithEvents lblEntryText As System.Windows.Forms.Label
    Friend WithEvents groEventEntry As System.Windows.Forms.GroupBox
    Friend WithEvents rdoWarning As System.Windows.Forms.RadioButton
    Friend WithEvents rdoInfo As System.Windows.Forms.RadioButton
    Friend WithEvents txtEventID As System.Windows.Forms.TextBox
    Friend WithEvents lblEventID As System.Windows.Forms.Label
    Friend WithEvents btnWriteEntry As System.Windows.Forms.Button
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents CloseToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
