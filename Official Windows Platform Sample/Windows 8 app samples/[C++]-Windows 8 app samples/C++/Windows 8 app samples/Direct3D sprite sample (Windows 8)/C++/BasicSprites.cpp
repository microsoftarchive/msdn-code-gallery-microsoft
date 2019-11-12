//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

// Warning: SpriteBatch performance is significantly decreased in
// the Debug configuration.  Release configuration is recommended
// for accurate performance assessment.

#include "pch.h"
#include "BasicSprites.h"
#include "DirectXSample.h"
#include "BasicLoader.h"

using namespace Microsoft::WRL;
using namespace BasicSprites;

SpriteBatch::SpriteBatch() :
    m_capacity(0)
{
}

void SpriteBatch::Initialize(
    _In_ ID3D11Device1* d3dDevice,
    _In_ int capacity = 1024
    )
{
    m_d3dDevice = d3dDevice;
    m_d3dDevice->GetImmediateContext1(&m_d3dContext);
    m_capacity = capacity;

    // Determine the technique that will be used to render the sprites.
    auto featureLevel = m_d3dDevice->GetFeatureLevel();

    if (featureLevel >= D3D_FEATURE_LEVEL_10_0)
    {
        // On DirectX 10+ devices, the Geometry Shader allows the sprite vertices to be
        // generated on the GPU, significantly reducing memory bandwidth requirements.
        m_technique = RenderTechnique::GeometryShader;
    }
    else if (featureLevel >= D3D_FEATURE_LEVEL_9_3)
    {
        // On DirectX 9.3+ devices, instancing allows shared sprite geometry with unique
        // per-sprite instance parameters, eliminating redundant data transfer.
        m_technique = RenderTechnique::Instancing;
    }
    else
    {
        // On devices that do not support Instancing, sprite vertex data must be replicated
        // in order to achieve the desired effect.
        m_technique = RenderTechnique::Replication;

        if (capacity > static_cast<int>(Parameters::MaximumCapacityCompatible))
        {
            // The index buffer format for feature-level 9.1 devices may only be 16 bits.
            // With 4 vertices per sprite, this allows a maximum of (1 << 16) / 4 sprites.
            throw ref new Platform::InvalidArgumentException();
        }
    }

    // Create the texture sampler.

    D3D11_SAMPLER_DESC samplerDesc;
    ZeroMemory(&samplerDesc, sizeof(samplerDesc));
    samplerDesc.Filter = D3D11_FILTER_MIN_MAG_MIP_LINEAR;
    samplerDesc.AddressU = D3D11_TEXTURE_ADDRESS_CLAMP;
    samplerDesc.AddressV = D3D11_TEXTURE_ADDRESS_CLAMP;
    samplerDesc.AddressW = D3D11_TEXTURE_ADDRESS_CLAMP;
    samplerDesc.MipLODBias = 0.0f;
    samplerDesc.MaxAnisotropy = 0;
    samplerDesc.ComparisonFunc = D3D11_COMPARISON_NEVER;
    samplerDesc.BorderColor[0] = 0.0f;
    samplerDesc.BorderColor[1] = 0.0f;
    samplerDesc.BorderColor[2] = 0.0f;
    samplerDesc.BorderColor[3] = 0.0f;
    samplerDesc.MinLOD = 0.0f;
    samplerDesc.MaxLOD = FLT_MAX;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateSamplerState(
            &samplerDesc,
            &m_linearSampler
            )
        );

    // Create the blend states.

    D3D11_BLEND_DESC1 blendDesc;
    ZeroMemory(&blendDesc, sizeof(blendDesc));
    blendDesc.AlphaToCoverageEnable = false;
    blendDesc.IndependentBlendEnable = false;
    blendDesc.RenderTarget[0].BlendEnable = true;
    blendDesc.RenderTarget[0].LogicOpEnable = false;
    blendDesc.RenderTarget[0].SrcBlend = D3D11_BLEND_SRC_ALPHA;
    blendDesc.RenderTarget[0].DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
    blendDesc.RenderTarget[0].BlendOp = D3D11_BLEND_OP_ADD;
    blendDesc.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_ONE;
    blendDesc.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_ONE;
    blendDesc.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;
    blendDesc.RenderTarget[0].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateBlendState1(
            &blendDesc,
            &m_blendStateAlpha
            )
        );

    blendDesc.RenderTarget[0].DestBlend = D3D11_BLEND_ONE;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateBlendState1(
            &blendDesc,
            &m_blendStateAdditive
            )
        );

    BasicLoader^ loader = ref new BasicLoader(m_d3dDevice.Get());

    if (m_technique == RenderTechnique::GeometryShader)
    {
        D3D11_INPUT_ELEMENT_DESC layoutDesc[] =
        {
            { "TRANSFORM", 0, DXGI_FORMAT_R32G32_FLOAT,   0, 0,  D3D11_INPUT_PER_INSTANCE_DATA, 1 },
            { "TRANSFORM", 1, DXGI_FORMAT_R32G32_FLOAT,   0, 8,  D3D11_INPUT_PER_INSTANCE_DATA, 1 },
            { "TRANSFORM", 2, DXGI_FORMAT_R32_FLOAT,      0, 16, D3D11_INPUT_PER_INSTANCE_DATA, 1 },
            { "COLOR",     0, DXGI_FORMAT_R8G8B8A8_UNORM, 0, 20, D3D11_INPUT_PER_INSTANCE_DATA, 1 }
        };
        loader->LoadShader(
            "BasicSprites.GeometryShader.vs.cso",
            layoutDesc,
            ARRAYSIZE(layoutDesc),
            &m_vertexShader,
            &m_inputLayout
            );
        loader->LoadShader(
            "BasicSprites.GeometryShader.gs.cso",
            &m_geometryShader
            );
    }
    else if (m_technique == RenderTechnique::Instancing)
    {
        D3D11_INPUT_ELEMENT_DESC layoutDesc[] =
        {
            { "POSITION",  0, DXGI_FORMAT_R32G32_FLOAT,   0, 0,  D3D11_INPUT_PER_VERTEX_DATA,   0 },
            { "TEXCOORD",  0, DXGI_FORMAT_R32G32_FLOAT,   0, 8,  D3D11_INPUT_PER_VERTEX_DATA,   0 },
            { "TRANSFORM", 0, DXGI_FORMAT_R32G32_FLOAT,   1, 0,  D3D11_INPUT_PER_INSTANCE_DATA, 1 },
            { "TRANSFORM", 1, DXGI_FORMAT_R32G32_FLOAT,   1, 8,  D3D11_INPUT_PER_INSTANCE_DATA, 1 },
            { "TRANSFORM", 2, DXGI_FORMAT_R32_FLOAT,      1, 16, D3D11_INPUT_PER_INSTANCE_DATA, 1 },
            { "COLOR",     0, DXGI_FORMAT_R8G8B8A8_UNORM, 1, 20, D3D11_INPUT_PER_INSTANCE_DATA, 1 }
        };
        loader->LoadShader(
            "BasicSprites.Instancing.vs.cso",
            layoutDesc,
            ARRAYSIZE(layoutDesc),
            &m_vertexShader,
            &m_inputLayout
            );
    }
    else if (m_technique == RenderTechnique::Replication)
    {
        D3D11_INPUT_ELEMENT_DESC layoutDesc[] =
        {
            { "POSITIONT", 0, DXGI_FORMAT_R32G32_FLOAT,   0, 0,  D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "TEXCOORD",  0, DXGI_FORMAT_R32G32_FLOAT,   0, 8,  D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "COLOR",     0, DXGI_FORMAT_R8G8B8A8_UNORM, 0, 16, D3D11_INPUT_PER_VERTEX_DATA, 0 }
        };
        loader->LoadShader(
            "BasicSprites.Replication.vs.cso",
            layoutDesc,
            ARRAYSIZE(layoutDesc),
            &m_vertexShader,
            &m_inputLayout
            );
    }

    loader->LoadShader(
        "BasicSprites.ps.cso",
        &m_pixelShader
        );

    // Create buffers.

    if (m_technique == RenderTechnique::GeometryShader)
    {
        // Create the instance data buffer.

        CD3D11_BUFFER_DESC instanceDataBufferDesc(
            m_capacity * sizeof(InstanceData),
            D3D11_BIND_VERTEX_BUFFER,
            D3D11_USAGE_DEFAULT,
            0
            );

        DX::ThrowIfFailed(
            m_d3dDevice->CreateBuffer(
                &instanceDataBufferDesc,
                nullptr,
                &m_instanceDataBuffer
                )
            );

        m_instanceData.reset(new InstanceData[m_capacity]);
    }
    else if (m_technique == RenderTechnique::Instancing)
    {
        // Create the vertex buffer.

        InstancingVertex vertexBufferData[] =
        {
            { float2(-1.0f,  1.0f), float2(0.0f, 0.0f) },
            { float2( 1.0f,  1.0f), float2(1.0f, 0.0f) },
            { float2(-1.0f, -1.0f), float2(0.0f, 1.0f) },
            { float2( 1.0f, -1.0f), float2(1.0f, 1.0f) }
        };

        D3D11_SUBRESOURCE_DATA vertexInitialData;
        vertexInitialData.pSysMem = vertexBufferData;
        vertexInitialData.SysMemPitch = 0;
        vertexInitialData.SysMemSlicePitch = 0;

        CD3D11_BUFFER_DESC vertexBufferDesc(
            sizeof(vertexBufferData),
            D3D11_BIND_VERTEX_BUFFER,
            D3D11_USAGE_DEFAULT,
            0
            );

        DX::ThrowIfFailed(
            m_d3dDevice->CreateBuffer(
                &vertexBufferDesc,
                &vertexInitialData,
                &m_vertexBuffer
                )
            );

        // Create the instance data buffer.

        CD3D11_BUFFER_DESC instanceDataBufferDesc(
            m_capacity * sizeof(InstanceData),
            D3D11_BIND_VERTEX_BUFFER,
            D3D11_USAGE_DEFAULT,
            0
            );

        DX::ThrowIfFailed(
            m_d3dDevice->CreateBuffer(
                &instanceDataBufferDesc,
                nullptr,
                &m_instanceDataBuffer
                )
            );

        m_instanceData.reset(new InstanceData[m_capacity]);

        // Create the index buffer.

        unsigned int indexBufferData[] =
        {
            0, 1, 2,
            1, 3, 2
        };

        D3D11_SUBRESOURCE_DATA indexInitialData;
        indexInitialData.pSysMem = indexBufferData;
        indexInitialData.SysMemPitch = 0;
        indexInitialData.SysMemSlicePitch = 0;

        CD3D11_BUFFER_DESC indexBufferDesc(
            sizeof(indexBufferData),
            D3D11_BIND_INDEX_BUFFER,
            D3D11_USAGE_DEFAULT,
            0
            );

        DX::ThrowIfFailed(
            m_d3dDevice->CreateBuffer(
                &indexBufferDesc,
                &indexInitialData,
                &m_indexBuffer
                )
            );
    }
    else if (m_technique == RenderTechnique::Replication)
    {
        // Create the vertex buffer.

        CD3D11_BUFFER_DESC vertexBufferDesc(
            m_capacity * 4 * sizeof(ReplicationVertex),
            D3D11_BIND_VERTEX_BUFFER,
            D3D11_USAGE_DEFAULT,
            0
            );

        DX::ThrowIfFailed(
            m_d3dDevice->CreateBuffer(
                &vertexBufferDesc,
                nullptr,
                &m_vertexBuffer
                )
            );

        m_vertexData.reset(new ReplicationVertex[m_capacity * 4]);

        // Create the index buffer.

        std::unique_ptr<unsigned short[]> indexBufferData(new unsigned short[m_capacity * 6]);

        for (int i = 0; i < m_capacity; i++)
        {
            indexBufferData[i * 6 + 0] = i * 4 + 0;
            indexBufferData[i * 6 + 1] = i * 4 + 1;
            indexBufferData[i * 6 + 2] = i * 4 + 2;
            indexBufferData[i * 6 + 3] = i * 4 + 1;
            indexBufferData[i * 6 + 4] = i * 4 + 3;
            indexBufferData[i * 6 + 5] = i * 4 + 2;
        }

        D3D11_SUBRESOURCE_DATA initialData;
        initialData.pSysMem = indexBufferData.get();
        initialData.SysMemPitch = 0;
        initialData.SysMemSlicePitch = 0;

        CD3D11_BUFFER_DESC indexBufferDesc(
            m_capacity * 6 * sizeof(unsigned short),
            D3D11_BIND_INDEX_BUFFER,
            D3D11_USAGE_DEFAULT,
            0
            );

        DX::ThrowIfFailed(
            m_d3dDevice->CreateBuffer(
                &indexBufferDesc,
                &initialData,
                &m_indexBuffer
                )
            );
    }

    if (m_technique == RenderTechnique::GeometryShader || m_technique == RenderTechnique::Instancing)
    {
        // Both the Geometry Shader and Instancing techniques scale geometry in shader code.
        // As a result, they require information about the render target size.

        CD3D11_BUFFER_DESC renderTargetInfoCbufferDesc(
            16, // Constant buffer sizes must be a multiple of 16 bytes.  16 is sufficient for the required float2 data.
            D3D11_BIND_CONSTANT_BUFFER,
            D3D11_USAGE_DYNAMIC,
            D3D11_CPU_ACCESS_WRITE
            );

        DX::ThrowIfFailed(
            m_d3dDevice->CreateBuffer(
                &renderTargetInfoCbufferDesc,
                nullptr,
                &m_renderTargetInfoCbuffer
                )
            );
    }
}

