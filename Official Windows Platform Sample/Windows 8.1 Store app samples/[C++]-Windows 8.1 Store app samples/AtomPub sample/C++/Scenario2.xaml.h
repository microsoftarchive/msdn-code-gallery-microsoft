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
// Scenario2.xaml.h
// Declaration of the Scenario2 class
//

#pragma once

#include "pch.h"
#include "Scenario2.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Foundation;
using namespace Platform;

namespace SDKSample
{
    namespace AtomPub
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario2 sealed
        {
        public:
            Scenario2();
    
        protected:
            virtual void OnNavigatedTo(NavigationEventArgs^ e) override;
            virtual void OnNavigatingFrom(NavigatingCancelEventArgs^ e) override;
    
        private:
            MainPage^ rootPage;
            void SubmitItem_Click(Object^ sender, RoutedEventArgs^ e);
            Uri^ FindEditUri(ServiceDocument^ serviceDocument);
            void UserNameField_TextChanged(Object^ sender, TextChangedEventArgs^ e);
            void PasswordField_PasswordChanged(Object^ sender, RoutedEventArgs^ e);
        };
    }
}
