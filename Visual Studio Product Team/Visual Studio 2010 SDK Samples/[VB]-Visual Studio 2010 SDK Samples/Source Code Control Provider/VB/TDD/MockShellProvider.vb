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
Imports System.Windows.Forms
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio
Imports Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
	Friend Class MockShellProvider
		Private Shared ShellFactory As GenericMockFactory = Nothing

		#Region "Shell Getters"
		''' <summary>
        ''' Returns an IVsShell that does not implement any methods.
		''' </summary>
		''' <returns></returns>
		Private Sub New()
		End Sub
		Friend Shared Function GetShellInstance() As BaseMock
			If ShellFactory Is Nothing Then
				ShellFactory = New GenericMockFactory("Shell", New Type() { GetType(IVsShell) })
			End If
			Dim Shell As BaseMock = ShellFactory.GetInstance()
			Return Shell
		End Function

		''' <summary>
		''' Get an IVsShell that implement GetProperty and returns command line mode.
		''' </summary>
		''' <returns></returns>
		Friend Shared Function GetShellForCommandLine() As BaseMock
			Dim Shell As BaseMock = GetShellInstance()
			Dim name As String = String.Format("{0}.{1}", GetType(IVsShell).FullName, "GetProperty")
			Shell.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf GetPropertyCallBack1))
			Return Shell
		End Function

		''' <summary>
        ''' Get an IVsShell that implement GetProperty.
		''' </summary>
		''' <returns></returns>
		Friend Shared Function GetShellForUI() As BaseMock
			Dim Shell As BaseMock = GetShellInstance()
			Dim name As String = String.Format("{0}.{1}", GetType(IVsShell).FullName, "GetProperty")
			Shell.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf GetPropertyCallBack2))
			Return Shell
		End Function

		#End Region

		#Region "Callbacks"

		Private Shared Sub GetPropertyCallBack1(ByVal caller As Object, ByVal arguments As CallbackArgs)
			If CInt(Fix(arguments.GetParameter(0))) = CInt(Fix(__VSSPROPID.VSSPROPID_IsInCommandLineMode)) Then
				arguments.SetParameter(1, True)
				arguments.ReturnValue = VSConstants.S_OK
				Return
			End If

			arguments.ReturnValue = VSConstants.E_NOTIMPL
		End Sub

		Private Shared Sub GetPropertyCallBack2(ByVal caller As Object, ByVal arguments As CallbackArgs)
			If CInt(Fix(arguments.GetParameter(0))) = CInt(Fix(__VSSPROPID.VSSPROPID_IsInCommandLineMode)) Then
				arguments.SetParameter(1, False)
				arguments.ReturnValue = VSConstants.S_OK
				Return
			End If

			arguments.ReturnValue = VSConstants.E_NOTIMPL
		End Sub

		#End Region
	End Class
End Namespace
