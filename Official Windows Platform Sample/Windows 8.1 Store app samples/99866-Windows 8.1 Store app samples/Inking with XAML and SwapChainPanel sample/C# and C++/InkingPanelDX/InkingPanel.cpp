//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"

#include "InkingPanel.h"
#include "DirectXHelper.h"

#include <math.h>
#include <windows.ui.xaml.media.dxinterop.h>

using namespace InkingPanelDX;

//*****************************************************************************
// *NOTE*
//
// When using ink APIs off the UI thread, all ink objects should be consumed
// on a *single* thread. Only non-ink objects should be passed between threads.
//
//*****************************************************************************

#ifdef _DEBUG
#include <assert.h>
#define VERIFY_THREAD(x) { assert((x) == GetCurrentThreadId() && L"Wrong thread"); }
#else
#define VERIFY_THREAD(x)
#endif

InkingPanel::InkingPanel() :
    m_backgroundColor(D2D1::ColorF(
        static_cast<float>(29.0/255),
        static_cast<float>(29.0/255),
        static_cast<float>(29.0/255))),
    m_strokeColor(D2D1::ColorF(1.0,1.0,1.0)),
    m_strokeSize(5.0f),
    m_coreInput(nullptr),
    m_inkManager(nullptr),
    m_activePointerId(-1)
{
    CreateDeviceIndependentResources();
    CreateDeviceResources();
    CreateSizeDependentResources();
}

void InkingPanel::ProcessInputOnDelegateThread()
{
    // Define delegate thread function
    auto delegateInputHandler = ref new Windows::System::Threading::WorkItemHandler(
        [this](Windows::Foundation::IAsyncAction^)
    {
        m_coreInput = CreateCoreIndependentInputSource(
            Windows::UI::Core::CoreInputDeviceTypes::Mouse |
            Windows::UI::Core::CoreInputDeviceTypes::Pen);

        // Hook up pointer event handlers
        m_coreInput->PointerPressed += ref new Windows::Foundation::TypedEventHandler<
            Platform::Object^, Windows::UI::Core::PointerEventArgs^>(
                this, 
                &InkingPanel::OnPointerPressed);
        m_coreInput->PointerMoved += ref new Windows::Foundation::TypedEventHandler<
            Platform::Object^, 
            Windows::UI::Core::PointerEventArgs^>(
                this, 
                &InkingPanel::OnPointerMoved);
        m_coreInput->PointerReleased += ref new Windows::Foundation::TypedEventHandler<
            Platform::Object^, 
            Windows::UI::Core::PointerEventArgs^>(
                this, &InkingPanel::OnPointerReleased);

        // Windows::UI::Input::Inking objects *should* be used on a single thread.
        // Create them here since this is the thread that processes input and creates the strokes. 

        auto drawingAttributes = ref new Windows::UI::Input::Inking::InkDrawingAttributes();
        drawingAttributes->Color = DX::ConvertToColor(m_strokeColor);
        drawingAttributes->Size = Windows::Foundation::Size(m_strokeSize, m_strokeSize);

        m_inkManager = ref new Windows::UI::Input::Inking::InkManager();
        m_inkManager->SetDefaultDrawingAttributes(drawingAttributes);

#ifdef _DEBUG
        m_coreInputThreadId = GetCurrentThreadId();
#endif

        m_coreInput->Dispatcher->ProcessEvents(
            Windows::UI::Core::CoreProcessEventsOption::ProcessUntilQuit);
    });

    // Create and run the delegate thread that processes input
    Windows::System::Threading::ThreadPool::RunAsync(
        delegateInputHandler,
        Windows::System::Threading::WorkItemPriority::High, 
        Windows::System::Threading::WorkItemOptions::TimeSliced);
}

void InkingPanel::Clear()
{
    // Use m_coreInput's dispatcher to update Windows::UI::Input::Inking objects
    m_coreInput->Dispatcher->RunAsync(
        Windows::UI::Core::CoreDispatcherPriority::High,
        ref new Windows::UI::Core::DispatchedHandler([this]()
    {
        OnClear();
    }));
}

