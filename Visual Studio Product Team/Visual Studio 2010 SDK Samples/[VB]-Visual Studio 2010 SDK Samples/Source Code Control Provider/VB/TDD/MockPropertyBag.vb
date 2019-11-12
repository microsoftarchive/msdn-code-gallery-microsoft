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
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio
Imports Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
	Friend Class MockPropertyBagProvider
		Private Shared PBFactory As GenericMockFactory = Nothing
        ' The names of the properties stored by the provider in the solution file.
		Private Const strSolutionControlledProperty As String = "SolutionIsControlled"
		Private Const strSolutionBindingsProperty As String = "SolutionBindings"

		#Region "PB Getters"
		''' <summary>
        ''' Returns a property bag that does not implement any methods.
		''' </summary>
		''' <returns></returns>
		Private Sub New()
		End Sub
		Friend Shared Function GetPBInstance() As BaseMock
			If PBFactory Is Nothing Then
				PBFactory = New GenericMockFactory("PropertyBag", New Type() { GetType(IPropertyBag) })
			End If
			Dim pb As BaseMock = PBFactory.GetInstance()
			Return pb
		End Function

		''' <summary>
        ''' Get a property bag that implements Read method.
		''' </summary>
		''' <returns></returns>
		Friend Shared Function GetReadPropertyBag() As BaseMock
			Dim pb As BaseMock = GetPBInstance()
			Dim name As String = String.Format("{0}.{1}", GetType(IPropertyBag).FullName, "Read")
			pb.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf ReadCallback))
			Return pb
		End Function

		''' <summary>
        ''' Get a property bag that implements Write method.
		''' </summary>
		''' <returns></returns>
		Friend Shared Function GetWritePropertyBag() As BaseMock
			Dim pb As BaseMock = GetPBInstance()
			Dim name As String = String.Format("{0}.{1}", GetType(IPropertyBag).FullName, "Write")
			pb.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf WriteCallback))
			Return pb
		End Function

		#End Region

		#Region "Callbacks"

		Private Shared Sub WriteCallback(ByVal caller As Object, ByVal arguments As CallbackArgs)
			arguments.ReturnValue = VSConstants.E_NOTIMPL
		End Sub

		Private Shared Sub ReadCallback(ByVal caller As Object, ByVal arguments As CallbackArgs)
			Dim propertyName As String = CStr(arguments.GetParameter(0))
			If propertyName = strSolutionControlledProperty Then
				arguments.SetParameter(1, True)
				arguments.ReturnValue = VSConstants.S_OK
				Return
			ElseIf propertyName = strSolutionBindingsProperty Then
				arguments.SetParameter(1, "Solution's location")
				arguments.ReturnValue = VSConstants.S_OK
				Return
			End If

			arguments.ReturnValue = VSConstants.E_NOTIMPL
		End Sub

		#End Region
	End Class
End Namespace
