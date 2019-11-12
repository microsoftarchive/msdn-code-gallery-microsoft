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
    /// Constants specific to the Windows Phone version.
    /// </summary>
    static class Constants
    {
        // On Phone the accelerometer returns values relative to Portrait orientation.
        // This sample always treats rotation as relative to Landscape regardless of platform.
        public const int UIAngleOffset = 180;
    }
}