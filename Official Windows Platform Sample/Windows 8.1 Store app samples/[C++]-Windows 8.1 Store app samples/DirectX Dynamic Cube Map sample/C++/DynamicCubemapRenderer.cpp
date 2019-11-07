//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DynamicCubemapRenderer.h"
#include "DirectXHelper.h"

using namespace DynamicCubemap;
using namespace DirectX;
using namespace Windows::Foundation;

// Initialization.
DynamicCubemapRenderer::DynamicCubemapRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources),
    m_indexCountCube(0),
    m_indexCountSphere(0),
    m_environmentMapWidth(512),
    m_environmentMapHeight(512),
    m_cube1Offset(3.0f, 0.0f, 0.0f),
    m_cube2Offset(0.0f, 3.0f, 0.0f),
    m_sphereOffset(0.0f, 0.0f, 0.0f)
{   
    // Create the cameras.
    m_camera = ref new BasicCamera();
    m_environmentMapCamera = ref new BasicCamera();

    // Allocate and initialize a controller object.
    m_controller = ref new LonLatController();
    m_controller->Initialize(m_deviceResources->GetWindow());
    m_controller->SetPosition(StartPosition);

    CreateDeviceDependentResources();
    CreateWindowSizeDependentResources();
}

// Initialization.
void DynamicCubemapRenderer::CreateDeviceDependentResources()
{
    // Load texture/shader resources.
    BasicLoader^ loader = ref new BasicLoader(m_deviceResources->GetD3DDevice());

    loader->LoadShader(
        "VertexShaderCube.cso",
        nullptr,
        0,
        &m_vertexShaderCube,
        &m_inputLayoutCube
        );

    loader->LoadShader(
        "VertexShaderSphere.cso",
        nullptr,
        0,
        &m_vertexShaderSphere,
        &m_inputLayoutSphere
        );

    loader->LoadShader(
        "PixelShaderCube.cso",
        &m_pixelShaderCube
        );

    loader->LoadShader(
        "PixelShaderSphere.cso",
        &m_pixelShaderSphere
        );

    loader->LoadTexture(
        "reference.dds",
        &m_texture,
        &m_textureShaderResourceView
        );

    // Create the vertex and index buffers for drawing the cube.
    BasicShapes^ shapes = ref new BasicShapes(m_deviceResources->GetD3DDevice());

    shapes->CreateCube(
        &m_vertexBufferCube,
        &m_indexBufferCube,
        nullptr,
        &m_indexCountCube
        );

    // Create the vertex and index buffers for drawing the sphere.
    shapes->CreateSphere(
        &m_vertexBufferSphere,
        &m_indexBufferSphere,
        nullptr,
        &m_indexCountSphere
        );

    // Create the constant buffer for updating model and camera data.
    CD3D11_BUFFER_DESC constantBufferDescription(sizeof(ModelViewProjectionConstantBuffer), D3D11_BIND_CONSTANT_BUFFER);
    DX::ThrowIfFailed(
        m_deviceResources->GetD3DDevice()->CreateBuffer(
            &constantBufferDescription,
            nullptr,             // Leave the buffer uninitialized
            &m_constantBuffer
            )
        );

    // Create the texture sampler.
    D3D11_SAMPLER_DESC samplerDescription;
    samplerDescription.Filter = D3D11_FILTER_ANISOTROPIC;
    samplerDescription.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDescription.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDescription.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDescription.MipLODBias = 0.0f;
    samplerDescription.MaxAnisotropy = 2;
    samplerDescription.ComparisonFunc = D3D11_COMPARISON_NEVER;
    samplerDescription.BorderColor[0] = 0.0f;
    samplerDescription.BorderColor[1] = 0.0f;
    samplerDescription.BorderColor[2] = 0.0f;
    samplerDescription.BorderColor[3] = 0.0f;
    samplerDescription.MinLOD = 0;      // This allows the use of all mip levels.
    samplerDescription.MaxLOD = D3D11_FLOAT32_MAX;

    DX::ThrowIfFailed(
        m_deviceResources->GetD3DDevice()->CreateSamplerState(
            &samplerDescription,
            &m_sampler
            )
        );

    // Create environment map.
    D3D11_TEXTURE2D_DESC environmentMapTextureDescription;
    ZeroMemory(&environmentMapTextureDescription, sizeof(D3D11_TEXTURE2D_DESC));
    environmentMapTextureDescription.ArraySize = NumberOfFaces;
    environmentMapTextureDescription.BindFlags = D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE;
    environmentMapTextureDescription.Usage = D3D11_USAGE_DEFAULT;
    environmentMapTextureDescription.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
    environmentMapTextureDescription.Width = m_environmentMapWidth;
    environmentMapTextureDescription.Height = m_environmentMapHeight;
    environmentMapTextureDescription.MipLevels = 0;
    environmentMapTextureDescription.SampleDesc.Count = 1;
    environmentMapTextureDescription.SampleDesc.Quality = 0;
    environmentMapTextureDescription.CPUAccessFlags = 0;
    environmentMapTextureDescription.MiscFlags = D3D11_RESOURCE_MISC_GENERATE_MIPS | D3D11_RESOURCE_MISC_TEXTURECUBE;

    DX::ThrowIfFailed(
        m_deviceResources->GetD3DDevice()->CreateTexture2D(
            &environmentMapTextureDescription,
            0,
            &m_environmentMapTexture
            )
        );

    D3D11_RENDER_TARGET_VIEW_DESC environmentMapRenderTargetDescription;
    ZeroMemory(&environmentMapRenderTargetDescription, sizeof(D3D11_RENDER_TARGET_VIEW_DESC));
    environmentMapRenderTargetDescription.Format = environmentMapTextureDescription.Format;
    environmentMapRenderTargetDescription.ViewDimension = D3D11_RTV_DIMENSION_TEXTURE2DARRAY;
    environmentMapRenderTargetDescription.Texture2DArray.MipSlice = 0;
    environmentMapRenderTargetDescription.Texture2DArray.ArraySize = 1;

    for (int i = 0; i < NumberOfFaces; ++i)
    {
        environmentMapRenderTargetDescription.Texture2DArray.FirstArraySlice = i;
        DX::ThrowIfFailed(
            m_deviceResources->GetD3DDevice()->CreateRenderTargetView(
                m_environmentMapTexture.Get(),
                &environmentMapRenderTargetDescription,
                &m_environmentMapRenderTargetView[i]
                )
            );
    }

    D3D11_SHADER_RESOURCE_VIEW_DESC environmentMapShaderResourceViewDescription;
    ZeroMemory(&environmentMapShaderResourceViewDescription, sizeof(D3D11_SHADER_RESOURCE_VIEW_DESC));
    environmentMapShaderResourceViewDescription.Format = environmentMapTextureDescription.Format;
    environmentMapShaderResourceViewDescription.ViewDimension = D3D11_SRV_DIMENSION_TEXTURECUBE;
    environmentMapShaderResourceViewDescription.TextureCube.MostDetailedMip = 0;
    environmentMapShaderResourceViewDescription.TextureCube.MipLevels = -1;

    DX::ThrowIfFailed(
        m_deviceResources->GetD3DDevice()->CreateShaderResourceView(
            m_environmentMapTexture.Get(),
            &environmentMapShaderResourceViewDescription,
            &m_environmentMapShaderResourceView
            )
        );

    CD3D11_TEXTURE2D_DESC environmentMapTextureDepthStencilDescription(
        DXGI_FORMAT_D24_UNORM_S8_UINT,
        m_environmentMapWidth,
        m_environmentMapHeight,
        1,
        1,
        D3D11_BIND_DEPTH_STENCIL
        );

    DX::ThrowIfFailed(
        m_deviceResources->GetD3DDevice()->CreateTexture2D(
            &environmentMapTextureDepthStencilDescription,
            nullptr,
            &m_environmentMapDepthStencilTexture
            )
        );

    CD3D11_DEPTH_STENCIL_VIEW_DESC environmentMapDepthStencilViewDescription(D3D11_DSV_DIMENSION_TEXTURE2D);
    DX::ThrowIfFailed(
        m_deviceResources->GetD3DDevice()->CreateDepthStencilView(
            m_environmentMapDepthStencilTexture.Get(),
            &environmentMapDepthStencilViewDescription,
            &m_environmentMapDepthStencilView
            )
        );
}

