//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "GeometryRealizationSample.h"

using namespace D2D1;
using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::UI::Popups;
using namespace Windows::Storage;
using namespace Windows::System;

static const UINT sc_minNumSquares = 1;
static const UINT sc_maxNumSquares = 1024;

static const D2D1_RECT_F textInfoBox = { 10, 100, 350, 300 };
static const float textInfoBoxInset = 10;

static const uint32 defaultNumberOfSquares = 16;
static const uint32 minNumberOfSquares = 1;
static const uint32 maxNumberOfSquares = 1024;

static const float rotationSpeed = 3.0f;

static const float loupeInset = 20.0f;

static const float strokeWidth = 1.0f;

// This determines that maximum texture size we will
// generate for our realizations.
static const uint32 maxRealizationDimension = 2000;

GeometryRealizationSample::GeometryRealizationSample()
{
    m_antialiasMode = D2D1_ANTIALIAS_MODE_ALIASED;
    m_useRealizations = false;
    m_updateRealization = true;
    m_updateDisplayText = true;
    m_drawStroke = true;
    m_paused = false;
    m_pausedTime = 0;
    m_numberOfSquares = defaultNumberOfSquares;

    LARGE_INTEGER time;
    QueryPerformanceCounter(&time);

    m_timeDelta = -time.QuadPart;
    m_windowClosed = false;
    m_windowVisible = true;
}

void GeometryRealizationSample::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    // Create a DirectWrite text format object.
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
        L"Segoe UI",
        nullptr,
        DWRITE_FONT_WEIGHT_LIGHT,
        DWRITE_FONT_STYLE_NORMAL,
        DWRITE_FONT_STRETCH_NORMAL,
        20.0f,
        L"en-US", // locale
        &m_textFormat
        )
        );
}

void GeometryRealizationSample::CreateDeviceResources()
{
    // DirectXBase creates a Direct2D device context that enables multithreaded optimizations.
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct2D geometry realization sample"
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::Black),
            &m_blackBrush
            )
        );

    ComPtr<IGeometryRealizationFactory> realizationFactory;
    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::White),
            &m_solidColorBrush
            )
        );

    DX::ThrowIfFailed(
        CreateGeometryRealizationFactory(
            m_d2dContext.Get(),
            maxRealizationDimension,
            &realizationFactory
            )
        );

    DX::ThrowIfFailed(
        realizationFactory->CreateGeometryRealization(&m_realization)
        );

    m_updateRealization = true;
    m_displayText = nullptr;
}

void GeometryRealizationSample::UpdateForWindowSizeChange()
{
    DirectXBase::UpdateForWindowSizeChange();
    m_displayText = nullptr;
    m_updateDisplayText = true;
    DiscardGeometries();
    m_sampleOverlay->UpdateForWindowSizeChange();
}

void GeometryRealizationSample::CreateGeometries()
{
    if (!m_geometry)
    {
        ComPtr<IGeometryRealizationFactory> realizationFactory;
        ComPtr<IGeometryRealization> realization;

        ComPtr<ID2D1TransformedGeometry>   geometry;
        ComPtr<ID2D1PathGeometry> pathGeometry;
        ComPtr<ID2D1GeometrySink> sink;

        float boardWidth = m_d2dContext->GetSize().width;
        float squareWidth = 0.5f * boardWidth / m_numberOfSquares;

        // Create the path geometry.
        DX::ThrowIfFailed(
            m_d2dFactory->CreatePathGeometry(&pathGeometry)
            );

        DX::ThrowIfFailed(
            pathGeometry->Open(&sink)
            );

        sink->SetFillMode(D2D1_FILL_MODE_ALTERNATE);

        sink->BeginFigure(
            D2D1::Point2F(0, 0),
            D2D1_FIGURE_BEGIN_FILLED
            );

        sink->AddLine(D2D1::Point2F(1.0f, 0));

        sink->AddBezier(
            D2D1::BezierSegment(
                D2D1::Point2F(0.75f, 0.25f),
                D2D1::Point2F(0.75f, 0.75f),
                D2D1::Point2F(1.0f, 1.0f)
                )
            );

        sink->AddLine(D2D1::Point2F(0, 1.0f));

        sink->AddBezier(
            D2D1::BezierSegment(
                D2D1::Point2F(0.25f, 0.75f),
                D2D1::Point2F(0.25f, 0.25f),
                D2D1::Point2F(0, 0)
                )
            );

        sink->EndFigure(D2D1_FIGURE_END_CLOSED);

        DX::ThrowIfFailed(
            sink->Close()
            );

        D2D1_MATRIX_3X2_F scale = D2D1::Matrix3x2F::Scale(squareWidth, squareWidth);
        D2D1_MATRIX_3X2_F translation = D2D1::Matrix3x2F::Translation(-squareWidth / 2, -squareWidth / 2);

        DX::ThrowIfFailed(
            m_d2dFactory->CreateTransformedGeometry(
                pathGeometry.Get(),
                scale * translation,
                &geometry
                )
            );
        // Transfer the reference.
        m_geometry = geometry;
    }
}

