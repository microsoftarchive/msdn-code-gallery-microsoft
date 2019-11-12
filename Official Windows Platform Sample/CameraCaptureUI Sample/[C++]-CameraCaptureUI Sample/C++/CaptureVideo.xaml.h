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
// CaptureVideo.xaml.h
// Declaration of the CaptureVideo class
//

#pragma once

#include "pch.h"
#include "CaptureVideo.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace CameraCapture
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class CaptureVideo sealed
        {
        public:
            CaptureVideo();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;        
            void CaptureVideo_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void Reset_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void LoadVideo(Platform::String^ filePath);
    
            Windows::Foundation::Collections::IPropertySet^ appSettings;
        };
    }
}
