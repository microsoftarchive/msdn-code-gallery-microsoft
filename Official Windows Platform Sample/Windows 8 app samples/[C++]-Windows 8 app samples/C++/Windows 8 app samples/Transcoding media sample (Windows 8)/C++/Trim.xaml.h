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
// Trim.xaml.h
// Declaration of the Trim class
//

#pragma once

#include "pch.h"
#include "Trim.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace Transcode
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class Trim sealed
        {
        public:
            Trim();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    
        private:
            void GetPresetProfile(Windows::UI::Xaml::Controls::ComboBox^ comboBox);
            void PickFile(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void OnTargetFormatChanged(Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);
            void SetPickFileButton(bool isEnabled);
            void StopPlayers();
            void PlayFile(Windows::Storage::StorageFile^ MediaFile);
    
            // Trim point setters
            void MarkIn(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void MarkOut(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    
            void TranscodeTrim(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void TranscodeCancel(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void TranscodeProgress(Windows::Foundation::IAsyncActionWithProgress<double>^ asyncInfo, double percent);
            void TranscodeError(Platform::String^ error);
            void TranscodeFailure(Windows::Media::Transcoding::TranscodeFailureReason reason);
            void SetCancelButton(bool isEnabled);
            void EnableButtons();
            void DisableButtons();
    
            // Media Controls
            void InputPlayButton_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void InputPauseButton_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void InputStopButton_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void OutputPlayButton_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void OutputPauseButton_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void OutputStopButton_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    
            Platform::String^ _OutputFileName;
            Windows::Media::MediaProperties::MediaEncodingProfile^ _Profile;
            Windows::Storage::StorageFile^ _InputFile;
            Windows::Storage::StorageFile^ _OutputFile;
            Windows::Media::Transcoding::MediaTranscoder^ _Transcoder;
            Concurrency::cancellation_token_source _CTS;
            
            Windows::Foundation::TimeSpan _start; 
            Windows::Foundation::TimeSpan _stop;
    
            bool _UseMp4;
    
            MainPage^ rootPage;
        };
    }
}
