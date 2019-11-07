//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.h
// Declaration of the Scenario2 class
//

#pragma once
#include "Scenario2.g.h"

namespace SDKSample
{
    namespace Projection
    {
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario2 sealed
        {
        public:
            Scenario2();
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:    
            void OnProjectionDisplayAvailableChanged(Platform::Object^ sender, Platform::Object^ e);
            void UpdateTextBlock(bool screenAvailable);
            Windows::Foundation::EventRegistrationToken displayChangeToken;
            Windows::UI::Core::CoreDispatcher^ dispatcher;
        };
    }
}
