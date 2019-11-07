//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved.

//
// Main window for the TriColor sample provider

#include "pch.h"
#include "TriColorWindow.h"
#include "TriColorValue.h"
#include <UIAutomationCore.h>
#include <UIAutomationCoreApi.h>

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace UiaCoreWindowProvider;

// Factory methods for providers
HRESULT CAppWindowProvider_CreateInstance(IInspectable **retVal);
HRESULT CTriColorControlProvider_CreateInstance(IInspectable **retVal);
HRESULT CTriColorFragmentProvider_CreateInstance(IInspectable **retVal);

// Utility method to convert Foundation Rects to D2D RectFs
D2D1_RECT_F RectToRectF(_In_ Rect rect)
{
    return D2D1::RectF(
        rect.X,
        rect.Y,
        rect.X + rect.Width,
        rect.Y + rect.Height
        );
}

// Utility method to convert Object^ to IRawElementProviderSimple
// to assist in calling UIA methods
ComPtr<IRawElementProviderSimple> AsSimpleProvider(Platform::Object^ object)
{
    ComPtr<IRawElementProviderSimple> spSimple;
    DX::ThrowIfFailed(
        ((IUnknown*)reinterpret_cast<__abi_IUnknown*>(object))->QueryInterface(IID_PPV_ARGS(&spSimple))
    );
    return spSimple;
}

//
// CTriColorWindow methods
//

CTriColorWindow::CTriColorWindow() :
    m_value(TriColorValue::Red)
{
}

CTriColorWindow::~CTriColorWindow()
{
}

// Basic initialization procedure for this window object.
void CTriColorWindow::Initialize(
    _In_ Windows::ApplicationModel::Core::CoreApplicationView^ view
)
{
    view->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &CTriColorWindow::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &CTriColorWindow::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &CTriColorWindow::OnResuming);
}

void CTriColorWindow::SetWindow(
    _In_ Windows::UI::Core::CoreWindow^ window
)
{
    // Create helper settings objects
    m_accSettings = ref new AccessibilitySettings();
    m_uiSettings = ref new UISettings();

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);

    // Set cursor for the window
    m_window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    // Register all the delegates
    m_window->KeyDown +=
        ref new TypedEventHandler<CoreWindow^, KeyEventArgs^>(this, &CTriColorWindow::OnKeyDown);
    m_window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &CTriColorWindow::OnPointerReleased);
    m_window->Activated +=
        ref new TypedEventHandler<CoreWindow^, WindowActivatedEventArgs^>(this, &CTriColorWindow::OnWindowActivated);
    m_window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &CTriColorWindow::OnWindowSizeChanged);
    m_window->Closed +=
        ref new TypedEventHandler<CoreWindow^, CoreWindowEventArgs^>(this, &CTriColorWindow::OnWindowClosed);
    m_window->AutomationProviderRequested +=
        ref new TypedEventHandler<CoreWindow^, AutomationProviderRequestedEventArgs^>(this, &CTriColorWindow::OnAutomationProviderRequested);
    m_accSettings->HighContrastChanged +=
        ref new TypedEventHandler<AccessibilitySettings^, Platform::Object^>(this, &CTriColorWindow::OnHighContrastChanged);
    DisplayProperties::LogicalDpiChanged += 
        ref new DisplayPropertiesEventHandler(this, &CTriColorWindow::OnLogicalDpiChanged);
    DisplayProperties::DisplayContentsInvalidated += 
        ref new DisplayPropertiesEventHandler(this, &CTriColorWindow::OnDisplayContentsInvalidated);
}

// Load all appropriate resources based on the entry point.
// This sample does not make use of the entry point argument.
void CTriColorWindow::Load(_In_ Platform::String^ /*entryPoint*/)
{
}

// Run the window's event processing loop.
void CTriColorWindow::Run()
{
    m_window->Activate();

    Render();
    Present();

    m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);        
}

// Clean up resources for this window.
void CTriColorWindow::Uninitialize()
{
}

TriColorValue CTriColorWindow::ControlValue::get()
{
    return m_value;
}

void CTriColorWindow::ControlValue::set(_In_ TriColorValue value)
{
    SetValue(value);
}

short CTriColorWindow::HasFocus::get()
{
    return (m_activated) ? VARIANT_TRUE : VARIANT_FALSE;
}

Rect CTriColorWindow::ControlRect::get()
{
    return m_controlRect;
}

