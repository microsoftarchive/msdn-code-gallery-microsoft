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
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
	Friend Class MockTrackProjectDocumentsProvider
		Private Shared tpdFactory As GenericMockFactory = Nothing

		#Region "TPD Getters"

		''' <summary>
        ''' Return a IVsTrackProjectDocuments2 without any special implementation.
		''' </summary>
		''' <returns></returns>
		Friend Shared Function GetBaseTrackProjectDocuments() As BaseMock
			If tpdFactory Is Nothing Then
				tpdFactory = New GenericMockFactory("TPDProvider", New Type() { GetType(IVsTrackProjectDocuments2) })
			End If

			Dim tpd As BaseMock = tpdFactory.GetInstance()
			Return tpd
		End Function

		''' <summary>
		''' Get an IVsTrackProjectDocuments2 that implement AdviseTrackProjectDocumentsEvents and UnadviseTrackProjectDocumentsEvents.
		''' </summary>
		''' <returns></returns>
		Friend Shared Function GetTrackProjectDocuments() As BaseMock
			Dim tpd As BaseMock = GetBaseTrackProjectDocuments()
			Dim name As String = String.Format("{0}.{1}", GetType(IVsTrackProjectDocuments2).FullName, "AdviseTrackProjectDocumentsEvents")
			tpd.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf AdviseTrackProjectDocumentsEventsCallBack))
			name = String.Format("{0}.{1}", GetType(IVsTrackProjectDocuments2).FullName, "UnadviseTrackProjectDocumentsEvents")
			tpd.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf UnadviseTrackProjectDocumentsEventsCallBack))
			Return tpd
		End Function

		#End Region

		#Region "Callbacks"

		Private Shared Sub AdviseTrackProjectDocumentsEventsCallBack(ByVal caller As Object, ByVal arguments As CallbackArgs)
			Dim tpdTrackProjectDocumentsCookie As UInteger = 1
			arguments.SetParameter(1, tpdTrackProjectDocumentsCookie)
			arguments.ReturnValue = VSConstants.S_OK
		End Sub

		Private Shared Sub UnadviseTrackProjectDocumentsEventsCallBack(ByVal caller As Object, ByVal arguments As CallbackArgs)
			Assert.AreEqual(CUInt(1), CUInt(arguments.GetParameter(0)), "Incorrect cookie unregistered")
			arguments.ReturnValue = VSConstants.S_OK
		End Sub

		#End Region
	End Class
End Namespace
