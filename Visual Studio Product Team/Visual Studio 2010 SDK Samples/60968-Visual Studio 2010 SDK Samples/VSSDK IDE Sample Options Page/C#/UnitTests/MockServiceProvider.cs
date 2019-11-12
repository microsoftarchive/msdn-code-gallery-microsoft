/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.Samples.VisualStudio.IDE.OptionsPage.UnitTests
{
    /// <summary>
    /// Implements general mocks and methods of access to them.
    /// </summary>
    static class MockServiceProvider
    {
        #region Fields
        private static GenericMockFactory userSettingsFactory;
        #endregion Fields

        #region Methods (Mock's  getters)
        /// <summary>
        /// Returns an IVSUserSettings that does not implement any methods.
        /// </summary>
        /// <returns></returns>
        internal static BaseMock GetUserSettingsFactoryInstance()
        {
            if (userSettingsFactory == null)
            {
                userSettingsFactory = new GenericMockFactory("MockUserSettings", new Type[] { typeof(IVsUserSettings) });
            }
            BaseMock userSettings = userSettingsFactory.GetInstance();
            return userSettings;
        }
        #endregion Methods (Mock's  getters)
    }
}
