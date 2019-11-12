/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using Microsoft.Samples.VisualStudio.CodeSweep.BuildTask;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    /// <summary>
    ///This is a test class for CodeSweep.VSPackage.IgnoreInstance and is intended
    ///to contain all CodeSweep.VSPackage.IgnoreInstance Unit Tests
    ///</summary>
    [TestClass()]
    public class IgnoreInstanceTest
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


        [DeploymentItem("BuildTask.dll")]
        [TestMethod()]
        public void ConstructWithInvalidArgs()
        {
            bool thrown = Utilities.HasFunctionThrown<ArgumentNullException>(delegate { new BuildTask.IgnoreInstance_Accessor(null, "line text", "text", 0); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentNullException for null file name.");

            thrown = Utilities.HasFunctionThrown<ArgumentNullException>(delegate { new BuildTask.IgnoreInstance_Accessor("c:\\file.ext", null, "text", 0); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentNullException for null line text.");

            thrown = Utilities.HasFunctionThrown<ArgumentNullException>(delegate { new BuildTask.IgnoreInstance_Accessor("c:\\file.ext", "line text", null, 0); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentNullException for null term.");

            thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { new BuildTask.IgnoreInstance_Accessor("", "line text", "text", 0); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentException for empty file name.");

            thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { new BuildTask.IgnoreInstance_Accessor("c:\\file.ext", "", "text", 0); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentException for empty line text.");

            thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { new BuildTask.IgnoreInstance_Accessor("c:\\file.ext", "line text", "", 0); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentException for empty term.");

            thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { new BuildTask.IgnoreInstance_Accessor("c:\\file.ext", "line text", "text", -1); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentException for negative column.");

            thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { new BuildTask.IgnoreInstance_Accessor("c:\\file.ext", "line text", "text", 9); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentException for column too high.");
        }

        [DeploymentItem("BuildTask.dll")]
        [TestMethod()]
        public void ConstructAndTestProperties()
        {
            CodeSweep.BuildTask.IgnoreInstance_Accessor accessor = new CodeSweep.BuildTask.IgnoreInstance_Accessor("c:\\file.ext", "line text", "text", 5);

            Assert.AreEqual("c:\\file.ext", accessor.FilePath, "FilePath property incorrect.");
            Assert.AreEqual("line text", accessor.IgnoredLine, "IgnoredLine property incorrect.");
            Assert.AreEqual("text", accessor.IgnoredTerm, "IgnoredTerm property incorrect.");
            Assert.AreEqual(5, accessor.PositionOfIgnoredTerm, "PositionOfIgnoredTerm property incorrect.");
        }

        [DeploymentItem("BuildTask.dll")]
        [TestMethod()]
        public void DeserializeWithInvalidArgs()
        {
            // null serialization text
            bool thrown = Utilities.HasFunctionThrown<ArgumentNullException>(delegate { new BuildTask.IgnoreInstance_Accessor(null, "c:\\"); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentNullException for null serialization string.");

            // null project path
            thrown = Utilities.HasFunctionThrown<ArgumentNullException>(delegate { new BuildTask.IgnoreInstance_Accessor("filename,term,0,linetext", null); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentNullException for null project path.");

            // empty project path
            thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { new BuildTask.IgnoreInstance_Accessor("filename,term,0,linetext", ""); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentException for empty project path.");

            // non-rooted project path
            thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { new BuildTask.IgnoreInstance_Accessor("filename,term,0,linetext", "..\\"); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentException for non-rooted project path.");

            // too many fields
            thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { new BuildTask.IgnoreInstance_Accessor("filename,term,0,linetext,foo", "c:\\"); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentException with too many fields.");

            // too few fields
            thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { new BuildTask.IgnoreInstance_Accessor("filename,term,0", "c:\\"); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentException with too few fields.");

            // file field is empty
            thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { new BuildTask.IgnoreInstance_Accessor(",term,0,linetext", "c:\\"); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentException with empty file field.");

            // term field is empty
            thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { new BuildTask.IgnoreInstance_Accessor("filename,,0,linetext", "c:\\"); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentException with empty term field.");

            // column field is empty
            thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { new BuildTask.IgnoreInstance_Accessor("filename,term,,linetext", "c:\\"); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentException with empty column field.");

            // line text field is empty
            thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { new BuildTask.IgnoreInstance_Accessor("filename,term,0,", "c:\\"); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentException with empty line text field.");

            // column field cannot be parsed
            thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { new BuildTask.IgnoreInstance_Accessor("filename,term,abc,linetext", "c:\\"); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentException with unparsable column field.");

            // column field is too low
            thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { new BuildTask.IgnoreInstance_Accessor("filename,term,-1,linetext", "c:\\"); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentException with negative column field.");

            // column field is too high
            thrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { new BuildTask.IgnoreInstance_Accessor("filename,term,8,linetext", "c:\\"); });
            Assert.IsTrue(thrown, "Constructor did not throw ArgumentException with column field too high.");
        }

        [DeploymentItem("BuildTask.dll")]
        [TestMethod()]
        public void DeserializeAndTestProperties()
        {
            CodeSweep.BuildTask.IgnoreInstance_Accessor accessor = new CodeSweep.BuildTask.IgnoreInstance_Accessor("subdir\\\\file.ext,term,5,line\\,text with commas\\,", "c:\\");

            Assert.AreEqual("c:\\subdir\\file.ext", accessor.FilePath, "FilePath property incorrect.");
            Assert.AreEqual("line,text with commas,", accessor.IgnoredLine, "IgnoredLine property incorrect.");
            Assert.AreEqual("term", accessor.IgnoredTerm, "IgnoredTerm property incorrect.");
            Assert.AreEqual(5, accessor.PositionOfIgnoredTerm, "PositionOfIgnoredTerm property incorrect.");
        }

        [DeploymentItem("BuildTask.dll")]
        [TestMethod()]
        public void Serialize()
        {
            CodeSweep.BuildTask.IgnoreInstance_Accessor accessor = new CodeSweep.BuildTask.IgnoreInstance_Accessor("c:\\subdir\\file.ext", "line text, with commas", "text", 5);

            string serialization = accessor.Serialize("c:\\");
            Assert.AreEqual("subdir\\\\file.ext,text,5,line text\\, with commas", serialization, "Serialize did not return expected string.");

            CodeSweep.BuildTask.IgnoreInstance_Accessor accessor2 = new CodeSweep.BuildTask.IgnoreInstance_Accessor(serialization, "c:\\");

            Assert.AreEqual("c:\\subdir\\file.ext", accessor.FilePath, "FilePath property incorrect.");
            Assert.AreEqual("line text, with commas", accessor.IgnoredLine, "IgnoredLine property incorrect.");
            Assert.AreEqual("text", accessor.IgnoredTerm, "IgnoredTerm property incorrect.");
            Assert.AreEqual(5, accessor.PositionOfIgnoredTerm, "PositionOfIgnoredTerm property incorrect.");
        }

        [DeploymentItem("BuildTask.dll")]
        [TestMethod()]
        public void Compare()
        {
            CodeSweep.BuildTask.IgnoreInstance_Accessor accessor = new CodeSweep.BuildTask.IgnoreInstance_Accessor("file", "line", "term", 0);

            CodeSweep.BuildTask.IgnoreInstance_Accessor accessor2 = new CodeSweep.BuildTask.IgnoreInstance_Accessor("file", "line", "term", 0);

            Assert.AreEqual(0, accessor.CompareTo((IIgnoreInstance)accessor2.Target), "Comparison to equal instance returned wrong value.");

            CodeSweep.BuildTask.IgnoreInstance_Accessor accessor3 = new CodeSweep.BuildTask.IgnoreInstance_Accessor("file1", "line", "term", 0);

            Assert.AreNotEqual(0, accessor.CompareTo((IIgnoreInstance)accessor3.Target), "Comparison with different file returned wrong value.");

            CodeSweep.BuildTask.IgnoreInstance_Accessor accessor4 = new CodeSweep.BuildTask.IgnoreInstance_Accessor("file", "line1", "term", 0);

            Assert.AreNotEqual(0, accessor.CompareTo((IIgnoreInstance)accessor4.Target), "Comparison with different line text returned wrong value.");

            CodeSweep.BuildTask.IgnoreInstance_Accessor accessor5 = new CodeSweep.BuildTask.IgnoreInstance_Accessor("file", "line", "term1", 0);

            Assert.AreNotEqual(0, accessor.CompareTo((IIgnoreInstance)accessor5.Target), "Comparison with different term returned wrong value.");

            CodeSweep.BuildTask.IgnoreInstance_Accessor accessor6 = new CodeSweep.BuildTask.IgnoreInstance_Accessor("file", "line", "term", 1);

            Assert.AreNotEqual(0, accessor.CompareTo((IIgnoreInstance)accessor6.Target), "Comparison with different column returned wrong value.");
        }

    }


}
