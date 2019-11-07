//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "pch.h"

#include "DirectXPanelBase.h"
#include "DirectXHelper.h"

#include <math.h>
#include <windows.ui.xaml.media.dxinterop.h>

using namespace InkingPanelDX;

static const float s_dipsPerInch = 96.0f;

DirectXPanelBase::DirectXPanelBase() :
    m_backgroundColor(D2D1::ColorF(D2D1::ColorF::White)), // Default to white background.
    m_alphaMode(DXGI_ALPHA_MODE_UNSPECIFIED) // Default to ignore alpha, which can provide better performance if transparency is not required.
{
    // Cache XAML properties
    m_width = (float)this->Width;
    m_height = (float)this->Height;
    m_compositionScaleX = this->CompositionScaleX;
    m_compositionScaleY = this->CompositionScaleY;

    this->SizeChanged += 
        ref new Windows::UI::Xaml::SizeChangedEventHandler(this, &DirectXPanelBase::OnSizeChanged);
    this->CompositionScaleChanged += 
        ref new Windows::Foundation::TypedEventHandler<SwapChainPanel^, Object^>(this, &DirectXPanelBase::OnCompositionScaleChanged);
    Windows::UI::Xaml::Application::Current->Suspending +=
        ref new Windows::UI::Xaml::SuspendingEventHandler(this, &DirectXPanelBase::OnSuspending);
}

void DirectXPanelBase::CreateDeviceIndependentResources()
{
    D2D1_FACTORY_OPTIONS options;
    ZeroMemory(&options, sizeof(D2D1_FACTORY_OPTIONS) );

#if defined(_DEBUG)
    // Enable D2D debugging via SDK Layers when in debug mode.
    options.debugLevel = D2D1_DEBUG_LEVEL_INFORMATION;
#endif

    DX::ThrowIfFailed(
        D2D1CreateFactory(
            D2D1_FACTORY_TYPE_SINGLE_THREADED,
            __uuidof(ID2D1Factory2) ,
            &options,
            &m_d2dFactory
        )
    );
}

void DirectXPanelBase::CreateDeviceResources()
{
    // This flag adds support for surfaces with a different color channel ordering than the API default.
    // It is recommended usage, and is required for compatibility with Direct2D.
    UINT creationFlags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;

#if defined(_DEBUG)
    // If the project is in a debug build, enable debugging via SDK Layers with this flag.
    creationFlags |= D3D11_CREATE_DEVICE_DEBUG;
#endif

    // This array defines the set of DirectX hardware feature levels this app will support.
    // Note the ordering should be preserved.
    // Don't forget to declare your application's minimum required feature level in its
    // description.  All applications are assumed to support 9.1 unless otherwise stated.
    D3D_FEATURE_LEVEL featureLevels [] =
    {
        D3D_FEATURE_LEVEL_11_1,
        D3D_FEATURE_LEVEL_11_0,
        D3D_FEATURE_LEVEL_10_1,
        D3D_FEATURE_LEVEL_10_0,
        D3D_FEATURE_LEVEL_9_3,
        D3D_FEATURE_LEVEL_9_2,
        D3D_FEATURE_LEVEL_9_1
    };

    // Create the DX11 API device object, and get a corresponding context.
    Microsoft::WRL::ComPtr<ID3D11Device> device;
    Microsoft::WRL::ComPtr<ID3D11DeviceContext> context;
    DX::ThrowIfFailed(
        D3D11CreateDevice(
            nullptr,                    // Specify null to use the default adapter.
            D3D_DRIVER_TYPE_HARDWARE,
            0,
            creationFlags,              // Optionally set debug and Direct2D compatibility flags.
            featureLevels,              // List of feature levels this app can support.
            ARRAYSIZE(featureLevels),
            D3D11_SDK_VERSION,          // Always set this to D3D11_SDK_VERSION for Windows Store apps.
            &device,                    // Returns the Direct3D device created.
            NULL,                       // Returns feature level of device created.
            &context                    // Returns the device immediate context.
        )
    );

    // Get D3D11.1 device
    DX::ThrowIfFailed(
        device.As(&m_d3dDevice)
    );

    // Get D3D11.1 context
    DX::ThrowIfFailed(
        context.As(&m_d3dContext)
    );

    // Get underlying DXGI device of D3D device
    Microsoft::WRL::ComPtr<IDXGIDevice> dxgiDevice;
    DX::ThrowIfFailed(
        m_d3dDevice.As(&dxgiDevice)
    );

    // Get D2D device
    DX::ThrowIfFailed(
        m_d2dFactory->CreateDevice(dxgiDevice.Get(), &m_d2dDevice)
    );

    // Get D2D context
    DX::ThrowIfFailed(
        m_d2dDevice->CreateDeviceContext(
            D2D1_DEVICE_CONTEXT_OPTIONS_NONE,
            &m_d2dContext
        )
    );

    // Set D2D text anti-alias mode to Grayscale to ensure proper rendering of text on intermediate surfaces.
    m_d2dContext->SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE_GRAYSCALE);
}

