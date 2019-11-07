' Copyright (c) Microsoft Corporation. All rights reserved.
Partial Public Class MainForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.Label2 = New System.Windows.Forms.Label
        Me.Attachments = New System.Windows.Forms.ListBox
        Me.Browse = New System.Windows.Forms.Button
        Me.Priority = New System.Windows.Forms.ComboBox
        Me.BCC = New System.Windows.Forms.TextBox
        Me.CC = New System.Windows.Forms.TextBox
        Me.lblBCC = New System.Windows.Forms.Label
        Me.Body = New System.Windows.Forms.TextBox
        Me.Subject = New System.Windows.Forms.TextBox
        Me.lblCC = New System.Windows.Forms.Label
        Me.odlgAttachment = New System.Windows.Forms.OpenFileDialog
        Me.From = New System.Windows.Forms.TextBox
        Me.ToAddress = New System.Windows.Forms.TextBox
        Me.Send = New System.Windows.Forms.Button
        Me.lblBody = New System.Windows.Forms.Label
        Me.lblSubject = New System.Windows.Forms.Label
        Me.lblTo = New System.Windows.Forms.Label
        Me.lblFrom = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label2
        '
        Me.Label2.Image = CType(resources.GetObject("Label2.Image"), System.Drawing.Image)
        Me.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label2.Location = New System.Drawing.Point(36, 283)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(24, 23)
        Me.Label2.TabIndex = 33
        '
        'Attachments
        '
        Me.Attachments.CausesValidation = False
        Me.Attachments.FormattingEnabled = True
        Me.Attachments.Items.AddRange(New Object() {"(No attachments)"})
        Me.Attachments.Location = New System.Drawing.Point(61, 285)
        Me.Attachments.Name = "Attachments"
        Me.Attachments.Size = New System.Drawing.Size(256, 43)
        Me.Attachments.TabIndex = 34
        '
        'Browse
        '
        Me.Browse.CausesValidation = False
        Me.Browse.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.Browse.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Browse.Image = CType(resources.GetObject("Browse.Image"), System.Drawing.Image)
        Me.Browse.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Browse.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Browse.Location = New System.Drawing.Point(12, 33)
        Me.Browse.Name = "Browse"
        Me.Browse.Size = New System.Drawing.Size(96, 23)
        Me.Browse.TabIndex = 18
        Me.Browse.Text = "   Br&owse..."
        '
        'Priority
        '
        Me.Priority.CausesValidation = False
        Me.Priority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.Priority.FormattingEnabled = True
        Me.Priority.ItemHeight = 13
        Me.Priority.Location = New System.Drawing.Point(231, 34)
        Me.Priority.Name = "Priority"
        Me.Priority.Size = New System.Drawing.Size(86, 21)
        Me.Priority.TabIndex = 20
        '
        'BCC
        '
        Me.BCC.BackColor = System.Drawing.SystemColors.Window
        Me.BCC.CausesValidation = False
        Me.BCC.Location = New System.Drawing.Point(62, 144)
        Me.BCC.Name = "BCC"
        Me.BCC.Size = New System.Drawing.Size(256, 20)
        Me.BCC.TabIndex = 28
        '
        'CC
        '
        Me.CC.BackColor = System.Drawing.SystemColors.Window
        Me.CC.CausesValidation = False
        Me.CC.Location = New System.Drawing.Point(62, 120)
        Me.CC.Name = "CC"
        Me.CC.Size = New System.Drawing.Size(256, 20)
        Me.CC.TabIndex = 26
        '
        'lblBCC
        '
        Me.lblBCC.BackColor = System.Drawing.SystemColors.Control
        Me.lblBCC.CausesValidation = False
        Me.lblBCC.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.lblBCC.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblBCC.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblBCC.Location = New System.Drawing.Point(29, 144)
        Me.lblBCC.Name = "lblBCC"
        Me.lblBCC.Size = New System.Drawing.Size(40, 23)
        Me.lblBCC.TabIndex = 27
        Me.lblBCC.Text = "&Bcc:"
        '
        'Body
        '
        Me.Body.BackColor = System.Drawing.SystemColors.Window
        Me.Body.CausesValidation = False
        Me.Body.Location = New System.Drawing.Point(62, 192)
        Me.Body.Multiline = True
        Me.Body.Name = "Body"
        Me.Body.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.Body.Size = New System.Drawing.Size(256, 85)
        Me.Body.TabIndex = 32
        '
        'Subject
        '
        Me.Subject.BackColor = System.Drawing.SystemColors.Window
        Me.Subject.CausesValidation = False
        Me.Subject.Location = New System.Drawing.Point(62, 168)
        Me.Subject.Name = "Subject"
        Me.Subject.Size = New System.Drawing.Size(256, 20)
        Me.Subject.TabIndex = 30
        '
        'lblCC
        '
        Me.lblCC.BackColor = System.Drawing.SystemColors.Control
        Me.lblCC.CausesValidation = False
        Me.lblCC.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.lblCC.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblCC.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblCC.Location = New System.Drawing.Point(36, 120)
        Me.lblCC.Name = "lblCC"
        Me.lblCC.Size = New System.Drawing.Size(56, 23)
        Me.lblCC.TabIndex = 25
        Me.lblCC.Text = "&Cc:"
        '
        'From
        '
        Me.From.BackColor = System.Drawing.SystemColors.Window
        Me.From.CausesValidation = False
        Me.From.Location = New System.Drawing.Point(62, 72)
        Me.From.Name = "From"
        Me.From.Size = New System.Drawing.Size(256, 20)
        Me.From.TabIndex = 22
        '
        'ToAddress
        '
        Me.ToAddress.BackColor = System.Drawing.SystemColors.Window
        Me.ToAddress.CausesValidation = False
        Me.ToAddress.Location = New System.Drawing.Point(62, 96)
        Me.ToAddress.Name = "ToAddress"
        Me.ToAddress.Size = New System.Drawing.Size(256, 20)
        Me.ToAddress.TabIndex = 24
        '
        'Send
        '
        Me.Send.BackColor = System.Drawing.SystemColors.Control
        Me.Send.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.Send.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Send.Image = CType(resources.GetObject("Send.Image"), System.Drawing.Image)
        Me.Send.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.Send.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Send.Location = New System.Drawing.Point(242, 334)
        Me.Send.Name = "Send"
        Me.Send.Size = New System.Drawing.Size(75, 29)
        Me.Send.TabIndex = 35
        Me.Send.Text = "    &Send!"
        Me.Send.UseVisualStyleBackColor = False
        '
        'lblBody
        '
        Me.lblBody.BackColor = System.Drawing.SystemColors.Control
        Me.lblBody.CausesValidation = False
        Me.lblBody.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.lblBody.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblBody.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblBody.Location = New System.Drawing.Point(20, 192)
        Me.lblBody.Name = "lblBody"
        Me.lblBody.Size = New System.Drawing.Size(48, 23)
        Me.lblBody.TabIndex = 31
        Me.lblBody.Text = "Bo&dy:"
        '
        'lblSubject
        '
        Me.lblSubject.BackColor = System.Drawing.SystemColors.Control
        Me.lblSubject.CausesValidation = False
        Me.lblSubject.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.lblSubject.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblSubject.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblSubject.Location = New System.Drawing.Point(4, 168)
        Me.lblSubject.Name = "lblSubject"
        Me.lblSubject.Size = New System.Drawing.Size(74, 23)
        Me.lblSubject.TabIndex = 29
        Me.lblSubject.Text = "Sub&ject:"
        '
        'lblTo
        '
        Me.lblTo.BackColor = System.Drawing.SystemColors.Control
        Me.lblTo.CausesValidation = False
        Me.lblTo.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.lblTo.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblTo.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblTo.Location = New System.Drawing.Point(36, 96)
        Me.lblTo.Name = "lblTo"
        Me.lblTo.Size = New System.Drawing.Size(31, 23)
        Me.lblTo.TabIndex = 23
        Me.lblTo.Text = "&To:"
        '
        'lblFrom
        '
        Me.lblFrom.BackColor = System.Drawing.SystemColors.Control
        Me.lblFrom.CausesValidation = False
        Me.lblFrom.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.lblFrom.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblFrom.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblFrom.Location = New System.Drawing.Point(20, 72)
        Me.lblFrom.Name = "lblFrom"
        Me.lblFrom.Size = New System.Drawing.Size(56, 23)
        Me.lblFrom.TabIndex = 21
        Me.lblFrom.Text = "F&rom:"
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(173, 36)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(64, 23)
        Me.Label1.TabIndex = 19
        Me.Label1.Text = "&Priority:"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(332, 24)
        Me.MenuStrip1.TabIndex = 36
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExitToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(40, 20)
        Me.FileToolStripMenuItem.Text = "&File"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(109, 22)
        Me.ExitToolStripMenuItem.Text = "E&xit"
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(332, 372)
        Me.Controls.Add(Me.Priority)
        Me.Controls.Add(Me.BCC)
        Me.Controls.Add(Me.CC)
        Me.Controls.Add(Me.lblBCC)
        Me.Controls.Add(Me.Body)
        Me.Controls.Add(Me.Subject)
        Me.Controls.Add(Me.lblCC)
        Me.Controls.Add(Me.From)
        Me.Controls.Add(Me.ToAddress)
        Me.Controls.Add(Me.Send)
        Me.Controls.Add(Me.lblBody)
        Me.Controls.Add(Me.lblSubject)
        Me.Controls.Add(Me.lblTo)
        Me.Controls.Add(Me.lblFrom)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Attachments)
        Me.Controls.Add(Me.Browse)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "MainForm"
        Me.Text = "Send Email Sample"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Attachments As System.Windows.Forms.ListBox
    Friend WithEvents Browse As System.Windows.Forms.Button
    Friend WithEvents Priority As System.Windows.Forms.ComboBox
    Friend WithEvents BCC As System.Windows.Forms.TextBox
    Friend WithEvents CC As System.Windows.Forms.TextBox
    Friend WithEvents lblBCC As System.Windows.Forms.Label
    Friend WithEvents Body As System.Windows.Forms.TextBox
    Friend WithEvents Subject As System.Windows.Forms.TextBox
    Friend WithEvents lblCC As System.Windows.Forms.Label
    Friend WithEvents From As System.Windows.Forms.TextBox
    Friend WithEvents ToAddress As System.Windows.Forms.TextBox
    Friend WithEvents Send As System.Windows.Forms.Button
    Friend WithEvents lblBody As System.Windows.Forms.Label
    Friend WithEvents lblSubject As System.Windows.Forms.Label
    Friend WithEvents lblTo As System.Windows.Forms.Label
    Friend WithEvents lblFrom As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Main As System.Windows.Forms.MainMenu
    Friend WithEvents File As System.Windows.Forms.MenuItem
    Friend WithEvents ExitMenu As System.Windows.Forms.MenuItem
    Friend WithEvents odlgAttachment As System.Windows.Forms.OpenFileDialog
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
