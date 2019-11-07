//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "pch.h"
#include "BasicShapes.h"
#include "DirectXSample.h"
#include <memory>

BasicShapes::BasicShapes(ID3D11Device *d3dDevice) : m_d3dDevice(d3dDevice)
{
}

void BasicShapes::CreateVertexBuffer(
    _In_ unsigned int numVertices,
    _In_ BasicVertex *vertexData,
    _Out_ ID3D11Buffer **vertexBuffer
    )
{
    *vertexBuffer = nullptr;
    Microsoft::WRL::ComPtr<ID3D11Buffer> vertexBufferInternal;

    D3D11_BUFFER_DESC VertexBufferDesc;
    VertexBufferDesc.ByteWidth = sizeof(BasicVertex) * numVertices;
    VertexBufferDesc.Usage = D3D11_USAGE_DEFAULT;
    VertexBufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    VertexBufferDesc.CPUAccessFlags = 0;
    VertexBufferDesc.MiscFlags = 0;
    VertexBufferDesc.StructureByteStride = 0;

    D3D11_SUBRESOURCE_DATA VertexBufferData;
    VertexBufferData.pSysMem = vertexData;
    VertexBufferData.SysMemPitch = 0;
    VertexBufferData.SysMemSlicePitch = 0;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &VertexBufferDesc,
            &VertexBufferData,
            &vertexBufferInternal
            )
        );


    *vertexBuffer = vertexBufferInternal.Detach();
}

void BasicShapes::CreateIndexBuffer(
    _In_ unsigned int numIndices,
    _In_ unsigned short *indexData,
    _Out_ ID3D11Buffer **indexBuffer
    )
{
    *indexBuffer = nullptr;
    Microsoft::WRL::ComPtr<ID3D11Buffer> indexBufferInternal;

    D3D11_BUFFER_DESC IndexBufferDesc;
    IndexBufferDesc.ByteWidth = sizeof(unsigned short) * numIndices;
    IndexBufferDesc.Usage = D3D11_USAGE_DEFAULT;
    IndexBufferDesc.BindFlags = D3D11_BIND_INDEX_BUFFER;
    IndexBufferDesc.CPUAccessFlags = 0;
    IndexBufferDesc.MiscFlags = 0;
    IndexBufferDesc.StructureByteStride = 0;

    D3D11_SUBRESOURCE_DATA IndexBufferData;
    IndexBufferData.pSysMem = indexData;
    IndexBufferData.SysMemPitch = 0;
    IndexBufferData.SysMemSlicePitch = 0;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &IndexBufferDesc,
            &IndexBufferData,
            &indexBufferInternal
            )
        );

    *indexBuffer = indexBufferInternal.Detach();
}

void BasicShapes::CreateTangentVertexBuffer(
    _In_ unsigned int numVertices,
    _In_ TangentVertex *vertexData,
    _Out_ ID3D11Buffer **vertexBuffer
    )
{
    *vertexBuffer = nullptr;
    Microsoft::WRL::ComPtr<ID3D11Buffer> vertexBufferInternal;

    D3D11_BUFFER_DESC VertexBufferDesc;
    VertexBufferDesc.ByteWidth = sizeof(TangentVertex) * numVertices;
    VertexBufferDesc.Usage = D3D11_USAGE_DEFAULT;
    VertexBufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    VertexBufferDesc.CPUAccessFlags = 0;
    VertexBufferDesc.MiscFlags = 0;
    VertexBufferDesc.StructureByteStride = 0;

    D3D11_SUBRESOURCE_DATA VertexBufferData;
    VertexBufferData.pSysMem = vertexData;
    VertexBufferData.SysMemPitch = 0;
    VertexBufferData.SysMemSlicePitch = 0;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &VertexBufferDesc,
            &VertexBufferData,
            &vertexBufferInternal
            )
        );


    *vertexBuffer = vertexBufferInternal.Detach();
}