void DirectXPanelBase::ReleaseSizeDependentResources()
{
    m_d2dContext->SetTarget(nullptr);
    m_d2dTargetBitmap = nullptr;
    m_d3dContext->OMSetRenderTargets(0, nullptr, nullptr);
    m_d3dContext->Flush();
}

void DirectXPanelBase::CreateSizeDependentResources()
{
    bool setSwapChain = false;

    // Ensure dependent objects have been released.
    ReleaseSizeDependentResources();
    
    // If the swap chain already exists, then resize it.
    if (m_swapChain != nullptr)
    {
        // If the swap chain already exists, resize it.
        HRESULT hr = m_swapChain->ResizeBuffers(
            2, // Double-buffered swap chain.
            static_cast<UINT>((float) max(m_width * m_compositionScaleX, 1)),
            static_cast<UINT>((float) max(m_height * m_compositionScaleY, 1)),
            DXGI_FORMAT_B8G8R8A8_UNORM,
            0
        );

        if (hr == DXGI_ERROR_DEVICE_REMOVED || hr == DXGI_ERROR_DEVICE_RESET)
        {
            // If the device was removed for any reason, a new device and swap chain will need to be created.
            HandleDeviceLost();

            // Everything is set up now. Do not continue execution of this method. 
            return;
        }
        else
        {
            DX::ThrowIfFailed(hr);
        }
    }
    else // Otherwise, create a new one.
    {
        DXGI_SWAP_CHAIN_DESC1 swapChainDesc = { 0 };
        swapChainDesc.Width = static_cast<UINT>((float) max(m_width * m_compositionScaleX, 1));      // Match the size of the panel.
        swapChainDesc.Height = static_cast<UINT>((float) max(m_height * m_compositionScaleY, 1));
        swapChainDesc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;                  // This is the most common swap chain format.
        swapChainDesc.Stereo = false;
        swapChainDesc.SampleDesc.Count = 1;                                 // Don't use multi-sampling.
        swapChainDesc.SampleDesc.Quality = 0;
        swapChainDesc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
        swapChainDesc.BufferCount = 2;                                      // Use double buffering to enable flip.
        swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL;        // All Windows Store apps must use this SwapEffect.
        swapChainDesc.Flags = 0;
        swapChainDesc.AlphaMode = m_alphaMode;

        // Get underlying DXGI Device from D3D Device.
        Microsoft::WRL::ComPtr<IDXGIDevice1> dxgiDevice;
        DX::ThrowIfFailed(
            m_d3dDevice.As(&dxgiDevice)
        );

        // Get adapter.
        Microsoft::WRL::ComPtr<IDXGIAdapter> dxgiAdapter;
        DX::ThrowIfFailed(
            dxgiDevice->GetAdapter(&dxgiAdapter)
        );

        // Get factory.
        Microsoft::WRL::ComPtr<IDXGIFactory2> dxgiFactory;
        DX::ThrowIfFailed(
            dxgiAdapter->GetParent(IID_PPV_ARGS(&dxgiFactory))
        );

        Microsoft::WRL::ComPtr<IDXGISwapChain1> swapChain;
        // Create swap chain.
        DX::ThrowIfFailed(
            dxgiFactory->CreateSwapChainForComposition(
                m_d3dDevice.Get(),
                &swapChainDesc,
                nullptr,
                &swapChain
            )
        );
        swapChain.As(&m_swapChain);

        // Ensure that DXGI does not queue more than one frame at a time. This both reduces 
        // latency and ensures that the application will only render after each VSync, minimizing 
        // power consumption.
        DX::ThrowIfFailed(
            dxgiDevice->SetMaximumFrameLatency(1)
        );

        setSwapChain = true;
    }

    // Ensure the physical pixel size of the swap chain takes into account both the XAML SwapChainPanel's logical layout size and 
    // any cumulative composition scale applied due to zooming, render transforms, or the system's current scaling plateau.
    // For example, if a 100x100 SwapChainPanel has a cumulative 2x scale transform applied, we instead create a 200x200 swap chain 
    // to avoid artifacts from scaling it up by 2x, then apply an inverse 1/2x transform to the swap chain to cancel out the 2x transform.
    DXGI_MATRIX_3X2_F inverseScale = { 0 };
    inverseScale._11 = 1.0f / m_compositionScaleX;
    inverseScale._22 = 1.0f / m_compositionScaleY;

    m_swapChain->SetMatrixTransform(&inverseScale);

    D2D1_BITMAP_PROPERTIES1 bitmapProperties =
        D2D1::BitmapProperties1(
            D2D1_BITMAP_OPTIONS_TARGET | D2D1_BITMAP_OPTIONS_CANNOT_DRAW,
            D2D1::PixelFormat(DXGI_FORMAT_B8G8R8A8_UNORM, D2D1_ALPHA_MODE_PREMULTIPLIED),
            s_dipsPerInch * m_compositionScaleX,
            s_dipsPerInch * m_compositionScaleY
    );

    // Direct2D needs the DXGI version of the backbuffer surface pointer.
    Microsoft::WRL::ComPtr<IDXGISurface> dxgiBackBuffer;
    DX::ThrowIfFailed(
        m_swapChain->GetBuffer(0, IID_PPV_ARGS(&dxgiBackBuffer))
    );

    // Get a D2D surface from the DXGI back buffer to use as the D2D render target.
    DX::ThrowIfFailed(
        m_d2dContext->CreateBitmapFromDxgiSurface(
            dxgiBackBuffer.Get(),
            &bitmapProperties,
            &m_d2dTargetBitmap
        )
    );

    m_d2dContext->SetDpi(s_dipsPerInch * m_compositionScaleX, s_dipsPerInch * m_compositionScaleY);
    m_d2dContext->SetTarget(m_d2dTargetBitmap.Get());

    if (setSwapChain)
    {
        SetSwapChain();
    }
}

