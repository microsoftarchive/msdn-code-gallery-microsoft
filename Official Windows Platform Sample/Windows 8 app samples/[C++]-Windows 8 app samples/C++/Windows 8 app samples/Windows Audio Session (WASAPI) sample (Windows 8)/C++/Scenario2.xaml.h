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
// Scenario2.xaml.h
// Declaration of the Scenario2 class
//

#pragma once

#include "pch.h"
#include "Scenario2.g.h"
#include "MainPage.xaml.h"
#include "WASAPIRenderer.h"

namespace SDKSample
{
    namespace WASAPIAudio
    {
        public enum class ContentType
        {
            ContentTypeTone,
            ContentTypeFile
        };

        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario2 sealed
        {

        public:
            Scenario2();

        protected:
            // Template Support
            virtual void OnNavigatedTo( Windows::UI::Xaml::Navigation::NavigationEventArgs^ e ) override;
            virtual void OnNavigatedFrom( Windows::UI::Xaml::Navigation::NavigationEventArgs^ e ) override;

        private:
            ~Scenario2();

            // UI Events
            void btnPlay_Click( Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e );
            void btnPause_Click( Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e );
            void btnPlayPause_Click( Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e );
            void btnStop_Click( Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e );
            void btnFilePicker_Click( Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e );
            void radioTone_Checked( Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e );
            void radioFile_Checked( Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e );
            void sliderFrequency_ValueChanged( Platform::Object^ sender, Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e );
            void sliderVolume_ValueChanged( Platform::Object^ sender, Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e );

            // UI Helpers
            void ShowStatusMessage( Platform::String^ str, NotifyType messageType );
            void UpdateContentProps( Platform::String^ str );
            void UpdateContentUI( Platform::Boolean disableAll );
            void UpdateMediaControlUI( DeviceState deviceState );

            // Handlers
            void OnDeviceStateChange( Object^ sender, DeviceStateChangedEventArgs^ e );
            void OnPickFileAsync();
            void OnSetVolume( UINT32 volume );

            void InitializeDevice();
            void StartDevice( Object^ sender, Object^ e );
            void StopDevice( Object^ sender, Object^ e );
            void PauseDevice( Object^ sender, Object^ e );
            void PlayPauseToggle( Object^ sender, Object^ e );

        private:
            MainPage^                                       rootPage;
            Windows::UI::Core::CoreDispatcher^              m_CoreDispatcher;
            Windows::Foundation::EventRegistrationToken     m_deviceStateChangeToken;
            Windows::Foundation::EventRegistrationToken     m_MediaControlPlayToken;
            Windows::Foundation::EventRegistrationToken     m_MediaControlPauseToken;
            Windows::Foundation::EventRegistrationToken     m_MediaControlStopToken;
            Windows::Foundation::EventRegistrationToken     m_MediaControlPlayPauseToken;

            Platform::Boolean          m_IsMFLoaded;
            IRandomAccessStream^       m_ContentStream;
            ContentType                m_ContentType;
            DeviceStateChangedEvent^   m_StateChangedEvent;
            ComPtr<WASAPIRenderer>     m_spRenderer;
        };
    }
}
