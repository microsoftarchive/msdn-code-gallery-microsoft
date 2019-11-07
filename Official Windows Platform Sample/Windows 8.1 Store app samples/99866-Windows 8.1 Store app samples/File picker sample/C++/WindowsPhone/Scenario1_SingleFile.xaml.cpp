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
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1_SingleFile.xaml.h"

using namespace SDKSample::FilePicker;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario1::Scenario1()
{
    InitializeComponent();
    rootPage = MainPage::Current;
    PickAFileButton->Click += ref new RoutedEventHandler(this, &Scenario1::PickAFileButton_Click);
}

void Scenario1::PickAFileButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    // Clear previous returned file name, if it exists, between iterations of this scenario
    OutputTextBlock->Text = "";

    FileOpenPicker^ openPicker = ref new FileOpenPicker();
    openPicker->ViewMode = PickerViewMode::Thumbnail;
    openPicker->SuggestedStartLocation = PickerLocationId::PicturesLibrary;
    openPicker->FileTypeFilter->Append(".jpg");
    openPicker->FileTypeFilter->Append(".jpeg");
    openPicker->FileTypeFilter->Append(".png");

    openPicker->PickSingleFileAndContinue();
}

/// <summary>
/// Handle the returned files from file picker
/// This method is triggered by ContinuationManager based on ActivationKind
/// </summary>
/// <param name="args">File open picker continuation activation argment. It cantains the list of files user selected with file open picker </param>
void Scenario1::ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs^ args)
{
    if (args->Files->Size > 0)
    {
        OutputTextBlock->Text = "Picked photo: " + args->Files->GetAt(0)->Name;
    }
    else
    {
        OutputTextBlock->Text = "Operation cancelled.";
    }
}