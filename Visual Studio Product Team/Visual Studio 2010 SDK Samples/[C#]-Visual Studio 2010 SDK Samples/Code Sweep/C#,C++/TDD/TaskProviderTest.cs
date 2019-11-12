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
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.Build.Evaluation;
using Microsoft.Samples.VisualStudio.CodeSweep.Scanner;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    /// <summary>
    ///This is a test class for CodeSweep.VSPackage.TaskProvider and is intended
    ///to contain all CodeSweep.VSPackage.TaskProvider Unit Tests
    ///</summary>
    [TestClass()]
    public class TaskProviderTest
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
        //    _serviceProvider = new MockServiceProvider();
        //}
        
        //Use TestCleanup to run code after each test has run
        //
        [TestCleanup()]
        public void MyTestCleanup()
        {
            Utilities.CleanUpTempFiles();
            Utilities.RemoveCommandHandlers(_serviceProvider);

            MockTaskList taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as MockTaskList;
            taskList.Clear();

            CodeSweep.VSPackage.Factory_Accessor._taskProvider = null;
        }
        //
        #endregion

        // Keep this in sync with the one in task.cs.
        public enum TaskFields
        {
            Priority,
            PriorityNumber,
            Term,
            Class,
            Replacement,
            Comment,
            File,
            Line,
            Project
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod]
        public void AddDuplicateTermsAndVerifyWarningAppearsOnce()
        {
            CodeSweep.VSPackage.TaskProvider_Accessor accessor = new CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider);

            MockTermTable table = new MockTermTable("termtable.xml");
            MockTerm term0 = new MockTerm("dupText", 0, "term0Class", "term0Comment", "term0recommended", table);
            MockTerm term1 = new MockTerm("dupText", 3, "term1Class", "term1Comment", "term1recommended", table);
            MockTerm term2 = new MockTerm("term2Text", 2, "term2Class", "term2Comment", "term2recommended", table);
            MockScanHit hit0 = new MockScanHit("file0", 1, 10, "line text", term0, "warning 1");
            MockScanHit hit1 = new MockScanHit("file1", 4, 1, "line text 2", term1, "warning 2");
            MockScanHit hit2 = new MockScanHit("file2", 3, 2, "line text 3", term2, "warning 3");
            MockScanResult scanResult = new MockScanResult("file0", new IScanHit[] { hit0, hit1, hit2 }, true);
            accessor.AddResult(scanResult, "c:\\projFile");

            IVsEnumTaskItems enumerator;
            int hr = accessor.EnumTaskItems(out enumerator);

            Assert.AreEqual(VSConstants.S_OK, hr, "EnumTaskItems returned wrong hresult.");

            // Verify there are three hits plus two warnings.
            List<IVsTaskItem> tasks = Utilities.TasksFromEnumerator(enumerator);
            Assert.AreEqual(5, tasks.Count, "EnumTaskItems did not enumerate correct number of tasks.");

            string[] termValues = new string[5] { "dupText", "dupText", "dupText", "term2Text", "term2Text" };
            int[] priorityValues = new int[5] { 0, 0, 3, 2, 2 };
            string[] classValues = new string[5] { "term0Class", "term0Class", "term1Class", "term2Class", "term2Class" };
            string[] replacementValues = new string[5] { "", "term0recommended", "term1recommended", "", "term2recommended" };
            string[] commentValues = new string[5] { "warning 1", "term0Comment", "term1Comment", "warning 3", "term2Comment" };
            string[] fileValues = new string[5] { "", "file0", "file1", "", "file2" };

            for (int taskIndex = 0; taskIndex < tasks.Count; ++taskIndex)
            {
                uint type;
                object val;
                uint flags;
                string acc;

                hr = (tasks[taskIndex] as IVsTaskItem3).GetColumnValue((int)TaskFields.Term, out type, out flags, out val, out acc);
                Assert.AreEqual(VSConstants.S_OK, hr);
                Assert.AreEqual(__VSTASKVALUETYPE.TVT_TEXT, (__VSTASKVALUETYPE)type, "Task " + taskIndex.ToString() + " term type is wrong.");
                Assert.AreEqual(termValues[taskIndex], (string)val, "Task " + taskIndex.ToString() + " term text is wrong.");

                hr = (tasks[taskIndex] as IVsTaskItem3).GetColumnValue((int)TaskFields.Comment, out type, out flags, out val, out acc);
                Assert.AreEqual(VSConstants.S_OK, hr);
                Assert.AreEqual(__VSTASKVALUETYPE.TVT_LINKTEXT, (__VSTASKVALUETYPE)type, "Task " + taskIndex.ToString() + " comment type is wrong.");
                Assert.AreEqual(commentValues[taskIndex], (string)val, "Task " + taskIndex.ToString() + " comment text is wrong.");

                hr = (tasks[taskIndex] as IVsTaskItem3).GetColumnValue((int)TaskFields.Replacement, out type, out flags, out val, out acc);
                Assert.AreEqual(VSConstants.S_OK, hr);
                Assert.AreEqual(__VSTASKVALUETYPE.TVT_TEXT, (__VSTASKVALUETYPE)type, "Task " + taskIndex.ToString() + " replacement type is wrong.");
                Assert.AreEqual(replacementValues[taskIndex], (string)val, "Task " + taskIndex.ToString() + " replacement text is wrong.");
            }
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void AddResultsThenClear()
        {
            CodeSweep.VSPackage.TaskProvider_Accessor accessor = new CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider);

            // Add some tasks
            MockTermTable table = new MockTermTable("termtable.xml");
            MockTerm term0 = new MockTerm("term0Text", 0, "term0Class", "term0Comment", "term0recommended", table);
            MockTerm term1 = new MockTerm("term1Text", 3, "term1Class", "term1Comment", "term1recommended", table);
            MockScanHit hit0 = new MockScanHit("file0", 1, 10, "line text", term0, null);
            MockScanHit hit1 = new MockScanHit("file1", 4, 1, "line text 2", term1, null);
            MockScanResult scanResult = new MockScanResult("file0", new IScanHit[] { hit0, hit1 }, true);
            accessor.AddResult(scanResult, "c:\\projFile");

            IVsEnumTaskItems enumerator;
            int hr = accessor.EnumTaskItems(out enumerator);

            Assert.AreEqual(VSConstants.S_OK, hr, "EnumTaskItems returned wrong hresult.");

            List<IVsTaskItem> tasks = Utilities.TasksFromEnumerator(enumerator);
            Assert.AreEqual(2, tasks.Count, "EnumTaskItems did not enumerate correct number of tasks.");

            accessor.Clear();

            hr = accessor.EnumTaskItems(out enumerator);

            Assert.AreEqual(VSConstants.S_OK, hr, "EnumTaskItems returned wrong hresult.");

            tasks = Utilities.TasksFromEnumerator(enumerator);
            Assert.AreEqual(0, tasks.Count, "EnumTaskItems did not enumerate correct number of tasks after Clear().");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void TestEnumerator()
        {
            CodeSweep.VSPackage.TaskProvider_Accessor accessor = new CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider);

            // Add some tasks
            MockTermTable table = new MockTermTable("termtable.xml");
            MockTerm term0 = new MockTerm("term0Text", 0, "term0Class", "term0Comment", "term0recommended", table);
            MockTerm term1 = new MockTerm("term1Text", 3, "term1Class", "term1Comment", "term1recommended", table);
            MockScanHit hit0 = new MockScanHit("file0", 1, 10, "line text", term0, null);
            MockScanHit hit1 = new MockScanHit("file1", 4, 1, "line text 2", term1, null);
            MockScanResult scanResult = new MockScanResult("file0", new IScanHit[] { hit0, hit1 }, true);
            accessor.AddResult(scanResult, "c:\\projFile");

            IVsEnumTaskItems enumerator;
            int hr = accessor.EnumTaskItems(out enumerator);

            Assert.AreEqual(VSConstants.S_OK, hr, "EnumTaskItems returned wrong hresult.");

            IVsTaskItem[] items = new IVsTaskItem[3] { null, null, null };
            uint[] fetched = new uint[1] { 0 };
            hr = enumerator.Next(3, items, fetched);
            Assert.AreEqual(VSConstants.S_FALSE, hr, "Next returned wrong hresult with celt too high.");
            Assert.AreEqual((uint)2, fetched[0], "Next returned wrong value for fetched with celt too high.");
            Assert.IsNotNull(items[0], "Next failed to set first item with celt too high.");
            Assert.IsNotNull(items[1], "Next failed to set second item with celt too high.");
            Assert.IsNull(items[2], "Next set third item with celt too high.");

            hr = enumerator.Reset();
            Assert.AreEqual(VSConstants.S_OK, hr, "Reset returned wrong hresult.");

            items[0] = items[1] = items[2] = null;
            fetched[0] = 0;
            hr = enumerator.Next(2, items, fetched);
            Assert.AreEqual(VSConstants.S_OK, hr, "Next returned wrong hresult with celt at max.");
            Assert.AreEqual((uint)2, fetched[0], "Next returned wrong value for fetched with celt at max.");
            Assert.IsNotNull(items[0], "Next failed to set first item with celt at max.");
            Assert.IsNotNull(items[1], "Next failed to set second item with celt at max.");

            enumerator.Reset();

            items[0] = items[1] = items[2] = null;
            fetched[0] = 0;
            hr = enumerator.Next(1, items, fetched);
            Assert.AreEqual(VSConstants.S_OK, hr, "Next returned wrong hresult with celt lower than max.");
            Assert.AreEqual((uint)1, fetched[0], "Next returned wrong value for fetched with celt lower than max.");
            Assert.IsNotNull(items[0], "Next failed to set first item with celt lower than max.");
            Assert.IsNull(items[1], "Next set second item with celt lower than max.");

            enumerator.Reset();

            items[0] = items[1] = items[2] = null;
            fetched[0] = 1;
            hr = enumerator.Next(0, items, fetched);
            Assert.AreEqual(VSConstants.S_OK, hr, "Next returned wrong hresult with celt = 0.");
            Assert.AreEqual((uint)0, fetched[0], "Next returned wrong value for fetched with celt = 0.");
            Assert.IsNull(items[0], "Next set first item with celt = 0.");

            enumerator.Reset();

            hr = enumerator.Skip(0);
            Assert.AreEqual(VSConstants.S_OK, hr, "Skip(0) returned wrong hresult.");

            hr = enumerator.Skip(1);
            Assert.AreEqual(VSConstants.S_OK, hr, "Skip(1) returned wrong hresult.");

            hr = enumerator.Skip(2);
            Assert.AreEqual(VSConstants.S_FALSE, hr, "Skip(2) returned wrong hresult.");

            List<IVsTaskItem> tasks = Utilities.TasksFromEnumerator(enumerator);

            IVsEnumTaskItems enumerator2 = null;
            hr = enumerator.Clone(out enumerator2);
            Assert.AreEqual(VSConstants.S_OK, hr, "Clone returned wrong hresult.");

            List<IVsTaskItem> tasks2 = Utilities.TasksFromEnumerator(enumerator2);

            Assert.IsTrue(CodeSweep.Utilities.OrderedCollectionsAreEqual(tasks, tasks2), "Clone did not produce an equivalent collection.");
        }

        /// <summary>
        ///A test case for GetColumn (int, VSTASKCOLUMN[])
        ///</summary>
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void GetColumns()
        {
            CodeSweep.VSPackage.TaskProvider_Accessor accessor = new CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider);

            VSTASKCOLUMN expectedPriority;
            expectedPriority.bstrCanonicalName = "Priority";
            expectedPriority.bstrHeading = "!";
            expectedPriority.bstrLocalizedName = "Priority";
            expectedPriority.bstrTip = "Priority";
            expectedPriority.cxDefaultWidth = 22;
            expectedPriority.cxMinWidth = 0;
            expectedPriority.fAllowHide = 1;
            expectedPriority.fAllowUserSort = 1;
            expectedPriority.fDescendingSort = 0;
            expectedPriority.fDynamicSize = 0;
            expectedPriority.fFitContent = 0;
            expectedPriority.fMoveable = 1;
            expectedPriority.fShowSortArrow = 0;
            expectedPriority.fSizeable = 1;
            expectedPriority.fVisibleByDefault = 1;
            expectedPriority.iDefaultSortPriority = -1;
            expectedPriority.iField = (int)TaskFields.Priority;
            expectedPriority.iImage = -1;

            VSTASKCOLUMN expectedPriorityNumber;
            expectedPriorityNumber.bstrCanonicalName = "Priority Number";
            expectedPriorityNumber.bstrHeading = "!#";
            expectedPriorityNumber.bstrLocalizedName = "Priority Number";
            expectedPriorityNumber.bstrTip = "Priority Number";
            expectedPriorityNumber.cxDefaultWidth = 50;
            expectedPriorityNumber.cxMinWidth = 0;
            expectedPriorityNumber.fAllowHide = 1;
            expectedPriorityNumber.fAllowUserSort = 1;
            expectedPriorityNumber.fDescendingSort = 0;
            expectedPriorityNumber.fDynamicSize = 0;
            expectedPriorityNumber.fFitContent = 0;
            expectedPriorityNumber.fMoveable = 1;
            expectedPriorityNumber.fShowSortArrow = 0;
            expectedPriorityNumber.fSizeable = 1;
            expectedPriorityNumber.fVisibleByDefault = 0;
            expectedPriorityNumber.iDefaultSortPriority = 0;
            expectedPriorityNumber.iField = (int)TaskFields.PriorityNumber;
            expectedPriorityNumber.iImage = -1;

            VSTASKCOLUMN expectedTerm;
            expectedTerm.bstrCanonicalName = "Term";
            expectedTerm.bstrHeading = "Term";
            expectedTerm.bstrLocalizedName = "Term";
            expectedTerm.bstrTip = "";
            expectedTerm.cxDefaultWidth = 103;
            expectedTerm.cxMinWidth = 0;
            expectedTerm.fAllowHide = 1;
            expectedTerm.fAllowUserSort = 1;
            expectedTerm.fDescendingSort = 0;
            expectedTerm.fDynamicSize = 1;
            expectedTerm.fFitContent = 0;
            expectedTerm.fMoveable = 1;
            expectedTerm.fShowSortArrow = 1;
            expectedTerm.fSizeable = 1;
            expectedTerm.fVisibleByDefault = 1;
            expectedTerm.iDefaultSortPriority = -1;
            expectedTerm.iField = (int)TaskFields.Term;
            expectedTerm.iImage = -1;

            VSTASKCOLUMN expectedClass;
            expectedClass.bstrCanonicalName = "Class";
            expectedClass.bstrHeading = "Class";
            expectedClass.bstrLocalizedName = "Class";
            expectedClass.bstrTip = "";
            expectedClass.cxDefaultWidth = 91;
            expectedClass.cxMinWidth = 0;
            expectedClass.fAllowHide = 1;
            expectedClass.fAllowUserSort = 1;
            expectedClass.fDescendingSort = 0;
            expectedClass.fDynamicSize = 1;
            expectedClass.fFitContent = 0;
            expectedClass.fMoveable = 1;
            expectedClass.fShowSortArrow = 1;
            expectedClass.fSizeable = 1;
            expectedClass.fVisibleByDefault = 1;
            expectedClass.iDefaultSortPriority = -1;
            expectedClass.iField = (int)TaskFields.Class;
            expectedClass.iImage = -1;

            VSTASKCOLUMN expectedReplacement;
            expectedReplacement.bstrCanonicalName = "Replacement";
            expectedReplacement.bstrHeading = "Replacement";
            expectedReplacement.bstrLocalizedName = "Replacement";
            expectedReplacement.bstrTip = "";
            expectedReplacement.cxDefaultWidth = 140;
            expectedReplacement.cxMinWidth = 0;
            expectedReplacement.fAllowHide = 1;
            expectedReplacement.fAllowUserSort = 1;
            expectedReplacement.fDescendingSort = 0;
            expectedReplacement.fDynamicSize = 0;
            expectedReplacement.fFitContent = 0;
            expectedReplacement.fMoveable = 1;
            expectedReplacement.fShowSortArrow = 1;
            expectedReplacement.fSizeable = 1;
            expectedReplacement.fVisibleByDefault = 0;
            expectedReplacement.iDefaultSortPriority = -1;
            expectedReplacement.iField = (int)TaskFields.Replacement;
            expectedReplacement.iImage = -1;

            VSTASKCOLUMN expectedComment;
            expectedComment.bstrCanonicalName = "Comment";
            expectedComment.bstrHeading = "Comment";
            expectedComment.bstrLocalizedName = "Comment";
            expectedComment.bstrTip = "";
            expectedComment.cxDefaultWidth = 400;
            expectedComment.cxMinWidth = 0;
            expectedComment.fAllowHide = 1;
            expectedComment.fAllowUserSort = 1;
            expectedComment.fDescendingSort = 0;
            expectedComment.fDynamicSize = 1;
            expectedComment.fFitContent = 0;
            expectedComment.fMoveable = 1;
            expectedComment.fShowSortArrow = 1;
            expectedComment.fSizeable = 1;
            expectedComment.fVisibleByDefault = 1;
            expectedComment.iDefaultSortPriority = -1;
            expectedComment.iField = (int)TaskFields.Comment;
            expectedComment.iImage = -1;

            VSTASKCOLUMN expectedFile;
            expectedFile.bstrCanonicalName = "File";
            expectedFile.bstrHeading = "File";
            expectedFile.bstrLocalizedName = "File";
            expectedFile.bstrTip = "";
            expectedFile.cxDefaultWidth = 92;
            expectedFile.cxMinWidth = 0;
            expectedFile.fAllowHide = 1;
            expectedFile.fAllowUserSort = 1;
            expectedFile.fDescendingSort = 0;
            expectedFile.fDynamicSize = 0;
            expectedFile.fFitContent = 0;
            expectedFile.fMoveable = 1;
            expectedFile.fShowSortArrow = 1;
            expectedFile.fSizeable = 1;
            expectedFile.fVisibleByDefault = 1;
            expectedFile.iDefaultSortPriority = 2;
            expectedFile.iField = (int)TaskFields.File;
            expectedFile.iImage = -1;

            VSTASKCOLUMN expectedLine;
            expectedLine.bstrCanonicalName = "Line";
            expectedLine.bstrHeading = "Line";
            expectedLine.bstrLocalizedName = "Line";
            expectedLine.bstrTip = "";
            expectedLine.cxDefaultWidth = 63;
            expectedLine.cxMinWidth = 0;
            expectedLine.fAllowHide = 1;
            expectedLine.fAllowUserSort = 1;
            expectedLine.fDescendingSort = 0;
            expectedLine.fDynamicSize = 0;
            expectedLine.fFitContent = 0;
            expectedLine.fMoveable = 1;
            expectedLine.fShowSortArrow = 1;
            expectedLine.fSizeable = 1;
            expectedLine.fVisibleByDefault = 1;
            expectedLine.iDefaultSortPriority = 3;
            expectedLine.iField = (int)TaskFields.Line;
            expectedLine.iImage = -1;

            VSTASKCOLUMN expectedProject;
            expectedProject.bstrCanonicalName = "Project";
            expectedProject.bstrHeading = "Project";
            expectedProject.bstrLocalizedName = "Project";
            expectedProject.bstrTip = "";
            expectedProject.cxDefaultWidth = 116;
            expectedProject.cxMinWidth = 0;
            expectedProject.fAllowHide = 1;
            expectedProject.fAllowUserSort = 1;
            expectedProject.fDescendingSort = 0;
            expectedProject.fDynamicSize = 0;
            expectedProject.fFitContent = 0;
            expectedProject.fMoveable = 1;
            expectedProject.fShowSortArrow = 1;
            expectedProject.fSizeable = 1;
            expectedProject.fVisibleByDefault = 1;
            expectedProject.iDefaultSortPriority = 1;
            expectedProject.iField = (int)TaskFields.Project;
            expectedProject.iImage = -1;

            VSTASKCOLUMN[] expectedColumns = new VSTASKCOLUMN[]
            {
                expectedPriority,
                expectedPriorityNumber,
                expectedTerm,
                expectedClass,
                expectedReplacement,
                expectedComment,
                expectedFile,
                expectedLine,
                expectedProject
            };

            VSTASKCOLUMN dummy;
            dummy.bstrCanonicalName = null;
            dummy.bstrHeading = null;
            dummy.bstrLocalizedName = null;
            dummy.bstrTip = null;
            dummy.cxDefaultWidth = -3;
            dummy.cxMinWidth = -3;
            dummy.fAllowHide = -3;
            dummy.fAllowUserSort = -3;
            dummy.fDescendingSort = -3;
            dummy.fDynamicSize = -3;
            dummy.fFitContent = -3;
            dummy.fMoveable = -3;
            dummy.fShowSortArrow = -3;
            dummy.fSizeable = -3;
            dummy.fVisibleByDefault = -3;
            dummy.iDefaultSortPriority = -3;
            dummy.iField = -3;
            dummy.iImage = -3;

            VSTASKCOLUMN[] column = new VSTASKCOLUMN[] { dummy };

            int hr;

            int count;
            hr = accessor.GetColumnCount(out count);
            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnCount returned wrong hresult.");
            Assert.AreEqual(Enum.GetValues(typeof(TaskFields)).Length, count, "GetColumnCount returned wrong count.");

            for (int i = 0; i < expectedColumns.Length; ++i)
            {
                hr = accessor.GetColumn(i, column);
                Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnCount returned wrong hresult for " + expectedColumns[i].bstrCanonicalName + ".");
                Assert.AreEqual(expectedColumns[i].bstrCanonicalName, column[0].bstrCanonicalName, "bstrCanonicalName was wrong for " + expectedColumns[i].bstrCanonicalName + ".");
                Assert.AreEqual(expectedColumns[i].bstrHeading, column[0].bstrHeading, "bstrHeading was wrong for " + expectedColumns[i].bstrCanonicalName + ".");
                Assert.AreEqual(expectedColumns[i].bstrLocalizedName, column[0].bstrLocalizedName, "bstrLocalizedName was wrong for " + expectedColumns[i].bstrCanonicalName + ".");
                Assert.AreEqual(expectedColumns[i].bstrTip, column[0].bstrTip, "bstrTip was wrong for " + expectedColumns[i].bstrCanonicalName + ".");
                Assert.AreEqual(expectedColumns[i].cxDefaultWidth, column[0].cxDefaultWidth, "cxDefaultWidth was wrong for " + expectedColumns[i].bstrCanonicalName + ".");
                Assert.AreEqual(expectedColumns[i].cxMinWidth, column[0].cxMinWidth, "cxMinWidth was wrong for " + expectedColumns[i].bstrCanonicalName + ".");
                Assert.AreEqual(expectedColumns[i].fAllowHide, column[0].fAllowHide, "fAllowHide was wrong for " + expectedColumns[i].bstrCanonicalName + ".");
                Assert.AreEqual(expectedColumns[i].fAllowUserSort, column[0].fAllowUserSort, "fAllowUserSort was wrong for " + expectedColumns[i].bstrCanonicalName + ".");
                Assert.AreEqual(expectedColumns[i].fDescendingSort, column[0].fDescendingSort, "fDescendingSort was wrong for " + expectedColumns[i].bstrCanonicalName + ".");
                Assert.AreEqual(expectedColumns[i].fDynamicSize, column[0].fDynamicSize, "fDynamicSize was wrong for " + expectedColumns[i].bstrCanonicalName + ".");
                Assert.AreEqual(expectedColumns[i].fFitContent, column[0].fFitContent, "fFitContent was wrong for " + expectedColumns[i].bstrCanonicalName + ".");
                Assert.AreEqual(expectedColumns[i].fMoveable, column[0].fMoveable, "fMoveable was wrong for " + expectedColumns[i].bstrCanonicalName + ".");
                Assert.AreEqual(expectedColumns[i].fShowSortArrow, column[0].fShowSortArrow, "fShowSortArrow was wrong for " + expectedColumns[i].bstrCanonicalName + ".");
                Assert.AreEqual(expectedColumns[i].fSizeable, column[0].fSizeable, "fSizeable was wrong for " + expectedColumns[i].bstrCanonicalName + ".");
                Assert.AreEqual(expectedColumns[i].fVisibleByDefault, column[0].fVisibleByDefault, "fVisibleByDefault was wrong for " + expectedColumns[i].bstrCanonicalName + ".");
                Assert.AreEqual(expectedColumns[i].iDefaultSortPriority, column[0].iDefaultSortPriority, "iDefaultSortPriority was wrong for " + expectedColumns[i].bstrCanonicalName + ".");
                Assert.AreEqual(expectedColumns[i].iField, column[0].iField, "iField was wrong for " + expectedColumns[i].bstrCanonicalName + ".");
                Assert.AreEqual(expectedColumns[i].iImage, column[0].iImage, "iImage was wrong for " + expectedColumns[i].bstrCanonicalName + ".");
            }
        }

        /// <summary>
        ///A test case for GetProviderFlags (out uint)
        ///</summary>
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void GetProviderFlagsTest()
        {
            CodeSweep.VSPackage.TaskProvider_Accessor accessor = new CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider);

            uint flags;
            int hr = accessor.GetProviderFlags(out flags);

            Assert.AreEqual(VSConstants.S_OK, hr, "GetProvider flags returned wrong hresult.");
            Assert.AreEqual(__VSTASKPROVIDERFLAGS.TPF_NOAUTOROUTING | __VSTASKPROVIDERFLAGS.TPF_ALWAYSVISIBLE, (__VSTASKPROVIDERFLAGS)flags, "GetProviderFlags returned wrong flags.");
        }

        /// <summary>
        ///A test case for GetProviderGuid (out Guid)
        ///</summary>
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void GetProviderGuidTest()
        {
            CodeSweep.VSPackage.TaskProvider_Accessor accessor1 = new CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider);

            Utilities.RemoveCommandHandlers(_serviceProvider);

            CodeSweep.VSPackage.TaskProvider_Accessor accessor2 = new CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider);

            Guid guid1;
            Guid guid2;
            int hr = accessor1.GetProviderGuid(out guid1);

            Assert.AreEqual(VSConstants.S_OK, hr, "GetProviderGuid returned wrong hresult.");
            Assert.AreNotEqual(Guid.Empty, guid1, "GetProviderGuid returned null guid.");

            hr = accessor2.GetProviderGuid(out guid2);

            Assert.AreEqual(VSConstants.S_OK, hr, "GetProviderGuid returned wrong hresult (second instance).");
            Assert.AreEqual(guid1, guid2, "GetProviderGuid did not return the same guid for two different instances.");
        }

        /// <summary>
        ///A test case for GetProviderName (out string)
        ///</summary>
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void GetProviderNameTest()
        {
            CodeSweep.VSPackage.TaskProvider_Accessor accessor = new CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider);

            string name;
            int hr = accessor.GetProviderName(out name);

            Assert.AreEqual(VSConstants.S_OK, hr, "GetProviderName returned wrong hresult.");
            Assert.AreEqual("CodeSweep", name, "GetProviderName returned wrong name.");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void GetProviderToolbarTest()
        {
            CodeSweep.VSPackage.TaskProvider_Accessor accessor = new CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider);

            uint id = 0;
            Guid group = Guid.Empty;
            int hr = accessor.GetProviderToolbar(out group, out id);

            Assert.AreEqual(VSConstants.S_OK, hr, "GetProviderToolbar returned wrong hresult.");
            Assert.AreEqual(CodeSweep.VSPackage.GuidList_Accessor.guidVSPackageCmdSet, group, "GetProviderToolbar returned wrong group.");
            Assert.AreEqual((uint)0x2020, id, "GetProviderToolbar returned wrong id.");
        }

        [DllImport("comctl32.dll")]
        private static extern int ImageList_GetImageCount(IntPtr himl);

        [DllImport("comctl32.dll")]
        static extern void ImageList_Destroy(IntPtr handle);

        /// <summary>
        ///A test case for ImageList (out IntPtr)
        ///</summary>
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void ImageListTest()
        {
            CodeSweep.VSPackage.TaskProvider_Accessor accessor = new CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider);

            IntPtr handle = IntPtr.Zero;

            try
            {
                int hr = accessor.ImageList(out handle);

                Assert.AreEqual(VSConstants.S_OK, hr, "ImageList returned wrong hresult.");
                Assert.AreNotEqual(IntPtr.Zero, handle, "ImageList returned null image list.");
                Assert.AreEqual(3, ImageList_GetImageCount(handle), "ImageList returned wrong number of images.");

                IntPtr handle2 = IntPtr.Zero;

                try
                {
                    accessor.ImageList(out handle);

                    Assert.AreNotEqual(handle, handle2, "ImageList did not return a new list handle each time.");
                }
                finally
                {
                    if (handle2 != IntPtr.Zero)
                    {
                        ImageList_Destroy(handle2);
                    }
                }
            }
            finally
            {
                if (handle != IntPtr.Zero)
                {
                    ImageList_Destroy(handle);
                }
            }
        }

        /// <summary>
        ///A test case for ReRegistrationKey (out string)
        ///</summary>
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void ReRegistrationKeyTest()
        {
            CodeSweep.VSPackage.TaskProvider_Accessor accessor = new CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider);

            string key;
            int hr = accessor.ReRegistrationKey(out key);
            Assert.AreEqual(VSConstants.E_NOTIMPL, hr, "ReRegistrationKey returned wrong hresult.");
        }

        /// <summary>
        ///A test case for GetSurrogateProviderGuid (out Guid)
        ///</summary>
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void GetSurrogateProviderGuidTest()
        {
            CodeSweep.VSPackage.TaskProvider_Accessor accessor = new CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider);

            Guid surrogate;
            int hr = accessor.GetSurrogateProviderGuid(out surrogate);

            Assert.AreEqual(VSConstants.E_NOTIMPL, hr, "GetSurrogateProviderGuid returned wrong hresult.");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void TestRegistrationAndUnregistration()
        {
            List<uint> registered = new List<uint>();
            List<uint> unregistered = new List<uint>();

            MockTaskList taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as MockTaskList;
            taskList.OnRegisterTaskProvider +=
                delegate(object sender, MockTaskList.RegisterTaskProviderArgs args)
                {
                    registered.Add(args.Cookie);
                };
            taskList.OnUnregisterTaskProvider +=
                delegate(object sender, MockTaskList.UnregisterTaskProviderArgs args)
                {
                    unregistered.Add(args.Cookie);
                };

            CodeSweep.VSPackage.TaskProvider_Accessor accessor = new CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider);

            Assert.AreEqual(1, registered.Count, "Task provider did not register itself when created.");

            accessor.OnTaskListFinalRelease(taskList);

            Assert.AreEqual(1, unregistered.Count, "Task provider did not unregister itself from OnTaskListFinalRelease.");
            Assert.AreEqual(registered[0], unregistered[0], "Cookies did not match.");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void TestIgnore()
        {
            CodeSweep.VSPackage.TaskProvider_Accessor accessor = new CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider);
            
            Project project = Utilities.SetupMSBuildProject();
            Utilities.RegisterProjectWithMocks(project, _serviceProvider);

            MockTermTable table = new MockTermTable("termtable.xml");
            MockTerm term0 = new MockTerm("dupText", 0, "term0Class", "term0Comment", "term0recommended", table);
            MockTerm term1 = new MockTerm("term2Text", 2, "term2Class", "term2Comment", "term2recommended", table);
            MockScanHit hit0 = new MockScanHit("file0", 1, 5, "line text", term0, null);
            MockScanHit hit1 = new MockScanHit("file1", 4, 1, "line text 2", term1, null);
            MockScanHit hit2 = new MockScanHit("file2", 3, 2, "line text 3", term1, null);
            MockScanResult scanResult = new MockScanResult("file0", new IScanHit[] { hit0, hit1, hit2 }, true);
            accessor.AddResult(scanResult, project.FullPath);

            IVsEnumTaskItems enumerator = null;
            accessor.EnumTaskItems(out enumerator);
            List<IVsTaskItem> items = Utilities.TasksFromEnumerator(enumerator);
            CodeSweep.VSPackage.Task_Accessor task0Accessor = new CodeSweep.VSPackage.Task_Accessor(new PrivateObject(items[0]));
            CodeSweep.VSPackage.Task_Accessor task1Accessor = new CodeSweep.VSPackage.Task_Accessor(new PrivateObject(items[1]));

            MockTaskList taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as MockTaskList;

            // Ensure cmd is disabled with no selection
            OleMenuCommandService mcs = _serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            MenuCommand command = mcs.FindCommand(new CommandID(CodeSweep.VSPackage.GuidList_Accessor.guidVSPackageCmdSet, (int)CodeSweep.VSPackage.PkgCmdIDList_Accessor.cmdidIgnore));

            // NOTE: simply getting command.Supported or command.Enabled doesn't seem to invoke
            // QueryStatus, so I'll explicitly call the status update method as a workaround.
            accessor.QueryIgnore(null, EventArgs.Empty);

            Assert.IsTrue(command.Supported, "Command not supported.");
            Assert.IsFalse(command.Enabled, "Command enabled with no selection.");

            // Ensure cmd is disabled with an ignored item selected
            task0Accessor.Ignored = true;
            taskList.SetSelected(items[0], true);
            accessor.QueryIgnore(null, EventArgs.Empty);
            Assert.IsFalse(command.Enabled, "Command enabled with ignored item selected.");

            // Ensure cmd is enabled with one ignored and one non-ignored item selected
            taskList.SetSelected(items[1], true);
            accessor.QueryIgnore(null, EventArgs.Empty);
            Assert.IsTrue(command.Enabled, "Command disabled with a non-ignored item selected.");

            // Fire cmd, ensure selected items are ignored
            command.Invoke();
            accessor.QueryIgnore(null, EventArgs.Empty);
            Assert.IsTrue(task0Accessor.Ignored, "Command set ignored task to non-ignored.");
            Assert.IsTrue(task1Accessor.Ignored, "Command did not set non-ignored task to ignored.");

            // Ensure cmd is now disabled
            accessor.QueryIgnore(null, EventArgs.Empty);
            Assert.IsFalse(command.Enabled, "Command still enabled after invocation.");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void TestDontIgnore()
        {
            CodeSweep.VSPackage.TaskProvider_Accessor accessor = new CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider);

            Project project = Utilities.SetupMSBuildProject();
            Utilities.RegisterProjectWithMocks(project, _serviceProvider);

            MockTermTable table = new MockTermTable("termtable.xml");
            MockTerm term0 = new MockTerm("dupText", 0, "term0Class", "term0Comment", "term0recommended", table);
            MockTerm term1 = new MockTerm("term2Text", 2, "term2Class", "term2Comment", "term2recommended", table);
            MockScanHit hit0 = new MockScanHit("file0", 1, 5, "line text", term0, null);
            MockScanHit hit1 = new MockScanHit("file1", 4, 1, "line text 2", term1, null);
            MockScanHit hit2 = new MockScanHit("file2", 3, 2, "line text 3", term1, null);
            MockScanResult scanResult = new MockScanResult("file0", new IScanHit[] { hit0, hit1, hit2 }, true);
            accessor.AddResult(scanResult, project.FullPath);

            IVsEnumTaskItems enumerator = null;
            accessor.EnumTaskItems(out enumerator);
            List<IVsTaskItem> items = Utilities.TasksFromEnumerator(enumerator);
            CodeSweep.VSPackage.Task_Accessor task0Accessor = new CodeSweep.VSPackage.Task_Accessor(new PrivateObject(items[0]));
            CodeSweep.VSPackage.Task_Accessor task1Accessor = new CodeSweep.VSPackage.Task_Accessor(new PrivateObject(items[1]));

            MockTaskList taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as MockTaskList;

            // Ensure cmd is disabled with no selection
            OleMenuCommandService mcs = _serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            MenuCommand command = mcs.FindCommand(new CommandID(CodeSweep.VSPackage.GuidList_Accessor.guidVSPackageCmdSet, (int)CodeSweep.VSPackage.PkgCmdIDList_Accessor.cmdidDoNotIgnore));


            // NOTE: simply getting command.Supported or command.Enabled doesn't seem to invoke
            // QueryStatus, so I'll explicitly call the status update method as a workaround.
            accessor.QueryDontIgnore(null, EventArgs.Empty);

            Assert.IsTrue(command.Supported, "Command not supported.");
            Assert.IsFalse(command.Enabled, "Command enabled with no selection.");

            // Ensure cmd is enabled with an ignored item selected
            task0Accessor.Ignored = true;
            taskList.SetSelected(items[0], true);
            accessor.QueryDontIgnore(null, EventArgs.Empty);
            Assert.IsTrue(command.Enabled, "Command disabled with ignored item selected.");

            // Ensure cmd is enabled with one ignored and one non-ignored item selected
            taskList.SetSelected(items[1], true);
            accessor.QueryDontIgnore(null, EventArgs.Empty);
            Assert.IsTrue(command.Enabled, "Command disabled with a non-ignored item selected.");

            // Fire cmd, ensure selected items are not ignored
            command.Invoke();
            accessor.QueryDontIgnore(null, EventArgs.Empty);
            Assert.IsFalse(task0Accessor.Ignored, "Command did not set ignored task to non-ignored.");
            Assert.IsFalse(task1Accessor.Ignored, "Command set non-ignored task to ignored.");

            // Ensure cmd is now disabled
            accessor.QueryDontIgnore(null, EventArgs.Empty);
            Assert.IsFalse(command.Enabled, "Command still enabled after invocation.");
        }

        // TODO: ensure the Stop Scan command is enabled only while a scan is in progress (this is handled in VsPkg.cs, so it should be in the tests for that class).
        // TODO: ensure the Repeat Last Scan command is disabled while a scan is in progress or if no scan has been done (this is handled in VsPkg.cs, so it should be in the tests for that class).
        // TODO: ensure Show Ignore Instances acts as a toggle (becomes ninched when you press it) and has the desired effect.
        // TODO: ensure that the tasks for a project are removed when the project is unloaded, and that the tasks for other projects remain.

    }


}
