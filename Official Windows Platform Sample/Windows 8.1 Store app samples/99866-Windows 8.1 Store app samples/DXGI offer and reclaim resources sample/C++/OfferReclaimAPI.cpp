//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "OfferReclaimAPI.h"
#include "BasicShapes.h"
#include "BasicLoader.h"

using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;


OfferReclaimAPI::OfferReclaimAPI()
{
}

void OfferReclaimAPI::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();
}

void OfferReclaimAPI::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        L"DXGI resource offer and reclaim sample"
        );

    BasicLoader^ loader = ref new BasicLoader(m_d3dDevice.Get());

    loader->LoadShader(
        L"SimpleVertexShader.cso",
        nullptr,
        0,
        &m_vertexShader,
        &m_inputLayout
        );

    // Create the vertex and index buffers for drawing the cube.

    BasicShapes^ shapes = ref new BasicShapes(m_d3dDevice.Get());

    shapes->CreateCube(
        &m_vertexBuffer,
        &m_indexBuffer,
        nullptr,
        &m_indexCount
        );

    // Create the constant buffer for updating model and camera data.
    CD3D11_BUFFER_DESC constantBufferDescription(sizeof(ConstantBuffer), D3D11_BIND_CONSTANT_BUFFER);
    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &constantBufferDescription,
            nullptr,             // leave the buffer uninitialized
            &m_constantBuffer
            )
        );

    loader->LoadShader(
        L"SimplePixelShader.cso",
        &m_pixelShader
        );

    loader->LoadTexture(
        L"texture.dds",
        &m_texture,
        &m_textureSRV
        );

    // Create the sampler.
    D3D11_SAMPLER_DESC samplerDescription;
    ZeroMemory(&samplerDescription, sizeof(D3D11_SAMPLER_DESC));
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
    // allow use of all mip levels
    samplerDescription.MinLOD = 0;
    samplerDescription.MaxLOD = D3D11_FLOAT32_MAX;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateSamplerState(
            &samplerDescription,
            &m_sampler)
        );

    m_Camera = ref new BasicCamera();

    // To test the Reclaim code, force the discard of the allocation by setting the flag - D3D11_DEBUG_FEATURE_ALWAYS_DISCARD_OFFERED_RESOURCE
    // Also, enable Debug Layers by passing in the D3D11_CREATE_DEVICE_DEBUG flag into "creationFlags" in the CreateDevice call (see DirectXBase.cpp)
#ifdef _DEBUG
    ComPtr<ID3D11Debug> pDebug;
    DX::ThrowIfFailed(m_d3dDevice.As(&pDebug));
    pDebug->SetFeatureMask(D3D11_DEBUG_FEATURE_ALWAYS_DISCARD_OFFERED_RESOURCE);
#endif

}

void OfferReclaimAPI::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // update the 3D projection matrix
    m_Camera->SetProjectionParameters(
        70.0f,                                                  // use a 70-degree vertical field of view
        m_renderTargetSize.Width /  m_renderTargetSize.Height,  // specify the aspect ratio of the window
        0.01f,                                                  // specify the nearest Z-distance at which to draw vertices
        100.0f                                                  // specify the farthest Z-distance at which to draw vertices
        );

    // save the new projection matrix to the constant buffer data struct
    m_Camera->GetProjectionMatrix(&m_constantBufferData.projection);

    m_sampleOverlay->UpdateForWindowSizeChange();
}

void OfferReclaimAPI::Render()
{
    UINT stride = sizeof(BasicVertex);
    UINT offset = 0;

    // bind the render targets
    m_d3dContext->OMSetRenderTargets(
        1,
        m_d3dRenderTargetView.GetAddressOf(),
        m_d3dDepthStencilView.Get()
        );

    // clear both the render target and depth stencil to default values
    const float ClearColor[4] = {0.071f, 0.040f, 0.561f, 1.0f};

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

void OfferReclaimAPI::Update(float TimeTotal, float TimeDelta)
{
    // update the model matrix based on the time
    m_constantBufferData.model = rotationY(-TimeTotal * 60.0f);

    // update the view matrix based on the camera position
    // note that for this sample, the camera position is fixed
    m_Camera->SetViewParameters(
        float3(0, 1.0f, 2.0f),
        float3(0, 0, 0),
        float3(0, 1, 0)
        );

    m_Camera->GetViewMatrix(&m_constantBufferData.view);

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

// Suspend handler
void OfferReclaimAPI::OnSuspending()
{
    ComPtr<IDXGIDevice2> pDXGIDevice;
    ComPtr<IDXGIResource> pDXGIResource;

    DX::ThrowIfFailed(m_d3dDevice.As(&pDXGIDevice));
    DX::ThrowIfFailed(m_texture.As(&pDXGIResource));

    // Call OfferResources on the resource
    DX::ThrowIfFailed(pDXGIDevice->OfferResources(1, pDXGIResource.GetAddressOf(), DXGI_OFFER_RESOURCE_PRIORITY_NORMAL));
}

// Resume Handler
void OfferReclaimAPI::OnResuming()
{
    static BOOL swapTexture = 1;
    ComPtr<IDXGIDevice2> pDXGIDevice;
    DX::ThrowIfFailed(m_d3dDevice.As(&pDXGIDevice));

    ComPtr<IDXGIResource> pDXGIResource;
    DX::ThrowIfFailed(m_texture.As(&pDXGIResource));

    // Call ReclaimResources API on the resource
    BOOL discarded;
    DX::ThrowIfFailed(pDXGIDevice->ReclaimResources(1, pDXGIResource.GetAddressOf(), &discarded));

    // Usage of ReclaimResources: check for BOOL discarded.
    // If discarded is 0, then it means that the texture resource is still around and we were able to reclaim it.
    // If discarded is 1, then we need to recreate the texture resource.

    if (discarded)
    {
        // Regenerate the texture as it has been discarded.
        BasicLoader^ loader = ref new BasicLoader (m_d3dDevice.Get());
        // Swap the texture on every discard in resume.
        if (swapTexture)
        {
            loader->LoadTexture("texture2.dds", &m_texture, &m_textureSRV);
        }
        else
        {
            loader->LoadTexture("texture.dds", &m_texture, &m_textureSRV);
        }
        swapTexture = !swapTexture;
    }
}

