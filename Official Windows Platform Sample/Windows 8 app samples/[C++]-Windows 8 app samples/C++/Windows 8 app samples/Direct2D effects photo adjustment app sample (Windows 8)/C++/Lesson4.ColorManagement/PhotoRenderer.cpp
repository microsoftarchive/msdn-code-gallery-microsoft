//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "PhotoRenderer.h"

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace concurrency;
using namespace std;


static const unsigned int ExifColorspaceSrgb = 1; // Defined by the EXIF specification

PhotoRenderer::PhotoRenderer() :
    m_bitmapPixelSize()
{
}

void PhotoRenderer::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    // Load the source image using a Windows Imaging Component decoder.
    ComPtr<IWICBitmapDecoder> decoder;
    DX::ThrowIfFailed(
        m_wicFactory->CreateDecoderFromFilename(
            L"mammoth.jpg",
            nullptr,
            GENERIC_READ,
            WICDecodeMetadataCacheOnDemand,
            &decoder
            )
        );

    ComPtr<IWICBitmapFrameDecode> frame;
    DX::ThrowIfFailed(
        decoder->GetFrame(0, &frame)
        );

    // Lesson 4:
    // Get a WIC color context to store the image's embedded color space data.
    DX::ThrowIfFailed(
        m_wicFactory->CreateColorContext(&m_wicColorContext)
        );

    unsigned int actualCount = 0;
    // We only care about the first color context in the frame, hence the "1".
    DX::ThrowIfFailed(
        frame->GetColorContexts(1, m_wicColorContext.GetAddressOf(), &actualCount)
        );

    // If the frame does not have any color contexts, we default to the sRGB color space.
    if (actualCount == 0)
    {
        DX::ThrowIfFailed(
            m_wicColorContext->InitializeFromExifColorSpace(ExifColorspaceSrgb)
            );
    }

    DX::ThrowIfFailed(
        m_wicFactory->CreateFormatConverter(&m_wicFormatConverter)
        );

    // We format convert to a pixel format that is compatible with Direct2D.
    // To optimize for performance when using WIC and Direct2D together, we need to
    // select the target pixel format based on the image's native precision:
    // - <= 8 bits per channel precision: use BGRA channel order
    //   (example: all JPEGs, including the image in this sample, are 8bpc)
    // -  > 8 bits per channel precision: use RGBA channel order
    //   (example: TIFF and JPEG-XR images support 32bpc float
    // Note that a fully robust system will arbitrate between various WIC pixel formats and
    // hardware feature level support using the IWICPixelFormatInfo2 interface.
    DX::ThrowIfFailed(
        m_wicFormatConverter->Initialize(
            frame.Get(),
            GUID_WICPixelFormat32bppPBGRA,
            WICBitmapDitherTypeNone,
            nullptr,
            0.0f,
            WICBitmapPaletteTypeCustom // premultiplied BGRA has no paletting, so this is ignored
            )
        );

    UINT width;
    UINT height;
    DX::ThrowIfFailed(
        m_wicFormatConverter->GetSize(&width, &height)
        );

    // Store the WIC bitmap size in pixels - this remains constant regardless of the app's DPI.
    m_bitmapPixelSize = D2D1::SizeU(width, height);
}

