//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

namespace BackgroundTasks
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class IoSyncBackgroundTask sealed : public Windows::ApplicationModel::Background::IBackgroundTask
    {
    public:
        IoSyncBackgroundTask();

        virtual void Run(Windows::ApplicationModel::Background::IBackgroundTaskInstance^ taskInstance);
    private:
        Windows::ApplicationModel::Background::IBackgroundTaskInstance^ backgroundTaskInstance;
		Windows::Devices::Background::DeviceUseDetails^ deviceSyncDetails;

        Windows::Devices::Usb::UsbDevice^ device;

        Concurrency::cancellation_token_source cancellationTokenSource;

        Concurrency::task<void> OpenDeviceAsync();

        void OnCanceled(Windows::ApplicationModel::Background::IBackgroundTaskInstance^ sender, Windows::ApplicationModel::Background::BackgroundTaskCancellationReason reason);

        Concurrency::task<uint32> WriteToDeviceAsync();

        Platform::String^ GetDeviceId();
    };
}