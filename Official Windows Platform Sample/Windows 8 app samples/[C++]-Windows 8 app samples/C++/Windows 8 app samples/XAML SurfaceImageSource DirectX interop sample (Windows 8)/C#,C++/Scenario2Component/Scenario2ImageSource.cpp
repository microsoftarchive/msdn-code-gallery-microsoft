//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "Scenario2ImageSource.h"
#include "DirectXSample.h"
#include "BasicReaderWriter.h"

using namespace Platform;
using namespace Microsoft::WRL;
using namespace Windows::UI;
using namespace DirectX;
using namespace Scenario2Component;

Scenario2ImageSource::Scenario2ImageSource(int pixelWidth, int pixelHeight, bool isOpaque) :
    SurfaceImageSource(pixelWidth, pixelHeight, isOpaque)
{
    m_width = pixelWidth;
    m_height = pixelHeight;

    CreateDeviceIndependentResources();
    CreateDeviceResources();
}

// Initialize resources that are independent of hardware.
void Scenario2ImageSource::CreateDeviceIndependentResources()
{
    // Query for ISurfaceImageSourceNative interface.
    DX::ThrowIfFailed(
        reinterpret_cast<IUnknown*>(this)->QueryInterface(IID_PPV_ARGS(&m_sisNative))
        );
}

