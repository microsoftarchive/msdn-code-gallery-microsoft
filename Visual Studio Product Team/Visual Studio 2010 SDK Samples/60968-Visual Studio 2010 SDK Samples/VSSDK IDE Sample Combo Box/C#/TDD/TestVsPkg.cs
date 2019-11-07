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
using Microsoft.Samples.VisualStudio.ComboBox;

namespace Microsoft.Samples.VisualStudio.ComboBox.UnitTest {

    [TestClass()]
    public class ComboBoxPackageTest {

        [TestMethod()]
        public void PackageCreation() {
            ComboBoxPackage package = new ComboBoxPackage();
            Assert.IsNotNull(package, "Failed to create package");
        }

        [TestMethod()]
        public void PackageInitialize() {
            ComboBoxPackage packageObject = new ComboBoxPackage();
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
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");
            IVsPackage package = packageObject as IVsPackage;
            Assert.IsNotNull(package, "Can not get the package interface from the package object.");
            Assert.AreEqual(0, package.SetSite(null), "SetSite returned an unexpected value");
        }

        [TestMethod()]
        public void PackageResourcesCreation()
        {
            System.Type t = System.Type.GetType("Microsoft.Samples.VisualStudio.ComboBox.Resources, Reference.ComboBox");
            object resources = System.Activator.CreateInstance(t, true);

            Assert.IsNotNull(resources, "Failed to create resources");

            MethodInfo method_get_Culture = t.GetMethod("get_Culture", BindingFlags.NonPublic | BindingFlags.Static);
            object result1 = method_get_Culture.Invoke(null, null);

            MethodInfo method_set_Culture = t.GetMethod("set_Culture", BindingFlags.NonPublic | BindingFlags.Static);
            object result2 = method_set_Culture.Invoke(null, new object[] { System.Globalization.CultureInfo.CurrentCulture });
        }
    }
}
