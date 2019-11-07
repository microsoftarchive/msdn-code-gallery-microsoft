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
using Microsoft.Build.Construction;

namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
{
    internal delegate void EmptyEvent();

    interface IBuildManager
    {
        event EmptyEvent BuildStarted;
        event EmptyEvent BuildStopped;

        bool IsListeningToBuildEvents { get; set; }
        ProjectTaskElement GetBuildTask(IVsProject project, bool createIfNecessary);
        ICollection<string> AllItemsInProject(IVsProject project);
        void SetProperty(IVsProject project, string name, string value);
        string GetProperty(IVsProject project, string name);
        void CreatePerUserFilesAsNecessary();
    }
}
