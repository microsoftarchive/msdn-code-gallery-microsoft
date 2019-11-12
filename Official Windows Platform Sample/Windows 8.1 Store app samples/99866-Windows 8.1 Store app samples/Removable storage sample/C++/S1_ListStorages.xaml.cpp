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
// S1_ListStorages.xaml.cpp
// Implementation of the S1_ListStorages class
//

#include "pch.h"
#include "S1_ListStorages.xaml.h"

using namespace SDKSample::RemovableStorageCPP;

using namespace concurrency;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

S1_ListStorages::S1_ListStorages() : rootPage(MainPage::Current)
{
    InitializeComponent();
}

/// <summary>
/// This is the click handler for the 'List Storages' button.
/// </summary>
/// <remarks>
/// There are two ways to find removable storages:
/// The first way uses the Removable Devices KnownFolder to get a snapshot of the currently
/// connected devices as StorageFolders.  This is demonstrated in this scenario.
/// The second way uses Windows.Devices.Enumeration and is demonstrated in the second scenario.
/// Windows.Devices.Enumeration supports more advanced scenarios such as subscibing for device
/// arrival, removal and updates. Refer to the DeviceEnumeration sample for details on
/// Windows.Devices.Enumeration.
/// </remarks>
/// <param name="sender"></param>
/// <param name="e"></param>
void S1_ListStorages::ListStorages_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ScenarioOutput->Text = "";

    // Find all storage devices using the known folder
    create_task(KnownFolders::RemovableDevices->GetFoldersAsync()).then([this] (IVectorView<StorageFolder^>^ removableStorages)
    {
        if (removableStorages->Size > 0)
        {
            // Display each storage device
            std::for_each(begin(removableStorages), end(removableStorages), [this](IStorageFolder^ storage)
            {
                ScenarioOutput->Text += safe_cast<StorageFolder^>(storage)->DisplayName + "\n";
            });
        }
        else
        {
            rootPage->NotifyUser("No removable storages were found. Please attach a removable storage to the system (e.g. a camera or camera memory)", NotifyType::StatusMessage);
        }
    });
}
