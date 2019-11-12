//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// ReceiveFile.xaml.h
// Declaration of the ReceiveFile class
//

#pragma once
#include "Scenario3_ReceiveFile.g.h"

namespace SDKSample
{
    namespace AssociationLaunching
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class ReceiveFile sealed
        {
        public:
            ReceiveFile();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        };
    }
}
