//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// ReceiveUri.xaml.h
// Declaration of the ReceiveUri class
//

#pragma once
#include "Scenario4_ReceiveUri.g.h"

namespace SDKSample
{
    namespace AssociationLaunching
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
       [Windows::Foundation::Metadata::WebHostHidden]
        public ref class ReceiveUri sealed
        {
        public:
            ReceiveUri();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        };
    }
}
