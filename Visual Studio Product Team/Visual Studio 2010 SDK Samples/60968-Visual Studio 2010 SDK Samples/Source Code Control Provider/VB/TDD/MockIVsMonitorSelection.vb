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
Imports System.IO
Imports System.Text
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
	Friend Class MockIVsMonitorSelectionFactory
		Private Shared MonSelFactory As GenericMockFactory = Nothing

		#Region "MonSel Getters"
		''' <summary>
        ''' Returns a monitor selection object that does not implement any methods.
		''' </summary>
		''' <returns></returns>
		Private Sub New()
		End Sub
		Friend Shared Function GetBaseMonSelInstance() As BaseMock
			If MonSelFactory Is Nothing Then
				MonSelFactory = New GenericMockFactory("MonitorSelection", New Type() { GetType(IVsMonitorSelection), GetType(IVsMultiItemSelect) })
			End If
			Dim pb As BaseMock = MonSelFactory.GetInstance()
			Return pb
		End Function

		''' <summary>
        ''' Returns a monitor selection object that implement GetCurrentSelection and GetSelectionInfo/GetSelectedItems.
		''' </summary>
		''' <returns></returns>
		Friend Shared Function GetMonSel() As BaseMock
			Dim pb As BaseMock = GetBaseMonSelInstance()

            ' Add the callback methods.
			Dim name As String = String.Format("{0}.{1}", GetType(IVsMonitorSelection).FullName, "GetCurrentSelection")
			pb.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf GetCurrentSelectionCallback))
			name = String.Format("{0}.{1}", GetType(IVsMultiItemSelect).FullName, "GetSelectionInfo")
			pb.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf GetSelectionInfoCallback))
			name = String.Format("{0}.{1}", GetType(IVsMultiItemSelect).FullName, "GetSelectedItems")
			pb.AddMethodCallback(name, New EventHandler(Of CallbackArgs)(AddressOf GetSelectedItemsCallback))

            ' Initialize selection data to empty selection.
			pb("Selection") = Nothing
			Return pb
		End Function

		#End Region

		#Region "Callbacks"

		Private Shared Sub GetCurrentSelectionCallback(ByVal caller As Object, ByVal arguments As CallbackArgs)
            ' Read the current selection data.
			Dim selection As VSITEMSELECTION() = CType((CType(caller, BaseMock))("Selection"), VSITEMSELECTION())

            ' Initialize output parameters for empty selection.
            ' hierarchyPtr
            arguments.SetParameter(0, IntPtr.Zero)
            ' itemid
            arguments.SetParameter(1, VSConstants.VSITEMID_NIL)
            ' multiItemSelect
            arguments.SetParameter(2, Nothing)
            ' selectionContainer
            arguments.SetParameter(3, IntPtr.Zero)

            If selection IsNot Nothing Then
                If selection.Length = 1 Then
                    If selection(0).pHier IsNot Nothing Then
                        Dim ptrHier As IntPtr = Marshal.GetComInterfaceForObject(selection(0).pHier, GetType(IVsHierarchy))
                        ' hierarchyPtr
                        arguments.SetParameter(0, ptrHier)
                    End If
                    ' itemid
                    arguments.SetParameter(1, selection(0).itemid)
                Else
                    ' Multiple selection, return IVsMultiItemSelect interface.
                    ' itemid
                    arguments.SetParameter(1, VSConstants.VSITEMID_SELECTION)
                    ' multiItemSelect
                    arguments.SetParameter(2, TryCast(caller, IVsMultiItemSelect))
                End If
            End If

			arguments.ReturnValue = VSConstants.S_OK
		End Sub

		Private Shared Sub GetSelectionInfoCallback(ByVal caller As Object, ByVal arguments As CallbackArgs)
            ' Read the current selection data.
			Dim selection As VSITEMSELECTION() = CType((CType(caller, BaseMock))("Selection"), VSITEMSELECTION())

            ' Initialize output parameters for empty selection.
            ' numberOfSelectedItems
            arguments.SetParameter(0, CUInt(0))
            ' isSingleHierarchyInt
            arguments.SetParameter(1, CInt(Fix(1)))

            If selection IsNot Nothing Then
                ' numberOfSelectedItems
                arguments.SetParameter(0, CUInt(selection.Length))
                If selection.Length > 0 Then
                    For i As Integer = 1 To selection.Length - 1
                        If Not selection(i).pHier Is selection(0).pHier Then
                            ' isSingleHierarchyInt
                            arguments.SetParameter(1, CInt(Fix(0)))
                            Exit For
                        End If
                    Next i
                End If
            End If

			arguments.ReturnValue = VSConstants.S_OK
		End Sub

		Private Shared Sub GetSelectedItemsCallback(ByVal caller As Object, ByVal arguments As CallbackArgs)
            ' Read the current selection data.
			Dim selection As VSITEMSELECTION() = CType((CType(caller, BaseMock))("Selection"), VSITEMSELECTION())

            ' Get the arguments.
			Dim grfGSI As UInteger = CUInt(arguments.GetParameter(0))
			Dim cRequestedItems As UInteger = CUInt(arguments.GetParameter(1))
			Dim rgItemSel As VSITEMSELECTION() = CType(arguments.GetParameter(2), VSITEMSELECTION())

			If selection Is Nothing AndAlso cRequestedItems > 0 OrElse selection.Length < cRequestedItems Then
				arguments.ReturnValue = VSConstants.E_INVALIDARG
				Return
			End If

            For i As Integer = 0 To CInt(cRequestedItems - 1)
                rgItemSel(i).itemid = selection(i).itemid
                If (grfGSI And CUInt(__VSGSIFLAGS.GSI_fOmitHierPtrs)) = 0 Then
                    rgItemSel(i).pHier = selection(i).pHier
                End If
            Next i

			arguments.ReturnValue = VSConstants.S_OK
		End Sub

		#End Region
	End Class
End Namespace
