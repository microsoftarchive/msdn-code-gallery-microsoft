//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include <DirectXMath.h>

namespace TiledResources
{
    class FreeCamera
    {
    public:
        FreeCamera();

        void SetViewParameters(DirectX::XMFLOAT3 eye, DirectX::XMFLOAT3 at, DirectX::XMFLOAT3 up);
        void SetProjectionParameters(float width, float height);

        // +X = right, +Y = up, +Z = out of the screen
        void ApplyTranslation(DirectX::XMFLOAT3 delta);

        // +X = pitch up, +Y = yaw left, +Z = roll counter-clockwise
        void ApplyRotation(DirectX::XMFLOAT3 delta);

        DirectX::XMFLOAT3 GetPosition() const;
        DirectX::XMFLOAT4X4 GetViewMatrix() const;
        DirectX::XMFLOAT4X4 GetProjectionMatrix() const;

    private:
        DirectX::XMFLOAT3 m_position;
        DirectX::XMFLOAT4 m_orientation;
        DirectX::XMFLOAT4X4 m_projectionMatrix;
    };
}
