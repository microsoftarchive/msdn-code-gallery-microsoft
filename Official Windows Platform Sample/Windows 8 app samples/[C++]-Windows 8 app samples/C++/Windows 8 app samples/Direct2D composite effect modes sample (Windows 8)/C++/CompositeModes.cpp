//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "CompositeModes.h"
#include <math.h>

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::UI::ViewManagement;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;

static const float GutterSize = 24.0f;
static const float HeadingHeight = 36.0f;
static const float LabelHeight = 16.0f;
static const unsigned int BlendModesLength = 26;
static const unsigned int CompositeModesLength = 11;

// We divide the screen into a virtual grid to fit each blend/composite mode.
// Note that GridColumns = GridCompositeColumns + GridBlendColumns,
// but the same is not true for rows.
static const unsigned int GridRows = 6;
static const unsigned int GridColumns = 7;

// We reserve a 2x6 section of the grid to display composite modes.
static const unsigned int GridCompositeRows = 6;
static const unsigned int GridCompositeColumns = 2;

// We reserve a 5x6 section of the grid to display blend modes.
static const unsigned int GridBlendRows = 6;
static const unsigned int GridBlendColumns = 5;

CompositeModes::CompositeModes() :
    m_alpha(1.0f),
    m_drawEnabled(true),
    m_bitmapPixelSize(D2D1::SizeU()),
    m_imageSize(D2D1::SizeF()),
    m_contextSize(D2D1::SizeF()),
    m_cellSize(D2D1::SizeF())
{
}

CompositeModes::~CompositeModes()
{
    if (m_captionLayouts != nullptr)
    {
        delete[] m_captionLayouts;
    }
}

