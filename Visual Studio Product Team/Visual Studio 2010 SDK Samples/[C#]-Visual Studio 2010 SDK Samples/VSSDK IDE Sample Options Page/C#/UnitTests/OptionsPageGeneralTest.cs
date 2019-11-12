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
using System.Drawing;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage.UnitTests
{
    /// <summary>
    /// This is a test class for OptionsPage.OptionsPageGeneral and is intended
    /// to contain all OptionsPage.OptionsPageGeneral Unit Testsю
    /// </summary>
    [TestClass()]
    public class OptionsPageGeneralTest : IDisposable
    {
        #region Fields
        private TestContext testContextInstance;
        // instance of tested object
        private OptionsPageGeneral optionsPageGeneral;
        private string testString;
        private string tmpImgFilePath = @"OptionsPageTest.jpg";
        Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageGeneralAccessor optionsPageGeneralAccessor;
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

        #region Initialization && CLeanup
        [TestInitialize()]
        public void OptionsPageGeneralTestInitialize()
        {
            testString = "This is the test string.";
            tmpImgFilePath = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + tmpImgFilePath;
            optionsPageGeneral = new OptionsPageGeneral();
            optionsPageGeneralAccessor = new Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageGeneralAccessor(optionsPageGeneral);
            // to forbid display of MessageBoxes
            WinFormsHelper.AllowMessageBox = false;
        }
        
        [TestCleanup()]
        public void OptionsPageGeneralTestCleanup()
        {
            optionsPageGeneral = null;
            Dispose();
        }
        #endregion Initialization && CLeanup

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
                if (optionsPageGeneral != null)
                {
                    optionsPageGeneral = null;
                }
                GC.SuppressFinalize(this);
            }
        }
        #endregion IDisposable Pattern implementation

        #region Test methods

        #region Properties tests
        /// <summary>
        /// The test for CustomSize property.
        ///</summary>
        [TestMethod()]
        public void CustomSizeTest()
        {
            OptionsPageGeneral target = optionsPageGeneral;
            Size expectedSizeValue = new Size(100,100);
            target.CustomSize = expectedSizeValue;

            Assert.AreEqual(expectedSizeValue, target.CustomSize, "CustomSize property was not returned expected value.");
        }
        /// <summary>
        /// The test for OptionInteger property.
        ///</summary>
        [TestMethod()]
        public void OptionIntegerTest()
        {
            OptionsPageGeneral target = optionsPageGeneral;
            int expectedIntegerValue = int.MaxValue;
            target.OptionInteger = expectedIntegerValue;

            Assert.AreEqual(expectedIntegerValue, target.OptionInteger, "OptionInteger property was not returned expected value.");
        }

        /// <summary>
        /// The test for OptionString property.
        ///</summary>
        [TestMethod()]
        public void OptionStringTest()
        {
            OptionsPageGeneral target = optionsPageGeneral;
            string expectedStringValue = testString;
            target.OptionString = expectedStringValue;

            Assert.AreEqual(expectedStringValue, target.OptionString, "OptionString property was not returned expected value.");
        }

        #endregion Properties tests

        #region Event handlers tests
        /// <summary>
        /// The test for OnActivate() event handler.
        ///</summary>
        [TestMethod()]
        public void OnActivateTest()
        {
            OptionsPageGeneral target = optionsPageGeneral;
            Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageGeneralAccessor accessor = optionsPageGeneralAccessor;
            CancelEventArgs cancelEventArgs = new CancelEventArgs(false);

            // we simulate Cancel button choice, in this case we expect that 
            // cancelEventArgs.Cancel was switched to the true state.
            WinFormsHelper.FakeDialogResult = DialogResult.Cancel;
            accessor.OnActivate(cancelEventArgs);

            Assert.IsTrue(cancelEventArgs.Cancel, "CancelEventArgs Cancel property was initialized by not expected value in case when simulated Cancel button choice.");
        }

        /// <summary>
        /// The test for OnClosed() event handler.
        ///</summary>
        ///<remarks>Tested event handling function does not performs any actions.</remarks>
        [TestMethod()]
        public void OnClosedTest()
        {
            OptionsPageGeneral target = new OptionsPageGeneral();
            Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageGeneralAccessor accessor = optionsPageGeneralAccessor;
            EventArgs e = null;

            accessor.OnClosed(e);
        }

        /// <summary>
        /// The test for OnDeactivate event handler.
        ///</summary>
        [TestMethod()]
        public void OnDeactivateTest()
        {
            OptionsPageGeneral target = new OptionsPageGeneral();
            Microsoft_Samples_VisualStudio_IDE_OptionsPage_OptionsPageGeneralAccessor accessor = optionsPageGeneralAccessor;

            CancelEventArgs cancelEventArgs = new CancelEventArgs(false);
            
            // we simulate Cancel button choice, in this case we expect that 
            // cancelEventArgs.Cancel was switched to the true state.
            WinFormsHelper.FakeDialogResult = DialogResult.Cancel;
            accessor.OnDeactivate(cancelEventArgs);

            Assert.IsTrue(cancelEventArgs.Cancel, "CancelEventArgs Cancel property was initialized by not expected value in case when simulated Cancel button choice.");
        }
        #endregion Event handlers tests

        #endregion Test methods

        #region Service functions
        public void CreateTmpImgFile()
        {
            if (File.Exists(tmpImgFilePath))
            {
                File.Delete(tmpImgFilePath);
            }
            StreamWriter sw = File.AppendText(tmpImgFilePath);
            sw.Write(tmpImgFilePath);
            sw.Close();

        }
        public void DestroyTmpImgFile()
        {
            if (File.Exists(tmpImgFilePath))
            {
                File.Delete(tmpImgFilePath);
            }
        }
        #endregion Service functions
    }
}
