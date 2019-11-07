/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
{
    class MockIVsProject : IVsProject, IVsProject2, IVsHierarchy, IVsSccProject2 
    {
        readonly List<string> _items = new List<string>();
        readonly string _projFile;

        public MockIVsProject(string projFile)
        {
            _projFile = projFile.ToLower();
        }

        ~MockIVsProject()
        {
            // Cleanup the projects and files from disk
            _items.Add(_projFile);
            _items.Add(_projFile + ".storage");

            foreach (string file in _items)
            {
                if (File.Exists(file))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
            }
        }

        public string ProjectFile
        {
            get { return _projFile; }
        }

        public IList<string> ProjectItems
        {
            get { return _items; }
        }

        public void AddItem(string itemName)
        {
            _items.Add(itemName.ToLower());
        }

        public void RenameItem(string itemNameOld, string itemNameNew)
        {
            for (int iIndex = 0; iIndex < _items.Count; iIndex++)
            {
                if (itemNameOld.CompareTo(_items[iIndex]) == 0)
                {
                    _items[iIndex] = itemNameNew.ToLower();
                    break;
                }
            }
        }

        public void RemoveItem(string itemName)
        {
            for (int iIndex = 0; iIndex < _items.Count; iIndex++)
            {
                if (itemName.CompareTo(_items[iIndex]) == 0)
                {
                    _items.RemoveAt(iIndex);
                    break;
                }
            }
        }

        #region IVsProject Members

        public int AddItem(uint itemidLoc, VSADDITEMOPERATION dwAddItemOperation, string pszItemName, uint cFilesToOpen, string[] rgpszFilesToOpen, IntPtr hwndDlgOwner, VSADDRESULT[] pResult)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GenerateUniqueItemName(uint itemidLoc, string pszExt, string pszSuggestedRoot, out string pbstrItemName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetItemContext(uint itemid, out Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetMkDocument(uint itemid, out string pbstrMkDocument)
        {
            if (itemid == VSConstants.VSITEMID_ROOT)
            {
                pbstrMkDocument = _projFile;
                return VSConstants.S_OK;
            }
            else if (itemid >= 0 && itemid < _items.Count)
            {
                pbstrMkDocument = _items[(int)itemid];
                return VSConstants.S_OK;
            }
            throw new Exception("The method or operation is not implemented.");
        }

        public int IsDocumentInProject(string pszMkDocument, out int pfFound, VSDOCUMENTPRIORITY[] pdwPriority, out uint pitemid)
        {
            pfFound = 0;
            pitemid = VSConstants.VSITEMID_NIL;
            pszMkDocument = pszMkDocument.ToLower();

            if (pszMkDocument.CompareTo(_projFile) == 0)
            {
                pfFound = 1;
                pitemid = VSConstants.VSITEMID_ROOT;
            }
            else
            {
                for (int iIndex = 0; iIndex < _items.Count; iIndex++)
                {
                    if (pszMkDocument.CompareTo(_items[iIndex]) == 0)
                    {
                        pfFound = 1;
                        pitemid = (uint)iIndex;
                        break;
                    }
                }
            }

            return VSConstants.S_OK;
        }

        public int OpenItem(uint itemid, ref Guid rguidLogicalView, IntPtr punkDocDataExisting, out IVsWindowFrame ppWindowFrame)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IVsProject2 Members

        public int RemoveItem(uint dwReserved, uint itemid, out int pfResult)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int ReopenItem ( uint itemid, ref Guid rguidEditorType, string pszPhysicalView, ref Guid rguidLogicalView, IntPtr punkDocDataExisting, out IVsWindowFrame ppWindowFrame)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IVsHierarchy Members

        public int AdviseHierarchyEvents(IVsHierarchyEvents pEventSink, out uint pdwCookie)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Close()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetCanonicalName(uint itemid, out string pbstrName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetGuidProperty(uint itemid, int propid, out Guid pguid)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetNestedHierarchy(uint itemid, ref Guid iidHierarchyNested, out IntPtr ppHierarchyNested, out uint pitemidNested)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetProperty(uint itemid, int propid, out object pvar)
        {
            if (itemid == VSConstants.VSITEMID_ROOT)
            {
                if (propid == (int)__VSHPROPID.VSHPROPID_FirstChild)
                {
                    if (_items.Count > 0)
                    {
                        pvar = 0;
                    }
                    else
                    {
                        unchecked
                        {
                            pvar = (int)VSConstants.VSITEMID_NIL;
                        }
                    }
                    return VSConstants.S_OK;
                }
                else if (propid == (int)__VSHPROPID.VSHPROPID_Name)
                {
                    pvar = Path.GetFileNameWithoutExtension(ProjectFile);
                    return VSConstants.S_OK;
                }
                else if (propid == (int)__VSHPROPID2.VSHPROPID_Container)
                {
                    pvar = (_items.Count > 0);
                    return VSConstants.S_OK;
                }
                else if (propid == (int)__VSHPROPID.VSHPROPID_Expandable)
                {
                    pvar = (int)((_items.Count > 0) ? 1 : 0);
                    return VSConstants.S_OK;
                }
            }
            else if (itemid >= 0 && itemid < _items.Count)
            {
                if (propid == (int)__VSHPROPID.VSHPROPID_NextSibling)
                {
                    if (itemid < _items.Count - 1)
                    {
                        pvar = (int)itemid + 1;
                    }
                    else
                    {
                        unchecked
                        {
                            pvar = (int)VSConstants.VSITEMID_NIL;
                        }
                    }
                    return VSConstants.S_OK;
                }
                else if (propid == (int)__VSHPROPID.VSHPROPID_FirstChild)
                {
                    unchecked
                    {
                        pvar = (int)VSConstants.VSITEMID_NIL;
                    }
                    return VSConstants.S_OK;
                }
                else if (propid == (int)__VSHPROPID.VSHPROPID_Name)
                {
                    pvar = Path.GetFileNameWithoutExtension(ProjectItems[(int)itemid]);
                    return VSConstants.S_OK;
                }
                else if (propid == (int)__VSHPROPID2.VSHPROPID_Container)
                {
                    // The project support only files, which are not expandable like folders, 
                    // and they are not unexpandable containers (like the MyProject node in a VB app)
                    pvar = false;
                    return VSConstants.S_OK;
                }
                else if (propid == (int)__VSHPROPID.VSHPROPID_Expandable)
                {
                    pvar = (int)0;
                    return VSConstants.S_OK;
                }
            }
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetSite(out Microsoft.VisualStudio.OLE.Interop.IServiceProvider ppSP)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int ParseCanonicalName(string pszName, out uint pitemid)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int QueryClose(out int pfCanClose)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetGuidProperty(uint itemid, int propid, ref Guid rguid)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetProperty(uint itemid, int propid, object var)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SetSite(Microsoft.VisualStudio.OLE.Interop.IServiceProvider psp)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int UnadviseHierarchyEvents(uint dwCookie)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Unused0()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Unused1()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Unused2()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Unused3()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Unused4()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IVsSccProject2 Members

        public int GetSccFiles(uint itemid, CALPOLESTR[] pCaStringsOut, CADWORD[] pCaFlagsOut)
        {
            if ((null == pCaStringsOut) || (0 == pCaStringsOut.Length))
                throw new ArgumentNullException();
            if ((null == pCaFlagsOut) || (0 == pCaFlagsOut.Length))
                throw new ArgumentNullException();

            pCaStringsOut[0] = new CALPOLESTR();
            pCaStringsOut[0].cElems = 0;
            pCaStringsOut[0].pElems = IntPtr.Zero;

            pCaFlagsOut[0] = new CADWORD();
            pCaFlagsOut[0].cElems = 0;
            pCaFlagsOut[0].pElems = IntPtr.Zero;

            string fileForNode = null;
            if (itemid == VSConstants.VSITEMID_ROOT)
            {
                fileForNode = _projFile;
            }
            else if (itemid >= 0 && itemid < _items.Count)
            {
                fileForNode = _items[(int)itemid];
            }

            if (fileForNode != null)
            {
                // There is only one scc controllable file per each hierarchy node
                pCaStringsOut[0].cElems = 1;
                pCaStringsOut[0].pElems = Marshal.AllocCoTaskMem(IntPtr.Size);
                Marshal.WriteIntPtr(pCaStringsOut[0].pElems, Marshal.StringToCoTaskMemUni(fileForNode));

                pCaFlagsOut[0].cElems = 1;
                pCaFlagsOut[0].pElems = Marshal.AllocCoTaskMem(sizeof(Int32));
                Marshal.WriteInt32(pCaFlagsOut[0].pElems, 0);
            }

            return VSConstants.S_OK;
        }

        public int GetSccSpecialFiles(uint itemid, string pszSccFile, CALPOLESTR[] pCaStringsOut, CADWORD[] pCaFlagsOut)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int SccGlyphChanged(int cAffectedNodes, uint[] rgitemidAffectedNodes, VsStateIcon[] rgsiNewGlyphs, uint[] rgdwNewSccStatus)
        {
            return VSConstants.S_OK;
        }

        public int SetSccLocation(string pszSccProjectName, string pszSccAuxPath, string pszSccLocalPath, string pszSccProvider)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
