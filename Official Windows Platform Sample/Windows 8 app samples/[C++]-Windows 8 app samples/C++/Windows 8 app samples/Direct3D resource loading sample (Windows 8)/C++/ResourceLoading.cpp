//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "ResourceLoading.h"
#include "BasicShapes.h"
#include "BasicLoader.h"

using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Core;

ResourceLoading::ResourceLoading()
{
}

void ResourceLoading::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    m_camera = ref new BasicCamera();

    m_controller = ref new LonLatController();
    m_controller->Initialize(m_window.Get());
    m_controller->SetPosition(float3(0.7f, 0.7f, 0.7f));
}

void ResourceLoading::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    // Create the SampleOverlay, which synchronously loads its required resources.
    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct3D resource loading sample"
        );

    // This flag will keep track of whether or not all application
    // resources have been loaded.  Until all resources are loaded,
    // only the sample overlay will be drawn on the screen.
    m_loadingComplete = false;

    // Create a BasicLoader, and use it to asynchronously load all
    // application resources.  When an output value becomes non-null,
    // this indicates that the asynchronous operation has completed.
    BasicLoader^ loader = ref new BasicLoader(m_d3dDevice.Get());

    auto loadVertexShaderTask = loader->LoadShaderAsync(
        "SimpleVertexShader.cso",
        nullptr,
        0,
        &m_vertexShader,
        &m_inputLayout
        );

    auto loadPixelShaderTask = loader->LoadShaderAsync(
        "SimplePixelShader.cso",
        &m_pixelShader
        );

    auto loadTextureTask = loader->LoadTextureAsync(
        "reftexture.dds",
        nullptr,
        &m_textureSRV
        );

    auto loadMeshTask = loader->LoadMeshAsync(
        "refmesh.vbo",
        &m_vertexBuffer,
        &m_indexBuffer,
        nullptr,
        &m_indexCount
        );

    // The && operator can be used to create a single task that represents
    // a group of multiple tasks. The new task's completed handler will only
    // be called once all associated tasks have completed. In this case, the
    // new task represents a task to load various assets from the package.
    (loadVertexShaderTask && loadPixelShaderTask && loadTextureTask && loadMeshTask).then([=]()
    {
        m_loadingComplete = true;
    });

    CD3D11_BUFFER_DESC constantBufferDesc(sizeof(ConstantBuffer), D3D11_BIND_CONSTANT_BUFFER);
    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &constantBufferDesc,
            nullptr,
            &m_constantBuffer
            )
        );

    CD3D11_SAMPLER_DESC samplerDesc(
        D3D11_FILTER_ANISOTROPIC,
        D3D11_TEXTURE_ADDRESS_WRAP,
        D3D11_TEXTURE_ADDRESS_WRAP,
        D3D11_TEXTURE_ADDRESS_WRAP,
        0.0f,
        m_featureLevel > D3D_FEATURE_LEVEL_9_1 ? 4 : 2,
        D3D11_COMPARISON_NEVER,
        nullptr,
        0,
        D3D11_FLOAT32_MAX
        );
    DX::ThrowIfFailed(
        m_d3dDevice->CreateSamplerState(
            &samplerDesc,
            &m_sampler
            )
        );

    m_constantBufferData.model = identity();
}

void ResourceLoading::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    m_camera->SetProjectionParameters(
        70.0f,
        m_renderTargetSize.Width / m_renderTargetSize.Height,
        0.01f,
        100.0f
        );

    m_camera->GetProjectionMatrix(&m_constantBufferData.projection);

    m_sampleOverlay->UpdateForWindowSizeChange();
}

void ResourceLoading::Update(float timeTotal, float timeDelta)
{
    m_controller->Update();

    m_camera->SetViewParameters(
        m_controller->get_Position(),
        m_controller->get_LookPoint(),
        float3(0, 1, 0)
        );

    m_camera->GetViewMatrix(&m_constantBufferData.view);
}

void ResourceLoading::Render()
{
    m_d3dContext->OMSetRenderTargets(
        1,
        m_d3dRenderTargetView.GetAddressOf(),
        m_d3dDepthStencilView.Get()
        );

    const float clearColor[4] = { 0.071f, 0.040f, 0.561f, 1.0f };

    m_d3dContext->ClearRenderTargetView(
        m_d3dRenderTargetView.Get(),
        clearColor
        );

    m_d3dContext->ClearDepthStencilView(
        m_d3dDepthStencilView.Get(),
        D3D11_CLEAR_DEPTH,
        1.0f,
        0
        );

    // Only render objects if all required resources have been loaded.
    if (m_loadingComplete)
    {
        m_d3dContext->UpdateSubresource(
            m_constantBuffer.Get(),
            0,
            nullptr,
            &m_constantBufferData,
            0,
            0
            );

        UINT stride = sizeof(BasicVertex);
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
            m_constantBuffer.GetAddressOf()
            );

        m_d3dContext->PSSetShader(
            m_pixelShader.Get(),
            nullptr,
            0
            );

        m_d3dContext->PSSetShaderResources(
            0,
            1,
            m_textureSRV.GetAddressOf()
            );

        m_d3dContext->PSSetSamplers(
            0,
            1,
            m_sampler.GetAddressOf()
            );

        m_d3dContext->DrawIndexed(
            m_indexCount,
            0,
            0
            );
    }

    m_sampleOverlay->Render();
}

void ResourceLoading::SaveInternalState(_In_ IPropertySet^ state)
{
    m_controller->SaveInternalState(state);
}

void ResourceLoading::LoadInternalState(_In_ IPropertySet^ state)
{
    m_controller->LoadInternalState(state);
}
