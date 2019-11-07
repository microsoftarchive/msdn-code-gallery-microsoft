//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "D3DPostProcessing.h"
#include "BasicShapes.h"
#include "BasicLoader.h"

using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;

D3DPostProcessing::D3DPostProcessing()
{
}

void D3DPostProcessing::HandleDeviceLost()
{
    // Release window size-dependent resources prior to creating a new device and swap chain.
    m_intermediateTextureRenderTargetView = nullptr;
    m_intermediateTextureShaderResourceView = nullptr;
    m_brightPassTextureRenderTargetView = nullptr;
    m_brightPassTextureShaderResourceView = nullptr;

    for (int n = 0; n < numberOfGlowTextures; n++)
    {
        m_glowTextureRenderTargetViews[n] = nullptr;
        m_glowTextureShaderResourceViews[n] = nullptr;
    }

    DirectXBase::HandleDeviceLost();
}

void D3DPostProcessing::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();
}

void D3DPostProcessing::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct3D post-processing effects sample"
        );

    BasicLoader^ loader = ref new BasicLoader(m_d3dDevice.Get());

    loader->LoadShader(
        "SimpleVertexShader.cso",
        nullptr,
        0,
        &m_vertexShader,
        &m_inputLayout
        );

    // create the vertex and index buffers for drawing the cube

    BasicShapes^ shapes = ref new BasicShapes(m_d3dDevice.Get());

    shapes->CreateCube(
        &m_vertexBuffer,
        &m_indexBuffer,
        nullptr,
        &m_indexCount
        );

    // Create the constant buffer for updating model and camera data
    CD3D11_BUFFER_DESC constantBufferDescription(sizeof(ConstantBuffer), D3D11_BIND_CONSTANT_BUFFER);
    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &constantBufferDescription,
            nullptr,             // Leave the buffer uninitialized
            &m_constantBuffer
            )
        );

    loader->LoadShader(
        "SimplePixelShader.cso",
        &m_pixelShader
        );

    loader->LoadTexture(
        "texture.dds",
        &m_texture,
        &m_textureShaderResourceView
        );

    // create the sampler
    D3D11_SAMPLER_DESC samplerDescription;
    samplerDescription.Filter = D3D11_FILTER_ANISOTROPIC;
    samplerDescription.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDescription.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDescription.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDescription.MipLODBias = 0.0f;
    samplerDescription.MaxAnisotropy = m_featureLevel > D3D_FEATURE_LEVEL_9_1 ? 4 : 2;
    samplerDescription.ComparisonFunc = D3D11_COMPARISON_NEVER;
    samplerDescription.BorderColor[0] = 0.0f;
    samplerDescription.BorderColor[1] = 0.0f;
    samplerDescription.BorderColor[2] = 0.0f;
    samplerDescription.BorderColor[3] = 0.0f;
    samplerDescription.MinLOD = 0;      // This allows the use of all mip levels
    samplerDescription.MaxLOD = D3D11_FLOAT32_MAX;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateSamplerState(
            &samplerDescription,
            &m_sampler)
        );

    m_camera = ref new BasicCamera();

    // Create resources for the Post Processing sample
    // Load the compiled Post Processing pixel shaders

    if (m_featureLevel <= D3D_FEATURE_LEVEL_9_3)
    {
        // load pixel shader for feature level 9
        loader->LoadShader(
            "DownScaleBrightPass9.cso",
            &m_downScaleBrightPass9PixelShader
            );
    }
    else
    {
        // load pixel shader for DX10 and higher feature-level hardware
        loader->LoadShader(
            "DownScale3x3BrightPass.cso",
            &m_downScaleBrightPassPixelShader
            );
    }

    loader->LoadShader(
        "Glow.cso",
        &m_glowPixelShader
        );

    loader->LoadShader(
        "FinalPass.cso",
        &m_finalPassPixelShader
        );

    // create the full window quad input layout
    D3D11_INPUT_ELEMENT_DESC quadLayout[] =
    {
        { "POSITION", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0 },
        { "TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT, 0, D3D11_APPEND_ALIGNED_ELEMENT, D3D11_INPUT_PER_VERTEX_DATA, 0 },
    };

    // load the compiled Quad Vertex Shader

    loader->LoadShader(
        "QuadVertexShader.cso",
        quadLayout,
        2,
        &m_quadVertexShader,
        &m_quadLayout
        );

    // Setup constant buffer for DX9 version of the brightness threshold pixel shader
    D3D11_BUFFER_DESC brightPassConstantBufferDescription;
    brightPassConstantBufferDescription.Usage = D3D11_USAGE_DYNAMIC;
    brightPassConstantBufferDescription.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
    brightPassConstantBufferDescription.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    brightPassConstantBufferDescription.MiscFlags = 0;
    brightPassConstantBufferDescription.ByteWidth =
        (sizeof(ConstantBufferLayoutForDX9BrightPassPixelShader) + 15) & ~0xF; // round size up to multiple of 16 bytes

    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &brightPassConstantBufferDescription,
            nullptr,             // leave the buffer uninitialized
            &m_brightPassConstantBuffer
            )
        );

    // Setup constant buffer for Glow Pixel Shader
    D3D11_BUFFER_DESC glowConstantBufferDescription;
    glowConstantBufferDescription.Usage = D3D11_USAGE_DYNAMIC;
    glowConstantBufferDescription.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
    glowConstantBufferDescription.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    glowConstantBufferDescription.MiscFlags = 0;
    glowConstantBufferDescription.ByteWidth = sizeof(ConstantBufferLayoutForGlowPixelShader);

    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &glowConstantBufferDescription,
            nullptr,             // leave the buffer uninitialized
            &m_glowConstantBuffer
            )
        );

    // Samplers for Post Processing
    samplerDescription.AddressU = D3D11_TEXTURE_ADDRESS_CLAMP;
    samplerDescription.AddressV = D3D11_TEXTURE_ADDRESS_CLAMP;
    samplerDescription.AddressW = D3D11_TEXTURE_ADDRESS_CLAMP;
    samplerDescription.Filter = D3D11_FILTER_MIN_MAG_MIP_LINEAR;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateSamplerState(
            &samplerDescription,
            &m_linearSampler
            )
        );

    samplerDescription.Filter = D3D11_FILTER_MIN_MAG_MIP_POINT;
    DX::ThrowIfFailed(
        m_d3dDevice->CreateSamplerState(
            &samplerDescription,
            &m_pointSampler
            )
        );

    // Create a quad vertex buffer for render-to-texture operations
    ScreenQuadVertex screenVertexQuad[4];
    screenVertexQuad[0].pos = float4(-1.0f, 1.0f, 0.5f, 1.0f);
    screenVertexQuad[0].tex = float2(0.0f, 0.0f);
    screenVertexQuad[1].pos = float4(1.0f, 1.0f, 0.5f, 1.0f);
    screenVertexQuad[1].tex = float2(1.0f, 0.0f);
    screenVertexQuad[2].pos = float4(-1.0f, -1.0f, 0.5f, 1.0f);
    screenVertexQuad[2].tex = float2(0.0f, 1.0f);
    screenVertexQuad[3].pos = float4(1.0f, -1.0f, 0.5f, 1.0f);
    screenVertexQuad[3].tex = float2(1.0f, 1.0f);

    D3D11_BUFFER_DESC vertexBufferDescription =
    {
        4 * sizeof(ScreenQuadVertex),
        D3D11_USAGE_DEFAULT,
        D3D11_BIND_VERTEX_BUFFER,
        0,
        0
    };

    D3D11_SUBRESOURCE_DATA initData;
    initData.pSysMem = screenVertexQuad;
    initData.SysMemPitch = 0;
    initData.SysMemSlicePitch = 0;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &vertexBufferDescription,
            &initData,
            &m_quadVertexBuffer
            )
        );
}

