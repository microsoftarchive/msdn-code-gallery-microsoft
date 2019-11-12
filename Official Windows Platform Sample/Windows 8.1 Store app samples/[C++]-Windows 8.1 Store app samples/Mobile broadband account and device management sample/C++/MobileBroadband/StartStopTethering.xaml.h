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
// StartStopTethering.xaml.h
// Declaration of the StartStopTethering class
//

#pragma once

using namespace Windows::Foundation::Collections;
using namespace Windows::Networking::NetworkOperators;
using namespace Platform;

#include "pch.h"
#include "StartStopTethering.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace MobileBroadband
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class StartStopTethering sealed
        {
        public:
			StartStopTethering();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
			NetworkOperatorTetheringManager^ tetheringManager;
            Windows::Foundation::Collections::IVectorView<Platform::String^>^ deviceAccountId;
			Windows::UI::Core::CoreDispatcher^ sampleDispatcher;

            void PrepareScenario();
			void StartTetheringButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void StopTetheringButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void Apply_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void MakeTetheringManagerAvailable();
			void UpdateUI();
			void OnCapabilityError(TetheringCapability capabilities);
			String^ GetTetheringErrorString(TetheringOperationStatus errorCode);
			void OnCompleted(Windows::ApplicationModel::Background::BackgroundTaskRegistration^ sender, Windows::ApplicationModel::Background::BackgroundTaskCompletedEventArgs^ e);
		};
    }
}