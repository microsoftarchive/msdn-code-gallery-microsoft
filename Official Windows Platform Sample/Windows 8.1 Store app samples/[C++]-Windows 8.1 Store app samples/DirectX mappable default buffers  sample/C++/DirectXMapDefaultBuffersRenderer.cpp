//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXMapDefaultBuffersRenderer.h"

#include "DirectXHelper.h"

static const int ProjectileDispatchCount = 4;
static const int ProjectileCountPerDispatch = 256;
static const int ProjectileCount = ProjectileDispatchCount * ProjectileCountPerDispatch;
static const float CollisionTextWidth = 600.0f;
static const float WallThickness = 6.0f;

using namespace DirectXMapDefaultBuffers;

using namespace DirectX;
using namespace Windows::Foundation;

// Loads compute shader, creates buffers to store projectile / collision data.
DirectXMapDefaultBuffersRenderer::DirectXMapDefaultBuffersRenderer(
    const std::shared_ptr<DX::DeviceResources>& deviceResources,
    const std::shared_ptr<SampleOverlay>& sampleOverlay
    ) :
    m_deviceResources(deviceResources),
    m_sampleOverlay(sampleOverlay)
{
    // Create DirectWrite resources that will be used to display the number of collisions
    // the projectiles have had with the walls of the screen.
    DX::ThrowIfFailed(
        m_deviceResources->GetDWriteFactory()->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_LIGHT,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            36.0f,
            L"en-US",
            &m_collisionCountTextFormat
            )
        );

    CreateDeviceDependentResources();
}

// Initialization.
void DirectXMapDefaultBuffersRenderer::CreateDeviceDependentResources()
{
    // Determine whether current Direct3D device can support Mappable Default Buffers functionality. If not, fall back to
    // using Staging Buffers to access data on the CPU.
    D3D11_FEATURE_DATA_D3D11_OPTIONS1 featureOptions;
    DX::ThrowIfFailed(
        m_deviceResources->GetD3DDevice()->CheckFeatureSupport(
            D3D11_FEATURE_D3D11_OPTIONS1,
            &featureOptions,
            sizeof(featureOptions)
            )
        );

    m_cpuAccessMethod = (featureOptions.MapOnDefaultBuffers ? CpuAccessMethod::MapDefaultBuffers : CpuAccessMethod::MapStagingBuffers);

    // Load physics Compute Shader.
    BasicReaderWriter^ basicReaderWriter = ref new BasicReaderWriter();
    Platform::Array<byte>^ physicsShaderData = basicReaderWriter->ReadData("PhysicsShader.cs.cso");
    DX::ThrowIfFailed(
        m_deviceResources->GetD3DDevice()->CreateComputeShader(
            physicsShaderData->Data,
            physicsShaderData->Length,
            nullptr,
            &m_physicsShader
            )
        );

    // Initialize the Direct3D Buffer to store the projectile position data and the Direct3D
    // Unordered Access View to access that data in the phyics Compute Shader.
    InitializeProjectileBuffer();

    // Initialize the Direct3D Buffer to store the collision data and the Direct3D
    // Unordered Access View to access that data in the phyics Compute Shader.
    InitializeCollisionBuffer();

    // Initialize the Staging Buffers if this CPU access method will be used.
    if (m_cpuAccessMethod == CpuAccessMethod::MapStagingBuffers)
    {
        InitializeStagingBuffers();
    }

    // Create Direct2D resources that will be used to render buffer contents to the screen.
    DX::ThrowIfFailed(
        m_deviceResources->GetD2DDeviceContext()->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::White),
            &m_whiteBrush
            )
        );

    // Tell Direct3D to use the already-loaded physics Compute Shader for execution.
    m_deviceResources->GetD3DDeviceContext()->CSSetShader(m_physicsShader.Get(), nullptr, 0);

    // Bind Unordered Access Views for projectile and collusion data to the physics Compute Shader.
    ID3D11UnorderedAccessView* unorderedAccessViews[2] = { m_projectileBufferView.Get(), m_collisionBufferView.Get() };
    m_deviceResources->GetD3DDeviceContext()->CSSetUnorderedAccessViews(0, 2, unorderedAccessViews, nullptr);
}

// Release all references to resources that depend on the graphics device.
// This method is invoked when the device is lost and resources are no longer usable.
void DirectXMapDefaultBuffersRenderer::ReleaseDeviceDependentResources()
{
    m_projectileBuffer.Reset();
    m_projectileBufferView.Reset();
    m_collisionBuffer.Reset();
    m_collisionBufferView.Reset();
    m_physicsShader.Reset();

    m_projectileStagingBuffer.Reset();
    m_projectileStagingBufferView.Reset();
    m_collisionStagingBuffer.Reset();
    m_collisionStagingBufferView.Reset();

    m_whiteBrush.Reset();
}

