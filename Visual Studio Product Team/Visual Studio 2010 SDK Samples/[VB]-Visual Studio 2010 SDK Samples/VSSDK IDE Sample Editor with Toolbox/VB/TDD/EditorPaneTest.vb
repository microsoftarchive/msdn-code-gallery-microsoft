'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************


Imports Microsoft.VisualBasic
Imports System
Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VsSDK.UnitTestLibrary
Imports System.Windows.Forms
Imports OleInterop = Microsoft.VisualStudio.OLE.Interop

Imports Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.OLE.Interop

Namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests
	''' <summary>
    ''' This is a test class for Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.EditorPane and is intended
    ''' to contain all Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.EditorPane Unit Tests.
	'''</summary>
	<TestClass()> _
	Public Class EditorPaneTest
		Implements IDisposable
		#Region "Fields"
		Private editorPane As EditorPane
		Private editorPaneAccessor As Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor

		Private filePath As String = String.Empty
		Private testString As String = String.Empty
		Private serviceProvider As OleServiceProvider
		Private testContextInstance As TestContext

		#End Region

		#Region "Properties"

		''' <summary>
        ''' Gets or sets the test context which provides
        ''' information about and functionality for the current test run.
		'''</summary>
		Public Property TestContext() As TestContext
			Get
				Return testContextInstance
			End Get
			Set(ByVal value As TestContext)
				testContextInstance = value
			End Set
		End Property
		#End Region

		#Region "Initialization && Cleanup"
		''' <summary>
		''' Runs before the test to allocate and configure resources needed 
		''' by all tests in the test class.
		''' </summary>
		<TestInitialize()> _
		Public Sub EditorPaneTestInitialize()
            ' Initialize base test context.
			testString = "This is a temporary file"

            filePath = AppDomain.CurrentDomain.BaseDirectory & Path.DirectorySeparatorChar & "TempFile.tbx"

			CreateTempFile()

            ' Prepare service provider.
			serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()
			AddBasicSiteSupport(serviceProvider)

            ' Prepare editorPane.
			editorPane = New EditorPane()
			Assert.AreEqual(Of Integer)(VSConstants.S_OK, (CType(editorPane, IVsWindowPane)).SetSite(serviceProvider))
			editorPaneAccessor = New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(editorPane)

			editorPaneAccessor.fileName = filePath
		End Sub
		''' <summary>
		''' Runs after the test has run and to free resources obtained 
		''' by all the tests in the test class.
		''' </summary>
		<TestCleanup()> _
		Public Sub EditorPaneTestCleanup()
			DropTempFile()
			serviceProvider = Nothing
		End Sub
		#End Region

		#Region "IDisposable Pattern implementation"
		''' <summary>
		''' Implement IDisposable interface.
		''' </summary>
		Public Sub Dispose() Implements IDisposable.Dispose
			Dispose(True)
		End Sub
		''' <summary> 
		''' Clean up any resources being used.
		''' </summary>
		Protected Overridable Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
                If editorPane IsNot Nothing Then
                    editorPane.Dispose()
                    editorPane = Nothing
                End If
                If serviceProvider IsNot Nothing Then
                    serviceProvider.Dispose()
                    serviceProvider = Nothing
                End If
				GC.SuppressFinalize(Me)
			End If
		End Sub
		#End Region

		#Region "Test methods"
		#Region "Constructors && Initializers tests"
		''' <summary>
        ''' A test for EditorPane (EditorPackage) method.
        ''' </summary>
		<TestMethod()> _
		Public Sub ConstructorTestWithParameters()
            ' Prepare service provider. 
			serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()
			AddBasicSiteSupport(serviceProvider)

            Dim target As New EditorPane()
			Assert.IsNotNull(target, "EditorPane object was not created")

			Assert.AreEqual(Of Integer)(VSConstants.S_OK, (CType(target, IVsWindowPane)).SetSite(serviceProvider))
		End Sub

#End Region

		#Region "IDisposable tests"
		''' <summary>
        ''' The test for IDisposable interface implementation.
        ''' </summary>
		<TestMethod()> _
		Public Sub CheckIDisposableImplementationTest()
			Using target As EditorPane = editorPane
				Assert.IsNotNull(TryCast(target, IDisposable), "The object does not implement IDisposable")
			End Using
		End Sub

		''' <summary>
        ''' The test for Dispose() method.
        ''' </summary>
		<TestMethod()> _
		Public Sub DisposeTest()
			Dim target As EditorPane = editorPane

			target.Dispose()
			Assert.IsNull(editorPaneAccessor.editorControl, "Dispose() call for the EditorPane was not free editorControl object.")
		End Sub
		#End Region

		#Region "IOleCommandTarget tests"
		''' <summary>
        ''' The common test for QueryStatus method.
        ''' </summary>
		<TestMethod()> _
		Public Sub QueryStatusStandardTest()
			Dim target As EditorPane = editorPane
			Dim guidCmdGroup As Guid = VSConstants.GUID_VSStandardCommandSet97
            Dim oleCmd As New OleInterop.OLECMD()

			oleCmd.cmdf = 0
			oleCmd.cmdID = CUInt(Microsoft.VisualStudio.VSConstants.VSStd97CmdID.SelectAll)

            Dim prgCmds() As OleInterop.OLECMD = {oleCmd}
            Dim ptr As New IntPtr()

			Dim res As Integer = target.QueryStatus(guidCmdGroup, CUInt(1), prgCmds, ptr)
			Assert.AreEqual(VSConstants.S_OK, res, "The method has not return expected value")
		End Sub

		''' <summary>
        ''' The test for QueryStatus method. The SyncDesigner test scenario.
        ''' </summary>
		<TestMethod()> _
		Public Sub QueryStatusSyncDesignerTest()
			Dim target As EditorPane = editorPane
			Dim guidCmdGroup As Guid = VSConstants.GUID_VsTaskListViewHTMLTasks
            Dim oleCmd As New OleInterop.OLECMD()
			oleCmd.cmdf = 0
			oleCmd.cmdID = 265
            Dim prgCmds() As OleInterop.OLECMD = {oleCmd}
            Dim ptr As New IntPtr()

			Dim res As Integer = target.QueryStatus(guidCmdGroup, CUInt(1), prgCmds, ptr)
			Assert.AreEqual(CInt(Fix(OleInterop.Constants.OLECMDERR_E_NOTSUPPORTED)), res, "The QueryStatus method has not return expected value")
		End Sub

		''' <summary>
        ''' The test for QueryStatus method.
        ''' </summary>
		<TestMethod()> _
		Public Sub QueryStatusOtherTest()
			Dim target As EditorPane = editorPane
			Dim guidCmdGroup As Guid = VSConstants.GUID_VSStandardCommandSet97
            Dim oleCmd As New OleInterop.OLECMD()
			oleCmd.cmdf = 0
			oleCmd.cmdID = CUInt(Microsoft.VisualStudio.VSConstants.VSStd97CmdID.SelectAll)
            Dim prgCmds() As OleInterop.OLECMD = {oleCmd}
            Dim ptr As New IntPtr()

			Dim actual_result As Integer = target.QueryStatus(guidCmdGroup, CUInt(1), prgCmds, ptr)
			Assert.AreEqual(VSConstants.S_OK, actual_result)
			Assert.IsNotNull(prgCmds(0), "Wrong object reference stored in prgCmds[0]")
			Assert.IsTrue((prgCmds(0).cmdf And CUInt(OleInterop.OLECMDF.OLECMDF_ENABLED)) <> 0, "The QueryStatus method has not return expected value")
		End Sub

		''' <summary>
		''' The test for QueryStatus method. The scenario of null value of commands set.
		''' </summary>
		<TestMethod()> _
		Public Sub QueryStatusTestNullValueCommandsTest()
			Dim target As EditorPane = editorPane
			Dim guidCmdGroup As Guid = VSConstants.GUID_VSStandardCommandSet97
            Dim oleCmd As New OleInterop.OLECMD()
            oleCmd.cmdf = 0
            ' Any command.
            oleCmd.cmdID = CUInt(VSConstants.VSStd97CmdID.Copy)
            Dim ptr As New IntPtr()
            ' Pass null-referenced argument.
			Dim prgCmds As OleInterop.OLECMD() = Nothing

			Dim res As Integer = target.QueryStatus(guidCmdGroup, CUInt(1), prgCmds, ptr)
			Assert.AreEqual(res, VSConstants.E_INVALIDARG, "The QueryStatus method has not return expected value")
		End Sub
		''' <summary>
		''' The test for QueryStatus. The scenario of processing of not supported commands.
		''' </summary>
		<TestMethod()> _
		Public Sub QueryStatusTestNotSupportedCommandTest()
			Dim target As EditorPane = editorPane
			Dim guidCmdGroup As Guid = VSConstants.GUID_VSStandardCommandSet97
            Dim oleCmd As New OleInterop.OLECMD()
			oleCmd.cmdf = 0

            Dim prgCmds() As OleInterop.OLECMD = {oleCmd}
            Dim ptr As New IntPtr()

            ' Pass not supported command ID value.
			oleCmd.cmdID = 0

			Dim res As Integer = target.QueryStatus(guidCmdGroup, CUInt(1), prgCmds, ptr)
			Assert.AreEqual(res, CInt(Fix(OleInterop.Constants.OLECMDERR_E_NOTSUPPORTED)), "The QueryStatus method has not return expected value")
		End Sub
		''' <summary>
		''' The test for QueryStatus. The scenario of processing of not supported commands groups.
		''' </summary>
		<TestMethod()> _
		Public Sub QueryStatusTestNotSupportedCommandGroupTest()
			Dim target As EditorPane = editorPane
            ' Test for our local command group (treated as not supported yet).
			Dim guidCmdGroup As Guid = Guid.NewGuid()
            Dim oleCmd As New OleInterop.OLECMD()
			oleCmd.cmdf = 0
			oleCmd.cmdID = 0
            Dim prgCmds() As OleInterop.OLECMD = {oleCmd}
            Dim ptr As New IntPtr()

			Dim res As Integer = target.QueryStatus(guidCmdGroup, CUInt(1), prgCmds, ptr)
			Assert.AreEqual(CInt(Fix(OleInterop.Constants.OLECMDERR_E_NOTSUPPORTED)), res, "The QueryStatus method has not return expected value")
		End Sub

		''' <summary>
        ''' The test for QueryStatus. Case of guidEditorWithToolboxCmdSet.
		''' </summary>
		<TestMethod()> _
		Public Sub QueryStatusTestToolboxCmdSetGuidTest()
			Dim target As EditorPane = editorPane
			Dim guidCmdGroup As Guid = Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_GuidListAccessor.guidEditorCmdSet

            Dim oleCmd As New OleInterop.OLECMD()
			oleCmd.cmdf = 0
            oleCmd.cmdID = 0
            Dim prgCmds() As OleInterop.OLECMD = {oleCmd}

            Dim ptr As New IntPtr()

			Dim actual_result As Integer = target.QueryStatus(guidCmdGroup, CUInt(1), prgCmds, ptr)
			Assert.AreEqual(CInt(Fix(OleInterop.Constants.OLECMDERR_E_NOTSUPPORTED)), actual_result, "QueryStatus, Select All command testing. Method has not return expected value")
		End Sub

		''' <summary>
		''' The test for QueryStatus. Case of execution of command "SelectAll".
		''' </summary>
		<TestMethod()> _
		Public Sub QueryStatusTestSelectAllCommandTest()
			Dim target As EditorPane = editorPane
			Dim guidCmdGroup As Guid = VSConstants.GUID_VSStandardCommandSet97
            Dim oleCmd As New OleInterop.OLECMD()
            oleCmd.cmdf = 0
            ' Test for "SelectAll" command.
            oleCmd.cmdID = CUInt(VSConstants.VSStd97CmdID.SelectAll)
            Dim prgCmds() As OleInterop.OLECMD = {oleCmd}

            Dim ptr As New IntPtr()

			Dim res As Integer = target.QueryStatus(guidCmdGroup, CUInt(1), prgCmds, ptr)
			Assert.AreEqual(res, VSConstants.S_OK, "QueryStatus, Select All command testing. Method has not return expected value")
		End Sub

		''' <summary>
		''' The test for QueryStatus. Case of execution of command "Cut".
		''' </summary>
		<TestMethod()> _
		Public Sub QueryStatusTestCutCommandTest()
			Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

			Dim guidCmdGroup As Guid = VSConstants.GUID_VSStandardCommandSet97
            Dim oleCmd As New OleInterop.OLECMD()
            oleCmd.cmdf = 0
            ' Test for "Cut" command.
            oleCmd.cmdID = CUInt(VSConstants.VSStd97CmdID.Cut)
            Dim prgCmds() As OleInterop.OLECMD = {oleCmd}

            Dim ptr As New IntPtr()

            ' Here we tests "Cut" command with some selected text block.
			accessor.editorControl.Text = testString
			accessor.editorControl.Select(0, 2)

			Dim res As Integer = target.QueryStatus(guidCmdGroup, CUInt(1), prgCmds, ptr)
			Assert.AreEqual(res, VSConstants.S_OK, "QueryStatus for Cut command returned unexpected value.")
		End Sub

		''' <summary>
		''' The test for QueryStatus. Case of execution of command "Paste".
		''' </summary>
		<TestMethod()> _
		Public Sub QueryStatusTestPasteCommandTest()
			Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

			Dim guidCmdGroup As Guid = VSConstants.GUID_VSStandardCommandSet97
            Dim oleCmd As New OleInterop.OLECMD()
            oleCmd.cmdf = 0
            ' Test for "Paste" command.
            oleCmd.cmdID = CUInt(VSConstants.VSStd97CmdID.Paste)
            Dim prgCmds() As OleInterop.OLECMD = {oleCmd}

            Dim ptr As New IntPtr()

            ' Here we do something for store data to the clipboard.
            accessor.editorControl.Text = testString
            accessor.editorControl.Select(0, 2)
            accessor.editorControl.Cut()

            Dim res As Integer = target.QueryStatus(guidCmdGroup, CUInt(1), prgCmds, ptr)
            Assert.AreEqual(res, VSConstants.S_OK, "QueryStatus for Paste command returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for QueryStatus. Case of execution of command "Redo".
        ''' </summary>
        <TestMethod()> _
        Public Sub QueryStatusTestRedoCommandTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            Dim guidCmdGroup As Guid = VSConstants.GUID_VSStandardCommandSet97
            Dim oleCmd As New OleInterop.OLECMD()
            oleCmd.cmdf = 0
            ' Test for "Redo" command.
            oleCmd.cmdID = CUInt(VSConstants.VSStd97CmdID.Redo)
            Dim prgCmds() As OleInterop.OLECMD = {oleCmd}

            Dim ptr As New IntPtr()

            ' Here we do something and "Undo" it for enabling "Redo" command.
            accessor.editorControl.Text = testString
            accessor.editorControl.Select(0, 2)
            accessor.editorControl.Cut()
            accessor.editorControl.Undo()

            Dim res As Integer = target.QueryStatus(guidCmdGroup, CUInt(1), prgCmds, ptr)
            Assert.AreEqual(res, VSConstants.S_OK, "QueryStatus for Redo command returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for QueryStatus. Case of execution of command "Undo".
        ''' </summary>
        <TestMethod()> _
        Public Sub QueryStatusTestUndoCommandTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            Dim guidCmdGroup As Guid = VSConstants.GUID_VSStandardCommandSet97
            Dim oleCmd As New OleInterop.OLECMD()
            oleCmd.cmdf = 0
            ' Test for "Undo" command.
            oleCmd.cmdID = CUInt(VSConstants.VSStd97CmdID.Undo)
            Dim prgCmds() As OleInterop.OLECMD = {oleCmd}

            Dim ptr As New IntPtr()

            ' Here we execute Cut() TextBox action for enable command.
            accessor.editorControl.Text = testString
            accessor.editorControl.Select(0, 2)
            accessor.editorControl.Cut()

            Dim res As Integer = target.QueryStatus(guidCmdGroup, CUInt(1), prgCmds, ptr)
            Assert.AreEqual(VSConstants.S_OK, res, "QueryStatus for Undo command returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for Exec. Case of execution of command "Copy".
        ''' </summary>
        <TestMethod()> _
        Public Sub ExecTestCopyCommandTest()
            Dim target As EditorPane = editorPane
            ' Check whether object is of IVsWindowPane type.
            Assert.IsNotNull(TryCast(target, IVsWindowPane), "The object doesn't implement IVsWindowPane interface")

            ' Prepare component for command execution.
            PrepareComponentSelectedText()

            Dim pguidCmdGroup As Guid = VSConstants.GUID_VSStandardCommandSet97
            ' Test for Copy command.
            Dim nCmdID As UInteger = CUInt(VSConstants.VSStd97CmdID.Copy)

            Dim res As Integer = target.Exec(pguidCmdGroup, nCmdID, 0, New IntPtr(), New IntPtr())
            Assert.AreEqual(VSConstants.S_OK, res, "Exec function for Copy command returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for Exec. Case of presence of "Not supported" commands group.
        ''' </summary>
        <TestMethod()> _
        Public Sub ExecTestNotSupportedCmdGroupTest()
            Dim target As EditorPane = editorPane

            ' Check whether object is of IVsWindowPane type.
            Assert.IsNotNull(TryCast(target, IVsWindowPane), "The object doesn't implement IVsWindowPane interface")

            ' Test for Not supported group.
            Dim pguidCmdGroup As Guid = Guid.Empty
            ' Any supported command.
            Dim nCmdID As UInteger = CUInt(VSConstants.VSStd97CmdID.NewWindow)

            Dim res As Integer = target.Exec(pguidCmdGroup, nCmdID, 0, New IntPtr(), New IntPtr())
            Assert.AreEqual(CInt(Fix(OleInterop.Constants.OLECMDERR_E_UNKNOWNGROUP)), res, "Exec function for notSupported group returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for Exec. Case of a "Not Supported" command execution.
        ''' </summary>
        <TestMethod()> _
        Public Sub ExecTestNotSupportedCommandTest()
            Dim target As EditorPane = editorPane
            ' Check whether object is of IVsWindowPane type.
            Assert.IsNotNull(TryCast(target, IVsWindowPane), "The object doesn't implement IVsWindowPane interface")

            ' Any supported group.
            Dim pguidCmdGroup As Guid = VSConstants.GUID_VSStandardCommandSet97
            ' Test for Not supported command.
            Dim nCmdID As UInteger = UInteger.MinValue

            ' Checking unknown command for VS standard command set. 
            Dim res As Integer = target.Exec(pguidCmdGroup, nCmdID, 0, New IntPtr(), New IntPtr())
            Assert.AreEqual(CInt(Fix(OleInterop.Constants.OLECMDERR_E_NOTSUPPORTED)), res)

            ' Checking unknown command for editor specific command set. 
            pguidCmdGroup = Guid.NewGuid()

            res = target.Exec(pguidCmdGroup, nCmdID, 0, New IntPtr(), New IntPtr())
            Assert.AreEqual(CInt(Fix(OleInterop.Constants.OLECMDERR_E_UNKNOWNGROUP)), res, "Exec function for notSupported command returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for Exec. Case of a "Cut" command execution.
        ''' </summary>
        <TestMethod()> _
        Public Sub ExecTestCutCommandTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            ' Test for "Cut" command.
            Dim nCmdID As UInt32 = CUInt(VSConstants.VSStd97CmdID.Cut)

            ' Expected and actual RichTextBox Text property values.
            Dim expectedTextValue As String = String.Empty
            Dim actualTextValue As String = String.Empty

            ' Expected and actual return code values.
            Dim expectedReturnCode As Int32 = -1
            Dim actualReturnCode As Int32 = -1

            ' Init by standard command set.
            Dim pguidCmdGroup As Guid = VSConstants.GUID_VSStandardCommandSet97
            Dim pvaIn As New IntPtr()
            Dim pvaOut As New IntPtr()

            ' Here we start to tests the "Cut" command without selection
            ' Prepare expected values.
            expectedReturnCode = VSConstants.S_OK
            ' Store Text value before "Cut" operation.
            expectedTextValue = accessor.editorControl.Text
            ' No selection.
            accessor.editorControl.Select(0, 0)

            actualReturnCode = target.Exec(pguidCmdGroup, nCmdID, 0, pvaIn, pvaOut)

            ' Store Text value after "Cut" operation.
            actualTextValue = accessor.editorControl.Text

            Assert.AreEqual(expectedTextValue, actualTextValue, "After void Cutting operation we have unexpected Text value")
            Assert.AreEqual(expectedReturnCode, actualReturnCode, "Cut command test failed. Exec method did not return the expected value")

            ' Here we start to tests the "Cut" command with some selection.
            ' Prepare expected values.
            accessor.editorControl.Text = "One two three four five"
            expectedTextValue = "two three four five"
            expectedReturnCode = VSConstants.S_OK
            ' Select "One " substring.
            accessor.editorControl.Select(0, 4)
            actualReturnCode = target.Exec(pguidCmdGroup, nCmdID, 0, pvaIn, pvaOut)

            ' Store Text value after "Cut" operation.
            actualTextValue = accessor.editorControl.Text

            Assert.AreEqual(expectedTextValue, actualTextValue, "Invalid expected RichTextBox Text value after Cut operation")
            Assert.AreEqual(expectedReturnCode, actualReturnCode, "Cut command test failed. Exec returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for Exec. Case of a "Paste" command execution.
        ''' </summary>
        <TestMethod()> _
        Public Sub ExecTestPasteCommandTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            ' "Paste" command is tested.
            Dim nCmdID As UInteger = CUInt(VSConstants.VSStd97CmdID.Paste)

            ' Expected and actual RichTextBox Text property values.
            Dim expectedTextValue As String = String.Empty

            ' Expected and actual return code values.
            Dim expectedReturnCode As Integer = -1
            Dim actualReturnCode As Integer = -1

            ' Init by standard commands set.
            Dim pguidCmdGroup As Guid = VSConstants.GUID_VSStandardCommandSet97
            Dim pvaIn As New IntPtr()
            Dim pvaOut As New IntPtr()
            accessor.editorControl.Text = testString

            ' Here we start to tests the "Paste" command with:
            ' not the empty clipboard and with some text selected in the RichTextBox
            ' select first 5 letters.
            accessor.editorControl.Select(0, 5)
            accessor.editorControl.Cut()
            Assert.AreNotEqual(accessor.editorControl.Text, testString, "The value of editorControl.Text after Cut operation was unexpected.")

            ' Actually test paste operation:
            expectedReturnCode = VSConstants.S_OK
            ' We expect this value.
            expectedTextValue = testString

            ' NOTE: Sometimes this test is crashed during Paste() operation executing.
            ' Probably it is the internal clipboard synchronization issue.
            actualReturnCode = target.Exec(pguidCmdGroup, nCmdID, 0, pvaIn, pvaOut)

            Assert.AreEqual(expectedTextValue, accessor.editorControl.Text, "The value of editorControl.Text after Paste operation with selected text was unexpected.")
            Assert.AreEqual(expectedReturnCode, actualReturnCode, "Paste command test failed. Exec returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for Exec. Case of a "Redo" command execution.
        ''' </summary>
        <TestMethod()> _
        Public Sub ExecTestRedoCommandTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            ' Test for "Redo" command.
            Dim nCmdID As UInt32 = CUInt(VSConstants.VSStd97CmdID.Redo)

            ' Expected and actual RichTextBox Text property values.
            Dim expectedTextValue As String = String.Empty
            Dim actualTextValue As String = String.Empty

            ' Expected and actual return code values.
            Dim expectedReturnCode As Int32 = -1
            Dim actualReturnCode As Int32 = -1

            ' Init by standard command set.
            Dim pguidCmdGroup As Guid = VSConstants.GUID_VSStandardCommandSet97

            Dim pvaIn As New IntPtr()
            Dim pvaOut As New IntPtr()

            ' Here we start to tests the "Redo" command
            ' prepare expected values.
            expectedReturnCode = VSConstants.S_OK
            expectedTextValue = String.Empty

            accessor.editorControl.Text = testString

            ' Perform text-distortion operation.
            accessor.editorControl.SelectAll()
            accessor.editorControl.Cut()
            Assert.IsTrue(accessor.editorControl.Text.Length = 0, "Delete all operation was not completed successfully.")

            accessor.editorControl.Undo()
            ' Is Redo command are enabled?
            Assert.AreEqual(accessor.editorControl.Text, testString, False, "Undo operation leaded to unexpected result.")

            ' Perform Redo operation.
            actualReturnCode = target.Exec(pguidCmdGroup, nCmdID, 0, pvaIn, pvaOut)

            ' Store Text value after "Redo" operation.
            ' Empty string is expected.
            actualTextValue = accessor.editorControl.Text

            Assert.AreEqual(expectedTextValue, actualTextValue, "After Redo operation we have not empty string (Redo Clear operation scenario)")
            Assert.AreEqual(expectedReturnCode, actualReturnCode, "Redo command test failed. Exec returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for Exec. Case of a "Undo" command execution.
        ''' </summary>
        <TestMethod()> _
        Public Sub ExecTestUndoCommandTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            ' "Undo" command is tested.
            Dim nCmdID As UInt32 = CUInt(VSConstants.VSStd97CmdID.Undo)
            ' Expected and actual RichTextBox Text property values.
            Dim expectedTextValue As String = String.Empty
            Dim actualTextValue As String = String.Empty

            ' Containers for function return code.
            Dim expectedReturnCode As Int32 = -1
            Dim actualReturnCode As Int32 = -1

            Dim pguidCmdGroup As New Guid()
            ' Init by standard command set.
            pguidCmdGroup = VSConstants.GUID_VSStandardCommandSet97

            Dim pvaIn As New IntPtr()
            Dim pvaOut As New IntPtr()

            ' Here we start to tests the "Undo" command.
            ' Prepare expected values.
            expectedReturnCode = VSConstants.S_OK

            Dim textBeforeChanges As String = "Original RichTextBox text"
            expectedTextValue = textBeforeChanges

            ' Save text value before text-distortion operation.
            accessor.editorControl.Text = textBeforeChanges

            ' Execute test routine context.
            ' Perform text-distortion operation.
            accessor.editorControl.SelectAll()
            accessor.editorControl.Cut()

            Assert.IsTrue(accessor.editorControl.Text.Length = 0, "Delete all operation was not completed successfully.")

            ' Perform Undo operation for recover original text value.
            actualReturnCode = target.Exec(pguidCmdGroup, nCmdID, 0, pvaIn, pvaOut)

            ' Store Text value after "Undo" operation.
            actualTextValue = accessor.editorControl.Text
            Assert.AreEqual(expectedTextValue, actualTextValue, "After Undo operation we have not recovered original text value")
            Assert.AreEqual(expectedReturnCode, actualReturnCode, "Undo command test failed. Exec returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for Exec. Case of a "Select All" command execution.
        ''' </summary>
        <TestMethod()> _
        Public Sub ExecTestSelectAllCommandTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            ' Test for "SelectAll" command.
            Dim nCmdID As UInt32 = CUInt(VSConstants.VSStd97CmdID.SelectAll)

            ' Expected and actual text property values.
            Dim expectedSelectionLength As Int32 = -1
            Dim actualSelectionLength As Int32 = -1

            ' Expected and actual return code values.
            Dim expectedReturnCode As Int32 = -1
            Dim actualReturnCode As Int32 = -1

            Dim pguidCmdGroup As New Guid()
            ' Init by standard command set.
            pguidCmdGroup = VSConstants.GUID_VSStandardCommandSet97

            Dim pvaIn As New IntPtr()
            Dim pvaOut As New IntPtr()

            ' Here we start to tests the "SelectAll" command.
            ' Prepare some text in RichTextBox.
            Dim testSelectionString As String = "It's some text for test SelectAll feature"
            accessor.editorControl.Text = testSelectionString

            ' Prepare expected values.
            expectedReturnCode = VSConstants.S_OK
            expectedSelectionLength = testSelectionString.Length

            ' Execute test routine context.
            actualReturnCode = target.Exec(pguidCmdGroup, nCmdID, 0, pvaIn, pvaOut)

            actualSelectionLength = accessor.editorControl.SelectionLength

            Assert.AreEqual(expectedSelectionLength, actualSelectionLength, "After SelectAll command not all text was selected")
            Assert.AreEqual(expectedReturnCode, actualReturnCode, "SelectAll command test failed. Exec returned expected value.")
        End Sub

        ''' <summary>
        ''' The test for Exec. Scenario when pguidCmdGroup == GuidList.guidEditorCmdSet.
        ''' </summary>
        <TestMethod()> _
        Public Sub ExecTestGuidEditorCmdGroupTest()
            Dim target As EditorPane = editorPane
            ' Check whether object is of IVsWindowPane type.
            Assert.IsNotNull(TryCast(target, IVsWindowPane), "The object doesn't implement IVsWindowPane interface")

            Dim pguidCmdGroup As Guid = Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_GuidListAccessor.guidEditorCmdSet
            ' Test for Not supported command.
            Dim nCmdID As UInteger = UInteger.MinValue

            ' Checking unknown command for VS standard command set. 
            Dim res As Integer = target.Exec(pguidCmdGroup, nCmdID, 0, New IntPtr(), New IntPtr())
            Assert.AreEqual(CInt(Fix(OleInterop.Constants.OLECMDERR_E_NOTSUPPORTED)), res)
        End Sub

#End Region

#Region "IPersist tests"

        ''' <summary>
        ''' A test for GetClassID() method.
        ''' </summary>
        <TestMethod()> _
        Public Sub IPersistGetClassIdTest()
            Dim target As EditorPane = editorPane
            Dim pClassId As Guid

            Dim actual_result As Integer = (CType(target, IPersist)).GetClassID(pClassId)
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method IPersist.GetClassID() returned unexpected result")
            Assert.IsNotNull(pClassId, "pClassId are not assigned")
        End Sub

#End Region

#Region "IVsPersistDocData tests"

        ''' <summary>
        ''' The test for IsDocDataDirty() method.
        ''' </summary>
        <TestMethod()> _
        Public Sub IsDocDataDirtyTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            Dim pfDirty As Integer
            Dim fExpectedDirtyValue As Integer

            If accessor.isDirty Then
                fExpectedDirtyValue = (1)
            Else
                fExpectedDirtyValue = (0)
            End If

            Dim actual_result As Integer = (CType(target, IVsPersistDocData)).IsDocDataDirty(pfDirty)
            Assert.AreEqual(fExpectedDirtyValue, pfDirty, String.Format("IsDocDataDirty() returned unexpected pfDirty value, expected are {0}", fExpectedDirtyValue))
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method IsDocDataDirty() returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for LoadDocData() method.
        ''' </summary>
        <TestMethod()> _
        Public Sub LoadDocDataTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            Dim pszMkDocument As String = accessor.fileName
            Dim actual_result As Integer = (CType(target, IVsPersistDocData)).LoadDocData(pszMkDocument)
            Assert.AreEqual(VSConstants.S_OK, actual_result, String.Format("LoadDocData() returned unexpected Document value, expected are {0}", VSConstants.S_OK))
        End Sub

        ''' <summary>
        ''' The test for SetUntitledDocPath() method.
        ''' </summary>
        <TestMethod()> _
        Public Sub SetUntitledDocPathTest()
            Dim target As EditorPane = editorPane

            Dim pszDocPath As String = "some invalid path"
            Dim actual_result As Integer = (CType(target, IVsPersistDocData)).SetUntitledDocPath(pszDocPath)
            Assert.AreEqual(VSConstants.S_OK, actual_result, "LoadDocData() returned unexpected Document value, expected are S_OK")
        End Sub

        ''' <summary>
        ''' The test for Close() method.
        ''' </summary>
        <TestMethod()> _
        Public Sub IPersistDocDataCloseTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            accessor.editorControl = New EditorControl()
            Dim actual_result As Integer = (CType(target, IVsPersistDocData)).Close()
            Assert.IsTrue(accessor.editorControl.IsDisposed, "EditorPane Text Box instance was not Disposed properly after Close() method execution.")
            Assert.AreEqual(VSConstants.S_OK, actual_result, "IVsPersistDocData.Close() returned unexpected result.")
        End Sub

        ''' <summary>
        ''' The test for GetGuidEditorType() method.
        ''' </summary>
        <TestMethod()> _
        Public Sub GetGuidEditorTypeTest()
            Dim target As EditorPane = editorPane
            ' Create empty guid instance.
            Dim pClassID As Guid = Guid.Empty

            Dim actual_result As Integer = (CType(target, IVsPersistDocData)).GetGuidEditorType(pClassID)
            Assert.IsTrue((pClassID <> Guid.Empty), "Editor type Guid was not initialized.")
            Assert.AreEqual(VSConstants.S_OK, actual_result, "IVsPersistDocData.Close() returned unexpected result.")
        End Sub

        ''' <summary>
        ''' The test for IsDocDataReloadable() method.
        ''' </summary>
        <TestMethod()> _
        Public Sub IsDocDataCanReloadTest()
            Dim target As EditorPane = editorPane
            ' Create uninitialized.
            Dim pfReloadable As Integer

            Dim actual_result As Integer = (CType(target, IVsPersistDocData)).IsDocDataReloadable(pfReloadable)
            Assert.IsTrue(pfReloadable <> 0, "pfReloadable was not properly initialized")
            Assert.AreEqual(VSConstants.S_OK, actual_result, "IVsPersistDocData.IsDocDataReloadable() returned unexpected result.")
        End Sub

        ''' <summary>
        ''' The test for RenameDocData() method.
        ''' </summary>
        <TestMethod()> _
        Public Sub RenameDocDataTest()
            Dim target As EditorPane = editorPane
            Dim gfrAttrin As UInteger = 0, itemidNew As UInteger = 0
            Dim pHierNew As Object = TryCast(New Object, IVsHierarchy)
            Dim pszMkDocumentNew As String = "New Document"

            Dim actual_result As Integer = (CType(target, IVsPersistDocData)).RenameDocData(gfrAttrin, CType(pHierNew, IVsHierarchy), itemidNew, pszMkDocumentNew)
            Assert.AreEqual(VSConstants.S_OK, actual_result, "IVsPersistDocData.RenameDocData() returned unexpected result.")
        End Sub

        ''' <summary>
        ''' The test for ReloadDocData() method.
        ''' </summary>
        <TestMethod()> _
        Public Sub ReloadDocDataTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            Dim grfFlags As UInteger = 0

            Dim actual_result As Integer = (CType(target, IVsPersistDocData)).ReloadDocData(grfFlags)
            Assert.IsFalse(accessor.isDirty, "IsDirty flag was not reseted after document data reloading.")
            Assert.AreEqual(VSConstants.S_OK, actual_result, "IVsPersistDocData.ReloadDocData() returned unexpected result.")
        End Sub

        ''' <summary>
        ''' The test for OnRegisterDocData() method.
        ''' </summary>
        <TestMethod()> _
        Public Sub OnRegisterDocData()
            Dim target As EditorPane = editorPane
            Dim docCookie As UInteger = 0, itemidNew As UInteger = 0
            Dim pHierNew As Object = TryCast(New Object, IVsHierarchy)

            Dim actual_result As Integer = (CType(target, IVsPersistDocData)).OnRegisterDocData(docCookie, CType(pHierNew, IVsHierarchy), itemidNew)
            Assert.AreEqual(VSConstants.S_OK, actual_result, "IVsPersistDocData.OnRegisterDocData() returned unexpected result.")
        End Sub

        ''' <summary>
        ''' The test for SaveDocData() method.
        ''' </summary>
        <TestMethod()> _
        Public Sub SaveDocDataTest()
            CType(editorPane, IPersistFileFormat).Load(filePath, 0, 0)

            Dim appendText As String = "Text Appended from the Editor"
            editorPaneAccessor.editorControl.Text &= appendText

            Dim saveCanceled As Integer = 0

            ' Save functionality.
            Assert.AreEqual(VSConstants.S_OK, (CType(editorPane, IVsPersistDocData)).SaveDocData(VSSAVEFLAGS.VSSAVE_Save, filePath, saveCanceled))
            Assert.AreEqual(VSConstants.S_OK, VerifyFileContents(filePath, filePath, editorPaneAccessor.editorControl.Text))

            ' Save As functionality.
            Dim newFilePath As String = String.Empty
            Assert.AreEqual(VSConstants.S_OK, (CType(editorPane, IVsPersistDocData)).SaveDocData(VSSAVEFLAGS.VSSAVE_SaveAs, newFilePath, saveCanceled))
            Assert.AreEqual(VSConstants.S_OK, VerifyFileContents(filePath, newFilePath, editorPaneAccessor.editorControl.Text))

            ' SaveCopyAs Functionality.
            Assert.AreEqual(VSConstants.S_OK, (CType(editorPane, IVsPersistDocData)).SaveDocData(VSSAVEFLAGS.VSSAVE_SaveCopyAs, newFilePath, saveCanceled))
            Assert.AreEqual(VSConstants.S_OK, VerifyFileContents(filePath, newFilePath, editorPaneAccessor.editorControl.Text))

            ' SilentSave functionality.
            Assert.AreEqual(VSConstants.S_OK, (CType(editorPane, IVsPersistDocData)).SaveDocData(VSSAVEFLAGS.VSSAVE_SilentSave, filePath, saveCanceled))
            Assert.AreEqual(VSConstants.S_OK, VerifyFileContents(filePath, filePath, editorPaneAccessor.editorControl.Text))
        End Sub

#End Region

#Region "IPersistFileFormat tests"
        ''' <summary>
        ''' The test for SaveCompleted() method.
        ''' </summary>
        <TestMethod()> _
        Public Sub SaveCompletedTest()
            Dim target As EditorPane = editorPane

            Dim pszFileName As String = "SomeFileName.Txt"
            Dim actual_result As Integer = (CType(target, IPersistFileFormat)).SaveCompleted(pszFileName)
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method SaveCompleted() returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for GetCurFile() method.
        ''' </summary>
        <TestMethod()> _
        Public Sub GetCurFileTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            ' Create uninitialized.
            Dim pszFileName As String = Nothing
            Dim pnFormatIndex As UInteger

            Dim actual_result As Integer = (CType(target, IPersistFileFormat)).GetCurFile(pszFileName, pnFormatIndex)
            Assert.AreEqual(accessor.fileName, pszFileName, "Returned file name value is unexpected.")
            Assert.AreEqual(Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor.fileFormat, pnFormatIndex, "Returned format index value is unexpected.")

            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method GetCurFile() returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for InitNew() method in case of invalid specified format value.
        ''' </summary>
        <TestMethod(), ExpectedException(GetType(ArgumentException))> _
        Public Sub InitNewWithInvalidFormatTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            ' Prepare format index, which are not equivalent with myFormat value
            ' and expect exception occurrence.
            Dim pnFormatIndex As UInteger = 1UI + Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor.fileFormat

            Dim actual_result As Integer = (CType(target, IPersistFileFormat)).InitNew(pnFormatIndex)

            Assert.IsFalse(accessor.isDirty, "Dirty flag is not set to true")
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method InitNew() returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for InitNew() method in case of correct specified format value.
        ''' </summary>
        <TestMethod()> _
        Public Sub InitNewWithCorrectFormatTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            ' Prepare format index, which are equivalent with myFormat value.
            Dim pnFormatIndex As UInteger = Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor.fileFormat

            Dim actual_result As Integer = (CType(target, IPersistFileFormat)).InitNew(pnFormatIndex)

            Assert.IsFalse(accessor.isDirty, "Dirty flag is not set to true")
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method InitNew() returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for GetClassID() method.
        ''' </summary>
        <TestMethod()> _
        Public Sub GetClassIdTest()
            Dim target As EditorPane = editorPane

            Dim pClassId As Guid
            Dim pExpectedClassID As Guid

            CType(target, IPersist).GetClassID(pExpectedClassID)
            Dim actual_result As Integer = (CType(target, IPersistFileFormat)).GetClassID(pClassId)
            Assert.AreEqual(pExpectedClassID, pClassId, "Class ID was initialized by unexpected Guid value.")

            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method GetClassID() returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for GetFormatList() method.
        ''' </summary>
        <TestMethod()> _
        Public Sub GetFormatListTest()
            Dim target As EditorPane = editorPane
            Dim ppszFormatList As String = Nothing

            Dim actual_result As Integer = (CType(target, IPersistFileFormat)).GetFormatList(ppszFormatList)
            Assert.IsNotNull(ppszFormatList, "Format List was not initialized.")
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method GetFormatList() returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for IsDirty() method when isDirty is true.
        ''' </summary>
        <TestMethod()> _
        Public Sub IsDirtyTestWithDirtyTrueTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            Dim pfIsDirty As Integer
            accessor.isDirty = True
            Dim actual_result As Integer = (CType(target, IPersistFileFormat)).IsDirty(pfIsDirty)
            Assert.AreEqual(pfIsDirty, 1, "IsDirty returned unexpected value of isDirty flag")
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method IsDirty() returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for IsDirty() method when isDirty is false.
        ''' </summary>
        <TestMethod()> _
        Public Sub IsDirtyTestWithDirtyFalseTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            Dim pfIsDirty As Integer
            accessor.isDirty = False
            Dim actual_result As Integer = (CType(target, IPersistFileFormat)).IsDirty(pfIsDirty)
            Assert.AreEqual(pfIsDirty, 0, "IsDirty returned unexpected value of isDirty flag")
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method IsDirty() returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for Load() method in case of invalid input file name argument.
        ''' </summary>
        <TestMethod(), ExpectedException(GetType(ArgumentNullException))> _
        Public Sub IPersistFileFormatLoadWithInvalidFileNameTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)
            Dim pszFileName As String = Nothing
            accessor.fileName = Nothing
            Dim grfMode As UInteger = 0
            Dim fReadOnly As Integer = 0

            Dim actual_result As Integer = (CType(target, IPersistFileFormat)).Load(pszFileName, grfMode, fReadOnly)
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method Load() returned unexpected value.")
        End Sub

        ''' <summary>
        ''' The test for Load() method in scenario with notification about reloading.
        ''' </summary>
        <TestMethod()> _
        Public Sub IPersistFileFormatLoadWithReloadingTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            Dim pszFileName As String = Nothing
            Dim grfMode As UInteger = 0
            Dim fReadOnly As Integer = 0
            Dim actual_result As Integer = (CType(target, IPersistFileFormat)).Load(pszFileName, grfMode, fReadOnly)

            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method Load() returned unexpected value.")
            Assert.IsFalse(accessor.isDirty, "isDirty flag must be false after successful file loading.")
        End Sub

        ''' <summary>
        ''' The test for Load() method in scenario with notification about loading.
        ''' </summary>
        <TestMethod()> _
        Public Sub IPersistFileFormatLoadWithoutReloadingTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            Dim pszFileName As String = accessor.fileName
            Dim grfMode As UInteger = 0
            Dim fReadOnly As Integer = 0
            Dim actual_result As Integer = (CType(target, IPersistFileFormat)).Load(pszFileName, grfMode, fReadOnly)

            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method Load() returned unexpected value.")
            Assert.IsFalse(accessor.isDirty, "isDirty flag must be false after successful file loading.")
        End Sub

        ''' <summary>
        ''' The test for Save() method with equivalent internal and specified file names.
        ''' </summary>
        <TestMethod()> _
        Public Sub IPersistFileFormatSaveWithSameNameTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            ' Assign to the argument same as the internal file name.
            Dim pszFileName As String = accessor.fileName
            Dim uFormatIndex As UInteger = 0
            Dim fRemember As Integer = 0
            Dim actual_result As Integer = (CType(target, IPersistFileFormat)).Save(pszFileName, fRemember, uFormatIndex)

            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method Save() returned unexpected value.")
            Assert.IsFalse(accessor.isDirty, "isDirty flag must be false after successful file loading.")
        End Sub

        ''' <summary>
        ''' The test for Save() method in case of different internal file name. 
        ''' and specified in input argument.
        ''' </summary>
        <TestMethod()> _
        Public Sub IPersistFileFormatSaveWithAnotherNameTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            ' Assign to the argument same as the internal file name.
            Dim pszFileName As String = "SomeFileName.Ext"
            Dim uFormatIndex As UInteger = 0
            Dim fRemember As Integer = 0
            Dim actual_result As Integer = (CType(target, IPersistFileFormat)).Save(pszFileName, fRemember, uFormatIndex)

            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method Save() returned unexpected value.")
            Assert.IsFalse(accessor.isDirty, "isDirty flag must be false after successful file loading.")
        End Sub

        ''' <summary>
        ''' The test for Save() method in scenario when file name 
        ''' was reassigned to the specified value.
        ''' </summary>
        <TestMethod()> _
        Public Sub IPersistFileFormatSaveWithReassignFileNameTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            ' Assign to the argument same as the internal file name.
            Dim pszFileName As String = "SomeFileName.Ext"
            Dim uFormatIndex As UInteger = 0
            ' Reassign file name.
            Dim fRemember As Integer = 1
            Dim actual_result As Integer = (CType(target, IPersistFileFormat)).Save(pszFileName, fRemember, uFormatIndex)

            Assert.AreEqual(accessor.fileName, pszFileName, "FileName was not reassigned in Save() with switched fRemember option.")
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method Save() returned unexpected value.")
            Assert.IsFalse(accessor.isDirty, "isDirty flag must be false after successful file loading.")
        End Sub

#End Region

#Region "IVsToolboxUser tests"
        ''' <summary>
        ''' The test for IsSupported() method in case when ToolboxItemData is not included.
        ''' in object data.
        ''' </summary>
        <TestMethod()> _
        Public Sub IsSupportedWithoutToolboxItemDataTest()
            Dim target As EditorPane = editorPane

            Dim dataObject As New OleDataObject()
            Dim actual_result As Integer = (CType(target, IVsToolboxUser)).IsSupported(dataObject)
            Assert.AreEqual(VSConstants.S_FALSE, actual_result, "Method IsSupported() returned unexpected result")
        End Sub

        ''' <summary>
        ''' The test for IsSupported() method in case when ToolboxItemData is included.
        ''' in object data.
        ''' </summary>
        <TestMethod()> _
        Public Sub IsSupportedWithToolboxItemDataTest()
            Dim target As EditorPane = editorPane

            Dim dataObject As New OleDataObject()
            Dim toolboxdata As New ToolboxItemData("test sentence")
            dataObject.SetData(toolboxdata)
            Dim actual_result As Integer = (CType(target, IVsToolboxUser)).IsSupported(dataObject)
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method IsSupported() returned unexpected result")
        End Sub

        ''' <summary>
        ''' The test for ItemPicked() method in case when ToolboxItemData is included.
        ''' in object.
        ''' </summary>
        <TestMethod()> _
        Public Sub ItemPickedTest()
            Dim target As EditorPane = editorPane
            Dim dataObject As New OleDataObject()
            Dim toolboxdata As New ToolboxItemData("test sentence")

            dataObject.SetData(toolboxdata)
            Dim actual_result As Integer = (CType(target, IVsToolboxUser)).ItemPicked(dataObject)
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Method IsSupported() returned unexpected result")
        End Sub
#End Region

#Region "Event handlers tests"
        ''' <summary>
        ''' The test for OnDragEnter() event handler.
        ''' </summary>
        <TestMethod()> _
        Public Sub EditorControlDragEnterTest()
            Dim target As EditorPane = editorPane

            Dim accessor As New Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests.Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)
            ' Any sender.
            Dim sender As Object = Nothing
            Dim dataObject As New OleDataObject()
            Dim toolboxdata As New ToolboxItemData("test sentence")
            dataObject.SetData(toolboxdata)

            Dim e As New DragEventArgs(dataObject, 0, 0, 0, DragDropEffects.All, DragDropEffects.None)

            accessor.OnDragEnter(sender, e)
            Assert.AreEqual(DragDropEffects.Copy, e.Effect, "Drag effect was not properly initialized.")
        End Sub

        ''' <summary>
        ''' The test for OnDragDrop() event handler.
        ''' </summary>
        <TestMethod()> _
        Public Sub EditorControlDragDropTest()
            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests.Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            Dim sender As Object = target

            Dim toolBoxText As String = "TollBoxItem"
            Dim dataObject As New OleDataObject()
            Dim toolboxdata As New ToolboxItemData(toolBoxText)
            dataObject.SetData(toolboxdata)

            Dim e As New DragEventArgs(dataObject, 0, 0, 0, DragDropEffects.All, DragDropEffects.Copy)

            accessor.OnDragDrop(sender, e)

            Assert.IsTrue(accessor.editorControl.Text.IndexOf(toolBoxText) >= 0, "Drop operation was not successful")
        End Sub
#End Region

#Region "Other methods tests"

        ''' <summary>
        ''' The test for CanEditFile() method.
        ''' </summary>
        <TestMethod()> _
        Public Sub CanEditFileTest()
            Dim target As EditorPane = editorPane ' the component is already sited
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)
            Dim expected As Boolean = True
            Dim actual As Boolean

            actual = accessor.CanEditFile()

            Assert.AreEqual(expected, actual, "EditorPane.CanEditFile did " & "not return the expected value.")

            accessor.gettingCheckoutStatus = True
            expected = False
            actual = accessor.CanEditFile()

            Assert.AreEqual(expected, actual, "EditorPane.CanEditFile did " & "not return the expected value.")
        End Sub

        ''' <summary>
        ''' The test for NotifyDocChanged() method.
        ''' </summary>
        <TestMethod()> _
        Public Sub NotifyDocChangedTest()
            Dim target As EditorPane = editorPane 'Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor.CreatePrivate();
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            ' Initialize by null, valid value was assigned into the function.
            accessor.NotifyDocChanged()
        End Sub
#End Region
#End Region

#Region "Service functions"
        ''' <summary>
        ''' Compare the contents of a file against the specified text.
        ''' </summary>
        ''' <returns>S_OK if method was succeeds, otherwise E_FAIL.</returns>
        Public Function VerifyFileContents(ByVal oldFilePath As String, ByVal newFilePath As String, ByVal text As String) As Integer
            Dim target As EditorPane = editorPane

            ' Load the new file.
            CType(target, IPersistFileFormat).Load(newFilePath, 0, 0)

            Dim hr As Integer = VSConstants.E_FAIL

            ' Verify the contents.
            If editorPaneAccessor.editorControl.Text.Equals(text) Then
                hr = VSConstants.S_OK
            End If

            ' Load the old file.
            CType(target, IPersistFileFormat).Load(oldFilePath, 0, 0)
            Return hr
        End Function

        ''' <summary>
        ''' Prepare some text in text box with some selection.
        ''' </summary>
        Public Sub PrepareComponentSelectedText()
            Assert.IsNotNull(editorPane, "editorPane null reference assertion failed")

            Dim target As EditorPane = editorPane
            Dim accessor As New Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPaneAccessor(target)

            ' "this is a test string."
            accessor.editorControl.Text = testString
            accessor.editorControl.SelectionStart = 2
            ' First "is" substring is selected.
            accessor.editorControl.SelectionLength = 2
        End Sub

        ''' <summary>
        ''' Create a TempFile for loading purposes.
        ''' </summary>
        Public Sub CreateTempFile()
            If File.Exists(filePath) Then
                File.Delete(filePath)
            End If
            Dim sw As StreamWriter = File.AppendText(filePath)
            sw.Write(testString)
            sw.Close()
        End Sub

        ''' <summary>
        ''' Drop TempFile.
        ''' </summary>
        Public Sub DropTempFile()
            If File.Exists(filePath) Then
                File.Delete(filePath)
            End If
        End Sub

        ''' <summary>
        ''' Add some basic service mock objects to the service provider.
        ''' </summary>
        ''' <param name="serviceProvider"></param>
        Public Shared Sub AddBasicSiteSupport(ByVal serviceProvider As OleServiceProvider)
            If serviceProvider Is Nothing Then
                Throw New ArgumentNullException("serviceProvider")
            End If

            ' Add site support for UI Shell.
            Dim uiShell As BaseMock = MockServicesProvider.GetUiShellInstance0()
            serviceProvider.AddService(GetType(SVsUIShell), uiShell, False)
            serviceProvider.AddService(GetType(SVsUIShellOpenDocument), CType(uiShell, IVsUIShellOpenDocument), False)

            ' Add site support for Running Document Table.
            Dim runningDoc As BaseMock = MockServicesProvider.GetRunningDocTableInstance()
            serviceProvider.AddService(GetType(SVsRunningDocumentTable), runningDoc, False)

            ' Add site support for IVsTextManager.
            Dim queryEditQuerySave As BaseMock = MockServicesProvider.GetQueryEditQuerySaveInstance()
            serviceProvider.AddService(GetType(SVsQueryEditQuerySave), queryEditQuerySave, False)

            Dim toolbox As BaseMock = MockIVsToolbox.GetIVsToolboxInstance()
            serviceProvider.AddService(GetType(SVsToolbox), toolbox, False)
        End Sub
#End Region
	End Class
End Namespace
