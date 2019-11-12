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
Imports System.Globalization
Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Runtime.InteropServices

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider
	''' <summary>
	''' Summary description for SccProviderToolWindowControl.
	''' </summary>
	Public Class SccProviderToolWindowControl
		Inherits System.Windows.Forms.UserControl
		Private label1 As Label

		Public Sub New()
			' This call is required by the Windows.Forms Form Designer.
			InitializeComponent()
		End Sub

		''' <summary> 
		''' Let this control process the mnemonics.
		''' </summary>
		Protected Overrides Function ProcessDialogChar(ByVal charCode As Char) As Boolean
            ' If we're the top-level form or control, we need to do the mnemonic handling.
            If charCode <> " " AndAlso ProcessMnemonic(charCode) Then
                Return True
            End If
			  Return MyBase.ProcessDialogChar(charCode)
		End Function

		#Region "Component Designer generated code"
		''' <summary> 
		''' Required method for Designer support - do not modify 
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
            Dim resources As New System.ComponentModel.ComponentResourceManager(GetType(SccProviderToolWindowControl))
			Me.label1 = New System.Windows.Forms.Label()
			Me.SuspendLayout()
			' 
			' label1
			' 
			resources.ApplyResources(Me.label1, "label1")
			Me.label1.Name = "label1"
			' 
			' SccProviderToolWindowControl
			' 
			Me.BackColor = System.Drawing.SystemColors.Window
			Me.Controls.Add(Me.label1)
			Me.Name = "SccProviderToolWindowControl"
			resources.ApplyResources(Me, "$this")
			Me.ResumeLayout(False)
			Me.PerformLayout()

		End Sub
		#End Region
	End Class
End Namespace