void SpriteBatch::AddTexture(
    _In_ ID3D11Texture2D* texture
    )
{
    TextureMapElement mapElement;
    CD3D11_SHADER_RESOURCE_VIEW_DESC mapElementSrvDesc(
        texture,
        D3D11_SRV_DIMENSION_TEXTURE2D
        );
    DX::ThrowIfFailed(
        m_d3dDevice->CreateShaderResourceView(
            texture,
            &mapElementSrvDesc,
            &mapElement.srv
            )
        );

    D3D11_TEXTURE2D_DESC textureDesc;
    texture->GetDesc(&textureDesc);
    mapElement.size = float2(
        static_cast<float>(textureDesc.Width),
        static_cast<float>(textureDesc.Height)
        );

    m_textureMap[texture] = mapElement;
}

void SpriteBatch::RemoveTexture(
    _In_ ID3D11Texture2D* texture
    )
{
    m_textureMap.erase(texture);
}

void SpriteBatch::Begin()
{
    // Reset internal sprite data.

    m_numSpritesDrawn = 0;
    m_spritesInRun = 0;
    m_spriteRuns.clear();

    // Get the current render target dimensions and logical DPI.

    ComPtr<ID3D11RenderTargetView> renderTargetView;
    m_d3dContext->OMGetRenderTargets(
        1,
        &renderTargetView,
        nullptr
        );

    ComPtr<ID3D11Resource> renderTarget;
    renderTargetView->GetResource(&renderTarget);

    ComPtr<ID3D11Texture2D> renderTargetTexture;
    renderTarget.As(&renderTargetTexture);

    D3D11_TEXTURE2D_DESC renderTargetTextureDesc;
    renderTargetTexture->GetDesc(&renderTargetTextureDesc);

    m_renderTargetSize = float2(
        static_cast<float>(renderTargetTextureDesc.Width),
        static_cast<float>(renderTargetTextureDesc.Height)
        );

    m_dpi = Windows::Graphics::Display::DisplayProperties::LogicalDpi;
}

