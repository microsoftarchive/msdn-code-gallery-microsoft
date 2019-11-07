//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXFractal.h"
#include "BasicLoader.h"

using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Core;

DirectXFractal::DirectXFractal()
{
    m_transform.scale = 4.0f;
    m_transform.rotation = 0.0f;
    m_transform.translationX = -0.6f;
    m_transform.translationY = 0.0f;
}

void DirectXFractal::HandleDeviceLost()
{
    // Release window size-dependent resources prior to creating a new device and swap chain.
    m_tempRTV[0] = nullptr;
    m_tempRTV[1] = nullptr;
    m_tempUAV = nullptr;
    m_tempSRV[0] = nullptr;
    m_tempSRV[1] = nullptr;
    m_finalRTV = nullptr;
    m_finalSRV = nullptr;

    DirectXBase::HandleDeviceLost();
}

void DirectXFractal::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    // Create the performance throttler.

    m_movingThrottle = ref new AutoThrottle(SampleSettings::Performance::TargetFrameTime);
}

void DirectXFractal::DetermineFractalTechnique()
{
    if (m_featureLevel >= D3D_FEATURE_LEVEL_11_0)
    {
        // DirectCompute is always available on 11.0 and higher feature levels.
        m_technique = FractalTechnique::DirectCompute;

        // Additionally, double-precision is optionally available.  Where available,
        // this will enable deeper zoom into the fractal.
        D3D11_FEATURE_DATA_DOUBLES doubleFeatureData;
            DX::ThrowIfFailed(
                m_d3dDevice->CheckFeatureSupport(
                    D3D11_FEATURE_DOUBLES,
                    &doubleFeatureData,
                    sizeof(doubleFeatureData)
                    )
                );
        m_useDoubles = (doubleFeatureData.DoublePrecisionFloatShaderOps == TRUE);
    }
    else if (m_featureLevel >= D3D_FEATURE_LEVEL_10_0)
    {
        // Feature level 10.0 and 10.1 can optionally support DirectCompute.
        D3D11_FEATURE_DATA_D3D10_X_HARDWARE_OPTIONS computeFeatureData;
        DX::ThrowIfFailed(
            m_d3dDevice->CheckFeatureSupport(
                D3D11_FEATURE_D3D10_X_HARDWARE_OPTIONS,
                &computeFeatureData,
                sizeof(computeFeatureData)
                )
            );

        if (computeFeatureData.ComputeShaders_Plus_RawAndStructuredBuffers_Via_Shader_4_x)
        {
            m_technique = FractalTechnique::DirectCompute;
        }
        else
        {
            m_technique = FractalTechnique::SinglePass;
        }
    }
    else
    {
        // The instruction count and register limitations of
        // feature level 9 shaders require multi-pass generation.
        m_technique = FractalTechnique::ThreePhase;

        // Additionally, on feature level 9.1, only a handful of specific
        // texture formats are available for use as render targets.
        m_useUnormTextures = m_featureLevel < D3D_FEATURE_LEVEL_9_3;
    }
}

