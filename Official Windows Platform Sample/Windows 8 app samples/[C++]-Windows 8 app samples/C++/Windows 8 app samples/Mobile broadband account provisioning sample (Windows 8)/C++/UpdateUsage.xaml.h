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
// UpdateUsage.xaml.h
// Declaration of the UpdateUsage class
//

#pragma once

#include "pch.h"
#include "UpdateUsage.g.h"
#include "MainPage.xaml.h"
#include "Util.h"

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::ViewManagement;
using namespace Platform;

namespace SDKSample
{
    namespace ProvisioningAgent
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class UpdateUsage sealed
        {
        public:
            UpdateUsage();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            Util^ util;
            Windows::Networking::NetworkOperators::ProfileMediaType profileMediaType;
    
            void MediaType_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ e);
            void UpdateUsage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
