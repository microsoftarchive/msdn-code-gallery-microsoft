//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "S1_AddFolder.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::LibraryManagement;

using namespace concurrency;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

Scenario1::Scenario1()
{
    InitializeComponent();
}

/// <summary>
/// Displays the folder Picker to request that the user select a folder that will be added to the
/// Pictures library.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void LibraryManagement::Scenario1::AddFolderButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    create_task(StorageLibrary::GetLibraryAsync(KnownLibraryId::Pictures)).then([](StorageLibrary^ picturesLibrary)
    {
        return create_task(picturesLibrary->RequestAddFolderAsync());
    }).then([this](StorageFolder^ folderAdded)
    {
        if (folderAdded != nullptr)
        {
            OutputTextBlock->Text = folderAdded->DisplayName + " was added to the Pictures library.";
        }
        else
        {
            OutputTextBlock->Text = "Operation canceled.";
        }
    });
}
