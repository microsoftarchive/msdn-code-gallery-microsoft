'****************************** Module Header ******************************\
'Module Name:    Form1.Designer.vb
'Project:        VBColorToGrayScale2010
'Copyright (c) Microsoft Corporation

'The project illustrates how to convert a colored image to gray scale.

'This source is subject to the Microsoft Public License.
'See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
'All other rights reserved.

'*****************************************************************************/

Imports System
Imports System.Windows.Forms
Imports System.Drawing

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits Form

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
        Me.SuspendLayout()
        '
        'Form1
        '
        Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(292, 273)
        Me.Name = "Form1"
        Me.Text = "Form1"

        Dim resources As New System.ComponentModel.ComponentResourceManager(GetType(Form1))

        'Initialize custom controls
        Me.mnuActionMenu = New MenuStrip()
        Me.mnuChooseFile = New ToolStripMenuItem()
        Me.mnuConvert = New ToolStripMenuItem()
        Me.mnuSaveOutput = New ToolStripMenuItem()
        Me.mnuReset = New ToolStripMenuItem()

        ' 
        ' mnuActionMenu
        ' 
        Me.mnuActionMenu.Items.AddRange(New ToolStripItem() {Me.mnuChooseFile, Me.mnuConvert, Me.mnuSaveOutput, Me.mnuReset})
        Me.mnuActionMenu.Location = New Point(0, 0)
        Me.mnuActionMenu.Name = "mnuActionMenu"
        Me.mnuActionMenu.Size = New Size(302, 24)
        Me.mnuActionMenu.TabIndex = 0
        Me.mnuActionMenu.Text = "menuStrip1"
        ' 
        ' mnuChooseFile
        ' 
        Me.mnuChooseFile.Name = "mnuChooseFile"
        Me.mnuChooseFile.ShortcutKeys = Keys.Alt Or Keys.F
        Me.mnuChooseFile.Size = New Size(89, 20)
        Me.mnuChooseFile.Text = "Choose File..."
        ' 
        ' mnuConvert
        ' 
        Me.mnuConvert.Name = "mnuConvert"
        Me.mnuConvert.ShortcutKeys = Keys.Alt Or Keys.C
        Me.mnuConvert.Size = New Size(61, 20)
        Me.mnuConvert.Text = "Convert"
        ' 
        ' mnuSaveOutput
        ' 
        Me.mnuSaveOutput.Name = "mnuSaveOutput"
        Me.mnuSaveOutput.ShortcutKeys = Keys.Alt Or Keys.S
        Me.mnuSaveOutput.Size = New Size(84, 20)
        Me.mnuSaveOutput.Text = "Save Output"
        ' 
        ' mnuReset
        ' 
        Me.mnuReset.Name = "mnuReset"
        Me.mnuReset.ShortcutKeys = Keys.Alt Or Keys.R
        Me.mnuReset.Size = New Size(47, 20)
        Me.mnuReset.Text = "Reset"
        ' 
        ' GrayScaleConverterForm
        ' 
        Me.AutoScaleDimensions = New SizeF(6.0F, 13.0F)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(302, 261)
        Me.Controls.Add(Me.mnuActionMenu)
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), Icon)

        Me.MainMenuStrip = Me.mnuActionMenu
        Me.Name = "GrayScaleConverterForm"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Text = "Image Converter"
        Me.WindowState = FormWindowState.Maximized

        Me.mnuActionMenu.ResumeLayout(False)
        Me.mnuActionMenu.PerformLayout()


        Me.ResumeLayout(False)

    End Sub

    'controls required in application
    Friend WithEvents mnuActionMenu As MenuStrip
    Friend WithEvents mnuChooseFile As ToolStripMenuItem
    Friend WithEvents mnuConvert As ToolStripMenuItem
    Friend WithEvents mnuSaveOutput As ToolStripMenuItem
    Friend WithEvents mnuReset As ToolStripMenuItem

End Class