void DirectXFractal::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    DetermineFractalTechnique();

    // Create vertex and index buffers.
    // These are sufficiently general that they can be used with all fractal generation techniques.

    FractalVertex vertices[] =
    {
        { float2(-1.0f,  1.0f), float2(0.0f, 0.0f), float2(-0.5f,  0.5f) },
        { float2( 1.0f,  1.0f), float2(1.0f, 0.0f), float2( 0.5f,  0.5f) },
        { float2(-1.0f, -1.0f), float2(0.0f, 1.0f), float2(-0.5f, -0.5f) },
        { float2( 1.0f, -1.0f), float2(1.0f, 1.0f), float2( 0.5f, -0.5f) },
    };

    D3D11_SUBRESOURCE_DATA vertexBufferData = {0};
    vertexBufferData.pSysMem = vertices;
    vertexBufferData.SysMemPitch = 0;
    vertexBufferData.SysMemSlicePitch = 0;
    CD3D11_BUFFER_DESC vertexBufferDesc(sizeof(vertices), D3D11_BIND_VERTEX_BUFFER);
    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &vertexBufferDesc,
            &vertexBufferData,
            &m_vertexBuffer
            )
        );

    uint16 indices[] =
    {
        0, 1, 2,
        1, 3, 2,
    };
    m_indexCount = ARRAYSIZE(indices);

    D3D11_SUBRESOURCE_DATA indexBufferData = {0};
    indexBufferData.pSysMem = indices;
    indexBufferData.SysMemPitch = 0;
    indexBufferData.SysMemSlicePitch = 0;
    CD3D11_BUFFER_DESC indexBufferDesc(sizeof(indices), D3D11_BIND_INDEX_BUFFER);
    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &indexBufferDesc,
            &indexBufferData,
            &m_indexBuffer
            )
        );

    // Load all shaders and textures from the package.

    BasicLoader^ loader = ref new BasicLoader(m_d3dDevice.Get());

    D3D11_INPUT_ELEMENT_DESC inputLayoutDesc[] =
    {
        { "POSITION", 0, DXGI_FORMAT_R32G32_FLOAT, 0,  0, D3D11_INPUT_PER_VERTEX_DATA, 0 },
        { "TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT, 0,  8, D3D11_INPUT_PER_VERTEX_DATA, 0 },
        { "TEXCOORD", 1, DXGI_FORMAT_R32G32_FLOAT, 0, 16, D3D11_INPUT_PER_VERTEX_DATA, 0 },
    };

    loader->LoadShader(
        "DrawRect.vs.cso",
        inputLayoutDesc,
        ARRAYSIZE(inputLayoutDesc),
        &m_vertexShader,
        &m_inputLayout
        );

    loader->LoadTexture(
        "gradient.dds",
        nullptr,
        &m_gradient
        );

    if (m_technique == FractalTechnique::ThreePhase)
    {
        loader->LoadShader(
            m_useUnormTextures ? "Initial.unorm.ps.cso" : "Initial.ps.cso",
            &m_initialPS
            );

        loader->LoadShader(
            m_useUnormTextures ? "Step.unorm.ps.cso" : "Step.ps.cso",
            &m_stepPS
            );

        loader->LoadShader(
            m_useUnormTextures ? "Final.unorm.ps.cso" : "Final.ps.cso",
            &m_finalPS
            );

        loader->LoadShader(
            "RenderTexture.ps.cso",
            &m_renderTexturePS
            );
    }

    if (m_technique == FractalTechnique::SinglePass)
    {
        loader->LoadShader(
            "SinglePass.ps.cso",
            &m_singlePassPS
            );

        loader->LoadShader(
            "RenderTexture.ps.cso",
            &m_renderTexturePS
            );
    }

    if (m_technique == FractalTechnique::DirectCompute)
    {
        loader->LoadShader(
            m_useDoubles ? "DirectCompute.double.cs.cso" : "DirectCompute.cs.cso",
            &m_directComputeCS
            );

        loader->LoadShader(
            "RenderBuffer.ps.cso",
            &m_renderBufferPS
            );
    }

    // Create constant buffers.

    CD3D11_BUFFER_DESC drawRectCBDesc(
        RoundUpTo16(sizeof(DrawRectCB)),
        D3D11_BIND_CONSTANT_BUFFER,
        D3D11_USAGE_DYNAMIC,
        D3D11_CPU_ACCESS_WRITE
        );

    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &drawRectCBDesc,
            nullptr,
            &m_drawRectCB
            )
        );

    if (m_technique == FractalTechnique::DirectCompute)
    {
        CD3D11_BUFFER_DESC computeCBDesc(
            RoundUpTo16(
                m_useDoubles ?
                sizeof(ComputeDoubleCB) :
                sizeof(ComputeCB)
                ),
            D3D11_BIND_CONSTANT_BUFFER,
            D3D11_USAGE_DYNAMIC,
            D3D11_CPU_ACCESS_WRITE
            );

        DX::ThrowIfFailed(
            m_d3dDevice->CreateBuffer(
                &computeCBDesc,
                nullptr,
                &m_computeCB
                )
            );

        CD3D11_BUFFER_DESC bufferInfoCBDesc(
            RoundUpTo16(sizeof(int)),
            D3D11_BIND_CONSTANT_BUFFER,
            D3D11_USAGE_DYNAMIC,
            D3D11_CPU_ACCESS_WRITE
            );

        DX::ThrowIfFailed(
            m_d3dDevice->CreateBuffer(
                &bufferInfoCBDesc,
                nullptr,
                &m_bufferInfoCB
                )
            );
    }

    // Create texture samplers.

    D3D11_SAMPLER_DESC samplerDesc;
    ZeroMemory(&samplerDesc, sizeof(D3D11_SAMPLER_DESC));
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
    samplerDesc.MinLOD = 0;
    samplerDesc.MaxLOD = D3D11_FLOAT32_MAX;

    samplerDesc.Filter = D3D11_FILTER_MIN_MAG_MIP_LINEAR;
    DX::ThrowIfFailed(
        m_d3dDevice->CreateSamplerState(
            &samplerDesc,
            &m_linearSampler
            )
        );

    if (m_technique != FractalTechnique::DirectCompute)
    {
        samplerDesc.Filter = D3D11_FILTER_MIN_MAG_MIP_POINT;
        DX::ThrowIfFailed(
            m_d3dDevice->CreateSamplerState(
                &samplerDesc,
                &m_pointSampler
                )
            );
    }

    // Create the Sample Overlay.

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "DirectX and DirectCompute fractal generator sample"
        );
}

