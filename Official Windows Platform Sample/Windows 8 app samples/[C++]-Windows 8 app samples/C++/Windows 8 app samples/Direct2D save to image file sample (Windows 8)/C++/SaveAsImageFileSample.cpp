//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "SaveAsImageFileSample.h"

using namespace concurrency;
using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;
using namespace Windows::System;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;

SaveAsImageFile::SaveAsImageFile() :
    m_screenSavingState(ScreenSavingState::NotSaved),
    m_isSnapped(false)
{
}

void SaveAsImageFile::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    // Create a DirectWrite text format object.
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Segoe UI",                // font family name
            nullptr,                    // system font collection
            DWRITE_FONT_WEIGHT_LIGHT,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            64,                         // font size
            L"en-US",                   // locale
            &m_textFormat
            )
        );

    // Center the text horizontally.
    DX::ThrowIfFailed(
        m_textFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER)
        );

    // Center the text vertically.
    DX::ThrowIfFailed(
        m_textFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER)
        );

    // Create a DirectWrite text format object for the snapped view message.
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Segoe UI",                // font family name
            nullptr,                    // system font collection
            DWRITE_FONT_WEIGHT_LIGHT,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            20,                         // font size
            L"en-US",                   // locale
            &m_snappedViewFormat
            )
        );

    // Center the text horizontally.
    DX::ThrowIfFailed(
        m_snappedViewFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER)
        );

    // Center the text vertically.
    DX::ThrowIfFailed(
        m_snappedViewFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER)
        );
}

void SaveAsImageFile::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct2D save to image file sample"
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::Black),
            &m_blackBrush
            )
        );
}

void SaveAsImageFile::Render()
{
    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));
    m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());

    Platform::String^ string;
    IDWriteTextFormat *format;

    if (m_isSnapped)
    {
        string = "This sample does not support snapped view.";
        format = m_snappedViewFormat.Get();
    }
    else if (m_screenSavingState == ScreenSavingState::NotSaved || m_screenSavingState == ScreenSavingState::Saving)
    {
        string = "Tap to save screen.";
        format = m_textFormat.Get();
    }
    else
    {
        string = Platform::String::Concat("Screen saved to ", m_imageFileName);
        format = m_textFormat.Get();
    }

    D2D1_SIZE_F size = m_d2dContext->GetSize();

    m_d2dContext->DrawText(
        string->Data(),
        string->Length(),
        format,
        D2D1::RectF(0, 0, size.width, size.height),
        m_blackBrush.Get()
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    m_sampleOverlay->Render();
}

// Save render target bitmap to an image file, using file picker for users to input the image file name.
void SaveAsImageFile::SaveBitmapToFile()
{
    if (m_isSnapped || m_screenSavingState != ScreenSavingState::NotSaved || m_d2dTargetBitmap == nullptr)
    {
        return;
    }

    // Set state to Saving to block another copy of this function being called before saving completes.
    m_screenSavingState = ScreenSavingState::Saving;
    Render();
    Present();

    // Prepare a file picker for customers to input image file name.
    Pickers::FileSavePicker^ savePicker = ref new Pickers::FileSavePicker();
    auto pngExtensions = ref new Platform::Collections::Vector<Platform::String^>();
    pngExtensions->Append(".png");
    savePicker->FileTypeChoices->Insert("PNG file", pngExtensions);
    auto jpgExtensions = ref new Platform::Collections::Vector<Platform::String^>();
    jpgExtensions->Append(".jpg");
    savePicker->FileTypeChoices->Insert("JPEG file", jpgExtensions);
    auto bmpExtensions = ref new Platform::Collections::Vector<Platform::String^>();
    bmpExtensions->Append(".bmp");
    savePicker->FileTypeChoices->Insert("BMP file", bmpExtensions);
    savePicker->DefaultFileExtension = ".png";
    savePicker->SuggestedFileName = "SaveScreen";
    savePicker->SuggestedStartLocation = Pickers::PickerLocationId::PicturesLibrary;

    std::shared_ptr<GUID> wicFormat = std::make_shared<GUID>(GUID_ContainerFormatPng);

    create_task(savePicker->PickSaveFileAsync()).then([=](StorageFile^ file)
    {
        if (file == nullptr)
        {
            // If user clicks "Cancel", reset the saving state, then cancel the current task.
            m_screenSavingState = ScreenSavingState::NotSaved;
            cancel_current_task();
        }

        m_imageFileName = file->Name;
        if (file->FileType == ".bmp")
        {
            *wicFormat = GUID_ContainerFormatBmp;
        }
        else if (file->FileType == ".jpg")
        {
            *wicFormat = GUID_ContainerFormatJpeg;
        }
        return file->OpenAsync(FileAccessMode::ReadWrite);

    }).then([=](Streams::IRandomAccessStream^ randomAccessStream)
    {
        // Convert the RandomAccessStream to an IStream.
        ComPtr<IStream> stream;
        DX::ThrowIfFailed(
            CreateStreamOverRandomAccessStream(randomAccessStream, IID_PPV_ARGS(&stream))
            );

        // Render the screen contents to an off-screen bitmap and then save that bitmap to a file.
        ComPtr<ID2D1Bitmap1> targetBitmap;
        D2D1_SIZE_U pixelSize = m_d2dContext->GetPixelSize();

        DX::ThrowIfFailed(
            m_d2dContext->CreateBitmap(
                pixelSize,
                nullptr,
                pixelSize.width * 4,    // pitch = width * size of pixel (4 bytes for B8G8R8A8)
                D2D1::BitmapProperties1(
                    D2D1_BITMAP_OPTIONS_TARGET,
                    D2D1::PixelFormat(DXGI_FORMAT_B8G8R8A8_UNORM, D2D1_ALPHA_MODE_PREMULTIPLIED)
                    ),
                &targetBitmap
                )
            );

        m_d2dContext->SetTarget(targetBitmap.Get());
        Render();
        m_d2dContext->SetTarget(m_d2dTargetBitmap.Get());

        SaveBitmapToStream(targetBitmap, m_wicFactory, m_d2dContext, *wicFormat, stream.Get());

        m_screenSavingState = ScreenSavingState::Saved;
        Render();
        Present();
    });
}

