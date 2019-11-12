/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Samples.VisualStudio.CodeSweep.Scanner;
using CodeSweep = Microsoft.Samples.VisualStudio.CodeSweep;
using System.Reflection;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    /// <summary>
    ///This is a test class for CodeSweep.Scanner.MatchFinder.MatchBase and is intended
    ///to contain all CodeSweep.Scanner.MatchFinder.MatchBase Unit Tests
    ///</summary>
    [TestClass()]
    public class MatchBaseTest
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


    }
    /// <summary>
    ///This is a test class for CodeSweep.Scanner.MatchFinder.ExclusionMatch and is intended
    ///to contain all CodeSweep.Scanner.MatchFinder.ExclusionMatch Unit Tests
    ///</summary>
    [TestClass()]
    public class MatchFinderTest
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
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
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
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        private IList<IScanHit> InternalScanText(string fileContent, IList<ITermTable> list)
        {
            CodeSweep.Scanner.MatchFinder_Accessor accessor = new CodeSweep.Scanner.MatchFinder_Accessor(list);

            return InternalScanText(fileContent, accessor);
        }

        private delegate void MatchFoundCallback_Shadow(ISearchTerm term, int line, int column, string lineText, string warning);

        private static IList<IScanHit> InternalScanText(string fileContent, CodeSweep.Scanner.MatchFinder_Accessor accessor)
        {
            List<IScanHit> hits = new List<IScanHit>();

            MatchFoundCallback_Shadow shadow = delegate(ISearchTerm term, int line, int column, string lineText, string warning)
                    {
                        hits.Add(new MockScanHit("", line, column, lineText, term, warning));
                    };

            CodeSweep.Scanner.MatchFoundCallback_Accessor callback = new MatchFoundCallback_Accessor(shadow.Target, shadow.Method.MethodHandle.GetFunctionPointer());

            foreach (char c in fileContent)
            {
                accessor.AnalyzeNextCharacter(c, callback);
            }

            accessor.Finish(callback);
            
            return hits;
        }

        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void ScanTextWithNoTables()
        {
            IList<IScanHit> hits = InternalScanText("some text file content", new List<ITermTable>());
            Assert.AreEqual(0, hits.Count, "Scanning with no tables did not produce zero hits.");
        }

        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void ScanTextWithEmptyTables()
        {
            List<ITermTable> tables = new List<ITermTable>();
            tables.Add(new MockTermTable("file1"));
            tables.Add(new MockTermTable("file2"));
            IList<IScanHit> hits = InternalScanText("some text file content", tables);
            Assert.AreEqual(0, hits.Count, "Scanning with empty tables did not produce zero hits.");
        }

        /// <summary>
        /// Tests finding a search term that is a single character in length.
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void ScanTextWithSingleCharTermHit()
        {
            MockTermTable table = new MockTermTable("file1");
            MockTerm term = new MockTerm("d", 1, "fooClass", "fooComment", "fooRecommended", table);
            table.AddSearchTerm(term);
            IList<IScanHit> hits = InternalScanText("the term I want to find\nis \"d\".  Is it here?", new ITermTable[] { table });
            Assert.AreEqual(1, hits.Count, "Failed to find a single-character term.");
            Assert.AreEqual(term, hits[0].Term, "Term property of hit is incorrect.");
            Assert.AreEqual(1, hits[0].Line, "Line property of hit is incorrect.");
            Assert.AreEqual("is \"d\".  Is it here?", hits[0].LineText, "LineText property of hit is incorrect.");
            Assert.AreEqual(4, hits[0].Column, "Column property of hit is incorrect.");
        }

        private bool TestTokenMatchWithSpecificSurround(string before, string after)
        {
            MockTermTable table = new MockTermTable("file1");
            MockTerm term = new MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table);
            table.AddSearchTerm(term);
            IList<IScanHit> hits = InternalScanText(before + "foo" + after, new ITermTable[] { table });
            return hits.Count == 1;
        }

        /// <summary>
        /// Test different valid and invalid separators for tokens in the text.
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void TestTokenSeparators()
        {
            foreach (string bookEnd in new string[] { "a", "3", "_" })
            {
                Assert.IsFalse(TestTokenMatchWithSpecificSurround(bookEnd, bookEnd), "A term surrounded with '" + bookEnd + "' was matched.");
            }

            foreach (string bookEnd in new string[] { "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "-", "=", "+", "|", "\\", "{", "}", "[", "]", ",", ".", "/", "?", ":", ";", "'", "\"", "<", ">", " ", "\t", "`", "~" })
            {
                Assert.IsTrue(TestTokenMatchWithSpecificSurround(bookEnd, bookEnd), "A term surrounded with '" + bookEnd + "' was not matched.");
                Assert.IsFalse(TestTokenMatchWithSpecificSurround(bookEnd, "x"), "A term preceeded with '" + bookEnd + "' and followed by 'x' was matched.");
                Assert.IsFalse(TestTokenMatchWithSpecificSurround("x", bookEnd), "A term preceeded with 'x' and followed by '" + bookEnd + "' was matched.");
            }
        }

        /// <summary>
        /// Tests finding a search term that has another embedded inside it (e.g. the two terms are
        /// "abcdef" and "bcd".
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void ScanTextWithEmbeddedTermHit()
        {
            MockTermTable table = new MockTermTable("file1");
            MockTerm bigTerm = new MockTerm("a bcd ef", 1, "fooClass", "fooComment", "fooRecommended", table);
            MockTerm smallTerm = new MockTerm("bcd", 1, "fooClass", "fooComment", "fooRecommended", table);
            table.AddSearchTerm(bigTerm);
            table.AddSearchTerm(smallTerm);
            IList<IScanHit> hits = InternalScanText("foo a bcd ef bar", new ITermTable[] { table });
            Assert.AreEqual(1, hits.Count, "Search returned wrong number of hits.");
            Assert.AreEqual(bigTerm, hits[0].Term, "Term property of hit is incorrect.");
            Assert.AreEqual(0, hits[0].Line, "Line property of hit is incorrect.");
            Assert.AreEqual("foo a bcd ef bar", hits[0].LineText, "LineText property of hit is incorrect.");
            Assert.AreEqual(4, hits[0].Column, "Column property of hit is incorrect.");
        }

        /// <summary>
        /// Test scanning with n search terms which begin at the same location; the longest one
        /// that matches should be returned.
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void ScanTextWithOverlappingTermHit()
        {
            MockTermTable table = new MockTermTable("file1");
            MockTerm bigTerm = new MockTerm("abc def", 1, "fooClass", "fooComment", "fooRecommended", table);
            MockTerm smallTerm = new MockTerm("abc", 1, "fooClass", "fooComment", "fooRecommended", table);
            table.AddSearchTerm(bigTerm);
            table.AddSearchTerm(smallTerm);
            IList<IScanHit> hits = InternalScanText("foo abc def bar", new ITermTable[] { table });
            Assert.AreEqual(1, hits.Count, "Search returned wrong number of hits.");
            Assert.AreEqual(bigTerm, hits[0].Term, "Term property of hit is incorrect.");
            Assert.AreEqual(0, hits[0].Line, "Line property of hit is incorrect.");
            Assert.AreEqual("foo abc def bar", hits[0].LineText, "LineText property of hit is incorrect.");
            Assert.AreEqual(4, hits[0].Column, "Column property of hit is incorrect.");
        }

        /// <summary>
        /// Make sure an exclusion is detected properly when the term appears at the very beginning
        /// of the exclusion.
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void ScanTextWithTermAtStartOfExclusion()
        {
            MockTermTable table = new MockTermTable("file1");
            MockTerm term = new MockTerm("abcdef", 1, "fooClass", "fooComment", "fooRecommended", table);
            term.AddExclusion("abcdef is not a valid term");
            table.AddSearchTerm(term);
            IList<IScanHit> hits = InternalScanText("foo abcdef is not a valid term", new ITermTable[] { table });
            Assert.AreEqual(0, hits.Count, "Search returned wrong number of hits.");
        }

        /// <summary>
        /// Make sure an exclusion is detected properly when the term appears at the very end of
        /// the exclusion.
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void ScanTextWithTermAtEndOfExclusion()
        {
            MockTermTable table = new MockTermTable("file1");
            MockTerm term = new MockTerm("abcdef", 1, "fooClass", "fooComment", "fooRecommended", table);
            term.AddExclusion("the text at the end of this exclusion is abcdef");
            table.AddSearchTerm(term);
            IList<IScanHit> hits = InternalScanText("the text at the end of this exclusion is abcdef", new ITermTable[] { table });
            Assert.AreEqual(0, hits.Count, "Search returned wrong number of hits.");
        }

        /// <summary>
        /// Make sure the exclusion is detected properly when it appears in the middle of the
        /// exclusion text (not at the beginning or end).
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void ScanTextWithTermInMiddleOfExclusion()
        {
            MockTermTable table = new MockTermTable("file1");
            MockTerm term = new MockTerm("abcdef", 1, "fooClass", "fooComment", "fooRecommended", table);
            term.AddExclusion("this time the term (abcdef) is in the middle");
            table.AddSearchTerm(term);
            IList<IScanHit> hits = InternalScanText("foo - this time the term (abcdef) is in the middle - bar", new ITermTable[] { table });
            Assert.AreEqual(0, hits.Count, "Search returned wrong number of hits.");
        }

        /// <summary>
        /// If an exclusion contains the same text multiple times, make sure it is excluded
        /// properly (i.e. none of the instances counts as a match).
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void ScanTextWithTermAppearingMultipleTimesInExclusion()
        {
            MockTermTable table = new MockTermTable("file1");
            MockTerm term = new MockTerm("smurf", 1, "fooClass", "fooComment", "fooRecommended", table);
            term.AddExclusion("a smurf a day keeps the smurf away");
            table.AddSearchTerm(term);
            IList<IScanHit> hits = InternalScanText("foo - a smurf a day keeps the smurf away - bar", new ITermTable[] { table });
            Assert.AreEqual(0, hits.Count, "Search returned wrong number of hits.");
        }

        /// <summary>
        /// Make sure exclusions at the end and middle of a term's exclusion list are matched as
        /// are the ones at the beginning.
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void ScanTextWithMultipleExclusions()
        {
            MockTermTable table = new MockTermTable("file1");
            MockTerm term = new MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table);
            term.AddExclusion("first foo");
            term.AddExclusion("second foo");
            term.AddExclusion("third foo");
            table.AddSearchTerm(term);
            IList<IScanHit> hits = InternalScanText("this first-foo will not exclude, but this second foo will, as will this third foo", new ITermTable[] { table });
            Assert.AreEqual(1, hits.Count, "Search returned wrong number of hits.");
            Assert.AreEqual(term, hits[0].Term, "Term property of hit is incorrect.");
            Assert.AreEqual(0, hits[0].Line, "Line property of hit is incorrect.");
            Assert.AreEqual(11, hits[0].Column, "Column property of hit is incorrect.");
        }

        /// <summary>
        /// If the exclusion for a term includes other terms that match, make sure they are not
        /// excluded.  E.g. if
        ///     term1 = "foo"
        ///     exclusion1 = "a foo is not a bar"
        ///     term2 = "bar"
        ///     the full text is equal to exclusion1
        /// term1 should be excluded but term2 should be a valid hit.
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void ScanTextWithOtherTermsAppearingInExclusion()
        {
            MockTermTable table = new MockTermTable("file1");
            MockTerm term1 = new MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table);
            term1.AddExclusion("a foo is not a bar");
            MockTerm term2 = new MockTerm("bar", 1, "barClass", "barComment", "fooRecommended", table);
            table.AddSearchTerm(term1);
            table.AddSearchTerm(term2);
            IList<IScanHit> hits = InternalScanText("a foo is not a bar", new ITermTable[] { table });
            Assert.AreEqual(1, hits.Count, "Search returned wrong number of hits.");
            Assert.AreEqual(term2, hits[0].Term, "Term property of hit is incorrect.");
            Assert.AreEqual(0, hits[0].Line, "Line property of hit is incorrect.");
            Assert.AreEqual(15, hits[0].Column, "Column property of hit is incorrect.");
        }

        /// <summary>
        /// Test multiple terms with overlapping partially-matched exclusions when the exclusion
        /// for the second term is invalidated before the first.  In that case, the first term
        /// should be returned first even though the second term was discovered first.
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void ScanTextWithOverlappingExclusionsWhenSecondExclusionIsInvalidatedFirst()
        {
            MockTermTable table = new MockTermTable("file1");
            MockTerm term1 = new MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table);
            term1.AddExclusion("abc foo then bar and some more text");
            MockTerm term2 = new MockTerm("bar", 1, "barClass", "barComment", "fooRecommended", table);
            term2.AddExclusion("foo then bar and some more");
            table.AddSearchTerm(term1);
            table.AddSearchTerm(term2);
            IList<IScanHit> hits = InternalScanText("abc foo then bar and some morINVALIDATED", new ITermTable[] { table });
            Assert.AreEqual(2, hits.Count, "Search returned wrong number of hits.");
            Assert.AreEqual(term1, hits[0].Term, "Term property of hit 0 is incorrect.");
            Assert.AreEqual(0, hits[0].Line, "Line property of hit 0 is incorrect.");
            Assert.AreEqual(4, hits[0].Column, "Column property of hit 0 is incorrect.");
            Assert.AreEqual(term2, hits[1].Term, "Term property of hit 1 is incorrect.");
            Assert.AreEqual(0, hits[1].Line, "Line property of hit 1 is incorrect.");
            Assert.AreEqual(13, hits[1].Column, "Column property of hit 1 is incorrect.");
        }

        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void ScanTextWithDifferentNewlines()
        {
            MockTermTable table = new MockTermTable("file1");
            MockTerm term = new MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table);
            table.AddSearchTerm(term);
            IList<IScanHit> hits = InternalScanText("\nthis foo is on line 1\rthis foo is on line 2\r\nthis foo is on line 3\n\rthis foo is on line 5\n\nthis foo is on line 7\r\rthis foo is on line 9", new ITermTable[] { table });
            Assert.AreEqual(6, hits.Count, "Search returned wrong number of hits.");
            Assert.AreEqual(1, hits[0].Line, "Line property of hit 0 is incorrect.");
            Assert.AreEqual("this foo is on line 1", hits[0].LineText, "LineText property of hit 0 is incorrect.");
            Assert.AreEqual(2, hits[1].Line, "Line property of hit 1 is incorrect.");
            Assert.AreEqual("this foo is on line 2", hits[1].LineText, "LineText property of hit 1 is incorrect.");
            Assert.AreEqual(3, hits[2].Line, "Line property of hit 2 is incorrect.");
            Assert.AreEqual("this foo is on line 3", hits[2].LineText, "LineText property of hit 2 is incorrect.");
            Assert.AreEqual(5, hits[3].Line, "Line property of hit 3 is incorrect.");
            Assert.AreEqual("this foo is on line 5", hits[3].LineText, "LineText property of hit 3 is incorrect.");
            Assert.AreEqual(7, hits[4].Line, "Line property of hit 4 is incorrect.");
            Assert.AreEqual("this foo is on line 7", hits[4].LineText, "LineText property of hit 4 is incorrect.");
            Assert.AreEqual(9, hits[5].Line, "Line property of hit 5 is incorrect.");
            Assert.AreEqual("this foo is on line 9", hits[5].LineText, "LineText property of hit 5 is incorrect.");
        }

        /// <summary>
        /// Test matching a term immediately adjacent to another, with no intervening characters.
        /// </summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void ScanTextWithAdjacentTerms()
        {
            MockTermTable table = new MockTermTable("file1");
            MockTerm term1 = new MockTerm("foo.", 1, "fooClass", "fooComment", "fooRecommended", table);
            MockTerm term2 = new MockTerm(".bar", 1, "barClass", "barComment", "fooRecommended", table);
            table.AddSearchTerm(term1);
            table.AddSearchTerm(term2);
            IList<IScanHit> hits = InternalScanText("foo..bar", new ITermTable[] { table });
            Assert.AreEqual(2, hits.Count, "Search returned wrong number of hits.");
            Assert.AreEqual(term1, hits[0].Term, "Term property of hit 0 is incorrect.");
            Assert.AreEqual(0, hits[0].Line, "Line property of hit 0 is incorrect.");
            Assert.AreEqual(0, hits[0].Column, "Column property of hit 0 is incorrect.");
            Assert.AreEqual(term2, hits[1].Term, "Term property of hit 1 is incorrect.");
            Assert.AreEqual(0, hits[1].Line, "Line property of hit 1 is incorrect.");
            Assert.AreEqual(4, hits[1].Column, "Column property of hit 1 is incorrect.");
        }

        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void ScanTextWithExclusionPresentButSeparateFromTerm()
        {
            MockTermTable table = new MockTermTable("file1");
            MockTerm term = new MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table);
            term.AddExclusion("this foo is excluded");
            table.AddSearchTerm(term);
            IList<IScanHit> hits = InternalScanText("this foo is excluded; this foo is not", new ITermTable[] { table });
            Assert.AreEqual(1, hits.Count, "Search returned wrong number of hits.");
            Assert.AreEqual(term, hits[0].Term, "Term property of hit 0 is incorrect.");
            Assert.AreEqual(0, hits[0].Line, "Line property of hit 0 is incorrect.");
            Assert.AreEqual(27, hits[0].Column, "Column property of hit 0 is incorrect.");
        }

        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void ScanTextWithTermPartiallyOverlappingExclusion()
        {
            MockTermTable table = new MockTermTable("file1");
            MockTerm term = new MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table);
            term.AddExclusion("a foo is not a fo");
            table.AddSearchTerm(term);
            IList<IScanHit> hits = InternalScanText("a foo is not a foo bar", new ITermTable[] { table });
            Assert.AreEqual(1, hits.Count, "Search returned wrong number of hits.");
            Assert.AreEqual(term, hits[0].Term, "Term property of hit 0 is incorrect.");
            Assert.AreEqual(0, hits[0].Line, "Line property of hit 0 is incorrect.");
            Assert.AreEqual(15, hits[0].Column, "Column property of hit 0 is incorrect.");
        }

        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void ScanTextWithDifferentCase()
        {
            MockTermTable table = new MockTermTable("file1");
            MockTerm term = new MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table);
            term.AddExclusion("this Foo is excluded");
            table.AddSearchTerm(term);
            IList<IScanHit> hits = InternalScanText("THIS FOO IS EXCLUDED, BUT THIS FOO IS NOT", new ITermTable[] { table });
            Assert.AreEqual(1, hits.Count, "Search returned wrong number of hits.");
            Assert.AreEqual(term, hits[0].Term, "Term property of hit 0 is incorrect.");
            Assert.AreEqual(0, hits[0].Line, "Line property of hit 0 is incorrect.");
            Assert.AreEqual(31, hits[0].Column, "Column property of hit 0 is incorrect.");
        }

        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void ScanTextWithDuplicateTerms()
        {
            MockTermTable table1 = new MockTermTable("file1");
            MockTerm term1 = new MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table1);
            term1.AddExclusion("foo exclusion 1");
            term1.AddExclusion("foo exclusion 2");
            table1.AddSearchTerm(term1);

            MockTermTable table2 = new MockTermTable("file1");
            MockTerm term2 = new MockTerm("foo", 1, "fooClass2", "fooComment2", "fooRecommended", table2);
            term2.AddExclusion("foo exclusion 2");
            term2.AddExclusion("foo exclusion 3");
            table2.AddSearchTerm(term2);

            IList<IScanHit> hits = InternalScanText("'foo exclusion 2' should be excluded, 'foo exclusion 1' and 'foo exclusion 3' should not", new ITermTable[] { table1, table2 });
            Assert.AreEqual(2, hits.Count, "Search returned wrong number of hits.");
            Assert.AreEqual(term2, hits[0].Term, "Term property of hit 0 is incorrect.");
            Assert.AreEqual(0, hits[0].Line, "Line property of hit 0 is incorrect.");
            Assert.AreEqual(39, hits[0].Column, "Column property of hit 0 is incorrect.");
            Assert.IsNotNull(hits[0].Warning, "Hit 0 did not have a warning.");
            Assert.AreEqual(term1, hits[1].Term, "Term property of hit 1 is incorrect.");
            Assert.AreEqual(0, hits[1].Line, "Line property of hit 1 is incorrect.");
            Assert.AreEqual(61, hits[1].Column, "Column property of hit 1 is incorrect.");
            Assert.IsNotNull(hits[1].Warning, "Hit 0 did not have a warning.");
        }

        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void TestReset()
        {
            MockTermTable table = new MockTermTable("file1");
            MockTerm term = new MockTerm("foo", 1, "fooClass", "fooComment", "fooRecommended", table);
            table.AddSearchTerm(term);

            CodeSweep.Scanner.MatchFinder_Accessor accessor = new CodeSweep.Scanner.MatchFinder_Accessor(new ITermTable[] { table });

            IList<IScanHit> hits1 = InternalScanText("foo bar ", accessor);

            Assert.AreEqual(1, hits1.Count, "First search returned wrong number of hits.");
            Assert.AreEqual(0, hits1[0].Column, "Column property of first hit is incorrect.");

            IList<IScanHit> hits2 = InternalScanText("foo bar", accessor);

            // This is what happens if you DON'T call Reset...
            Assert.AreEqual(1, hits2.Count, "Second search returned wrong number of hits.");
            Assert.AreEqual(8, hits2[0].Column, "Column property of second hit is incorrect.");

            accessor.Reset();

            IList<IScanHit> hits3 = InternalScanText("foo bar", accessor);

            Assert.AreEqual(1, hits3.Count, "Third search returned wrong number of hits.");
            Assert.AreEqual(0, hits3[0].Column, "Column property of third hit is incorrect.");
        }

    }


}
