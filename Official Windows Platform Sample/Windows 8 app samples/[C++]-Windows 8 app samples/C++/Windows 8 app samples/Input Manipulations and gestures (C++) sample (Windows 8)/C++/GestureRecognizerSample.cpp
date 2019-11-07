// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#include "pch.h"
#include "GestureRecognizerSample.h"
#include "BackgroundObject.h"

///////////////////////////////////////////////////////////////////////////////
// Application view implementation
//

GestureRecognizerSample::GestureRecognizerSample(
    _In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView)
:   _background(nullptr),
    _objects(nullptr),
    _redraw(true),
    _done(false)
{
    for (int i=0; i<ARRAYSIZE(_zorder); ++i)
    {
        _zorder[i] = i;
    }

    _background = ref new BackgroundObject(nullptr, this);

    _objects = ref new Platform::Array<DrawingObject^>(DRAWING_OBJECTS_COUNT);
    for (unsigned int i=0; i<_objects->Length; ++i)
    {
        _objects[i] = ref new DrawingObject(_background, this);
    }

    // Set up activation handler
    applicationView->Activated +=
        ref new Windows::Foundation::TypedEventHandler<
            Windows::ApplicationModel::Core::CoreApplicationView^,
            Windows::ApplicationModel::Activation::IActivatedEventArgs^>(
                this, &GestureRecognizerSample::OnViewActivated);

    // Set up process lifetime management handlers
    Windows::ApplicationModel::Core::CoreApplication::Suspending +=
        ref new Windows::Foundation::EventHandler<
            Windows::ApplicationModel::SuspendingEventArgs^>(
                this, &GestureRecognizerSample::OnSuspending);

    Windows::ApplicationModel::Core::CoreApplication::Resuming +=
        ref new Windows::Foundation::EventHandler<
            Platform::Object^>(
                this, &GestureRecognizerSample::OnResuming);

    ResetScene();

    _gestureRecognizers = ref new Platform::Array<Windows::UI::Input::GestureRecognizer^>(_objects->Length + 1);
    // Create gesture recognizers for the objects
    for (unsigned int i=0; i<_objects->Length; ++i)
    {
        _gestureRecognizers[i] = ref new Windows::UI::Input::GestureRecognizer();
        // Statically associate gesture recognizer with one drawing object
        // In this sample we assume 1:1 mapping between recognizers and objects (but this may not be the case in general,
        // we may create less recognizers than objects and dynamically assign them only to those objects that need them).
        _objects[i]->Attach(_gestureRecognizers[i]);
    }
    // Create gesture recognizer for the background
    _gestureRecognizers[_objects->Length] = ref new Windows::UI::Input::GestureRecognizer();
    // Statically associate gesture recognizer with background object
    _background->Attach(_gestureRecognizers[_objects->Length]);
}

void GestureRecognizerSample::SetWindow(_In_ Windows::UI::Core::CoreWindow^ window)
{
    DirectXBase::Initialize(window, Windows::Graphics::Display::DisplayProperties::LogicalDpi);

    // Set up window event handlers
    window->PointerCursor = ref new Windows::UI::Core::CoreCursor(
        Windows::UI::Core::CoreCursorType::Arrow, 0);

    window->Activated +=
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Core::CoreWindow^, Windows::UI::Core::WindowActivatedEventArgs^>(
                this, &GestureRecognizerSample::OnWindowActivated);

    window->SizeChanged +=
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Core::CoreWindow^, Windows::UI::Core::WindowSizeChangedEventArgs^>(
                this, &GestureRecognizerSample::OnWindowSizeChanged);

    window->Closed +=
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Core::CoreWindow^, Windows::UI::Core::CoreWindowEventArgs^>(
                this, &GestureRecognizerSample::OnWindowClosed);

    window->PointerPressed +=
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Core::CoreWindow^, Windows::UI::Core::PointerEventArgs^>(
                this, &GestureRecognizerSample::OnPointerPressed);

    window->PointerMoved +=
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Core::CoreWindow^, Windows::UI::Core::PointerEventArgs^>(
                this, &GestureRecognizerSample::OnPointerMoved);

    window->PointerReleased +=
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Core::CoreWindow^, Windows::UI::Core::PointerEventArgs^>(
                this, &GestureRecognizerSample::OnPointerReleased);

    window->PointerWheelChanged +=
        ref new Windows::Foundation::TypedEventHandler<
            Windows::UI::Core::CoreWindow^, Windows::UI::Core::PointerEventArgs^>(
                this, &GestureRecognizerSample::OnPointerWheelChanged);

    // Set up DPI change notification handler
    Windows::Graphics::Display::DisplayProperties::LogicalDpiChanged += 
        ref new Windows::Graphics::Display::DisplayPropertiesEventHandler(
            this, &GestureRecognizerSample::OnLogicalDpiChanged);

    Windows::Graphics::Display::DisplayProperties::DisplayContentsInvalidated += 
        ref new Windows::Graphics::Display::DisplayPropertiesEventHandler(
            this, &GestureRecognizerSample::OnDisplayContentsInvalidated);
}