// Initialization.
void DynamicCubemapRenderer::CreateWindowSizeDependentResources()
{
    // Update the 3D projection matrix of the scene camera.
    m_camera->SetProjectionParameters(
        90.0f,                                                  // Use a 90-degree vertical field of view.
        m_deviceResources->GetScreenViewport().Width /
        m_deviceResources->GetScreenViewport().Height,          // Specify the aspect ratio of the window.
        0.01f,                                                  // Specify the nearest Z-distance at which to draw vertices.
        100.0f                                                  // Specify the farthest Z-distance at which to draw vertices.
        );

    // Update the 3D projection matrix of the environment camera.
    m_environmentMapCamera->SetProjectionParameters(
        90.0f,                                                  // Use a 90-degree vertical field of view
        static_cast<float>(m_environmentMapHeight) /
        static_cast<float>(m_environmentMapWidth),              // Specify the aspect ratio of the window.
        0.01f,                                                  // Specify the nearest Z-distance at which to draw vertices.
        100.0f                                                  // Specify the farthest Z-distance at which to draw vertices.
        );
}

// Release all references to resources that depend on the graphics device.
// This method is invoked when the device is lost and resources are no longer usable.
void DynamicCubemapRenderer::ReleaseDeviceDependentResources()
{
    m_texture.Reset();
    m_textureShaderResourceView.Reset();
    m_sampler.Reset();
    m_inputLayoutCube.Reset();
    m_vertexBufferCube.Reset();
    m_indexBufferCube.Reset();
    m_vertexShaderCube.Reset();
    m_pixelShaderCube.Reset();
    m_inputLayoutSphere.Reset();
    m_vertexBufferSphere.Reset();
    m_indexBufferSphere.Reset();
    m_vertexShaderSphere.Reset();
    m_pixelShaderSphere.Reset();
    m_constantBuffer.Reset();
    m_environmentMapShaderResourceView.Reset();
    m_environmentMapDepthStencilView.Reset();
    m_environmentMapDepthStencilTexture.Reset();
    m_environmentMapTexture.Reset();
    
    for (int i = 0; i < NumberOfFaces; i++)
    {
        m_environmentMapRenderTargetView[i].Reset();
    }
}