void GeometryRealizationSample::DiscardGeometries()
{
    m_geometry = nullptr;
    m_updateRealization = true;
}

void GeometryRealizationSample::RenderMainContent(float time)
{
    static DWORD dwTimeStart = 0;
    static DWORD dwTimeLast = 0;

    D2D1_SIZE_F rtSize = m_d2dContext->GetSize();

    float boardWidth = rtSize.width * 0.5f;
    float squareWidth = boardWidth / m_numberOfSquares;

    m_d2dContext->SetAntialiasMode(m_antialiasMode);

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::Black));

    D2D1_MATRIX_3X2_F currentTransform;

    m_d2dContext->GetTransform(&currentTransform);

    D2D1_MATRIX_3X2_F worldTransform =
        D2D1::Matrix3x2F::Translation(
            0.5f * (rtSize.width - squareWidth * m_numberOfSquares),
            0.5f * (rtSize.height - squareWidth * m_numberOfSquares)
            ) * currentTransform;

    for (uint32 i = 0; i < m_numberOfSquares; ++i)
    {
        for (uint32 j = 0; j < m_numberOfSquares; ++j)
        {
            float dx = i + 0.5f - 0.5f * m_numberOfSquares;
            float dy = j + 0.5f - 0.5f * m_numberOfSquares;

            float length = sqrtf(2) * m_numberOfSquares;

            // The intensity variable determines the color and speed of rotation of the
            // realization instance. We choose a function that is rotationally
            // symmetric about the center of the grid, which produces a nice effect.
            float intensity =
                0.5f * (1 + sinf((0.2f * time + 10.0f * sqrtf(static_cast<float>(dx * dx + dy * dy)) / length)));

            D2D1_MATRIX_3X2_F rotateTransform =
                D2D1::Matrix3x2F::Rotation(
                    (intensity * rotationSpeed * time * 360.0f) * ((float) M_PI / 180.0f)
                    );

            D2D1_MATRIX_3X2_F newWorldTransform =
                rotateTransform *
                D2D1::Matrix3x2F::Translation(
                    (i + 0.5f) * squareWidth,
                    (j + 0.5f) * squareWidth
                    ) * worldTransform;

            if (m_updateRealization)
            {
                DX::ThrowIfFailed(
                    m_realization->Update(
                        m_geometry.Get(),
                        static_cast<REALIZATION_CREATION_OPTIONS>(
                            REALIZATION_CREATION_OPTIONS_ANTI_ALIASED |
                            REALIZATION_CREATION_OPTIONS_ALIASED |
                            REALIZATION_CREATION_OPTIONS_FILLED |
                            REALIZATION_CREATION_OPTIONS_STROKED |
                            REALIZATION_CREATION_OPTIONS_UNREALIZED
                            ),
                        &newWorldTransform,
                        strokeWidth,
                        nullptr // pIStrokeStyle
                        )
                    );

                m_updateRealization = false;
            }

            m_d2dContext->SetTransform(newWorldTransform);
            m_solidColorBrush->SetColor(
                D2D1::ColorF(0.0f, intensity, 1.0f - intensity)
                );

            DX::ThrowIfFailed(
                m_realization->Fill(
                    m_d2dContext.Get(),
                    m_solidColorBrush.Get(),
                    m_useRealizations ?
                        REALIZATION_RENDER_MODE_DEFAULT :
                        REALIZATION_RENDER_MODE_FORCE_UNREALIZED
                    )
                );

            if (m_drawStroke)
            {
                m_solidColorBrush->SetColor(D2D1::ColorF(D2D1::ColorF::White));

                DX::ThrowIfFailed(
                    m_realization->Draw(
                        m_d2dContext.Get(),
                        m_solidColorBrush.Get(),
                        m_useRealizations ?
                            REALIZATION_RENDER_MODE_DEFAULT :
                            REALIZATION_RENDER_MODE_FORCE_UNREALIZED
                        )
                    );
            }
        }
    }
}

