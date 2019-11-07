//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved
////----------------------------------------------------------------------

#pragma once

#include "BasicMath.h"

// a simple camera class
ref class Camera
{
private:
    float3 m_position;      // the position of the camera
    float3 m_direction;     // the unit vector of the viewing direction
    float4x4 m_view;        // view matrix
    float4x4 m_projection;  // projection matrix

internal:
    void GetViewMatrix(_Out_ float4x4 *viewMatrix);
    void GetProjectionMatrix(_Out_ float4x4 *projectionMatrix);

    // this method updates the view matrix based on new position and focus coordinates
    void SetViewParameters(
        _In_ float3 eyePosition,    // the position of the camera
        _In_ float3 lookPosition,   // the point the camera should look at
        _In_ float3 up              // the durection vector for up
        );

    // this method updates the projection matrix based on new parameters
    void SetProjectionParameters(
        _In_ float minimumFieldOfView,  // the minimum horizontal or vertical field of view, in degrees
        _In_ float aspectRatio,         // the aspect ratio of the projection (width / height)
        _In_ float nearPlane,           // depth to map to 0
        _In_ float farPlane             // depth to map to 1
        );
};
