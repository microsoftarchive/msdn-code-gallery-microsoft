//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "ShadowSceneRenderer.h"
#include <DirectXColors.h>

#include "DirectXHelper.h"

using namespace ShadowMapping;

using namespace DirectX;
using namespace Windows::Foundation;

// Loads vertex and pixel shaders from files and instantiates the cube geometry.
ShadowSceneRenderer::ShadowSceneRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_loadingComplete(false),
    m_deviceResources(deviceResources),
    m_degreesPerSecond(22.5),
    m_indexCountCube(0),
    m_indexCountFloor(0),
    m_indexCountQuad(0),
    m_useLinear(false),
    m_shadowMapDimension(1024.f)
{
    CreateDeviceDependentResources();
    CreateWindowSizeDependentResources();
}

// Initialization.
void ShadowSceneRenderer::CreateDeviceDependentResources()
{
    DetermineShadowFeatureSupport();

    // Load shaders asynchronously.
    auto loadVSTask = DX::ReadDataAsync(L"VertexShader.cso");
    auto loadSimpleVSTask = DX::ReadDataAsync(L"SimpleVertexShader.cso");
    auto loadShadowPSPointTask = DX::ReadDataAsync(L"ShadowPixelShader_Point.cso");
    auto loadShadowPSLinearTask = DX::ReadDataAsync(L"ShadowPixelShader_Linear.cso");
    auto loadTexturePSTask = DX::ReadDataAsync(L"TextureShader.cso");
    auto loadComparisonPSTask = DX::ReadDataAsync(L"ComparisonShader.cso");

    // After the vertex shader file is loaded, create the shader and input layout.
    auto createVSTask = loadVSTask.then([this](const std::vector<byte>& fileData)
    {
        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateVertexShader(
                &fileData[0],
                fileData.size(),
                nullptr,
                &m_vertexShader
                )
            );

        static const D3D11_INPUT_ELEMENT_DESC vertexDesc[] =
        {
            { "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0,  D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT,    0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "NORMAL",   0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 20, D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "COLOR",    0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 32, D3D11_INPUT_PER_VERTEX_DATA, 0 },
        };

        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateInputLayout(
                vertexDesc,
                ARRAYSIZE(vertexDesc),
                &fileData[0],
                fileData.size(),
                &m_inputLayout
                )
            );

        CD3D11_BUFFER_DESC viewProjectionConstantBufferDesc(sizeof(ViewProjectionConstantBuffer), D3D11_BIND_CONSTANT_BUFFER);
        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateBuffer(
                &viewProjectionConstantBufferDesc,
                nullptr,
                &m_cubeViewProjectionBuffer
                )
            );

        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateBuffer(
                &viewProjectionConstantBufferDesc,
                nullptr,
                &m_lightViewProjectionBuffer
                )
            );

        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateBuffer(
                &viewProjectionConstantBufferDesc,
                nullptr,
                &m_orthoViewProjectionBuffer
                )
            );

        CD3D11_BUFFER_DESC modelConstantBufferDesc(sizeof(ModelConstantBuffer), D3D11_BIND_CONSTANT_BUFFER);
        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateBuffer(
                &modelConstantBufferDesc,
                nullptr,
                &m_staticModelBuffer
                )
            );

        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateBuffer(
                &modelConstantBufferDesc,
                nullptr,
                &m_rotatedModelBuffer
                )
            );

        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateBuffer(
                &modelConstantBufferDesc,
                nullptr,
                &m_orthoTransformBuffer
                )
            );
    });

    auto createSimpleVSTask = loadSimpleVSTask.then([this](const std::vector<byte>& fileData)
    {
        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateVertexShader(
                &fileData[0],
                fileData.size(),
                nullptr,
                &m_simpleVertexShader
                )
            );
    });

    // After the pixel shader file is loaded, create the shader.
    auto createShadowPSLinearTask = loadShadowPSLinearTask.then([this](const std::vector<byte>& fileData)
    {
        if (m_deviceSupportsD3D9Shadows)
        {
            DX::ThrowIfFailed(
                m_deviceResources->GetD3DDevice()->CreatePixelShader(
                &fileData[0],
                fileData.size(),
                nullptr,
                &m_shadowPixelShader_linear
                )
                );
        }
    });

    auto createShadowPSPointTask = loadShadowPSPointTask.then([this](const std::vector<byte>& fileData)
    {
        if (m_deviceSupportsD3D9Shadows)
        {
            DX::ThrowIfFailed(
                m_deviceResources->GetD3DDevice()->CreatePixelShader(
                &fileData[0],
                fileData.size(),
                nullptr,
                &m_shadowPixelShader_point
                )
                );
        }
    });

    auto createTexturePSTask = loadTexturePSTask.then([this](const std::vector<byte>& fileData)
    {
        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreatePixelShader(
                &fileData[0],
                fileData.size(),
                nullptr,
                &m_textureShader
                )
            );
    });

    auto createComparisonPSTask = loadComparisonPSTask.then([this](const std::vector<byte>& fileData)
    {
        if (m_deviceSupportsD3D9Shadows)
        {
            DX::ThrowIfFailed(
                m_deviceResources->GetD3DDevice()->CreatePixelShader(
                &fileData[0],
                fileData.size(),
                nullptr,
                &m_comparisonShader
                )
                );
        }
    });

    // Once the shaders are loaded, create meshes.
    auto createQuadTask = (createVSTask && createSimpleVSTask).then([this]()
    {

        // Load mesh vertices. Each vertex has a position and a color.
        static const VertexPositionTexNormColor quadVertices [] =
        {
            {XMFLOAT3(0.f, 0.f, 0.0f), XMFLOAT2(0.f, 1.f), XMFLOAT3(0.f, 0.f, 1.f), XMFLOAT3(1.0f, 1.0f, 1.0f)},
            {XMFLOAT3(0.f, 1.f, 0.0f), XMFLOAT2(0.f, 0.f), XMFLOAT3(0.f, 0.f, 1.f), XMFLOAT3(1.0f, 1.0f, 1.0f)},
            {XMFLOAT3(1.f, 1.f, 0.0f), XMFLOAT2(1.f, 0.f), XMFLOAT3(0.f, 0.f, 1.f), XMFLOAT3(1.0f, 1.0f, 1.0f)},
            {XMFLOAT3(1.f, 0.f, 0.0f), XMFLOAT2(1.f, 1.f), XMFLOAT3(0.f, 0.f, 1.f), XMFLOAT3(1.0f, 1.0f, 1.0f)},
        };

        D3D11_SUBRESOURCE_DATA vertexBufferData = { 0 };
        vertexBufferData.pSysMem = quadVertices;
        vertexBufferData.SysMemPitch = 0;
        vertexBufferData.SysMemSlicePitch = 0;
        CD3D11_BUFFER_DESC vertexBufferDesc(sizeof(quadVertices), D3D11_BIND_VERTEX_BUFFER);
        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateBuffer(
                &vertexBufferDesc,
                &vertexBufferData,
                &m_vertexBufferQuad
                )
            );

        // Load mesh indices. Each triple of indices represents
        // a triangle to be rendered on the screen.
        // For example, 0, 2, 1 means that the vertices with indexes
        // 0, 2 and 1 from the vertex buffer compose the
        // first triangle of this mesh.
        static const unsigned short quadIndices [] =
        {
            0, 1, 2,
            0, 2, 3,
        };

        m_indexCountQuad = ARRAYSIZE(quadIndices);

        D3D11_SUBRESOURCE_DATA indexBufferData = { 0 };
        indexBufferData.pSysMem = quadIndices;
        indexBufferData.SysMemPitch = 0;
        indexBufferData.SysMemSlicePitch = 0;
        CD3D11_BUFFER_DESC indexBufferDesc(sizeof(quadIndices), D3D11_BIND_INDEX_BUFFER);
        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateBuffer(
                &indexBufferDesc,
                &indexBufferData,
                &m_indexBufferQuad
                )
            );
    });

    auto createFloorTask = createQuadTask.then([this]()
    {

        float w = 999.f; // Tile width.

        // Load a quad that appears to be an infinite plane.
        static const VertexPositionTexNormColor floorVertices [] =
        {
            {XMFLOAT3(-w, -10.f, -w), XMFLOAT2(0.f, 0.f), XMFLOAT3(0.f, 1.f, 0.f), XMFLOAT3(1.0f, 1.0f, 1.0f)},
            {XMFLOAT3(-w, -10.f, w), XMFLOAT2(0.f, 1.f), XMFLOAT3(0.f, 1.f, 0.f), XMFLOAT3(1.0f, 1.0f, 1.0f)},
            {XMFLOAT3(w, -10.f, w), XMFLOAT2(1.f, 1.f), XMFLOAT3(0.f, 1.f, 0.f), XMFLOAT3(1.0f, 1.0f, 1.0f)},
            {XMFLOAT3(w, -10.f, -w), XMFLOAT2(1.f, 0.f), XMFLOAT3(0.f, 1.f, 0.f), XMFLOAT3(1.0f, 1.0f, 1.0f)},
        };

        D3D11_SUBRESOURCE_DATA vertexBufferData = { 0 };
        vertexBufferData.pSysMem = floorVertices;
        vertexBufferData.SysMemPitch = 0;
        vertexBufferData.SysMemSlicePitch = 0;
        CD3D11_BUFFER_DESC vertexBufferDesc(sizeof(floorVertices), D3D11_BIND_VERTEX_BUFFER);
        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateBuffer(
                &vertexBufferDesc,
                &vertexBufferData,
                &m_vertexBufferFloor
                )
            );

        // Load mesh indices. Each triple of indices represents
        // a triangle to be rendered on the screen.
        // For example, 0, 2, 1 means that the vertices with indexes
        // 0, 2 and 1 from the vertex buffer compose the
        // first triangle of this mesh.
        static const unsigned short floorIndices [] =
        {
            0, 2, 1,
            3, 2, 0,
        };

        m_indexCountFloor = ARRAYSIZE(floorIndices);

        D3D11_SUBRESOURCE_DATA indexBufferData = { 0 };
        indexBufferData.pSysMem = floorIndices;
        indexBufferData.SysMemPitch = 0;
        indexBufferData.SysMemSlicePitch = 0;
        CD3D11_BUFFER_DESC indexBufferDesc(sizeof(floorIndices), D3D11_BIND_INDEX_BUFFER);
        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateBuffer(
                &indexBufferDesc,
                &indexBufferData,
                &m_indexBufferFloor
                )
            );
    });

    auto createCubeTask = createFloorTask.then([this]()
    {

        // Load mesh vertices. Each vertex has a position and a color.

        float a = 10.f;

        VertexPositionTexNormColor cubeVertices [] =
        {
            {XMFLOAT3(a, a, -a), XMFLOAT2(1.0f, 0.0f), XMFLOAT3(0.f, 1.f, 0.f), XMFLOAT3(1.0f, 1.0f, 0.0f)}, // +y
            {XMFLOAT3(a, a, a), XMFLOAT2(1.0f, 1.0f), XMFLOAT3(0.f, 1.f, 0.f), XMFLOAT3(1.0f, 1.0f, 1.0f)},
            {XMFLOAT3(-a, a, a), XMFLOAT2(0.0f, 1.0f), XMFLOAT3(0.f, 1.f, 0.f), XMFLOAT3(0.0f, 1.0f, 1.0f)},
            {XMFLOAT3(-a, a, -a), XMFLOAT2(0.0f, 0.0f), XMFLOAT3(0.f, 1.f, 0.f), XMFLOAT3(0.0f, 1.0f, 0.0f)},

            {XMFLOAT3(-a, -a, -a), XMFLOAT2(0.0f, 0.0f), XMFLOAT3(0.f, -1.f, 0.f), XMFLOAT3(0.0f, 0.0f, 0.0f)}, // -y
            {XMFLOAT3(-a, -a, a), XMFLOAT2(0.0f, 1.0f), XMFLOAT3(0.f, -1.f, 0.f), XMFLOAT3(0.0f, 0.0f, 1.0f)},
            {XMFLOAT3(a, -a, a), XMFLOAT2(1.0f, 1.0f), XMFLOAT3(0.f, -1.f, 0.f), XMFLOAT3(1.0f, 0.0f, 1.0f)},
            {XMFLOAT3(a, -a, -a), XMFLOAT2(1.0f, 0.0f), XMFLOAT3(0.f, -1.f, 0.f), XMFLOAT3(1.0f, 0.0f, 0.0f)},

            {XMFLOAT3(a, -a, -a), XMFLOAT2(0.0f, 0.0f), XMFLOAT3(1.f, 0.f, 0.f), XMFLOAT3(1.0f, 0.0f, 0.0f)}, // +x
            {XMFLOAT3(a, -a, a), XMFLOAT2(0.0f, 1.0f), XMFLOAT3(1.f, 0.f, 0.f), XMFLOAT3(1.0f, 0.0f, 1.0f)},
            {XMFLOAT3(a, a, a), XMFLOAT2(1.0f, 1.0f), XMFLOAT3(1.f, 0.f, 0.f), XMFLOAT3(1.0f, 1.0f, 1.0f)},
            {XMFLOAT3(a, a, -a), XMFLOAT2(1.0f, 0.0f), XMFLOAT3(1.f, 0.f, 0.f), XMFLOAT3(1.0f, 1.0f, 0.0f)},

            {XMFLOAT3(-a, a, -a), XMFLOAT2(1.0f, 0.0f), XMFLOAT3(-1.f, 0.f, 0.f), XMFLOAT3(0.0f, 1.0f, 0.0f)}, // -x
            {XMFLOAT3(-a, a, a), XMFLOAT2(1.0f, 1.0f), XMFLOAT3(-1.f, 0.f, 0.f), XMFLOAT3(0.0f, 1.0f, 1.0f)},
            {XMFLOAT3(-a, -a, a), XMFLOAT2(0.0f, 1.0f), XMFLOAT3(-1.f, 0.f, 0.f), XMFLOAT3(0.0f, 0.0f, 1.0f)},
            {XMFLOAT3(-a, -a, -a), XMFLOAT2(0.0f, 0.0f), XMFLOAT3(-1.f, 0.f, 0.f), XMFLOAT3(0.0f, 0.0f, 0.0f)},

            {XMFLOAT3(-a, -a, a), XMFLOAT2(0.0f, 0.0f), XMFLOAT3(0.f, 0.f, 1.f), XMFLOAT3(0.0f, 0.0f, 1.0f)}, // +z
            {XMFLOAT3(-a, a, a), XMFLOAT2(0.0f, 1.0f), XMFLOAT3(0.f, 0.f, 1.f), XMFLOAT3(0.0f, 1.0f, 1.0f)},
            {XMFLOAT3(a, a, a), XMFLOAT2(1.0f, 1.0f), XMFLOAT3(0.f, 0.f, 1.f), XMFLOAT3(1.0f, 1.0f, 1.0f)},
            {XMFLOAT3(a, -a, a), XMFLOAT2(1.0f, 0.0f), XMFLOAT3(0.f, 0.f, 1.f), XMFLOAT3(1.0f, 0.0f, 1.0f)},

            {XMFLOAT3(a, -a, -a), XMFLOAT2(1.0f, 0.0f), XMFLOAT3(0.f, 0.f, -1.f), XMFLOAT3(1.0f, 0.0f, 0.0f)}, // -z
            {XMFLOAT3(a, a, -a), XMFLOAT2(1.0f, 1.0f), XMFLOAT3(0.f, 0.f, -1.f), XMFLOAT3(1.0f, 1.0f, 0.0f)},
            {XMFLOAT3(-a, a, -a), XMFLOAT2(0.0f, 1.0f), XMFLOAT3(0.f, 0.f, -1.f), XMFLOAT3(0.0f, 1.0f, 0.0f)},
            {XMFLOAT3(-a, -a, -a), XMFLOAT2(0.0f, 0.0f), XMFLOAT3(0.f, 0.f, -1.f), XMFLOAT3(0.0f, 0.0f, 0.0f)},
        };

        unsigned short cubeIndices [] =
        {
            0, 1, 2,    // +y
            0, 2, 3,

            4, 5, 6,    // -y
            4, 6, 7,

            8, 9, 10,    // +x
            8, 10, 11,

            12, 13, 14, // -x
            12, 14, 15,

            16, 17, 18, // +z
            16, 18, 19,

            20, 21, 22, // -z
            20, 22, 23,
        };

        m_indexCountCube = ARRAYSIZE(cubeIndices);

        D3D11_SUBRESOURCE_DATA vertexBufferData = {0};
        vertexBufferData.pSysMem = cubeVertices;
        vertexBufferData.SysMemPitch = 0;
        vertexBufferData.SysMemSlicePitch = 0;
        CD3D11_BUFFER_DESC vertexBufferDesc(sizeof(cubeVertices), D3D11_BIND_VERTEX_BUFFER);
        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateBuffer(
                &vertexBufferDesc,
                &vertexBufferData,
                &m_vertexBuffer
                )
            );

        D3D11_SUBRESOURCE_DATA indexBufferData = {0};
        indexBufferData.pSysMem = cubeIndices;
        indexBufferData.SysMemPitch = 0;
        indexBufferData.SysMemSlicePitch = 0;
        CD3D11_BUFFER_DESC indexBufferDesc(sizeof(cubeIndices), D3D11_BIND_INDEX_BUFFER);
        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateBuffer(
                &indexBufferDesc,
                &indexBufferData,
                &m_indexBuffer
                )
            );
    });

    auto createShadowBuffersTask = (createShadowPSPointTask && createShadowPSLinearTask).then([this]()
    {
        if (m_deviceSupportsD3D9Shadows)
        {
            // Init shadow map resources

            InitShadowMap();

            // Init samplers

            ID3D11Device1* pD3DDevice = m_deviceResources->GetD3DDevice();

            D3D11_SAMPLER_DESC comparisonSamplerDesc;
            ZeroMemory(&comparisonSamplerDesc, sizeof(D3D11_SAMPLER_DESC));
            comparisonSamplerDesc.AddressU = D3D11_TEXTURE_ADDRESS_BORDER;
            comparisonSamplerDesc.AddressV = D3D11_TEXTURE_ADDRESS_BORDER;
            comparisonSamplerDesc.AddressW = D3D11_TEXTURE_ADDRESS_BORDER;
            comparisonSamplerDesc.BorderColor[0] = 1.0f;
            comparisonSamplerDesc.BorderColor[1] = 1.0f;
            comparisonSamplerDesc.BorderColor[2] = 1.0f;
            comparisonSamplerDesc.BorderColor[3] = 1.0f;
            comparisonSamplerDesc.MinLOD = 0.f;
            comparisonSamplerDesc.MaxLOD = D3D11_FLOAT32_MAX;
            comparisonSamplerDesc.MipLODBias = 0.f;
            comparisonSamplerDesc.MaxAnisotropy = 0;
            comparisonSamplerDesc.ComparisonFunc = D3D11_COMPARISON_LESS_EQUAL;
            comparisonSamplerDesc.Filter = D3D11_FILTER_COMPARISON_MIN_MAG_MIP_POINT;

            // Point filtered shadows can be faster, and may be a good choice when
            // rendering on hardware with lower feature levels. This sample has a
            // UI option to enable/disable filtering so you can see the difference
            // in quality and speed.

            DX::ThrowIfFailed(
                pD3DDevice->CreateSamplerState(
                    &comparisonSamplerDesc,
                    &m_comparisonSampler_point
                    )
                );

            comparisonSamplerDesc.Filter = D3D11_FILTER_COMPARISON_MIN_MAG_MIP_LINEAR;
            DX::ThrowIfFailed(
                pD3DDevice->CreateSamplerState(
                    &comparisonSamplerDesc,
                    &m_comparisonSampler_linear
                    )
                );

            D3D11_SAMPLER_DESC linearSamplerDesc;
            ZeroMemory(&linearSamplerDesc, sizeof(D3D11_SAMPLER_DESC));
            linearSamplerDesc.AddressU = D3D11_TEXTURE_ADDRESS_CLAMP;
            linearSamplerDesc.AddressV = D3D11_TEXTURE_ADDRESS_CLAMP;
            linearSamplerDesc.AddressW = D3D11_TEXTURE_ADDRESS_CLAMP;
            linearSamplerDesc.MinLOD = 0;
            linearSamplerDesc.MaxLOD = D3D11_FLOAT32_MAX;
            linearSamplerDesc.MipLODBias = 0.f;
            linearSamplerDesc.MaxAnisotropy = 0;
            linearSamplerDesc.Filter = D3D11_FILTER_MIN_MAG_MIP_LINEAR;
            linearSamplerDesc.ComparisonFunc = D3D11_COMPARISON_NEVER;

            DX::ThrowIfFailed(
                pD3DDevice->CreateSamplerState(
                    &linearSamplerDesc,
                    &m_linearSampler
                    )
                );

            // Init render states for back/front face culling

            D3D11_RASTERIZER_DESC drawingRenderStateDesc;
            ZeroMemory(&drawingRenderStateDesc, sizeof(D3D11_RASTERIZER_DESC));
            drawingRenderStateDesc.CullMode = D3D11_CULL_BACK;
            drawingRenderStateDesc.FillMode = D3D11_FILL_SOLID;
            drawingRenderStateDesc.DepthClipEnable = true; // Feature level 9_1 requires DepthClipEnable == true
            DX::ThrowIfFailed(
                pD3DDevice->CreateRasterizerState(
                    &drawingRenderStateDesc,
                    &m_drawingRenderState
                    )
                );

            D3D11_RASTERIZER_DESC shadowRenderStateDesc;
            ZeroMemory(&shadowRenderStateDesc, sizeof(D3D11_RASTERIZER_DESC));
            shadowRenderStateDesc.CullMode = D3D11_CULL_FRONT;
            shadowRenderStateDesc.FillMode = D3D11_FILL_SOLID;
            shadowRenderStateDesc.DepthClipEnable = true;

            DX::ThrowIfFailed(
                pD3DDevice->CreateRasterizerState(
                    &shadowRenderStateDesc,
                    &m_shadowRenderState
                    )
                );
        }
    });

    // Once everything is loaded, we can let rendering happen.
    (createTexturePSTask && createComparisonPSTask && createCubeTask && createShadowBuffersTask).then([this]()
    {
        // Init static constant buffers
        UpdateAllConstantBuffers();

        m_loadingComplete = true;
    });
}