// Save render target bitmap to a stream using WIC.
void SaveAsImageFile::SaveBitmapToStream(
    _In_ ComPtr<ID2D1Bitmap1> d2dBitmap,
    _In_ ComPtr<IWICImagingFactory2> wicFactory2,
    _In_ ComPtr<ID2D1DeviceContext> d2dContext,
    _In_ REFGUID wicFormat,
    _In_ IStream* stream
    )
{
    // Create and initialize WIC Bitmap Encoder.
    ComPtr<IWICBitmapEncoder> wicBitmapEncoder;
    DX::ThrowIfFailed(
        wicFactory2->CreateEncoder(
            wicFormat,
            nullptr,    // No preferred codec vendor.
            &wicBitmapEncoder
            )
        );

    DX::ThrowIfFailed(
        wicBitmapEncoder->Initialize(
            stream,
            WICBitmapEncoderNoCache
            )
        );

    // Create and initialize WIC Frame Encoder.
    ComPtr<IWICBitmapFrameEncode> wicFrameEncode;
    DX::ThrowIfFailed(
        wicBitmapEncoder->CreateNewFrame(
            &wicFrameEncode,
            nullptr     // No encoder options.
            )
        );

    DX::ThrowIfFailed(
        wicFrameEncode->Initialize(nullptr)
        );

    // Retrieve D2D Device.
    ComPtr<ID2D1Device> d2dDevice;
    d2dContext->GetDevice(&d2dDevice);

    // Create IWICImageEncoder.
    ComPtr<IWICImageEncoder> imageEncoder;
    DX::ThrowIfFailed(
        wicFactory2->CreateImageEncoder(
            d2dDevice.Get(),
            &imageEncoder
            )
        );

    DX::ThrowIfFailed(
        imageEncoder->WriteFrame(
            d2dBitmap.Get(),
            wicFrameEncode.Get(),
            nullptr     // Use default WICImageParameter options.
            )
        );

    DX::ThrowIfFailed(
        wicFrameEncode->Commit()
        );

    DX::ThrowIfFailed(
        wicBitmapEncoder->Commit()
        );

    // Flush all memory buffers to the next-level storage object.
    DX::ThrowIfFailed(
        stream->Commit(STGC_DEFAULT)
        );
}

void SaveAsImageFile::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &SaveAsImageFile::OnActivated);
}

void SaveAsImageFile::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &SaveAsImageFile::OnPointerPressed);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &SaveAsImageFile::OnWindowSizeChanged);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &SaveAsImageFile::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &SaveAsImageFile::OnDisplayContentsInvalidated);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

void SaveAsImageFile::Load(
    _In_ Platform::String^ entryPoint
    )
{
}

void SaveAsImageFile::Run()
{
    Render();
    Present();

    m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);
}

void SaveAsImageFile::Uninitialize()
{
}

void SaveAsImageFile::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    // If the view state is snapped to one side of the screen, diasble saving.
    m_isSnapped = (ApplicationView::Value == ApplicationViewState::Snapped);

    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();
    Render();
    Present();
}

void SaveAsImageFile::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
    Present();
}

void SaveAsImageFile::OnDisplayContentsInvalidated(
    _In_ Platform::Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    Render();
    Present();
}

void SaveAsImageFile::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

void SaveAsImageFile::OnPointerPressed(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    SaveBitmapToFile();
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new SaveAsImageFile();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
