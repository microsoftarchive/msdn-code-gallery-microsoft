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
// Scenario1.xaml.h
// Declaration of the Scenario1 class
//

#pragma once

#include "pch.h"
#include "Scenario1.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Navigation;
using namespace Platform;

namespace SDKSample
{
    namespace AtomPub
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario1 sealed
        {
        public:
            Scenario1();
    
        protected:
            virtual void OnNavigatedTo(NavigationEventArgs^ e) override;
            virtual void OnNavigatingFrom(NavigatingCancelEventArgs^ e) override;
    
        private:
            MainPage^ rootPage;
            SyndicationItemIterator^ feedIterator;
    
            void GetFeed_Click(Object^ sender, RoutedEventArgs^ e);
            void PreviousItem_Click(Object^ sender, RoutedEventArgs^ e);
            void NextItem_Click(Object^ sender, RoutedEventArgs^ e);
            void UserNameField_TextChanged(Object^ sender, TextChangedEventArgs^ e);
            void PasswordField_PasswordChanged(Object^ sender, RoutedEventArgs^ e);
            void DisplayItem();
        };
    }
}
