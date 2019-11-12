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
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio.Shell.Interop

Namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage.UnitTests
	''' <summary>
	''' Implements general mocks and methods of access to them.
	''' </summary>
	Friend Class MockServiceProvider
        ' Fields
		#Region "Fields"
		Private Shared userSettingsFactory As GenericMockFactory
		#End Region 

		#Region "Methods (Mock's  getters)"
		''' <summary>
		''' Returns an IVSUserSettings that does not implement any methods.
		''' </summary>
		''' <returns></returns>
		Private Sub New()
		End Sub
		Friend Shared Function GetUserSettingsFactoryInstance() As BaseMock
			If userSettingsFactory Is Nothing Then
				userSettingsFactory = New GenericMockFactory("MockUserSettings", New Type() { GetType(IVsUserSettings) })
			End If
			Dim userSettings As BaseMock = userSettingsFactory.GetInstance()
			Return userSettings
		End Function
        ' Methods (Mock's  getters).
		#End Region 
	End Class
End Namespace
