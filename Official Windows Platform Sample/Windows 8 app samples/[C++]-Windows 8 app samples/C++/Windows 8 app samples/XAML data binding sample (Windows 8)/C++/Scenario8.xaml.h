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
// Scenario8.xaml.h
// Declaration of the Scenario8 class
//

#pragma once

#include "pch.h"
#include "Scenario8.g.h"
#include "MainPage.xaml.h"
#include "Employee.h"

namespace SDKSample
{
    namespace DataBinding
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario8 sealed
        {
        public:
            Scenario8();
    		property Teams^ _observableTeams;

        protected:
    		void Scenario8Reset(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    		void BindableVectorChanged(Windows::UI::Xaml::Interop::IBindableObservableVector^ sender, Object^ e);

        private:
            MainPage^ rootPage;
    		GeneratorIncrementalLoadingClass^ employees;
    		Windows::Foundation::EventRegistrationToken vectorChangedHandlerToken;
        
        };
    }
}