// Called once per frame.
void DirectXMapDefaultBuffersRenderer::Update(DX::StepTimer const& timer)
{
    // No timing-dependent updates happen on the CPU.
}

// Renders one frame.
void DirectXMapDefaultBuffersRenderer::Render()
{
    if (m_cpuAccessMethod == CpuAccessMethod::MapDefaultBuffers)
    {
        // Mapping a Default Buffer bound to the rendering pipeline will cause the buffer to be unbound
        // to maintain memory consistency. After each Map call, it must be re-bound to the pipeline.
        ID3D11UnorderedAccessView* unorderedAccessViews[2] = { m_projectileBufferView.Get(), m_collisionBufferView.Get() };
        m_deviceResources->GetD3DDeviceContext()->CSSetUnorderedAccessViews(0, 2, unorderedAccessViews, nullptr);
    }

    // Execute physics shader to update projectile positions/collision count.
    m_deviceResources->GetD3DDeviceContext()->Dispatch(ProjectileDispatchCount, 1, 1);

    // Begin rendering destination buffer contents to screen.
    m_deviceResources->GetD2DDeviceContext()->BeginDraw();

    // Apply the 2D transform to account for the display's current orientation.
    m_deviceResources->GetD2DDeviceContext()->SetTransform(m_deviceResources->GetOrientationTransform2D());

    // Render objects.
    RenderObjects();

    // Render walls.
    RenderWalls();

    // Render collision count.
    RenderCollisionCount();

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_deviceResources->GetD2DDeviceContext()->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}

void DirectXMapDefaultBuffersRenderer::RenderObjects()
{
    ID3D11Buffer *bufferToMap = nullptr;
    if (m_cpuAccessMethod == CpuAccessMethod::MapDefaultBuffers)
    {
        // If Mappable Default Buffers functionality is available, directly map the projectile buffer.
        bufferToMap = m_projectileBuffer.Get();
    }
    else
    {
        // Otherwise, copy the projectile buffer to the corresponding staging buffer and map that.
        m_deviceResources->GetD3DDeviceContext()->CopyResource(
            m_projectileStagingBuffer.Get(),
            m_projectileBuffer.Get()
            );

        bufferToMap = m_projectileStagingBuffer.Get();
    }

    // NOTE: The Map call will block until the shader Dispatch has completed on the GPU
    // since the DO_NOT_WAIT flag was not provided. For this reason, any other GPU work
    // that can be done in parallel should be dispatched/drawn before calling Map.
    D3D11_MAPPED_SUBRESOURCE mappedSubresource;
    ZeroMemory(&mappedSubresource, sizeof(mappedSubresource));
    DX::ThrowIfFailed(
        m_deviceResources->GetD3DDeviceContext()->Map(
            bufferToMap,
            0,
            D3D11_MAP_READ,
            0,
            &mappedSubresource
            )
        );

    Projectile* projectiles = reinterpret_cast<Projectile*>(mappedSubresource.pData);
    for (int i = 0; i < ProjectileCount; i++)
    {
        m_deviceResources->GetD2DDeviceContext()->FillEllipse(
            D2D1::Ellipse(
                D2D1::Point2F(
                    projectiles[i].positionX * m_deviceResources->GetLogicalSize().Width,
                    projectiles[i].positionY * m_deviceResources->GetLogicalSize().Height
                    ),
                2.0f,
                2.0f
                ),
                m_whiteBrush.Get()
            );
    }

    m_deviceResources->GetD3DDeviceContext()->Unmap(bufferToMap, 0);
}

void DirectXMapDefaultBuffersRenderer::RenderWalls()
{
    m_deviceResources->GetD2DDeviceContext()->DrawRectangle(
        D2D1::RectF(
            0.0f,
            0.0f,
            m_deviceResources->GetLogicalSize().Width,
            m_deviceResources->GetLogicalSize().Height
            ),
        m_whiteBrush.Get(),
        WallThickness
        );
}

