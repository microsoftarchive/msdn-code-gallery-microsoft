// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"
#include "Scenario10_DeleteAFile.xaml.h"

using namespace SDKSample::FileAccess;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;

Scenario10::Scenario10() : rootPage(MainPage::Current)
{
    InitializeComponent();
    rootPage->Initialize();
    rootPage->ValidateFile();
    DeleteFileButton->Click += ref new RoutedEventHandler(this, &Scenario10::DeleteFileButton_Click);
}

void Scenario10::DeleteFileButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    StorageFile^ file = rootPage->SampleFile;
    if (file != nullptr)
    {
        String^ fileName = file->Name;
        // Deletes the file
        create_task(file->DeleteAsync()).then([this, fileName](task<void> task)
        {
            try
            {
                task.get();
                rootPage->SampleFile = nullptr;
                rootPage->NotifyUser("The file '" + fileName + "' was deleted", NotifyType::StatusMessage);
            }
            catch (COMException^ ex)
            {
                rootPage->HandleFileNotFoundException(ex);
            }
        });
    }
    else
    {
        rootPage->NotifyUserFileNotExist();
    }
}
