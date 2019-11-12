//  Microsoft Windows
//  Copyright (c) Microsoft Corporation. All rights reserved.

#include "pch.h"
#include "D2DInterpolationModes.h"

using namespace D2D1;
using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;
using namespace Windows::System;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::UI::Popups;
using namespace Windows::UI::ViewManagement;

static const float maxZoom = 20.0f;
static const float minZoom = 0.05f;
static const float menuTop = 70;
static const float menuMargin = 10;
static const float menuBottom = 120;
static const float renderTimeTextOffset = 300;

D2DInterpolationModes::D2DInterpolationModes() :
    m_totalZoom(0.0f),
    m_imageRotate(0.0f),
    m_viewPosition(D2D1::Point2F(0, 0)),
    m_renderTimer(ref new BasicTimer()),
    m_isWindowClosed(false),
    m_isWindowVisible(true),
    m_isSnapped(false)
{
}

void D2DInterpolationModes::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct2D effects interpolation modes sample"
        );

    // Create Text Formats.
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_THIN,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            30,
            L"en-US",
            &m_modeTextFormat
            )
        );

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_THIN,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            20,
            L"en-US",
            &m_renderTimeFormat
            )
        );

    // Create white brush.
    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::White),
            &m_whiteBrush
            )
        );

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

    ComPtr<IWICFormatConverter> converter;
    DX::ThrowIfFailed(
        m_wicFactory->CreateFormatConverter(&converter)
        );

    DX::ThrowIfFailed(
        converter->Initialize(
            frame.Get(),
            GUID_WICPixelFormat32bppPBGRA,
            WICBitmapDitherTypeNone,
            nullptr,
            0.0f,
            WICBitmapPaletteTypeCustom
            )
        );

    UINT width;
    UINT height;
    frame->GetSize(&width, &height);
    m_imageSize = D2D1::SizeU(width, height);

    // Create BitmapSource effect using the WIC Format Converter as an input.
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(
            CLSID_D2D1BitmapSource,
            &m_bitmapSourceEffect
            )
        );

    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(
            D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE,
            converter.Get()
            )
        );

    // Because the image produced by m_bitmapSourceEffect will never change,
    // enable caching for a performance gain.
    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(
            D2D1_PROPERTY_CACHED,
            TRUE
            )
        );

    // Create two 2DAffineTransform effects and set the inputs to the BitmapSource effect.
    // Both effects will default to the Nearest Neighbor interpolation mode.

    // Create left effect.
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(
            CLSID_D2D12DAffineTransform,
            &m_2DAffineTransformEffectLeft
            )
        );

    m_2DAffineTransformEffectLeft->SetInputEffect(0, m_bitmapSourceEffect.Get(), false);

    DX::ThrowIfFailed(
        m_2DAffineTransformEffectLeft->SetValue(
            D2D1_2DAFFINETRANSFORM_PROP_INTERPOLATION_MODE,
            D2D1_INTERPOLATION_MODE_LINEAR
            )
        );

    m_modeTextLeft = "Interpolation mode: linear";

    // Create right effect.
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(
            CLSID_D2D12DAffineTransform,
            &m_2DAffineTransformEffectRight
            )
        );

    m_2DAffineTransformEffectRight->SetInputEffect(0, m_bitmapSourceEffect.Get(), false);

    DX::ThrowIfFailed(
        m_2DAffineTransformEffectRight->SetValue(
            D2D1_2DAFFINETRANSFORM_PROP_INTERPOLATION_MODE,
            D2D1_INTERPOLATION_MODE_LINEAR
            )
        );

    m_modeTextRight = "Interpolation mode: linear";

    m_selectedEffect = nullptr;
}

