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
// Scenario7.xaml.h
// Declaration of the Scenario7 class
//

#pragma once

#include "pch.h"
#include "Scenario7.g.h"
#include "MainPage.xaml.h"
#include "Team.h"

namespace SDKSample
{
    namespace DataBinding
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario7 sealed
        {
        public:
            Scenario7();
    		property Teams^ _observableTeams;
    
        protected:
            void BtnRemoveTeam_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void VectorChanged(Windows::Foundation::Collections::IObservableVector<Object^>^ sender, Windows::Foundation::Collections::IVectorChangedEventArgs^ e);
    		void Scenario7Reset(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        private:
            MainPage^ rootPage;
            
            
        };
    }
}
