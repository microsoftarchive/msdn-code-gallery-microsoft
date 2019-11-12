//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "ScannerContext.h"
#include "App.xaml.h"

using namespace SDKSample;
using namespace SDKSample::Common;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::Storage;

ScannerContext^ ModelDataContext::currentScanContext = nullptr;
Windows::Storage::StorageFolder^ ModelDataContext::destinationFolder = KnownFolders::PicturesLibrary;


/// <summary>
/// Constructor for ScannerContext object which maitains the list of the scanners that are present
/// </summary>
ScannerContext::ScannerContext()
{
    scannerInfoList = ref new Platform::Collections::Vector<ScannerDataItem^>();
    watcherStarted = false;
    
    watcher = nullptr;
    InitScannerWatcher();

     // Register for app suspend/resume handlers
    App::Current->Suspending += ref new SuspendingEventHandler(this, &ScannerContext::SuspendWatcher);
    App::Current->Resuming += ref new EventHandler<Object^>(this, &ScannerContext::ResumeWatcher);
}

/// <summary>
/// Destructor for ScannerContext object
/// </summary>
ScannerContext::~ScannerContext()
{
    if (watcherStarted)
    {     
        StopScannerWatcher();
    }
}

/// <summary>
/// Start Watcher for scanner devices
/// </summary>
void ScannerContext::StartScannerWatcher()
{
    std::for_each(
    begin(scannerInfoList), 
    end(scannerInfoList), 
        [](ScannerDataItem^ item) 
        {
            item->IsMatched = false;
        }
    );
    watcher->Start();
    watcherStarted = true;
}

/// <summary>
/// Stop Watcher for scanner devices
/// </summary>
void ScannerContext::StopScannerWatcher()
{
    scannerInfoList->Clear();
    watcher->Stop();
    watcherStarted = false;
}

/// <summary>
/// Initializes the watcher which is used for enumerating scanners
/// </summary>
void ScannerContext::InitScannerWatcher()
{
    // Create a Device Watcher class for type Image Scanner for enumerating scanners
    watcher = DeviceInformation::CreateWatcher(DeviceClass::ImageScanner);

    // Register for added, removed and enumeration completed events
    watcher->Added += ref new TypedEventHandler<DeviceWatcher^, DeviceInformation^>(this, &ScannerContext::OnScannerAdded);
    watcher->Removed += ref new TypedEventHandler<DeviceWatcher^, DeviceInformationUpdate^>(this, &ScannerContext::OnScannerRemoved);
    watcher->EnumerationCompleted += ref new TypedEventHandler<DeviceWatcher^,Object^>(this, &ScannerContext::OnScannerEnumerationComplete);
}

/// <summary>
/// Event handler for arrival of scanner devices.  If the scanner isn't already in the
/// list then it's added.  If it is already in the list, then the verified property
/// is set to true so that onScannerEnumerationComplete() knows the device is still present.
/// </summary>
/// <param name="sender"></param>
/// <param name="deviceInfo">The device info for the device which was added</param>
void ScannerContext::OnScannerAdded(_In_ WDE::DeviceWatcher^ sender, _In_ WDE::DeviceInformation^ deviceInfo)
{
    MainPage::Current->Dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler(
            [this, deviceInfo] () -> void 
            {
                MainPage::Current->NotifyUser("Scanner with device id " + deviceInfo->Id + " has been added", NotifyType::StatusMessage);

                // search the device list for a device with a matching device id
                auto match = FindDevice(deviceInfo->Id);

                // If we found a match then mark it as verified and return
                if (match != nullptr) 
                {
                    match->IsMatched = true;
                    return;
                }
                AppendToList(deviceInfo);
            }
        )
    );
}

/// <summary>
/// Event handler for removal of an scanner device.  If the device is in the list, it clears
/// </summary>
/// <param name="sender"></param>
/// <param name="deviceInfo">The device info for the device which was added</param>
void ScannerContext::OnScannerRemoved(_In_ WDE::DeviceWatcher^ sender, _In_ WDE::DeviceInformationUpdate^ deviceInfo)
{
    auto deviceId = deviceInfo->Id;
    MainPage::Current->Dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler(
            [this, deviceId] () -> void 
            {
                MainPage::Current->NotifyUser("Scanner with device id " + deviceId + " has been removed", NotifyType::StatusMessage);
                auto i = std::remove_if(begin(scannerInfoList), 
                                end(scannerInfoList),
                                [deviceId](ScannerDataItem^ item) {return item->Id == deviceId;});

                // if there's no match return.
                if (i == end(scannerInfoList))
                {
                    return;
                }
                MainPage::Current->NotifyUser("Scanner with device id " + deviceId + " has been removed from list", NotifyType::StatusMessage);
                RemoveFromListAtEnd();
            }
        )
    );
   
}

