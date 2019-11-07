/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections;
using System.Text;
using System.Reflection;
using System.ComponentModel.Design;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider;

using MsVsShell = Microsoft.VisualStudio.Shell;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.UnitTests
{
    public class TestSccCommand
    {
        public void OnSccCommand(object sender, EventArgs e)
        {
        }
    }

	[TestClass()]
	public class PackageTest
	{
		[TestMethod()]
		public void CreateInstance()
		{
			BasicSccProvider package = new BasicSccProvider();
		}

        [TestMethod()]
		public void IsIVsPackage()
		{
			BasicSccProvider package = new BasicSccProvider();
            Assert.IsNotNull(package as IVsPackage, "The object does not implement IVsPackage");
		}

        [TestMethod()]
		public void SetSite()
		{
			// Create the package
			IVsPackage package = new BasicSccProvider() as IVsPackage;
            Assert.IsNotNull(package, "The object does not implement IVsPackage");

			// Create a basic service provider
			OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Need to mock a service implementing IVsRegisterScciProvider, because the scc provider will register with it
            IVsRegisterScciProvider registerScciProvider = MockRegisterScciProvider.GetBaseRegisterScciProvider();
            serviceProvider.AddService(typeof(IVsRegisterScciProvider), registerScciProvider, true);

			// Site the package
            Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");

			// Unsite the package
            Assert.AreEqual(0, package.SetSite(null), "SetSite(null) did not return S_OK");

            // Remove the mock service
            serviceProvider.RemoveService(typeof(IVsRegisterScciProvider));
		}


        [TestMethod()]
        public void TestSccComand()
        {
            // Create the package
            BasicSccProvider package = new BasicSccProvider();
            // Create a basic service provider
            OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Need to mock a service implementing IVsRegisterScciProvider, because the scc provider will register with it
            IVsRegisterScciProvider registerScciProvider = MockRegisterScciProvider.GetBaseRegisterScciProvider();
            serviceProvider.AddService(typeof(IVsRegisterScciProvider), registerScciProvider, true);

            // Site the package
            Assert.AreEqual(0, ((IVsPackage)package).SetSite(serviceProvider), "SetSite did not return S_OK");

            // Test the scc command by toggleing it twice
            MethodInfo method = typeof(BasicSccProvider).GetMethod("OnSccCommand", BindingFlags.NonPublic | BindingFlags.Instance);

            TestSccCommand commandWell = new TestSccCommand();
            CommandID cmdID = new CommandID(new System.Guid(), 0);
            MenuCommand command = new MenuCommand(new EventHandler(commandWell.OnSccCommand), cmdID);
            object result = method.Invoke(package, new object[] { command, null });
            Assert.AreEqual(true, command.Checked, "OnSccCommand did not execute correctly");

            result = method.Invoke(package, new object[] { command, null });
            Assert.AreEqual(false, command.Checked, "OnSccCommand did not execute correctly");
        }
	}
}
