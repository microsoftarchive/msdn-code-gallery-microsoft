//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S3-Map-Address.xaml.h
// Declaration of the MapAddressScenario class
//

#pragma once
#include "S3-Map-Address.g.h"

namespace SDKSample
{
    namespace ContactActions
    {
        /// <summary>
        /// A page for 'Handling an activation to map an address' scenario that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class MapAddressScenario sealed
        {
        public:
            MapAddressScenario();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        };
    }
}