void CompositeModes::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_LIGHT,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            HeadingHeight,
            L"en-US", // locale
            &m_headingTextFormat
            )
        );

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_LIGHT,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            LabelHeight,
            L"en-US", // locale
            &m_textFormat
            )
        );

    DX::ThrowIfFailed(
        m_textFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER)
        );

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_LIGHT,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            20.0f,
            L"en-us", // locale
            &m_snappedViewFormat
            )
        );

    DX::ThrowIfFailed(
        m_snappedViewFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER)
        );

    DX::ThrowIfFailed(
        m_snappedViewFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER)
        );

    m_blendModeNames = ref new Platform::Array<Platform::String^>(BlendModesLength);
    m_blendModeNames[0]  = "Multiply";
    m_blendModeNames[1]  = "Screen";
    m_blendModeNames[2]  = "Darken";
    m_blendModeNames[3]  = "Lighten";
    m_blendModeNames[4]  = "Dissolve";
    m_blendModeNames[5]  = "Color Burn";
    m_blendModeNames[6]  = "Linear Burn";
    m_blendModeNames[7]  = "Darker Color";
    m_blendModeNames[8]  = "Lighter Color";
    m_blendModeNames[9]  = "Color Dodge";
    m_blendModeNames[10] = "Linear Dodge";
    m_blendModeNames[11] = "Overlay";
    m_blendModeNames[12] = "Soft Light";
    m_blendModeNames[13] = "Hard Light";
    m_blendModeNames[14] = "Vivid Light";
    m_blendModeNames[15] = "Linear Light";
    m_blendModeNames[16] = "Pin Light";
    m_blendModeNames[17] = "Hard Mix";
    m_blendModeNames[18] = "Difference";
    m_blendModeNames[19] = "Exclusion";
    m_blendModeNames[20] = "Hue";
    m_blendModeNames[21] = "Saturation";
    m_blendModeNames[22] = "Color";
    m_blendModeNames[23] = "Luminosity";
    m_blendModeNames[24] = "Subtract";
    m_blendModeNames[25] = "Division";

    m_compositeModeNames = ref new Platform::Array<Platform::String^>(CompositeModesLength);
    m_compositeModeNames[0]  = "Source Over";
    m_compositeModeNames[1]  = "Destination Over";
    m_compositeModeNames[2]  = "Source In";
    m_compositeModeNames[3]  = "Destination In";
    m_compositeModeNames[4]  = "Source Out";
    m_compositeModeNames[5]  = "Destination Out";
    m_compositeModeNames[6]  = "Source Atop";
    m_compositeModeNames[7]  = "Destination Atop";
    m_compositeModeNames[8]  = "Xor";
    m_compositeModeNames[9]  = "Plus";
    m_compositeModeNames[10] = "Source Copy";
    // There are two extra modes we are not covering in this sample: bounded source copy
    // and mask invert. Bounded source copy looks just like source copy but limits the
    // copy to the bounds of the source rather than the union of the source and destination.
    // Mask invert is meant for caret drawing and appears much like an XOR.

    // Create the apple bitmap source.
    ComPtr<IWICBitmapDecoder> decoder;
    DX::ThrowIfFailed(
        m_wicFactory->CreateDecoderFromFilename(
            L"apple.png",
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

    DX::ThrowIfFailed(
        m_wicFactory->CreateFormatConverter(&m_appleBitmap)
        );

    DX::ThrowIfFailed(
        m_appleBitmap->Initialize(
            frame.Get(),
            GUID_WICPixelFormat32bppPBGRA,
            WICBitmapDitherTypeNone,
            nullptr,
            0.0f,
            WICBitmapPaletteTypeCustom  // Premultiplied BGRA has no paletting, so this is ignored.
            )
        );

    // Create the banana bitmap source.
    DX::ThrowIfFailed(
        m_wicFactory->CreateDecoderFromFilename(
            L"banana.png",
            nullptr,
            GENERIC_READ,
            WICDecodeMetadataCacheOnDemand,
            &decoder
            )
        );

    DX::ThrowIfFailed(
        decoder->GetFrame(0, &frame)
        );

    DX::ThrowIfFailed(
        m_wicFactory->CreateFormatConverter(&m_bananaBitmap)
        );

    DX::ThrowIfFailed(
        m_bananaBitmap->Initialize(
            frame.Get(),
            GUID_WICPixelFormat32bppPBGRA,
            WICBitmapDitherTypeNone,
            nullptr,
            0.0f,
            WICBitmapPaletteTypeCustom  // Premultiplied BGRA has no paletting, so this is ignored.
            )
        );

    // Store the image size in pixels (the apple and banana images are assumed to be the same size).
    // This remains constant regardless of the app's DPI.
    UINT width;
    UINT height;
    DX::ThrowIfFailed(
        m_bananaBitmap->GetSize(&width, &height)
        );

    m_bitmapPixelSize = D2D1::SizeU(width, height);
}

void CompositeModes::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct2D composite and blend effects sample"
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::White),
            &m_whiteBrush
            )
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1BitmapSource, &m_appleEffect)
        );

    DX::ThrowIfFailed(
        m_appleEffect->SetValue(
            D2D1_BITMAPSOURCE_PROP_INTERPOLATION_MODE,
            D2D1_BITMAPSOURCE_INTERPOLATION_MODE_CUBIC
            )
        );

    // The property system expects TRUE and FALSE rather than true or false to ensure that
    // the size of the value is consistent regardless of architecture.
    DX::ThrowIfFailed(
        m_appleEffect->SetValue(D2D1_PROPERTY_CACHED, TRUE)
        );

    DX::ThrowIfFailed(
        m_appleEffect->SetValue(D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE, m_appleBitmap.Get())
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1BitmapSource, &m_bananaEffect)
        );

    DX::ThrowIfFailed(
        m_bananaEffect->SetValue(
            D2D1_BITMAPSOURCE_PROP_INTERPOLATION_MODE,
            D2D1_BITMAPSOURCE_INTERPOLATION_MODE_CUBIC
            )
        );

    DX::ThrowIfFailed(
        m_bananaEffect->SetValue(D2D1_PROPERTY_CACHED, TRUE)
        );

    DX::ThrowIfFailed(
        m_bananaEffect->SetValue(D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE, m_bananaBitmap.Get())
        );

    // Create and initialize the composite and blend effects.
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1Composite, &m_compositeEffect)
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1Blend, &m_blendEffect)
        );

    // Create the effects that will set the alpha of each bitmap.
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1ColorMatrix, &m_appleAlphaEffect)
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1ColorMatrix, &m_bananaAlphaEffect)
        );

    // Wire up the graph. Note that the color matrix effects are used as the inputs for
    // both the blend and composite effects, so all we have to do is draw one or the other.
    m_appleAlphaEffect->SetInputEffect(0, m_appleEffect.Get());
    m_bananaAlphaEffect->SetInputEffect(0, m_bananaEffect.Get());
    m_blendEffect->SetInputEffect(0, m_appleAlphaEffect.Get());
    m_blendEffect->SetInputEffect(1, m_bananaAlphaEffect.Get());
    m_compositeEffect->SetInputEffect(0, m_appleAlphaEffect.Get());
    m_compositeEffect->SetInputEffect(1, m_bananaAlphaEffect.Get());
}

