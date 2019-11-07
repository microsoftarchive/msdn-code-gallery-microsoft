//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "S2_ListFolders.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::LibraryManagement;

using namespace concurrency;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

Scenario2::Scenario2()
{
    InitializeComponent();
}

/// <summary>
/// Obtains the list of folders that make up the Pictures library and binds the FoldersListView
/// to this list.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void LibraryManagement::Scenario2::ListFoldersButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ListFoldersButton->IsEnabled = false;
    create_task(StorageLibrary::GetLibraryAsync(KnownLibraryId::Pictures)).then([this](StorageLibrary^ library)
    {
        picturesLibrary = library;

        // Bind the FoldersListView to the list of folders that make up the library
        FoldersListView->ItemsSource = picturesLibrary->Folders;

        // Register for the DefinitionChanged event to be notified if other apps modify the library
        picturesLibrary->DefinitionChanged += ref new TypedEventHandler<StorageLibrary^, Platform::Object^>(
            [this](StorageLibrary^ /*sender*/, Platform::Object^ /*e*/)
        {
            Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this]()
            {
                UpdateHeaderText();
            }));
        });
        UpdateHeaderText();
    });
}

/// <summary>
/// Updates the FoldersListHeaderTextBlock with the count of folders in the Pictures library.
/// </summary>
void LibraryManagement::Scenario2::UpdateHeaderText()
{
    FoldersListHeaderTextBlock->Text = "Pictures library (" + picturesLibrary->Folders->Size + " folders)";
}