void BasicShapes::CreateCube(
    _Out_ ID3D11Buffer **vertexBuffer,
    _Out_ ID3D11Buffer **indexBuffer,
    _Out_opt_ unsigned int *vertexCount,
    _Out_opt_ unsigned int *indexCount
    )
{
    BasicVertex cubeVertices[] =
    {
        { float3(-0.5f, 0.5f, -0.5f), float3(0.0f, 1.0f, 0.0f), float2(0.0f, 0.0f) }, // +Y (top face)
        { float3( 0.5f, 0.5f, -0.5f), float3(0.0f, 1.0f, 0.0f), float2(1.0f, 0.0f) },
        { float3( 0.5f, 0.5f,  0.5f), float3(0.0f, 1.0f, 0.0f), float2(1.0f, 1.0f) },
        { float3(-0.5f, 0.5f,  0.5f), float3(0.0f, 1.0f, 0.0f), float2(0.0f, 1.0f) },

        { float3(-0.5f, -0.5f,  0.5f), float3(0.0f, -1.0f, 0.0f), float2(0.0f, 0.0f) }, // -Y (bottom face)
        { float3( 0.5f, -0.5f,  0.5f), float3(0.0f, -1.0f, 0.0f), float2(1.0f, 0.0f) },
        { float3 (0.5f, -0.5f, -0.5f), float3(0.0f, -1.0f, 0.0f), float2(1.0f, 1.0f) },
        { float3(-0.5f, -0.5f, -0.5f), float3(0.0f, -1.0f, 0.0f), float2(0.0f, 1.0f) },

        { float3(0.5f,  0.5f,  0.5f), float3(1.0f, 0.0f, 0.0f), float2(0.0f, 0.0f) }, // +X (right face)
        { float3(0.5f,  0.5f, -0.5f), float3(1.0f, 0.0f, 0.0f), float2(1.0f, 0.0f) },
        { float3(0.5f, -0.5f, -0.5f), float3(1.0f, 0.0f, 0.0f), float2(1.0f, 1.0f) },
        { float3(0.5f, -0.5f,  0.5f), float3(1.0f, 0.0f, 0.0f), float2(0.0f, 1.0f) },

        { float3(-0.5f,  0.5f, -0.5f), float3(-1.0f, 0.0f, 0.0f), float2(0.0f, 0.0f) }, // -X (left face)
        { float3(-0.5f,  0.5f,  0.5f), float3(-1.0f, 0.0f, 0.0f), float2(1.0f, 0.0f) },
        { float3(-0.5f, -0.5f,  0.5f), float3(-1.0f, 0.0f, 0.0f), float2(1.0f, 1.0f) },
        { float3(-0.5f, -0.5f, -0.5f), float3(-1.0f, 0.0f, 0.0f), float2(0.0f, 1.0f) },

        { float3(-0.5f,  0.5f, 0.5f), float3(0.0f, 0.0f, 1.0f), float2(0.0f, 0.0f) }, // +Z (front face)
        { float3( 0.5f,  0.5f, 0.5f), float3(0.0f, 0.0f, 1.0f), float2(1.0f, 0.0f) },
        { float3( 0.5f, -0.5f, 0.5f), float3(0.0f, 0.0f, 1.0f), float2(1.0f, 1.0f) },
        { float3(-0.5f, -0.5f, 0.5f), float3(0.0f, 0.0f, 1.0f), float2(0.0f, 1.0f) },

        { float3( 0.5f,  0.5f, -0.5f), float3(0.0f, 0.0f, -1.0f), float2(0.0f, 0.0f) }, // -Z (back face)
        { float3(-0.5f,  0.5f, -0.5f), float3(0.0f, 0.0f, -1.0f), float2(1.0f, 0.0f) },
        { float3(-0.5f, -0.5f, -0.5f), float3(0.0f, 0.0f, -1.0f), float2(1.0f, 1.0f) },
        { float3( 0.5f, -0.5f, -0.5f), float3(0.0f, 0.0f, -1.0f), float2(0.0f, 1.0f) },
    };

    unsigned short cubeIndices[] =
    {
        0, 1, 2,
        0, 2, 3,

        4, 5, 6,
        4, 6, 7,

        8, 9, 10,
        8, 10, 11,

        12, 13, 14,
        12, 14, 15,

        16, 17, 18,
        16, 18, 19,

        20, 21, 22,
        20, 22, 23
    };

    CreateVertexBuffer(
        ARRAYSIZE(cubeVertices),
        cubeVertices,
        vertexBuffer
        );
    if (vertexCount != nullptr)
    {
        *vertexCount = ARRAYSIZE(cubeVertices);
    }

    CreateIndexBuffer(
        ARRAYSIZE(cubeIndices),
        cubeIndices,
        indexBuffer
        );
    if (indexCount != nullptr)
    {
        *indexCount = ARRAYSIZE(cubeIndices);
    }
}

