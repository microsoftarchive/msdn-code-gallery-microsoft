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
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    Friend Class MockIVsProject
        Implements IVsProject, IVsHierarchy
        Private ReadOnly _items As New List(Of String)()
        Private ReadOnly _projFile As String

        Public Sub New(ByVal projFile As String)
            _projFile = projFile
        End Sub

        Public ReadOnly Property FullPath() As String
            Get
                Return _projFile
            End Get
        End Property

        Public Sub AddItem(ByVal itemName As String)
            _items.Add(itemName)
        End Sub

#Region "IVsProject Members"

        Public Function AddItem(ByVal itemidLoc As UInteger, ByVal dwAddItemOperation As VSADDITEMOPERATION, ByVal pszItemName As String, ByVal cFilesToOpen As UInteger, ByVal rgpszFilesToOpen As String(), ByVal hwndDlgOwner As IntPtr, ByVal pResult As VSADDRESULT()) As Integer Implements IVsProject.AddItem
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GenerateUniqueItemName(ByVal itemidLoc As UInteger, ByVal pszExt As String, ByVal pszSuggestedRoot As String, <System.Runtime.InteropServices.Out()> ByRef pbstrItemName As String) As Integer Implements IVsProject.GenerateUniqueItemName
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetItemContext(ByVal itemid As UInteger, <System.Runtime.InteropServices.Out()> ByRef ppSP As Microsoft.VisualStudio.OLE.Interop.IServiceProvider) As Integer Implements IVsProject.GetItemContext
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetMkDocument(ByVal itemid As UInteger, <System.Runtime.InteropServices.Out()> ByRef pbstrMkDocument As String) As Integer Implements IVsProject.GetMkDocument
            If itemid = CodeSweep.VSPackage.HierarchyConstants_Accessor.VSITEMID_ROOT Then
                pbstrMkDocument = _projFile
                Return VSConstants.S_OK
            ElseIf itemid >= 0 AndAlso itemid < _items.Count Then
                pbstrMkDocument = _items(CInt(Fix(itemid)))
                Return VSConstants.S_OK
            End If
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function IsDocumentInProject(ByVal pszMkDocument As String, <System.Runtime.InteropServices.Out()> ByRef pfFound As Integer, ByVal pdwPriority As VSDOCUMENTPRIORITY(), <System.Runtime.InteropServices.Out()> ByRef pitemid As UInteger) As Integer Implements IVsProject.IsDocumentInProject
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function OpenItem(ByVal itemid As UInteger, ByRef rguidLogicalView As Guid, ByVal punkDocDataExisting As IntPtr, <System.Runtime.InteropServices.Out()> ByRef ppWindowFrame As IVsWindowFrame) As Integer Implements IVsProject.OpenItem
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
            If itemid = CodeSweep.VSPackage.HierarchyConstants_Accessor.VSITEMID_ROOT Then
                If propid = CInt(Fix(__VSHPROPID.VSHPROPID_FirstChild)) Then
                    If _items.Count > 0 Then
                        pvar = 0
                    Else
                        Try
                            pvar = CInt(Fix(CodeSweep.VSPackage.HierarchyConstants_Accessor.VSITEMID_NIL))
                        Catch ex As Exception
                            pvar = -1
                        End Try
                    End If
                    Return VSConstants.S_OK
                End If
            ElseIf itemid >= 0 AndAlso itemid < _items.Count Then
                If propid = CInt(Fix(__VSHPROPID.VSHPROPID_NextSibling)) Then
                    If itemid < _items.Count - 1 Then
                        pvar = CInt(Fix(itemid)) + 1
                    Else
                        Try
                            pvar = CInt(Fix(CodeSweep.VSPackage.HierarchyConstants_Accessor.VSITEMID_NIL))
                        Catch ex As Exception
                            pvar = -1
                        End Try
                    End If
                    Return VSConstants.S_OK
                ElseIf propid = CInt(Fix(__VSHPROPID.VSHPROPID_FirstChild)) Then
                    Try
                        pvar = CInt(Fix(CodeSweep.VSPackage.HierarchyConstants_Accessor.VSITEMID_NIL))
                    Catch ex As Exception
                        pvar = -1
                    End Try
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
    End Class
End Namespace
