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
// PhotoSequence.xaml.h
// Declaration of the PhotoSequence class
//

#pragma once

#include "pch.h"
#include "PhotoSequence.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::ViewManagement;
using namespace Windows::Devices::Enumeration;
#define PHOTOSEQ_FILE_NAME "photoSequence.jpg"

namespace SDKSample
{
    namespace MediaCapture
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class PhotoSequence sealed
        {
        public:
            PhotoSequence();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;


        private:
            MainPage^ rootPage;
            void ScenarioInit();
            void ScenarioClose();

            void btnStartDevice_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void btnStartPreview_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void btnStartStopPhotoSequence_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);    
            void ItemSelected_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void btnSaveToFile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void ShowStatusMessage(Platform::String^ text);
            void ShowExceptionMessage(Platform::Exception^ ex);    
            void Failed(Windows::Media::Capture::MediaCapture ^ mediaCapture, Windows::Media::Capture::MediaCaptureFailedEventArgs ^ args);
            void Clear(); 
            void SystemMediaControlsPropertyChanged(Windows::Media::SystemMediaTransportControls^ sender, Windows::Media::SystemMediaTransportControlsPropertyChangedEventArgs^ e);

            Platform::Agile<Windows::Media::Capture::MediaCapture> m_capture;
            Platform::Agile<Windows::Media::Capture::LowLagPhotoSequenceCapture> m_photoSequenceCapture;
            Platform::Collections::Vector<Windows::Media::Capture::CapturedFrame^>^ m_framePtr; 

            Windows::Storage::StorageFile^ m_photoStorageFile;
            Windows::Foundation::EventRegistrationToken m_eventRegistrationToken;

            bool m_highLighted;
            int m_selectedIndex;
            unsigned int m_ThumbnailNum;
            unsigned int m_frameNum;
            unsigned int m_pastFrame;
            unsigned int m_futureFrame;
            bool m_bPhotoSequence;
            bool m_bPreviewing;
        };
    }
}
