// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"
#include "Scenario2_GetTheParentFolderOfAFile.xaml.h"

using namespace SDKSample::FileAccess;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;

Scenario2::Scenario2() : rootPage(MainPage::Current)
{
    InitializeComponent();
    rootPage->Initialize();
    rootPage->ValidateFile();
    GetParentButton->Click += ref new RoutedEventHandler(this, &Scenario2::GetParentButton_Click);
}

void Scenario2::GetParentButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    StorageFile^ file = rootPage->SampleFile;
    if (file != nullptr)
    {
        create_task(file->GetParentAsync()).then([this, file](StorageFolder^ parentFolder)
        {
            if (parentFolder != nullptr)
            {
                String^ outputText = "Item: " + file->Name + " (" + file->Path + ")";
                outputText += "\nParent: " + parentFolder->Name + " (" + parentFolder->Path + ")";
                rootPage->NotifyUser(outputText, NotifyType::StatusMessage);
            }
        });
    }
    else
    {
        rootPage->NotifyUserFileNotExist();
    }
}
