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
// DoNotAutoInvoke.xaml.h
// Declaration of the DoNotAutoInvoke class
//

#pragma once

#include "pch.h"
#include "DoNotAutoInvoke.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace TouchKeyboard
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class DoNotAutoInvoke sealed
        {
        public:
            DoNotAutoInvoke();

    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
			void onFocusClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void onAutoInvokedChecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void onAutoInvokedUnchecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
