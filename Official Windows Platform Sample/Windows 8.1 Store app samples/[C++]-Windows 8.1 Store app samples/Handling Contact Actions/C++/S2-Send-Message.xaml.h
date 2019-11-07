//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S2-Send-Message.xaml.h
// Declaration of the SendMessageScenario class
//

#pragma once
#include "S2-Send-Message.g.h"

namespace SDKSample
{
    namespace ContactActions
    {
        /// <summary>
        /// A page for 'Handling an activation to send a message' scenario that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class SendMessageScenario sealed
        {
        public:
            SendMessageScenario();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        };
    }
}
