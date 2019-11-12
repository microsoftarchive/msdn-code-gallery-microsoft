//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S2_Edges.xaml.h
// Declaration of the S2_Edges class
//

#pragma once
#include "S2_Edges.g.h"

namespace SDKSample
{
    namespace ApplicationViews
    {
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class S2_Edges sealed
        {
        public:
            S2_Edges();

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