// Initialize hardware-dependent resources.
void Scenario2ImageSource::CreateDeviceResources()
{
    // This flag adds support for surfaces with a different color channel ordering
    // than the API default.
    UINT creationFlags = D3D11_CREATE_DEVICE_BGRA_SUPPORT; 

#if defined(_DEBUG)    
    // If the project is in a debug build, enable debugging via SDK Layers.
    creationFlags |= D3D11_CREATE_DEVICE_DEBUG;
#endif

    // This array defines the set of DirectX hardware feature levels this app will support.
    // Note the ordering should be preserved.
    // Don't forget to declare your application's minimum required feature level in its
    // description.  All applications are assumed to support 9.1 unless otherwise stated.
    const D3D_FEATURE_LEVEL featureLevels[] =
    {
        D3D_FEATURE_LEVEL_11_1,
        D3D_FEATURE_LEVEL_11_0,
        D3D_FEATURE_LEVEL_10_1,
        D3D_FEATURE_LEVEL_10_0,
        D3D_FEATURE_LEVEL_9_3,
        D3D_FEATURE_LEVEL_9_2,
        D3D_FEATURE_LEVEL_9_1,
    };

    // Create the DX11 API device object, and get a corresponding context.
    DX::ThrowIfFailed(
        D3D11CreateDevice(
            nullptr,                        // Specify nullptr to use the default adapter.
            D3D_DRIVER_TYPE_HARDWARE,
            nullptr,
            creationFlags,                  // Set debug and Direct2D compatibility flags.
            featureLevels,                  // List of feature levels this app can support.
            ARRAYSIZE(featureLevels),
            D3D11_SDK_VERSION,              // Always set this to D3D11_SDK_VERSION for Metro style apps.
            &m_d3dDevice,                   // Returns the Direct3D device created.
            nullptr,
            &m_d3dContext                   // Returns the device immediate context.
            )
        );

    // Get the Direct3D 11.1 API device.
    ComPtr<IDXGIDevice> dxgiDevice;
    DX::ThrowIfFailed(
        m_d3dDevice.As(&dxgiDevice)
        );

    // Associate the DXGI device with the SurfaceImageSource.
    DX::ThrowIfFailed(
        m_sisNative->SetDevice(dxgiDevice.Get())
        );

    BasicReaderWriter^ reader = ref new BasicReaderWriter();

    // Load the vertex shader.
    auto vsBytecode = reader->ReadData("Scenario2Component\\SimpleVertexShader.cso");
    DX::ThrowIfFailed(
        m_d3dDevice->CreateVertexShader(
            vsBytecode->Data,
            vsBytecode->Length,
            nullptr,
            &m_vertexShader
            )
        );

    // Create input layout for vertex shader.
    const D3D11_INPUT_ELEMENT_DESC vertexDesc[] = 
    {
        { "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 0,  D3D11_INPUT_PER_VERTEX_DATA, 0 },
        { "COLOR",    0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 },
    };
    DX::ThrowIfFailed(
        m_d3dDevice->CreateInputLayout(
            vertexDesc,
            ARRAYSIZE(vertexDesc),
            vsBytecode->Data,
            vsBytecode->Length,
            &m_inputLayout
            )
        );

    // Load the pixel shader.
    auto psBytecode = reader->ReadData("Scenario2Component\\SimplePixelShader.cso");
    DX::ThrowIfFailed(
        m_d3dDevice->CreatePixelShader(
            psBytecode->Data,
            psBytecode->Length,
            nullptr,
            &m_pixelShader
            )
        );

    // Create the constant buffer.
    const CD3D11_BUFFER_DESC constantBufferDesc = CD3D11_BUFFER_DESC(sizeof(ModelViewProjectionConstantBuffer), D3D11_BIND_CONSTANT_BUFFER);
    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &constantBufferDesc,
            nullptr,
            &m_constantBuffer
            )
        );

    // Describe the vertices of the cube.
    VertexPositionColor cubeVertices[] =
        {
            {XMFLOAT3(-0.5f, -0.5f, -0.5f), XMFLOAT3(0.0f, 0.0f, 0.0f)},
            {XMFLOAT3(-0.5f, -0.5f,  0.5f), XMFLOAT3(0.0f, 0.0f, 1.0f)},
            {XMFLOAT3(-0.5f,  0.5f, -0.5f), XMFLOAT3(0.0f, 1.0f, 0.0f)},
            {XMFLOAT3(-0.5f,  0.5f,  0.5f), XMFLOAT3(0.0f, 1.0f, 1.0f)},
            {XMFLOAT3( 0.5f, -0.5f, -0.5f), XMFLOAT3(1.0f, 0.0f, 0.0f)},
            {XMFLOAT3( 0.5f, -0.5f,  0.5f), XMFLOAT3(1.0f, 0.0f, 1.0f)},
            {XMFLOAT3( 0.5f,  0.5f, -0.5f), XMFLOAT3(1.0f, 1.0f, 0.0f)},
            {XMFLOAT3( 0.5f,  0.5f,  0.5f), XMFLOAT3(1.0f, 1.0f, 1.0f)},
        };

    // Create the vertex buffer.
    D3D11_SUBRESOURCE_DATA vertexBufferData = {0};
    vertexBufferData.pSysMem = cubeVertices;
    vertexBufferData.SysMemPitch = 0;
    vertexBufferData.SysMemSlicePitch = 0;    
    const CD3D11_BUFFER_DESC vertexBufferDesc = CD3D11_BUFFER_DESC(sizeof(cubeVertices), D3D11_BIND_VERTEX_BUFFER);
    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &vertexBufferDesc,
            &vertexBufferData,
            &m_vertexBuffer
            )
        );

    // Describe the cube indices.
    unsigned short cubeIndices[] = 
    {
        0,2,1, // -x
        1,2,3,

        4,5,6, // +x
        5,7,6,

        0,1,5, // -y
        0,5,4,

        2,6,7, // +y
        2,7,3,

        0,4,6, // -z
        0,6,2,

        1,3,7, // +z
        1,7,5,
    };
    m_indexCount = ARRAYSIZE(cubeIndices);

    // Create the index buffer.
    D3D11_SUBRESOURCE_DATA indexBufferData = {0};
    indexBufferData.pSysMem = cubeIndices;
    indexBufferData.SysMemPitch = 0;
    indexBufferData.SysMemSlicePitch = 0;
    const CD3D11_BUFFER_DESC indexBufferDesc = CD3D11_BUFFER_DESC(sizeof(cubeIndices), D3D11_BIND_INDEX_BUFFER);
    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &indexBufferDesc,
            &indexBufferData,
            &m_indexBuffer
            )
        );

    // Calculate the aspect ratio and field of view.
    float aspectRatio = (float)m_width / (float)m_height;
    
    float fovAngleY = 70.0f * XM_PI / 180.0f;
    if (aspectRatio < 1.0f)
    {
        fovAngleY /= aspectRatio;
    }

    // Set right-handed perspective projection based on aspect ratio and field of view.
    m_constantBufferData.projection = XMMatrixTranspose(
        XMMatrixPerspectiveFovRH(
            fovAngleY,
            aspectRatio,
            0.01f,
            100.0f
            )
        );

    // Start animating at frame 0.
    m_frameCount = 0;
}

