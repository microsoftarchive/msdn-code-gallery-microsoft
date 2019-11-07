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
using CodeSweep = Microsoft.Samples.VisualStudio.CodeSweep;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    class MockRDT : IVsRunningDocumentTable
    {
        #region IVsRunningDocumentTable Members

        public int AdviseRunningDocTableEvents(IVsRunningDocTableEvents pSink, out uint pdwCookie)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int FindAndLockDocument(uint dwRDTLockType, string pszMkDocument, out IVsHierarchy ppHier, out uint pitemid, out IntPtr ppunkDocData, out uint pdwCookie)
        {
            ppHier = null;
            pitemid = CodeSweep.VSPackage.HierarchyConstants_Accessor.VSITEMID_NIL;
            ppunkDocData = IntPtr.Zero;
            pdwCookie = 0;

            return VSConstants.S_FALSE;
        }

        public int GetDocumentInfo(uint docCookie, out uint pgrfRDTFlags, out uint pdwReadLocks, out uint pdwEditLocks, out string pbstrMkDocument, out IVsHierarchy ppHier, out uint pitemid, out IntPtr ppunkDocData)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetRunningDocumentsEnum(out IEnumRunningDocuments ppenum)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int LockDocument(uint grfRDTLockType, uint dwCookie)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int ModifyDocumentFlags(uint docCookie, uint grfFlags, int fSet)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int NotifyDocumentChanged(uint dwCookie, uint grfDocChanged)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int NotifyOnAfterSave(uint dwCookie)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int NotifyOnBeforeSave(uint dwCookie)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int RegisterAndLockDocument(uint grfRDTLockType, string pszMkDocument, IVsHierarchy pHier, uint itemid, IntPtr punkDocData, out uint pdwCookie)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int RegisterDocumentLockHolder(uint grfRDLH, uint dwCookie, IVsDocumentLockHolder pLockHolder, out uint pdwLHCookie)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int RenameDocument(string pszMkDocumentOld, string pszMkDocumentNew, IntPtr pHier, uint itemidNew)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SaveDocuments(uint grfSaveOpts, IVsHierarchy pHier, uint itemid, uint docCookie)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int UnadviseRunningDocTableEvents(uint dwCookie)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int UnlockDocument(uint grfRDTLockType, uint dwCookie)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int UnregisterDocumentLockHolder(uint dwLHCookie)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
