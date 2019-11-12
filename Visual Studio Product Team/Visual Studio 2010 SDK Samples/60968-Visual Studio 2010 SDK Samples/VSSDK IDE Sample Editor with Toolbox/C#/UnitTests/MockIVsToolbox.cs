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
using Microsoft.VisualStudio;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests
{
    /// <summary>
    /// Provides implementation and Getter methods for the IVsToolbox Mock instance.
    /// </summary>
    internal static class MockIVsToolbox
    {
        #region Methods
        /// <summary>
        /// Getter method for the IVsToolbox Mock object.
        /// </summary>
        /// <returns></returns>
        internal static BaseMock GetIVsToolboxInstance()
        {
            GenericMockFactory factory = new GenericMockFactory("IVsToolbox", new Type[] { typeof(IVsToolbox) });
            BaseMock mockObj = factory.GetInstance();
            return mockObj;
        }
        #endregion Methods
    }
}
