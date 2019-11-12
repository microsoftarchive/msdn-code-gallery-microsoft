//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXHelper.h"
#include "SampleSettings.h"
#include "SamplingRenderer.h"

using namespace TiledResources;

using namespace concurrency;
using namespace DirectX;
using namespace Windows::Foundation;

SamplingRenderer::SamplingRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources),
    m_sampleIndex(0),
    m_debugMode(false)
{
    srand(0);
    CreateDeviceDependentResources();
    CreateWindowSizeDependentResources();
}

void SamplingRenderer::CreateDeviceDependentResources()
{
    auto device = m_deviceResources->GetD3DDevice();

    // Create a constant buffer for viewer constants.
    D3D11_BUFFER_DESC constantBufferDesc;
    ZeroMemory(&constantBufferDesc, sizeof(constantBufferDesc));
    constantBufferDesc.ByteWidth = sizeof(XMFLOAT4X4) * 2;
    constantBufferDesc.Usage = D3D11_USAGE_DYNAMIC;
    constantBufferDesc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
    constantBufferDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    DX::ThrowIfFailed(device->CreateBuffer(&constantBufferDesc, nullptr, &m_viewerVertexShaderConstantBuffer));

    // Create a vertex buffer for the viewer.
    float vertexBufferData[] =
    {
        -1.0f, 1.0f, 0.0f, 0.0f,
         1.0f, 1.0f, 1.0f, 0.0f,
        -1.0f, -1.0f, 0.0f, 1.0f,
         1.0f, -1.0f, 1.0f, 1.0f
    };
    D3D11_BUFFER_DESC vertexBufferDesc;
    ZeroMemory(&vertexBufferDesc, sizeof(vertexBufferDesc));
    vertexBufferDesc.ByteWidth = sizeof(vertexBufferData);
    vertexBufferDesc.Usage = D3D11_USAGE_DEFAULT;
    vertexBufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    D3D11_SUBRESOURCE_DATA vertexBufferInitialData = {vertexBufferData, 0, 0};
    DX::ThrowIfFailed(device->CreateBuffer(&vertexBufferDesc, &vertexBufferInitialData, &m_viewerVertexBuffer));

    // Create an index buffer for the viewer.
    unsigned int indexBufferData[] =
    {
        0, 1, 2,
        3, 2, 1
    };
    D3D11_BUFFER_DESC indexBufferDesc;
    ZeroMemory(&indexBufferDesc, sizeof(indexBufferDesc));
    indexBufferDesc.ByteWidth = sizeof(indexBufferData);
    indexBufferDesc.Usage = D3D11_USAGE_DEFAULT;
    indexBufferDesc.BindFlags = D3D11_BIND_INDEX_BUFFER;
    D3D11_SUBRESOURCE_DATA indexBufferInitialData = {indexBufferData, 0, 0};
    DX::ThrowIfFailed(device->CreateBuffer(&indexBufferDesc, &indexBufferInitialData, &m_viewerIndexBuffer));

    // Create a constant buffer for sampling constants.
    D3D11_BUFFER_DESC pixelShaderConstantBufferDesc;
    ZeroMemory(&pixelShaderConstantBufferDesc, sizeof(pixelShaderConstantBufferDesc));
    pixelShaderConstantBufferDesc.ByteWidth = 16; // Minimum size for a constant buffer.
    pixelShaderConstantBufferDesc.Usage = D3D11_USAGE_DYNAMIC;
    pixelShaderConstantBufferDesc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
    pixelShaderConstantBufferDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;

    // For simplicity, assume all textures are maximally sized and have full MIP chains.
    // Extracted sample values will be transformed as necessary to account for this.
    float pixelShaderConstants[4] = {16384.0f / SampleSettings::Sampling::Ratio, 15.0f, 0.0f, 0.0f};
    D3D11_SUBRESOURCE_DATA constantBufferInitialData = {pixelShaderConstants, 0, 0};
    DX::ThrowIfFailed(device->CreateBuffer(&pixelShaderConstantBufferDesc, &constantBufferInitialData, &m_pixelShaderConstantBuffer));

    // Create clamped bilinear sampler.
    D3D11_SAMPLER_DESC samplerDesc;
    ZeroMemory(&samplerDesc, sizeof(samplerDesc));
    samplerDesc.Filter = D3D11_FILTER_MIN_MAG_LINEAR_MIP_POINT;
    samplerDesc.AddressU = D3D11_TEXTURE_ADDRESS_CLAMP;
    samplerDesc.AddressV = D3D11_TEXTURE_ADDRESS_CLAMP;
    samplerDesc.AddressW = D3D11_TEXTURE_ADDRESS_CLAMP;
    samplerDesc.ComparisonFunc = D3D11_COMPARISON_NEVER;
    samplerDesc.MaxLOD = D3D11_FLOAT32_MAX;
    DX::ThrowIfFailed(device->CreateSamplerState(&samplerDesc, &m_viewerSampler));
}

