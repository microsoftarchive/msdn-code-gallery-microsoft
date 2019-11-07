//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXBase.h"
#include "windows.ui.xaml.media.dxinterop.h"

using namespace Windows::UI::Core;
using namespace Windows::Foundation;
using namespace Microsoft::WRL;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Xaml::Controls;
using namespace D2D1;
using namespace DirectX;

// Constructor.
DirectXBase::DirectXBase() :
    m_dpi(-1.0f)
{
}

// Initialize the DirectX resources required to run.
void DirectXBase::Initialize(
    CoreWindow^ window,
    SwapChainBackgroundPanel^ swapChainPanel,
    float dpi
    )
{
    m_window = window;
    m_swapChainPanel = swapChainPanel;

    CreateDeviceIndependentResources();
    CreateDeviceResources();
    SetDpi(dpi);

    DisplayProperties::StereoEnabledChanged +=
        ref new DisplayPropertiesEventHandler(this, &DirectXBase::OnStereoEnabledChanged);
}

// Recreate all device resources and set them back to the current state.
void DirectXBase::HandleDeviceLost()
{
    float dpi = m_dpi;
    m_dpi = -1.0f;
    m_windowBounds.Width = 0;
    m_windowBounds.Height = 0;
    m_swapChain = nullptr;

    // Make sure the rendering state has been released.
    m_d3dContext->OMSetRenderTargets(0, nullptr, nullptr);
    m_d3dDepthStencilView = nullptr;
    m_d3dRenderTargetView = nullptr;
    m_d3dRenderTargetViewRight = nullptr;

    m_d2dContext->SetTarget(nullptr);
    m_d2dTargetBitmap = nullptr;
    m_d2dTargetBitmapRight = nullptr;
    m_d2dContext = nullptr;
    m_d2dDevice = nullptr;

    m_d3dContext->Flush();

    CreateDeviceResources();
    SetDpi(dpi);
}

// These are the resources required independent of the device.
void DirectXBase::CreateDeviceIndependentResources()
{
    D2D1_FACTORY_OPTIONS options;
    ZeroMemory(&options, sizeof(D2D1_FACTORY_OPTIONS));

#if defined(_DEBUG)
    // If the project is in a debug build, enable Direct2D debugging via SDK Layers.
    options.debugLevel = D2D1_DEBUG_LEVEL_INFORMATION;
#endif

    DX::ThrowIfFailed(
        D2D1CreateFactory(
            D2D1_FACTORY_TYPE_SINGLE_THREADED,
            __uuidof(ID2D1Factory1),
            &options,
            &m_d2dFactory
            )
        );

    DX::ThrowIfFailed(
        DWriteCreateFactory(
            DWRITE_FACTORY_TYPE_SHARED,
            __uuidof(IDWriteFactory),
            &m_dwriteFactory
            )
        );

    DX::ThrowIfFailed(
        CoCreateInstance(
            CLSID_WICImagingFactory,
            nullptr,
            CLSCTX_INPROC_SERVER,
            IID_PPV_ARGS(&m_wicFactory)
            )
        );
}

