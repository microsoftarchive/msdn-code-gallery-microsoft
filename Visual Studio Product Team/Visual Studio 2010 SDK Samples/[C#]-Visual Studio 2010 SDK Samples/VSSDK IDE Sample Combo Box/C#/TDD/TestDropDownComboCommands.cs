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
using Microsoft.Samples.VisualStudio.ComboBox;
using System.Reflection;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;

namespace ComboBox.UnitTest
{
    /// <summary>
    /// Summary description for TestDropDownComboCommands
    /// </summary>
    [TestClass]
    public class TestDropDownComboCommands
    {
        private string[] expectedDropDownComboChoices = { "Apples", "Oranges", "Pears", "Bananas" };
        private string expectedInitialDropDownComboChoice = "Apples";

        public TestDropDownComboCommands()
        {
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestDropDownComboNoParams()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");
            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsTrue(Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, null }); }));
        }

        [TestMethod]
        public void TestDropDownComboInvalidInParamValue()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam = "Non-valid string";
            IntPtr outParam = IntPtr.Zero;
            OleMenuCmdEventArgs eventArgs = new OleMenuCmdEventArgs(inParam, outParam);
            Assert.IsTrue(Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, eventArgs }); }));
        }

        [TestMethod]
        public void TestDropDownComboBothInOutParamsGiven()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam = "Oranges";
            IntPtr outParam = Marshal.AllocCoTaskMem(64);   //64 == size of a variant + a little extra padding;
            try
            {
                OleMenuCmdEventArgs eventArgs = new OleMenuCmdEventArgs(inParam, outParam);
                bool hasThrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, eventArgs }); });
                Assert.IsTrue(hasThrown);
            }
            finally
            {
                if (outParam != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(outParam);
            }
        }

        [TestMethod]
        public void TestDropDownComboInvalidInParamEmptyString()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam = String.Empty;
            IntPtr outParam = IntPtr.Zero;

            OleMenuCmdEventArgs eventArgs = new OleMenuCmdEventArgs(inParam, outParam);
            bool hasThrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, eventArgs }); });
            Assert.IsTrue(hasThrown);
        }

        [TestMethod]
        public void TestDropDownComboInvalidInParamNumber()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam = 42;
            IntPtr outParam = IntPtr.Zero;

            OleMenuCmdEventArgs eventArgs = new OleMenuCmdEventArgs(inParam, outParam);
            bool hasThrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, eventArgs }); });
            Assert.IsTrue(hasThrown);
        }

        [TestMethod]
        public void TestDropDownComboInvalidInParamObject()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam = new object();
            IntPtr outParam = IntPtr.Zero;

            OleMenuCmdEventArgs eventArgs = new OleMenuCmdEventArgs(inParam, outParam);
            bool hasThrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, eventArgs }); });
            Assert.IsTrue(hasThrown);
        }

        [TestMethod]
        public void TestDropDownComboNoInOutParams()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam = null;
            IntPtr outParam = IntPtr.Zero;

            OleMenuCmdEventArgs eventArgs = new OleMenuCmdEventArgs(inParam, outParam);
            bool hasThrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, eventArgs }); });
            Assert.IsTrue(hasThrown);
        }

        [TestMethod]
        public void TestDropDownComboEmptyEventArgs()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            bool hasThrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, EventArgs.Empty }); });
            Assert.IsTrue(hasThrown);

            hasThrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, null }); });
            Assert.IsTrue(hasThrown);

            EventArgs args = new EventArgs();
            hasThrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, args }); });
            Assert.IsTrue(hasThrown);
        }

        [TestMethod]
        public void TestDropDownComboGetCurVal()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");
            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam = null;
            IntPtr outParam = Marshal.AllocCoTaskMem(64);   //64 == size of a variant + a little extra padding
            try
            {
                OleMenuCmdEventArgs eventArgs = new OleMenuCmdEventArgs(inParam, outParam);
                object result = method.Invoke(packageObject, new object[] { null, eventArgs });
                
                string retrieved = (string)Marshal.GetObjectForNativeVariant(outParam);
                Assert.AreEqual<string>(expectedInitialDropDownComboChoice, retrieved);
            }
            finally
            {
                if (outParam != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(outParam);
            }
        }

        [TestMethod]
        public void TestDropDownComboSetCurVal()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");
            OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Add site support to create and enumerate tool windows
            GenericMockFactory mockFactory = new GenericMockFactory("MockUIShell", new Type[] { typeof(IVsUIShell) });
            BaseMock uiShell = mockFactory.GetInstance();
            serviceProvider.AddService(typeof(SVsUIShell), uiShell, false);

            // Site the package
            Assert.AreEqual(0, ((IVsPackage)packageObject).SetSite(serviceProvider), "SetSite did not return S_OK");

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDropDownCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam1 = "Oranges";
            IntPtr outParam1 = IntPtr.Zero;
            object inParam2 = null;
            IntPtr outParam2 = Marshal.AllocCoTaskMem(64);   //64 == size of a variant + a little extra padding
            try
            {
                // Set DropDownCombo to "Oranges"
                OleMenuCmdEventArgs eventArgs1 = new OleMenuCmdEventArgs(inParam1, outParam1);
                object result = method.Invoke(packageObject, new object[] { null, eventArgs1 });

                // Retrieve current value of DropDown and verify it is "Oranges"
                OleMenuCmdEventArgs eventArgs2 = new OleMenuCmdEventArgs(inParam2, outParam2);
                result = method.Invoke(packageObject, new object[] { null, eventArgs2 });
                
                string retrieved = (string)Marshal.GetObjectForNativeVariant(outParam2);
                Assert.AreEqual<string>((string)inParam1, retrieved);

                Assert.AreEqual(1, uiShell.FunctionCalls(String.Format("{0}.{1}", typeof(IVsUIShell).FullName, "ShowMessageBox")), "IVsUIShell.ShowMessageBox was not called");
            }
            finally
            {
                if (outParam2 != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(outParam2);
            }
        }

        [TestMethod]
        public void TestDropDownComboGetListNoInOutParams()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");
            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDropDownComboGetList", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam = null;
            IntPtr outParam = IntPtr.Zero;

            OleMenuCmdEventArgs eventArgs = new OleMenuCmdEventArgs(inParam, outParam);
            Assert.IsTrue(Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, eventArgs }); }));
        }

        [TestMethod]
        public void TestDropDownComboGetListInParamGiven()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");
            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDropDownComboGetList", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam = "Oranges";
            IntPtr outParam = Marshal.AllocCoTaskMem(64);   //64 == size of a variant + a little extra padding;
            try
            {
                OleMenuCmdEventArgs eventArgs = new OleMenuCmdEventArgs(inParam, outParam);
                Assert.IsTrue(Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, eventArgs }); }));
            }
            finally
            {
                if (outParam != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(outParam);
            }
        }

        [TestMethod]
        public void TestDropDownComboGetListEmptyEventArgs()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDropDownComboGetList", BindingFlags.NonPublic | BindingFlags.Instance);

            bool hasThrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, EventArgs.Empty }); });
            Assert.IsTrue(hasThrown);

            hasThrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, null }); });
            Assert.IsTrue(hasThrown);
        }

        [TestMethod]
        public void TestDropDownComboGetList()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");
            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDropDownComboGetList", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam = null;
            IntPtr outParam = Marshal.AllocCoTaskMem(64);   //64 == size of a variant + a little extra padding
            try
            {
                OleMenuCmdEventArgs eventArgs = new OleMenuCmdEventArgs(inParam, outParam);
                object result = method.Invoke(packageObject, new object[] { null, eventArgs });
                
                string[] retrieved = (string[])Marshal.GetObjectForNativeVariant(outParam);
                Utilities.SameArray<string>(expectedDropDownComboChoices, retrieved);
            }
            finally
            {
                if (outParam != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(outParam);
            }
        }
    }
}