void D2DInterpolationModes::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // Convert m_imageSize to DIPs in order to be DPI-independent.
    m_imageSizeDips =
        D2D1::SizeF(
            m_imageSize.width * 96.0f / m_dpi,
            m_imageSize.height * 96.0f / m_dpi
            );

    // Screen size in DIPs.
    D2D1_SIZE_F size = m_d2dContext->GetSize();

    m_totalZoom = min(
        size.width / m_imageSizeDips.width,
        size.height / m_imageSizeDips.height
        );

    UpdateEffectMatrices();
}

void D2DInterpolationModes::Render()
{
    D2D1_SIZE_F size = m_d2dContext->GetSize();

    // Start render duration timer.
    m_renderTimer->Update();

    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::DarkMagenta));

    // Caculate width, horizontal midpoint and height of screen, which will be used create
    // the rectangles that bound the left and right 2DAffineTransform effects.
    const float screenWidthMidpoint = size.width / 2.0f;
    const float screenWidth = size.width;
    const float screenHeight = size.height;

    // Draw left interpolation mode effect.
    m_d2dContext->DrawImage(
        m_2DAffineTransformEffectLeft.Get(),
        D2D1::Point2F(0, 0),
        D2D1::RectF(
            0,
            0,
            screenWidthMidpoint,
            screenHeight
            )
        );

    // Draw right interpolation mode effect.
    m_d2dContext->DrawImage(
        m_2DAffineTransformEffectRight.Get(),
        D2D1::Point2F(size.width / 2.0f, 0),
        D2D1::RectF(
            screenWidthMidpoint,
            0,
            screenWidth,
            screenHeight
            )
        );

    // Only draw text when not in snapped mode.
    if (!m_isSnapped)
    {
        // Draw left interpolation mode text.
        m_d2dContext->DrawText(
            m_modeTextLeft->Data(),
            m_modeTextLeft->Length(),
            m_modeTextFormat.Get(),
            D2D1::RectF(
                menuMargin,
                menuTop,
                screenWidthMidpoint,
                menuBottom
                ),
            m_whiteBrush.Get()
            );

        // Draw right interpolation mode text.
        m_d2dContext->DrawText(
            m_modeTextRight->Data(),
            m_modeTextRight->Length(),
            m_modeTextFormat.Get(),
            D2D1::RectF(
                menuMargin + screenWidthMidpoint,
                menuTop,
                screenWidth,
                menuBottom
                ),
            m_whiteBrush.Get()
            );

        // Don't draw render time on startup.
        if (m_renderDuration > 0.0f)
        {
            // Draw renderTime text, truncated to 2 decimal places.
            Platform::String^ renderTimeString =
                "Time to render frame: " + (floor(m_renderDuration * 100000.F) / 100.0f).ToString() + "ms";

            m_d2dContext->DrawText(
                renderTimeString->Data(),
                renderTimeString->Length(),
                m_renderTimeFormat.Get(),
                D2D1::RectF(
                    screenWidth - renderTimeTextOffset,
                    0,
                    screenWidth,
                    menuBottom
                    ),
                m_whiteBrush.Get()
                );
        }
    }

    // Create vertical line to divide the two interpolation modes.
    m_d2dContext->DrawLine(
        D2D1::Point2F(screenWidthMidpoint, 0.0f),
        D2D1::Point2F(screenWidthMidpoint, screenHeight),
        m_whiteBrush.Get(),
        0.5F
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    m_sampleOverlay->Render();

    // Measure how long it took to perform image rendering operations.
    m_renderTimer->Update();
    m_renderDuration = m_renderTimer->Delta;

    Present();
}

void D2DInterpolationModes::AddMenuItem(PopupMenu^ popupMenu, Platform::String^ caption)
{
    popupMenu->Commands->Append(
        ref new UICommand(
            caption,
            ref new UICommandInvokedHandler(this, &D2DInterpolationModes::MenuItemSelected)
            )
        );
}