// These are the resources that depend on the device.
void DirectXBase::CreateDeviceResources()
{
    // This flag adds support for surfaces with a different color channel ordering
    // than the API default. It is required for compatibility with Direct2D.
    UINT creationFlags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;
    ComPtr<IDXGIDevice> dxgiDevice;

#if defined(_DEBUG)
    // If the project is in a debug build, enable debugging via SDK Layers with this flag.
    creationFlags |= D3D11_CREATE_DEVICE_DEBUG;
#endif

    // This array defines the set of DirectX hardware feature levels this app will support.
    // Note the ordering should be preserved.
    // Don't forget to declare your application's minimum required feature level in its
    // description.  All applications are assumed to support 9.1 unless otherwise stated.
    D3D_FEATURE_LEVEL featureLevels[] =
    {
        D3D_FEATURE_LEVEL_11_1,
        D3D_FEATURE_LEVEL_11_0,
        D3D_FEATURE_LEVEL_10_1,
        D3D_FEATURE_LEVEL_10_0,
        D3D_FEATURE_LEVEL_9_3,
        D3D_FEATURE_LEVEL_9_2,
        D3D_FEATURE_LEVEL_9_1
    };

    // Create the Direct3D 11 API device object and a corresponding context.
    ComPtr<ID3D11Device> device;
    ComPtr<ID3D11DeviceContext> context;
    DX::ThrowIfFailed(
        D3D11CreateDevice(
            nullptr,                    // Specify nullptr to use the default adapter.
            D3D_DRIVER_TYPE_HARDWARE,
            0,
            creationFlags,              // Set debug and Direct2D compatibility flags.
            featureLevels,              // List of feature levels this app can support.
            ARRAYSIZE(featureLevels),
            D3D11_SDK_VERSION,          // Always set this to D3D11_SDK_VERSION for Metro style apps.
            &device,                    // Returns the Direct3D device created.
            &m_featureLevel,            // Returns feature level of device created.
            &context                    // Returns the device immediate context.
            )
        );

    // Get the Direct3D 11.1 API device and context interfaces.
    DX::ThrowIfFailed(
        device.As(&m_d3dDevice)
        );

    DX::ThrowIfFailed(
        context.As(&m_d3dContext)
        );

    // Get the underlying DXGI device of the Direct3D device.
    DX::ThrowIfFailed(
        m_d3dDevice.As(&dxgiDevice)
        );

    // Create the Direct2D device object and a corresponding context.
    DX::ThrowIfFailed(
        m_d2dFactory->CreateDevice(dxgiDevice.Get(), &m_d2dDevice)
        );

    DX::ThrowIfFailed(
        m_d2dDevice->CreateDeviceContext(
            D2D1_DEVICE_CONTEXT_OPTIONS_NONE,
            &m_d2dContext
            )
        );
}

// Event handler for StereoEnabled changes.
// This is called whenever the Stereo changes.
void DirectXBase::OnStereoEnabledChanged(
    _In_ Platform::Object^ /* sender */
    )
{
    UpdateForWindowSizeChange();
}

// Helps track the DPI in the helper class.
// This is called in the dpiChanged event handler in the view class.
void DirectXBase::SetDpi(float dpi)
{
    if (dpi != m_dpi)
    {
        // Save the DPI of this display in our class.
        m_dpi = dpi;

        // Update Direct2D's stored DPI.
        m_d2dContext->SetDpi(m_dpi, m_dpi);

        // Often a DPI change implies a window size change. In some cases Windows will issue
        // both a size changed event and a DPI changed event. In this case, the resulting bounds
        // will not change, and the window resize code will only be executed once.
        UpdateForWindowSizeChange();
    }
}

void DirectXBase::CheckStereoEnabledStatus()
{
    // first, retrieve the underlying DXGI Device from the D3D Device
    ComPtr<IDXGIDevice1> dxgiDevice;
    DX::ThrowIfFailed(
        m_d3dDevice.As(&dxgiDevice)
        );

    // next, get the associated adapter from the DXGI Device
    ComPtr<IDXGIAdapter> dxgiAdapter;
    DX::ThrowIfFailed(
        dxgiDevice->GetAdapter(&dxgiAdapter)
        );

    // next, get the parent factory from the DXGI adapter
    ComPtr<IDXGIFactory2> dxgiFactory;
    DX::ThrowIfFailed(
        dxgiAdapter->GetParent(IID_PPV_ARGS(&dxgiFactory))
        );

    m_stereoEnabled = dxgiFactory->IsWindowedStereoEnabled() ? true : false;
}

