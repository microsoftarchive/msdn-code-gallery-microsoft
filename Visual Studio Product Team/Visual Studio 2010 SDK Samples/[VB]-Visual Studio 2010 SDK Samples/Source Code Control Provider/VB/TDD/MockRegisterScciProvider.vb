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
Imports System.Text
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
	Friend Class MockRegisterScciProvider
		Private Shared registerScciProviderFactory As GenericMockFactory = Nothing

		#Region "RegisterScciProvider Getters"

		''' <summary>
        ''' Return a IVsRegisterScciProvider without any special implementation.
		''' </summary>
		''' <returns></returns>
		Friend Shared Function GetBaseRegisterScciProvider() As IVsRegisterScciProvider
			If registerScciProviderFactory Is Nothing Then
				registerScciProviderFactory = New GenericMockFactory("RegisterScciProvider", New Type() { GetType(IVsRegisterScciProvider) })
			End If
			Dim registerProvider As IVsRegisterScciProvider = CType(registerScciProviderFactory.GetInstance(), IVsRegisterScciProvider)
			Return registerProvider
		End Function

		#End Region

		#Region "Callbacks"
		#End Region
	End Class
End Namespace
