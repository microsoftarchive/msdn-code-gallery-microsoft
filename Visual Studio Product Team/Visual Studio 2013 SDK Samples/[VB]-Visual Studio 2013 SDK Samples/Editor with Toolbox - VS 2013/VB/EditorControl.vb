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

Namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox
	''' <summary>
	''' This class provides room for extension of a RichTextBox.
	''' </summary>
	Public Class EditorControl
		Inherits RichTextBox
		#Region "Constructor"
		''' <summary>
		''' Explicitly defined default constructor.
		''' Initialize new instance of the EditorControl.
		''' </summary>
		Public Sub New()
			InitializeComponent()
		End Sub
		#End Region

		#Region "Methods"
		''' <summary>
		''' Required method for Designer support - do not modify
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
			Me.WordWrap = False
		End Sub
		#End Region
	End Class
End Namespace
