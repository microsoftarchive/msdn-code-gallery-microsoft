/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    /// <summary>
    ///This is a test class for CodeSweep.VSPackage.NonMSBuildProjectConfigStore and is intended
    ///to contain all CodeSweep.VSPackage.NonMSBuildProjectConfigStore Unit Tests
    ///</summary>
    [TestClass()]
    public class NonMSBuildProjectConfigStoreTest
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
        }
        //
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

            MockDTE dte = _serviceProvider.GetService(typeof(DTE)) as MockDTE;

            foreach (EnvDTE.Project project in dte.Solution.Projects)
            {
                MockDTEGlobals globals = project.Globals as MockDTEGlobals;
                globals.ClearAll();
            }
        }
        //
        #endregion

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void HasConfiguration()
        {
            MockSolution solution = _serviceProvider.GetService(typeof(SVsSolution)) as MockSolution;
            MockIVsProject vsProject = new MockIVsProject("c:\\temp.proj");
            solution.AddProject(vsProject);

            CodeSweep.VSPackage.NonMSBuildProjectConfigStore_Accessor accessor = new CodeSweep.VSPackage.NonMSBuildProjectConfigStore_Accessor(vsProject, _serviceProvider);

            Assert.IsFalse(accessor.HasConfiguration, "HasConfiguration was true for a new project.");

            accessor.CreateDefaultConfiguration();

            Assert.IsTrue(accessor.HasConfiguration, "HasConfiguration was false after CreateDefaultConfiguration.");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void TermTables()
        {
            MockSolution solution = _serviceProvider.GetService(typeof(SVsSolution)) as MockSolution;
            MockIVsProject vsProject = new MockIVsProject("c:\\temp.proj");
            solution.AddProject(vsProject);

            CodeSweep.VSPackage.NonMSBuildProjectConfigStore_Accessor accessor = new CodeSweep.VSPackage.NonMSBuildProjectConfigStore_Accessor(vsProject, _serviceProvider);

            Assert.AreEqual(0, accessor.TermTableFiles.Count, "TermTableFiles not initially empty.");

            accessor.CreateDefaultConfiguration();

            Assert.AreEqual(1, accessor.TermTableFiles.Count, "TermTableFiles wrong size after CreateDefaultConfiguration.");

            MockDTE dte = _serviceProvider.GetService(typeof(DTE)) as MockDTE;
            MockDTEGlobals globals = dte.Solution.Projects.Item(0).Globals as MockDTEGlobals;

            globals.ClearNonPersistedVariables();

            // Create a new proj config store to see if the change was persisted.
            CodeSweep.VSPackage.NonMSBuildProjectConfigStore_Accessor accessor2 = new CodeSweep.VSPackage.NonMSBuildProjectConfigStore_Accessor(vsProject, _serviceProvider);

            Assert.AreEqual(1, accessor2.TermTableFiles.Count, "CreateDefaultConfiguration changes did not persist.");

            accessor.TermTableFiles.Remove(Utilities.ListFromEnum(accessor.TermTableFiles)[0]);
            globals.ClearNonPersistedVariables();

            // Create a new proj config store to see if the change was persisted.
            CodeSweep.VSPackage.NonMSBuildProjectConfigStore_Accessor accessor3 = new CodeSweep.VSPackage.NonMSBuildProjectConfigStore_Accessor(vsProject, _serviceProvider);

            Assert.AreEqual(0, accessor3.TermTableFiles.Count, "Deletion did not persist.");

            accessor.TermTableFiles.Add("c:\\foo");
            accessor.TermTableFiles.Add("c:\\bar");
            globals.ClearNonPersistedVariables();

            // Create a new proj config store to see if the change was persisted.
            CodeSweep.VSPackage.NonMSBuildProjectConfigStore_Accessor accessor4 = new CodeSweep.VSPackage.NonMSBuildProjectConfigStore_Accessor(vsProject, _serviceProvider);

            Assert.AreEqual(2, accessor4.TermTableFiles.Count, "Additions did not persist.");
        }

        // Note: the IgnoreInstances property can't be accessed from these tests because the test
        // framework doesn't support generic types in that capacity.
    }


}