task<void> SamplingRenderer::CreateDeviceDependentResourcesAsync()
{
    // Load and create the vertex shader and input layout.
    auto vsTask = DX::ReadDataAsync(L"SamplingViewer.vs.cso").then([this](std::vector<byte> fileData)
    {
        auto device = m_deviceResources->GetD3DDevice();
        D3D11_INPUT_ELEMENT_DESC inputLayoutDesc[] =
        {
            { "POSITION", 0, DXGI_FORMAT_R32G32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT, 0, 8, D3D11_INPUT_PER_VERTEX_DATA, 0 },
        };
        DX::ThrowIfFailed(device->CreateInputLayout(inputLayoutDesc, ARRAYSIZE(inputLayoutDesc), fileData.data(), fileData.size(), &m_viewerInputLayout));
        DX::ThrowIfFailed(device->CreateVertexShader(fileData.data(), fileData.size(), nullptr, &m_viewerVertexShader));
    });

    // Load and create the sampling pixel shader.
    auto psTask = DX::ReadDataAsync(L"SamplingRenderer.ps.cso").then([this](std::vector<byte> fileData)
    {
        auto device = m_deviceResources->GetD3DDevice();
        DX::ThrowIfFailed(device->CreatePixelShader(fileData.data(), fileData.size(), nullptr, &m_samplingPixelShader));
    });

    // Load and create the viewer pixel shader.
    auto viewerPsTask = DX::ReadDataAsync(L"SamplingViewer.ps.cso").then([this](std::vector<byte> fileData)
    {
        auto device = m_deviceResources->GetD3DDevice();
        DX::ThrowIfFailed(device->CreatePixelShader(fileData.data(), fileData.size(), nullptr, &m_viewerPixelShader));
    });

    return (vsTask && psTask && viewerPsTask);
}

