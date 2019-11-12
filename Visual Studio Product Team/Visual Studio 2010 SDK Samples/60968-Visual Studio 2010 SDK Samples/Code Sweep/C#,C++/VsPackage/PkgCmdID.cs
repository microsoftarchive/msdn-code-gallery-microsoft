/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

// MUST match PkgCmdID.h (for the satellite DLL)
using System;

namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
{
    class PkgCmdIDList
    {
        PkgCmdIDList()
        {}
        public const uint cmdidConfig =        0x100;
        public const uint cmdidStopScan = 0x101;
        public const uint cmdidRepeatLastScan = 0x102;
        public const uint cmdidIgnore = 0x103;
        public const uint cmdidDoNotIgnore = 0x104;
        public const uint cmdidShowIgnoredInstances = 0x105;

    };
}