Windows::Foundation::Rect CTriColorWindow::RectFromValue(_In_ UiaCoreWindowProvider::TriColorValue value)
{
    Rect rect(0,0,0,0);
    switch (value)
    {
    case TriColorValue::Red: rect = GetRedRect(); break;
    case TriColorValue::Yellow: rect = GetYellowRect(); break;
    case TriColorValue::Green: rect = GetGreenRect(); break;
    }
    return rect;
}

UiaCoreWindowProvider::TriColorValue CTriColorWindow::ValueFromPoint(_In_ Windows::Foundation::Point pt)
{
    TriColorValue value;
    DX::ThrowIfFailed(ValueFromPoint(pt, &value));
    return value;
}

Platform::Object ^CTriColorWindow::GetAppWindowProvider()
{
    EnsureAppWindowProviderCreated();
    return m_appWindowProvider;
}

Platform::Object ^CTriColorWindow::GetTriColorControlProvider()
{
    EnsureControlProviderCreated();
    return m_controlProvider;
}

Platform::Object ^CTriColorWindow::GetTriColorFragmentProvider(_In_ UiaCoreWindowProvider::TriColorValue value)
{
    EnsureFragmentProviderCreated(value);
    switch (value)
    {
    case TriColorValue::Red: return m_redProvider;
    case TriColorValue::Yellow: return m_yellowProvider;
    case TriColorValue::Green: return m_greenProvider;        
    default: DX::ThrowIfFailed(E_INVALIDARG); 
    }
    return nullptr;
}

//
// Event handlers for CoreWindow events
//

void CTriColorWindow::OnWindowSizeChanged(
    _In_ CoreWindow^ /*sender*/,
    _In_ WindowSizeChangedEventArgs^ /*args*/
    )
{
    UpdateForWindowSizeChange();
    Render();
    Present();
}

void CTriColorWindow::OnLogicalDpiChanged(
    _In_ Platform::Object^ /*sender*/
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
    Present();
}

void CTriColorWindow::OnDisplayContentsInvalidated(
    _In_ Platform::Object^ /*sender*/
    )
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    Render();
    Present();
}

void CTriColorWindow::OnActivated(
    _In_ CoreApplicationView^ /*applicationView*/,
    _In_ IActivatedEventArgs^ /*args*/
    )
{
    m_window->Activate();
}

void CTriColorWindow::OnSuspending(
    _In_ Platform::Object^ /*sender*/,
    _In_ SuspendingEventArgs^ /*args*/
    )
{
    // This sample has no interesting state,
    // so there is no work to do on suspension.
}
 
void CTriColorWindow::OnResuming(
    _In_ Platform::Object^ /*sender*/,
    _In_ Platform::Object^ /*args*/
    )
{
    // This sample has no interesting state,
    // so there is no work to do on resume.
}

void CTriColorWindow::OnKeyDown(
    _In_ Windows::UI::Core::CoreWindow^ /*sender*/, 
    _In_ Windows::UI::Core::KeyEventArgs^ e
    )
{
    VirtualKey vkey = e->VirtualKey;

    TriColorValue newValue = m_value;
    if (vkey == VirtualKey::Up)
    {
        if (!TriColorValueHelper::IsFirst(newValue))
        {
            newValue = TriColorValueHelper::PreviousValue(newValue);
            SetValue(newValue);
        }
    }
    else if (vkey == VirtualKey::Down)
    {
        if (!TriColorValueHelper::IsLast(newValue))
        {
            newValue = TriColorValueHelper::NextValue(newValue);
            SetValue(newValue);
        }
    }
}

void CTriColorWindow::OnPointerReleased(
    _In_ Windows::UI::Core::CoreWindow^ /*sender*/, 
    _In_ Windows::UI::Core::PointerEventArgs^ e
    )
{
    SetValue(ValueFromPoint(e->CurrentPoint->RawPosition));
}

void CTriColorWindow::OnWindowActivated(
    _In_ Windows::UI::Core::CoreWindow^ /*sender*/, 
    _In_ Windows::UI::Core::WindowActivatedEventArgs^ e
    )
{
    CoreWindowActivationState state = e->WindowActivationState;

    if (state == CoreWindowActivationState::CodeActivated ||
        state == CoreWindowActivationState::PointerActivated)
    {
        m_activated = true;

        // Render for the first time when window is activated.
        Render();
        Present();

        // Fire a focus changed event, after converting control provider to an IRawElementProviderSimple
        EnsureControlProviderCreated();
        UiaRaiseAutomationEvent(AsSimpleProvider(m_controlProvider).Get(), UIA_AutomationFocusChangedEventId);
    }
    else if (state == CoreWindowActivationState::Deactivated)
    {
        m_activated = false;
    }
}

