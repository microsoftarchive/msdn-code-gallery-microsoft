//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S5_Cleanup.xaml.cpp
// Implementation of the S5_Cleanup class
//

#include "pch.h"
#include "S5_Cleanup.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::FileRevocation;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Security::EnterpriseData;

S5_Cleanup::S5_Cleanup()
{
    InitializeComponent();
    RootPage = MainPage::Current;
}

void FileRevocation::S5_Cleanup::Cleanup_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        if (nullptr != RootPage->SampleFolder)
        {
            Windows::Storage::Search::StorageItemQueryResult^ FolderStorageQuery = RootPage->SampleFolder->CreateItemQuery();
            create_task(FolderStorageQuery->GetItemCountAsync()).then([this](unsigned int FolderItems)
            {
                if (FolderItems > 0)
                {
                    RootPage->NotifyUser("You need to delete the items inside the " + RootPage->SampleFolder->Name + " folder in order to delete the folder.", NotifyType::ErrorMessage);
                    return;
                }

                create_task(RootPage->SampleFolder->DeleteAsync()).then([this](task<void> task)
                {
                    task.get();
                    RootPage->SampleFolder = nullptr;
                });
            });
        }

        if (nullptr != RootPage->TargetFolder)
        {
            Windows::Storage::Search::StorageItemQueryResult^ FolderStorageQuery = RootPage->TargetFolder->CreateItemQuery();
            create_task(FolderStorageQuery->GetItemCountAsync()).then([this](unsigned int FolderItems)
            {
                if (FolderItems > 0)
                {
                    RootPage->NotifyUser("You need to delete the items inside the " + RootPage->TargetFolder->Name + " folder in order to delete the folder.", NotifyType::ErrorMessage);
                    return;
                }

                create_task(RootPage->TargetFolder->DeleteAsync()).then([this](task<void> task)
                {
                    task.get();
                    RootPage->TargetFolder = nullptr;
                });
            });
        }

        if (nullptr != RootPage->SampleFile)
        {
            create_task(RootPage->SampleFile->DeleteAsync()).then([this](task<void> task)
            {
                task.get();
                RootPage->SampleFile = nullptr;
            });
        }

        if (nullptr != RootPage->TargetFile)
        {
            create_task(RootPage->TargetFile->DeleteAsync()).then([this](task<void> task)
            {
                task.get();
                RootPage->TargetFile = nullptr;
            });
        }

        if (nullptr != RootPage->PickedFolder)
        {
            RootPage->PickedFolder = nullptr;
        }

        Windows::Storage::AccessCache::StorageApplicationPermissions::FutureAccessList->Clear();

        RootPage->NotifyUser("The files " + RootPage->SampleFilename + " and " + RootPage->TargetFilename + " were deleted.\n" +
                            "The folders " + RootPage->SampleFoldername + " and " + RootPage->TargetFoldername + " were deleted.",
                            NotifyType::StatusMessage);
    }
    catch (COMException^ ex)
    {
        RootPage->HandleFileNotFoundException(ex);
    }
}
