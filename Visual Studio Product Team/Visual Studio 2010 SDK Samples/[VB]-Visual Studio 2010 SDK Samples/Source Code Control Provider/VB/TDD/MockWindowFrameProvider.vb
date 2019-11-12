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
	Friend Class MockWindowFrameProvider
		Private Const propertiesName As String = "properties"

		Private Shared frameFactory As GenericMockFactory = Nothing
		Private Shared trackSelectionFactory_Renamed As GenericMockFactory = Nothing

		''' <summary>
        ''' Return a IVsWindowFrame without any special implementation.
		''' </summary>
		''' <returns></returns>
		Friend Shared Function GetBaseFrame() As IVsWindowFrame
			If frameFactory Is Nothing Then
                frameFactory = New GenericMockFactory("WindowFrame", New Type() {GetType(IVsWindowFrame), GetType(IVsWindowFrame2)})
			End If
			Dim frame As IVsWindowFrame = CType(frameFactory.GetInstance(), IVsWindowFrame)
			Return frame
		End Function

		''' <summary>
        ''' Return an IVsWindowFrame implements GetProperty.
		''' The peopertiesList will be used too look up PropertyIDs to find values for
		''' requested properties.
		''' </summary>
		''' <param name="propertiesList">The dictionary contains PropertyID/Value pairs</param>
		''' <returns></returns>
		Friend Shared Function GetFrameWithProperties(ByVal propertiesList As Dictionary(Of Integer, Object)) As IVsWindowFrame

			If frameFactory Is Nothing Then
                frameFactory = New GenericMockFactory("WindowFrame", New Type() {GetType(IVsWindowFrame), GetType(IVsWindowFrame2)})
			End If
			Dim frame As BaseMock = CType(frameFactory.GetInstance(), BaseMock)
			frame(propertiesName) = propertiesList
            ' Add support for GetProperty.
			Dim name As String = String.Format("{0}.{1}", GetType(IVsWindowFrame).FullName, "GetProperty")
			frame.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf GetPropertiesCallBack))
            ' Add support for GetGuidProperty.
			name = String.Format("{0}.{1}", GetType(IVsWindowFrame).FullName, "GetGuidProperty")
			frame.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf GetPropertiesCallBack))

			Return CType(frame, IVsWindowFrame)
		End Function

		Friend Shared ReadOnly Property TrackSelectionFactory() As GenericMockFactory
			Get
				If trackSelectionFactory_Renamed Is Nothing Then
					trackSelectionFactory_Renamed = New GenericMockFactory("MockTrackSelection", New Type() { GetType(ITrackSelection) })
				End If
				Return trackSelectionFactory_Renamed
			End Get
		End Property

		#Region "Callbacks"
		Private Shared Sub GetPropertiesCallBack(ByVal caller As Object, ByVal arguments As CallbackArgs)
			arguments.ReturnValue = VSConstants.S_OK

            ' Find the corresponding property.
			Dim propertyID As Object = arguments.GetParameter(0)
			Dim properties As Dictionary(Of Integer, Object) = CType((CType(caller, BaseMock))(propertiesName), Dictionary(Of Integer, Object))
			Dim propertyValue As Object = Nothing
            If properties IsNot Nothing AndAlso propertyID IsNot Nothing Then
                propertyValue = properties(CInt(Fix(propertyID)))
            End If
            ' Set the value we ended up with as the return value.
			arguments.SetParameter(1, propertyValue)
		End Sub
		#End Region
	End Class
End Namespace