void CTriColorWindow::OnWindowClosed(
    _In_ Windows::UI::Core::CoreWindow^ /*sender*/, 
    _In_ Windows::UI::Core::CoreWindowEventArgs^ /*e*/
    )
{
    // Disconnect our provider.
    if (m_appWindowProvider)
    {
        // Tell the provider it is now disconnected
        IDisconnectableProvider^ disconnectableProvider = dynamic_cast<IDisconnectableProvider^>(m_appWindowProvider);
        disconnectableProvider->Disconnect();
    }
}

// Respond to a request for an automation provider.
// We create one if we haven't yet, and then return it.
void CTriColorWindow::OnAutomationProviderRequested(
    _In_ Windows::UI::Core::CoreWindow^ /*sender*/, 
    _In_ Windows::UI::Core::AutomationProviderRequestedEventArgs ^e
    )
{
    EnsureAppWindowProviderCreated();
    e->AutomationProvider = m_appWindowProvider;
    e->Handled = true;
}

// Handle a high contrast change
void CTriColorWindow::OnHighContrastChanged(
    _In_ Windows::UI::ViewManagement::AccessibilitySettings^ /*sender*/, 
    _In_ Object^ /*e*/
    )
{
    DiscardThemeSpecificResources();
    CreateThemeSpecificResources();

    Render();
    Present();
}

// Create an automation provider if we don't already have one
void CTriColorWindow::EnsureAppWindowProviderCreated()
{
    if (!m_appWindowProvider)
    {
        // This unusual code sequence creates a new non-ref class
        // through a factory method, and then converts from IInspectable*
        // to Object^.
        ComPtr<IInspectable> spAppWindowProvider;
        DX::ThrowIfFailed(
            CAppWindowProvider_CreateInstance(&spAppWindowProvider)
            );

        IAppWindowProvider^ appWindowProviderInit = nullptr;
        DX::ThrowIfFailed(
            spAppWindowProvider.Get()->QueryInterface(__uuidof(IAppWindowProvider^), (void**)&appWindowProviderInit)
            );
        appWindowProviderInit->Initialize(this, m_window.Get());
        m_appWindowProvider = appWindowProviderInit;
    }
}

// Create a control provider if we don't already have one
void CTriColorWindow::EnsureControlProviderCreated()
{
    if (!m_controlProvider)
    {
        // This unusual code sequence creates a new non-ref class
        // through a factory method, and then converts from IInspectable*
        // to Object^.
        ComPtr<IInspectable> spControlProvider;
        DX::ThrowIfFailed(
            CTriColorControlProvider_CreateInstance(&spControlProvider)
            );

        ITriColorControlProvider^ controlProviderInit = nullptr;
        DX::ThrowIfFailed(
            spControlProvider.Get()->QueryInterface(__uuidof(ITriColorControlProvider^), (void**)&controlProviderInit)
            );
        controlProviderInit->Initialize(this);
        m_controlProvider = controlProviderInit;
    }
}

// Create a fragment provider if we don't already have one
void CTriColorWindow::EnsureFragmentProviderCreated(_In_ TriColorValue value)
{
    // Get the right pointer to update
    Platform::Object ^* pspProvider = nullptr;
    switch (value)
    {
    case TriColorValue::Red: pspProvider = &m_redProvider; break;
    case TriColorValue::Yellow: pspProvider = &m_yellowProvider; break;
    case TriColorValue::Green: pspProvider = &m_greenProvider; break;
    }

    if (pspProvider && !(*pspProvider))
    {
        // This unusual code sequence creates a new non-ref class
        // through a factory method, and then converts from IInspectable*
        // to Object^.
        ComPtr<IInspectable> spFragmentProvider;
        DX::ThrowIfFailed(
            CTriColorFragmentProvider_CreateInstance(&spFragmentProvider)
            );

        ITriColorFragmentProvider^ fragmentProviderInit = nullptr;
        DX::ThrowIfFailed(
            spFragmentProvider.Get()->QueryInterface(__uuidof(ITriColorFragmentProvider^), (void**)&fragmentProviderInit)
            );
        fragmentProviderInit->Initialize(this, value);
        *pspProvider = fragmentProviderInit;
    }
}