void SpriteBatch::End()
{
    // If no sprites were drawn, do nothing.
    if (m_numSpritesDrawn == 0)
    {
        return;
    }

    // Save the final sprite run info.

    SpriteRunInfo runInfo;
    runInfo.textureView = m_currentTextureView;
    runInfo.blendState = m_currentBlendState;
    runInfo.numSprites = m_spritesInRun;
    m_spriteRuns.push_back(runInfo);

    // Update the buffer data.

    if (m_technique == RenderTechnique::GeometryShader || m_technique == RenderTechnique::Instancing)
    {
        CD3D11_BOX instanceDataBox(
            0,
            0,
            0,
            sizeof(InstanceData) * m_numSpritesDrawn,
            1,
            1
            );

        m_d3dContext->UpdateSubresource(
            m_instanceDataBuffer.Get(),
            0,
            &instanceDataBox,
            m_instanceData.get(),
            0,
            0
            );
    }
    else if (m_technique == RenderTechnique::Replication)
    {
        CD3D11_BOX vertexDataBox(
            0,
            0,
            0,
            sizeof(ReplicationVertex) * m_numSpritesDrawn * 4,
            1,
            1
            );

        m_d3dContext->UpdateSubresource(
            m_vertexBuffer.Get(),
            0,
            &vertexDataBox,
            m_vertexData.get(),
            0,
            0
            );
    }

    if (m_technique == RenderTechnique::GeometryShader || m_technique == RenderTechnique::Instancing)
    {
        D3D11_MAPPED_SUBRESOURCE mappedSubresource;
        m_d3dContext->Map(
            m_renderTargetInfoCbuffer.Get(),
            0,
            D3D11_MAP_WRITE_DISCARD,
            0,
            &mappedSubresource
            );
        *static_cast<float2*>(mappedSubresource.pData) = m_renderTargetSize;
        m_d3dContext->Unmap(
            m_renderTargetInfoCbuffer.Get(),
            0
            );
    }

    // Set the pipeline state

    if (m_technique == RenderTechnique::Instancing)
    {
        m_d3dContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
        m_d3dContext->IASetIndexBuffer(
            m_indexBuffer.Get(),
            DXGI_FORMAT_R32_UINT,
            0
            );
    }
    else if (m_technique == RenderTechnique::Replication)
    {
        m_d3dContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
        m_d3dContext->IASetIndexBuffer(
            m_indexBuffer.Get(),
            DXGI_FORMAT_R16_UINT,
            0
            );
    }
    else if (m_technique == RenderTechnique::GeometryShader)
    {
        m_d3dContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_POINTLIST);
        m_d3dContext->GSSetShader(
            m_geometryShader.Get(),
            nullptr,
            0
            );
    }

    m_d3dContext->IASetInputLayout(m_inputLayout.Get());

    m_d3dContext->VSSetShader(
        m_vertexShader.Get(),
        nullptr,
        0
        );

    if (m_technique == RenderTechnique::GeometryShader)
    {
        m_d3dContext->GSSetConstantBuffers(
            0,
            1,
            m_renderTargetInfoCbuffer.GetAddressOf()
            );
    }
    else if (m_technique == RenderTechnique::Instancing)
    {
        m_d3dContext->VSSetConstantBuffers(
            0,
            1,
            m_renderTargetInfoCbuffer.GetAddressOf()
            );
    }

    m_d3dContext->PSSetShader(
        m_pixelShader.Get(),
        nullptr,
        0
        );

    m_d3dContext->PSSetSamplers(
        0,
        1,
        m_linearSampler.GetAddressOf()
        );

    if (m_technique == RenderTechnique::GeometryShader)
    {
        unsigned int stride = sizeof(InstanceData);
        unsigned int offset = 0;
        m_d3dContext->IASetVertexBuffers(
            0,
            1,
            m_instanceDataBuffer.GetAddressOf(),
            &stride,
            &offset
            );
    }
    else if (m_technique == RenderTechnique::Instancing)
    {
        unsigned int stride = sizeof(InstancingVertex);
        unsigned int offset = 0;
        m_d3dContext->IASetVertexBuffers(
            0,
            1,
            m_vertexBuffer.GetAddressOf(),
            &stride,
            &offset
            );
    }
    else if (m_technique == RenderTechnique::Replication)
    {
        unsigned int stride = sizeof(ReplicationVertex);
        unsigned int offset = 0;
        m_d3dContext->IASetVertexBuffers(
            0,
            1,
            m_vertexBuffer.GetAddressOf(),
            &stride,
            &offset
            );
    }

    // Draw each sprite run

    unsigned int indexBase = 0;
    for (auto runIterator = m_spriteRuns.begin(); runIterator != m_spriteRuns.end(); runIterator++)
    {
        m_d3dContext->PSSetShaderResources(
            0,
            1,
            &runIterator->textureView
            );

        const FLOAT blendFactor[] = {0.0f, 0.0f, 0.0f, 0.0f};

        m_d3dContext->OMSetBlendState(
            runIterator->blendState,
            nullptr,
            0xFFFFFFFF
            );

        if (m_technique == RenderTechnique::GeometryShader)
        {
            unsigned int instancesToDraw = runIterator->numSprites;
            m_d3dContext->DrawInstanced(
                1,
                instancesToDraw,
                0,
                indexBase
                );
            indexBase += instancesToDraw;
        }
        else if (m_technique == RenderTechnique::Instancing)
        {
            unsigned int instancesToDraw = runIterator->numSprites;
            unsigned int stride = sizeof(InstanceData);
            unsigned int offset = indexBase * stride;
            // Instance data offset must be zero for the draw call on feature level 9.3 and below.
            // Instead, set the offset in the input assembler.
            m_d3dContext->IASetVertexBuffers(
                1,
                1,
                m_instanceDataBuffer.GetAddressOf(),
                &stride,
                &offset
                );
            m_d3dContext->DrawIndexedInstanced(
                6,
                instancesToDraw,
                0,
                0,
                0
                );
            indexBase += instancesToDraw;
        }
        else if (m_technique == RenderTechnique::Replication)
        {
            unsigned int indicesToDraw = runIterator->numSprites * 6;
            m_d3dContext->DrawIndexed(indicesToDraw, indexBase, 0);
            indexBase += indicesToDraw;
        }
    }
}

