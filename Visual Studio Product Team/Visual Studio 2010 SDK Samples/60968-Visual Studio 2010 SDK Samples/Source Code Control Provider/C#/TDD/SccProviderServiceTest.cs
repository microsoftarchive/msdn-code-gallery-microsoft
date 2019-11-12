/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel.Design;
using Microsoft.VisualStudio;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider;
using MsVsShell = Microsoft.VisualStudio.Shell;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
{
    /// <summary>
    ///This is a test class for Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService and is intended
    ///to contain all Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService Unit Tests
    ///</summary>
    [TestClass()]
    public class SccProviderServiceTest
    {
        OleServiceProvider _serviceProvider;
        MockSolution _solution;
        SccProvider _sccProvider;
        
        /// <summary>
        /// The service provider
        ///</summary>
        OleServiceProvider serviceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    _serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
                }

                return _serviceProvider;
            }
        }

        /// <summary>
        /// The solution
        ///</summary>
        MockSolution solution
        {
            get
            {
                if (_solution == null)
                {
                    _solution = new MockSolution();
                }

                return _solution;
            }
        }

        /// <summary>
        /// The provider
        ///</summary>
        SccProvider sccProvider
        {
            get
            {
                if (_sccProvider == null)
                {
                    // Create a provider package
                    _sccProvider = new SccProvider();
                }

                return _sccProvider;
            }
        }

        /// <summary>
        /// Creates a SccProviderService object
        ///</summary>
        public SccProviderService GetSccProviderServiceInstance
        {
            get
            {
                // Need to mock a service implementing IVsRegisterScciProvider, because the scc provider will register with it
                IVsRegisterScciProvider registerScciProvider = MockRegisterScciProvider.GetBaseRegisterScciProvider();
                serviceProvider.AddService(typeof(IVsRegisterScciProvider), registerScciProvider, true);

                // Register solution events because the provider will try to subscribe to them
                serviceProvider.AddService(typeof(SVsSolution), solution as IVsSolution, true);

                // Register TPD service because the provider will try to subscribe to TPD
                IVsTrackProjectDocuments2 tpd = MockTrackProjectDocumentsProvider.GetTrackProjectDocuments() as IVsTrackProjectDocuments2;
                serviceProvider.AddService(typeof(SVsTrackProjectDocuments), tpd, true);

                // Site the package
                IVsPackage package = sccProvider as IVsPackage;
                package.SetSite(serviceProvider);

                //  Get the source control provider service object
                FieldInfo sccServiceMember = typeof(SccProvider).GetField("sccService", BindingFlags.Instance | BindingFlags.NonPublic);
                SccProviderService target = sccServiceMember.GetValue(sccProvider) as SccProviderService;

                return target;
            }
        }

        /// <summary>
        ///A test for menu command status
        ///</summary>
        void VerifyCommandStatus(OLECMDF expectedStatus, OLECMD[] command)
        {
            Guid guidCmdGroup = GuidList.guidSccProviderCmdSet;

            int result = _sccProvider.QueryStatus(ref guidCmdGroup, 1, command, IntPtr.Zero);
            Assert.AreEqual(VSConstants.S_OK, result);
            Debug.Assert((uint)(expectedStatus) == command[0].cmdf);
            Assert.AreEqual((uint)(expectedStatus), command[0].cmdf);
        }

        void VerifyCommandExecution(OLECMD[] command)
        {
            MsVsShell.OleMenuCommandService mcs = sccProvider.GetService(typeof(IMenuCommandService)) as MsVsShell.OleMenuCommandService;
            CommandID cmd = new CommandID(GuidList.guidSccProviderCmdSet, (int)command[0].cmdID);
            MenuCommand menuCmd = mcs.FindCommand(cmd);
            menuCmd.Invoke();
        }

        /// <summary>
        ///A test for SccProviderService creation and interfaces
        ///</summary>
        [TestMethod()]
        public void ConstructorTest()
        {
            SccProviderService target = GetSccProviderServiceInstance;

            Assert.AreNotEqual(null, target, "Could not create provider service");
            Assert.IsNotNull(target as IVsSccProvider, "The object does not implement IVsPackage");
        }

        /// <summary>
        ///A test for Active
        ///</summary>
        [TestMethod()]
        public void ActiveTest()
        {
            SccProviderService target = GetSccProviderServiceInstance;

            // After the object is created, the provider is inactive
            Assert.AreEqual(false, target.Active, "Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService.Active was not reported correctly.");

            // Activate the provider and test the result
            target.SetActive();
            Assert.AreEqual(true, target.Active, "Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService.Active was not reported correctly.");

            // Deactivate the provider and test the result
            target.SetInactive();
            Assert.AreEqual(false, target.Active, "Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService.Active was not reported correctly.");
        }

        /// <summary>
        ///A test for AnyItemsUnderSourceControl (out int)
        ///</summary>
        [TestMethod()]
        public void AnyItemsUnderSourceControlTest()
        {
            SccProviderService target = GetSccProviderServiceInstance;

            int pfResult = 0;
            int actual = target.AnyItemsUnderSourceControl(out pfResult);

            // The method is not supposed to fail, and the basic provider cannot control any projects
            Assert.AreEqual(VSConstants.S_OK, pfResult, "pfResult_AnyItemsUnderSourceControl_expected was not set correctly.");
            Assert.AreEqual(0, actual, "Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService.AnyItemsUnderSourceControl did not return the expected value.");
        }

        /// <summary>
        ///A test for SetActive ()
        ///</summary>
        [TestMethod()]
        public void SetActiveTest()
        {
            SccProviderService target = GetSccProviderServiceInstance;

            int actual = target.SetActive();
            Assert.AreEqual(VSConstants.S_OK, actual, "Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService.SetActive failed.");
        }

        /// <summary>
        ///A test for SetInactive ()
        ///</summary>
        [TestMethod()]
        public void SetInactiveTest()
        {
            SccProviderService target = GetSccProviderServiceInstance;

            int actual = target.SetInactive();
            Assert.AreEqual(VSConstants.S_OK, actual, "Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService.SetInactive failed.");
        }

        /// <summary>
        ///A test for QueryEditQuerySave interface
        ///</summary>
        [TestMethod()]
        public void QueryEditQuerySaveTest()
        {
            uint pfEditVerdict;
            uint prgfMoreInfo;
            uint pdwQSResult;
            int result;

            SccProviderService target = GetSccProviderServiceInstance;

            // check the functions that are not implemented
            Assert.AreEqual((int)VSConstants.S_OK, (int)target.BeginQuerySaveBatch());
            Assert.AreEqual((int)VSConstants.S_OK, (int)target.EndQuerySaveBatch());
            Assert.AreEqual((int)VSConstants.S_OK, (int)target.DeclareReloadableFile("", 0, null));
            Assert.AreEqual((int)VSConstants.S_OK, (int)target.DeclareUnreloadableFile("", 0, null));
            Assert.AreEqual((int)VSConstants.S_OK, (int)target.OnAfterSaveUnreloadableFile("", 0, null));
            Assert.AreEqual((int)VSConstants.S_OK, (int)target.IsReloadable("", out result));
            Assert.AreEqual(1, result, "Not the right return value from IsReloadable");

            // Create a basic service provider
            
            IVsShell shell = MockShellProvider.GetShellForCommandLine() as IVsShell;
            serviceProvider.AddService(typeof(IVsShell), shell, true);

            // Command line tests
            result = target.QueryEditFiles((uint)tagVSQueryEditFlags.QEF_ReportOnly, 1, new string[] { "Dummy.txt" }, null, null, out pfEditVerdict, out prgfMoreInfo);
            Assert.AreEqual(VSConstants.S_OK, result, "QueryEdit failed.");
            Assert.AreEqual((uint)tagVSQueryEditResult.QER_EditOK, pfEditVerdict, "QueryEdit failed.");
            Assert.AreEqual((uint)0, prgfMoreInfo, "QueryEdit failed.");

            result = target.QuerySaveFile("Dummy.txt", 0, null, out pdwQSResult);
            Assert.AreEqual(VSConstants.S_OK, result, "QuerySave failed.");
            Assert.AreEqual((uint)tagVSQuerySaveResult.QSR_SaveOK, pdwQSResult, "QueryEdit failed.");

            serviceProvider.RemoveService(typeof(SVsShell));

            // UI mode tests
            shell = MockShellProvider.GetShellForUI() as IVsShell;
            serviceProvider.AddService(typeof(SVsShell), shell, true);

            // Edit of an uncontrolled file that doesn't exist on disk
            result = target.QueryEditFiles((uint)tagVSQueryEditFlags.QEF_ReportOnly, 1, new string[] { "Dummy.txt" }, null, null, out pfEditVerdict, out prgfMoreInfo);
            Assert.AreEqual(VSConstants.S_OK, result, "QueryEdit failed.");
            Assert.AreEqual((uint)tagVSQueryEditResult.QER_EditOK, pfEditVerdict, "QueryEdit failed.");
            Assert.AreEqual((uint)0, prgfMoreInfo, "QueryEdit failed.");

            // Mock a solution with a project and a file
            solution.SolutionFile = Path.GetTempFileName();
            MockIVsProject project = new MockIVsProject(Path.GetTempFileName());
            solution.AddProject(project);
            // Add only the project to source control.
            Hashtable uncontrolled = new Hashtable();
            uncontrolled[project as IVsSccProject2] = true;
            target.AddProjectsToSourceControl(ref uncontrolled, false);
            // Check that solution file is not controlled
            Assert.AreEqual(SourceControlStatus.scsUncontrolled, target.GetFileStatus(solution.SolutionFile), "Incorrect status returned");
            // Make the solution read-only on disk
            File.SetAttributes(solution.SolutionFile, FileAttributes.ReadOnly);

            // QueryEdit in report mode for uncontrolled readonly file
            result = target.QueryEditFiles((uint)tagVSQueryEditFlags.QEF_ReportOnly, 1, new string[] { solution.SolutionFile }, null, null, out pfEditVerdict, out prgfMoreInfo);
            Assert.AreEqual(VSConstants.S_OK, result, "QueryEdit failed.");
            Assert.AreEqual((uint)tagVSQueryEditResult.QER_EditNotOK, pfEditVerdict, "QueryEdit failed.");
            Assert.AreEqual((uint)(tagVSQueryEditResultFlags.QER_EditNotPossible | tagVSQueryEditResultFlags.QER_ReadOnlyNotUnderScc), prgfMoreInfo, "QueryEdit failed.");

            // QueryEdit in silent mode for uncontrolled readonly file
            result = target.QueryEditFiles((uint)tagVSQueryEditFlags.QEF_SilentMode, 1, new string[] { solution.SolutionFile }, null, null, out pfEditVerdict, out prgfMoreInfo);
            Assert.AreEqual(VSConstants.S_OK, result, "QueryEdit failed.");
            Assert.AreEqual((uint)tagVSQueryEditResult.QER_EditNotOK, pfEditVerdict, "QueryEdit failed.");
            Assert.AreEqual((uint)(tagVSQueryEditResultFlags.QER_NoisyPromptRequired), (uint)(tagVSQueryEditResultFlags.QER_NoisyPromptRequired) & prgfMoreInfo, "QueryEdit failed.");

            // Mock the UIShell service to answer Yes to the dialog invocation
            BaseMock mockUIShell = MockUiShellProvider.GetShowMessageBoxYes();
            serviceProvider.AddService(typeof(IVsUIShell), mockUIShell, true);

            // QueryEdit for uncontrolled readonly file: allow the edit and make the file read-write
            result = target.QueryEditFiles(0, 1, new string[] { solution.SolutionFile }, null, null, out pfEditVerdict, out prgfMoreInfo);
            Assert.AreEqual(VSConstants.S_OK, result, "QueryEdit failed.");
            Assert.AreEqual((uint)tagVSQueryEditResult.QER_EditOK, pfEditVerdict, "QueryEdit failed.");
            Assert.AreEqual((uint)0, prgfMoreInfo, "QueryEdit failed.");
            Assert.AreEqual<FileAttributes>(FileAttributes.Normal, File.GetAttributes(solution.SolutionFile), "File was not made writable");
            serviceProvider.RemoveService(typeof(IVsUIShell));

            // QueryEdit in report mode for controlled readonly file
            result = target.QueryEditFiles((uint)tagVSQueryEditFlags.QEF_ReportOnly, 1, new string[] { project.ProjectFile }, null, null, out pfEditVerdict, out prgfMoreInfo);
            Assert.AreEqual(VSConstants.S_OK, result, "QueryEdit failed.");
            Assert.AreEqual((uint)tagVSQueryEditResult.QER_EditNotOK, pfEditVerdict, "QueryEdit failed.");
            Assert.AreEqual((uint)(tagVSQueryEditResultFlags.QER_EditNotPossible | tagVSQueryEditResultFlags.QER_ReadOnlyUnderScc), prgfMoreInfo, "QueryEdit failed.");

            // QueryEdit in silent mode for controlled readonly file: should allow the edit and make the file read-write
            result = target.QueryEditFiles((uint)tagVSQueryEditFlags.QEF_SilentMode, 1, new string[] { project.ProjectFile }, null, null, out pfEditVerdict, out prgfMoreInfo);
            Assert.AreEqual(VSConstants.S_OK, result, "QueryEdit failed.");
            Assert.AreEqual((uint)tagVSQueryEditResult.QER_EditOK, pfEditVerdict, "QueryEdit failed.");
            Assert.AreEqual((uint)tagVSQueryEditResultFlags.QER_MaybeCheckedout, prgfMoreInfo, "QueryEdit failed.");
            Assert.AreEqual<FileAttributes>(FileAttributes.Normal, File.GetAttributes(solution.SolutionFile), "File was not made writable");
            serviceProvider.RemoveService(typeof(IVsUIShell));
        }

        /// <summary>
        ///A test for GetFileStatus/GetSccGlyphs
        ///</summary>
        [TestMethod()]
        public void TestFileStatus()
        {
            SccProviderService target = GetSccProviderServiceInstance;
            solution.SolutionFile = Path.GetTempFileName();
            MockIVsProject project = new MockIVsProject(Path.GetTempFileName());
            project.AddItem(Path.GetTempFileName());
            solution.AddProject(project);

            VsStateIcon [] rgsiGlyphs = new VsStateIcon[1];
            VsStateIcon [] rgsiGlyphsFromStatus = new VsStateIcon[1];
            uint[] rgdwSccStatus = new uint[1];
            int result = 0;
            string strTooltip;

            // Check glyphs and statuses for uncontrolled items
            IList<string> files = new string[] { solution.SolutionFile, project.ProjectFile, project.ProjectItems[0] };
            foreach (string file in files)
            {
                Assert.AreEqual(SourceControlStatus.scsUncontrolled, target.GetFileStatus(file), "Incorrect status returned");

                result = target.GetSccGlyph(1, new string[] { file }, rgsiGlyphs, rgdwSccStatus);
                Assert.AreEqual<int>(VSConstants.S_OK, result);
                Assert.AreEqual<VsStateIcon>(VsStateIcon.STATEICON_BLANK, rgsiGlyphs[0]);
                Assert.AreEqual<uint>((uint) __SccStatus.SCC_STATUS_NOTCONTROLLED, rgdwSccStatus[0]);

                result = target.GetSccGlyphFromStatus(rgdwSccStatus[0], rgsiGlyphsFromStatus);
                Assert.AreEqual<int>(VSConstants.S_OK, result);
                Assert.AreEqual<VsStateIcon>(rgsiGlyphs[0], rgsiGlyphsFromStatus[0]);
            }

            // Uncontrolled items should not have tooltips
            target.GetGlyphTipText(project as IVsHierarchy, VSConstants.VSITEMID_ROOT, out strTooltip);
            Assert.IsTrue(strTooltip.Length == 0);

            Hashtable uncontrolled = new Hashtable();
            uncontrolled[project as IVsSccProject2] = true;
            target.AddProjectsToSourceControl(ref uncontrolled, true);

            foreach (string file in files)
            {
                Assert.AreEqual(SourceControlStatus.scsCheckedIn, target.GetFileStatus(file), "Incorrect status returned");

                result = target.GetSccGlyph(1, new string[] { file }, rgsiGlyphs, rgdwSccStatus);
                Assert.AreEqual<int>(VSConstants.S_OK, result);
                Assert.AreEqual<VsStateIcon>(VsStateIcon.STATEICON_CHECKEDIN, rgsiGlyphs[0]);
                Assert.AreEqual<uint>((uint) __SccStatus.SCC_STATUS_CONTROLLED, rgdwSccStatus[0]);

                result = target.GetSccGlyphFromStatus(rgdwSccStatus[0], rgsiGlyphsFromStatus);
                Assert.AreEqual<int>(VSConstants.S_OK, result);
                Assert.AreEqual<VsStateIcon>(rgsiGlyphs[0], rgsiGlyphsFromStatus[0]);
            }

            // Checked in items should have tooltips
            target.GetGlyphTipText(project as IVsHierarchy, VSConstants.VSITEMID_ROOT, out strTooltip);
            Assert.IsTrue(strTooltip.Length > 0);

            foreach (string file in files)
            {
                target.CheckoutFile(file);
                Assert.AreEqual(SourceControlStatus.scsCheckedOut, target.GetFileStatus(file), "Incorrect status returned");

                result = target.GetSccGlyph(1, new string[] { file }, rgsiGlyphs, rgdwSccStatus);
                Assert.AreEqual<int>(VSConstants.S_OK, result);
                Assert.AreEqual<VsStateIcon>(VsStateIcon.STATEICON_CHECKEDOUT, rgsiGlyphs[0]);
                Assert.AreEqual<uint>((uint) __SccStatus.SCC_STATUS_CHECKEDOUT, rgdwSccStatus[0]);

                result = target.GetSccGlyphFromStatus(rgdwSccStatus[0], rgsiGlyphsFromStatus);
                Assert.AreEqual<int>(VSConstants.S_OK, result);
                Assert.AreEqual<VsStateIcon>(rgsiGlyphs[0], rgsiGlyphsFromStatus[0]);
            }

            // Checked out items should have tooltips, too
            target.GetGlyphTipText(project as IVsHierarchy, VSConstants.VSITEMID_ROOT, out strTooltip);
            Assert.IsTrue(strTooltip.Length > 0);

            foreach (string file in files)
            {
                target.CheckinFile(file);
                Assert.AreEqual(SourceControlStatus.scsCheckedIn, target.GetFileStatus(file), "Incorrect status returned");

                result = target.GetSccGlyph(1, new string[] { file }, rgsiGlyphs, rgdwSccStatus);
                Assert.AreEqual<int>(VSConstants.S_OK, result);
                Assert.AreEqual<VsStateIcon>(VsStateIcon.STATEICON_CHECKEDIN, rgsiGlyphs[0]);
                Assert.AreEqual<uint>((uint) __SccStatus.SCC_STATUS_CONTROLLED, rgdwSccStatus[0]);
            }

            // Add a new file to the project (don't worry about TPD events for now)
            string pendingAddFile = Path.GetTempFileName();
            project.AddItem(pendingAddFile);
            Assert.AreEqual(SourceControlStatus.scsUncontrolled, target.GetFileStatus(pendingAddFile), "Incorrect status returned");

            result = target.GetSccGlyph(1, new string[] { pendingAddFile }, rgsiGlyphs, rgdwSccStatus);
            Assert.AreEqual<int>(VSConstants.S_OK, result);
            Assert.AreEqual<VsStateIcon>(VsStateIcon.STATEICON_CHECKEDOUT, rgsiGlyphs[0]);
            Assert.AreEqual<uint>((uint) __SccStatus.SCC_STATUS_CHECKEDOUT, rgdwSccStatus[0]);

            // Pending add items should have tooltips, too
            target.GetGlyphTipText(project as IVsHierarchy, 1, out strTooltip);
            Assert.IsTrue(strTooltip.Length > 0);

            // Checkin the pending add file
            target.AddFileToSourceControl(pendingAddFile);
            Assert.AreEqual(SourceControlStatus.scsCheckedIn, target.GetFileStatus(pendingAddFile), "Incorrect status returned");

            result = target.GetSccGlyph(1, new string[] { pendingAddFile }, rgsiGlyphs, rgdwSccStatus);
            Assert.AreEqual<int>(VSConstants.S_OK, result);
            Assert.AreEqual<VsStateIcon>(VsStateIcon.STATEICON_CHECKEDIN, rgsiGlyphs[0]);
            Assert.AreEqual<uint>((uint) __SccStatus.SCC_STATUS_CONTROLLED, rgdwSccStatus[0]);
        }

        /// <summary>
        ///A test for TrackProjectDocuments
        ///</summary>
        [TestMethod()]
        public void TestTPDEvents()
        {
            int result = 0;

            SccProviderService target = GetSccProviderServiceInstance;
            solution.SolutionFile = Path.GetTempFileName();
            MockIVsProject project = new MockIVsProject(Path.GetTempFileName());
            solution.AddProject(project);

            Hashtable uncontrolled = new Hashtable();
            uncontrolled[project as IVsSccProject2] = true;
            target.AddProjectsToSourceControl(ref uncontrolled, true);
            // In real live, a QueryEdit call on the project file would be necessary to add/rename/delete items

            // Add a new item and fire the appropriate events
            string pendingAddFile = Path.GetTempFileName();
            VSQUERYADDFILERESULTS[] pSummaryResultAdd = new VSQUERYADDFILERESULTS[1];
            VSQUERYADDFILERESULTS[] rgResultsAdd = new VSQUERYADDFILERESULTS[1];
            result = target.OnQueryAddFiles(project as IVsProject, 1, new string[] {pendingAddFile},  null, pSummaryResultAdd, rgResultsAdd);
            Assert.AreEqual<int>(VSConstants.E_NOTIMPL, result);
            project.AddItem(pendingAddFile);
            result = target.OnAfterAddFilesEx(1, 1, new IVsProject[] { project as IVsProject }, new int[] { 0 }, new string[] { pendingAddFile }, null);
            Assert.AreEqual<int>(VSConstants.E_NOTIMPL, result);
            Assert.AreEqual(SourceControlStatus.scsUncontrolled, target.GetFileStatus(pendingAddFile), "Incorrect status returned");

            // Checkin the pending add file
            target.AddFileToSourceControl(pendingAddFile);

            // Rename the item and verify the file remains is controlled
            string newName = pendingAddFile + ".renamed";
            VSQUERYRENAMEFILERESULTS[] pSummaryResultRen = new VSQUERYRENAMEFILERESULTS[1];
            VSQUERYRENAMEFILERESULTS[] rgResultsRen = new VSQUERYRENAMEFILERESULTS[1];
            result = target.OnQueryRenameFiles(project as IVsProject, 1, new string[] { pendingAddFile }, new string[] { newName }, null, pSummaryResultRen, rgResultsRen);
            Assert.AreEqual<int>(VSConstants.E_NOTIMPL, result);
            project.RenameItem(pendingAddFile, newName);
            result = target.OnAfterRenameFiles(1, 1, new IVsProject[] {project as IVsProject}, new int[] {0}, new string[] { pendingAddFile }, new string[] { newName }, new VSRENAMEFILEFLAGS[] {VSRENAMEFILEFLAGS.VSRENAMEFILEFLAGS_NoFlags});
            Assert.AreEqual<int>(VSConstants.S_OK, result);
            Assert.AreEqual(SourceControlStatus.scsUncontrolled, target.GetFileStatus(pendingAddFile), "Incorrect status returned");
            Assert.AreEqual(SourceControlStatus.scsCheckedIn, target.GetFileStatus(newName), "Incorrect status returned");

            // Mock the UIShell service to answer Cancel to the dialog invocation
            BaseMock mockUIShell = MockUiShellProvider.GetShowMessageBoxCancel();
            serviceProvider.AddService(typeof(IVsUIShell), mockUIShell, true);
            // Try to delete the file from project; the delete should not be allowed
            VSQUERYREMOVEFILERESULTS[] pSummaryResultDel = new VSQUERYREMOVEFILERESULTS[1];
            VSQUERYREMOVEFILERESULTS[] rgResultsDel = new VSQUERYREMOVEFILERESULTS[1];
            result = target.OnQueryRemoveFiles(project as IVsProject, 1, new string[] { newName }, null, pSummaryResultDel, rgResultsDel);
            Assert.AreEqual<int>(VSConstants.S_OK, result);
            Assert.AreEqual<VSQUERYREMOVEFILERESULTS>(VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveNotOK, pSummaryResultDel[0]);
            // Mock the UIShell service to answer Yes to the dialog invocation
            serviceProvider.RemoveService(typeof(IVsUIShell));
            mockUIShell = MockUiShellProvider.GetShowMessageBoxYes();
            serviceProvider.AddService(typeof(IVsUIShell), mockUIShell, true);
            // Try to delete the file from project; the delete should be allowed this time
            result = target.OnQueryRemoveFiles(project as IVsProject, 1, new string[] { newName }, null, pSummaryResultDel, rgResultsDel);
            Assert.AreEqual<int>(VSConstants.S_OK, result);
            Assert.AreEqual<VSQUERYREMOVEFILERESULTS>(VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveOK, pSummaryResultDel[0]);
            // Remove the file from project
            project.RemoveItem(newName);
            result = target.OnAfterRemoveFiles(1, 1, new IVsProject[] { project as IVsProject }, new int[] { 0 }, new string[] { newName }, null);
            Assert.AreEqual<int>(VSConstants.E_NOTIMPL, result);
        }

        /// <summary>
        ///A test for opening and closing a controlled solution
        ///</summary>
        [TestMethod()]
        public void TestOpenCloseControlled()
        {
            const string strProviderName = "Sample Source Control Provider:{B0BAC05D-2000-41D1-A6C3-704E6C1A3DE2}";
            const string strSolutionPersistanceKey = "SampleSourceControlProviderSolutionProperties";
            const string strSolutionUserOptionsKey = "SampleSourceControlProvider";

            int result = 0;

            // Create a solution
            SccProviderService target = GetSccProviderServiceInstance;
            solution.SolutionFile = Path.GetTempFileName();
            MockIVsProject project = new MockIVsProject(Path.GetTempFileName());
            solution.AddProject(project);

            // Check solution props
            VSQUERYSAVESLNPROPS[] saveSolnProps = new VSQUERYSAVESLNPROPS[1];
            result = sccProvider.QuerySaveSolutionProps(null, saveSolnProps);
            Assert.AreEqual(VSConstants.S_OK, result);
            Assert.AreEqual<VSQUERYSAVESLNPROPS>(VSQUERYSAVESLNPROPS.QSP_HasNoProps, saveSolnProps[0]);

            // Add the solution to source control.
            Hashtable uncontrolled = new Hashtable();
            uncontrolled[project as IVsSccProject2] = true;
            target.AddProjectsToSourceControl(ref uncontrolled, true);

            // Solution should be dirty now
            result = sccProvider.QuerySaveSolutionProps(null, saveSolnProps);
            Assert.AreEqual(VSConstants.S_OK, result);
            Assert.AreEqual<VSQUERYSAVESLNPROPS>(VSQUERYSAVESLNPROPS.QSP_HasDirtyProps, saveSolnProps[0]);

            // Set the project offline so we'll have something to save in the "suo" stream
            target.ToggleOfflineStatus(project);

            // Force the provider to write the solution info into a stream 
            IStream pOptionsStream = new ComStreamFromDataStream(new MemoryStream()) as IStream;
            sccProvider.WriteUserOptions(pOptionsStream, strSolutionUserOptionsKey);
            // Move the stream position to the beginning
            LARGE_INTEGER liOffset;
            liOffset.QuadPart = 0;
            ULARGE_INTEGER[] ulPosition = new ULARGE_INTEGER[1];
            pOptionsStream.Seek(liOffset, 0, ulPosition);

            // Write solution props
            BaseMock propertyBag = MockPropertyBagProvider.GetWritePropertyBag();
            sccProvider.WriteSolutionProps(null, strSolutionPersistanceKey, propertyBag as IPropertyBag);

            // Close the solution to clean up the scc status
            int pfCancel = 0;
            target.OnQueryCloseProject(project, 0, ref pfCancel);
            target.OnQueryCloseSolution(null, ref pfCancel);
            Assert.AreEqual(pfCancel, 0, "Solution close was canceled");
            target.OnBeforeCloseProject(project, 0);
            // Theoretically the project should have called this, but especially after an add to scc, some projects forget to call it
            // target.UnregisterSccProject(project);
            target.OnBeforeCloseSolution(null);
            target.OnAfterCloseSolution(null);

            // Now attempt the "reopen"
            // The solution reads the solution properties
            propertyBag = MockPropertyBagProvider.GetReadPropertyBag();
            sccProvider.ReadSolutionProps(null, null, null, strSolutionPersistanceKey, 1, propertyBag as IPropertyBag);
            // The solution reads the user options from the stream where they were written before
            sccProvider.ReadUserOptions(pOptionsStream, strSolutionUserOptionsKey);
            // Then the projects are opened and register with the provider
            target.RegisterSccProject(project, "Project's location", "AuxPath", Path.GetDirectoryName(project.ProjectFile), strProviderName);
            // solution event fired for this project
            target.OnAfterOpenProject(project, 0);
            // Then solution completes opening
            target.OnAfterOpenSolution(null, 0);

            Assert.IsTrue(target.IsProjectControlled(null), "The solution's controlled status was not correctly persisted or read from property bag");
            Assert.IsTrue(target.IsProjectControlled(project), "The project's controlled status was not correctly set");
            Assert.IsTrue(target.IsProjectOffline(project), "The project's offline status was incorrectly persisted or read from suo stream");
        }

    
        /// <summary>
        ///A test for scc menu commands
        ///</summary>
        [TestMethod()]
        public void TestSccMenuCommands()
        {
            int result = 0;
            Guid badGuid = new Guid();
            Guid guidCmdGroup = GuidList.guidSccProviderCmdSet;

            OLECMD[] cmdAddToScc = new OLECMD[1];
            cmdAddToScc[0].cmdID = CommandId.icmdAddToSourceControl;
            OLECMD[] cmdCheckin = new OLECMD[1];
            cmdCheckin[0].cmdID = CommandId.icmdCheckin;
            OLECMD[] cmdCheckout = new OLECMD[1];
            cmdCheckout[0].cmdID = CommandId.icmdCheckout;
            OLECMD[] cmdUseSccOffline = new OLECMD[1];
            cmdUseSccOffline[0].cmdID = CommandId.icmdUseSccOffline;
            OLECMD[] cmdViewToolWindow = new OLECMD[1];
            cmdViewToolWindow[0].cmdID = CommandId.icmdViewToolWindow;
            OLECMD[] cmdToolWindowToolbarCommand = new OLECMD[1];
            cmdToolWindowToolbarCommand[0].cmdID = CommandId.icmdToolWindowToolbarCommand;
            OLECMD[] cmdUnsupported = new OLECMD[1];
            cmdUnsupported[0].cmdID = 0;
                        
            // Initialize the provider, etc
            SccProviderService target = GetSccProviderServiceInstance;

            // Mock a service implementing IVsMonitorSelection
            BaseMock monitorSelection = MockIVsMonitorSelectionFactory.GetMonSel();
            serviceProvider.AddService(typeof(IVsMonitorSelection), monitorSelection, true);

            // Commands that don't belong to our package should not be supported
            result = _sccProvider.QueryStatus(ref badGuid, 1, cmdAddToScc, IntPtr.Zero);
            Assert.AreEqual((int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED, result);

            // The command should be invisible when there is no solution
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_INVISIBLE, cmdAddToScc);

            // Activate the provider and test the result
            target.SetActive();
            Assert.AreEqual(true, target.Active, "Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService.Active was not reported correctly.");

            // The commands should be invisible when there is no solution
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_INVISIBLE, cmdAddToScc);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_INVISIBLE, cmdCheckin);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_INVISIBLE, cmdCheckout);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_INVISIBLE, cmdUseSccOffline);

            // Commands that don't belong to our package should not be supported
            result = _sccProvider.QueryStatus(ref guidCmdGroup, 1, cmdUnsupported, IntPtr.Zero);
            Assert.AreEqual((int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED, result);

            // Deactivate the provider and test the result
            target.SetInactive();
            Assert.AreEqual(false, target.Active, "Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService.Active was not reported correctly.");

            // Create a solution
            solution.SolutionFile = Path.GetTempFileName();
            MockIVsProject project = new MockIVsProject(Path.GetTempFileName());
            project.AddItem(Path.GetTempFileName());
            solution.AddProject(project);

            // The commands should be invisible when the provider is not active
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_INVISIBLE, cmdAddToScc);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_INVISIBLE, cmdCheckin);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_INVISIBLE, cmdCheckout);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_INVISIBLE, cmdUseSccOffline);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_INVISIBLE, cmdViewToolWindow);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_INVISIBLE, cmdToolWindowToolbarCommand);

            // Activate the provider and test the result
            target.SetActive();
            Assert.AreEqual(true, target.Active, "Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderService.Active was not reported correctly.");

            // The command should be visible but disabled now, except the toolwindow ones
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdAddToScc);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckin);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckout);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdUseSccOffline);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdViewToolWindow);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdToolWindowToolbarCommand);

            // Set selection to solution node
            VSITEMSELECTION selSolutionRoot;
            selSolutionRoot.pHier = _solution as IVsHierarchy;
            selSolutionRoot.itemid = VSConstants.VSITEMID_ROOT;
            monitorSelection["Selection"] = new VSITEMSELECTION[] { selSolutionRoot };

            // The add command should be available, rest should be disabled
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdAddToScc);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckin);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckout);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdUseSccOffline);

            // Still solution hierarchy, but other way
            selSolutionRoot.pHier = null;
            selSolutionRoot.itemid = VSConstants.VSITEMID_ROOT;
            monitorSelection["Selection"] = new VSITEMSELECTION[] { selSolutionRoot };

            // The add command should be available, rest should be disabled
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdAddToScc);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckin);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckout);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdUseSccOffline);

            // Set selection to project node
            VSITEMSELECTION selProjectRoot;
            selProjectRoot.pHier = project as IVsHierarchy;
            selProjectRoot.itemid = VSConstants.VSITEMID_ROOT;
            monitorSelection["Selection"] = new VSITEMSELECTION[] { selProjectRoot };

            // The add command should be available, rest should be disabled
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdAddToScc);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckin);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckout);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdUseSccOffline);

            // Set selection to project item
            VSITEMSELECTION selProjectItem;
            selProjectItem.pHier = project as IVsHierarchy;
            selProjectItem.itemid = 0;
            monitorSelection["Selection"] = new VSITEMSELECTION[] { selProjectItem };

            // The add command should be available, rest should be disabled
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdAddToScc);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckin);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckout);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdUseSccOffline);

            // Set selection to project and item node and add project to scc
            monitorSelection["Selection"] = new VSITEMSELECTION[] { selProjectRoot, selProjectItem };
            VerifyCommandExecution(cmdAddToScc);

            // The add command and checkin should be disabled, rest should be available now
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdAddToScc);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckin);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdCheckout);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdUseSccOffline);

            // Checkout the project 
            VerifyCommandExecution(cmdCheckout);

            // The add command and checkout should be disabled, rest should be available now
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdAddToScc);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdCheckin);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckout);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdUseSccOffline);

            // Select the solution
            monitorSelection["Selection"] = new VSITEMSELECTION[] { selSolutionRoot };

            // The checkout and offline should be disabled, rest should be available now
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdAddToScc);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdCheckin);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckout);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdUseSccOffline);
            
            // Checkin the project 
            VerifyCommandExecution(cmdCheckin);

            // The add command and checkout should be enabled, rest should be disabled
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdAddToScc);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckin);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdCheckout);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdUseSccOffline);

            // Add the solution to scc
            VerifyCommandExecution(cmdAddToScc);

            // The add command and checkin should be disabled, rest should be available now
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdAddToScc);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckin);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdCheckout);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdUseSccOffline);

            // Select the solution and project
            monitorSelection["Selection"] = new VSITEMSELECTION[] { selSolutionRoot, selProjectRoot };

            // Take the project and solution offline
            VerifyCommandExecution(cmdUseSccOffline);
            
            // The offline command should be latched
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_LATCHED, cmdUseSccOffline);
            
            // Select the solution only
            monitorSelection["Selection"] = new VSITEMSELECTION[] { selSolutionRoot};

            // Take the solution online
            VerifyCommandExecution(cmdUseSccOffline);

            // The offline command should be normal again
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdUseSccOffline);

            // Select the solution and project
            monitorSelection["Selection"] = new VSITEMSELECTION[] { selSolutionRoot, selProjectRoot };

            // The offline command should be disabled for mixed selection
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdUseSccOffline);

            // Add a new item to the project
            project.AddItem(Path.GetTempFileName());

            // Select the new item
            selProjectItem.pHier = project as IVsHierarchy;
            selProjectItem.itemid = 1;
            monitorSelection["Selection"] = new VSITEMSELECTION[] { selProjectItem };

            // The add command and checkout should be disabled, rest should be available now
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdAddToScc);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdCheckin);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckout);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_LATCHED, cmdUseSccOffline);

            // Checkin the new file (this should do an add)
            VerifyCommandExecution(cmdCheckin);

            // The add command and checkout should be disabled, rest should be available now
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdAddToScc);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED, cmdCheckin);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED, cmdCheckout);
            VerifyCommandStatus(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_LATCHED, cmdUseSccOffline);
        }
    }
}