void D3DPostProcessing::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // update the 3D projection matrix
    m_camera->SetProjectionParameters(
        70.0f,                                                  // use a 70-degree vertical field of view
        m_renderTargetSize.Width / m_renderTargetSize.Height,   // specify the aspect ratio of the window
        0.01f,                                                  // specify the nearest Z-distance at which to draw vertices
        100.0f                                                  // specify the farthest Z-distance at which to draw vertices
        );

    // save the new projection matrix to the constant buffer data struct
    m_camera->GetProjectionMatrix(&m_constantBufferData.projection);

    // setup Post Processing sample-specific window size-dependent resources

    m_intermediateRenderTargetWidth  = static_cast<unsigned int>(m_renderTargetSize.Width  * 0.6f);
    m_intermediateRenderTargetHeight = static_cast<unsigned int>(m_renderTargetSize.Height * 0.6f);

    m_glowTextureWidth  = static_cast<unsigned int>(m_intermediateRenderTargetWidth  / 4.0f);
    m_glowTextureHeight = static_cast<unsigned int>(m_intermediateRenderTargetHeight / 4.0f);

    // set pixel size in DX9 level brightness threshold pass constant buffer

    D3D11_MAPPED_SUBRESOURCE mappedResource;
    DX::ThrowIfFailed (
        m_d3dContext->Map(
            m_brightPassConstantBuffer.Get(),
            0,
            D3D11_MAP_WRITE_DISCARD,
            0,
            &mappedResource
            )
        );

    ConstantBufferLayoutForDX9BrightPassPixelShader* brightPassConstantBuffer =
        static_cast<ConstantBufferLayoutForDX9BrightPassPixelShader*>(mappedResource.pData);

    brightPassConstantBuffer->pixelWidth  = 1.0f / m_intermediateRenderTargetWidth;
    brightPassConstantBuffer->pixelHeight = 1.0f / m_intermediateRenderTargetHeight;

    m_d3dContext->Unmap(
        m_brightPassConstantBuffer.Get(),
        0
        );

    // create intermediate render target for 3D cube scene

    Microsoft::WRL::ComPtr<ID3D11Texture2D> intermediateTexture; // will recieve 3D cube scene

    D3D11_TEXTURE2D_DESC intermediateRenderTargetDescription;
    ZeroMemory(&intermediateRenderTargetDescription, sizeof(D3D11_TEXTURE2D_DESC));
    intermediateRenderTargetDescription.ArraySize = 1;
    intermediateRenderTargetDescription.BindFlags = D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE;
    intermediateRenderTargetDescription.Usage = D3D11_USAGE_DEFAULT;
    intermediateRenderTargetDescription.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
    intermediateRenderTargetDescription.Width  = m_intermediateRenderTargetWidth;
    intermediateRenderTargetDescription.Height = m_intermediateRenderTargetHeight;
    intermediateRenderTargetDescription.MipLevels = 1;
    intermediateRenderTargetDescription.SampleDesc.Count = 1;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateTexture2D(
            &intermediateRenderTargetDescription,
            nullptr,
            &intermediateTexture
            )
        );

    // create the render target view
    D3D11_RENDER_TARGET_VIEW_DESC intermediateRenderTargetViewDescription;
    intermediateRenderTargetViewDescription.Format = intermediateRenderTargetDescription.Format;
    intermediateRenderTargetViewDescription.ViewDimension = D3D11_RTV_DIMENSION_TEXTURE2D;
    intermediateRenderTargetViewDescription.Texture2D.MipSlice = 0;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateRenderTargetView(
            intermediateTexture.Get(),
            &intermediateRenderTargetViewDescription,
            &m_intermediateTextureRenderTargetView
            )
        );

    // Create the shader resource view
    D3D11_SHADER_RESOURCE_VIEW_DESC intermediateShaderResourceViewDescription;
    intermediateShaderResourceViewDescription.Format = intermediateRenderTargetDescription.Format;
    intermediateShaderResourceViewDescription.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
    intermediateShaderResourceViewDescription.Texture2D.MipLevels = 1;
    intermediateShaderResourceViewDescription.Texture2D.MostDetailedMip = 0;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateShaderResourceView(
            intermediateTexture.Get(),
            &intermediateShaderResourceViewDescription,
            &m_intermediateTextureShaderResourceView
            )
        );

    // Create a descriptor for the depth/stencil buffer.
    CD3D11_TEXTURE2D_DESC depthStencilDesc(
        DXGI_FORMAT_D24_UNORM_S8_UINT,
        m_intermediateRenderTargetWidth,
        m_intermediateRenderTargetHeight,
        1,
        1,
        D3D11_BIND_DEPTH_STENCIL
        );

    // Allocate a 2-D surface as the depth/stencil buffer.
    ComPtr<ID3D11Texture2D> depthStencil;
    DX::ThrowIfFailed(
        m_d3dDevice->CreateTexture2D(
            &depthStencilDesc,
            nullptr,
            &depthStencil
            )
        );

    // Create a DepthStencil view on this surface to use on bind.
    CD3D11_DEPTH_STENCIL_VIEW_DESC depthStencilViewDesc(D3D11_DSV_DIMENSION_TEXTURE2D);
    DX::ThrowIfFailed(
        m_d3dDevice->CreateDepthStencilView(
            depthStencil.Get(),
            &depthStencilViewDesc,
            &m_d3dDepthStencilView
            )
        );

    // Create the intermediate glow effect blur textures and views

    Microsoft::WRL::ComPtr<ID3D11Texture2D> glowTextures[numberOfGlowTextures];      // render targets for horizontal and vertical glow blur

    // Render target description for glow effect textures
    D3D11_TEXTURE2D_DESC glowTextureDescription;
    ZeroMemory(&glowTextureDescription, sizeof(D3D11_TEXTURE2D_DESC));
    glowTextureDescription.ArraySize = 1;
    glowTextureDescription.BindFlags = D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE;
    glowTextureDescription.Usage = D3D11_USAGE_DEFAULT;
    glowTextureDescription.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
    glowTextureDescription.Width = m_glowTextureWidth;
    glowTextureDescription.Height = m_glowTextureHeight;
    glowTextureDescription.MipLevels = 1;
    glowTextureDescription.SampleDesc.Count = 1;

    // Render target view description
    ZeroMemory(&intermediateRenderTargetViewDescription, sizeof(D3D11_RENDER_TARGET_VIEW_DESC));
    intermediateRenderTargetViewDescription.Format = glowTextureDescription.Format;
    intermediateRenderTargetViewDescription.ViewDimension = D3D11_RTV_DIMENSION_TEXTURE2D;
    intermediateRenderTargetViewDescription.Texture2D.MipSlice = 0;

    // Shader resource view description
    ZeroMemory(&intermediateShaderResourceViewDescription, sizeof(D3D11_SHADER_RESOURCE_VIEW_DESC));
    intermediateShaderResourceViewDescription.Format = glowTextureDescription.Format;
    intermediateShaderResourceViewDescription.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
    intermediateShaderResourceViewDescription.Texture2D.MipLevels = 1;
    intermediateShaderResourceViewDescription.Texture2D.MostDetailedMip = 0;

    for (int i = 0; i < numberOfGlowTextures; i++)
    {
        DX::ThrowIfFailed(
            m_d3dDevice->CreateTexture2D(
                &glowTextureDescription,
                nullptr,
                &(glowTextures[i])
                )
            );

        DX::ThrowIfFailed(
            m_d3dDevice->CreateRenderTargetView(
                glowTextures[i].Get(),
                &intermediateRenderTargetViewDescription,
                &(m_glowTextureRenderTargetViews[i])
                )
            );

        DX::ThrowIfFailed(
            m_d3dDevice->CreateShaderResourceView(
                glowTextures[i].Get(),
                &intermediateShaderResourceViewDescription,
                &(m_glowTextureShaderResourceViews[i])
                )
            );
    }

    // Create the brightness threshold pass texture and views

    Microsoft::WRL::ComPtr<ID3D11Texture2D> brightPassTexture;    // render target for scaled down brightness threshold image

    DX::ThrowIfFailed(
        m_d3dDevice->CreateTexture2D(
            &glowTextureDescription,
            nullptr,
            &brightPassTexture
            )
        );

    intermediateRenderTargetViewDescription.Format = glowTextureDescription.Format;
    intermediateRenderTargetViewDescription.ViewDimension = D3D11_RTV_DIMENSION_TEXTURE2D;
    intermediateRenderTargetViewDescription.Texture2D.MipSlice = 0;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateRenderTargetView(
            brightPassTexture.Get(),
            &intermediateRenderTargetViewDescription,
            &m_brightPassTextureRenderTargetView
            )
        );

    intermediateShaderResourceViewDescription.Format = glowTextureDescription.Format;
    intermediateShaderResourceViewDescription.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
    intermediateShaderResourceViewDescription.Texture2D.MipLevels = 1;
    intermediateShaderResourceViewDescription.Texture2D.MostDetailedMip = 0;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateShaderResourceView(
            brightPassTexture.Get(),
            &intermediateShaderResourceViewDescription,
            &m_brightPassTextureShaderResourceView
            )
        );

    m_sampleOverlay->UpdateForWindowSizeChange();
}

