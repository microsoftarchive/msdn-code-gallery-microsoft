'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports Microsoft.VisualBasic
Imports System
Imports System.Globalization
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider
	Public Partial Class DlgQuerySaveCheckedInFile
		Inherits Form
		Public Const qscifCheckout As Integer = 1
		Public Const qscifSkipSave As Integer = 2
		Public Const qscifForceSaveAs As Integer = 3
		Public Const qscifCancel As Integer = 4

		Private _answer As Integer = qscifCancel

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
			Answer = qscifCheckout
			Close()
		End Sub

		Private Sub btnSkipSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSkipSave.Click
			Answer = qscifSkipSave
			Close()
		End Sub

		Private Sub btnSaveAs_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveAs.Click
			Answer = qscifForceSaveAs
			Close()
		End Sub

		Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
			Answer = qscifCancel
			Close()
		End Sub
	End Class
End Namespace