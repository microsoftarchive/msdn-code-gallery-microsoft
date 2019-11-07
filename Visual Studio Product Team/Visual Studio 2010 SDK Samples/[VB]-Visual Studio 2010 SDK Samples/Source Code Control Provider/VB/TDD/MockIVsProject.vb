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
	Friend Class MockIVsProject
		Implements IVsProject, IVsProject2, IVsHierarchy, IVsSccProject2
		Private ReadOnly _items As List(Of String) = New List(Of String)()
		Private ReadOnly _projFile As String

		Public Sub New(ByVal projFile As String)
			_projFile = projFile.ToLower()
		End Sub

		Protected Overrides Sub Finalize()
            ' Cleanup the projects and files from disk.
			_items.Add(_projFile)
			_items.Add(_projFile & ".storage")

			For Each file As String In _items
                If System.IO.File.Exists(file) Then
                    System.IO.File.SetAttributes(file, FileAttributes.Normal)
                    System.IO.File.Delete(file)
                End If
			Next file
		End Sub

		Public ReadOnly Property ProjectFile() As String
			Get
				Return _projFile
			End Get
		End Property

		Public ReadOnly Property ProjectItems() As IList(Of String)
			Get
				Return _items
			End Get
		End Property

		Public Sub AddItem(ByVal itemName As String)
			_items.Add(itemName.ToLower())
		End Sub

		Public Sub RenameItem(ByVal itemNameOld As String, ByVal itemNameNew As String)
			For iIndex As Integer = 0 To _items.Count - 1
				If itemNameOld.CompareTo(_items(iIndex)) = 0 Then
					_items(iIndex) = itemNameNew.ToLower()
					Exit For
				End If
			Next iIndex
		End Sub

		Public Sub RemoveItem(ByVal itemName As String)
			Dim iIndex As Integer = 0
			Do While iIndex < _items.Count
				If itemName.CompareTo(_items(iIndex)) = 0 Then
					_items.RemoveAt(iIndex)
					Exit Do
				End If
				iIndex += 1
			Loop
		End Sub

		#Region "IVsProject Members"

		Public Function AddItem(ByVal itemidLoc As UInteger, ByVal dwAddItemOperation As VSADDITEMOPERATION, ByVal pszItemName As String, ByVal cFilesToOpen As UInteger, ByVal rgpszFilesToOpen As String(), ByVal hwndDlgOwner As IntPtr, ByVal pResult As VSADDRESULT()) As Integer Implements IVsProject.AddItem, IVsProject2.AddItem
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GenerateUniqueItemName(ByVal itemidLoc As UInteger, ByVal pszExt As String, ByVal pszSuggestedRoot As String, <System.Runtime.InteropServices.Out()> ByRef pbstrItemName As String) As Integer Implements IVsProject.GenerateUniqueItemName, IVsProject2.GenerateUniqueItemName
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetItemContext(ByVal itemid As UInteger, <System.Runtime.InteropServices.Out()> ByRef ppSP As Microsoft.VisualStudio.OLE.Interop.IServiceProvider) As Integer Implements IVsProject.GetItemContext, IVsProject2.GetItemContext
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetMkDocument(ByVal itemid As UInteger, <System.Runtime.InteropServices.Out()> ByRef pbstrMkDocument As String) As Integer Implements IVsProject.GetMkDocument, IVsProject2.GetMkDocument
			If itemid = VSConstants.VSITEMID_ROOT Then
				pbstrMkDocument = _projFile
				Return VSConstants.S_OK
			ElseIf itemid >= 0 AndAlso itemid < _items.Count Then
				pbstrMkDocument = _items(CInt(Fix(itemid)))
				Return VSConstants.S_OK
			End If
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function IsDocumentInProject(ByVal pszMkDocument As String, <System.Runtime.InteropServices.Out()> ByRef pfFound As Integer, ByVal pdwPriority As VSDOCUMENTPRIORITY(), <System.Runtime.InteropServices.Out()> ByRef pitemid As UInteger) As Integer Implements IVsProject.IsDocumentInProject, IVsProject2.IsDocumentInProject
			pfFound = 0
			pitemid = VSConstants.VSITEMID_NIL
			pszMkDocument = pszMkDocument.ToLower()

			If pszMkDocument.CompareTo(_projFile) = 0 Then
				pfFound = 1
				pitemid = VSConstants.VSITEMID_ROOT
			Else
				For iIndex As Integer = 0 To _items.Count - 1
					If pszMkDocument.CompareTo(_items(iIndex)) = 0 Then
						pfFound = 1
						pitemid = CUInt(iIndex)
						Exit For
					End If
				Next iIndex
			End If

			Return VSConstants.S_OK
		End Function

		Public Function OpenItem(ByVal itemid As UInteger, ByRef rguidLogicalView As Guid, ByVal punkDocDataExisting As IntPtr, <System.Runtime.InteropServices.Out()> ByRef ppWindowFrame As IVsWindowFrame) As Integer Implements IVsProject.OpenItem, IVsProject2.OpenItem
			Throw New Exception("The method or operation is not implemented.")
		End Function

		#End Region

		#Region "IVsProject2 Members"

		Public Function RemoveItem(ByVal dwReserved As UInteger, ByVal itemid As UInteger, <System.Runtime.InteropServices.Out()> ByRef pfResult As Integer) As Integer Implements IVsProject2.RemoveItem
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function ReopenItem(ByVal itemid As UInteger, ByRef rguidEditorType As Guid, ByVal pszPhysicalView As String, ByRef rguidLogicalView As Guid, ByVal punkDocDataExisting As IntPtr, <System.Runtime.InteropServices.Out()> ByRef ppWindowFrame As IVsWindowFrame) As Integer Implements IVsProject2.ReopenItem
			Throw New Exception("The method or operation is not implemented.")
		End Function

		#End Region

		#Region "IVsHierarchy Members"

		Public Function AdviseHierarchyEvents(ByVal pEventSink As IVsHierarchyEvents, <System.Runtime.InteropServices.Out()> ByRef pdwCookie As UInteger) As Integer Implements IVsHierarchy.AdviseHierarchyEvents
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function Close() As Integer Implements IVsHierarchy.Close
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetCanonicalName(ByVal itemid As UInteger, <System.Runtime.InteropServices.Out()> ByRef pbstrName As String) As Integer Implements IVsHierarchy.GetCanonicalName
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetGuidProperty(ByVal itemid As UInteger, ByVal propid As Integer, <System.Runtime.InteropServices.Out()> ByRef pguid As Guid) As Integer Implements IVsHierarchy.GetGuidProperty
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetNestedHierarchy(ByVal itemid As UInteger, ByRef iidHierarchyNested As Guid, <System.Runtime.InteropServices.Out()> ByRef ppHierarchyNested As IntPtr, <System.Runtime.InteropServices.Out()> ByRef pitemidNested As UInteger) As Integer Implements IVsHierarchy.GetNestedHierarchy
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetProperty(ByVal itemid As UInteger, ByVal propid As Integer, <System.Runtime.InteropServices.Out()> ByRef pvar As Object) As Integer Implements IVsHierarchy.GetProperty
			If itemid = VSConstants.VSITEMID_ROOT Then
				If propid = CInt(Fix(__VSHPROPID.VSHPROPID_FirstChild)) Then
					If _items.Count > 0 Then
						pvar = 0
                    Else
                        Try
                            pvar = CInt(Fix(VSConstants.VSITEMID_NIL))
                        Catch
                            pvar = -1
                        End Try
                    End If
                    Return VSConstants.S_OK
                ElseIf propid = CInt(Fix(__VSHPROPID.VSHPROPID_Name)) Then
                    pvar = Path.GetFileNameWithoutExtension(ProjectFile)
                    Return VSConstants.S_OK
                ElseIf propid = CInt(Fix(__VSHPROPID2.VSHPROPID_Container)) Then
                    pvar = (_items.Count > 0)
                    Return VSConstants.S_OK
                ElseIf propid = CInt(Fix(__VSHPROPID.VSHPROPID_Expandable)) Then
                    If (_items.Count > 0) Then
                        pvar = CInt(Fix(1))
                    Else
                        pvar = CInt(Fix(0))
                    End If
                    Return VSConstants.S_OK
                End If
			ElseIf itemid >= 0 AndAlso itemid < _items.Count Then
				If propid = CInt(Fix(__VSHPROPID.VSHPROPID_NextSibling)) Then
					If itemid < _items.Count - 1 Then
                        Try
                            pvar = CInt(Fix(itemid)) + 1
                        Catch
                            pvar = -1
                        End Try
					Else
                        Try
                            pvar = CInt(Fix(VSConstants.VSITEMID_NIL))
                        Catch
                            pvar = -1
                        End Try
					End If
					Return VSConstants.S_OK
				ElseIf propid = CInt(Fix(__VSHPROPID.VSHPROPID_FirstChild)) Then
                    Try
                        pvar = CInt(Fix(VSConstants.VSITEMID_NIL))
                    Catch
                        pvar = -1
                    End Try
					Return VSConstants.S_OK
				ElseIf propid = CInt(Fix(__VSHPROPID.VSHPROPID_Name)) Then
					pvar = Path.GetFileNameWithoutExtension(ProjectItems(CInt(Fix(itemid))))
					Return VSConstants.S_OK
				ElseIf propid = CInt(Fix(__VSHPROPID2.VSHPROPID_Container)) Then
					' The project support only files, which are not expandable like folders, 
                    ' and they are not unexpandable containers (like the MyProject node in a VB app).
					pvar = False
					Return VSConstants.S_OK
				ElseIf propid = CInt(Fix(__VSHPROPID.VSHPROPID_Expandable)) Then
					pvar = CInt(Fix(0))
					Return VSConstants.S_OK
				End If
			End If
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function GetSite(<System.Runtime.InteropServices.Out()> ByRef ppSP As Microsoft.VisualStudio.OLE.Interop.IServiceProvider) As Integer Implements IVsHierarchy.GetSite
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function ParseCanonicalName(ByVal pszName As String, <System.Runtime.InteropServices.Out()> ByRef pitemid As UInteger) As Integer Implements IVsHierarchy.ParseCanonicalName
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function QueryClose(<System.Runtime.InteropServices.Out()> ByRef pfCanClose As Integer) As Integer Implements IVsHierarchy.QueryClose
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function SetGuidProperty(ByVal itemid As UInteger, ByVal propid As Integer, ByRef rguid As Guid) As Integer Implements IVsHierarchy.SetGuidProperty
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function SetProperty(ByVal itemid As UInteger, ByVal propid As Integer, ByVal var As Object) As Integer Implements IVsHierarchy.SetProperty
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function SetSite(ByVal psp As Microsoft.VisualStudio.OLE.Interop.IServiceProvider) As Integer Implements IVsHierarchy.SetSite
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function UnadviseHierarchyEvents(ByVal dwCookie As UInteger) As Integer Implements IVsHierarchy.UnadviseHierarchyEvents
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function Unused0() As Integer Implements IVsHierarchy.Unused0
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function Unused1() As Integer Implements IVsHierarchy.Unused1
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function Unused2() As Integer Implements IVsHierarchy.Unused2
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function Unused3() As Integer Implements IVsHierarchy.Unused3
			Throw New Exception("The method or operation is not implemented.")
		End Function

		Public Function Unused4() As Integer Implements IVsHierarchy.Unused4
			Throw New Exception("The method or operation is not implemented.")
		End Function

		#End Region

		#Region "IVsSccProject2 Members"

		Public Function GetSccFiles(ByVal itemid As UInteger, ByVal pCaStringsOut As CALPOLESTR(), ByVal pCaFlagsOut As CADWORD()) As Integer Implements IVsSccProject2.GetSccFiles
			If (Nothing Is pCaStringsOut) OrElse (0 = pCaStringsOut.Length) Then
				Throw New ArgumentNullException()
			End If
			If (Nothing Is pCaFlagsOut) OrElse (0 = pCaFlagsOut.Length) Then
				Throw New ArgumentNullException()
			End If

			pCaStringsOut(0) = New CALPOLESTR()
			pCaStringsOut(0).cElems = 0
			pCaStringsOut(0).pElems = IntPtr.Zero

			pCaFlagsOut(0) = New CADWORD()
			pCaFlagsOut(0).cElems = 0
			pCaFlagsOut(0).pElems = IntPtr.Zero

			Dim fileForNode As String = Nothing
			If itemid = VSConstants.VSITEMID_ROOT Then
				fileForNode = _projFile
			ElseIf itemid >= 0 AndAlso itemid < _items.Count Then
				fileForNode = _items(CInt(Fix(itemid)))
			End If
            Dim dummyx As Int32
            If fileForNode IsNot Nothing Then
                ' There is only one scc controllable file per each hierarchy node.
                pCaStringsOut(0).cElems = 1
                pCaStringsOut(0).pElems = Marshal.AllocCoTaskMem(IntPtr.Size)
                Marshal.WriteIntPtr(pCaStringsOut(0).pElems, Marshal.StringToCoTaskMemUni(fileForNode))

                pCaFlagsOut(0).cElems = 1
                pCaFlagsOut(0).pElems = Marshal.AllocCoTaskMem(System.Runtime.InteropServices.Marshal.SizeOf(dummyx))
                Marshal.WriteInt32(pCaFlagsOut(0).pElems, 0)
            End If

			Return VSConstants.S_OK
		End Function

		Public Function GetSccSpecialFiles(ByVal itemid As UInteger, ByVal pszSccFile As String, ByVal pCaStringsOut As CALPOLESTR(), ByVal pCaFlagsOut As CADWORD()) As Integer Implements IVsSccProject2.GetSccSpecialFiles
			Return VSConstants.E_NOTIMPL
		End Function

		Public Function SccGlyphChanged(ByVal cAffectedNodes As Integer, ByVal rgitemidAffectedNodes As UInteger(), ByVal rgsiNewGlyphs As VsStateIcon(), ByVal rgdwNewSccStatus As UInteger()) As Integer Implements IVsSccProject2.SccGlyphChanged
			Return VSConstants.S_OK
		End Function

		Public Function SetSccLocation(ByVal pszSccProjectName As String, ByVal pszSccAuxPath As String, ByVal pszSccLocalPath As String, ByVal pszSccProvider As String) As Integer Implements IVsSccProject2.SetSccLocation
			Return VSConstants.S_OK
		End Function

		#End Region
	End Class
End Namespace