void BasicShapes::CreateBox(
    float3 r,
    _Out_ ID3D11Buffer **vertexBuffer,
    _Out_ ID3D11Buffer **indexBuffer,
    _Out_opt_ unsigned int *vertexCount,
    _Out_opt_ unsigned int *indexCount
    )
{
    BasicVertex boxVertices[] =
    {
        // FLOOR
        {float3(-r.x, -r.y,  r.z), float3(0.0f, 1.0f, 0.0f), float2(0.0f, 0.0f)},
        {float3( r.x, -r.y,  r.z), float3(0.0f, 1.0f, 0.0f), float2(1.0f, 0.0f)},
        {float3(-r.x, -r.y, -r.z), float3(0.0f, 1.0f, 0.0f), float2(0.0f, 1.5f)},
        {float3( r.x, -r.y, -r.z), float3(0.0f, 1.0f, 0.0f), float2(1.0f, 1.5f)},
        // WALL
        {float3(-r.x,  r.y, r.z), float3(0.0f, 0.0f, -1.0f), float2(0.0f, 0.0f)},
        {float3( r.x,  r.y, r.z), float3(0.0f, 0.0f, -1.0f), float2(2.0f, 0.0f)},
        {float3(-r.x, -r.y, r.z), float3(0.0f, 0.0f, -1.0f), float2(0.0f, 1.5f)},
        {float3( r.x, -r.y, r.z), float3(0.0f, 0.0f, -1.0f), float2(2.0f, 1.5f)},
        // WALL
        {float3(r.x,  r.y,  r.z), float3(-1.0f, 0.0f, 0.0f), float2(0.0f, 0.0f)},
        {float3(r.x,  r.y, -r.z), float3(-1.0f, 0.0f, 0.0f), float2(r.y,  0.0f)},
        {float3(r.x, -r.y,  r.z), float3(-1.0f, 0.0f, 0.0f), float2(0.0f, 1.5f)},
        {float3(r.x, -r.y, -r.z), float3(-1.0f, 0.0f, 0.0f), float2(r.y,  1.5f)},
        // WALL
        {float3( r.x,  r.y, -r.z), float3(0.0f, 0.0f, 1.0f), float2(0.0f, 0.0f)},
        {float3(-r.x,  r.y, -r.z), float3(0.0f, 0.0f, 1.0f), float2(2.0f, 0.0f)},
        {float3( r.x, -r.y, -r.z), float3(0.0f, 0.0f, 1.0f), float2(0.0f, 1.5f)},
        {float3(-r.x, -r.y, -r.z), float3(0.0f, 0.0f, 1.0f), float2(2.0f, 1.5f)},
        // WALL
        {float3(-r.x,  r.y, -r.z), float3(1.0f, 0.0f, 0.0f), float2(0.0f, 0.0f)},
        {float3(-r.x,  r.y,  r.z), float3(1.0f, 0.0f, 0.0f), float2(r.y,  0.0f)},
        {float3(-r.x, -r.y, -r.z), float3(1.0f, 0.0f, 0.0f), float2(0.0f, 1.5f)},
        {float3(-r.x, -r.y,  r.z), float3(1.0f, 0.0f, 0.0f), float2(r.y,  1.5f)},
        // CEILING
        {float3(-r.x, r.y, -r.z), float3(0.0f, -1.0f, 0.0f), float2(-0.15f, 0.0f)},
        {float3( r.x, r.y, -r.z), float3(0.0f, -1.0f, 0.0f), float2( 1.25f, 0.0f)},
        {float3(-r.x, r.y,  r.z), float3(0.0f, -1.0f, 0.0f), float2(-0.15f, 2.1f)},
        {float3( r.x, r.y,  r.z), float3(0.0f, -1.0f, 0.0f), float2( 1.25f, 2.1f)},
    };

    unsigned short boxIndices[] =
    {
        0, 2, 1,
        1, 2, 3,

        4, 6, 5,
        5, 6, 7,

        8, 10, 9,
        9, 10, 11,

        12, 14, 13,
        13, 14, 15,

        16, 18, 17,
        17, 18, 19,

        20, 22, 21,
        21, 22, 23,
    };

    CreateVertexBuffer(
        ARRAYSIZE(boxVertices),
        boxVertices,
        vertexBuffer
        );
    if (vertexCount != nullptr)
    {
        *vertexCount = ARRAYSIZE(boxVertices);
    }

    CreateIndexBuffer(
        ARRAYSIZE(boxIndices),
        boxIndices,
        indexBuffer
        );
    if (indexCount != nullptr)
    {
        *indexCount = ARRAYSIZE(boxIndices);
    }
}

