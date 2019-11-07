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
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
{
    [TestClass()]
    public class SccProviderToolWindowTest
    {
        [TestMethod()]
        public void UseToolWindow()
        {
            // Create the package
            SccProvider package = new SccProvider();
            // Create a basic service provider
            OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Need to mock a service implementing IVsRegisterScciProvider, because the scc provider will register with it
            IVsRegisterScciProvider registerScciProvider = MockRegisterScciProvider.GetBaseRegisterScciProvider();
            serviceProvider.AddService(typeof(IVsRegisterScciProvider), registerScciProvider, true);

            // Add site support to create and enumerate tool windows
            BaseMock uiShell = MockUiShellProvider.GetWindowEnumerator0();
            serviceProvider.AddService(typeof(SVsUIShell), uiShell, false);

            // Register solution events because the provider will try to subscribe to them
            MockSolution solution = new MockSolution();
            serviceProvider.AddService(typeof(SVsSolution), solution as IVsSolution, true);

            // Register TPD service because the provider will try to subscribe to TPD
            IVsTrackProjectDocuments2 tpd = MockTrackProjectDocumentsProvider.GetTrackProjectDocuments() as IVsTrackProjectDocuments2;
            serviceProvider.AddService(typeof(SVsTrackProjectDocuments), tpd, true);

            // Site the package
            Assert.AreEqual(0, ((IVsPackage)package).SetSite(serviceProvider), "SetSite did not return S_OK");

            // Test that toolwindow can be created
            MethodInfo method = typeof(SccProvider).GetMethod("Exec_icmdViewToolWindow", BindingFlags.NonPublic | BindingFlags.Instance);
            object result = method.Invoke(package, new object[] { null, null });

            // Test that toolwindow toolbar's command can be executed
            method = typeof(SccProvider).GetMethod("Exec_icmdToolWindowToolbarCommand", BindingFlags.NonPublic | BindingFlags.Instance);
            result = method.Invoke(package, new object[] { null, null });

            // Toggle the toolwindow color back
            method = typeof(SccProvider).GetMethod("Exec_icmdToolWindowToolbarCommand", BindingFlags.NonPublic | BindingFlags.Instance);
            result = method.Invoke(package, new object[] { null, null });

            // Get the window and test the dispose function
            SccProviderToolWindow window = (SccProviderToolWindow)package.FindToolWindow(typeof(SccProviderToolWindow), 0, true);
            method = typeof(SccProviderToolWindow).GetMethod("Dispose", BindingFlags.NonPublic | BindingFlags.Instance);
            result = method.Invoke(window, new object[] { true });
        }
    }
}