// Determine whether the driver supports depth comparison in the pixel shader.
void ShadowSceneRenderer::DetermineShadowFeatureSupport()
{
    ID3D11Device1* pD3DDevice = m_deviceResources->GetD3DDevice();

    D3D11_FEATURE_DATA_D3D9_SHADOW_SUPPORT isD3D9ShadowSupported;
    ZeroMemory(&isD3D9ShadowSupported, sizeof(isD3D9ShadowSupported));
    pD3DDevice->CheckFeatureSupport(
        D3D11_FEATURE_D3D9_SHADOW_SUPPORT,
        &isD3D9ShadowSupported,
        sizeof(D3D11_FEATURE_D3D9_SHADOW_SUPPORT)
        );

    if (isD3D9ShadowSupported.SupportsDepthAsTextureWithLessEqualComparisonFilter)
    {
        m_deviceSupportsD3D9Shadows = true;
    }
    else
    {
        m_deviceSupportsD3D9Shadows = false;
    }
}

// Initialize a new shadow buffer with size equal to m_shadowMapDimension.
void ShadowSceneRenderer::InitShadowMap()
{
    ID3D11Device1* pD3DDevice = m_deviceResources->GetD3DDevice();

    D3D11_TEXTURE2D_DESC shadowMapDesc;
    ZeroMemory(&shadowMapDesc, sizeof(D3D11_TEXTURE2D_DESC));
    shadowMapDesc.Format = DXGI_FORMAT_R24G8_TYPELESS;
    shadowMapDesc.MipLevels = 1;
    shadowMapDesc.ArraySize = 1;
    shadowMapDesc.SampleDesc.Count = 1;
    shadowMapDesc.BindFlags = D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_DEPTH_STENCIL;
    shadowMapDesc.Height = static_cast<UINT>(m_shadowMapDimension);
    shadowMapDesc.Width = static_cast<UINT>(m_shadowMapDimension);

    D3D11_DEPTH_STENCIL_VIEW_DESC depthStencilViewDesc;
    ZeroMemory(&depthStencilViewDesc, sizeof(D3D11_DEPTH_STENCIL_VIEW_DESC));
    depthStencilViewDesc.Format = DXGI_FORMAT_D24_UNORM_S8_UINT;
    depthStencilViewDesc.ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2D;
    depthStencilViewDesc.Texture2D.MipSlice = 0;

    D3D11_SHADER_RESOURCE_VIEW_DESC shaderResourceViewDesc;
    ZeroMemory(&shaderResourceViewDesc, sizeof(D3D11_SHADER_RESOURCE_VIEW_DESC));
    shaderResourceViewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
    shaderResourceViewDesc.Format = DXGI_FORMAT_R24_UNORM_X8_TYPELESS;
    shaderResourceViewDesc.Texture2D.MipLevels = 1;

    HRESULT hr = pD3DDevice->CreateTexture2D(
        &shadowMapDesc,
        nullptr,
        &m_shadowMap
        );

    hr = pD3DDevice->CreateDepthStencilView(
        m_shadowMap.Get(),
        &depthStencilViewDesc,
        &m_shadowDepthView
        );

    hr = pD3DDevice->CreateShaderResourceView(
        m_shadowMap.Get(),
        &shaderResourceViewDesc,
        &m_shadowResourceView
        );

    if (FAILED(hr))
    {
        shadowMapDesc.Format = DXGI_FORMAT_R16_TYPELESS;
        depthStencilViewDesc.Format = DXGI_FORMAT_D16_UNORM;
        shaderResourceViewDesc.Format = DXGI_FORMAT_R16_UNORM;

        DX::ThrowIfFailed(
            pD3DDevice->CreateTexture2D(
            &shadowMapDesc,
            nullptr,
            &m_shadowMap
            )
            );

        DX::ThrowIfFailed(
            pD3DDevice->CreateDepthStencilView(
            m_shadowMap.Get(),
            &depthStencilViewDesc,
            &m_shadowDepthView
            )
            );

        DX::ThrowIfFailed(
            pD3DDevice->CreateShaderResourceView(
            m_shadowMap.Get(),
            &shaderResourceViewDesc,
            &m_shadowResourceView
            )
            );
    }

    // Init viewport for shadow rendering.
    ZeroMemory(&m_shadowViewport, sizeof(D3D11_VIEWPORT));
    m_shadowViewport.Height = m_shadowMapDimension;
    m_shadowViewport.Width = m_shadowMapDimension;
    m_shadowViewport.MinDepth = 0.f;
    m_shadowViewport.MaxDepth = 1.f;
}

