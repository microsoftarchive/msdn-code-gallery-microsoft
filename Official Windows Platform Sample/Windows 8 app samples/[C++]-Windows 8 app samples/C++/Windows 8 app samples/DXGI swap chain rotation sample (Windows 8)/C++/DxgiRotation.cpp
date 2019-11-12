//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DxgiRotation.h"
#include "BasicShapes.h"
#include "BasicLoader.h"

using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace D2D1;

void DxgiRotation::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    // Decode a reference 2D image using WIC.
    // This will be used to later create a D2D bitmap.
    ComPtr<IWICBitmapDecoder> decoder;
    DX::ThrowIfFailed(
        m_wicFactory->CreateDecoderFromFilename(
            L"reference2d.png",
            nullptr,
            GENERIC_READ,
            WICDecodeMetadataCacheOnDemand,
            &decoder
            )
        );

    Microsoft::WRL::ComPtr<IWICBitmapFrameDecode> frame;
    DX::ThrowIfFailed(
        decoder->GetFrame(0, &frame)
        );

    DX::ThrowIfFailed(
        m_wicFactory->CreateFormatConverter(&m_referenceBitmapSource)
        );

    DX::ThrowIfFailed(
        m_referenceBitmapSource->Initialize(
            frame.Get(),
            GUID_WICPixelFormat32bppPBGRA,
            WICBitmapDitherTypeNone,
            nullptr,
            0.0f,
            WICBitmapPaletteTypeCustom
            )
        );

    // Create a text format for rendering 2D text.
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_REGULAR,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            18.0f,
            L"en-US",
            &m_textFormat
            )
        );

    DX::ThrowIfFailed(
        m_textFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER)
        );

    DX::ThrowIfFailed(
        m_textFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_NEAR)
        );
}

void DxgiRotation::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    BasicLoader^ loader = ref new BasicLoader(m_d3dDevice.Get());

    loader->LoadShader(
        "SimpleVertexShader.cso",
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
        "SimplePixelShader.cso",
        &m_pixelShader
        );

    loader->LoadTexture(
        "reference3d.png",
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
            &m_sampler
            )
        );

    m_Camera = ref new BasicCamera();

    // Create a 2D bitmap using a reference image.
    DX::ThrowIfFailed(
        m_d2dContext->CreateBitmapFromWicBitmap(
            m_referenceBitmapSource.Get(),
            &m_referenceBitmap
            )
        );

    // Create a brush to draw the reference text with.
    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::DarkGray),
            &m_textBrush
            )
        );

    // Create the Sample Overlay.

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "DXGI swap chain rotation sample"
        );
}

void DxgiRotation::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // update the 3D projection matrix
    m_Camera->SetProjectionParameters(
        70.0f,                                                  // use a 70-degree minimum field of view
        m_renderTargetSize.Width /  m_renderTargetSize.Height,  // specify the aspect ratio of the window
        0.01f,                                                  // specify the nearest Z-distance at which to draw vertices
        100.0f                                                  // specify the farthest Z-distance at which to draw vertices
        );

    // save the new projection matrix to the constant buffer data struct
    m_Camera->GetProjectionMatrix(&m_constantBufferData.projection);

    // rotate the projection matrix as it will be used to render to the rotated swap chain
    m_constantBufferData.projection = mul(m_constantBufferData.projection, m_rotationTransform3D);

    // create a text layout to fill the screen
    Platform::String^ repeatString =
        "Lorem ipsum dolor sit amet, consectetur adipiscing " \
        "elit. Vivamus tempor scelerisque lorem in vehicula. " \
        "Aliquam tincidunt, lacus ut sagittis tristique, turpis " \
        "massa volutpat augue, eu rutrum ligula ante a ante. " \
        "Pellentesque porta, mauris quis interdum vehicula, " \
        "urna sapien ultrices velit, nec venenatis dui odio " \
        "in augue. Cras posuere, enim a cursus convallis, neque " \
        "turpis malesuada erat, ut adipiscing neque tortor ac erat. ";

    // repeat the text enough times to ensure that it fills the window
    Platform::String^ textString = "";
    for (int i = 0; i < 50; i++)
    {
        textString = Platform::String::Concat(textString, repeatString);
    }

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextLayout(
            textString->Data(),                 // Text to be displayed
            textString->Length(),               // Length of the text
            m_textFormat.Get(),                 // DirectWrite Text Format object
            m_windowBounds.Width,               // Width of the Text Layout
            m_windowBounds.Height,              // Height of the Text Layout
            &m_textLayout
            )
        );

    m_sampleOverlay->UpdateForWindowSizeChange();
}

void DxgiRotation::Render()
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

    // render 2D reference image and text
    m_d2dContext->BeginDraw();

    // draw reference text
    m_d2dContext->SetTransform(
        Matrix3x2F::Translation(0.0f, 64.0f) *
        m_rotationTransform2D
        );
    m_d2dContext->DrawTextLayout(
        Point2F(0, 0),
        m_textLayout.Get(),
        m_textBrush.Get()
        );

    // draw reference bitmap at the center of the screen
    m_d2dContext->SetTransform(
        Matrix3x2F::Translation(
            m_windowBounds.Width / 2.0f - 768.0f / 2.0f,
            m_windowBounds.Height / 2.0f - 768.0f / 2.0f
            ) *
        m_rotationTransform2D // apply 2D prerotation transform
        );
    m_d2dContext->DrawBitmap(m_referenceBitmap.Get());

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    // set the vertex and index buffers, and specify the way they define geometry
    m_d3dContext->IASetInputLayout(m_inputLayout.Get());

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

    // Render the Sample Overlay.
    m_sampleOverlay->Render(m_rotationTransform2D);
}

void DxgiRotation::Update(float TimeTotal, float TimeDelta)
{
    // update the model matrix based on the time
    m_constantBufferData.model = rotationY(-TimeTotal * 60.0f);

    // update the view matrix based on the camera position
    // note that for this sample, the camera position is fixed
    m_Camera->SetViewParameters(
        float3(0, 1, 2),
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