void PhotoRenderer::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct2D photo adjustment sample lesson 4: color management"
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1BitmapSource, &m_bitmapSourceEffect)
        );

    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(
            D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE,
            m_wicFormatConverter.Get()
            )
        );

    // The mipmap linear interpolation mode on the BitmapSource effect instructs it to construct
    // a software (CPU memory) mipmap. If Direct2D requests the image at a small scale factor
    // (<= 0.5), BitmapSource only needs to perform the scale operation from the nearest
    // mip level, which is a performance optimization. In addition, for very large images,
    // performing scaling on the CPU reduces GPU bus bandwidth consumption and GPU memory
    // consumption.
    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(
            D2D1_BITMAPSOURCE_PROP_INTERPOLATION_MODE,
            D2D1_BITMAPSOURCE_INTERPOLATION_MODE_MIPMAP_LINEAR
            )
        );

    // Lesson 4:
    // Create the first ColorManagement effect. This effect converts betwen the image's embedded
    // color space to a standard working color space in which to perform image processing.
    // This application uses sRGB; other pipelines may choose to use a color space with a different
    // (linear) gamma or a wider gamut, such as scRGB.
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1ColorManagement, &m_colorManagementEffectWorkingSpace)
        );

    // Lesson 4:
    // Create a D2D color context to set as the source color space.
    ComPtr<ID2D1ColorContext> imageColorContext;
    DX::ThrowIfFailed(
        m_d2dContext->CreateColorContextFromWicColorContext(
            m_wicColorContext.Get(),
            &imageColorContext
            )
        );

    DX::ThrowIfFailed(
        m_colorManagementEffectWorkingSpace->SetValue(
            D2D1_COLORMANAGEMENT_PROP_SOURCE_COLOR_CONTEXT,
            imageColorContext.Get()
            )
        );

    // Lesson 4:
    // Create and set the destination color space.
    ComPtr<ID2D1ColorContext> workingColorContext;
    DX::ThrowIfFailed(
        m_d2dContext->CreateColorContext(D2D1_COLOR_SPACE_SRGB, nullptr, 0, &workingColorContext)
        );

    DX::ThrowIfFailed(
        m_colorManagementEffectWorkingSpace->SetValue(
            D2D1_COLORMANAGEMENT_PROP_DESTINATION_COLOR_CONTEXT,
            workingColorContext.Get()
            )
        );

    // Lesson 4:
    // Create the second ColorManagement effect. This effect converts between the working space
    // and the display's color space. To get the display color space, we invoke the color profile
    // changed handler; this returns the display color context asynchronously.
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1ColorManagement, &m_colorManagementEffectDisplaySpace)
        );

    DX::ThrowIfFailed(
        m_colorManagementEffectDisplaySpace->SetValue(
            D2D1_COLORMANAGEMENT_PROP_SOURCE_COLOR_CONTEXT,
            workingColorContext.Get()
            )
        );

    // Lesson 4:
    // We know the source image used in this scenario is opaque (JPEG does not support alpha).
    // ColorManagement internally operates on straight alpha data, so we can avoid a premultiply/
    // un-premultiply operation by setting the effect's alpha mode to Straight.
    DX::ThrowIfFailed(
        m_colorManagementEffectWorkingSpace->SetValue(
            D2D1_COLORMANAGEMENT_PROP_ALPHA_MODE,
            D2D1_COLORMANAGEMENT_ALPHA_MODE_STRAIGHT
            )
        );

    DX::ThrowIfFailed(
        m_colorManagementEffectDisplaySpace->SetValue(
            D2D1_COLORMANAGEMENT_PROP_ALPHA_MODE,
            D2D1_COLORMANAGEMENT_ALPHA_MODE_STRAIGHT
            )
        );

    m_colorManagementEffectWorkingSpace->SetInputEffect(0, m_bitmapSourceEffect.Get());

    // Lesson 4:
    // All photo adjustment work, such as changing brightness or color temperature, should be done
    // in the working color space for the most accurate results (placed between the two color management effects).

    m_colorManagementEffectDisplaySpace->SetInputEffect(0, m_colorManagementEffectWorkingSpace.Get());
}

void PhotoRenderer::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // Compute a zoom value so that the entire image fits on the screen.
    float zoom = max(
        m_renderTargetSize.Width / m_bitmapPixelSize.width,
        m_renderTargetSize.Height / m_bitmapPixelSize.height
        );

    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(
            D2D1_BITMAPSOURCE_PROP_SCALE,
            D2D1::Vector2F(
                zoom,
                zoom
                )
            )
        );
}

void PhotoRenderer::Render()
{
    m_d2dContext->BeginDraw();

    // Lesson 4:
    // Render the last effect (display ColorManagement). The source image we are rendering does
    // not contain alpha; as an optimization we use the source copy composite mode which
    // ignores alpha. If we were rendering an image with alpha (e.g. a PNG image with alpha)
    // then we would need to use another composite mode such as source over.
    // The interpolation mode is ignored here because there is no world transform
    // (no scaling/interpolation in the draw operation).
    m_d2dContext->DrawImage(
        m_colorManagementEffectDisplaySpace.Get(),
        D2D1_INTERPOLATION_MODE_LINEAR,
        D2D1_COMPOSITE_MODE_SOURCE_COPY
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    m_sampleOverlay->Render();

    Present();
}

void PhotoRenderer::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &PhotoRenderer::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &PhotoRenderer::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &PhotoRenderer::OnResuming);
}

