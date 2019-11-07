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
using Microsoft.VisualStudio;
using System.Collections.Generic;
using OleInterop = Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VsSDK.UnitTestLibrary;

namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests
{
    /// <summary>
    ///This is a test class for Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.EditorPackage and is intended
    ///to contain all Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.EditorPackage Unit Tests
    ///</summary>
    [TestClass()]
    public class EditorPackageTest : IDisposable
    {
        #region Fields

        private TestContext testContextInstance;
        private EditorPackage editorPackage;

        #endregion Fields

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
        #endregion Properties

        #region Initialization && Cleanup
        /// <summary>
        /// Runs before the test to allocate and configure resources needed 
        /// by all tests in the test class.
        /// </summary>
        [TestInitialize()]
        public void EditorPackageTestInitialize()
        {
            editorPackage = new EditorPackage();
        }
        /// <summary>
        /// Runs after the test has run and to free resources obtained 
        /// by all the tests in the test class.
        /// </summary>
        [TestCleanup()]
        public void EditorPackageTestCleanup()
        {
            editorPackage = null;
        }

        #endregion

        #region IDisposable Pattern implementation
        /// <summary>
        /// Implement IDisposable.
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
                if (editorPackage != null)
                {
                    editorPackage.Dispose();
                    editorPackage = null;
                }
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        #region IDisposable tests
        /// <summary>
        ///A test for Dispose (bool)
        ///</summary>
        [TestMethod()]
        public void DisposeTest()
        {
            EditorPackage target = editorPackage;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPackageAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorPackageAccessor(target);
            accessor.editorFactory = new EditorFactory();
            target.Dispose();
            Assert.IsNull(accessor.editorFactory, "Internal Editor Factory object was not disposed properly.");
        }
        #endregion IDisposable tests

        [TestMethod()]
        public void SetSite()
        {
            // Create the package
            IVsPackage package = new EditorPackage() as IVsPackage;
            Assert.IsNotNull(package, "The object does not implement IVsPackage");

            // Create a basic service provider
            OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Add site support to register editor factory
            BaseMock registerEditor = MockRegisterEditors.GetRegisterEditorsInstance();
            serviceProvider.AddService(typeof(SVsRegisterEditors), registerEditor, false);

            // Site the package
            Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");
        }
    }
}
