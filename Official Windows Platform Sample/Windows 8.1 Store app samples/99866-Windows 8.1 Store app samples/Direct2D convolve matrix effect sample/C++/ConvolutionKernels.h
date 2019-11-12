//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

typedef enum class KernelSelection
{
    Passthrough,
    BoxBlur,
    SimpleEdgeDetect,
    SimpleSharpen,
    Emboss,
    HorizontalSmear
} KernelSelection;

namespace ConvolutionKernels
{
    static const unsigned int PassthroughWidth      = 1;
    static const unsigned int BoxBlurWidth          = 5;
    static const unsigned int SimpleEdgeDetectWidth = 3;
    static const unsigned int SimpleSharpenWidth    = 3;
    static const unsigned int EmbossWidth           = 3;
    static const unsigned int HorizontalSmearWidth  = 20;
    static const unsigned int HorizontalSmearHeight = 1;

    static const float BoxBlurDivisor               = 25.0f;
    static const float SimpleSharpenDivisor         = 2.0f;
    static const float HorizontalSmearDivisor       = 20.0f;
    static const float DefaultDivisor               = 1.0f;

    static Platform::String^ PassthroughTitle       = "Passthrough";
    static Platform::String^ BoxBlurTitle           = "Box Blur (width 5)";
    static Platform::String^ SimpleEdgeDetectTitle  = "Simple Edge Detect";
    static Platform::String^ SimpleSharpenTitle     = "Simple Sharpen";
    static Platform::String^ EmbossTitle            = "Emboss";
    static Platform::String^ HorizontalSmearTitle   = "Horizontal Smear (width 20)";

    static const float Passthrough[] =
    {
        1.0f
    };

    static const float BoxBlur[] =
    {
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f,
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f,
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f,
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f,
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f
    };

    static const float SimpleEdgeDetect[] =
    {
         0.0f, -1.0f,  0.0f,
        -1.0f,  4.0f, -1.0f,
         0.0f, -1.0f,  0.0f
    };

    static const float SimpleSharpen[] =
    {
         0.0f, -1.0f,  0.0f,
        -1.0f,  6.0f, -1.0f,
         0.0f, -1.0f,  0.0f
    };

    static const float Emboss[] =
    {
        2.0f,  1.0f,  0.0f,
        1.0f,  1.0f, -1.0f,
        0.0f, -1.0f, -2.0f
    };

    static const float HorizontalSmear[] =
    {
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f,
        1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f
    };
}