void GeometryRealizationSample::Render()
{
    LARGE_INTEGER time;
    LARGE_INTEGER frequency;
    QueryPerformanceCounter(&time);
    QueryPerformanceFrequency(&frequency);

    float floatTime;

    if (!m_paused)
    {
        floatTime = static_cast<float>(time.QuadPart + m_timeDelta) / static_cast<float>(frequency.QuadPart);
    }
    else
    {
        floatTime = static_cast<float>(m_pausedTime + m_timeDelta) / static_cast<float>(frequency.QuadPart);
    }

    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));
    m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());

    CreateGeometries();
    RenderMainContent(floatTime);

    m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());

    if (m_updateDisplayText)
    {
        UpdateDisplayText();
    }
    m_displayText->Render();

    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
        // is lost. It will be handled during the next call to Present.
        DX::ThrowIfFailed(hr);
    }

    m_sampleOverlay->Render();
}

void GeometryRealizationSample::UpdateDisplayText()
{
    WCHAR textBuffer[400];
    LARGE_INTEGER frequency;

    QueryPerformanceFrequency(&frequency);

    uint32 numPrimitives = m_numberOfSquares * m_numberOfSquares;

    if (m_drawStroke)
    {
        numPrimitives *= 2;
    }

    StringCchPrintf(
        textBuffer,
        400,
        L"%s\n"
        L"%s\n"
        L"# primitives: %d x %d %s = %d\n"
        L"Fps: ",
        m_antialiasMode == D2D1_ANTIALIAS_MODE_ALIASED ? L"Aliased" : L"PerPrimitive",
        m_useRealizations ? L"Realized"  : L"Unrealized",
        m_numberOfSquares,
        m_numberOfSquares,
        m_drawStroke ? L" x 2" : L"",
        numPrimitives
        );

    m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());

    m_solidColorBrush->SetColor(D2D1::ColorF(D2D1::ColorF::White));

    m_displayText = ref new FPSCounter(
        m_d2dContext.Get(),
        m_dpi,
        ref new Platform::String(textBuffer),
        m_textFormat.Get(),
        D2D1::Point2F(
            textInfoBox.left + textInfoBoxInset,
            textInfoBox.top + textInfoBoxInset
            )
        );
    m_displayText->Initialize();

    m_updateDisplayText = false;
}

void GeometryRealizationSample::ShowMenu(Point position)
{
    PopupMenu^ popupMenu = ref new PopupMenu();
    popupMenu->Commands->Append(
        ref new UICommand("Toggle Antialiasing",
        ref new UICommandInvokedHandler(this, &GeometryRealizationSample::UpdateGeometryRendering))
        );

    popupMenu->Commands->Append(
        ref new UICommand("Toggle Realization",
        ref new UICommandInvokedHandler(this, &GeometryRealizationSample::UpdateGeometryRendering))
        );

    popupMenu->Commands->Append(
        ref new UICommand("Toggle Stroking",
        ref new UICommandInvokedHandler(this, &GeometryRealizationSample::UpdateGeometryRendering))
        );

    popupMenu->Commands->Append(
        ref new UICommand("Increase Primitives",
        ref new UICommandInvokedHandler(this, &GeometryRealizationSample::UpdateGeometryRendering))
        );

    popupMenu->Commands->Append(
        ref new UICommand("Decrease Primitives",
        ref new UICommandInvokedHandler(this, &GeometryRealizationSample::UpdateGeometryRendering))
        );

    popupMenu->Commands->Append(
        ref new UICommand("Toggle Animation",
        ref new UICommandInvokedHandler(this, &GeometryRealizationSample::UpdateGeometryRendering))
        );

    popupMenu->ShowAsync(position);
}

