' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
'
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SampleForm
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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SampleForm))
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.samplesTreeView = New System.Windows.Forms.TreeView
        Me.imageList = New System.Windows.Forms.ImageList(Me.components)
        Me.samplesLabel = New System.Windows.Forms.Label
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer
        Me.descriptionTextBox = New System.Windows.Forms.TextBox
        Me.descriptionLabel = New System.Windows.Forms.Label
        Me.Button1 = New System.Windows.Forms.Button
        Me.codeRichTextBox = New System.Windows.Forms.RichTextBox
        Me.codeLabel = New System.Windows.Forms.Label
        Me.runButton = New System.Windows.Forms.Button
        Me.outputTextBox = New System.Windows.Forms.TextBox
        Me.outputLabel = New System.Windows.Forms.Label
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange
        Me.SplitContainer1.Cursor = System.Windows.Forms.Cursors.Default
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SplitContainer1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.samplesTreeView)
        Me.SplitContainer1.Panel1.Controls.Add(Me.samplesLabel)
        Me.SplitContainer1.Panel1.Cursor = System.Windows.Forms.Cursors.Default
        Me.SplitContainer1.Panel1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SplitContainer1.Panel1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.SplitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer2)
        Me.SplitContainer1.Panel2.Cursor = System.Windows.Forms.Cursors.Default
        Me.SplitContainer1.Panel2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SplitContainer1.Panel2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.SplitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.SplitContainer1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.SplitContainer1.Size = New System.Drawing.Size(858, 744)
        Me.SplitContainer1.SplitterDistance = 286
        Me.SplitContainer1.TabIndex = 0
        '
        'samplesTreeView
        '
        Me.samplesTreeView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.samplesTreeView.Cursor = System.Windows.Forms.Cursors.Default
        Me.samplesTreeView.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.samplesTreeView.HideSelection = False
        Me.samplesTreeView.ImageKey = "Item"
        Me.samplesTreeView.ImageList = Me.imageList
        Me.samplesTreeView.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.samplesTreeView.Location = New System.Drawing.Point(6, 25)
        Me.samplesTreeView.Name = "samplesTreeView"
        Me.samplesTreeView.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.samplesTreeView.SelectedImageKey = "Item"
        Me.samplesTreeView.ShowNodeToolTips = True
        Me.samplesTreeView.ShowRootLines = False
        Me.samplesTreeView.Size = New System.Drawing.Size(277, 716)
        Me.samplesTreeView.TabIndex = 3
        '
        'imageList
        '
        Me.imageList.ImageStream = CType(resources.GetObject("imageList.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.imageList.TransparentColor = System.Drawing.Color.Transparent
        Me.imageList.Images.SetKeyName(0, "Unknown")
        Me.imageList.Images.SetKeyName(1, "BookStack")
        Me.imageList.Images.SetKeyName(2, "BookClose")
        Me.imageList.Images.SetKeyName(3, "BookOpen")
        Me.imageList.Images.SetKeyName(4, "Help")
        Me.imageList.Images.SetKeyName(5, "Run")
        '
        'samplesLabel
        '
        Me.samplesLabel.AutoSize = True
        Me.samplesLabel.Cursor = System.Windows.Forms.Cursors.Default
        Me.samplesLabel.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.samplesLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.samplesLabel.Location = New System.Drawing.Point(3, 6)
        Me.samplesLabel.Name = "samplesLabel"
        Me.samplesLabel.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.samplesLabel.Size = New System.Drawing.Size(62, 16)
        Me.samplesLabel.TabIndex = 2
        Me.samplesLabel.Text = "Samples:"
        '
        'SplitContainer2
        '
        Me.SplitContainer2.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange
        Me.SplitContainer2.Cursor = System.Windows.Forms.Cursors.Default
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SplitContainer2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.SplitContainer3)
        Me.SplitContainer2.Panel1.Cursor = System.Windows.Forms.Cursors.Default
        Me.SplitContainer2.Panel1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SplitContainer2.Panel1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.SplitContainer2.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.runButton)
        Me.SplitContainer2.Panel2.Controls.Add(Me.outputTextBox)
        Me.SplitContainer2.Panel2.Controls.Add(Me.outputLabel)
        Me.SplitContainer2.Panel2.Cursor = System.Windows.Forms.Cursors.Default
        Me.SplitContainer2.Panel2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SplitContainer2.Panel2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.SplitContainer2.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.SplitContainer2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.SplitContainer2.Size = New System.Drawing.Size(568, 744)
        Me.SplitContainer2.SplitterDistance = 365
        Me.SplitContainer2.TabIndex = 0
        '
        'SplitContainer3
        '
        Me.SplitContainer3.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange
        Me.SplitContainer3.Cursor = System.Windows.Forms.Cursors.Default
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SplitContainer3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer3.Name = "SplitContainer3"
        Me.SplitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.descriptionTextBox)
        Me.SplitContainer3.Panel1.Controls.Add(Me.descriptionLabel)
        Me.SplitContainer3.Panel1.Cursor = System.Windows.Forms.Cursors.Default
        Me.SplitContainer3.Panel1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SplitContainer3.Panel1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.SplitContainer3.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.Button1)
        Me.SplitContainer3.Panel2.Controls.Add(Me.codeRichTextBox)
        Me.SplitContainer3.Panel2.Controls.Add(Me.codeLabel)
        Me.SplitContainer3.Panel2.Cursor = System.Windows.Forms.Cursors.Default
        Me.SplitContainer3.Panel2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.SplitContainer3.Panel2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.SplitContainer3.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.SplitContainer3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.SplitContainer3.Size = New System.Drawing.Size(568, 365)
        Me.SplitContainer3.SplitterDistance = 80
        Me.SplitContainer3.TabIndex = 0
        '
        'descriptionTextBox
        '
        Me.descriptionTextBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.descriptionTextBox.BackColor = System.Drawing.SystemColors.ControlLight
        Me.descriptionTextBox.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.descriptionTextBox.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.descriptionTextBox.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.descriptionTextBox.Location = New System.Drawing.Point(7, 25)
        Me.descriptionTextBox.Multiline = True
        Me.descriptionTextBox.Name = "descriptionTextBox"
        Me.descriptionTextBox.ReadOnly = True
        Me.descriptionTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.descriptionTextBox.Size = New System.Drawing.Size(558, 52)
        Me.descriptionTextBox.TabIndex = 3
        '
        'descriptionLabel
        '
        Me.descriptionLabel.AutoSize = True
        Me.descriptionLabel.Cursor = System.Windows.Forms.Cursors.Default
        Me.descriptionLabel.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.descriptionLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.descriptionLabel.Location = New System.Drawing.Point(2, 9)
        Me.descriptionLabel.Name = "descriptionLabel"
        Me.descriptionLabel.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.descriptionLabel.Size = New System.Drawing.Size(76, 16)
        Me.descriptionLabel.TabIndex = 2
        Me.descriptionLabel.Text = "Description:"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(481, -1)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 4
        Me.Button1.Text = "Copy"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'codeRichTextBox
        '
        Me.codeRichTextBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.codeRichTextBox.BackColor = System.Drawing.SystemColors.ControlLight
        Me.codeRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.codeRichTextBox.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.codeRichTextBox.Font = New System.Drawing.Font("Lucida Console", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.codeRichTextBox.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.codeRichTextBox.Location = New System.Drawing.Point(8, 24)
        Me.codeRichTextBox.Name = "codeRichTextBox"
        Me.codeRichTextBox.ReadOnly = True
        Me.codeRichTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.codeRichTextBox.Size = New System.Drawing.Size(557, 256)
        Me.codeRichTextBox.TabIndex = 3
        Me.codeRichTextBox.Text = ""
        Me.codeRichTextBox.WordWrap = False
        '
        'codeLabel
        '
        Me.codeLabel.AutoSize = True
        Me.codeLabel.Cursor = System.Windows.Forms.Cursors.Default
        Me.codeLabel.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.codeLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.codeLabel.Location = New System.Drawing.Point(5, 5)
        Me.codeLabel.Name = "codeLabel"
        Me.codeLabel.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.codeLabel.Size = New System.Drawing.Size(42, 16)
        Me.codeLabel.TabIndex = 2
        Me.codeLabel.Text = "Code:"
        '
        'runButton
        '
        Me.runButton.Cursor = System.Windows.Forms.Cursors.Default
        Me.runButton.Enabled = False
        Me.runButton.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.runButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.runButton.ImageIndex = 5
        Me.runButton.ImageList = Me.imageList
        Me.runButton.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.runButton.Location = New System.Drawing.Point(8, 3)
        Me.runButton.Name = "runButton"
        Me.runButton.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.runButton.Size = New System.Drawing.Size(119, 27)
        Me.runButton.TabIndex = 3
        Me.runButton.Text = " Run Sample!"
        Me.runButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        '
        'outputTextBox
        '
        Me.outputTextBox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.outputTextBox.BackColor = System.Drawing.SystemColors.ControlLight
        Me.outputTextBox.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.outputTextBox.Font = New System.Drawing.Font("Lucida Console", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.outputTextBox.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.outputTextBox.Location = New System.Drawing.Point(8, 52)
        Me.outputTextBox.Multiline = True
        Me.outputTextBox.Name = "outputTextBox"
        Me.outputTextBox.ReadOnly = True
        Me.outputTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.outputTextBox.Size = New System.Drawing.Size(557, 320)
        Me.outputTextBox.TabIndex = 5
        Me.outputTextBox.WordWrap = False
        '
        'outputLabel
        '
        Me.outputLabel.AutoSize = True
        Me.outputLabel.Cursor = System.Windows.Forms.Cursors.Default
        Me.outputLabel.Font = New System.Drawing.Font("Tahoma", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.outputLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.outputLabel.Location = New System.Drawing.Point(5, 33)
        Me.outputLabel.Name = "outputLabel"
        Me.outputLabel.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.outputLabel.Size = New System.Drawing.Size(51, 16)
        Me.outputLabel.TabIndex = 4
        Me.outputLabel.Text = "Output:"
        '
        'SampleForm
        '
        Me.AcceptButton = Me.runButton
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(858, 744)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "SampleForm"
        Me.Text = "Samples"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        Me.SplitContainer2.Panel2.PerformLayout()
        Me.SplitContainer2.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel1.PerformLayout()
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        Me.SplitContainer3.Panel2.PerformLayout()
        Me.SplitContainer3.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
    Friend WithEvents SplitContainer3 As System.Windows.Forms.SplitContainer
    Private WithEvents samplesTreeView As System.Windows.Forms.TreeView
    Private WithEvents samplesLabel As System.Windows.Forms.Label
    Private WithEvents descriptionTextBox As System.Windows.Forms.TextBox
    Private WithEvents descriptionLabel As System.Windows.Forms.Label
    Private WithEvents codeRichTextBox As System.Windows.Forms.RichTextBox
    Private WithEvents codeLabel As System.Windows.Forms.Label
    Private WithEvents runButton As System.Windows.Forms.Button
    Private WithEvents outputTextBox As System.Windows.Forms.TextBox
    Private WithEvents outputLabel As System.Windows.Forms.Label
    Private WithEvents imageList As System.Windows.Forms.ImageList
    Friend WithEvents Button1 As System.Windows.Forms.Button
End Class
