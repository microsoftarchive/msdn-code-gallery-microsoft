// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"
#include "Scenario11_TryToGetAFileWithoutGettingAnError.xaml.h"

using namespace SDKSample::FileAccess;

using namespace concurrency;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;

Scenario11::Scenario11() : rootPage(MainPage::Current)
{
    InitializeComponent();
    rootPage->Initialize();
    GetFileButton->Click += ref new RoutedEventHandler(this, &Scenario11::GetFileButton_Click);
}

void Scenario11::GetFileButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    // Gets a file without throwing an exception
    create_task(KnownFolders::PicturesLibrary->TryGetItemAsync("sample.dat")).then([this](IStorageItem^ item)
    {
        if (item != nullptr)
        {
            rootPage->NotifyUser("Operation result: " + item->Name, NotifyType::StatusMessage);
        }
        else
        {
            rootPage->NotifyUser("Operation result: null", NotifyType::StatusMessage);
        }
    });
}