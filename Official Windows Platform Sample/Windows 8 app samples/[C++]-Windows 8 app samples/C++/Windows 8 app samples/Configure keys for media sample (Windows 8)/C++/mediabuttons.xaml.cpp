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
// mediabuttons.xaml.cpp
// Implementation of the mediabuttons class
//

#include "pch.h"
#include "mediabuttons.xaml.h"

using namespace SDKSample::MediaButtons;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Media;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Platform;
using namespace Windows::Storage::Pickers;
using namespace Windows::Foundation::Collections;
using namespace Concurrency;

mediabuttons::mediabuttons()
{
    InitializeComponent();
    _dispatcher = Window::Current->Dispatcher;
    _wasPlaying = false;
    _currentSongIndex = 0;
    _playlistCount = 0;
    _prevTrackPressedEventToken.Value = 0;
    _nextTrackPressedEventToken.Value = 0;
    _playlist = ref new Platform::Collections::Vector<Song^>();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void mediabuttons::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    if (IsInitialized)
    {
      return;
    }

    PlayButton->Click += ref new RoutedEventHandler(this, &mediabuttons::Play_Click);

    MediaControl::PlayPauseTogglePressed += ref new EventHandler<Platform::Object^>(this, &mediabuttons::PlayPause);
    MediaControl::PlayPressed += ref new EventHandler<Platform::Object^>(this, &mediabuttons::Play);
    MediaControl::StopPressed += ref new EventHandler<Platform::Object^>(this, &mediabuttons::Stop);
    MediaControl::PausePressed += ref new EventHandler<Platform::Object^>(this, &mediabuttons::Pause);
    MediaControl::FastForwardPressed += ref new EventHandler<Platform::Object^>(this, &mediabuttons::FastForward);
    MediaControl::RewindPressed += ref new EventHandler<Platform::Object^>(this, &mediabuttons::Rewind);
    MediaControl::ChannelUpPressed += ref new EventHandler<Platform::Object^>(this, &mediabuttons::ChannelUp);
    MediaControl::ChannelDownPressed += ref new EventHandler<Platform::Object^>(this, &mediabuttons::ChannelDown);
    MediaControl::RecordPressed += ref new EventHandler<Platform::Object^>(this, &mediabuttons::Record);

    OutputMediaElement->MediaOpened += ref new RoutedEventHandler(this, &mediabuttons::MediaElement_MediaOpened);
    OutputMediaElement->MediaEnded += ref new RoutedEventHandler(this, &mediabuttons::MediaElement_MediaEnded);
    OutputMediaElement->CurrentStateChanged += ref new RoutedEventHandler(this, &mediabuttons::MediaElement_CurrentStateChanged);

    DisplayStatus("Media Buttons Initialized");
    IsInitialized = true;

}

void mediabuttons::SelectFiles_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        FileOpenPicker^ picker = ref new FileOpenPicker();
        picker->ViewMode = PickerViewMode::List;
        picker->SuggestedStartLocation = PickerLocationId::MusicLibrary;
        picker->FileTypeFilter->Append(".mp3");
        picker->FileTypeFilter->Append(".m4a");
        picker->FileTypeFilter->Append(".wma");


		create_task(picker->PickMultipleFilesAsync()).then([this](IVectorView<StorageFile^>^ files)
		{
			if(files->Size > 0)
			{
				CreatePlaylist(files);
				SetCurrentPlayingAsync(_currentSongIndex);
			}
		});
		
    }
    catch(Exception^ ex)
    {
        rootPage->NotifyUser(ex->ToString(), NotifyType::ErrorMessage);
    }
}


void mediabuttons::Play_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    PlayPause(sender, e);
}


void mediabuttons::SetMediaElementSourceAsync(Song^ song)
{
    if (song)
    {
        create_task(song->File->OpenAsync(FileAccessMode::Read)).then([this, song](Streams::IRandomAccessStream^ stream)
		{
			OutputMediaElement->SetSource(stream, song->File->ContentType);
            MediaControl::ArtistName = song->Artist;
            MediaControl::TrackName  = song->Track;
		});
		
		
    }
}