// Initializes view parameters when the window size changes.
void ShadowSceneRenderer::CreateWindowSizeDependentResources()
{
    Size windowSize = m_deviceResources->GetLogicalSize();
    float aspectRatio = windowSize.Width / windowSize.Height;
    float fovAngleY = 70.0f * XM_PI / 180.0f;

    // This is a simple example of change that can be made when the app is in
    // portrait or snapped view.
    if (aspectRatio < 1.0f)
    {
        fovAngleY *= 1.5f;
    }

    // Note that the OrientationTransform3D matrix is post-multiplied here
    // in order to correctly orient the scene to match the display orientation.
    // This post-multiplication step is required for any draw calls that are
    // made to the swap chain render target. For draw calls to other targets,
    // this transform should not be applied.

    XMFLOAT4X4 orientation = m_deviceResources->GetOrientationTransform3D();
    XMMATRIX orientationMatrix = XMLoadFloat4x4(&orientation);

    // This sample makes use of a right-handed coordinate system using row-major matrices.

    // Set up the camera view.
    {
        XMMATRIX perspectiveMatrix = XMMatrixPerspectiveFovRH(
            fovAngleY,
            aspectRatio,
            8.f,
            600.0f
            );

        XMStoreFloat4x4(
            &m_cubeViewProjectionBufferData.projection,
            XMMatrixTranspose(perspectiveMatrix * orientationMatrix)
            );

        // Eye is at (0, 14, 30), looking at point (0, -0.1, 0) with the up-vector along the y-axis.
        static const XMVECTORF32 eye = { 0.0f, 14.f, 30.f, 0.0f };
        static const XMVECTORF32 at = { 0.0f, -0.1f, 0.0f, 0.0f };
        static const XMVECTORF32 up = { 0.0f, 1.0f, 0.0f, 0.0f };

        XMStoreFloat4x4(
            &m_cubeViewProjectionBufferData.view,
            XMMatrixTranspose(XMMatrixLookAtRH(eye, at, up))
            );

        // Store the eye position for lighting calculations.
        XMStoreFloat4(&m_cubeViewProjectionBufferData.pos, eye);
    }

    // Initialize the model matrix for objects that aren't animated.
    XMStoreFloat4x4(&m_staticModelBufferData.model, XMMatrixIdentity());

    // Set up the light view.
    {
        XMMATRIX lightPerspectiveMatrix = XMMatrixPerspectiveFovRH(
            XM_PIDIV2,
            1.0f,
            12.f,
            50.f
            );

        XMStoreFloat4x4(
            &m_lightViewProjectionBufferData.projection,
            XMMatrixTranspose(lightPerspectiveMatrix)
            );

        // Point light at (20, 15, 20), pointed at the origin. POV up-vector is along the y-axis.
        static const XMVECTORF32 eye = { 20.0f, 15.0f, 20.0f, 0.0f };
        static const XMVECTORF32 at = { 0.0f, 0.0f, 0.0f, 0.0f };
        static const XMVECTORF32 up = { 0.0f, 1.0f, 0.0f, 0.0f };

        XMStoreFloat4x4(
            &m_lightViewProjectionBufferData.view,
            XMMatrixTranspose(XMMatrixLookAtRH(eye, at, up))
            );

        // Store the light position to help calculate the shadow offset.
        XMStoreFloat4(&m_lightViewProjectionBufferData.pos, eye);
    }

    // Set up the overlay view.
    {
        float orthoHeight = 3.f;
        float orthoWidth = orthoHeight * aspectRatio;

        XMStoreFloat4x4(
            &m_orthoViewProjectionBufferData.projection,
            XMMatrixTranspose(
                XMMatrixOrthographicRH(orthoWidth, orthoHeight, 0.01f, 10.f) * orientationMatrix
                )
            );

        // Overlay camera is at (0, 0, 1), pointed at origin, with the up-vector along the y-axis.
        static const XMVECTORF32 eye = { 0.0f, 0.0f, 1.0f, 0.0f };
        static const XMVECTORF32 at = { 0.0f, 0.0f, 0.0f, 0.0f };
        static const XMVECTORF32 up = { 0.0f, 1.0f, 0.0f, 0.0f };

        XMStoreFloat4x4(
            &m_orthoViewProjectionBufferData.view,
            XMMatrixTranspose(XMMatrixLookAtRH(eye, at, up))
            );

        // Initialize the model matrix for the overlay quad used to display the shadow buffer contents.
        XMVECTORF32 overlayPlacement = { (orthoWidth / 2.f) - 1.1f, 0.2f, 0.f, 0.f };
        XMStoreFloat4x4(
            &m_orthoTransformBufferData.model,
            XMMatrixTranspose(XMMatrixTranslationFromVector(overlayPlacement))
            );
    }

    if (m_loadingComplete)
    {
        UpdateAllConstantBuffers();
    }
}

