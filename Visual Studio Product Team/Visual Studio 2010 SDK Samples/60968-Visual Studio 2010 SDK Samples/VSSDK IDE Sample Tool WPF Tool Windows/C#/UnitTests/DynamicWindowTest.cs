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
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Samples.VisualStudio.IDE.ToolWindow;

namespace Microsoft.Samples.VisualStudio.IDE.ToolWindow.UnitTests
{
	[TestClass()]
	public class DynamicWindowTest
	{
		[TestMethod()]
		public void ShowDynamicWindow0()
		{
			// Create the package
			PackageToolWindow package = new PackageToolWindow();
			// Create a basic service provider
			OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

			// Add site support to create and enumerate tool windows
			BaseMock uiShell = MockUiShellProvider.GetWindowEnumerator0();
			serviceProvider.AddService(typeof(SVsUIShell), uiShell, false);

			// Site the package
            Assert.AreEqual(0, ((IVsPackage)package).SetSite(serviceProvider), "SetSite did not return S_OK");

			MethodInfo method = typeof(PackageToolWindow).GetMethod("ShowDynamicWindow", BindingFlags.NonPublic | BindingFlags.Instance);

			object result = method.Invoke(package, new object[] { null, null });
		}
	}
}