void DirectXFractal::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // Set the initial size of the intermediate render target.
    // This will be automatically refined based on performace measurements.
    m_downSampleRatio = SampleSettings::Performance::InitialDownSampleRatio;

    // Force a regeneration of the fractal.
    m_fastGenerate = true;
    m_complete = false;

    if (m_technique == FractalTechnique::DirectCompute)
    {
        // Create a buffer to save completed fractal calculations.
        m_bufferWidth =
            ((static_cast<int>(m_renderTargetSize.Width) + SampleSettings::DirectCompute::ThreadGroupSizeX - 1) /
            SampleSettings::DirectCompute::ThreadGroupSizeX) *
            SampleSettings::DirectCompute::ThreadGroupSizeX; // round up elements per row to match group dimension
        m_bufferHeight = static_cast<int>(m_renderTargetSize.Height);

        ComPtr<ID3D11Buffer> tempBuffer;
        CD3D11_BUFFER_DESC tempBufferDesc(
            sizeof(FractalBufferElement) * m_bufferWidth * m_bufferHeight,
            D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_UNORDERED_ACCESS,
            D3D11_USAGE_DEFAULT,
            0,
            D3D11_RESOURCE_MISC_BUFFER_STRUCTURED,
            sizeof(FractalBufferElement)
            );
        DX::ThrowIfFailed(
            m_d3dDevice->CreateBuffer(
                &tempBufferDesc,
                nullptr,
                &tempBuffer
                )
            );

        CD3D11_SHADER_RESOURCE_VIEW_DESC tempSRVDesc(
            tempBuffer.Get(),
            DXGI_FORMAT_UNKNOWN,
            0,
            m_bufferWidth * m_bufferHeight
            );
        DX::ThrowIfFailed(
            m_d3dDevice->CreateShaderResourceView(
                tempBuffer.Get(),
                &tempSRVDesc,
                &m_tempSRV[0]
                )
            );

        CD3D11_UNORDERED_ACCESS_VIEW_DESC tempUAVDesc(
            tempBuffer.Get(),
            DXGI_FORMAT_UNKNOWN,
            0,
            m_bufferWidth * m_bufferHeight
            );
        DX::ThrowIfFailed(
            m_d3dDevice->CreateUnorderedAccessView(
                tempBuffer.Get(),
                &tempUAVDesc,
                &m_tempUAV
                )
            );

        // Update the buffer info constant buffer.

        D3D11_MAPPED_SUBRESOURCE mappedResource;
        DX::ThrowIfFailed(
            m_d3dContext->Map(
                m_bufferInfoCB.Get(),
                0,
                D3D11_MAP_WRITE_DISCARD,
                0,
                &mappedResource
                )
            );

        *static_cast<int*>(mappedResource.pData) = m_bufferWidth;

        m_d3dContext->Unmap(
            m_bufferInfoCB.Get(),
            0
            );
    }

    if (m_technique == FractalTechnique::ThreePhase)
    {
        // Create two intermediate textures to save partially completed fractal calculations.
        for (int i = 0; i < 2; i++)
        {
            ComPtr<ID3D11Texture2D> tempTexture;
            CD3D11_TEXTURE2D_DESC tempTextureDesc(
                m_useUnormTextures ?
                    DXGI_FORMAT_R8G8B8A8_UNORM :
                    DXGI_FORMAT_R16G16B16A16_FLOAT,
                static_cast<UINT>(m_renderTargetSize.Width),
                static_cast<UINT>(m_renderTargetSize.Height),
                1,
                1,
                D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_RENDER_TARGET
                );
            DX::ThrowIfFailed(
                m_d3dDevice->CreateTexture2D(
                    &tempTextureDesc,
                    nullptr,
                    &tempTexture
                    )
                );

            CD3D11_RENDER_TARGET_VIEW_DESC tempRTVDesc(
                tempTexture.Get(),
                D3D11_RTV_DIMENSION_TEXTURE2D
                );
            DX::ThrowIfFailed(
                m_d3dDevice->CreateRenderTargetView(
                    tempTexture.Get(),
                    &tempRTVDesc,
                    &m_tempRTV[i]
                    )
                );

            CD3D11_SHADER_RESOURCE_VIEW_DESC tempSRVDesc(
                tempTexture.Get(),
                D3D11_SRV_DIMENSION_TEXTURE2D
                );
            DX::ThrowIfFailed(
                m_d3dDevice->CreateShaderResourceView(
                    tempTexture.Get(),
                    &tempSRVDesc,
                    &m_tempSRV[i]
                    )
                );
        }
    }

    if (m_technique != FractalTechnique::DirectCompute)
    {
        // Create a texture to save the full-resolution fractal once generated.

        CD3D11_TEXTURE2D_DESC tempTextureDesc(
            DXGI_FORMAT_R8G8B8A8_UNORM,
            static_cast<UINT>(m_renderTargetSize.Width),
            static_cast<UINT>(m_renderTargetSize.Height),
            1,
            1,
            D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_RENDER_TARGET
            );
        ComPtr<ID3D11Texture2D> tempTexture;
        DX::ThrowIfFailed(
            m_d3dDevice->CreateTexture2D(
                &tempTextureDesc,
                nullptr,
                &tempTexture
                )
            );

        CD3D11_RENDER_TARGET_VIEW_DESC finalRTVDesc(
            tempTexture.Get(),
            D3D11_RTV_DIMENSION_TEXTURE2D
            );
        DX::ThrowIfFailed(
            m_d3dDevice->CreateRenderTargetView(
                tempTexture.Get(),
                &finalRTVDesc,
                &m_finalRTV
                )
            );

        CD3D11_SHADER_RESOURCE_VIEW_DESC finalSRVDesc(
            tempTexture.Get(),
            D3D11_SRV_DIMENSION_TEXTURE2D
            );
        DX::ThrowIfFailed(
            m_d3dDevice->CreateShaderResourceView(
                tempTexture.Get(),
                &finalSRVDesc,
                &m_finalSRV
                )
            );
    }

    m_sampleOverlay->UpdateForWindowSizeChange();
}

