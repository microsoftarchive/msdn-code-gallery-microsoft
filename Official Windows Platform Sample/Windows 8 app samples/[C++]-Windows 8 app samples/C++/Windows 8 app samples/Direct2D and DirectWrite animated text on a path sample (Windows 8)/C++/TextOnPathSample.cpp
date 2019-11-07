//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "TextOnPathSample.h"
#include "BasicTimer.h"

using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;

static const float sc_textLayoutWidth = 4096.0f;    // The width of the text layout used to render the string.
static const float sc_textLayoutHeight = 256.0f;    // The height of the text layout used to render the string.
static const int sc_maxStringLength = 100;          // The maximum number of characters that the user can enter.

TextOnPath::TextOnPath() :
    m_windowClosed(false),
    m_windowVisible(true),
    m_isSnapped(false)
{
}

void TextOnPath::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    ComPtr<IDWriteRenderingParams> renderingParams;
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateRenderingParams(&renderingParams)
        );

    // Custom text rendering param object is created that uses all default values except
    // for the rendering mode which is now set to outline. The outline mode is much faster
    // in this case as every time text is relaid out on the path, it is rasterized as
    // geometry. This saves the extra step of trying to find the text bitmaps in the font
    // cache and then repopulating the cache with the new ones. Since the text is rotating with
    // the animating path, new glyph bitmaps would be generated every frame.

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateCustomRenderingParams(
            renderingParams->GetGamma(),
            renderingParams->GetEnhancedContrast(),
            renderingParams->GetClearTypeLevel(),
            renderingParams->GetPixelGeometry(),
            DWRITE_RENDERING_MODE_OUTLINE,
            &m_renderingParams
            )
        );

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Gabriola",
            nullptr,
            DWRITE_FONT_WEIGHT_REGULAR,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            48.0f,
            L"en-US",
            &m_textFormat
            )
        );

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_REGULAR,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            20.0f,
            L"en-US",
            &m_snappedViewFormat
            )
        );

    DX::ThrowIfFailed(
        m_snappedViewFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER)
        );

    DX::ThrowIfFailed(
        m_snappedViewFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER)
        );
}

void TextOnPath::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct2D animated text on a path sample"
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::Black),
            &m_blackBrush
            )
        );

    m_displayString.append(L"Hüllo, world! مرحبا ، العالم! Здравствуй, мир!");
}

void TextOnPath::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // Pixels per DIP = (pixels per inch) * (1 inch per 96.0 DIPs).
    FLOAT pixelsPerDip = m_dpi / 96.0f;

    // Create a new text renderer using the current DPI.
    PathTextRenderer::CreatePathTextRenderer(
        pixelsPerDip,
        &m_textRenderer
        );
}

//
// Creates the curve rendererd in the center of the screen.
// Depending on the current value of m_ticks, the curve's
// control points will be in different places, resulting in
// what appears to be a sinusoidal movement over time.
//
void TextOnPath::CreateGeometry()
{
    if (!m_geometry)
    {
        ComPtr<ID2D1GeometrySink> sink;

        // Calculate t based on how many ticks have passed.
        // This function generates a pleasing sinusoid.
        FLOAT t = sin(m_currentTime);

        DX::ThrowIfFailed(
            m_d2dFactory->CreatePathGeometry(&m_geometry)
            );

        DX::ThrowIfFailed(
            m_geometry->Open(&sink)
            );

        //
        // Create a Bezier curve with two endpoints and two control points.
        // The position of the control points depends on t, resulting in
        // rudimentary animation as t changes.
        //
        static const FLOAT xIncrement = 130;
        static const FLOAT maxAmplitude = 600;

        sink->SetFillMode(D2D1_FILL_MODE_WINDING);

        sink->BeginFigure(
            D2D1::Point2F(-2 * xIncrement, 0),
            D2D1_FIGURE_BEGIN_FILLED
            );

        sink->AddBezier(
            D2D1::BezierSegment(
                D2D1::Point2F(-0.5f * xIncrement, -(t * maxAmplitude)),
                D2D1::Point2F(0.5f * xIncrement, (t * maxAmplitude)),
                D2D1::Point2F(2 * xIncrement, 0))
                );

        sink->EndFigure(D2D1_FIGURE_END_OPEN);

        DX::ThrowIfFailed(
            sink->Close()
            );
    }
}

//
// Creates the text layout object using the current string
// of text to display. If the string to display is empty,
// this method leaves the text layout object null.
//
void TextOnPath::CreateTextLayout()
{
    if (!m_textLayout && m_displayString.size() > 0)
    {
        DX::ThrowIfFailed(
            m_dwriteFactory->CreateTextLayout(
                &m_displayString.front(),
                static_cast<UINT32>(m_displayString.size()),
                m_textFormat.Get(),
                sc_textLayoutWidth,
                sc_textLayoutHeight,
                &m_textLayout
                )
            );
    }
}

