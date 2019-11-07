//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DWriteParagraphText.h"

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Input;
using namespace Windows::UI::Core;
using namespace Windows::UI::Popups;
using namespace Windows::Storage;

DWriteParagraphText::DWriteParagraphText()
{
}

void DWriteParagraphText::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    // Create a DirectWrite text format object.
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_REGULAR,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            64.0f,
            L"en-US", // locale
            &m_textFormat
            )
        );

    // Create two booleans to store the state of the layout.
    m_isJustified = false;
    m_isIncreasedSpacing = false;

    // Center the text horizontally.
    DX::ThrowIfFailed(
        m_textFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER)
        );

    // Center the text vertically.
    DX::ThrowIfFailed(
        m_textFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER)
        );
}

void DWriteParagraphText::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "DirectWrite paragraph text sample"
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::Black),
            &m_blackBrush
            )
        );
}

void DWriteParagraphText::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    Platform::String^ text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc convallis, \
dui vel luctus posuere, mauris metus sodales justo, sit amet tincidunt velit \
ipsum placerat augue. Integer consequat pellentesque orci, id pharetra dolor \
lacinia quis. Donec leo neque, tempor ac imperdiet in, adipiscing vel nibh. \
Curabitur sed est nibh. Etiam tincidunt sagittis tortor sed luctus. Nam purus \
turpis, mattis nec malesuada vel, molestie ut velit. Etiam semper rutrum vestibulum.\
Proin eget mi nunc. Vivamus lobortis iaculis risus at dapibus. In libero ipsum, feugiat \
quis convallis ut, rutrum eget quam. ";

    D2D1_SIZE_F size = m_d2dContext->GetSize();
    size.height = size.height / 3;
    size.width = size.width / 3;

    ComPtr<IDWriteTextLayout> textLayout;

    // Create a DirectWrite text layout object.
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextLayout(
            text->Data(),                       // Text to be displayed
            text->Length(),                     // Length of the text
            m_textFormat.Get(),                 // DirectWrite Text Format object
            size.width,                         // Width of the Text Layout
            size.height,                        // Height of the Text Layout
            &textLayout
            )
        );

    // QI the IDWriteTextLayout into a IDWriteTextLayout1.
    textLayout.As(&m_textLayout1);

    m_textRange.startPosition = 0;
    m_textRange.length = text->Length();
    m_textLayout1->SetFontSize(16, m_textRange);
    m_textLayout1->SetCharacterSpacing(0.5f, 0.5f, 0, m_textRange);
}

void DWriteParagraphText::Render()
{
    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));
    m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());

    D2D1_SIZE_F screenSize = m_d2dContext->GetSize();

    D2D1_SIZE_F size = m_d2dContext->GetSize();
    size.height = size.height / 3;
    size.width = size.width / 3;

    m_d2dContext->DrawLine(
        D2D1::Point2F((screenSize.width - 2 * size.width), 0.0f),
        D2D1::Point2F((screenSize.width - 2 * size.width), screenSize.height),
        m_blackBrush.Get()
        );

    m_d2dContext->DrawLine(
        D2D1::Point2F((screenSize.width - size.width), 0.0f),
        D2D1::Point2F((screenSize.width - size.width), screenSize.height),
        m_blackBrush.Get()
        );

    m_d2dContext->DrawTextLayout(
        D2D1::Point2F(size.width, size.height),
        m_textLayout1.Get(),
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

void DWriteParagraphText::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &DWriteParagraphText::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &DWriteParagraphText::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &DWriteParagraphText::OnResuming);
}

void DWriteParagraphText::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &DWriteParagraphText::OnWindowSizeChanged);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &DWriteParagraphText::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &DWriteParagraphText::OnDisplayContentsInvalidated);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &DWriteParagraphText::OnPointerMoved);

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &DWriteParagraphText::OnPointerPressed);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &DWriteParagraphText::OnPointerReleased);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);

    m_gestureRecognizer = ref new GestureRecognizer();

    m_gestureRecognizer->GestureSettings = GestureSettings::Hold;

    m_gestureRecognizer->Holding +=
        ref new TypedEventHandler<GestureRecognizer^, HoldingEventArgs^>(this, &DWriteParagraphText::OnHolding);
}

