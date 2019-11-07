// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"
#include "Scenario9_CompareTwoFilesToSeeIfTheyAreTheSame.xaml.h"

using namespace SDKSample::FileAccess;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::UI::Xaml;

Scenario9::Scenario9() : rootPage(MainPage::Current)
{
    InitializeComponent();
    rootPage->Initialize();
    rootPage->ValidateFile();
    CompareFilesButton->Click += ref new RoutedEventHandler(this, &Scenario9::CompareFilesButton_Click);
}

void Scenario9::CompareFilesButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    StorageFile^ file = rootPage->SampleFile;
    if (file != nullptr)
    {
        // Compares a picked file with sample.dat
        FileOpenPicker^ picker = ref new FileOpenPicker();
        picker->SuggestedStartLocation = PickerLocationId::PicturesLibrary;
        picker->FileTypeFilter->Append("*");
        create_task(picker->PickSingleFileAsync()).then([this, file](StorageFile^ comparand)
        {
            if (comparand != nullptr)
            {
                try
                {
                    if (file->IsEqual(comparand))
                    {
                        rootPage->NotifyUser("Files are equal", NotifyType::StatusMessage);
                    }
                    else
                    {
                        rootPage->NotifyUser("Files are not equal", NotifyType::StatusMessage);
                    }
                }
                catch (COMException^ ex)
                {
                    rootPage->HandleFileNotFoundException(ex);
                }
            }
            else
            {
                rootPage->NotifyUser("Operation cancelled", NotifyType::StatusMessage);
            }
        });
    }
    else
    {
        rootPage->NotifyUserFileNotExist();
    }
}
