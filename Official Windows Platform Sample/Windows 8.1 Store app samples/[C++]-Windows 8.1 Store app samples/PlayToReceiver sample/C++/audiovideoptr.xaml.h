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
// Scenario1.xaml.h
// Declaration of the Scenario1 class
//

#pragma once
#include <agile.h>
#include "pch.h"
#include "AudioVideoPTR.g.h"
#include "MainPage.xaml.h"
using namespace Windows::Media::PlayTo;
using namespace Windows::UI::Xaml::Media::Imaging;
namespace SDKSample
{
    namespace PlayToreceiver
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario1 sealed
        {
        public:
            Scenario1();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            Platform::Agile<Windows::Media::PlayTo::PlayToReceiver> receiver;
            bool IsReceiverStarted;
            bool IsSeeking;
            bool IsMediaJustLoaded;
            bool IsPlayReceivedPreMediaLoaded;
            double bufferedPlaybackRate;
            enum MediaType { None, Image, AudioVisual};
            MediaType currentType;
            BitmapImage^ imageRecd;
    
            void InitialisePlayToReceiver();
            void startPlayToReceiver(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void stopPlayToReceiver(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    
            //MediaElement Event Handlers
            void dmrVideo_VolumeChanged(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void dmrVideo_RateChanged(Platform::Object^ sender, Windows::UI::Xaml::Media::RateChangedRoutedEventArgs^ e);
            void dmrVideo_MediaOpened(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void dmrVideo_CurrentStateChanged(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void dmrVideo_MediaEnded(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void dmrVideo_MediaFailed(Platform::Object^ sender, Windows::UI::Xaml::ExceptionRoutedEventArgs^ e);
            void dmrVideo_SeekCompleted(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    
            //Bitmap Event Handlers
            void btmapImage_ImageOpened(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            //PlayToReceiver Event Handlers
            void receiver_PlayRequested(PlayToReceiver^, Object^);
            void receiver_PauseRequested(PlayToReceiver^, Object^);
            void receiver_StopRequested(PlayToReceiver^, Object^);
            void receiver_TimeUpdateRequested(PlayToReceiver^, Object^);
            void receiver_CurrentTimeChangeRequested(PlayToReceiver^, CurrentTimeChangeRequestedEventArgs^);
            void receiver_SourceChangeRequested(PlayToReceiver^, SourceChangeRequestedEventArgs^);
            void receiver_MuteChangeRequested(PlayToReceiver^, MuteChangeRequestedEventArgs^);
            void receiver_PlaybackRateChangeRequested(PlayToReceiver^, PlaybackRateChangeRequestedEventArgs^);
            void receiver_VolumeChangeRequestedEvent(PlayToReceiver^, VolumeChangeRequestedEventArgs^);
            void dmrImage_ImageFailed_1(Platform::Object^ sender, Windows::UI::Xaml::ExceptionRoutedEventArgs^ e);
            void dmrVideo_DownloadProgressChanged_1(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