GestureRecognizerSample::~GestureRecognizerSample()
{
    // Disconnect gesture recognizers from drawing objects
    for (unsigned int i=0; i<_objects->Length; ++i)
    {
        _objects[i]->Detach(_gestureRecognizers[i]);
    }
    _background->Detach(_gestureRecognizers[_objects->Length]);
}

void GestureRecognizerSample::Run()
{
    while (!_done)
    {
        // Input processing phase:
        // If the window's message queue contains pending window events, process them and proceed to drawing phase.
        // Otherwise, wait for window events to arrive.
        GetWindow()->Dispatcher->ProcessEvents(Windows::UI::Core::CoreProcessEventsOption::ProcessOneAndAllPending);

        // Drawing phase:
        // If the scene has changed since last drawing, render and present the scene.
        if (_redraw)
        {
            _redraw = false;
            Render();
            Present();
        }
    }
}

///////////////////////////////////////////////////////////////////////////////
// Application view activation handler.
//

void GestureRecognizerSample::OnViewActivated(
    _In_ Windows::ApplicationModel::Core::CoreApplicationView^, 
    _In_ Windows::ApplicationModel::Activation::IActivatedEventArgs^ args)
{
    if (args->PreviousExecutionState == Windows::ApplicationModel::Activation::ApplicationExecutionState::Terminated)
    {
        // If activating from Terminated state, restore previously saved state of the application.
        _background->Deserialize(GetObjectProperties(L"GRS.Bgnd"));
        for (unsigned int i=0; i<_objects->Length; ++i)
        {
            WCHAR name[32];
            StringCchPrintf(name, ARRAYSIZE(name), L"GRS.Obj%u", i+1);            
            _objects[i]->Deserialize(GetObjectProperties(ref new Platform::String(name)));    
        }
    }

    // Activate the window so that it will be made visible.
    GetWindow()->Activate();
}

///////////////////////////////////////////////////////////////////////////////
// Process lifetime management handlers and helpers.
//

void GestureRecognizerSample::OnSuspending(
    _In_ Platform::Object^,
    _In_ Windows::ApplicationModel::SuspendingEventArgs^ args)
{
    // Suspending operation has a timeout, so the application should ask for deferral, if it expects
    // some lengthy operation to be done here.
    Windows::ApplicationModel::SuspendingDeferral^ deferral = args->SuspendingOperation->GetDeferral();

    // Save the application state
    _background->Serialize(GetObjectProperties(L"GRS.Bgnd"));
    for (unsigned int i=0; i<_objects->Length; ++i)
    {
        WCHAR name[32];
        StringCchPrintf(name, ARRAYSIZE(name), L"GRS.Obj%u", i+1);
        _objects[i]->Serialize(GetObjectProperties(ref new Platform::String(name)));    
    }

    // Mark the operation completed, so that the application can be suspended.
    deferral->Complete();
}

void GestureRecognizerSample::OnResuming(
    _In_ Platform::Object^,
    _In_ Platform::Object^)
{
}

Windows::Foundation::Collections::IPropertySet^ GestureRecognizerSample::GetObjectProperties(_In_ Platform::String^ name)
{
    Windows::Storage::ApplicationDataContainer^ settings = Windows::Storage::ApplicationData::Current->LocalSettings;
    Windows::Storage::ApplicationDataContainer^ container = nullptr;
    if (settings->Containers->HasKey(name))
    {
        container = settings->Containers->Lookup(name);
    }
    else
    {
        container = settings->CreateContainer(name, Windows::Storage::ApplicationDataCreateDisposition::Always);
    }
    return container->Values;
}

