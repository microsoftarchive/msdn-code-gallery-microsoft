//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "PhysicalDpiRenderer.h"

#include "DirectXHelper.h"

using namespace PhysicalDpi;

using namespace DirectX;
using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;

// A helper method to convert lengths from inches to DIPs (device-independent pixels).
static inline float ConvertInchesToDips(float inches)
{
    return inches * 96.0f; // 1 inch = 96 DIPs.
}

// Initialization.
PhysicalDpiRenderer::PhysicalDpiRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources) :
    m_deviceResources(deviceResources)
{
    DX::ThrowIfFailed(
        m_deviceResources->GetDWriteFactory()->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_NORMAL,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            24.0f,
            L"en-us",
            &m_textFormat
            )
        );

    CreateDeviceDependentResources();
    CreateWindowSizeDependentResources();
}

// Initialization.
void PhysicalDpiRenderer::CreateDeviceDependentResources()
{
    DX::ThrowIfFailed(
        m_deviceResources->GetD2DDeviceContext()->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::Black),
            &m_blackBrush
            )
        );
    
    DX::ThrowIfFailed(
        m_deviceResources->GetD2DDeviceContext()->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::DarkGoldenrod),
            &m_goldBrush
            )
        );

    DX::ThrowIfFailed(
        m_deviceResources->GetD2DDeviceContext()->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::White),
            &m_whiteBrush
            )
        );
}

// Initialization.
void PhysicalDpiRenderer::CreateWindowSizeDependentResources()
{
}

// Release all references to resources that depend on the graphics device.
// This method is invoked when the device is lost and resources are no longer usable.
void PhysicalDpiRenderer::ReleaseDeviceDependentResources()
{
    m_blackBrush.Reset();
    m_goldBrush.Reset();
    m_whiteBrush.Reset();
}

