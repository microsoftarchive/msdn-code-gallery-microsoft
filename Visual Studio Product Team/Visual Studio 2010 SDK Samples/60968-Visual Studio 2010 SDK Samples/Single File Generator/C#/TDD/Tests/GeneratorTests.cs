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
using Microsoft.Samples.VisualStudio.GeneratorSample;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell;
using System.Collections;

namespace Microsoft.Samples.VisualStudio.Project.GeneratorSample.UnitTests.Tests
{
    /// <summary>
    /// This class tests the XmlClassGenerator class
    /// </summary>
    [TestClass]
    public class GeneratorTests
    {
        private XmlClassGenerator _generator = null;

        /// <summary>
        /// Gets an appropiately populated attribute instance
        /// </summary>
        [CLSCompliant(false)]
        public XmlClassGenerator Generator
        {
            get
            {
                if (_generator == null)
                {
                    _generator = new XmlClassGenerator();
                }
                return _generator;
            }
        }

        /// <summary>
        /// This scenario tests if an instance of the generator can be constructed
        /// </summary>
        [TestMethod()]
        public void CreateInstance()
        {
            Assert.IsNotNull(this.Generator);
        }

        /// <summary>
        /// This scenario tests if you can QI for IVsSingleFileGenerator
        /// </summary>
        [TestMethod()]
        public void CheckForIVsSingleFileGenerator()
        {
            IVsSingleFileGenerator sfg = Generator as IVsSingleFileGenerator;
            Assert.IsNotNull(sfg);
        }

    }
}