///////////////////////////////////////////////////////////////////////////////
// CoreWindow event handlers.
//

void GestureRecognizerSample::OnWindowActivated(
    _In_ Windows::UI::Core::CoreWindow^,
    _In_ Windows::UI::Core::WindowActivatedEventArgs^)
{
    RequestRedraw();
}

void GestureRecognizerSample::OnWindowSizeChanged(
    _In_ Windows::UI::Core::CoreWindow^,
    _In_ Windows::UI::Core::WindowSizeChangedEventArgs^)
{
    UpdateForWindowSizeChange();
    RequestRedraw();
}

void GestureRecognizerSample::OnWindowClosed(
    _In_ Windows::UI::Core::CoreWindow^,
    _In_ Windows::UI::Core::CoreWindowEventArgs^)
{
    _done = true;
}

void GestureRecognizerSample::OnPointerPressed(
    _In_ Windows::UI::Core::CoreWindow^,
    _In_ Windows::UI::Core::PointerEventArgs^ args)
{
    unsigned int pointerId = args->CurrentPoint->PointerId;

    // Iterate over objects, respecting z-order: from the one at the top to the one at the bottom.
    for (unsigned int i=0; i<_objects->Length; ++i)
    {
        int iz = _zorder[i];
        // Create transformed PointerPoint relative to parent object
        Windows::UI::Input::PointerPoint^ pointerPoint = Windows::UI::Input::PointerPoint::GetCurrentPoint(
            pointerId,
            ref new ManipulatableObjectTransform(_objects[iz]->Parent()));

        // Hit testing: find the object under the pointer.
        if (_objects[iz]->HitTest(pointerPoint->RawPosition))
        {
            // Assign the pointer to the object found in hit testing.
            _gestureRecognizers[iz]->ProcessDownEvent(pointerPoint);
            // Make the object topmost, the first in z-order array.
            ChangeZOrder(iz);
            RequestRedraw();
            return;
        }
    }

    // No objects found in hit testing. Assign this pointer to background.
    Windows::UI::Input::PointerPoint^ pointerPoint = Windows::UI::Input::PointerPoint::GetCurrentPoint(
        pointerId,
        ref new ManipulatableObjectTransform(_background->Parent()));
    _background->SetDevice(pointerPoint->PointerDevice);
    _gestureRecognizers[_objects->Length]->ProcessDownEvent(pointerPoint);
}

void GestureRecognizerSample::OnPointerMoved(
    _In_ Windows::UI::Core::CoreWindow^,
    _In_ Windows::UI::Core::PointerEventArgs^ args)
{
    unsigned int pointerId = args->CurrentPoint->PointerId;

    for (unsigned int i=0; i<_gestureRecognizers->Length; ++i)
    {
        // Create transformed PointerPoints relative to parent object
        ManipulatableObject^ parent = (i<_objects->Length) ? _objects[i]->Parent() : _background->Parent();
        Windows::Foundation::Collections::IVector<Windows::UI::Input::PointerPoint^>^ pointerPoints = Windows::UI::Input::PointerPoint::GetIntermediatePoints(
            pointerId,
            ref new ManipulatableObjectTransform(parent));
        _gestureRecognizers[i]->ProcessMoveEvents(pointerPoints);
    }
}

void GestureRecognizerSample::OnPointerReleased(
    _In_ Windows::UI::Core::CoreWindow^,
    _In_ Windows::UI::Core::PointerEventArgs^ args)
{
    unsigned int pointerId = args->CurrentPoint->PointerId;

    for (unsigned int i=0; i<_gestureRecognizers->Length; ++i)
    {
        // Create transformed PointerPoint relative to parent object
        ManipulatableObject^ parent = (i<_objects->Length) ? _objects[i]->Parent() : _background->Parent();
        Windows::UI::Input::PointerPoint^ pointerPoint = Windows::UI::Input::PointerPoint::GetCurrentPoint(
            pointerId,
            ref new ManipulatableObjectTransform(parent));
        _gestureRecognizers[i]->ProcessUpEvent(pointerPoint);
    }
}

