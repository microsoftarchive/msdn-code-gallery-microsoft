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
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox;

namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests
{
    /// <summary>
    ///This is a test class for EditorWithToolbox.ToolboxItemData and is intended
    ///to contain all IDE.EditorWithToolbox.ToolboxItemData Unit Tests.
    ///</summary>
    [TestClass()]
    public class ToolboxItemDataTest
    {
        #region Test methods
        /// <summary>
        /// The test for ToolboxItemData default constructor.
        ///</summary>
        [TestMethod()]
        public void ConstructorTest()
        {
            string sentence = "This is MyToolbox test sentence";
            ToolboxItemData target = new ToolboxItemData(sentence);
            Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_ToolboxItemDataAccessor accessor = new Microsoft_Samples_VisualStudio_IDE_EditorWithToolbox_ToolboxItemDataAccessor(target);

            Assert.AreEqual(sentence, accessor.content, "ToolBox content was not properly initialized.");
        }

        /// <summary>
        /// The test for the Content() method.
        ///</summary>
        [TestMethod()]
        public void ContentTest()
        {
            string sentence = "This is MyToolbox test sentence";
            ToolboxItemData target = new ToolboxItemData(sentence);
            Assert.AreEqual(sentence, target.Content, "ToolBox content was not properly initialized.");
        }
        #endregion Test methods
    }
}
