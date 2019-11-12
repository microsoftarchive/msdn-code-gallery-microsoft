//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DXForegroundSwapChainsRenderer.h"

#include "DirectXHelper.h"

static const float ResolutionTextWidth = 500.0f;

using namespace DXForegroundSwapChains;

using namespace DirectX;
using namespace Windows::Foundation;

// Loads vertex and pixel shaders from files and instantiates the cube geometry.
DXForegroundSwapChainsRenderer::DXForegroundSwapChainsRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources),
    m_loadingComplete(false)
{
    // Create text resources.
    DX::ThrowIfFailed(
        m_deviceResources->GetDWriteFactory()->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_THIN,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            16,
            L"en-US",
            &m_textFormat
            )
        );

    CreateDeviceDependentResources();
    CreateWindowSizeDependentResources();
}

// Initialization.
void DXForegroundSwapChainsRenderer::CreateDeviceDependentResources()
{
    auto loadVSTask = DX::ReadDataAsync(L"SimpleVertexShader.cso");
    auto loadPSTask = DX::ReadDataAsync(L"SimplePixelShader.cso");

    auto createVSTask = loadVSTask.then([this](std::vector<byte> fileData)
    {
        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateVertexShader(
                fileData.data(),
                fileData.size(),
                nullptr,
                &m_vertexShader
                )
            );

        const D3D11_INPUT_ELEMENT_DESC vertexDesc[] =
        {
            { "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0,  0, D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "COLOR",    0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 },
        };

        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateInputLayout(
                vertexDesc,
                ARRAYSIZE(vertexDesc),
                fileData.data(),
                fileData.size(),
                &m_inputLayout
                )
            );
    });

    auto createPSTask = loadPSTask.then([this](std::vector<byte> fileData)
    {
        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreatePixelShader(
                fileData.data(),
                fileData.size(),
                nullptr,
                &m_pixelShader
                )
            );

        CD3D11_BUFFER_DESC constantBufferDesc(sizeof(ModelViewProjectionConstantBuffer), D3D11_BIND_CONSTANT_BUFFER);
        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateBuffer(
                &constantBufferDesc,
                nullptr,
                &m_constantBuffer
                )
            );
    });

    auto createCubeTask = Concurrency::create_task([this] ()
    {
        VertexPositionColor cubeVertices[] =
        {
            {XMFLOAT3(-0.5f, -0.5f, -0.5f), XMFLOAT3(0.0f, 0.0f, 0.0f)},
            {XMFLOAT3(-0.5f, -0.5f, 0.5f), XMFLOAT3(0.0f, 0.0f, 1.0f)},
            {XMFLOAT3(-0.5f, 0.5f, -0.5f), XMFLOAT3(0.0f, 1.0f, 0.0f)},
            {XMFLOAT3(-0.5f, 0.5f, 0.5f), XMFLOAT3(0.0f, 1.0f, 1.0f)},
            {XMFLOAT3(0.5f, -0.5f, -0.5f), XMFLOAT3(1.0f, 0.0f, 0.0f)},
            {XMFLOAT3(0.5f, -0.5f, 0.5f), XMFLOAT3(1.0f, 0.0f, 1.0f)},
            {XMFLOAT3(0.5f, 0.5f, -0.5f), XMFLOAT3(1.0f, 1.0f, 0.0f)},
            {XMFLOAT3(0.5f, 0.5f, 0.5f), XMFLOAT3(1.0f, 1.0f, 1.0f)},
        };

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

        unsigned short cubeIndices[] =
        {
            0, 2, 1, // -x
            1, 2, 3,

            4, 5, 6, // +x
            5, 7, 6,

            0, 1, 5, // -y
            0, 5, 4,

            2, 6, 7, // +y
            2, 7, 3,

            0, 4, 6, // -z
            0, 6, 2,

            1, 3, 7, // +z
            1, 7, 5,
        };

        m_indexCount = ARRAYSIZE(cubeIndices);

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

    (createPSTask && createVSTask && createCubeTask).then([this] ()
    {
        m_loadingComplete = true;
    });

    DX::ThrowIfFailed(
        m_deviceResources->GetD2DDeviceContext()->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::White),
            &m_whiteBrush
            )
        );
}

// Initialization.
void DXForegroundSwapChainsRenderer::CreateWindowSizeDependentResources()
{
    float aspectRatio = m_deviceResources->GetLogicalSize().Width / m_deviceResources->GetLogicalSize().Height;
    float fovAngleY = 70.0f * XM_PI / 180.0f;

    // Note that the m_orientationTransform3D matrix is post-multiplied here
    // in order to correctly orient the scene to match the display orientation.
    // This post-multiplication step is required for any draw calls that are
    // made to the swap chain render target. For draw calls to other targets,
    // this transform should not be applied.
    XMStoreFloat4x4(
        &m_constantBufferData.projection,
        XMMatrixTranspose(
            XMMatrixMultiply(
                XMMatrixPerspectiveFovRH(
                    fovAngleY,
                    aspectRatio,
                    0.01f,
                    100.0f
                    ),
                XMLoadFloat4x4(&m_deviceResources->GetOrientationTransform3D())
                )
            )
        );
}

