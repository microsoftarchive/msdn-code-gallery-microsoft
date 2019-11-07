//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S2_CopyProtection.xaml.cpp
// Implementation of the S2_CopyProtection class
//

#include "pch.h"
#include "S2_CopyProtection.xaml.h"
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

S2_CopyProtection::S2_CopyProtection()
{
    InitializeComponent();
    RootPage = MainPage::Current;
}

void FileRevocation::S2_CopyProtection::CopyProtectionToFile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        if ((nullptr == RootPage->SampleFile) || (nullptr == RootPage->TargetFile))
        {
            RootPage->NotifyUser("You need to click the Setup button in the Protect a file or file with an Enterprise Identity scenario.", NotifyType::ErrorMessage);
            return;
        }

        create_task(FileRevocationManager::CopyProtectionAsync(RootPage->SampleFile, RootPage->TargetFile)).then([this](bool IsProtectionCopied)
        {
            if (!IsProtectionCopied)
            {
                // Make sure the source file is protected
                create_task(FileRevocationManager::GetStatusAsync(RootPage->SampleFile)).then([this](FileProtectionStatus SourceProtectionStatus)
                {
                    if (FileProtectionStatus::Protected != SourceProtectionStatus)
                    {
                        RootPage->NotifyUser("The protection cannot be copied since the status of the source file " + RootPage->SampleFile->Name + " is " + SourceProtectionStatus.ToString() + ".\n" +
                                            "Please try again after clicking the Setup Button followed by the Protect File button in the Protect a file or folder with an Enterprise Identity scenario.",
                                            NotifyType::ErrorMessage);
                        return;
                    }
                });

                // Get the target file protection status
                create_task(FileRevocationManager::GetStatusAsync(RootPage->TargetFile)).then([this](FileProtectionStatus TargetProtectionStatus)
                {
                    // Check the target file protection status
                    if (FileProtectionStatus::Protected == TargetProtectionStatus)
                    {
                        RootPage->NotifyUser("The protection cannot be copied since the target file " + RootPage->TargetFile->Name + " is already protected by another Enterprise Identity.\n" +
                                            "Please try again after clicking the Setup Button followed by the Protect File button in the Protect a file or folder with an Enterprise Identity scenario.",
                                            NotifyType::ErrorMessage);
                        return;
                    }
                    else
                    {
                        RootPage->NotifyUser("The protection cannot be copied since the status of the target file " + RootPage->TargetFile->Name + " is " + TargetProtectionStatus.ToString() +
                                            "Please try again after clicking the Setup Button followed by the Protect File button in the Protect a file or folder with an Enterprise Identity scenario.",
                                            NotifyType::ErrorMessage);
                        return;
                    }
                });
            }

            create_task(FileRevocationManager::GetStatusAsync(RootPage->TargetFile)).then([this](FileProtectionStatus TargetProtectionStatus)
            {
                RootPage->NotifyUser("The protection was copied.\n" +
                                    "The protection status of the target file " + RootPage->TargetFile->Name + " is " + TargetProtectionStatus.ToString() + ".\n",
                                    NotifyType::StatusMessage);
            });
        });
    }
    catch (COMException^ ex)
    {
        RootPage->HandleFileNotFoundException(ex);
    }
}

void FileRevocation::S2_CopyProtection::CopyProtectionToFolder_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        if ((nullptr == RootPage->SampleFolder) || (nullptr == RootPage->TargetFolder))
        {
            RootPage->NotifyUser("You need to click the Setup button in the Protect a file or folder with an Enterprise Identity scenario.", NotifyType::ErrorMessage);
            return;
        }

        // Make sure the folder is empty before you protect it
        Windows::Storage::Search::StorageItemQueryResult^ StorageQuery = RootPage->TargetFolder->CreateItemQuery();
        create_task(StorageQuery->GetItemCountAsync()).then([this](unsigned int Items)
        {
            if (Items > 0)
            {
                RootPage->NotifyUser("You need to empty the " + RootPage->TargetFolder->Name + " before you can protect it.", NotifyType::ErrorMessage);
                return;
            }

            create_task(FileRevocationManager::CopyProtectionAsync(RootPage->SampleFolder, RootPage->TargetFolder)).then([this](bool IsProtectionCopied)
            {
                if(IsProtectionCopied)
                {
                    create_task(FileRevocationManager::GetStatusAsync(RootPage->TargetFolder)).then([this](FileProtectionStatus TargetProtectionStatus)
                    {
                        RootPage->NotifyUser("The protection was copied.\n" +
                                            "The protection status of the target folder " + RootPage->TargetFolder->Name + " is " + TargetProtectionStatus.ToString() + ".\n",
                                            NotifyType::StatusMessage);
                    });

                    return;
                }
                else
                {
                    // Make sure the source folder is protected
                    create_task(FileRevocationManager::GetStatusAsync(RootPage->SampleFolder)).then([this](FileProtectionStatus SourceProtectionStatus)
                    {
                        if (FileProtectionStatus::Protected != SourceProtectionStatus)
                        {
                            RootPage->NotifyUser("The protection cannot be copied since the status of the source folder " + RootPage->SampleFolder->Name + " is " + SourceProtectionStatus.ToString() + ".\n" +
                                                "Please try again after clicking the Setup Button followed by the Protect Folder button in the Protect a file or folder with an Enterprise Identity scenario.",
                                                NotifyType::ErrorMessage);
                            return;
                        }
                    });

                    // Get the target folder protection status
                    create_task(FileRevocationManager::GetStatusAsync(RootPage->TargetFolder)).then([this](FileProtectionStatus TargetProtectionStatus)
                    {
                        // Check the target folder protection status
                        if (FileProtectionStatus::Protected == TargetProtectionStatus)
                        {
                            RootPage->NotifyUser("The protection cannot be copied since the target folder " + RootPage->TargetFolder->Name + " is already protected by another Enterprise Identity.\n" +
                                                "Please try again after clicking the Setup Button followed by the Protect Folder button in the Protect a file or folder with an Enterprise Identity scenario.",
                                                NotifyType::ErrorMessage);
                            return;
                        }
                        else
                        {
                            RootPage->NotifyUser("The protection cannot be copied since the status of the target folder " + RootPage->TargetFolder->Name + " is " + TargetProtectionStatus.ToString() + ".\n" +
                                                "Please try again after clicking the Setup Button followed by the Protect Folder button in the Protect a file or folder with an Enterprise Identity scenario.",
                                                NotifyType::ErrorMessage);
                            return;
                        }
                    });
                }
            });
        });
    }
    catch (COMException^ ex)
    {
        RootPage->HandleFileNotFoundException(ex);
    }
}
