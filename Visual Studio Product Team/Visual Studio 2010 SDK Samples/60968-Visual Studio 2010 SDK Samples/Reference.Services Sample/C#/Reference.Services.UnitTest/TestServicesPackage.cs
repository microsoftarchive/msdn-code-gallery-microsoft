/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.Shell.Interop;

using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

using Microsoft.Samples.VisualStudio.Services;
using Microsoft.Samples.VisualStudio.Services.Interfaces;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Reference.Services.UnitTest
{
	[TestClass()]
	public class TestServicesPackage
	{
		[TestMethod()]
		public void SetSiteSimple()
		{
			ServicesPackage packageObject = new ServicesPackage();
			IVsPackage package = (IVsPackage)packageObject;
			using (OleServiceProvider provider = OleServiceProvider.CreateOleServiceProviderWithBasicServices())
			{
				int result = package.SetSite(provider);
                Assert.IsTrue(Microsoft.VisualStudio.ErrorHandler.Succeeded(result), "SetSite failed.");
			}
			package.SetSite(null);
			package.Close();
		}

		[TestMethod()]
		public void GetGlobalServiceSimple()
		{
			ServicesPackage packageObject = new ServicesPackage();
			IVsPackage package = (IVsPackage)packageObject;
			using (OleServiceProvider provider = OleServiceProvider.CreateOleServiceProviderWithBasicServices())
			{
				int result = package.SetSite(provider);
                Assert.IsTrue(Microsoft.VisualStudio.ErrorHandler.Succeeded(result), "SetSite failed.");
				IServiceProvider serviceProvider = package as IServiceProvider;
				object o = serviceProvider.GetService(typeof(SMyGlobalService));
				Assert.IsNotNull(o, "GetService returned null for the global service.");
				IMyGlobalService service = o as IMyGlobalService;
				Assert.IsNotNull(service, "The service SMyGlobalService does not implements IMyGlobalService.");
				service.GlobalServiceFunction();
			}
			package.SetSite(null);
			package.Close();
		}

		[TestMethod()]
		public void GetGlobalServiceFromNativeProvider()
		{
			ServicesPackage packageObject = new ServicesPackage();
			IVsPackage package = (IVsPackage)packageObject;
			using (OleServiceProvider provider = OleServiceProvider.CreateOleServiceProviderWithBasicServices())
			{
				int result = package.SetSite(provider);
                Assert.IsTrue(Microsoft.VisualStudio.ErrorHandler.Succeeded(result), "SetSite failed.");
				IOleServiceProvider sp = package as IOleServiceProvider;
                Assert.IsNotNull(sp, "The pacckage does not implements the native IServiceProvider");
				Guid guidService = typeof(SMyGlobalService).GUID;
				Guid guidInterface = typeof(IMyGlobalService).GUID;
				IntPtr ppvObj = IntPtr.Zero;
				int hr = sp.QueryService(ref guidService, ref guidInterface, out ppvObj);
                Assert.IsTrue(Microsoft.VisualStudio.ErrorHandler.Succeeded(hr), "QueryService failed for the global service.");
				Assert.IsTrue(ppvObj != IntPtr.Zero, "QueryService returned a NULL pointer for the global service.");
				Marshal.Release(ppvObj);
			}
			package.SetSite(null);
			package.Close();
		}

		[TestMethod()]
		public void GetLocalServiceSimple()
		{
			ServicesPackage packageObject = new ServicesPackage();
			IVsPackage package = (IVsPackage)packageObject;
			using (OleServiceProvider provider = OleServiceProvider.CreateOleServiceProviderWithBasicServices())
			{
				int result = package.SetSite(provider);
                Assert.IsTrue(Microsoft.VisualStudio.ErrorHandler.Succeeded(result), "SetSite failed.");
				IServiceProvider serviceProvider = package as IServiceProvider;
				object o = serviceProvider.GetService(typeof(SMyLocalService));
                Assert.IsNotNull(o, "GetService returned null for the local service.");
				IMyLocalService service = o as IMyLocalService;
                Assert.IsNotNull(service, "The service SMyLocalService does not implements IMyLocalService.");
				service.LocalServiceFunction();
			}
			package.SetSite(null);
			package.Close();
		}

		[TestMethod()]
		public void GetLocalServiceFromNativeProvider()
		{
			ServicesPackage packageObject = new ServicesPackage();
			IVsPackage package = (IVsPackage)packageObject;
			using (OleServiceProvider provider = OleServiceProvider.CreateOleServiceProviderWithBasicServices())
			{
				int result = package.SetSite(provider);
                Assert.IsTrue(Microsoft.VisualStudio.ErrorHandler.Succeeded(result), "SetSite failed.");
				IOleServiceProvider sp = package as IOleServiceProvider;
                Assert.IsNotNull(sp, "The package does not implements the native IServiceProvider");
				Guid guidService = typeof(SMyLocalService).GUID;
				Guid guidInterface = typeof(IMyLocalService).GUID;
				IntPtr ppvObj = IntPtr.Zero;
				int hr = sp.QueryService(ref guidService, ref guidInterface, out ppvObj);
                Assert.IsTrue(Microsoft.VisualStudio.ErrorHandler.Succeeded(hr), "QueryService failed for the local service.");
                Assert.IsTrue(ppvObj != IntPtr.Zero, "QueryService returned a NULL pointer for the local service.");
				Marshal.Release(ppvObj);
			}
			package.SetSite(null);
			package.Close();
		}
	}
}
