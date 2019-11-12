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
// PageWithAppBar.xaml.h
// Declaration of the PageWithAppBar class
//

#pragma once

#include "pch.h"
#include "PageWithAppBar.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace WebViewControl
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]    
        public ref class PageWithAppBar sealed
        {
        public:
            PageWithAppBar();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            void BottomAppBar_Opened(Platform::Object^ sender, Platform::Object^ obj);
            void BottomAppBar_Closed(Platform::Object^ sender, Platform::Object^ obj);
            void Home_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
