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
        Me.Label1 = New System.Windows.Forms.Label
        Me.ContentTextbox = New System.Windows.Forms.TextBox
        Me.CurrentDeploymentInfoButton = New System.Windows.Forms.Button
        Me.UpdateInfoButton = New System.Windows.Forms.Button
        Me.DownloadButton = New System.Windows.Forms.Button
        Me.UpdateButton = New System.Windows.Forms.Button
        Me.ViewSourceCodeToolStripButton = New System.Windows.Forms.Button
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 51)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(59, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Description"
        '
        'ContentTextbox
        '
        Me.ContentTextbox.Location = New System.Drawing.Point(12, 68)
        Me.ContentTextbox.Multiline = True
        Me.ContentTextbox.Name = "ContentTextbox"
        Me.ContentTextbox.Size = New System.Drawing.Size(216, 73)
        Me.ContentTextbox.TabIndex = 1
        '
        'CurrentDeploymentInfoButton
        '
        Me.CurrentDeploymentInfoButton.Location = New System.Drawing.Point(12, 147)
        Me.CurrentDeploymentInfoButton.Name = "CurrentDeploymentInfoButton"
        Me.CurrentDeploymentInfoButton.Size = New System.Drawing.Size(105, 30)
        Me.CurrentDeploymentInfoButton.TabIndex = 2
        Me.CurrentDeploymentInfoButton.Text = "Deployment Info"
        '
        'UpdateInfoButton
        '
        Me.UpdateInfoButton.Location = New System.Drawing.Point(123, 147)
        Me.UpdateInfoButton.Name = "UpdateInfoButton"
        Me.UpdateInfoButton.Size = New System.Drawing.Size(105, 30)
        Me.UpdateInfoButton.TabIndex = 3
        Me.UpdateInfoButton.Text = "Update Info"
        '
        'DownloadButton
        '
        Me.DownloadButton.Location = New System.Drawing.Point(12, 183)
        Me.DownloadButton.Name = "DownloadButton"
        Me.DownloadButton.Size = New System.Drawing.Size(105, 30)
        Me.DownloadButton.TabIndex = 4
        Me.DownloadButton.Text = "Download"
        '
        'UpdateButton
        '
        Me.UpdateButton.Location = New System.Drawing.Point(123, 183)
        Me.UpdateButton.Name = "UpdateButton"
        Me.UpdateButton.Size = New System.Drawing.Size(105, 30)
        Me.UpdateButton.TabIndex = 5
        Me.UpdateButton.Text = "Update"
        '
        'ViewSourceCodeToolStripButton
        '
        Me.ViewSourceCodeToolStripButton.Location = New System.Drawing.Point(72, 219)
        Me.ViewSourceCodeToolStripButton.Name = "ViewSourceCodeToolStripButton"
        Me.ViewSourceCodeToolStripButton.Size = New System.Drawing.Size(105, 30)
        Me.ViewSourceCodeToolStripButton.TabIndex = 6
        Me.ViewSourceCodeToolStripButton.Text = "View Source Code"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(242, 26)
        Me.MenuStrip1.TabIndex = 7
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'fileToolStripMenuItem
        '
        Me.fileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.exitToolStripMenuItem})
        Me.fileToolStripMenuItem.Name = "fileToolStripMenuItem"
        Me.fileToolStripMenuItem.Text = "&File"
        '
        'exitToolStripMenuItem
        '
        Me.exitToolStripMenuItem.Name = "exitToolStripMenuItem"
        Me.exitToolStripMenuItem.Text = "E&xit"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(242, 269)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.ViewSourceCodeToolStripButton)
        Me.Controls.Add(Me.UpdateButton)
        Me.Controls.Add(Me.DownloadButton)
        Me.Controls.Add(Me.UpdateInfoButton)
        Me.Controls.Add(Me.CurrentDeploymentInfoButton)
        Me.Controls.Add(Me.ContentTextbox)
        Me.Controls.Add(Me.Label1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "MainForm"
        Me.Text = "MainForm"
        Me.MenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ContentTextbox As System.Windows.Forms.TextBox
    Friend WithEvents CurrentDeploymentInfoButton As System.Windows.Forms.Button
    Friend WithEvents UpdateInfoButton As System.Windows.Forms.Button
    Friend WithEvents DownloadButton As System.Windows.Forms.Button
    Friend WithEvents UpdateButton As System.Windows.Forms.Button
    Friend WithEvents ViewSourceCodeToolStripButton As System.Windows.Forms.Button
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