// Called once per frame.
void DynamicCubemapRenderer::Update(DX::StepTimer const& timer)
{
    m_controller->Update();

    // Update the view matrix based on the camera position.
    m_camera->SetViewParameters(
        m_controller->get_Position(),       // The point the camera is at.
        m_controller->get_LookPoint(),      // The point to look towards.
        float3(0, 1, 0)                     // The up-vector  (+Y).
        );

    // Update the position of the cube and sphere objects.
    XMStoreFloat4x4(
        &m_cubeRotation, 
        XMMatrixTranspose(
            XMMatrixRotationY(static_cast<float>(timer.GetTotalSeconds()) / 4 * XM_PIDIV4)
            )
        );

    XMStoreFloat4x4(
        &m_cube2Rotation, 
        XMMatrixTranspose(
            XMMatrixRotationX(static_cast<float>(timer.GetTotalSeconds()) / 4 * XM_PIDIV4)
            )
        );

    XMStoreFloat4x4(
        &m_sphereRotation, 
        XMMatrixTranspose(XMMatrixTranslation(0.0f, 0.0f, 0.0f))
        );
}

// Renders one frame.
void DynamicCubemapRenderer::Render()
{
    // Save the old render target and depth stencil buffer views.
    Microsoft::WRL::ComPtr<ID3D11RenderTargetView> previousRenderTargetView;
    Microsoft::WRL::ComPtr<ID3D11DepthStencilView> previousDepthStencilView;
    m_deviceResources->GetD3DDeviceContext()->OMGetRenderTargets(
        1, 
        previousRenderTargetView.GetAddressOf(), 
        &previousDepthStencilView
        );

    // Save the old viewport
    D3D11_VIEWPORT previousViewport;
    UINT numberOfPreviousViewports = 1;
    m_deviceResources->GetD3DDeviceContext()->RSGetViewports(
        &numberOfPreviousViewports, 
        &previousViewport
        );

    // Create the EnvironmentMap Viewport.
    D3D11_VIEWPORT environmentMapViewport;
    environmentMapViewport.TopLeftX = 0.0f;
    environmentMapViewport.TopLeftY = 0.0f;
    environmentMapViewport.Width = static_cast<float>(m_environmentMapWidth);
    environmentMapViewport.Height = static_cast<float>(m_environmentMapHeight);
    environmentMapViewport.MinDepth = 0.0f;
    environmentMapViewport.MaxDepth = 1.0f;

    // Define the look vector for each Environment Map face.
    DirectX::XMFLOAT3 environmentMapTargets[NumberOfFaces] = {
        DirectX::XMFLOAT3(-1, 0, 0),  // Point camera at negative x axis.
        DirectX::XMFLOAT3(1, 0, 0),   // Point camera at positive x axis.
        DirectX::XMFLOAT3(0, 1, 0),   // Point camera at positive y axis.
        DirectX::XMFLOAT3(0, -1, 0),  // Point camera at negative y axis.
        DirectX::XMFLOAT3(0, 0, 1),   // Point camera at positive z axis.
        DirectX::XMFLOAT3(0, 0, -1)   // Point camera at negative z axis.
    };

    // Define the up vector for each Environment Map face.
    DirectX::XMFLOAT3 environmentMapUp[NumberOfFaces] = {
        DirectX::XMFLOAT3(0, 1, 0),
        DirectX::XMFLOAT3(0, 1, 0),
        DirectX::XMFLOAT3(0, 0, -1),
        DirectX::XMFLOAT3(0, 0, 1),
        DirectX::XMFLOAT3(0, 1, 0),
        DirectX::XMFLOAT3(0, 1, 0)
    };

    XMVECTORF32 environmentMapClearColors[NumberOfFaces] = {
        DirectX::Colors::Yellow,
        DirectX::Colors::Green,
        DirectX::Colors::Blue,
        DirectX::Colors::Red,
        DirectX::Colors::Snow,
        DirectX::Colors::Magenta
    };

    // Start to render the environment map faces, one face at a time.
    m_deviceResources->GetD3DDeviceContext()->RSSetViewports(1, &environmentMapViewport);
    for (int view = 0; view < NumberOfFaces; ++view)
    {
        m_deviceResources->GetD3DDeviceContext()->ClearRenderTargetView(
            m_environmentMapRenderTargetView[view].Get(),
            environmentMapClearColors[view]
            );

        m_deviceResources->GetD3DDeviceContext()->ClearDepthStencilView(
            m_environmentMapDepthStencilView.Get(),
            D3D11_CLEAR_DEPTH,
            1.0f,
            0
            );

        m_deviceResources->GetD3DDeviceContext()->OMSetRenderTargets(
            1,
            m_environmentMapRenderTargetView[view].GetAddressOf(),
            m_environmentMapDepthStencilView.Get()
            );

        // Set the environment map camera to look at the specified look vector.  We will use the environment map 
        // to create a reflection of the world onto the sphere, so we set the camera's position to match where
        // the sphere is located.
        m_environmentMapCamera->SetViewParameters(
            float3(0, 0, 0),
            float3(environmentMapTargets[view].x, environmentMapTargets[view].y, environmentMapTargets[view].z),
            float3(environmentMapUp[view].x, environmentMapUp[view].y, environmentMapUp[view].z)
            );

        m_environmentMapCamera->GetViewMatrix(reinterpret_cast<float4x4*>(&m_constantBufferData.view));
        m_environmentMapCamera->GetProjectionMatrix(reinterpret_cast<float4x4*>(&m_constantBufferData.projection));

        RenderScene();
    }

    // Restore old view port.
    m_deviceResources->GetD3DDeviceContext()->RSSetViewports(1, &previousViewport);

    // Restore old render target and depth stencil buffer views.
    m_deviceResources->GetD3DDeviceContext()->OMSetRenderTargets(
        1,
        previousRenderTargetView.GetAddressOf(),
        previousDepthStencilView.Get()
        );

    // Generate Mip Maps.
    m_deviceResources->GetD3DDeviceContext()->GenerateMips(m_environmentMapShaderResourceView.Get());

    m_deviceResources->GetD3DDeviceContext()->ClearRenderTargetView(
        m_deviceResources->GetBackBufferRenderTargetView(),
        DirectX::Colors::CornflowerBlue
        );

    m_deviceResources->GetD3DDeviceContext()->ClearDepthStencilView(
        m_deviceResources->GetDepthStencilView(),
        D3D11_CLEAR_DEPTH,
        1.0f,
        0
        );

    // Set render targets to the screen.
    ID3D11RenderTargetView *const targets[1] = { m_deviceResources->GetBackBufferRenderTargetView() };
    m_deviceResources->GetD3DDeviceContext()->OMSetRenderTargets(
        1,
        targets,
        m_deviceResources->GetDepthStencilView()
        );

    // Change cameras from the environment map camera to the world camera.
    m_camera->GetViewMatrix(reinterpret_cast<float4x4*>(&m_constantBufferData.view));
    m_camera->GetProjectionMatrix(reinterpret_cast<float4x4*>(&m_constantBufferData.projection));

    // Account for the orientation of the display in the view matrix.
    XMFLOAT4X4 orientation = m_deviceResources->GetOrientationTransform3D();
    XMStoreFloat4x4(
        &m_constantBufferData.view,
        XMMatrixMultiply(
            XMLoadFloat4x4(&m_constantBufferData.view),
            XMLoadFloat4x4(&orientation)
            )
        );

    RenderScene();

    // Since the sphere is the object which we reflect the world onto, we must render it after
    // the environment map is rendered.
    RenderSphereModel(m_sphereOffset, m_sphereRotation);
}

