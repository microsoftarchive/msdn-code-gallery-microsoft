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
using Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox;
using Microsoft.VisualStudio.Shell.Interop;
using OleInterop = Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests
{
    /// <summary>
    ///This is a test class for EditorWithToolbox.EditorFactory and is intended
    ///to contain all EditorWithToolbox.EditorFactory Unit Tests.
    ///</summary>
    [TestClass()]
    public class EditorFactoryTest : IDisposable
    {
        #region Fields

        private TestContext testContextInstance;
        private EditorPackage editorPackage;
        private EditorFactory editorFactory;

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
        public void EditorFactoryTestInitialize()
        {
            editorPackage = new EditorPackage();
            editorFactory = new EditorFactory();
        }
        /// <summary>
        /// Runs after the test has run and to free resources obtained 
        /// by all the tests in the test class.
        /// </summary>
        [TestCleanup()]
        public void EditorFactoryTestCleanup()
        {
            editorPackage.Dispose();
            editorPackage = null;

            editorFactory.Dispose();
            editorFactory = null;
        }

        #endregion Initialize && Cleanup

        #region IDisposable Pattern implementation
        /// <summary>
        /// Clean up any resources being used.
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
                if (editorFactory != null)
                {
                    editorFactory.Dispose();
                    editorFactory = null;
                }
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        #region Test methods
        #region Constructors tests
        /// <summary>
        ///A test for EditorFactory (EditorPackage)
        ///</summary>
        [TestMethod()]
        public void ConstructorTest()
        {
            editorFactory = null;
            editorFactory = new EditorFactory();
            Assert.IsNotNull(editorFactory, "EditorFactory instance was not created successfully");
        }
        #endregion Constructors && Initializers tests

        #region IDisposable tests
        /// <summary>
        /// Verifies that the object implement IDisposable interface
        /// </summary>
        [TestMethod()]
        public void IsIDisposableTest()
        {
            EditorFactory target= editorFactory;
            Assert.IsNotNull(target as IDisposable, "The object does not implement IDisposable interface");
        }

        /// <summary>
        /// Object is destroyed deterministically by Dispose() method call test
        /// </summary>
        [TestMethod()]
        public void DisposeTest()
        {
            EditorFactory target = editorFactory;
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorFactoryAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorFactoryAccessor(target);

            Assert.IsNull(accessor.vsServiceProvider, "Internal service provider object was not disposed properly.");
        }
        #endregion IDisposable tests

        #region IVsEditorFactory tests
        [TestMethod()]
        public void IsIVsEditorFactory()
        {
            EditorFactory editorFactory = new EditorFactory();
            Assert.IsNotNull(editorFactory as IVsEditorFactory, "The object does not implement IVsEditorFactory");
        }

        [TestMethod()]
        public void CreateEditorInstanceTest()
        {
            //Create the editor factory
            EditorFactory targetFactory = editorFactory;

            // Create a basic service provider
            OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
            AddBasicSiteSupport(serviceProvider);

            // Site the editor factory
            Assert.AreEqual(0, targetFactory.SetSite(serviceProvider), "SetSite did not return S_OK");

            IntPtr ppunkDocView;
            IntPtr ppunkDocData;
            string pbstrEditorCaption = String.Empty;
            Guid pguidCmdUI = Guid.Empty;
            int pgrfCDW = 0;
            uint cef_option = VSConstants.CEF_OPENFILE;

            int actual_result = targetFactory.CreateEditorInstance(cef_option, null, null, null,
                0, IntPtr.Zero, out ppunkDocView, out ppunkDocData, out pbstrEditorCaption, out pguidCmdUI, out pgrfCDW);

            Assert.AreEqual(VSConstants.S_OK, actual_result);
        }

        [TestMethod()]
        public void CreateEditorInstanceWithBadInputsTest()
        {
            //Create the editor factory
            EditorFactory targetFactory = editorFactory;

            // Create a basic service provider
            OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Site the editor factory
            Assert.AreEqual(0, targetFactory.SetSite(serviceProvider), "SetSite did not return S_OK");

            IntPtr ppunkDocView;
            IntPtr ppunkDocData;
            string pbstrEditorCaption = String.Empty;
            Guid pguidCmdUI = Guid.Empty;
            int pgrfCDW = 0;
            uint cef_option = 0;

            // test scenario with invalid CEF_* option
            IntPtr punkDocDataExisting = IntPtr.Zero;

            int actual_result = targetFactory.CreateEditorInstance(cef_option, null, null, null,
                0, punkDocDataExisting, out ppunkDocView, out ppunkDocData, out pbstrEditorCaption, out pguidCmdUI, out pgrfCDW);
            Assert.AreEqual(VSConstants.E_INVALIDARG, actual_result, "CreateEditorInstance() can not process invalid CEF_* arguments");

            ppunkDocView = IntPtr.Zero;
            ppunkDocData = IntPtr.Zero;
            pbstrEditorCaption = String.Empty;
            pguidCmdUI = Guid.Empty;
            pgrfCDW = 0;
            cef_option = VSConstants.CEF_OPENFILE;

            // test scenario with not-null punkDocDataExisting parameter value
            punkDocDataExisting = new IntPtr(Int32.MaxValue);

            actual_result = targetFactory.CreateEditorInstance(cef_option, null, null, null,
                0, punkDocDataExisting, out ppunkDocView, out ppunkDocData, out pbstrEditorCaption, out pguidCmdUI, out pgrfCDW);
            Assert.AreEqual(VSConstants.VS_E_INCOMPATIBLEDOCDATA, actual_result, "CreateEditorInstance() can not process incompatible document data argument");
        }
        [TestMethod()]
        public void SetSite()
        {
            //Create the editor factory
            EditorFactory editorFactory = new EditorFactory();

            // Create a basic service provider
            OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Site the editor factory
            Assert.AreEqual(0, editorFactory.SetSite(serviceProvider), "SetSite did not return S_OK");
        }

        /// <summary>
        ///A test for Close ()
        ///</summary>
        [TestMethod()]
        public void CloseTest()
        {
            int actual_result = editorFactory.Close();
            Assert.AreEqual(VSConstants.S_OK, actual_result, "Close(0 method does not return expected S_OK value");
        }

        /// <summary>
        ///A test for MapLogicalView in case of not supported view
        ///</summary>
        [TestMethod()]
        public void MapLogicalViewNotSupportedIdTest()
        {
            EditorFactory target = editorFactory;

            string pbstrPhysicalView = string.Empty;

            // specify a not supported view ID
            Guid rguidLogicalView = VSConstants.LOGVIEWID_TextView;
            int actual_result = target.MapLogicalView(ref rguidLogicalView, out pbstrPhysicalView);
            Assert.IsNull(pbstrPhysicalView, "pbstrPhysicalView out parameter not initialized by null");
            Assert.AreEqual(VSConstants.E_NOTIMPL, actual_result, "In case of supported view ID was expected E_NOTIMPL result");
        }
        /// <summary>
        ///A test for MapLogicalView in case of single physical view
        ///</summary>
        [TestMethod()]
        public void MapLogicalViewSupportedIdTest()
        {
            EditorFactory target = editorFactory;

            string pbstrPhysicalView = string.Empty;

            // specify a primary physical view
            Guid rguidLogicalView = VSConstants.LOGVIEWID_Primary;
            int actual_result = target.MapLogicalView(ref rguidLogicalView, out pbstrPhysicalView);
            Assert.IsNull(pbstrPhysicalView, "pbstrPhysicalView out parameter not initialized by null");
            Assert.AreEqual(VSConstants.S_OK, actual_result, "In case of supported view ID was expected S_OK result");
        }
        #endregion IVsEditorFactory tests
        #endregion Test methods

        #region Service functions
        /// <summary>
        /// Add some basic service mock objects to the service provider.
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void AddBasicSiteSupport(OleServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            // Add site support for UI Shell
            BaseMock uiShell = MockServicesProvider.GetUiShellInstance0();
            serviceProvider.AddService(typeof(SVsUIShell), uiShell, false);
            serviceProvider.AddService(typeof(SVsUIShellOpenDocument), (IVsUIShellOpenDocument)uiShell, false);

            //Add site support for Running Document Table
            BaseMock runningDoc = MockServicesProvider.GetRunningDocTableInstance();
            serviceProvider.AddService(typeof(SVsRunningDocumentTable), runningDoc, false);

            //Add site support for IVsTextManager
            BaseMock queryEditQuerySave = MockServicesProvider.GetQueryEditQuerySaveInstance();
            serviceProvider.AddService(typeof(SVsQueryEditQuerySave), queryEditQuerySave, false);

            BaseMock toolbox = MockIVsToolbox.GetIVsToolboxInstance();
            serviceProvider.AddService(typeof(SVsToolbox), toolbox, false);
        }
        #endregion Service functions
    }
}
