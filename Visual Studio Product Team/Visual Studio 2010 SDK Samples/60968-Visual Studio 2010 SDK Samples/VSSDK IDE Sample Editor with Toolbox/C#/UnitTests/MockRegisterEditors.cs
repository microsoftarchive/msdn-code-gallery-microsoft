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
using Microsoft.VsSDK.UnitTestLibrary;

namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests
{
    /// <summary>
    /// Provides implementation and Getter methods for the IVsRegisterEditors Mock instance.
    /// </summary>
    static class MockRegisterEditors
    {
        #region Fields
        private static GenericMockFactory registerEditorFactory;
        #endregion Fields

        #region Methods
        /// <summary>
        /// Getter method for the IVsRegisterEditors Mock object.
        /// </summary>
        internal static BaseMock GetRegisterEditorsInstance()
        {
            if (registerEditorFactory == null)
            {
                registerEditorFactory = new GenericMockFactory("SVsRegisterEditors", new Type[] { typeof(IVsRegisterEditors) });
            }
            BaseMock registerEditor = registerEditorFactory.GetInstance();
            return registerEditor;
        }
        #endregion Methods
    }
}
