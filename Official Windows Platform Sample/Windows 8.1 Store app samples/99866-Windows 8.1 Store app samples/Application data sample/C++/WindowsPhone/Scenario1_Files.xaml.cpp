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
// Files.xaml.cpp
// Implementation of the Files class
//

#include "pch.h"
#include "Scenario1_Files.xaml.h"

using namespace SDKSample::ApplicationDataSample;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Storage;
using namespace concurrency;

#define filename "sampleFile.txt"

Files::Files()
{
    InitializeComponent();
    
    localFolder = ApplicationData::Current->LocalFolder;
    localCounter = 0;

    localCacheFolder = ApplicationData::Current->LocalCacheFolder;
    localCacheCounter = 0;

    roamingFolder = ApplicationData::Current->RoamingFolder;
    roamingCounter = 0;

    temporaryFolder = ApplicationData::Current->TemporaryFolder;
    temporaryCounter = 0;

    DisplayOutput();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Files::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

// Guidance for Local, LocalCache, Roaming, and Temporary files.
//
// Files are ideal for storing large data-sets, databases, or data that is
// in a common file-format.
//
// Files can exist in either the Local, LocalCache, Roaming, or Temporary folders.
//
// Roaming files will be synchronized across machines on which the user has
// singed in with a connected account.  Roaming of files is not instant; the
// system weighs several factors when determining when to send the data.  Usage
// of roaming data should be kept below the quota (available via the 
// RoamingStorageQuota property), or else roaming of data will be suspended.
// Files cannot be roamed while an application is writing to them, so be sure
// to close your application's file objects when they are no longer needed.
//
// Local files are not synchronized, but are backed up, and can then be restored to a 
// machine different than where they were originally written. These should be for 
// important files that allow the feel that the user did not loose anything
// when they restored to a new device.
//
// Temporary files are subject to deletion when not in use.  The system 
// considers factors such as available disk capacity and the age of a file when
// determining when or whether to delete a temporary file.
//
// LocalCache files are for larger files that can be recreated by the app, and for
// machine specific or private files that should not be restored to a new device.


void Files::Increment_Local_Click(Object^ sender, RoutedEventArgs^ e)
{
    localCounter++;

    create_task(localFolder->CreateFileAsync(filename, CreationCollisionOption::ReplaceExisting)).then(
        [this](StorageFile^ file)
    {
        return FileIO::WriteTextAsync(file, localCounter.ToString());
    }).then([this](task<void> previousTask)
    {
        try
        {
            previousTask.get();
        }
        catch (Platform::Exception^)
        {
        }
        Read_Local_Counter();
    });
}

void Files::Read_Local_Counter()
{
    create_task(localFolder->GetFileAsync(filename)).then([this](StorageFile^ file)
    {
        return FileIO::ReadTextAsync(file);
    }).then([this](task<String^> previousTask)
    {
        try
        {
            String^ text = previousTask.get();

            LocalOutputTextBlock->Text = "Local Counter: " + text;

            localCounter = _wtoi(text->Data());
        }
        catch (...)
        {
            LocalOutputTextBlock->Text = "Local Counter: <not found>";
        }
    });
}

void Files::Increment_LocalCache_Click(Object^ sender, RoutedEventArgs^ e)
{
    localCacheCounter++;

    create_task(localCacheFolder->CreateFileAsync(filename, CreationCollisionOption::ReplaceExisting)).then(
        [this](StorageFile^ file)
    {
        return FileIO::WriteTextAsync(file, localCacheCounter.ToString());
    }).then([this](task<void> previousTask)
    {
        try
        {
            previousTask.get();
        }
        catch (Platform::Exception^)
        {
        }
        Read_LocalCache_Counter();
    });
}

void Files::Read_LocalCache_Counter()
{
    create_task(localCacheFolder->GetFileAsync(filename)).then([this](StorageFile^ file)
    {
        return FileIO::ReadTextAsync(file);
    }).then([this](task<String^> previousTask)
    {
        try
        {
            String^ text = previousTask.get();

            LocalCacheOutputTextBlock->Text = "LocalCache Counter: " + text;

            localCacheCounter = _wtoi(text->Data());
        }
        catch (...)
        {
            LocalCacheOutputTextBlock->Text = "LocalCache Counter: <not found>";
        }
    });
}

void Files::Increment_Roaming_Click(Object^ sender, RoutedEventArgs^ e)
{
    roamingCounter++;

    create_task(roamingFolder->CreateFileAsync(filename, CreationCollisionOption::ReplaceExisting)).then(
        [this](StorageFile^ file)
    {
        return FileIO::WriteTextAsync(file, roamingCounter.ToString());
    }).then([this](task<void> previousTask)
    {
        try
        {
            previousTask.get();
        }
        catch (Platform::Exception^)
        {
        }
        Read_Roaming_Counter();
    });
}

void Files::Read_Roaming_Counter()
{
    create_task(roamingFolder->GetFileAsync(filename)).then([this](StorageFile^ file)
    {
        return FileIO::ReadTextAsync(file);
    }).then([this](task<String^> previousTask)
    {
        try
        {
            String^ text = previousTask.get();

            RoamingOutputTextBlock->Text = "Roaming Counter: " + text;

            roamingCounter = _wtoi(text->Data());
        }
        catch (...)
        {
            RoamingOutputTextBlock->Text = "Roaming Counter: <not found>";
        }
    });
}

void Files::Increment_Temporary_Click(Object^ sender, RoutedEventArgs^ e)
{
    temporaryCounter++;

    create_task(temporaryFolder->CreateFileAsync(filename, CreationCollisionOption::ReplaceExisting)).then(
        [this](StorageFile^ file)
    {
        return FileIO::WriteTextAsync(file, temporaryCounter.ToString());
    }).then([this](task<void> previousTask)
    {
        try
        {
            previousTask.get();
        }
        catch (Platform::Exception^)
        {
        }
        Read_Temporary_Counter();
    });
}

void Files::Read_Temporary_Counter()
{
    create_task(temporaryFolder->GetFileAsync(filename)).then([this](StorageFile^ file)
    {
        return FileIO::ReadTextAsync(file);
    }).then([this](task<String^> previousTask)
    {
        try
        {
            String^ text = previousTask.get();

            TemporaryOutputTextBlock->Text = "Temporary Counter: " + text;

            temporaryCounter = _wtoi(text->Data());
        }
        catch (...)
        {
            TemporaryOutputTextBlock->Text = "Temporary Counter: <not found>";
        }
    });
}

void Files::DisplayOutput()
{
    Read_Local_Counter();
    Read_LocalCache_Counter();
    Read_Roaming_Counter();
    Read_Temporary_Counter();
}