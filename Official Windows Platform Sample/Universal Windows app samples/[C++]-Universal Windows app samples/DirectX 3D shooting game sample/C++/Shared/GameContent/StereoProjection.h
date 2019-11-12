//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

typedef struct STEREO_PARAMETERS
{
    float viewerDistanceInches;
    float displaySizeInches;
    float pixelResolutionWidth;
    float pixelResolutionHeight;
    float stereoSeparationFactor;
    float stereoExaggerationFactor;
} STEREO_PARAMETERS;

// Enumeration for stereo channels (left and right).
typedef enum class STEREO_CHANNEL
{
    LEFT = 0,
    RIGHT
} STEREO_CHANNEL;

// Enumeration for stereo mode (normal or inverted).
typedef enum class STEREO_MODE
{
    NORMAL = 0,
    INVERTED,
} STEREO_MODE;

void StereoCreateDefaultParameters(_Out_ STEREO_PARAMETERS* pStereoParameters);

DirectX::XMMATRIX MatrixStereoProjectionFovLH(
    _In_opt_ const STEREO_PARAMETERS* stereoParameters,
    STEREO_CHANNEL channel,
    float fovAngleY,
    float aspectHByW,
    float nearZ,
    float farZ,
    STEREO_MODE stereoMode
    );

