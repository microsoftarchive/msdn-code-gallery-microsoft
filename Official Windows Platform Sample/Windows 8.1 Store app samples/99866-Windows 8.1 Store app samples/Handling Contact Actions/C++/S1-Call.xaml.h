//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S1-Call.xaml.h
// Declaration of the CallScenario class
//

#pragma once
#include "S1-Call.g.h"

namespace SDKSample
{
    namespace ContactActions
    {
        /// <summary>
        /// A page for 'Handling an activation to make a call' scenario that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class CallScenario sealed
        {
        public:
            CallScenario();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        };
    }
}
