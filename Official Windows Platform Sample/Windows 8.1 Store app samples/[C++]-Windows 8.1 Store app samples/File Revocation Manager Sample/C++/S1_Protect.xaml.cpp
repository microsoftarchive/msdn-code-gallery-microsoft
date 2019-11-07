//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S1_Protect.xaml.cpp
// Implementation of the S1_Protect class
//

#include "pch.h"
#include "S1_Protect.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::FileRevocation;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::Storage::AccessCache;
using namespace Windows::Storage::Pickers;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Security::EnterpriseData;

S1_Protect::S1_Protect()
{
    InitializeComponent();
    RootPage = MainPage::Current;
    Initialize();
}

void FileRevocation::S1_Protect::Initialize()
{
    try
    {
        if (Windows::Storage::AccessCache::StorageApplicationPermissions::FutureAccessList->ContainsItem(MainPage::PickedFolderToken))
        {
            RootPage->PickedFolder = nullptr;

            create_task(Windows::Storage::AccessCache::StorageApplicationPermissions::FutureAccessList->GetFolderAsync(MainPage::PickedFolderToken)).then([this](StorageFolder^ folder)
            {
                RootPage->PickedFolder = folder;

                RootPage->SampleFile = nullptr;
                RootPage->TargetFile = nullptr;

                RootPage->SampleFolder = nullptr;
                RootPage->TargetFolder = nullptr;

                create_task(RootPage->PickedFolder->CreateFileAsync(RootPage->SampleFilename, CreationCollisionOption::OpenIfExists)).then([this](StorageFile^ file)
                {
                    RootPage->SampleFile = file;
                });

                create_task(RootPage->PickedFolder->CreateFileAsync(RootPage->TargetFilename, CreationCollisionOption::OpenIfExists)).then([this](StorageFile^ file)
                {
                    RootPage->TargetFile = file;
                });

                create_task(RootPage->PickedFolder->CreateFolderAsync(RootPage->SampleFoldername, CreationCollisionOption::OpenIfExists)).then([this](StorageFolder^ folder)
                {
                    RootPage->SampleFolder = folder;
                });

                create_task(RootPage->PickedFolder->CreateFolderAsync(RootPage->TargetFoldername, CreationCollisionOption::OpenIfExists)).then([this](StorageFolder^ folder)
                {
                    RootPage->TargetFolder = folder;
                });
            });
        }
    }
    catch (COMException^ ex)
    {
        RootPage->HandleFileNotFoundException(ex);
    }
}

void FileRevocation::S1_Protect::Setup_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
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
                    RootPage->NotifyUser("You need to delete the items inside the " + RootPage->SampleFolder->Name + " folder in order to regenerate the folder.", NotifyType::ErrorMessage);
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
                    RootPage->NotifyUser("You need to delete the items inside the " + RootPage->TargetFolder->Name + " folder in order to regenerate the folder.", NotifyType::ErrorMessage);
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

        FolderPicker^ folderPicker = ref new FolderPicker();
        folderPicker->SuggestedStartLocation = PickerLocationId::DocumentsLibrary;
        folderPicker->FileTypeFilter->Append(".docx");
        folderPicker->FileTypeFilter->Append(".xlsx");
        folderPicker->FileTypeFilter->Append(".pptx");
        folderPicker->FileTypeFilter->Append(".txt");

        create_task(folderPicker->PickSingleFolderAsync()).then([this](StorageFolder^ folder)
        {
            RootPage->PickedFolder = folder;

            if (nullptr == RootPage->PickedFolder)
            {
                RootPage->NotifyUser("Please choose a base folder in which to create the SDK Sample related files and folders by clicking the Setup button.", NotifyType::ErrorMessage);
                return;
            }
            else
            {
                StorageApplicationPermissions::FutureAccessList->AddOrReplace(MainPage::PickedFolderToken, RootPage->PickedFolder);

                create_task(RootPage->PickedFolder->CreateFolderAsync(RootPage->SampleFoldername, CreationCollisionOption::ReplaceExisting)).then([this](StorageFolder^ folder)
                {
                    RootPage->SampleFolder = folder;

                    create_task(RootPage->PickedFolder->CreateFolderAsync(RootPage->TargetFoldername, CreationCollisionOption::ReplaceExisting)).then([this](StorageFolder^ folder)
                    {
                        RootPage->TargetFolder = folder;

                        create_task(RootPage->PickedFolder->CreateFileAsync(RootPage->SampleFilename, CreationCollisionOption::ReplaceExisting)).then([this](StorageFile^ file)
                        {
                            RootPage->SampleFile = file;

                            create_task(RootPage->PickedFolder->CreateFileAsync(RootPage->TargetFilename, CreationCollisionOption::ReplaceExisting)).then([this](StorageFile^ file)
                            {
                                RootPage->TargetFile = file;

                                RootPage->NotifyUser("The files " + RootPage->SampleFile->Name + " and " + RootPage->TargetFile->Name + " were created.\n" +
                                                    "The folders " + RootPage->SampleFolder->Name + " and " + RootPage->TargetFolder->Name + " were created.",
                                                    NotifyType::StatusMessage);
                            });
                        });
                    });
                });
            }
        });
    }
    catch (COMException^ ex)
    {
        RootPage->HandleFileNotFoundException(ex);
    }
}

