/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Samples.VisualStudio.Services;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Reference.Services.UnitTest
{
	[TestClass()]
	public class TestMyGlobalService
	{
		private GenericMockFactory mockOutputWindowFactory;

		/*
		 * We can not run this test against a debug version of the platform because it will
		 * assert and this will let the test fail.
		[Test()]
		public void TestNoInitialize()
		{
			// Check that the service can handle the fact that the package is not initialize.
			MyGlobalService service = new MyGlobalService();
			service.GlobalServiceFunction();
		}
		*/

		[TestMethod()]
		public void TestOutputNoPane()
		{
			// Create an instance of the package and initialize it so that the GetService
			// will succeed, but the GetPane will fail.

			// As first create a service provider.
			using (OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices())
			{
				// Now create the mock object for the output window.
				if (null == mockOutputWindowFactory)
				{
					mockOutputWindowFactory = new GenericMockFactory("MockOutputWindow", new Type[] { typeof(IVsOutputWindow) });
				}
				BaseMock mockBase = mockOutputWindowFactory.GetInstance() as BaseMock;
				mockBase.AddMethodReturnValues(string.Format("{0}.{1}", typeof(IVsOutputWindow).FullName, "GetPane"),
											   new object[] { -1, Guid.Empty, null });
				// Add the output window to the services provided by the service provider.
				serviceProvider.AddService(typeof(SVsOutputWindow), mockBase, false);

				// Create an instance of the package and initialize it calling SetSite.
				ServicesPackage package = new ServicesPackage();
				int result = ((IVsPackage)package).SetSite(serviceProvider);
				Assert.IsTrue(Microsoft.VisualStudio.ErrorHandler.Succeeded(result), "SetSite failed.");

				// Now we can create an instance of the service
				MyGlobalService service = new MyGlobalService(package);
				service.GlobalServiceFunction();
				((IVsPackage)package).SetSite(null);
				((IVsPackage)package).Close();
			}
		}

		private bool callbackExecuted;
		private void OutputWindowPaneCallback(object sender, CallbackArgs args)
		{
			callbackExecuted = true;
			string expectedText = " ======================================\n" +
			                      "\tGlobalServiceFunction called.\n" +
			                      " ======================================\n";
			string inputText = (string)args.GetParameter(0);
			Assert.AreEqual(expectedText, inputText, "OutputString called with wrong text.");
			args.ReturnValue = 0;
		}
		[TestMethod()]
		public void TestOutput()
		{
			callbackExecuted = false;
			// As first create a service provider.
			using (OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices())
			{
				// Create a mock object for the output window pane.
				GenericMockFactory mockWindowPaneFactory = new GenericMockFactory("MockOutputWindowPane", new Type[] { typeof(IVsOutputWindowPane) });
				BaseMock mockWindowPane = mockWindowPaneFactory.GetInstance();
				mockWindowPane.AddMethodCallback(string.Format("{0}.{1}", typeof(IVsOutputWindowPane).FullName, "OutputString"),
												 new EventHandler<CallbackArgs>(OutputWindowPaneCallback));

				// Now create the mock object for the output window.
				if (null == mockOutputWindowFactory)
				{
					mockOutputWindowFactory = new GenericMockFactory("MockOutputWindow1", new Type[] { typeof(IVsOutputWindow) });
				}
				BaseMock mockOutputWindow = mockOutputWindowFactory.GetInstance();
				mockOutputWindow.AddMethodReturnValues(
						string.Format("{0}.{1}", typeof(IVsOutputWindow).FullName, "GetPane"),
						new object[] { 0, Guid.Empty, (IVsOutputWindowPane)mockWindowPane });

				// Add the output window to the services provided by the service provider.
				serviceProvider.AddService(typeof(SVsOutputWindow), mockOutputWindow, false);

				// Create an instance of the package and initialize it calling SetSite.
				ServicesPackage package = new ServicesPackage();
				int result = ((IVsPackage)package).SetSite(serviceProvider);
				Assert.IsTrue(Microsoft.VisualStudio.ErrorHandler.Succeeded(result), "SetSite failed.");

				// Now we can create an instance of the service
				MyGlobalService service = new MyGlobalService(package);
				service.GlobalServiceFunction();
				Assert.IsTrue(callbackExecuted, "OutputText not called.");
				((IVsPackage)package).SetSite(null);
				((IVsPackage)package).Close();
			}
		}
	}
}