void D3DPostProcessing::Render()
{
    // for Post Processing sample bind intermediate render target instead of backbuffer
    m_d3dContext->OMSetRenderTargets(
        1,
        m_intermediateTextureRenderTargetView.GetAddressOf(),
        m_d3dDepthStencilView.Get()
        );

    // clear both the render target and depth stencil to default values
    const float clearColor[4] = { 0.071f, 0.040f, 0.561f, 1.0f };

    m_d3dContext->ClearRenderTargetView(
        m_intermediateTextureRenderTargetView.Get(),
        clearColor
        );

    m_d3dContext->ClearDepthStencilView(
        m_d3dDepthStencilView.Get(),
        D3D11_CLEAR_DEPTH,
        1.0f,
        0
        );

    m_d3dContext->IASetInputLayout(m_inputLayout.Get());

    // set the vertex and index buffers, and specify the way they define geometry
    unsigned int stride = sizeof(BasicVertex);
    unsigned int offset = 0;

    m_d3dContext->IASetVertexBuffers(
        0,                              // starting at the first vertex buffer slot
        1,                              // set one vertex buffer binding
        m_vertexBuffer.GetAddressOf(),
        &stride,                        // specify the size in bytes of a single vertex
        &offset                         // specify the base vertex in the buffer
        );

    // set the index buffer
    m_d3dContext->IASetIndexBuffer(
        m_indexBuffer.Get(),
        DXGI_FORMAT_R16_UINT,   // unsigned short index format
        0                       // specify the base index in the buffer
        );

    // specify the way the vertex and index buffers define geometry
    m_d3dContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

    // set the vertex shader stage state
    m_d3dContext->VSSetShader(
        m_vertexShader.Get(),
        nullptr,                // don't use shader linkage
        0                       // don't use shader linkage
        );

    m_d3dContext->VSSetConstantBuffers(
        0,                              // starting at the first constant buffer slot
        1,                              // set one constant buffer binding
        m_constantBuffer.GetAddressOf()
        );

    // set the pixel shader stage state
    m_d3dContext->PSSetShader(
        m_pixelShader.Get(),
        nullptr,                // don't use shader linkage
        0                       // don't use shader linkage
        );

    m_d3dContext->PSSetShaderResources(
        0,                          // starting at the first shader resource slot
        1,                          // set one shader resource binding
        m_textureShaderResourceView.GetAddressOf()
        );

    m_d3dContext->PSSetSamplers(
        0,                          // starting at the first sampler slot
        1,                          // set one sampler binding
        m_sampler.GetAddressOf()
        );

    // Save the backbuffer viewport
    D3D11_VIEWPORT oldViewPort[1];
    unsigned int numberOfViewPorts = 1;
    m_d3dContext->RSGetViewports(&numberOfViewPorts, oldViewPort);

    // Setup the viewport to match the Intermediate Render Target
    D3D11_VIEWPORT viewPort = {0};
    viewPort.Width  = static_cast<float>(m_intermediateRenderTargetWidth);
    viewPort.Height = static_cast<float>(m_intermediateRenderTargetHeight);
    viewPort.MinDepth = 0.0f;
    viewPort.MaxDepth = 1.0f;
    viewPort.TopLeftX = 0;
    viewPort.TopLeftY = 0;
    m_d3dContext->RSSetViewports(1, &viewPort);

    // draw the cube
    m_d3dContext->DrawIndexed(
        m_indexCount,   // draw all created vertices
        0,              // starting with the first vertex
        0               // and the first index
        );

    // Restore the backbuffer viewport
    m_d3dContext->RSSetViewports(numberOfViewPorts, oldViewPort);

    // post processing: rendered 3D cube frame is now in the intermediate texture buffer

    BrightPassDownFilter(); // render brightness thresholded down-sample version

    RenderGlow();  // apply separable blur to brightness image -> glow

    CombineGlow(); // combine 3D cube frame and glow into backbuffer

    m_sampleOverlay->Render();
}

