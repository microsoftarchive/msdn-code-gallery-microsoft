//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S3_GetStatus.xaml.cpp
// Implementation of the S3_GetStatus class
//

#include "pch.h"
#include "S3_GetStatus.xaml.h"
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

S3_GetStatus::S3_GetStatus()
{
    InitializeComponent();
    RootPage = MainPage::Current;
}

void FileRevocation::S3_GetStatus::GetStatus_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        if ((nullptr == RootPage->SampleFile)
            || (nullptr == RootPage->TargetFile)
            || (nullptr == RootPage->SampleFolder)
            || (nullptr == RootPage->TargetFolder))
        {
            RootPage->NotifyUser("You need to click the Setup button in the Protect a file or folder with an Enterprise Identity scenario.", NotifyType::ErrorMessage);
            return;
        }

        create_task(FileRevocationManager::GetStatusAsync(RootPage->SampleFile)).then([this](FileProtectionStatus ProtectionStatus)
        {
            OutputTextBlock->Foreground = ref new Windows::UI::Xaml::Media::SolidColorBrush(Windows::UI::Colors::Green);
            OutputTextBlock->Text = "The protection status of the file " + RootPage->SampleFile->Name + " is " + ProtectionStatus.ToString() + ".\n";

            create_task(FileRevocationManager::GetStatusAsync(RootPage->TargetFile)).then([this](FileProtectionStatus ProtectionStatus)
            {
                OutputTextBlock->Foreground = ref new Windows::UI::Xaml::Media::SolidColorBrush(Windows::UI::Colors::Green);
                OutputTextBlock->Text += "The protection status of the file " + RootPage->TargetFile->Name + " is " + ProtectionStatus.ToString() + ".\n";

                create_task(FileRevocationManager::GetStatusAsync(RootPage->SampleFolder)).then([this](FileProtectionStatus ProtectionStatus)
                {
                    OutputTextBlock->Foreground = ref new Windows::UI::Xaml::Media::SolidColorBrush(Windows::UI::Colors::Green);
                    OutputTextBlock->Text += "The protection status of the folder " + RootPage->SampleFolder->Name + " is " + ProtectionStatus.ToString() + ".\n";

                    create_task(FileRevocationManager::GetStatusAsync(RootPage->TargetFolder)).then([this](FileProtectionStatus ProtectionStatus)
                    {
                        OutputTextBlock->Foreground = ref new Windows::UI::Xaml::Media::SolidColorBrush(Windows::UI::Colors::Green);
                        OutputTextBlock->Text += "The protection status of the folder " + RootPage->TargetFolder->Name + " is " + ProtectionStatus.ToString() + ".\n";
                    });
                });
            });
        });
    }
    catch (COMException^ ex)
    {
        RootPage->HandleFileNotFoundException(ex);
    }
}
