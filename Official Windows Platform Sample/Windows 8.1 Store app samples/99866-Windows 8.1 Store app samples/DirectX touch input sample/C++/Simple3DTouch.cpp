//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "BasicShapes.h"
#include "BasicLoader.h"
#include "Simple3DTouch.h"

using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Core;

Simple3DTouch::Simple3DTouch()
{
}

void Simple3DTouch::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    // Allocate and initialize a controller object
    m_controller = ref new MoveLookController();
    m_controller->Initialize(m_window.Get());
    m_controller->SetPosition(START_POSITION);

    // Create a TextFormat for use in displaying the text to delineate the move / look control areas.
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_LIGHT,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            15.0f,
            L"en-us",
            &m_textFormat
            )
        );

    DX::ThrowIfFailed(m_textFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_LEADING));
    DX::ThrowIfFailed(m_textFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_NEAR));
}

void Simple3DTouch::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    // Allocate and initialize the sample header
    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "DirectX touch input sample"
        );

    // Create D2D brush for drawing the overlay to show the Move control box.
    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::White),
            &m_textBrush
            )
        );

    // Start loading assets
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

    shapes->CreateBox(
        ROOM_BOUNDS,
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
        L"texture.dds",
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
    samplerDesc.MaxAnisotropy = m_featureLevel >= D3D_FEATURE_LEVEL_9_2 ? 4 : 2;
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

    // create the scene camera
    m_camera = ref new BasicCamera();

}

void Simple3DTouch::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // Initialize

    // allocate and initialize the scene camera
    m_camera = ref new BasicCamera();

    // specify the 3D projection matrix
    m_camera->SetProjectionParameters(
        70.0f,                                                  // use a 70-degree vertical field of view
        m_renderTargetSize.Width /  m_renderTargetSize.Height,  // specify the aspect ratio of the window
        0.01f,                                                  // specify the nearest Z-distance at which to draw vertices
        100.0f                                                  // specify the farthest Z-distance at which to draw vertices
        );

    // save the new projection matrix to the constant buffer data struct
    m_camera->GetProjectionMatrix(&m_constantBufferData.projection);

    m_sampleOverlay->UpdateForWindowSizeChange();

    // Create the text layouts for the strings to delineate the move / look control areas.
    static Platform::String^ moveString = "Move Touch Control Area";
    static Platform::String^ lookString = "Look Touch Control Area";

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextLayout(
            moveString->Data(),
            moveString->Length(),
            m_textFormat.Get(),
            m_windowBounds.Width,
            m_windowBounds.Height,
            &m_moveControlLayout
            )
        );
    DX::ThrowIfFailed(
        m_moveControlLayout->GetMetrics(&m_moveMetrics)
        );

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextLayout(
            lookString->Data(),
            lookString->Length(),
            m_textFormat.Get(),
            m_windowBounds.Width,
            m_windowBounds.Height,
            &m_lookControlLayout
            )
        );
    DX::ThrowIfFailed(
        m_lookControlLayout->GetMetrics(&m_lookMetrics)
        );
}

