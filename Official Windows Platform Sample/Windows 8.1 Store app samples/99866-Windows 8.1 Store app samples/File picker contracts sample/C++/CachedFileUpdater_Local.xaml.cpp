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
// CachedFileUpdater_Local.xaml.cpp
// Implementation of the CachedFileUpdater_Local class
//

#include "pch.h"
#include "CachedFileUpdaterPage.xaml.h"
#include "CachedFileUpdater_Local.xaml.h"
#include "Constants.h"

using namespace SDKSample::FilePickerContracts;

using namespace concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::Foundation;
using namespace Windows::Globalization::DateTimeFormatting;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers::Provider;
using namespace Windows::Storage::Provider;
using namespace Windows::UI::Core;
using namespace Windows::UI::Popups;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

CachedFileUpdater_Local::CachedFileUpdater_Local()
{
    InitializeComponent();
    FileIsCurrentButton->Click += ref new RoutedEventHandler(this, &CachedFileUpdater_Local::FileIsCurrentButton_Click);
    ProvideUpdatedVersionButton->Click += ref new RoutedEventHandler(this, &CachedFileUpdater_Local::ProvideUpdatedVersionButton_Click);
}

void CachedFileUpdater_Local::FileIsCurrentButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    FileUpdateRequest^ fileUpdateRequest = CachedFileUpdaterPage::Current->fileUpdateRequest;
    FileUpdateRequestDeferral^ fileUpdateRequestDeferral = CachedFileUpdaterPage::Current->fileUpdateRequestDeferral;

    OutputFileAsync(fileUpdateRequest->File);

    fileUpdateRequest->Status = FileUpdateStatus::Complete;
    fileUpdateRequestDeferral->Complete();

    UpdateUI(CachedFileUpdaterPage::Current->cachedFileUpdaterUI->UIStatus);
}

void CachedFileUpdater_Local::ProvideUpdatedVersionButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    FileUpdateRequest^ fileUpdateRequest = CachedFileUpdaterPage::Current->fileUpdateRequest;
    FileUpdateRequestDeferral^ fileUpdateRequestDeferral = CachedFileUpdaterPage::Current->fileUpdateRequestDeferral;

    Windows::Globalization::Calendar^ Calendar = ref new Windows::Globalization::Calendar;
    create_task(FileIO::AppendTextAsync(fileUpdateRequest->File, L"\nNew content added @ " + DateTimeFormatter::LongTime->Format(Calendar->GetDateTime()))).then([=]()
    {
        OutputFileAsync(fileUpdateRequest->File);

        fileUpdateRequest->Status = FileUpdateStatus::Complete;
        fileUpdateRequestDeferral->Complete();

        UpdateUI(CachedFileUpdaterPage::Current->cachedFileUpdaterUI->UIStatus);
    });
}

void CachedFileUpdater_Local::OutputFileAsync(StorageFile^ file)
{
    create_task(FileIO::ReadTextAsync(file)).then([this, file](String^ fileContent)
    {
        OutputFileName->Text = L"Received file: " + file->Name;
        OutputFileContent->Text = L"File content:\n" + fileContent;
    });
}

void CachedFileUpdater_Local::UpdateUI(UIStatus uiStatus)
{
    if (uiStatus == UIStatus::Complete)
    {
        FileIsCurrentButton->IsEnabled = false;
        ProvideUpdatedVersionButton->IsEnabled = false;
    }
}
