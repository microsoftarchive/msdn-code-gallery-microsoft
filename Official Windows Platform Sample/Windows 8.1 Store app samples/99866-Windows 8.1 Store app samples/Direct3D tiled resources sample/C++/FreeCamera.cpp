//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "FreeCamera.h"

using namespace TiledResources;
using namespace DirectX;

FreeCamera::FreeCamera()
{
}

void FreeCamera::SetViewParameters(XMFLOAT3 eye, XMFLOAT3 at, XMFLOAT3 up)
{
    m_position = eye;
    XMVECTOR orientation = XMQuaternionRotationMatrix(
        XMMatrixLookAtRH(
            XMLoadFloat3(&eye),
            XMLoadFloat3(&at),
            XMLoadFloat3(&up)
            )
        );
    XMStoreFloat4(&m_orientation, orientation);
}

void FreeCamera::SetProjectionParameters(float width, float height)
{
    XMMATRIX projectionMatrix = XMMatrixPerspectiveFovRH(70.0f * XM_PI / 180.0f, width / height, 1.0f / 256.0f, 256.0f);
    XMStoreFloat4x4(&m_projectionMatrix, projectionMatrix);
}

void FreeCamera::ApplyTranslation(XMFLOAT3 delta)
{
    XMFLOAT4 deltaVector(delta.x, delta.y, delta.z, 1);
    XMVECTOR deltaCameraSpace = XMVector4Transform(XMLoadFloat4(&deltaVector), XMMatrixRotationQuaternion(XMQuaternionInverse(XMLoadFloat4(&m_orientation))));
    XMStoreFloat3(&m_position, XMVectorAdd(XMLoadFloat3(&m_position), deltaCameraSpace));
}

void FreeCamera::ApplyRotation(XMFLOAT3 delta)
{
    XMStoreFloat4(&m_orientation, XMQuaternionMultiply(XMLoadFloat4(&m_orientation), XMQuaternionRotationRollPitchYawFromVector(XMVectorNegate(XMLoadFloat3(&delta)))));
}

XMFLOAT3 FreeCamera::GetPosition() const
{
    return m_position;
}

XMFLOAT4X4 FreeCamera::GetViewMatrix() const
{
    XMFLOAT4X4 ret;
    XMMATRIX viewMatrix = XMMatrixMultiply(
            XMMatrixTranslationFromVector(XMVectorNegate(XMLoadFloat3(&m_position))),
            XMMatrixRotationQuaternion(XMLoadFloat4(&m_orientation))
            );
    XMStoreFloat4x4(&ret, viewMatrix);
    return ret;
}

XMFLOAT4X4 FreeCamera::GetProjectionMatrix() const
{
    return m_projectionMatrix;
}
