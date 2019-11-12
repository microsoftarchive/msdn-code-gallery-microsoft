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
	Public Partial Class DlgQueryEditCheckedInFile
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
            Me.btnCheckout = New System.Windows.Forms.Button()
            Me.btnEdit = New System.Windows.Forms.Button()
            Me.btnCancel = New System.Windows.Forms.Button()
            Me.SuspendLayout()

            ' 
            ' msgText
            ' 
            Me.msgText.Location = New System.Drawing.Point(4, 13)
            Me.msgText.Name = "msgText"
            Me.msgText.Size = New System.Drawing.Size(517, 51)
            Me.msgText.TabIndex = 0
            Me.msgText.Text = "The read only file {0} is under source control and checked in.What do you want to" & " do?"
            ' 
            ' btnCheckout
            ' 
            Me.btnCheckout.Location = New System.Drawing.Point(62, 67)
            Me.btnCheckout.Name = "btnCheckout"
            Me.btnCheckout.Size = New System.Drawing.Size(108, 23)
            Me.btnCheckout.TabIndex = 1
            Me.btnCheckout.Text = "Checkout the file"
            Me.btnCheckout.UseVisualStyleBackColor = True
            AddHandler Me.btnCheckout.Click, New System.EventHandler(AddressOf Me.btnCheckout_Click)

            '			 
            ' 
            ' btnEdit
            ' 
            Me.btnEdit.Location = New System.Drawing.Point(203, 67)
            Me.btnEdit.Name = "btnEdit"
            Me.btnEdit.Size = New System.Drawing.Size(132, 23)
            Me.btnEdit.TabIndex = 2
            Me.btnEdit.Text = "Edit the file in memory"
            Me.btnEdit.UseVisualStyleBackColor = True
            AddHandler Me.btnEdit.Click, New System.EventHandler(AddressOf Me.btnEdit_Click)
            '
            ' 
            ' btnCancel
            ' 
            Me.btnCancel.Location = New System.Drawing.Point(366, 67)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(95, 23)
            Me.btnCancel.TabIndex = 3
            Me.btnCancel.Text = "Cancel the edit"
            Me.btnCancel.UseVisualStyleBackColor = True
            AddHandler Me.btnCancel.Click, New System.EventHandler(AddressOf btnCancel_Click)
            '
            ' 
            ' DlgQueryEditCheckedInFile
            ' 
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0F, 13.0F)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(523, 102)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.btnEdit)
            Me.Controls.Add(Me.btnCheckout)
            Me.Controls.Add(Me.msgText)
            Me.Name = "DlgQueryEditCheckedInFile"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Edit of read only file"
            Me.ResumeLayout(False)
        End Sub

		#End Region

		Private msgText As System.Windows.Forms.Label
		Private WithEvents btnCheckout As System.Windows.Forms.Button
		Private WithEvents btnEdit As System.Windows.Forms.Button
		Private WithEvents btnCancel As System.Windows.Forms.Button
	End Class
End Namespace