// *should* be executed on the same thread that created m_inkManager and all its inking objects
void InkingPanel::OnClear()
{
    VERIFY_THREAD (m_coreInputThreadId);

    auto strokes = m_inkManager->GetStrokes();
    for (unsigned int i = 0; i < strokes->Size; i++)
    {
        auto stroke = strokes->GetAt(i);
        stroke->Selected = true;
    }
    m_inkManager->DeleteSelected();
    OnRender();
}

bool InkingPanel::IsErase(Windows::UI::Core::PointerEventArgs^ e)
{
    auto pointerDevice = e->CurrentPoint->PointerDevice;
    auto pointerProperties = e->CurrentPoint->Properties;

    bool rval = e->CurrentPoint->IsInContact;
    rval &= ((pointerProperties->IsEraser)
         ||  (pointerDevice->PointerDeviceType == Windows::Devices::Input::PointerDeviceType::Pen && 
              !pointerProperties->IsBarrelButtonPressed && pointerProperties->IsRightButtonPressed)
         ||  (pointerDevice->PointerDeviceType == Windows::Devices::Input::PointerDeviceType::Mouse &&
              pointerProperties->IsRightButtonPressed &&
              e->KeyModifiers == Windows::System::VirtualKeyModifiers::None));

    return rval;
}

bool InkingPanel::IsInk(Windows::UI::Core::PointerEventArgs^ e)
{
    auto pointerDevice = e->CurrentPoint->PointerDevice;
    auto pointerProperties = e->CurrentPoint->Properties;

    bool rval = e->CurrentPoint->IsInContact;
    rval &= ((pointerDevice->PointerDeviceType == Windows::Devices::Input::PointerDeviceType::Pen && 
              !pointerProperties->IsBarrelButtonPressed && !pointerProperties->IsRightButtonPressed)
         ||  (pointerDevice->PointerDeviceType == Windows::Devices::Input::PointerDeviceType::Mouse && 
              pointerProperties->IsLeftButtonPressed && 
              e->KeyModifiers == Windows::System::VirtualKeyModifiers::None));

    return rval;
}

// *should* be executed on the same thread that created m_inkManager and all its inking objects
void InkingPanel::OnPointerPressed(
    Platform::Object^ sender, Windows::UI::Core::PointerEventArgs^ e)
{
    VERIFY_THREAD (m_coreInputThreadId);

    // Ignore the event if another pointer is already active
    if (m_activePointerId == -1)
    {
        if (IsErase(e))
        {
            m_inkManager->Mode = Windows::UI::Input::Inking::InkManipulationMode::Erasing;
        }
        else if (IsInk(e))
        {
            m_inkManager->Mode = Windows::UI::Input::Inking::InkManipulationMode::Inking;
        }
        else
        {
            return; // Ignore the event
        }
        m_inkManager->ProcessPointerDown(e->CurrentPoint);
        m_activePointerId = (int)e->CurrentPoint->PointerId;
    }
}

// *should* be executed on the same thread that created m_inkManager and all its inking objects
void InkingPanel::OnPointerMoved(
    Platform::Object^ sender, Windows::UI::Core::PointerEventArgs^ e)
{
    VERIFY_THREAD (m_coreInputThreadId);

    // Make sure the event belongs to the pointer that is currently active
    if (m_activePointerId == (int)e->CurrentPoint->PointerId)
    {
        if (m_inkManager->Mode == Windows::UI::Input::Inking::InkManipulationMode::Erasing)
        {
            // In erase mode InkManager::ProcessPointerUpdate returns an invalidate
            // rectangle: if it is not degenerate something has been erased           
            auto invalidateRect = (Windows::Foundation::Rect) 
                m_inkManager->ProcessPointerUpdate(e->CurrentPoint);
            if (invalidateRect.Height != 0 && invalidateRect.Width != 0)
            {
                OnRender();
            }
        }
        else
        {
            // In inking mode InkManager::ProcessPointerUpdate returns the last
            // point that was processed by the InkManager
            // NOTE: for smoother render, process all intermediate points returned
            // by Windows::UI::Core::PointerEventArgs::GetIntermediatePoints
            auto previousPoint = (Windows::Foundation::Point)
                m_inkManager->ProcessPointerUpdate(e->CurrentPoint);

            HRESULT hr = S_OK;
            do
            {
                // Copy the content of the last presented buffer: it contains the Beziers
                // as well as the lines we rendered at previous pointer moves.
                m_d3dContext->CopyResource(m_currentBuffer.Get(), m_previousBuffer.Get());

                // Render line from previous point to current point
                // doesn't render all intermediate points
                m_d2dContext->BeginDraw();
                m_d2dContext->DrawLine(
                    D2D1::Point2F(previousPoint.X, previousPoint.Y),
                    D2D1::Point2F(e->CurrentPoint->Position.X, e->CurrentPoint->Position.Y),
                    m_strokeBrush.Get(),
                    m_strokeSize,
                    m_inkStrokeStyle.Get()
                    );
                hr = m_d2dContext->EndDraw();
                if (hr == D2DERR_RECREATE_TARGET)
                {
                    CreateSizeDependentResources();
                }
            } 
            while (hr == D2DERR_RECREATE_TARGET);
            DX::ThrowIfFailed(hr);

            Present();
        }
    }
}

