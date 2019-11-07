//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario4_BulkPipes.xaml.h
// Declaration of the Scenario4_BulkPipes class
//

#pragma once
#include "Scenario4_BulkPipes.g.h"

namespace SDKSample
{
    namespace CustomUsbDeviceAccess
    {
        /// <summary>
        /// This page demonstrates how to propertly use bulk pipes to read and write to the device.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class BulkPipes sealed
        {
        public:
            BulkPipes(void);

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ eventArgs) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ eventArgs) override;

        private:
            Concurrency::cancellation_token_source cancellationTokenSource;
            Concurrency::cancellation_token_registration cancellationTokenRegistration;

            bool runningReadTask;
            bool runningWriteTask;
            bool runningReadWriteTask;

            bool stopNewIo;

            SRWLOCK cancelIoLock;

            uint32 totalBytesWritten;
            uint32 totalBytesRead;

            // Did we navigate away from this page?
            bool navigatedAway;

            void BulkRead_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void BulkWrite_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void BulkReadWrite_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void CancelAllIoTasks_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            
            void OnAppSuspension(Platform::Object^ sender, Windows::ApplicationModel::SuspendingEventArgs^ args);
            void OnDeviceConnected(EventHandlerForDevice^ sender, Windows::Devices::Enumeration::DeviceInformation^ deviceInformation);

            void UpdateButtonStates(void);

            Concurrency::task<void> BulkWriteAsync(uint32 bulkPipeIndex, uint32 bytesToWrite, Concurrency::cancellation_token cancellationToken);
            Concurrency::task<void> BulkReadAsync(uint32 bulkPipeIndex, uint32 bytesToRead, Concurrency::cancellation_token cancellationToken);

            Concurrency::task<void> BulkReadWriteLoopAsync(uint32 bulkInPipeIndex, uint32 bulkOutPipeIndex, uint32 bytesToReadWrite);

            void CancelAllIoTasks(void);

            bool IsPerformingIo(void);

            void PrintTotalReadWriteBytes(void);
            
            void ResetCancellationTokenSource(void);

            void NotifyCancelingTask(void);
            void NotifyTaskCanceled(void);
        };
    }
}
