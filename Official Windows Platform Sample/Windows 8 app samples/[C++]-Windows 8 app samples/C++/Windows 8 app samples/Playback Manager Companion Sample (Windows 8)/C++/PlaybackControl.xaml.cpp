//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// PlaybackControl.xaml.cpp
// Implementation of the PlaybackControl class
//

#include "pch.h"
#include "PlaybackControl.xaml.h"

using namespace SDKSample::PlaybackManager2;

using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::Media;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage::Streams;
using namespace Platform;
using namespace Concurrency;

bool PlaybackControl::IsRegistered = false;
EventRegistrationToken PlaybackControl::SoundLevelChangedToken;
EventRegistrationToken PlaybackControl::PlayPauseTogglePressedToken;
EventRegistrationToken PlaybackControl::PlayPressedToken;
EventRegistrationToken PlaybackControl::PausePressedToken;
EventRegistrationToken PlaybackControl::StopPressedToken;

PlaybackControl::PlaybackControl()
{
    InitializeComponent();

    cw = Window::Current->CoreWindow;
    IsPlaying = false;
    if(IsRegistered)
    {
        // remove handlers
        MediaControl::SoundLevelChanged      -= SoundLevelChangedToken;
        MediaControl::PlayPauseTogglePressed -= PlayPauseTogglePressedToken;
        MediaControl::PlayPressed            -= PlayPressedToken;
        MediaControl::PausePressed           -= PausePressedToken;
        MediaControl::StopPressed            -= StopPressedToken;
    }
    // add handlers and save token
    SoundLevelChangedToken      = MediaControl::SoundLevelChanged      += ref new EventHandler<Object^>(this, &PlaybackControl::MediaControl_SoundLevelChanged);
    PlayPauseTogglePressedToken = MediaControl::PlayPauseTogglePressed += ref new EventHandler<Object^>(this, &PlaybackControl::MediaControl_PlayPauseTogglePressed);
    PlayPressedToken            = MediaControl::PlayPressed            += ref new EventHandler<Object^>(this, &PlaybackControl::MediaControl_PlayPressed);
    PausePressedToken           = MediaControl::PausePressed           += ref new EventHandler<Object^>(this, &PlaybackControl::MediaControl_PausePressed);
    StopPressedToken            = MediaControl::StopPressed            += ref new EventHandler<Object^>(this, &PlaybackControl::MediaControl_StopPressed);

    IsRegistered = true;

    MediaControl::IsPlaying = false;
}


void PlaybackControl::Play()
{
    cw->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
    {
        OutputMedia->Play();
        MediaControl::IsPlaying = true;
    }
    , CallbackContext::Any)); 
}


void PlaybackControl::Pause()
{
    cw->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
    {
        OutputMedia->Pause();
        MediaControl::IsPlaying = false;
    }
    , CallbackContext::Any)); 
}


void PlaybackControl::Stop()
{
    cw->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
    {
        OutputMedia->Stop();
        MediaControl::IsPlaying = false;
    }
    , CallbackContext::Any)); 
}


void PlaybackControl::SetAudioCategory(AudioCategory category)
{
    OutputMedia->AudioCategory = category;
}


void PlaybackControl::SetAudioDeviceType(AudioDeviceType devicetype)
{
    OutputMedia->AudioDeviceType = devicetype;
}


void PlaybackControl::SelectFile()
{
    try
    {
        FileOpenPicker^ picker = ref new FileOpenPicker();
        picker->ViewMode = PickerViewMode::List;
        picker->SuggestedStartLocation = PickerLocationId::MusicLibrary;
        picker->FileTypeFilter->Append(".mp3");
        picker->FileTypeFilter->Append(".mp4");
        picker->FileTypeFilter->Append(".m4a");
        picker->FileTypeFilter->Append(".wma");
        picker->FileTypeFilter->Append(".wav");

		create_task(picker->PickSingleFileAsync()).then([this](StorageFile^ file)
		{
			if (file)
            {
                create_task(file->OpenAsync(FileAccessMode::Read)).then([this, file](Streams::IRandomAccessStream^ stream)
                {
                    if (stream)
					{
						cw->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
						{
							OutputMedia->SetSource(stream, file->ContentType);
						}
						, CallbackContext::Any)); 
					}
                });
            }
        });

    }
    catch(Exception^ ex)
    {
        DisplayStatus(ex->ToString());
    }
}