void CompositeModes::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    m_contextSize = m_d2dContext->GetSize();

    // m_imageSize is in DIPs and changes depending on the app DPI.
    // This app ignores the source bitmap's DPI and assumes it is 96.
    m_imageSize = D2D1::SizeF(
        static_cast<float>(m_bitmapPixelSize.width) * 96.0f / m_dpi,
        static_cast<float>(m_bitmapPixelSize.height) * 96.0f / m_dpi
        );

    // The sample will render its content into a square area in both landscape and
    // portrait orientations. Make the square as large as the smallest dimension available.
    float minDimension = min(
        m_contextSize.width,
        m_contextSize.height - m_sampleOverlay->GetTitleHeightInDips()
        );

    // m_horizontalOffset is used for centering the content to be rendered
    m_horizontalOffset = (m_contextSize.width - minDimension) / 2.0f;

    // We want to divide the screen into cells in which each mode demonstration will sit.
    // m_cellSize stores the dimensions of a single cell. The width and height dimensions
    // are adjusted by the space taken up by the sample overlay, heading text, and gutters.
    m_cellSize = D2D1::SizeF(
        (minDimension - 4 * HeadingHeight - GutterSize) / GridColumns,
        (minDimension - GutterSize) / GridRows
        );

    float imageScaleSize = m_cellSize.width / m_imageSize.width;
    if (m_cellSize.height < m_cellSize.width)
    {
        imageScaleSize = m_cellSize.height / m_imageSize.height;
    }

    DX::ThrowIfFailed(
        m_appleEffect->SetValue(D2D1_BITMAPSOURCE_PROP_SCALE, D2D1::Point2F(imageScaleSize, imageScaleSize))
        );

    DX::ThrowIfFailed(
        m_bananaEffect->SetValue(D2D1_BITMAPSOURCE_PROP_SCALE, D2D1::Point2F(imageScaleSize, imageScaleSize))
        );

    if (m_captionLayouts != nullptr)
    {
        delete[] m_captionLayouts;
    }

    m_captionLayouts = new IDWriteTextLayout*[CompositeModesLength + BlendModesLength];
    for (int i = 0; i < CompositeModesLength; i++)
    {
        DX::ThrowIfFailed(
            m_dwriteFactory->CreateTextLayout(
                m_compositeModeNames[i]->Data(),
                m_compositeModeNames[i]->Length(),
                m_textFormat.Get(),
                m_cellSize.width,
                m_cellSize.height,
                &m_captionLayouts[i]
                )
            );
    }

    for (int i = CompositeModesLength; i < CompositeModesLength + BlendModesLength; i++)
    {
        DX::ThrowIfFailed(
            m_dwriteFactory->CreateTextLayout(
                m_blendModeNames[i - CompositeModesLength]->Data(),
                m_blendModeNames[i - CompositeModesLength]->Length(),
                m_textFormat.Get(),
                m_cellSize.width,
                m_cellSize.height,
                &m_captionLayouts[i]
                )
            );
    }

    Platform::String^ compositeModes = "Composite modes";
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextLayout(
            compositeModes->Data(),
            compositeModes->Length(),
            m_headingTextFormat.Get(),
            m_contextSize.height,
            m_contextSize.width,
            &m_compositeModeTextLayout
            )
        );

    Platform::String^ blendModes = "Blend modes";
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextLayout(
            blendModes->Data(),
            blendModes->Length(),
            m_headingTextFormat.Get(),
            m_contextSize.height,
            m_contextSize.width,
            &m_blendModeTextLayout
            )
        );
}

