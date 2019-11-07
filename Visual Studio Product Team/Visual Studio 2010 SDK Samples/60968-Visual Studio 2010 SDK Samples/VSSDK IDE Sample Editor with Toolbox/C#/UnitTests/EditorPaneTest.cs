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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VsSDK.UnitTestLibrary;
using System.Windows.Forms;
using OleInterop = Microsoft.VisualStudio.OLE.Interop;

using Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.OLE.Interop;

namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests
{
    /// <summary>
    ///This is a test class for Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.EditorPane and is intended
    ///to contain all Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.EditorPane Unit Tests
    ///</summary>
    [TestClass()]
    public class EditorPaneTest : IDisposable
    {
        #region Fields
        private EditorPane editorPane;
        Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor editorPaneAccessor;

        private string filePath = string.Empty;
        private string testString = string.Empty;
        OleServiceProvider serviceProvider;
        private TestContext testContextInstance;

        #endregion

        #region Properties

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
        #endregion

        #region Initialization && Cleanup
        /// <summary>
        /// Runs before the test to allocate and configure resources needed 
        /// by all tests in the test class.
        /// </summary>
        [TestInitialize()]
        public void EditorPaneTestInitialize()
        {
            // initialize base test context
            testString = "This is a temporary file";

            filePath = AppDomain.CurrentDomain.BaseDirectory +
                        Path.DirectorySeparatorChar + "TempFile.tbx";

            CreateTempFile();

            // prepare service provider 
            serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
            AddBasicSiteSupport(serviceProvider);

            // prepare editorPane
            editorPane = new EditorPane();
            Assert.AreEqual<int>(VSConstants.S_OK, ((IVsWindowPane)editorPane).SetSite(serviceProvider));
            editorPaneAccessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(editorPane);

            editorPaneAccessor.fileName = filePath;
        }
        /// <summary>
        /// Runs after the test has run and to free resources obtained 
        /// by all the tests in the test class.
        /// </summary>
        [TestCleanup()]
        public void EditorPaneTestCleanup()
        {
            DropTempFile();
            serviceProvider = null;
        }
        #endregion

        #region IDisposable Pattern implementation
        /// <summary>
        /// Implement IDisposable interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (editorPane != null)
                {
                    editorPane.Dispose();
                    editorPane = null;
                }
                if (serviceProvider != null)
                {
                    serviceProvider.Dispose();
                    serviceProvider = null;
                }
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        #region Test methods
        #region Constructors && Initializers tests
        /// <summary>
        ///A test for EditorPane (EditorPackage) method.
        ///</summary>
        [TestMethod()]
        public void ConstructorTestWithParameters()
        {
            // prepare service provider 
            serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
            AddBasicSiteSupport(serviceProvider);

            EditorPane target = new EditorPane();
            Assert.IsNotNull(target, "EditorPane object was not created");

            Assert.AreEqual<int>(VSConstants.S_OK, ((IVsWindowPane)target).SetSite(serviceProvider));
        }

        #endregion Constructors && Initializers tests

        #region IDisposable tests
        /// <summary>
        ///The test for IDisposable interface implementation.
        ///</summary>
        [TestMethod()]
        public void CheckIDisposableImplementationTest()
        {
            using (EditorPane target = editorPane)
            {
                Assert.IsNotNull(target as IDisposable, "The object does not implement IDisposable");
            }
        }

        /// <summary>
        ///The test for Dispose() method.
        ///</summary>
        [TestMethod()]
        public void DisposeTest()
        {
            EditorPane target = editorPane;

            target.Dispose();
            Assert.IsNull(editorPaneAccessor.editorControl, "Dispose() call for the EditorPane was not free editorControl object.");
        }
        #endregion

        #region IOleCommandTarget tests
        /// <summary>
        ///The common test for QueryStatus method.
        ///</summary>
        [TestMethod()]
        public void QueryStatusStandardTest()
        {
            EditorPane target = editorPane;
            Guid guidCmdGroup = VSConstants.GUID_VSStandardCommandSet97;
            OleInterop.OLECMD oleCmd = new OleInterop.OLECMD();

            oleCmd.cmdf = 0;
            oleCmd.cmdID = (uint)Microsoft.VisualStudio.VSConstants.VSStd97CmdID.SelectAll;

            OleInterop.OLECMD[] prgCmds = new OleInterop.OLECMD[] { oleCmd };
            IntPtr ptr = new IntPtr();

            int res = target.QueryStatus(ref guidCmdGroup, (uint)1, prgCmds, ptr);
            Assert.AreEqual(VSConstants.S_OK, res, "The method has not return expected value");
        }

        /// <summary>
        ///The test for QueryStatus method. The SyncDesigner test scenario.
        ///</summary>
        [TestMethod()]
        public void QueryStatusSyncDesignerTest()
        {
            EditorPane target = editorPane;
            Guid guidCmdGroup = VSConstants.GUID_VsTaskListViewHTMLTasks;
            OleInterop.OLECMD oleCmd = new OleInterop.OLECMD();
            oleCmd.cmdf = 0;
            oleCmd.cmdID = 265;
            OleInterop.OLECMD[] prgCmds = new OleInterop.OLECMD[] { oleCmd };
            IntPtr ptr = new IntPtr();

            int res = target.QueryStatus(ref guidCmdGroup, (uint)1, prgCmds, ptr);
            Assert.AreEqual((int)OleInterop.Constants.OLECMDERR_E_NOTSUPPORTED, res, "The QueryStatus method has not return expected value");
        }

        /// <summary>
        ///The test for QueryStatus method.
        ///</summary>
        [TestMethod()]
        public void QueryStatusOtherTest()
        {
            EditorPane target = editorPane;
            Guid guidCmdGroup = VSConstants.GUID_VSStandardCommandSet97;
            OleInterop.OLECMD oleCmd = new OleInterop.OLECMD();
            oleCmd.cmdf = 0;
            oleCmd.cmdID = (uint)Microsoft.VisualStudio.VSConstants.VSStd97CmdID.SelectAll;
            OleInterop.OLECMD[] prgCmds = new OleInterop.OLECMD[] { oleCmd };
            IntPtr ptr = new IntPtr();

            int actual_result = target.QueryStatus(ref guidCmdGroup, (uint)1, prgCmds, ptr);
            Assert.AreEqual(VSConstants.S_OK, actual_result);
            Assert.IsNotNull(prgCmds[0], "Wrong object reference stored in prgCmds[0]");
            Assert.IsTrue((prgCmds[0].cmdf & (uint)OleInterop.OLECMDF.OLECMDF_ENABLED) != 0, "The QueryStatus method has not return expected value");
        }

        /// <summary>
        /// The test for QueryStatus method. The scenario of null value of commands set.
        /// </summary>
        [TestMethod()]
        public void QueryStatusTestNullValueCommandsTest()
        {
            EditorPane target = editorPane;
            Guid guidCmdGroup = VSConstants.GUID_VSStandardCommandSet97;
            OleInterop.OLECMD oleCmd = new OleInterop.OLECMD();
            oleCmd.cmdf = 0;
            oleCmd.cmdID = (uint)VSConstants.VSStd97CmdID.Copy; // any command
            IntPtr ptr = new IntPtr();
            // pass null-referenced argument
            OleInterop.OLECMD[] prgCmds = null;

            int res = target.QueryStatus(ref guidCmdGroup, (uint)1, prgCmds, ptr);
            Assert.AreEqual(res, VSConstants.E_INVALIDARG, "The QueryStatus method has not return expected value");
        }
        /// <summary>
        /// The test for QueryStatus. The scenario of processing of not supported commands.
        /// </summary>
        [TestMethod()]
        public void QueryStatusTestNotSupportedCommandTest()
        {
            EditorPane target = editorPane;
            Guid guidCmdGroup = VSConstants.GUID_VSStandardCommandSet97;
            OleInterop.OLECMD oleCmd = new OleInterop.OLECMD();
            oleCmd.cmdf = 0;

            OleInterop.OLECMD[] prgCmds = new OleInterop.OLECMD[] { oleCmd };
            IntPtr ptr = new IntPtr();

            // Pass not supported command ID value
            oleCmd.cmdID = 0;

            int res = target.QueryStatus(ref guidCmdGroup, (uint)1, prgCmds, ptr);
            Assert.AreEqual(res, (int)(OleInterop.Constants.OLECMDERR_E_NOTSUPPORTED), "The QueryStatus method has not return expected value");
        }
        /// <summary>
        /// The test for QueryStatus. The scenario of processing of not supported commands groups.
        /// </summary>
        [TestMethod()]
        public void QueryStatusTestNotSupportedCommandGroupTest()
        {
            EditorPane target = editorPane;
            // test for our local command group (treated as not supported yet)
            Guid guidCmdGroup = Guid.NewGuid();
            OleInterop.OLECMD oleCmd = new OleInterop.OLECMD();
            oleCmd.cmdf = 0;
            oleCmd.cmdID = 0;
            OleInterop.OLECMD[] prgCmds = new OleInterop.OLECMD[] { oleCmd };
            IntPtr ptr = new IntPtr();

            int res = target.QueryStatus(ref guidCmdGroup, (uint)1, prgCmds, ptr);
            Assert.AreEqual((int)(OleInterop.Constants.OLECMDERR_E_NOTSUPPORTED), res, "The QueryStatus method has not return expected value");
        }

        /// <summary>
        /// The test for QueryStatus. Case of guidEditorWithToolboxCmdSet
        /// </summary>
        [TestMethod()]
        public void QueryStatusTestToolboxCmdSetGuidTest()
        {
            EditorPane target = editorPane;
            Guid guidCmdGroup = Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_GuidListAccessor.guidEditorCmdSet;

            OleInterop.OLECMD oleCmd = new OleInterop.OLECMD();
            oleCmd.cmdf = 0;
            oleCmd.cmdID = 0;//uint)VSConstants.VSStd97CmdID.SelectAll;    // test for "SelectAll" command
            OleInterop.OLECMD[] prgCmds = new OleInterop.OLECMD[] { oleCmd };

            IntPtr ptr = new IntPtr();

            int actual_result = target.QueryStatus(ref guidCmdGroup, (uint)1, prgCmds, ptr);
            Assert.AreEqual((int)(OleInterop.Constants.OLECMDERR_E_NOTSUPPORTED), actual_result, "QueryStatus, Select All command testing. Method has not return expected value");
        }

        /// <summary>
        /// The test for QueryStatus. Case of execution of command "SelectAll".
        /// </summary>
        [TestMethod()]
        public void QueryStatusTestSelectAllCommandTest()
        {
            EditorPane target = editorPane;
            Guid guidCmdGroup = VSConstants.GUID_VSStandardCommandSet97;
            OleInterop.OLECMD oleCmd = new OleInterop.OLECMD();
            oleCmd.cmdf = 0;
            oleCmd.cmdID = (uint)VSConstants.VSStd97CmdID.SelectAll;    // test for "SelectAll" command
            OleInterop.OLECMD[] prgCmds = new OleInterop.OLECMD[] { oleCmd };

            IntPtr ptr = new IntPtr();

            int res = target.QueryStatus(ref guidCmdGroup, (uint)1, prgCmds, ptr);
            Assert.AreEqual(res, VSConstants.S_OK, "QueryStatus, Select All command testing. Method has not return expected value");
        }

        /// <summary>
        /// The test for QueryStatus. Case of execution of command "Cut".
        /// </summary>
        [TestMethod()]
        public void QueryStatusTestCutCommandTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            Guid guidCmdGroup = VSConstants.GUID_VSStandardCommandSet97;
            OleInterop.OLECMD oleCmd = new OleInterop.OLECMD();
            oleCmd.cmdf = 0;
            oleCmd.cmdID = (uint)VSConstants.VSStd97CmdID.Cut;  // test for "Cut" command
            OleInterop.OLECMD[] prgCmds = new OleInterop.OLECMD[] { oleCmd };

            IntPtr ptr = new IntPtr();

            // Here we tests "Cut" command with some selected text block
            accessor.editorControl.Text = testString;
            accessor.editorControl.Select(0, 2);

            int res = target.QueryStatus(ref guidCmdGroup, (uint)1, prgCmds, ptr);
            Assert.AreEqual(res, VSConstants.S_OK, "QueryStatus for Cut command returned unexpected value.");
        }

        /// <summary>
        /// The test for QueryStatus. Case of execution of command "Paste".
        /// </summary>
        [TestMethod()]
        public void QueryStatusTestPasteCommandTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            Guid guidCmdGroup = VSConstants.GUID_VSStandardCommandSet97;
            OleInterop.OLECMD oleCmd = new OleInterop.OLECMD();
            oleCmd.cmdf = 0;
            oleCmd.cmdID = (uint)VSConstants.VSStd97CmdID.Paste;    // test for "Paste" command
            OleInterop.OLECMD[] prgCmds = new OleInterop.OLECMD[] { oleCmd };

            IntPtr ptr = new IntPtr();

            // Here we do something for store data to the clipboard
            accessor.editorControl.Text = testString;
            accessor.editorControl.Select(0, 2);
            accessor.editorControl.Cut();

            int res = target.QueryStatus(ref guidCmdGroup, (uint)1, prgCmds, ptr);
            Assert.AreEqual(res, VSConstants.S_OK, "QueryStatus for Paste command returned unexpected value.");
        }

        /// <summary>
        /// The test for QueryStatus. Case of execution of command "Redo".
        /// </summary>
        [TestMethod()]
        public void QueryStatusTestRedoCommandTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            Guid guidCmdGroup = VSConstants.GUID_VSStandardCommandSet97;
            OleInterop.OLECMD oleCmd = new OleInterop.OLECMD();
            oleCmd.cmdf = 0;
            oleCmd.cmdID = (uint)VSConstants.VSStd97CmdID.Redo;     // test for "Redo" command
            OleInterop.OLECMD[] prgCmds = new OleInterop.OLECMD[] { oleCmd };

            IntPtr ptr = new IntPtr();

            // Here we do something and "Undo" it for enabling "Redo" command
            accessor.editorControl.Text = testString;
            accessor.editorControl.Select(0, 2);
            accessor.editorControl.Cut();
            accessor.editorControl.Undo();

            int res = target.QueryStatus(ref guidCmdGroup, (uint)1, prgCmds, ptr);
            Assert.AreEqual(res, VSConstants.S_OK, "QueryStatus for Redo command returned unexpected value.");
        }

        /// <summary>
        /// The test for QueryStatus. Case of execution of command "Undo".
        /// </summary>
        [TestMethod()]
        public void QueryStatusTestUndoCommandTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            Guid guidCmdGroup = VSConstants.GUID_VSStandardCommandSet97;
            OleInterop.OLECMD oleCmd = new OleInterop.OLECMD();
            oleCmd.cmdf = 0;
            oleCmd.cmdID = (uint)VSConstants.VSStd97CmdID.Undo; // test for "Undo" command
            OleInterop.OLECMD[] prgCmds = new OleInterop.OLECMD[] { oleCmd };

            IntPtr ptr = new IntPtr();

            // Here we execute Cut() TextBox action for enable command
            accessor.editorControl.Text = testString;
            accessor.editorControl.Select(0, 2);
            accessor.editorControl.Cut();

            int res = target.QueryStatus(ref guidCmdGroup, (uint)1, prgCmds, ptr);
            Assert.AreEqual(VSConstants.S_OK, res, "QueryStatus for Undo command returned unexpected value.");
        }

        /// <summary>
        /// The test for Exec. Case of execution of command "Copy".
        ///</summary>
        [TestMethod()]
        public void ExecTestCopyCommandTest()
        {
            EditorPane target = editorPane;
            //check whether object is of IVsWindowPane type
            Assert.IsNotNull(target as IVsWindowPane, "The object doesn't implement IVsWindowPane interface");

            //prepare component for command execution
            PrepareComponentSelectedText();

            Guid pguidCmdGroup = VSConstants.GUID_VSStandardCommandSet97;
            uint nCmdID = (uint)VSConstants.VSStd97CmdID.Copy;  // test for Copy command

            int res = target.Exec(ref pguidCmdGroup, nCmdID, 0, new IntPtr(), new IntPtr());
            Assert.AreEqual(VSConstants.S_OK, res, "Exec function for Copy command returned unexpected value.");
        }

        /// <summary>
        /// The test for Exec. Case of presence of "Not supported" commands group.
        ///</summary>
        [TestMethod()]
        public void ExecTestNotSupportedCmdGroupTest()
        {
            EditorPane target = editorPane; ;
            //check whether object is of IVsWindowPane type
            Assert.IsNotNull(target as IVsWindowPane, "The object doesn't implement IVsWindowPane interface");

            Guid pguidCmdGroup = Guid.Empty;    // test for Not supported group
            uint nCmdID = (uint)VSConstants.VSStd97CmdID.NewWindow; // any supported command 

            int res = target.Exec(ref pguidCmdGroup, nCmdID, 0, new IntPtr(), new IntPtr());
            Assert.AreEqual((int)OleInterop.Constants.OLECMDERR_E_UNKNOWNGROUP, res,
                   "Exec function for notSupported group returned unexpected value.");
        }

        ///<summary>
        /// The test for Exec. Case of a "Not Supported" command execution.
        ///</summary>
        [TestMethod()]
        public void ExecTestNotSupportedCommandTest()
        {
            EditorPane target = editorPane;
            //check whether object is of IVsWindowPane type
            Assert.IsNotNull(target as IVsWindowPane, "The object doesn't implement IVsWindowPane interface");

            Guid pguidCmdGroup = VSConstants.GUID_VSStandardCommandSet97;   // any supported group
            uint nCmdID = uint.MinValue;    // test for Not supported command

            //checking unknown command for VS standard command set 
            int res = target.Exec(ref pguidCmdGroup, nCmdID, 0, new IntPtr(), new IntPtr());
            Assert.AreEqual((int)OleInterop.Constants.OLECMDERR_E_NOTSUPPORTED, res);

            //checking unknown command for editor specific command set 
            pguidCmdGroup = Guid.NewGuid();

            res = target.Exec(ref pguidCmdGroup, nCmdID, 0, new IntPtr(), new IntPtr());
            Assert.AreEqual((int)OleInterop.Constants.OLECMDERR_E_UNKNOWNGROUP, res, "Exec function for notSupported command returned unexpected value.");
        }

        /// <summary>
        /// The test for Exec. Case of a "Cut" command execution.
        ///</summary>
        [TestMethod()]
        public void ExecTestCutCommandTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            UInt32 nCmdID = (UInt32)VSConstants.VSStd97CmdID.Cut;   // test for "Cut" command

            // Expected and actual RichTextBox Text property values
            String expectedTextValue = String.Empty;
            String actualTextValue = String.Empty;

            // Expected and actual return code values
            Int32 expectedReturnCode = -1;
            Int32 actualReturnCode = -1;

            Guid pguidCmdGroup = VSConstants.GUID_VSStandardCommandSet97; // init by standard command set
            IntPtr pvaIn = new IntPtr();
            IntPtr pvaOut = new IntPtr();

            // Here we start to tests the "Cut" command without selection
            // Prepare expected values
            expectedReturnCode = VSConstants.S_OK;
            // store Text value before "Cut" operation
            expectedTextValue = accessor.editorControl.Text;
            // no selection
            accessor.editorControl.Select(0, 0);

            actualReturnCode = target.Exec(ref pguidCmdGroup, nCmdID, 0, pvaIn, pvaOut);

            // store Text value after "Cut" operation
            actualTextValue = accessor.editorControl.Text;

            Assert.AreEqual(expectedTextValue, actualTextValue, "After void Cutting operation we have unexpected Text value");
            Assert.AreEqual(expectedReturnCode, actualReturnCode, "Cut command test failed. Exec method did not return the expected value");

            // Here we start to tests the "Cut" command with some selection
            // Prepare expected values
            accessor.editorControl.Text = "One two three four five";
            expectedTextValue = "two three four five";
            expectedReturnCode = VSConstants.S_OK;
            // select "One " substring
            accessor.editorControl.Select(0, 4);
            actualReturnCode = target.Exec(ref pguidCmdGroup, nCmdID, 0, pvaIn, pvaOut);

            // store Text value after "Cut" operation
            actualTextValue = accessor.editorControl.Text;

            Assert.AreEqual(expectedTextValue, actualTextValue, "Invalid expected RichTextBox Text value after Cut operation");
            Assert.AreEqual(expectedReturnCode, actualReturnCode, "Cut command test failed. Exec returned unexpected value.");
        }

        /// <summary>
        /// The test for Exec. Case of a "Paste" command execution.
        ///</summary>
        [TestMethod()]
        public void ExecTestPasteCommandTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            uint nCmdID = (uint)VSConstants.VSStd97CmdID.Paste; // "Paste" command is tested

            // Expected and actual RichTextBox Text property values
            String expectedTextValue = String.Empty;

            // Expected and actual return code values
            int expectedReturnCode = -1;
            int actualReturnCode = -1;

            Guid pguidCmdGroup = VSConstants.GUID_VSStandardCommandSet97;   // init by standard commands set
            IntPtr pvaIn = new IntPtr();
            IntPtr pvaOut = new IntPtr();
            accessor.editorControl.Text = testString;

            // Here we start to tests the "Paste" command with:
            // not the empty clipboard and with some text selected in the RichTextBox
            // select first 5 letters
            accessor.editorControl.Select(0, 5);
            accessor.editorControl.Cut();
            Assert.AreNotEqual(accessor.editorControl.Text, testString, "The value of editorControl.Text after Cut operation was unexpected.");

            // actually test paste operation:
            expectedReturnCode = VSConstants.S_OK;
            // we expect this value
            expectedTextValue = testString;

            // NOTE: Sometimes this test is crashed during Paste() operation executing.
            // Probably it is the internal clipboard synchronization issue.
            actualReturnCode = target.Exec(ref pguidCmdGroup, nCmdID, 0, pvaIn, pvaOut);

            Assert.AreEqual(expectedTextValue, accessor.editorControl.Text, "The value of editorControl.Text after Paste operation with selected text was unexpected.");
            Assert.AreEqual(expectedReturnCode, actualReturnCode, "Paste command test failed. Exec returned unexpected value.");
        }

        /// <summary>
        /// The test for Exec. Case of a "Redo" command execution.
        ///</summary>
        [TestMethod()]
        public void ExecTestRedoCommandTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            UInt32 nCmdID = (UInt32)VSConstants.VSStd97CmdID.Redo;  // test for "Redo" command

            // Expected and actual RichTextBox Text property values
            String expectedTextValue = String.Empty;
            String actualTextValue = String.Empty;

            // Expected and actual return code values
            Int32 expectedReturnCode = -1;
            Int32 actualReturnCode = -1;

            Guid pguidCmdGroup = VSConstants.GUID_VSStandardCommandSet97; // init by standard command set

            IntPtr pvaIn = new IntPtr();
            IntPtr pvaOut = new IntPtr();

            // Here we start to tests the "Redo" command
            // prepare expected values
            expectedReturnCode = VSConstants.S_OK;
            expectedTextValue = String.Empty;

            accessor.editorControl.Text = testString;

            // perform text-distortion operation
            accessor.editorControl.SelectAll();
            accessor.editorControl.Cut();
            Assert.IsTrue(accessor.editorControl.Text.Length == 0, "Delete all operation was not completed successfully.");

            accessor.editorControl.Undo();
            // is Redo command are enabled?
            Assert.AreEqual(accessor.editorControl.Text, testString, false, "Undo operation leaded to unexpected result.");

            // perform Redo operation
            actualReturnCode = target.Exec(ref pguidCmdGroup, nCmdID, 0, pvaIn, pvaOut);

            // store Text value after "Redo" operation
            actualTextValue = accessor.editorControl.Text;    // empty string is expected.

            Assert.AreEqual(expectedTextValue, actualTextValue, "After Redo operation we have not empty string (Redo Clear operation scenario)");
            Assert.AreEqual(expectedReturnCode, actualReturnCode, "Redo command test failed. Exec returned unexpected value.");
        }

        /// <summary>
        /// The test for Exec. Case of a "Undo" command execution.
        ///</summary>
        [TestMethod()]
        public void ExecTestUndoCommandTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            UInt32 nCmdID = (UInt32)VSConstants.VSStd97CmdID.Undo; // "Undo" command is tested         

            // Expected and actual RichTextBox Text property values
            String expectedTextValue = String.Empty;
            String actualTextValue = String.Empty;

            // containers for function return code
            Int32 expectedReturnCode = -1;
            Int32 actualReturnCode = -1;

            Guid pguidCmdGroup = new Guid();
            // init by standard command set
            pguidCmdGroup = VSConstants.GUID_VSStandardCommandSet97;

            IntPtr pvaIn = new IntPtr();
            IntPtr pvaOut = new IntPtr();

            // Here we start to tests the "Undo" command.
            // prepare expected values
            expectedReturnCode = VSConstants.S_OK;

            String textBeforeChanges = "Original RichTextBox text";
            expectedTextValue = textBeforeChanges;

            // save text value before text-distortion operation
            accessor.editorControl.Text = textBeforeChanges;

            // execute test routine context
            // perform text-distortion operation
            accessor.editorControl.SelectAll();
            accessor.editorControl.Cut();

            Assert.IsTrue(accessor.editorControl.Text.Length == 0, "Delete all operation was not completed successfully.");

            // perform Undo operation for recover original text value
            actualReturnCode = target.Exec(ref pguidCmdGroup, nCmdID, 0, pvaIn, pvaOut);

            // store Text value after "Undo" operation
            actualTextValue = accessor.editorControl.Text;
            Assert.AreEqual(expectedTextValue, actualTextValue, "After Undo operation we have not recovered original text value");
            Assert.AreEqual(expectedReturnCode, actualReturnCode, "Undo command test failed. Exec returned unexpected value.");
        }

        /// <summary>
        /// The test for Exec. Case of a "Select All" command execution.
        ///</summary>
        [TestMethod()]
        public void ExecTestSelectAllCommandTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            UInt32 nCmdID = (UInt32)VSConstants.VSStd97CmdID.SelectAll; // test for "SelectAll" command

            // Expected and actual text property values
            Int32 expectedSelectionLength = -1;
            Int32 actualSelectionLength = -1;

            // Expected and actual return code values
            Int32 expectedReturnCode = -1;
            Int32 actualReturnCode = -1;

            Guid pguidCmdGroup = new Guid();
            // init by standard command set
            pguidCmdGroup = VSConstants.GUID_VSStandardCommandSet97;

            IntPtr pvaIn = new IntPtr();
            IntPtr pvaOut = new IntPtr();

            // Here we start to tests the "SelectAll" command
            // Prepare some text in RichTextBox
            String testSelectionString = "It's some text for test SelectAll feature";
            accessor.editorControl.Text = testSelectionString;

            // Prepare expected values
            expectedReturnCode = VSConstants.S_OK;
            expectedSelectionLength = testSelectionString.Length;

            // Execute test routine context
            actualReturnCode = target.Exec(ref pguidCmdGroup, nCmdID, 0, pvaIn, pvaOut);

            actualSelectionLength = accessor.editorControl.SelectionLength;

            Assert.AreEqual(expectedSelectionLength, actualSelectionLength, "After SelectAll command not all text was selected");
            Assert.AreEqual(expectedReturnCode, actualReturnCode, "SelectAll command test failed. Exec returned expected value.");
        }

        ///<summary>
        /// The test for Exec. Scenario when pguidCmdGroup == GuidList.guidEditorCmdSet
        ///</summary>
        [TestMethod()]
        public void ExecTestGuidEditorCmdGroupTest()
        {
            EditorPane target = editorPane;
            //check whether object is of IVsWindowPane type
            Assert.IsNotNull(target as IVsWindowPane, "The object doesn't implement IVsWindowPane interface");

            Guid pguidCmdGroup = Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_GuidListAccessor.guidEditorCmdSet;
            uint nCmdID = uint.MinValue;    // test for Not supported command

            //checking unknown command for VS standard command set 
            int res = target.Exec(ref pguidCmdGroup, nCmdID, 0, new IntPtr(), new IntPtr());
            Assert.AreEqual((int)OleInterop.Constants.OLECMDERR_E_NOTSUPPORTED, res);
        }

        #endregion

        #region IPersist tests

        /// <summary>
        /// A test for GetClassID() method.
        /// </summary>
        [TestMethod()]
        public void IPersistGetClassIdTest()
        {
            EditorPane target = editorPane;
            Guid pClassId;

            int actual_result = ((IPersist)target).GetClassID(out pClassId);
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method IPersist.GetClassID() returned unexpected result");
            Assert.IsNotNull(pClassId, "pClassId are not assigned");
        }

        #endregion IPersist tests

        #region IVsPersistDocData tests

        /// <summary>
        /// The test for IsDocDataDirty() method.
        /// </summary>
        [TestMethod()]
        public void IsDocDataDirtyTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            int pfDirty;
            int fExpectedDirtyValue = (accessor.isDirty ? 1 : 0);

            int actual_result = ((IVsPersistDocData)target).IsDocDataDirty(out pfDirty);
            Assert.AreEqual(fExpectedDirtyValue, pfDirty,
                String.Format("IsDocDataDirty() returned unexpected pfDirty value, expected are {0}", fExpectedDirtyValue));
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method IsDocDataDirty() returned unexpected value.");
        }
        /// <summary>
        /// The test for LoadDocData() method.
        /// </summary>
        [TestMethod()]
        public void LoadDocDataTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            string pszMkDocument = accessor.fileName;
            int actual_result = ((IVsPersistDocData)target).LoadDocData(pszMkDocument);
            Assert.AreEqual(VSConstants.S_OK, actual_result,
                String.Format("LoadDocData() returned unexpected Document value, expected are {0}", VSConstants.S_OK));
        }
        /// <summary>
        /// The test for SetUntitledDocPath() method.
        /// </summary>
        [TestMethod()]
        public void SetUntitledDocPathTest()
        {
            EditorPane target = editorPane;

            string pszDocPath = "some invalid path";
            int actual_result = ((IVsPersistDocData)target).SetUntitledDocPath(pszDocPath);
            Assert.AreEqual(VSConstants.S_OK, actual_result, "LoadDocData() returned unexpected Document value, expected are S_OK");
        }
        /// <summary>
        /// The test for Close() method.
        /// </summary>
        [TestMethod()]
        public void IPersistDocDataCloseTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            accessor.editorControl = new EditorControl();
            int actual_result = ((IVsPersistDocData)target).Close();
            Assert.IsTrue(accessor.editorControl.IsDisposed, "EditorPane Text Box instance was not Disposed properly after Close() method execution.");
            Assert.AreEqual(VSConstants.S_OK, actual_result, "IVsPersistDocData.Close() returned unexpected result.");
        }
        /// <summary>
        /// The test for GetGuidEditorType() method.
        /// </summary>
        [TestMethod()]
        public void GetGuidEditorTypeTest()
        {
            EditorPane target = editorPane;
            // create empty guid instance
            Guid pClassID = Guid.Empty;

            int actual_result = ((IVsPersistDocData)target).GetGuidEditorType(out pClassID);
            Assert.IsTrue((pClassID != Guid.Empty), "Editor type Guid was not initialized.");
            Assert.AreEqual(VSConstants.S_OK, actual_result, "IVsPersistDocData.Close() returned unexpected result.");
        }
        /// <summary>
        /// The test for IsDocDataReloadable() method.
        /// </summary>
        [TestMethod()]
        public void IsDocDataCanReloadTest()
        {
            EditorPane target = editorPane;
            // create uninitialized
            int pfReloadable;

            int actual_result = ((IVsPersistDocData)target).IsDocDataReloadable(out pfReloadable);
            Assert.IsTrue(pfReloadable != 0, "pfReloadable was not properly initialized");
            Assert.AreEqual(VSConstants.S_OK, actual_result, "IVsPersistDocData.IsDocDataReloadable() returned unexpected result.");
        }
        /// <summary>
        /// The test for RenameDocData() method.
        /// </summary>
        [TestMethod()]
        public void RenameDocDataTest()
        {
            EditorPane target = editorPane;
            uint gfrAttrin = 0, itemidNew = 0;
            object pHierNew = new object() as IVsHierarchy;
            string pszMkDocumentNew = "New Document";

            int actual_result = ((IVsPersistDocData)target).RenameDocData(gfrAttrin, (IVsHierarchy)pHierNew, itemidNew, pszMkDocumentNew);
            Assert.AreEqual(VSConstants.S_OK, actual_result, "IVsPersistDocData.RenameDocData() returned unexpected result.");
        }
        /// <summary>
        /// The test for ReloadDocData() method.
        /// </summary>
        [TestMethod()]
        public void ReloadDocDataTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            uint grfFlags = 0;

            int actual_result = ((IVsPersistDocData)target).ReloadDocData(grfFlags);
            Assert.IsFalse(accessor.isDirty, "IsDirty flag was not reseted after document data reloading.");
            Assert.AreEqual(VSConstants.S_OK, actual_result, "IVsPersistDocData.ReloadDocData() returned unexpected result.");
        }
        /// <summary>
        /// The test for OnRegisterDocData() method.
        /// </summary>
        [TestMethod()]
        public void OnRegisterDocData()
        {
            EditorPane target = editorPane;
            uint docCookie = 0, itemidNew = 0;
            object pHierNew = new object() as IVsHierarchy;

            int actual_result = ((IVsPersistDocData)target).OnRegisterDocData(docCookie, (IVsHierarchy)pHierNew, itemidNew);
            Assert.AreEqual(VSConstants.S_OK, actual_result, "IVsPersistDocData.OnRegisterDocData() returned unexpected result.");
        }
        /// <summary>
        /// The test for SaveDocData() method.
        /// </summary>
        [TestMethod()]
        public void SaveDocDataTest()
        {
            ((IPersistFileFormat)editorPane).Load(filePath, 0, 0);

            string appendText = "Text Appended from the Editor";
            editorPaneAccessor.editorControl.Text += appendText;

            int saveCanceled = 0;

            //Save functionality
            Assert.AreEqual(VSConstants.S_OK, ((IVsPersistDocData)editorPane).SaveDocData(VSSAVEFLAGS.VSSAVE_Save, out filePath, out saveCanceled));
            Assert.AreEqual(VSConstants.S_OK, VerifyFileContents(filePath, filePath, editorPaneAccessor.editorControl.Text));

            //Save As functionality
            string newFilePath = string.Empty;
            Assert.AreEqual(VSConstants.S_OK, ((IVsPersistDocData)editorPane).SaveDocData(VSSAVEFLAGS.VSSAVE_SaveAs, out newFilePath, out saveCanceled));
            Assert.AreEqual(VSConstants.S_OK, VerifyFileContents(filePath, newFilePath, editorPaneAccessor.editorControl.Text));

            //SaveCopyAs Functionality
            Assert.AreEqual(VSConstants.S_OK, ((IVsPersistDocData)editorPane).SaveDocData(VSSAVEFLAGS.VSSAVE_SaveCopyAs, out newFilePath, out saveCanceled));
            Assert.AreEqual(VSConstants.S_OK, VerifyFileContents(filePath, newFilePath, editorPaneAccessor.editorControl.Text));

            //SilentSave functionality
            Assert.AreEqual(VSConstants.S_OK, ((IVsPersistDocData)editorPane).SaveDocData(VSSAVEFLAGS.VSSAVE_SilentSave, out filePath, out saveCanceled));
            Assert.AreEqual(VSConstants.S_OK, VerifyFileContents(filePath, filePath, editorPaneAccessor.editorControl.Text));
        }

        //[TestMethod()]
        //public void DoDragAndDropTest()
        //{
        //    EditorPane target = editorPane;
        //    Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

        //    object testedObject = new object();
        //    OleDataObject dataObject = new OleDataObject();
        //    ToolboxItemData toolboxdata = new ToolboxItemData("test sentence");
        //    dataObject.SetData(toolboxdata);

        //    testedObject = dataObject;

        //    accessor.editorControl.DoDragDrop(dataObject, DragDropEffects.All);
        //}

        #endregion IVsPersistDocData tests

        #region IPersistFileFormat tests
        /// <summary>
        /// The test for SaveCompleted() method.
        /// </summary>
        [TestMethod()]
        public void SaveCompletedTest()
        {
            EditorPane target = editorPane;

            string pszFileName = "SomeFileName.Txt";
            int actual_result = ((IPersistFileFormat)target).SaveCompleted(pszFileName);
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method SaveCompleted() returned unexpected value.");
        }
        /// <summary>
        /// The test for GetCurFile() method.
        /// </summary>
        [TestMethod()]
        public void GetCurFileTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            // create uninitialized
            string pszFileName;
            uint pnFormatIndex;

            int actual_result = ((IPersistFileFormat)target).GetCurFile(out pszFileName, out pnFormatIndex);
            Assert.AreEqual(accessor.fileName, pszFileName, "Returned file name value is unexpected.");
            Assert.AreEqual(
                Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor.fileFormat,
                pnFormatIndex,
                "Returned format index value is unexpected.");

            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method GetCurFile() returned unexpected value.");
        }
        /// <summary>
        /// The test for InitNew() method in case of invalid specified format value.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void InitNewWithInvalidFormatTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            // prepare format index, which are not equivalent with myFormat value
            // and expect exception occurrence
            uint pnFormatIndex = 1 + Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor.fileFormat;

            int actual_result = ((IPersistFileFormat)target).InitNew(pnFormatIndex);

            Assert.IsFalse(accessor.isDirty, "Dirty flag is not set to true");
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method InitNew() returned unexpected value.");
        }
        /// <summary>
        /// The test for InitNew() method in case of correct specified format value.
        /// </summary>
        [TestMethod()]
        public void InitNewWithCorrectFormatTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            // prepare format index, which are equivalent with myFormat value
            uint pnFormatIndex = Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor.fileFormat;

            int actual_result = ((IPersistFileFormat)target).InitNew(pnFormatIndex);

            Assert.IsFalse(accessor.isDirty, "Dirty flag is not set to true");
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method InitNew() returned unexpected value.");
        }
        /// <summary>
        /// The test for GetClassID() method.
        /// </summary>
        [TestMethod()]
        public void GetClassIdTest()
        {
            EditorPane target = editorPane;

            Guid pClassId;
            Guid pExpectedClassID;

            ((IPersist)target).GetClassID(out pExpectedClassID);
            int actual_result = ((IPersistFileFormat)target).GetClassID(out pClassId);
            Assert.AreEqual(pExpectedClassID, pClassId, "Class ID was initialized by unexpected Guid value.");

            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method GetClassID() returned unexpected value.");
        }
        /// <summary>
        /// The test for GetFormatList() method.
        /// </summary>
        [TestMethod()]
        public void GetFormatListTest()
        {
            EditorPane target = editorPane;
            string ppszFormatList = null;

            int actual_result = ((IPersistFileFormat)target).GetFormatList(out ppszFormatList);
            Assert.IsNotNull(ppszFormatList, "Format List was not initialized.");
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method GetFormatList() returned unexpected value.");
        }
        /// <summary>
        /// The test for IsDirty() method when isDirty is true.
        /// </summary>
        [TestMethod()]
        public void IsDirtyTestWithDirtyTrueTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            int pfIsDirty;
            accessor.isDirty = true;
            int actual_result = ((IPersistFileFormat)target).IsDirty(out pfIsDirty);
            Assert.AreEqual(pfIsDirty, 1, "IsDirty returned unexpected value of isDirty flag");
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method IsDirty() returned unexpected value.");
        }
        /// <summary>
        /// The test for IsDirty() method when isDirty is false.
        /// </summary>
        [TestMethod()]
        public void IsDirtyTestWithDirtyFalseTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            int pfIsDirty;
            accessor.isDirty = false;
            int actual_result = ((IPersistFileFormat)target).IsDirty(out pfIsDirty);
            Assert.AreEqual(pfIsDirty, 0, "IsDirty returned unexpected value of isDirty flag");
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method IsDirty() returned unexpected value.");
        }
        /// <summary>
        /// The test for Load() method in case of invalid input file name argument.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IPersistFileFormatLoadWithInvalidFileNameTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);
            string pszFileName = null;
            accessor.fileName = null;
            uint grfMode = 0;
            int fReadOnly = 0;

            int actual_result = ((IPersistFileFormat)target).Load(pszFileName, grfMode, fReadOnly);
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method Load() returned unexpected value.");
        }
        /// <summary>
        /// The test for Load() method in scenario with notification about reloading.
        /// </summary>
        [TestMethod()]
        public void IPersistFileFormatLoadWithReloadingTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            string pszFileName = null;
            uint grfMode = 0;
            int fReadOnly = 0;
            int actual_result = ((IPersistFileFormat)target).Load(pszFileName, grfMode, fReadOnly);

            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method Load() returned unexpected value.");
            Assert.IsFalse(accessor.isDirty, "isDirty flag must be false after successful file loading.");
        }
        /// <summary>
        /// The test for Load() method in scenario with notification about loading.
        /// </summary>
        [TestMethod()]
        public void IPersistFileFormatLoadWithoutReloadingTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            string pszFileName = accessor.fileName;
            uint grfMode = 0;
            int fReadOnly = 0;
            int actual_result = ((IPersistFileFormat)target).Load(pszFileName, grfMode, fReadOnly);

            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method Load() returned unexpected value.");
            Assert.IsFalse(accessor.isDirty, "isDirty flag must be false after successful file loading.");
        }
        /// <summary>
        /// The test for Save() method with equivalent internal and specified file names.
        /// </summary>
        [TestMethod()]
        public void IPersistFileFormatSaveWithSameNameTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            // assign to the argument same as the internal file name
            string pszFileName = accessor.fileName;
            uint uFormatIndex = 0;
            int fRemember = 0;
            int actual_result = ((IPersistFileFormat)target).Save(pszFileName, fRemember, uFormatIndex);

            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method Save() returned unexpected value.");
            Assert.IsFalse(accessor.isDirty, "isDirty flag must be false after successful file loading.");
        }
        /// <summary>
        /// The test for Save() method in case of different internal file name 
        /// and specified in input argument.
        /// </summary>
        [TestMethod()]
        public void IPersistFileFormatSaveWithAnotherNameTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            // assign to the argument same as the internal file name
            string pszFileName = "SomeFileName.Ext";
            uint uFormatIndex = 0;
            int fRemember = 0;
            int actual_result = ((IPersistFileFormat)target).Save(pszFileName, fRemember, uFormatIndex);

            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method Save() returned unexpected value.");
            Assert.IsFalse(accessor.isDirty, "isDirty flag must be false after successful file loading.");
        }
        /// <summary>
        /// The test for Save() method in scenario when file name 
        /// was reassigned to the specified value.
        /// </summary>
        [TestMethod()]
        public void IPersistFileFormatSaveWithReassignFileNameTest()
        {
            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            // assign to the argument same as the internal file name
            string pszFileName = "SomeFileName.Ext";
            uint uFormatIndex = 0;
            // reassign file name
            int fRemember = 1;
            int actual_result = ((IPersistFileFormat)target).Save(pszFileName, fRemember, uFormatIndex);

            Assert.AreEqual(accessor.fileName, pszFileName, "FileName was not reassigned in Save() with switched fRemember option.");
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method Save() returned unexpected value.");
            Assert.IsFalse(accessor.isDirty, "isDirty flag must be false after successful file loading.");
        }

        #endregion IPersistFileFormat tests

        #region IVsToolboxUser tests
        /// <summary>
        /// The test for IsSupported() method in case when ToolboxItemData is not included
        /// in object data.
        /// </summary>
        [TestMethod()]
        public void IsSupportedWithoutToolboxItemDataTest()
        {
            EditorPane target = editorPane;

            OleDataObject dataObject = new OleDataObject();
            int actual_result = ((IVsToolboxUser)target).IsSupported(dataObject);
            Assert.AreEqual(VSConstants.S_FALSE, actual_result, "Method IsSupported() returned unexpected result");
        }
        /// <summary>
        /// The test for IsSupported() method in case when ToolboxItemData is included
        /// in object data.
        /// </summary>
        [TestMethod()]
        public void IsSupportedWithToolboxItemDataTest()
        {
            EditorPane target = editorPane;

            OleDataObject dataObject = new OleDataObject();
            ToolboxItemData toolboxdata = new ToolboxItemData("test sentence");
            dataObject.SetData(toolboxdata);
            int actual_result = ((IVsToolboxUser)target).IsSupported(dataObject);
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method IsSupported() returned unexpected result");
        }
        /// <summary>
        /// The test for ItemPicked() method in case when ToolboxItemData is included
        /// in object.
        /// </summary>
        [TestMethod()]
        public void ItemPickedTest()
        {
            EditorPane target = editorPane;
            OleDataObject dataObject = new OleDataObject();
            ToolboxItemData toolboxdata = new ToolboxItemData("test sentence");

            dataObject.SetData(toolboxdata);
            int actual_result = ((IVsToolboxUser)target).ItemPicked(dataObject);
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method IsSupported() returned unexpected result");
        }
        #endregion IVsToolboxUser tests

        #region Event handlers tests
        /// <summary>
        /// The test for OnDragEnter() event handler.
        ///</summary>
        [TestMethod()]
        public void EditorControlDragEnterTest()
        {
            EditorPane target = editorPane;

            Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests.Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests.Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            object sender = null; // any sender
            OleDataObject dataObject = new OleDataObject();
            ToolboxItemData toolboxdata = new ToolboxItemData("test sentence");
            dataObject.SetData(toolboxdata);

            DragEventArgs e = new DragEventArgs(dataObject, 0, 0, 0, DragDropEffects.All, DragDropEffects.None);

            accessor.OnDragEnter(sender, e);
            Assert.AreEqual(DragDropEffects.Copy, e.Effect, "Drag effect was not properly initialized.");
        }

        /// <summary>
        /// The test for OnDragDrop() event handler.
        ///</summary>
        [TestMethod()]
        public void EditorControlDragDropTest()
        {
            EditorPane target = editorPane;
            Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests.Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests.Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            object sender = target;

            string toolBoxText = "TollBoxItem";
            OleDataObject dataObject = new OleDataObject();
            ToolboxItemData toolboxdata = new ToolboxItemData(toolBoxText);
            dataObject.SetData(toolboxdata);

            DragEventArgs e = new DragEventArgs(dataObject, 0, 0, 0, DragDropEffects.All, DragDropEffects.Copy);

            accessor.OnDragDrop(sender, e);

            Assert.IsTrue(accessor.editorControl.Text.IndexOf(toolBoxText) >= 0, "Drop operation was not successful");
        }
        #endregion Event handlers tests

        #region Other methods tests

        /// <summary>
        ///The test for CanEditFile() method.
        ///</summary>
        [TestMethod()]
        public void CanEditFileTest()
        {
            EditorPane target = editorPane; // the component is already sited
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);
            bool expected = true;
            bool actual;

            actual = accessor.CanEditFile();

            Assert.AreEqual(expected, actual, "EditorPane.CanEditFile did " +
                    "not return the expected value.");

            accessor.gettingCheckoutStatus = true;
            expected = false;
            actual = accessor.CanEditFile();

            Assert.AreEqual(expected, actual, "EditorPane.CanEditFile did " +
                   "not return the expected value.");
        }

        /// <summary>
        /// The test for NotifyDocChanged() method.
        ///</summary>
        [TestMethod()]
        public void NotifyDocChangedTest()
        {
            EditorPane target = editorPane;//Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor.CreatePrivate();
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            // initialize by null, valid value was assigned into the function
            accessor.NotifyDocChanged();
        }
        #endregion Other methods tests
        #endregion Test methods

        #region Service functions
        /// <summary>
        // Compare the contents of a file against the specified text
        /// </summary>
        /// <returns>S_OK if method was succeeds, otherwise E_FAIL.</returns>
        public int VerifyFileContents(string oldFilePath, string newFilePath, string text)
        {
            EditorPane target = editorPane;

            //Load the new file
            ((IPersistFileFormat)target).Load(newFilePath, 0, 0);

            int hr = VSConstants.E_FAIL;

            //verify the contents
            if (editorPaneAccessor.editorControl.Text.Equals(text))
            {
                hr = VSConstants.S_OK;
            }

            //load the old file
            ((IPersistFileFormat)target).Load(oldFilePath, 0, 0);
            return hr;
        }
        /// <summary>
        /// Prepare some text in text box with some selection.
        /// </summary>
        public void PrepareComponentSelectedText()
        {
            Assert.IsNotNull(editorPane, "editorPane null reference assertion failed");

            EditorPane target = editorPane;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target);

            //object accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor.CreatePrivate();

            accessor.editorControl.Text = testString; // "this is a test string."
            accessor.editorControl.SelectionStart = 2;
            accessor.editorControl.SelectionLength = 2;// first "is" substring is selected
        }
        /// <summary>
        /// Create a TempFile for loading purposes.
        /// </summary>
        public void CreateTempFile()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            StreamWriter sw = File.AppendText(filePath);
            sw.Write(testString);
            sw.Close();
        }
        /// <summary>
        /// Drop TempFile.
        /// </summary>
        public void DropTempFile()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        /// <summary>
        /// Add some basic service mock objects to the service provider.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void AddBasicSiteSupport(OleServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            // Add site support for UI Shell
            BaseMock uiShell = MockServicesProvider.GetUiShellInstance0();
            serviceProvider.AddService(typeof(SVsUIShell), uiShell, false);
            serviceProvider.AddService(typeof(SVsUIShellOpenDocument), (IVsUIShellOpenDocument)uiShell, false);

            //Add site support for Running Document Table
            BaseMock runningDoc = MockServicesProvider.GetRunningDocTableInstance();
            serviceProvider.AddService(typeof(SVsRunningDocumentTable), runningDoc, false);

            //Add site support for IVsTextManager
            BaseMock queryEditQuerySave = MockServicesProvider.GetQueryEditQuerySaveInstance();
            serviceProvider.AddService(typeof(SVsQueryEditQuerySave), queryEditQuerySave, false);

            BaseMock toolbox = MockIVsToolbox.GetIVsToolboxInstance();
            serviceProvider.AddService(typeof(SVsToolbox), toolbox, false);
        }
        #endregion Service Functions
    }
}