void PlaybackControl::DisplayStatus(Platform::String^ text)
{
    cw->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
    {
        Status->Text += text + "\n";
    }
    , CallbackContext::Any)); 
}


String^ PlaybackControl::GetTimeStampedMessage(String^ EventCalled)
{
    Platform::String^ message;
    Windows::Foundation::DateTime timestamp = Windows::Foundation::DateTime();

    Windows::Globalization::DateTimeFormatting::DateTimeFormatter^ formatter = ref new Windows::Globalization::DateTimeFormatting::DateTimeFormatter("longtime");
    Windows::Globalization::Calendar^ calendar = ref new Windows::Globalization::Calendar();
    calendar->SetToNow();

    message = EventCalled + "   " + formatter->Format(calendar->GetDateTime());

    return message;
}

	//If your app is playing media you feel that a user should not miss if a VOIP call comes in, you may
	//want to consider pausing playback when your app receives a SoundLevel(Low) notification.
	//A SoundLevel(Low) means your app volume has been attenuated by the system (likely for a VOIP call).


String^ PlaybackControl::SoundLevelToString(SoundLevel level)
{
    String^ LevelString;

    switch (level)
    {
    case Windows::Media::SoundLevel::Muted:
        LevelString = "Muted";
        break;
    case Windows::Media::SoundLevel::Low:
        LevelString = "Low";
        break;
    case Windows::Media::SoundLevel::Full:
        LevelString = "Full";
        break;
    default:
        LevelString = "Unknown";
        break;
    }

    return LevelString;
}


void PlaybackControl::AppMuted()
{
    cw->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
    {
        if (OutputMedia->CurrentState != MediaElementState::Paused)
        {
            IsPlaying = true;
            Pause();
        }
        else
        {
            IsPlaying = false;
        }
    }
    , CallbackContext::Any)); 
    DisplayStatus(GetTimeStampedMessage("Muted"));
}


void PlaybackControl::AppUnmuted()
{
    cw->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
    {
        if (IsPlaying)
        {
            Play();
        }
    }
    , CallbackContext::Any)); 
    DisplayStatus(GetTimeStampedMessage("Unmuted"));
}


void PlaybackControl::MediaControl_SoundLevelChanged(Object^ sender, Object^ e)
{
    auto soundLevelState = MediaControl::SoundLevel;

    DisplayStatus(GetTimeStampedMessage("App sound level is ["+SoundLevelToString(soundLevelState)+"]"));
    if(soundLevelState == SoundLevel::Muted)
    {
        AppMuted();
    }
    else
    {
        AppUnmuted();	
    }
}


void PlaybackControl::MediaControl_PlayPauseTogglePressed(Object^ sender, Object^ e)
{
    if(MediaControl::IsPlaying)
    {
        Pause();
        DisplayStatus(GetTimeStampedMessage("Play/Pause Pressed - Pause"));
    }
    else
    {
        Play();
        DisplayStatus(GetTimeStampedMessage("Play/Pause Pressed - Play"));
    }
}


void PlaybackControl::MediaControl_PlayPressed(Object^ sender, Object^ e)
{
    Play();
    DisplayStatus(GetTimeStampedMessage("Play Pressed"));
}


void PlaybackControl::MediaControl_PausePressed(Object^ sender, Object^ e)
{
    Pause();
    DisplayStatus(GetTimeStampedMessage("Pause Pressed"));
}


void PlaybackControl::MediaControl_StopPressed(Object^ sender, Object^ e)
{
    Stop();
    DisplayStatus(GetTimeStampedMessage("Stop Pressed"));
}


void PlaybackControl::Button_Play_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Play();
}


void PlaybackControl::Button_Pause_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Pause();
}


void PlaybackControl::Button_Stop_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Stop();
}