// Release all references to resources that depend on the graphics device.
// This method is invoked when the device is lost and resources are no longer usable.
void ShadowSceneRenderer::ReleaseDeviceDependentResources()
{
    m_loadingComplete = false;

    m_inputLayout.Reset();
    m_vertexBuffer.Reset();
    m_indexBuffer.Reset();
    m_vertexShader.Reset();
    m_simpleVertexShader.Reset();
    m_shadowPixelShader_point.Reset();
    m_shadowPixelShader_linear.Reset();
    m_comparisonShader.Reset();
    m_textureShader.Reset();

    m_shadowMap.Reset();
    m_shadowDepthView.Reset();
    m_shadowResourceView.Reset();
    m_comparisonSampler_point.Reset();
    m_comparisonSampler_linear.Reset();
    m_linearSampler.Reset();

    m_cubeViewProjectionBuffer.Reset();
    m_lightViewProjectionBuffer.Reset();
    m_orthoViewProjectionBuffer.Reset();
    m_staticModelBuffer.Reset();
    m_rotatedModelBuffer.Reset();
    m_orthoTransformBuffer.Reset();

    m_shadowRenderState.Reset();
    m_drawingRenderState.Reset();

    m_vertexBufferQuad.Reset();
    m_indexBufferQuad.Reset();
    m_vertexBufferFloor.Reset();
    m_indexBufferFloor.Reset();
}