void D3DPostProcessing::Update(float timeTotal, float timeDelta)
{
    // update the model matrix based on the time
    m_constantBufferData.model = rotationY(-timeTotal * 60.0f);

    // update the view matrix based on the camera position
    // note that for this sample, the camera position is fixed
    m_camera->SetViewParameters(
        float3(0, 1.0f, 2.0f),
        float3(0, 0, 0),
        float3(0, 1, 0));

    m_camera->GetViewMatrix(&m_constantBufferData.view);

    // update the constant buffer with the new data
    m_d3dContext->UpdateSubresource(
        m_constantBuffer.Get(),
        0,
        nullptr,
        &m_constantBufferData,
        0,
        0
        );
}

void D3DPostProcessing::DrawQuad(
    ID3D11PixelShader* pixelShader,
    uint width,
    uint height
    )
{
    // Save the old viewport
    D3D11_VIEWPORT oldViewPort[1];
    unsigned int numberOfViewPorts = 1;
    m_d3dContext->RSGetViewports(&numberOfViewPorts, oldViewPort);

    // Setup the viewport to match the Render Target
    D3D11_VIEWPORT viewPort = {0};
    viewPort.Width = static_cast<float>(width);
    viewPort.Height = static_cast<float>(height);
    viewPort.MinDepth = 0.0f;
    viewPort.MaxDepth = 1.0f;
    viewPort.TopLeftX = 0;
    viewPort.TopLeftY = 0;
    m_d3dContext->RSSetViewports(1, &viewPort);

    unsigned int strides = sizeof(ScreenQuadVertex);
    unsigned int offsets = 0;

    m_d3dContext->IASetInputLayout(m_quadLayout.Get());

    m_d3dContext->IASetVertexBuffers(
        0,
        1,
        m_quadVertexBuffer.GetAddressOf(),
        &strides,
        &offsets
        );

    m_d3dContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP);

    m_d3dContext->VSSetShader(
        m_quadVertexShader.Get(),
        nullptr,
        0
        );

    m_d3dContext->PSSetShader(
        pixelShader,
        nullptr,
        0
        );

    m_d3dContext->Draw(4, 0);  // draw all 4 vertices of the quad, starting with vertex 0

    // Restore the Old viewport
    m_d3dContext->RSSetViewports(numberOfViewPorts, oldViewPort);
}

