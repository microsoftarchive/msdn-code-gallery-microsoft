/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Samples.VisualStudio.CodeSweep.BuildTask;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    /// <summary>
    ///This is a test class for CodeSweep.BuildTask.ScannerTask and is intended
    ///to contain all CodeSweep.BuildTask.ScannerTask Unit Tests
    ///</summary>
    [TestClass()]
    public class ScannerTaskTest
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
        }
        //
        #endregion


        private string CreateTermTableXml(string term, int severity, string termClass, string comment)
        {
            StringBuilder fileContent = new StringBuilder();

            fileContent.Append("<?xml version=\"1.0\"?>\n");
            fileContent.Append("<xmldata>\n");
            fileContent.Append("  <PLCKTT>\n");
            fileContent.Append("    <Lang>\n");
            fileContent.Append("      <Term Term=\"" + term + "\" Severity=\"" + severity.ToString() + "\" TermClass=\"" + termClass + "\">\n");
            fileContent.Append("        <Comment>" + comment + "</Comment>\n");
            fileContent.Append("      </Term>\n");
            fileContent.Append("    </Lang>\n");
            fileContent.Append("  </PLCKTT>\n");
            fileContent.Append("</xmldata>\n");

            return fileContent.ToString();
        }

        /// <summary>
        ///A test case for Execute ()
        ///</summary>
        [DeploymentItem("BuildTask.dll")]
        [TestMethod()]
        public void ExecuteWithoutHost()
        {
            ScannerTask target = new ScannerTask();

            // Create the term tables and target files.
            string termFile1 = Utilities.CreateTempFile(CreateTermTableXml("countries", 2, "Geopolitical", "comment"));
            string termFile2 = Utilities.CreateTempFile(CreateTermTableXml("shoot", 3, "Profanity", "comment"));

            string scanFile1 = Utilities.CreateTempTxtFile("the word 'countries' should produce a hit");
            string scanFile2 = Utilities.CreateTempTxtFile("the word 'shoot' should produce a hit");

            // Create the project that will execute the task.
            Microsoft.Build.Evaluation.Project project = Utilities.SetupMSBuildProject(new string[] { scanFile1, scanFile2 }, new string[] { termFile1, termFile2 });

            // Set up a custom logger to capture the output.
            MockLogger logger = new MockLogger();
            project.ProjectCollection.RegisterLogger(logger);
            int errors = 0;
            int warnings = 0;
            int messages = 0;
            logger.OnError += delegate(object sender, BuildErrorEventArgs args)
            {
                ++errors;
            };
            logger.OnWarning += delegate(object sender, BuildWarningEventArgs args)
            {
                ++warnings;
            };
            logger.OnMessage += delegate(object sender, BuildMessageEventArgs args)
            {
                ++messages;
            };

            project.Build("AfterBuild");

            Assert.AreEqual(0, errors, "Build did not log expected number of errors.");
            Assert.AreEqual(2, warnings, "Build did not log expected number of warnings.");
            Assert.AreEqual(2, messages, "Build did not log expected number of messages.");
        }

        [DeploymentItem("BuildTask.dll")]
        [TestMethod()]
        public void ExecuteWithHost()
        {
            ScannerTask target = new ScannerTask();

            // Create the term tables and target files.
            string termFile1 = Utilities.CreateTempFile(CreateTermTableXml("countries", 2, "Geopolitical", "comment"));
            string termFile2 = Utilities.CreateTempFile(CreateTermTableXml("shoot", 3, "Profanity", "comment"));

            string scanFile1 = Utilities.CreateTempTxtFile("the word 'countries' should produce a hit");
            string scanFile2 = Utilities.CreateTempTxtFile("the word 'shoot' should produce a hit");

            // Create the project that will execute the task.
            Microsoft.Build.Evaluation.Project project = Utilities.SetupMSBuildProject(new string[] { scanFile1, scanFile2 }, new string[] { termFile1, termFile2 });

            // Set up a custom logger to capture the output.
            MockLogger logger = new MockLogger();
            project.ProjectCollection.RegisterLogger(logger);
            int errors = 0;
            int warnings = 0;
            int messages = 0;
            logger.OnError += delegate(object sender, BuildErrorEventArgs args)
            {
                ++errors;
            };
            logger.OnWarning += delegate(object sender, BuildWarningEventArgs args)
            {
                ++warnings;
            };
            logger.OnMessage += delegate(object sender, BuildMessageEventArgs args)
            {
                ++messages;
            };

            Microsoft.Build.Construction.ProjectTaskElement task = Utilities.GetScannerTask(project);

            // Set the host object for the task.
            int hostUpdates = 0;
            MockHostObject host = new MockHostObject();
            host.OnAddResult += delegate(object sender, MockHostObject.AddResultArgs args)
            {
                ++hostUpdates;
            };
            logger.OnBuildStart += delegate(object sender, BuildStartedEventArgs args)
            {
                project.ProjectCollection.HostServices.RegisterHostObject(project.FullPath, "AfterBuild", "ScannerTask", host);
            };

            project.Build("AfterBuild");

            Assert.AreEqual(0, errors, "Build did not log expected number of errors.");
            Assert.AreEqual(0, warnings, "Build did not log expected number of warnings.");
            Assert.AreEqual(4, messages, "Build did not log expected number of messages.");
            Assert.AreEqual(2, hostUpdates, "Build did not send expected number of results to host.");
        }

        /// <summary>
        ///A test case for TermTables
        ///</summary>
        [DeploymentItem("BuildTask.dll")]
        [TestMethod()]
        public void TermTablesTest()
        {
            ScannerTask target = new ScannerTask();

            SetAndGetTermTables(target, null);
            SetAndGetTermTables(target, "");
            SetAndGetTermTables(target, "z:\fileName.ext");
            SetAndGetTermTables(target, "first;second;third");
        }

        private void SetAndGetTermTables(ScannerTask target, string val)
        {
            target.TermTables = val;
            Assert.AreEqual(val, target.TermTables, "TermTables did not return expected value (" + val + ").");
        }

        private class MockTaskItem : ITaskItem
        {
            readonly string _file;

            public MockTaskItem(string file)
            {
                _file = file;
            }

            #region ITaskItem Members

            public System.Collections.IDictionary CloneCustomMetadata()
            {
                throw new System.Exception("The method or operation is not implemented.");
            }

            public void CopyMetadataTo(ITaskItem destinationItem)
            {
                throw new System.Exception("The method or operation is not implemented.");
            }

            public string GetMetadata(string metadataName)
            {
                throw new System.Exception("The method or operation is not implemented.");
            }

            public string ItemSpec
            {
                get
                {
                    return _file;
                }
                set
                {
                    throw new System.Exception("The method or operation is not implemented.");
                }
            }

            public int MetadataCount
            {
                get { throw new System.Exception("The method or operation is not implemented."); }
            }

            public System.Collections.ICollection MetadataNames
            {
                get { throw new System.Exception("The method or operation is not implemented."); }
            }

            public void RemoveMetadata(string metadataName)
            {
                throw new System.Exception("The method or operation is not implemented.");
            }

            public void SetMetadata(string metadataName, string metadataValue)
            {
                throw new System.Exception("The method or operation is not implemented.");
            }

            #endregion
        }

        /// <summary>
        ///A test case for FilesToScan
        ///</summary>
        [DeploymentItem("BuildTask.dll")]
        [TestMethod()]
        public void FilesToScanTest()
        {
            ScannerTask target = new ScannerTask();

            SetAndGetFilesToScan(target, null);
            SetAndGetFilesToScan(target, new ITaskItem[] { });
            SetAndGetFilesToScan(target, new ITaskItem[] { new MockTaskItem("foo"), new MockTaskItem("bar") });
        }

        private void SetAndGetFilesToScan(ScannerTask target, ITaskItem[] val)
        {
            target.FilesToScan = val;
            if (val == null)
            {
                Assert.IsNull(target.FilesToScan, "FilesToScan did not return expected value (null).");
            }
            else
            {
                Assert.AreEqual(val.Length, target.FilesToScan.Length, "FilesToScan returned array of wrong length.");
                for (int i = 0; i < val.Length; ++i)
                {
                    Assert.AreEqual(val[i], target.FilesToScan[i], "Item " + i.ToString() + " of FilesToScan is incorrect.");
                }
            }
        }

    }


}
