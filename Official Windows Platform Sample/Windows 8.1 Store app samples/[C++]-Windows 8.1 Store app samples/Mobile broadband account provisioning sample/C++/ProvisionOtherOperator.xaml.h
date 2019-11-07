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
// ProvisionOtherOperator.xaml.h
// Declaration of the ProvisionOtherOperator class
//

#pragma once

#include "pch.h"
#include "ProvisionOtherOperator.g.h"
#include "MainPage.xaml.h"
#include "Util.h"

namespace SDKSample
{
    namespace ProvisioningAgent
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class ProvisionOtherOperator sealed
        {
        public:
            ProvisionOtherOperator();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            Util^ util;
            void ProvisionOtherOperator_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
