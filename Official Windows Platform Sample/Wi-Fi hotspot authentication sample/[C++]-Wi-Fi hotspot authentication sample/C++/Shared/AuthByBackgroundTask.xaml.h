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
// AuthByBackgroundTask.xaml.h
// Declaration of the AuthByBackgroundTask class
//

#pragma once

#include "pch.h"
#include "AuthByBackgroundTask.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace HotspotAuthenticationApp
    {
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class AuthByBackgroundTask sealed
        {
        public:
            AuthByBackgroundTask();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
        };
    }
}