void BasicShapes::CreateSphere(
    _Out_ ID3D11Buffer **vertexBuffer,
    _Out_ ID3D11Buffer **indexBuffer,
    _Out_opt_ unsigned int *vertexCount,
    _Out_opt_ unsigned int *indexCount
    )
{
    const int numSegments = 64;
    const int numSlices = numSegments / 2;

    const int numVertices = (numSlices + 1) * (numSegments + 1);
    std::unique_ptr<BasicVertex[]> sphereVertices(new BasicVertex[numVertices]);

    for (int slice = 0; slice <= numSlices; slice++)
    {
        float v = (float)slice/(float)numSlices;
        float inclination = v * PI_F;
        float y = cos(inclination);
        float r = sin(inclination);
        for (int segment = 0; segment <= numSegments; segment++)
        {
            float u = (float)segment/(float)numSegments;
            float azimuth = u * PI_F * 2.0f;
            int index = slice * (numSegments+1) + segment;
            sphereVertices[index].pos = float3(r * sin(azimuth), y, r * cos(azimuth));
            sphereVertices[index].norm = sphereVertices[index].pos;
            sphereVertices[index].tex = float2(u, v);
        }
    }

    const int numIndices = numSlices * (numSegments-2) * 6;
    std::unique_ptr<unsigned short[]> sphereIndices(new unsigned short[numIndices]);

    unsigned int index = 0;
    for (int slice = 0; slice < numSlices; slice++)
    {
        unsigned short sliceBase0 = (unsigned short)((slice)*(numSegments+1));
        unsigned short sliceBase1 = (unsigned short)((slice+1)*(numSegments+1));
        for (int segment = 0; segment < numSegments; segment++)
        {
            if (slice > 0)
            {
                sphereIndices[index++] = sliceBase0 + segment;
                sphereIndices[index++] = sliceBase0 + segment + 1;
                sphereIndices[index++] = sliceBase1 + segment + 1;
            }
            if (slice < numSlices-1)
            {
                sphereIndices[index++] = sliceBase0 + segment;
                sphereIndices[index++] = sliceBase1 + segment + 1;
                sphereIndices[index++] = sliceBase1 + segment;
            }
        }
    }

    CreateVertexBuffer(
        numVertices,
        sphereVertices.get(),
        vertexBuffer
        );
    if (vertexCount != nullptr)
    {
        *vertexCount = numVertices;
    }

    CreateIndexBuffer(
        numIndices,
        sphereIndices.get(),
        indexBuffer
        );
    if (indexCount != nullptr)
    {
        *indexCount = numIndices;
    }
}
void BasicShapes::CreateTangentSphere(
    _Out_ ID3D11Buffer **vertexBuffer,
    _Out_ ID3D11Buffer **indexBuffer,
    _Out_opt_ unsigned int *vertexCount,
    _Out_opt_ unsigned int *indexCount
    )
{
    const int numSegments = 64;
    const int numSlices = numSegments / 2;

    const int numVertices = (numSlices + 1) * (numSegments + 1);
    std::unique_ptr<TangentVertex[]> sphereVertices(new TangentVertex[numVertices]);

    for (int slice = 0; slice <= numSlices; slice++)
    {
        float v = (float)slice/(float)numSlices;
        float inclination = v * PI_F;
        float y = cos(inclination);
        float r = sin(inclination);
        for (int segment = 0; segment <= numSegments; segment++)
        {
            float u = (float)segment/(float)numSegments;
            float azimuth = u * PI_F * 2.0f;
            int index = slice * (numSegments+1) + segment;
            sphereVertices[index].pos = float3(r*sin(azimuth), y, r*cos(azimuth));
            sphereVertices[index].tex = float2(u, v);
            sphereVertices[index].uTan = float3(cos(azimuth), 0, -sin(azimuth));
            sphereVertices[index].vTan = float3(cos(inclination)*sin(azimuth), -sin(inclination), cos(inclination)*cos(azimuth));

        }
    }

    const int numIndices = numSlices * (numSegments-2) * 6;
    std::unique_ptr<unsigned short[]> sphereIndices(new unsigned short[numIndices]);

    unsigned int index = 0;
    for (int slice = 0; slice < numSlices; slice++)
    {
        unsigned short sliceBase0 = (unsigned short)((slice)*(numSegments+1));
        unsigned short sliceBase1 = (unsigned short)((slice+1)*(numSegments+1));
        for (int segment = 0; segment < numSegments; segment++)
        {
            if (slice > 0)
            {
                sphereIndices[index++] = sliceBase0 + segment;
                sphereIndices[index++] = sliceBase0 + segment + 1;
                sphereIndices[index++] = sliceBase1 + segment + 1;
            }
            if (slice < numSlices-1)
            {
                sphereIndices[index++] = sliceBase0 + segment;
                sphereIndices[index++] = sliceBase1 + segment + 1;
                sphereIndices[index++] = sliceBase1 + segment;
            }
        }
    }

    CreateTangentVertexBuffer(
        numVertices,
        sphereVertices.get(),
        vertexBuffer
        );
    if (vertexCount != nullptr)
    {
        *vertexCount = numVertices;
    }

    CreateIndexBuffer(
        numIndices,
        sphereIndices.get(),
        indexBuffer
        );
    if (indexCount != nullptr)
    {
        *indexCount = numIndices;
    }
}

