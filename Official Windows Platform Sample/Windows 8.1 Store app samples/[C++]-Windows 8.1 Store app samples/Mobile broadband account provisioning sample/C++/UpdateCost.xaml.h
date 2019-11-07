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
// UpdateCost.xaml.h
// Declaration of the UpdateCost class
//

#pragma once

#include "pch.h"
#include "UpdateCost.g.h"
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
        public ref class UpdateCost sealed
        {
        public:
            UpdateCost();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            Util^ util;
            Windows::Networking::NetworkOperators::ProfileMediaType profileMediaType;
            Windows::Networking::Connectivity::NetworkCostType networkCostType;
    
            void MediaType_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ e);
            void NetworkCostCategory_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ e);
            void UpdateCost_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
