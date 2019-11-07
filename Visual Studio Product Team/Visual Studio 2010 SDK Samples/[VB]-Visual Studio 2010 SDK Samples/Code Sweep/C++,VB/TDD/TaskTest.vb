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
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.TextManager.Interop
Imports System.IO

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    ''' <summary>
    ''' This is a test class for CodeSweep.VSPackage.Task and is intended
    ''' to contain all CodeSweep.VSPackage.Task Unit Tests.
    ''' </summary>
    <TestClass()> _
    Public Class TaskTest

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



        ''' <summary>
        ''' A test case for CanDelete (out int).
        ''' </summary>
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub CanDeleteTest()
            Dim accessor As New CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", "z:\dir\file.ext", 1, 1, "projFile", "full line text", Nothing, Nothing)

            Dim canDelete As Integer

            Dim hr As Integer = accessor.CanDelete(canDelete)

            Assert.AreEqual(VSConstants.S_OK, hr, "CanDelete had unexpected return code.")
            Assert.AreEqual(0, canDelete, "CanDelete indicated deletion was possible.")
        End Sub

        ''' <summary>
        ''' A test case for GetColumnValue (int, out uint, out uint, out object, out string).
        ''' </summary>
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub GetColumnValueTest()
            Dim msbuildProj As Microsoft.Build.Evaluation.Project = Utilities.SetupMSBuildProject()

            ' Set up a project so the project column can be populated.
            Dim solution As MockSolution = TryCast(_serviceProvider.GetService(GetType(SVsSolution)), MockSolution)
            Dim project As New MockIVsProject(msbuildProj.FullPath)
            solution.AddProject(project)

            Dim providerAccessor As New CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider)

            Dim accessor As New CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment with link:  http://www.microsoft.com", "replacement", "z:\dir\file.ext", 2, 3, msbuildProj.FullPath, "full line text", providerAccessor, Nothing)

            Dim type As UInteger
            Dim flags As UInteger
            Dim val As Object = Nothing
            Dim accName As String = String.Empty

            Dim hr As Integer = accessor.GetColumnValue(CInt(Fix(TaskFields.Priority)), type, flags, val, accName)

            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnValue did not return S_OK for the Priority column.")
            Assert.AreEqual(__VSTASKVALUETYPE.TVT_IMAGE, CType(type, __VSTASKVALUETYPE), "Type of Priority column is incorrect.")
            Assert.AreEqual(__VSTASKVALUEFLAGS.TVF_HORZ_CENTER, CType(flags, __VSTASKVALUEFLAGS), "Flags for Priority column are incorrect.")
            Assert.AreEqual(GetType(Integer), val.GetType(), "Value of Priority column has wrong type.")
            Assert.IsTrue(CInt(Fix(val)) >= 0 AndAlso CInt(Fix(val)) <= 2, "Image index for Priority column is out of range.")
            Dim imageAccText() As String = {"High", "Medium", "Low"}
            Assert.AreEqual(imageAccText(CInt(Fix(val))), accName, "Accessibility text of Priority column is incorrect.")

            hr = accessor.GetColumnValue(CInt(Fix(TaskFields.PriorityNumber)), type, flags, val, accName)

            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnValue did not return S_OK for the PriorityNumber column.")
            Assert.AreEqual(__VSTASKVALUETYPE.TVT_BASE10, CType(type, __VSTASKVALUETYPE), "Type of PriorityNumber column is incorrect.")
            Assert.AreEqual(0, CInt(Fix(flags)), "Flags for PriorityNumber column are incorrect.")
            Assert.AreEqual(GetType(Integer), val.GetType(), "Value of PriorityNumber column has wrong type.")
            Assert.AreEqual(1, CInt(Fix(val)), "Value of PriorityNumber column is incorrect.")
            Assert.AreEqual("", accName, "Accessibility text of PriorityNumber column is incorrect.")

            hr = accessor.GetColumnValue(CInt(Fix(TaskFields.Term)), type, flags, val, accName)

            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnValue did not return S_OK for the Term column.")
            Assert.AreEqual(__VSTASKVALUETYPE.TVT_TEXT, CType(type, __VSTASKVALUETYPE), "Type of Term column is incorrect.")
            Assert.AreEqual(0, CInt(Fix(flags)), "Flags for Term column are incorrect.")
            Assert.AreEqual(GetType(String), val.GetType(), "Value of Term column has wrong type.")
            Assert.AreEqual("term", CStr(val), "Value of Term column is incorrect.")
            Assert.AreEqual("", accName, "Accessibility text of Term column is incorrect.")

            hr = accessor.GetColumnValue(System.Convert.ToInt32(TaskFields.Class), type, flags, val, accName)

            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnValue did not return S_OK for the Class column.")
            Assert.AreEqual(__VSTASKVALUETYPE.TVT_TEXT, CType(type, __VSTASKVALUETYPE), "Type of Class column is incorrect.")
            Assert.AreEqual(0, CInt(Fix(flags)), "Flags for Class column are incorrect.")
            Assert.AreEqual(GetType(String), val.GetType(), "Value of Class column has wrong type.")
            Assert.AreEqual("class", CStr(val), "Value of Class column is incorrect.")
            Assert.AreEqual("", accName, "Accessibility text of Class column is incorrect.")

            hr = accessor.GetColumnValue(CInt(Fix(TaskFields.Replacement)), type, flags, val, accName)

            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnValue did not return S_OK for the Replacement column.")
            Assert.AreEqual(__VSTASKVALUETYPE.TVT_TEXT, CType(type, __VSTASKVALUETYPE), "Type of Replacement column is incorrect.")
            Assert.AreEqual(0, CInt(Fix(flags)), "Flags for Replacement column are incorrect.")
            Assert.AreEqual(GetType(String), val.GetType(), "Value of Replacement column has wrong type.")
            Assert.AreEqual("replacement", CStr(val), "Value of Replacement column is incorrect.")
            Assert.AreEqual("", accName, "Accessibility text of Replacement column is incorrect.")

            hr = accessor.GetColumnValue(System.Convert.ToInt32(TaskFields.Comment), type, flags, val, accName)

            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnValue did not return S_OK for the Comment column.")
            Assert.AreEqual(__VSTASKVALUETYPE.TVT_LINKTEXT, CType(type, __VSTASKVALUETYPE), "Type of Comment column is incorrect.")
            Assert.AreEqual(0, CInt(Fix(flags)), "Flags for Comment column are incorrect.")
            Assert.AreEqual(GetType(String), val.GetType(), "Value of Comment column has wrong type.")
            Assert.AreEqual("comment with link:  @http://www.microsoft.com@", CStr(val), "Value of Comment column is incorrect.")
            Assert.AreEqual("", accName, "Accessibility text of Comment column is incorrect.")

            hr = accessor.GetColumnValue(CInt(Fix(TaskFields.File)), type, flags, val, accName)

            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnValue did not return S_OK for the File column.")
            Assert.AreEqual(__VSTASKVALUETYPE.TVT_TEXT, CType(type, __VSTASKVALUETYPE), "Type of File column is incorrect.")
            Assert.AreEqual(__VSTASKVALUEFLAGS.TVF_FILENAME, CType(flags, __VSTASKVALUEFLAGS), "Flags for File column are incorrect.")
            Assert.AreEqual(GetType(String), val.GetType(), "Value of File column has wrong type.")
            Assert.AreEqual("z:\dir\file.ext", CStr(val), "Value of File column is incorrect.")
            Assert.AreEqual("", accName, "Accessibility text of File column is incorrect.")

            hr = accessor.GetColumnValue(CInt(Fix(TaskFields.Line)), type, flags, val, accName)

            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnValue did not return S_OK for the Line column.")
            Assert.AreEqual(__VSTASKVALUETYPE.TVT_BASE10, CType(type, __VSTASKVALUETYPE), "Type of Line column is incorrect.")
            Assert.AreEqual(0, CInt(Fix(flags)), "Flags for Line column are incorrect.")
            Assert.AreEqual(GetType(Integer), val.GetType(), "Value of Line column has wrong type.")
            Assert.AreEqual(3, CInt(Fix(val)), "Value of Line column is incorrect.")
            Assert.AreEqual("", accName, "Accessibility text of Line column is incorrect.")

            hr = accessor.GetColumnValue(CInt(Fix(TaskFields.Project)), type, flags, val, accName)

            Assert.AreEqual(VSConstants.S_OK, hr, "GetColumnValue did not return S_OK for the Project column.")
            Assert.AreEqual(__VSTASKVALUETYPE.TVT_TEXT, CType(type, __VSTASKVALUETYPE), "Type of Project column is incorrect.")
            Assert.AreEqual(0, CInt(Fix(flags)), "Flags for Project column are incorrect.")
            Assert.AreEqual(GetType(String), val.GetType(), "Value of Project column has wrong type.")
            Dim uniqueName As String = String.Empty
            solution.GetUniqueUINameOfProject(project, uniqueName)
            Assert.AreEqual(uniqueName, CStr(val), "Value of Project column is incorrect.")
            Assert.AreEqual("", accName, "Accessibility text of Project column is incorrect.")
        End Sub

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub UnderlinedIfIgnored()
            Dim msbuildProj As Microsoft.Build.Evaluation.Project = Utilities.SetupMSBuildProject()

            ' Set up a project so the project column can be populated.
            Dim solution As MockSolution = TryCast(_serviceProvider.GetService(GetType(SVsSolution)), MockSolution)
            Dim project As New MockIVsProject(msbuildProj.FullPath)
            solution.AddProject(project)

            Dim providerAccessor As New CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider)

            Dim projectDrive As String = Path.GetPathRoot(msbuildProj.FullPath)

            Dim accessor As New CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", projectDrive & "\dir\file.ext", 2, 3, msbuildProj.FullPath, "full line text", providerAccessor, Nothing)

            accessor.Ignored = True

            Dim type As UInteger
            Dim flags As UInteger
            Dim val As Object = Nothing
            Dim accName As String = String.Empty

            For Each fieldObj As Object In System.Enum.GetValues(GetType(TaskFields))
                accessor.GetColumnValue(CInt(Fix(fieldObj)), type, flags, val, accName)
                Assert.IsTrue((flags And CInt(Fix(__VSTASKVALUEFLAGS.TVF_STRIKETHROUGH))) <> 0, "Strikethrough flag not set for ignored task field " & CInt(Fix(fieldObj)))
            Next fieldObj
        End Sub

        ''' <summary>
        ''' A test case for GetNavigationStatusText (out string).
        ''' </summary>
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub GetNavigationStatusTextTest()
            Dim accessor As New CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", "z:\dir\file.ext", 2, 3, "projFile", "full line text", Nothing, Nothing)

            Dim text As String = String.Empty
            Dim hr As Integer = accessor.GetNavigationStatusText(text)

            Assert.AreEqual(VSConstants.S_OK, hr, "GetNavigationStatusText did not return S_OK.")
            Assert.AreEqual("comment", text, "GetNavigationStatusText returned wrong name.")
        End Sub

        ''' <summary>
        ''' A test case for GetTaskName (out string).
        ''' </summary>
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub GetTaskNameTest()
            Dim accessor As New CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", "z:\dir\file.ext", 2, 3, "projFile", "full line text", Nothing, Nothing)

            Dim name As String = String.Empty
            Dim hr As Integer = accessor.GetTaskName(name)

            Assert.AreEqual(VSConstants.S_OK, hr, "GetTaskName did not return S_OK.")
            Assert.AreEqual("term", name, "GetTaskName returned wrong name.")
        End Sub

        ''' <summary>
        ''' A test case for GetTaskProvider (out IVsTaskProvider3).
        ''' </summary>
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub GetTaskProviderTest()
            Dim providerAccessor As New CodeSweep.VSPackage.TaskProvider_Accessor(_serviceProvider)

            Dim accessor As New CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", "z:\dir\file.ext", 2, 3, "projFile", "full line text", providerAccessor, Nothing)

            Dim provider As IVsTaskProvider3 = Nothing
            Dim hr As Integer = accessor.GetTaskProvider(provider)

            Assert.AreEqual(VSConstants.S_OK, hr, "GetTaskProvider returned wrong hresult.")
            Assert.AreEqual(providerAccessor, provider, "GetTaskProvider returned wrong provider.")
        End Sub

        ''' <summary>
        ''' A test case for GetTipText (int, out string).
        ''' </summary>
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub GetTipTextTest()
            Dim accessor As New CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", "z:\dir\file.ext", 2, 3, "projFile", "full line text", Nothing, Nothing)

            For Each fieldObj As Object In System.Enum.GetValues(GetType(TaskFields))
                Dim tip As String = String.Empty
                Dim hr As Integer = accessor.GetTipText(CInt(Fix(fieldObj)), tip)

                Assert.AreEqual(VSConstants.E_NOTIMPL, hr, "GetTipText returned wrong hresult for field " & (CType(fieldObj, TaskFields)).ToString())
            Next fieldObj
        End Sub

        ''' <summary>
        ''' A test case for HasHelp (out int).
        ''' </summary>
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub HasHelpTest()
            Dim accessor As New CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", "z:\dir\file.ext", 2, 3, "projFile", "full line text", Nothing, Nothing)

            Dim hasHelp As Integer
            Dim hr As Integer = accessor.HasHelp(hasHelp)

            Assert.AreEqual(VSConstants.S_OK, hr, "HasHelp returned wrong hresult.")
            Assert.AreEqual(0, hasHelp, "HasHelp returned wrong value.")
        End Sub

        ''' <summary>
        ''' A test case for IsDirty (out int).
        ''' </summary>
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub IsDirtyTest()
            Dim accessor As New CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", "z:\dir\file.ext", 2, 3, "projFile", "full line text", Nothing, Nothing)

            Dim isDirty As Integer
            Dim hr As Integer = accessor.IsDirty(isDirty)

            Assert.AreEqual(VSConstants.E_NOTIMPL, hr, "IsDirty returned wrong hresult.")
            Assert.AreEqual(0, isDirty, "IsDirty returned wrong value.")
        End Sub

        ''' <summary>
        ''' A test case for IsReadOnly (VSTASKFIELD, out int).
        ''' </summary>
        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub IsReadOnlyTest()
            Dim accessor As New CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", "z:\dir\file.ext", 2, 3, "projFile", "full line text", Nothing, Nothing)

            For Each fieldObj As Object In System.Enum.GetValues(GetType(TaskFields))
                Dim [readOnly] As Integer
                Dim hr As Integer = accessor.IsReadOnly(CType(fieldObj, VSTASKFIELD), [readOnly])

                Assert.AreEqual(1, [readOnly], "IsReadOnly returned wrong hresult for field " & (CType(fieldObj, TaskFields)).ToString())
            Next fieldObj
        End Sub

        Private NotInheritable Class AnonymousClass14
            Public line As Integer = -1
            Public col As Integer = -1

            Public Sub AnonymousMethod(ByVal sender As Object, ByVal args As MockTextView.SetCaretPosEventArgs)
                line = args.Line
                col = args.Column
            End Sub
        End Class

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub Navigate()
            Dim fileName As String = "z:\dir\file.ext"
            Dim locals As New AnonymousClass14()

            Dim accessor As New CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment", "replacement", fileName, 2, 3, "projFile", "full line text", Nothing, _serviceProvider)

            Dim openDoc As MockUIShellOpenDocument = TryCast(_serviceProvider.GetService(GetType(SVsUIShellOpenDocument)), MockUIShellOpenDocument)
            openDoc.AddDocument(fileName)

            Dim textMgr As MockTextManager = TryCast(_serviceProvider.GetService(GetType(SVsTextManager)), MockTextManager)
            Dim view As MockTextView = textMgr.AddView(fileName)

            AddHandler view.OnSetCaretPos, AddressOf locals.AnonymousMethod


            Dim hr As Integer = accessor.NavigateTo()

            Assert.AreEqual(VSConstants.S_OK, hr, "NavigateTo returned wrong hresult.")
            Assert.AreEqual(2, locals.line, "NavigateTo did not navigate to correct line.")
            Assert.AreEqual(3, locals.col, "NavigateTo did not navigate to correct column.")
        End Sub

        Private NotInheritable Class AnonymousClass15
            Public url As String = Nothing

            Public Sub AnonymousMethod(ByVal sender As Object, ByVal args As MockWebBrowsingService.NavigateEventArgs)
                url = args.Url
            End Sub
        End Class

        <DeploymentItem("VsPackage.dll"), TestMethod()> _
        Public Sub OnLinkClicked()
            Dim accessor As New CodeSweep.VSPackage.Task_Accessor("term", 1, "class", "comment: http://www.microsoft.com; some more text; http://msdn.microsoft.com.", "replacement", "z:\dir\file.ext", 2, 3, "projFile", "full line text", Nothing, _serviceProvider)
            Dim locals As New AnonymousClass15()

            Dim browser As MockWebBrowsingService = TryCast(_serviceProvider.GetService(GetType(SVsWebBrowsingService)), MockWebBrowsingService)

            AddHandler browser.OnNavigate, AddressOf locals.AnonymousMethod


            Dim hr As Integer = accessor.OnLinkClicked(System.Convert.ToInt32(TaskFields.Comment), 0)

            Assert.AreEqual(VSConstants.S_OK, hr, "OnLinkClicked returned wrong hresult for link 0.")
            Assert.AreEqual("http://www.microsoft.com", locals.url, "OnLinkClicked sent wrong url for link 0.")

            hr = accessor.OnLinkClicked(System.Convert.ToInt32(TaskFields.Comment), 1)

            Assert.AreEqual(VSConstants.S_OK, hr, "OnLinkClicked returned wrong hresult for link 1.")
            Assert.AreEqual("http://msdn.microsoft.com", locals.url, "OnLinkClicked sent wrong url for link 1.")
        End Sub

    End Class


End Namespace
