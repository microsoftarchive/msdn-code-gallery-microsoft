//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.h
// Declaration of the Scenario1 class
//

#pragma once
#include "Scenario1.g.h"

namespace SDKSample
{
    namespace PasswordVaultCPP
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class Scenario1 sealed
        {
        public:
            Scenario1();
//		protected:
//            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;	

        private:
            void Reset_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void Save_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);		
            void Read_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void Delete_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			Windows::Foundation::IAsyncAction^ InitializeVaultAsync();
			void DebugPrint(Platform::String^ Message);
        };
    }
}
