//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXSample.h"
#include "BasicMath.h"
#include "BasicReaderWriter.h"

using namespace Microsoft::WRL;
using namespace Windows::UI::Core;
using namespace Windows::Foundation;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;

struct BasicVertex
{
    float3 pos;  // position
    float3 norm; // surface normal vector
    float2 tex;  // texture coordinate
};

struct ConstantBuffer
{
    float4x4 model;
    float4x4 view;
    float4x4 projection;
};

// This class defines the application as a whole.
ref class Direct3DTutorialFrameworkView : public IFrameworkView
{
private:
    Platform::Agile<CoreWindow> m_window;
    ComPtr<IDXGISwapChain1> m_swapChain;
    ComPtr<ID3D11Device1> m_d3dDevice;
    ComPtr<ID3D11DeviceContext1> m_d3dDeviceContext;
    ComPtr<ID3D11RenderTargetView> m_renderTargetView;
    ComPtr<ID3D11DepthStencilView> m_depthStencilView;
    ComPtr<ID3D11Buffer> m_constantBuffer;
    ConstantBuffer m_constantBufferData;

public:
    // This method is called on application launch.
    virtual void Initialize(
        _In_ CoreApplicationView^ applicationView
        )
    {
        applicationView->Activated +=
            ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &Direct3DTutorialFrameworkView::OnActivated);
    }

    // This method is called after Initialize.
    virtual void SetWindow(
        _In_ CoreWindow^ window
        )
    {
        m_window = window;

        // Specify the cursor type as the standard arrow cursor.
        m_window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

        // Allow the application to respond when the window size changes.
        m_window->SizeChanged +=
            ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(
                this,
                &Direct3DTutorialFrameworkView::OnWindowSizeChanged
                );
    }

    void OnActivated(
        _In_ CoreApplicationView^ applicationView,
        _In_ IActivatedEventArgs^ args
        )
    {
        // Activate the application window, making it visible and enabling it to receive events.
        CoreWindow::GetForCurrentThread()->Activate();
    }

    virtual void Load(_In_ Platform::String^ entryPoint)
    {
    }

    // This method is called after Load.
    virtual void Run()
    {
        // First, create the Direct3D device.

        // This flag is required in order to enable compatibility with Direct2D.
        UINT creationFlags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;

#if defined(_DEBUG)
        // If the project is in a debug build, enable debugging via SDK Layers with this flag.
        creationFlags |= D3D11_CREATE_DEVICE_DEBUG;
#endif

        // This array defines the ordering of feature levels that D3D should attempt to create.
        D3D_FEATURE_LEVEL featureLevels[] =
        {
            D3D_FEATURE_LEVEL_11_1,
            D3D_FEATURE_LEVEL_11_0,
            D3D_FEATURE_LEVEL_10_1,
            D3D_FEATURE_LEVEL_10_0,
            D3D_FEATURE_LEVEL_9_3,
            D3D_FEATURE_LEVEL_9_1
        };

        ComPtr<ID3D11Device> d3dDevice;
        ComPtr<ID3D11DeviceContext> d3dDeviceContext;
        DX::ThrowIfFailed(
            D3D11CreateDevice(
                nullptr,                    // specify nullptr to use the default adapter
                D3D_DRIVER_TYPE_HARDWARE,
                nullptr,                    // leave as nullptr if hardware is used
                creationFlags,              // optionally set debug and Direct2D compatibility flags
                featureLevels,
                ARRAYSIZE(featureLevels),
                D3D11_SDK_VERSION,          // always set this to D3D11_SDK_VERSION
                &d3dDevice,
                nullptr,
                &d3dDeviceContext
                )
            );

        // Retrieve the Direct3D 11.1 interfaces.
        DX::ThrowIfFailed(
            d3dDevice.As(&m_d3dDevice)
            );

        DX::ThrowIfFailed(
            d3dDeviceContext.As(&m_d3dDeviceContext)
            );


        // After the D3D device is created, create additional application resources.
        CreateWindowSizeDependentResources();

        // Create a Basic Reader-Writer class to load data from disk.  This class is examined
        // in the Resource Loading sample.
        BasicReaderWriter^ reader = ref new BasicReaderWriter();

        // Load the raw vertex shader bytecode from disk and create a vertex shader with it.
        auto vertexShaderBytecode = reader->ReadData("SimpleVertexShader.cso");
        ComPtr<ID3D11VertexShader> vertexShader;
        DX::ThrowIfFailed(
            m_d3dDevice->CreateVertexShader(
                vertexShaderBytecode->Data,
                vertexShaderBytecode->Length,
                nullptr,
                &vertexShader
                )
            );

        // Create an input layout that matches the layout defined in the vertex shader code.
        // These correspond to the elements of the BasicVertex struct defined above.
        const D3D11_INPUT_ELEMENT_DESC basicVertexLayoutDesc[] =
        {
            { "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT, 0,  0, D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "NORMAL",   0, DXGI_FORMAT_R32G32B32_FLOAT, 0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 },
            { "TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT,    0, 24, D3D11_INPUT_PER_VERTEX_DATA, 0 },
        };

        ComPtr<ID3D11InputLayout> inputLayout;
        DX::ThrowIfFailed(
            m_d3dDevice->CreateInputLayout(
                basicVertexLayoutDesc,
                ARRAYSIZE(basicVertexLayoutDesc),
                vertexShaderBytecode->Data,
                vertexShaderBytecode->Length,
                &inputLayout
                )
            );

        // Load the raw pixel shader bytecode from disk and create a pixel shader with it.
        auto pixelShaderBytecode = reader->ReadData("SimplePixelShader.cso");
        ComPtr<ID3D11PixelShader> pixelShader;
        DX::ThrowIfFailed(
            m_d3dDevice->CreatePixelShader(
                pixelShaderBytecode->Data,
                pixelShaderBytecode->Length,
                nullptr,
                &pixelShader
                )
            );

        // Create vertex and index buffers that define a simple unit cube.

        // In the array below, which will be used to initialize the cube vertex buffers,
        // multiple vertices are used for each corner to allow different normal vectors and
        // texture coordinates to be defined for each face.
        BasicVertex cubeVertices[] =
        {
            { float3(-0.5f, 0.5f, -0.5f), float3(0.0f, 1.0f, 0.0f), float2(0.0f, 0.0f) }, // +Y (top face)
            { float3( 0.5f, 0.5f, -0.5f), float3(0.0f, 1.0f, 0.0f), float2(1.0f, 0.0f) },
            { float3( 0.5f, 0.5f,  0.5f), float3(0.0f, 1.0f, 0.0f), float2(1.0f, 1.0f) },
            { float3(-0.5f, 0.5f,  0.5f), float3(0.0f, 1.0f, 0.0f), float2(0.0f, 1.0f) },

            { float3(-0.5f, -0.5f,  0.5f), float3(0.0f, -1.0f, 0.0f), float2(0.0f, 0.0f) }, // -Y (bottom face)
            { float3( 0.5f, -0.5f,  0.5f), float3(0.0f, -1.0f, 0.0f), float2(1.0f, 0.0f) },
            { float3( 0.5f, -0.5f, -0.5f), float3(0.0f, -1.0f, 0.0f), float2(1.0f, 1.0f) },
            { float3(-0.5f, -0.5f, -0.5f), float3(0.0f, -1.0f, 0.0f), float2(0.0f, 1.0f) },

            { float3(0.5f,  0.5f,  0.5f), float3(1.0f, 0.0f, 0.0f), float2(0.0f, 0.0f) }, // +X (right face)
            { float3(0.5f,  0.5f, -0.5f), float3(1.0f, 0.0f, 0.0f), float2(1.0f, 0.0f) },
            { float3(0.5f, -0.5f, -0.5f), float3(1.0f, 0.0f, 0.0f), float2(1.0f, 1.0f) },
            { float3(0.5f, -0.5f,  0.5f), float3(1.0f, 0.0f, 0.0f), float2(0.0f, 1.0f) },

            { float3(-0.5f,  0.5f, -0.5f), float3(-1.0f, 0.0f, 0.0f), float2(0.0f, 0.0f) }, // -X (left face)
            { float3(-0.5f,  0.5f,  0.5f), float3(-1.0f, 0.0f, 0.0f), float2(1.0f, 0.0f) },
            { float3(-0.5f, -0.5f,  0.5f), float3(-1.0f, 0.0f, 0.0f), float2(1.0f, 1.0f) },
            { float3(-0.5f, -0.5f, -0.5f), float3(-1.0f, 0.0f, 0.0f), float2(0.0f, 1.0f) },

            { float3(-0.5f,  0.5f, 0.5f), float3(0.0f, 0.0f, 1.0f), float2(0.0f, 0.0f) }, // +Z (front face)
            { float3( 0.5f,  0.5f, 0.5f), float3(0.0f, 0.0f, 1.0f), float2(1.0f, 0.0f) },
            { float3( 0.5f, -0.5f, 0.5f), float3(0.0f, 0.0f, 1.0f), float2(1.0f, 1.0f) },
            { float3(-0.5f, -0.5f, 0.5f), float3(0.0f, 0.0f, 1.0f), float2(0.0f, 1.0f) },

            { float3( 0.5f,  0.5f, -0.5f), float3(0.0f, 0.0f, -1.0f), float2(0.0f, 0.0f) }, // -Z (back face)
            { float3(-0.5f,  0.5f, -0.5f), float3(0.0f, 0.0f, -1.0f), float2(1.0f, 0.0f) },
            { float3(-0.5f, -0.5f, -0.5f), float3(0.0f, 0.0f, -1.0f), float2(1.0f, 1.0f) },
            { float3( 0.5f, -0.5f, -0.5f), float3(0.0f, 0.0f, -1.0f), float2(0.0f, 1.0f) },
        };

        unsigned short cubeIndices[] =
        {
            0, 1, 2,
            0, 2, 3,

            4, 5, 6,
            4, 6, 7,

            8, 9, 10,
            8, 10, 11,

            12, 13, 14,
            12, 14, 15,

            16, 17, 18,
            16, 18, 19,

            20, 21, 22,
            20, 22, 23
        };

        D3D11_BUFFER_DESC vertexBufferDesc = {0};
        vertexBufferDesc.ByteWidth = sizeof(BasicVertex) * ARRAYSIZE(cubeVertices);
        vertexBufferDesc.Usage = D3D11_USAGE_DEFAULT;
        vertexBufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
        vertexBufferDesc.CPUAccessFlags = 0;
        vertexBufferDesc.MiscFlags = 0;
        vertexBufferDesc.StructureByteStride = 0;

        D3D11_SUBRESOURCE_DATA vertexBufferData;
        vertexBufferData.pSysMem = cubeVertices;
        vertexBufferData.SysMemPitch = 0;
        vertexBufferData.SysMemSlicePitch = 0;

        ComPtr<ID3D11Buffer> vertexBuffer;
        DX::ThrowIfFailed(
            m_d3dDevice->CreateBuffer(
                &vertexBufferDesc,
                &vertexBufferData,
                &vertexBuffer
                )
            );

        D3D11_BUFFER_DESC indexBufferDesc;
        indexBufferDesc.ByteWidth = sizeof(unsigned short) * ARRAYSIZE(cubeIndices);
        indexBufferDesc.Usage = D3D11_USAGE_DEFAULT;
        indexBufferDesc.BindFlags = D3D11_BIND_INDEX_BUFFER;
        indexBufferDesc.CPUAccessFlags = 0;
        indexBufferDesc.MiscFlags = 0;
        indexBufferDesc.StructureByteStride = 0;

        D3D11_SUBRESOURCE_DATA indexBufferData;
        indexBufferData.pSysMem = cubeIndices;
        indexBufferData.SysMemPitch = 0;
        indexBufferData.SysMemSlicePitch = 0;

        ComPtr<ID3D11Buffer> indexBuffer;
        DX::ThrowIfFailed(
            m_d3dDevice->CreateBuffer(
                &indexBufferDesc,
                &indexBufferData,
                &indexBuffer
                )
            );


        // Create a constant buffer for passing model, view, and projection matrices
        // to the vertex shader.  This will allow us to rotate the cube and apply
        // a perspective projection to it.

        D3D11_BUFFER_DESC constantBufferDesc = {0};
        constantBufferDesc.ByteWidth = sizeof(m_constantBufferData);
        constantBufferDesc.Usage = D3D11_USAGE_DEFAULT;
        constantBufferDesc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
        constantBufferDesc.CPUAccessFlags = 0;
        constantBufferDesc.MiscFlags = 0;
        constantBufferDesc.StructureByteStride = 0;
        DX::ThrowIfFailed(
            m_d3dDevice->CreateBuffer(
                &constantBufferDesc,
                nullptr,
                &m_constantBuffer
                )
            );

        // Specify the view transform corresponding to a camera position of
        // X = 0, Y = 1, Z = 2.  See Lesson 5 for a generalized camera class.

        m_constantBufferData.view = float4x4(
            -1.00000000f, 0.00000000f,  0.00000000f,  0.00000000f,
             0.00000000f, 0.89442718f,  0.44721359f,  0.00000000f,
             0.00000000f, 0.44721359f, -0.89442718f, -2.23606800f,
             0.00000000f, 0.00000000f,  0.00000000f,  1.00000000f
            );


        // Load the raw texture data from disk and construct a subresource description that references it.
        auto textureData = reader->ReadData("texturedata.bin");
        D3D11_SUBRESOURCE_DATA textureSubresourceData = {0};
        textureSubresourceData.pSysMem = textureData->Data;

        // Specify the size of a row in bytes, known a priori about the texture data.
        textureSubresourceData.SysMemPitch = 1024;

        // As this is not a texture array or 3D texture, this parameter is ignored.
        textureSubresourceData.SysMemSlicePitch = 0;

        // Create a texture description from information known a priori about the data.
        // Generalized texture loading code can be found in the Resource Loading sample.
        D3D11_TEXTURE2D_DESC textureDesc = {0};
        textureDesc.Width = 256;
        textureDesc.Height = 256;
        textureDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
        textureDesc.Usage = D3D11_USAGE_DEFAULT;
        textureDesc.CPUAccessFlags = 0;
        textureDesc.MiscFlags = 0;

        // Most textures contain more than one MIP level.  For simplicity, this sample uses only one.
        textureDesc.MipLevels = 1;

        // As this will not be a texture array, this parameter is ignored.
        textureDesc.ArraySize = 1;

        // Don't use multi-sampling.
        textureDesc.SampleDesc.Count = 1;
        textureDesc.SampleDesc.Quality = 0;

        // Allow the texture to be bound as a shader resource.
        textureDesc.BindFlags = D3D11_BIND_SHADER_RESOURCE;

        ComPtr<ID3D11Texture2D> texture;
        DX::ThrowIfFailed(
            m_d3dDevice->CreateTexture2D(
                &textureDesc,
                &textureSubresourceData,
                &texture
                )
            );

        // Once the texture is created, we must create a shader resource view of it
        // so that shaders may use it.  In general, the view description will match
        // the texture description.
        D3D11_SHADER_RESOURCE_VIEW_DESC textureViewDesc;
        ZeroMemory(&textureViewDesc, sizeof(textureViewDesc));
        textureViewDesc.Format = textureDesc.Format;
        textureViewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
        textureViewDesc.Texture2D.MipLevels = textureDesc.MipLevels;
        textureViewDesc.Texture2D.MostDetailedMip = 0;

        ComPtr<ID3D11ShaderResourceView> textureView;
        DX::ThrowIfFailed(
            m_d3dDevice->CreateShaderResourceView(
                texture.Get(),
                &textureViewDesc,
                &textureView
                )
            );

        // Once the texture view is created, create a sampler.  This defines how the color
        // for a particular texture coordinate is determined using the relevant texture data.
        D3D11_SAMPLER_DESC samplerDesc;
        ZeroMemory(&samplerDesc, sizeof(samplerDesc));

        samplerDesc.Filter = D3D11_FILTER_MIN_MAG_MIP_LINEAR;

        // The sampler does not use anisotropic filtering, so this parameter is ignored.
        samplerDesc.MaxAnisotropy = 0;

        // Specify how texture coordinates outside of the range 0..1 are resolved.
        samplerDesc.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
        samplerDesc.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
        samplerDesc.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;

        // Use no special MIP clamping or bias.
        samplerDesc.MipLODBias = 0.0f;
        samplerDesc.MinLOD = 0;
        samplerDesc.MaxLOD = D3D11_FLOAT32_MAX;

        // Don't use a comparison function.
        samplerDesc.ComparisonFunc = D3D11_COMPARISON_NEVER;

        // Border address mode is not used, so this parameter is ignored.
        samplerDesc.BorderColor[0] = 0.0f;
        samplerDesc.BorderColor[1] = 0.0f;
        samplerDesc.BorderColor[2] = 0.0f;
        samplerDesc.BorderColor[3] = 0.0f;

        ComPtr<ID3D11SamplerState> sampler;
        DX::ThrowIfFailed(
            m_d3dDevice->CreateSamplerState(
                &samplerDesc,
                &sampler
                )
            );


        // This value will be used to animate the cube by rotating it every frame;
        float degree = 0.0f;

        // Enter the render loop. Note that Windows Store apps should never exit.
        while (true)
        {
            // Process events incoming to the window.
            m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessAllIfPresent);

            // Update the constant buffer to rotate the cube model.
            m_constantBufferData.model = rotationY(-degree);
            degree += 1.0f;

            m_d3dDeviceContext->UpdateSubresource(
                m_constantBuffer.Get(),
                0,
                nullptr,
                &m_constantBufferData,
                0,
                0
                );

            // Specify the render target and depth stencil we created as the output target.
            m_d3dDeviceContext->OMSetRenderTargets(
                1,
                m_renderTargetView.GetAddressOf(),
                m_depthStencilView.Get()
                );

            // Clear the render target to a solid color, and reset the depth stencil.
            const float clearColor[4] = { 0.071f, 0.04f, 0.561f, 1.0f };
            m_d3dDeviceContext->ClearRenderTargetView(
                m_renderTargetView.Get(),
                clearColor
                );

            m_d3dDeviceContext->ClearDepthStencilView(
                m_depthStencilView.Get(),
                D3D11_CLEAR_DEPTH,
                1.0f,
                0
                );

            m_d3dDeviceContext->IASetInputLayout(inputLayout.Get());

            // Set the vertex and index buffers, and specify the way they define geometry.
            UINT stride = sizeof(BasicVertex);
            UINT offset = 0;
            m_d3dDeviceContext->IASetVertexBuffers(
                0,
                1,
                vertexBuffer.GetAddressOf(),
                &stride,
                &offset
                );

            m_d3dDeviceContext->IASetIndexBuffer(
                indexBuffer.Get(),
                DXGI_FORMAT_R16_UINT,
                0
                );

            m_d3dDeviceContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

            // Set the vertex and pixel shader stage state.
            m_d3dDeviceContext->VSSetShader(
                vertexShader.Get(),
                nullptr,
                0
                );

            m_d3dDeviceContext->VSSetConstantBuffers(
                0,
                1,
                m_constantBuffer.GetAddressOf()
                );

            m_d3dDeviceContext->PSSetShader(
                pixelShader.Get(),
                nullptr,
                0
                );

            m_d3dDeviceContext->PSSetShaderResources(
                0,
                1,
                textureView.GetAddressOf()
                );

            m_d3dDeviceContext->PSSetSamplers(
                0,
                1,
                sampler.GetAddressOf()
                );

            // Draw the cube.
            m_d3dDeviceContext->DrawIndexed(
                ARRAYSIZE(cubeIndices),
                0,
                0
                );

            // Present the rendered image to the window.  Because the maximum frame latency is set to 1,
            // the render loop will generally be throttled to the screen refresh rate, typically around
            // 60Hz, by sleeping the application on Present until the screen is refreshed.
            DX::ThrowIfFailed(
                m_swapChain->Present(1, 0)
                );
        }
    }

    // This method is called before the application exits.
    virtual void Uninitialize()
    {
    }

private:

    // This method is called whenever the application window size changes.
    void OnWindowSizeChanged(
        _In_ CoreWindow^ sender,
        _In_ WindowSizeChangedEventArgs^ args
        )
    {
        m_renderTargetView = nullptr;
        m_depthStencilView = nullptr;
        CreateWindowSizeDependentResources();
    }

    // This method creates all application resources that depend on
    // the application window size.  It is called at app initialization,
    // and whenever the application window size changes.
    void CreateWindowSizeDependentResources()
    {
        if (m_swapChain != nullptr)
        {
            // If the swap chain already exists, resize it.
            DX::ThrowIfFailed(
                m_swapChain->ResizeBuffers(
                    2,
                    0,
                    0,
                    DXGI_FORMAT_B8G8R8A8_UNORM,
                    0
                    )
                );
        }
        else
        {
            // If the swap chain does not exist, create it.
            DXGI_SWAP_CHAIN_DESC1 swapChainDesc = {0};

            swapChainDesc.Stereo = false;
            swapChainDesc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
            swapChainDesc.Scaling = DXGI_SCALING_NONE;
            swapChainDesc.Flags = 0;

            // Use automatic sizing.
            swapChainDesc.Width = 0;
            swapChainDesc.Height = 0;

            // This is the most common swap chain format.
            swapChainDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;

            // Don't use multi-sampling.
            swapChainDesc.SampleDesc.Count = 1;
            swapChainDesc.SampleDesc.Quality = 0;

            // Use two buffers to enable flip effect.
            swapChainDesc.BufferCount = 2;

            // We recommend using this swap effect for all applications.
            swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL;


            // Once the swap chain description is configured, it must be
            // created on the same adapter as the existing D3D Device.

            // First, retrieve the underlying DXGI Device from the D3D Device.
            ComPtr<IDXGIDevice2> dxgiDevice;
            DX::ThrowIfFailed(
                m_d3dDevice.As(&dxgiDevice)
                );

            // Ensure that DXGI does not queue more than one frame at a time. This both reduces
            // latency and ensures that the application will only render after each VSync, minimizing
            // power consumption.
            DX::ThrowIfFailed(
                dxgiDevice->SetMaximumFrameLatency(1)
                );

            // Next, get the parent factory from the DXGI Device.
            ComPtr<IDXGIAdapter> dxgiAdapter;
            DX::ThrowIfFailed(
                dxgiDevice->GetAdapter(&dxgiAdapter)
                );

            ComPtr<IDXGIFactory2> dxgiFactory;
            DX::ThrowIfFailed(
                dxgiAdapter->GetParent(IID_PPV_ARGS(&dxgiFactory))
                );

            // Finally, create the swap chain.
            CoreWindow^ window = m_window.Get();
            DX::ThrowIfFailed(
                dxgiFactory->CreateSwapChainForCoreWindow(
                    m_d3dDevice.Get(),
                    reinterpret_cast<IUnknown*>(window),
                    &swapChainDesc,
                    nullptr, // allow on all displays
                    &m_swapChain
                    )
                );
        }

        // Once the swap chain is created, create a render target view.  This will
        // allow Direct3D to render graphics to the window.

        ComPtr<ID3D11Texture2D> backBuffer;
        DX::ThrowIfFailed(
            m_swapChain->GetBuffer(0, IID_PPV_ARGS(&backBuffer))
            );

        DX::ThrowIfFailed(
            m_d3dDevice->CreateRenderTargetView(
                backBuffer.Get(),
                nullptr,
                &m_renderTargetView
                )
            );

        // Once the render target view is created, create a depth stencil view.  This
        // allows Direct3D to efficiently render objects closer to the camera in front
        // of objects further from the camera.

        D3D11_TEXTURE2D_DESC backBufferDesc = {0};
        backBuffer->GetDesc(&backBufferDesc);

        D3D11_TEXTURE2D_DESC depthStencilDesc;
        depthStencilDesc.Width = backBufferDesc.Width;
        depthStencilDesc.Height = backBufferDesc.Height;
        depthStencilDesc.MipLevels = 1;
        depthStencilDesc.ArraySize = 1;
        depthStencilDesc.Format = DXGI_FORMAT_D24_UNORM_S8_UINT;
        depthStencilDesc.SampleDesc.Count = 1;
        depthStencilDesc.SampleDesc.Quality = 0;
        depthStencilDesc.Usage = D3D11_USAGE_DEFAULT;
        depthStencilDesc.BindFlags = D3D11_BIND_DEPTH_STENCIL;
        depthStencilDesc.CPUAccessFlags = 0;
        depthStencilDesc.MiscFlags = 0;
        ComPtr<ID3D11Texture2D> depthStencil;
        DX::ThrowIfFailed(
            m_d3dDevice->CreateTexture2D(
                &depthStencilDesc,
                nullptr,
                &depthStencil
                )
            );

        D3D11_DEPTH_STENCIL_VIEW_DESC depthStencilViewDesc;
        depthStencilViewDesc.Format = depthStencilDesc.Format;
        depthStencilViewDesc.ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2D;
        depthStencilViewDesc.Flags = 0;
        depthStencilViewDesc.Texture2D.MipSlice = 0;
        DX::ThrowIfFailed(
            m_d3dDevice->CreateDepthStencilView(
                depthStencil.Get(),
                &depthStencilViewDesc,
                &m_depthStencilView
                )
            );

        // After the render target and depth stencil views are created, specify that
        // the viewport, which describes what portion of the window to draw to, should
        // cover the entire window.

        D3D11_VIEWPORT viewport;
        viewport.TopLeftX = 0.0f;
        viewport.TopLeftY = 0.0f;
        viewport.Width = static_cast<float>(backBufferDesc.Width);
        viewport.Height = static_cast<float>(backBufferDesc.Height);
        viewport.MinDepth = D3D11_MIN_DEPTH;
        viewport.MaxDepth = D3D11_MAX_DEPTH;

        m_d3dDeviceContext->RSSetViewports(1, &viewport);

        // Finally, update the constant buffer perspective projection parameters
        // to account for the size of the application window.  In this sample,
        // the parameters are fixed to a 70-degree field of view, with a depth
        // range of 0.01 to 100.  See Lesson 5 for a generalized camera class.

        float xScale = 1.42814801f;
        float yScale = 1.42814801f;
        if (backBufferDesc.Width > backBufferDesc.Height)
        {
            xScale = yScale *
                static_cast<float>(backBufferDesc.Height) /
                static_cast<float>(backBufferDesc.Width);
        }
        else
        {
            yScale = xScale *
                static_cast<float>(backBufferDesc.Width) /
                static_cast<float>(backBufferDesc.Height);
        }

        m_constantBufferData.projection = float4x4(
            xScale, 0.0f,    0.0f,  0.0f,
            0.0f,   yScale,  0.0f,  0.0f,
            0.0f,   0.0f,   -1.0f, -0.01f,
            0.0f,   0.0f,   -1.0f,  0.0f
            );
    }
};

// This class defines how to create the custom View Provider defined above.
ref class Direct3DTutorialFrameworkViewSource : IFrameworkViewSource
{
public:
    virtual IFrameworkView^ CreateView()
    {
        return ref new Direct3DTutorialFrameworkView();
    }
};

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto frameworkViewSource = ref new Direct3DTutorialFrameworkViewSource();
    Windows::ApplicationModel::Core::CoreApplication::Run(frameworkViewSource);
    return 0;
}
