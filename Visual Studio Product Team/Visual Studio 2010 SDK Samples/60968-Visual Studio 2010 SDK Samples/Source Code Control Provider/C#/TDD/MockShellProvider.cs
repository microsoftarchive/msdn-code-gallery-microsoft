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
using Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
{
	static class MockShellProvider
	{
		private static GenericMockFactory ShellFactory = null;

		#region Shell Getters
		/// <summary>
		/// Returns an IVsShell that does not implement any methods
		/// </summary>
		/// <returns></returns>
		internal static BaseMock GetShellInstance()
		{
			if (ShellFactory == null)
				ShellFactory = new GenericMockFactory("Shell", new Type[] { typeof(IVsShell) });
			BaseMock Shell = ShellFactory.GetInstance();
			return Shell;
		}

		/// <summary>
		/// Get an IVsShell that implement GetProperty and returns command line mode.
		/// </summary>
		/// <returns></returns>
		internal static BaseMock GetShellForCommandLine()
		{
			BaseMock Shell = GetShellInstance();
			string name = string.Format("{0}.{1}", typeof(IVsShell).FullName, "GetProperty");
			Shell.AddMethodCallback(name, new EventHandler<CallbackArgs>(GetPropertyCallBack1));
			return Shell;
		}

		/// <summary>
		/// Get an IVsShell that implement GetProperty
		/// </summary>
		/// <returns></returns>
		internal static BaseMock GetShellForUI()
		{
			BaseMock Shell = GetShellInstance();
			string name = string.Format("{0}.{1}", typeof(IVsShell).FullName, "GetProperty");
			Shell.AddMethodCallback(name, new EventHandler<CallbackArgs>(GetPropertyCallBack2));
			return Shell;
		}

		#endregion

		#region Callbacks

		private static void GetPropertyCallBack1(object caller, CallbackArgs arguments)
        {
            if ((int)arguments.GetParameter(0) == (int)__VSSPROPID.VSSPROPID_IsInCommandLineMode)
            {
                arguments.SetParameter(1, true);
                arguments.ReturnValue = VSConstants.S_OK;
                return;
            }

            arguments.ReturnValue = VSConstants.E_NOTIMPL;
        }

        private static void GetPropertyCallBack2(object caller, CallbackArgs arguments)
        {
            if ((int)arguments.GetParameter(0) == (int)__VSSPROPID.VSSPROPID_IsInCommandLineMode)
            {
                arguments.SetParameter(1, false);
                arguments.ReturnValue = VSConstants.S_OK;
                return;
            }

            arguments.ReturnValue = VSConstants.E_NOTIMPL;
        }
        
        #endregion
	}
}