void SamplingRenderer::CreateWindowSizeDependentResources()
{
    auto device = m_deviceResources->GetD3DDevice();
    auto context = m_deviceResources->GetD3DDeviceContext();

    // Define the sampling viewport as a fraction of the window.
    UINT targetWidth = static_cast<UINT>(m_deviceResources->GetScreenViewport().Width / SampleSettings::Sampling::Ratio);
    UINT targetHeight = static_cast<UINT>(m_deviceResources->GetScreenViewport().Height / SampleSettings::Sampling::Ratio);
    ZeroMemory(&m_viewport, sizeof(m_viewport));
    m_viewport.Width = static_cast<FLOAT>(targetWidth);
    m_viewport.Height = static_cast<FLOAT>(targetHeight);
    m_viewport.MaxDepth = 1.0f;

    // Create a render target texture for the sampling pass.
    D3D11_TEXTURE2D_DESC textureDesc;
    ZeroMemory(&textureDesc, sizeof(textureDesc));
    textureDesc.Width = targetWidth;
    textureDesc.Height = targetHeight;
    textureDesc.MipLevels = 1;
    textureDesc.ArraySize = 1;
    textureDesc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
    textureDesc.SampleDesc.Count = 1;
    textureDesc.Usage = D3D11_USAGE_DEFAULT;
    textureDesc.BindFlags = D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE;
    DX::ThrowIfFailed(device->CreateTexture2D(&textureDesc, nullptr, &m_colorTexture));

    // Create a staging texture for extracting samples.
    textureDesc.Usage = D3D11_USAGE_STAGING;
    textureDesc.BindFlags = 0;
    textureDesc.CPUAccessFlags = D3D11_CPU_ACCESS_READ;
    DX::ThrowIfFailed(device->CreateTexture2D(&textureDesc, nullptr, &m_colorStagingTexture));

    // Create the render target view.
    D3D11_RENDER_TARGET_VIEW_DESC renderTargetViewDesc;
    ZeroMemory(&renderTargetViewDesc, sizeof(renderTargetViewDesc));
    renderTargetViewDesc.Format = textureDesc.Format;
    renderTargetViewDesc.ViewDimension = D3D11_RTV_DIMENSION_TEXTURE2D;
    DX::ThrowIfFailed(device->CreateRenderTargetView(m_colorTexture.Get(), &renderTargetViewDesc, &m_colorTextureRenderTargetView));

    // Create the shader resource view that will be used by the visualizer.
    D3D11_SHADER_RESOURCE_VIEW_DESC shaderResourceViewDesc;
    ZeroMemory(&shaderResourceViewDesc, sizeof(shaderResourceViewDesc));
    shaderResourceViewDesc.Format = textureDesc.Format;
    shaderResourceViewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
    shaderResourceViewDesc.Texture2D.MipLevels = 1;
    DX::ThrowIfFailed(device->CreateShaderResourceView(m_colorTexture.Get(), &shaderResourceViewDesc, &m_colorTextureView));

    // Create a depth texture and view to match the render target texture.
    D3D11_TEXTURE2D_DESC depthTextureDesc;
    ZeroMemory(&depthTextureDesc, sizeof(depthTextureDesc));
    depthTextureDesc.Width = textureDesc.Width;
    depthTextureDesc.Height = textureDesc.Height;
    depthTextureDesc.MipLevels = 1;
    depthTextureDesc.ArraySize = 1;
    depthTextureDesc.Format = DXGI_FORMAT_D24_UNORM_S8_UINT;
    depthTextureDesc.SampleDesc.Count = 1;
    depthTextureDesc.Usage = D3D11_USAGE_DEFAULT;
    depthTextureDesc.BindFlags = D3D11_BIND_DEPTH_STENCIL;
    DX::ThrowIfFailed(device->CreateTexture2D(&depthTextureDesc, nullptr, &m_depthTexture));

    D3D11_DEPTH_STENCIL_VIEW_DESC depthStencilViewDesc;
    ZeroMemory(&depthStencilViewDesc, sizeof(depthStencilViewDesc));
    depthStencilViewDesc.Format = depthTextureDesc.Format;
    depthStencilViewDesc.ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2D;
    DX::ThrowIfFailed(device->CreateDepthStencilView(m_depthTexture.Get(), &depthStencilViewDesc, &m_depthTextureDepthStencilView));

    // Update the sampling pixel shader constant buffer with size-dependent constants.
    D3D11_MAPPED_SUBRESOURCE mappedResource;
    DX::ThrowIfFailed(context->Map(m_viewerVertexShaderConstantBuffer.Get(), 0, D3D11_MAP_WRITE_DISCARD, 0, &mappedResource));
    const float xPosition = 48.0f;
    const float yPosition = m_deviceResources->GetOutputSize().Height - m_viewport.Height - 48.0f;
    float xWidth = m_viewport.Width / m_deviceResources->GetScreenViewport().Width;
    float yWidth = m_viewport.Height / m_deviceResources->GetScreenViewport().Height;
    float xOffset = 2.0f * (xPosition + m_viewport.Width / 2.0f) / m_deviceResources->GetOutputSize().Width - 1.0f;
    float yOffset = 1.0f - 2.0f * (yPosition + m_viewport.Height / 2.0f) / m_deviceResources->GetOutputSize().Height;
    XMFLOAT4X4 matrix(xWidth, 0.0f, 0.0f, 0.0f, 0.0f, yWidth, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, xOffset, yOffset, 0.0f, 1.0f);
    XMFLOAT4X4 transform = m_deviceResources->GetOrientationTransform3D();
    XMStoreFloat4x4(&static_cast<XMFLOAT4X4*>(mappedResource.pData)[0], XMMatrixTranspose(XMLoadFloat4x4(&transform)));
    XMStoreFloat4x4(&static_cast<XMFLOAT4X4*>(mappedResource.pData)[1], XMMatrixTranspose(XMMatrixMultiply(XMLoadFloat4x4(&matrix), XMLoadFloat4x4(&transform))));
    context->Unmap(m_viewerVertexShaderConstantBuffer.Get(), 0);
}