void mediabuttons::CreatePlaylist(IVectorView<StorageFile^>^ files)
{
    _playlistCount = files->Size;

    if (_prevTrackPressedEventToken.Value != 0)
    {
        MediaControl::PreviousTrackPressed -= _prevTrackPressedEventToken;
        _prevTrackPressedEventToken.Value = 0;
    }
    if (_nextTrackPressedEventToken.Value != 0)
    {
        MediaControl::NextTrackPressed -= _nextTrackPressedEventToken;
        _nextTrackPressedEventToken.Value = 0;
    }

    _currentSongIndex = 0;
    _playlist->Clear();

    if (files->Size > 0)
    {
        // Application now has read/write access to the picked file(s) 
        if (files->Size > 1)
        {
            _nextTrackPressedEventToken = MediaControl::NextTrackPressed += ref new EventHandler<Platform::Object^>(this, &mediabuttons::NextTrack, CallbackContext::Same);
        }

        // Create the playlist
            
        //for each (StorageFile^ file in files)
        std::for_each(begin(files), end(files), [this](StorageFile^ file)
        {
            Song^ newSong = ref new Song(file);
            _playlist->Append(newSong);
        });
      }

    return;
}


void mediabuttons::SetCurrentPlayingAsync(int playlistIndex)
{
    _wasPlaying = MediaControl::IsPlaying;

    try
    {
        Song^ currSong = _playlist->GetAt(playlistIndex);
        create_task(currSong->File->OpenAsync(FileAccessMode::Read)).then([this, currSong, playlistIndex](Streams::IRandomAccessStream^ stream)
        {
            OutputMediaElement->SetSource(stream, currSong->File->ContentType);
            create_task(currSong->File->Properties->GetMusicPropertiesAsync()).then([this, currSong](Windows::Storage::FileProperties::MusicProperties^ properties)
            {
                currSong->Artist  = properties->Artist;
                currSong->Track   = properties->Title;
                MediaControl::ArtistName = currSong->Artist;
                MediaControl::TrackName  = currSong->Track;
            });
          
        });
      
    }
    catch(Exception^ e)
    {
        DisplayStatus(GetTimeStampedMessage(e->Message));
    }
}


void mediabuttons::DisplayStatus(Platform::String^ text)
{
    _dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
    {
        rootPage->NotifyUser(text, NotifyType::StatusMessage);
    }
    , CallbackContext::Any)); 
}


Platform::String^ mediabuttons::GetTimeStampedMessage(Platform::String^ EventCalled)
{
    Platform::String^ message;
    Windows::Foundation::DateTime timestamp = Windows::Foundation::DateTime();

    Windows::Globalization::DateTimeFormatting::DateTimeFormatter^ formatter = ref new Windows::Globalization::DateTimeFormatting::DateTimeFormatter("longtime");
    Windows::Globalization::Calendar^ calendar = ref new Windows::Globalization::Calendar();
    calendar->SetToNow();

    message = EventCalled + "   " + formatter->Format(calendar->GetDateTime());

    return message;
}


void mediabuttons::MediaElement_MediaEnded(Object^ sender, RoutedEventArgs^ e)
{
    if (_currentSongIndex < _playlistCount - 1)
    {
        _currentSongIndex++;

        SetCurrentPlayingAsync(_currentSongIndex);

        if (_wasPlaying)
        {
            OutputMediaElement->Play();
        }
    }
}


void mediabuttons::MediaElement_MediaOpened(Object^ sender, RoutedEventArgs^ e)
{
    if (_wasPlaying)
    {
        OutputMediaElement->Play();
    }
}


void mediabuttons::MediaElement_CurrentStateChanged(Object^ sender, RoutedEventArgs^ e)
{
    if (OutputMediaElement->CurrentState == Media::MediaElementState::Playing)
    {
        MediaControl::IsPlaying = true;                
        PlayButton->Content = "Pause"; 
    }
    else
    {
        MediaControl::IsPlaying = false;
        PlayButton->Content = "Play";                
    }
}


void mediabuttons::PlayPause(Object^ sender, Object^ e)
{
    if (MediaControl::IsPlaying)
    {
        _dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
        {
            OutputMediaElement->Pause();
        }
        , CallbackContext::Any)); 
        DisplayStatus(GetTimeStampedMessage("Play/Pause Pressed - Pause"));
    }
    else
    {
        _dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
        {
            OutputMediaElement->Play();
        }
        , CallbackContext::Any)); 
        DisplayStatus(GetTimeStampedMessage("Play/Pause Pressed - Play"));
    }
}


