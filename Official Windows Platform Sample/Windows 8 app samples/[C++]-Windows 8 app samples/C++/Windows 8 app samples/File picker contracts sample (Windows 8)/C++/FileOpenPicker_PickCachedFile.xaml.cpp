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
// FileOpenPicker_PickCachedFile.xaml.cpp
// Implementation of the FileOpenPicker_PickCachedFile class
//

#include "pch.h"
#include "FileOpenPickerPage.xaml.h"
#include "FileOpenPicker_PickCachedFile.xaml.h"
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
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

FileOpenPicker_PickCachedFile::FileOpenPicker_PickCachedFile()
{
    InitializeComponent();

    fileID = ref new String(L"MyCachedFile");
    fileOpenPickerUI = FileOpenPickerPage::Current->fileOpenPickerUI;

    AddFileButton->Click += ref new RoutedEventHandler(this, &FileOpenPicker_PickCachedFile::AddFileButton_Click);
    RemoveFileButton->Click += ref new RoutedEventHandler(this, &FileOpenPicker_PickCachedFile::RemoveFileButton_Click);
}

void FileOpenPicker_PickCachedFile::UpdateButtonState(bool fileInBasket)
{
    AddFileButton->IsEnabled = !fileInBasket;
    RemoveFileButton->IsEnabled = fileInBasket;
}

void FileOpenPicker_PickCachedFile::OnNavigatedTo(NavigationEventArgs^ e)
{
    UpdateButtonState(fileOpenPickerUI->ContainsFile(fileID));
    token = fileOpenPickerUI->FileRemoved += ref new TypedEventHandler<FileOpenPickerUI^, FileRemovedEventArgs^>(this, &FileOpenPicker_PickCachedFile::OnFileRemoved, CallbackContext::Same);
}

void FileOpenPicker_PickCachedFile::OnNavigatedFrom(NavigationEventArgs^ e)
{
    fileOpenPickerUI->FileRemoved -= token;
}

void FileOpenPicker_PickCachedFile::OnFileRemoved(FileOpenPickerUI^ fileOPenPickerUI, FileRemovedEventArgs^ e)
{
    // make sure that the item got removed matches the one we added.
    if (e->Id == fileID)
    {
        OutputTextBlock->Text = Status::FileRemoved;
        UpdateButtonState(false);
    }
}

void FileOpenPicker_PickCachedFile::AddFileButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    create_task(ApplicationData::Current->LocalFolder->CreateFileAsync(L"CachedFile.txt", CreationCollisionOption::ReplaceExisting)).then([this](StorageFile^ file)
    {
        create_task(FileIO::WriteTextAsync(file, L"Cached file created...")).then([this, file]()
        {
            CachedFileUpdater::SetUpdateInformation(file, "CachedFile", ReadActivationMode::BeforeAccess, WriteActivationMode::NotNeeded, CachedFileOptions::RequireUpdateOnAccess);

            bool inBasket;
            switch (fileOpenPickerUI->AddFile(fileID, file))
            {
            case AddFileResult::Added:
            case AddFileResult::AlreadyAdded:
                inBasket = true;
                OutputTextBlock->Text = Status::FileAdded;
                break;

            default:
                inBasket = false;
                OutputTextBlock->Text = Status::FileAddFailed;
                break;
            }
            UpdateButtonState(inBasket);
        });
    });
}

void FileOpenPicker_PickCachedFile::RemoveFileButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    if (fileOpenPickerUI->ContainsFile(fileID))
    {
        fileOpenPickerUI->RemoveFile(fileID);
        OutputTextBlock->Text = Status::FileRemoved;
    }
    UpdateButtonState(false);
}
