//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

// Constants specific to the Windows Store version.
namespace SDKSample
{
    namespace Constants
    {
        // On Windows the accelerometer returns values relative to Landscape orientation,
        // even if the device has a native orientation of Portrait.
        // This sample always treats rotation as relative to Landscape regardless of platform.
        static const int UIAngleOffset = 270;
    }
}