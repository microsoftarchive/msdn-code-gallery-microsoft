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
using EnvDTE;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.CommandBars;

namespace Microsoft.Samples.VisualStudio.ComboBox.IntegrationTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ComboBoxIntegrationTest
    {
        public ComboBoxIntegrationTest()
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

        private delegate void ThreadInvoker();

        /// <summary>
        /// Test calling the Tools.DropDownCombo command.
        /// </summary>
        [TestMethod]
        [HostType("VS IDE")]
        public void TestDropDownComboBox()
        {
            UIThreadInvoker.Invoke((ThreadInvoker)delegate()
            {
                //Get the global service provider and the dte
                IServiceProvider sp = VsIdeTestHostContext.ServiceProvider;
                DTE dte = (DTE)sp.GetService(typeof(DTE));

                // Create the messageBoxListener Thread.
                string expectedDialogBoxText = String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}\n\n{1}", "My DropDown Combo", "Bananas");
                DialogBoxPurger purger = new DialogBoxPurger(NativeMethods.IDOK, expectedDialogBoxText);

                try
                {
                    purger.Start();

                    dte.ExecuteCommand("Tools.DropDownCombo", "Bananas");
                }
                finally
                {
                    Assert.IsTrue(purger.WaitForDialogThreadToTerminate(), "The Drop Down Combo dialog box has not shown");
                }
            });
        }

        /// <summary>
        /// Test calling the Tools.IndexCombo command.
        /// </summary>
        [TestMethod]
        [HostType("VS IDE")]
        public void TestIndexComboBox()
        {
            UIThreadInvoker.Invoke((ThreadInvoker)delegate()
            {
                //Get the global service provider and the dte
                IServiceProvider sp = VsIdeTestHostContext.ServiceProvider;
                DTE dte = (DTE)sp.GetService(typeof(DTE));

                //Show toolbar
                CommandBars commandBars = (CommandBars)dte.CommandBars;
                commandBars["ComboBoxSample"].Visible = true;

                // Create the messageBoxListener Thread.
                string expectedDialogBoxText = String.Format(System.Globalization.CultureInfo.CurrentCulture, 
                    "{0}\n\n{1}", "My Index Combo", "1");
                DialogBoxPurger purger = new DialogBoxPurger(NativeMethods.IDOK, expectedDialogBoxText);

                try
                {
                    purger.Start();

                    dte.ExecuteCommand("Tools.IndexCombo", "Tigers");
                }
                finally
                {
                    Assert.IsTrue(purger.WaitForDialogThreadToTerminate(), "The Index Combo dialog box has not shown");
                }
            });
        }

        /// <summary>
        /// Test calling the Tools.MRUCombo command.
        /// </summary>
        [TestMethod]
        [HostType("VS IDE")]
        public void TestMRUComboBox()
        {
            UIThreadInvoker.Invoke((ThreadInvoker)delegate()
            {
                //Get the global service provider and the dte
                IServiceProvider sp = VsIdeTestHostContext.ServiceProvider;
                DTE dte = (DTE)sp.GetService(typeof(DTE));

                //Show toolbar
                CommandBars commandBars = (CommandBars)dte.CommandBars;
                commandBars["ComboBoxSample"].Visible = true;

                // Create the messageBoxListener Thread.
                string expectedDialogBoxText = String.Format(System.Globalization.CultureInfo.CurrentCulture,
                    "{0}\n\n{1}", "My MRU Combo", "Hello World!");
                DialogBoxPurger purger = new DialogBoxPurger(NativeMethods.IDOK, expectedDialogBoxText);

                try
                {
                    purger.Start();

                    dte.ExecuteCommand("Tools.MRUCombo", "Hello World!");
                }
                finally
                {
                    Assert.IsTrue(purger.WaitForDialogThreadToTerminate(), "The MRU Combo dialog box has not shown");
                }
            });
        }

        /// <summary>
        /// Test calling the Tools.DynamicCombo command.
        /// </summary>
        [TestMethod]
        [HostType("VS IDE")]
        public void TestDynamicComboBox()
        {
            UIThreadInvoker.Invoke((ThreadInvoker)delegate()
            {
                //Get the global service provider and the dte
                IServiceProvider sp = VsIdeTestHostContext.ServiceProvider;
                DTE dte = (DTE)sp.GetService(typeof(DTE));

                //Show toolbar
                CommandBars commandBars = (CommandBars)dte.CommandBars;
                commandBars["ComboBoxSample"].Visible = true;

                // Create the messageBoxListener Thread.
                string expectedDialogBoxText = String.Format(System.Globalization.CultureInfo.CurrentCulture,
                    "{0}\n\n{1}", "My Dynamic Combo", "72");
                DialogBoxPurger purger = new DialogBoxPurger(NativeMethods.IDOK, expectedDialogBoxText);

                try
                {
                    purger.Start();

                    dte.ExecuteCommand("Tools.DynamicCombo", "72");
                }
                finally
                {
                    Assert.IsTrue(purger.WaitForDialogThreadToTerminate(), "The Dynamic Combo dialog box has not shown");
                }
            });
        }
    }
}
