/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System.Collections.Generic;
using Microsoft.Samples.VisualStudio.CodeSweep.Scanner;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    /// <summary>
    ///This is a test class for CodeSweep.VSPackage.ScannerHost and is intended
    ///to contain all CodeSweep.VSPackage.ScannerHost Unit Tests
    ///</summary>
    [TestClass()]
    public class ScannerHostTest
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


        /// <summary>
        ///A test for AddResult (IScanResult, string)
        ///</summary>
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void AddResultTest()
        {
            CodeSweep.VSPackage.ScannerHost_Accessor accessor = new CodeSweep.VSPackage.ScannerHost_Accessor(_serviceProvider);

            MockTermTable table = new MockTermTable("scannedFile");
            MockTerm term = new MockTerm("term text", 0, "term class", "term comment", "recommended", table);
            MockScanHit hit = new MockScanHit("scannedFile", 5, 6, "line text", term, null);
            MockScanResult scanResult = new MockScanResult("scannedFile", new IScanHit[] { hit }, true);

            List<int> resultCounts = new List<int>();
            MockTaskList taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as MockTaskList;
            taskList.OnRefreshTasks += delegate(object sender, MockTaskList.RefreshTasksArgs args)
            {
                resultCounts.Add(Utilities.TasksOfProvider(args.Provider).Count);
            };

            accessor.AddResult(scanResult, "c:\\projFile");

            Assert.AreEqual(1, resultCounts.Count, "Task list was not updated by AddResult.");
            Assert.AreEqual(1, resultCounts[0], "Refresh did not enumerate one task.");
        }

    }


}