// *should* be executed on the same thread that created m_inkManager and all its inking objects
void InkingPanel::OnPointerReleased(
    Platform::Object^ sender, Windows::UI::Core::PointerEventArgs^ e)
{
    VERIFY_THREAD (m_coreInputThreadId);

    // Make sure the event belongs to the pointer that is currently active
    if (m_activePointerId == (int) e->CurrentPoint->PointerId)
    {
        m_activePointerId = -1;

        m_inkManager->ProcessPointerUp(e->CurrentPoint);
        OnRender();

        OnRecognize();
    }
}

// *should* be executed on the same thread that created m_inkManager and all its inking objects
void InkingPanel::OnRecognize()
{
    VERIFY_THREAD (m_coreInputThreadId);

    // The following call to RecognizeAsync may fail for various reasons, most notably if another recognition is in progress
    try
    {
        auto recognizeOp = m_inkManager->RecognizeAsync(Windows::UI::Input::Inking::InkRecognitionTarget::All);
        recognizeOp->Completed = ref new Windows::Foundation::AsyncOperationCompletedHandler<Windows::Foundation::Collections::IVectorView<Windows::UI::Input::Inking::InkRecognitionResult^>^>([this](
            Windows::Foundation::IAsyncOperation<Windows::Foundation::Collections::IVectorView<Windows::UI::Input::Inking::InkRecognitionResult^>^>^ asyncOp,
            Windows::Foundation::AsyncStatus /*asyncStatus*/)
        {
            Windows::Foundation::Collections::IVectorView<Windows::UI::Input::Inking::InkRecognitionResult^>^ recognitionResults = asyncOp->GetResults();

            // See if we find a single digit number within the text candidates
            // We are expecting one number so we only process first recognition result
            Windows::Foundation::Collections::IVectorView<Platform::String^> ^
                textCandidates = recognitionResults->GetAt(0)->GetTextCandidates();
            for (unsigned int i = 0; i < textCandidates->Size; i++)
            {
                Platform::String^ candidate = textCandidates->GetAt(i);
                if (candidate == L"0" || candidate == L"1" || candidate == L"2" || candidate == L"3" || candidate == L"4" ||
                    candidate == L"5" || candidate == L"6" || candidate == L"7" || candidate == L"8" || candidate == L"9")
                {
                    // Use the UI core dispatcher to update UI thread
                    Dispatcher->RunAsync(
                        Windows::UI::Core::CoreDispatcherPriority::High,
                        ref new Windows::UI::Core::DispatchedHandler([this, candidate]()
                    {
                        InkAnswerUpdated(this, candidate);
                    }));
                    break;
                }
            }
        });
    }
    catch (Platform::COMException^)
    {
        // RecognizeAsync returned an error, most likely because there was another recognition in progress
    }
}

