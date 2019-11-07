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
Imports System.Windows.Forms

Namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage
	''' <summary>
	''' Provides Windows.Forms GUI helper functions.
	''' It provides a boolean property to suppress message boxes. 
	''' </summary>
	Public Class WinFormsHelper
        ' Fields
		#Region "Fields"
		Private Shared messageBoxAllowed As Boolean = True
		Private Shared fakeResult As DialogResult = DialogResult.None
		#End Region 

		#Region "Properties"
		''' <summary>
		''' Gets or sets the value that indicates whether to display MessageBox.
		''' </summary>
		''' <remarks>Used in MessageBox simulation purposes. By default is true.</remarks>
		Private Sub New()
		End Sub
		Public Shared Property AllowMessageBox() As Boolean
			Get
				Return messageBoxAllowed
			End Get
			Set(ByVal value As Boolean)
				messageBoxAllowed = value
			End Set
		End Property
		''' <summary>
		''' Gets or sets fake DialogResult value.
		''' </summary>
		''' <remarks>Used in MessageBox simulation purposes. By default - DialogResult.None.</remarks>
		Public Shared Property FakeDialogResult() As DialogResult
			Get
				Return fakeResult
			End Get
			Set(ByVal value As DialogResult)
				fakeResult = value
			End Set
		End Property
        ' Properties
		#End Region 

		#Region "Methods"
		''' <summary>
		''' Shows Windows.Forms message box based on passed parameters if AllowMessageBox property is true.
		''' </summary>
		''' <returns>If MessageBox was not shown - method returns FakeDialogResult.</returns>
		Public Shared Function ShowMessageBox(ByVal text As String, ByVal caption As String, ByVal buttons As MessageBoxButtons, ByVal icon As MessageBoxIcon) As DialogResult
			If (Not String.IsNullOrEmpty(text)) AndAlso messageBoxAllowed Then
                Return MessageBox.Show(text, caption, buttons, icon, MessageBoxDefaultButton.Button1)
            End If
			Return fakeResult
		End Function

		''' <summary>
		''' Shows Windows.Forms message box (with specified text message and button set) if AllowMessageBox property is true.
		''' </summary>
		''' <returns>If MessageBox was not shown - method returns FakeDialogResult.</returns>
		Public Shared Function ShowMessageBox(ByVal text As String, ByVal buttons As MessageBoxButtons) As DialogResult
			Return ShowMessageBox(text, Resources.MessageCaption, buttons, MessageBoxIcon.Information)
		End Function

		''' <summary>
		''' Shows Windows.Forms message box (with specified text message and OKCancel button set) if AllowMessageBox property is true.
		''' </summary>
		''' <returns>If MessageBox was not shown - method returns FakeDialogResult.</returns>
		Public Shared Function ShowMessageBox(ByVal text As String) As DialogResult
			Return ShowMessageBox(text, Resources.MessageCaption, MessageBoxButtons.OKCancel, MessageBoxIcon.Information)
		End Function

		#End Region 
	End Class
End Namespace
