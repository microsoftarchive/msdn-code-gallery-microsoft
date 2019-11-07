// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"
#include "Scenario1_CreateAFileInThePicturesLibrary.xaml.h"

using namespace SDKSample::FileAccess;

using namespace concurrency;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;

Scenario1::Scenario1() : rootPage(MainPage::Current)
{
    InitializeComponent();
    rootPage->Initialize();
    CreateFileButton->Click += ref new RoutedEventHandler(this, &Scenario1::CreateFileButton_Click);
}

void Scenario1::CreateFileButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    create_task(KnownFolders::PicturesLibrary->CreateFileAsync(rootPage->Filename, CreationCollisionOption::ReplaceExisting)).then([this](StorageFile^ file)
    {
        rootPage->SampleFile = file;
        rootPage->NotifyUser("The file '" + file->Name + "' was created.", NotifyType::StatusMessage);
    });
}