void D2DInterpolationModes::HandlePointerPressed(PointerPoint^ point)
{
    D2D1_SIZE_F size = m_d2dContext->GetSize();

    Point position = point->Position;
    if (position.Y > menuTop && position.Y < menuBottom && !m_isSnapped)
    {
        if (position.X > menuMargin &&
            position.X < size.width / 2)
        {
            m_selectedEffect = m_2DAffineTransformEffectLeft;
        }

        if (position.X > size.width / 2 &&
            position.X < size.width)
        {
            m_selectedEffect = m_2DAffineTransformEffectRight;
        }

        PopupMenu^ popupMenu = ref new PopupMenu();

        // Separate function to abstract away adding items to popup menu.
        AddMenuItem(popupMenu, "Nearest neighbor interpolation");
        AddMenuItem(popupMenu, "Linear interpolation");
        AddMenuItem(popupMenu, "Cubic interpolation");
        AddMenuItem(popupMenu, "Multisample linear interpolation");
        AddMenuItem(popupMenu, "Anisotropic interpolation");
        AddMenuItem(popupMenu, "High-quality cubic interpolation");

        popupMenu->ShowAsync(position);
    }
    else
    {
        m_gestureRecognizer->ProcessDownEvent(point);
    }
}

void D2DInterpolationModes::HandleManipulationUpdated(
    Point position,
    Point positionDelta,
    float zoomDelta
    )
{
    D2D1_SIZE_F size = m_d2dContext->GetSize();

    // In this method, the transformation variables are updated to reflect how
    // the user is manipulating the image. The GestureRecognizer does the
    // majority of the math; only the internal vars need to be updated here.

    // The zoom operation should be centered around the position of the pointer/gesture,
    // which is stored as the "position" argument, and is in the screen coordinate space.

    // When transforming the pointer position from screen space to image space, take
    // any rotation into account.
    Matrix3x2F inverseRotation =
        Matrix3x2F::Rotation(
            m_imageRotate * -1,
            D2D1::Point2F(
                size.width / 2,
                size.height / 2
                )
            );

    D2D1_POINT_2F rotatedPosition =
        inverseRotation.TransformPoint(
            D2D1::Point2F(
                position.X,
                position.Y
                )
            );

    // Apply the inverse render transforms to the pointer position to obtain the
    // corresponding position in the image.
    D2D1_POINT_2F pointerPosition_ImageSpace = D2D1::Point2F(
        (m_viewPosition.x - rotatedPosition.x) / m_totalZoom,
        (m_viewPosition.y - rotatedPosition.y) / m_totalZoom
        );

    // Apply the zoom operation. zoomDelta is a coefficient for the change in zoom, thus take
    // the product of it and the current zoom value, not the sum.
    m_totalZoom *= zoomDelta;
    m_totalZoom = Clamp(m_totalZoom, minZoom, maxZoom);

    // Calculate the view position based on the new m_totalZoom value.
    m_viewPosition.x = pointerPosition_ImageSpace.x * m_totalZoom + rotatedPosition.x;
    m_viewPosition.y = pointerPosition_ImageSpace.y * m_totalZoom + rotatedPosition.y;

    float offsetX = position.X - size.width / 2.0f;
    float offsetY = position.Y - size.height / 2.0f;

    float circumference = static_cast<float>(sqrtf(offsetX * offsetX + offsetY * offsetY) * 2 * M_PI);

    // Detect edge case when pointer is directly over center of image.
    if (offsetX != 0 || offsetY != 0)
    {
        float distance =
            positionDelta.Y * (offsetX / sqrtf(offsetX * offsetX + offsetY * offsetY)) -
            positionDelta.X * (offsetY / sqrtf(offsetX * offsetX + offsetY * offsetY));

        m_imageRotate += distance / circumference * 360;
    }

    UpdateEffectMatrices();
}

