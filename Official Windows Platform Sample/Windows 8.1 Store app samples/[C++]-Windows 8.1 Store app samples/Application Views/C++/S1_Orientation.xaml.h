//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S1_Orientation.xaml.h
// Declaration of the S1_Orientation class
//

#pragma once
#include "S1_Orientation.g.h"

namespace SDKSample
{
    namespace ApplicationViews
    {
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class S1_Orientation sealed
        {
        public:
            S1_Orientation();

        protected:
            void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:  
            void WindowSizeChanged(Platform::Object ^sender, Windows::UI::Core::WindowSizeChangedEventArgs ^e);          
            void InvalidateLayout();
            Windows::Foundation::EventRegistrationToken sizeChangedToken;
        };
    }
}