void BasicShapes::CreateReferenceAxis(
    _Out_ ID3D11Buffer **vertexBuffer,
    _Out_ ID3D11Buffer **indexBuffer,
    _Out_opt_ unsigned int *vertexCount,
    _Out_opt_ unsigned int *indexCount
    )
{
    BasicVertex axisVertices[] =
    {
        { float3( 0.500f, 0.000f, 0.000f), float3( 0.125f, 0.500f, 0.500f), float2(0.250f, 0.250f) },
        { float3( 0.000f, 0.125f, 0.000f), float3( 0.125f, 0.500f, 0.500f), float2(0.250f, 0.250f) },
        { float3( 0.000f, 0.000f, 0.125f), float3( 0.125f, 0.500f, 0.500f), float2(0.250f, 0.250f) },
        { float3( 0.500f, 0.000f, 0.000f), float3( 0.125f,-0.500f, 0.500f), float2(0.250f, 0.250f) },
        { float3( 0.000f, 0.000f, 0.125f), float3( 0.125f,-0.500f, 0.500f), float2(0.250f, 0.250f) },
        { float3( 0.000f,-0.125f, 0.000f), float3( 0.125f,-0.500f, 0.500f), float2(0.250f, 0.250f) },
        { float3( 0.500f, 0.000f, 0.000f), float3( 0.125f,-0.500f,-0.500f), float2(0.250f, 0.250f) },
        { float3( 0.000f,-0.125f, 0.000f), float3( 0.125f,-0.500f,-0.500f), float2(0.250f, 0.250f) },
        { float3( 0.000f, 0.000f,-0.125f), float3( 0.125f,-0.500f,-0.500f), float2(0.250f, 0.250f) },
        { float3( 0.500f, 0.000f, 0.000f), float3( 0.125f, 0.500f,-0.500f), float2(0.250f, 0.250f) },
        { float3( 0.000f, 0.000f,-0.125f), float3( 0.125f, 0.500f,-0.500f), float2(0.250f, 0.250f) },
        { float3( 0.000f, 0.125f, 0.000f), float3( 0.125f, 0.500f,-0.500f), float2(0.250f, 0.250f) },
        { float3( 0.000f, 0.125f, 0.000f), float3(-0.125f, 0.000f, 0.000f), float2(0.250f, 0.250f) },
        { float3( 0.000f, 0.000f,-0.125f), float3(-0.125f, 0.000f, 0.000f), float2(0.250f, 0.250f) },
        { float3( 0.000f,-0.125f, 0.000f), float3(-0.125f, 0.000f, 0.000f), float2(0.250f, 0.250f) },
        { float3( 0.000f, 0.000f, 0.125f), float3(-0.125f, 0.000f, 0.000f), float2(0.250f, 0.250f) },
        { float3(-0.500f, 0.000f, 0.000f), float3(-0.125f, 0.500f, 0.500f), float2(0.250f, 0.500f) },
        { float3( 0.000f, 0.000f, 0.125f), float3(-0.125f, 0.500f, 0.500f), float2(0.250f, 0.500f) },
        { float3( 0.000f, 0.125f, 0.000f), float3(-0.125f, 0.500f, 0.500f), float2(0.250f, 0.500f) },
        { float3(-0.500f, 0.000f, 0.000f), float3(-0.125f, 0.500f,-0.500f), float2(0.250f, 0.500f) },
        { float3( 0.000f, 0.125f, 0.000f), float3(-0.125f, 0.500f,-0.500f), float2(0.250f, 0.500f) },
        { float3( 0.000f, 0.000f,-0.125f), float3(-0.125f, 0.500f,-0.500f), float2(0.250f, 0.500f) },
        { float3(-0.500f, 0.000f, 0.000f), float3(-0.125f,-0.500f,-0.500f), float2(0.250f, 0.500f) },
        { float3( 0.000f, 0.000f,-0.125f), float3(-0.125f,-0.500f,-0.500f), float2(0.250f, 0.500f) },
        { float3( 0.000f,-0.125f, 0.000f), float3(-0.125f,-0.500f,-0.500f), float2(0.250f, 0.500f) },
        { float3(-0.500f, 0.000f, 0.000f), float3(-0.125f,-0.500f, 0.500f), float2(0.250f, 0.500f) },
        { float3( 0.000f,-0.125f, 0.000f), float3(-0.125f,-0.500f, 0.500f), float2(0.250f, 0.500f) },
        { float3( 0.000f, 0.000f, 0.125f), float3(-0.125f,-0.500f, 0.500f), float2(0.250f, 0.500f) },
        { float3( 0.000f, 0.000f, 0.125f), float3( 0.125f, 0.000f, 0.000f), float2(0.250f, 0.500f) },
        { float3( 0.000f,-0.125f, 0.000f), float3( 0.125f, 0.000f, 0.000f), float2(0.250f, 0.500f) },
        { float3( 0.000f, 0.000f,-0.125f), float3( 0.125f, 0.000f, 0.000f), float2(0.250f, 0.500f) },
        { float3( 0.000f, 0.125f, 0.000f), float3( 0.125f, 0.000f, 0.000f), float2(0.250f, 0.500f) },
        { float3( 0.000f, 0.500f, 0.000f), float3( 0.500f, 0.125f, 0.500f), float2(0.500f, 0.250f) },
        { float3( 0.000f, 0.000f, 0.125f), float3( 0.500f, 0.125f, 0.500f), float2(0.500f, 0.250f) },
        { float3( 0.125f, 0.000f, 0.000f), float3( 0.500f, 0.125f, 0.500f), float2(0.500f, 0.250f) },
        { float3( 0.000f, 0.500f, 0.000f), float3( 0.500f, 0.125f,-0.500f), float2(0.500f, 0.250f) },
        { float3( 0.125f, 0.000f, 0.000f), float3( 0.500f, 0.125f,-0.500f), float2(0.500f, 0.250f) },
        { float3( 0.000f, 0.000f,-0.125f), float3( 0.500f, 0.125f,-0.500f), float2(0.500f, 0.250f) },
        { float3( 0.000f, 0.500f, 0.000f), float3(-0.500f, 0.125f,-0.500f), float2(0.500f, 0.250f) },
        { float3( 0.000f, 0.000f,-0.125f), float3(-0.500f, 0.125f,-0.500f), float2(0.500f, 0.250f) },
        { float3(-0.125f, 0.000f, 0.000f), float3(-0.500f, 0.125f,-0.500f), float2(0.500f, 0.250f) },
        { float3( 0.000f, 0.500f, 0.000f), float3(-0.500f, 0.125f, 0.500f), float2(0.500f, 0.250f) },
        { float3(-0.125f, 0.000f, 0.000f), float3(-0.500f, 0.125f, 0.500f), float2(0.500f, 0.250f) },
        { float3( 0.000f, 0.000f, 0.125f), float3(-0.500f, 0.125f, 0.500f), float2(0.500f, 0.250f) },
        { float3( 0.125f, 0.000f, 0.000f), float3( 0.000f,-0.125f, 0.000f), float2(0.500f, 0.250f) },
        { float3( 0.000f, 0.000f, 0.125f), float3( 0.000f,-0.125f, 0.000f), float2(0.500f, 0.250f) },
        { float3(-0.125f, 0.000f, 0.000f), float3( 0.000f,-0.125f, 0.000f), float2(0.500f, 0.250f) },
        { float3( 0.000f, 0.000f,-0.125f), float3( 0.000f,-0.125f, 0.000f), float2(0.500f, 0.250f) },
        { float3( 0.000f,-0.500f, 0.000f), float3( 0.500f,-0.125f, 0.500f), float2(0.500f, 0.500f) },
        { float3( 0.125f, 0.000f, 0.000f), float3( 0.500f,-0.125f, 0.500f), float2(0.500f, 0.500f) },
        { float3( 0.000f, 0.000f, 0.125f), float3( 0.500f,-0.125f, 0.500f), float2(0.500f, 0.500f) },
        { float3( 0.000f,-0.500f, 0.000f), float3(-0.500f,-0.125f, 0.500f), float2(0.500f, 0.500f) },
        { float3( 0.000f, 0.000f, 0.125f), float3(-0.500f,-0.125f, 0.500f), float2(0.500f, 0.500f) },
        { float3(-0.125f, 0.000f, 0.000f), float3(-0.500f,-0.125f, 0.500f), float2(0.500f, 0.500f) },
        { float3( 0.000f,-0.500f, 0.000f), float3(-0.500f,-0.125f,-0.500f), float2(0.500f, 0.500f) },
        { float3(-0.125f, 0.000f, 0.000f), float3(-0.500f,-0.125f,-0.500f), float2(0.500f, 0.500f) },
        { float3( 0.000f, 0.000f,-0.125f), float3(-0.500f,-0.125f,-0.500f), float2(0.500f, 0.500f) },
        { float3( 0.000f,-0.500f, 0.000f), float3( 0.500f,-0.125f,-0.500f), float2(0.500f, 0.500f) },
        { float3( 0.000f, 0.000f,-0.125f), float3( 0.500f,-0.125f,-0.500f), float2(0.500f, 0.500f) },
        { float3( 0.125f, 0.000f, 0.000f), float3( 0.500f,-0.125f,-0.500f), float2(0.500f, 0.500f) },
        { float3( 0.000f, 0.000f,-0.125f), float3( 0.000f, 0.125f, 0.000f), float2(0.500f, 0.500f) },
        { float3(-0.125f, 0.000f, 0.000f), float3( 0.000f, 0.125f, 0.000f), float2(0.500f, 0.500f) },
        { float3( 0.000f, 0.000f, 0.125f), float3( 0.000f, 0.125f, 0.000f), float2(0.500f, 0.500f) },
        { float3( 0.125f, 0.000f, 0.000f), float3( 0.000f, 0.125f, 0.000f), float2(0.500f, 0.500f) },
        { float3( 0.000f, 0.000f, 0.500f), float3( 0.500f, 0.500f, 0.125f), float2(0.750f, 0.250f) },
        { float3( 0.125f, 0.000f, 0.000f), float3( 0.500f, 0.500f, 0.125f), float2(0.750f, 0.250f) },
        { float3( 0.000f, 0.125f, 0.000f), float3( 0.500f, 0.500f, 0.125f), float2(0.750f, 0.250f) },
        { float3( 0.000f, 0.000f, 0.500f), float3(-0.500f, 0.500f, 0.125f), float2(0.750f, 0.250f) },
        { float3( 0.000f, 0.125f, 0.000f), float3(-0.500f, 0.500f, 0.125f), float2(0.750f, 0.250f) },
        { float3(-0.125f, 0.000f, 0.000f), float3(-0.500f, 0.500f, 0.125f), float2(0.750f, 0.250f) },
        { float3( 0.000f, 0.000f, 0.500f), float3(-0.500f,-0.500f, 0.125f), float2(0.750f, 0.250f) },
        { float3(-0.125f, 0.000f, 0.000f), float3(-0.500f,-0.500f, 0.125f), float2(0.750f, 0.250f) },
        { float3( 0.000f,-0.125f, 0.000f), float3(-0.500f,-0.500f, 0.125f), float2(0.750f, 0.250f) },
        { float3( 0.000f, 0.000f, 0.500f), float3( 0.500f,-0.500f, 0.125f), float2(0.750f, 0.250f) },
        { float3( 0.000f,-0.125f, 0.000f), float3( 0.500f,-0.500f, 0.125f), float2(0.750f, 0.250f) },
        { float3( 0.125f, 0.000f, 0.000f), float3( 0.500f,-0.500f, 0.125f), float2(0.750f, 0.250f) },
        { float3( 0.125f, 0.000f, 0.000f), float3( 0.000f, 0.000f,-0.125f), float2(0.750f, 0.250f) },
        { float3( 0.000f,-0.125f, 0.000f), float3( 0.000f, 0.000f,-0.125f), float2(0.750f, 0.250f) },
        { float3(-0.125f, 0.000f, 0.000f), float3( 0.000f, 0.000f,-0.125f), float2(0.750f, 0.250f) },
        { float3( 0.000f, 0.125f, 0.000f), float3( 0.000f, 0.000f,-0.125f), float2(0.750f, 0.250f) },
        { float3( 0.000f, 0.000f,-0.500f), float3( 0.500f, 0.500f,-0.125f), float2(0.750f, 0.500f) },
        { float3( 0.000f, 0.125f, 0.000f), float3( 0.500f, 0.500f,-0.125f), float2(0.750f, 0.500f) },
        { float3( 0.125f, 0.000f, 0.000f), float3( 0.500f, 0.500f,-0.125f), float2(0.750f, 0.500f) },
        { float3( 0.000f, 0.000f,-0.500f), float3( 0.500f,-0.500f,-0.125f), float2(0.750f, 0.500f) },
        { float3( 0.125f, 0.000f, 0.000f), float3( 0.500f,-0.500f,-0.125f), float2(0.750f, 0.500f) },
        { float3( 0.000f,-0.125f, 0.000f), float3( 0.500f,-0.500f,-0.125f), float2(0.750f, 0.500f) },
        { float3( 0.000f, 0.000f,-0.500f), float3(-0.500f,-0.500f,-0.125f), float2(0.750f, 0.500f) },
        { float3( 0.000f,-0.125f, 0.000f), float3(-0.500f,-0.500f,-0.125f), float2(0.750f, 0.500f) },
        { float3(-0.125f, 0.000f, 0.000f), float3(-0.500f,-0.500f,-0.125f), float2(0.750f, 0.500f) },
        { float3( 0.000f, 0.000f,-0.500f), float3(-0.500f, 0.500f,-0.125f), float2(0.750f, 0.500f) },
        { float3(-0.125f, 0.000f, 0.000f), float3(-0.500f, 0.500f,-0.125f), float2(0.750f, 0.500f) },
        { float3( 0.000f, 0.125f, 0.000f), float3(-0.500f, 0.500f,-0.125f), float2(0.750f, 0.500f) },
        { float3( 0.000f, 0.125f, 0.000f), float3( 0.000f, 0.000f, 0.125f), float2(0.750f, 0.500f) },
        { float3(-0.125f, 0.000f, 0.000f), float3( 0.000f, 0.000f, 0.125f), float2(0.750f, 0.500f) },
        { float3( 0.000f,-0.125f, 0.000f), float3( 0.000f, 0.000f, 0.125f), float2(0.750f, 0.500f) },
        { float3( 0.125f, 0.000f, 0.000f), float3( 0.000f, 0.000f, 0.125f), float2(0.750f, 0.500f) },
    };

    unsigned short axisIndices[] =
    {
         0,  2,  1,
         3,  5,  4,
         6,  8,  7,
         9, 11, 10,
        12, 14, 13,
        12, 15, 14,
        16, 18, 17,
        19, 21, 20,
        22, 24, 23,
        25, 27, 26,
        28, 30, 29,
        28, 31, 30,
        32, 34, 33,
        35, 37, 36,
        38, 40, 39,
        41, 43, 42,
        44, 46, 45,
        44, 47, 46,
        48, 50, 49,
        51, 53, 52,
        54, 56, 55,
        57, 59, 58,
        60, 62, 61,
        60, 63, 62,
        64, 66, 65,
        67, 69, 68,
        70, 72, 71,
        73, 75, 74,
        76, 78, 77,
        76, 79, 78,
        80, 82, 81,
        83, 85, 84,
        86, 88, 87,
        89, 91, 90,
        92, 94, 93,
        92, 95, 94,
    };

    CreateVertexBuffer(
        ARRAYSIZE(axisVertices),
        axisVertices,
        vertexBuffer
        );
    if (vertexCount != nullptr)
    {
        *vertexCount = ARRAYSIZE(axisVertices);
    }

    CreateIndexBuffer(
        ARRAYSIZE(axisIndices),
        axisIndices,
        indexBuffer
        );
    if (indexCount != nullptr)
    {
        *indexCount = ARRAYSIZE(axisIndices);
    }
}
