/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio;
using Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.UnitTests
{
	static class MockUiShellProvider
	{
		private static GenericMockFactory uiShellFactory = null;
		private static GenericMockFactory enumWindowsFactory0 = null;
		private static GenericMockFactory enumWindowsFactory2 = null;
		private static int windowCount = 0;

		#region UiShell Getters
		/// <summary>
		/// Returns an IVsUiShell that does not implement any methods
		/// </summary>
		/// <returns></returns>
		internal static BaseMock GetUiShellInstance()
		{
			if (uiShellFactory == null)
				uiShellFactory = new GenericMockFactory("UiShell", new Type[] { typeof(IVsUIShell) });
			BaseMock uiShell = uiShellFactory.GetInstance();
			return uiShell;
		}

		/// <summary>
		/// Get an IVsUiShell that implement CreateToolWindow and GetToolWindowEnum.
		/// The enumeration does not contain any window.
		/// </summary>
		/// <returns></returns>
		internal static BaseMock GetWindowEnumerator0()
		{
			BaseMock uiShell = GetUiShellInstance();
			string name = string.Format("{0}.{1}", typeof(IVsUIShell).FullName, "CreateToolWindow");
			uiShell.AddMethodCallback(name, new EventHandler<CallbackArgs>(CreateToolWindowCallBack));
			name = string.Format("{0}.{1}", typeof(IVsUIShell).FullName, "GetToolWindowEnum");
			uiShell.AddMethodCallback(name, new EventHandler<CallbackArgs>(GetToolWindowEnumCallBack0));
			name = string.Format("{0}.{1}", typeof(IVsUIShell).FullName, "FindToolWindow");
			uiShell.AddMethodCallback(name, new EventHandler<CallbackArgs>(FindToolWindowCallBack));
			return uiShell;
		}

		/// <summary>
		/// Get an IVsUiShell that implement CreateToolWindow and GetToolWindowEnum.
		/// The enumeration contains 2 windows.
		/// </summary>
		/// <returns></returns>
		internal static BaseMock GetWindowEnumerator2()
		{
			BaseMock uiShell = GetUiShellInstance();
			string name = string.Format("{0}.{1}", typeof(IVsUIShell).FullName, "CreateToolWindow");
			uiShell.AddMethodCallback(name, new EventHandler<CallbackArgs>(CreateToolWindowCallBack));
			name = string.Format("{0}.{1}", typeof(IVsUIShell).FullName, "GetToolWindowEnum");
			uiShell.AddMethodCallback(name, new EventHandler<CallbackArgs>(GetToolWindowEnumCallBack2));
			return uiShell;
		}

        
		/// <summary>
        /// Get an IVsUiShell that implement ShowMessageBox and returns Cancel from pressing the buttons
		/// </summary>
		/// <returns></returns>
        internal static BaseMock GetShowMessageBoxCancel()
		{
			BaseMock uiShell = GetUiShellInstance();
            string name = string.Format("{0}.{1}", typeof(IVsUIShell).FullName, "ShowMessageBox");
            uiShell.AddMethodCallback(name, new EventHandler<CallbackArgs>(ShowMessageBoxCancel));
			return uiShell;
		}

		#endregion


		#region Callbacks
		private static void CreateToolWindowCallBack(object caller, CallbackArgs arguments)
		{
			arguments.ReturnValue = VSConstants.S_OK;

			// Create the output mock object for the frame
			IVsWindowFrame frame = MockWindowFrameProvider.GetBaseFrame();
			arguments.SetParameter(9, frame);

			// The window pane (if one is provided) needs to be sited
			IVsWindowPane pane = arguments.GetParameter(2) as IVsWindowPane;
			if (pane != null)
			{
				// Create a service provider to site the window pane
				OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
				// It needs to provide STrackSelection
				GenericMockFactory trackSelectionFactory = MockWindowFrameProvider.TrackSelectionFactory;
				serviceProvider.AddService(typeof(STrackSelection), trackSelectionFactory.GetInstance(), false);
				// Add support for output window
				serviceProvider.AddService(typeof(SVsOutputWindow), new OutputWindowService(), false);
				// Finally we need support for FindToolWindow
				serviceProvider.AddService(typeof(SVsUIShell), GetWindowEnumerator0(), false);

				pane.SetSite(serviceProvider);
			}
		}

		private static void FindToolWindowCallBack(object caller, CallbackArgs arguments)
		{
			arguments.ReturnValue = VSConstants.S_OK;

			// Create the output mock object for the frame
			IVsWindowFrame frame = MockWindowFrameProvider.GetBaseFrame();
			arguments.SetParameter(2, frame);
		}

		private static void GetToolWindowEnumCallBack0(object caller, CallbackArgs arguments)
		{
			arguments.ReturnValue = VSConstants.S_OK;

			// Create the output mock object
			if (enumWindowsFactory0 == null)
				enumWindowsFactory0 = new GenericMockFactory("EnumWindows", new Type[] { typeof(IEnumWindowFrames) });
			BaseMock enumWindows = enumWindowsFactory0.GetInstance();
			// Add support for Next
			string name = string.Format("{0}.{1}", typeof(IEnumWindowFrames).FullName, "Next");
			enumWindows.AddMethodCallback(name, new EventHandler<CallbackArgs>(NextCallBack0));

			arguments.SetParameter(0, enumWindows);
		}

		private static void NextCallBack0(object caller, CallbackArgs arguments)
		{
			arguments.ReturnValue = VSConstants.S_FALSE;
			arguments.SetParameter(2, (uint)0);
		}

		private static void GetToolWindowEnumCallBack2(object caller, CallbackArgs arguments)
		{
			arguments.ReturnValue = VSConstants.S_OK;

			// Create the output mock object
			if (enumWindowsFactory2 == null)
				enumWindowsFactory2 = new GenericMockFactory("EnumWindows2", new Type[] { typeof(IEnumWindowFrames) });
			BaseMock enumWindows = enumWindowsFactory2.GetInstance();
			// Add support for Next
			string name = string.Format("{0}.{1}", typeof(IEnumWindowFrames).FullName, "Next");
			enumWindows.AddMethodCallback(name, new EventHandler<CallbackArgs>(NextCallBack2));
			windowCount = 0;

			arguments.SetParameter(0, enumWindows);
		}

		private static void NextCallBack2(object caller, CallbackArgs arguments)
		{
			if (windowCount >= 2)
			{
				// We already enumerated 2 window frames, we are done (0 left to enumerate)
				NextCallBack0(caller, arguments);
				return;
			}

			arguments.ReturnValue = VSConstants.S_OK;
			// Create the list of properties we expect being asked for
			Dictionary<int, object> properties = new Dictionary<int,object>();
			properties.Add((int)__VSFPROPID.VSFPROPID_Caption, "Tool Window " + windowCount.ToString());
			++windowCount;
			properties.Add((int)__VSFPROPID.VSFPROPID_GuidPersistenceSlot, Guid.NewGuid());
			// Create the output mock object for the frame
			object o = arguments.GetParameter(1);
			IVsWindowFrame[] frame = (IVsWindowFrame[])o;
			frame[0] = MockWindowFrameProvider.GetFrameWithProperties(properties);
			// fetched 1 frame
			arguments.SetParameter(2, (uint)1);
		}

		private static void ShowMessageBoxCancel(object caller, CallbackArgs arguments)
        {
            arguments.SetParameter(10, (int)DialogResult.Cancel);
            arguments.ReturnValue = VSConstants.S_OK;
        }

		#endregion
	}
}
