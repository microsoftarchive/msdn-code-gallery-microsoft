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
Imports System.IO
Imports Microsoft.Samples.VisualStudio.CodeSweep.Scanner

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    ''' <summary>
    ''' This is a test class for CodeSweep.Scanner.Scanner and is intended
    ''' to contain all CodeSweep.Scanner.Scanner Unit Tests.
    ''' </summary>
    <TestClass()> _
    Public Class ScannerTest


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

            CodeSweep.VSPackage.Factory_Accessor.GetBuildManager().CreatePerUserFilesAsNecessary()
        End Sub
        '
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
        End Sub
        '
#End Region

        Private NotInheritable Class AnonymousClass
            Public accessor As New CodeSweep.Scanner.Scanner_Accessor()
            Public Sub AnonymousMethod()
                accessor.Scan(Nothing, Nothing)
            End Sub

        End Class


        ''' <summary>
        ''' A test case for Scan (IEnumerable&lt;string&gt;).
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanWithNullList()
            Dim locals As New AnonymousClass()
            Dim thrown As Boolean = Utilities.HasFunctionThrown(Of ArgumentNullException)(AddressOf locals.AnonymousMethod)
            Assert.IsTrue(thrown, "Scanner.Scanner.Scan did not throw ArgumentNullException with null list.")
        End Sub

        ''' <summary>
        ''' A test case for Scan (IEnumerable&lt;string&gt;).
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanWithEmptyList()
            Dim accessor As New CodeSweep.Scanner.Scanner_Accessor()

            Dim result As IMultiFileScanResult = accessor.Scan(New List(Of String)(), New List(Of ITermTable)())

            Assert.AreEqual(result.Attempted, 0, "Attempted != 0 in return value from Scanner.Scanner.Scan with empty list.")
            Assert.AreEqual(result.FailedScan, 0, "FailedScan != 0 in return value from Scanner.Scanner.Scan with empty list.")
            Assert.AreEqual(result.PassedScan, 0, "PassedScan != 0 in return value from Scanner.Scanner.Scan with empty list.")
            Assert.AreEqual(result.UnableToScan, 0, "UnableToScan != 0 in return value from Scanner.Scanner.Scan with empty list.")
            Assert.IsNotNull(result.Results, "Results property was null in return value from Scanner.Scanner.Scan with empty list.")

            Dim count As Integer = 0
            For Each scanResult As IScanResult In result.Results
                count += 1
            Next scanResult
            Assert.AreEqual(count, 0, "Results list was not empty in return value from Scanner.Scanner.Scan with empty list.")
        End Sub

        ''' <summary>
        ''' A test case for Scan (IEnumerable&lt;string&gt;).
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanWithAllPassingFiles()
            Dim filePaths As New List(Of String)()

            filePaths.Add(Utilities.CreateTempTxtFile(""))
            filePaths.Add(Utilities.CreateTempTxtFile("some text"))
            filePaths.Add(Utilities.CreateTempTxtFile("some more text"))

            Dim accessor As New CodeSweep.Scanner.Scanner_Accessor()

            Dim result As IMultiFileScanResult = accessor.Scan(filePaths, New List(Of ITermTable)())

            Assert.AreEqual(result.Attempted, 3, "Attempted != 3 in return value from Scanner.Scanner.Scan with 3 passing files.")
            Assert.AreEqual(result.FailedScan, 0, "FailedScan != 0 in return value from Scanner.Scanner.Scan with 3 passing files.")
            Assert.AreEqual(result.PassedScan, 3, "PassedScan != 3 in return value from Scanner.Scanner.Scan with 3 passing files.")
            Assert.AreEqual(result.UnableToScan, 0, "UnableToScan != 0 in return value from Scanner.Scanner.Scan with 3 passing files.")
            Assert.IsNotNull(result.Results, "Results property was null in return value from Scanner.Scanner.Scan with 3 passing files.")

            Dim count As Integer = 0
            For Each scanResult As IScanResult In result.Results
                count += 1
            Next scanResult
            Assert.AreEqual(count, 3, "Results list did not contain 3 entries in return value from Scanner.Scanner.Scan with 3 passing files.")
        End Sub
        Private NotInheritable Class AnonymousClass11
            Public callbackResults As New List(Of IScanResult)()
            Public Sub AnonymousMethod(ByVal result As IScanResult)
                callbackResults.Add(result)
            End Sub

        End Class

        Private Sub InternalScanWithSomeFailingFiles(ByVal useCallback As Boolean)
            Dim target As IScanner = Factory.GetScanner()
            Dim locals As New AnonymousClass11()

            Dim table As New MockTermTable("file")
            table.AddSearchTerm(New MockTerm("foo", 1, "", "", "", table))
            table.AddSearchTerm(New MockTerm("bar", 1, "", "", "", table))

            Dim filePaths As New List(Of String)()

            filePaths.Add(Utilities.CreateTempTxtFile("line 1" & Constants.vbLf & "foo" & Constants.vbLf & "line 3"))
            filePaths.Add(Utilities.CreateTempTxtFile("line 1" & Constants.vbLf & "line 2" & Constants.vbLf & "line 3"))
            filePaths.Add(Utilities.CreateTempTxtFile("bar"))

            Dim scanResults As IMultiFileScanResult
            If useCallback Then

                scanResults = target.Scan(filePaths, New ITermTable() {table}, AddressOf locals.AnonymousMethod)
                Dim i As Integer = 0
                For Each result As IScanResult In scanResults.Results
                    Assert.AreEqual(locals.callbackResults(i), result)
                    i += 1
                Next result
            Else
                scanResults = target.Scan(filePaths, New ITermTable() {table})
            End If

            Assert.AreEqual(scanResults.Attempted, 3, "Attempted != 3 in return value from Scanner.Scanner.Scan with 1 passing / 2 failing files.")
            Assert.AreEqual(scanResults.FailedScan, 2, "FailedScan != 2 in return value from Scanner.Scanner.Scan with 1 passing / 2 failing files.")
            Assert.AreEqual(scanResults.PassedScan, 1, "PassedScan != 1 in return value from Scanner.Scanner.Scan with 1 passing / 2 failing files.")
            Assert.AreEqual(scanResults.UnableToScan, 0, "UnableToScan != 0 in return value from Scanner.Scanner.Scan with 1 passing / 2 failing files.")
            Assert.IsNotNull(scanResults.Results, "Results property was null in return value from Scanner.Scanner.Scan with 1 passing / 2 failing files.")

            Dim count As Integer = 0
            For Each scanResult As IScanResult In scanResults.Results
                count += 1
            Next scanResult
            Assert.AreEqual(count, 3, "Results list did not contain 3 entries in return value from Scanner.Scanner.Scan with 1 passing / 2 failing files.")
        End Sub
        Private NotInheritable Class AnonymousClass12
            Public callbackResults As New List(Of IScanResult)()
            Public Sub AnonymousMethod(ByVal result As IScanResult)
                callbackResults.Add(result)
            End Sub

        End Class
        Private Sub InternalScanWithInvalidEntries(ByVal useCallback As Boolean)
            Dim target As IScanner = Factory.GetScanner()
            Dim locals As New AnonymousClass11()
            Dim filePaths As New List(Of String)()

            ' Hold a file open with non-shared access so it can't be opened by the scanner.
            Dim holdOpen As String = Utilities.CreateTempTxtFile("some text")
            Dim file As FileStream = System.IO.File.Open(holdOpen, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None)

            filePaths.Add("z:" & Constants.vbLf & "onexistant.cpp")
            filePaths.Add(Utilities.CreateTempTxtFile("line 1" & Constants.vbLf & "line 2" & Constants.vbLf & "line 3"))
            filePaths.Add(holdOpen)

            Dim table As New MockTermTable("file")

            Dim scanResults As IMultiFileScanResult
            If useCallback Then

                scanResults = target.Scan(filePaths, New ITermTable() {table}, AddressOf locals.AnonymousMethod)

                Dim i As Integer = 0
                For Each result As IScanResult In scanResults.Results
                    Assert.AreEqual(locals.callbackResults(i), result)
                    i += 1
                Next result
            Else
                scanResults = target.Scan(filePaths, New ITermTable() {table})
            End If

            file.Close()

            Assert.AreEqual(3, scanResults.Attempted, "Attempted != 3 in return value from Scanner.Scanner.Scan with 1 passing / 2 invalid files.")
            Assert.AreEqual(0, scanResults.FailedScan, "FailedScan != 0 in return value from Scanner.Scanner.Scan with 1 passing / 2 invalid files.")
            Assert.AreEqual(1, scanResults.PassedScan, "PassedScan != 1 in return value from Scanner.Scanner.Scan with 1 passing / 2 invalid files.")
            Assert.AreEqual(2, scanResults.UnableToScan, "UnableToScan != 2 in return value from Scanner.Scanner.Scan with 1 passing / 2 invalid files.")
            Assert.IsNotNull(scanResults.Results, "Results property was null in return value from Scanner.Scanner.Scan with 1 passing / 2 invalid files.")

            Dim count As Integer = 0
            For Each scanResult As IScanResult In scanResults.Results
                count += 1
            Next scanResult
            Assert.AreEqual(count, 3, "Results list did not contain 3 entries in return value from Scanner.Scanner.Scan with 1 passing / 2 invalid files.")
        End Sub

        ''' <summary>
        ''' A test case for Scan (IEnumerable&lt;string&gt;).
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanWithSomeFailingFiles()
            InternalScanWithSomeFailingFiles(False)
        End Sub

        ''' <summary>
        ''' A test case for Scan (IEnumerable&lt;string&gt;).
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanWithSomeFailingFilesWithCallback()
            InternalScanWithSomeFailingFiles(True)
        End Sub

        ''' <summary>
        ''' A test case for Scan (IEnumerable&lt;string&gt;).
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanWithInvalidEntries()
            InternalScanWithInvalidEntries(False)
        End Sub

        ''' <summary>
        ''' A test case for Scan (IEnumerable&lt;string&gt;).
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanWithInvalidEntriesWithCallback()
            InternalScanWithInvalidEntries(True)
        End Sub

        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanWithDifferentEncodings()
            Dim filePaths As New List(Of String)()

            filePaths.Add(Utilities.CreateTempTxtFile("first file", Encoding.Unicode))
            filePaths.Add(Utilities.CreateTempTxtFile("second file", Encoding.BigEndianUnicode))
            filePaths.Add(Utilities.CreateTempTxtFile("third file", Encoding.UTF32))
            filePaths.Add(Utilities.CreateTempTxtFile("fourth file", Encoding.UTF8))
            filePaths.Add(Utilities.CreateTempTxtFile("fifth file", Encoding.UTF7))
            filePaths.Add(Utilities.CreateTempTxtFile("sixth file", Encoding.ASCII))

            ' TODO: what about different code pages within the ASCII encoding?

            Dim termTables As New List(Of ITermTable)()

            Dim table As New MockTermTable("file")
            table.AddSearchTerm(New MockTerm("first", 1, "class", "comment", "recommended", table))
            table.AddSearchTerm(New MockTerm("second", 1, "class", "comment", "recommended", table))
            table.AddSearchTerm(New MockTerm("third", 1, "class", "comment", "recommended", table))
            table.AddSearchTerm(New MockTerm("fourth", 1, "class", "comment", "recommended", table))
            table.AddSearchTerm(New MockTerm("fifth", 1, "class", "comment", "recommended", table))
            table.AddSearchTerm(New MockTerm("sixth", 1, "class", "comment", "recommended", table))

            termTables.Add(table)

            Dim accessor As New CodeSweep.Scanner.Scanner_Accessor()

            Dim result As IMultiFileScanResult = accessor.Scan(filePaths, termTables)

            Assert.AreEqual(6, result.Attempted, "Attempted count incorrect.")
            Assert.AreEqual(6, result.FailedScan, "FailedScan count incorrect.")
            Assert.AreEqual(0, result.PassedScan, "PassedScan count incorrect.")
            Assert.AreEqual(0, result.UnableToScan, "UnableToScan count incorrect.")

            Dim fileIndex As Integer = 0
            For Each scanResult As IScanResult In result.Results
                Dim count As Integer = 0
                For Each hits As IScanHit In scanResult.Results
                    count += 1
                Next hits
                Assert.AreEqual(1, count, "Result " & fileIndex.ToString() & " did not have the expected number of hits.")
                fileIndex += 1
            Next scanResult
        End Sub

        ' TODO: somewhere (either here or in another test module), we need to ensure that the
        ' hits returned from a scan have all the right attributes.

        ' TODO: test allowed vs. disallowed file extensions
    End Class
End Namespace