void DirectXMapDefaultBuffersRenderer::RenderCollisionCount()
{
    if (m_deviceResources->GetLogicalSize().Width < CollisionTextWidth)
    {
        // Do not display collision text when window not wide enough.
        return;
    }

    ID3D11Buffer *bufferToMap = nullptr;
    if (m_cpuAccessMethod == CpuAccessMethod::MapDefaultBuffers)
    {
        // If Mappable Default Buffers functionality is available, directly map the collision buffer.
        bufferToMap = m_collisionBuffer.Get();
    }
    else
    {
        // Otherwise, copy collision buffer to a Staging Buffer and map that.
        m_deviceResources->GetD3DDeviceContext()->CopyResource(
            m_collisionStagingBuffer.Get(),
            m_collisionBuffer.Get()
            );

        bufferToMap = m_collisionStagingBuffer.Get();
    }

    // NOTE: The Map call will block until the shader Dispatch has completed on the GPU
    // since the DO_NOT_WAIT flag was not provided. For this reason, any other GPU work
    // that can be done in parallel should be dispatched/drawn before calling Map.
    D3D11_MAPPED_SUBRESOURCE mappedSubresource;
    ZeroMemory(&mappedSubresource, sizeof(mappedSubresource));
    DX::ThrowIfFailed(
        m_deviceResources->GetD3DDeviceContext()->Map(
            bufferToMap,
            0,
            D3D11_MAP_READ,
            0,
            &mappedSubresource
            )
        );

    // Store collision buffer result.
    int collisionCount = *((int*)mappedSubresource.pData);

    // Display number of projectile collisions with wall.
    Platform::String^ destinationBufferString = "Number of collisions with wall: " + collisionCount.ToString();
    m_deviceResources->GetD2DDeviceContext()->DrawText(
        destinationBufferString->Data(),
        destinationBufferString->Length(),
        m_collisionCountTextFormat.Get(),
        D2D1::RectF(
            WallThickness,
            m_sampleOverlay->GetTitleHeightInDips(),
            WallThickness + CollisionTextWidth,
            m_sampleOverlay->GetTitleHeightInDips() + 50
            ),
        m_whiteBrush.Get()
        );

    m_deviceResources->GetD3DDeviceContext()->Unmap(bufferToMap, 0);
}

void DirectXMapDefaultBuffersRenderer::InitializeProjectileBuffer()
{
    // Randomly generate projectile data. The compute shader will perform parallelized
    // computations on the physics interactions between the projectile instances.
    std::unique_ptr<Projectile[]> projectileData(new Projectile[ProjectileCount]);
    for (int i = 0; i < ProjectileCount; i++)
    {
        projectileData[i].positionX = static_cast<float>(rand()) / RAND_MAX;
        projectileData[i].positionY = static_cast<float>(rand()) / RAND_MAX;
        projectileData[i].velocityX = ((static_cast<float>(rand()) / RAND_MAX) - 0.5f) / 500.0f;
        projectileData[i].velocityY = ((static_cast<float>(rand()) / RAND_MAX) - 0.5f) / 500.0f;
    }

    // Create descriptor for projectile data Direct3D Buffer.
    D3D11_BUFFER_DESC projectileBufferDesc;
    ZeroMemory(&projectileBufferDesc, sizeof(projectileBufferDesc));
    projectileBufferDesc.BindFlags = D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_UNORDERED_ACCESS;

    if (m_cpuAccessMethod == CpuAccessMethod::MapDefaultBuffers)
    {
        // If Mappable Default Buffers are available, create Buffer as mappable
        // by passing the desired D3D11_CPU_ACCESS flag.
        projectileBufferDesc.CPUAccessFlags = D3D11_CPU_ACCESS_READ;
    }

    projectileBufferDesc.Usage = D3D11_USAGE_DEFAULT;
    projectileBufferDesc.MiscFlags = D3D11_RESOURCE_MISC_BUFFER_STRUCTURED;
    projectileBufferDesc.ByteWidth = sizeof(Projectile) * ProjectileCount;
    projectileBufferDesc.StructureByteStride = sizeof(Projectile);

    D3D11_SUBRESOURCE_DATA projectileSubresourceData;
    ZeroMemory(&projectileSubresourceData, sizeof(projectileSubresourceData));
    projectileSubresourceData.pSysMem = projectileData.get();

    // Create Direct3D Buffer for projectile data.
    DX::ThrowIfFailed(
        m_deviceResources->GetD3DDevice()->CreateBuffer(
            &projectileBufferDesc,
            &projectileSubresourceData,
            &m_projectileBuffer
            )
        );

    // Create Direct3D Unordered Access View for projectile buffer.
    D3D11_BUFFER_UAV projectileBufferUav;
    ZeroMemory(&projectileBufferUav, sizeof(projectileBufferUav));
    projectileBufferUav.NumElements = ProjectileCount;

    D3D11_UNORDERED_ACCESS_VIEW_DESC projectileBufferUavDesc;
    ZeroMemory(&projectileBufferUavDesc, sizeof(projectileBufferUavDesc));
    projectileBufferUavDesc.Format = DXGI_FORMAT_UNKNOWN;
    projectileBufferUavDesc.Buffer = projectileBufferUav;
    projectileBufferUavDesc.ViewDimension = D3D11_UAV_DIMENSION_BUFFER;

    DX::ThrowIfFailed(
        m_deviceResources->GetD3DDevice()->CreateUnorderedAccessView(
            m_projectileBuffer.Get(),
            &projectileBufferUavDesc,
            &m_projectileBufferView
            )
        );
}

