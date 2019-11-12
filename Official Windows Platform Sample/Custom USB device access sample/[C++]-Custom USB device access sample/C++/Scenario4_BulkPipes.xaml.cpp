//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario4_BulkPipes.xaml.cpp
// Implementation of the Scenario4_BulkPipes class
//

#include "pch.h"
#include "Scenario4_BulkPipes.xaml.h"
#include "MainPage.xaml.h"

using namespace Platform;
using namespace Platform::Collections;
using namespace Concurrency;
using namespace Windows::ApplicationModel;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Devices::Usb;
using namespace Windows::Foundation;
using namespace Windows::Storage::Streams;
using namespace Windows::UI;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace SDKSample;
using namespace SDKSample::Common;
using namespace SDKSample::CustomUsbDeviceAccess;

BulkPipes::BulkPipes(void) :
    totalBytesRead(0),
    totalBytesWritten(0),
    runningReadTask(false),
    runningWriteTask(false),
    runningReadWriteTask(false),
    navigatedAway(false)
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
///
/// We will enable/disable parts of the UI if the device doesn't support it.
/// Will will also register a callback for whenever we cancel a task because we want
/// to prevent the user from doing anymore IO (disable buttons to prevent user IO)
/// </summary>
/// <param name="eventArgs">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void BulkPipes::OnNavigatedTo(NavigationEventArgs^ /* eventArgs */)
{
    navigatedAway = false;

    InitializeSRWLock(&cancelIoLock);

    // Both the OSRFX2 and the SuperMutt use the same scenario
    // If no devices are connected, none of the scenarios will be shown and an error will be displayed
    Map<DeviceType, UIElement^>^ deviceScenarios = ref new Map<DeviceType, UIElement^>();
    deviceScenarios->Insert(DeviceType::OsrFx2, GeneralScenario);
    deviceScenarios->Insert(DeviceType::SuperMutt, GeneralScenario);

    Utilities::SetUpDeviceScenarios(deviceScenarios, DeviceScenarioContainer);
    
    EventHandlerForDevice::Current->OnAppSuspendCallback = ref new SuspendingEventHandler(this, &BulkPipes::OnAppSuspension);

    // Reset the buttons if the app resumed and the device is reconnected
    EventHandlerForDevice::Current->OnDeviceConnected = ref new TypedEventHandler<EventHandlerForDevice^, DeviceInformation^>(this, &BulkPipes::OnDeviceConnected);

    // So we can reset future tasks
    ResetCancellationTokenSource();

    UpdateButtonStates();
}

/// <summary>
/// Cancel any on going tasks when navigating away from the page so the device is in a consistent state throughout
/// all the scenarios
/// </summary>
/// <param name="eventArgs"></param>
void BulkPipes::OnNavigatedFrom(NavigationEventArgs^ /* eventArgs */) 
{
    navigatedAway = true;

    CancelAllIoTasks();

    // We don't need to worry about app suspend for this scenario anymore
    EventHandlerForDevice::Current->OnAppSuspendCallback = nullptr;
    EventHandlerForDevice::Current->OnDeviceConnected = nullptr;

    cancellationTokenSource.get_token().deregister_callback(cancellationTokenRegistration);
}

void BulkPipes::BulkRead_Click(Object^ /* sender */, RoutedEventArgs^ /* e */)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        MainPage::Current->NotifyUser("Reading...", NotifyType::StatusMessage);

        // Both supported devices have the bulk in pipes on index 0
        uint32 bulkInPipeIndex = 0;

        // Read as much data as possible in one packet
        uint32 bytesToRead = 512;  

        runningReadTask = true;

        // Save the cancellation token now in case the cancellationTokenSource is reset while IO is going on.
        auto cancellationToken = cancellationTokenSource.get_token();

        // Re-enable read button after completing the read
        BulkReadAsync(bulkInPipeIndex, bytesToRead, cancellationToken).then([this](task<void> readTask)
        {
            try
            {
                runningReadTask = false;

                UpdateButtonStates();

                // May throw exception if there's an error reading data or if the task is cancelled
                readTask.get(); 
            }
            catch (const task_canceled& /* taskCanceled */)
            {
                NotifyTaskCanceled();
            }
        });

        // Disable buttons to prevent multiple reads if the read has not completed
        UpdateButtonStates();
    }
    else
    {
        Utilities::NotifyDeviceNotConnected();
    }
}

