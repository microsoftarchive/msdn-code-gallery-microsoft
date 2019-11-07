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
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.VisualStudio.IDE.ToolWindow;

namespace Microsoft.Samples.VisualStudio.IDE.ToolWindow.UnitTests
{
	[TestClass()]
	public class PersistedWindowTest
	{
		[TestMethod()]
		public void ShowPersistedWindow0()
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

			MethodInfo method = typeof(PackageToolWindow).GetMethod("ShowPersistedWindow", BindingFlags.NonPublic | BindingFlags.Instance);

			object result = method.Invoke(package, new object[] { null, null });
		}

		[TestMethod()]
		public void ShowPersistedWindow2()
		{
			// Create the package
			PackageToolWindow package = new PackageToolWindow();
			// Create a basic service provider
			OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

			// Add site support to create and enumerate tool windows
			BaseMock uiShell = MockUiShellProvider.GetWindowEnumerator2();
			serviceProvider.AddService(typeof(SVsUIShell), uiShell, false);

			// Site the package
            Assert.AreEqual(0, ((IVsPackage)package).SetSite(serviceProvider), "SetSite did not return S_OK");

			MethodInfo method = typeof(PackageToolWindow).GetMethod("ShowPersistedWindow", BindingFlags.NonPublic | BindingFlags.Instance);

			object result = method.Invoke(package, new object[] { null, null });
		}

		/*
		 * There's an issue with this test which result in the mock iterator Next implementation passing
		 * in null instead of the array of frame.
		 * There could potentially be other issues pass that.
		[Test()]
		public void ListViewControl()
		{
			// In order to test event, we need the Package global service provider to be garanteed to
			// have been initialized, so in case we are called before other method that site a package
			// we create a package and site it.
			PackageToolWindow package = new PackageToolWindow();
			// Create a basic service provider
			OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
			// Add site support to create and enumerate tool windows
			BaseMock uiShell = MockUiShellProvider.GetWindowEnumerator2();
			serviceProvider.AddService(typeof(SVsUIShell), uiShell, false);
			// Site the package
			Assertion.AssertEquals("SetSite did not return S_OK", 0, ((IVsPackage)package).SetSite(serviceProvider));

			// Create the control
			PersistedWindowControl control = new PersistedWindowControl();

			// We need to provide it a TrackSelection service
			PropertyInfo trackSelectionProperty = typeof(PersistedWindowControl).GetProperty("TrackSelection", BindingFlags.NonPublic | BindingFlags.Instance);
			trackSelectionProperty.SetValue(control, (ITrackSelection)MockWindowFrameProvider.TrackSelectionFactory.GetInstance(), null);

			// We are ready to test the event handler
			MethodInfo method = typeof(PersistedWindowControl).GetMethod("listView1_SelectedIndexChanged", BindingFlags.NonPublic | BindingFlags.Instance);

			// This is testing with empty selection
			object result = method.Invoke(control, new object[] { null, null });

			////////////////////////////////////////////////////////
			// At this point we want to test with something selected

			// Populate the window
			MethodInfo refreshData = typeof(PersistedWindowControl).GetMethod("RefreshData", BindingFlags.NonPublic | BindingFlags.Instance);
			result = refreshData.Invoke(control, new object[] { });
			// Get the listview
			FieldInfo listViewField = typeof(PersistedWindowControl).GetField("listView1", BindingFlags.NonPublic | BindingFlags.Instance);
			ListView listView = (ListView)listViewField.GetValue(control);
			Assertion.AssertEquals("The listview should contains 2 items", 2, listView.Items.Count);
			// Select the first item
			listView.Items[0].Selected = true;

			// This is testing with a selection
			result = method.Invoke(control, new object[] { null, null });
		}
		 */
	}
}