void DynamicCubemapRenderer::RenderScene()
{
    // Render two cubes.
    RenderCubeModel(m_cube1Offset, m_cubeRotation);
    RenderCubeModel(m_cube2Offset, m_cube2Rotation);
}

void DynamicCubemapRenderer::RenderCubeModel(DirectX::XMFLOAT3 positionOffset, DirectX::XMFLOAT4X4 rotation)
{
    XMStoreFloat4x4(
        &m_constantBufferData.model, 
        XMMatrixTranspose(
            XMMatrixMultiply(
                XMMatrixTranslation(
                    positionOffset.x, 
                    positionOffset.y, 
                    positionOffset.z
                    ),
                XMLoadFloat4x4(&rotation)
                )
            )
        );

    m_deviceResources->GetD3DDeviceContext()->IASetInputLayout(m_inputLayoutCube.Get());

    m_deviceResources->GetD3DDeviceContext()->UpdateSubresource(
        m_constantBuffer.Get(),
        0,
        NULL,
        &m_constantBufferData,
        0,
        0
        );

    // Set the vertex and index buffers, and specify the way they define geometry.
    unsigned int stride = sizeof(BasicVertex) ;
    unsigned int offset = 0;

    m_deviceResources->GetD3DDeviceContext()->IASetVertexBuffers(
        0,                      // Starting at the first vertex buffer slot.
        1,                      // Set one vertex buffer binding.
        m_vertexBufferCube.GetAddressOf(),
        &stride,                // Specify the size in bytes of a single vertex.
        &offset                 // Specify the base vertex in the buffer.
        );

    // Set the index buffer.
    m_deviceResources->GetD3DDeviceContext()->IASetIndexBuffer(
        m_indexBufferCube.Get(),
        DXGI_FORMAT_R16_UINT,   // Unsigned short index format.
        0                       // Specify the base index in the buffer.
        );

    // Specify the way the vertex and index buffers define geometry.
    m_deviceResources->GetD3DDeviceContext()->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

    // Set the vertex shader stage state.
    m_deviceResources->GetD3DDeviceContext()->VSSetShader(
        m_vertexShaderCube.Get(),
        nullptr,                // Don't use shader linkage.
        0                       // Don't use shader linkage.
        );

    m_deviceResources->GetD3DDeviceContext()->VSSetConstantBuffers(
        0,                      // Starting at the first constant buffer slot.
        1,                      // Set one constant buffer binding.
        m_constantBuffer.GetAddressOf()
        );

    // Set the pixel shader stage state.
    m_deviceResources->GetD3DDeviceContext()->PSSetShader(
        m_pixelShaderCube.Get(),
        nullptr,                // Don't use shader linkage.
        0                       // Don't use shader linkage.
        );

    m_deviceResources->GetD3DDeviceContext()->PSSetShaderResources(
        0,                     // Starting at the first shader resource slot.
        1,                     // Set one shader resource binding.
        m_textureShaderResourceView.GetAddressOf()
        );

    m_deviceResources->GetD3DDeviceContext()->PSSetSamplers(
        0,                     // Starting at the first sampler slot.
        1,                     // Set one sampler binding.
        m_sampler.GetAddressOf()
        );

    // Draw the cube.
    m_deviceResources->GetD3DDeviceContext()->DrawIndexed(
        m_indexCountCube,      // Draw all created vertices.
        0,                     // Starting with the first vertex
        0                      // and the first index.
        );
}

