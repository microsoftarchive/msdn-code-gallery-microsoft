//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "TextInterop.h"
#include "BasicShapes.h"
#include "BasicLoader.h"

using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;

TextInterop::TextInterop()
{
}

void TextInterop::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();
}

void TextInterop::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    String^ title = "Direct3D-Direct2D interop sample";

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        title
        );

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

    // Create a Direct3D texture and view, which will be used to texture a spinning cube.

    CD3D11_TEXTURE2D_DESC textureDesc(
        DXGI_FORMAT_B8G8R8A8_UNORM,
        512,        // Width
        512,        // Height
        1,          // MipLevels
        1,          // ArraySize
        D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_RENDER_TARGET
        );

    DX::ThrowIfFailed(
        m_d3dDevice->CreateTexture2D(
            &textureDesc,
            nullptr,
            &m_texture
            )
        );

    CD3D11_SHADER_RESOURCE_VIEW_DESC shaderResourceViewDesc(
        m_texture.Get(),
        D3D11_SRV_DIMENSION_TEXTURE2D
        );

    DX::ThrowIfFailed(
        m_d3dDevice->CreateShaderResourceView(
            m_texture.Get(),
            &shaderResourceViewDesc,
            &m_textureShaderResourceView
            )
        );

    // Use D2D to attach to and draw some text to the cube texture.  Note that the cube
    // texture is DPI-independent, so when drawing content to it using D2D, a fixed DPI
    // value should be used.

    const float dxgiDpi = 96.0f;

    m_d2dContext->SetDpi(dxgiDpi, dxgiDpi);

    D2D1_BITMAP_PROPERTIES1 bitmapProperties =
        D2D1::BitmapProperties1(
            D2D1_BITMAP_OPTIONS_TARGET | D2D1_BITMAP_OPTIONS_CANNOT_DRAW,
            D2D1::PixelFormat(DXGI_FORMAT_B8G8R8A8_UNORM, D2D1_ALPHA_MODE_PREMULTIPLIED),
            dxgiDpi,
            dxgiDpi
            );

    ComPtr<ID2D1Bitmap1> cubeTextureTarget;
    ComPtr<IDXGISurface> cubeTextureSurface;
    DX::ThrowIfFailed(
        m_texture.As(&cubeTextureSurface)
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateBitmapFromDxgiSurface(
            cubeTextureSurface.Get(),
            &bitmapProperties,
            &cubeTextureTarget
            )
        );

    m_d2dContext->SetTarget(cubeTextureTarget.Get());

    ComPtr<ID2D1SolidColorBrush> whiteBrush;
    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::White),
            &whiteBrush
            )
        );

    D2D1_SIZE_F renderTargetSize = m_d2dContext->GetSize();

    m_d2dContext->BeginDraw();

    m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::Red));

    ComPtr<IDWriteTextFormat> textFormat;

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Verdana",
            nullptr,
            DWRITE_FONT_WEIGHT_LIGHT,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            52,
            L"en-US", // locale
            &textFormat
            )
        );

    // Center the text horizontally.
    DX::ThrowIfFailed(
        textFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER)
        );

    // Center the text vertically.
    DX::ThrowIfFailed(
        textFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER)
        );

    m_d2dContext->DrawText(
        title->Data(),
        title->Length(),
        textFormat.Get(),
        D2D1::RectF(0.0f, 0.0f, renderTargetSize.width, renderTargetSize.height),
        whiteBrush.Get()
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    // create the sampler
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
            &m_sampler
            )
        );

    m_camera = ref new BasicCamera();
}

void TextInterop::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // update the 3D projection matrix
    m_camera->SetProjectionParameters(
        70.0f,                                                  // use a 70-degree vertical field of view
        m_renderTargetSize.Width / m_renderTargetSize.Height,  // specify the aspect ratio of the window
        0.01f,                                                  // specify the nearest Z-distance at which to draw vertices
        100.0f                                                  // specify the farthest Z-distance at which to draw vertices
        );

    // save the new projection matrix to the constant buffer data struct
    m_camera->GetProjectionMatrix(&m_constantBufferData.projection);

    m_sampleOverlay->UpdateForWindowSizeChange();
}

void TextInterop::Render()
{
    // bind the render targets
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
        0,
        1,
        m_vertexBuffer.GetAddressOf(),
        &stride,
        &offset
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

    // draw the cube
    m_d3dContext->DrawIndexed(
        m_indexCount,   // draw all created vertices
        0,              // starting with the first vertex
        0               // and the first index
        );

    m_sampleOverlay->Render();
}

void TextInterop::Update(float timeTotal, float timeDelta)
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
