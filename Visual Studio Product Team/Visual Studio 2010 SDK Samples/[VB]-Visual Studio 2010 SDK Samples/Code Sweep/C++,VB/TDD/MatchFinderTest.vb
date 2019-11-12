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

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    ''' <summary>
    ''' This is a test class for CodeSweep.Scanner.MatchFinder.MatchBase and is intended
    ''' to contain all CodeSweep.Scanner.MatchFinder.MatchBase Unit Tests.
    ''' </summary>
    <TestClass()> _
    Public Class MatchBaseTest


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


    End Class
    ''' <summary>
    ''' This is a test class for CodeSweep.Scanner.MatchFinder.ExclusionMatch and is intended
    ''' to contain all CodeSweep.Scanner.MatchFinder.ExclusionMatch Unit Tests.
    ''' </summary>
    <TestClass()> _
    Public Class MatchFinderTest


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
        '[ClassInitialize()]
        'public static void MyClassInitialize(TestContext testContext)
        '{
        '}
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
        '[TestCleanup()]
        'public void MyTestCleanup()
        '{
        '}
        '
#End Region


        Private Function InternalScanText(ByVal fileContent As String, ByVal list As IList(Of ITermTable)) As IList(Of IScanHit)
            Dim accessor As New CodeSweep.Scanner.MatchFinder_Accessor(list)

            Return InternalScanText(fileContent, accessor)
        End Function
        Private NotInheritable Class AnonymousClass8
            Public hits As New List(Of IScanHit)()

            Public Sub AnonymousMethod(ByVal term As ISearchTerm, ByVal line As Integer, ByVal column As Integer, ByVal lineText As String, ByVal warning As String)
                hits.Add(New MockScanHit("", line, column, lineText, term, warning))
            End Sub
        End Class

        Private Delegate Sub MatchFoundCallback_Shadow(ByVal term As ISearchTerm, ByVal line As Integer, ByVal column As Integer, ByVal lineText As String, ByVal warning As String)

        Private Shared Function InternalScanText(ByVal fileContent As String, ByVal accessor As CodeSweep.Scanner.MatchFinder_Accessor) As IList(Of IScanHit)
            Dim locals As New AnonymousClass8()

            Dim shadow As MatchFoundCallback_Shadow = AddressOf locals.AnonymousMethod
            Dim callback As New CodeSweep.Scanner.MatchFoundCallback_Accessor(shadow.Target, shadow.Method.MethodHandle.GetFunctionPointer())


            For Each c As Char In fileContent
                accessor.AnalyzeNextCharacter(c, callback)
            Next c

            accessor.Finish(callback)

            Return locals.hits
        End Function

        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanTextWithNoTables()
            Dim hits As IList(Of IScanHit) = InternalScanText("some text file content", New List(Of ITermTable)())
            Assert.AreEqual(0, hits.Count, "Scanning with no tables did not produce zero hits.")
        End Sub

        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanTextWithEmptyTables()
            Dim tables As New List(Of ITermTable)()
            tables.Add(New MockTermTable("file1"))
            tables.Add(New MockTermTable("file2"))
            Dim hits As IList(Of IScanHit) = InternalScanText("some text file content", tables)
            Assert.AreEqual(0, hits.Count, "Scanning with empty tables did not produce zero hits.")
        End Sub

        ''' <summary>
        ''' Tests finding a search term that is a single character in length.
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanTextWithSingleCharTermHit()
            Dim table As New MockTermTable("file1")
            Dim term As New MockTerm("d", 1, "fooClass", "fooComment", "fooRecommended", table)
            table.AddSearchTerm(term)
            Dim hits As IList(Of IScanHit) = InternalScanText("the term I want to find" & Constants.vbLf & "is ""d"".  Is it here?", New ITermTable() {table})
            Assert.AreEqual(1, hits.Count, "Failed to find a single-character term.")
            Assert.AreEqual(term, hits(0).Term, "Term property of hit is incorrect.")
            Assert.AreEqual(1, hits(0).Line, "Line property of hit is incorrect.")
            Assert.AreEqual("is ""d"".  Is it here?", hits(0).LineText, "LineText property of hit is incorrect.")
            Assert.AreEqual(4, hits(0).Column, "Column property of hit is incorrect.")
        End Sub

        Private Function TestTokenMatchWithSpecificSurround(ByVal before As String, ByVal after As String) As Boolean
            Dim table As New MockTermTable("file1")
            Dim term As New MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table)
            table.AddSearchTerm(term)
            Dim hits As IList(Of IScanHit) = InternalScanText(before & "foo" & after, New ITermTable() {table})
            Return hits.Count = 1
        End Function

        ''' <summary>
        ''' Test different valid and invalid separators for tokens in the text.
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub TestTokenSeparators()
            For Each bookEnd As String In New String() {"a", "3", "_"}
                Assert.IsFalse(TestTokenMatchWithSpecificSurround(bookEnd, bookEnd), "A term surrounded with '" & bookEnd & "' was matched.")
            Next bookEnd

            For Each bookEnd As String In New String() {"!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "-", "=", "+", "|", "\", "{", "}", "[", "]", ",", ".", "/", "?", ":", ";", "'", """", "<", ">", " ", Constants.vbTab, "`", "~"}
                Assert.IsTrue(TestTokenMatchWithSpecificSurround(bookEnd, bookEnd), "A term surrounded with '" & bookEnd & "' was not matched.")
                Assert.IsFalse(TestTokenMatchWithSpecificSurround(bookEnd, "x"), "A term preceeded with '" & bookEnd & "' and followed by 'x' was matched.")
                Assert.IsFalse(TestTokenMatchWithSpecificSurround("x", bookEnd), "A term preceeded with 'x' and followed by '" & bookEnd & "' was matched.")
            Next bookEnd
        End Sub

        ''' <summary>
        ''' Tests finding a search term that has another embedded inside it (e.g. the two terms are
        ''' "abcdef" and "bcd".
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanTextWithEmbeddedTermHit()
            Dim table As New MockTermTable("file1")
            Dim bigTerm As New MockTerm("a bcd ef", 1, "fooClass", "fooComment", "fooRecommended", table)
            Dim smallTerm As New MockTerm("bcd", 1, "fooClass", "fooComment", "fooRecommended", table)
            table.AddSearchTerm(bigTerm)
            table.AddSearchTerm(smallTerm)
            Dim hits As IList(Of IScanHit) = InternalScanText("foo a bcd ef bar", New ITermTable() {table})
            Assert.AreEqual(1, hits.Count, "Search returned wrong number of hits.")
            Assert.AreEqual(bigTerm, hits(0).Term, "Term property of hit is incorrect.")
            Assert.AreEqual(0, hits(0).Line, "Line property of hit is incorrect.")
            Assert.AreEqual("foo a bcd ef bar", hits(0).LineText, "LineText property of hit is incorrect.")
            Assert.AreEqual(4, hits(0).Column, "Column property of hit is incorrect.")
        End Sub

        ''' <summary>
        ''' Test scanning with n search terms which begin at the same location; the longest one
        ''' that matches should be returned.
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanTextWithOverlappingTermHit()
            Dim table As New MockTermTable("file1")
            Dim bigTerm As New MockTerm("abc def", 1, "fooClass", "fooComment", "fooRecommended", table)
            Dim smallTerm As New MockTerm("abc", 1, "fooClass", "fooComment", "fooRecommended", table)
            table.AddSearchTerm(bigTerm)
            table.AddSearchTerm(smallTerm)
            Dim hits As IList(Of IScanHit) = InternalScanText("foo abc def bar", New ITermTable() {table})
            Assert.AreEqual(1, hits.Count, "Search returned wrong number of hits.")
            Assert.AreEqual(bigTerm, hits(0).Term, "Term property of hit is incorrect.")
            Assert.AreEqual(0, hits(0).Line, "Line property of hit is incorrect.")
            Assert.AreEqual("foo abc def bar", hits(0).LineText, "LineText property of hit is incorrect.")
            Assert.AreEqual(4, hits(0).Column, "Column property of hit is incorrect.")
        End Sub

        ''' <summary>
        ''' Make sure an exclusion is detected properly when the term appears at the very beginning
        ''' of the exclusion.
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanTextWithTermAtStartOfExclusion()
            Dim table As New MockTermTable("file1")
            Dim term As New MockTerm("abcdef", 1, "fooClass", "fooComment", "fooRecommended", table)
            term.AddExclusion("abcdef is not a valid term")
            table.AddSearchTerm(term)
            Dim hits As IList(Of IScanHit) = InternalScanText("foo abcdef is not a valid term", New ITermTable() {table})
            Assert.AreEqual(0, hits.Count, "Search returned wrong number of hits.")
        End Sub

        ''' <summary>
        ''' Make sure an exclusion is detected properly when the term appears at the very end of
        ''' the exclusion.
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanTextWithTermAtEndOfExclusion()
            Dim table As New MockTermTable("file1")
            Dim term As New MockTerm("abcdef", 1, "fooClass", "fooComment", "fooRecommended", table)
            term.AddExclusion("the text at the end of this exclusion is abcdef")
            table.AddSearchTerm(term)
            Dim hits As IList(Of IScanHit) = InternalScanText("the text at the end of this exclusion is abcdef", New ITermTable() {table})
            Assert.AreEqual(0, hits.Count, "Search returned wrong number of hits.")
        End Sub

        ''' <summary>
        ''' Make sure the exclusion is detected properly when it appears in the middle of the
        ''' exclusion text (not at the beginning or end).
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanTextWithTermInMiddleOfExclusion()
            Dim table As New MockTermTable("file1")
            Dim term As New MockTerm("abcdef", 1, "fooClass", "fooComment", "fooRecommended", table)
            term.AddExclusion("this time the term (abcdef) is in the middle")
            table.AddSearchTerm(term)
            Dim hits As IList(Of IScanHit) = InternalScanText("foo - this time the term (abcdef) is in the middle - bar", New ITermTable() {table})
            Assert.AreEqual(0, hits.Count, "Search returned wrong number of hits.")
        End Sub

        ''' <summary>
        ''' If an exclusion contains the same text multiple times, make sure it is excluded
        ''' properly (i.e. none of the instances counts as a match).
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanTextWithTermAppearingMultipleTimesInExclusion()
            Dim table As New MockTermTable("file1")
            Dim term As New MockTerm("smurf", 1, "fooClass", "fooComment", "fooRecommended", table)
            term.AddExclusion("a smurf a day keeps the smurf away")
            table.AddSearchTerm(term)
            Dim hits As IList(Of IScanHit) = InternalScanText("foo - a smurf a day keeps the smurf away - bar", New ITermTable() {table})
            Assert.AreEqual(0, hits.Count, "Search returned wrong number of hits.")
        End Sub

        ''' <summary>
        ''' Make sure exclusions at the end and middle of a term's exclusion list are matched as
        ''' are the ones at the beginning.
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanTextWithMultipleExclusions()
            Dim table As New MockTermTable("file1")
            Dim term As New MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table)
            term.AddExclusion("first foo")
            term.AddExclusion("second foo")
            term.AddExclusion("third foo")
            table.AddSearchTerm(term)
            Dim hits As IList(Of IScanHit) = InternalScanText("this first-foo will not exclude, but this second foo will, as will this third foo", New ITermTable() {table})
            Assert.AreEqual(1, hits.Count, "Search returned wrong number of hits.")
            Assert.AreEqual(term, hits(0).Term, "Term property of hit is incorrect.")
            Assert.AreEqual(0, hits(0).Line, "Line property of hit is incorrect.")
            Assert.AreEqual(11, hits(0).Column, "Column property of hit is incorrect.")
        End Sub

        ''' <summary>
        ''' If the exclusion for a term includes other terms that match, make sure they are not
        ''' excluded.  E.g. if
        '''     term1 = "foo"
        '''     exclusion1 = "a foo is not a bar"
        '''     term2 = "bar"
        '''     the full text is equal to exclusion1
        ''' term1 should be excluded but term2 should be a valid hit.
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanTextWithOtherTermsAppearingInExclusion()
            Dim table As New MockTermTable("file1")
            Dim term1 As New MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table)
            term1.AddExclusion("a foo is not a bar")
            Dim term2 As New MockTerm("bar", 1, "barClass", "barComment", "fooRecommended", table)
            table.AddSearchTerm(term1)
            table.AddSearchTerm(term2)
            Dim hits As IList(Of IScanHit) = InternalScanText("a foo is not a bar", New ITermTable() {table})
            Assert.AreEqual(1, hits.Count, "Search returned wrong number of hits.")
            Assert.AreEqual(term2, hits(0).Term, "Term property of hit is incorrect.")
            Assert.AreEqual(0, hits(0).Line, "Line property of hit is incorrect.")
            Assert.AreEqual(15, hits(0).Column, "Column property of hit is incorrect.")
        End Sub

        ''' <summary>
        ''' Test multiple terms with overlapping partially-matched exclusions when the exclusion
        ''' for the second term is invalidated before the first.  In that case, the first term
        ''' should be returned first even though the second term was discovered first.
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanTextWithOverlappingExclusionsWhenSecondExclusionIsInvalidatedFirst()
            Dim table As New MockTermTable("file1")
            Dim term1 As New MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table)
            term1.AddExclusion("abc foo then bar and some more text")
            Dim term2 As New MockTerm("bar", 1, "barClass", "barComment", "fooRecommended", table)
            term2.AddExclusion("foo then bar and some more")
            table.AddSearchTerm(term1)
            table.AddSearchTerm(term2)
            Dim hits As IList(Of IScanHit) = InternalScanText("abc foo then bar and some morINVALIDATED", New ITermTable() {table})
            Assert.AreEqual(2, hits.Count, "Search returned wrong number of hits.")
            Assert.AreEqual(term1, hits(0).Term, "Term property of hit 0 is incorrect.")
            Assert.AreEqual(0, hits(0).Line, "Line property of hit 0 is incorrect.")
            Assert.AreEqual(4, hits(0).Column, "Column property of hit 0 is incorrect.")
            Assert.AreEqual(term2, hits(1).Term, "Term property of hit 1 is incorrect.")
            Assert.AreEqual(0, hits(1).Line, "Line property of hit 1 is incorrect.")
            Assert.AreEqual(13, hits(1).Column, "Column property of hit 1 is incorrect.")
        End Sub

        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanTextWithDifferentNewlines()
            Dim table As New MockTermTable("file1")
            Dim term As New MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table)
            table.AddSearchTerm(term)
            Dim hits As IList(Of IScanHit) = InternalScanText(Constants.vbLf & "this foo is on line 1" & Constants.vbCr & "this foo is on line 2" & Constants.vbCrLf & "this foo is on line 3" & Constants.vbLf + Constants.vbCr & "this foo is on line 5" & Constants.vbLf + Constants.vbLf & "this foo is on line 7" & Constants.vbCr + Constants.vbCr & "this foo is on line 9", New ITermTable() {table})
            Assert.AreEqual(6, hits.Count, "Search returned wrong number of hits.")
            Assert.AreEqual(1, hits(0).Line, "Line property of hit 0 is incorrect.")
            Assert.AreEqual("this foo is on line 1", hits(0).LineText, "LineText property of hit 0 is incorrect.")
            Assert.AreEqual(2, hits(1).Line, "Line property of hit 1 is incorrect.")
            Assert.AreEqual("this foo is on line 2", hits(1).LineText, "LineText property of hit 1 is incorrect.")
            Assert.AreEqual(3, hits(2).Line, "Line property of hit 2 is incorrect.")
            Assert.AreEqual("this foo is on line 3", hits(2).LineText, "LineText property of hit 2 is incorrect.")
            Assert.AreEqual(5, hits(3).Line, "Line property of hit 3 is incorrect.")
            Assert.AreEqual("this foo is on line 5", hits(3).LineText, "LineText property of hit 3 is incorrect.")
            Assert.AreEqual(7, hits(4).Line, "Line property of hit 4 is incorrect.")
            Assert.AreEqual("this foo is on line 7", hits(4).LineText, "LineText property of hit 4 is incorrect.")
            Assert.AreEqual(9, hits(5).Line, "Line property of hit 5 is incorrect.")
            Assert.AreEqual("this foo is on line 9", hits(5).LineText, "LineText property of hit 5 is incorrect.")
        End Sub

        ''' <summary>
        ''' Test matching a term immediately adjacent to another, with no intervening characters.
        ''' </summary>
        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanTextWithAdjacentTerms()
            Dim table As New MockTermTable("file1")
            Dim term1 As New MockTerm("foo.", 1, "fooClass", "fooComment", "fooRecommended", table)
            Dim term2 As New MockTerm(".bar", 1, "barClass", "barComment", "fooRecommended", table)
            table.AddSearchTerm(term1)
            table.AddSearchTerm(term2)
            Dim hits As IList(Of IScanHit) = InternalScanText("foo..bar", New ITermTable() {table})
            Assert.AreEqual(2, hits.Count, "Search returned wrong number of hits.")
            Assert.AreEqual(term1, hits(0).Term, "Term property of hit 0 is incorrect.")
            Assert.AreEqual(0, hits(0).Line, "Line property of hit 0 is incorrect.")
            Assert.AreEqual(0, hits(0).Column, "Column property of hit 0 is incorrect.")
            Assert.AreEqual(term2, hits(1).Term, "Term property of hit 1 is incorrect.")
            Assert.AreEqual(0, hits(1).Line, "Line property of hit 1 is incorrect.")
            Assert.AreEqual(4, hits(1).Column, "Column property of hit 1 is incorrect.")
        End Sub

        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanTextWithExclusionPresentButSeparateFromTerm()
            Dim table As New MockTermTable("file1")
            Dim term As New MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table)
            term.AddExclusion("this foo is excluded")
            table.AddSearchTerm(term)
            Dim hits As IList(Of IScanHit) = InternalScanText("this foo is excluded; this foo is not", New ITermTable() {table})
            Assert.AreEqual(1, hits.Count, "Search returned wrong number of hits.")
            Assert.AreEqual(term, hits(0).Term, "Term property of hit 0 is incorrect.")
            Assert.AreEqual(0, hits(0).Line, "Line property of hit 0 is incorrect.")
            Assert.AreEqual(27, hits(0).Column, "Column property of hit 0 is incorrect.")
        End Sub

        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanTextWithTermPartiallyOverlappingExclusion()
            Dim table As New MockTermTable("file1")
            Dim term As New MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table)
            term.AddExclusion("a foo is not a fo")
            table.AddSearchTerm(term)
            Dim hits As IList(Of IScanHit) = InternalScanText("a foo is not a foo bar", New ITermTable() {table})
            Assert.AreEqual(1, hits.Count, "Search returned wrong number of hits.")
            Assert.AreEqual(term, hits(0).Term, "Term property of hit 0 is incorrect.")
            Assert.AreEqual(0, hits(0).Line, "Line property of hit 0 is incorrect.")
            Assert.AreEqual(15, hits(0).Column, "Column property of hit 0 is incorrect.")
        End Sub

        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanTextWithDifferentCase()
            Dim table As New MockTermTable("file1")
            Dim term As New MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table)
            term.AddExclusion("this Foo is excluded")
            table.AddSearchTerm(term)
            Dim hits As IList(Of IScanHit) = InternalScanText("THIS FOO IS EXCLUDED, BUT THIS FOO IS NOT", New ITermTable() {table})
            Assert.AreEqual(1, hits.Count, "Search returned wrong number of hits.")
            Assert.AreEqual(term, hits(0).Term, "Term property of hit 0 is incorrect.")
            Assert.AreEqual(0, hits(0).Line, "Line property of hit 0 is incorrect.")
            Assert.AreEqual(31, hits(0).Column, "Column property of hit 0 is incorrect.")
        End Sub

        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub ScanTextWithDuplicateTerms()
            Dim table1 As New MockTermTable("file1")
            Dim term1 As New MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table1)
            term1.AddExclusion("foo exclusion 1")
            term1.AddExclusion("foo exclusion 2")
            table1.AddSearchTerm(term1)

            Dim table2 As New MockTermTable("file1")
            Dim term2 As New MockTerm("foo", 1, "fooClass2", "fooComment2", "fooRecommended", table2)
            term2.AddExclusion("foo exclusion 2")
            term2.AddExclusion("foo exclusion 3")
            table2.AddSearchTerm(term2)

            Dim hits As IList(Of IScanHit) = InternalScanText("'foo exclusion 2' should be excluded, 'foo exclusion 1' and 'foo exclusion 3' should not", New ITermTable() {table1, table2})
            Assert.AreEqual(2, hits.Count, "Search returned wrong number of hits.")
            Assert.AreEqual(term2, hits(0).Term, "Term property of hit 0 is incorrect.")
            Assert.AreEqual(0, hits(0).Line, "Line property of hit 0 is incorrect.")
            Assert.AreEqual(39, hits(0).Column, "Column property of hit 0 is incorrect.")
            Assert.IsNotNull(hits(0).Warning, "Hit 0 did not have a warning.")
            Assert.AreEqual(term1, hits(1).Term, "Term property of hit 1 is incorrect.")
            Assert.AreEqual(0, hits(1).Line, "Line property of hit 1 is incorrect.")
            Assert.AreEqual(61, hits(1).Column, "Column property of hit 1 is incorrect.")
            Assert.IsNotNull(hits(1).Warning, "Hit 0 did not have a warning.")
        End Sub

        <DeploymentItem("Scanner.dll"), TestMethod()> _
        Public Sub TestReset()
            Dim table As New MockTermTable("file1")
            Dim term As New MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table)
            table.AddSearchTerm(term)

            Dim accessor As New CodeSweep.Scanner.MatchFinder_Accessor(New ITermTable() {table})

            Dim hits1 As IList(Of IScanHit) = InternalScanText("foo bar ", accessor)

            Assert.AreEqual(1, hits1.Count, "First search returned wrong number of hits.")
            Assert.AreEqual(0, hits1(0).Column, "Column property of first hit is incorrect.")

            Dim hits2 As IList(Of IScanHit) = InternalScanText("foo bar", accessor)

            ' This is what happens if you DON'T call Reset...
            Assert.AreEqual(1, hits2.Count, "Second search returned wrong number of hits.")
            Assert.AreEqual(8, hits2(0).Column, "Column property of second hit is incorrect.")

            accessor.Reset()

            Dim hits3 As IList(Of IScanHit) = InternalScanText("foo bar", accessor)

            Assert.AreEqual(1, hits3.Count, "Third search returned wrong number of hits.")
            Assert.AreEqual(0, hits3(0).Column, "Column property of third hit is incorrect.")
        End Sub

    End Class


End Namespace
