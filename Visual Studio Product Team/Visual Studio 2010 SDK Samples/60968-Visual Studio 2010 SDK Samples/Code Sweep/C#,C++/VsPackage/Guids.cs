/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

// MUST match guid.h (for the satellite DLL)
using System;

namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
{
    static class GuidList
    {
        public static readonly Guid guidVSPackagePkg = new Guid("{2b621c1e-60a3-48c5-a07d-0ad6d3dd3417}");
        public static readonly Guid guidVSPackageCmdSet =        new Guid("{d0882566-3d01-4578-b4f2-0aff36119700}");
    };
}