// This routine is called in the event handler for the view SizeChanged event.
void DirectXBase::UpdateForWindowSizeChange()
{
    // Only handle window size changed if there is no pending DPI change.
    if (m_dpi != DisplayProperties::LogicalDpi)
    {
        return;
    }

    bool previousStereoState = m_stereoEnabled;
    CheckStereoEnabledStatus();
    if (previousStereoState != m_stereoEnabled)
    {
        // Swap chain needs to be recreated so release the existing one.
        // The rest of the dependent resources with be released in CreateWindowSizeDependentResources.
        m_swapChain = nullptr;
        CreateWindowSizeDependentResources();
    }
    else if (m_window->Bounds.Width != m_windowBounds.Width ||
        m_window->Bounds.Height != m_windowBounds.Height ||
        m_orientation != DisplayProperties::CurrentOrientation)
    {
        CreateWindowSizeDependentResources();
    }
}

// Allocate all memory resources that change on a window SizeChanged event.
void DirectXBase::CreateWindowSizeDependentResources()
{
    // Store the window bounds so the next time we get a SizeChanged event we can
    // avoid rebuilding everything if the size is identical.
    m_windowBounds = m_window->Bounds;

    // Calculate the necessary swap chain and render target size in pixels.
    auto windowWidth = ConvertDipsToPixels(m_windowBounds.Width);
    auto windowHeight = ConvertDipsToPixels(m_windowBounds.Height);

    // Swap width and height based on orientation.
    m_orientation = DisplayProperties::CurrentOrientation;
    bool swapDimensions = (
        m_orientation == DisplayOrientations::Portrait ||
        m_orientation == DisplayOrientations::PortraitFlipped
        );
    m_renderTargetSize.Width = swapDimensions ? windowHeight : windowWidth;
    m_renderTargetSize.Height = swapDimensions ? windowWidth : windowHeight;

    // If the swap chain already exists, resize it.
    if (m_swapChain != nullptr)
    {
        // Existing swap chain needs to be resized
        // make sure that the depend objects have been released.
        m_d3dContext->OMSetRenderTargets(0, nullptr, nullptr);
        m_d3dRenderTargetView = nullptr;
        m_d3dDepthStencilView = nullptr;
        m_d2dContext->SetTarget(nullptr);
        m_d2dTargetBitmap = nullptr;
        m_d2dTargetBitmapRight = nullptr;
        m_d3dRenderTargetViewRight = nullptr;

        HRESULT hr = m_swapChain->ResizeBuffers(
            2,
            static_cast<UINT>(m_renderTargetSize.Width),
            static_cast<UINT>(m_renderTargetSize.Height),
            DXGI_FORMAT_B8G8R8A8_UNORM,
            0
            );

        if (hr == DXGI_ERROR_DEVICE_REMOVED)
        {
            // If the device was removed for any reason, a new device and swapchain will need to be created.
            HandleDeviceLost();

            // Everything is set up now. Do not continue execution of this method.
            return;
        }
        else
        {
            DX::ThrowIfFailed(hr);
        }
    }
    else    // Otherwise, create a new one.
    {
        // m_swapChain is nullptr either because it has never been created or because it has been
        // invalidated. Make sure that the dependent objects are also released.
        m_d3dContext->OMSetRenderTargets(0, nullptr, nullptr);
        m_d3dRenderTargetView = nullptr;
        m_d3dDepthStencilView = nullptr;
        m_d2dContext->SetTarget(nullptr);
        m_d2dTargetBitmap = nullptr;
        m_d3dRenderTargetViewRight = nullptr;
        m_d2dTargetBitmapRight = nullptr;
        m_d3dContext->Flush();

        CheckStereoEnabledStatus();

        // Allocate a descriptor.
        DXGI_SWAP_CHAIN_DESC1 swapChainDesc = {0};
        swapChainDesc.Width = static_cast<UINT>(m_renderTargetSize.Width); // Match the size of the window.
        swapChainDesc.Height = static_cast<UINT>(m_renderTargetSize.Height);
        swapChainDesc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;           // this is the most common swapchain format
        swapChainDesc.Stereo = m_stereoEnabled;
        swapChainDesc.SampleDesc.Count = 1;                          // don't use multi-sampling
        swapChainDesc.SampleDesc.Quality = 0;
        swapChainDesc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
        swapChainDesc.BufferCount = 2;                               // use double buffering to enable flip
        swapChainDesc.Scaling = DXGI_SCALING_STRETCH;                // Required to be STRETCH for Composition
        swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL; // all Metro style apps must use this SwapEffect
        swapChainDesc.Flags = 0;

        // Once the desired swap chain description is configured, it must be created on the same adapter as our D3D Device

        // First, retrieve the underlying DXGI Device from the D3D Device.
        ComPtr<IDXGIDevice1> dxgiDevice;
        DX::ThrowIfFailed(
            m_d3dDevice.As(&dxgiDevice)
            );

        // Identify the physical adapter (GPU or card) this device is running on.
        ComPtr<IDXGIAdapter> dxgiAdapter;
        DX::ThrowIfFailed(
            dxgiDevice->GetAdapter(&dxgiAdapter)
            );

        // And obtain the factory object that created it.
        ComPtr<IDXGIFactory2> dxgiFactory;
        DX::ThrowIfFailed(
            dxgiAdapter->GetParent(IID_PPV_ARGS(&dxgiFactory))
            );

        // Create the swap chain and then associate it with the SwapChainBackgroundPanel.
        DX::ThrowIfFailed(
            dxgiFactory->CreateSwapChainForComposition(
                m_d3dDevice.Get(),
                &swapChainDesc,
                nullptr,    // allow on all displays
                &m_swapChain
                )
            );

        ComPtr<ISwapChainBackgroundPanelNative> dxRootPanelAsNative;

        // set the swap chain on the SwapChainBackgroundPanel
        reinterpret_cast<IUnknown*>(m_swapChainPanel)->QueryInterface(__uuidof(ISwapChainBackgroundPanelNative), (void**)&dxRootPanelAsNative);

        DX::ThrowIfFailed(
            dxRootPanelAsNative->SetSwapChain(m_swapChain.Get())
            );

        // Ensure that DXGI does not queue more than one frame at a time. This both reduces
        // latency and ensures that the application will only render after each VSync, minimizing
        // power consumption.
        DX::ThrowIfFailed(
            dxgiDevice->SetMaximumFrameLatency(1)
            );
    }

    // Set the proper orientation for the swap chain, and generate 2D and
    // 3D matrix transformations for rendering to the rotated swap chain.
    // Note the rotation angle for the 2D and 3D transforms are different.
    // This is due to the difference in coordinate spaces.  Additionally,
    // the 3D matrix is specified explicitly to avoid rounding errors.
    DXGI_MODE_ROTATION rotation = DXGI_MODE_ROTATION_UNSPECIFIED;
    switch (m_orientation)
    {
        case DisplayOrientations::Landscape:
            rotation = DXGI_MODE_ROTATION_IDENTITY;
            m_rotationTransform2D = Matrix3x2F::Identity();
            XMStoreFloat4x4(&m_rotationTransform3D, XMMatrixIdentity());
            break;

        case DisplayOrientations::Portrait:
            rotation = DXGI_MODE_ROTATION_ROTATE270;
            m_rotationTransform2D =
                Matrix3x2F::Rotation(270.0f) *
                Matrix3x2F::Translation(0.0f, m_windowBounds.Width);
            XMStoreFloat4x4(
                &m_rotationTransform3D,
                XMMATRIX( // 90-degree Z-rotation
                     0.0f, 1.0f, 0.0f, 0.0f,
                    -1.0f, 0.0f, 0.0f, 0.0f,
                     0.0f, 0.0f, 1.0f, 0.0f,
                     0.0f, 0.0f, 0.0f, 1.0f
                    )
                );
            break;

        case DisplayOrientations::LandscapeFlipped:
            rotation = DXGI_MODE_ROTATION_ROTATE180;
            m_rotationTransform2D =
                Matrix3x2F::Rotation(180.0f) *
                Matrix3x2F::Translation(m_windowBounds.Width, m_windowBounds.Height);
            XMStoreFloat4x4(
                &m_rotationTransform3D,
                XMMATRIX( // 180-degree Z-rotation
                    -1.0f,  0.0f, 0.0f, 0.0f,
                     0.0f, -1.0f, 0.0f, 0.0f,
                     0.0f,  0.0f, 1.0f, 0.0f,
                     0.0f,  0.0f, 0.0f, 1.0f
                    )
                );
            break;

        case DisplayOrientations::PortraitFlipped:
            rotation = DXGI_MODE_ROTATION_ROTATE90;
            m_rotationTransform2D =
                Matrix3x2F::Rotation(90.0f) *
                Matrix3x2F::Translation(m_windowBounds.Height, 0.0f);
            XMStoreFloat4x4(
                &m_rotationTransform3D,
                XMMATRIX( // 270-degree Z-rotation
                    0.0f, -1.0f, 0.0f, 0.0f,
                    1.0f,  0.0f, 0.0f, 0.0f,
                    0.0f,  0.0f, 1.0f, 0.0f,
                    0.0f,  0.0f, 0.0f, 1.0f
                    )
                );
            break;

        default:
            throw ref new Platform::FailureException();
            break;
    }

    DX::ThrowIfFailed(
        m_swapChain->SetRotation(rotation)
        );

    // Obtain the backbuffer for this window which will be the final 3D rendertarget.
    ComPtr<ID3D11Texture2D> backBuffer;
    DX::ThrowIfFailed(
        m_swapChain->GetBuffer(0, IID_PPV_ARGS(&backBuffer))
        );

    // Create a descriptor for the RenderTargetView.
    CD3D11_RENDER_TARGET_VIEW_DESC renderTargetViewDesc(
        D3D11_RTV_DIMENSION_TEXTURE2DARRAY,
        DXGI_FORMAT_B8G8R8A8_UNORM,
        0,
        0,
        1
        );

    // Create a view interface on the rendertarget to use on bind for mono or left eye view.
    DX::ThrowIfFailed(
        m_d3dDevice->CreateRenderTargetView(
            backBuffer.Get(),
            &renderTargetViewDesc,
            &m_d3dRenderTargetView
            )
        );

    // Stereo swapchains have an arrayed resource, so create a second Render Target
    // for the right eye buffer.
    if (m_stereoEnabled)
    {
        CD3D11_RENDER_TARGET_VIEW_DESC renderTargetViewRightDesc(
            D3D11_RTV_DIMENSION_TEXTURE2DARRAY,
            DXGI_FORMAT_B8G8R8A8_UNORM,
            0,
            1,
            1
            );

        DX::ThrowIfFailed(
            m_d3dDevice->CreateRenderTargetView(
                backBuffer.Get(),
                &renderTargetViewRightDesc,
                &m_d3dRenderTargetViewRight
                )
            );
    }

    // Create a descriptor for the depth/stencil buffer.
    CD3D11_TEXTURE2D_DESC depthStencilDesc(
        DXGI_FORMAT_D24_UNORM_S8_UINT,
        static_cast<UINT>(m_renderTargetSize.Width),
        static_cast<UINT>(m_renderTargetSize.Height),
        1,
        1,
        D3D11_BIND_DEPTH_STENCIL
        );

    // Allocate a 2-D surface as the depth/stencil buffer.
    ComPtr<ID3D11Texture2D> depthStencil;
    DX::ThrowIfFailed(
        m_d3dDevice->CreateTexture2D(
            &depthStencilDesc,
            nullptr,
            &depthStencil
            )
        );

    // Create a DepthStencil view on this surface to use on bind.
    auto viewDesc = CD3D11_DEPTH_STENCIL_VIEW_DESC(D3D11_DSV_DIMENSION_TEXTURE2D);
    DX::ThrowIfFailed(
        m_d3dDevice->CreateDepthStencilView(
            depthStencil.Get(),
            &viewDesc,
            &m_d3dDepthStencilView
            )
        );

    // Create a viewport descriptor of the full window size.
    CD3D11_VIEWPORT viewport(
        0.0f,
        0.0f,
        m_renderTargetSize.Width,
        m_renderTargetSize.Height
        );

    // Set the current viewport using the descriptor.
    m_d3dContext->RSSetViewports(1, &viewport);

    // Now we set up the Direct2D render target bitmap linked to the swapchain.
    // Whenever we render to this bitmap, it will be directly rendered to the
    // swapchain associated with the window.
    D2D1_BITMAP_PROPERTIES1 bitmapProperties =
        BitmapProperties1(
            D2D1_BITMAP_OPTIONS_TARGET | D2D1_BITMAP_OPTIONS_CANNOT_DRAW,
            PixelFormat(DXGI_FORMAT_B8G8R8A8_UNORM, D2D1_ALPHA_MODE_PREMULTIPLIED),
            m_dpi,
            m_dpi
            );

    // Direct2D needs the dxgi version of the backbuffer surface pointer.
    ComPtr<IDXGIResource1> dxgiBackBuffer;
    DX::ThrowIfFailed(
        m_swapChain->GetBuffer(0, IID_PPV_ARGS(&dxgiBackBuffer))
        );

    ComPtr<IDXGISurface2> dxgiSurface;
    DX::ThrowIfFailed(
        dxgiBackBuffer->CreateSubresourceSurface(0, &dxgiSurface)
        );

    // Get a D2D surface from the DXGI back buffer to use as the D2D render target.
    DX::ThrowIfFailed(
        m_d2dContext->CreateBitmapFromDxgiSurface(
            dxgiSurface.Get(),
            &bitmapProperties,
            &m_d2dTargetBitmap
            )
        );

    // Stereo swapchains have an arrayed resource, so create a second Target Bitmap
    // for the right eye buffer.
    if (m_stereoEnabled)
    {
        DX::ThrowIfFailed(
            dxgiBackBuffer->CreateSubresourceSurface(1, &dxgiSurface)
            );
        DX::ThrowIfFailed(
            m_d2dContext->CreateBitmapFromDxgiSurface(
                dxgiSurface.Get(),
                &bitmapProperties,
                &m_d2dTargetBitmapRight
            )
        );
    }

    // So now we can set the Direct2D render target.
    m_d2dContext->SetTarget(m_d2dTargetBitmap.Get());

    // Set D2D text anti-alias mode to Grayscale to ensure proper rendering of text on intermediate surfaces.
    m_d2dContext->SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE_GRAYSCALE);
}