// Renders one frame. This method draws a small ruler at the correct physical size.
// The distance ticks on the ruler should be accurate, regardless of any user or 
// OS DPI settings.
void PhysicalDpiRenderer::Render()
{
    ComPtr<ID2D1DeviceContext1> deviceContext = m_deviceResources->GetD2DDeviceContext();

    // Retrieve the physical DPI (raw DPI) of the current display. This is the actual pixel density
    // reported by the display. These values do not change based on any user or OS settings.
    DisplayInformation^ displayInfo = DisplayInformation::GetForCurrentView();
    float rawDpiX = displayInfo->RawDpiX;
    float rawDpiY = displayInfo->RawDpiY;

    // Save the device context's current DPI so that it can be restored later.
    float savedDpiX;
    float savedDpiY;
    deviceContext->GetDpi(&savedDpiX, &savedDpiY);

    // Set the device context's DPI to the physical DPI. Note that the DisplayInformation object
    // returns 0 for RawDpiX and RawDpiY if the monitor does not report a DPI or if the monitor
    // is being run in Duplicate mode. Passing 0 values to SetDpi specifies the factory-read system
    // DPI.
    deviceContext->SetDpi(rawDpiX, rawDpiY);

    // Compute the physical size (in DIPs) of the current display from its pixel size. The physical
    // size is useful when laying out elements based on physical DPI.
    Size pixelSize = m_deviceResources->GetOutputSize();
    Size physicalSize(
        (pixelSize.Width / displayInfo->RawDpiX) * 96.0f,
        (pixelSize.Height / displayInfo->RawDpiY) * 96.0f
        );

    deviceContext->BeginDraw();

    // Set the transform to the physical (raw) orientation transform, so that content rendered
    // according to the physical DPI is rotated and translated appropriately.
    deviceContext->SetTransform(m_deviceResources->GetRawOrientationTransform2D());

    // Draw a horizontal ruler: a rectangle five inches wide and half an inch tall. By
    // using the physical DPI, this ruler will have this size regardless of any user or
    // OS DPI settings.
    float xPos = ConvertInchesToDips(1.0f);
    float yPos = ConvertInchesToDips(1.0f);
    float width = ConvertInchesToDips(5.0f);
    float height = ConvertInchesToDips(0.5f);
    deviceContext->FillRectangle(
        D2D1::RectF(xPos, yPos, xPos + width, yPos + height),
        m_goldBrush.Get()
        );

    // Draw ticks to denote the one-inch intervals on the ruler.
    for (float tick = 1.0f; tick <= 4.0f; tick += 1.0f)
    {
        deviceContext->DrawLine(
            D2D1::Point2F(xPos + ConvertInchesToDips(tick), yPos),
            D2D1::Point2F(xPos + ConvertInchesToDips(tick), yPos + ConvertInchesToDips(0.2f)),
            m_blackBrush.Get()
            );
    }

    // Draw ticks to denote the half-inch intervals on the ruler.
    for (float tick = 0.5f; tick <= 4.5f; tick += 1.0f)
    {
        deviceContext->DrawLine(
            D2D1::Point2F(xPos + ConvertInchesToDips(tick), yPos),
            D2D1::Point2F(xPos + ConvertInchesToDips(tick), yPos + ConvertInchesToDips(0.1f)),
            m_blackBrush.Get()
            );
    }

    // Draw ticks to denote the quarter-inch intervals on the ruler.
    for (float tick = 0.25f; tick <= 4.75f; tick += 0.5f)
    {
        deviceContext->DrawLine(
            D2D1::Point2F(xPos + ConvertInchesToDips(tick), yPos),
            D2D1::Point2F(xPos + ConvertInchesToDips(tick), yPos + ConvertInchesToDips(0.05f)),
            m_blackBrush.Get()
            );
    }

    // Draw a vertical ruler: a rectangle 3 inches tall and half an inch wide. By
    // using the physical DPI, this ruler will have this size regardless of any user or
    // OS DPI settings.
    xPos = ConvertInchesToDips(1.0f);
    yPos = ConvertInchesToDips(2.0f);
    width = ConvertInchesToDips(0.5f);
    height = ConvertInchesToDips(3.0f);
    deviceContext->FillRectangle(
        D2D1::RectF(xPos, yPos, xPos + width, yPos + height),
        m_goldBrush.Get()
        );

    // Draw ticks to denote the one-inch intervals on the ruler.
    for (float tick = 1.0f; tick <= 2.0f; tick += 1.0f)
    {
        deviceContext->DrawLine(
            D2D1::Point2F(xPos, yPos + ConvertInchesToDips(tick)),
            D2D1::Point2F(xPos + ConvertInchesToDips(0.2f), yPos + ConvertInchesToDips(tick)),
            m_blackBrush.Get()
            );
    }

    // Draw ticks to denote the half-inch intervals on the ruler.
    for (float tick = 0.5f; tick <= 2.5f; tick += 1.0f)
    {
        deviceContext->DrawLine(
            D2D1::Point2F(xPos, yPos + ConvertInchesToDips(tick)),
            D2D1::Point2F(xPos + ConvertInchesToDips(0.1f), yPos + ConvertInchesToDips(tick)),
            m_blackBrush.Get()
            );
    }

    // Draw ticks to denote the quarter-inch intervals on the ruler.
    for (float tick = 0.25f; tick <= 2.75f; tick += 0.5f)
    {
        deviceContext->DrawLine(
            D2D1::Point2F(xPos, yPos + ConvertInchesToDips(tick)),
            D2D1::Point2F(xPos + ConvertInchesToDips(0.05f), yPos + ConvertInchesToDips(tick)),
            m_blackBrush.Get()
            );
    }

    // Draw a message explaining the sizes of the rulers.
    String^ message = "The ruler above is 5 inches long. The ruler to the left is 3 "
        "inches long. Their sizes are independent of user and OS DPI settings.";

    xPos = ConvertInchesToDips(2.0f);
    yPos = ConvertInchesToDips(2.0f);
    deviceContext->DrawText(
        message->Data(),
        message->Length(),
        m_textFormat.Get(),
        D2D1::RectF(xPos, yPos, physicalSize.Width - ConvertInchesToDips(1.0f), physicalSize.Height - ConvertInchesToDips(1.0f)),
        m_whiteBrush.Get()
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = deviceContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    // Restore the DPI Direct2D was using. The rest of the sample will be drawn using the
    // logical DPI, rather than the physical DPI, so it will change size in accordance
    // with user DPI settings.
    deviceContext->SetDpi(savedDpiX, savedDpiY);
}
