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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Samples.VisualStudio.CodeSweep.Scanner;

namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
{
    interface ITaskProvider : IVsTaskProvider, IVsTaskProvider3
    {
        void AddResult(IScanResult result, string projectFile);
        void Clear();
        void SetAsActiveProvider();
        void ShowTaskList();
        bool IsShowingIgnoredInstances { get; }
    }
}