// *should* be executed on the same thread that created m_inkManager and all its inking objects
void InkingPanel::OnRender()
{
    VERIFY_THREAD (m_coreInputThreadId);

    HRESULT hr = S_OK;
    do
    {
        m_d2dContext->BeginDraw();
        m_d2dContext->Clear(m_backgroundColor);
        RenderStrokes();
        HRESULT hr = m_d2dContext->EndDraw();
        if (hr == D2DERR_RECREATE_TARGET)
        {
            CreateSizeDependentResources();
        }
    }
    while (hr == D2DERR_RECREATE_TARGET);
    DX::ThrowIfFailed(hr);

    Present();
}

// *should* be executed on the same thread that created m_inkManager and all its inking objects
void InkingPanel::RenderStrokes()
{
    VERIFY_THREAD (m_coreInputThreadId);

    if (m_inkManager)
    {
        Windows::Foundation::Collections::IVectorView<Windows::UI::Input::Inking::InkStroke^>^
            strokes = m_inkManager->GetStrokes();

        for (unsigned int i = 0; i < strokes->Size; i++)
        {
            Windows::UI::Input::Inking::InkStroke^ stroke = strokes->GetAt(i);

            Microsoft::WRL::ComPtr<ID2D1PathGeometry> strokeGeometry;
            CreateStrokeGeometry(stroke, &strokeGeometry);

            // Create brush from current stroke
            Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> strokeBrush;
            DX::ThrowIfFailed(
                m_d2dContext->CreateSolidColorBrush(
                DX::ConvertToColorF(stroke->DrawingAttributes->Color),
                &strokeBrush)
                );

            m_d2dContext->DrawGeometry(
                strokeGeometry.Get(),
                strokeBrush.Get(),
                stroke->DrawingAttributes->Size.Width,
                m_inkStrokeStyle.Get());
        }
    }
}

// *should* be executed on the same thread that created the stroke
void InkingPanel::CreateStrokeGeometry(
    Windows::UI::Input::Inking::InkStroke^ stroke,
    ID2D1PathGeometry** geometry)
{
    VERIFY_THREAD (m_coreInputThreadId);

    // Create geometry path
    DX::ThrowIfFailed(
        m_d2dFactory->CreatePathGeometry(geometry)
    );

    // Create and initialize geometry sink
    Microsoft::WRL::ComPtr<ID2D1GeometrySink> sink;
    DX::ThrowIfFailed(
        (*geometry)->Open(&sink)
    );

    sink->SetSegmentFlags(D2D1_PATH_SEGMENT_FORCE_ROUND_LINE_JOIN);
    sink->SetFillMode(D2D1_FILL_MODE_ALTERNATE);

    Windows::Foundation::Collections::IVectorView<
        Windows::UI::Input::Inking::InkStrokeRenderingSegment^>^
            strokeSegments = stroke->GetRenderingSegments();

    // Set starting point to position of first segment
    sink->BeginFigure(
        DX::ConvertToPoint2F(strokeSegments->GetAt(0)->Position), 
        D2D1_FIGURE_BEGIN_FILLED);

    for (unsigned int i = 1; i < strokeSegments->Size; i++)
    {
        Windows::UI::Input::Inking::InkStrokeRenderingSegment^
            strokeSegment = strokeSegments->GetAt(i);         
        
        // Add segment to geometry path sink
        sink->AddBezier(
            D2D1::BezierSegment(
                DX::ConvertToPoint2F(strokeSegment->BezierControlPoint1),
                DX::ConvertToPoint2F(strokeSegment->BezierControlPoint2),
                DX::ConvertToPoint2F(strokeSegment->Position)
                )
            );
    }
    
    sink->EndFigure(D2D1_FIGURE_END_OPEN);
    DX::ThrowIfFailed(
        sink->Close()
    );
}

#pragma region DirectXPanelBase overrides

void InkingPanel::CreateDeviceIndependentResources()
{
    DirectXPanelBase::CreateDeviceIndependentResources();

    DX::ThrowIfFailed(
        m_d2dFactory->CreateStrokeStyle(
            D2D1::StrokeStyleProperties(
                D2D1_CAP_STYLE_ROUND,
                D2D1_CAP_STYLE_ROUND,
                D2D1_CAP_STYLE_ROUND,
                D2D1_LINE_JOIN_ROUND,
                1.0f,
                D2D1_DASH_STYLE_SOLID,
                0.0f),
            nullptr,
            0,
            &m_inkStrokeStyle)
    );
}

