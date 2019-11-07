'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************


Imports Microsoft.VisualBasic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System
Imports System.Text
Imports System.Collections.Generic
Imports Microsoft.Samples.VisualStudio.CodeSweep.Scanner
Imports Microsoft.VisualStudio.Shell.Interop
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell
Imports System.ComponentModel.Design

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    ''' <summary>
    ''' This is a test class for CodeSweep.VSPackage.TaskProvider and is intended
    ''' to contain all CodeSweep.VSPackage.TaskProvider Unit Tests.
    ''' </summary>
    <TestClass()> _
    Public Class TaskProviderTest


        Private testContextInstance As TestContext

        ''' <summary>
        ''' Gets or sets the test context which provides
        ''' information about and functionality for the current test run.
        ''' </summary>
        Public Property TestContext() As TestContext
            Get
                Return testContextInstance
            End Get
            Set(ByVal value As TestContext)
                testContextInstance = value
            End Set
        End Property
#Region "Additional test attributes"
        ' 
        'You can use the following additional attributes as you write your tests:
        '
        'Use ClassInitialize to run code before running the first test in the class
        '
        Private Shared _serviceProvider As MockServiceProvider
        <ClassInitialize()> _
        Public Shared Sub MyClassInitialize(ByVal testContext As TestContext)
            If CodeSweep.VSPackage.Factory_Accessor.ServiceProvider Is Nothing Then
                _serviceProvider = New MockServiceProvider()
                CodeSweep.VSPackage.Factory_Accessor.ServiceProvider = _serviceProvider
            Else
                _serviceProvider = TryCast(CodeSweep.VSPackage.Factory_Accessor.ServiceProvider, MockServiceProvider)
            End If
        End Sub

        'Use ClassCleanup to run code after all tests in a class have run
        '
        '[ClassCleanup()]
        'public static void MyClassCleanup()
        '{
        '}
        '
        'Use TestInitialize to run code before running each test
        '
        '[TestInitialize()]
        'public void MyTestInitialize()
        '{
        '}
        '
        'Use TestCleanup to run code after each test has run
        '
        <TestCleanup()> _
        Public Sub MyTestCleanup()
            Utilities.CleanUpTempFiles()
            Utilities.RemoveCommandHandlers(_serviceProvider)

            Dim taskList As MockTaskList = TryCast(_serviceProvider.GetService(GetType(SVsTaskList)), MockTaskList)
            taskList.Clear()

            CodeSweep.VSPackage.Factory_Accessor._taskProvider = Nothing
        End Sub
        '
