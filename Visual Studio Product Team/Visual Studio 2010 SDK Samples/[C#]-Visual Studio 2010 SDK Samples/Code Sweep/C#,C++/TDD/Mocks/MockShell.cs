/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    class MockShell : IVsUIShell
    {
        public class PostExecCommandArgs : EventArgs
        {
            public readonly Guid Group;
            public readonly uint ID;
            public readonly uint ExecOpt;
            public readonly object Input;
            public PostExecCommandArgs(Guid group, uint id, uint execOpt, object input)
            {
                Group = group;
                ID = id;
                ExecOpt = execOpt;
                Input = input;
            }
        }
        public event EventHandler<PostExecCommandArgs> OnPostExecCommand;

        #region IVsUIShell Members

        public int AddNewBFNavigationItem(IVsWindowFrame pWindowFrame, string bstrData, object punk, int fReplaceCurrent)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int CenterDialogOnWindow(IntPtr hwndDialog, IntPtr hwndParent)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int CreateDocumentWindow(uint grfCDW, string pszMkDocument, IVsUIHierarchy pUIH, uint itemid, IntPtr punkDocView, IntPtr punkDocData, ref Guid rguidEditorType, string pszPhysicalView, ref Guid rguidCmdUI, Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp, string pszOwnerCaption, string pszEditorCaption, int[] pfDefaultPosition, out IVsWindowFrame ppWindowFrame)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int CreateToolWindow(uint grfCTW, uint dwToolWindowId, object punkTool, ref Guid rclsidTool, ref Guid rguidPersistenceSlot, ref Guid rguidAutoActivate, Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp, string pszCaption, int[] pfDefaultPosition, out IVsWindowFrame ppWindowFrame)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int EnableModeless(int fEnable)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int FindToolWindow(uint grfFTW, ref Guid rguidPersistenceSlot, out IVsWindowFrame ppWindowFrame)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int FindToolWindowEx(uint grfFTW, ref Guid rguidPersistenceSlot, uint dwToolWinId, out IVsWindowFrame ppWindowFrame)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetAppName(out string pbstrAppName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetCurrentBFNavigationItem(out IVsWindowFrame ppWindowFrame, out string pbstrData, out object ppunk)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetDialogOwnerHwnd(out IntPtr phwnd)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetDirectoryViaBrowseDlg(VSBROWSEINFOW[] pBrowse)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetDocumentWindowEnum(out IEnumWindowFrames ppenum)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetErrorInfo(out string pbstrErrText)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetNextBFNavigationItem(out IVsWindowFrame ppWindowFrame, out string pbstrData, out object ppunk)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetOpenFileNameViaDlg(VSOPENFILENAMEW[] pOpenFileName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetPreviousBFNavigationItem(out IVsWindowFrame ppWindowFrame, out string pbstrData, out object ppunk)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetSaveFileNameViaDlg(VSSAVEFILENAMEW[] pSaveFileName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetToolWindowEnum(out IEnumWindowFrames ppenum)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetURLViaDlg(string pszDlgTitle, string pszStaticLabel, string pszHelpTopic, out string pbstrURL)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetVSSysColor(VSSYSCOLOR dwSysColIndex, out uint pdwRGBval)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int OnModeChange(DBGMODE dbgmodeNew)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int PostExecCommand(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, ref object pvaIn)
        {
            if (OnPostExecCommand != null)
            {
                OnPostExecCommand(this, new PostExecCommandArgs(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn));
            }
            return VSConstants.S_OK;
        }

        public int PostSetFocusMenuCommand(ref Guid pguidCmdGroup, uint nCmdID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int RefreshPropertyBrowser(int dispid)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int RemoveAdjacentBFNavigationItem(RemoveBFDirection rdDir)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int RemoveCurrentNavigationDupes(RemoveBFDirection rdDir)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int ReportErrorInfo(int hr)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SaveDocDataToFile(VSSAVEFLAGS grfSave, object pPersistFile, string pszUntitledPath, out string pbstrDocumentNew, out int pfCanceled)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetErrorInfo(int hr, string pszDescription, uint dwReserved, string pszHelpKeyword, string pszSource)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetForegroundWindow()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetMRUComboText(ref Guid pguidCmdGroup, uint dwCmdID, string lpszText, int fAddToList)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetMRUComboTextW(Guid[] pguidCmdGroup, uint dwCmdID, string pwszText, int fAddToList)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetToolbarVisibleInFullScreen(Guid[] pguidCmdGroup, uint dwToolbarId, int fVisibleInFullScreen)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetWaitCursor()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetupToolbar(IntPtr hwnd, IVsToolWindowToolbar ptwt, out IVsToolWindowToolbarHost pptwth)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int ShowContextMenu(uint dwCompRole, ref Guid rclsidActive, int nMenuId, POINTS[] pos, Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget pCmdTrgtActive)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int ShowMessageBox(uint dwCompRole, ref Guid rclsidComp, string pszTitle, string pszText, string pszHelpFile, uint dwHelpContextID, OLEMSGBUTTON msgbtn, OLEMSGDEFBUTTON msgdefbtn, OLEMSGICON msgicon, int fSysAlert, out int pnResult)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int TranslateAcceleratorAsACmd(Microsoft.VisualStudio.OLE.Interop.MSG[] pMsg)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int UpdateCommandUI(int fImmediateUpdate)
        {
            return VSConstants.S_OK;
        }

        public int UpdateDocDataIsDirtyFeedback(uint docCookie, int fDirty)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