void GestureRecognizerSample::OnPointerWheelChanged(
    _In_ Windows::UI::Core::CoreWindow^,
    _In_ Windows::UI::Core::PointerEventArgs^ args)
{
    unsigned int pointerId = args->CurrentPoint->PointerId;

    // Create transformed PointerPoint relative to parent object
    Windows::UI::Input::PointerPoint^ pointerPoint = Windows::UI::Input::PointerPoint::GetCurrentPoint(
        pointerId,
        ref new ManipulatableObjectTransform(_background->Parent()));

    // Set mouse wheel parameters
    Windows::UI::Input::MouseWheelParameters^ params = _gestureRecognizers[_objects->Length]->MouseWheelParameters;
    Windows::Foundation::Size viewSize = ViewSize();
    params->CharTranslation = Windows::Foundation::Point(0.05f * viewSize.Width, 0.05f * viewSize.Height);
    params->PageTranslation = Windows::Foundation::Point(viewSize.Width, viewSize.Height);
    params->DeltaScale = 1.5f;
    params->DeltaRotationAngle = 22.5f;

    // Assign wheel pointer to background.
    Windows::System::VirtualKeyModifiers vkmod = args->KeyModifiers;
    _gestureRecognizers[_objects->Length]->ProcessMouseWheelEvent(
        pointerPoint,
        (int)(vkmod & Windows::System::VirtualKeyModifiers::Shift) != 0,
        (int)(vkmod & Windows::System::VirtualKeyModifiers::Control) != 0);
}

///////////////////////////////////////////////////////////////////////////////
// IRenderer implementation.
//

void GestureRecognizerSample::RequestRedraw()
{
    _redraw = true;
}

Windows::Foundation::Size GestureRecognizerSample::ViewSize()
{
    D2D1_SIZE_F d2dsize = GetDeviceContext()->GetSize();
    Windows::Foundation::Size size;
    size.Width = d2dsize.width;
    size.Height = d2dsize.height;
    return size;
}

///////////////////////////////////////////////////////////////////////////////
// DirectXBase base class overrides.
//

//
// Create resources which are not bound to any device. 
// Their lifetime effectively extends for the duration of the app. 
// These resources include the Direct2D and DirectWrite factories, 
// and a DirectWrite Text Format object.
//
void GestureRecognizerSample::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    // Create a DirectWrite text format object.
    DX::ThrowIfFailed(
        GetDWriteFactory()->CreateTextFormat(L"Segoe UI", NULL, DWRITE_FONT_WEIGHT_NORMAL,
            DWRITE_FONT_STYLE_NORMAL, DWRITE_FONT_STRETCH_NORMAL, 20.0f, L"",
            &_eventTextFormat));

    // Center the text horizontally and vertically.
    _eventTextFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_LEADING);
    _eventTextFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_NEAR);
}

//
//  This method creates resources which are bound to a particular
//  Direct3D device. It's all centralized here, in case the resources
//  need to be recreated in case of Direct3D device loss (eg. display
//  change, remoting, removal of video card, etc).
//
void GestureRecognizerSample::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    // Create a white brush.
    DX::ThrowIfFailed(
        GetDeviceContext()->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::White), &_whiteBrush));

    // Create object brushes.
    D2D1_COLOR_F colors[(int)DrawingObject::Color::MaxCount] = 
    {
        D2D1::ColorF(0x008c00), // green
        D2D1::ColorF(0x00628c), // blue
        D2D1::ColorF(0xc24a00), // orange
        D2D1::ColorF(0x59008c), // purple
        D2D1::ColorF(0xbf9600), // yellow
        D2D1::ColorF(0x8c0000)  // red
    };
    for (DrawingObject::Color color=DrawingObject::Color::First; color<DrawingObject::Color::MaxCount; ++color)
    {
        DX::ThrowIfFailed(
            GetDeviceContext()->CreateSolidColorBrush(colors[(int)color], &_objectBrush[(int)color]));
    }
}

