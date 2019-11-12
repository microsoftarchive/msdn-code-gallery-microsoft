'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************


Imports Microsoft.VisualBasic
Imports System
Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider
	Public Partial Class DlgQuerySaveCheckedInFile
		''' <summary>
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.IContainer = Nothing

		''' <summary>
		''' Clean up any resources being used.
		''' </summary>
		''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
			MyBase.Dispose(disposing)
		End Sub

		#Region "Windows Form Designer generated code"

		''' <summary>
		''' Required method for Designer support - do not modify
		''' the contents of this method with the code editor.
		''' </summary>
        Private Sub InitializeComponent()
            Me.msgText = New System.Windows.Forms.Label()
            Me.btnCancel = New System.Windows.Forms.Button()
            Me.btnCheckout = New System.Windows.Forms.Button()
            Me.btnSaveAs = New System.Windows.Forms.Button()
            Me.btnSkipSave = New System.Windows.Forms.Button()
            Me.SuspendLayout()
            ' 
            ' msgText
            ' 
            Me.msgText.Location = New System.Drawing.Point(-1, 9)
            Me.msgText.Name = "msgText"
            Me.msgText.Size = New System.Drawing.Size(517, 51)
            Me.msgText.TabIndex = 1
            Me.msgText.Text = "The read only file {0} is under source control and checked in.What do you want to" & " do?"
            ' 
            ' btnCancel
            ' 
            Me.btnCancel.Location = New System.Drawing.Point(482, 72)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(73, 23)
            Me.btnCancel.TabIndex = 5
            Me.btnCancel.Text = "Cancel"
            Me.btnCancel.UseVisualStyleBackColor = True
            AddHandler Me.btnCancel.Click, New System.EventHandler(AddressOf Me.btnCancel_Click)
            '
            ' 
            ' btnCheckout
            ' 
            Me.btnCheckout.Location = New System.Drawing.Point(11, 72)
            Me.btnCheckout.Name = "btnCheckout"
            Me.btnCheckout.Size = New System.Drawing.Size(150, 23)
            Me.btnCheckout.TabIndex = 4
            Me.btnCheckout.Text = "Checkout the file and save it"
            Me.btnCheckout.UseVisualStyleBackColor = True
            AddHandler Me.btnCheckout.Click, New System.EventHandler(AddressOf Me.btnCheckout_Click)
            ' 
            ' btnSaveAs
            ' 
            Me.btnSaveAs.Location = New System.Drawing.Point(297, 72)
            Me.btnSaveAs.Name = "btnSaveAs"
            Me.btnSaveAs.Size = New System.Drawing.Size(174, 23)
            Me.btnSaveAs.TabIndex = 6
            Me.btnSaveAs.Text = "Save the file with different name"
            Me.btnSaveAs.UseVisualStyleBackColor = True
            AddHandler Me.btnSaveAs.Click, New System.EventHandler(AddressOf Me.btnSaveAs_Click)
            ' 
            ' btnSkipSave
            ' 
            Me.btnSkipSave.Location = New System.Drawing.Point(172, 72)
            Me.btnSkipSave.Name = "btnSkipSave"
            Me.btnSkipSave.Size = New System.Drawing.Size(114, 23)
            Me.btnSkipSave.TabIndex = 7
            Me.btnSkipSave.Text = "Do not save this file"
            Me.btnSkipSave.UseVisualStyleBackColor = True
            AddHandler Me.btnSkipSave.Click, New System.EventHandler(AddressOf Me.btnSkipSave_Click)
            ' 
            ' DlgQuerySaveCheckedInFile
            ' 
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0F, 13.0F)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(567, 108)
            Me.Controls.Add(Me.btnSkipSave)
            Me.Controls.Add(Me.btnSaveAs)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.btnCheckout)
            Me.Controls.Add(Me.msgText)
            Me.Name = "DlgQuerySaveCheckedInFile"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Save of read only file"
            Me.ResumeLayout(False)
        End Sub

		#End Region

		Private msgText As System.Windows.Forms.Label
		Private WithEvents btnCancel As System.Windows.Forms.Button
		Private WithEvents btnCheckout As System.Windows.Forms.Button
		Private WithEvents btnSaveAs As System.Windows.Forms.Button
		Private WithEvents btnSkipSave As System.Windows.Forms.Button
	End Class
End Namespace