// Create resources which are not bound to any device. 
// Their lifetime effectively extends for the duration of the app. 
// These resources include the Direct2D and DirectWrite factories, 
// and a DirectWrite Text Format object.
void CTriColorWindow::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    // Create a DirectWrite text format object.
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Segoe UI", 
            nullptr, 
            DWRITE_FONT_WEIGHT_NORMAL, 
            DWRITE_FONT_STYLE_NORMAL, 
            DWRITE_FONT_STRETCH_NORMAL, 
            24.0f, 
            L"en-US", 
            &m_titleFormat
            )
    );
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Segoe UI", 
            nullptr, 
            DWRITE_FONT_WEIGHT_NORMAL, 
            DWRITE_FONT_STYLE_NORMAL, 
            DWRITE_FONT_STRETCH_NORMAL, 
            14.0f, 
            L"en-US", 
            &m_labelFormat
            )
    );
}

//  This method creates resources which are bound to a particular
//  Direct3D device. It's all centralized here, in case the resources
//  need to be recreated in case of Direct3D device loss (eg. display
//  change, remoting, removal of video card, etc).
void CTriColorWindow::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    // Create brushes.
    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::Black), &m_blackBrush)
    );
    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::White), &m_whiteBrush)
    );
    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::Red), &m_redBrush)
    );
    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::Yellow), &m_yellowBrush)
    );
    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::Green), &m_greenBrush)
    );

    CreateThemeSpecificResources();
}

void CTriColorWindow::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();
}

void CTriColorWindow::CreateThemeSpecificResources()
{
    Windows::UI::Color color = m_uiSettings->UIElementColor(UIElementType::WindowText);
    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(D2D1::ColorF(RGB(color.R, color.G, color.B)), &m_windowTextBrush)
    );
}
    
void CTriColorWindow::DiscardThemeSpecificResources()
{
    m_windowTextBrush = nullptr;
}

// Re-render the contents of the window
void CTriColorWindow::Render()
{
    m_d2dContext->BeginDraw();

    // Retrieve the size of the render target.
    D2D1_SIZE_F renderTargetSize = m_d2dContext->GetSize();

    Windows::UI::Color color = m_uiSettings->UIElementColor(UIElementType::Window);
    m_d2dContext->Clear(D2D1::ColorF(RGB(color.R, color.G, color.B)));

    // Define the control coordinates - control is at (100, 200) and extends
    // out to a 100-pixel border, unless total window is too narrow for that.
    m_controlRect.X = 100;
    m_controlRect.Y = 200;
    m_controlRect.Width = max(renderTargetSize.width - 200, 0);
    m_controlRect.Height = max(renderTargetSize.height - 300, 0);

    // Draw the title
    static const WCHAR sc_title[] = L"UIA CoreWindow Sample";
    m_d2dContext->DrawText(
        sc_title, 
        ARRAYSIZE(sc_title) - 1, 
        m_titleFormat.Get(),
        D2D1::RectF(10, 10, renderTargetSize.width - 10, renderTargetSize.height - 10), 
        m_windowTextBrush.Get()
        );

    static const WCHAR sc_description[] = L"Demonstrates techniques for creating Accessible applications using the CoreWindow " \
        L"and Direct2D APIs.  This application has a simple UI Automation (UIA) tree which includes the custom control below and " \
        L"demonstrates the basic techniques for making the control programmatically accessible.  This application also follows " \
        L"the High Contrast themes and is keyboard-accessible (arrow keys move the control selection).";
    m_d2dContext->DrawText(
        sc_description, 
        ARRAYSIZE(sc_description) - 1, 
        m_labelFormat.Get(),
        D2D1::RectF(10, 60, renderTargetSize.width - 10, renderTargetSize.height - 60), 
        m_windowTextBrush.Get()
        );

    // Paint rectangles and labels
    D2D1_RECT_F rectRed = RectToRectF(GetRedRect());
    D2D1_RECT_F rectYellow = RectToRectF(GetYellowRect());
    D2D1_RECT_F rectGreen = RectToRectF(GetGreenRect());
    D2D1_RECT_F rectControl = RectToRectF(m_controlRect);

    m_d2dContext->FillRectangle(
        &rectRed,
        m_redBrush.Get()
        );

    m_d2dContext->FillRectangle(
        &rectYellow,
        m_yellowBrush.Get()
        );

    m_d2dContext->FillRectangle(
        &rectGreen,
        m_greenBrush.Get()
        );

    m_d2dContext->DrawRectangle(
        &rectControl,
        m_blackBrush.Get()
        );

    m_d2dContext->DrawText(
        L"Red",
        3,
        m_labelFormat.Get(),
        rectRed,
        m_whiteBrush.Get()
        );

    m_d2dContext->DrawText(
        L"Yellow",
        6,
        m_labelFormat.Get(),
        rectYellow,
        m_blackBrush.Get()
        );

    m_d2dContext->DrawText(
        L"Green",
        5,
        m_labelFormat.Get(),
        rectGreen,
        m_whiteBrush.Get()
        );

    // Paint the selection circle.
    ComPtr<ID2D1SolidColorBrush> selectionBrush = 
        (this->m_value == TriColorValue::Yellow) ?
        m_blackBrush :
        m_whiteBrush;

    Rect selectionRect = RectFromValue(m_value);
    m_d2dContext->FillEllipse(
        D2D1::Ellipse(
            D2D1::Point2F(
                selectionRect.X + (selectionRect.Width / 2),
                selectionRect.Y + (selectionRect.Height / 2)
                ),
            10.0,
            10.0
            ),
        selectionBrush.Get());
    
    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}