void GeometryRealizationSample::UpdateGeometryRendering(IUICommand^ command)
{
    if (command->Label == "Toggle Antialiasing")
    {
        m_antialiasMode =
            (m_antialiasMode == D2D1_ANTIALIAS_MODE_ALIASED) ?
                D2D1_ANTIALIAS_MODE_PER_PRIMITIVE :
                D2D1_ANTIALIAS_MODE_ALIASED;
        m_updateDisplayText = true;
    }
    else if (command->Label == "Toggle Realization")
    {
        m_useRealizations = !m_useRealizations;
        m_updateDisplayText = true;
    }
    else if (command->Label == "Toggle Stroking")
    {
        m_drawStroke = !m_drawStroke;
        m_updateDisplayText = true;
    }
    else if (command->Label == "Increase Primitives")
    {
        m_numberOfSquares = min(m_numberOfSquares * 2, maxNumberOfSquares);
        m_updateDisplayText = true;
        DiscardGeometries();
    }
    else if (command->Label == "Decrease Primitives")
    {
        m_numberOfSquares = max(m_numberOfSquares / 2, minNumberOfSquares);
        m_updateDisplayText = true;
        DiscardGeometries();
    }
    else if (command->Label == "Toggle Animation")
    {
        LARGE_INTEGER time;
        QueryPerformanceCounter(&time);

        if (!m_paused)
        {
            m_pausedTime = time.QuadPart;
        }
        else
        {
            m_timeDelta += m_pausedTime - time.QuadPart;
        }

        m_paused = !m_paused;
        m_updateRealization = true;
    }
}

void GeometryRealizationSample::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &GeometryRealizationSample::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &GeometryRealizationSample::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &GeometryRealizationSample::OnResuming);
}

void GeometryRealizationSample::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &GeometryRealizationSample::OnWindowSizeChanged);

    window->VisibilityChanged +=
        ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &GeometryRealizationSample::OnVisibilityChanged);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &GeometryRealizationSample::OnPointerMoved);

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &GeometryRealizationSample::OnPointerPressed);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &GeometryRealizationSample::OnPointerReleased);

    window->Closed +=
        ref new TypedEventHandler<CoreWindow^, CoreWindowEventArgs^>(this, &GeometryRealizationSample::OnWindowClosed);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &GeometryRealizationSample::OnLogicalDpiChanged);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);

    m_gestureRecognizer = ref new GestureRecognizer();

    m_gestureRecognizer->GestureSettings = GestureSettings::Hold;

    m_gestureRecognizer->Holding +=
        ref new TypedEventHandler<GestureRecognizer^, HoldingEventArgs^>(this, &GeometryRealizationSample::OnHolding);
}

void GeometryRealizationSample::Load(
    _In_ Platform::String^ entryPoint
    )
{
    // Retrieve any stored variables from the LocalSettings collection.
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    if (settingsValues->HasKey("m_antialiasMode"))
    {
        m_antialiasMode = static_cast<D2D1_ANTIALIAS_MODE>(
            safe_cast<IPropertyValue^>(settingsValues->Lookup("m_antialiasMode"))->GetInt32()
            );
    }

    if (settingsValues->HasKey("m_useRealizations"))
    {
        m_useRealizations = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_useRealizations"))->GetBoolean();
    }

    if (settingsValues->HasKey("m_drawStroke"))
    {
        m_drawStroke = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_drawStroke"))->GetBoolean();
    }

    if (settingsValues->HasKey("m_numberOfSquares"))
    {
        m_numberOfSquares = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_numberOfSquares"))->GetUInt32();
    }

    if (settingsValues->HasKey("m_paused"))
    {
        m_paused = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_paused"))->GetBoolean();
    }

    if (settingsValues->HasKey("m_pausedTime"))
    {
        m_pausedTime = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_pausedTime"))->GetInt64();
    }

    if (settingsValues->HasKey("m_timeDelta"))
    {
        m_timeDelta = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_timeDelta"))->GetInt64();
    }
}

