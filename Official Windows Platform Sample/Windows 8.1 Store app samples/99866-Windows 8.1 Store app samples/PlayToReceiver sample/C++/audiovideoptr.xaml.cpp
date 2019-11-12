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
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include <string.h>
#include "AudioVideoPTR.xaml.h"

using namespace SDKSample::PlayToreceiver;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Platform;
using namespace Windows::UI::Xaml::Media;
Scenario1^ g_this;

Scenario1::Scenario1():receiver(nullptr)
{
    InitializeComponent();
    g_this = this;
    IsReceiverStarted = false;
    IsSeeking = false;
    bufferedPlaybackRate = 0;
    IsMediaJustLoaded = false;
    currentType = MediaType::None;
    imageRecd = nullptr;
    IsPlayReceivedPreMediaLoaded = false;
    InitialisePlayToReceiver();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void Scenario1::InitialisePlayToReceiver()
{
    try
    {
        Windows::Media::PlayTo::PlayToReceiver^ localReceiver = ref new Windows::Media::PlayTo::PlayToReceiver();
        localReceiver->PlayRequested += ref new TypedEventHandler<PlayToReceiver^, Object^>(this, &Scenario1::receiver_PlayRequested);
        localReceiver->PauseRequested += ref new TypedEventHandler<PlayToReceiver^, Object^>(this, &Scenario1::receiver_PauseRequested);
        localReceiver->StopRequested += ref new TypedEventHandler<PlayToReceiver^, Object^>(this, &Scenario1::receiver_StopRequested);
        localReceiver->TimeUpdateRequested += ref new TypedEventHandler<PlayToReceiver^, Object^>(this, &Scenario1::receiver_TimeUpdateRequested);
        localReceiver->CurrentTimeChangeRequested += ref new TypedEventHandler<PlayToReceiver^, CurrentTimeChangeRequestedEventArgs^>(this, &Scenario1::receiver_CurrentTimeChangeRequested);
        localReceiver->SourceChangeRequested += ref new TypedEventHandler<PlayToReceiver^, SourceChangeRequestedEventArgs^>(this, &Scenario1::receiver_SourceChangeRequested);
        localReceiver->MuteChangeRequested += ref new TypedEventHandler<PlayToReceiver^, MuteChangeRequestedEventArgs^>(this, &Scenario1::receiver_MuteChangeRequested);
        localReceiver->PlaybackRateChangeRequested += ref new TypedEventHandler<PlayToReceiver^, PlaybackRateChangeRequestedEventArgs^>(this, &Scenario1::receiver_PlaybackRateChangeRequested);
        localReceiver->VolumeChangeRequested += ref new TypedEventHandler<PlayToReceiver^, VolumeChangeRequestedEventArgs^>(this, &Scenario1::receiver_VolumeChangeRequestedEvent);

        localReceiver->SupportsAudio = true;
        localReceiver->SupportsVideo = true;
        localReceiver->SupportsImage = true;

        localReceiver->FriendlyName = "SDK CPP Sample PlayToReceiver";
        receiver = localReceiver;
    }
    catch(Platform::Exception^ e)
    {
        startDMRButton->IsEnabled = false;
        stopDMRButton->IsEnabled = true;
        rootPage->NotifyUser("PlayToReceiver Initialization failed, Error: " + e->Message, NotifyType::ErrorMessage);
    }
}

void Scenario1::startPlayToReceiver(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        startDMRButton->IsEnabled = false;
        stopDMRButton->IsEnabled = true;
        IAsyncAction^ startoperation = receiver->StartAsync();
        startoperation->Completed = ref new AsyncActionCompletedHandler([=](IAsyncInfo^ asyncInfo, AsyncStatus /**/)
        {
            IsReceiverStarted = true;
            auto handler = ref new DispatchedHandler(
            []()
            {
                g_this->rootPage->NotifyUser("PlayToReceiver started", NotifyType::StatusMessage);
            },CallbackContext::Any);
            g_this->Dispatcher->RunAsync(CoreDispatcherPriority::High,handler);
        });
    }
    catch(Platform::Exception^ e)
    {
        IsReceiverStarted = false;
        startDMRButton->IsEnabled = true;
        stopDMRButton->IsEnabled = false;
        rootPage->NotifyUser("PlayToReceiver start failed, Error: " + e->Message, NotifyType::ErrorMessage);
    }
}