void DynamicCubemapRenderer::RenderSphereModel(DirectX::XMFLOAT3 positionOffset, DirectX::XMFLOAT4X4 rotation)
{
    XMStoreFloat4x4(
        &m_constantBufferData.model, 
        XMMatrixTranspose(
            XMMatrixMultiply(
                XMMatrixTranslation(
                    positionOffset.x, 
                    positionOffset.y, 
                    positionOffset.z
                ),
                XMLoadFloat4x4(&rotation)
                )
            )
        );

    m_deviceResources->GetD3DDeviceContext()->UpdateSubresource(
        m_constantBuffer.Get(),
        0,
        NULL,
        &m_constantBufferData,
        0,
        0
        );

    UINT stride = sizeof(BasicVertex) ;
    UINT offset = 0;
    m_deviceResources->GetD3DDeviceContext()->IASetVertexBuffers(
        0,
        1,
        m_vertexBufferSphere.GetAddressOf(),
        &stride,
        &offset
        );

    m_deviceResources->GetD3DDeviceContext()->IASetIndexBuffer(
        m_indexBufferSphere.Get(),
        DXGI_FORMAT_R16_UINT,
        0
        );

    m_deviceResources->GetD3DDeviceContext()->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

    m_deviceResources->GetD3DDeviceContext()->IASetInputLayout(m_inputLayoutSphere.Get());

    m_deviceResources->GetD3DDeviceContext()->VSSetShader(
        m_vertexShaderSphere.Get(),
        nullptr,
        0
        );

    m_deviceResources->GetD3DDeviceContext()->VSSetConstantBuffers(
        0,
        1,
        m_constantBuffer.GetAddressOf()
        );

    m_deviceResources->GetD3DDeviceContext()->PSSetShader(
        m_pixelShaderSphere.Get(),
        nullptr,
        0
        );

    m_deviceResources->GetD3DDeviceContext()->PSSetShaderResources(
        0,                          // Starting at the first shader resource slot.
        1,                          // Set one shader resource binding.
        m_environmentMapShaderResourceView.GetAddressOf()
        );

    m_deviceResources->GetD3DDeviceContext()->PSSetSamplers(
        0,                          // Starting at the first sampler slot.
        1,                          // Set one sampler binding.
        m_sampler.GetAddressOf()
        );

    m_deviceResources->GetD3DDeviceContext()->DrawIndexed(
        m_indexCountSphere,
        0,
        0
        );

    ID3D11ShaderResourceView* nullViews[1] = { nullptr };
    m_deviceResources->GetD3DDeviceContext()->PSSetShaderResources(
        0,                     // Starting at the first shader resource slot.
        1,                     // Set one shader resource binding.
        nullViews
        );
}