unsigned int SpriteBatch::MakeUnorm(float4 color)
{
    unsigned int r = max(0, min(255, static_cast<unsigned int>(color.r * 255.0f)));
    unsigned int g = max(0, min(255, static_cast<unsigned int>(color.g * 255.0f)));
    unsigned int b = max(0, min(255, static_cast<unsigned int>(color.b * 255.0f)));
    unsigned int a = max(0, min(255, static_cast<unsigned int>(color.a * 255.0f)));
    return
        (a << 24) |
        (b << 16) |
        (g << 8) |
        r;
}

float2 SpriteBatch::StandardOrigin(float2 position, PositionUnits positionUnits, float2 renderTargetSize, float dpi)
{
    float2 origin;
    if (positionUnits == PositionUnits::Pixels)
    {
        origin.x = (position.x / renderTargetSize.x) * 2.0f - 1.0f;
        origin.y = 1.0f - (position.y / renderTargetSize.y) * 2.0f;
    }
    else if (positionUnits == PositionUnits::DIPs)
    {
        origin.x = ((position.x * dpi / 96.0f) / renderTargetSize.x) * 2.0f - 1.0f;
        origin.y = 1.0f - ((position.y * dpi / 96.0f) / renderTargetSize.y) * 2.0f;
    }
    else if (positionUnits == PositionUnits::Normalized)
    {
        origin.x = position.x * 2.0f - 1.0f;
        origin.y = 1.0f - position.y * 2.0f;
    }
    else if (positionUnits == PositionUnits::UniformWidth)
    {
        origin.x = position.x * 2.0f - 1.0f;
        origin.y = 1.0f - position.y * (renderTargetSize.x / renderTargetSize.y) * 2.0f;
    }
    else if (positionUnits == PositionUnits::UniformHeight)
    {
        origin.x = position.x * (renderTargetSize.y / renderTargetSize.x) * 2.0f - 1.0f;
        origin.y = 1.0f - position.y * 2.0f;
    }
    return origin;
}