//
// This method will be called whenever the application needs to 
// display the client window.
//
void GestureRecognizerSample::Render()
{
    Microsoft::WRL::ComPtr<ID2D1DeviceContext> deviceContext = GetDeviceContext();

    // Drawing
    deviceContext->BeginDraw();
    deviceContext->SetTransform(D2D1::Matrix3x2F::Identity());
    deviceContext->Clear(D2D1::ColorF(D2D1::ColorF::Black));
    _background->DrawBackground(
        deviceContext.Get(),
        _whiteBrush.Get());
    for (int i=_objects->Length-1; i>=0; --i)
    {
        int iz = _zorder[i];
        _objects[iz]->Draw(
            deviceContext.Get(),
            _objectBrush[(int)_objects[iz]->GetColor()].Get());
    }
    _background->DrawOverlay(
        deviceContext.Get(),
        _eventTextFormat.Get(),
        _whiteBrush.Get());
    for (unsigned int i=0; i<_objects->Length; ++i)
    {
        _objects[i]->DrawOverlay(
            deviceContext.Get(),
            _eventTextFormat.Get(),
            _whiteBrush.Get());
    }

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = deviceContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }
}

///////////////////////////////////////////////////////////////////////////////
// Graphics handlers.
//

void GestureRecognizerSample::OnLogicalDpiChanged(
    _In_ Platform::Object^)
{
    SetDpi(Windows::Graphics::Display::DisplayProperties::LogicalDpi);
    RequestRedraw();
}

void GestureRecognizerSample::OnDisplayContentsInvalidated(
    _In_ Platform::Object^)
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    RequestRedraw();
}

///////////////////////////////////////////////////////////////////////////////
// Scene helpers.
//

void GestureRecognizerSample::ResetScene()
{
    const float gap = 5.0f;
    const float size = 200.0f;

    _background->Initialize();
    for (unsigned int i=0; i<_objects->Length; ++i)
    {
        int signX = (i % 2) ? 1 : -1;
        int signY = ((i / 2) % 2) ? 1 : -1;
        _objects[i]->Initialize(
            (DrawingObject::Color)((int)DrawingObject::Color::First + (i % (int)DrawingObject::Color::MaxCount)),
            gap * signX, gap * signY,
            size * signX, size * signY);
    }
}

void GestureRecognizerSample::ChangeZOrder(int iFirst)
{
    int iOldPos = -1; // the old position of the iFirst, before it became the first
    for(int i=ARRAYSIZE(_zorder)-1; i > 0; --i)
    {
        if (iFirst == _zorder[i])
        {
            iOldPos = i;
        }
        if (i <= iOldPos)
        {
            _zorder[i] = _zorder[i-1];
        }
    }
    _zorder[0] = iFirst;
}

///////////////////////////////////////////////////////////////////////////////
// IFrameworkView implementation.
//

GestureRecognizerSampleFrameworkView::GestureRecognizerSampleFrameworkView()
:   _activationEntryPoint(ActivationEntryPoint::Unknown),
    _application(nullptr)
{
}

void GestureRecognizerSampleFrameworkView::Initialize(
    _In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView)
{
    // Create application object and store application view
    _application = ref new GestureRecognizerSample(applicationView);
}

void GestureRecognizerSampleFrameworkView::SetWindow(
    _In_ Windows::UI::Core::CoreWindow^ window)
{
    // Store window
    _application->SetWindow(window);
}

void GestureRecognizerSampleFrameworkView::Load(_In_ Platform::String^ entryPoint)
{
    if (entryPoint == "GestureRecognizerSample.App")
    {
        _activationEntryPoint = ActivationEntryPoint::GestureRecognizerSample;
    }
}

void GestureRecognizerSampleFrameworkView::Run()
{
    if (_activationEntryPoint == ActivationEntryPoint::GestureRecognizerSample)
    {
        _application->Run();
    }
    else
    {
        DX::ThrowIfFailed(E_UNEXPECTED);
    }
}

void GestureRecognizerSampleFrameworkView::Uninitialize()
{
    // Must delete the application explicitly, in order to break a circular dependency
    // between it and its contained objects.
    delete _application;
}

///////////////////////////////////////////////////////////////////////////////
// IFrameworkViewSource implementation.
//

Windows::ApplicationModel::Core::IFrameworkView^ GestureRecognizerSampleFrameworkViewSource::CreateView()
{
    return ref new GestureRecognizerSampleFrameworkView();
}

///////////////////////////////////////////////////////////////////////////////
// Application entry point function.
//

// The main entry point for the sample program.
[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto frameworkViewSource = ref new GestureRecognizerSampleFrameworkViewSource();
    Windows::ApplicationModel::Core::CoreApplication::Run(frameworkViewSource);
    return 0;
}
