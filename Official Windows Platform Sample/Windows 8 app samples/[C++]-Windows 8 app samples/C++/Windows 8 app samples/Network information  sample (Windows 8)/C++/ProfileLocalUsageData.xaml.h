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
// ProfileLocalUsageData.xaml.h
// Declaration of the ProfileLocalUsageData class
//

#pragma once

#include "pch.h"
#include "ProfileLocalUsageData.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace NetworkInformationApi
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class ProfileLocalUsageData sealed
        {
        public:
            ProfileLocalUsageData();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            void ProfileLocalUsageData_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