void DirectXFractal::Update(float timeTotal, float timeDelta)
{
    if (m_fastGenerate)
    {
        // Only update throttle settings if the performance-sensitive code path is being utilized.

        auto control = m_movingThrottle->Update(timeDelta);

        if (control == FrameWorkload::Increase)
        {
            m_downSampleRatio -= SampleSettings::Performance::DownSampleRatioDeltaDecrease;
        }
        if (control == FrameWorkload::Decrease)
        {
            m_downSampleRatio += SampleSettings::Performance::DownSampleRatioDeltaIncrease;
        }
        if (control != FrameWorkload::Maintain)
        {
            m_downSampleRatio = max(1.0f, min(SampleSettings::Performance::DownSampleRatioMax, m_downSampleRatio));
        }
    }
}

void DirectXFractal::Render()
{
    // Initialize the fractal target size to equal the screen render target size.
    float2 targetSize = float2(
        m_renderTargetSize.Width,
        m_renderTargetSize.Height
        );

    // If the fractal is being manipulated, scale back the target size in order to reduce
    // frame workload, enabling more responsive manipulation.  If the fractal is not being
    // manipulated, iterate at the full resolution.
    if (m_fastGenerate)
    {
        targetSize.x = floor(targetSize.x / m_downSampleRatio);
        targetSize.y = floor(targetSize.y / m_downSampleRatio);
    }

    // Calculate the fractal boundary transformation matrix.
    double4x4 boundsTransform = identityDouble();
    boundsTransform = mulDouble(boundsTransform, translationDouble(m_transform.translationX, m_transform.translationY, 0.0));
    boundsTransform = mulDouble(boundsTransform, rotationZDouble(m_transform.rotation));
    boundsTransform = mulDouble(boundsTransform, scaleDouble(m_transform.scale, m_transform.scale, 1.0));
    boundsTransform = mulDouble(boundsTransform, scaleDouble(1.0, m_windowBounds.Height / m_windowBounds.Width, 1.0));

    // Update constant buffers.
    D3D11_MAPPED_SUBRESOURCE mappedResource = {0};
    DX::ThrowIfFailed(
        m_d3dContext->Map(
            m_drawRectCB.Get(),
            0,
            D3D11_MAP_WRITE_DISCARD,
            0,
            &mappedResource
            )
        );

    DrawRectCB* drawRectCB = static_cast<DrawRectCB*>(mappedResource.pData);

    drawRectCB->boundsTransform = convert(boundsTransform);

    if (m_technique == FractalTechnique::DirectCompute)
    {
        // When using the DirectCompute generation technique, the buffer is rendered using
        // indexed access, so scaling the pixel shader tex coordinate to matching values
        // makes this task simpler.
        drawRectCB->texScale = targetSize;
    }
    else
    {
        if (m_fastGenerate)
        {
            // If fast generation is used, only a portion of the intermediate texture is drawn.
            drawRectCB->texScale.x = targetSize.x / m_renderTargetSize.Width;
            drawRectCB->texScale.y = targetSize.y / m_renderTargetSize.Height;
        }
        else
        {
            // Otherwise, the entire texture is drawn.
            drawRectCB->texScale = float2(1.0f, 1.0f);
        }
    }

    m_d3dContext->Unmap(
        m_drawRectCB.Get(),
        0
        );

    if (m_technique == FractalTechnique::DirectCompute)
    {
        // Update DirectCompute-specific constant buffers.
        DX::ThrowIfFailed(
            m_d3dContext->Map(
                m_computeCB.Get(),
                0,
                D3D11_MAP_WRITE_DISCARD,
                0,
                &mappedResource
                )
            );

        // This constant buffer provides information on how to map a particular compute thread to
        // a coordinate in the complex plane, and how to map that to an index in the structured buffer.
        double2 topLeftCorner = mulDouble(double2(-0.5, 0.5), boundsTransform);
        double2 deltaAllDtidX = mulDouble(double2(0.5, 0.5), boundsTransform) - topLeftCorner;
        double2 deltaAllDtidY = mulDouble(double2(-0.5, -0.5), boundsTransform) - topLeftCorner;

        if (m_useDoubles)
        {
            ComputeDoubleCB* computeCB = static_cast<ComputeDoubleCB*>(mappedResource.pData);

            computeCB->originDtid00.x = topLeftCorner.x;
            computeCB->originDtid00.y = topLeftCorner.y;
            computeCB->deltaPerDtidX.x = deltaAllDtidX.x / targetSize.x;
            computeCB->deltaPerDtidX.y = deltaAllDtidX.y / targetSize.x;
            computeCB->deltaPerDtidY.x = deltaAllDtidY.x / targetSize.y;
            computeCB->deltaPerDtidY.y = deltaAllDtidY.y / targetSize.y;
        }
        else
        {
            ComputeCB* computeCB = static_cast<ComputeCB*>(mappedResource.pData);

            computeCB->originDtid00.x = static_cast<float>(topLeftCorner.x);
            computeCB->originDtid00.y = static_cast<float>(topLeftCorner.y);
            computeCB->deltaPerDtidX.x = static_cast<float>(deltaAllDtidX.x) / targetSize.x;
            computeCB->deltaPerDtidX.y = static_cast<float>(deltaAllDtidX.y) / targetSize.x;
            computeCB->deltaPerDtidY.x = static_cast<float>(deltaAllDtidY.x) / targetSize.y;
            computeCB->deltaPerDtidY.y = static_cast<float>(deltaAllDtidY.y) / targetSize.y;
        }

        m_d3dContext->Unmap(
            m_computeCB.Get(),
            0
            );
    }

    // Set up pipeline state common to all fractal generation techniques.
    UINT stride = sizeof(FractalVertex);
    UINT offset = 0;
    m_d3dContext->IASetVertexBuffers(
        0,
        1,
        m_vertexBuffer.GetAddressOf(),
        &stride,
        &offset
        );

    m_d3dContext->IASetIndexBuffer(
        m_indexBuffer.Get(),
        DXGI_FORMAT_R16_UINT,
        0
        );

    m_d3dContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

    m_d3dContext->IASetInputLayout(m_inputLayout.Get());

    m_d3dContext->VSSetShader(
        m_vertexShader.Get(),
        nullptr,
        0
        );

    m_d3dContext->VSSetConstantBuffers(
        0,
        1,
        m_drawRectCB.GetAddressOf()
        );

    // Create null shader binding arrays.  These will be used to unbind resources from pipeline stages.
    ID3D11ShaderResourceView* nullSRV[] = {nullptr, nullptr};
    ID3D11RenderTargetView* nullRTV[] = {nullptr};
    ID3D11UnorderedAccessView* nullUAV[] = {nullptr};

    if (m_technique == FractalTechnique::DirectCompute)
    {
        if (!m_complete)
        {
            // Generate the fractal using the DirectCompute technique.

            m_d3dContext->CSSetShader(
                m_directComputeCS.Get(),
                nullptr,
                0
                );

            ID3D11Buffer* directComputeCBs[] =
            {
                m_bufferInfoCB.Get(),
                m_computeCB.Get()
            };

            m_d3dContext->CSSetConstantBuffers(
                0,
                ARRAYSIZE(directComputeCBs),
                directComputeCBs
                );

            m_d3dContext->CSSetUnorderedAccessViews(
                0,
                1,
                m_tempUAV.GetAddressOf(),
                nullptr
                );

            // Dispatch the minimum number of threads required to assign one thread
            // to each complex coordinate in the fractal we wish to iterate on.
            m_d3dContext->Dispatch(
                (static_cast<int>(targetSize.x) + SampleSettings::DirectCompute::ThreadGroupSizeX - 1) / SampleSettings::DirectCompute::ThreadGroupSizeX,
                (static_cast<int>(targetSize.y) + SampleSettings::DirectCompute::ThreadGroupSizeY - 1) / SampleSettings::DirectCompute::ThreadGroupSizeY,
                1
                );

            m_d3dContext->CSSetUnorderedAccessViews(
                0,
                ARRAYSIZE(nullUAV),
                nullUAV,
                nullptr
                );
        }

        // Draw the intermediate buffer.

        CD3D11_VIEWPORT renderTargetViewport(
            0.0f,
            0.0f,
            m_renderTargetSize.Width,
            m_renderTargetSize.Height
            );

        m_d3dContext->RSSetViewports(
            1,
            &renderTargetViewport
            );

        m_d3dContext->OMSetRenderTargets(
            1,
            m_d3dRenderTargetView.GetAddressOf(),
            nullptr
            );

        m_d3dContext->PSSetShader(
            m_renderBufferPS.Get(),
            nullptr,
            0
            );

        m_d3dContext->PSSetConstantBuffers(
            0,
            1,
            m_bufferInfoCB.GetAddressOf()
            );

        ID3D11ShaderResourceView* renderBufferSRVs[] =
        {
            m_tempSRV[0].Get(),
            m_gradient.Get()
        };
        m_d3dContext->PSSetShaderResources(
            0,
            ARRAYSIZE(renderBufferSRVs),
            renderBufferSRVs
            );

        m_d3dContext->PSSetSamplers(
            0,
            1,
            m_linearSampler.GetAddressOf()
            );

        m_d3dContext->DrawIndexed(
            m_indexCount,
            0,
            0
            );

        m_d3dContext->PSSetShaderResources(
            0,
            ARRAYSIZE(nullSRV),
            nullSRV
            );
    }

    if (m_technique == FractalTechnique::SinglePass)
    {
        if (!m_complete)
        {
            // Generate the fractal using the single-pass technique.

            // Constrain the rasterizer to only draw to a portion of the intermediate texture.

            CD3D11_VIEWPORT targetSizeViewport(
                0.0f,
                0.0f,
                targetSize.x,
                targetSize.y
                );

            m_d3dContext->RSSetViewports(
                1,
                &targetSizeViewport
                );

            m_d3dContext->OMSetRenderTargets(
                1,
                m_finalRTV.GetAddressOf(),
                nullptr
                );

            m_d3dContext->PSSetShader(
                m_singlePassPS.Get(),
                nullptr,
                0
                );

            m_d3dContext->PSSetShaderResources(
                0,
                1,
                m_gradient.GetAddressOf()
                );

            m_d3dContext->PSSetSamplers(
                0,
                1,
                m_linearSampler.GetAddressOf()
                );

            m_d3dContext->DrawIndexed(
                m_indexCount,
                0,
                0
                );
        }

        // Draw the intermediate texture.

        CD3D11_VIEWPORT renderTargetViewport(
            0.0f,
            0.0f,
            m_renderTargetSize.Width,
            m_renderTargetSize.Height
            );

        m_d3dContext->RSSetViewports(
            1,
            &renderTargetViewport
            );

        m_d3dContext->OMSetRenderTargets(
            1,
            m_d3dRenderTargetView.GetAddressOf(),
            nullptr
            );

        m_d3dContext->PSSetShader(
            m_renderTexturePS.Get(),
            nullptr,
            0
            );

        m_d3dContext->PSSetShaderResources(
            0,
            1,
            m_finalSRV.GetAddressOf()
            );

        m_d3dContext->DrawIndexed(
            m_indexCount,
            0,
            0
            );

        m_d3dContext->PSSetShaderResources(
            0,
            ARRAYSIZE(nullSRV),
            nullSRV
            );
    }

    if (m_technique == FractalTechnique::ThreePhase)
    {
        if (!m_complete)
        {
            // Generate the fractal using the three-phase technique.

            // Perform the initial pass, constraining the rasterizer to only draw to a portion of the intermediate texture.
            CD3D11_VIEWPORT targetSizeViewport(
                0.0f,
                0.0f,
                targetSize.x,
                targetSize.y
                );

            m_d3dContext->RSSetViewports(
                1,
                &targetSizeViewport
                );

            m_d3dContext->OMSetRenderTargets(
                1,
                m_tempRTV[0].GetAddressOf(),
                nullptr
                );

            m_d3dContext->PSSetShader(
                m_initialPS.Get(),
                nullptr,
                0
                );

            m_d3dContext->DrawIndexed(
                m_indexCount,
                0,
                0
                );

            // Determine the number of intermediate iteration steps to run, based on the target number
            // of iterations, the number of iterations performed during the initial phase, and the
            // number of iterations to be performed during the final phase.
            int numStepsToRun =
                (SampleSettings::TargetIterations - SampleSettings::ThreePhase::InitialIterations - SampleSettings::ThreePhase::FinalIterations) /
                SampleSettings::ThreePhase::StepIterations;
            for (int i = 0; i < numStepsToRun; i++)
            {
                // Use the previous generation's intermediate data (index 0) and write the output
                // to the next generation's intermediate data (index 1).
                m_d3dContext->OMSetRenderTargets(
                    1,
                    m_tempRTV[1].GetAddressOf(),
                    nullptr
                    );

                m_d3dContext->PSSetShader(
                    m_stepPS.Get(),
                    nullptr,
                    0
                    );

                m_d3dContext->PSSetShaderResources(
                    0,
                    1,
                    m_tempSRV[0].GetAddressOf()
                    );

                m_d3dContext->PSSetSamplers(
                    0,
                    1,
                    m_pointSampler.GetAddressOf()
                    );

                m_d3dContext->DrawIndexed(
                    m_indexCount,
                    0,
                    0
                    );

                m_d3dContext->PSSetShaderResources(
                    0,
                    ARRAYSIZE(nullSRV),
                    nullSRV
                    );

                // Swap the intermediate textures, as the output of the current generation will
                // be the input of the next generation.
                m_tempRTV[0].Swap(m_tempRTV[1]);
                m_tempSRV[0].Swap(m_tempSRV[1]);
            }

            // Perform the final generation step.
            m_d3dContext->OMSetRenderTargets(
                1,
                m_finalRTV.GetAddressOf(),
                nullptr
                );

            m_d3dContext->PSSetShader(
                m_finalPS.Get(),
                nullptr,
                0
                );

            ID3D11ShaderResourceView* finalSRVs[] =
            {
                m_tempSRV[0].Get(),
                m_gradient.Get()
            };
            m_d3dContext->PSSetShaderResources(
                0,
                ARRAYSIZE(finalSRVs),
                finalSRVs
                );

            ID3D11SamplerState* finalSamplers[] =
            {
                m_pointSampler.Get(),
                m_linearSampler.Get()
            };
            m_d3dContext->PSSetSamplers(
                0,
                ARRAYSIZE(finalSamplers),
                finalSamplers
                );

            m_d3dContext->DrawIndexed(
                m_indexCount,
                0,
                0
                );

            m_d3dContext->PSSetShaderResources(
                0,
                ARRAYSIZE(nullSRV),
                nullSRV
                );
        }

        // Draw the intermediate texture.

        CD3D11_VIEWPORT renderTargetViewport(
            0.0f,
            0.0f,
            m_renderTargetSize.Width,
            m_renderTargetSize.Height
            );

        m_d3dContext->RSSetViewports(
            1,
            &renderTargetViewport
            );

        m_d3dContext->OMSetRenderTargets(
            1,
            m_d3dRenderTargetView.GetAddressOf(),
            nullptr
            );

        m_d3dContext->PSSetShader(
            m_renderTexturePS.Get(),
            nullptr,
            0
            );

        m_d3dContext->PSSetShaderResources(
            0,
            1,
            m_finalSRV.GetAddressOf()
            );

        m_d3dContext->PSSetSamplers(
            0,
            1,
            m_linearSampler.GetAddressOf()
            );

        m_d3dContext->DrawIndexed(
            m_indexCount,
            0,
            0
            );

        m_d3dContext->PSSetShaderResources(
            0,
            ARRAYSIZE(nullSRV),
            nullSRV
            );
    }

    if (m_fastGenerate)
    {
        // If the generation just performed was a fast generation, set the next generation
        // to be a full generation.  If manipulation occurs, this flag will be reset to true.
        m_fastGenerate = false;
    }
    else
    {
        // Otherwise, the intermediate surface contains full-resolution fractal data, and should
        // simply be displayed in future frames, as long as it is valid.
        m_complete = true;
    }

    // Render the Sample Overlay.
    m_sampleOverlay->Render();
}