void CompositeModes::Render()
{
    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::Black));
    m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());

    if (m_drawEnabled)
    {
        // Draw a virtual 2x6 grid of the composite mode examples. There is room for 12 examples but only
        // 11 composite modes are shown, so we ignore the additional cells.
        for (int i = 0; i < GridCompositeColumns; i++)
        {
            for (int j = 0; j < GridCompositeRows; j++)
            {
                if (j * GridCompositeColumns + i <= CompositeModesLength - 1)
                {
                    // The grid must be offset to account for the space taken up by the sample overlay and the
                    // "Composite Modes" heading text.
                    float left = m_cellSize.width * i + 2 * HeadingHeight + m_horizontalOffset;
                    float top = m_cellSize.height * j + m_sampleOverlay->GetTitleHeightInDips() + LabelHeight;

                    // Note we're setting a value as an integer rather than using the enumeration. While
                    // we recommend using the actual enumeration values for properties, they are simply
                    // integers and automated systems can iterate through them like integers.
                    DX::ThrowIfFailed(
                        m_compositeEffect->SetValue(D2D1_COMPOSITE_PROP_MODE, j * GridCompositeColumns + i)
                        );

                    // Draw the image in the top-left corner of the cell.
                    m_d2dContext->DrawImage(
                        m_compositeEffect.Get(),
                        D2D1::Point2F(left, top),
                        D2D1::RectF(0.0f, 0.0f, m_cellSize.width, m_cellSize.height)
                        );
                }
            }
        }

        // Now we draw a the grid of the blend mode examples.
        for (int i = 0; i < GridBlendColumns; i++)
        {
            for (int j = 0; j < GridBlendRows; j++)
            {
                // Again, there is room for 30 examples but only 26 blend modes are shown, so
                // we ignore the additional cells.
                if (j * GridBlendColumns + i <= BlendModesLength - 1)
                {
                    // The grid must be offset to account for the space taken up by the sample overlay and the
                    // composite modes grid, as well as the "Blend Modes" heading text.
                    float left = m_cellSize.width * (i + GridCompositeColumns) + 4 * HeadingHeight + m_horizontalOffset;
                    float top = m_cellSize.height * j + m_sampleOverlay->GetTitleHeightInDips() + LabelHeight;
                    DX::ThrowIfFailed(
                        m_blendEffect->SetValue(D2D1_BLEND_PROP_MODE, j * GridBlendColumns + i)
                        );

                    m_d2dContext->DrawImage(
                        m_blendEffect.Get(),
                        D2D1::Point2F(left, top),
                        D2D1::RectF(0.0f, 0.0f, m_cellSize.width, m_cellSize.height)
                        );
                }
            }
        }

        // As a performance optimization, we group all of the Direct2D Effects drawing operations
        // together, and draw the text afterwards.
        for (int i = 0; i < GridCompositeColumns; i++)
        {
            for (int j = 0; j < GridCompositeRows; j++)
            {
                if (j * GridCompositeColumns + i <= CompositeModesLength - 1)
                {
                    float left = m_cellSize.width * i + 2 * HeadingHeight + m_horizontalOffset;
                    float top = m_cellSize.height * j + m_sampleOverlay->GetTitleHeightInDips();

                    m_d2dContext->DrawTextLayout(
                        D2D1::Point2F(left, top),
                        m_captionLayouts[j * GridCompositeColumns + i],
                        m_whiteBrush.Get()
                        );
                }
            }
        }

        for (int i = 0; i < GridBlendColumns; i++)
        {
            for (int j = 0; j < GridBlendRows; j++)
            {
                if (j * GridBlendColumns + i <= BlendModesLength - 1)
                {
                    float left = m_cellSize.width * (i + GridCompositeColumns) + 4 * HeadingHeight + m_horizontalOffset;
                    float top = m_cellSize.height * j + m_sampleOverlay->GetTitleHeightInDips();

                    m_d2dContext->DrawTextLayout(
                        D2D1::Point2F(left, top),
                        m_captionLayouts[j * GridBlendColumns + i + CompositeModesLength],
                        m_whiteBrush.Get()
                        );
                }
            }
        }

        // Draw the "Blend Modes" and "Composite Modes" labels vertically.
        m_d2dContext->SetTransform(
            D2D1::Matrix3x2F::Rotation(-90.0f) *
            D2D1::Matrix3x2F::Translation(0, m_cellSize.height * GridRows / 2.0f + m_sampleOverlay->GetTitleHeightInDips())
            );

        m_d2dContext->DrawTextLayout(
            D2D1::Point2F(0.0f, 0.5f * HeadingHeight + m_horizontalOffset),
            m_compositeModeTextLayout.Get(),
            m_whiteBrush.Get()
            );

        m_d2dContext->DrawTextLayout(
            D2D1::Point2F(0.0f, m_cellSize.width * (GridCompositeColumns) + 2.5f * HeadingHeight + m_horizontalOffset),
            m_blendModeTextLayout.Get(),
            m_whiteBrush.Get()
            );
    }
    else // The blend/composite mode images are not drawn in the snapped-state.
    {
        Platform::String^ message = "This sample does not support snapped view.";

        m_d2dContext->DrawText(
            message->Data(),
            message->Length(),
            m_snappedViewFormat.Get(),
            D2D1::RectF(0, 0, m_contextSize.width, m_contextSize.height),
            m_whiteBrush.Get()
            );
    }

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    m_sampleOverlay->Render();
}

