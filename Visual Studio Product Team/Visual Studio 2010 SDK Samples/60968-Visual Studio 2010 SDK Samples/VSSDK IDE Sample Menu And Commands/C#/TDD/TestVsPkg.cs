/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

// System references.
using System;
using System.Reflection;

// Platform references
using Microsoft.VisualStudio.Shell.Interop;

// Unit Test framework
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// Assembly to test
using Microsoft.Samples.VisualStudio.MenuCommands;

namespace Microsoft.Samples.VisualStudio.MenuAndCommands.UnitTest {

    [TestClass()]
    public class MenuAndCommandPackageTest {

        [TestMethod()]
        public void PackageCreation() {
            MenuCommandsPackage package = new MenuCommandsPackage();
            Assert.IsNotNull(package, "Failed to create package");
        }

        [TestMethod()]
        public void PackageInitialize() {
            MenuCommandsPackage packageObject = new MenuCommandsPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");
            IVsPackage package = packageObject as IVsPackage;
            Assert.IsNotNull(package, "Can not get the package interface from the package object.");
            using (OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices())
            {
                Assert.AreEqual(0, package.SetSite(serviceProvider), "Can not get the package interface from the package object.");
            }
        }

        [TestMethod()]
        public void PackageInitializeWithNull()
        {
            MenuCommandsPackage packageObject = new MenuCommandsPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");
            IVsPackage package = packageObject as IVsPackage;
            Assert.IsNotNull(package, "Can not get the package interface from the package object.");
            Assert.AreEqual(0, package.SetSite(null), "SetSite returned an unexpected value");
        }
    }

}
