//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved
////----------------------------------------------------------------------

#include "pch.h"
#include "Camera.h"

void Camera::GetViewMatrix(_Out_ float4x4 *viewMatrix)
{
    *viewMatrix = m_view;
}

void Camera::GetProjectionMatrix(_Out_ float4x4 *projectionMatrix)
{
    *projectionMatrix = m_projection;
}

// this method updates the view matrix based on new position and focus coordinates
void Camera::SetViewParameters(
    _In_ float3 eyePosition,    // the position of the camera
    _In_ float3 lookPosition,   // the point the camera should look at
    _In_ float3 up              // the durection vector for up
    )
{
    m_position = eyePosition;
    m_direction = normalize(lookPosition - eyePosition);
    float3 zAxis = m_direction;
    float3 xAxis = normalize(cross(up, zAxis));
    float3 yAxis = cross(zAxis, xAxis);
    float xOffset = -dot(xAxis, m_position);
    float yOffset = -dot(yAxis, m_position);
    float zOffset = -dot(zAxis, m_position);
    m_view = float4x4(
        xAxis.x, xAxis.y, xAxis.z, xOffset,
        yAxis.x, yAxis.y, yAxis.z, yOffset,
        zAxis.x, zAxis.y, zAxis.z, zOffset,
        0.0f,    0.0f,    0.0f,    1.0f
        );
}
    // this method updates the projection matrix based on new parameters
void Camera::SetProjectionParameters(
    _In_ float minimumFieldOfView,  // the minimum horizontal or vertical field of view, in degrees
    _In_ float aspectRatio,         // the aspect ratio of the projection (width / height)
    _In_ float nearPlane,           // depth to map to 0
    _In_ float farPlane             // depth to map to 1
    )
{
    float minScale = 1.0f / tan(minimumFieldOfView * PI_F / 360.0f);
    float xScale = 1.0f;
    float yScale = 1.0f;
    if (aspectRatio < 1.0f)
    {
        xScale = minScale;
        yScale = minScale * aspectRatio;
    }
    else
    {
        xScale = minScale / aspectRatio;
        yScale = minScale;
    }
    float zScale = farPlane / (farPlane - nearPlane);
    m_projection = float4x4(
        xScale, 0.0f,   0.0f,    0.0f,
        0.0f,   yScale, 0.0f,    0.0f,
        0.0f,   0.0f,   zScale, -nearPlane*zScale,
        0.0f,   0.0f,   1.0f,    0.0f
        );
}