void InkingPanel::CreateDeviceResources()
{
    DirectXPanelBase::CreateDeviceResources();

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(m_strokeColor, &m_strokeBrush)
    );    
}

void InkingPanel::CreateSizeDependentResources()
{
    DirectXPanelBase::CreateSizeDependentResources();

    DX::ThrowIfFailed(
        m_swapChain->GetBuffer(
            0,
            __uuidof(ID3D11Texture2D),
            &m_currentBuffer
            )
    );
        
    DX::ThrowIfFailed(
        m_swapChain->GetBuffer(
            1,
            __uuidof(ID3D11Texture2D),
            &m_previousBuffer
            )
    );
}

void InkingPanel::ReleaseSizeDependentResources()
{
    DirectXPanelBase::ReleaseSizeDependentResources();

    m_currentBuffer = nullptr;
    m_previousBuffer = nullptr;
}

void InkingPanel::Render()
{
    // Use m_coreInput's dispatcher to access Windows::UI::Input::Inking objects
    m_coreInput->Dispatcher->RunAsync(
        Windows::UI::Core::CoreDispatcherPriority::High,
        ref new Windows::UI::Core::DispatchedHandler([this]()
    {
        VERIFY_THREAD(m_coreInputThreadId);
        OnRender();
    }));
}

void InkingPanel::SetSwapChain()
{
    // The swap chain must be associated on the UI thread
    Dispatcher->RunAsync(
        Windows::UI::Core::CoreDispatcherPriority::High,
        ref new Windows::UI::Core::DispatchedHandler([this]()
    {
        DirectXPanelBase::SetSwapChain();
    }));
}

void InkingPanel::OnSuspending(Object^ sender, Windows::ApplicationModel::SuspendingEventArgs^ e)
{
    // This event arrives on the UI thread. Handle it on the core dispatcher thread to 
    // synchronize access to DX resources which are not thread safe.
    m_coreInput->Dispatcher->RunAsync(
        Windows::UI::Core::CoreDispatcherPriority::High,
        ref new Windows::UI::Core::DispatchedHandler([this]()
    {
        VERIFY_THREAD(m_coreInputThreadId);
        Microsoft::WRL::ComPtr<IDXGIDevice3> dxgiDevice;
        m_d3dDevice.As(&dxgiDevice);

        // Hints to the driver that the app is entering an idle state and that its memory can be used temporarily for other apps.
        dxgiDevice->Trim();
    }));
}

void InkingPanel::OnSizeChanged(Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e)
{
    // This event arrives on the UI thread. Cache XAML properties here since they can only
    // be accesed from UI thread but handle the rest of the event on the core dispatcher
    // thread to synchronize access to DX resources which are not thread safe.

    m_width = (float) this->Width;
    m_height = (float) this->Height;
    m_compositionScaleX = this->CompositionScaleX;
    m_compositionScaleY = this->CompositionScaleY;

    m_coreInput->Dispatcher->RunAsync(
        Windows::UI::Core::CoreDispatcherPriority::High,
        ref new Windows::UI::Core::DispatchedHandler([this]()
    {
        VERIFY_THREAD(m_coreInputThreadId);

        CreateSizeDependentResources();
        OnRender();
    }));
}

void InkingPanel::OnCompositionScaleChanged(SwapChainPanel^ sender, Object^ args)
{
    // This event arrives on the UI thread. Cache XAML properties here since they can only
    // be accesed from UI thread but handle the rest of the event on the core dispatcher
    // thread to synchronize access to DX resources which are not thread safe.

    m_width = (float) this->Width;
    m_height = (float) this->Height;
    m_compositionScaleX = this->CompositionScaleX;
    m_compositionScaleY = this->CompositionScaleY;

    m_coreInput->Dispatcher->RunAsync(
        Windows::UI::Core::CoreDispatcherPriority::High,
        ref new Windows::UI::Core::DispatchedHandler([this]()
    {
        VERIFY_THREAD(m_coreInputThreadId);

        CreateSizeDependentResources();
        OnRender();
    }));
}

#pragma endregion