/// <summary>
/// Event handler for the end of the enumeration triggered when starting the watcher
/// This calls onScannerEnumerationComplete() to purge the scanner list of any scanners which
/// are no longer present 
/// </summary>
/// <param name="sender"></param>
/// <param name="deviceInfo">The device info for the device which was added</param>
void ScannerContext::OnScannerEnumerationComplete(_In_ WDE::DeviceWatcher^ sender, _In_ Platform::Object^ args)
{
    MainPage::Current->Dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler(
            [this] () {
                MainPage::Current->NotifyUser("Enumeration of scanners is complete", NotifyType::StatusMessage);

                // Move all the unmatched elements to the end of the list
                auto i = std::remove_if(begin(scannerInfoList), 
                                        end(scannerInfoList),
                                        [](ScannerDataItem^ e) { return e->IsMatched == true;});

                // Determine the number of unmatched entries
                auto unmatchedCount = end(scannerInfoList) - i;
                
                while (unmatchedCount > 0) 
                {
                    scannerInfoList->RemoveAtEnd();
                    unmatchedCount -= 1;
                }
            }
        )
    );
}

/// <summary>
/// Finds the ScanDataItem instance for a specified id in the list of ScanDataItem vector
/// </summary>
/// <param name="Id">device id for which scanner item needs to be found</param>
ScannerDataItem^ ScannerContext::FindDevice(_In_ String^ id)
{
    auto i = std::find_if(begin(scannerInfoList), 
                          end(scannerInfoList), 
                          [id](ScannerDataItem^ e) {return e->Id == id;});
    if (i == end(scannerInfoList)) 
    {
        return nullptr;
    } 
    else 
    {
        return *i;
    }
}

/// <summary>
/// Appends a SannerDataItem to the list with the given device information
/// </summary>
/// <param name="deviceInfo">device information of the scanner that is to be added</param>
void ScannerContext::AppendToList(_In_ WDE::DeviceInformation^ deviceInfo)
{
    scannerInfoList->Append(ref new ScannerDataItem(deviceInfo));
    OnPropertyChanged("ScannerListSize");
    // Set the current scanner id if it was not previously set before.
    if (currentScannerDeviceId == nullptr) 
    {
        CurrentScannerDeviceId = deviceInfo->Id;
    }
}

/// <summary>
/// Removes a ScannerDataItem at the end of the list, and updated the current scanner id is if
/// the scanner device that is removed the current selected scanner.
/// </summary>
void  ScannerContext::RemoveFromListAtEnd()
{
    String^ deviceId = scannerInfoList->GetAt(scannerInfoList->Size - 1)->Id;

    // If currently selected scanner is being removed, make sure
    // to select a different one
    if (currentScannerDeviceId == deviceId) 
    {
        String^ scannerDeviceIdToSelect = (scannerInfoList->Size == 0) ? 
                                     nullptr :
                                     scannerInfoList->GetAt(0)->Id;

        CurrentScannerDeviceId = scannerDeviceIdToSelect;
    }
    scannerInfoList->RemoveAtEnd();
    OnPropertyChanged("ScannerListSize");
}



/// <summary>
/// Event handler to suspend watcher when application is suspended
/// </summary>
void ScannerContext::SuspendWatcher(_In_ Object^ Sender, _In_ Windows::ApplicationModel::SuspendingEventArgs^ Args)
{
    if (WatcherStarted) 
    {
        watcherSuspended = true;
        StopScannerWatcher();
    } 
    else 
    {
        watcherSuspended = false;
    }
}

/// <summary>
/// Event handler to resume watcher when application is started
/// </summary>
void ScannerContext::ResumeWatcher(_In_ Object^ Sender, _In_ Object^ Args)
{
    if (watcherSuspended)
    {
        watcherSuspended = false;
        StartScannerWatcher();
    }
}
