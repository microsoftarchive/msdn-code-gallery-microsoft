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
using Microsoft.Samples.VisualStudio.IDE.OptionsPage;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Microsoft.VsSDK.UnitTestLibrary;

namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage.UnitTests
{
    /// <summary>
    ///This is a test class for Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPagePackage and is intended
    ///to contain all Microsoft.Samples.VisualStudio.IDE.OptionsPage.OptionsPagePackage Unit Tests
    ///</summary>
    [TestClass()]
    public class OptionsPagePackageTest : IDisposable
    {
        #region Fields
        private TestContext testContextInstance;
        private OptionsPagePackageCS testPackage;
        private OleServiceProvider serviceProvider;
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
        public void OptionsPagePackageTestInitialize()
        {
            testPackage = new OptionsPagePackageCS();

            // Create a basic service provider
            serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
            AddBasicSiteSupport(serviceProvider);

            ((IVsPackage)testPackage).SetSite(serviceProvider);
        }
        /// <summary>
        /// Runs after the test has run and to free resources obtained 
        /// by all the tests in the test class.
        /// </summary>
        [TestCleanup()]
        public void OptionsPagePackageTestCleanup()
        {
            testPackage = null;
            serviceProvider = null;
            Dispose();
        }
        #endregion Initialization && Cleanup

        #region Test methods
        /// <summary>
        /// The test for OptionsPagePackage() default constructor.
        ///</summary>
        [TestMethod()]
        public void OptionsPagePackageConstructorTest()
        {
            OptionsPagePackageCS target = new OptionsPagePackageCS();
            Assert.IsNotNull(target, "Instance of the OptionsPagePackage object was not created successfully.");
        }

        /// <summary>
        /// The test for Initialize() method.
        ///</summary>
        [TestMethod()]
        public void InitializeTest()
        {
            OptionsPagePackageCS target = testPackage;
            Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPagePackageAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPagePackageAccessor(target);
            accessor.Initialize();
            Assert.IsNotNull(target, "Instance of the OptionsPagePackage object was not initialized successfully.");
        }
        #endregion Test methods

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
                if (testPackage != null)
                {
                    testPackage = null;
                }
                if( serviceProvider != null )
                {
                    serviceProvider = null;
                }
                GC.SuppressFinalize(this);
            }
        }
        #endregion IDisposable Pattern implementation

        #region Service functions

        public static void AddBasicSiteSupport(OleServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentException("serviceProvider");
            }

            // Add solution Support
            BaseMock solution = MockServiceProvider.GetUserSettingsFactoryInstance();
            serviceProvider.AddService(typeof(IVsUserSettings), solution, false);
        }
        #endregion Service functions
    }
}
