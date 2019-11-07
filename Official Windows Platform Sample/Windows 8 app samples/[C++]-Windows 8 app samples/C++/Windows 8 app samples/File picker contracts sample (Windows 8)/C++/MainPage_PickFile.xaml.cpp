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
// MainPage_PickFile.xaml.cpp
// Implementation of the MainPage_PickFile class
//

#include "pch.h"
#include "MainPage.xaml.h"
#include "MainPage_PickFile.xaml.h"

using namespace SDKSample::FilePickerContracts;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

MainPage_PickFile::MainPage_PickFile()
{
    InitializeComponent();
    PickFileButton->Click += ref new RoutedEventHandler(this, &MainPage_PickFile::PickFileButton_Click);
}

void MainPage_PickFile::PickFileButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    MainPage^ page = MainPage::Current;

    page->ResetScenarioOutput(OutputTextBlock);
    if (page->EnsureUnsnapped())
    {
        // Set up and launch the Open Picker
        auto fileOpenPicker = ref new FileOpenPicker();
        fileOpenPicker->ViewMode = PickerViewMode::Thumbnail;
        fileOpenPicker->FileTypeFilter->Append(".png");

        create_task(fileOpenPicker->PickMultipleFilesAsync()).then([this](IVectorView<StorageFile^>^ files)
        {
            String^ fileNames = "";
            std::for_each(begin(files), end(files), [this, &fileNames](StorageFile^ file)
            {
                // At this point, the app can begin reading from the provided file
                fileNames += file->Name + "\n";
            });

            OutputTextBlock->Text = fileNames->ToString();
        });
    }
}