void SamplingRenderer::ReleaseDeviceDependentResources()
{
    m_colorTexture.Reset();
    m_colorStagingTexture.Reset();
    m_colorTextureRenderTargetView.Reset();
    m_colorTextureView.Reset();
    m_depthTexture.Reset();
    m_depthTextureDepthStencilView.Reset();
    m_samplingPixelShader.Reset();
    m_pixelShaderConstantBuffer.Reset();
    m_viewerVertexBuffer.Reset();
    m_viewerIndexBuffer.Reset();
    m_viewerInputLayout.Reset();
    m_viewerVertexShader.Reset();
    m_viewerPixelShader.Reset();
    m_viewerSampler.Reset();
    m_viewerVertexShaderConstantBuffer.Reset();
}

void SamplingRenderer::SetTargetsForSampling()
{
    auto context = m_deviceResources->GetD3DDeviceContext();

    // Set pipeline state from the Pixel Shader through the Output Merger.
    context->OMSetRenderTargets(1, m_colorTextureRenderTargetView.GetAddressOf(), m_depthTextureDepthStencilView.Get());
    context->ClearDepthStencilView(m_depthTextureDepthStencilView.Get(), D3D11_CLEAR_DEPTH, 1.0f, 0);
    context->ClearRenderTargetView(m_colorTextureRenderTargetView.Get(), DirectX::Colors::Transparent);
    context->RSSetViewports(1, &m_viewport);
    context->PSSetShader(m_samplingPixelShader.Get(), nullptr, 0);
    context->PSSetConstantBuffers(0, 1, m_pixelShaderConstantBuffer.GetAddressOf());
    context->PSSetSamplers(0, 1, m_viewerSampler.GetAddressOf());
}

void SamplingRenderer::RenderVisualization()
{
    auto context = m_deviceResources->GetD3DDeviceContext();

    // Set up the pipeline to render the visualization.
    auto targetView = m_deviceResources->GetBackBufferRenderTargetView();
    context->OMSetRenderTargets(1, &targetView, nullptr);
    UINT stride = sizeof(float) * 4;
    UINT offset = 0;
    context->IASetVertexBuffers(0, 1, m_viewerVertexBuffer.GetAddressOf(), &stride, &offset);
    context->IASetInputLayout(m_viewerInputLayout.Get());
    context->IASetIndexBuffer(m_viewerIndexBuffer.Get(), DXGI_FORMAT_R32_UINT, 0);
    context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
    context->VSSetShader(m_viewerVertexShader.Get(), nullptr, 0);
    context->VSSetConstantBuffers(0, 1, m_viewerVertexShaderConstantBuffer.GetAddressOf());
    context->PSSetShader(m_viewerPixelShader.Get(), nullptr, 0);
    context->PSSetShaderResources(0, 1, m_colorTextureView.GetAddressOf());

    // Render the visualization.
    context->DrawIndexed(6, 0, 0);

    // Unbind the sampling SRV to enable the corresponding RTV for the next frame.
    ID3D11ShaderResourceView* nullView[] = {nullptr};
    context->PSSetShaderResources(0, 1, nullView);
}

std::vector<DecodedSample> SamplingRenderer::CollectSamples()
{
    auto context = m_deviceResources->GetD3DDeviceContext();

    // Copy the render target data to the staging texture.
    context->CopyResource(m_colorStagingTexture.Get(), m_colorTexture.Get());

    // Map the staging texture to enable access to the data.
    D3D11_MAPPED_SUBRESOURCE mappedResource;
    DX::ThrowIfFailed(context->Map(m_colorStagingTexture.Get(), 0, D3D11_MAP_READ, 0, &mappedResource));

    // Read and decode the samples.
    std::vector<DecodedSample> decodedSamples;
    int row = 0;
    int column = 0;
    m_sampleCount = 0;
    while (GetNextSamplePosition(&row, &column))
    {
        unsigned int encodedSample = reinterpret_cast<unsigned int*>(static_cast<unsigned char*>(mappedResource.pData) + mappedResource.RowPitch * row)[column];
        if (encodedSample != 0)
        {
            // If the sample is still the clear color (all zeros), then no geometry hit this pixel.
            // No valid encodings will result in all zeros, as such a sample would correspond to a
            // texture coordinate of x = y = z = -1, which is clearly not a normalized vector.
            decodedSamples.push_back(DecodeSample(encodedSample));
        }
    }

    context->Unmap(m_colorStagingTexture.Get(), 0);

    return decodedSamples;
}

void SamplingRenderer::SetDebugMode(bool value)
{
    m_debugMode = value;
    m_newDebugSamplePosition = false;
}