void D3DPostProcessing::BrightPassDownFilter()
{
    m_d3dContext->OMSetRenderTargets(
        1,
        m_brightPassTextureRenderTargetView.GetAddressOf(),
        nullptr
        );

    m_d3dContext->PSSetShaderResources(
        0,
        1,
        m_intermediateTextureShaderResourceView.GetAddressOf()
        );

    m_d3dContext->PSSetSamplers(
        0,
        1,
        m_linearSampler.GetAddressOf()
        );

    if (m_featureLevel <= D3D_FEATURE_LEVEL_9_3)
    {
        // Bind constant buffer for pixel size
        m_d3dContext->PSSetConstantBuffers(
            0,
            1,
            m_brightPassConstantBuffer.GetAddressOf()
            );

        // Use DX9 pixel shader on a device of DX9 feature level
        DrawQuad(
            m_downScaleBrightPass9PixelShader.Get(),
            m_glowTextureWidth,
            m_glowTextureHeight
            );

        // unbind constant buffer
        ID3D11Buffer* nullConstantBuffers[] = {0};
        m_d3dContext->PSSetConstantBuffers(
            0,
            1,
            nullConstantBuffers
            );

    }
    else
    {
        // use the DX10 or later pixel shader
        DrawQuad(
            m_downScaleBrightPassPixelShader.Get(),
            m_glowTextureWidth,
            m_glowTextureHeight
            );
    }

    ID3D11ShaderResourceView* nullShaderResourceViews[] = { 0, 0 };
    m_d3dContext->PSSetShaderResources(0, 2, nullShaderResourceViews);
}

