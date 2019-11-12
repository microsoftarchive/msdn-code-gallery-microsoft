//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "Direct3DTutorial.h"
#include "BasicShapes.h"
#include "BasicLoader.h"

using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;

Direct3DTutorial::Direct3DTutorial()
{
}

void Direct3DTutorial::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();
}

void Direct3DTutorial::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    BasicLoader^ loader = ref new BasicLoader(m_d3dDevice.Get());

    loader->LoadShader(
        L"SimpleVertexShader.cso",
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

    // create the constant buffer for updating model and camera data
    CD3D11_BUFFER_DESC constantBufferDesc(sizeof(ConstantBuffer), D3D11_BIND_CONSTANT_BUFFER);
    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &constantBufferDesc,
            nullptr,             // leave the buffer uninitialized
            &m_constantBuffer
            )
        );

    loader->LoadShader(
        L"SimplePixelShader.cso",
        &m_pixelShader
        );

    loader->LoadTexture(
        L"reftexture.dds",
        &m_texture,
        &m_textureSRV
        );

    // create the sampler
    D3D11_SAMPLER_DESC samplerDesc;
    ZeroMemory(&samplerDesc, sizeof(D3D11_SAMPLER_DESC));
    samplerDesc.Filter = D3D11_FILTER_ANISOTROPIC;
    samplerDesc.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.MipLODBias = 0.0f;
    // use 4x on feature level 9.2 and above, otherwise use only 2x
    samplerDesc.MaxAnisotropy = m_featureLevel > D3D_FEATURE_LEVEL_9_1 ? 4 : 2;
    samplerDesc.ComparisonFunc = D3D11_COMPARISON_NEVER;
    samplerDesc.BorderColor[0] = 0.0f;
    samplerDesc.BorderColor[1] = 0.0f;
    samplerDesc.BorderColor[2] = 0.0f;
    samplerDesc.BorderColor[3] = 0.0f;
    // allow use of all mip levels
    samplerDesc.MinLOD = 0;
    samplerDesc.MaxLOD = D3D11_FLOAT32_MAX;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateSamplerState(
            &samplerDesc,
            &m_sampler)
        );

    m_camera = ref new BasicCamera();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "C++ and Direct3D tutorial sample for Windows Store apps"
        );
}

void Direct3DTutorial::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // update the 3D projection matrix
    m_camera->SetProjectionParameters(
        70.0f,                                                // use a 70-degree vertical field of view
        m_renderTargetSize.Width / m_renderTargetSize.Height, // specify the aspect ratio of the window
        0.01f,                                                // specify the nearest Z-distance at which to draw vertices
        100.0f                                                // specify the farthest Z-distance at which to draw vertices
        );

    // save the new projection matrix to the constant buffer data struct
    m_camera->GetProjectionMatrix(&m_constantBufferData.projection);

    m_sampleOverlay->UpdateForWindowSizeChange();
}

void Direct3DTutorial::Update(float timeTotal, float timeDelta)
{
    // update the model matrix based on the time
    m_constantBufferData.model = rotationY(-timeTotal * 60.0f);

    // update the view matrix based on the camera position
    // note that for this sample, the camera position is fixed
    m_camera->SetViewParameters(
        float3(0, 1.0f, 2.0f),
        float3(0, 0, 0),
        float3(0, 1, 0)
        );

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

void Direct3DTutorial::Render()
{
    m_d3dContext->OMSetRenderTargets(
        1,
        m_d3dRenderTargetView.GetAddressOf(),
        m_d3dDepthStencilView.Get()
        );

    // clear both the render target and depth stencil to default values
    const float ClearColor[4] = { 0.071f, 0.040f, 0.561f, 1.0f };
    m_d3dContext->ClearRenderTargetView(
        m_d3dRenderTargetView.Get(),
        ClearColor
        );

    m_d3dContext->ClearDepthStencilView(
        m_d3dDepthStencilView.Get(),
        D3D11_CLEAR_DEPTH,
        1.0f,
        0
        );

    m_d3dContext->IASetInputLayout(m_inputLayout.Get());

    // set the vertex and index buffers, and specify the way they define geometry
    UINT stride = sizeof(BasicVertex);
    UINT offset = 0;
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
        m_textureSRV.GetAddressOf()
        );

    m_d3dContext->PSSetSamplers(
        0,                          // starting at the first sampler slot
        1,                          // set one sampler binding
        m_sampler.GetAddressOf()
        );

    // draw the cube
    m_d3dContext->DrawIndexed(
        m_indexCount,   // draw all created vertices
        0,              // starting with the first vertex
        0               // and the first index
        );

    m_sampleOverlay->Render();
}