void SamplingRenderer::DebugSample(float x, float y)
{
    m_debugSamplePositionX = x / SampleSettings::Sampling::Ratio;
    m_debugSamplePositionY = y / SampleSettings::Sampling::Ratio;
    m_newDebugSamplePosition = true;
}

bool SamplingRenderer::GetNextSamplePosition(int* row, int* column)
{
    if (m_debugMode)
    {
        if (m_newDebugSamplePosition)
        {
            *row = static_cast<int>(m_debugSamplePositionY + 0.5f);
            *column = static_cast<int>(m_debugSamplePositionX + 0.5f);
            m_newDebugSamplePosition = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    *row = rand() % static_cast<int>(m_viewport.Height);
    *column = rand() % static_cast<int>(m_viewport.Width);

    return ++m_sampleCount <= SampleSettings::Sampling::SamplesPerFrame;
}

DecodedSample SamplingRenderer::DecodeSample(unsigned int encodedSample)
{
    unsigned char sampleB = static_cast<unsigned char>(encodedSample & 0xFF);
    unsigned char sampleG = static_cast<unsigned char>((encodedSample >> 8) & 0xFF);
    unsigned char sampleR = static_cast<unsigned char>((encodedSample >> 16) & 0xFF);
    unsigned char sampleA = static_cast<unsigned char>((encodedSample >> 24) & 0xFF);
    float x = 2.0f * static_cast<float>(sampleR) / 255.0f - 1.0f;
    float y = 2.0f * static_cast<float>(sampleG) / 255.0f - 1.0f;
    float z = 2.0f * static_cast<float>(sampleB) / 255.0f - 1.0f;
    float lod = (static_cast<float>(sampleA) / 255.0f) * 16.0f;
    short mip = lod < 0.0f ? 0 :  lod > 14.0f ? 14 : static_cast<unsigned short>(lod);
    short face = 0;
    float u = 0.0f;
    float v = 0.0f;
    if (abs(x) > abs(y) && abs(x) > abs(z))
    {
        if (x > 0) // +X
        {
            face = 0;
            u = (1.0f - z / x) / 2.0f;
            v = (1.0f - y / x) / 2.0f;
        }
        else // -X
        {
            face = 1;
            u = (z / -x + 1.0f) / 2.0f;
            v = (1.0f - y / -x) / 2.0f;
        }
    }
    else if (abs(y) > abs(x) && abs(y) > abs(z))
    {
        if (y > 0) // +Y
        {
            face = 2;
            u = (x / y + 1.0f) / 2.0f;
            v = (z / y + 1.0f) / 2.0f;
        }
        else // -Y
        {
            face = 3;
            u = (x / -y + 1.0f) / 2.0f;
            v = (1.0f - z / -y) / 2.0f;
        }
    }
    else
    {
        if (z > 0) // +Z
        {
            face = 4;
            u = (x / z + 1.0f) / 2.0f;
            v = (1.0f - y / z) / 2.0f;
        }
        else // -Z
        {
            face = 5;
            u = (1.0f - x / -z) / 2.0f;
            v = (1.0f - y / -z) / 2.0f;
        }
    }
    DecodedSample decodedSample;
    ZeroMemory(&decodedSample, sizeof(decodedSample));
    decodedSample.u = u;
    decodedSample.v = v;
    decodedSample.mip = mip;
    decodedSample.face = face;

#ifdef _DEBUG
    if (m_debugMode)
    {
        std::ostringstream sampleInfoMessage;
        sampleInfoMessage << "SamplingRenderer::DecodeSample --> Face = ";
        switch (face)
        {
        case 0:
            sampleInfoMessage << "[+X]";
            break;
        case 1:
            sampleInfoMessage << "[-X]";
            break;
        case 2:
            sampleInfoMessage << "[+Y]";
            break;
        case 3:
            sampleInfoMessage << "[-Y]";
            break;
        case 4:
            sampleInfoMessage << "[+Z]";
            break;
        case 5:
            sampleInfoMessage << "[-Z]";
            break;
        default:
            sampleInfoMessage << "[error]";
            break;
        }
        sampleInfoMessage << " MIP = [" << mip << "]";
        sampleInfoMessage << " UV = (" << u << ", " << v << ")" << std::endl;
        OutputDebugStringA(sampleInfoMessage.str().c_str());
    }
#endif

    return decodedSample;
}
