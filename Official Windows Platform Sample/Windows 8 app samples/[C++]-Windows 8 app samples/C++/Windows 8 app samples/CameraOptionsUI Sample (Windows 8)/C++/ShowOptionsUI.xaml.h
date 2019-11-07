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
// ShowOptionsUI.xaml.h
// Declaration of the ShowOptionsUI class
//

#pragma once

#include "pch.h"
#include "ShowOptionsUI.g.h"
#include "MainPage.xaml.h"
#include <agile.h>

namespace SDKSample
{
    namespace CameraOptions
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class ShowOptionsUI sealed
        {
        public:
            ShowOptionsUI();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    
        private:
            MainPage^ rootPage;
            void StartPreview_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ShowSettings_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void MediaControl_SoundLevelChanged(Platform::Object^ sender, Platform::Object^ e);
    
            Platform::Agile<Windows::Media::Capture::MediaCapture> mediaCaptureMgr;
            bool previewStarted;
    
            Windows::UI::Core::CoreDispatcher^ dispatcher;
        };
    }
}