int DirectXFractal::RoundUpTo16(int value)
{
    // This method is useful when defining constant buffers, as
    // their size must be a multiple of 16 bytes.
    return (value + 15) & ~0xF;
}

void DirectXFractal::HandleViewManipulation(float dRotation, float dScale, float dX, float dY)
{
    // Save the old transform values.  If manipulation of the fractal view by the user
    // changes any of these values, then fast fractal generation will be enabled and any
    // existing intermediate surfaces will be invalidated.
    ViewTransform transformOld;
    memcpy(&transformOld, &m_transform, sizeof(transformOld));

    // Translations must be interpreted based on the current scale and rotation of the view.
    float translateX = dX * static_cast<float>(m_transform.scale) / m_windowBounds.Width;
    float translateY = dY * static_cast<float>(m_transform.scale) / m_windowBounds.Width;
    float sinRotation = sinf(static_cast<float>(m_transform.rotation) * PI_F / 180.0f);
    float cosRotation = cosf(static_cast<float>(m_transform.rotation) * PI_F / 180.0f);
    m_transform.translationX -= translateX * cosRotation + translateY * sinRotation;
    m_transform.translationY += translateY * cosRotation - translateX * sinRotation;

    // Scale and rotation changes are independent of the current view.
    m_transform.scale /= dScale;
    m_transform.rotation += dRotation;

    // Ensure rotation values stay near zero to ensure trigonometric function accuracy.
    if (m_transform.rotation < -360.0f) m_transform.rotation += 360.0f;
    if (m_transform.rotation > 360.0f) m_transform.rotation -= 360.0f;

    // Clamp the scale setting.
    double minScale;
    if (m_useDoubles)
    {
        minScale = SampleSettings::Manipulation::MinimumF64Scale;
    }
    else if (m_technique == FractalTechnique::DirectCompute || m_technique == FractalTechnique::SinglePass)
    {
        minScale = SampleSettings::Manipulation::MinimumF32Scale;
    }
    else if (!m_useUnormTextures)
    {
        minScale = SampleSettings::Manipulation::MinimumF16Scale;
    }
    else
    {
        minScale = SampleSettings::Manipulation::MinimumF8Scale;
    }


    m_transform.scale = min(SampleSettings::Manipulation::MaximumScale, max(minScale, m_transform.scale));

    // Clamp translation to a certain radius about the origin of the complex plane.
    float translationMag = sqrtf(
        static_cast<float>(m_transform.translationX) * static_cast<float>(m_transform.translationX) +
        static_cast<float>(m_transform.translationY) * static_cast<float>(m_transform.translationY)
        );
    if (translationMag > SampleSettings::Manipulation::MaximumTranslationMagnitude)
    {
        m_transform.translationX *= SampleSettings::Manipulation::MaximumTranslationMagnitude / translationMag;
        m_transform.translationY *= SampleSettings::Manipulation::MaximumTranslationMagnitude / translationMag;
    }

    // If the transform values have changed, enable fast fractal generation and
    // invalidate existing intermediate surfaces.
    if (memcmp(&m_transform, &transformOld, sizeof(transformOld)) != 0)
    {
        m_fastGenerate = true;
        m_complete = false;
    }
}

