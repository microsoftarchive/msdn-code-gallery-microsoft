//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

namespace BackgroundTask
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class UpdateFirmwareTask sealed : public Windows::ApplicationModel::Background::IBackgroundTask
    {
    public:
        UpdateFirmwareTask();

        virtual void Run(Windows::ApplicationModel::Background::IBackgroundTaskInstance^ taskInstance);
    private:
        Windows::ApplicationModel::Background::IBackgroundTaskInstance^ backgroundTaskInstance;

        Windows::Devices::Background::DeviceServicingDetails^ deviceServicingDetails;

        Windows::Devices::Usb::UsbDevice^ device;

        Concurrency::cancellation_token_source cancellationTokenSource;

        void CloseDevice();

        Concurrency::task<void> OpenDeviceAsync();

        Concurrency::task<uint32> ResetDeviceAsync();

        Concurrency::task<void> SetupDeviceForFirmwareUpdateAsync();

        Concurrency::task<uint32> WriteFirmwareAsync();

        void OnCanceled(Windows::ApplicationModel::Background::IBackgroundTaskInstance^ sender, Windows::ApplicationModel::Background::BackgroundTaskCancellationReason reason);

        Concurrency::task<uint32> WriteSectorAsync(FirmwareSector^ firmwareSector);

        uint32 GetTotalFirmwareSize();
    };
}