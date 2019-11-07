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

using namespace SDKSample::PlaybackManager;

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

PlaybackControl::PlaybackControl()
{
    InitializeComponent();

    cw = Window::Current->CoreWindow;
    IsPlaying = false;
    systemMediaControls = SystemMediaTransportControls::GetForCurrentView();

    // add handlers and save token
    PropertyChangedToken = systemMediaControls->PropertyChanged += ref new TypedEventHandler<SystemMediaTransportControls^, SystemMediaTransportControlsPropertyChangedEventArgs^>(this, &PlaybackControl::SystemMediaControls_PropertyChanged);
    ButtonPressedToken = systemMediaControls->ButtonPressed += ref new TypedEventHandler<SystemMediaTransportControls^, SystemMediaTransportControlsButtonPressedEventArgs^>(this, &PlaybackControl::SystemMediaControls_ButtonPressed);
    systemMediaControls->IsPlayEnabled = true;
    systemMediaControls->IsPauseEnabled = true;
    systemMediaControls->IsStopEnabled = true;
    systemMediaControls->PlaybackStatus = MediaPlaybackStatus::Closed;
}

PlaybackControl::~PlaybackControl()
{
    if (systemMediaControls)
    {
        // remove handlers
        systemMediaControls->PropertyChanged -= PropertyChangedToken;
        systemMediaControls->ButtonPressed -= ButtonPressedToken;
    }
}

void PlaybackControl::Play()
{
    cw->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([= ]()
    {
        OutputMedia->Play();
        systemMediaControls->PlaybackStatus = MediaPlaybackStatus::Playing;
    }
    , CallbackContext::Any));
}


void PlaybackControl::Pause()
{
    cw->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([= ]()
    {
        OutputMedia->Pause();
        systemMediaControls->PlaybackStatus = MediaPlaybackStatus::Paused;
    }
    , CallbackContext::Any));
}


void PlaybackControl::Stop()
{
    cw->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([= ]()
    {
        OutputMedia->Stop();
        systemMediaControls->PlaybackStatus = MediaPlaybackStatus::Stopped;
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
                        cw->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([= ]()
                        {
                            OutputMedia->SetSource(stream, file->ContentType);
                        }
                        , CallbackContext::Any));
                    }
                });
            }

        });

    }
    catch (Exception^ ex)
    {
        DisplayStatus(ex->ToString());
    }
}


void PlaybackControl::DisplayStatus(Platform::String^ text)
{
    cw->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([= ]()
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
    cw->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([= ]()
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
    cw->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([= ]()
    {
        if (IsPlaying)
        {
            Play();
        }
    }
    , CallbackContext::Any));
    DisplayStatus(GetTimeStampedMessage("Unmuted"));
}


void PlaybackControl::SystemMediaControls_PropertyChanged(SystemMediaTransportControls^ sender, SystemMediaTransportControlsPropertyChangedEventArgs^ e)
{
    if (e->Property == SystemMediaTransportControlsProperty::SoundLevel)
    {
        auto soundLevelState = sender->SoundLevel;

        DisplayStatus(GetTimeStampedMessage("App sound level is [" + SoundLevelToString(soundLevelState) + "]"));
        if (soundLevelState == SoundLevel::Muted)
        {
            AppMuted();
        }
        else
        {
            AppUnmuted();
        }
    }
}


void PlaybackControl::SystemMediaControls_ButtonPressed(SystemMediaTransportControls ^ /* sender */, SystemMediaTransportControlsButtonPressedEventArgs^ e)
{
    switch (e->Button)
    {
    case SystemMediaTransportControlsButton::Play:
        Play();
        DisplayStatus(GetTimeStampedMessage("Play Pressed"));
        break;

    case SystemMediaTransportControlsButton::Pause:
        Pause();
        DisplayStatus(GetTimeStampedMessage("Pause Pressed"));
        break;

    case SystemMediaTransportControlsButton::Stop:
        Stop();
        DisplayStatus(GetTimeStampedMessage("Stop Pressed"));
        break;

    default:
        break;
    }
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