void DirectXFractal::SaveInternalValue(
    _In_ IPropertySet^ state,
    _In_ Platform::String^ key,
    _In_ Platform::Object^ value
    )
{
    if (state->HasKey(key))
    {
        state->Remove(key);
    }
    state->Insert(key, value);
}

void DirectXFractal::SaveInternalState(_In_ IPropertySet^ state)
{
    SaveInternalValue(state, "m_transform.scale", PropertyValue::CreateDouble(m_transform.scale));
    SaveInternalValue(state, "m_transform.rotation", PropertyValue::CreateDouble(m_transform.rotation));
    SaveInternalValue(state, "m_transform.translationX", PropertyValue::CreateDouble(m_transform.translationX));
    SaveInternalValue(state, "m_transform.translationY", PropertyValue::CreateDouble(m_transform.translationY));
}

void DirectXFractal::LoadInternalState(_In_ IPropertySet^ state)
{
    if (
        state->HasKey("m_transform.scale") &&
        state->HasKey("m_transform.rotation") &&
        state->HasKey("m_transform.translationX") &&
        state->HasKey("m_transform.translationY")
        )
    {
        m_transform.scale = safe_cast<IPropertyValue^>(state->Lookup("m_transform.scale"))->GetDouble();
        m_transform.rotation = safe_cast<IPropertyValue^>(state->Lookup("m_transform.rotation"))->GetDouble();
        m_transform.translationX = safe_cast<IPropertyValue^>(state->Lookup("m_transform.translationX"))->GetDouble();
        m_transform.translationY = safe_cast<IPropertyValue^>(state->Lookup("m_transform.translationY"))->GetDouble();
    }
}
