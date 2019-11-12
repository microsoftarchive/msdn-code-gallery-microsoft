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
#include "Files.xaml.h"

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

    roamingFolder = ApplicationData::Current->RoamingFolder;
    counter = 0;

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

// Guidance for Local, Roaming, and Temporary files.
//
// Files are ideal for storing large data-sets, databases, or data that is
// in a common file-format.
//
// Files can exist in either the Local, Roaming, or Temporary folders.
//
// Roaming files will be synchronized across machines on which the user has
// singed in with a connected account.  Roaming of files is not instant; the
// system weighs several factors when determining when to send the data.  Usage
// of roaming data should be kept below the quota (available via the 
// RoamingStorageQuota property), or else roaming of data will be suspended.
// Files cannot be roamed while an application is writing to them, so be sure
// to close your application's file objects when they are no longer needed.
//
// Local files are not synchronized and remain on the machine on which they
// were originally written.
//
// Temporary files are subject to deletion when not in use.  The system 
// considers factors such as available disk capacity and the age of a file when
// determining when or whether to delete a temporary file.

// This sample illustrates reading and writing a file in the Roaming folder, though a
// Local or Temporary file could be used just as easily.

void Files::Increment_Click(Object^ sender, RoutedEventArgs^ e)
{
    counter++;

    create_task(roamingFolder->CreateFileAsync(filename, CreationCollisionOption::ReplaceExisting)).then(
        [this](StorageFile^ file) 
    {
        return FileIO::WriteTextAsync(file, counter.ToString());
    }).then([this](task<void> previousTask) 
    {
        try
        {
            previousTask.get();
        }
        catch (Platform::Exception^)
        {
        }
        DisplayOutput();
    });
}

void Files::ReadCounter()
{
    create_task(roamingFolder->GetFileAsync(filename)).then([this](StorageFile^ file) 
    {
        return FileIO::ReadTextAsync(file);
    }).then([this](task<String^> previousTask) 
    {
        try
        {
            String^ text = previousTask.get();

            OutputTextBlock->Text = "Counter: " + text;

            counter = _wtoi(text->Data());
        }
        catch (...)
        {
            OutputTextBlock->Text = "Counter: <not found>";
        }
    });
}

void Files::DisplayOutput()
{
    ReadCounter();
}
