/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
{
    /// <summary>
    ///This is a test class for Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.DlgQuerySaveCheckedInFile and is intended
    ///to contain all Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.DlgQuerySaveCheckedInFile Unit Tests
    ///</summary>
    [TestClass()]
    public class DlgQuerySaveCheckedInFileTest
    {
        /// <summary>
        ///A test for DlgQuerySaveCheckedInFile (string)
        ///</summary>
        [TestMethod()]
        public void ConstructorTest()
        {
            DlgQuerySaveCheckedInFile target = new DlgQuerySaveCheckedInFile("Dummy.txt");
            Assert.IsNotNull(target, "DlgQuerySaveCheckedInFile cannot be created");
        }

        /// <summary>
        ///A test for btnCancel_Click (object, EventArgs)
        ///</summary>
        [TestMethod()]
        public void btnCancel_ClickTest()
        {
            DlgQuerySaveCheckedInFile target = new DlgQuerySaveCheckedInFile("Dummy.txt");
            MethodInfo method = typeof(DlgQuerySaveCheckedInFile).GetMethod("btnCancel_Click", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(target, new object[] { null, null });
            Assert.AreEqual(target.Answer, DlgQuerySaveCheckedInFile.qscifCancel);
        }

        /// <summary>
        ///A test for btnCheckout_Click (object, EventArgs)
        ///</summary>
        [TestMethod()]
        public void btnCheckout_ClickTest()
        {
            DlgQuerySaveCheckedInFile target = new DlgQuerySaveCheckedInFile("Dummy.txt");
            MethodInfo method = typeof(DlgQuerySaveCheckedInFile).GetMethod("btnCheckout_Click", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(target, new object[] { null, null });
            Assert.AreEqual(target.Answer, DlgQuerySaveCheckedInFile.qscifCheckout);
        }

        /// <summary>
        ///A test for btnSaveAs_Click (object, EventArgs)
        ///</summary>
        [TestMethod()]
        public void btnSaveAs_ClickTest()
        {
            DlgQuerySaveCheckedInFile target = new DlgQuerySaveCheckedInFile("Dummy.txt");
            MethodInfo method = typeof(DlgQuerySaveCheckedInFile).GetMethod("btnSaveAs_Click", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(target, new object[] { null, null });
            Assert.AreEqual(target.Answer, DlgQuerySaveCheckedInFile.qscifForceSaveAs);
        }

        /// <summary>
        ///A test for btnSkipSave_Click (object, EventArgs)
        ///</summary>
        [TestMethod()]
        public void btnSkipSave_ClickTest()
        {
            DlgQuerySaveCheckedInFile target = new DlgQuerySaveCheckedInFile("Dummy.txt");
            MethodInfo method = typeof(DlgQuerySaveCheckedInFile).GetMethod("btnSkipSave_Click", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(target, new object[] { null, null });
            Assert.AreEqual(target.Answer, DlgQuerySaveCheckedInFile.qscifSkipSave);
        }
   }
}
