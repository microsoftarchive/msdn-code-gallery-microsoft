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
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
{
    class MockRegisterScciProvider
    {
		private static GenericMockFactory registerScciProviderFactory = null;

        #region RegisterScciProvider Getters

        /// <summary>
        /// Return a IVsRegisterScciProvider without any special implementation
		/// </summary>
		/// <returns></returns>
        internal static IVsRegisterScciProvider GetBaseRegisterScciProvider()
		{
            if (registerScciProviderFactory == null)
                registerScciProviderFactory = new GenericMockFactory("RegisterScciProvider", new Type[] { typeof(IVsRegisterScciProvider) });
            IVsRegisterScciProvider registerProvider = (IVsRegisterScciProvider)registerScciProviderFactory.GetInstance();
            return registerProvider;
		}

		#endregion

		#region Callbacks
		#endregion
	}
}