// Begins drawing.
void Scenario2ImageSource::BeginDraw()
{
    POINT offset;
    ComPtr<IDXGISurface> surface;

    // Express target area as a native RECT type.
    RECT updateRectNative; 
    updateRectNative.left = 0;
    updateRectNative.top = 0;
    updateRectNative.right = m_width;
    updateRectNative.bottom = m_height;

    // Begin drawing - returns a target surface and an offset to use as the top left origin when drawing.
    HRESULT beginDrawHR = m_sisNative->BeginDraw(updateRectNative, &surface, &offset);

    if (beginDrawHR == DXGI_ERROR_DEVICE_REMOVED || beginDrawHR == DXGI_ERROR_DEVICE_RESET)
    {
        // If the device has been removed or reset, attempt to recreate it and continue drawing.
        CreateDeviceResources();
        BeginDraw();
    }
    else
    {
        // Notify the caller by throwing an exception if any other error was encountered.
        DX::ThrowIfFailed(beginDrawHR);
    }

    // QI for target texture from DXGI surface.
    ComPtr<ID3D11Texture2D> d3DTexture;
    surface.As(&d3DTexture);

    // Create render target view.
    DX::ThrowIfFailed(
        m_d3dDevice->CreateRenderTargetView(d3DTexture.Get(), nullptr, &m_renderTargetView)
        );

    // Set viewport to the target area in the surface, taking into account the offset returned by BeginDraw.
    D3D11_VIEWPORT viewport;
    viewport.TopLeftX = static_cast<float>(offset.x);
    viewport.TopLeftY = static_cast<float>(offset.y);
    viewport.Width = static_cast<float>(m_width);
    viewport.Height = static_cast<float>(m_height);
    viewport.MinDepth = D3D11_MIN_DEPTH;
    viewport.MaxDepth = D3D11_MAX_DEPTH;
    m_d3dContext->RSSetViewports(1, &viewport);

    // Create depth/stencil buffer descriptor.
    CD3D11_TEXTURE2D_DESC depthStencilDesc(
        DXGI_FORMAT_D24_UNORM_S8_UINT, 
        m_width,
        m_height,
        1,
        1,
        D3D11_BIND_DEPTH_STENCIL
        );

    // Allocate a 2-D surface as the depth/stencil buffer.
    ComPtr<ID3D11Texture2D> depthStencil;
    DX::ThrowIfFailed(
        m_d3dDevice->CreateTexture2D(&depthStencilDesc, nullptr, &depthStencil)
        );

    // Create depth/stencil view based on depth/stencil buffer.
    const CD3D11_DEPTH_STENCIL_VIEW_DESC depthStencilViewDesc = CD3D11_DEPTH_STENCIL_VIEW_DESC(D3D11_DSV_DIMENSION_TEXTURE2D);    
    DX::ThrowIfFailed(
        m_d3dDevice->CreateDepthStencilView(
            depthStencil.Get(),
            &depthStencilViewDesc,
            &m_depthStencilView
            )
        );
}

// Clears the surface with the given color.  This must be called 
// after BeginDraw and before EndDraw for a given update.
void Scenario2ImageSource::Clear(Color color)
{
    // Convert color values.
    const float clearColor[4] = { color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f };
    // Clear render target view with given color.
    m_d3dContext->ClearRenderTargetView(m_renderTargetView.Get(), clearColor);
}

// Clears the surface with the given color.  This must be called
// after BeginDraw and before EndDraw for a given update
void Scenario2ImageSource::RenderNextAnimationFrame()
{
    XMVECTOR eye = XMVectorSet(0.0f, 0.7f, 1.5f, 0.0f); // Define camera position.
    XMVECTOR at = XMVectorSet(0.0f, -0.1f, 0.0f, 0.0f); // Define focus position.
    XMVECTOR up = XMVectorSet(0.0f, 1.0f, 0.0f, 0.0f);  // Define up direction.

    if (m_frameCount == FLT_MAX)
    {
        m_frameCount = 0;
    }

    // Set view based on camera position, focal point, and up direction.
    m_constantBufferData.view = XMMatrixTranspose(XMMatrixLookAtRH(eye, at, up));

    // Rotate around Y axis by (pi/4) * 16ms per elapsed frame.
    m_constantBufferData.model = XMMatrixTranspose(XMMatrixRotationY(m_frameCount++ * 0.016f * XM_PIDIV4));
    
    // Clear depth/stencil view.
    m_d3dContext->ClearDepthStencilView(m_depthStencilView.Get(), D3D11_CLEAR_DEPTH, 1.0f, 0);

    // Set render target.
    m_d3dContext->OMSetRenderTargets(1, m_renderTargetView.GetAddressOf(), m_depthStencilView.Get());

    // Map update to constant buffer.
    m_d3dContext->UpdateSubresource(
        m_constantBuffer.Get(),
        0,
        nullptr,
        &m_constantBufferData,
        0,
        0
        );

    // Set vertex buffer.
    UINT stride = sizeof(VertexPositionColor);
    UINT offset = 0;
    m_d3dContext->IASetVertexBuffers(
        0,
        1,
        m_vertexBuffer.GetAddressOf(),
        &stride,
        &offset
        );

    // Set index buffer.
    m_d3dContext->IASetIndexBuffer(m_indexBuffer.Get(), DXGI_FORMAT_R16_UINT, 0);

    // Set topology to triangle list.
    m_d3dContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

    // Set input layout.
    m_d3dContext->IASetInputLayout(m_inputLayout.Get());

    // Set vertex shader.
    m_d3dContext->VSSetShader(m_vertexShader.Get(), nullptr, 0);

    // Set constant buffer.
    m_d3dContext->VSSetConstantBuffers(0, 1, m_constantBuffer.GetAddressOf());

    // Set pixel shader.
    m_d3dContext->PSSetShader(m_pixelShader.Get(), nullptr, 0);

    // Draw cube faces.
    m_d3dContext->DrawIndexed(m_indexCount, 0, 0);
}

// Ends drawing updates started by a previous BeginDraw call.
void Scenario2ImageSource::EndDraw()
{
    DX::ThrowIfFailed(
        m_sisNative->EndDraw()
        );
}
