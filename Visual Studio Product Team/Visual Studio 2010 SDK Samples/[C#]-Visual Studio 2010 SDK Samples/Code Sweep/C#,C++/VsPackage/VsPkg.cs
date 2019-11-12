/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using MsVsShell = Microsoft.VisualStudio.Shell;
using EnvDTE;
using System.Collections.Generic;
using Microsoft.VisualStudio;

namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
{
    /////////////////////////////////////////////////////////////////////////////
    // VSPackage
    [MsVsShell.InstalledProductRegistration("#100", "#102", "1.0.0.0", IconResourceID = 400)]
    [MsVsShell.ProvideMenuResource(1000, 1)]
    [Guid("2b621c1e-60a3-48c5-a07d-0ad6d3dd3417")]
    [MsVsShell.ProvideAutoLoad("ADFC4E66-0397-11D1-9F4E-00A0C911004F")] //UICONTEXT_SolutionHasSingleProject
    public sealed class VSPackage : MsVsShell.Package
    {
        public VSPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
            Factory.ServiceProvider = this;
            Factory.GetBuildManager().CreatePerUserFilesAsNecessary();
        }

        public static string GetResourceString(string resourceName)
        {
            string resourceValue;
            IVsResourceManager resourceManager = (IVsResourceManager)GetGlobalService(typeof(SVsResourceManager));
            Guid packageGuid = typeof(VSPackage).GUID;
            int hr = resourceManager.LoadResourceString(ref packageGuid, -1, resourceName, out resourceValue);
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr);
            return resourceValue;
        }

        public static string GetResourceString(int resourceID)
        {
            return GetResourceString(string.Format(CultureInfo.InvariantCulture, "#{0}", resourceID));
        }

        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            MsVsShell.OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as MsVsShell.OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidVSPackageCmdSet, (int)PkgCmdIDList.cmdidConfig);
                OleMenuCommand menuItem = new OleMenuCommand(new EventHandler(MenuItemCallback), menuCommandID);
                menuItem.BeforeQueryStatus += new EventHandler(QueryStatus);
                mcs.AddCommand( menuItem );

                // Create the commands for the tasklist toolbar.
                menuCommandID = new CommandID(GuidList.guidVSPackageCmdSet, (int)PkgCmdIDList.cmdidStopScan);
                menuItem = new OleMenuCommand(new EventHandler(StopScan), menuCommandID);
                menuItem.Enabled = false;
                mcs.AddCommand(menuItem);

                menuCommandID = new CommandID(GuidList.guidVSPackageCmdSet, (int)PkgCmdIDList.cmdidRepeatLastScan);
                menuItem = new OleMenuCommand(new EventHandler(RepeatLastScan), menuCommandID);
                menuItem.Enabled = false;
                mcs.AddCommand(menuItem);
            }

            Factory.GetBuildManager().IsListeningToBuildEvents = true;
            Factory.GetBuildManager().BuildStarted += new EmptyEvent(BuildManager_BuildStarted);
            Factory.GetBuildManager().BuildStopped += new EmptyEvent(BuildManager_BuildStopped);
            Factory.GetBackgroundScanner().Started += new EventHandler(BackgroundScanner_Started);
            Factory.GetBackgroundScanner().Stopped += new EventHandler(BackgroundScanner_Stopped);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Dispose() of: {0}", this.ToString()));
                    Factory.GetBuildManager().IsListeningToBuildEvents = false;
                    Factory.CleanupFactory();
                    GC.SuppressFinalize(this);
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #endregion

        void BuildManager_BuildStopped()
        {
            MsVsShell.OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as MsVsShell.OleMenuCommandService;
            if (mcs != null)
            {
                mcs.FindCommand(new CommandID(GuidList.guidVSPackageCmdSet, (int)PkgCmdIDList.cmdidConfig)).Enabled = true;
            }
        }

        void BuildManager_BuildStarted()
        {
            MsVsShell.OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as MsVsShell.OleMenuCommandService;
            if (mcs != null)
            {
                mcs.FindCommand(new CommandID(GuidList.guidVSPackageCmdSet, (int)PkgCmdIDList.cmdidConfig)).Enabled = false;
            }
        }

        void BackgroundScanner_Stopped(object sender, EventArgs e)
        {
            MsVsShell.OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as MsVsShell.OleMenuCommandService;
            if (mcs != null)
            {
                MenuCommand stopCommand = mcs.FindCommand(new CommandID(GuidList.guidVSPackageCmdSet, (int)PkgCmdIDList.cmdidStopScan));
                stopCommand.Enabled = false;
                stopCommand.Checked = false;
                mcs.FindCommand(new CommandID(GuidList.guidVSPackageCmdSet, (int)PkgCmdIDList.cmdidRepeatLastScan)).Enabled = true;
            }
        }

        void BackgroundScanner_Started(object sender, EventArgs e)
        {
            MsVsShell.OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as MsVsShell.OleMenuCommandService;
            if (mcs != null)
            {
                mcs.FindCommand(new CommandID(GuidList.guidVSPackageCmdSet, (int)PkgCmdIDList.cmdidStopScan)).Enabled = true;
                mcs.FindCommand(new CommandID(GuidList.guidVSPackageCmdSet, (int)PkgCmdIDList.cmdidRepeatLastScan)).Enabled = false;
            }
        }

        private void StopScan(object sender, EventArgs e)
        {
            Factory.GetBackgroundScanner().StopIfRunning(false);
            if (Factory.GetBackgroundScanner().IsRunning)
            {
                MsVsShell.OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as MsVsShell.OleMenuCommandService;
                mcs.FindCommand(new CommandID(GuidList.guidVSPackageCmdSet, (int)PkgCmdIDList.cmdidStopScan)).Checked = true;
            }
        }

        private void RepeatLastScan(object sender, EventArgs e)
        {
            Factory.GetBackgroundScanner().RepeatLast();
        }

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            Factory.GetDialog().Invoke(ProjectUtilities.GetProjectsOfCurrentSelections());
        }

        /// <summary>
        /// This function is the callback used to query the status of the Code Sweep... project menu item
        /// </summary>
        void QueryStatus(object sender, EventArgs e)
        {
            bool menuVisible = false;
            DTE dte = GetService(typeof(DTE)) as DTE;
            foreach (EnvDTE.Project dteProject in dte.Solution.Projects)
            {
                Guid SolutionFolder = new Guid(EnvDTE.Constants.vsProjectKindSolutionItems);
                Guid MiscellaneousFiles = new Guid(EnvDTE.Constants.vsProjectKindMisc);
                Guid currentProjectKind = new Guid(dteProject.Kind);
                if (currentProjectKind != SolutionFolder && currentProjectKind != MiscellaneousFiles)
                {
                    menuVisible = true;
                }
            }

            OleMenuCommand menuCommand = sender as OleMenuCommand;
            menuCommand.Visible = menuVisible;
        }
    }
}
