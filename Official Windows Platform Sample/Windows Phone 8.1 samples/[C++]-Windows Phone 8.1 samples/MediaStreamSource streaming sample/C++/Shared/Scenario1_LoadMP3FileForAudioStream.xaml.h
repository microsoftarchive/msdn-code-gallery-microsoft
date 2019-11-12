//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1_LoadMP3FileForAudioStream.xaml.h
// Declaration of the Scenario1_LoadMP3FileForAudioStream class
//

#pragma once
#include "Scenario1_LoadMP3FileForAudioStream.g.h"

namespace SDKSample
{
    namespace MediaStreamSource
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
        public ref class Scenario1_LoadMP3FileForAudioStream sealed : public IFileOpenPickerContinuable
#else           
        public ref class Scenario1_LoadMP3FileForAudioStream sealed
#endif
        {
        public:
            Scenario1_LoadMP3FileForAudioStream();

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
            virtual void ContinueFileOpenPicker(Windows::ApplicationModel::Activation::FileOpenPickerContinuationEventArgs^ args);
#endif
        private:
            void InitializeMediaStreamSource();
            void PickMP3_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void playButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void pauseButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void OnStarting(Windows::Media::Core::MediaStreamSource ^sender, Windows::Media::Core::MediaStreamSourceStartingEventArgs ^args);
            void OnSampleRequested(Windows::Media::Core::MediaStreamSource ^sender, Windows::Media::Core::MediaStreamSourceSampleRequestedEventArgs ^args);
            void OnClosed(Windows::Media::Core::MediaStreamSource ^sender, Windows::Media::Core::MediaStreamSourceClosedEventArgs ^args);

            concurrency::task<void> getMP3FileProperties();

            Windows::Storage::StorageFile^ inputMP3File;
            Windows::Media::Core::MediaStreamSource^ MSS;
            Windows::Storage::Streams::IRandomAccessStream^ mssStream;

            Windows::Foundation::EventRegistrationToken sampleRequestedToken;
            Windows::Foundation::EventRegistrationToken startingRequestedToken;
            Windows::Foundation::EventRegistrationToken closedRequestedToken;

            UINT64 byteOffset;
            Windows::Foundation::TimeSpan timeOffset;
            Platform::String^ title;
            UINT32 sampleSize;
            Windows::Foundation::TimeSpan sampleDuration;
            Windows::Foundation::TimeSpan songDuration;

            UINT32 nSampleRate;
            UINT32 nChannelCount;
            UINT32 nBitrate;

        };
    }
}
