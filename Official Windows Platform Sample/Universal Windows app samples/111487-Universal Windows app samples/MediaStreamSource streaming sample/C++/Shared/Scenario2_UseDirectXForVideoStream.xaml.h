//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2_UseDirectXForVideoStream.xaml.h
// Declaration of the Scenario2_UseDirectXForVideoStream class
//

#pragma once

#include "Scenario2_UseDirectXForVideoStream.g.h"

namespace SDKSample
{
    namespace MediaStreamSource
    {

        /// <summary>
        /// A basic page that provides characteristics common to most applications.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario2_UseDirectXForVideoStream sealed
        {
        public:
            Scenario2_UseDirectXForVideoStream();
            
            virtual void OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs^ e) override;

        private:
            void playButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void pauseButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void OnStarting(Windows::Media::Core::MediaStreamSource ^sender, Windows::Media::Core::MediaStreamSourceStartingEventArgs ^args);
            void OnSampleRequested(Windows::Media::Core::MediaStreamSource ^sender, Windows::Media::Core::MediaStreamSourceSampleRequestedEventArgs ^args);
            void OnClosed(Windows::Media::Core::MediaStreamSource ^sender, Windows::Media::Core::MediaStreamSourceClosedEventArgs ^args);
            void OnLoaded(Platform::Object ^sender, Windows::UI::Xaml::RoutedEventArgs ^e);

            void InitializeMediaPlayer();

            DXSurfaceGenerator::SampleGenerator ^ _sampleGenerator;
            Windows::Media::Core::MediaStreamSource ^ _mss;
            Windows::Media::Core::VideoStreamDescriptor^ _videoDesc;
            bool _fHasSetMediaSource;
            bool _fMediaSourceIsLoaded;
            bool _fPlayRequestPending;
            void OnCurrentStateChanged(Platform::Object ^sender, Windows::UI::Xaml::RoutedEventArgs ^e);
        };
    }
}