void PhotoRenderer::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(
            this,
            &PhotoRenderer::OnWindowSizeChanged
            );

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &PhotoRenderer::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &PhotoRenderer::OnDisplayContentsInvalidated);

    try
    {
        DisplayProperties::ColorProfileChanged +=
            ref new DisplayPropertiesEventHandler(this, &PhotoRenderer::OnColorProfileChanged);
    }
    catch (Platform::FailureException^ e)
    {
        // Lesson 4:
        // If there is no physical display (such as when running over remote desktop or in the Simulator)
        // this registration will throw a FailureException (HRESULT E_FAIL).
    }

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

// Retrieve application state from LocalSettings if the application was previously suspended.
void PhotoRenderer::Load(
    _In_ Platform::String^ entryPoint
    )
{
    // This app does not store any state.
}

void PhotoRenderer::Run()
{
    // Lesson 4:
    // Kick off the async operation to initialize the display color context.
    // This must be called after the effects graph and context are fully setup,
    // as the completion callback will call Render().
    OnColorProfileChanged(this);

    Render();

    // ProcessUntilQuit blocks until the app closes (while still firing input events).
    m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);
}

void PhotoRenderer::Uninitialize()
{
}

void PhotoRenderer::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();
    Render();
}

void PhotoRenderer::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
}

void PhotoRenderer::OnDisplayContentsInvalidated(
    _In_ Platform::Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    Render();
}

void PhotoRenderer::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

// Save application state when suspending. If the application is closed by the process lifetime
// manager, this allows it to resume in the same state as the user left it.
void PhotoRenderer::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    // Lesson 4:
    // We do not save the color management state. The image's embedded color profile is loaded
    // from the input image, and the display color profile is retrieved from the system.
}

void PhotoRenderer::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
}

void PhotoRenderer::OnColorProfileChanged(_In_ Object^ sender)
{
    // Lesson 4:
    // The color profile is stored in the DisplayProperties static object.
    // We need to kick off an asynchronous operation to load the information
    // before passing it to the renderer to update the image graph.
    shared_ptr<DataReader^> profileReaderPtr = make_shared<DataReader^>();

    create_task(DisplayProperties::GetColorProfileAsync()).then([=](IRandomAccessStream^ profileStream)
    {
        // Extract the profile bytes from the IRandomAccessStream to initialize the Direct2D color context.
        *profileReaderPtr = ref new DataReader(profileStream);

        return (*profileReaderPtr)->LoadAsync(static_cast<unsigned int>(profileStream->Size));
    }).then([=](unsigned int readBytes)
    {
        UpdateDisplayColorContext(*profileReaderPtr);
        Render();
    }).then([=](task<void> previousTask)
    {
        try
        {
            // Any errors from the preceding async operations will throw here.
            previousTask.get();
        }
        catch (Platform::COMException^ e)
        {
            if (e->HResult == HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND))
            {
                // ERROR_FILE_NOT_FOUND is returned from GetColorProfileAsync when the display does not
                // have a color profile associated with it, implying the sRGB color space.
                UpdateDisplayColorContext(nullptr);
                Render();
            }
            else
            {
                throw;
            }
        }
    });
}

void PhotoRenderer::UpdateDisplayColorContext(
    _In_ Windows::Storage::Streams::DataReader^ colorProfileDataReader
    )
{
    ComPtr<ID2D1ColorContext> displayColorContext;

    if (colorProfileDataReader == nullptr)
    {
        // Lesson 4:
        // If there is no color profile data, we assume the standard sRGB color space.
        DX::ThrowIfFailed(
            m_d2dContext->CreateColorContext(D2D1_COLOR_SPACE_SRGB, nullptr, 0, &displayColorContext)
            );
    }
    else
    {
        Platform::Array<byte>^ profileBytes = ref new Platform::Array<byte>(colorProfileDataReader->UnconsumedBufferLength);
        colorProfileDataReader->ReadBytes(profileBytes);
        DX::ThrowIfFailed(
            m_d2dContext->CreateColorContext(
                D2D1_COLOR_SPACE_CUSTOM,
                profileBytes->Data,
                profileBytes->Length,
                &displayColorContext
                )
            );
    }

    DX::ThrowIfFailed(
        m_colorManagementEffectDisplaySpace->SetValue(
            D2D1_COLORMANAGEMENT_PROP_DESTINATION_COLOR_CONTEXT,
            displayColorContext.Get()
            )
        );
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new PhotoRenderer();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
