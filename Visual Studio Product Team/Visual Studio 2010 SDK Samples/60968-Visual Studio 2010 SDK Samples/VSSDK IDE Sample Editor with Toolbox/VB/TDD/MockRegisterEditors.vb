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
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VsSDK.UnitTestLibrary

Namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests
	''' <summary>
	''' Provides implementation and Getter methods for the IVsRegisterEditors Mock instance.
	''' </summary>
	Friend Class MockRegisterEditors
		#Region "Fields"
		Private Shared registerEditorFactory As GenericMockFactory
#End Region

		#Region "Methods"
		''' <summary>
		''' Getter method for the IVsRegisterEditors Mock object.
		''' </summary>
		Private Sub New()
		End Sub
		Friend Shared Function GetRegisterEditorsInstance() As BaseMock
			If registerEditorFactory Is Nothing Then
				registerEditorFactory = New GenericMockFactory("SVsRegisterEditors", New Type() { GetType(IVsRegisterEditors) })
			End If
			Dim registerEditor As BaseMock = registerEditorFactory.GetInstance()
			Return registerEditor
		End Function
#End Region
	End Class
End Namespace