void ShadowSceneRenderer::UpdateAllConstantBuffers()
{
    auto context = m_deviceResources->GetD3DDeviceContext();

    context->UpdateSubresource(
        m_cubeViewProjectionBuffer.Get(),
        0,
        NULL,
        &m_cubeViewProjectionBufferData,
        0,
        0
        );

    context->UpdateSubresource(
        m_lightViewProjectionBuffer.Get(),
        0,
        NULL,
        &m_lightViewProjectionBufferData,
        0,
        0
        );

    context->UpdateSubresource(
        m_orthoViewProjectionBuffer.Get(),
        0,
        NULL,
        &m_orthoViewProjectionBufferData,
        0,
        0
        );

    context->UpdateSubresource(
        m_staticModelBuffer.Get(),
        0,
        NULL,
        &m_staticModelBufferData,
        0,
        0
        );

    context->UpdateSubresource(
        m_orthoTransformBuffer.Get(),
        0,
        NULL,
        &m_orthoTransformBufferData,
        0,
        0
        );

    context->UpdateSubresource(
        m_rotatedModelBuffer.Get(),
        0,
        NULL,
        &m_rotatedModelBufferData,
        0,
        0
        );
}

// Called once per frame, creates a Y rotation based on elapsed time.
void ShadowSceneRenderer::Update(DX::StepTimer const& timer)
{
    // Convert degrees to radians, then convert seconds to rotation angle.
    float radiansPerSecond = XMConvertToRadians(m_degreesPerSecond);
    double totalRotation = timer.GetTotalSeconds() * radiansPerSecond;

    // Uncomment the following line of code to oscillate the cube instead of
    // rotating it. Useful for testing different margin coefficients in the
    // pixel shader.

    //totalRotation = 9.f + cos(totalRotation) *.2;

    float animRadians = (float)fmod(totalRotation, XM_2PI);

    // Prepare to pass the view matrix, and updated model matrix, to the shader.
    XMStoreFloat4x4(&m_rotatedModelBufferData.model, XMMatrixTranspose(XMMatrixRotationY(animRadians)));

    // If the shadow dimension has changed, recreate it.
    D3D11_TEXTURE2D_DESC desc = { 0 };
    if (m_shadowMap != nullptr)
    {
        m_shadowMap->GetDesc(&desc);
        if (m_shadowMapDimension != desc.Height)
        {
            InitShadowMap();
        }
    }
}