float2 SpriteBatch::StandardOffset(float2 size, SizeUnits sizeUnits, float2 spriteSize, float dpi)
{
    float2 offset;
    if (sizeUnits == SizeUnits::Pixels)
    {
        offset = size;
    }
    else if (sizeUnits == SizeUnits::DIPs)
    {
        offset = size * dpi / 96.0f;
    }
    else if (sizeUnits == SizeUnits::Normalized)
    {
        offset = spriteSize * size;
    }
    return offset;
}

void SpriteBatch::Draw(
    _In_ ID3D11Texture2D* texture,
    _In_ float2 position,
    _In_ PositionUnits positionUnits = PositionUnits::DIPs
    )
{
    Draw(
        texture,
        position,
        positionUnits,
        float2(1.0f, 1.0f),
        SizeUnits::Normalized
        );
}

void SpriteBatch::Draw(
    _In_ ID3D11Texture2D* texture,
    _In_ float2 position,
    _In_ PositionUnits positionUnits,
    _In_ float2 size,
    _In_ SizeUnits sizeUnits
    )
{
    Draw(
        texture,
        position,
        positionUnits,
        size,
        sizeUnits,
        float4(1.0f, 1.0f, 1.0f, 1.0f)
        );
}

void SpriteBatch::Draw(
    _In_ ID3D11Texture2D* texture,
    _In_ float2 position,
    _In_ PositionUnits positionUnits,
    _In_ float2 size,
    _In_ SizeUnits sizeUnits,
    _In_ float4 color
    )
{
    Draw(
        texture,
        position,
        positionUnits,
        size,
        sizeUnits,
        color,
        0.0f
        );
}

