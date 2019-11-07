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
// CachedFileUpdater_Remote.xaml.cpp
// Implementation of the CachedFileUpdater_Remote class
//

#include "pch.h"
#include "CachedFileUpdaterPage.xaml.h"
#include "CachedFileUpdater_Remote.xaml.h"
#include "Constants.h"

using namespace SDKSample::FilePickerContracts;

using namespace concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers::Provider;
using namespace Windows::Storage::Provider;
using namespace Windows::UI::Core;
using namespace Windows::UI::Popups;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

CachedFileUpdater_Remote::CachedFileUpdater_Remote()
{
    InitializeComponent();
    OverwriteButton->Click += ref new RoutedEventHandler(this, &CachedFileUpdater_Remote::OverwriteButton_Click);
    RenameButton->Click += ref new RoutedEventHandler(this, &CachedFileUpdater_Remote::RenameButton_Click);
}

void CachedFileUpdater_Remote::OverwriteButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    FileUpdateRequest^ fileUpdateRequest = CachedFileUpdaterPage::Current->fileUpdateRequest;
    FileUpdateRequestDeferral^ fileUpdateRequestDeferral = CachedFileUpdaterPage::Current->fileUpdateRequestDeferral;

    // update the remote version of file...
    // Printing the file content
    OutputFileAsync(fileUpdateRequest->File);

    fileUpdateRequest->Status = FileUpdateStatus::Complete;
    fileUpdateRequestDeferral->Complete();

    UpdateUI(CachedFileUpdaterPage::Current->cachedFileUpdaterUI->UIStatus);
}

void CachedFileUpdater_Remote::RenameButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    FileUpdateRequest^ fileUpdateRequest = CachedFileUpdaterPage::Current->fileUpdateRequest;
    FileUpdateRequestDeferral^ fileUpdateRequestDeferral = CachedFileUpdaterPage::Current->fileUpdateRequestDeferral;

    create_task(fileUpdateRequest->File->CopyAsync(ApplicationData::Current->LocalFolder, fileUpdateRequest->File->Name, NameCollisionOption::GenerateUniqueName)).then([this, fileUpdateRequest, fileUpdateRequestDeferral](StorageFile^ file)
    {
        CachedFileUpdater::SetUpdateInformation(file, "CachedFile", ReadActivationMode::NotNeeded, WriteActivationMode::AfterWrite, CachedFileOptions::RequireUpdateOnAccess);
        fileUpdateRequest->UpdateLocalFile(file);
        OutputFileAsync(file);
        fileUpdateRequest->Status = FileUpdateStatus::Complete;
        fileUpdateRequestDeferral->Complete();
        UpdateUI(CachedFileUpdaterPage::Current->cachedFileUpdaterUI->UIStatus);
    });

}

void CachedFileUpdater_Remote::OutputFileAsync(StorageFile^ file)
{
    create_task(FileIO::ReadTextAsync(file)).then([this, file](String^ fileContent)
    {
        OutputFileName->Text = L"File Name: " + file->Name;
        OutputFileContent->Text = L"File content:\n" + fileContent;
    });
}

void CachedFileUpdater_Remote::UpdateUI(UIStatus uiStatus)
{
    if (uiStatus == UIStatus::Complete)
    {
        OverwriteButton->IsEnabled = false;
        RenameButton->IsEnabled = false;
    }
}