// Release all references to resources that depend on the graphics device.
// This method is invoked when the device is lost and resources are no longer usable.
void DXForegroundSwapChainsRenderer::ReleaseDeviceDependentResources()
{
    m_loadingComplete = false;

    m_inputLayout.Reset();
    m_vertexBuffer.Reset();
    m_indexBuffer.Reset();
    m_vertexShader.Reset();
    m_pixelShader.Reset();
    m_constantBuffer.Reset();

    m_whiteBrush.Reset();
}

// Called once per frame.
void DXForegroundSwapChainsRenderer::Update(DX::StepTimer const& timer)
{
    XMVECTOR eye = XMVectorSet(0.0f, 0.7f, 1.5f, 0.0f);
    XMVECTOR at = XMVectorSet(0.0f, -0.1f, 0.0f, 0.0f);
    XMVECTOR up = XMVectorSet(0.0f, 1.0f, 0.0f, 0.0f);

    XMStoreFloat4x4(&m_constantBufferData.view, XMMatrixTranspose(XMMatrixLookAtRH(eye, at, up)));
    XMStoreFloat4x4(&m_constantBufferData.model, XMMatrixTranspose(XMMatrixRotationY(static_cast<float>(timer.GetTotalSeconds()) * XM_PIDIV4)));
}

// Renders one frame.
void DXForegroundSwapChainsRenderer::Render()
{
    // Only draw the cube once it has loaded (loading is asynchrnous).
    if (!m_loadingComplete)
    {
        return;
    }

    auto d3dContext = m_deviceResources->GetD3DDeviceContext();

    d3dContext->UpdateSubresource(
        m_constantBuffer.Get(),
        0,
        NULL,
        &m_constantBufferData,
        0,
        0
        );

    UINT stride = sizeof(VertexPositionColor);
    UINT offset = 0;
    d3dContext->IASetVertexBuffers(
        0,
        1,
        m_vertexBuffer.GetAddressOf(),
        &stride,
        &offset
        );

    d3dContext->IASetIndexBuffer(
        m_indexBuffer.Get(),
        DXGI_FORMAT_R16_UINT,
        0
        );

    d3dContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

    d3dContext->IASetInputLayout(m_inputLayout.Get());

    d3dContext->VSSetShader(
        m_vertexShader.Get(),
        nullptr,
        0
        );

    d3dContext->VSSetConstantBuffers(
        0,
        1,
        m_constantBuffer.GetAddressOf()
        );

    d3dContext->PSSetShader(
        m_pixelShader.Get(),
        nullptr,
        0
        );

    d3dContext->DrawIndexed(
        m_indexCount,
        0,
        0
        );

    // Display the current resolution of the swap chain(s) if window is sufficiently wide.
    if (m_deviceResources->GetLogicalSize().Width < ResolutionTextWidth)
    {
        return;
    }

    auto d2dContext = m_deviceResources->GetD2DDeviceContext();

    d2dContext->BeginDraw();

    float renderingSizePecentage = m_deviceResources->GetRenderSize();
    UINT backSwapChainWidth = static_cast<UINT>(renderingSizePecentage * m_deviceResources->GetD3DRenderTargetSize().Width);
    UINT backSwapChainHeight = static_cast<UINT>(renderingSizePecentage * m_deviceResources->GetD3DRenderTargetSize().Height);
    Platform::String^ backSwapChainInformation =
        "Background Swap Chain Resolution: " + backSwapChainWidth + "x" + backSwapChainHeight;

    d2dContext->DrawText(
        backSwapChainInformation->Data(),
        backSwapChainInformation->Length(),
        m_textFormat.Get(),
        D2D1::RectF(10.0f, 100.0f, 600.0f, 125.0f),
        m_whiteBrush.Get()
        );

    // If applicable, display the size of the foreground swap chain.
    Platform::String^ foregroundSwapChainInformation =
        "Foreground Swap Chain Resolution: " +
        (m_deviceResources->GetOverlaySupportExists() ?
            m_deviceResources->GetD3DRenderTargetSize().Width + "x" + m_deviceResources->GetD3DRenderTargetSize().Height :
            "N/A (enter to force)");

    d2dContext->DrawText(
        foregroundSwapChainInformation->Data(),
        foregroundSwapChainInformation->Length(),
        m_textFormat.Get(),
        D2D1::RectF(10.0f, 125.0f, 600.0f, 200.0f),
        m_whiteBrush.Get()
        );

    DX::ThrowIfFailed(d2dContext->EndDraw());
}