void SpriteBatch::Draw(
    _In_ ID3D11Texture2D* texture,
    _In_ float2 position,
    _In_ PositionUnits positionUnits,
    _In_ float2 size,
    _In_ SizeUnits sizeUnits,
    _In_ float4 color,
    _In_ float rotation
    )
{
    Draw(
        texture,
        position,
        positionUnits,
        size,
        sizeUnits,
        color,
        rotation,
        BlendMode::Alpha
        );
}

void SpriteBatch::Draw(
    _In_ ID3D11Texture2D* texture,
    _In_ float2 position,
    _In_ PositionUnits positionUnits,
    _In_ float2 size,
    _In_ SizeUnits sizeUnits,
    _In_ float4 color,
    _In_ float rotation,
    _In_ BlendMode blendMode
    )
{
    // Fail if drawing this sprite would exceed the capacity of the sprite batch.
    if (m_numSpritesDrawn >= m_capacity)
    {
        throw ref new Platform::OutOfBoundsException();
    }

    // Retrieve information about the sprite.
    TextureMapElement element = m_textureMap[texture];
    ID3D11ShaderResourceView* textureView = element.srv.Get();
    float2 textureSize = element.size;
    ID3D11BlendState1* blendState = blendMode == BlendMode::Additive ? m_blendStateAdditive.Get() : m_blendStateAlpha.Get();

    // Fail if the texture has not previously been added to the sprite batch.
    if (textureView == nullptr)
    {
        throw ref new Platform::NullReferenceException();
    }

    // Unless this is the first sprite run, save out the previous run info if a new run is required.
    if (
        m_numSpritesDrawn > 0 && (
            textureView != m_currentTextureView ||
            blendState != m_currentBlendState
            )
        )
    {
        SpriteRunInfo runInfo;
        runInfo.textureView = m_currentTextureView;
        runInfo.blendState = m_currentBlendState;
        runInfo.numSprites = m_spritesInRun;
        m_spriteRuns.push_back(runInfo);
        m_spritesInRun = 0; // Reset for the next sprite run.
    }
    m_currentTextureView = textureView;
    m_currentBlendState = blendState;

    // Add the sprite to the buffer.

    float2 origin = StandardOrigin(position, positionUnits, m_renderTargetSize, m_dpi);
    float2 offset = StandardOffset(size, sizeUnits, textureSize, m_dpi);

    if (m_technique == RenderTechnique::GeometryShader || m_technique == RenderTechnique::Instancing)
    {
        m_instanceData[m_numSpritesDrawn].origin = origin;
        m_instanceData[m_numSpritesDrawn].offset = offset;
        m_instanceData[m_numSpritesDrawn].rotation = rotation;
        m_instanceData[m_numSpritesDrawn].color = MakeUnorm(color);
    }
    else if (m_technique == RenderTechnique::Replication)
    {
        float2 offsets[4] =
        {
            float2(-offset.x,  offset.y),
            float2( offset.x,  offset.y),
            float2(-offset.x, -offset.y),
            float2( offset.x, -offset.y)
        };

        float sinRotation = sinf(rotation);
        float cosRotation = cosf(rotation);

        for (int i = 0; i < 4; i++)
        {
            offsets[i] = float2(
                offsets[i].x * cosRotation - offsets[i].y * sinRotation,
                offsets[i].x * sinRotation + offsets[i].y * cosRotation
                );
            offsets[i].x /= m_renderTargetSize.x;
            offsets[i].y /= m_renderTargetSize.y;
        }

        // Write vertex buffer data.

        ReplicationVertex* singleSpriteVertices = &m_vertexData[m_numSpritesDrawn * 4];
        unsigned int colorUnorm = MakeUnorm(color);
        singleSpriteVertices[0].pos = origin + offsets[0];
        singleSpriteVertices[1].pos = origin + offsets[1];
        singleSpriteVertices[2].pos = origin + offsets[2];
        singleSpriteVertices[3].pos = origin + offsets[3];
        singleSpriteVertices[0].color = colorUnorm;
        singleSpriteVertices[1].color = colorUnorm;
        singleSpriteVertices[2].color = colorUnorm;
        singleSpriteVertices[3].color = colorUnorm;
        singleSpriteVertices[0].tex = float2(0.0f, 0.0f);
        singleSpriteVertices[1].tex = float2(1.0f, 0.0f);
        singleSpriteVertices[2].tex = float2(0.0f, 1.0f);
        singleSpriteVertices[3].tex = float2(1.0f, 1.0f);
    }

    m_spritesInRun++;
    m_numSpritesDrawn++;
}
