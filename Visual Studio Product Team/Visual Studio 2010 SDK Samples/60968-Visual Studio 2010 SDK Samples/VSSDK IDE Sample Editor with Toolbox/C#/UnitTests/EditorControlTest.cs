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

namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests
{
    ///<summary>
    ///This is a test class for EditorWithToolbox.EditorControl and is intended
    ///to contain all EditorWithToolbox.EditorControl Unit Tests.
    ///</summary>
    [TestClass()]
    public class EditorControlTest : IDisposable
    {
        #region Fields

        private TestContext testContextInstance;
        private EditorControl editorControl;
        private string testString = string.Empty;

        #endregion

        #region Initialization && Cleanup
        /// <summary>
        /// Runs before the test to allocate and configure resources needed 
        /// by all tests in the test class.
        /// </summary>
        [TestInitialize()]
        public void EditorControlTestInitialize()
        {
            testString = "This is a test string";

            editorControl = new EditorControl();
            editorControl.Text = testString;
        }
        /// <summary>
        /// Runs after the test has run and to free resources obtained 
        /// by all the tests in the test class.
        /// </summary>
        [TestCleanup()]
        public void EditorControlTestCleanup()
        {
            editorControl = null;
        }

        #endregion Initialization && Cleanup

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
        #endregion properties

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
                if (editorControl != null)
                {
                    editorControl.Dispose();
                    editorControl = null;
                }
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        #region Constructors tests
        /// <summary>
        /// The test for EditorControl default constructor.
        ///</summary>
        [TestMethod()]
        public void ConstructorTest()
        {
            EditorControl target = new EditorControl();

            Assert.IsNotNull(target, "EditorControl object was not created successfully");
        }

        /// <summary>
        /// The test for InitializeComponent() method.
        ///</summary>
        [TestMethod()]
        public void InitializeComponentTest()
        {
            EditorControl target = new EditorControl();

            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorControlAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_EditorControlAccessor(target);

            accessor.InitializeComponent();

            Assert.IsFalse(target.WordWrap, "WordWrap property is not switched in to False state");
        }
        #endregion Constructors && Initializers tests
    }
}
