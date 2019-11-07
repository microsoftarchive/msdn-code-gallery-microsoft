//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario7_SyncDevice.xaml.h
// Declaration of the SyncDevice class
//

#pragma once
#include "Scenario7_SyncDevice.g.h"

namespace SDKSample
{
    namespace CustomUsbDeviceAccess
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class SyncDevice sealed
        {
        public:
            SyncDevice(void);

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ eventArgs) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ eventArgs) override;

        private:
            Windows::Devices::Enumeration::DeviceInformation^ syncDeviceInformation;
            Platform::String^ syncDeviceSelector;

            Windows::ApplicationModel::Background::BackgroundTaskRegistration^ backgroundSyncTaskRegistration;
            Windows::ApplicationModel::Background::DeviceUseTrigger^ syncBackgroundTaskTrigger;

            Windows::Foundation::EventRegistrationToken syncCompletedEventToken;
            Windows::Foundation::EventRegistrationToken syncProgressEventToken;
            bool isSyncing;

            void CancelSyncWithDevice(void);
            Concurrency::task<void> SyncWithDeviceAsync(void);
            Concurrency::task<Windows::ApplicationModel::Background::DeviceTriggerResult> StartSyncBackgroundTaskAsync(void);
            void SetupBackgroundTask(void);

            Windows::ApplicationModel::Background::BackgroundTaskRegistration^ FindSyncTask();

            void OnSyncWithDeviceCompleted(
                Windows::ApplicationModel::Background::BackgroundTaskRegistration^ sender, 
                Windows::ApplicationModel::Background::BackgroundTaskCompletedEventArgs^ args);

            void OnSyncWithDeviceProgress(
                Windows::ApplicationModel::Background::BackgroundTaskRegistration^ sender, 
                Windows::ApplicationModel::Background::BackgroundTaskProgressEventArgs^ args);

            void Sync_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void CancelSync_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void UpdateButtonStates(void);
        };
    }
}