// Method to deliver the final image to the display.
void DirectXBase::Present()
{
    // The application may optionally specify "dirty" or "scroll" rects to improve efficiency
    // in certain scenarios.  In this sample, however, we do not utilize those features.
    DXGI_PRESENT_PARAMETERS parameters = {0};
    parameters.DirtyRectsCount = 0;
    parameters.pDirtyRects = nullptr;
    parameters.pScrollRect = nullptr;
    parameters.pScrollOffset = nullptr;

    // The first argument instructs DXGI to block until VSync, putting the application
    // to sleep until the next VSync. This ensures we don't waste any cycles rendering
    // frames that will never be displayed to the screen.
    HRESULT hr = m_swapChain->Present1(1, 0, &parameters);

    // If the device was removed either by a disconnect or a driver upgrade, we
    // must recreate all device resources.
    if (hr == DXGI_ERROR_DEVICE_REMOVED)
    {
        // Send notifications via the DeviceLost event to any registered handlers.
        DeviceLost();
        // Recreate the Device Resources.
        HandleDeviceLost();
        // Send notifications via the DeviceReset event to any registered handlers.
        DeviceReset();
    }
    else
    {
        DX::ThrowIfFailed(hr);
    }
}

// Method to convert a length in device-independent pixels (DIPs) to a length in physical pixels.
float DirectXBase::ConvertDipsToPixels(float dips)
{
    static const float dipsPerInch = 96.0f;
    return floor(dips * m_dpi / dipsPerInch + 0.5f); // Round to nearest integer.
}
