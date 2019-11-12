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
    Friend Class MockShell
        Implements IVsUIShell
        Public Class PostExecCommandArgs
            Inherits EventArgs
            Public ReadOnly Group As Guid
            Public ReadOnly ID As UInteger
            Public ReadOnly ExecOpt As UInteger
            Public ReadOnly Input As Object
            Public Sub New(ByVal group As Guid, ByVal id As UInteger, ByVal execOpt As UInteger, ByVal input As Object)
                Me.Group = group
                Me.ID = id
                Me.ExecOpt = execOpt
                Me.Input = input
            End Sub
        End Class
        Public Event OnPostExecCommand As EventHandler(Of PostExecCommandArgs)

#Region "IVsUIShell Members"

        Public Function AddNewBFNavigationItem(ByVal pWindowFrame As IVsWindowFrame, ByVal bstrData As String, ByVal punk As Object, ByVal fReplaceCurrent As Integer) As Integer Implements IVsUIShell.AddNewBFNavigationItem
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function CenterDialogOnWindow(ByVal hwndDialog As IntPtr, ByVal hwndParent As IntPtr) As Integer Implements IVsUIShell.CenterDialogOnWindow
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function CreateDocumentWindow(ByVal grfCDW As UInteger, ByVal pszMkDocument As String, ByVal pUIH As IVsUIHierarchy, ByVal itemid As UInteger, ByVal punkDocView As IntPtr, ByVal punkDocData As IntPtr, ByRef rguidEditorType As Guid, ByVal pszPhysicalView As String, ByRef rguidCmdUI As Guid, ByVal psp As Microsoft.VisualStudio.OLE.Interop.IServiceProvider, ByVal pszOwnerCaption As String, ByVal pszEditorCaption As String, ByVal pfDefaultPosition As Integer(), <System.Runtime.InteropServices.Out()> ByRef ppWindowFrame As IVsWindowFrame) As Integer Implements IVsUIShell.CreateDocumentWindow
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function CreateToolWindow(ByVal grfCTW As UInteger, ByVal dwToolWindowId As UInteger, ByVal punkTool As Object, ByRef rclsidTool As Guid, ByRef rguidPersistenceSlot As Guid, ByRef rguidAutoActivate As Guid, ByVal psp As Microsoft.VisualStudio.OLE.Interop.IServiceProvider, ByVal pszCaption As String, ByVal pfDefaultPosition As Integer(), <System.Runtime.InteropServices.Out()> ByRef ppWindowFrame As IVsWindowFrame) As Integer Implements IVsUIShell.CreateToolWindow
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function EnableModeless(ByVal fEnable As Integer) As Integer Implements IVsUIShell.EnableModeless
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function FindToolWindow(ByVal grfFTW As UInteger, ByRef rguidPersistenceSlot As Guid, <System.Runtime.InteropServices.Out()> ByRef ppWindowFrame As IVsWindowFrame) As Integer Implements IVsUIShell.FindToolWindow
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function FindToolWindowEx(ByVal grfFTW As UInteger, ByRef rguidPersistenceSlot As Guid, ByVal dwToolWinId As UInteger, <System.Runtime.InteropServices.Out()> ByRef ppWindowFrame As IVsWindowFrame) As Integer Implements IVsUIShell.FindToolWindowEx
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetAppName(<System.Runtime.InteropServices.Out()> ByRef pbstrAppName As String) As Integer Implements IVsUIShell.GetAppName
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetCurrentBFNavigationItem(<System.Runtime.InteropServices.Out()> ByRef ppWindowFrame As IVsWindowFrame, <System.Runtime.InteropServices.Out()> ByRef pbstrData As String, <System.Runtime.InteropServices.Out()> ByRef ppunk As Object) As Integer Implements IVsUIShell.GetCurrentBFNavigationItem
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetDialogOwnerHwnd(<System.Runtime.InteropServices.Out()> ByRef phwnd As IntPtr) As Integer Implements IVsUIShell.GetDialogOwnerHwnd
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetDirectoryViaBrowseDlg(ByVal pBrowse As VSBROWSEINFOW()) As Integer Implements IVsUIShell.GetDirectoryViaBrowseDlg
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetDocumentWindowEnum(<System.Runtime.InteropServices.Out()> ByRef ppenum As IEnumWindowFrames) As Integer Implements IVsUIShell.GetDocumentWindowEnum
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetErrorInfo(<System.Runtime.InteropServices.Out()> ByRef pbstrErrText As String) As Integer Implements IVsUIShell.GetErrorInfo
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetNextBFNavigationItem(<System.Runtime.InteropServices.Out()> ByRef ppWindowFrame As IVsWindowFrame, <System.Runtime.InteropServices.Out()> ByRef pbstrData As String, <System.Runtime.InteropServices.Out()> ByRef ppunk As Object) As Integer Implements IVsUIShell.GetNextBFNavigationItem
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetOpenFileNameViaDlg(ByVal pOpenFileName As VSOPENFILENAMEW()) As Integer Implements IVsUIShell.GetOpenFileNameViaDlg
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetPreviousBFNavigationItem(<System.Runtime.InteropServices.Out()> ByRef ppWindowFrame As IVsWindowFrame, <System.Runtime.InteropServices.Out()> ByRef pbstrData As String, <System.Runtime.InteropServices.Out()> ByRef ppunk As Object) As Integer Implements IVsUIShell.GetPreviousBFNavigationItem
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetSaveFileNameViaDlg(ByVal pSaveFileName As VSSAVEFILENAMEW()) As Integer Implements IVsUIShell.GetSaveFileNameViaDlg
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetToolWindowEnum(<System.Runtime.InteropServices.Out()> ByRef ppenum As IEnumWindowFrames) As Integer Implements IVsUIShell.GetToolWindowEnum
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetURLViaDlg(ByVal pszDlgTitle As String, ByVal pszStaticLabel As String, ByVal pszHelpTopic As String, <System.Runtime.InteropServices.Out()> ByRef pbstrURL As String) As Integer Implements IVsUIShell.GetURLViaDlg
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function GetVSSysColor(ByVal dwSysColIndex As VSSYSCOLOR, <System.Runtime.InteropServices.Out()> ByRef pdwRGBval As UInteger) As Integer Implements IVsUIShell.GetVSSysColor
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function OnModeChange(ByVal dbgmodeNew As DBGMODE) As Integer Implements IVsUIShell.OnModeChange
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function PostExecCommand(ByRef pguidCmdGroup As Guid, ByVal nCmdID As UInteger, ByVal nCmdexecopt As UInteger, ByRef pvaIn As Object) As Integer Implements IVsUIShell.PostExecCommand
            If OnPostExecCommandEvent IsNot Nothing Then
                RaiseEvent OnPostExecCommand(Me, New PostExecCommandArgs(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn))
            End If
            Return VSConstants.S_OK
        End Function

        Public Function PostSetFocusMenuCommand(ByRef pguidCmdGroup As Guid, ByVal nCmdID As UInteger) As Integer Implements IVsUIShell.PostSetFocusMenuCommand
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function RefreshPropertyBrowser(ByVal dispid As Integer) As Integer Implements IVsUIShell.RefreshPropertyBrowser
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function RemoveAdjacentBFNavigationItem(ByVal rdDir As RemoveBFDirection) As Integer Implements IVsUIShell.RemoveAdjacentBFNavigationItem
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function RemoveCurrentNavigationDupes(ByVal rdDir As RemoveBFDirection) As Integer Implements IVsUIShell.RemoveCurrentNavigationDupes
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function ReportErrorInfo(ByVal hr As Integer) As Integer Implements IVsUIShell.ReportErrorInfo
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SaveDocDataToFile(ByVal grfSave As VSSAVEFLAGS, ByVal pPersistFile As Object, ByVal pszUntitledPath As String, <System.Runtime.InteropServices.Out()> ByRef pbstrDocumentNew As String, <System.Runtime.InteropServices.Out()> ByRef pfCanceled As Integer) As Integer Implements IVsUIShell.SaveDocDataToFile
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetErrorInfo(ByVal hr As Integer, ByVal pszDescription As String, ByVal dwReserved As UInteger, ByVal pszHelpKeyword As String, ByVal pszSource As String) As Integer Implements IVsUIShell.SetErrorInfo
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetForegroundWindow() As Integer Implements IVsUIShell.SetForegroundWindow
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetMRUComboText(ByRef pguidCmdGroup As Guid, ByVal dwCmdID As UInteger, ByVal lpszText As String, ByVal fAddToList As Integer) As Integer Implements IVsUIShell.SetMRUComboText
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetMRUComboTextW(ByVal pguidCmdGroup As Guid(), ByVal dwCmdID As UInteger, ByVal pwszText As String, ByVal fAddToList As Integer) As Integer Implements IVsUIShell.SetMRUComboTextW
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetToolbarVisibleInFullScreen(ByVal pguidCmdGroup As Guid(), ByVal dwToolbarId As UInteger, ByVal fVisibleInFullScreen As Integer) As Integer Implements IVsUIShell.SetToolbarVisibleInFullScreen
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetWaitCursor() As Integer Implements IVsUIShell.SetWaitCursor
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function SetupToolbar(ByVal hwnd As IntPtr, ByVal ptwt As IVsToolWindowToolbar, <System.Runtime.InteropServices.Out()> ByRef pptwth As IVsToolWindowToolbarHost) As Integer Implements IVsUIShell.SetupToolbar
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function ShowContextMenu(ByVal dwCompRole As UInteger, ByRef rclsidActive As Guid, ByVal nMenuId As Integer, ByVal pos As POINTS(), ByVal pCmdTrgtActive As Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget) As Integer Implements IVsUIShell.ShowContextMenu
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function ShowMessageBox(ByVal dwCompRole As UInteger, ByRef rclsidComp As Guid, ByVal pszTitle As String, ByVal pszText As String, ByVal pszHelpFile As String, ByVal dwHelpContextID As UInteger, ByVal msgbtn As OLEMSGBUTTON, ByVal msgdefbtn As OLEMSGDEFBUTTON, ByVal msgicon As OLEMSGICON, ByVal fSysAlert As Integer, <System.Runtime.InteropServices.Out()> ByRef pnResult As Integer) As Integer Implements IVsUIShell.ShowMessageBox
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function TranslateAcceleratorAsACmd(ByVal pMsg As Microsoft.VisualStudio.OLE.Interop.MSG()) As Integer Implements IVsUIShell.TranslateAcceleratorAsACmd
            Throw New Exception("The method or operation is not implemented.")
        End Function

        Public Function UpdateCommandUI(ByVal fImmediateUpdate As Integer) As Integer Implements IVsUIShell.UpdateCommandUI
            Return VSConstants.S_OK
        End Function

        Public Function UpdateDocDataIsDirtyFeedback(ByVal docCookie As UInteger, ByVal fDirty As Integer) As Integer Implements IVsUIShell.UpdateDocDataIsDirtyFeedback
            Throw New Exception("The method or operation is not implemented.")
        End Function

#End Region
    End Class
End Namespace