float GaussianDistribution(float x, float y, float rho)
{
    float g = 1.0f / sqrtf(2.0f * PI_F * rho * rho);
    g *= expf(- (x * x + y * y) / (2 * rho * rho));

    return g;
}

// The separable blur function uses a gaussian weighting function applied to 15 (blurKernelSpan) texels centered on the texture
// coordinate currently being processed. These are applied along a row (horizontal) or column (vertical). Element blurKernelMidPoint aligns with the current texel.
// Because the offsets and weights are symmetrical about the center texel, the offsets can be computed for one "side" and mirrored with
// a sign change for the other "side". Similarly the weights can be computed for one "side" and copied symmetrically to the other "side".

void GetSampleOffsetsWeightsForBlur(
    DWORD textureWidthOrHeight,
    float textureCoordinateOffsets[blurKernelSpan],
    float4* colorWeights,
    float deviation,
    float multiplier
    )
{
    float texelWidthOrHeight = 1.0f / static_cast<float>(textureWidthOrHeight);

    // Fill the center texel
    float weight = 1.0f * GaussianDistribution(0, 0, deviation);
    colorWeights[blurKernelMidPoint] = float4(weight, weight, weight, 1.0f);

    textureCoordinateOffsets[blurKernelMidPoint] = 0.0f;

    // Fill one side
    for (int i = 1; i <= blurKernelMidPoint; i++)
    {
        weight = multiplier * GaussianDistribution(static_cast<float>(i), 0, deviation);
        textureCoordinateOffsets[blurKernelMidPoint - i] = -i * texelWidthOrHeight;

        colorWeights[blurKernelMidPoint - i] = float4(weight, weight, weight, 1.0f);
    }

    // Copy to the other side
    for (int i = (blurKernelMidPoint + 1); i < blurKernelSpan; i++)
    {
        colorWeights[i] = colorWeights[(blurKernelSpan - 1) - i];
        textureCoordinateOffsets[i] = - textureCoordinateOffsets[(blurKernelSpan - 1) - i];
    }
}