void DirectXMapDefaultBuffersRenderer::InitializeCollisionBuffer()
{
    // Create Direct3D Buffer for collision data.
    D3D11_BUFFER_DESC collisionBufferDesc;
    ZeroMemory(&collisionBufferDesc, sizeof(collisionBufferDesc));
    collisionBufferDesc.BindFlags = D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_UNORDERED_ACCESS;

    if (m_cpuAccessMethod == CpuAccessMethod::MapDefaultBuffers)
    {
        // If Mappable Default Buffers available, create Default Buffer as mappable
        // by passing the desired D3D11_CPU_ACCESS flag.
        collisionBufferDesc.CPUAccessFlags = D3D11_CPU_ACCESS_READ;
    }

    collisionBufferDesc.Usage = D3D11_USAGE_DEFAULT;
    collisionBufferDesc.ByteWidth = sizeof(int);

    // Create Direct3D Buffer for collision data.
    DX::ThrowIfFailed(
        m_deviceResources->GetD3DDevice()->CreateBuffer(
            &collisionBufferDesc,
            nullptr,
            &m_collisionBuffer
            )
        );

    // Create Direct3D Unordered Access View for collision buffer.
    D3D11_BUFFER_UAV collisionBufferUav;
    ZeroMemory(&collisionBufferUav, sizeof(collisionBufferUav));
    collisionBufferUav.NumElements = 1;

    D3D11_UNORDERED_ACCESS_VIEW_DESC collisionBufferUavDesc;
    ZeroMemory(&collisionBufferUavDesc, sizeof(collisionBufferUavDesc));
    collisionBufferUavDesc.Format = DXGI_FORMAT_R32_SINT;
    collisionBufferUavDesc.Buffer = collisionBufferUav;
    collisionBufferUavDesc.ViewDimension = D3D11_UAV_DIMENSION_BUFFER;

    DX::ThrowIfFailed(
        m_deviceResources->GetD3DDevice()->CreateUnorderedAccessView(
            m_collisionBuffer.Get(),
            &collisionBufferUavDesc,
            &m_collisionBufferView
            )
        );
}

void DirectXMapDefaultBuffersRenderer::InitializeStagingBuffers()
{
    // Create Direct3D Staging Buffer for reading projectile data back on the CPU.
    D3D11_BUFFER_DESC projectileStagingBufferDesc;
    ZeroMemory(&projectileStagingBufferDesc, sizeof(projectileStagingBufferDesc));
    projectileStagingBufferDesc.BindFlags = 0;
    projectileStagingBufferDesc.CPUAccessFlags = D3D11_CPU_ACCESS_READ;
    projectileStagingBufferDesc.Usage = D3D11_USAGE_STAGING;
    projectileStagingBufferDesc.MiscFlags = D3D11_RESOURCE_MISC_BUFFER_STRUCTURED;
    projectileStagingBufferDesc.ByteWidth = sizeof(Projectile) * ProjectileCount;
    projectileStagingBufferDesc.StructureByteStride = sizeof(Projectile);

    DX::ThrowIfFailed(
        m_deviceResources->GetD3DDevice()->CreateBuffer(
            &projectileStagingBufferDesc,
            nullptr,
            &m_projectileStagingBuffer
            )
        );

    // Create Direct3D Staging Buffer for reading collision data back on the CPU.
    D3D11_BUFFER_DESC collisionStagingBufferDesc;
    ZeroMemory(&collisionStagingBufferDesc, sizeof(collisionStagingBufferDesc));
    collisionStagingBufferDesc.BindFlags = 0;
    collisionStagingBufferDesc.CPUAccessFlags = D3D11_CPU_ACCESS_READ;
    collisionStagingBufferDesc.Usage = D3D11_USAGE_STAGING;
    collisionStagingBufferDesc.ByteWidth = sizeof(int);

    DX::ThrowIfFailed(
        m_deviceResources->GetD3DDevice()->CreateBuffer(
            &collisionStagingBufferDesc,
            nullptr,
            &m_collisionStagingBuffer
            )
        );
}