void BulkPipes::BulkWrite_Click(Object^ /* sender */, RoutedEventArgs^ /* e */)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        MainPage::Current->NotifyUser("Writing...", NotifyType::StatusMessage);

        // Both supported devices have the bulk out pipes on index 0
        uint32 bulkOutPipeIndex = 0;

        // Write as much data as possible in one packet
        uint32 bytesToWrite = 512;  

        runningWriteTask = true;

        // Save the cancellation token now in case the cancellationTokenSource is reset while IO is going on.
        auto cancellationToken = cancellationTokenSource.get_token();

        // Re-enable write button after completing the write
        BulkWriteAsync(bulkOutPipeIndex, bytesToWrite, cancellationToken).then([this](task<void> writeTask)
        {
            try
            {
                runningWriteTask = false;

                UpdateButtonStates();

                // May throw an exception if there's an error writing or if the task is cancelled
                writeTask.get();    
            }
            catch (const task_canceled& /* taskCanceled */)
            {
                NotifyTaskCanceled();
            }
        });

        // Disable buttons to prevent multiple writes if the  write async has not completed
        UpdateButtonStates();
    }
    else
    {
        Utilities::NotifyDeviceNotConnected();
    }
}

void BulkPipes::BulkReadWrite_Click(Object^ /* sender */, RoutedEventArgs^ /* e */)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        MainPage::Current->NotifyUser("Reading/Writing...", NotifyType::StatusMessage);

        // We need to set this to true so that the buttons can be updated to disable the read and write button, 
        // but enable the individual read/write buttons. We will not be able to update the button states until after the read/write completes.
        runningReadWriteTask = true;
        UpdateButtonStates();

        // Both supported devices have the bulk in/out pipes on index 0
        uint32 bulkOutPipeIndex = 0;
        uint32 bulkInPipeIndex = 0;

        // Write as much data as possible in one packet
        uint32 bytesToWrite = 512;  

        // Re-enable read/write buttons after completing the read/write
        BulkReadWriteLoopAsync(bulkInPipeIndex, bulkOutPipeIndex, bytesToWrite).then([this](task<void> readWriteTask)
        {
            try
            {
                runningReadWriteTask = false;

                UpdateButtonStates();

                // May throw exception if there was a problem reading/writing or if the task was canceled
                readWriteTask.get();    

                // This line should not be reached unless the bulk read write loops stopped because the device was disconnected.
                MainPage::Current->Dispatcher->RunAsync(
                    CoreDispatcherPriority::Normal,
                    ref new DispatchedHandler([this]()
                    {    
                        // If we navigated away from this page, do not print anything. The dispatch may be handled after
                        // we move to a different page.
                        if (!navigatedAway)
                        {
                            MainPage::Current->NotifyUser(
                                "Device was disconnected before we could read/write to it",
                                NotifyType::ErrorMessage);
                        }
                    }));
            }
            catch (const task_canceled& /* taskCanceled */)
            {
                NotifyTaskCanceled();
            }
        });
    }
    else
    {
        Utilities::NotifyDeviceNotConnected();
    }
}

void BulkPipes::CancelAllIoTasks_Click(Object^ /* sender */, RoutedEventArgs^ /* e */)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        CancelAllIoTasks();
    }
    else
    {
        Utilities::NotifyDeviceNotConnected();
    }
}

/// <summary>
/// It is important to be able to cancel tasks that may take a while to complete. Canceling tasks is the only way to stop any pending IO
/// operations asynchronously. If the UsbDevice is closed/deleted while there are pending IOs, the destructor will cancel all pending IO 
/// operations.
/// </summary>
void BulkPipes::CancelAllIoTasks(void)
{
    // This variable is used to prevent other methods from taking a shared lock, which can eventually starve our exclusive lock acquire
    stopNewIo = true;

    AcquireSRWLockExclusive(&cancelIoLock);

    // Stop newly called IO methods from taking a shared lock so that we can get the exclusive lock
    if (!cancellationTokenSource.get_token().is_canceled())
    {
        cancellationTokenSource.cancel();

        // Existing IO already has a local copy of the old cancellation token so this reset won't affect its
        ResetCancellationTokenSource();
    }

    ReleaseSRWLockExclusive(&cancelIoLock);
}

/// <summary>
/// Determines if we are reading, writing, or reading and writing.
/// </summary>
/// <returns>If we are doing any of the above operations, we return true; false otherwise</returns>
bool BulkPipes::IsPerformingIo(void)
{
    return (runningReadTask || runningWriteTask || runningReadWriteTask);
}

