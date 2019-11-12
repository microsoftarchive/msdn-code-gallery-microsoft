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
// FileSavePicker_SaveToAppStorage.xaml.cpp
// Implementation of the FileSavePicker_SaveToAppStorage class
//

#include "pch.h"
#include "FileSavePickerPage.xaml.h"
#include "FileSavePicker_SaveToAppStorage.xaml.h"
#include "Constants.h"

using namespace SDKSample::FilePickerContracts;

using namespace concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers::Provider;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

FileSavePicker_SaveToAppStorage::FileSavePicker_SaveToAppStorage()
{
    InitializeComponent();
    fileSavePickerUI = FileSavePickerPage::Current->fileSavePickerUI;
}

void FileSavePicker_SaveToAppStorage::OnNavigatedTo(NavigationEventArgs^ e)
{
    token = fileSavePickerUI->TargetFileRequested += ref new TypedEventHandler<FileSavePickerUI^, TargetFileRequestedEventArgs^>(this, &FileSavePicker_SaveToAppStorage::OnTargetFileRequested, CallbackContext::Same);
}

void FileSavePicker_SaveToAppStorage::OnNavigatedFrom(NavigationEventArgs^ e)
{
    fileSavePickerUI->TargetFileRequested -= token;
}

void FileSavePicker_SaveToAppStorage::OnTargetFileRequested(FileSavePickerUI^ sender, TargetFileRequestedEventArgs^ e)
{
    // Requesting a deferral allows the app to call another asynchronous method and complete the request at a later time
    auto request = e->Request;
    auto deferral = request->GetDeferral();

    // Create a file to provide back to the Picker
    create_task(ApplicationData::Current->LocalFolder->CreateFileAsync(sender->FileName, CreationCollisionOption::GenerateUniqueName)).then([request, deferral](StorageFile^ file)
    {
        // Assign the resulting file to the targetFile property indicates success
        request->TargetFile = file;

        // Complete the deferral to let the Picker know the request is finished.
        deferral->Complete();
    });
}
