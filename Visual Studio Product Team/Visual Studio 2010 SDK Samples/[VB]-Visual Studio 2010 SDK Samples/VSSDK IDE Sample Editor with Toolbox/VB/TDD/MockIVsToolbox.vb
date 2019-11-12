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
Imports System.Text
Imports Microsoft.VisualStudio
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio.Shell.Interop

Namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests
	''' <summary>
	''' Provides implementation and Getter methods for the IVsToolbox Mock instance.
	''' </summary>
	Friend Class MockIVsToolbox
		#Region "Methods"
		''' <summary>
		''' Getter method for the IVsToolbox Mock object.
		''' </summary>
		''' <returns></returns>
		Private Sub New()
		End Sub
		Friend Shared Function GetIVsToolboxInstance() As BaseMock
            Dim factory As New GenericMockFactory("IVsToolbox", New Type() {GetType(IVsToolbox)})
			Dim mockObj As BaseMock = factory.GetInstance()
			Return mockObj
		End Function
#End Region
	End Class
End Namespace
