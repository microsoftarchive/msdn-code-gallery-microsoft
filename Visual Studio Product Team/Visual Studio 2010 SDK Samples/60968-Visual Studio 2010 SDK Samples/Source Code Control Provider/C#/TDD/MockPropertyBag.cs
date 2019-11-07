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
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;
using Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
{
	static class MockPropertyBagProvider
	{
		private static GenericMockFactory PBFactory = null;
        // The names of the properties stored by the provider in the solution file
        private const string strSolutionControlledProperty = "SolutionIsControlled";
        private const string strSolutionBindingsProperty = "SolutionBindings";

		#region PB Getters
		/// <summary>
		/// Returns a property bag that does not implement any methods
		/// </summary>
		/// <returns></returns>
		internal static BaseMock GetPBInstance()
		{
			if (PBFactory == null)
                PBFactory = new GenericMockFactory("PropertyBag", new Type[] { typeof(IPropertyBag) });
            BaseMock pb = PBFactory.GetInstance();
			return pb;
		}

		/// <summary>
		/// Get a property bag that implements Read method
		/// </summary>
		/// <returns></returns>
		internal static BaseMock GetReadPropertyBag()
		{
            BaseMock pb = GetPBInstance();
            string name = string.Format("{0}.{1}", typeof(IPropertyBag).FullName, "Read");
            pb.AddMethodCallback(name, new EventHandler<CallbackArgs>(ReadCallback));
            return pb;
		}

		/// <summary>
        /// Get a property bag that implements Write method
        /// </summary>
		/// <returns></returns>
        internal static BaseMock GetWritePropertyBag()
		{
            BaseMock pb = GetPBInstance();
			string name = string.Format("{0}.{1}", typeof(IPropertyBag).FullName, "Write");
			pb.AddMethodCallback(name, new EventHandler<CallbackArgs>(WriteCallback));
			return pb;
		}

		#endregion

		#region Callbacks

        private static void WriteCallback(object caller, CallbackArgs arguments)
        {
            arguments.ReturnValue = VSConstants.E_NOTIMPL;
        }

        private static void ReadCallback(object caller, CallbackArgs arguments)
        {
            string propertyName = (string)arguments.GetParameter(0);
            if (propertyName == strSolutionControlledProperty)
            {
                arguments.SetParameter(1, true);
                arguments.ReturnValue = VSConstants.S_OK;
                return;
            }
            else if (propertyName == strSolutionBindingsProperty)
            {
                arguments.SetParameter(1, "Solution's location");
                arguments.ReturnValue = VSConstants.S_OK;
                return;
            }

            arguments.ReturnValue = VSConstants.E_NOTIMPL;
        }
        
        #endregion
	}
}