// Renders one frame using the vertex and pixel shaders.
void ShadowSceneRenderer::Render()
{
    // Loading is asynchronous. Only draw geometry after it's loaded.
    if (!m_loadingComplete)
    {
        return;
    }

    auto context = m_deviceResources->GetD3DDeviceContext();

    // This sample demonstrates sample comparison in Direct3D feature levels
    // 9_1 and 9_3, so it doesn't try to render if the Direct3D device does
    // not support sample comparison in D3D_FEATURE_LEVEL_9_x shaders.
    //
    // For example, legacy devices that use WDDM 1.1 drivers do not support
    // sample comparison in D3D_FEATURE_LEVEL_9_x shaders.
    if (!m_deviceSupportsD3D9Shadows)
    {
        context->ClearRenderTargetView(m_deviceResources->GetBackBufferRenderTargetView(), DirectX::Colors::Black);
        return;
    }

    // Clear the back buffer and depth stencil view.
    context->ClearRenderTargetView(m_deviceResources->GetBackBufferRenderTargetView(), DirectX::Colors::CornflowerBlue);
    context->ClearDepthStencilView(m_shadowDepthView.Get(), D3D11_CLEAR_DEPTH | D3D11_CLEAR_STENCIL, 1.0f, 0);
    context->ClearDepthStencilView(m_deviceResources->GetDepthStencilView(), D3D11_CLEAR_DEPTH | D3D11_CLEAR_STENCIL, 1.0f, 0);

    // Update constant buffer data that has changed.
    m_deviceResources->GetD3DDeviceContext()->UpdateSubresource(
        m_rotatedModelBuffer.Get(),
        0,
        NULL,
        &m_rotatedModelBufferData,
        0,
        0
        );

    // Render the shadow buffer, then render the scene with shadows.
    RenderShadowMap();
    RenderSceneWithShadows();

    // For this example we display the shadow buffer as an overlay.
    RenderQuad();
}

