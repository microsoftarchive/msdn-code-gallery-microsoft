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
        Me.tabOptions = New System.Windows.Forms.TabControl
        Me.pgeOpenSaveFile = New System.Windows.Forms.TabPage
        Me.txtDirectory = New System.Windows.Forms.TextBox
        Me.txtFileContents = New System.Windows.Forms.TextBox
        Me.btnSaveTextFile = New System.Windows.Forms.Button
        Me.openTextFile = New System.Windows.Forms.Button
        Me.btnSelectColor = New System.Windows.Forms.Button
        Me.btnSelectFont = New System.Windows.Forms.Button
        Me.btnBrowseFolders = New System.Windows.Forms.Button
        Me.pgeOpenMultipleFiles = New System.Windows.Forms.TabPage
        Me.lstFiles = New System.Windows.Forms.ListBox
        Me.btnRetriveFileNames = New System.Windows.Forms.Button
        Me.odlgTextFile = New System.Windows.Forms.OpenFileDialog
        Me.odlgFileNames = New System.Windows.Forms.OpenFileDialog
        Me.sdlgTextFile = New System.Windows.Forms.SaveFileDialog
        Me.fldlgList = New System.Windows.Forms.FolderBrowserDialog
        Me.fdlgText = New System.Windows.Forms.FontDialog
        Me.cdlgText = New System.Windows.Forms.ColorDialog
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.tabOptions.SuspendLayout()
        Me.pgeOpenSaveFile.SuspendLayout()
        Me.pgeOpenMultipleFiles.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabOptions
        '
        Me.tabOptions.Controls.Add(Me.pgeOpenSaveFile)
        Me.tabOptions.Controls.Add(Me.pgeOpenMultipleFiles)
        Me.tabOptions.Location = New System.Drawing.Point(9, 48)
        Me.tabOptions.Name = "tabOptions"
        Me.tabOptions.SelectedIndex = 0
        Me.tabOptions.Size = New System.Drawing.Size(687, 426)
        Me.tabOptions.TabIndex = 0
        '
        'pgeOpenSaveFile
        '
        Me.pgeOpenSaveFile.Controls.Add(Me.txtDirectory)
        Me.pgeOpenSaveFile.Controls.Add(Me.txtFileContents)
        Me.pgeOpenSaveFile.Controls.Add(Me.btnSaveTextFile)
        Me.pgeOpenSaveFile.Controls.Add(Me.openTextFile)
        Me.pgeOpenSaveFile.Controls.Add(Me.btnSelectColor)
        Me.pgeOpenSaveFile.Controls.Add(Me.btnSelectFont)
        Me.pgeOpenSaveFile.Controls.Add(Me.btnBrowseFolders)
        Me.pgeOpenSaveFile.Location = New System.Drawing.Point(4, 22)
        Me.pgeOpenSaveFile.Name = "pgeOpenSaveFile"
        Me.pgeOpenSaveFile.Padding = New System.Windows.Forms.Padding(3)
        Me.pgeOpenSaveFile.Size = New System.Drawing.Size(679, 400)
        Me.pgeOpenSaveFile.TabIndex = 0
        Me.pgeOpenSaveFile.Text = "Open/SaveFileDialog"
        Me.pgeOpenSaveFile.UseVisualStyleBackColor = False
        '
        'txtDirectory
        '
        Me.txtDirectory.Location = New System.Drawing.Point(258, 328)
        Me.txtDirectory.Multiline = True
        Me.txtDirectory.Name = "txtDirectory"
        Me.txtDirectory.Size = New System.Drawing.Size(400, 50)
        Me.txtDirectory.TabIndex = 6
        '
        'txtFileContents
        '
        Me.txtFileContents.Location = New System.Drawing.Point(258, 20)
        Me.txtFileContents.Multiline = True
        Me.txtFileContents.Name = "txtFileContents"
        Me.txtFileContents.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtFileContents.Size = New System.Drawing.Size(400, 281)
        Me.txtFileContents.TabIndex = 5
        '
        'btnSaveTextFile
        '
        Me.btnSaveTextFile.Location = New System.Drawing.Point(35, 97)
        Me.btnSaveTextFile.Name = "btnSaveTextFile"
        Me.btnSaveTextFile.Size = New System.Drawing.Size(175, 50)
        Me.btnSaveTextFile.TabIndex = 1
        Me.btnSaveTextFile.Text = "&Save a File"
        '
        'openTextFile
        '
        Me.openTextFile.Location = New System.Drawing.Point(35, 20)
        Me.openTextFile.Name = "openTextFile"
        Me.openTextFile.Size = New System.Drawing.Size(175, 50)
        Me.openTextFile.TabIndex = 0
        Me.openTextFile.Text = "&Open a File"
        '
        'btnSelectColor
        '
        Me.btnSelectColor.Location = New System.Drawing.Point(35, 174)
        Me.btnSelectColor.Name = "btnSelectColor"
        Me.btnSelectColor.Size = New System.Drawing.Size(175, 50)
        Me.btnSelectColor.TabIndex = 2
        Me.btnSelectColor.Text = "Select a &Color"
        '
        'btnSelectFont
        '
        Me.btnSelectFont.Location = New System.Drawing.Point(35, 251)
        Me.btnSelectFont.Name = "btnSelectFont"
        Me.btnSelectFont.Size = New System.Drawing.Size(175, 50)
        Me.btnSelectFont.TabIndex = 3
        Me.btnSelectFont.Text = "S&elect a Font"
        '
        'btnBrowseFolders
        '
        Me.btnBrowseFolders.Location = New System.Drawing.Point(35, 328)
        Me.btnBrowseFolders.Name = "btnBrowseFolders"
        Me.btnBrowseFolders.Size = New System.Drawing.Size(175, 50)
        Me.btnBrowseFolders.TabIndex = 4
        Me.btnBrowseFolders.Text = "&Browse Folders"
        '
        'pgeOpenMultipleFiles
        '
        Me.pgeOpenMultipleFiles.Controls.Add(Me.lstFiles)
        Me.pgeOpenMultipleFiles.Controls.Add(Me.btnRetriveFileNames)
        Me.pgeOpenMultipleFiles.Location = New System.Drawing.Point(4, 22)
        Me.pgeOpenMultipleFiles.Name = "pgeOpenMultipleFiles"
        Me.pgeOpenMultipleFiles.Padding = New System.Windows.Forms.Padding(3)
        Me.pgeOpenMultipleFiles.Size = New System.Drawing.Size(679, 400)
        Me.pgeOpenMultipleFiles.TabIndex = 1
        Me.pgeOpenMultipleFiles.Text = "Select Multiple Files"
        Me.pgeOpenMultipleFiles.UseVisualStyleBackColor = False
        '
        'lstFiles
        '
        Me.lstFiles.FormattingEnabled = True
        Me.lstFiles.Location = New System.Drawing.Point(238, 13)
        Me.lstFiles.Name = "lstFiles"
        Me.lstFiles.Size = New System.Drawing.Size(420, 355)
        Me.lstFiles.TabIndex = 1
        '
        'btnRetriveFileNames
        '
        Me.btnRetriveFileNames.Location = New System.Drawing.Point(35, 20)
        Me.btnRetriveFileNames.Name = "btnRetriveFileNames"
        Me.btnRetriveFileNames.Size = New System.Drawing.Size(175, 50)
        Me.btnRetriveFileNames.TabIndex = 0
        Me.btnRetriveFileNames.Text = "&Retrieve File Names"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(718, 24)
        Me.MenuStrip1.TabIndex = 1
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
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(718, 505)
        Me.Controls.Add(Me.tabOptions)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Name = "MainForm"
        Me.Text = "Common Dialogs Sample"
        Me.tabOptions.ResumeLayout(False)
        Me.pgeOpenSaveFile.ResumeLayout(False)
        Me.pgeOpenSaveFile.PerformLayout()
        Me.pgeOpenMultipleFiles.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend Shared ReadOnly Property GetInstance() As MainForm
        Get
            If m_DefaultInstance Is Nothing OrElse m_DefaultInstance.IsDisposed() Then
                SyncLock m_SyncObject
                    If m_DefaultInstance Is Nothing OrElse m_DefaultInstance.IsDisposed() Then
                        m_DefaultInstance = New MainForm
                    End If
                End SyncLock
            End If
            Return m_DefaultInstance
        End Get
    End Property

    Private Shared m_DefaultInstance As MainForm
    Private Shared m_SyncObject As New Object
    Friend WithEvents tabOptions As System.Windows.Forms.TabControl
    Friend WithEvents pgeOpenSaveFile As System.Windows.Forms.TabPage
    Friend WithEvents pgeOpenMultipleFiles As System.Windows.Forms.TabPage
    Friend WithEvents btnBrowseFolders As System.Windows.Forms.Button
    Friend WithEvents btnSelectFont As System.Windows.Forms.Button
    Friend WithEvents btnSelectColor As System.Windows.Forms.Button
    Friend WithEvents btnSaveTextFile As System.Windows.Forms.Button
    Friend WithEvents openTextFile As System.Windows.Forms.Button
    Friend WithEvents txtFileContents As System.Windows.Forms.TextBox
    Friend WithEvents btnRetriveFileNames As System.Windows.Forms.Button
    Friend WithEvents lstFiles As System.Windows.Forms.ListBox
    Friend WithEvents odlgTextFile As System.Windows.Forms.OpenFileDialog
    Friend WithEvents odlgFileNames As System.Windows.Forms.OpenFileDialog
    Friend WithEvents sdlgTextFile As System.Windows.Forms.SaveFileDialog
    Friend WithEvents fldlgList As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents fdlgText As System.Windows.Forms.FontDialog
    Friend WithEvents cdlgText As System.Windows.Forms.ColorDialog
    Friend WithEvents txtDirectory As System.Windows.Forms.TextBox
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