void D2DInterpolationModes::UpdateEffectMatrices()
{
    D2D1_SIZE_F size = m_d2dContext->GetSize();

    // Snaps image to center of screen once zoomed out.
    if (m_imageSizeDips.width * m_totalZoom < size.width ||
        m_imageSizeDips.height * m_totalZoom < size.height)
    {
        m_viewPosition.x = size.width / 2 - m_imageSizeDips.width * m_totalZoom / 2;
        m_viewPosition.y = size.height / 2 - m_imageSizeDips.height * m_totalZoom / 2;
    }

    Matrix3x2F scale = Matrix3x2F::Scale(m_totalZoom, m_totalZoom);

    Matrix3x2F translate = Matrix3x2F::Translation(
        m_viewPosition.x,
        m_viewPosition.y
        );

    Matrix3x2F rotate = Matrix3x2F::Rotation(
        m_imageRotate,
        D2D1::Point2F(
            size.width / 2,
            size.height / 2
            )
        );

    Matrix3x2F transformMatrix = scale * translate * rotate;

    DX::ThrowIfFailed(
        m_2DAffineTransformEffectLeft->SetValue(
            D2D1_2DAFFINETRANSFORM_PROP_TRANSFORM_MATRIX,
            transformMatrix
            )
        );

    DX::ThrowIfFailed(
        m_2DAffineTransformEffectRight->SetValue(
            D2D1_2DAFFINETRANSFORM_PROP_TRANSFORM_MATRIX,
            transformMatrix
            )
        );

    Render();
}

void D2DInterpolationModes::MenuItemSelected(IUICommand^ command)
{
    Platform::String^ menuText;
    D2D1_INTERPOLATION_MODE interpolationMode;

    if (command->Label == "Nearest neighbor interpolation")
    {
        menuText = "Interpolation mode: nearest neighbor";
        interpolationMode = D2D1_INTERPOLATION_MODE_NEAREST_NEIGHBOR;
    }
    else if (command->Label == "Linear interpolation")
    {
        menuText = "Interpolation mode: linear";
        interpolationMode = D2D1_INTERPOLATION_MODE_LINEAR;
    }
    else if (command->Label == "Cubic interpolation")
    {
        menuText = "Interpolation mode: cubic interpolation";
        interpolationMode = D2D1_INTERPOLATION_MODE_CUBIC;
    }
    else if (command->Label == "Multisample linear interpolation")
    {
        menuText = "Interpolation mode: multisample linear";
        interpolationMode = D2D1_INTERPOLATION_MODE_MULTI_SAMPLE_LINEAR;
    }
    else if (command->Label == "Anisotropic interpolation")
    {
        menuText = "Interpolation mode: anisotropic";
        interpolationMode = D2D1_INTERPOLATION_MODE_ANISOTROPIC;
    }
    else if (command->Label == "High-quality cubic interpolation")
    {
        menuText = "Interpolation mode: high-quality cubic";
        interpolationMode = D2D1_INTERPOLATION_MODE_HIGH_QUALITY_CUBIC;
    }
    else
    {
        // If no match, throw exception.
        throw ref new Platform::FailureException();
    }

    // Update corresponding effect with new interpolation mode.
    DX::ThrowIfFailed(
        m_selectedEffect->SetValue(
            D2D1_2DAFFINETRANSFORM_PROP_INTERPOLATION_MODE,
            interpolationMode
            )
        );

    // Update menu text based on selected interpolation mode.
    if (m_selectedEffect == m_2DAffineTransformEffectLeft)
    {
        m_modeTextLeft = menuText;
    }
    else
    {
        m_modeTextRight = menuText;
    }

    Render();
}

void D2DInterpolationModes::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    m_isSnapped = (ApplicationView::Value == ApplicationViewState::Snapped);
    m_sampleOverlay->UpdateForWindowSizeChange();
    UpdateForWindowSizeChange();
}

void D2DInterpolationModes::OnVisibilityChanged(
    _In_ CoreWindow^ sender,
    _In_ VisibilityChangedEventArgs^ args
    )
{
    m_isWindowVisible = args->Visible;
}

void D2DInterpolationModes::OnWindowClosed(
    _In_ CoreWindow^ window,
    _In_ CoreWindowEventArgs^ args
    )
{
    m_isWindowClosed = true;
}

