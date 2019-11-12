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

namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage.UnitTests
{
    ///<summary>
    /// This is a test class for OptionsPage.OptionsPageCustom and is intended
    /// to contain all OptionsPage.OptionsPageCustom Unit Tests.
    ///</summary>
    [TestClass()]
    public class OptionsPageCustomTest : IDisposable
    {
        #region Fields
        private TestContext testContextInstance;
        // instance of tested object
        private OptionsPageCustom optionsPageCustom;
        #endregion Fields

        #region Proeprties
        ///<summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
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
        #endregion Proeprties

        #region Initialization && Cleanup
        /// <summary>
        /// Runs before the test to allocate and configure resources needed 
        /// by all tests in the test class.
        /// </summary>
        [TestInitialize()]
        public void OptionsPageCustomTestInitialize()
        {
            optionsPageCustom = new OptionsPageCustom();
        }
        /// <summary>
        /// Runs after the test has run and to free resources obtained 
        /// by all the tests in the test class.
        /// </summary>
        [TestCleanup()]
        public void OptionsPageCustomTestCleanup()
        {
            optionsPageCustom = null;
        }

        #endregion Initialization && Cleanup

        #region Test Methods

        #region Methods of testing of the Properties
        /// <summary>
        /// The test for CustomBitmap property.
        ///</summary>
        [TestMethod()]
        public void CustomBitmapTest()
        {
            OptionsPageCustom target = optionsPageCustom;
            string expectedPathValue = AppDomain.CurrentDomain.BaseDirectory+"SomeBitmap.Bmp";
            target.CustomBitmap = expectedPathValue;

            Assert.AreEqual(expectedPathValue, target.CustomBitmap, "CustomBitmap property value was initialized by unexpected value.");
        }

        /// <summary>
        /// The test for Window property.
        ///</summary>
        [TestMethod()]
        public void WindowTest()
        {
            OptionsPageCustom target = optionsPageCustom;
            Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageCustomAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageCustomAccessor(target);
            OptionsCompositeControl optionsControl = new OptionsCompositeControl();

            optionsControl = accessor.Window as OptionsCompositeControl;
            Assert.IsNotNull(optionsControl, "Internal Window property was not initialized by expected value.");
            Assert.AreEqual(optionsControl.OptionsPage, target, "Internal CompositeCOntrol options page property was initialized by unexpected value.");
        }
        #endregion Methods of testing of the Properties

        #endregion Test Methods

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
                if (optionsPageCustom!= null)
                {
                    optionsPageCustom = null;
                }
                GC.SuppressFinalize(this);
            }
        }
        #endregion IDisposable Pattern implementation
    }
}
