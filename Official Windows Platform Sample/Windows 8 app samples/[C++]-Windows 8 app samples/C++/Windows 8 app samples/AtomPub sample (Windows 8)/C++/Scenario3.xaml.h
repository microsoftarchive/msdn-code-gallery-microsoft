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
// Scenario3.xaml.h
// Declaration of the Scenario3 class
//

#pragma once

#include "pch.h"
#include "Scenario3.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Navigation;

namespace SDKSample
{
    namespace AtomPub
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario3 sealed
        {
        public:
            Scenario3();
    
        protected:
            virtual void OnNavigatedTo(NavigationEventArgs^ e) override;
            virtual void OnNavigatingFrom(NavigatingCancelEventArgs^ e) override;
    
        private:
            MainPage^ rootPage;
            SyndicationItemIterator^ feedIterator;
    
            void GetFeed_Click(Object^ sender, RoutedEventArgs^ e);
            void DeleteItem_Click(Object^ sender, RoutedEventArgs^ e);
            void PreviousItem_Click(Object^ sender, RoutedEventArgs^ e);
            void NextItem_Click(Object^ sender, RoutedEventArgs^ e);
            void UserNameField_TextChanged(Object^ sender, TextChangedEventArgs^ e);
            void PasswordField_PasswordChanged(Object^ sender, RoutedEventArgs^ e);
            void DisplayItem();
        };
    }
}
