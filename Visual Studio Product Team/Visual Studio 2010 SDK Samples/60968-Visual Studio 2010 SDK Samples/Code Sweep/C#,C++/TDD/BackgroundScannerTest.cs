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
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    /// <summary>
    ///This is a test class for CodeSweep.VSPackage.BackgroundScanner and is intended
    ///to contain all CodeSweep.VSPackage.BackgroundScanner Unit Tests
    ///</summary>
    [TestClass()]
    public class BackgroundScannerTest
    {

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //
        static MockServiceProvider _serviceProvider;
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            if (CodeSweep.VSPackage.Factory_Accessor.ServiceProvider == null)
            {
                _serviceProvider = new MockServiceProvider();
                CodeSweep.VSPackage.Factory_Accessor.ServiceProvider = _serviceProvider;
            }
            else
            {
                _serviceProvider = CodeSweep.VSPackage.Factory_Accessor.ServiceProvider as MockServiceProvider;
            }

            CodeSweep.VSPackage.Factory_Accessor.GetBuildManager().CreatePerUserFilesAsNecessary();
        }

        //Use ClassCleanup to run code after all tests in a class have run
        //
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //
        [TestCleanup()]
        public void MyTestCleanup()
        {
            // If the test left the scanner running, stop it now.
            if (_scanner != null)
            {
                _scanner.StopIfRunning(true);
                _scanner = null;
            }

            Utilities.CleanUpTempFiles();
            Utilities.RemoveCommandHandlers(_serviceProvider);
        }
        //
        #endregion

        CodeSweep.VSPackage.BackgroundScanner_Accessor _scanner;

        CodeSweep.VSPackage.BackgroundScanner_Accessor GetScanner()
        {
            _scanner = new CodeSweep.VSPackage.BackgroundScanner_Accessor(_serviceProvider);
            return _scanner;
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateWithNullArg()
        {
            IServiceProvider provider = null;
            object target = new CodeSweep.VSPackage.BackgroundScanner_Accessor(provider);
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StartWithNullArg()
        {
            CodeSweep.VSPackage.BackgroundScanner_Accessor accessor = GetScanner();
            accessor.Start(null);
        }

        string CreateMinimalTermTableFile()
        {
            return Utilities.CreateTermTable(new string[] { "foo" });
        }

        //TODO: This test is subject to timing issues where the build finishes before checking
        //the properties of the object under test. Need to fix this issue to enable this unit test.
        [Ignore()] 
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void StartWhenAlreadyRunning()
        {
            CodeSweep.VSPackage.BackgroundScanner_Accessor accessor = GetScanner();

            Project project = Utilities.SetupMSBuildProject(new string[] { Utilities.CreateBigFile() }, new string[] { CreateMinimalTermTableFile() });
            MockIVsProject vsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider);

            bool thrown = false;

            accessor.Start(new IVsProject[] { vsProject });

            try
            {
                accessor.Start(new IVsProject[] { new MockIVsProject(project.FullPath) });
            }
            catch (InvalidOperationException)
            {
                thrown = true;
            }  

            Utilities.WaitForStop(accessor);

            Assert.IsTrue(thrown, "Start did not throw InvalidOperationException with scan already running.");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void StartOnProjectWithoutScannerTask()
        {
            CodeSweep.VSPackage.BackgroundScanner_Accessor accessor = GetScanner();

            Project project = Utilities.SetupMSBuildProject();
            MockIVsProject vsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider);

            // TODO: if we could, it would be good to make sure the Started event isn't fired --
            // but apparently the VS test framework doesn't support events.

            accessor.Start(new IVsProject[] { vsProject });

            Assert.IsFalse(accessor.IsRunning, "IsRunning returned true after Start() called with project that does not contain a scanner config.");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void StartUpdatesTaskList()
        {
            CodeSweep.VSPackage.BackgroundScanner_Accessor accessor = GetScanner();

            // Set up events so we know when the task list is called.
            Guid activeProvider = Guid.Empty;
            MockTaskList taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as MockTaskList;
            taskList.OnSetActiveProvider += delegate(object sender, MockTaskList.SetActiveProviderArgs args) { activeProvider = args.ProviderGuid; };

            bool cmdPosted = false;
            MockShell shell = _serviceProvider.GetService(typeof(SVsUIShell)) as MockShell;
            shell.OnPostExecCommand += delegate(object sender, MockShell.PostExecCommandArgs args)
            {
                if (args.Group == VSConstants.GUID_VSStandardCommandSet97 && args.ID == (uint)VSConstants.VSStd97CmdID.TaskListWindow)
                {
                    cmdPosted = true;
                }
            };

            Project project = Utilities.SetupMSBuildProject(new string[] { Utilities.CreateBigFile() }, new string[] { CreateMinimalTermTableFile() });
            MockIVsProject vsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider);

            accessor.Start(new IVsProject[] { vsProject });

            Assert.IsTrue(cmdPosted, "Start did not activate the task list.");
            Assert.AreEqual(new Guid("{9ACC41B7-15B4-4dd7-A0F3-0A935D5647F3}"), activeProvider, "Start did not select the correct task bucket.");

            Utilities.WaitForStop(accessor);
        }

        //TODO: This test is subject to timing issues where the build finishes before checking
        //the properties of the object under test. Need to fix this issue to enable this unit test.
        [Ignore()] 
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void ProgressShowsInStatusBar()
        {
            CodeSweep.VSPackage.BackgroundScanner_Accessor accessor = GetScanner();

            bool inProgress = false;
            string label = null;
            uint complete = 0;
            uint total = 0;
            MockStatusBar statusBar = _serviceProvider.GetService(typeof(SVsStatusbar)) as MockStatusBar;
            statusBar.OnProgress += delegate(object sender, MockStatusBar.ProgressArgs args)
            {
                inProgress = (args.InProgress == 0) ? false : true;
                label = args.Label;
                complete = args.Complete;
                total = args.Total;
            };

            Project project = Utilities.SetupMSBuildProject(new string[] { Utilities.CreateBigFile() }, new string[] { CreateMinimalTermTableFile() });
            MockIVsProject vsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider);

            accessor.Start(new IVsProject[] { vsProject });

            Assert.IsTrue(inProgress, "Start did not show progress in status bar.");
            Assert.AreEqual((uint)0, complete, "Scan did not start with zero complete.");

            // TODO: if time allows: scan several files, detect (by task list updates) when each one is done, and check to make sure the status bar has been updated accordingly.

            Utilities.WaitForStop(accessor);

            Assert.IsFalse(inProgress, "Progress bar not cleared when scan ends.");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void TaskListIsUpdated()
        {
            CodeSweep.VSPackage.BackgroundScanner_Accessor accessor = GetScanner();

            List<int> resultCounts = new List<int>();
            MockTaskList taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as MockTaskList;
            taskList.OnRefreshTasks += delegate(object sender, MockTaskList.RefreshTasksArgs args)
            {
                resultCounts.Add(Utilities.TasksOfProvider(args.Provider).Count);
            };

            string firstFile = Utilities.CreateTempTxtFile("foo abc foo def foo");
            string secondFile = Utilities.CreateTempTxtFile("bar bar bar floop doop bar");
            string termTable1 = Utilities.CreateTermTable(new string[] { "foo", "bar" });
            string termTable2 = Utilities.CreateTermTable(new string[] { "floop" });
            Project project1 = Utilities.SetupMSBuildProject(new string[] { firstFile, secondFile }, new string[] { termTable1, termTable2 });
            MockIVsProject vsProject1 = Utilities.RegisterProjectWithMocks(project1, _serviceProvider);

            string thirdFile = Utilities.CreateTempTxtFile("blarg");
            string termTable3 = Utilities.CreateTermTable(new string[] { "blarg" });
            Project project2 = Utilities.SetupMSBuildProject(new string[] { thirdFile }, new string[] { termTable3 });
            MockIVsProject vsProject2 = Utilities.RegisterProjectWithMocks(project2, _serviceProvider);

            accessor.Start(new IVsProject[] { vsProject1, vsProject2 });

            Utilities.WaitForStop(accessor);

            Assert.AreEqual(4, resultCounts.Count, "Task list did not recieve correct number of updates.");
            Assert.AreEqual(0, resultCounts[0], "Number of hits in first update is wrong.");
            Assert.AreEqual(3, resultCounts[1], "Number of hits in second update is wrong.");
            Assert.AreEqual(3 + 5, resultCounts[2], "Number of hits in third update is wrong.");
            Assert.AreEqual(3 + 5 + 1, resultCounts[3], "Number of hits in fourth update is wrong.");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void RepeatLastProducesSameResultsAsPreviousScan()
        {
            CodeSweep.VSPackage.BackgroundScanner_Accessor accessor = GetScanner();

            string firstFile = Utilities.CreateTempTxtFile("foo abc foo def foo");
            string secondFile = Utilities.CreateTempTxtFile("bar bar bar floop doop bar");
            string termTable1 = Utilities.CreateTermTable(new string[] { "foo", "bar" });
            string termTable2 = Utilities.CreateTermTable(new string[] { "floop" });
            Project project1 = Utilities.SetupMSBuildProject(new string[] { firstFile, secondFile }, new string[] { termTable1, termTable2 });
            MockIVsProject vsProject1 = Utilities.RegisterProjectWithMocks(project1, _serviceProvider);

            string thirdFile = Utilities.CreateTempTxtFile("blarg");
            string termTable3 = Utilities.CreateTermTable(new string[] { "blarg" });
            Project project2 = Utilities.SetupMSBuildProject(new string[] { thirdFile }, new string[] { termTable3 });
            MockIVsProject vsProject2 = Utilities.RegisterProjectWithMocks(project2, _serviceProvider);

            accessor.Start(new IVsProject[] { vsProject1, vsProject2 });

            Utilities.WaitForStop(accessor);

            List<int> resultCounts = new List<int>();
            MockTaskList taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as MockTaskList;
            taskList.OnRefreshTasks += delegate(object sender, MockTaskList.RefreshTasksArgs args)
            {
                resultCounts.Add(Utilities.TasksOfProvider(args.Provider).Count);
            };

            accessor.RepeatLast();

            Utilities.WaitForStop(accessor);

            Assert.AreEqual(4, resultCounts.Count, "Task list did not recieve correct number of updates.");
            Assert.AreEqual(0, resultCounts[0], "Number of hits in first update is wrong.");
            Assert.AreEqual(3, resultCounts[1], "Number of hits in second update is wrong.");
            Assert.AreEqual(3 + 5, resultCounts[2], "Number of hits in third update is wrong.");
            Assert.AreEqual(3 + 5 + 1, resultCounts[3], "Number of hits in fourth update is wrong.");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RepeatLastWithNoPreviousScan()
        {
            CodeSweep.VSPackage.BackgroundScanner_Accessor accessor = GetScanner();
            accessor.RepeatLast();
        }

        //TODO: This test is subject to timing issues where the build finishes before checking
        //the properties of the object under test. Need to fix this issue to enable this unit test.
        [Ignore()]
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RepeatLastWhenAlreadyRunning()
        {
            CodeSweep.VSPackage.BackgroundScanner_Accessor accessor = GetScanner();

            Project project = Utilities.SetupMSBuildProject(new string[] { Utilities.CreateBigFile() }, new string[] { CreateMinimalTermTableFile() });
            MockIVsProject vsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider);

            accessor.Start(new IVsProject[] { vsProject });
            accessor.RepeatLast();
        }

        //TODO: This test is subject to timing issues where the build finishes before checking
        //the properties of the object under test. Need to fix this issue to enable this unit test.
        [Ignore()] 
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void StopStopsScanBeforeNextFile()
        {
            CodeSweep.VSPackage.BackgroundScanner_Accessor accessor = GetScanner();

            int refreshes = 0;
            MockTaskList taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as MockTaskList;
            taskList.OnRefreshTasks +=
                delegate(object sender, MockTaskList.RefreshTasksArgs args)
                {
                    ++refreshes;
                };

            string firstFile = Utilities.CreateBigFile();
            string secondFile = Utilities.CreateTempTxtFile("bar bar bar floop doop bar");
            string termTable = Utilities.CreateTermTable(new string[] { "foo", "bar" });
            Project project = Utilities.SetupMSBuildProject(new string[] { firstFile, secondFile }, new string[] { termTable });
            MockIVsProject vsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider);

            accessor.Start(new IVsProject[] { vsProject });
            accessor.StopIfRunning(true);

            // There should be one update, when the task list was initially cleared.
            Assert.AreEqual(1, refreshes, "Stop did not stop scan before next file.");
        }

        //TODO: This test is subject to timing issues where the build finishes before checking
        //the properties of the object under test. Need to fix this issue to enable this unit test.
        [Ignore()] 
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void IsRunning()
        {
            CodeSweep.VSPackage.BackgroundScanner_Accessor accessor = GetScanner();

            int refreshes = 0;
            MockTaskList taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as MockTaskList;
            taskList.OnRefreshTasks += delegate(object sender, MockTaskList.RefreshTasksArgs args) { ++refreshes; };

            string firstFile = Utilities.CreateBigFile();
            string secondFile = Utilities.CreateTempTxtFile("bar bar bar floop doop bar");
            string termTable = Utilities.CreateTermTable(new string[] { "foo", "bar" });
            Project project = Utilities.SetupMSBuildProject(new string[] { firstFile, secondFile }, new string[] { termTable });
            MockIVsProject vsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider);

            accessor.Start(new IVsProject[] { vsProject });

            Assert.IsTrue(accessor.IsRunning, "IsRunning was not true after Start.");

            accessor.StopIfRunning(false);

            Assert.IsTrue(accessor.IsRunning, "IsRunning was not true after Stop called while scan is still running.");
        }

    }


}
