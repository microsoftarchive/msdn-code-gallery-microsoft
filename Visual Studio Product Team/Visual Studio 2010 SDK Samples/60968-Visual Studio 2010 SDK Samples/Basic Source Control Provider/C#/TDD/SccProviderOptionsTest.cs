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
using System.Reflection;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.UnitTests
{
    /// <summary>
    ///This is a test class for Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.SccProviderOptions and is intended
    ///to contain all Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.SccProviderOptions Unit Tests
    ///</summary>
    [TestClass()]
    public class SccProviderOptionsTest
    {
        /// <summary>
        ///A test for OnActivate (CancelEventArgs)
        ///</summary>
        [TestMethod()]
        public void OnActivateTest()
        {
            SccProviderOptions target = new SccProviderOptions();

            MethodInfo method = typeof(SccProviderOptions).GetMethod("OnActivate", BindingFlags.NonPublic | BindingFlags.Instance);
            CancelEventArgs e = new CancelEventArgs();
            method.Invoke(target, new object[] { e });
        }

        /// <summary>
        ///A test for OnApply (PageApplyEventArgs)
        ///</summary>
        [TestMethod()]
        public void OnApplyTest()
        {
            SccProviderOptions target = new SccProviderOptions();

            // Create a basic service provider
            using (OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices())
            {
                // Mock the UIShell service to answer Cancel to the dialog invocation
                BaseMock mockUIShell = MockUiShellProvider.GetShowMessageBoxCancel();
                serviceProvider.AddService(typeof(IVsUIShell), mockUIShell, true);

                // Create an ISite wrapper over the service provider
                SiteWrappedServiceProvider wrappedProvider = new SiteWrappedServiceProvider(serviceProvider);
                target.Site = wrappedProvider;

                Assembly shell = typeof(Microsoft.VisualStudio.Shell.DialogPage).Assembly;
                Type argtype = shell.GetType("Microsoft.VisualStudio.Shell.DialogPage+PageApplyEventArgs", true);

                MethodInfo method = typeof(SccProviderOptions).GetMethod("OnApply", BindingFlags.NonPublic | BindingFlags.Instance);
                object eventargs = shell.CreateInstance(argtype.FullName);

                method.Invoke(target, new object[] { eventargs });
            }
        }

        /// <summary>
        ///A test for OnClosed (EventArgs)
        ///</summary>
        [TestMethod()]
        public void OnClosedTest()
        {
            SccProviderOptions target = new SccProviderOptions();

            MethodInfo method = typeof(SccProviderOptions).GetMethod("OnClosed", BindingFlags.NonPublic | BindingFlags.Instance);
            EventArgs e = new EventArgs();
            method.Invoke(target, new object[] { e });
        }

        /// <summary>
        ///A test for OnDeactivate (CancelEventArgs)
        ///</summary>
        [TestMethod()]
        public void OnDeactivateTest()
        {
            SccProviderOptions target = new SccProviderOptions();

            MethodInfo method = typeof(SccProviderOptions).GetMethod("OnDeactivate", BindingFlags.NonPublic | BindingFlags.Instance);
            CancelEventArgs e = new CancelEventArgs();
            method.Invoke(target, new object[] { e });
        }

        /// <summary>
        ///A test for Window
        ///</summary>
        [TestMethod()]
        public void WindowTest()
        {
            SccProviderOptions target = new SccProviderOptions();

            PropertyInfo property = typeof(SccProviderOptions).GetProperty("Window", BindingFlags.NonPublic | BindingFlags.Instance);
            IWin32Window val = property.GetValue(target, null) as IWin32Window;
            Assert.IsNotNull(val, "The property page control was not created");
        }
    }
}
