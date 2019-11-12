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
    ///This is a test class for Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.DlgQueryEditCheckedInFile and is intended
    ///to contain all Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.DlgQueryEditCheckedInFile Unit Tests
    ///</summary>
    [TestClass()]
    public class DlgQueryEditCheckedInFileTest
    {
        /// <summary>
        ///A test for DlgQueryEditCheckedInFile (string)
        ///</summary>
        [TestMethod()]
        public void ConstructorTest()
        {
            DlgQueryEditCheckedInFile target = new DlgQueryEditCheckedInFile("Dummy.txt");
            Assert.IsNotNull(target, "DlgQueryEditCheckedInFile cannot be created");
        }

        /// <summary>
        ///A test for btnCancel_Click (object, EventArgs)
        ///</summary>
        [TestMethod()]
        public void btnCancel_ClickTest()
        {
            DlgQueryEditCheckedInFile target = new DlgQueryEditCheckedInFile("Dummy.txt");
            MethodInfo method = typeof(DlgQueryEditCheckedInFile).GetMethod("btnCancel_Click", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(target, new object[] { null, null });
            Assert.AreEqual(target.Answer, DlgQueryEditCheckedInFile.qecifCancelEdit);
        }

        /// <summary>
        ///A test for btnCheckout_Click (object, EventArgs)
        ///</summary>
        [TestMethod()]
        public void btnCheckout_ClickTest()
        {
            DlgQueryEditCheckedInFile target = new DlgQueryEditCheckedInFile("Dummy.txt");
            MethodInfo method = typeof(DlgQueryEditCheckedInFile).GetMethod("btnCheckout_Click", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(target, new object[] { null, null });
            Assert.AreEqual(target.Answer, DlgQueryEditCheckedInFile.qecifCheckout);
        }

        /// <summary>
        ///A test for btnEdit_Click (object, EventArgs)
        ///</summary>
        [TestMethod()]
        public void btnEdit_ClickTest()
        {
            DlgQueryEditCheckedInFile target = new DlgQueryEditCheckedInFile("Dummy.txt");
            MethodInfo method = typeof(DlgQueryEditCheckedInFile).GetMethod("btnEdit_Click", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(target, new object[] {null, null} );
            Assert.AreEqual(target.Answer, DlgQueryEditCheckedInFile.qecifEditInMemory);
        }
    }
}