void DWriteParagraphText::OnPointerMoved(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessMoveEvents(args->GetIntermediatePoints());
}

void DWriteParagraphText::OnPointerPressed(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessDownEvent(args->CurrentPoint);
}

void DWriteParagraphText::OnPointerReleased(
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

void DWriteParagraphText::OnHolding(
    _In_ GestureRecognizer^ recognizer,
    _In_ HoldingEventArgs^ args
    )
{
    if (args->HoldingState == HoldingState::Started)
    {
        ShowMenu(args->Position);
    }
}

void DWriteParagraphText::Load(
    _In_ Platform::String^ entryPoint
    )
{
    // Retrieve any stored variables from the LocalSettings collection.
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    if (settingsValues->HasKey("m_isJustified"))
    {
        m_isJustified = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_isJustified"))->GetBoolean();
        ApplyJustificationSettings();
    }
    if (settingsValues->HasKey("m_isIncreasedSpacing"))
    {
        m_isIncreasedSpacing = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_isIncreasedSpacing"))->GetBoolean();
        ApplySpacingSettings();
    }
}

void DWriteParagraphText::Run()
{
    Render();
    Present();

    CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);
}

void DWriteParagraphText::Uninitialize()
{
}

void DWriteParagraphText::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();
    ApplyJustificationSettings();
    ApplySpacingSettings();
    Render();
    Present();
}

void DWriteParagraphText::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
    Present();
}

void DWriteParagraphText::OnDisplayContentsInvalidated(
    _In_ Platform::Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    ApplyJustificationSettings();
    ApplySpacingSettings();
    Render();
    Present();
}

void DWriteParagraphText::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

void DWriteParagraphText::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    if (settingsValues->HasKey("m_isJustified"))
    {
        settingsValues->Remove("m_isJustified");
    }
    settingsValues->Insert("m_isJustified", PropertyValue::CreateBoolean(m_isJustified));

    if (settingsValues->HasKey("m_isIncreasedSpacing"))
    {
        settingsValues->Remove("m_isIncreasedSpacing");
    }
    settingsValues->Insert("m_isIncreasedSpacing", PropertyValue::CreateBoolean(m_isIncreasedSpacing));
}

void DWriteParagraphText::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
}

void DWriteParagraphText::ShowMenu(Point position)
{
    PopupMenu^ popupMenu = ref new PopupMenu();
    popupMenu->Commands->Append(
        ref new UICommand("Toggle Justification",
        ref new UICommandInvokedHandler(this, &DWriteParagraphText::UpdateJustification))
        );

    popupMenu->Commands->Append(
        ref new UICommand("Toggle Loose Character Spacing",
        ref new UICommandInvokedHandler(this, &DWriteParagraphText::UpdateCharacterSpacing))
        );

    popupMenu->ShowAsync(position);
}

void DWriteParagraphText::UpdateJustification(IUICommand^ command)
{
    if (m_textLayout1->GetTextAlignment() == DWRITE_TEXT_ALIGNMENT_JUSTIFIED)
    {
        m_isJustified = false;
    }
    else
    {
        m_isJustified = true;
    }
    ApplyJustificationSettings();
    Render();
    Present();
}

void DWriteParagraphText::UpdateCharacterSpacing(IUICommand^ command)
{
    FLOAT leadingSpacing;
    FLOAT trailingSpacing;
    FLOAT minimumAdvanceWidth;
    m_textLayout1->GetCharacterSpacing(0, &leadingSpacing, &trailingSpacing, &minimumAdvanceWidth);
    if (leadingSpacing == 2)
    {
        m_isIncreasedSpacing = false;
    }
    else
    {
        m_isIncreasedSpacing = true;
    }
    ApplySpacingSettings();
    Render();
    Present();
}

void DWriteParagraphText::ApplyJustificationSettings()
{
    if (!m_isJustified)
    {
        m_textLayout1->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER);
    }
    else
    {
        m_textLayout1->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_JUSTIFIED);
    }
}

void DWriteParagraphText::ApplySpacingSettings()
{
    if (!m_isIncreasedSpacing)
    {
        m_textLayout1->SetCharacterSpacing(0.5f, 0.5f, 0, m_textRange);
    }
    else
    {
        m_textLayout1->SetCharacterSpacing(2, 2, 0, m_textRange);
    }
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new DWriteParagraphText();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}