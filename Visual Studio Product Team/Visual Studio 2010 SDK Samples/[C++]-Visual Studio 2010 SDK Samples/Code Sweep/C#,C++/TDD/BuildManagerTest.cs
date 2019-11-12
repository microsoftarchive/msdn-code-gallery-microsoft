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
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    /// <summary>
    ///This is a test class for CodeSweep.VSPackage.BuildManager and is intended
    ///to contain all CodeSweep.VSPackage.BuildManager Unit Tests
    ///</summary>
    [TestClass()]
    public class BuildManagerTest
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

            Utilities.CopyTargetsFileToBinDir();
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
            Utilities.CleanUpTempFiles();
            Utilities.RemoveCommandHandlers(_serviceProvider);
        }
        //
        #endregion

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructWithNullArg()
        {
            IServiceProvider provider = null;
            object target = new CodeSweep.VSPackage.BuildManager_Accessor(provider);
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void IsListeningToBuildEventsTest()
        {
            CodeSweep.VSPackage.BuildManager_Accessor accessor = new CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider);

            MockDTE dte = _serviceProvider.GetService(typeof(EnvDTE.DTE)) as MockDTE;
            MockBuildEvents buildEvents = dte.Events.BuildEvents as MockBuildEvents;

            Assert.IsFalse(accessor.IsListeningToBuildEvents, "IsListeningToBuildEvents should be false by default.");

            int expectedSubscriberCount = buildEvents.OnBuildBeginSubscriberCount + 1;
            accessor.IsListeningToBuildEvents = true;
            Assert.IsTrue(accessor.IsListeningToBuildEvents, "IsListeningToBuildEvents could not be set to true.");
            Assert.AreEqual(expectedSubscriberCount, buildEvents.OnBuildBeginSubscriberCount, "Build manager did not subscribe to OnBuildBegin when IsListeningToBuildEvents set to true.");

            accessor.IsListeningToBuildEvents = false;
            expectedSubscriberCount--;
            Assert.IsFalse(accessor.IsListeningToBuildEvents, "IsListeningToBuildEvents could not be set to false.");
            Assert.AreEqual(expectedSubscriberCount, buildEvents.OnBuildBeginSubscriberCount, "Build manager did not unsubscribe from OnBuildBegin when IsListeningToBuildEvents set to false.");
        }

        private ProjectImportElement GetImport(Microsoft.Build.Evaluation.Project project, string importPath)
        {
            foreach (ProjectImportElement import in project.Xml.Imports)
            {
                if (import.Project == importPath)
                {
                    return import;
                }
            }

            return null;
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void TestBuildBegin()
        {
            CodeSweep.VSPackage.BuildManager_Accessor accessor = new CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider);

            accessor.IsListeningToBuildEvents = true;

            // Listen for task list refresh events.
            List<int> resultCounts = new List<int>();
            MockTaskList taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as MockTaskList;
            taskList.OnRefreshTasks += delegate(object sender, MockTaskList.RefreshTasksArgs args)
            {
                resultCounts.Add(Utilities.TasksOfProvider(args.Provider).Count);
            };

            // Create multiple projects with ScannerTask tasks.
            string scanFile = Utilities.CreateTempTxtFile("foo abc foo def foo");
            string termTable = Utilities.CreateTermTable(new string[] { "foo", "bar" });
            Microsoft.Build.Evaluation.Project project1 = Utilities.SetupMSBuildProject(new string[] { scanFile }, new string[] { termTable });
            Microsoft.Build.Evaluation.Project project2 = Utilities.SetupMSBuildProject(new string[] { scanFile }, new string[] { termTable });

            Utilities.RegisterProjectWithMocks(project1, _serviceProvider);
            Utilities.RegisterProjectWithMocks(project2, _serviceProvider);

            // Fire the build begin event.
            MockDTE dte = _serviceProvider.GetService(typeof(EnvDTE.DTE)) as MockDTE;
            MockBuildEvents buildEvents = dte.Events.BuildEvents as MockBuildEvents;
            buildEvents.FireOnBuildBegin(vsBuildScope.vsBuildScopeProject, vsBuildAction.vsBuildActionBuild);

            try
            {
                Assert.IsNotNull(ProjectCollection.GlobalProjectCollection.HostServices.GetHostObject(project1.FullPath, "AfterBuild", "ScannerTask"), "Host object for task in first project not set.");
                Assert.IsNotNull(ProjectCollection.GlobalProjectCollection.HostServices.GetHostObject(project2.FullPath, "AfterBuild", "ScannerTask"), "Host object for task in second project not set.");

                Assert.AreEqual(1, resultCounts.Count, "Task list recieved wrong number of refresh requests.");
                Assert.AreEqual(0, resultCounts[0], "Task list was not cleared.");
            }
            finally
            {
                buildEvents.FireOnBuildDone(vsBuildScope.vsBuildScopeProject, vsBuildAction.vsBuildActionBuild);
            }
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetBuildTaskWithNullArg()
        {
            CodeSweep.VSPackage.BuildManager_Accessor accessor = new CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider);
            accessor.GetBuildTask(null, false);
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void GetBuildTaskWithNoCreation()
        {
            CodeSweep.VSPackage.BuildManager_Accessor accessor = new CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider);

            // Create a project without a build task.
            Microsoft.Build.Evaluation.Project project1 = Utilities.SetupMSBuildProject();
            MockIVsProject vsProject = Utilities.RegisterProjectWithMocks(project1, _serviceProvider);

            Assert.IsNull(accessor.GetBuildTask(vsProject, false), "GetBuildTask did not return null for project without a ScannerTask.");

            // Create a project with a build task.
            string scanFile = Utilities.CreateTempTxtFile("foo abc foo def foo");
            string termTable = Utilities.CreateTermTable(new string[] { "foo", "bar" });
            Microsoft.Build.Evaluation.Project project2 = Utilities.SetupMSBuildProject(new string[] { scanFile }, new string[] { termTable });

            Microsoft.Build.Construction.ProjectTaskElement existingTask = Utilities.GetScannerTask(project2);

            vsProject = Utilities.RegisterProjectWithMocks(project2, _serviceProvider);

            Assert.AreEqual(existingTask, accessor.GetBuildTask(vsProject, false), "GetBuildTask did not return expected task object.");
            Assert.IsNull(GetImport(project2, Utilities.GetTargetsPath()), "GetBuildTask created Import unexpected.");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void GetBuildTaskWithCreation()
        {
            CodeSweep.VSPackage.BuildManager_Accessor accessor = new CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider);

            // Create a project without a build task.
            Microsoft.Build.Evaluation.Project project = Utilities.SetupMSBuildProject();

            project.Xml.AddItem("foo", "blah.txt");
            project.Xml.AddItem("bar", "blah2.cs");
            project.Xml.AddItem("Reference", "My.Namespace.Etc");

            MockIVsProject vsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider);

            ProjectTaskElement task = accessor.GetBuildTask(vsProject, true);

            Assert.IsNotNull(task, "GetBuildTask did not create task.");
            Assert.AreEqual("false", task.ContinueOnError, "ContinueOnError is wrong.");
            Assert.AreEqual("Exists('" + Utilities.GetTargetsPath() + "') and '$(RunCodeSweepAfterBuild)' == 'true'", task.Condition, "Condition is wrong.");
            Assert.AreEqual("@(foo);@(bar)", task.GetParameter("FilesToScan"), "FilesToScan property is wrong.");
            Assert.AreEqual("$(MSBuildProjectFullPath)", task.GetParameter("Project"), "Project property is wrong.");

            string projectFolder = Path.GetDirectoryName(project.FullPath);
            string expectedTermTable = CodeSweep.Utilities.RelativePathFromAbsolute(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\CodeSweep\\sample_term_table.xml", projectFolder);
            Assert.AreEqual(expectedTermTable, task.GetParameter("TermTables"), "TermTables property is wrong.");

            // Ensure the task is in the AfterBuild target.
            bool found = false;
            foreach (ProjectTaskElement thisTask in project.Xml.Targets.FirstOrDefault(target => target.Name == "AfterBuild").Tasks)
            {
                if (thisTask == task)
                {
                    found = true;
                    break;
                }
            }
            Assert.IsTrue(found, "The task was not in the AfterBuild target.");

            ProjectImportElement import = GetImport(project, Utilities.GetTargetsPath());
            Assert.IsNotNull(import, "GetBuildTask did not create Import.");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AllItemsInProjectWithNullArg()
        {
            CodeSweep.VSPackage.BuildManager_Accessor accessor = new CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider);
            accessor.AllItemsInProject(null);
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void AllItemsInProjectTest()
        {
            CodeSweep.VSPackage.BuildManager_Accessor accessor = new CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider);

            Microsoft.Build.Evaluation.Project project = Utilities.SetupMSBuildProject();
            string projectFolder = Path.GetDirectoryName(project.FullPath);

            project.Xml.AddItem("foo", "blah.txt");
            project.Xml.AddItem("bar", "blah2.cs");
            project.Xml.AddItem("bar", "My.Namespace.Etc");
            project.Xml.AddItem("bar", "blah3.vsmdi");

            // Create the files on disk, otherwise AllItemsInProject will exclude them.
            File.WriteAllText(projectFolder + "\\blah.txt", "");
            File.WriteAllText(projectFolder + "\\blah2.cs", "");
            File.WriteAllText(projectFolder + "\\blah3.vsmdi", "");

            MockIVsProject vsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider);

            string projectDir = Path.GetDirectoryName(project.FullPath);

            List<string> items = Utilities.ListFromEnum(accessor.AllItemsInProject(vsProject));

            Assert.AreEqual(3, items.Count, "AllItemsInProject returned wrong number of items.");
            CollectionAssert.Contains(items, projectDir + "\\blah.txt", "AllItemsInProject did not return blah.txt.");
            CollectionAssert.Contains(items, projectDir + "\\blah2.cs", "AllItemsInProject did not return blah2.cs.");
            CollectionAssert.Contains(items, projectDir + "\\blah3.vsmdi", "AllItemsInProject did not return blah3.vsmdi.");
            CollectionAssert.DoesNotContain(items, "My.Namespace.Etc", "AllItemsInProject returned Reference entry.");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void GetPropertyWithInvalidArgs()
        {
            CodeSweep.VSPackage.BuildManager_Accessor accessor = new CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider);

            bool thrown = Utilities.HasFunctionThrown<ArgumentNullException>(delegate { accessor.GetProperty(null, "foo"); });
            Assert.IsTrue(thrown, "GetProperty did not throw ArgumentNullException with null project arg.");

            Microsoft.Build.Evaluation.Project project = Utilities.SetupMSBuildProject();

            MockIVsProject vsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider);

            thrown = Utilities.HasFunctionThrown<ArgumentNullException>(delegate { accessor.GetProperty(vsProject, null); });
            Assert.IsTrue(thrown, "GetProperty did not throw ArgumentNullException with null name arg.");

            thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { accessor.GetProperty(vsProject, ""); });
            Assert.IsTrue(thrown, "GetProperty did not throw ArgumentException with empty name arg.");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void GetPropertyTest()
        {
            CodeSweep.VSPackage.BuildManager_Accessor accessor = new CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider);

            Microsoft.Build.Evaluation.Project project = Utilities.SetupMSBuildProject();

            ProjectPropertyGroupElement group = project.Xml.AddPropertyGroup();
            group.AddProperty("foo", "bar");

            MockIVsProject vsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider);

            Assert.IsNull(accessor.GetProperty(vsProject, "blah"), "GetProperty did not return null with invalid name.");
            Assert.AreEqual("bar", accessor.GetProperty(vsProject, "foo"), "GetProperty did not return correct value.");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void SetPropertyWithInvalidArgs()
        {
            CodeSweep.VSPackage.BuildManager_Accessor accessor = new CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider);

            bool thrown = Utilities.HasFunctionThrown<ArgumentNullException>(delegate { accessor.SetProperty(null, "foo", "bar"); });
            Assert.IsTrue(thrown, "SetProperty did not throw ArgumentNullException with null project arg.");

            Microsoft.Build.Evaluation.Project project = Utilities.SetupMSBuildProject();
            MockIVsProject vsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider);

            thrown = Utilities.HasFunctionThrown<ArgumentNullException>(delegate { accessor.SetProperty(vsProject, null, "bar"); });
            Assert.IsTrue(thrown, "SetProperty did not throw ArgumentNullException with null name arg.");

            thrown = Utilities.HasFunctionThrown<ArgumentNullException>(delegate { accessor.SetProperty(vsProject, "foo", null); });
            Assert.IsTrue(thrown, "SetProperty did not throw ArgumentNullException with null value arg.");

            thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { accessor.SetProperty(vsProject, "", "bar"); });
            Assert.IsTrue(thrown, "SetProperty did not throw ArgumentException with empty name arg.");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void SetPropertyTest()
        {
            CodeSweep.VSPackage.BuildManager_Accessor accessor = new CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider);

            Microsoft.Build.Evaluation.Project project = Utilities.SetupMSBuildProject();
            MockIVsProject vsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider);

            accessor.SetProperty(vsProject, "foo", "bar");
            Assert.AreEqual("bar", accessor.GetProperty(vsProject, "foo"), "SetProperty did not set value correctly.");

            accessor.SetProperty(vsProject, "foo", "");
            Assert.AreEqual("", accessor.GetProperty(vsProject, "foo"), "SetProperty did not set value correctly (2).");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void ScanStopsWhenBuildBegins()
        {
            CodeSweep.VSPackage.BuildManager_Accessor accessor = new CodeSweep.VSPackage.BuildManager_Accessor(_serviceProvider);

            accessor.IsListeningToBuildEvents = true;

            string firstFile = Utilities.CreateBigFile();
            string secondFile = Utilities.CreateTempTxtFile("bar bar bar floop doop bar");
            string termTable = Utilities.CreateTermTable(new string[] { "foo", "bar" });
            Microsoft.Build.Evaluation.Project project = Utilities.SetupMSBuildProject(new string[] { firstFile, secondFile }, new string[] { termTable });
            MockIVsProject vsProject = Utilities.RegisterProjectWithMocks(project, _serviceProvider);

            // Start a background scan.
            CodeSweep.VSPackage.IBackgroundScanner_Accessor scannerAccessor = CodeSweep.VSPackage.Factory_Accessor.GetBackgroundScanner();

            scannerAccessor.Start(new IVsProject[] { vsProject });

            // Fire the build begin event.
            MockDTE dte = _serviceProvider.GetService(typeof(EnvDTE.DTE)) as MockDTE;
            MockBuildEvents buildEvents = dte.Events.BuildEvents as MockBuildEvents;
            buildEvents.FireOnBuildBegin(vsBuildScope.vsBuildScopeProject, vsBuildAction.vsBuildActionBuild);

            try
            {
                Assert.IsFalse(scannerAccessor.IsRunning, "Background scan did not stop when build began.");
            }
            finally
            {
                buildEvents.FireOnBuildDone(vsBuildScope.vsBuildScopeProject, vsBuildAction.vsBuildActionBuild);
                accessor.IsListeningToBuildEvents = false;
            }
        }
    }


}
