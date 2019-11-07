//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved
////----------------------------------------------------------------------

#pragma once

using namespace DirectX;

// a simple camera class
class Camera
{
private:
    XMFLOAT3 m_position;      // the position of the camera
    XMFLOAT3 m_direction;     // the unit vector of the viewing direction
    XMFLOAT4X4 m_view;        // view matrix
    XMFLOAT4X4 m_projection;  // projection matrix

public:
    void GetViewMatrix(_Out_ XMFLOAT4X4 *viewMatrix);
    void GetProjectionMatrix(_Out_ XMFLOAT4X4 *projectionMatrix);

    // this method updates the view matrix based on new position and focus coordinates
    void SetViewParameters(
        _In_ XMFLOAT3 eyePosition,    // the position of the camera
        _In_ XMFLOAT3 lookPosition,   // the point the camera should look at
        _In_ XMFLOAT3 up              // the durection vector for up
        );

    // this method updates the projection matrix based on new parameters
    void SetProjectionParameters(
        _In_ float minimumFieldOfView,  // the minimum horizontal or vertical field of view, in degrees
        _In_ float aspectRatio,         // the aspect ratio of the projection (width / height)
        _In_ float nearPlane,           // depth to map to 0
        _In_ float farPlane             // depth to map to 1
        );
};