/// <summary>
/// Will write garbage data to the specified output pipe. Since writing to the device may take a while to complete,
/// we provide the write async with a known cancellation token to cancel any pending or running writes.
///
/// Any errors in async function will be passed down the task chain and will not be caught here because errors should be 
/// handled at the end of the task chain.
/// </summary>
/// <param name="pipeIndex">Index of pipe in the list of Device->DefaultInterface->BulkOutPipes</param>
/// <param name="bytesToWrite">Bytes of garbage data to write</param>
/// <param name="cancellationToken">Cancellation token to associate the task with</param>
task<void> BulkPipes::BulkWriteAsync(uint32 bulkPipeIndex, uint32 bytesToWrite, cancellation_token cancellationToken)
{
    return task<void>([this, bulkPipeIndex, bytesToWrite, cancellationToken]()
    {
        // Create an array, all default initialized to 0, and write it to the buffer
        // The data inside the buffer will be garbage
        auto arrayBuffer = ref new Array<uint8>(bytesToWrite);

        auto stream = EventHandlerForDevice::Current->Device->DefaultInterface->BulkOutPipes->GetAt(bulkPipeIndex)->OutputStream;

        auto writer = ref new DataWriter(stream);
        writer->WriteBytes(arrayBuffer);

        // Stop new IO if the cancel thread is waiting for the SRW lock
        if (stopNewIo)
        {
            cancel_current_task();
        }

        AcquireSRWLockShared(&cancelIoLock);

        if (cancellationToken.is_canceled())
        {
            cancel_current_task();
        }

        // This is where the data is flushed out to the device.
        //
        // Cancellation Token will be used so we can stop the task operation explicitly
        // The completion function should still be called so that we can properly handle a canceled task
        task<uint32> storeAsyncTask = create_task(writer->StoreAsync(), cancellationToken);

        ReleaseSRWLockShared(&cancelIoLock);

        return storeAsyncTask.then([this](task<uint32> bytesWrittenTask)
        {
            auto bytesWritten = bytesWrittenTask.get();

            totalBytesWritten += bytesWritten;

            PrintTotalReadWriteBytes();
        });
    }, cancellationToken);

}

/// <summary>
/// Will read data from the specified input pipe. The data being read is garbage data because the samples devices are giving us garbage data.
/// We use cancellation tokens to cancel reads that are pending or running.
///
/// Any errors in async function will be passed down the task chain and will not be caught here because errors should be 
/// handled at the end of the task chain.
/// </summary>
/// <param name="pipeIndex">Index of pipe in the list of Device->DefaultInterface->BulkInPipes</param>
/// <param name="bytesToRead">Bytes of garbage data to read</param>
/// <param name="cancellationToken">Cancellation token to associate the task with</param>
task<void> BulkPipes::BulkReadAsync(uint32 bulkPipeIndex, uint32 bytesToRead, cancellation_token cancellationToken)
{
    return task<void>([this, bulkPipeIndex, bytesToRead, cancellationToken]()
    {
        auto stream = EventHandlerForDevice::Current->Device->DefaultInterface->BulkInPipes->GetAt(bulkPipeIndex)->InputStream;

        DataReader^ reader = ref new DataReader(stream);

        // Stop new IO if the cancel thread is waiting for the SRW lock
        if (stopNewIo)
        {
            cancel_current_task();
        }

        AcquireSRWLockShared(&cancelIoLock);

        if (cancellationToken.is_canceled())
        {
            cancel_current_task();
        }

        // Cancellation Token will be used so we can stop the task operation explicitly
        // The completion function should still be called so that we can properly handle a canceled task
        task<uint32> loadAsyncTask = create_task(reader->LoadAsync(bytesToRead), cancellationToken);

        ReleaseSRWLockShared(&cancelIoLock);

        return loadAsyncTask.then([this, reader](task<uint32> bytesReadTask)
        {
            auto bytesRead = bytesReadTask.get();

            totalBytesRead += bytesRead;

            PrintTotalReadWriteBytes();

            // The data that is read is stored in the reader object
            // e.g. To read a string from the buffer:
            // reader->ReadString(bytesRead);   
        });
    }, cancellationToken);
}

/// <summary>
/// A read and a write will be initiated simultaneously. Reads and writes are looped; after each read/write succeeds, another read/write is initiated.
///
/// Any errors in async function will be passed down the task chain and will not be caught here because errors should be 
/// handled at the end of the task chain.
/// </summary>
/// <param name="bulkInPipeIndex">Index of pipe in the list of Device->DefaultInterface->BulkInPipes</param>
/// <param name="bulkOutPipeIndex">Index of pipe in the list of Device->DefaultInterface->BulkOutPipes</param>
/// <param name="bytesToReadWrite">Bytes of garbage data to read/write</param>
task<void> BulkPipes::BulkReadWriteLoopAsync(uint32 bulkInPipeIndex, uint32 bulkOutPipeIndex, uint32 bytesToReadWrite)
{
    return task<void>([this, bulkInPipeIndex, bulkOutPipeIndex, bytesToReadWrite]()
    {
        // Save the cancellation token now in case the cancellationTokenSource is reset while IO is going on.
        auto cancellationToken = cancellationTokenSource.get_token();

        task_group readWriteTasks(cancellationToken);
    
        readWriteTasks.run([this, &readWriteTasks, bulkInPipeIndex, bytesToReadWrite, cancellationToken]()
        {
            // While this task is not cancelled, initiate one read after one succeeds
            while (!readWriteTasks.is_canceling() && EventHandlerForDevice::Current->IsDeviceConnected)
            {
                auto readTask = BulkReadAsync(bulkInPipeIndex, bytesToReadWrite, cancellationToken);

                readTask.wait();

                // Allow exceptions to be thrown
                readTask.get();
            }
        });

        readWriteTasks.run([this, &readWriteTasks, bulkOutPipeIndex, bytesToReadWrite, cancellationToken]()
        {
            while (!readWriteTasks.is_canceling() && EventHandlerForDevice::Current->IsDeviceConnected)
            {
                // While this task is not cancelled, initiate one write after one succeeds
                auto writeTask = BulkWriteAsync(bulkOutPipeIndex, bytesToReadWrite, cancellationToken);

                writeTask.wait();

                // Allow exceptions to be thrown
                writeTask.get();
            }
        });

        // Wait until all the tasks complete before leaving
        readWriteTasks.wait();
    }, cancellationTokenSource.get_token());
}

