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
// PlaybackControl.xaml.h
// Declaration of the PlaybackControl class
//

#pragma once

#include "pch.h"
#include "PlaybackControl.g.h"
#include "MainPage.xaml.h"
#include <agile.h>

namespace SDKSample
{
    namespace  PlaybackManager
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class PlaybackControl sealed
        {
        public:
            PlaybackControl();
            void Play();
            void Pause();
            void Stop();
            void SetAudioCategory(Windows::UI::Xaml::Media::AudioCategory category);
            void SetAudioDeviceType(Windows::UI::Xaml::Media::AudioDeviceType devicetype);
            void SelectFile();
    
        private:
            void DisplayStatus(Platform::String^ text);
            Platform::String^ GetTimeStampedMessage(Platform::String^ EventCalled);
            Platform::String^ SoundLevelToString(Windows::Media::SoundLevel level);
            void AppMuted();
            void AppUnmuted();
            void MediaControl_SoundLevelChanged(Platform::Object^ sender, Platform::Object^ e);
            void MediaControl_PlayPauseTogglePressed(Platform::Object^ sender, Platform::Object^ e);
            void MediaControl_PlayPressed(Platform::Object^ sender, Platform::Object^ e);
            void MediaControl_StopPressed(Platform::Object^ sender, Platform::Object^ e);
            void MediaControl_PausePressed(Platform::Object^ sender, Platform::Object^ e);
            void Button_Play_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void Button_Pause_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void Button_Stop_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    
        private:
            bool IsPlaying;
            Platform::Agile<Windows::UI::Core::CoreWindow> cw;
    
            static bool IsRegistered;
            static Windows::Foundation::EventRegistrationToken SoundLevelChangedToken;
            static Windows::Foundation::EventRegistrationToken PlayPauseTogglePressedToken;
            static Windows::Foundation::EventRegistrationToken PlayPressedToken;
            static Windows::Foundation::EventRegistrationToken PausePressedToken;
            static Windows::Foundation::EventRegistrationToken StopPressedToken;
    
        };
    }
}
