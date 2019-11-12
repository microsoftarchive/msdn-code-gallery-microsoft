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
using System.IO;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
{
    class MockSolution : IVsSolution, IVsSolution3, IVsHierarchy
    {
        readonly List<MockIVsProject> _projects = new List<MockIVsProject>();
        readonly List<IVsSolutionEvents> _eventSinks = new List<IVsSolutionEvents>();
        string _solutionFile;

        ~MockSolution()
        {
            // Cleanup the solution and storage file from disk
            List<string> _items = new List<string>();
            _items.Add(_solutionFile);
            _items.Add(_solutionFile + ".storage");

            foreach (string file in _items)
            {
                if (File.Exists(file))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
            }
        }

        public string SolutionFile
        {
            get { return _solutionFile; }
            set { _solutionFile = value.ToLower(); }
        }

        public void AddProject(MockIVsProject project)
        {
            if (_solutionFile != null)
            {
                _projects.Add(project);
                foreach (IVsSolutionEvents sink in _eventSinks)
                {
                    if (sink != null)
                    {
                        sink.OnAfterOpenProject(project, 1);
                    }
                }
            }
        }

        public IEnumerable<MockIVsProject> Projects
        {
            get { return _projects; }
        }

        #region IVsSolution Members

        public int AddVirtualProject(IVsHierarchy pHierarchy, uint grfAddVPFlags)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int AddVirtualProjectEx(IVsHierarchy pHierarchy, uint grfAddVPFlags, ref Guid rguidProjectID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int AdviseSolutionEvents(IVsSolutionEvents pSink, out uint pdwCookie)
        {
            _eventSinks.Add(pSink);
            pdwCookie = (uint)_eventSinks.Count;
            return VSConstants.S_OK;
        }

        public int CanCreateNewProjectAtLocation(int fCreateNewSolution, string pszFullProjectFilePath, out int pfCanCreate)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int CloseSolutionElement(uint grfCloseOpts, IVsHierarchy pHier, uint docCookie)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int CreateNewProjectViaDlg(string pszExpand, string pszSelect, uint dwReserved)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int CreateProject(ref Guid rguidProjectType, string lpszMoniker, string lpszLocation, string lpszName, uint grfCreateFlags, ref Guid iidProject, out IntPtr ppProject)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int CreateSolution(string lpszLocation, string lpszName, uint grfCreateFlags)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GenerateNextDefaultProjectName(string pszBaseName, string pszLocation, out string pbstrProjectName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GenerateUniqueProjectName(string lpszRoot, out string pbstrProjectName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetGuidOfProject(IVsHierarchy pHierarchy, out Guid pguidProjectID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetItemInfoOfProjref(string pszProjref, int propid, out object pvar)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetItemOfProjref(string pszProjref, out IVsHierarchy ppHierarchy, out uint pitemid, out string pbstrUpdatedProjref, VSUPDATEPROJREFREASON[] puprUpdateReason)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetProjectEnum(uint grfEnumFlags, ref Guid rguidEnumOnlyThisType, out IEnumHierarchies ppenum)
        {
            ppenum = new MockEnumHierarchies(_projects);
            return VSConstants.S_OK;
        }

        public int GetProjectFactory(uint dwReserved, Guid[] pguidProjectType, string pszMkProject, out IVsProjectFactory ppProjectFactory)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetProjectFilesInSolution(uint grfGetOpts, uint cProjects, string[] rgbstrProjectNames, out uint pcProjectsFetched)
        {
            if (cProjects == 0)
            {
                pcProjectsFetched = (uint)_projects.Count;
            }
            else
            {
                for (int i = 0; i < cProjects; ++i)
                {
                    rgbstrProjectNames[i] = _projects[i].ProjectFile;
                }
                pcProjectsFetched = cProjects;
            }

            return VSConstants.S_OK;
        }

        public int GetProjectInfoOfProjref(string pszProjref, int propid, out object pvar)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetProjectOfGuid(ref Guid rguidProjectID, out IVsHierarchy ppHierarchy)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetProjectOfProjref(string pszProjref, out IVsHierarchy ppHierarchy, out string pbstrUpdatedProjref, VSUPDATEPROJREFREASON[] puprUpdateReason)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetProjectOfUniqueName(string pszUniqueName, out IVsHierarchy ppHierarchy)
        {
            // Unique name of projects are in general based on the solution.
            // They can be the project name relativized to the solution's folder, or the full project path
            // when such relativization cannot be done (e.g. project on different drive or web project, etc) 

            // However, for our testing purpose, the full project file path was used and should be good enough
            for (int iProject = 0; iProject < _projects.Count; iProject++)
            {
                if (pszUniqueName == _projects[iProject].ProjectFile)
                {
                    ppHierarchy = _projects[iProject] as IVsHierarchy;
                    if (ppHierarchy != null)
                    {
                        return VSConstants.S_OK;
                    }
                    break;
                }
            }

            ppHierarchy = null;
            return VSConstants.E_FAIL;
        }

        public int GetProjectTypeGuid(uint dwReserved, string pszMkProject, out Guid pguidProjectType)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetProjrefOfItem(IVsHierarchy pHierarchy, uint itemid, out string pbstrProjref)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetProjrefOfProject(IVsHierarchy pHierarchy, out string pbstrProjref)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetProperty(int propid, out object pvar)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetSolutionInfo(out string pbstrSolutionDirectory, out string pbstrSolutionFile, out string pbstrUserOptsFile)
        {
            if (_solutionFile != null && _solutionFile.Length > 0)
            {
                pbstrSolutionFile = _solutionFile;
                pbstrSolutionDirectory = Path.GetDirectoryName(_solutionFile);
                pbstrUserOptsFile = Path.Combine(pbstrSolutionDirectory, Path.GetFileNameWithoutExtension(_solutionFile) + ".suo");

                return VSConstants.S_OK;
            }
            else
            {
                pbstrSolutionFile = null;
                pbstrSolutionDirectory = null;
                pbstrUserOptsFile = null;
                return VSConstants.S_FALSE;
            }
        }

        public int GetUniqueNameOfProject(IVsHierarchy pHierarchy, out string pbstrUniqueName)
        {
            // Unique name of projects are in general based on the solution.
            // They can be the project name relativized to the solution's folder, or the full project path
            // when such relativization cannot be done (e.g. project on different drive or web project, etc) 
            
            // However, for our testing purpose, returning the full project file path should be good enough
            for (int iProject = 0; iProject < _projects.Count; iProject++)
            {
                if (pHierarchy == _projects[iProject] as IVsHierarchy)
                {
                    pbstrUniqueName = _projects[iProject].ProjectFile;
                    return VSConstants.S_OK;
                }
            }

            pbstrUniqueName = null;
            return VSConstants.E_FAIL;
        }

        public int GetVirtualProjectFlags(IVsHierarchy pHierarchy, out uint pgrfAddVPFlags)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int OnAfterRenameProject(IVsProject pProject, string pszMkOldName, string pszMkNewName, uint dwReserved)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int OpenSolutionFile(uint grfOpenOpts, string pszFilename)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int OpenSolutionViaDlg(string pszStartDirectory, int fDefaultToAllProjectsFilter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int QueryEditSolutionFile(out uint pdwEditResult)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int QueryRenameProject(IVsProject pProject, string pszMkOldName, string pszMkNewName, uint dwReserved, out int pfRenameCanContinue)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int RemoveVirtualProject(IVsHierarchy pHierarchy, uint grfRemoveVPFlags)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int SaveSolutionElement(uint grfSaveOpts, IVsHierarchy pHier, uint docCookie)
        {
            // Report success when saving files or projects.
            return VSConstants.S_OK;
        }

        public int SetProperty(int propid, object var)
        {
            throw new Exception("Other properties are not supported.");
        }

        public int UnadviseSolutionEvents(uint dwCookie)
        {
            _eventSinks[(int)dwCookie - 1] = null;
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsSolution3 Members

        public int CheckForAndSaveDeferredSaveSolution(int fCloseSolution, string pszMessage, string pszTitle, uint grfFlags)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int CreateNewProjectViaDlgEx(string pszDlgTitle, string pszTemplateDir, string pszExpand, string pszSelect, string pszHelpTopic, uint cnpvdeFlags, IVsBrowseProjectLocation pBrowse)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int GetUniqueUINameOfProject(IVsHierarchy pHierarchy, out string pbstrUniqueName)
        {
            MockIVsProject project = pHierarchy as MockIVsProject;

            pbstrUniqueName = "Unique name of " + project.ProjectFile;
            return VSConstants.S_OK;
        }

        public int UpdateProjectFileLocationForUpgrade(string pszCurrentLocation, string pszUpgradedLocation)
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
                    if (_projects.Count > 0)
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
            }
            else if (itemid >= 0 && itemid < _projects.Count)
            {
                if (propid == (int)__VSHPROPID.VSHPROPID_NextSibling)
                {
                    if (itemid < _projects.Count - 1)
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
            if (propid == (int)__VSHPROPID.VSHPROPID_StateIconIndex)
            {
                return VSConstants.S_OK;
            }

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
    }
}
