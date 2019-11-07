//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1_FirmwareUpdate.xaml.h
// Declaration of the FirmwareUpdate class
//

#pragma once
#include "Scenario1_FirmwareUpdate.g.h"

namespace SDKSample
{
    namespace FirmwareUpdateUsbDevice
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class FirmwareUpdate sealed
        {
        public:
            FirmwareUpdate();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ eventArgs) override;

        private:
            Windows::ApplicationModel::Background::DeviceServicingTrigger^ firmwareUpdateBackgroundTaskTrigger;
            Windows::ApplicationModel::Background::BackgroundTaskRegistration^ firmwareUpdateBackgroundTaskRegistration;

            bool isUpdatingFirmware;

            Concurrency::task<Windows::Devices::Enumeration::DeviceInformation^> FindFirstSuperMuttDeviceAsync();

            Concurrency::task<Platform::String^> StartFirmwareForDeviceAsync(Windows::Devices::Enumeration::DeviceInformation^ deviceInformation);

            void CancelFirmwareUpdate();

            Concurrency::task<void> UpdateFirmwareForFirstEnumeratedDeviceAsync();

            void RegisterForFirmwareUpdateBackgroundTask();
            void RegisterFirmwareUpdateBackgroundTaskCallbacks(Windows::ApplicationModel::Background::BackgroundTaskRegistration^ backgroundTaskRegistration);

            Windows::ApplicationModel::Background::BackgroundTaskRegistration^ FindFirmwareUpdateTask();

            void OnFirmwareUpdateCompleted(Windows::ApplicationModel::Background::BackgroundTaskRegistration^ sender, Windows::ApplicationModel::Background::BackgroundTaskCompletedEventArgs^ args);
            void OnFirmwareUpdateProgress(Windows::ApplicationModel::Background::BackgroundTaskRegistration^ sender, Windows::ApplicationModel::Background::BackgroundTaskProgressEventArgs^ args);

            void UpdateFirmwareOnFirstEnumeratedDevice_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void CancelFirmwareUpdate_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void UpdateButtonStates();
            void UpdateOldFirmwareVersionInUI(Platform::String^ version);
            void UpdateNewFirmwareVersionInUI(Platform::String^ version);
        };
    }
}
