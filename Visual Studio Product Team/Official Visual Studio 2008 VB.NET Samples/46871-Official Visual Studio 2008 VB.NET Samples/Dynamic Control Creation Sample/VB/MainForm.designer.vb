Partial Public Class MainForm
    Inherits System.Windows.Forms.Form

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
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
    <System.Diagnostics.DebuggerNonUserCode()> Private Sub InitializeComponent()
        Me.Label4 = New System.Windows.Forms.Label
        Me.btnAddButton = New System.Windows.Forms.Button
        Me.btnClearControls = New System.Windows.Forms.Button
        Me.btnTightlyBoundControls = New System.Windows.Forms.Button
        Me.btnCreateSurvey = New System.Windows.Forms.Button
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label4
        '
        Me.Label4.AccessibleDescription = "Label for Create Survey Form button."
        Me.Label4.AccessibleName = "Create Survey Form Label."
        Me.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label4.Location = New System.Drawing.Point(13, 267)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(200, 48)
        Me.Label4.TabIndex = 26
        Me.Label4.Text = "Press this button to create a new Survey based on an XML document."
        '
        'btnAddButton
        '
        Me.btnAddButton.AccessibleDescription = "Button executed to create a button."
        Me.btnAddButton.AccessibleName = "Add Button Button"
        Me.btnAddButton.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnAddButton.Location = New System.Drawing.Point(13, 147)
        Me.btnAddButton.Name = "btnAddButton"
        Me.btnAddButton.Size = New System.Drawing.Size(200, 23)
        Me.btnAddButton.TabIndex = 23
        Me.btnAddButton.Text = "&Add Button"
        '
        'btnClearControls
        '
        Me.btnClearControls.AccessibleDescription = "Button executed to reset form to original state."
        Me.btnClearControls.AccessibleName = "Clear Controls Button"
        Me.btnClearControls.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnClearControls.Location = New System.Drawing.Point(13, 227)
        Me.btnClearControls.Name = "btnClearControls"
        Me.btnClearControls.Size = New System.Drawing.Size(200, 23)
        Me.btnClearControls.TabIndex = 25
        Me.btnClearControls.Text = "&Clear Controls"
        '
        'btnTightlyBoundControls
        '
        Me.btnTightlyBoundControls.AccessibleDescription = "Button executed to create tightly bound controls."
        Me.btnTightlyBoundControls.AccessibleName = "Tightly Coupled Controls Button"
        Me.btnTightlyBoundControls.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnTightlyBoundControls.Location = New System.Drawing.Point(13, 83)
        Me.btnTightlyBoundControls.Name = "btnTightlyBoundControls"
        Me.btnTightlyBoundControls.Size = New System.Drawing.Size(200, 23)
        Me.btnTightlyBoundControls.TabIndex = 21
        Me.btnTightlyBoundControls.Text = "&Tightly Bound Controls"
        '
        'btnCreateSurvey
        '
        Me.btnCreateSurvey.AccessibleDescription = "Button executed to create a new Survey form."
        Me.btnCreateSurvey.AccessibleName = "Create Survey Form Button"
        Me.btnCreateSurvey.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnCreateSurvey.Location = New System.Drawing.Point(12, 301)
        Me.btnCreateSurvey.Name = "btnCreateSurvey"
        Me.btnCreateSurvey.Size = New System.Drawing.Size(200, 23)
        Me.btnCreateSurvey.TabIndex = 27
        Me.btnCreateSurvey.Text = "Create &Survey Form"
        '
        'Label3
        '
        Me.Label3.AccessibleDescription = "Label for Clear Controls button."
        Me.Label3.AccessibleName = "Clear Controls Label."
        Me.Label3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label3.Location = New System.Drawing.Point(13, 187)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(208, 32)
        Me.Label3.TabIndex = 24
        Me.Label3.Text = "Press this button to remove the new controls."
        '
        'Label2
        '
        Me.Label2.AccessibleDescription = "Label for Add button Control button."
        Me.Label2.AccessibleName = "Add Button Label."
        Me.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label2.Location = New System.Drawing.Point(13, 123)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(208, 24)
        Me.Label2.TabIndex = 22
        Me.Label2.Text = "Press this button to add a new button."
        '
        'Label1
        '
        Me.Label1.AccessibleDescription = "Label for Tightly Bound Controls button."
        Me.Label1.AccessibleName = "Tightly Bound Controls Label."
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(13, 43)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(200, 40)
        Me.Label1.TabIndex = 20
        Me.Label1.Text = "Press this button to create a pair of tightly bound controls."
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(502, 24)
        Me.MenuStrip1.TabIndex = 28
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
        Me.ClientSize = New System.Drawing.Size(502, 334)
        Me.Controls.Add(Me.btnAddButton)
        Me.Controls.Add(Me.btnClearControls)
        Me.Controls.Add(Me.btnTightlyBoundControls)
        Me.Controls.Add(Me.btnCreateSurvey)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Name = "MainForm"
        Me.Text = "Dynamic Controls Sample"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents btnAddButton As System.Windows.Forms.Button
    Friend WithEvents btnClearControls As System.Windows.Forms.Button
    Friend WithEvents btnTightlyBoundControls As System.Windows.Forms.Button
    Friend WithEvents btnCreateSurvey As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
