/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System.Collections.Generic;
using System.Text;
using Microsoft.Samples.VisualStudio.CodeSweep.Scanner;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    /// <summary>
    ///This is a test class for CodeSweep.Scanner.Factory and is intended
    ///to contain all CodeSweep.Scanner.Factory Unit Tests
    ///</summary>
    [TestClass()]
    public class Scanner_FactoryTest
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


        /// <summary>
        ///A test case for GetScanner ()
        ///</summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod()]
        public void GetScannerTest()
        {
            Assert.IsNotNull(Factory.GetScanner(), "GetScanner returned null.");
        }

        /// <summary>
        ///A test case for GetTermTable (string)
        ///</summary>
        [DeploymentItem("Scanner.dll")]
        [TestMethod()]
        public void GetTermTableFromEmptyFile()
        {
            string filePath = Utilities.CreateTempTxtFile("");

            bool thrown = false;

            try
            {
                Factory.GetTermTable(filePath);
            }
            catch (System.Xml.XmlException)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown, "GetTermTable did not throw XmlException with empty file.");
        }

        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void GetTermTableWithNullPath()
        {
            bool thrown = false;

            try
            {
                Factory.GetTermTable(null);
            }
            catch (System.ArgumentNullException)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown, "GetTermTable did not throw ArgumentNullException with null file name.");
        }

        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void GetTermTableWithInvalidPath()
        {
            bool thrown = false;

            try
            {
                Factory.GetTermTable("z:\\somedir\\somefile.ext");
            }
            catch (System.IO.FileNotFoundException)
            {
                thrown = true;
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown, "GetTermTable did not throw FileNotFoundException or DirectoryNotFoundException with invalid file name.");
        }

        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void GetTermTableUsingMinimalSchema()
        {
            StringBuilder fileContent = new StringBuilder();

            fileContent.Append("<?xml version=\"1.0\"?>\n");
            fileContent.Append("<xmldata>\n");
            fileContent.Append("  <PLCKTT>\n");
            fileContent.Append("    <Lang>\n");
            fileContent.Append("      <Term Term=\"countries\" Severity=\"2\" TermClass=\"Geopolitical\">\n");
            fileContent.Append("        <Comment>For detailed info see - http://relevant-url-here.com</Comment>\n");
            fileContent.Append("      </Term>\n");
            fileContent.Append("    </Lang>\n");
            fileContent.Append("  </PLCKTT>\n");
            fileContent.Append("</xmldata>\n");

            string fileName = Utilities.CreateTempTxtFile(fileContent.ToString());

            ITermTable table = Factory.GetTermTable(fileName);

            Assert.AreEqual(fileName, table.SourceFile, "SourceFile property is incorrect.");
            Assert.IsNotNull(table.Terms, "Terms property is null.");

            List<ISearchTerm> termList = Utilities.ListFromEnum(table.Terms);

            Assert.AreEqual(1, termList.Count, "Number of terms is incorrect.");
            Assert.AreEqual("countries", termList[0].Text, "Text property of term 0 is incorrect.");
            Assert.AreEqual("Geopolitical", termList[0].Class, "Class property of term 0 is incorrect.");
            Assert.AreEqual("For detailed info see - http://relevant-url-here.com", termList[0].Comment, "Comment property of term 0 is incorrect.");
            Assert.IsNull(termList[0].RecommendedTerm, "RecommendedTerm property of term 0 is incorrect.");

            List<IExclusion> term0exclusions = Utilities.ListFromEnum(termList[0].Exclusions);

            Assert.AreEqual(0, term0exclusions.Count, "Number of exclusions for term 0 is incorrect.");
        }

        [DeploymentItem("Scanner.dll")]
        [TestMethod]
        public void GetTermTableUsingFullSchema()
        {
            StringBuilder fileContent = new StringBuilder();

            fileContent.Append("<?xml version=\"1.0\"?>\n");
            fileContent.Append("<xmldata>\n");
            fileContent.Append("  <PLCKTT>\n");
            fileContent.Append("    <Lang>\n");
            fileContent.Append("      <Term Term=\"countries\" TermID=\"5\" Severity=\"2\" TermClass=\"Geopolitical\" CaseSensitive=\"0\" WholeWord=\"1\">\n");
            fileContent.Append("        <RecommendedTerm>countries/regions</RecommendedTerm>\n");
            fileContent.Append("        <Comment>For detailed info see - http://relevant-url-here.com</Comment>\n");
            fileContent.Append("        <Exclusion>and for other fareast countries</Exclusion>\n");
            fileContent.Append("        <Exclusion>\"countries\",</Exclusion>\n");
            fileContent.Append("        <ExclusionContext>hold true for other countries in the world</ExclusionContext>\n");
            fileContent.Append("        <ExclusionContext>lists the available countries in north america</ExclusionContext>\n");
            fileContent.Append("      </Term>\n");
            fileContent.Append("      <Term Term=\"shoot\" TermID=\"6\" Severity=\"3\" TermClass=\"Profanity\" CaseSensitive=\"1\" WholeWord=\"0\">\n");
            fileContent.Append("        <RecommendedTerm>darn</RecommendedTerm>\n");
            fileContent.Append("        <Comment>Watch that language!</Comment>\n");
            fileContent.Append("        <Exclusion>Did you shoot the sherrif?</Exclusion>\n");
            fileContent.Append("        <Exclusion>I did not shoot the deputy</Exclusion>\n");
            fileContent.Append("        <ExclusionContext>an old-fashioned shoot-em-up</ExclusionContext>\n");
            fileContent.Append("      </Term>\n");
            fileContent.Append("    </Lang>\n");
            fileContent.Append("  </PLCKTT>\n");
            fileContent.Append("</xmldata>\n");

            string fileName = Utilities.CreateTempTxtFile(fileContent.ToString());

            ITermTable table = Factory.GetTermTable(fileName);

            Assert.AreEqual(fileName, table.SourceFile, "SourceFile property is incorrect.");
            Assert.IsNotNull(table.Terms, "Terms property is null.");

            List<ISearchTerm> termList = Utilities.ListFromEnum(table.Terms);

            Assert.AreEqual(2, termList.Count, "Number of terms is incorrect.");
            Assert.AreEqual("countries", termList[0].Text, "Text property of term 0 is incorrect.");
            Assert.AreEqual("Geopolitical", termList[0].Class, "Class property of term 0 is incorrect.");
            Assert.AreEqual("For detailed info see - http://relevant-url-here.com", termList[0].Comment, "Comment property of term 0 is incorrect.");
            Assert.AreEqual("countries/regions", termList[0].RecommendedTerm, "RecommendedTerm property of term 0 is incorrect.");

            List<IExclusion> term0exclusions = Utilities.ListFromEnum(termList[0].Exclusions);

            Assert.AreEqual(4, term0exclusions.Count, "Number of exclusions for term 0 is incorrect.");
            Assert.AreEqual("and for other fareast countries", term0exclusions[0].Text, "Exclusion 0 of term 0 is incorrect.");
            Assert.AreEqual("\"countries\",", term0exclusions[1].Text, "Exclusion 1 of term 0 is incorrect.");
            Assert.AreEqual("hold true for other countries in the world", term0exclusions[2].Text, "Exclusion 2 of term 0 is incorrect.");
            Assert.AreEqual("lists the available countries in north america", term0exclusions[3].Text, "Exclusion 3 of term 0 is incorrect.");

            Assert.AreEqual("shoot", termList[1].Text, "Text property of term 1 is incorrect.");
            Assert.AreEqual("Profanity", termList[1].Class, "Class property of term 1 is incorrect.");
            Assert.AreEqual("Watch that language!", termList[1].Comment, "Comment property of term 1 is incorrect.");
            Assert.AreEqual("darn", termList[1].RecommendedTerm, "RecommendedTerm property of term 1 is incorrect.");

            List<IExclusion> term1exclusions = Utilities.ListFromEnum(termList[1].Exclusions);

            Assert.AreEqual(3, term1exclusions.Count, "Number of exclusions for term 1 is incorrect.");
            Assert.AreEqual("Did you shoot the sherrif?", term1exclusions[0].Text, "Exclusion 0 of term 1 is incorrect.");
            Assert.AreEqual("I did not shoot the deputy", term1exclusions[1].Text, "Exclusion 1 of term 1 is incorrect.");
            Assert.AreEqual("an old-fashioned shoot-em-up", term1exclusions[2].Text, "Exclusion 2 of term 1 is incorrect.");
        }

    }


}