// Loads the shadow buffer with shadow information.
void ShadowSceneRenderer::RenderShadowMap()
{
    auto context = m_deviceResources->GetD3DDeviceContext();

    // Ensure the depth buffer is not bound for input before using it as a render target.
    // If you don't do this, the Direct3D runtime will have to force an unbind which 
    // causes warnings in the debug output.
    ID3D11ShaderResourceView* nullSrv = nullptr;
    context->PSSetShaderResources(0, 1, &nullSrv);

    // Render all the objects in the scene that can cast shadows onto themselves or onto other objects.

    // Only bind the ID3D11DepthStencilView for output.
    context->OMSetRenderTargets(
        0,
        nullptr,
        m_shadowDepthView.Get()
        );

    // Set rendering state.
    context->RSSetState(m_shadowRenderState.Get());
    context->RSSetViewports(1, &m_shadowViewport);

    // Each vertex is one instance of the VertexPositionTexNormColor struct.
    UINT stride = sizeof(VertexPositionTexNormColor);
    UINT offset = 0;
    context->IASetVertexBuffers(
        0,
        1,
        m_vertexBuffer.GetAddressOf(),
        &stride,
        &offset
        );

    context->IASetIndexBuffer(
        m_indexBuffer.Get(),
        DXGI_FORMAT_R16_UINT, // Each index is one 16-bit unsigned integer (short).
        0
        );

    context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
    context->IASetInputLayout(m_inputLayout.Get());

    // Bind the vertex shader used to generate the depth map.
    context->VSSetShader(
        m_simpleVertexShader.Get(),
        nullptr,
        0
        );

    // Send the constant buffers to the Graphics device.
    context->VSSetConstantBuffers(
        0,
        1,
        m_lightViewProjectionBuffer.GetAddressOf()
        );

    context->VSSetConstantBuffers(
        1,
        1,
        m_rotatedModelBuffer.GetAddressOf()
        );

    // The vertex shader and rasterizer stages generate all the depth data that
    // we need. Set the pixel shader to null to disable the pixel shader and 
    // output-merger stages.
    ID3D11PixelShader* nullPS = nullptr;
    context->PSSetShader(
        nullPS,
        nullptr,
        0
        );

    // Draw the objects.
    context->DrawIndexed(
        m_indexCountCube,
        0,
        0
        );
}