void GeometryRealizationSample::Run()
{
    while (!m_windowClosed)
    {
        if (m_windowVisible)
        {
            m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessAllIfPresent);
            Render();
            Present(); // This call is synchronized to the display frame rate.
        }
        else
        {
            m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessOneAndAllPending);
        }
    }
}

void GeometryRealizationSample::Uninitialize()
{
}

void GeometryRealizationSample::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    Render();
    Present();
}

void GeometryRealizationSample::OnVisibilityChanged(
    _In_ CoreWindow^ sender,
    _In_ VisibilityChangedEventArgs^ args
    )
{
    m_windowVisible = args->Visible;
}

void GeometryRealizationSample::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    DiscardGeometries();
    Render();
    Present();
}

void GeometryRealizationSample::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

void GeometryRealizationSample::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    // Store variables in the LocalSettings collection.
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    // Check to ensure each key is not already in the collection. If it is present, remove
    // it before inserting the new value. These values will be retrieved in the Load method.

    if (settingsValues->HasKey("m_antialiasMode"))
    {
        settingsValues->Remove("m_antialiasMode");
    }
    settingsValues->Insert("m_antialiasMode", PropertyValue::CreateInt32(m_antialiasMode));

    if (settingsValues->HasKey("m_useRealizations"))
    {
        settingsValues->Remove("m_useRealizations");
    }
    settingsValues->Insert("m_useRealizations", PropertyValue::CreateBoolean(m_useRealizations));

    if (settingsValues->HasKey("m_drawStroke"))
    {
        settingsValues->Remove("m_drawStroke");
    }
    settingsValues->Insert("m_drawStroke", PropertyValue::CreateBoolean(m_drawStroke));

    if (settingsValues->HasKey("m_numberOfSquares"))
    {
        settingsValues->Remove("m_numberOfSquares");
    }
    settingsValues->Insert("m_numberOfSquares", PropertyValue::CreateUInt32(m_numberOfSquares));

    if (settingsValues->HasKey("m_paused"))
    {
        settingsValues->Remove("m_paused");
    }
    settingsValues->Insert("m_paused", PropertyValue::CreateBoolean(m_paused));

    if (settingsValues->HasKey("m_pausedTime"))
    {
        settingsValues->Remove("m_pausedTime");
    }
    settingsValues->Insert("m_pausedTime", PropertyValue::CreateInt64(m_pausedTime));

    if (settingsValues->HasKey("m_timeDelta"))
    {
        settingsValues->Remove("m_timeDelta");
    }
    settingsValues->Insert("m_timeDelta", PropertyValue::CreateInt64(m_timeDelta));
}

void GeometryRealizationSample::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
}

void GeometryRealizationSample::OnWindowClosed(
    _In_ CoreWindow^ sender,
    _In_ CoreWindowEventArgs^ args
    )
{
    m_windowClosed = true;
}

void GeometryRealizationSample::OnPointerMoved(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessMoveEvents(args->GetIntermediatePoints());
}

void GeometryRealizationSample::OnPointerPressed(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessDownEvent(args->CurrentPoint);
}

void GeometryRealizationSample::OnPointerReleased(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessUpEvent(args->CurrentPoint);

    if (args->CurrentPoint->Properties->PointerUpdateKind == PointerUpdateKind::RightButtonReleased)
    {
        ShowMenu(args->CurrentPoint->Position);
    }
}

void GeometryRealizationSample::OnHolding(
    _In_ GestureRecognizer^ recognizer,
    _In_ HoldingEventArgs^ args
    )
{
    if (args->HoldingState == HoldingState::Started)
    {
        ShowMenu(args->Position);
    }
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new GeometryRealizationSample();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
