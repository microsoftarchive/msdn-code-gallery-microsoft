#include "pch.h"
#include "VideoView.h"

using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Input;
using namespace Windows::Storage;

VideoView::VideoView()
{
}

void VideoView::Initialize(
    _In_ Platform::Agile<CoreWindow> window,
    _In_ Platform::Agile<Windows::ApplicationModel::Core::CoreApplicationView> applicationView
    )
{
    m_window = window;
    m_applicationView = applicationView;

    // Specify the cursor type as the standard arrow cursor.
    m_window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    m_window->SizeChanged += 
    ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &VideoView::OnWindowSizeChanged);

    m_window->PointerPressed += 
    ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &VideoView::OnPointerPressed);

    m_window->KeyDown += 
    ref new TypedEventHandler<CoreWindow^, KeyEventArgs^>(this, &VideoView::OnKeyPressed);

    Windows::ApplicationModel::Core::CoreApplication::Resuming += 
	ref new EventHandler<Platform::Object^>(this, &VideoView::OnResuming);

    Windows::ApplicationModel::Core::CoreApplication::Suspending += 
    ref new EventHandler<SuspendingEventArgs^>(this, &VideoView::OnSuspending);

    m_player = ref new MEPlayer();
	m_player->Initialize(m_window.Get());

}

void VideoView::Run()
{	
    // Activate the application window, making it visible and enabling it to receive events.
    m_window->Activate();

    // Open the media file using File Picker
    m_player->OpenFile();

    while (!m_player->ExitApp())
    {        
        //Because of the CPU overhead involved in the video renderering we will only use 
        //ProcessOneAndAllPending for events because it will throttle back the render loop to
        //only execute when an event occurs.
        m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessOneAndAllPending);     		
    }
}

void VideoView::OnWindowSizeChanged(
    _In_ Windows::UI::Core::CoreWindow^ sender,
    _In_ Windows::UI::Core::WindowSizeChangedEventArgs^ args
    )
{
    m_player->UpdateForWindowSizeChange();
}

void VideoView::OnPointerPressed(
    _In_ Windows::UI::Core::CoreWindow^ sender,
    _In_ Windows::UI::Core::PointerEventArgs^ args
    )
{	
    Windows::UI::Input::PointerPoint^ pPoint = args->CurrentPoint;

    if(pPoint)
    {
        Windows::UI::Input::PointerPointProperties^ pPointProperties = pPoint->Properties;

        if(pPointProperties)
        {
            bool fMiddleButtonPressed = pPointProperties->IsMiddleButtonPressed;
            bool fRightButtonPressed = pPointProperties->IsRightButtonPressed;

            if(fMiddleButtonPressed)
            {
                if(m_player->IsPlaying())
                {
                    m_player->Pause();
                }
                else
                {
                    m_player->Play();
                }
            }
            else if(fRightButtonPressed)
            {
                m_player->OpenFile();
            }
        }
    }
}

void VideoView::OnKeyPressed(
    _In_ Windows::UI::Core::CoreWindow^ sender,
    _In_ Windows::UI::Core::KeyEventArgs^ args
    )
{
    Windows::System::VirtualKey vKey = args->VirtualKey;	

    if(VirtualKey::Space == vKey)
    {		
        if(m_player->IsPlaying())
        {
            m_player->Pause();
        }
        else
        {
            m_player->Play();
        }
    }
    else if(VirtualKey::V == vKey)
    {
        m_player->EnableVideoEffect(TRUE);
    }	
    else if(VirtualKey::O == vKey)
    {
        m_player->OpenFile();
    }		
	else if(VirtualKey::L == vKey)
    {
        m_player->Loop();
    }		
}

void VideoView::OnResuming(
    _In_ Platform::Object^ sender, 
    _In_ Platform::Object^ args
    )
{   
    // When this application is suspended, it stores the drawing state.
    // This code attempts to restore the saved state.
    IPropertySet^ set = ApplicationData::Current->LocalSettings->Values;
    // an int called StereoExaggerationFactor is used as a key
    if (set->HasKey("MEPlaybackState"))
    {
        BOOL bMEPlaybackState = (safe_cast<IPropertyValue^>(set->Lookup("MEPlaybackState")))->GetInt32();

        if(bMEPlaybackState)
        {
            m_player->Play();
        }
    }   
    else
    {
        MEDIA::ThrowIfFailed(E_UNEXPECTED);
    }
}

void VideoView::OnSuspending(
    _In_ Platform::Object^ sender, 
    _In_ SuspendingEventArgs^ args
    )
{
    // Save application state.
    // If your application needs time to complete a lengthy operation, it can request a deferral.
    // The SuspendingOperation has a deadline time. Make sure all your operations are complete by that time!
    // If the app doesn't return from this handler within five seconds, it will be terminated.
    SuspendingOperation^ op = args->SuspendingOperation;
    SuspendingDeferral^ deferral = op->GetDeferral();

    // This is also a good time to save your application's state in case the process gets terminated.
    // That way, when the user relaunches the application, they will return to the position they left. 
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;
    if (settingsValues->HasKey("MEPlaybackState"))
    {
        settingsValues->Remove("MEPlaybackState");
    }

    BOOL bMEPlaybackState = m_player->IsPlaying();
    settingsValues->Insert("MEPlaybackState", PropertyValue::CreateInt32(bMEPlaybackState));

    if(bMEPlaybackState)
    {
        m_player->Pause();
    }

    deferral->Complete();
}