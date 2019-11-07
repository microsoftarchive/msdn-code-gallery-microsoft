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
using System.Text;
using Microsoft.Samples.VisualStudio.CodeSweep.Scanner;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    /// <summary>
    ///This is a test class for CodeSweep.Scanner.Scanner and is intended
    ///to contain all CodeSweep.Scanner.Scanner Unit Tests
    ///</summary>
    [TestClass()]
    public class ScannerTest
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
        }
        //
        #endregion


        /// <summary>
        /// A test case for Scan (IEnumerable&lt;string&gt;)
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod()]
        public void ScanWithNullList()
        {
            Scanner_Accessor accessor = new Scanner_Accessor();

            bool thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { accessor.Scan(null, null); });
            Assert.IsTrue(thrown, "Scanner.Scanner.Scan did not throw ArgumentNullException with null list.");
        }

        /// <summary>
        /// A test case for Scan (IEnumerable&lt;string&gt;)
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod()]
        public void ScanWithEmptyList()
        {
            Scanner_Accessor accessor = new Scanner_Accessor();

            IMultiFileScanResult result = accessor.Scan(new List<string>(), new List<ITermTable>());

            Assert.AreEqual(result.Attempted, 0, "Attempted != 0 in return value from Scanner.Scanner.Scan with empty list.");
            Assert.AreEqual(result.FailedScan, 0, "FailedScan != 0 in return value from Scanner.Scanner.Scan with empty list.");
            Assert.AreEqual(result.PassedScan, 0, "PassedScan != 0 in return value from Scanner.Scanner.Scan with empty list.");
            Assert.AreEqual(result.UnableToScan, 0, "UnableToScan != 0 in return value from Scanner.Scanner.Scan with empty list.");
            Assert.IsNotNull(result.Results, "Results property was null in return value from Scanner.Scanner.Scan with empty list.");

            int count = 0;
            foreach (IScanResult scanResult in result.Results)
            {
                ++count;
            }
            Assert.AreEqual(count, 0, "Results list was not empty in return value from Scanner.Scanner.Scan with empty list.");
        }

        /// <summary>
        /// A test case for Scan (IEnumerable&lt;string&gt;)
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod()]
        public void ScanWithAllPassingFiles()
        {
            List<string> filePaths = new List<string>();

            filePaths.Add(Utilities.CreateTempTxtFile(""));
            filePaths.Add(Utilities.CreateTempTxtFile("some text"));
            filePaths.Add(Utilities.CreateTempTxtFile("some more text"));

            Scanner_Accessor accessor = new Scanner_Accessor();

            IMultiFileScanResult result = accessor.Scan(filePaths, new List<ITermTable>());

            Assert.AreEqual(result.Attempted, 3, "Attempted != 3 in return value from Scanner.Scanner.Scan with 3 passing files.");
            Assert.AreEqual(result.FailedScan, 0, "FailedScan != 0 in return value from Scanner.Scanner.Scan with 3 passing files.");
            Assert.AreEqual(result.PassedScan, 3, "PassedScan != 3 in return value from Scanner.Scanner.Scan with 3 passing files.");
            Assert.AreEqual(result.UnableToScan, 0, "UnableToScan != 0 in return value from Scanner.Scanner.Scan with 3 passing files.");
            Assert.IsNotNull(result.Results, "Results property was null in return value from Scanner.Scanner.Scan with 3 passing files.");

            int count = 0;
            foreach (IScanResult scanResult in result.Results)
            {
                ++count;
            }
            Assert.AreEqual(count, 3, "Results list did not contain 3 entries in return value from Scanner.Scanner.Scan with 3 passing files.");
        }

        private void InternalScanWithSomeFailingFiles(bool useCallback)
        {
            IScanner target = Factory.GetScanner();

            MockTermTable table = new MockTermTable("file");
            table.AddSearchTerm(new MockTerm("foo", 1, "", "", "", table));
            table.AddSearchTerm(new MockTerm("bar", 1, "", "", "", table));

            List<string> filePaths = new List<string>();

            filePaths.Add(Utilities.CreateTempTxtFile("line 1\nfoo\nline 3"));
            filePaths.Add(Utilities.CreateTempTxtFile("line 1\nline 2\nline 3"));
            filePaths.Add(Utilities.CreateTempTxtFile("bar"));

            IMultiFileScanResult scanResults;
            if (useCallback)
            {
                List<IScanResult> callbackResults = new List<IScanResult>();
                scanResults = target.Scan(filePaths, new ITermTable[] { table }, delegate(IScanResult result) { callbackResults.Add(result); });

                int i = 0;
                foreach (IScanResult result in scanResults.Results)
                {
                    Assert.AreEqual(callbackResults[i++], result);
                }
            }
            else
            {
                scanResults = target.Scan(filePaths, new ITermTable[] { table });
            }

            Assert.AreEqual(scanResults.Attempted, 3, "Attempted != 3 in return value from Scanner.Scanner.Scan with 1 passing / 2 failing files.");
            Assert.AreEqual(scanResults.FailedScan, 2, "FailedScan != 2 in return value from Scanner.Scanner.Scan with 1 passing / 2 failing files.");
            Assert.AreEqual(scanResults.PassedScan, 1, "PassedScan != 1 in return value from Scanner.Scanner.Scan with 1 passing / 2 failing files.");
            Assert.AreEqual(scanResults.UnableToScan, 0, "UnableToScan != 0 in return value from Scanner.Scanner.Scan with 1 passing / 2 failing files.");
            Assert.IsNotNull(scanResults.Results, "Results property was null in return value from Scanner.Scanner.Scan with 1 passing / 2 failing files.");

            int count = 0;
            foreach (IScanResult scanResult in scanResults.Results)
            {
                ++count;
            }
            Assert.AreEqual(count, 3, "Results list did not contain 3 entries in return value from Scanner.Scanner.Scan with 1 passing / 2 failing files.");
        }

        private void InternalScanWithInvalidEntries(bool useCallback)
        {
            IScanner target = Factory.GetScanner();

            List<string> filePaths = new List<string>();

            // Hold a file open with non-shared access so it can't be opened by the scanner.
            string holdOpen = Utilities.CreateTempTxtFile("some text");
            FileStream file = File.Open(holdOpen, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);

            filePaths.Add("z:\nonexistant.cpp");
            filePaths.Add(Utilities.CreateTempTxtFile("line 1\nline 2\nline 3"));
            filePaths.Add(holdOpen);

            MockTermTable table = new MockTermTable("file");

            IMultiFileScanResult scanResults;
            if (useCallback)
            {
                List<IScanResult> callbackResults = new List<IScanResult>();
                scanResults = target.Scan(filePaths, new ITermTable[] { table }, delegate(IScanResult result) { callbackResults.Add(result); });

                int i = 0;
                foreach (IScanResult result in scanResults.Results)
                {
                    Assert.AreEqual(callbackResults[i++], result);
                }
            }
            else
            {
                scanResults = target.Scan(filePaths, new ITermTable[] { table });
            }

            file.Close();

            Assert.AreEqual(3, scanResults.Attempted, "Attempted != 3 in return value from Scanner.Scanner.Scan with 1 passing / 2 invalid files.");
            Assert.AreEqual(0, scanResults.FailedScan, "FailedScan != 0 in return value from Scanner.Scanner.Scan with 1 passing / 2 invalid files.");
            Assert.AreEqual(1, scanResults.PassedScan, "PassedScan != 1 in return value from Scanner.Scanner.Scan with 1 passing / 2 invalid files.");
            Assert.AreEqual(2, scanResults.UnableToScan, "UnableToScan != 2 in return value from Scanner.Scanner.Scan with 1 passing / 2 invalid files.");
            Assert.IsNotNull(scanResults.Results, "Results property was null in return value from Scanner.Scanner.Scan with 1 passing / 2 invalid files.");

            int count = 0;
            foreach (IScanResult scanResult in scanResults.Results)
            {
                ++count;
            }
            Assert.AreEqual(count, 3, "Results list did not contain 3 entries in return value from Scanner.Scanner.Scan with 1 passing / 2 invalid files.");
        }

        /// <summary>
        /// A test case for Scan (IEnumerable&lt;string&gt;)
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod()]
        public void ScanWithSomeFailingFiles()
        {
            InternalScanWithSomeFailingFiles(false);
        }

        /// <summary>
        /// A test case for Scan (IEnumerable&lt;string&gt;)
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod()]
        public void ScanWithSomeFailingFilesWithCallback()
        {
            InternalScanWithSomeFailingFiles(true);
        }

        /// <summary>
        /// A test case for Scan (IEnumerable&lt;string&gt;)
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod()]
        public void ScanWithInvalidEntries()
        {
            InternalScanWithInvalidEntries(false);
        }

        /// <summary>
        /// A test case for Scan (IEnumerable&lt;string&gt;)
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod()]
        public void ScanWithInvalidEntriesWithCallback()
        {
            InternalScanWithInvalidEntries(true);
        }

        [DeploymentItem("Scanner.dll")]
        [TestMethod()]
        public void ScanWithDifferentEncodings()
        {
            List<string> filePaths = new List<string>();

            filePaths.Add(Utilities.CreateTempTxtFile("first file", Encoding.Unicode));
            filePaths.Add(Utilities.CreateTempTxtFile("second file", Encoding.BigEndianUnicode));
            filePaths.Add(Utilities.CreateTempTxtFile("third file", Encoding.UTF32));
            filePaths.Add(Utilities.CreateTempTxtFile("fourth file", Encoding.UTF8));
            filePaths.Add(Utilities.CreateTempTxtFile("fifth file", Encoding.UTF7));
            filePaths.Add(Utilities.CreateTempTxtFile("sixth file", Encoding.ASCII));

            // TODO: what about different code pages within the ASCII encoding?

            List<ITermTable> termTables = new List<ITermTable>();

            MockTermTable table = new MockTermTable("file");
            table.AddSearchTerm(new MockTerm("first", 1, "class", "comment", "recommended", table));
            table.AddSearchTerm(new MockTerm("second", 1, "class", "comment", "recommended", table));
            table.AddSearchTerm(new MockTerm("third", 1, "class", "comment", "recommended", table));
            table.AddSearchTerm(new MockTerm("fourth", 1, "class", "comment", "recommended", table));
            table.AddSearchTerm(new MockTerm("fifth", 1, "class", "comment", "recommended", table));
            table.AddSearchTerm(new MockTerm("sixth", 1, "class", "comment", "recommended", table));

            termTables.Add(table);

            Scanner_Accessor accessor = new Scanner_Accessor();

            IMultiFileScanResult result = accessor.Scan(filePaths, termTables);

            Assert.AreEqual(6, result.Attempted, "Attempted count incorrect.");
            Assert.AreEqual(6, result.FailedScan, "FailedScan count incorrect.");
            Assert.AreEqual(0, result.PassedScan, "PassedScan count incorrect.");
            Assert.AreEqual(0, result.UnableToScan, "UnableToScan count incorrect.");

            int fileIndex = 0;
            foreach (IScanResult scanResult in result.Results)
            {
                int count = 0;
                foreach (IScanHit hits in scanResult.Results)
                {
                    ++count;
                }
                Assert.AreEqual(1, count, "Result " + fileIndex.ToString() + " did not have the expected number of hits.");
                ++fileIndex;
            }
        }

        // TODO: somewhere (either here or in another test module), we need to ensure that the
        // hits returned from a scan have all the right attributes.

        // TODO: test allowed vs. disallowed file extensions
    }


}
