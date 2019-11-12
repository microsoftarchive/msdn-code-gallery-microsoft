/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
//using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.VisualStudio.ComboBox;
using System.Reflection;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;
using System.Globalization;

namespace ComboBox.UnitTest
{
    /// <summary>
    /// Summary description for TestDynamicComboCommands
    /// </summary>
    [TestClass]
    public class TestDynamicComboCommands
    {
        private double[] numericZoomLevels = { 4.0, 3.0, 2.0, 1.5, 1.25, 1.0, .75, .66, .50, .33, .25, .10 };
        //private int[] numericZoomLevels = { 400, 300, 200, 150, 125, 100, 75, 66, 50, 33, 25, 10 };
        private string zoomToFit = "ZoomToFit";
        private string zoom_to_Fit = "Zoom to Fit";
        private string[] zoomLevels = null;
        //private NumberFormatInfo numberFormatInfo;
        private string expectedInitialZoomFactor = "100 %";

        public TestDynamicComboCommands()
        {
            NumberFormatInfo numberFormat = (NumberFormatInfo)CultureInfo.CurrentUICulture.NumberFormat.Clone();
            zoomLevels = new String[numericZoomLevels.Length + 1];
            for (int i = 0; i < numericZoomLevels.Length; i++)
            {
                zoomLevels[i] = numericZoomLevels[i].ToString("P0", numberFormat);
                //zoomLevels[i] = numericZoomLevels[i].ToString() + "%";
            }

            zoomLevels[zoomLevels.Length - 1] = zoom_to_Fit;
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
        public void TestDynamicComboNoParams()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");
            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            bool hasThrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, null }); });
            Assert.IsTrue(hasThrown);

            EventArgs args = new EventArgs();
            hasThrown = Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, args }); });
            Assert.IsTrue(hasThrown);
        }

        [TestMethod]
        public void TestDynamicComboInvalidInParamValue()
        {
            // NOTE: invalid input is ignored and treated as a NOP
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam = "Non-valid string";
            IntPtr outParam = IntPtr.Zero;

            OleMenuCmdEventArgs eventArgs = new OleMenuCmdEventArgs(inParam, outParam);
            object result = method.Invoke(packageObject, new object[] { null, eventArgs });
        }

        [TestMethod]
        public void TestDynamicComboBothInOutParamsGiven()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam = "200";
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
        public void TestDynamicComboInvalidInParamEmptyString()
        {
            // NOTE: invalid input is ignored and treated as a NOP
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam = String.Empty;
            IntPtr outParam = IntPtr.Zero;

            OleMenuCmdEventArgs eventArgs = new OleMenuCmdEventArgs(inParam, outParam);
            object result = method.Invoke(packageObject, new object[] { null, eventArgs });
        }

        [TestMethod]
        public void TestDynamicComboInvalidInParamObject()
        {
            // NOTE: invalid input is ignored and treated as a NOP
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam = new object();
            IntPtr outParam = IntPtr.Zero;

            OleMenuCmdEventArgs eventArgs = new OleMenuCmdEventArgs(inParam, outParam);
            object result = method.Invoke(packageObject, new object[] { null, eventArgs });
        }

        [TestMethod]
        public void TestDynamicComboNoInOutParams()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam = null;
            IntPtr outParam = IntPtr.Zero;

            OleMenuCmdEventArgs eventArgs = new OleMenuCmdEventArgs(inParam, outParam);
            Assert.IsTrue(Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, eventArgs }); }));
        }

        [TestMethod]
        public void TestDynamicComboEmptyEventArgs()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsTrue(Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, EventArgs.Empty }); }));
        }

        [TestMethod]
        public void TestDynamicComboGetCurVal()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");
            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam = null;
            IntPtr outParam = Marshal.AllocCoTaskMem(64);   //64 == size of a variant + a little extra padding
            try
            {
                OleMenuCmdEventArgs eventArgs = new OleMenuCmdEventArgs(inParam, outParam);
                object result = method.Invoke(packageObject, new object[] { null, eventArgs });
                
                string retrieved = (string)Marshal.GetObjectForNativeVariant(outParam);
                Assert.AreEqual<string>(expectedInitialZoomFactor, retrieved);
            }
            finally
            {
                if (outParam != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(outParam);
            }
        }

        [TestMethod]
        public void TestDynamicComboSetCurValWithString()
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

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam1 = zoomLevels[1];
            IntPtr outParam1 = IntPtr.Zero;
            object inParam2 = null;
            IntPtr outParam2 = Marshal.AllocCoTaskMem(64);   //64 == size of a variant + a little extra padding
            try
            {
                // Set DynamicCombo to 2nd choice in list
                OleMenuCmdEventArgs eventArgs1 = new OleMenuCmdEventArgs(inParam1, outParam1);
                object result = method.Invoke(packageObject, new object[] { null, eventArgs1 });

                // Retrieve current value of Zoom and verify
                OleMenuCmdEventArgs eventArgs2 = new OleMenuCmdEventArgs(inParam2, outParam2);
                result = method.Invoke(packageObject, new object[] { null, eventArgs2 });

                string retrieved = (string)Marshal.GetObjectForNativeVariant(outParam2);
                Assert.AreEqual<string>(inParam1.ToString(), retrieved);

                Assert.AreEqual(1, uiShell.FunctionCalls(String.Format("{0}.{1}", typeof(IVsUIShell).FullName, "ShowMessageBox")), "IVsUIShell.ShowMessageBox was not called");
            }
            finally
            {
                if (outParam2 != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(outParam2);
            }
        }

        [TestMethod]
        public void TestDynamicComboSetCurValWithInt()
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

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            int inChoice = 17;
            object inParam1 = inChoice;
            IntPtr outParam1 = IntPtr.Zero;
            object inParam2 = null;
            IntPtr outParam2 = Marshal.AllocCoTaskMem(64);   //64 == size of a variant + a little extra padding
            try
            {
                // Set DynamicCombo to 17 %
                OleMenuCmdEventArgs eventArgs1 = new OleMenuCmdEventArgs(inParam1, outParam1);
                object result = method.Invoke(packageObject, new object[] { null, eventArgs1 });

                // Retrieve current value of ZoomLevel and verify it is "17 %"
                OleMenuCmdEventArgs eventArgs2 = new OleMenuCmdEventArgs(inParam2, outParam2);
                result = method.Invoke(packageObject, new object[] { null, eventArgs2 });
                
                string retrieved = (string)Marshal.GetObjectForNativeVariant(outParam2);
                Assert.AreEqual<string>(String.Format("{0} %", inChoice), retrieved);

                Assert.AreEqual(1, uiShell.FunctionCalls(String.Format("{0}.{1}", typeof(IVsUIShell).FullName, "ShowMessageBox")), "IVsUIShell.ShowMessageBox was not called");
            }
            finally
            {
                if (outParam2 != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(outParam2);
            }
        }

        [TestMethod]
        public void TestDynamicComboSetCurValWithDouble()
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

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            double inChoice = 82.756;
            object inParam1 = inChoice.ToString();
            IntPtr outParam1 = IntPtr.Zero;
            object inParam2 = null;
            IntPtr outParam2 = Marshal.AllocCoTaskMem(64);   //64 == size of a variant + a little extra padding
            try
            {
                // Set DynamicCombo to 82.756
                OleMenuCmdEventArgs eventArgs1 = new OleMenuCmdEventArgs(inParam1, outParam1);
                object result = method.Invoke(packageObject, new object[] { null, eventArgs1 });

                // Retrieve current value of Zoom Level and verify it is "83 %"
                OleMenuCmdEventArgs eventArgs2 = new OleMenuCmdEventArgs(inParam2, outParam2);
                result = method.Invoke(packageObject, new object[] { null, eventArgs2 });

                string retrieved = (string)Marshal.GetObjectForNativeVariant(outParam2);
                Assert.AreEqual<string>(String.Format("{0} %", (int)Math.Round(inChoice)), retrieved); 

                Assert.AreEqual(1, uiShell.FunctionCalls(String.Format("{0}.{1}", typeof(IVsUIShell).FullName, "ShowMessageBox")), "IVsUIShell.ShowMessageBox was not called");
            }
            finally
            {
                if (outParam2 != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(outParam2);
            }
        }

        [TestMethod]
        public void TestDynamicComboSetCurValWithZoomToFit()
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

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam1 = zoomToFit;
            IntPtr outParam1 = IntPtr.Zero;
            object inParam2 = null;
            IntPtr outParam2 = Marshal.AllocCoTaskMem(64);   //64 == size of a variant + a little extra padding
            try
            {
                // Set DynamicCombo to "Zoom to Fit"
                OleMenuCmdEventArgs eventArgs1 = new OleMenuCmdEventArgs(inParam1, outParam1);
                object result = method.Invoke(packageObject, new object[] { null, eventArgs1 });

                // Retrieve current value of Zoom Level and verify it is "Zoom to Fit"
                OleMenuCmdEventArgs eventArgs2 = new OleMenuCmdEventArgs(inParam2, outParam2);
                result = method.Invoke(packageObject, new object[] { null, eventArgs2 });

                string retrieved = (string)Marshal.GetObjectForNativeVariant(outParam2);
                Assert.AreEqual<string>(zoom_to_Fit, retrieved); 

                Assert.AreEqual(1, uiShell.FunctionCalls(String.Format("{0}.{1}", typeof(IVsUIShell).FullName, "ShowMessageBox")), "IVsUIShell.ShowMessageBox was not called");
            }
            finally
            {
                if (outParam2 != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(outParam2);
            }
        }

        [TestMethod]
        public void TestDynamicComboSetCurValWithZoom_To_Fit()
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

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam1 = zoom_to_Fit;
            IntPtr outParam1 = IntPtr.Zero;
            object inParam2 = null;
            IntPtr outParam2 = Marshal.AllocCoTaskMem(64);   //64 == size of a variant + a little extra padding
            try
            {
                // Set DynamicCombo to "Zoom to Fit"
                OleMenuCmdEventArgs eventArgs1 = new OleMenuCmdEventArgs(inParam1, outParam1);
                object result = method.Invoke(packageObject, new object[] { null, eventArgs1 });

                // Retrieve current value of Zoom Level and verify it is "Zoom to Fit"
                OleMenuCmdEventArgs eventArgs2 = new OleMenuCmdEventArgs(inParam2, outParam2);
                result = method.Invoke(packageObject, new object[] { null, eventArgs2 });

                string retrieved = (string)Marshal.GetObjectForNativeVariant(outParam2);
                Assert.AreEqual<string>(zoom_to_Fit, retrieved);

                Assert.AreEqual(1, uiShell.FunctionCalls(String.Format("{0}.{1}", typeof(IVsUIShell).FullName, "ShowMessageBox")), "IVsUIShell.ShowMessageBox was not called");
            }
            finally
            {
                if (outParam2 != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(outParam2);
            }
        }

        [TestMethod]
        public void TestDynamicComboSetCurValWithNegativeInt()
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

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            int inChoice = -1;
            object inParam1 = inChoice.ToString();
            IntPtr outParam1 = IntPtr.Zero;
            object inParam2 = null;
            IntPtr outParam2 = Marshal.AllocCoTaskMem(64);   //64 == size of a variant + a little extra padding
            try
            {
                // Set DynamicCombo to -1
                OleMenuCmdEventArgs eventArgs1 = new OleMenuCmdEventArgs(inParam1, outParam1);
                Assert.IsTrue(Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, eventArgs1 }); }));

                OleMenuCmdEventArgs eventArgs2 = new OleMenuCmdEventArgs(inParam2, outParam2);
                object result = method.Invoke(packageObject, new object[] { null, eventArgs2 });
            }
            finally
            {
                if (outParam2 != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(outParam2);
            }
        }

        [TestMethod]
        public void TestDynamicComboSetCurValWithOverflowInt()
        {
            // NOTE: invalid input is ignored and treated as a NOP
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");
            OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Add site support to create and enumerate tool windows
            GenericMockFactory mockFactory = new GenericMockFactory("MockUIShell", new Type[] { typeof(IVsUIShell) });
            BaseMock uiShell = mockFactory.GetInstance();
            serviceProvider.AddService(typeof(SVsUIShell), uiShell, false);

            // Site the package
            Assert.AreEqual(0, ((IVsPackage)packageObject).SetSite(serviceProvider), "SetSite did not return S_OK");

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDynamicCombo", BindingFlags.NonPublic | BindingFlags.Instance);

            Int64 inChoice = Int64.MaxValue;
            object inParam1 = inChoice.ToString() + inChoice.ToString() + inChoice.ToString();
            IntPtr outParam1 = IntPtr.Zero;
            object inParam2 = null;
            IntPtr outParam2 = Marshal.AllocCoTaskMem(64);   //64 == size of a variant + a little extra padding
            try
            {
                // Set DynamicCombo to overflow value
                OleMenuCmdEventArgs eventArgs1 = new OleMenuCmdEventArgs(inParam1, outParam1);
                object result = method.Invoke(packageObject, new object[] { null, eventArgs1 });

                // Retrieve current value of Zoom and verify
                OleMenuCmdEventArgs eventArgs2 = new OleMenuCmdEventArgs(inParam2, outParam2);
                result = method.Invoke(packageObject, new object[] { null, eventArgs2 });

                string retrieved = (string)Marshal.GetObjectForNativeVariant(outParam2);
            }
            finally
            {
                if (outParam2 != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(outParam2);
            }
        }

        [TestMethod]
        public void TestDynamicComboGetListNoInOutParams()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");
            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDynamicComboGetList", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam = null;
            IntPtr outParam = IntPtr.Zero;

            OleMenuCmdEventArgs eventArgs = new OleMenuCmdEventArgs(inParam, outParam);
            Assert.IsTrue(Utilities.HasFunctionThrown<ArgumentException>(delegate { method.Invoke(packageObject, new object[] { null, eventArgs }); }));
        }

        [TestMethod]
        public void TestDynamicComboGetListInParamGiven()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");
            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDynamicComboGetList", BindingFlags.NonPublic | BindingFlags.Instance);

            object inParam = "73.2";
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
        public void TestDynamicComboGetListEmptyEventArgs()
        {
            ComboBoxPackage packageObject = new ComboBoxPackage();
            Assert.IsNotNull(packageObject, "Failed to create package");

            MethodInfo method = typeof(ComboBoxPackage).GetMethod("OnMenuMyDynamicComboGetList", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsTrue(Utilities.HasFunctionThrown<ArgumentNullException>(delegate { method.Invoke(packageObject, new object[] { null, EventArgs.Empty }); }));

            Assert.IsTrue(Utilities.HasFunctionThrown<ArgumentNullException>(delegate { method.Invoke(packageObject, new object[] { null, null }); }));
        }
    }
}
