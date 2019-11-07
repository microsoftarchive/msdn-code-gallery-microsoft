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
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
{
    interface IBackgroundScanner
    {
        event EventHandler Started;
        event EventHandler Stopped;

        bool IsRunning { get; }
        void Start(IEnumerable<IVsProject> projects);
        void RepeatLast();
        void StopIfRunning(bool blockUntilDone);
    }
}
