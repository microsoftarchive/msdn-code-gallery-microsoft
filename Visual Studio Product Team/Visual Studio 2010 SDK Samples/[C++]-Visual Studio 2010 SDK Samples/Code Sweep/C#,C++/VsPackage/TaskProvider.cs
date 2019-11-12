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
using Microsoft.Samples.VisualStudio.CodeSweep.Scanner;
using Microsoft.VisualStudio;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Design;
using System.Drawing;
using Microsoft.Samples.VisualStudio.CodeSweep.VSPackage.Properties;
using System.IO;
using System.Runtime.InteropServices;
using System.Globalization;

namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
{
    class TaskProvider : ITaskProvider, IVsSolutionEvents
    {
        public TaskProvider(IServiceProvider provider)
        {
            _imageList.ImageSize = new Size(9, 16);
            _imageList.Images.AddStrip(Resources.priority);
            _imageList.TransparentColor = Color.FromArgb(0, 255, 0);
            _serviceProvider = provider;
            IVsTaskList taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as IVsTaskList;
            int hr = taskList.RegisterTaskProvider(this, out _cookie);
            Debug.Assert(hr == VSConstants.S_OK, "RegisterTaskProvider did not return S_OK.");
            Debug.Assert(_cookie != 0, "RegisterTaskProvider did not return a nonzero cookie.");

            SetCommandHandlers();

            ListenForProjectUnload();
        }

        #region ITaskProvider Members

        public void AddResult(IScanResult result, string projectFile)
        {
            string fullPath = result.FilePath;
            if (!Path.IsPathRooted(fullPath))
            {
                fullPath = Utilities.AbsolutePathFromRelative(fullPath, Path.GetDirectoryName(projectFile));
            }

            if (result.Scanned)
            {
                foreach (IScanHit hit in result.Results)
                {
                    if (hit.Warning != null && hit.Warning.Length > 0)
                    {
                        // See if we've warned about this term before; if so, don't warn again.
                        if (null == _termsWithDuplicateWarning.Find(
                            delegate(string item)
                            {
                                return String.Compare(item, hit.Term.Text, StringComparison.OrdinalIgnoreCase) == 0;
                            }))
                        {
                            _tasks.Add(new Task(hit.Term.Text, hit.Term.Severity, hit.Term.Class, hit.Warning, "", "", -1, -1, "", "", this, _serviceProvider));
                            _termsWithDuplicateWarning.Add(hit.Term.Text);
                        }
                    }

                    _tasks.Add(new Task(hit.Term.Text, hit.Term.Severity, hit.Term.Class, hit.Term.Comment, hit.Term.RecommendedTerm, fullPath, hit.Line, hit.Column, projectFile, hit.LineText, this, _serviceProvider));
                }
            }
            else
            {
                _tasks.Add(new Task("", 1, "", String.Format(CultureInfo.CurrentUICulture, Resources.FileNotScannedError, fullPath), "", fullPath, -1, -1, projectFile, "", this, _serviceProvider));
            }
            Refresh();
        }

        public void Clear()
        {
            _tasks.Clear();
            Refresh();
        }

        public void SetAsActiveProvider()
        {
            IVsTaskList2 taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as IVsTaskList2;
            Guid ourGuid = _providerGuid;
            int hr = taskList.SetActiveProvider(ref ourGuid);
            Debug.Assert(hr == VSConstants.S_OK, "SetActiveProvider did not return S_OK.");
        }

        public void ShowTaskList()
        {
            IVsUIShell shell = _serviceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell;
            object dummy = null;
            Guid cmdSetGuid = VSConstants.GUID_VSStandardCommandSet97;
            int hr = shell.PostExecCommand(ref cmdSetGuid, (int)VSConstants.VSStd97CmdID.TaskListWindow, 0, ref dummy);
            Debug.Assert(hr == VSConstants.S_OK, "SetActiveProvider did not return S_OK.");
        }

        /// <summary>
        /// Returns an image index between 0 and 2 inclusive corresponding to the specified severity.
        /// </summary>
        public static int GetImageIndexForSeverity(int severity)
        {
            return Math.Max(1, Math.Min(3, severity)) - 1;
        }

        public bool IsShowingIgnoredInstances
        {
            get { return _showingIgnoredInstances; }
        }

        #endregion

        #region IVsTaskProvider Members

        public int EnumTaskItems(out IVsEnumTaskItems ppenum)
        {
            ppenum = new TaskEnumerator(_tasks, IsShowingIgnoredInstances);
            return VSConstants.S_OK;
        }

        [DllImport("comctl32.dll")]
        static extern IntPtr ImageList_Duplicate(IntPtr original);

        public int ImageList(out IntPtr phImageList)
        {
            phImageList = ImageList_Duplicate(_imageList.Handle);
            return VSConstants.S_OK;
        }

        public int OnTaskListFinalRelease(IVsTaskList pTaskList)
        {
            if ((_cookie != 0) && (null != pTaskList))
            {
                int hr = pTaskList.UnregisterTaskProvider(_cookie);
                Debug.Assert(hr == VSConstants.S_OK, "UnregisterTaskProvider did not return S_OK.");
            }

            return VSConstants.S_OK;
        }

        public int ReRegistrationKey(out string pbstrKey)
        {
            pbstrKey = "";
            return VSConstants.E_NOTIMPL;
        }

        public int SubcategoryList(uint cbstr, string[] rgbstr, out uint pcActual)
        {
            pcActual = 0;
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IVsTaskProvider3 Members

        public int GetColumn(int iColumn, VSTASKCOLUMN[] pColumn)
        {
            switch ((Task.TaskFields)iColumn)
            {
                case Task.TaskFields.Class:
                    pColumn[0].bstrCanonicalName = "Class";
                    pColumn[0].bstrHeading = Resources.ClassColumn;
                    pColumn[0].bstrLocalizedName = Resources.ClassColumn;
                    pColumn[0].bstrTip = "";
                    pColumn[0].cxDefaultWidth = 91;
                    pColumn[0].cxMinWidth = 0;
                    pColumn[0].fAllowHide = 1;
                    pColumn[0].fAllowUserSort = 1;
                    pColumn[0].fDescendingSort = 0;
                    pColumn[0].fDynamicSize = 1;
                    pColumn[0].fFitContent = 0;
                    pColumn[0].fMoveable = 1;
                    pColumn[0].fShowSortArrow = 1;
                    pColumn[0].fSizeable = 1;
                    pColumn[0].fVisibleByDefault = 1;
                    pColumn[0].iDefaultSortPriority = -1;
                    pColumn[0].iField = (int)Task.TaskFields.Class;
                    pColumn[0].iImage = -1;
                    break;
                case Task.TaskFields.Comment:
                    pColumn[0].bstrCanonicalName = "Comment";
                    pColumn[0].bstrHeading = Resources.CommentColumn;
                    pColumn[0].bstrLocalizedName = Resources.CommentColumn;
                    pColumn[0].bstrTip = "";
                    pColumn[0].cxDefaultWidth = 400;
                    pColumn[0].cxMinWidth = 0;
                    pColumn[0].fAllowHide = 1;
                    pColumn[0].fAllowUserSort = 1;
                    pColumn[0].fDescendingSort = 0;
                    pColumn[0].fDynamicSize = 1;
                    pColumn[0].fFitContent = 0;
                    pColumn[0].fMoveable = 1;
                    pColumn[0].fShowSortArrow = 1;
                    pColumn[0].fSizeable = 1;
                    pColumn[0].fVisibleByDefault = 1;
                    pColumn[0].iDefaultSortPriority = -1;
                    pColumn[0].iField = (int)Task.TaskFields.Comment;
                    pColumn[0].iImage = -1;
                    break;
                case Task.TaskFields.File:
                    pColumn[0].bstrCanonicalName = "File";
                    pColumn[0].bstrHeading = Resources.FileColumn;
                    pColumn[0].bstrLocalizedName = Resources.FileColumn;
                    pColumn[0].bstrTip = "";
                    pColumn[0].cxDefaultWidth = 92;
                    pColumn[0].cxMinWidth = 0;
                    pColumn[0].fAllowHide = 1;
                    pColumn[0].fAllowUserSort = 1;
                    pColumn[0].fDescendingSort = 0;
                    pColumn[0].fDynamicSize = 0;
                    pColumn[0].fFitContent = 0;
                    pColumn[0].fMoveable = 1;
                    pColumn[0].fShowSortArrow = 1;
                    pColumn[0].fSizeable = 1;
                    pColumn[0].fVisibleByDefault = 1;
                    pColumn[0].iDefaultSortPriority = 2;
                    pColumn[0].iField = (int)Task.TaskFields.File;
                    pColumn[0].iImage = -1;
                    break;
                case Task.TaskFields.Line:
                    pColumn[0].bstrCanonicalName = "Line";
                    pColumn[0].bstrHeading = Resources.LineColumn;
                    pColumn[0].bstrLocalizedName = Resources.LineColumn;
                    pColumn[0].bstrTip = "";
                    pColumn[0].cxDefaultWidth = 63;
                    pColumn[0].cxMinWidth = 0;
                    pColumn[0].fAllowHide = 1;
                    pColumn[0].fAllowUserSort = 1;
                    pColumn[0].fDescendingSort = 0;
                    pColumn[0].fDynamicSize = 0;
                    pColumn[0].fFitContent = 0;
                    pColumn[0].fMoveable = 1;
                    pColumn[0].fShowSortArrow = 1;
                    pColumn[0].fSizeable = 1;
                    pColumn[0].fVisibleByDefault = 1;
                    pColumn[0].iDefaultSortPriority = 3;
                    pColumn[0].iField = (int)Task.TaskFields.Line;
                    pColumn[0].iImage = -1;
                    break;
                case Task.TaskFields.Priority:
                    pColumn[0].bstrCanonicalName = "Priority";
                    pColumn[0].bstrHeading = "!";
                    pColumn[0].bstrLocalizedName = Resources.PriorityColumn;
                    pColumn[0].bstrTip = Resources.PriorityColumn;
                    pColumn[0].cxDefaultWidth = 22;
                    pColumn[0].cxMinWidth = 0;
                    pColumn[0].fAllowHide = 1;
                    pColumn[0].fAllowUserSort = 1;
                    pColumn[0].fDescendingSort = 0;
                    pColumn[0].fDynamicSize = 0;
                    pColumn[0].fFitContent = 0;
                    pColumn[0].fMoveable = 1;
                    pColumn[0].fShowSortArrow = 0;
                    pColumn[0].fSizeable = 1;
                    pColumn[0].fVisibleByDefault = 1;
                    pColumn[0].iDefaultSortPriority = -1;
                    pColumn[0].iField = (int)Task.TaskFields.Priority;
                    pColumn[0].iImage = -1;
                    break;
                case Task.TaskFields.PriorityNumber:
                    pColumn[0].bstrCanonicalName = "Priority Number";
                    pColumn[0].bstrHeading = "!#";
                    pColumn[0].bstrLocalizedName = Resources.PriorityNumberColumn;
                    pColumn[0].bstrTip = Resources.PriorityNumberColumn;
                    pColumn[0].cxDefaultWidth = 50;
                    pColumn[0].cxMinWidth = 0;
                    pColumn[0].fAllowHide = 1;
                    pColumn[0].fAllowUserSort = 1;
                    pColumn[0].fDescendingSort = 0;
                    pColumn[0].fDynamicSize = 0;
                    pColumn[0].fFitContent = 0;
                    pColumn[0].fMoveable = 1;
                    pColumn[0].fShowSortArrow = 0;
                    pColumn[0].fSizeable = 1;
                    pColumn[0].fVisibleByDefault = 0;
                    pColumn[0].iDefaultSortPriority = 0;
                    pColumn[0].iField = (int)Task.TaskFields.PriorityNumber;
                    pColumn[0].iImage = -1;
                    break;
                case Task.TaskFields.Project:
                    pColumn[0].bstrCanonicalName = "Project";
                    pColumn[0].bstrHeading = Resources.ProjectColumn;
                    pColumn[0].bstrLocalizedName = Resources.ProjectColumn;
                    pColumn[0].bstrTip = "";
                    pColumn[0].cxDefaultWidth = 116;
                    pColumn[0].cxMinWidth = 0;
                    pColumn[0].fAllowHide = 1;
                    pColumn[0].fAllowUserSort = 1;
                    pColumn[0].fDescendingSort = 0;
                    pColumn[0].fDynamicSize = 0;
                    pColumn[0].fFitContent = 0;
                    pColumn[0].fMoveable = 1;
                    pColumn[0].fShowSortArrow = 1;
                    pColumn[0].fSizeable = 1;
                    pColumn[0].fVisibleByDefault = 1;
                    pColumn[0].iDefaultSortPriority = 1;
                    pColumn[0].iField = (int)Task.TaskFields.Project;
                    pColumn[0].iImage = -1;
                    break;
                case Task.TaskFields.Replacement:
                    pColumn[0].bstrCanonicalName = "Replacement";
                    pColumn[0].bstrHeading = Resources.ReplacementColumn;
                    pColumn[0].bstrLocalizedName = Resources.ReplacementColumn;
                    pColumn[0].bstrTip = "";
                    pColumn[0].cxDefaultWidth = 140;
                    pColumn[0].cxMinWidth = 0;
                    pColumn[0].fAllowHide = 1;
                    pColumn[0].fAllowUserSort = 1;
                    pColumn[0].fDescendingSort = 0;
                    pColumn[0].fDynamicSize = 0;
                    pColumn[0].fFitContent = 0;
                    pColumn[0].fMoveable = 1;
                    pColumn[0].fShowSortArrow = 1;
                    pColumn[0].fSizeable = 1;
                    pColumn[0].fVisibleByDefault = 0;
                    pColumn[0].iDefaultSortPriority = -1;
                    pColumn[0].iField = (int)Task.TaskFields.Replacement;
                    pColumn[0].iImage = -1;
                    break;
                case Task.TaskFields.Term:
                    pColumn[0].bstrCanonicalName = "Term";
                    pColumn[0].bstrHeading = Resources.TermColumn;
                    pColumn[0].bstrLocalizedName = Resources.TermColumn;
                    pColumn[0].bstrTip = "";
                    pColumn[0].cxDefaultWidth = 103;
                    pColumn[0].cxMinWidth = 0;
                    pColumn[0].fAllowHide = 1;
                    pColumn[0].fAllowUserSort = 1;
                    pColumn[0].fDescendingSort = 0;
                    pColumn[0].fDynamicSize = 1;
                    pColumn[0].fFitContent = 0;
                    pColumn[0].fMoveable = 1;
                    pColumn[0].fShowSortArrow = 1;
                    pColumn[0].fSizeable = 1;
                    pColumn[0].fVisibleByDefault = 1;
                    pColumn[0].iDefaultSortPriority = -1;
                    pColumn[0].iField = (int)Task.TaskFields.Term;
                    pColumn[0].iImage = -1;
                    break;
                default:
                    return VSConstants.E_INVALIDARG;
            }

            return VSConstants.S_OK;
        }

        public int GetColumnCount(out int pnColumns)
        {
            pnColumns = Enum.GetValues(typeof(Task.TaskFields)).Length;
            return VSConstants.S_OK;
        }

        public int GetProviderFlags(out uint tpfFlags)
        {
            tpfFlags = (uint)(__VSTASKPROVIDERFLAGS.TPF_NOAUTOROUTING | __VSTASKPROVIDERFLAGS.TPF_ALWAYSVISIBLE);
            return VSConstants.S_OK;
        }

        public int GetProviderGuid(out Guid pguidProvider)
        {
            pguidProvider = _providerGuid;
            return VSConstants.S_OK;
        }

        public int GetProviderName(out string pbstrName)
        {
            pbstrName = Resources.AppName;
            return VSConstants.S_OK;
        }

        public int GetProviderToolbar(out Guid pguidGroup, out uint pdwID)
        {
            pguidGroup = GuidList.guidVSPackageCmdSet;
            pdwID = 0x2020;
            return VSConstants.S_OK;
        }

        public int GetSurrogateProviderGuid(out Guid pguidProvider)
        {
            pguidProvider = Guid.Empty;
            return VSConstants.E_NOTIMPL;
        }

        public int OnBeginTaskEdit(IVsTaskItem pItem)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnEndTaskEdit(IVsTaskItem pItem, int fCommitChanges, out int pfAllowChanges)
        {
            pfAllowChanges = 0;
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IVsSolutionEvents Members

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            string projFile = ProjectUtilities.GetProjectFilePath(pHierarchy as IVsProject);

            if (!string.IsNullOrEmpty(projFile))
            {
                // Remove all tasks for the project that is being closed.
                for (int i = 0; i < _tasks.Count; ++i)
                {
                    if (_tasks[i].ProjectFile == projFile)
                    {
                        _tasks.RemoveAt(i);
                        --i;
                    }
                }

                Refresh();
            }

            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region Private Members

        static readonly Guid _providerGuid = new Guid("{9ACC41B7-15B4-4dd7-A0F3-0A935D5647F3}");

        List<Task> _tasks = new List<Task>();
        readonly IServiceProvider _serviceProvider;
        readonly uint _cookie;
        List<string> _termsWithDuplicateWarning = new List<string>();
        bool _showingIgnoredInstances = false;
        System.Windows.Forms.ImageList _imageList = new System.Windows.Forms.ImageList();
        uint _solutionEventsCookie = 0;

        private void ListenForProjectUnload()
        {
            IVsSolution solution = _serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;

            int hr = solution.AdviseSolutionEvents(this, out _solutionEventsCookie);
            Debug.Assert(hr == VSConstants.S_OK, "AdviseSolutionEvents did not return S_OK.");
            Debug.Assert(_solutionEventsCookie != 0, "AdviseSolutionEvents did not return a nonzero cookie.");
        }

        private void SetCommandHandlers()
        {
            OleMenuCommandService mcs = _serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (mcs != null)
            {
                CommandID ignoreID = new CommandID(GuidList.guidVSPackageCmdSet, (int)PkgCmdIDList.cmdidIgnore);
                OleMenuCommand ignoreCommand = new OleMenuCommand(new EventHandler(IgnoreSelectedItems), new EventHandler(QueryIgnore), ignoreID);
                mcs.AddCommand(ignoreCommand);
                ignoreCommand.BeforeQueryStatus += new EventHandler(QueryIgnore);

                CommandID dontIgnoreID = new CommandID(GuidList.guidVSPackageCmdSet, (int)PkgCmdIDList.cmdidDoNotIgnore);
                OleMenuCommand dontIgnoreCommand = new OleMenuCommand(new EventHandler(DontIgnoreSelectedItems), new EventHandler(QueryDontIgnore), dontIgnoreID);
                mcs.AddCommand(dontIgnoreCommand);
                dontIgnoreCommand.BeforeQueryStatus += new EventHandler(QueryDontIgnore);

                CommandID showIgnoredID = new CommandID(GuidList.guidVSPackageCmdSet, (int)PkgCmdIDList.cmdidShowIgnoredInstances);
                OleMenuCommand showIgnoredCommand = new OleMenuCommand(new EventHandler(ShowIgnoredInstances), showIgnoredID);
                mcs.AddCommand(showIgnoredCommand);
            }
        }

        List<Task> SelectedTasks()
        {
            List<Task> result = new List<Task>();

            int hr = VSConstants.S_OK;
            IVsTaskList2 taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as IVsTaskList2;

            IVsEnumTaskItems enumerator = null;
            hr = taskList.EnumSelectedItems(out enumerator);
            Debug.Assert(hr == VSConstants.S_OK, "EnumSelectedItems did not return S_OK.");

            IVsTaskItem[] items = new IVsTaskItem[] { null };
            uint[] fetched = new uint[] { 0 };
            for (enumerator.Reset(); enumerator.Next(1, items, fetched) == VSConstants.S_OK && fetched[0] == 1; /*nothing*/)
            {
                Task task = items[0] as Task;
                if (task != null)
                {
                    result.Add(task);
                }
            }

            return result;
        }

        private void QueryIgnore(object sender, EventArgs e)
        {
            OleMenuCommandService mcs = _serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            MenuCommand command = mcs.FindCommand(new CommandID(GuidList.guidVSPackageCmdSet, (int)PkgCmdIDList.cmdidIgnore));

            if (IsActiveProvider)
            {
                bool anyNotIgnored = !SelectedTasks().TrueForAll(
                    delegate(Task task)
                    {
                        return task.Ignored;
                    });

                command.Supported = true;
                command.Enabled = anyNotIgnored;
            }
            else
            {
                command.Supported = false;
            }
        }

        private void QueryDontIgnore(object sender, EventArgs e)
        {
            OleMenuCommandService mcs = _serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            MenuCommand command = mcs.FindCommand(new CommandID(GuidList.guidVSPackageCmdSet, (int)PkgCmdIDList.cmdidDoNotIgnore));

            if (IsActiveProvider)
            {
                bool anyIgnored = !SelectedTasks().TrueForAll(
                    delegate(Task task)
                    {
                        return !task.Ignored;
                    });

                command.Supported = true;
                command.Enabled = anyIgnored;
            }
            else
            {
                command.Supported = false;
            }
        }

        private bool IsActiveProvider
        {
            get
            {
                IVsTaskList2 taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as IVsTaskList2;
                IVsTaskProvider activeProvider = null;
                int hr = taskList.GetActiveProvider(out activeProvider);
                Debug.Assert(hr == VSConstants.S_OK, "GetActiveProvider did not return S_OK.");
                return activeProvider == this;
            }
        }

        private void IgnoreSelectedItems(object sender, EventArgs e)
        {
            IgnoreSelectedItems(true);
        }

        private void DontIgnoreSelectedItems(object sender, EventArgs e)
        {
            IgnoreSelectedItems(false);
        }

        private void IgnoreSelectedItems(bool ignore)
        {
            int hr = VSConstants.S_OK;
            IVsTaskList2 taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as IVsTaskList2;

            IVsEnumTaskItems enumerator = null;
            hr = taskList.EnumSelectedItems(out enumerator);
            Debug.Assert(hr == VSConstants.S_OK, "EnumSelectedItems did not return S_OK.");

            IVsTaskItem[] items = new IVsTaskItem[] { null };
            uint[] fetched = new uint[] { 0 };
            for (enumerator.Reset(); enumerator.Next(1, items, fetched) == VSConstants.S_OK && fetched[0] == 1; /*nothing*/)
            {
                Task task = items[0] as Task;
                if (task != null)
                {
                    task.Ignored = ignore;
                }
            }

            Refresh();
        }

        private void ShowIgnoredInstances(object sender, EventArgs e)
        {
            _showingIgnoredInstances = !_showingIgnoredInstances;

            OleMenuCommandService mcs = _serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            mcs.FindCommand(new CommandID(GuidList.guidVSPackageCmdSet, (int)PkgCmdIDList.cmdidShowIgnoredInstances)).Checked = _showingIgnoredInstances;

            Refresh();
        }

        private void Refresh()
        {
            IVsTaskList taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as IVsTaskList;
            int hr = taskList.RefreshTasks(_cookie);
            Debug.Assert(hr == VSConstants.S_OK, "RefreshTasks did not return S_OK.");
        }

        #endregion
    }
}