void D2DInterpolationModes::OnLogicalDpiChanged(
    _In_ Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
}

void D2DInterpolationModes::OnDisplayContentsInvalidated(
    _In_ Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    Render();
}

// Input events.
void D2DInterpolationModes::OnPointerPressed(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    HandlePointerPressed(args->CurrentPoint);
}

void D2DInterpolationModes::OnPointerReleased(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessUpEvent(args->CurrentPoint);
}

void D2DInterpolationModes::OnPointerMoved(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessMoveEvents(args->GetIntermediatePoints());
}

void D2DInterpolationModes::OnPointerWheelChanged(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessMouseWheelEvent(args->CurrentPoint, FALSE, TRUE);
}

void D2DInterpolationModes::OnManipulationUpdated(
    _In_ GestureRecognizer^ gestureRecognizer,
    _In_ ManipulationUpdatedEventArgs^ args
    )
{
    HandleManipulationUpdated(
        args->Position,
        args->Delta.Translation,
        args->Delta.Scale
        );
}

void D2DInterpolationModes::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

void D2DInterpolationModes::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    // Store user-manipulated properties in the LocalSettings collection.
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    // Check to ensure each key is not already in the collection. If it is present, remove
    // it, before storing in the new value. These values will be retried in the OnResuming method.

    if (settingsValues->HasKey("m_totalZoom"))
    {
        settingsValues->Remove("m_totalZoom");
    }
    settingsValues->Insert("m_totalZoom", PropertyValue::CreateSingle(m_totalZoom));

    if (settingsValues->HasKey("m_viewPositionX"))
    {
        settingsValues->Remove("m_viewPositionX");
    }
    settingsValues->Insert("m_viewPositionX", PropertyValue::CreateSingle(m_viewPosition.x));

    if (settingsValues->HasKey("m_viewPositionY"))
    {
        settingsValues->Remove("m_viewPositionY");
    }
    settingsValues->Insert("m_viewPositionY", PropertyValue::CreateSingle(m_viewPosition.y));

    if (settingsValues->HasKey("m_imageRotate"))
    {
        settingsValues->Remove("m_imageRotate");
    }
    settingsValues->Insert("m_imageRotate", PropertyValue::CreateSingle(m_imageRotate));

    int leftInterpolationMode;
    m_2DAffineTransformEffectLeft->GetValue(D2D1_2DAFFINETRANSFORM_PROP_INTERPOLATION_MODE, &leftInterpolationMode);
    if (settingsValues->HasKey("leftInterpolationMode"))
    {
        settingsValues->Remove("leftInterpolationMode");
    }
    settingsValues->Insert("leftInterpolationMode", PropertyValue::CreateInt32(leftInterpolationMode));

    int rightInterpolationMode;
    m_2DAffineTransformEffectRight->GetValue(D2D1_2DAFFINETRANSFORM_PROP_INTERPOLATION_MODE, &rightInterpolationMode);
    if (settingsValues->HasKey("rightInterpolationMode"))
    {
        settingsValues->Remove("rightInterpolationMode");
    }
    settingsValues->Insert("rightInterpolationMode", PropertyValue::CreateInt32(rightInterpolationMode));

    if (settingsValues->HasKey("m_modeTextLeft"))
    {
        settingsValues->Remove("m_modeTextLeft");
    }
    settingsValues->Insert("m_modeTextLeft", PropertyValue::CreateString(m_modeTextLeft));

    if (settingsValues->HasKey("m_modeTextRight"))
    {
        settingsValues->Remove("m_modeTextRight");
    }
    settingsValues->Insert("m_modeTextRight", PropertyValue::CreateString(m_modeTextRight));
}

void D2DInterpolationModes::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
}

// Following methods implement the Windows::ApplicationModel::Core::IFrameworkView Interface.
void D2DInterpolationModes::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &D2DInterpolationModes::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &D2DInterpolationModes::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &D2DInterpolationModes::OnResuming);
}

