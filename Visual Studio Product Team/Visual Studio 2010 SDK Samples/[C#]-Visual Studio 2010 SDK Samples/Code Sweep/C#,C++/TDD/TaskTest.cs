/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    /// <summary>
    ///This is a test class for CodeSweep.VSPackage.Task and is intended
    ///to contain all CodeSweep.VSPackage.Task Unit Tests
    ///</summary>
    [TestClass()]
    public class TaskTest
    {

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

            MockTaskList taskList = _serviceProvider.GetService(typeof(SVsTaskList)) as MockTaskList;
            taskList.Clear();

            CodeSweep.VSPackage.Factory_Accessor._taskProvider = null;
        }
        //
        #endregion



        /// <summary>
        ///A test case for CanDelete (out int)
        ///</summary>
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void CanDeleteTest()
        {
            CodeSweep.VSPackage.Task_Accessor accessor = new CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", "z:\\dir\\file.ext", 1, 1, "projFile", "full line text", null, null);

            int canDelete;

            int hr = accessor.CanDelete(out canDelete);

            Assert.AreEqual(VSConstants.S_OK, hr, "CanDelete had unexpected return code.");
            Assert.AreEqual(0, canDelete, "CanDelete indicated deletion was possible.");
        }

        /// <summary>
        ///A test case for GetColumnValue (int, out uint, out uint, out object, out string)
        ///</summary>
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void GetColumnValueTest()
        {
            Microsoft.Build.Evaluation.Project msbuildProj = Utilities.SetupMSBuildProject();

            // Set up a project so the project column can be populated.
            MockSolution solution = _serviceProvider.GetService(typeof(SVsSolution)) as MockSolution;
            MockIVsProject project = new MockIVsProject(msbuildProj.FullPath);
            solution.AddProject(project);

            CodeSweep.VSPackage.TaskProvider_Accessor providerAccessor = new CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider);

            CodeSweep.VSPackage.Task_Accessor accessor = new CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment with link:  http://www.microsoft.com", "replacement", "z:\\dir\\file.ext", 2, 3, msbuildProj.FullPath, "full line text", providerAccessor, null);

            uint type;
            uint flags;
            object val;
            string accName;

            int hr = accessor.GetColumnValue((int)TaskFields.Priority, out type, out flags, out val, out accName);

            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnValue did not return S_OK for the Priority column.");
            Assert.AreEqual(__VSTASKVALUETYPE.TVT_IMAGE, (__VSTASKVALUETYPE)type, "Type of Priority column is incorrect.");
            Assert.AreEqual(__VSTASKVALUEFLAGS.TVF_HORZ_CENTER, (__VSTASKVALUEFLAGS)flags, "Flags for Priority column are incorrect.");
            Assert.AreEqual(typeof(int), val.GetType(), "Value of Priority column has wrong type.");
            Assert.IsTrue((int)val >= 0 && (int)val <= 2, "Image index for Priority column is out of range.");
            string[] imageAccText = new string[] { "High", "Medium", "Low" };
            Assert.AreEqual(imageAccText[(int)val], accName, "Accessibility text of Priority column is incorrect.");

            hr = accessor.GetColumnValue((int)TaskFields.PriorityNumber, out type, out flags, out val, out accName);

            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnValue did not return S_OK for the PriorityNumber column.");
            Assert.AreEqual(__VSTASKVALUETYPE.TVT_BASE10, (__VSTASKVALUETYPE)type, "Type of PriorityNumber column is incorrect.");
            Assert.AreEqual(0, (int)flags, "Flags for PriorityNumber column are incorrect.");
            Assert.AreEqual(typeof(int), val.GetType(), "Value of PriorityNumber column has wrong type.");
            Assert.AreEqual(1, (int)val, "Value of PriorityNumber column is incorrect.");
            Assert.AreEqual("", accName, "Accessibility text of PriorityNumber column is incorrect.");

            hr = accessor.GetColumnValue((int)TaskFields.Term, out type, out flags, out val, out accName);

            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnValue did not return S_OK for the Term column.");
            Assert.AreEqual(__VSTASKVALUETYPE.TVT_TEXT, (__VSTASKVALUETYPE)type, "Type of Term column is incorrect.");
            Assert.AreEqual(0, (int)flags, "Flags for Term column are incorrect.");
            Assert.AreEqual(typeof(string), val.GetType(), "Value of Term column has wrong type.");
            Assert.AreEqual("term", (string)val, "Value of Term column is incorrect.");
            Assert.AreEqual("", accName, "Accessibility text of Term column is incorrect.");

            hr = accessor.GetColumnValue((int)TaskFields.Class, out type, out flags, out val, out accName);

            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnValue did not return S_OK for the Class column.");
            Assert.AreEqual(__VSTASKVALUETYPE.TVT_TEXT, (__VSTASKVALUETYPE)type, "Type of Class column is incorrect.");
            Assert.AreEqual(0, (int)flags, "Flags for Class column are incorrect.");
            Assert.AreEqual(typeof(string), val.GetType(), "Value of Class column has wrong type.");
            Assert.AreEqual("class", (string)val, "Value of Class column is incorrect.");
            Assert.AreEqual("", accName, "Accessibility text of Class column is incorrect.");

            hr = accessor.GetColumnValue((int)TaskFields.Replacement, out type, out flags, out val, out accName);

            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnValue did not return S_OK for the Replacement column.");
            Assert.AreEqual(__VSTASKVALUETYPE.TVT_TEXT, (__VSTASKVALUETYPE)type, "Type of Replacement column is incorrect.");
            Assert.AreEqual(0, (int)flags, "Flags for Replacement column are incorrect.");
            Assert.AreEqual(typeof(string), val.GetType(), "Value of Replacement column has wrong type.");
            Assert.AreEqual("replacement", (string)val, "Value of Replacement column is incorrect.");
            Assert.AreEqual("", accName, "Accessibility text of Replacement column is incorrect.");

            hr = accessor.GetColumnValue((int)TaskFields.Comment, out type, out flags, out val, out accName);

            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnValue did not return S_OK for the Comment column.");
            Assert.AreEqual(__VSTASKVALUETYPE.TVT_LINKTEXT, (__VSTASKVALUETYPE)type, "Type of Comment column is incorrect.");
            Assert.AreEqual(0, (int)flags, "Flags for Comment column are incorrect.");
            Assert.AreEqual(typeof(string), val.GetType(), "Value of Comment column has wrong type.");
            Assert.AreEqual("comment with link:  @http://www.microsoft.com@", (string)val, "Value of Comment column is incorrect.");
            Assert.AreEqual("", accName, "Accessibility text of Comment column is incorrect.");

            hr = accessor.GetColumnValue((int)TaskFields.File, out type, out flags, out val, out accName);

            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnValue did not return S_OK for the File column.");
            Assert.AreEqual(__VSTASKVALUETYPE.TVT_TEXT, (__VSTASKVALUETYPE)type, "Type of File column is incorrect.");
            Assert.AreEqual(__VSTASKVALUEFLAGS.TVF_FILENAME, (__VSTASKVALUEFLAGS)flags, "Flags for File column are incorrect.");
            Assert.AreEqual(typeof(string), val.GetType(), "Value of File column has wrong type.");
            Assert.AreEqual("z:\\dir\\file.ext", (string)val, "Value of File column is incorrect.");
            Assert.AreEqual("", accName, "Accessibility text of File column is incorrect.");

            hr = accessor.GetColumnValue((int)TaskFields.Line, out type, out flags, out val, out accName);

            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnValue did not return S_OK for the Line column.");
            Assert.AreEqual(__VSTASKVALUETYPE.TVT_BASE10, (__VSTASKVALUETYPE)type, "Type of Line column is incorrect.");
            Assert.AreEqual(0, (int)flags, "Flags for Line column are incorrect.");
            Assert.AreEqual(typeof(int), val.GetType(), "Value of Line column has wrong type.");
            Assert.AreEqual(3, (int)val, "Value of Line column is incorrect.");
            Assert.AreEqual("", accName, "Accessibility text of Line column is incorrect.");

            hr = accessor.GetColumnValue((int)TaskFields.Project, out type, out flags, out val, out accName);

            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnValue did not return S_OK for the Project column.");
            Assert.AreEqual(__VSTASKVALUETYPE.TVT_TEXT, (__VSTASKVALUETYPE)type, "Type of Project column is incorrect.");
            Assert.AreEqual(0, (int)flags, "Flags for Project column are incorrect.");
            Assert.AreEqual(typeof(string), val.GetType(), "Value of Project column has wrong type.");
            string uniqueName;
            solution.GetUniqueUINameOfProject(project, out uniqueName);
            Assert.AreEqual(uniqueName, (string)val, "Value of Project column is incorrect.");
            Assert.AreEqual("", accName, "Accessibility text of Project column is incorrect.");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void UnderlinedIfIgnored()
        {
            Microsoft.Build.Evaluation.Project msbuildProj = Utilities.SetupMSBuildProject();

            // Set up a project so the project column can be populated.
            MockSolution solution = _serviceProvider.GetService(typeof(SVsSolution)) as MockSolution;
            MockIVsProject project = new MockIVsProject(msbuildProj.FullPath);
            solution.AddProject(project);

            CodeSweep.VSPackage.TaskProvider_Accessor providerAccessor = new CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider);

            string projectDrive = Path.GetPathRoot(msbuildProj.FullPath);

            CodeSweep.VSPackage.Task_Accessor accessor = new CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", projectDrive + "\\dir\\file.ext", 2, 3, msbuildProj.FullPath, "full line text", providerAccessor, null);

            accessor.Ignored = true;

            uint type;
            uint flags;
            object val;
            string accName;

            foreach (object fieldObj in Enum.GetValues(typeof(TaskFields)))
            {
                accessor.GetColumnValue((int)fieldObj, out type, out flags, out val, out accName);
                Assert.IsTrue((flags & (int)__VSTASKVALUEFLAGS.TVF_STRIKETHROUGH) != 0, "Strikethrough flag not set for ignored task field " + (int)fieldObj);
            }
        }

        /// <summary>
        ///A test case for GetNavigationStatusText (out string)
        ///</summary>
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void GetNavigationStatusTextTest()
        {
            CodeSweep.VSPackage.Task_Accessor accessor = new CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", "z:\\dir\\file.ext", 2, 3, "projFile", "full line text", null, null);

            string text;
            int hr = accessor.GetNavigationStatusText(out text);

            Assert.AreEqual(VSConstants.S_OK, hr, "GetNavigationStatusText did not return S_OK.");
            Assert.AreEqual("comment", text, "GetNavigationStatusText returned wrong name.");
        }

        /// <summary>
        ///A test case for GetTaskName (out string)
        ///</summary>
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void GetTaskNameTest()
        {
            CodeSweep.VSPackage.Task_Accessor accessor = new CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", "z:\\dir\\file.ext", 2, 3, "projFile", "full line text", null, null);

            string name;
            int hr = accessor.GetTaskName(out name);

            Assert.AreEqual(VSConstants.S_OK, hr, "GetTaskName did not return S_OK.");
            Assert.AreEqual("term", name, "GetTaskName returned wrong name.");
        }

        /// <summary>
        ///A test case for GetTaskProvider (out IVsTaskProvider3)
        ///</summary>
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void GetTaskProviderTest()
        {
            CodeSweep.VSPackage.TaskProvider_Accessor providerAccessor = new CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider);

            CodeSweep.VSPackage.Task_Accessor accessor = new CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", "z:\\dir\\file.ext", 2, 3, "projFile", "full line text", providerAccessor, null);

            IVsTaskProvider3 provider;
            int hr = accessor.GetTaskProvider(out provider);

            Assert.AreEqual(VSConstants.S_OK, hr, "GetTaskProvider returned wrong hresult.");
            Assert.AreEqual(providerAccessor, provider, "GetTaskProvider returned wrong provider.");
        }

        /// <summary>
        ///A test case for GetTipText (int, out string)
        ///</summary>
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void GetTipTextTest()
        {
            CodeSweep.VSPackage.Task_Accessor accessor = new CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", "z:\\dir\\file.ext", 2, 3, "projFile", "full line text", null, null);

            foreach (object fieldObj in System.Enum.GetValues(typeof(TaskFields)))
            {
                string tip;
                int hr = accessor.GetTipText((int)fieldObj, out tip);

                Assert.AreEqual(VSConstants.E_NOTIMPL, hr, "GetTipText returned wrong hresult for field " + ((TaskFields)fieldObj).ToString());
            }
        }

        /// <summary>
        ///A test case for HasHelp (out int)
        ///</summary>
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void HasHelpTest()
        {
            CodeSweep.VSPackage.Task_Accessor accessor = new CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", "z:\\dir\\file.ext", 2, 3, "projFile", "full line text", null, null);

            int hasHelp;
            int hr = accessor.HasHelp(out hasHelp);

            Assert.AreEqual(VSConstants.S_OK, hr, "HasHelp returned wrong hresult.");
            Assert.AreEqual(0, hasHelp, "HasHelp returned wrong value.");
        }

        /// <summary>
        ///A test case for IsDirty (out int)
        ///</summary>
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void IsDirtyTest()
        {

            CodeSweep.VSPackage.Task_Accessor accessor = new CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", "z:\\dir\\file.ext", 2, 3, "projFile", "full line text", null, null);

            int isDirty;
            int hr = accessor.IsDirty(out isDirty);

            Assert.AreEqual(VSConstants.E_NOTIMPL, hr, "IsDirty returned wrong hresult.");
            Assert.AreEqual(0, isDirty, "IsDirty returned wrong value.");
        }

        /// <summary>
        ///A test case for IsReadOnly (VSTASKFIELD, out int)
        ///</summary>
        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void IsReadOnlyTest()
        {
            CodeSweep.VSPackage.Task_Accessor accessor = new CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", "z:\\dir\\file.ext", 2, 3, "projFile", "full line text", null, null);

            foreach (object fieldObj in System.Enum.GetValues(typeof(TaskFields)))
            {
                int readOnly;
                int hr = accessor.IsReadOnly((VSTASKFIELD)fieldObj, out readOnly);

                Assert.AreEqual(1, readOnly, "IsReadOnly returned wrong hresult for field " + ((TaskFields)fieldObj).ToString());
            }
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void Navigate()
        {
            string fileName = "z:\\dir\\file.ext";

            CodeSweep.VSPackage.Task_Accessor accessor = new CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", fileName, 2, 3, "projFile", "full line text", null, _serviceProvider);

            MockUIShellOpenDocument openDoc = _serviceProvider.GetService(typeof(SVsUIShellOpenDocument)) as MockUIShellOpenDocument;
            openDoc.AddDocument(fileName);

            MockTextManager textMgr = _serviceProvider.GetService(typeof(SVsTextManager)) as MockTextManager;
            MockTextView view = textMgr.AddView(fileName);

            int line = -1;
            int col = -1;
            view.OnSetCaretPos +=
                delegate(object sender, MockTextView.SetCaretPosEventArgs args)
                {
                    line = args.Line;
                    col = args.Column;
                };

            int hr = accessor.NavigateTo();

            Assert.AreEqual(VSConstants.S_OK, hr, "NavigateTo returned wrong hresult.");
            Assert.AreEqual(2, line, "NavigateTo did not navigate to correct line.");
            Assert.AreEqual(3, col, "NavigateTo did not navigate to correct column.");
        }

        [DeploymentItem("VsPackage.dll")]
        [TestMethod()]
        public void OnLinkClicked()
        {

            CodeSweep.VSPackage.Task_Accessor accessor = new CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment: http://www.microsoft.com; some more text; http://msdn.microsoft.com.", "replacement", "z:\\dir\\file.ext", 2, 3, "projFile", "full line text", null, _serviceProvider);

            string url = null;

            MockWebBrowsingService browser = _serviceProvider.GetService(typeof(SVsWebBrowsingService)) as MockWebBrowsingService;
            browser.OnNavigate +=
                delegate(object sender, MockWebBrowsingService.NavigateEventArgs args)
                {
                    url = args.Url;
                };

            int hr = accessor.OnLinkClicked((int)TaskFields.Comment, 0);

            Assert.AreEqual(VSConstants.S_OK, hr, "OnLinkClicked returned wrong hresult for link 0.");
            Assert.AreEqual("http://www.microsoft.com", url, "OnLinkClicked sent wrong url for link 0.");

            hr = accessor.OnLinkClicked((int)TaskFields.Comment, 1);

            Assert.AreEqual(VSConstants.S_OK, hr, "OnLinkClicked returned wrong hresult for link 1.");
            Assert.AreEqual("http://msdn.microsoft.com", url, "OnLinkClicked sent wrong url for link 1.");
        }

    }


}