void BulkPipes::PrintTotalReadWriteBytes(void)
{
    MainPage::Current->Dispatcher->RunAsync(
        CoreDispatcherPriority::Low,
        ref new DispatchedHandler([this]()
        {    
            // If we navigated away from this page, do not print anything. The dispatch may be handled after
            // we move to a different page.
            if (!navigatedAway)
            {
                MainPage::Current->NotifyUser(
                    "Total bytes read: " + totalBytesRead.ToString() + "; Total bytes written: " + totalBytesWritten.ToString(),
                    NotifyType::StatusMessage);
            }
        }));
}

/// <summary>
/// Print a status message saying we are canceling a task and disable all buttons to prevent multiple cancel requests.
/// <summary>
void BulkPipes::NotifyCancelingTask(void)
{
    // Setting the dispatcher priority to high allows the UI to handle disabling of all the buttons
    // before any of the IO completion callbacks get a chance to modify the UI; that way this method
    // will never get the opportunity to overwrite UI changes made by IO callbacks
    MainPage::Current->Dispatcher->RunAsync(
        CoreDispatcherPriority::High,
        ref new DispatchedHandler([this]()
        {    
            ButtonBulkRead->IsEnabled = false;
            ButtonBulkWrite->IsEnabled = false;
            ButtonBulkReadWrite->IsEnabled = false;
            ButtonCancelAllIoTasks->IsEnabled = false;

            if (!navigatedAway)
            {
                MainPage::Current->NotifyUser("Canceling task... Please wait...", NotifyType::StatusMessage);
            }
        }));
}
    
/// <summary>
/// Notifies the UI that the operation has been cancelled
/// </summary>
void BulkPipes::NotifyTaskCanceled(void)
{
    MainPage::Current->Dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler([this]()
        {
            if (!navigatedAway)
            {
                MainPage::Current->NotifyUser("The read or write operation has been cancelled", NotifyType::StatusMessage);
            }
        }));
}

/// <summary>
/// Allow for one operation at a time
/// </summary>
void BulkPipes::UpdateButtonStates(void)
{
    MainPage::Current->Dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler([this]()
        {    
            ButtonBulkReadWrite->IsEnabled = !IsPerformingIo();
            ButtonBulkRead->IsEnabled = !runningReadWriteTask && !runningReadTask;
            ButtonBulkWrite->IsEnabled = !runningReadWriteTask && !runningWriteTask;
            ButtonCancelAllIoTasks->IsEnabled = IsPerformingIo();
        }));
}

/// <summary>
/// Stop any pending IO operations because the device will be closed when the app suspends
/// </summary>
/// <param name="sender"></param>
/// <param name="eventArgs"></param>
void BulkPipes::OnAppSuspension(Object^ /* sender */, SuspendingEventArgs^ /* args */)
{
    CancelAllIoTasks();
}

/// <summary>
/// Reset the buttons when the device is reopened
/// </summary>
/// <param name="sender"></param>
/// <param name="deviceInformation"></param>
void BulkPipes::OnDeviceConnected(EventHandlerForDevice ^ /* sender */, DeviceInformation ^ /* deviceInformation */)
{
    UpdateButtonStates();
}

void BulkPipes::ResetCancellationTokenSource(void)
{
    stopNewIo = false;

    // Create a new cancellation token source so that can cancel all the tokens again
    cancellationTokenSource = cancellation_token_source();

    // Register a callback to when a task is being canceled
    // Calling the notify cancellation method inside a lambda function is required otherwise the NotifyCancelingTask
    // would not have access to the "this" pointer, which is important because the method will be modifying the
    // UI.
    cancellationTokenRegistration = cancellationTokenSource.get_token().register_callback([this]()
    {
        NotifyCancelingTask();
    });
}
