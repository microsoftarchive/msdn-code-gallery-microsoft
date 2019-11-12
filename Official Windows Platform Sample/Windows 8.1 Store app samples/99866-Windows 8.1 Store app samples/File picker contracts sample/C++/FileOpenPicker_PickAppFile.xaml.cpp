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
// FileOpenPicker_PickAppFile.xaml.cpp
// Implementation of the FileOpenPicker_PickAppFile class
//

#include "pch.h"
#include "FileOpenPickerPage.xaml.h"
#include "FileOpenPicker_PickAppFile.xaml.h"
#include "Constants.h"

using namespace SDKSample::FilePickerContracts;

using namespace concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers::Provider;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

FileOpenPicker_PickAppFile::FileOpenPicker_PickAppFile()
{
    InitializeComponent();

    fileID = ref new String(L"MyLocalFile");
    fileOpenPickerUI = FileOpenPickerPage::Current->fileOpenPickerUI;

    AddLocalFileButton->Click += ref new RoutedEventHandler(this, &FileOpenPicker_PickAppFile::AddLocalFileButton_Click);
    RemoveLocalFileButton->Click += ref new RoutedEventHandler(this, &FileOpenPicker_PickAppFile::RemoveLocalFileButton_Click);
}

void FileOpenPicker_PickAppFile::UpdateButtonState(bool fileInBasket)
{
    AddLocalFileButton->IsEnabled = !fileInBasket;
    RemoveLocalFileButton->IsEnabled = fileInBasket;
}

void FileOpenPicker_PickAppFile::OnNavigatedTo(NavigationEventArgs^ e)
{
    UpdateButtonState(fileOpenPickerUI->ContainsFile(fileID));
    token = fileOpenPickerUI->FileRemoved += ref new TypedEventHandler<FileOpenPickerUI^, FileRemovedEventArgs^>(this, &FileOpenPicker_PickAppFile::OnFileRemoved, CallbackContext::Same);
}

void FileOpenPicker_PickAppFile::OnNavigatedFrom(NavigationEventArgs^ e)
{
    fileOpenPickerUI->FileRemoved -= token;
}

void FileOpenPicker_PickAppFile::OnFileRemoved(FileOpenPickerUI^ fileOPenPickerUI, FileRemovedEventArgs^ e)
{
    // make sure that the item got removed matches the one we added.
    if (e->Id == fileID)
    {
        OutputTextBlock->Text = Status::FileRemoved;
        UpdateButtonState(false);
    }
}

void FileOpenPicker_PickAppFile::AddLocalFileButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    create_task(Package::Current->InstalledLocation->GetFileAsync(L"Assets\\squareTile-sdk.png")).then([this](StorageFile^ file)
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

void FileOpenPicker_PickAppFile::RemoveLocalFileButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    if (fileOpenPickerUI->ContainsFile(fileID))
    {
        fileOpenPickerUI->RemoveFile(fileID);
        OutputTextBlock->Text = Status::FileRemoved;
    }
    UpdateButtonState(false);
}
