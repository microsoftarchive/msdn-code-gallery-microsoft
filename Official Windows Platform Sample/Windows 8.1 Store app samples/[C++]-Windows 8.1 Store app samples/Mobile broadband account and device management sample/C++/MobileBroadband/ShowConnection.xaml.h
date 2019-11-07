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
// ShowConnection.xaml.h
// Declaration of the ShowConnection class
//

#pragma once

#include "pch.h"
#include "ShowConnection.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace MobileBroadband
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class ShowConnection sealed
        {
        public:
            ShowConnection();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            Windows::Foundation::Collections::IVectorView<Platform::String^>^ deviceAccountId;

            void ShowConnectionUI_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void PrepareScenario();
        };
    }
}