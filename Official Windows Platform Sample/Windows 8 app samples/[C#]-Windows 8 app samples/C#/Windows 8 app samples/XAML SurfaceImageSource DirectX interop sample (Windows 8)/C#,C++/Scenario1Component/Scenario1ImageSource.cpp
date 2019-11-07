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
#include "Scenario1ImageSource.h"
#include "DirectXSample.h"

using namespace Scenario1Component;
using namespace Platform;
using namespace Microsoft::WRL;

Scenario1ImageSource::Scenario1ImageSource(int pixelWidth, int pixelHeight, bool isOpaque) :
    SurfaceImageSource(pixelWidth, pixelHeight, isOpaque)
{
    m_width = pixelWidth;
    m_height = pixelHeight;

    CreateDeviceIndependentResources();
    CreateDeviceResources();
}

// Initialize resources that are independent of hardware.
void Scenario1ImageSource::CreateDeviceIndependentResources()
{
    // Query for ISurfaceImageSourceNative interface.
    DX::ThrowIfFailed(
        reinterpret_cast<IUnknown*>(this)->QueryInterface(IID_PPV_ARGS(&m_sisNative))
        );
}

// Initialize hardware-dependent resources.
void Scenario1ImageSource::CreateDeviceResources()
{
    // This flag adds support for surfaces with a different color channel ordering
    // than the API default. It is required for compatibility with Direct2D.
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

    // Create the Direct3D 11 API device object.
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
            nullptr
            )
        );

    // Get the Direct3D 11.1 API device.
    ComPtr<IDXGIDevice> dxgiDevice;
    DX::ThrowIfFailed(
        m_d3dDevice.As(&dxgiDevice)
        );

    // Create the Direct2D device object and a corresponding context.
    DX::ThrowIfFailed(
        D2D1CreateDevice(
            dxgiDevice.Get(),
            nullptr,
            &m_d2dDevice
            )
        );

    DX::ThrowIfFailed(
        m_d2dDevice->CreateDeviceContext(
            D2D1_DEVICE_CONTEXT_OPTIONS_NONE,
            &m_d2dContext
            )
        );

    // Set DPI to the display's current DPI.
    SetDpi(Windows::Graphics::Display::DisplayProperties::LogicalDpi);

    // Associate the DXGI device with the SurfaceImageSource.
    DX::ThrowIfFailed(
        m_sisNative->SetDevice(dxgiDevice.Get())
        );
}

// Sets the current DPI.
void Scenario1ImageSource::SetDpi(float dpi)
{
    // Update Direct2D's stored DPI.
    m_d2dContext->SetDpi(dpi, dpi);
}

// Begins drawing, allowing updates to content in the specified area.
void Scenario1ImageSource::BeginDraw(Windows::Foundation::Rect updateRect)
{    
    POINT offset;
    ComPtr<IDXGISurface> surface;

    // Express target area as a native RECT type.
    RECT updateRectNative; 
    updateRectNative.left = static_cast<LONG>(updateRect.Left);
    updateRectNative.top = static_cast<LONG>(updateRect.Top);
    updateRectNative.right = static_cast<LONG>(updateRect.Right);
    updateRectNative.bottom = static_cast<LONG>(updateRect.Bottom);

    // Begin drawing - returns a target surface and an offset to use as the top left origin when drawing.
    HRESULT beginDrawHR = m_sisNative->BeginDraw(updateRectNative, &surface, &offset);
 
    if (beginDrawHR == DXGI_ERROR_DEVICE_REMOVED || beginDrawHR == DXGI_ERROR_DEVICE_RESET)
    {
        // If the device has been removed or reset, attempt to recreate it and continue drawing.
        CreateDeviceResources();
        BeginDraw(updateRect);
    }
    else
    {
        // Notify the caller by throwing an exception if any other error was encountered.
        DX::ThrowIfFailed(beginDrawHR);
    }

    // Create render target.
    ComPtr<ID2D1Bitmap1> bitmap;
    DX::ThrowIfFailed(
        m_d2dContext->CreateBitmapFromDxgiSurface(
            surface.Get(),
            nullptr,
            &bitmap
            )
        );
    
    // Set context's render target.
    m_d2dContext->SetTarget(bitmap.Get());

    // Begin drawing using D2D context.
    m_d2dContext->BeginDraw();

    // Apply a clip and transform to constrain updates to the target update area.
    // This is required to ensure coordinates within the target surface remain
    // consistent by taking into account the offset returned by BeginDraw, and
    // can also improve performance by optimizing the area that is drawn by D2D.
    // Apps should always account for the offset output parameter returned by 
    // BeginDraw, since it may not match the passed updateRect input parameter's location.
    m_d2dContext->PushAxisAlignedClip(
        D2D1::RectF(
            static_cast<float>(offset.x),  
            static_cast<float>(offset.y),  
            static_cast<float>(offset.x + updateRect.Width),
            static_cast<float>(offset.y + updateRect.Height)  
            ),  
        D2D1_ANTIALIAS_MODE_ALIASED  
        );

    m_d2dContext->SetTransform(
        D2D1::Matrix3x2F::Translation(
            static_cast<float>(offset.x),
            static_cast<float>(offset.y)
            )
        );
}

// Ends drawing updates started by a previous BeginDraw call.
void Scenario1ImageSource::EndDraw()
{
    // Remove the transform and clip applied in BeginDraw since
    // the target area can change on every update.
    m_d2dContext->SetTransform(D2D1::IdentityMatrix());
    m_d2dContext->PopAxisAlignedClip();

    // Remove the render target and end drawing.
    DX::ThrowIfFailed(
        m_d2dContext->EndDraw()
        );

    m_d2dContext->SetTarget(nullptr);

    DX::ThrowIfFailed(
        m_sisNative->EndDraw()
        );
}

// Clears the background with the given color.
void Scenario1ImageSource::Clear(Windows::UI::Color color)
{
    m_d2dContext->Clear(DX::ConvertToColorF(color));
}

// Draws a filled rectangle with the given color and position.
void Scenario1ImageSource::FillSolidRect(Windows::UI::Color color, Windows::Foundation::Rect rect)
{
    // Create a solid color D2D brush.
    ComPtr<ID2D1SolidColorBrush> brush;
    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            DX::ConvertToColorF(color),
            &brush
            )
        );

    // Draw a filled rectangle.
    m_d2dContext->FillRectangle(DX::ConvertToRectF(rect), brush.Get());
}
