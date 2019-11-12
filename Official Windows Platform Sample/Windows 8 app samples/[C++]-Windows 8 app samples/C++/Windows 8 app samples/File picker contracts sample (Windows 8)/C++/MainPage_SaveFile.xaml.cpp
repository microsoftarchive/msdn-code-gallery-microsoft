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
// MainPage_SaveFile.xaml.cpp
// Implementation of the MainPage_SaveFile class
//

#include "pch.h"
#include "MainPage.xaml.h"
#include "MainPage_SaveFile.xaml.h"

using namespace SDKSample::FilePickerContracts;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

MainPage_SaveFile::MainPage_SaveFile()
{
    InitializeComponent();
    SaveFileButton->Click += ref new RoutedEventHandler(this, &MainPage_SaveFile::SaveFileButton_Click);
}

void MainPage_SaveFile::SaveFileButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    MainPage^ page = MainPage::Current;

    page->ResetScenarioOutput(OutputTextBlock);
    if (page->EnsureUnsnapped())
    {
        // Set up and launch the FileSavePicker
        auto fileSavePicker = ref new FileSavePicker();
        auto fileExtensions = ref new Platform::Collections::Vector<String^>();
        fileExtensions->Append(".png");
        fileSavePicker->FileTypeChoices->Insert("PNG", fileExtensions);

        create_task(fileSavePicker->PickSaveFileAsync()).then([this](StorageFile^ file)
        {
            if (file != nullptr)
            {
                // At this point, the app can begin writing to the provided save file
                OutputTextBlock->Text = file->Name;
            }
        });
    }
}
