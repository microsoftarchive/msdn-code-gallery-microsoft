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
// FileOpenPicker_PickURLFile.xaml.cpp
// Implementation of the FileOpenPicker_PickURLFile class
//

#include "pch.h"
#include "FileOpenPickerPage.xaml.h"
#include "FileOpenPicker_PickURLFile.xaml.h"
#include "Constants.h"

using namespace SDKSample::FilePickerContracts;

using namespace concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::Storage::Pickers::Provider;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

FileOpenPicker_PickURLFile::FileOpenPicker_PickURLFile()
{
    InitializeComponent();

    fileID = ref new String(L"MyURLFile");
    fileOpenPickerUI = FileOpenPickerPage::Current->fileOpenPickerUI;
    AddURLFileButton->Click += ref new RoutedEventHandler(this, &FileOpenPicker_PickURLFile::AddURLFileButton_Click);
    RemoveURLFileButton->Click += ref new RoutedEventHandler(this, &FileOpenPicker_PickURLFile::RemoveURLFileButton_Click);
}

void FileOpenPicker_PickURLFile::UpdateButtonState(bool fileInBasket)
{
    AddURLFileButton->IsEnabled = !fileInBasket;
    RemoveURLFileButton->IsEnabled = fileInBasket;
}

void FileOpenPicker_PickURLFile::OnNavigatedTo(NavigationEventArgs^ e)
{
    UpdateButtonState(fileOpenPickerUI->ContainsFile(fileID));
    token = fileOpenPickerUI->FileRemoved += ref new TypedEventHandler<FileOpenPickerUI^, FileRemovedEventArgs^>(this, &FileOpenPicker_PickURLFile::OnFileRemoved, CallbackContext::Same);
}

void FileOpenPicker_PickURLFile::OnNavigatedFrom(NavigationEventArgs^ e)
{
    fileOpenPickerUI->FileRemoved -= token;
}

void FileOpenPicker_PickURLFile::OnFileRemoved(FileOpenPickerUI^ fileOpenPickerUI, FileRemovedEventArgs^ e)
{
    // make sure that the item got removed matches the one we added.
    if (e->Id == fileID)
    {
        OutputTextBlock->Text = Status::FileRemoved;
        UpdateButtonState(false);
    }
}

void FileOpenPicker_PickURLFile::AddURLFileButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    String^ const filename = ref new String(L"URI.png"); // This will be used as the filename of the StorageFile that references the specified URI

    FileOpenPickerPage::Current->NotifyUser("", NotifyType::StatusMessage);

    Uri^ uri = nullptr;
    try
    {
        uri = ref new Uri(URLInput->Text);
    }
    catch (Exception^)
    {
        FileOpenPickerPage::Current->NotifyUser("Please enter a valid URL.", NotifyType::ErrorMessage);
    }

    if (uri != nullptr)
    {
        create_task(StorageFile::CreateStreamedFileFromUriAsync(filename, uri, RandomAccessStreamReference::CreateFromUri(uri))).then([this](StorageFile^ file)
        {
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
    }
}

void FileOpenPicker_PickURLFile::RemoveURLFileButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    if (fileOpenPickerUI->ContainsFile(fileID))
    {
        fileOpenPickerUI->RemoveFile(fileID);
        OutputTextBlock->Text = Status::FileRemoved;
    }
    UpdateButtonState(false);
}