Rect CTriColorWindow::GetRedRect()
{
    Rect redRect = m_controlRect;
    redRect.Height = m_controlRect.Height / 3;
    return redRect;
}

Rect CTriColorWindow::GetYellowRect()
{
    Rect yellowRect = m_controlRect;
    yellowRect.Y = m_controlRect.Y + (m_controlRect.Height / 3);
    yellowRect.Height = m_controlRect.Height / 3;
    return yellowRect;
}

Rect CTriColorWindow::GetGreenRect()
{
    Rect greenRect = m_controlRect;
    greenRect.Y = m_controlRect.Y + (m_controlRect.Height / 3) + (m_controlRect.Height / 3);
    greenRect.Height = m_controlRect.Height / 3;
    return greenRect;
}

HRESULT CTriColorWindow::ValueFromPoint(_In_ Point pt, _Out_ TriColorValue * pValue)
{
    HRESULT hr = E_INVALIDARG;
    *pValue = TriColorValue::Red;

    // This could be optimized by caching the rectangles,
    // if this is called often.
    Rect redRect = GetRedRect();
    Rect yellowRect = GetYellowRect();
    Rect greenRect = GetGreenRect();
    if (PointInRect(redRect, pt))
    {
        *pValue = TriColorValue::Red;
        hr = S_OK;
    }
    else if (PointInRect(yellowRect, pt))
    {
        *pValue = TriColorValue::Yellow;
        hr = S_OK;
    }
    else if (PointInRect(greenRect, pt))
    {
        *pValue = TriColorValue::Green;
        hr = S_OK;
    }
    return hr;
}

/* static */
bool CTriColorWindow::PointInRect(_In_ Rect rect, _In_ Point pt)
{
    return (pt.X >= rect.X &&
            pt.X < (rect.X + rect.Width) &&
            pt.Y >= rect.Y &&
            pt.Y < (rect.Y + rect.Height));
}

void CTriColorWindow::SetValue(_In_ TriColorValue newValue)
{
    TriColorValue oldValue = m_value;

    m_value = newValue;
    Render();
    Present();

    if (oldValue != newValue)
    {
        VARIANT varOldValue;
        VARIANT varNewValue;
        VariantInit(&varOldValue);
        VariantInit(&varNewValue);

        // Query for string representations of the old and new values
        HRESULT hr = TriColorValueHelper::ValueToString(oldValue, &varOldValue.bstrVal);
        if (SUCCEEDED(hr))
        {
            varOldValue.vt = VT_BSTR;
            hr = TriColorValueHelper::ValueToString(newValue, &varNewValue.bstrVal);
            if (SUCCEEDED(hr))
            {
                varNewValue.vt = VT_BSTR;

                EnsureControlProviderCreated();
                // Notify the property change
                UiaRaiseAutomationPropertyChangedEvent(AsSimpleProvider(m_controlProvider).Get(), UIA_ValueValuePropertyId, varOldValue, varNewValue);
            }
        }

        // Notify the selection changed
        Platform::Object^ selectedFragment = GetTriColorFragmentProvider(newValue);
        UiaRaiseAutomationEvent(AsSimpleProvider(selectedFragment).Get(), UIA_SelectionItem_ElementSelectedEventId);

        VariantClear(&varOldValue);
        VariantClear(&varNewValue);
    }
}

//
// CTriColorWindowFactory methods
//

// Create an instance of the TriColorWindow
Windows::ApplicationModel::Core::IFrameworkView^ CTriColorWindowFactory::CreateView()
{
    return ref new CTriColorWindow();
}

// The main entry point for the sample program.
[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto frameworkViewSource = ref new CTriColorWindowFactory();
    Windows::ApplicationModel::Core::CoreApplication::Run(frameworkViewSource);
    return 0;
}