void Scenario1::stopPlayToReceiver(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{

    Windows::Media::PlayTo::PlayToReceiver^ localReceiver = receiver.Get();
    if(localReceiver != nullptr)
    {
        try
        {
            startDMRButton->IsEnabled = true;
            stopDMRButton->IsEnabled = false;
            IAsyncAction^ stopoperation = localReceiver->StopAsync();
            stopoperation->Completed = ref new AsyncActionCompletedHandler([=](IAsyncInfo^ asyncInfo, AsyncStatus /**/)
            {
                g_this->IsReceiverStarted = true;
                auto handler = ref new DispatchedHandler(
                []()
                {
                    g_this->rootPage->NotifyUser("PlayToReceiver stopped", NotifyType::StatusMessage);
                },CallbackContext::Any);
                g_this->Dispatcher->RunAsync(CoreDispatcherPriority::High,handler);
            });
        }
        catch(Platform::Exception^ e)
        {
            startDMRButton->IsEnabled = false;
            stopDMRButton->IsEnabled = true;
            rootPage->NotifyUser("PlayToReceiver stop failed, Error: " + e->Message, NotifyType::ErrorMessage);
        }
    }
}

void Scenario1::dmrVideo_VolumeChanged(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if(IsReceiverStarted)
    {
        receiver->NotifyVolumeChange(dmrVideo->Volume, dmrVideo->IsMuted);
    }
}


void Scenario1::dmrVideo_RateChanged(Platform::Object^ sender, Windows::UI::Xaml::Media::RateChangedRoutedEventArgs^ e)
{
    if(IsReceiverStarted)
    {
        receiver->NotifyRateChange(dmrVideo->PlaybackRate);
    }
}


void Scenario1::dmrVideo_MediaOpened(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if(IsReceiverStarted)
    {
        receiver->NotifyLoadedMetadata();
        receiver->NotifyDurationChange(dmrVideo->NaturalDuration.TimeSpan);
        if(IsPlayReceivedPreMediaLoaded == true)
        {
            dmrVideo->Play();
        }
    }
}


void Scenario1::dmrVideo_CurrentStateChanged(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if(IsReceiverStarted)
    {
        switch (dmrVideo->CurrentState)
        {
        case MediaElementState::Playing:
            receiver->NotifyPlaying();
            break;
        case MediaElementState::Paused:
            if(IsMediaJustLoaded)
            {
                receiver->NotifyStopped();
                IsMediaJustLoaded = false;
            }
            else
            {
                receiver->NotifyPaused();
            }
            break;
        case MediaElementState::Stopped:
            receiver->NotifyStopped();
            break;
        default:
            break;
        }
    }
}


void Scenario1::dmrVideo_MediaEnded(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if(IsReceiverStarted)
    {
        receiver->NotifyEnded();
        if(dmrVideo != nullptr)
            dmrVideo->Stop();
    }
}


void Scenario1::dmrVideo_MediaFailed(Platform::Object^ sender, Windows::UI::Xaml::ExceptionRoutedEventArgs^ e)
{
    if(IsReceiverStarted)
    {
        receiver->NotifyError();
    }
}


void Scenario1::dmrVideo_SeekCompleted(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if(IsReceiverStarted)
    {
        if(!IsSeeking)
            receiver->NotifySeeking();
        receiver->NotifySeeked();
        IsSeeking = false;
    }
}

void Scenario1::receiver_PlayRequested(PlayToReceiver^ , Object^ args)
{
    auto handler = ref new DispatchedHandler(
        [this, args]()
        {
            if(dmrVideo != nullptr && currentType == MediaType::AudioVisual)
            {
                IsPlayReceivedPreMediaLoaded = true;
                dmrVideo->Play();
            }
            else if(currentType == MediaType::Image && dmrImage != nullptr)
            {
                dmrImage->Source = imageRecd;
                receiver->NotifyPlaying();
            }
        },CallbackContext::Any);
    
    Dispatcher->RunAsync(CoreDispatcherPriority::High,handler);
}

void Scenario1::receiver_PauseRequested(PlayToReceiver^ , Object^ args)
{
    auto handler = ref new DispatchedHandler(
        [this, args]()
        {
            if(dmrVideo != nullptr && currentType == MediaType::AudioVisual)
            {
                if(dmrVideo->CurrentState == MediaElementState::Stopped)
                {
                    receiver->NotifyPaused();
                }
                else
                {
                    dmrVideo->Pause();
                }
            }
        },CallbackContext::Any);
    
    Dispatcher->RunAsync(CoreDispatcherPriority::High,handler);
}

void Scenario1::receiver_StopRequested(PlayToReceiver^ , Object^ args)
{
    auto handler = ref new DispatchedHandler(
        [this, args]()
        {
            if(dmrVideo != nullptr && currentType == MediaType::AudioVisual)
            {
                receiver->NotifyStopped();
                dmrVideo->Stop();
            }
            else if (dmrImage != nullptr && currentType == MediaType::Image)
            {
                dmrImage->Source = nullptr;
                receiver->NotifyStopped();
            }
        },CallbackContext::Any);
    
    Dispatcher->RunAsync(CoreDispatcherPriority::High,handler);
}

void Scenario1::receiver_TimeUpdateRequested(PlayToReceiver^ , Object^ args)
{
    auto handler = ref new DispatchedHandler(
        [this, args]()
        {
            if(g_this->IsReceiverStarted)
            {
                if(dmrVideo != nullptr && currentType == MediaType::AudioVisual)
                {
                    receiver->NotifyTimeUpdate(dmrVideo->Position);
                }
                else if (currentType == MediaType::Image)
                {
                    __int64 timeSpaninMilliseconds = 0;
                    TimeSpan ts = {0};
                    receiver->NotifyTimeUpdate(ts);
                }
            }
        },CallbackContext::Any);
    
    Dispatcher->RunAsync(CoreDispatcherPriority::High,handler);
}

void Scenario1::receiver_CurrentTimeChangeRequested(PlayToReceiver^, CurrentTimeChangeRequestedEventArgs^ args)
{
    auto handler = ref new DispatchedHandler(
        [this, args]()
        {
            if(g_this->IsReceiverStarted )
            {
                if(dmrVideo != nullptr && currentType == MediaType::AudioVisual)
                {
                    if(dmrVideo->CanSeek)
                    {
                        
                        dmrVideo->Position = args->Time;
                        receiver->NotifySeeking();
                        IsSeeking = true;
                        
                    }
                }
                else if(currentType == MediaType::Image)
                {
                    receiver->NotifySeeking();
                    receiver->NotifySeeked();
                }
            }
        },CallbackContext::Any);
    
    Dispatcher->RunAsync(CoreDispatcherPriority::High,handler);
}

void Scenario1::btmapImage_ImageOpened(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    receiver->NotifyLoadedMetadata();
}

void Scenario1::receiver_SourceChangeRequested(PlayToReceiver^, SourceChangeRequestedEventArgs^ args)
{
    auto handler = ref new DispatchedHandler(
    [this, args]()
    {
        String^ imgstr = "image";
        IsPlayReceivedPreMediaLoaded = false;
        if(args->Stream == nullptr)
        {
            if(currentType == MediaType::AudioVisual)
            {
                dmrVideo->Stop();
            }
            else if (currentType == MediaType::Image)
            {
                dmrImage->Source = nullptr;
                dmrImage->Opacity = 0;
            }
            currentType = MediaType::None;
        }
        else if(!FindStringOrdinal(FIND_FROMSTART,args->Stream->ContentType->Data(), args->Stream->ContentType->Length(), imgstr->Data(),imgstr->Length(),TRUE))
        {
            imageRecd = ref new BitmapImage();
            imageRecd->ImageOpened += ref new RoutedEventHandler(this, &Scenario1::btmapImage_ImageOpened);
            imageRecd->SetSource(args->Stream);
            if(currentType != MediaType::Image)
            {
                if(currentType == MediaType::AudioVisual)
                {
                    dmrVideo->Stop();
                }
                dmrImage->Opacity = 1;
                dmrVideo->Opacity = 0;
            }
            currentType = MediaType::Image;
        }
        else 
        {
            if(dmrVideo != nullptr)
            {
                try
                {
                    IsMediaJustLoaded = true;
                    dmrVideo->SetSource(args->Stream, args->Stream->ContentType);
                }
                catch(Exception^ e)
                {
                    rootPage->NotifyUser(e->Message->ToString() + " Content Type: " + args->Stream->ContentType, NotifyType::ErrorMessage);
                }
            }
            if(currentType == MediaType::Image)
            {
                dmrImage->Opacity = 0;
                if(dmrVideo != nullptr)
                {
                    dmrVideo->Opacity = 1;
                }
                dmrImage->Source = nullptr;
            }
            currentType = MediaType::AudioVisual;
        }
    },CallbackContext::Any);

    Dispatcher->RunAsync(CoreDispatcherPriority::High,handler);
}


void Scenario1::receiver_MuteChangeRequested(PlayToReceiver^, MuteChangeRequestedEventArgs^ args)
{
    auto handler = ref new DispatchedHandler(
        [this, args]()
        {
            if(dmrVideo != nullptr && currentType == MediaType::AudioVisual)
            {
                dmrVideo->IsMuted = args->Mute;
            }
            else if(currentType == MediaType::Image)
            {
                receiver->NotifyVolumeChange(0, args->Mute);
            }
        },CallbackContext::Any);
    
    Dispatcher->RunAsync(CoreDispatcherPriority::High,handler);
}


void Scenario1::receiver_PlaybackRateChangeRequested(PlayToReceiver^, PlaybackRateChangeRequestedEventArgs^ args)
{
    auto handler = ref new DispatchedHandler(
        [this, args]()
        {
            if(dmrVideo != nullptr && MediaType::AudioVisual == currentType)
            {
                if(dmrVideo->CurrentState != MediaElementState::Opening && dmrVideo->CurrentState != MediaElementState::Closed)
                {
                    dmrVideo->PlaybackRate = args->Rate;
                }
                else
                {
                    bufferedPlaybackRate = args->Rate;
                }
            }
        },CallbackContext::Any);
    
    Dispatcher->RunAsync(CoreDispatcherPriority::High,handler);
}


void Scenario1::receiver_VolumeChangeRequestedEvent(PlayToReceiver^, VolumeChangeRequestedEventArgs^ args)
{
    auto handler = ref new DispatchedHandler(
        [this, args]()
        {
            if(dmrVideo != nullptr && currentType == MediaType::AudioVisual)
            {
                dmrVideo->Volume = args->Volume;
            }
        },CallbackContext::Any);
    
    Dispatcher->RunAsync(CoreDispatcherPriority::High,handler);
}



void Scenario1::dmrImage_ImageFailed_1(Platform::Object^ sender, Windows::UI::Xaml::ExceptionRoutedEventArgs^ e)
{
    if(IsReceiverStarted)
    {
        receiver->NotifyError();
    }
}


void Scenario1::dmrVideo_DownloadProgressChanged_1(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if(IsReceiverStarted)
    {
        if(dmrVideo->DownloadProgress == 1 && bufferedPlaybackRate > 0)
        {
            dmrVideo->PlaybackRate = bufferedPlaybackRate;
            bufferedPlaybackRate = 0;
        }
    }
}
