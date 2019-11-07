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
// FileSavePicker_FailToSave.xaml.cpp
// Implementation of the FileSavePicker_FailToSave class
//

#include "pch.h"
#include "FileSavePickerPage.xaml.h"
#include "FileSavePicker_FailToSave.xaml.h"
#include "Constants.h"

using namespace SDKSample::FilePickerContracts;

using namespace concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers::Provider;
using namespace Windows::UI::Core;
using namespace Windows::UI::Popups;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

FileSavePicker_FailToSave::FileSavePicker_FailToSave()
{
    InitializeComponent();
    fileSavePickerUI = FileSavePickerPage::Current->fileSavePickerUI;
}

void FileSavePicker_FailToSave::OnNavigatedTo(NavigationEventArgs^ e)
{
    token = fileSavePickerUI->TargetFileRequested += ref new TypedEventHandler<FileSavePickerUI^, TargetFileRequestedEventArgs^>(this, &FileSavePicker_FailToSave::OnTargetFileRequested, CallbackContext::Same);
}

void FileSavePicker_FailToSave::OnNavigatedFrom(NavigationEventArgs^ e)
{
    fileSavePickerUI->TargetFileRequested -= token;
}

void FileSavePicker_FailToSave::OnTargetFileRequested(FileSavePickerUI^ fileSavePickerUI, TargetFileRequestedEventArgs^ e)
{
    // Requesting a deferral allows the app to call another asynchronous method and complete the request at a later time
    auto request = e->Request;
    auto deferral = request->GetDeferral();

    // Display a dialog indicating to the user that a corrective action needs to occur
    auto errorDialog = ref new MessageDialog("If the app needs the user to correct a problem before the app can save the file, the app can use a message like this to tell the user about the problem and how to correct it.");
    create_task(errorDialog->ShowAsync()).then([request, deferral](IUICommand^ command)
    {
        // Set the targetFile property to null and complete the deferral to indicate failure once the user has closed the
        // dialog.  This will allow the user to take any neccessary corrective action and click the Save button once again.
        request->TargetFile = nullptr;
        deferral->Complete();
    });
}