void CompositeModes::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(
            this,
            &CompositeModes::OnActivated
            );

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &CompositeModes::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &CompositeModes::OnResuming);

    // This application doesn't support the Portrait/PortraitFlipped orientations.
    DisplayProperties::AutoRotationPreferences =
        DisplayOrientations::Landscape |
        DisplayOrientations::LandscapeFlipped;
}

void CompositeModes::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(
            this,
            &CompositeModes::OnWindowSizeChanged
            );

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &CompositeModes::OnPointerPressed);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &CompositeModes::OnPointerReleased);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &CompositeModes::OnPointerMoved);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &CompositeModes::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &CompositeModes::OnDisplayContentsInvalidated);

    // The gesture recognizer automatically recognizes actions such as zooms
    // (whether by pinching or using the scroll wheel) and cursor movement.
    m_gestureRecognizer = ref new GestureRecognizer();

    m_gestureRecognizer->GestureSettings =
        GestureSettings::ManipulationTranslateY;

    m_gestureRecognizer->ManipulationUpdated +=
        ref new TypedEventHandler<GestureRecognizer^, ManipulationUpdatedEventArgs^>(
            this,
            &CompositeModes::OnManipulationUpdated
            );

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

void CompositeModes::Load(
    _In_ Platform::String^ entryPoint
    )
{
    IPropertySet^ appSettings = ApplicationData::Current->LocalSettings->Values;

    if (appSettings->HasKey("alpha"))
    {
        m_alpha = safe_cast<IPropertyValue^>(appSettings->Lookup("alpha"))->GetSingle();
    }

    // After the alpha value is loaded, invoke UpdateAlpha to update the properties on the effects.
    UpdateAlpha(0.0f);
}

void CompositeModes::Run()
{
    Render();
    Present();

    m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);
}

void CompositeModes::Uninitialize()
{
}

void CompositeModes::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();

    // We don't want to draw the content when the application is in snapped state.
    m_drawEnabled = !(ApplicationView::Value == ApplicationViewState::Snapped);

    Render();
    Present();
}

void CompositeModes::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
    Present();
}

void CompositeModes::OnDisplayContentsInvalidated(
    _In_ Platform::Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    Render();
    Present();
}

void CompositeModes::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

void CompositeModes::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    IPropertySet^ appSettings = ApplicationData::Current->LocalSettings->Values;

    // Save the alpha value. This state gets restored in the Load method.
    if (appSettings->HasKey("alpha"))
    {
        appSettings->Remove("alpha");
    }

    appSettings->Insert("alpha", PropertyValue::CreateSingle(m_alpha));
}

void CompositeModes::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
}

void CompositeModes::OnPointerPressed(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessDownEvent(args->CurrentPoint);
}

void CompositeModes::OnPointerReleased(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessUpEvent(args->CurrentPoint);
}

void CompositeModes::OnPointerMoved(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessMoveEvents(args->GetIntermediatePoints());
}

void CompositeModes::OnManipulationUpdated(
    _In_ GestureRecognizer^ gestureRecognizer,
    _In_ ManipulationUpdatedEventArgs^ args
    )
{
    UpdateAlpha(args->Delta.Translation.Y);
}

void CompositeModes::UpdateAlpha(
    _In_ float verticalPositionDelta
    )
{
    // Divide position data by screen height so that one swipe from top
    // to bottom of the screen changes alpha value exactly by '1'.
    m_alpha -= verticalPositionDelta / m_contextSize.height;

    m_alpha = Clamp(m_alpha, 0.0f, 1.0f);

    D2D1_MATRIX_5X4_F alphaMatrix = {0};
    alphaMatrix.m[0][0] = 1;
    alphaMatrix.m[1][1] = 1;
    alphaMatrix.m[2][2] = 1;
    alphaMatrix.m[3][3] = m_alpha;

    DX::ThrowIfFailed(
        m_appleAlphaEffect->SetValue(D2D1_COLORMATRIX_PROP_COLOR_MATRIX, alphaMatrix)
        );

    DX::ThrowIfFailed(
        m_bananaAlphaEffect->SetValue(D2D1_COLORMATRIX_PROP_COLOR_MATRIX, alphaMatrix)
        );

    Render();
    Present();
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new CompositeModes();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
