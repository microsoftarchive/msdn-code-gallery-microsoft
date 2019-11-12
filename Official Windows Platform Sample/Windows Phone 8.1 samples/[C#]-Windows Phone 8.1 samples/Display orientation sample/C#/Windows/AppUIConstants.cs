//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

namespace SDKTemplate
{
    /// <summary>
    /// Constants specific to the Windows Store version.
    /// </summary>
    static class Constants
    {
        // On Windows the accelerometer returns values relative to Landscape orientation,
        // even if the device has a native orientation of Portrait.
        // This sample always treats rotation as relative to Landscape regardless of platform.
        public const int UIAngleOffset = 270;
    }
}