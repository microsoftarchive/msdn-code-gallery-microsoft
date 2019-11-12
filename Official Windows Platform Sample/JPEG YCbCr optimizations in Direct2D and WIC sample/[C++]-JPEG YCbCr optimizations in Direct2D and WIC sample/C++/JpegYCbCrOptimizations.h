//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include <pch.h>

// Constants and sample-specific data.
namespace JpegYCbCrOptimizations
{
    typedef enum YCbCrSupportMode
    {
        Unknown,          // YCbCr support has not yet been determined.
        Enabled,          // YCbCr is supported and enabled.
        DisabledFallback, // YCbCr is not supported, using BGRA fallback.
        DisabledForced    // YCbCr was disabled by user.
    };

    typedef enum BackgroundTaskMode
    {
        Running,
        Cancelling,
        Completed
    };

    delegate void ResourcesLoadedHandler(YCbCrSupportMode mode);

    namespace SampleConstants
    {
        static const uint32 NumPlanes = 2;

        // Actual data is contained in JpegYCbCrOptimizations.cpp.
        extern const WICPixelFormatGUID WicYCbCrFormats[NumPlanes];
        extern Platform::String^ SampleModeString_DisabledFallback;
        extern Platform::String^ SampleModeString_DisabledForced;
        extern Platform::String^ SampleModeString_Enabled;
        extern Platform::String^ TitleString;
    }
}