#End Region

        ' Keep this in sync with the one in task.vb.
        Public Enum TaskFields
            Priority
            PriorityNumber
            Term
            [Class]
            Replacement
            Comment
            File
            Line
            Project
        End Enum

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub AddDuplicateTermsAndVerifyWarningAppearsOnce()
            Dim accessor As New CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider)

            Dim table As New MockTermTable("termtable.xml")
            Dim term0 As New MockTerm("dupText", 0, "term0Class", "term0Comment", "term0recommended", table)
            Dim term1 As New MockTerm("dupText", 3, "term1Class", "term1Comment", "term1recommended", table)
            Dim term2 As New MockTerm("term2Text", 2, "term2Class", "term2Comment", "term2recommended", table)
            Dim hit0 As New MockScanHit("file0", 1, 10, "line text", term0, "warning 1")
            Dim hit1 As New MockScanHit("file1", 4, 1, "line text 2", term1, "warning 2")
            Dim hit2 As New MockScanHit("file2", 3, 2, "line text 3", term2, "warning 3")
            Dim scanResult As New MockScanResult("file0", New IScanHit() {hit0, hit1, hit2}, True)
            accessor.AddResult(scanResult, "c:\projFile")

            Dim enumerator As IVsEnumTaskItems = Nothing
            Dim hr As Integer = accessor.EnumTaskItems(enumerator)

            Assert.AreEqual(VSConstants.S_OK, hr, "EnumTaskItems returned wrong hresult.")

            ' Verify there are three hits plus two warnings.
            Dim tasks As List(Of IVsTaskItem) = Utilities.TasksFromEnumerator(enumerator)
            Assert.AreEqual(5, tasks.Count, "EnumTaskItems did not enumerate correct number of tasks.")

            Dim termValues As String() = New String(4) {"dupText", "dupText", "dupText", "term2Text", "term2Text"}
            Dim priorityValues As Integer() = New Integer(4) {0, 0, 3, 2, 2}
            Dim classValues As String() = New String(4) {"term0Class", "term0Class", "term1Class", "term2Class", "term2Class"}
            Dim replacementValues As String() = New String(4) {"", "term0recommended", "term1recommended", "", "term2recommended"}
            Dim commentValues As String() = New String(4) {"warning 1", "term0Comment", "term1Comment", "warning 3", "term2Comment"}
            Dim fileValues As String() = New String(4) {"", "file0", "file1", "", "file2"}

            For taskIndex As Integer = 0 To tasks.Count - 1
                Dim type As UInteger
                Dim val As Object = Nothing
                Dim flags As UInteger
                Dim acc As String = String.Empty

                hr = (TryCast(tasks(taskIndex), IVsTaskItem3)).GetColumnValue(CInt(Fix(TaskFields.Term)), type, flags, val, acc)
                Assert.AreEqual(VSConstants.S_OK, hr)
                Assert.AreEqual(__VSTASKVALUETYPE.TVT_TEXT, CType(type, __VSTASKVALUETYPE), "Task " & taskIndex.ToString() & " term type is wrong.")
                Assert.AreEqual(termValues(taskIndex), CStr(val), "Task " & taskIndex.ToString() & " term text is wrong.")

                hr = (TryCast(tasks(taskIndex), IVsTaskItem3)).GetColumnValue(System.Convert.ToInt32(TaskFields.Comment), type, flags, val, acc)
                Assert.AreEqual(VSConstants.S_OK, hr)
                Assert.AreEqual(__VSTASKVALUETYPE.TVT_LINKTEXT, CType(type, __VSTASKVALUETYPE), "Task " & taskIndex.ToString() & " comment type is wrong.")
                Assert.AreEqual(commentValues(taskIndex), CStr(val), "Task " & taskIndex.ToString() & " comment text is wrong.")

                hr = (TryCast(tasks(taskIndex), IVsTaskItem3)).GetColumnValue(CInt(Fix(TaskFields.Replacement)), type, flags, val, acc)
                Assert.AreEqual(VSConstants.S_OK, hr)
                Assert.AreEqual(__VSTASKVALUETYPE.TVT_TEXT, CType(type, __VSTASKVALUETYPE), "Task " & taskIndex.ToString() & " replacement type is wrong.")
                Assert.AreEqual(replacementValues(taskIndex), CStr(val), "Task " & taskIndex.ToString() & " replacement text is wrong.")
            Next taskIndex
        End Sub
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub AddResultsThenClear()
            Dim accessor As New CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider)

            ' Add some tasks
            Dim table As New MockTermTable("termtable.xml")
            Dim term0 As New MockTerm("term0Text", 0, "term0Class", "term0Comment", "term0recommended", table)
            Dim term1 As New MockTerm("term1Text", 3, "term1Class", "term1Comment", "term1recommended", table)
            Dim hit0 As New MockScanHit("file0", 1, 10, "line text", term0, Nothing)
            Dim hit1 As New MockScanHit("file1", 4, 1, "line text 2", term1, Nothing)
            Dim scanResult As New MockScanResult("file0", New IScanHit() {hit0, hit1}, True)
            accessor.AddResult(scanResult, "c:\projFile")

            Dim enumerator As IVsEnumTaskItems = Nothing
            Dim hr As Integer = accessor.EnumTaskItems(enumerator)

            Assert.AreEqual(VSConstants.S_OK, hr, "EnumTaskItems returned wrong hresult.")

            Dim tasks As List(Of IVsTaskItem) = Utilities.TasksFromEnumerator(enumerator)
            Assert.AreEqual(2, tasks.Count, "EnumTaskItems did not enumerate correct number of tasks.")

            accessor.Clear()

            hr = accessor.EnumTaskItems(enumerator)

            Assert.AreEqual(VSConstants.S_OK, hr, "EnumTaskItems returned wrong hresult.")

            tasks = Utilities.TasksFromEnumerator(enumerator)
            Assert.AreEqual(0, tasks.Count, "EnumTaskItems did not enumerate correct number of tasks after Clear().")
        End Sub

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub TestEnumerator()
            Dim accessor As New CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider)

            ' Add some tasks
            Dim table As New MockTermTable("termtable.xml")
            Dim term0 As New MockTerm("term0Text", 0, "term0Class", "term0Comment", "term0recommended", table)
            Dim term1 As New MockTerm("term1Text", 3, "term1Class", "term1Comment", "term1recommended", table)
            Dim hit0 As New MockScanHit("file0", 1, 10, "line text", term0, Nothing)
            Dim hit1 As New MockScanHit("file1", 4, 1, "line text 2", term1, Nothing)
            Dim scanResult As New MockScanResult("file0", New IScanHit() {hit0, hit1}, True)
            accessor.AddResult(scanResult, "c:\projFile")

            Dim enumerator As IVsEnumTaskItems = Nothing
            Dim hr As Integer = accessor.EnumTaskItems(enumerator)

            Assert.AreEqual(VSConstants.S_OK, hr, "EnumTaskItems returned wrong hresult.")

            Dim items As IVsTaskItem() = New IVsTaskItem(2) {Nothing, Nothing, Nothing}
            Dim fetched As UInteger() = New UInteger(0) {0}
            hr = enumerator.Next(3, items, fetched)
            Assert.AreEqual(VSConstants.S_FALSE, hr, "Next returned wrong hresult with celt too high.")
            Assert.AreEqual(CUInt(2), fetched(0), "Next returned wrong value for fetched with celt too high.")
            Assert.IsNotNull(items(0), "Next failed to set first item with celt too high.")
            Assert.IsNotNull(items(1), "Next failed to set second item with celt too high.")
            Assert.IsNull(items(2), "Next set third item with celt too high.")

            hr = enumerator.Reset()
            Assert.AreEqual(VSConstants.S_OK, hr, "Reset returned wrong hresult.")

            items(2) = Nothing
            items(1) = items(2)
            items(0) = items(1)
            fetched(0) = 0
            hr = enumerator.Next(2, items, fetched)
            Assert.AreEqual(VSConstants.S_OK, hr, "Next returned wrong hresult with celt at max.")
            Assert.AreEqual(CUInt(2), fetched(0), "Next returned wrong value for fetched with celt at max.")
            Assert.IsNotNull(items(0), "Next failed to set first item with celt at max.")
            Assert.IsNotNull(items(1), "Next failed to set second item with celt at max.")

            enumerator.Reset()

            items(2) = Nothing
            items(1) = items(2)
            items(0) = items(1)
            fetched(0) = 0
            hr = enumerator.Next(1, items, fetched)
            Assert.AreEqual(VSConstants.S_OK, hr, "Next returned wrong hresult with celt lower than max.")
            Assert.AreEqual(CUInt(1), fetched(0), "Next returned wrong value for fetched with celt lower than max.")
            Assert.IsNotNull(items(0), "Next failed to set first item with celt lower than max.")
            Assert.IsNull(items(1), "Next set second item with celt lower than max.")

            enumerator.Reset()

            items(2) = Nothing
            items(1) = items(2)
            items(0) = items(1)
            fetched(0) = 1
            hr = enumerator.Next(0, items, fetched)
            Assert.AreEqual(VSConstants.S_OK, hr, "Next returned wrong hresult with celt = 0.")
            Assert.AreEqual(CUInt(0), fetched(0), "Next returned wrong value for fetched with celt = 0.")
            Assert.IsNull(items(0), "Next set first item with celt = 0.")

            enumerator.Reset()

            hr = enumerator.Skip(0)
            Assert.AreEqual(VSConstants.S_OK, hr, "Skip(0) returned wrong hresult.")

            hr = enumerator.Skip(1)
            Assert.AreEqual(VSConstants.S_OK, hr, "Skip(1) returned wrong hresult.")

            hr = enumerator.Skip(2)
            Assert.AreEqual(VSConstants.S_FALSE, hr, "Skip(2) returned wrong hresult.")

            Dim tasks As List(Of IVsTaskItem) = Utilities.TasksFromEnumerator(enumerator)

            Dim enumerator2 As IVsEnumTaskItems = Nothing
            hr = enumerator.Clone(enumerator2)
            Assert.AreEqual(VSConstants.S_OK, hr, "Clone returned wrong hresult.")

            Dim tasks2 As List(Of IVsTaskItem) = Utilities.TasksFromEnumerator(enumerator2)

            Assert.IsTrue(CodeSweep.Utilities.OrderedCollectionsAreEqual(tasks, tasks2), "Clone did not produce an equivalent collection.")
        End Sub

        ''' <summary>
        ''' A test case for GetColumn (int, VSTASKCOLUMN[]).
        ''' </summary>
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub GetColumns()
            Dim accessor As New CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider)

            Dim expectedPriority As VSTASKCOLUMN
            expectedPriority.bstrCanonicalName = "Priority"
            expectedPriority.bstrHeading = "!"
            expectedPriority.bstrLocalizedName = "Priority"
            expectedPriority.bstrTip = "Priority"
            expectedPriority.cxDefaultWidth = 22
            expectedPriority.cxMinWidth = 0
            expectedPriority.fAllowHide = 1
            expectedPriority.fAllowUserSort = 1
            expectedPriority.fDescendingSort = 0
            expectedPriority.fDynamicSize = 0
            expectedPriority.fFitContent = 0
            expectedPriority.fMoveable = 1
            expectedPriority.fShowSortArrow = 0
            expectedPriority.fSizeable = 1
            expectedPriority.fVisibleByDefault = 1
            expectedPriority.iDefaultSortPriority = -1
            expectedPriority.iField = CInt(Fix(TaskFields.Priority))
            expectedPriority.iImage = -1

            Dim expectedPriorityNumber As VSTASKCOLUMN
            expectedPriorityNumber.bstrCanonicalName = "Priority Number"
            expectedPriorityNumber.bstrHeading = "!#"
            expectedPriorityNumber.bstrLocalizedName = "Priority Number"
            expectedPriorityNumber.bstrTip = "Priority Number"
            expectedPriorityNumber.cxDefaultWidth = 50
            expectedPriorityNumber.cxMinWidth = 0
            expectedPriorityNumber.fAllowHide = 1
            expectedPriorityNumber.fAllowUserSort = 1
            expectedPriorityNumber.fDescendingSort = 0
            expectedPriorityNumber.fDynamicSize = 0
            expectedPriorityNumber.fFitContent = 0
            expectedPriorityNumber.fMoveable = 1
            expectedPriorityNumber.fShowSortArrow = 0
            expectedPriorityNumber.fSizeable = 1
            expectedPriorityNumber.fVisibleByDefault = 0
            expectedPriorityNumber.iDefaultSortPriority = 0
            expectedPriorityNumber.iField = CInt(Fix(TaskFields.PriorityNumber))
            expectedPriorityNumber.iImage = -1



            Dim expectedTerm As VSTASKCOLUMN
            expectedTerm.bstrCanonicalName = "Term"
            expectedTerm.bstrHeading = "Term"
            expectedTerm.bstrLocalizedName = "Term"
            expectedTerm.bstrTip = ""
            expectedTerm.cxDefaultWidth = 103
            expectedTerm.cxMinWidth = 0
            expectedTerm.fAllowHide = 1
            expectedTerm.fAllowUserSort = 1
            expectedTerm.fDescendingSort = 0
            expectedTerm.fDynamicSize = 1
            expectedTerm.fFitContent = 0
            expectedTerm.fMoveable = 1
            expectedTerm.fShowSortArrow = 1
            expectedTerm.fSizeable = 1
            expectedTerm.fVisibleByDefault = 1
            expectedTerm.iDefaultSortPriority = -1
            expectedTerm.iField = CInt(Fix(TaskFields.Term))
            expectedTerm.iImage = -1

            Dim expectedClass As VSTASKCOLUMN
            expectedClass.bstrCanonicalName = "Class"
            expectedClass.bstrHeading = "Class"
            expectedClass.bstrLocalizedName = "Class"
            expectedClass.bstrTip = ""
            expectedClass.cxDefaultWidth = 91
            expectedClass.cxMinWidth = 0
            expectedClass.fAllowHide = 1
            expectedClass.fAllowUserSort = 1
            expectedClass.fDescendingSort = 0
            expectedClass.fDynamicSize = 1
            expectedClass.fFitContent = 0
            expectedClass.fMoveable = 1
            expectedClass.fShowSortArrow = 1
            expectedClass.fSizeable = 1
            expectedClass.fVisibleByDefault = 1
            expectedClass.iDefaultSortPriority = -1
            expectedClass.iField = System.Convert.ToInt32(TaskFields.Class)
            expectedClass.iImage = -1

            Dim expectedReplacement As VSTASKCOLUMN
            expectedReplacement.bstrCanonicalName = "Replacement"
            expectedReplacement.bstrHeading = "Replacement"
            expectedReplacement.bstrLocalizedName = "Replacement"
            expectedReplacement.bstrTip = ""
            expectedReplacement.cxDefaultWidth = 140
            expectedReplacement.cxMinWidth = 0
            expectedReplacement.fAllowHide = 1
            expectedReplacement.fAllowUserSort = 1
            expectedReplacement.fDescendingSort = 0
            expectedReplacement.fDynamicSize = 0
            expectedReplacement.fFitContent = 0
            expectedReplacement.fMoveable = 1
            expectedReplacement.fShowSortArrow = 1
            expectedReplacement.fSizeable = 1
            expectedReplacement.fVisibleByDefault = 0
            expectedReplacement.iDefaultSortPriority = -1
            expectedReplacement.iField = CInt(Fix(TaskFields.Replacement))
            expectedReplacement.iImage = -1

            Dim expectedComment As VSTASKCOLUMN
            expectedComment.bstrCanonicalName = "Comment"
            expectedComment.bstrHeading = "Comment"
            expectedComment.bstrLocalizedName = "Comment"
            expectedComment.bstrTip = ""
            expectedComment.cxDefaultWidth = 400
            expectedComment.cxMinWidth = 0
            expectedComment.fAllowHide = 1
            expectedComment.fAllowUserSort = 1
            expectedComment.fDescendingSort = 0
            expectedComment.fDynamicSize = 1
            expectedComment.fFitContent = 0
            expectedComment.fMoveable = 1
            expectedComment.fShowSortArrow = 1
            expectedComment.fSizeable = 1
            expectedComment.fVisibleByDefault = 1
            expectedComment.iDefaultSortPriority = -1
            expectedComment.iField = System.Convert.ToInt32(TaskFields.Comment)
            expectedComment.iImage = -1

            Dim expectedFile As VSTASKCOLUMN
            expectedFile.bstrCanonicalName = "File"
            expectedFile.bstrHeading = "File"
            expectedFile.bstrLocalizedName = "File"
            expectedFile.bstrTip = ""
            expectedFile.cxDefaultWidth = 92
            expectedFile.cxMinWidth = 0
            expectedFile.fAllowHide = 1
            expectedFile.fAllowUserSort = 1
            expectedFile.fDescendingSort = 0
            expectedFile.fDynamicSize = 0
            expectedFile.fFitContent = 0
            expectedFile.fMoveable = 1
            expectedFile.fShowSortArrow = 1
            expectedFile.fSizeable = 1
            expectedFile.fVisibleByDefault = 1
            expectedFile.iDefaultSortPriority = 2
            expectedFile.iField = CInt(Fix(TaskFields.File))
            expectedFile.iImage = -1

            Dim expectedLine As VSTASKCOLUMN
            expectedLine.bstrCanonicalName = "Line"
            expectedLine.bstrHeading = "Line"
            expectedLine.bstrLocalizedName = "Line"
            expectedLine.bstrTip = ""
            expectedLine.cxDefaultWidth = 63
            expectedLine.cxMinWidth = 0
            expectedLine.fAllowHide = 1
            expectedLine.fAllowUserSort = 1
            expectedLine.fDescendingSort = 0
            expectedLine.fDynamicSize = 0
            expectedLine.fFitContent = 0
            expectedLine.fMoveable = 1
            expectedLine.fShowSortArrow = 1
            expectedLine.fSizeable = 1
            expectedLine.fVisibleByDefault = 1
            expectedLine.iDefaultSortPriority = 3
            expectedLine.iField = CInt(Fix(TaskFields.Line))
            expectedLine.iImage = -1

            Dim expectedProject As VSTASKCOLUMN
            expectedProject.bstrCanonicalName = "Project"
            expectedProject.bstrHeading = "Project"
            expectedProject.bstrLocalizedName = "Project"
            expectedProject.bstrTip = ""
            expectedProject.cxDefaultWidth = 116
            expectedProject.cxMinWidth = 0
            expectedProject.fAllowHide = 1
            expectedProject.fAllowUserSort = 1
            expectedProject.fDescendingSort = 0
            expectedProject.fDynamicSize = 0
            expectedProject.fFitContent = 0
            expectedProject.fMoveable = 1
            expectedProject.fShowSortArrow = 1
            expectedProject.fSizeable = 1
            expectedProject.fVisibleByDefault = 1
            expectedProject.iDefaultSortPriority = 1
            expectedProject.iField = CInt(Fix(TaskFields.Project))
            expectedProject.iImage = -1

            Dim expectedColumns() As VSTASKCOLUMN = {expectedPriority, expectedPriorityNumber, expectedTerm, expectedClass, expectedReplacement, expectedComment, expectedFile, expectedLine, expectedProject}

            Dim dummy As VSTASKCOLUMN
            dummy.bstrCanonicalName = Nothing
            dummy.bstrHeading = Nothing
            dummy.bstrLocalizedName = Nothing
            dummy.bstrTip = Nothing
            dummy.cxDefaultWidth = -3
            dummy.cxMinWidth = -3
            dummy.fAllowHide = -3
            dummy.fAllowUserSort = -3
            dummy.fDescendingSort = -3
            dummy.fDynamicSize = -3
            dummy.fFitContent = -3
            dummy.fMoveable = -3
            dummy.fShowSortArrow = -3
            dummy.fSizeable = -3
            dummy.fVisibleByDefault = -3
            dummy.iDefaultSortPriority = -3
            dummy.iField = -3
            dummy.iImage = -3

            Dim column() As VSTASKCOLUMN = {dummy}

            Dim hr As Integer

            Dim count As Integer
            hr = accessor.GetColumnCount(count)
            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnCount returned wrong hresult.")
            Assert.AreEqual(System.Enum.GetValues(GetType(TaskFields)).Length, count, "GetColumnCount returned wrong count.")

            For i As Integer = 0 To expectedColumns.Length - 1
                hr = accessor.GetColumn(i, column)
                Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnCount returned wrong hresult for " & expectedColumns(i).bstrCanonicalName & ".")
                Assert.AreEqual(expectedColumns(i).bstrCanonicalName, column(0).bstrCanonicalName, "bstrCanonicalName was wrong for " & expectedColumns(i).bstrCanonicalName & ".")
                Assert.AreEqual(expectedColumns(i).bstrHeading, column(0).bstrHeading, "bstrHeading was wrong for " & expectedColumns(i).bstrCanonicalName & ".")
                Assert.AreEqual(expectedColumns(i).bstrLocalizedName, column(0).bstrLocalizedName, "bstrLocalizedName was wrong for " & expectedColumns(i).bstrCanonicalName & ".")
                Assert.AreEqual(expectedColumns(i).bstrTip, column(0).bstrTip, "bstrTip was wrong for " & expectedColumns(i).bstrCanonicalName & ".")
                Assert.AreEqual(expectedColumns(i).cxDefaultWidth, column(0).cxDefaultWidth, "cxDefaultWidth was wrong for " & expectedColumns(i).bstrCanonicalName & ".")
                Assert.AreEqual(expectedColumns(i).cxMinWidth, column(0).cxMinWidth, "cxMinWidth was wrong for " & expectedColumns(i).bstrCanonicalName & ".")
                Assert.AreEqual(expectedColumns(i).fAllowHide, column(0).fAllowHide, "fAllowHide was wrong for " & expectedColumns(i).bstrCanonicalName & ".")
                Assert.AreEqual(expectedColumns(i).fAllowUserSort, column(0).fAllowUserSort, "fAllowUserSort was wrong for " & expectedColumns(i).bstrCanonicalName & ".")
                Assert.AreEqual(expectedColumns(i).fDescendingSort, column(0).fDescendingSort, "fDescendingSort was wrong for " & expectedColumns(i).bstrCanonicalName & ".")
                Assert.AreEqual(expectedColumns(i).fDynamicSize, column(0).fDynamicSize, "fDynamicSize was wrong for " & expectedColumns(i).bstrCanonicalName & ".")
                Assert.AreEqual(expectedColumns(i).fFitContent, column(0).fFitContent, "fFitContent was wrong for " & expectedColumns(i).bstrCanonicalName & ".")
                Assert.AreEqual(expectedColumns(i).fMoveable, column(0).fMoveable, "fMoveable was wrong for " & expectedColumns(i).bstrCanonicalName & ".")
                Assert.AreEqual(expectedColumns(i).fShowSortArrow, column(0).fShowSortArrow, "fShowSortArrow was wrong for " & expectedColumns(i).bstrCanonicalName & ".")
                Assert.AreEqual(expectedColumns(i).fSizeable, column(0).fSizeable, "fSizeable was wrong for " & expectedColumns(i).bstrCanonicalName & ".")
                Assert.AreEqual(expectedColumns(i).fVisibleByDefault, column(0).fVisibleByDefault, "fVisibleByDefault was wrong for " & expectedColumns(i).bstrCanonicalName & ".")
                Assert.AreEqual(expectedColumns(i).iDefaultSortPriority, column(0).iDefaultSortPriority, "iDefaultSortPriority was wrong for " & expectedColumns(i).bstrCanonicalName & ".")
                Assert.AreEqual(expectedColumns(i).iField, column(0).iField, "iField was wrong for " & expectedColumns(i).bstrCanonicalName & ".")
                Assert.AreEqual(expectedColumns(i).iImage, column(0).iImage, "iImage was wrong for " & expectedColumns(i).bstrCanonicalName & ".")
            Next i
        End Sub

        ''' <summary>
        ''' A test case for GetProviderFlags (out uint).
        ''' </summary>
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub GetProviderFlagsTest()
            Dim accessor As New CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider)

            Dim flags As UInteger
            Dim hr As Integer = accessor.GetProviderFlags(flags)

            Assert.AreEqual(VSConstants.S_OK, hr, "GetProvider flags returned wrong hresult.")
            Assert.AreEqual(__VSTASKPROVIDERFLAGS.TPF_NOAUTOROUTING Or __VSTASKPROVIDERFLAGS.TPF_ALWAYSVISIBLE, CType(flags, __VSTASKPROVIDERFLAGS), "GetProviderFlags returned wrong flags.")
        End Sub

        ''' <summary>
        ''' A test case for GetProviderGuid (out Guid).
        ''' </summary>
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub GetProviderGuidTest()
            Dim accessor1 As New CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider)

            Utilities.RemoveCommandHandlers(_serviceProvider)

            Dim accessor2 As New CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider)

            Dim guid1 As Guid
            Dim guid2 As Guid
            Dim hr As Integer = accessor1.GetProviderGuid(guid1)

            Assert.AreEqual(VSConstants.S_OK, hr, "GetProviderGuid returned wrong hresult.")
            Assert.AreNotEqual(Guid.Empty, guid1, "GetProviderGuid returned null guid.")

            hr = accessor2.GetProviderGuid(guid2)

            Assert.AreEqual(VSConstants.S_OK, hr, "GetProviderGuid returned wrong hresult (second instance).")
            Assert.AreEqual(guid1, guid2, "GetProviderGuid did not return the same guid for two different instances.")
        End Sub

        ''' <summary>
        ''' A test case for GetProviderName (out string).
        ''' </summary>
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub GetProviderNameTest()
            Dim accessor As New CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider)

            Dim name As String = String.Empty
            Dim hr As Integer = accessor.GetProviderName(name)

            Assert.AreEqual(VSConstants.S_OK, hr, "GetProviderName returned wrong hresult.")
            Assert.AreEqual("CodeSweep", name, "GetProviderName returned wrong name.")
        End Sub

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub GetProviderToolbarTest()
            Dim accessor As New CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider)

            Dim id As UInteger = 0
            Dim group As Guid = Guid.Empty
            Dim hr As Integer = accessor.GetProviderToolbar(group, id)

            Assert.AreEqual(VSConstants.S_OK, hr, "GetProviderToolbar returned wrong hresult.")
            Assert.AreEqual(CodeSweep.VSPackage.GuidList_Accessor.guidVSPackageCmdSet, group, "GetProviderToolbar returned wrong group.")
            Assert.AreEqual(CUInt(&H2020), id, "GetProviderToolbar returned wrong id.")
        End Sub

        <DllImport("comctl32.dll")> _
        Private Shared Function ImageList_GetImageCount(ByVal himl As IntPtr) As Integer
        End Function

        <DllImport("comctl32.dll")> _
        Shared Sub ImageList_Destroy(ByVal handle As IntPtr)
        End Sub

        ''' <summary>
        ''' A test case for ImageList (out IntPtr).
        ''' </summary>
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub ImageListTest()
            Dim accessor As New CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider)

            Dim handle As IntPtr = IntPtr.Zero

            Try
                Dim hr As Integer = accessor.ImageList(handle)

                Assert.AreEqual(VSConstants.S_OK, hr, "ImageList returned wrong hresult.")
                Assert.AreNotEqual(IntPtr.Zero, handle, "ImageList returned null image list.")
                Assert.AreEqual(3, ImageList_GetImageCount(handle), "ImageList returned wrong number of images.")

                Dim handle2 As IntPtr = IntPtr.Zero

                Try
                    accessor.ImageList(handle)

                    Assert.AreNotEqual(handle, handle2, "ImageList did not return a new list handle each time.")
                Finally
                    If handle2 <> IntPtr.Zero Then
                        ImageList_Destroy(handle2)
                    End If
                End Try
            Finally
                If handle <> IntPtr.Zero Then
                    ImageList_Destroy(handle)
                End If
            End Try
        End Sub

        ''' <summary>
        ''' A test case for ReRegistrationKey (out string).
        ''' </summary>
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub ReRegistrationKeyTest()
            Dim accessor As New CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider)

            Dim key As String = String.Empty
            Dim hr As Integer = accessor.ReRegistrationKey(key)
            Assert.AreEqual(VSConstants.E_NOTIMPL, hr, "ReRegistrationKey returned wrong hresult.")
        End Sub

        ''' <summary>
        ''' A test case for GetSurrogateProviderGuid (out Guid).
        ''' </summary>
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub GetSurrogateProviderGuidTest()
            Dim accessor As New CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider)

            Dim surrogate As Guid
            Dim hr As Integer = accessor.GetSurrogateProviderGuid(surrogate)

            Assert.AreEqual(VSConstants.E_NOTIMPL, hr, "GetSurrogateProviderGuid returned wrong hresult.")
        End Sub
        Private NotInheritable Class AnonymousClass13
            Public registered As New List(Of UInteger)()
            Public unregistered As New List(Of UInteger)()
            Public Sub AnonymousMethod1(ByVal sender As Object, ByVal args As MockTaskList.RegisterTaskProviderArgs)
                registered.Add(args.Cookie)
            End Sub
            Public Sub AnonymousMethod2(ByVal sender As Object, ByVal args As MockTaskList.UnregisterTaskProviderArgs)
                unregistered.Add(args.Cookie)
            End Sub

        End Class

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub TestRegistrationAndUnregistration()

            Dim locals As New AnonymousClass13()
            Dim taskList As MockTaskList = TryCast(_serviceProvider.GetService(GetType(SVsTaskList)), MockTaskList)
            AddHandler taskList.OnRegisterTaskProvider, AddressOf locals.AnonymousMethod1

            AddHandler taskList.OnUnregisterTaskProvider, AddressOf locals.AnonymousMethod2


            Dim accessor As New CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider)

            Assert.AreEqual(1, locals.registered.Count, "Task provider did not register itself when created.")

            accessor.OnTaskListFinalRelease(taskList)

            Assert.AreEqual(1, locals.unregistered.Count, "Task provider did not unregister itself from OnTaskListFinalRelease.")
            Assert.AreEqual(locals.registered(0), locals.unregistered(0), "Cookies did not match.")
        End Sub

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub TestIgnore()
            Dim accessor As New CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider)

            Dim project As Build.Evaluation.Project = Utilities.SetupMSBuildProject()
            Utilities.RegisterProjectWithMocks(project, _serviceProvider)

            Dim table As New MockTermTable("termtable.xml")
            Dim term0 As New MockTerm("dupText", 0, "term0Class", "term0Comment", "term0recommended", table)
            Dim term1 As New MockTerm("term2Text", 2, "term2Class", "term2Comment", "term2recommended", table)
            Dim hit0 As New MockScanHit("file0", 1, 5, "line text", term0, Nothing)
            Dim hit1 As New MockScanHit("file1", 4, 1, "line text 2", term1, Nothing)
            Dim hit2 As New MockScanHit("file2", 3, 2, "line text 3", term1, Nothing)
            Dim scanResult As New MockScanResult("file0", New IScanHit() {hit0, hit1, hit2}, True)
            accessor.AddResult(scanResult, project.FullPath)

            Dim enumerator As IVsEnumTaskItems = Nothing
            accessor.EnumTaskItems(enumerator)
            Dim items As List(Of IVsTaskItem) = Utilities.TasksFromEnumerator(enumerator)
            Dim task0Accessor As New CodeSweep.VSPackage.Task_Accessor(New PrivateObject(items(0)))
            Dim task1Accessor As New CodeSweep.VSPackage.Task_Accessor(New PrivateObject(items(1)))

            Dim taskList As MockTaskList = TryCast(_serviceProvider.GetService(GetType(SVsTaskList)), MockTaskList)

            ' Ensure cmd is disabled with no selection.
            Dim mcs As OleMenuCommandService = TryCast(_serviceProvider.GetService(GetType(IMenuCommandService)), OleMenuCommandService)
            Dim command As MenuCommand = mcs.FindCommand(New CommandID(CodeSweep.VSPackage.GuidList_Accessor.guidVSPackageCmdSet, CInt(Fix(CodeSweep.VSPackage.PkgCmdIDList_Accessor.cmdidIgnore))))

            ' NOTE: simply getting command.Supported or command.Enabled doesn't seem to invoke
            ' QueryStatus, so I'll explicitly call the status update method as a workaround.
            accessor.QueryIgnore(Nothing, EventArgs.Empty)

            Assert.IsTrue(command.Supported, "Command not supported.")
            Assert.IsFalse(command.Enabled, "Command enabled with no selection.")

            ' Ensure cmd is disabled with an ignored item selected.
            task0Accessor.Ignored = True
            taskList.SetSelected(items(0), True)
            accessor.QueryIgnore(Nothing, EventArgs.Empty)
            Assert.IsFalse(command.Enabled, "Command enabled with ignored item selected.")

            ' Ensure cmd is enabled with one ignored and one non-ignored item selected.
            taskList.SetSelected(items(1), True)
            accessor.QueryIgnore(Nothing, EventArgs.Empty)
            Assert.IsTrue(command.Enabled, "Command disabled with a non-ignored item selected.")

            ' Fire cmd, ensure selected items are ignored.
            command.Invoke()
            accessor.QueryIgnore(Nothing, EventArgs.Empty)
            Assert.IsTrue(task0Accessor.Ignored, "Command set ignored task to non-ignored.")
            Assert.IsTrue(task1Accessor.Ignored, "Command did not set non-ignored task to ignored.")

            ' Ensure cmd is now disabled.
            accessor.QueryIgnore(Nothing, EventArgs.Empty)
            Assert.IsFalse(command.Enabled, "Command still enabled after invocation.")
        End Sub

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub TestDontIgnore()
            Dim accessor As New CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider)

            Dim project As Build.Evaluation.Project = Utilities.SetupMSBuildProject()
            Utilities.RegisterProjectWithMocks(project, _serviceProvider)

            Dim table As New MockTermTable("termtable.xml")
            Dim term0 As New MockTerm("dupText", 0, "term0Class", "term0Comment", "term0recommended", table)
            Dim term1 As New MockTerm("term2Text", 2, "term2Class", "term2Comment", "term2recommended", table)
            Dim hit0 As New MockScanHit("file0", 1, 5, "line text", term0, Nothing)
            Dim hit1 As New MockScanHit("file1", 4, 1, "line text 2", term1, Nothing)
            Dim hit2 As New MockScanHit("file2", 3, 2, "line text 3", term1, Nothing)
            Dim scanResult As New MockScanResult("file0", New IScanHit() {hit0, hit1, hit2}, True)
            accessor.AddResult(scanResult, project.FullPath)

            Dim enumerator As IVsEnumTaskItems = Nothing
            accessor.EnumTaskItems(enumerator)
            Dim items As List(Of IVsTaskItem) = Utilities.TasksFromEnumerator(enumerator)
            Dim task0Accessor As New CodeSweep.VSPackage.Task_Accessor(New PrivateObject(items(0)))
            Dim task1Accessor As New CodeSweep.VSPackage.Task_Accessor(New PrivateObject(items(1)))

            Dim taskList As MockTaskList = TryCast(_serviceProvider.GetService(GetType(SVsTaskList)), MockTaskList)

            ' Ensure cmd is disabled with no selection.
            Dim mcs As OleMenuCommandService = TryCast(_serviceProvider.GetService(GetType(IMenuCommandService)), OleMenuCommandService)
            Dim command As MenuCommand = mcs.FindCommand(New CommandID(CodeSweep.VSPackage.GuidList_Accessor.guidVSPackageCmdSet, CInt(Fix(CodeSweep.VSPackage.PkgCmdIDList_Accessor.cmdidDoNotIgnore))))

            ' NOTE: simply getting command.Supported or command.Enabled doesn't seem to invoke
            ' QueryStatus, so I'll explicitly call the status update method as a workaround.
            accessor.QueryDontIgnore(Nothing, EventArgs.Empty)

            Assert.IsTrue(command.Supported, "Command not supported.")
            Assert.IsFalse(command.Enabled, "Command enabled with no selection.")

            ' Ensure cmd is enabled with an ignored item selected.
            task0Accessor.Ignored = True
            taskList.SetSelected(items(0), True)
            accessor.QueryDontIgnore(Nothing, EventArgs.Empty)
            Assert.IsTrue(command.Enabled, "Command disabled with ignored item selected.")

            ' Ensure cmd is enabled with one ignored and one non-ignored item selected.
            taskList.SetSelected(items(1), True)
            accessor.QueryDontIgnore(Nothing, EventArgs.Empty)
            Assert.IsTrue(command.Enabled, "Command disabled with a non-ignored item selected.")

            ' Fire cmd, ensure selected items are not ignored.
            command.Invoke()
            accessor.QueryDontIgnore(Nothing, EventArgs.Empty)
            Assert.IsFalse(task0Accessor.Ignored, "Command did not set ignored task to non-ignored.")
            Assert.IsFalse(task1Accessor.Ignored, "Command set non-ignored task to ignored.")

            ' Ensure cmd is now disabled.
            accessor.QueryDontIgnore(Nothing, EventArgs.Empty)
            Assert.IsFalse(command.Enabled, "Command still enabled after invocation.")
        End Sub

        ' TODO: ensure the Stop Scan command is enabled only while a scan is in progress (this is handled in VsPkg.vb, so it should be in the tests for that class).
        ' TODO: ensure the Repeat Last Scan command is disabled while a scan is in progress or if no scan has been done (this is handled in VsPkg.vb, so it should be in the tests for that class).
        ' TODO: ensure Show Ignore Instances acts as a toggle (becomes ninched when you press it) and has the desired effect.
        ' TODO: ensure that the tasks for a project are removed when the project is unloaded, and that the tasks for other projects remain.
    End Class
End Namespace