void Simple3DTouch::Render()
{
    UINT stride = sizeof(BasicVertex);
    UINT offset = 0;

    // bind the render targets
    m_d3dContext->OMSetRenderTargets(
        1,
        m_d3dRenderTargetView.GetAddressOf(),
        m_d3dDepthStencilView.Get());

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

    m_d2dContext->BeginDraw();
    // Draw the text to delineate the move / look control areas.

    D2D1_POINT_2F moveTextPosition;
    D2D1_POINT_2F lookTextPosition;

    if (m_windowBounds.Width < MoveLookControllerConstants::MoveMaxXCoordinate)
    {
        // The move control spans across the bottom so center the text
        // and stack it.
        moveTextPosition.x = (m_windowBounds.Width - m_moveMetrics.width) / 2.0f;
        moveTextPosition.y = m_windowBounds.Height - (m_moveMetrics.height + 5.0f);

        lookTextPosition.x = (m_windowBounds.Width - m_lookMetrics.width) / 2.0f;
        lookTextPosition.y = MoveLookControllerConstants::MoveMinYCoordinate - (m_lookMetrics.height + 5.0f);
    }
    else
    {
        // Draw the text on the bottom edge centered in each region.
        moveTextPosition.x = (MoveLookControllerConstants::MoveMaxXCoordinate - m_moveMetrics.width) / 2.0f;
        moveTextPosition.y = m_windowBounds.Height - (m_moveMetrics.height + 5.0f);

        lookTextPosition.x = MoveLookControllerConstants::MoveMaxXCoordinate + (m_windowBounds.Width - MoveLookControllerConstants::MoveMaxXCoordinate - m_lookMetrics.width) / 2.0f;
        lookTextPosition.y = m_windowBounds.Height - (m_lookMetrics.height + 5.0f);
    }

    m_d2dContext->DrawTextLayout(
        moveTextPosition,
        m_moveControlLayout.Get(),
        m_textBrush.Get()
        );

    m_d2dContext->DrawTextLayout(
        lookTextPosition,
        m_lookControlLayout.Get(),
        m_textBrush.Get()
        );

    // Draw a rectangle for the touch input for the move control.
    m_d2dContext->DrawRectangle(
        D2D1::RectF(
            0.0f,
            MoveLookControllerConstants::MoveMinYCoordinate,
            MoveLookControllerConstants::MoveMaxXCoordinate,
            m_windowBounds.Height
            ),
        m_textBrush.Get()
        );

    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        // The D2DERR_RECREATE_TARGET indicates there has been a problem with the underlying
        // D3D device.  All subsequent rendering will be ignored until the device is recreated.
        // This error will be propagated and the appropriate D3D error will be returned from the
        // swapchain->Present(...) call.   At that point the sample will recreate the device
        // and all associated resources.  As a result the D2DERR_RECREATE_TARGET doesn't
        // need to be handled here.
        DX::ThrowIfFailed(hr);
    }

    m_sampleOverlay->Render();

}

void Simple3DTouch::Update(float TimeTotal, float TimeDelta)
{

    // update the model matrix based on the time
    m_constantBufferData.model = identity(); // rotationY(-TimeTotal * 60.0f);

    m_controller->Update(m_window.Get());

    float3 camPosition = m_controller->GetPosition();

    // limit position to inside cell to keep from getting lost
    float3 halfBound = ROOM_BOUNDS;
    float3 radius = float3(0.1f, 0.1f, 0.1f);
    halfBound = halfBound - radius;

    if (camPosition.x > halfBound.x)
    {
        camPosition.x = halfBound.x;
    }
    else if (camPosition.x < -halfBound.x)
    {
        camPosition.x = -halfBound.x;
    }

    if (camPosition.y > halfBound.y)
    {
        camPosition.y = halfBound.y;
    }
    else if (camPosition.y < -halfBound.y)
    {
        camPosition.y = -halfBound.y;
    }

    if (camPosition.z > halfBound.z)
    {
        camPosition.z = halfBound.z;
    }
    else if (camPosition.z < -halfBound.z)
    {
        camPosition.z = -halfBound.z;
    }

    // notify controller that we clamped its result
    m_controller->SetPosition(camPosition);

    // update the view matrix based on the camera position
    m_camera->SetViewParameters(
        m_controller->GetPosition(),        // point we are at
        m_controller->GetLookPoint(),       // point to look towards
        float3(0, 1, 0)                     // up-vector
        );

    // get the camera transform for transmission to the GPU
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

void Simple3DTouch::SaveInternalState(_In_ IPropertySet^ state)
{
    m_controller->SaveInternalState(state);
}

void Simple3DTouch::LoadInternalState(_In_ IPropertySet^ state)
{
    m_controller->LoadInternalState(state);
}