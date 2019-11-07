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
// Pointer.xaml.h
// Declaration of the Pointer class
//

#pragma once

#include "pch.h"
#include "Pointer.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace DeviceCaps
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class Pointer sealed
        {
        public:
            Pointer();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            void PointerGetSettings_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		Platform::String^ PointerType(Windows::Devices::Input::PointerDevice^ pointerDevice);
        };
    }
}