// Blur using a separable convolution kernel
void D3DPostProcessing::RenderGlow()
{
    // Horizontal Blur
    D3D11_MAPPED_SUBRESOURCE mappedResource;
    DX::ThrowIfFailed (
        m_d3dContext->Map(
            m_glowConstantBuffer.Get(),
            0,
            D3D11_MAP_WRITE_DISCARD,
            0,
            &mappedResource
            )
        );

    ConstantBufferLayoutForGlowPixelShader* glowConstantBuffer =
        reinterpret_cast<ConstantBufferLayoutForGlowPixelShader*>(mappedResource.pData);

    float4* sampleOffsets = glowConstantBuffer->sampleOffsets;
    float4* sampleWeights = glowConstantBuffer->sampleWeights;

    float textureCoordinateOffsets[blurKernelSpan];

    GetSampleOffsetsWeightsForBlur(
        m_glowTextureWidth,
        textureCoordinateOffsets,
        sampleWeights,
        5.0f,
        1.35f
        );

    for (int i = 0; i < 15; i++)
    {
        sampleOffsets[i] = float4(textureCoordinateOffsets[i], 0.0f, 0.0f, 0.0f);
    }

    m_d3dContext->Unmap(
        m_glowConstantBuffer.Get(),
        0
        );

    m_d3dContext->PSSetConstantBuffers(
        0,
        1,
        m_glowConstantBuffer.GetAddressOf()
        );

    m_d3dContext->OMSetRenderTargets(
        1,
        m_glowTextureRenderTargetViews[1].GetAddressOf(),
        nullptr
        );

    m_d3dContext->PSSetShaderResources(
        0,
        1,
        m_brightPassTextureShaderResourceView.GetAddressOf()
        );

    m_d3dContext->PSSetSamplers(
        0,
        1,
        m_pointSampler.GetAddressOf()
        );

    // render horizontally blurred brightness threshold image
    // from m_brightPassTexture to m_glowTexture[1]
    DrawQuad(
        m_glowPixelShader.Get(),
        m_glowTextureWidth,
        m_glowTextureHeight
        );

    // unbind shader resource views
    ID3D11ShaderResourceView* nullShaderResourceViews[] = { 0, 0, 0, 0 };
    m_d3dContext->PSSetShaderResources(
        0,
        4,
        nullShaderResourceViews
        );


    // Vertical Blur
    DX::ThrowIfFailed (
        m_d3dContext->Map(
            m_glowConstantBuffer.Get(),
            0,
            D3D11_MAP_WRITE_DISCARD,
            0,
            &mappedResource
            )
        );

    glowConstantBuffer = reinterpret_cast<ConstantBufferLayoutForGlowPixelShader*>(mappedResource.pData);
    sampleOffsets = glowConstantBuffer->sampleOffsets;
    sampleWeights = glowConstantBuffer->sampleWeights;

    GetSampleOffsetsWeightsForBlur(
        m_glowTextureHeight,
        textureCoordinateOffsets,
        sampleWeights,
        5.0f,
        1.35f
        );

    for (int i = 0; i < 15; i++)
    {
        sampleOffsets[i] = float4(0.0f, textureCoordinateOffsets[i], 0.0f, 0.0f);
    }

    m_d3dContext->Unmap(
        m_glowConstantBuffer.Get(),
        0
        );

    m_d3dContext->PSSetConstantBuffers(
        0,
        1,
        m_glowConstantBuffer.GetAddressOf()
        );


    m_d3dContext->OMSetRenderTargets(
        1,
        m_glowTextureRenderTargetViews[0].GetAddressOf(),
        nullptr
        );

    m_d3dContext->PSSetShaderResources(
        0,
        1,
        m_glowTextureShaderResourceViews[1].GetAddressOf()
        );

    // render vertically blurred horizontally blurred brightness threshold image
    // from m_glowTexture[1] to m_glowTexture[0]
    DrawQuad(
        m_glowPixelShader.Get(),
        m_glowTextureWidth,
        m_glowTextureHeight
        );

    // unbind shader resource views
    m_d3dContext->PSSetShaderResources(
        0,
        4,
        nullShaderResourceViews
        );

    // unbind constant buffer
    ID3D11Buffer* nullConstantBuffers[] = {0};
    m_d3dContext->PSSetConstantBuffers(
        0,
        1,
        nullConstantBuffers
        );
}

void D3DPostProcessing::CombineGlow()
{
    // bind the final backbuffer render target
    m_d3dContext->OMSetRenderTargets(
        1,
        m_d3dRenderTargetView.GetAddressOf(),                 // backbuffer Render Target View
        nullptr                                            // no depth stencil
        );

    ID3D11ShaderResourceView* shaderResourceViews[] =
    {
        m_intermediateTextureShaderResourceView.Get(),
        m_glowTextureShaderResourceViews[0].Get()
    };

    m_d3dContext->PSSetShaderResources(
        0,
        2,
        shaderResourceViews
        );

    ID3D11SamplerState* samplers[] =
    {
        m_pointSampler.Get(),
        m_linearSampler.Get()
    };

    m_d3dContext->PSSetSamplers(
        0,
        2,
        samplers
        );

    DrawQuad(
        m_finalPassPixelShader.Get(),
        static_cast<uint>(m_renderTargetSize.Width),
        static_cast<uint>(m_renderTargetSize.Height)
        );

    // unbind shader resource views
    ID3D11ShaderResourceView* nullShaderResourceViews[] = { 0, 0, 0, 0 };
    m_d3dContext->PSSetShaderResources(
        0,
        4,
        nullShaderResourceViews
        );
}