void mediabuttons::Play(Object^ sender, Object^ e)
{
    _dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
    {
        OutputMediaElement->Play();
    }
    , CallbackContext::Any)); 
    DisplayStatus(GetTimeStampedMessage("Play Pressed"));
}


void mediabuttons::Stop(Object^ sender, Object^ e)
{
    _dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
    {
        OutputMediaElement->Stop();
    }
    , CallbackContext::Any)); 
    DisplayStatus(GetTimeStampedMessage("Stop Pressed"));
}


void mediabuttons::Pause(Object^ sender, Object^ e)
{
    _dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([=]()
    {
        OutputMediaElement->Pause();
    }
    , CallbackContext::Any)); 
    DisplayStatus(GetTimeStampedMessage("Pause Pressed"));
}


void mediabuttons::NextTrack(Object^ sender, Object^ e)
{
    DisplayStatus(GetTimeStampedMessage("Next Track Pressed"));
        
    if (_currentSongIndex < (_playlistCount - 1))
    {
        _currentSongIndex++;
            
        SetCurrentPlayingAsync(_currentSongIndex);

        if (_currentSongIndex > 0)
        {
            if (_prevTrackPressedEventToken.Value == 0)
            {
                _prevTrackPressedEventToken = MediaControl::PreviousTrackPressed += ref new EventHandler<Platform::Object^>(this, &mediabuttons::PrevTrack, CallbackContext::Same);
            }
        }
        if (_currentSongIndex == (_playlistCount - 1))
        {
            if (_nextTrackPressedEventToken.Value != 0)
            {
                MediaControl::NextTrackPressed -= _nextTrackPressedEventToken;
                _nextTrackPressedEventToken.Value = 0;
            }
        }
    }
}


void mediabuttons::PrevTrack(Object^ sender, Object^ e)
{
    DisplayStatus(GetTimeStampedMessage("Prev Track Pressed"));
    if (_currentSongIndex > 0) 
    {
        if (_currentSongIndex == (_playlistCount - 1))
        {
            if (_nextTrackPressedEventToken.Value == 0)
            {
                _nextTrackPressedEventToken = MediaControl::NextTrackPressed += ref new EventHandler<Platform::Object^>(this, &mediabuttons::NextTrack, CallbackContext::Same);
            }
        }
        
		_currentSongIndex--;
    SetCurrentPlayingAsync(_currentSongIndex);

		if (_currentSongIndex == 0)
        {
            MediaControl::PreviousTrackPressed -= _prevTrackPressedEventToken;
            _prevTrackPressedEventToken.Value = 0;
        }
    }
}


void mediabuttons::FastForward(Object^ sender, Object^ e)
{
    // Handle the fastforward event.  The fastforward event is a repeating event.
    // If the user holds down the button more events will be fired. We will just 
    // display something on the screen.
    DisplayStatus(GetTimeStampedMessage("Fast Forward Pressed"));
}


void mediabuttons::Rewind(Object^ sender, Object^ e)
{
    // Handle the rewind event.  The rewind event is a repeating event.
    // If the user holds down the button more events will be fired. We will just 
    // display something on the screen.
    DisplayStatus(GetTimeStampedMessage("Rewind Pressed"));
}


void mediabuttons::ChannelUp(Object^ sender, Object^ e)
{
    // Handle the channelup event.  The channelup event is a repeating event.
    // If the user holds down the button more events will be fired. We will just 
    // display something on the screen.
    DisplayStatus(GetTimeStampedMessage("Channel Up Pressed"));
}


void mediabuttons::ChannelDown(Object^ sender, Object^ e)
{
    // Handle the channeldown event.  The channeldown event is a repeating event.
    // If the user holds down the button more events will be fired. We will just 
    // display something on the screen.
    DisplayStatus(GetTimeStampedMessage("Channel Down Pressed"));
}


void mediabuttons::Record(Object^ sender, Object^ e)
{
    // Handle the Previous Track event.  We will just display something on the screen.
    DisplayStatus(GetTimeStampedMessage("Record Pressed"));
}
