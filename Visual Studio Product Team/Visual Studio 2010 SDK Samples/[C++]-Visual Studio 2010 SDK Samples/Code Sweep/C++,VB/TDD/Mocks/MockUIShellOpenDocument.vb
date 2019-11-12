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
    Friend Class MockUIShellOpenDocument
        Implements IVsUIShellOpenDocument
        Private ReadOnly _documents As New Dictionary(Of String, MockWindowFrame)()

        Public Function AddDocument(ByVal path As String) As MockWindowFrame
            Dim frame As New MockWindowFrame()
            frame.TextLines = New MockTextLines(path)
            _documents.Add(path, frame)
            Return frame
        End Function

#Region "IVsUIShellOpenDocument Members"

        Public Function AddStandardPreviewer(ByVal pszExePath As String, ByVal pszDisplayName As String, ByVal fUseDDE As Integer, ByVal pszDDEService As String, ByVal pszDDETopicOpenURL As String, ByVal pszDDEItemOpenURL As String, ByVal pszDDETopicActivate As String, ByVal pszDDEItemActivate As String, ByVal aspAddPreviewerFlags As UInteger) As Integer Implements IVsUIShellOpenDocument.AddStandardPreviewer
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetFirstDefaultPreviewer(<System.Runtime.InteropServices.Out()> ByRef pbstrDefBrowserPath As String, <System.Runtime.InteropServices.Out()> ByRef pfIsInternalBrowser As Integer, <System.Runtime.InteropServices.Out()> ByRef pfIsSystemBrowser As Integer) As Integer Implements IVsUIShellOpenDocument.GetFirstDefaultPreviewer
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetStandardEditorFactory(ByVal dwReserved As UInteger, ByRef pguidEditorType As Guid, ByVal pszMkDocument As String, ByRef rguidLogicalView As Guid, <System.Runtime.InteropServices.Out()> ByRef pbstrPhysicalView As String, <System.Runtime.InteropServices.Out()> ByRef ppEF As IVsEditorFactory) As Integer Implements IVsUIShellOpenDocument.GetStandardEditorFactory
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function InitializeEditorInstance(ByVal grfIEI As UInteger, ByVal punkDocView As IntPtr, ByVal punkDocData As IntPtr, ByVal pszMkDocument As String, ByRef rguidEditorType As Guid, ByVal pszPhysicalView As String, ByRef rguidLogicalView As Guid, ByVal pszOwnerCaption As String, ByVal pszEditorCaption As String, ByVal pHier As IVsUIHierarchy, ByVal itemid As UInteger, ByVal punkDocDataExisting As IntPtr, ByVal pSPHierContext As Microsoft.VisualStudio.OLE.Interop.IServiceProvider, ByRef rguidCmdUI As Guid, <System.Runtime.InteropServices.Out()> ByRef ppWindowFrame As IVsWindowFrame) As Integer Implements IVsUIShellOpenDocument.InitializeEditorInstance
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function IsDocumentInAProject(ByVal pszMkDocument As String, <System.Runtime.InteropServices.Out()> ByRef ppUIH As IVsUIHierarchy, <System.Runtime.InteropServices.Out()> ByRef pitemid As UInteger, <System.Runtime.InteropServices.Out()> ByRef ppSP As Microsoft.VisualStudio.OLE.Interop.IServiceProvider, <System.Runtime.InteropServices.Out()> ByRef pDocInProj As Integer) As Integer Implements IVsUIShellOpenDocument.IsDocumentInAProject
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function IsDocumentOpen(ByVal pHierCaller As IVsUIHierarchy, ByVal itemidCaller As UInteger, ByVal pszMkDocument As String, ByRef rguidLogicalView As Guid, ByVal grfIDO As UInteger, <System.Runtime.InteropServices.Out()> ByRef ppHierOpen As IVsUIHierarchy, ByVal pitemidOpen As UInteger(), <System.Runtime.InteropServices.Out()> ByRef ppWindowFrame As IVsWindowFrame, <System.Runtime.InteropServices.Out()> ByRef pfOpen As Integer) As Integer Implements IVsUIShellOpenDocument.IsDocumentOpen
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function IsSpecificDocumentViewOpen(ByVal pHierCaller As IVsUIHierarchy, ByVal itemidCaller As UInteger, ByVal pszMkDocument As String, ByRef rguidEditorType As Guid, ByVal pszPhysicalView As String, ByVal grfIDO As UInteger, <System.Runtime.InteropServices.Out()> ByRef ppHierOpen As IVsUIHierarchy, <System.Runtime.InteropServices.Out()> ByRef pitemidOpen As UInteger, <System.Runtime.InteropServices.Out()> ByRef ppWindowFrame As IVsWindowFrame, <System.Runtime.InteropServices.Out()> ByRef pfOpen As Integer) As Integer Implements IVsUIShellOpenDocument.IsSpecificDocumentViewOpen
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function MapLogicalView(ByRef rguidEditorType As Guid, ByRef rguidLogicalView As Guid, <System.Runtime.InteropServices.Out()> ByRef pbstrPhysicalView As String) As Integer Implements IVsUIShellOpenDocument.MapLogicalView
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function OpenCopyOfStandardEditor(ByVal pWindowFrame As IVsWindowFrame, ByRef rguidLogicalView As Guid, <System.Runtime.InteropServices.Out()> ByRef ppNewWindowFrame As IVsWindowFrame) As Integer Implements IVsUIShellOpenDocument.OpenCopyOfStandardEditor
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function OpenDocumentViaProject(ByVal pszMkDocument As String, ByRef rguidLogicalView As Guid, <System.Runtime.InteropServices.Out()> ByRef ppSP As Microsoft.VisualStudio.OLE.Interop.IServiceProvider, <System.Runtime.InteropServices.Out()> ByRef ppHier As IVsUIHierarchy, <System.Runtime.InteropServices.Out()> ByRef pitemid As UInteger, <System.Runtime.InteropServices.Out()> ByRef ppWindowFrame As IVsWindowFrame) As Integer Implements IVsUIShellOpenDocument.OpenDocumentViaProject
            ppSP = Nothing
            ppHier = Nothing
            pitemid = 0

            If _documents.ContainsKey(pszMkDocument) Then
                ppWindowFrame = _documents(pszMkDocument)
                Return VSConstants.S_OK
            Else
                ppWindowFrame = Nothing
                Return VSConstants.E_INVALIDARG
            End If
        End Function

        Public Function OpenDocumentViaProjectWithSpecific(ByVal pszMkDocument As String, ByVal grfEditorFlags As UInteger, ByRef rguidEditorType As Guid, ByVal pszPhysicalView As String, ByRef rguidLogicalView As Guid, <System.Runtime.InteropServices.Out()> ByRef ppSP As Microsoft.VisualStudio.OLE.Interop.IServiceProvider, <System.Runtime.InteropServices.Out()> ByRef ppHier As IVsUIHierarchy, <System.Runtime.InteropServices.Out()> ByRef pitemid As UInteger, <System.Runtime.InteropServices.Out()> ByRef ppWindowFrame As IVsWindowFrame) As Integer Implements IVsUIShellOpenDocument.OpenDocumentViaProjectWithSpecific
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function OpenSpecificEditor(ByVal grfOpenSpecific As UInteger, ByVal pszMkDocument As String, ByRef rguidEditorType As Guid, ByVal pszPhysicalView As String, ByRef rguidLogicalView As Guid, ByVal pszOwnerCaption As String, ByVal pHier As IVsUIHierarchy, ByVal itemid As UInteger, ByVal punkDocDataExisting As IntPtr, ByVal pSPHierContext As Microsoft.VisualStudio.OLE.Interop.IServiceProvider, <System.Runtime.InteropServices.Out()> ByRef ppWindowFrame As IVsWindowFrame) As Integer Implements IVsUIShellOpenDocument.OpenSpecificEditor
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function OpenStandardEditor(ByVal grfOpenStandard As UInteger, ByVal pszMkDocument As String, ByRef rguidLogicalView As Guid, ByVal pszOwnerCaption As String, ByVal pHier As IVsUIHierarchy, ByVal itemid As UInteger, ByVal punkDocDataExisting As IntPtr, ByVal psp As Microsoft.VisualStudio.OLE.Interop.IServiceProvider, <System.Runtime.InteropServices.Out()> ByRef ppWindowFrame As IVsWindowFrame) As Integer Implements IVsUIShellOpenDocument.OpenStandardEditor
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function OpenStandardPreviewer(ByVal ospOpenDocPreviewer As UInteger, ByVal pszURL As String, ByVal resolution As VSPREVIEWRESOLUTION, ByVal dwReserved As UInteger) As Integer Implements IVsUIShellOpenDocument.OpenStandardPreviewer
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SearchProjectsForRelativePath(ByVal grfRPS As UInteger, ByVal pszRelPath As String, ByVal pbstrAbsPath As String()) As Integer Implements IVsUIShellOpenDocument.SearchProjectsForRelativePath
            Throw New Exception("The method or operation is not implemented.")
        End Function

#End Region
    End Class
End Namespace