void DirectXPanelBase::HandleDeviceLost()
{
    m_swapChain = nullptr;

    // Make sure the rendering state has been released.
    m_d3dContext->OMSetRenderTargets(0, nullptr, nullptr);

    m_d2dContext->SetTarget(nullptr);
    m_d2dTargetBitmap = nullptr;

    m_d2dContext = nullptr;
    m_d2dDevice = nullptr;

    m_d3dContext->Flush();

    CreateDeviceResources();
    CreateSizeDependentResources();
    Render();
}

void DirectXPanelBase::Present()
{
    DXGI_PRESENT_PARAMETERS parameters = { 0 };
    parameters.DirtyRectsCount = 0;
    parameters.pDirtyRects = nullptr;
    parameters.pScrollRect = nullptr;
    parameters.pScrollOffset = nullptr;

    HRESULT hr = S_OK;

    hr = m_swapChain->Present1(1, 0, &parameters);

    if (hr == DXGI_ERROR_DEVICE_REMOVED || hr == DXGI_ERROR_DEVICE_RESET)
    {
        HandleDeviceLost();
    }
    else
    {
        DX::ThrowIfFailed(hr);
    }
}

void DirectXPanelBase::SetSwapChain()
{
    // Get backing native interface for SwapChainPanel.
    Microsoft::WRL::ComPtr<ISwapChainPanelNative> panelNative;
    DX::ThrowIfFailed(
        reinterpret_cast<IUnknown*>(this)->QueryInterface(IID_PPV_ARGS(&panelNative))
    );

    // Associate swap chain with SwapChainPanel.
    DX::ThrowIfFailed(
        panelNative->SetSwapChain(m_swapChain.Get())
    );
}

void DirectXPanelBase::OnSuspending(Object^ sender, Windows::ApplicationModel::SuspendingEventArgs^ e)
{
    Microsoft::WRL::ComPtr<IDXGIDevice3> dxgiDevice;
    m_d3dDevice.As(&dxgiDevice);

    // Hints to the driver that the app is entering an idle state and that its memory can be used temporarily for other apps.
    dxgiDevice->Trim();
}

void DirectXPanelBase::OnSizeChanged(Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e)
{
    // Cache XAML properties
    m_width = (float) this->Width;
    m_height = (float) this->Height;
    m_compositionScaleX = this->CompositionScaleX;
    m_compositionScaleY = this->CompositionScaleY;

    // Update swap chain size when the panel's size changes.
    CreateSizeDependentResources();
    Render();
}

void DirectXPanelBase::OnCompositionScaleChanged(SwapChainPanel^ sender, Object^ args)
{
    // Cache XAML properties
    m_width = (float) this->Width;
    m_height = (float) this->Height;
    m_compositionScaleX = this->CompositionScaleX;
    m_compositionScaleY = this->CompositionScaleY;

    // Recreate size-dependent resources when the composition scale is changed.
    CreateSizeDependentResources();
    Render();    
}