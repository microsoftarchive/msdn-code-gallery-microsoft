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
    class MockUIShellOpenDocument : IVsUIShellOpenDocument
    {
        readonly Dictionary<string, MockWindowFrame> _documents = new Dictionary<string, MockWindowFrame>();

        public MockWindowFrame AddDocument(string path)
        {
            MockWindowFrame frame = new MockWindowFrame();
            frame.TextLines = new MockTextLines(path);
            _documents.Add(path, frame);
            return frame;
        }

        #region IVsUIShellOpenDocument Members

        public int AddStandardPreviewer(string pszExePath, string pszDisplayName, int fUseDDE, string pszDDEService, string pszDDETopicOpenURL, string pszDDEItemOpenURL, string pszDDETopicActivate, string pszDDEItemActivate, uint aspAddPreviewerFlags)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetFirstDefaultPreviewer(out string pbstrDefBrowserPath, out int pfIsInternalBrowser, out int pfIsSystemBrowser)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetStandardEditorFactory(uint dwReserved, ref Guid pguidEditorType, string pszMkDocument, ref Guid rguidLogicalView, out string pbstrPhysicalView, out IVsEditorFactory ppEF)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int InitializeEditorInstance(uint grfIEI, IntPtr punkDocView, IntPtr punkDocData, string pszMkDocument, ref Guid rguidEditorType, string pszPhysicalView, ref Guid rguidLogicalView, string pszOwnerCaption, string pszEditorCaption, IVsUIHierarchy pHier, uint itemid, IntPtr punkDocDataExisting, Microsoft.VisualStudio.OLE.Interop.IServiceProvider pSPHierContext, ref Guid rguidCmdUI, out IVsWindowFrame ppWindowFrame)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int IsDocumentInAProject(string pszMkDocument, out IVsUIHierarchy ppUIH, out uint pitemid, out Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP, out int pDocInProj)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int IsDocumentOpen(IVsUIHierarchy pHierCaller, uint itemidCaller, string pszMkDocument, ref Guid rguidLogicalView, uint grfIDO, out IVsUIHierarchy ppHierOpen, uint[] pitemidOpen, out IVsWindowFrame ppWindowFrame, out int pfOpen)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int IsSpecificDocumentViewOpen(IVsUIHierarchy pHierCaller, uint itemidCaller, string pszMkDocument, ref Guid rguidEditorType, string pszPhysicalView, uint grfIDO, out IVsUIHierarchy ppHierOpen, out uint pitemidOpen, out IVsWindowFrame ppWindowFrame, out int pfOpen)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int MapLogicalView(ref Guid rguidEditorType, ref Guid rguidLogicalView, out string pbstrPhysicalView)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int OpenCopyOfStandardEditor(IVsWindowFrame pWindowFrame, ref Guid rguidLogicalView, out IVsWindowFrame ppNewWindowFrame)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int OpenDocumentViaProject(string pszMkDocument, ref Guid rguidLogicalView, out Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP, out IVsUIHierarchy ppHier, out uint pitemid, out IVsWindowFrame ppWindowFrame)
        {
            ppSP = null;
            ppHier = null;
            pitemid = 0;

            if (_documents.ContainsKey(pszMkDocument))
            {
                ppWindowFrame = _documents[pszMkDocument];
                return VSConstants.S_OK;
            }
            else
            {
                ppWindowFrame = null;
                return VSConstants.E_INVALIDARG;
            }
        }

        public int OpenDocumentViaProjectWithSpecific(string pszMkDocument, uint grfEditorFlags, ref Guid rguidEditorType, string pszPhysicalView, ref Guid rguidLogicalView, out Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP, out IVsUIHierarchy ppHier, out uint pitemid, out IVsWindowFrame ppWindowFrame)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int OpenSpecificEditor(uint grfOpenSpecific, string pszMkDocument, ref Guid rguidEditorType, string pszPhysicalView, ref Guid rguidLogicalView, string pszOwnerCaption, IVsUIHierarchy pHier, uint itemid, IntPtr punkDocDataExisting, Microsoft.VisualStudio.OLE.Interop.IServiceProvider pSPHierContext, out IVsWindowFrame ppWindowFrame)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int OpenStandardEditor(uint grfOpenStandard, string pszMkDocument, ref Guid rguidLogicalView, string pszOwnerCaption, IVsUIHierarchy pHier, uint itemid, IntPtr punkDocDataExisting, Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp, out IVsWindowFrame ppWindowFrame)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int OpenStandardPreviewer(uint ospOpenDocPreviewer, string pszURL, VSPREVIEWRESOLUTION resolution, uint dwReserved)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SearchProjectsForRelativePath(uint grfRPS, string pszRelPath, string[] pbstrAbsPath)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