void TextOnPath::Update(float timeTotal)
{
    m_currentTime = timeTotal;
}

void TextOnPath::Render()
{
    CreateGeometry();
    CreateTextLayout();

    D2D1_SIZE_F size = m_d2dContext->GetSize();
    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));
    m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());

    if (m_isSnapped)
    {
        // Sample is snapped; display a simple message instead of the sample.
        String^ message = "This sample does not support snapped view.";

        // The text in the message does not need to be converted to geometry outlines.
        m_d2dContext->SetTextRenderingParams(nullptr);

        m_d2dContext->DrawText(
            message->Data(),
            message->Length(),
            m_snappedViewFormat.Get(),
            D2D1::RectF(0, 0, size.width, size.height),
            m_blackBrush.Get()
            );
    }
    else
    {
        // Translate the scene so it's centered.
        m_d2dContext->SetTransform(
            D2D1::Matrix3x2F::Translation(size.width / 2, size.height / 2)
            );

        // Draw the geometry.
        m_d2dContext->DrawGeometry(
            m_geometry.Get(),
            m_blackBrush.Get()
            );

        //
        // Draw the text, if any.
        //
        if (m_textLayout)
        {
            PathTextDrawingContext context;
            context.brush = m_blackBrush.Get();
            context.geometry = m_geometry.Get();
            context.d2DContext = m_d2dContext.Get();

            m_d2dContext->SetTextRenderingParams(m_renderingParams.Get());

            DX::ThrowIfFailed(
                m_textLayout->Draw(
                    &context,
                    m_textRenderer.Get(),
                    0,
                    0
                    )
                );
        }
    }

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();

    DiscardGeometry();
    DiscardTextLayout();

    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    m_sampleOverlay->Render();
}

void TextOnPath::DiscardGeometry()
{
    m_geometry = nullptr;
}

void TextOnPath::DiscardTextLayout()
{
    m_textLayout = nullptr;
}

void TextOnPath::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &TextOnPath::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &TextOnPath::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &TextOnPath::OnResuming);
}

void TextOnPath::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &TextOnPath::OnWindowSizeChanged);

    window->VisibilityChanged +=
        ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &TextOnPath::OnVisibilityChanged);

    window->Closed +=
        ref new TypedEventHandler<CoreWindow^, CoreWindowEventArgs^>(this, &TextOnPath::OnWindowClosed);

    window->CharacterReceived +=
        ref new TypedEventHandler<CoreWindow^, CharacterReceivedEventArgs^>(this, &TextOnPath::OnKeyPressed);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &TextOnPath::OnLogicalDpiChanged);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

void TextOnPath::Load(
    _In_ Platform::String^ entryPoint
    )
{
    // Retrieve any stored variables from the LocalSettings collection.
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    if (settingsValues->HasKey("m_displayString"))
    {
        Platform::String^ string = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_displayString"))->GetString();
        m_displayString = string->Data();
    }
}

void TextOnPath::Run()
{
    BasicTimer^ timer = ref new BasicTimer();

    while (!m_windowClosed)
    {
        if (m_windowVisible)
        {
            timer->Update();
            m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessAllIfPresent);
            Update(timer->Total);
            Render();
            Present();
        }
        else
        {
            m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessOneAndAllPending);
        }
    }
}

void TextOnPath::Uninitialize()
{
}

void TextOnPath::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    m_isSnapped = (ApplicationView::Value == ApplicationViewState::Snapped);

    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();
}

void TextOnPath::OnVisibilityChanged(
    _In_ CoreWindow^ sender,
    _In_ VisibilityChangedEventArgs^ args
    )
{
    m_windowVisible = args->Visible;
}

void TextOnPath::OnWindowClosed(
    _In_ CoreWindow^ sender,
    _In_ CoreWindowEventArgs^ args
    )
{
    m_windowClosed = true;
}

void TextOnPath::OnKeyPressed(
    _In_ CoreWindow^ sender,
    _In_ CharacterReceivedEventArgs^ args
    )
{
    if (args->KeyCode == '\r')
    {
        // "Enter" key pressed; ignore it.
        return;
    }
    else if (args->KeyCode == '\b')
    {
        // Backspace pressed; remove a character.
        if (m_displayString.size() > 0)
        {
            m_displayString.pop_back();
        }
    }
    else
    {
        // Regular key pressed; append it to the string.
        if (m_displayString.size() < sc_maxStringLength)
        {
            m_displayString.push_back(args->KeyCode);
        }
    }

    // Recreate the text layout with the new string.
    DiscardTextLayout();
}

void TextOnPath::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
}

void TextOnPath::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

void TextOnPath::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    // Store variables in the LocalSettings collection.
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    if (settingsValues->HasKey("m_displayString"))
    {
        settingsValues->Remove("m_displayString");
    }
    Platform::String^ string = ref new Platform::String(const_cast<wchar_t*>(m_displayString.c_str()));
    settingsValues->Insert("m_displayString", string);
}

void TextOnPath::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new TextOnPath();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