void FileRevocation::S1_Protect::ProtectFile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        if (nullptr == RootPage->SampleFile)
        {
            RootPage->NotifyUser("You need to click the Setup button first.", NotifyType::ErrorMessage);
            return;
        }

        if ("" == InputTextBox->Text)
        {
            RootPage->NotifyUser("Please enter an Enterpise ID that you want to use.", NotifyType::ErrorMessage);
            return;
        }

        create_task(FileRevocationManager::ProtectAsync(RootPage->SampleFile, InputTextBox->Text)).then([this](FileProtectionStatus ProtectionStatus)
        {
            RootPage->NotifyUser("The protection status of the file " + RootPage->SampleFile->Name + " is " + ProtectionStatus.ToString(), NotifyType::StatusMessage);
        }).then([this](task<void> t)
        {
            try
            {
                t.get();
            }

            //
            // NOTE: Generally you should not rely on exception handling
            // to validate an Enterprise ID string. In real-world
            // applications, the domain name of the enterprise might be
            // parsed out of an email address or a URL, and may even be
            // entered by a user. Your app-specific code to extract the
            // Enterprise ID should validate the Enterprise ID string is an
            // internationalized domain name before passing it to
            // ProtectAsync.
            //

            catch (Platform::InvalidArgumentException^ e)
            {
                RootPage->NotifyUser("Given Enterprise ID string is invalid.\n" +
                                     "Please try again using a properly formatted Internationalized Domain Name as the Enterprise ID string.",
                                     NotifyType::ErrorMessage);
            }
        });
    }
    catch (COMException^ ex)
    {
        RootPage->HandleFileNotFoundException(ex);
    }
}

void FileRevocation::S1_Protect::ProtectFolder_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        if (nullptr == RootPage->SampleFolder)
        {
            RootPage->NotifyUser("You need to click the Setup button first.", NotifyType::ErrorMessage);
            return;
        }

        if ("" == InputTextBox->Text)
        {
            RootPage->NotifyUser("Please enter an Enterpise ID that you want to use.", NotifyType::ErrorMessage);
            return;
        }

        // Make sure the folder is empty before you protect it
        Windows::Storage::Search::StorageItemQueryResult^ StorageQuery = RootPage->SampleFolder->CreateItemQuery();
        create_task(StorageQuery->GetItemCountAsync()).then([this](unsigned int Items)
        {
            if (Items > 0)
            {
                RootPage->NotifyUser("You need to empty the " + RootPage->SampleFolder->Name + " before you can protect it.", NotifyType::ErrorMessage);
                return;
            }

            create_task(FileRevocationManager::ProtectAsync(RootPage->SampleFolder, InputTextBox->Text)).then([this](FileProtectionStatus ProtectionStatus)
            {
                RootPage->NotifyUser("The protection status of the folder " + RootPage->SampleFolder->Name + " is " + ProtectionStatus.ToString(), NotifyType::StatusMessage);
            }).then([this](task<void> t)
            {
                try
                {
                    t.get();
                }

                //
                // NOTE: Generally you should not rely on exception handling
                // to validate an Enterprise ID string. In real-world
                // applications, the domain name of the enterprise might be
                // parsed out of an email address or a URL, and may even be
                // entered by a user. Your app-specific code to extract the
                // Enterprise ID should validate the Enterprise ID string is an
                // internationalized domain name before passing it to
                // ProtectAsync.
                //

                catch (Platform::InvalidArgumentException^ e)
                {
                    RootPage->NotifyUser("Given Enterprise ID string is invalid.\n" +
                                         "Please try again using a properly formatted Internationalized Domain Name as the Enterprise ID string.",
                                         NotifyType::ErrorMessage);
                }
            });
        });
    }
    catch (COMException^ ex)
    {
        RootPage->HandleFileNotFoundException(ex);
    }
}
