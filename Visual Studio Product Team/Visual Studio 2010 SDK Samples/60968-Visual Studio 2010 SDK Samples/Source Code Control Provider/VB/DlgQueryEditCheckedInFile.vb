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
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Globalization
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider
	Public Partial Class DlgQueryEditCheckedInFile
		Inherits Form
		Public Const qecifCheckout As Integer = 1
		Public Const qecifEditInMemory As Integer = 2
		Public Const qecifCancelEdit As Integer = 3

		Private _answer As Integer = qecifCancelEdit

		Public Property Answer() As Integer
			Get
				Return _answer
			End Get
			Set(ByVal value As Integer)
				_answer = value
			End Set
		End Property

		Public Sub New(ByVal filename As String)
			InitializeComponent()

            ' Format the message text with the current file name.
			msgText.Text = String.Format(CultureInfo.CurrentUICulture, msgText.Text, filename)
		End Sub

		Private Sub btnCheckout_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCheckout.Click
			Answer = qecifCheckout
			Close()
		End Sub

		Private Sub btnEdit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnEdit.Click
			Answer = qecifEditInMemory
			Close()
		End Sub

		Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
			Answer = qecifCancelEdit
			Close()
		End Sub
	End Class
End Namespace