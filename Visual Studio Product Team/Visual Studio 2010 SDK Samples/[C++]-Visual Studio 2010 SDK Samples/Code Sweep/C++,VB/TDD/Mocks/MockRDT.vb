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
    Friend Class MockRDT
        Implements IVsRunningDocumentTable
#Region "IVsRunningDocumentTable Members"

        Public Function AdviseRunningDocTableEvents(ByVal pSink As IVsRunningDocTableEvents, <System.Runtime.InteropServices.Out()> ByRef pdwCookie As UInteger) As Integer Implements IVsRunningDocumentTable.AdviseRunningDocTableEvents
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function FindAndLockDocument(ByVal dwRDTLockType As UInteger, ByVal pszMkDocument As String, <System.Runtime.InteropServices.Out()> ByRef ppHier As IVsHierarchy, <System.Runtime.InteropServices.Out()> ByRef pitemid As UInteger, <System.Runtime.InteropServices.Out()> ByRef ppunkDocData As IntPtr, <System.Runtime.InteropServices.Out()> ByRef pdwCookie As UInteger) As Integer Implements IVsRunningDocumentTable.FindAndLockDocument
            ppHier = Nothing
            pitemid = CodeSweep.VSPackage.HierarchyConstants_Accessor.VSITEMID_NIL
            ppunkDocData = IntPtr.Zero
            pdwCookie = 0

            Return VSConstants.S_FALSE
        End Function

        Public Function GetDocumentInfo(ByVal docCookie As UInteger, <System.Runtime.InteropServices.Out()> ByRef pgrfRDTFlags As UInteger, <System.Runtime.InteropServices.Out()> ByRef pdwReadLocks As UInteger, <System.Runtime.InteropServices.Out()> ByRef pdwEditLocks As UInteger, <System.Runtime.InteropServices.Out()> ByRef pbstrMkDocument As String, <System.Runtime.InteropServices.Out()> ByRef ppHier As IVsHierarchy, <System.Runtime.InteropServices.Out()> ByRef pitemid As UInteger, <System.Runtime.InteropServices.Out()> ByRef ppunkDocData As IntPtr) As Integer Implements IVsRunningDocumentTable.GetDocumentInfo
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetRunningDocumentsEnum(<System.Runtime.InteropServices.Out()> ByRef ppenum As IEnumRunningDocuments) As Integer Implements IVsRunningDocumentTable.GetRunningDocumentsEnum
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function LockDocument(ByVal grfRDTLockType As UInteger, ByVal dwCookie As UInteger) As Integer Implements IVsRunningDocumentTable.LockDocument
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function ModifyDocumentFlags(ByVal docCookie As UInteger, ByVal grfFlags As UInteger, ByVal fSet As Integer) As Integer Implements IVsRunningDocumentTable.ModifyDocumentFlags
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function NotifyDocumentChanged(ByVal dwCookie As UInteger, ByVal grfDocChanged As UInteger) As Integer Implements IVsRunningDocumentTable.NotifyDocumentChanged
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function NotifyOnAfterSave(ByVal dwCookie As UInteger) As Integer Implements IVsRunningDocumentTable.NotifyOnAfterSave
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function NotifyOnBeforeSave(ByVal dwCookie As UInteger) As Integer Implements IVsRunningDocumentTable.NotifyOnBeforeSave
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function RegisterAndLockDocument(ByVal grfRDTLockType As UInteger, ByVal pszMkDocument As String, ByVal pHier As IVsHierarchy, ByVal itemid As UInteger, ByVal punkDocData As IntPtr, <System.Runtime.InteropServices.Out()> ByRef pdwCookie As UInteger) As Integer Implements IVsRunningDocumentTable.RegisterAndLockDocument
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function RegisterDocumentLockHolder(ByVal grfRDLH As UInteger, ByVal dwCookie As UInteger, ByVal pLockHolder As IVsDocumentLockHolder, <System.Runtime.InteropServices.Out()> ByRef pdwLHCookie As UInteger) As Integer Implements IVsRunningDocumentTable.RegisterDocumentLockHolder
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function RenameDocument(ByVal pszMkDocumentOld As String, ByVal pszMkDocumentNew As String, ByVal pHier As IntPtr, ByVal itemidNew As UInteger) As Integer Implements IVsRunningDocumentTable.RenameDocument
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SaveDocuments(ByVal grfSaveOpts As UInteger, ByVal pHier As IVsHierarchy, ByVal itemid As UInteger, ByVal docCookie As UInteger) As Integer Implements IVsRunningDocumentTable.SaveDocuments
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function UnadviseRunningDocTableEvents(ByVal dwCookie As UInteger) As Integer Implements IVsRunningDocumentTable.UnadviseRunningDocTableEvents
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function UnlockDocument(ByVal grfRDTLockType As UInteger, ByVal dwCookie As UInteger) As Integer Implements IVsRunningDocumentTable.UnlockDocument
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function UnregisterDocumentLockHolder(ByVal dwLHCookie As UInteger) As Integer Implements IVsRunningDocumentTable.UnregisterDocumentLockHolder
            Throw New Exception("The method or operation is not implemented.")
        End Function

#End Region
    End Class
End Namespace