void D2DInterpolationModes::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &D2DInterpolationModes::OnWindowSizeChanged);

    window->VisibilityChanged +=
        ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &D2DInterpolationModes::OnVisibilityChanged);

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &D2DInterpolationModes::OnPointerPressed);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &D2DInterpolationModes::OnPointerReleased);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &D2DInterpolationModes::OnPointerMoved);

    window->PointerWheelChanged +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &D2DInterpolationModes::OnPointerWheelChanged);

    window->Closed +=
        ref new TypedEventHandler<CoreWindow^, CoreWindowEventArgs^>(this, &D2DInterpolationModes::OnWindowClosed);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &D2DInterpolationModes::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &D2DInterpolationModes::OnDisplayContentsInvalidated);

    // The gesture recognizer automatically recognizes actions such as zooms
    // (whether by pinching or using the scroll wheel) and cursor movement.
    m_gestureRecognizer = ref new GestureRecognizer();

    m_gestureRecognizer->AutoProcessInertia = false;

    m_gestureRecognizer->GestureSettings =
        GestureSettings::DoubleTap                     |
        GestureSettings::ManipulationTranslateX        |
        GestureSettings::ManipulationTranslateY        |
        GestureSettings::ManipulationScale             |
        GestureSettings::ManipulationTranslateInertia  |
        GestureSettings::ManipulationScaleInertia;

    m_gestureRecognizer->ManipulationUpdated +=
        ref new TypedEventHandler<GestureRecognizer^, ManipulationUpdatedEventArgs^>(this, &D2DInterpolationModes::OnManipulationUpdated);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

void D2DInterpolationModes::Uninitialize()
{
}

void D2DInterpolationModes::Load(
    _In_ Platform::String^ entryPoint
    )
{
    // Retrieve user-manipulated variables from the LocalSettings collection if the app was previously suspended.
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    if (settingsValues->HasKey("m_totalZoom"))
    {
        m_totalZoom = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_totalZoom"))->GetSingle();
    }

    if (settingsValues->HasKey("m_viewPositionX"))
    {
        m_viewPosition.x = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_viewPositionX"))->GetSingle();
    }

    if (settingsValues->HasKey("m_viewPositionY"))
    {
        m_viewPosition.y = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_viewPositionY"))->GetSingle();
    }

    if (settingsValues->HasKey("m_imageRotate"))
    {
        m_imageRotate = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_imageRotate"))->GetSingle();
    }

    if (settingsValues->HasKey("leftInterpolationMode"))
    {
        int leftInterpolationMode = safe_cast<IPropertyValue^>(settingsValues->Lookup("leftInterpolationMode"))->GetInt32();
        m_2DAffineTransformEffectLeft->SetValue(D2D1_2DAFFINETRANSFORM_PROP_INTERPOLATION_MODE, leftInterpolationMode);
    }

    if (settingsValues->HasKey("rightInterpolationMode"))
    {
        int rightInterpolationMode = safe_cast<IPropertyValue^>(settingsValues->Lookup("rightInterpolationMode"))->GetInt32();
        m_2DAffineTransformEffectRight->SetValue(D2D1_2DAFFINETRANSFORM_PROP_INTERPOLATION_MODE, rightInterpolationMode);
    }

    if (settingsValues->HasKey("m_modeTextLeft"))
    {
        m_modeTextLeft = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_modeTextLeft"))->GetString();
    }

    if (settingsValues->HasKey("m_modeTextRight"))
    {
        m_modeTextRight = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_modeTextRight"))->GetString();
    }

    UpdateEffectMatrices();
}

void D2DInterpolationModes::Run()
{
    while (!m_isWindowClosed)
    {
        if (m_isWindowVisible)
        {
            m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessAllIfPresent);
            m_gestureRecognizer->ProcessInertia();
        }
        else
        {
            m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessOneAndAllPending);
        }
    }
}

// Following methods implement the Windows::ApplicationModel::Core::IFrameworkViewSource interface.
IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new D2DInterpolationModes();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}