// Render the objects in the scene that will have shadows cast onto them.
void ShadowSceneRenderer::RenderSceneWithShadows()
{
    auto context = m_deviceResources->GetD3DDeviceContext();

    // Set render targets to the screen.
    ID3D11RenderTargetView *const targets[1] = { m_deviceResources->GetBackBufferRenderTargetView() };
    context->OMSetRenderTargets(1, targets, m_deviceResources->GetDepthStencilView());

    // Set rendering state.
    context->RSSetState(m_drawingRenderState.Get());

    D3D11_VIEWPORT view = m_deviceResources->GetScreenViewport();
    context->RSSetViewports(1, &view);

    // Set up IA format for floor and cube.
    UINT stride = sizeof(VertexPositionTexNormColor);
    UINT offset = 0;
    context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
    context->IASetInputLayout(m_inputLayout.Get());

    // First, draw the floor.

    // Each vertex is one instance of the VertexPositionTexNormColor struct.
    context->IASetVertexBuffers(
        0,
        1,
        m_vertexBufferFloor.GetAddressOf(),
        &stride,
        &offset
        );

    context->IASetIndexBuffer(
        m_indexBufferFloor.Get(),
        DXGI_FORMAT_R16_UINT, // Each index is one 16-bit unsigned integer (short).
        0
        );

    // Attach our vertex shader.
    context->VSSetShader(
        m_vertexShader.Get(),
        nullptr,
        0
        );

    // Send the constant buffers to the Graphics device.
    context->VSSetConstantBuffers(
        0,
        1,
        m_cubeViewProjectionBuffer.GetAddressOf()
        );

    context->VSSetConstantBuffers(
        1,
        1,
        m_staticModelBuffer.GetAddressOf()
        );

    context->VSSetConstantBuffers(
        2,
        1,
        m_lightViewProjectionBuffer.GetAddressOf()
        );

    ID3D11PixelShader* pixelShader;
    ID3D11SamplerState** comparisonSampler;
    if (m_useLinear)
    {
        pixelShader = m_shadowPixelShader_linear.Get();
        comparisonSampler = m_comparisonSampler_linear.GetAddressOf();
    }
    else
    {
        pixelShader = m_shadowPixelShader_point.Get();
        comparisonSampler = m_comparisonSampler_point.GetAddressOf();
    }

    // Attach our pixel shader.
    context->PSSetShader(
        pixelShader,
        nullptr,
        0
        );

    // Send the camera position to the pixel shader.
    context->VSSetConstantBuffers(
        0,
        1,
        m_cubeViewProjectionBuffer.GetAddressOf()
        );

    context->PSSetSamplers(0, 1, comparisonSampler);
    context->PSSetShaderResources(0, 1, m_shadowResourceView.GetAddressOf());

    // Draw the floor.
    context->DrawIndexed(
        m_indexCountFloor,
        0,
        0
        );

    // Then draw the cube. We only need to change the pipeline state that's different.
    // Each vertex is one instance of the VertexPositionTexNormColor struct.
    context->IASetVertexBuffers(
        0,
        1,
        m_vertexBuffer.GetAddressOf(),
        &stride,
        &offset
        );

    context->IASetIndexBuffer(
        m_indexBuffer.Get(),
        DXGI_FORMAT_R16_UINT, // Each index is one 16-bit unsigned integer (short).
        0
        );

    // Set a different model matrix for rotating the cube.
    context->VSSetConstantBuffers(
        1,
        1,
        m_rotatedModelBuffer.GetAddressOf()
        );

    // Draw the cube.
    context->DrawIndexed(
        m_indexCountCube,
        0,
        0
        );
}

// Uses an ortho projection to render the shadow map as an overlay.
void ShadowSceneRenderer::RenderQuad()
{
    auto context = m_deviceResources->GetD3DDeviceContext();

    // Set render targets to the screen.
    ID3D11RenderTargetView *const targets[1] = { m_deviceResources->GetBackBufferRenderTargetView() };
    context->OMSetRenderTargets(1, targets, nullptr);
    context->RSSetState(m_drawingRenderState.Get());
    context->RSSetViewports(1, &m_deviceResources->GetScreenViewport());

    // Each vertex is one instance of the VertexPositionTexNormColor struct.
    UINT stride = sizeof(VertexPositionTexNormColor);
    UINT offset = 0;
    context->IASetVertexBuffers(
        0,
        1,
        m_vertexBufferQuad.GetAddressOf(),
        &stride,
        &offset
        );

    context->IASetIndexBuffer(
        m_indexBufferQuad.Get(),
        DXGI_FORMAT_R16_UINT, // Each index is one 16-bit unsigned integer (short).
        0
        );

    context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
    context->IASetInputLayout(m_inputLayout.Get());

    // Attach our vertex shader.
    context->VSSetShader(
        m_vertexShader.Get(),
        nullptr,
        0
        );

    // Send the constant buffer to the Graphics device.
    context->VSSetConstantBuffers(
        0,
        1,
        m_orthoViewProjectionBuffer.GetAddressOf()
        );

    context->VSSetConstantBuffers(
        1,
        1,
        m_orthoTransformBuffer.GetAddressOf()
        );

    context->VSSetConstantBuffers(
        2,
        1,
        m_lightViewProjectionBuffer.GetAddressOf()
        );

    ID3D11SamplerState** sampler;
    ID3D11PixelShader* shader;
    if (m_deviceResources->GetDeviceFeatureLevel() <= D3D_FEATURE_LEVEL_9_3)
    {
        // In feature level 9_1, we can't use a depth buffer with a non-comparison
        // sampler. So we display the stencil.
        // Note that 9_2 and 9_3 can work around that with a 16-bit stencil and a
        // texture copy - but this example doesn't do that.
        sampler = m_comparisonSampler_point.GetAddressOf();
        shader = m_comparisonShader.Get();
    }
    else
    {
        // In feature levels 10_0 and above, we can use the shadow buffer with
        // a non-comparison sampler and display varying depth as shading.
        sampler = m_linearSampler.GetAddressOf();
        shader = m_textureShader.Get();
    }

    // Attach our pixel shader.
    context->PSSetShader(
        shader,
        nullptr,
        0
        );

    context->PSSetSamplers(0, 1, sampler);
    context->PSSetShaderResources(0, 1, m_shadowResourceView.GetAddressOf());

    // Draw the quad.
    context->DrawIndexed(
        m_indexCountQuad,
        0,
        0
        );
}