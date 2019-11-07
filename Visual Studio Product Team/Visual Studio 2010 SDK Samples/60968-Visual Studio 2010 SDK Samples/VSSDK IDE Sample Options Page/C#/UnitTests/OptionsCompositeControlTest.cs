/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.VisualStudio.IDE.OptionsPage;

namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage.UnitTests
{
    ///<summary>
    /// This is a test class for OptionsPage.OptionsCompositeControl and is intended
    /// to contain all OptionsPage.OptionsCompositeControl Unit Tests.
    ///</summary>
    [TestClass()]
    public class OptionsCompositeControlTest : IDisposable
    {
        #region Fields
        private readonly string testCustomImagePath = "SomeImagePath.bmp";
        private TestContext testContextInstance;
        // instance of tested object
        private OptionsCompositeControl compositeControl;
        // accessor for the private interface of the tested object
        private Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsCompositeControlAccessor compositeControlAccessor;
        #endregion Fields

        #region Properties
        /// <summary>
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
        #endregion Properties

        #region Initialization && Cleanup
        
        /// <summary>
        /// Runs before the test to allocate and configure resources needed 
        /// by all tests in the test class.
        /// </summary>
        [TestInitialize()]
        public void TestInitialize()
        {
            compositeControl = new OptionsCompositeControl();
            compositeControlAccessor = new Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsCompositeControlAccessor(compositeControl);
            Assert.IsNotNull(compositeControl, "General OptionsCompositeControl instance (compositeControl) was not created successfully.");
        }

        /// <summary>
        /// Runs after the test has run and to free resources obtained 
        /// by all the tests in the test class.
        /// </summary>
        [TestCleanup()]
        public void TestCleanup()
        {
            compositeControl = null;
            compositeControlAccessor = null;
        }
        #endregion Initialization && Cleanup
        
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
                if (compositeControl != null)
                {
                    compositeControl.Dispose();
                    compositeControl = null;
                }
                if (compositeControlAccessor != null)
                {
                    compositeControlAccessor.Dispose(true);
                    compositeControlAccessor = null;
                }
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        #region Test methods
        #region Methods of testing of the Constructors and Initializers
        /// <summary>
        /// The test for OptionsCompositeControl()
        ///</summary>
        [TestMethod()]
        public void ConstructorTest()
        {
            OptionsCompositeControl target = new OptionsCompositeControl();
            Assert.IsNotNull(target, "Instance of the OptionsCompositeControl was not created successfully after default constructor call.");
        }
        /// <summary>
        /// The test for InitializeComponent() method.
        ///</summary>
        [TestMethod()]
        public void InitializeComponentTest()
        {
            OptionsCompositeControl target = new OptionsCompositeControl();
            Assert.IsNotNull(target, "Instance of the OptionsCompositeControl object was not created successfully.");
            Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsCompositeControlAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsCompositeControlAccessor(target);
            Assert.IsNotNull(accessor, "Instance of the OptionsCompositeControl accessor was not created successfully.");

            accessor.InitializeComponent();

            Assert.IsNotNull(target.Controls, "Controls collection was not initialized after InitializeComponent() call.");
            Assert.IsTrue((target.Controls.Count>0), "Controls collection was not populated by controls objects.");
        }
        #endregion Methods of testing of the Constructors.

        #region Methods of testing of the IDisposable implementation
        /// <summary>
        /// The test for Dispose() method.
        ///</summary>
        [TestMethod()]
        public void DisposeTest()
        {
            OptionsCompositeControl target = compositeControl;

            Assert.IsTrue((target is IDisposable), 
                "Tested OptionsCompositeControl instance does not implements IDisposable interface.");
            
            target.Dispose();
            Assert.IsTrue(target.IsDisposed, 
                "Internal state of the OptionsCompositeControl instance is in the NotDosposed state, was expected that IsDisosed is True.");
        }
        #endregion Methods of testing of the IDisposable implementation

        #region Methods of testing of the GUI Event Handlers
        /// <summary>
        /// The test for OnClearImage() event handler.
        ///</summary>
        [TestMethod()]
        public void OnClearImageTest()
        {
            OptionsCompositeControl target = compositeControl;
            Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsCompositeControlAccessor accessor = compositeControlAccessor;
            accessor.customOptionsPage = new OptionsPageCustom();
            
            accessor.customOptionsPage.CustomBitmap = testCustomImagePath;
            Assert.AreEqual(testCustomImagePath, accessor.customOptionsPage.CustomBitmap, 
                "CustomBitmap path was not initialized by expected value.");

            accessor.OnClearImage(this, EventArgs.Empty);

            Assert.IsNull(accessor.customOptionsPage.CustomBitmap,
                "CustomBitmap path after Clear command was not cleared.");
        }
        #endregion Methods of testing of the GUI Event Handlers.

        #region Methods of testing of the public properties
        /// <summary>
        /// The test for OptionsPage public property.
        ///</summary>
        [TestMethod()]
        public void OptionsPageTest()
        {
            OptionsCompositeControl target = compositeControl;
            OptionsPageCustom expectedValue = new OptionsPageCustom();

            target.OptionsPage = expectedValue;
            Assert.AreEqual(expectedValue, target.OptionsPage, 
                "OptionsPage property was initialized by unexpected value.");
        }
        #endregion Methods of testing of the public properties.

        #region Methods of testing of the public methods
        /// <summary>
        /// The test for RefreshImage() in scenario when internal OptionsPageCustom object is null.
        ///</summary>
        [TestMethod()]
        public void RefreshImageWithNullableOptionPageTest()
        {
            OptionsCompositeControl target = compositeControl;
            Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsCompositeControlAccessor accessor = compositeControlAccessor;

            accessor.customOptionsPage = null;
            accessor.RefreshImage();
        }
        /// <summary>
        /// The test for RefreshImage() in scenario when internal OptionsPageCustom object
        /// is properly initialized.
        ///</summary>
        [TestMethod()]
        public void RefreshImageWithCompleteOptionPageTest()
        {
            OptionsCompositeControl target = compositeControl;
            Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsCompositeControlAccessor accessor = compositeControlAccessor;

            try
            {
                CreateTestBitmapFile();

                accessor.customOptionsPage = new OptionsPageCustom();
                accessor.pictureBox.Image = null;
                accessor.customOptionsPage.CustomBitmap = testCustomImagePath;
                accessor.RefreshImage();

                Assert.IsNotNull(accessor.pictureBox.Image,
                    "Internal PictureBox Image object was not initialized.");
            }finally
            {
                DestroyTestBitmapFile();
            }
        }
        #endregion Methods of testing of the public methods.
        #endregion Test methods

        #region Service functions
        /// <summary>
        /// Create test bitmap image file.
        /// </summary>
        public void CreateTestBitmapFile()
        {
            if (File.Exists(testCustomImagePath))
            {
                File.Delete(testCustomImagePath);
            }
            
            Bitmap bitmapData = new Bitmap(10, 10);
            
            try
            {
                bitmapData.Save(testCustomImagePath);
            }
            catch(ArgumentNullException)
            {
                Assert.Fail("Path to the test bitmap image file was not properly initialized.");
            }
            catch(ExternalException handledException)
            {
                Assert.Fail(handledException.Message);
            }
        }
        /// <summary>
        /// Destroy test bitmap image file.
        /// </summary>
        public void DestroyTestBitmapFile()
        {
            if (File.Exists(testCustomImagePath))
            {
                try
                {
                    File.Delete(testCustomImagePath);
                }
                catch(Exception handleedException)
                {
                    Assert.Fail(handleedException.Message);
                }
            }
        }
        #endregion Service functions.
    }
}
