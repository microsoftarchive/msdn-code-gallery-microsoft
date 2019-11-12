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
using Microsoft.Samples.VisualStudio.CodeSweep.VSPackage;
using Microsoft.Samples.VisualStudio.CodeSweep.Scanner;
using Microsoft.Samples.VisualStudio.CodeSweep.BuildTask;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    class MockHostObject : IScannerHost
    {
        public class AddResultArgs : EventArgs
        {
            public readonly IScanResult Result;
            public readonly string ProjectFile;
            public AddResultArgs(IScanResult result, string projectFile) { Result = result; ProjectFile = projectFile; }
        }
        public event EventHandler<AddResultArgs> OnAddResult;

        #region IScannerHost Members

        public void AddResult(IScanResult result, string projectFile)
        {
            if (OnAddResult != null)
            {
                OnAddResult(this, new AddResultArgs(result, projectFile));
            }
        }

        public string GetTextOfFileIfOpenInIde(string filePath)
        {
            return null;
        }

        #endregion
    }
}
