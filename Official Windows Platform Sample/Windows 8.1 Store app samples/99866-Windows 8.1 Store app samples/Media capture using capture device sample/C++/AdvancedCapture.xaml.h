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
// AdvancedCapture.xaml.h
// Declaration of the AdvancedCapture class
//

#pragma once

#include "pch.h"
#include "AdvancedCapture.g.h"
#include "MainPage.xaml.h"
#include <ppl.h>

#define VIDEO_FILE_NAME "video.mp4"
#define PHOTO_FILE_NAME "photo.jpg"
#define TEMP_PHOTO_FILE_NAME "photoTmp.jpg"

using namespace concurrency;
using namespace Windows::Devices::Enumeration;

namespace SDKSample
{
    namespace MediaCapture
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class AdvancedCapture sealed
        {
        public:
            AdvancedCapture();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            MainPage^ rootPage;
            void ScenarioInit();
            void ScenarioClose();
            task<void> StopLowLagPhotoAsync();
            task<void> StopLowLagRecordAsync();

            void SystemMediaControlsPropertyChanged(Windows::Media::SystemMediaTransportControls^ sender, Windows::Media::SystemMediaTransportControlsPropertyChangedEventArgs^ e);
            void RecordLimitationExceeded(Windows::Media::Capture::MediaCapture ^ mediaCapture);
            void Failed(Windows::Media::Capture::MediaCapture ^ mediaCapture, Windows::Media::Capture::MediaCaptureFailedEventArgs ^ args);

            void btnStartDevice_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void btnStartPreview_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void btnStartStopRecord_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void btnTakePhoto_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void lstEnumedDevices_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);
            void EnumerateWebcamsAsync();

            void EnumerateMicrophonesAsync();

            void EnumerateSceneModeAsync();

            void SceneMode_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);

            void chkAddRemoveEffect_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void chkAddRemoveEffect_Unchecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void AddEffectToImageStream();

            void radTakePhoto_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void radRecord_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void PrepareLowLagRecordAsync();

            void ShowStatusMessage(Platform::String^ text);
            void ShowExceptionMessage(Platform::Exception^ ex);

            task<Windows::Storage::StorageFile^> ReencodePhotoAsync(
                Windows::Storage::Streams::IRandomAccessStream ^stream,
                Windows::Storage::FileProperties::PhotoOrientation photoRotation);
            Windows::Storage::FileProperties::PhotoOrientation GetCurrentPhotoRotation();
            void PrepareForVideoRecording();
            void OrientationChanged();
            void DisplayInfo_OrientationChanged(_In_ Windows::Graphics::Display::DisplayInformation^ sender, _In_ Platform::Object^ args);
            Windows::Storage::FileProperties::PhotoOrientation PhotoRotationLookup(
                Windows::Graphics::Display::DisplayOrientations displayOrientation, bool counterclockwise);
            Windows::Media::Capture::VideoRotation VideoRotationLookup(
                Windows::Graphics::Display::DisplayOrientations displayOrientation, bool counterclockwise);
            unsigned int AdvancedCapture::VideoPreviewRotationLookup(
                Windows::Graphics::Display::DisplayOrientations displayOrientation, bool counterclockwise);

            Platform::Agile<Windows::Media::Capture::MediaCapture> m_mediaCaptureMgr;
            Platform::Agile<Windows::Media::Capture::LowLagPhotoCapture> m_lowLagPhoto;
            Platform::Agile<Windows::Media::Capture::LowLagMediaRecording> m_lowLagRecord;
            Windows::Storage::StorageFile^ m_recordStorageFile;

            bool m_bRecording;
            bool m_bEffectAdded;
            bool m_bEffectAddedToRecord;
            bool m_bEffectAddedToPhoto;
            bool m_bSuspended;
            bool m_bPreviewing;
            bool m_bLowLagPrepared;
            DeviceInformationCollection^ m_devInfoCollection;
            DeviceInformationCollection^ m_microPhoneInfoCollection;
            Windows::Foundation::EventRegistrationToken m_eventRegistrationToken;
            bool m_bRotateVideoOnOrientationChange;
            bool m_bReversePreviewRotation;
            Windows::Graphics::Display::DisplayOrientations m_displayOrientation;
            Windows::Foundation::EventRegistrationToken m_orientationChangedEventToken;

            double m_rotHeight;
            double m_rotWidth;
        };
    }
}
