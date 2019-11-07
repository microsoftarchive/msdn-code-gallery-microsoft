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
// Scenario6.xaml.cpp
// Implementation of the Scenario6 class
//

#include "pch.h"
#include "Scenario6.xaml.h"

using namespace SDKSample::FileAccess;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::Storage::AccessCache;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario6::Scenario6()
{
    InitializeComponent();
    rootPage = MainPage::Current;
    AddToListButton->Click += ref new RoutedEventHandler(this, &Scenario6::AddToListButton_Click);
    ShowListButton->Click += ref new RoutedEventHandler(this, &Scenario6::ShowListButton_Click);
    OpenFromListButton->Click += ref new RoutedEventHandler(this, &Scenario6::OpenFromListButton_Click);
}

void Scenario6::AddToListButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    rootPage->ResetScenarioOutput(OutputTextBlock);
    StorageFile^ file = rootPage->SampleFile;
    if (file != nullptr)
    {
        if (MRURadioButton->IsChecked->Value)
        {
            // Add the file to the MRU
            rootPage->MruToken = StorageApplicationPermissions::MostRecentlyUsedList->Add(file, file->Name);
            OutputTextBlock->Text = "The file '" + file->Name + "' was added to the MRU list and a token was stored.";
        }
        else if (FALRadioButton->IsChecked->Value)
        {
            // Add the file to the MRU
            rootPage->FalToken = StorageApplicationPermissions::FutureAccessList->Add(file, file->Name);
            OutputTextBlock->Text = "The file '" + file->Name + "' was added to the FAL list and a token was stored.";
        }
    }
}

void Scenario6::ShowListButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    rootPage->ResetScenarioOutput(OutputTextBlock);
    StorageFile^ file = rootPage->SampleFile;
    if (file != nullptr)
    {
        String^ outputText;
        if (MRURadioButton->IsChecked->Value)
        {
            AccessListEntryView^ entries = StorageApplicationPermissions::MostRecentlyUsedList->Entries;
            if (entries->Size > 0)
            {
                outputText = "The MRU list contains the following item(s):\n\n";
                std::for_each(begin(entries), end(entries), [this, &outputText](const AccessListEntry& entry)
                {
                    outputText += entry.Metadata + "\n"; // Application previously chose to store sampleFile->Name in this field
                });
            }
            else
            {
                outputText = "The MRU list is empty, please select 'Most Recently Used' list and click 'Add to List' to add a file to the MRU list.";
            }
        }
        else if (FALRadioButton->IsChecked->Value)
        {
            AccessListEntryView^ entries = StorageApplicationPermissions::FutureAccessList->Entries;
            if (entries->Size > 0)
            {
                outputText = "The FAL list contains the following item(s):\n\n";
                std::for_each(begin(entries), end(entries), [this, &outputText](const AccessListEntry& entry)
                {
                    outputText += entry.Metadata + "\n"; // Application previously chose to store sampleFile->Name in this field
                });
            }
            else
            {
                outputText = "The FAL list is empty, please select 'Future Access List' list and click 'Add to List' to add a file to the FAL list.";
            }
        }
        OutputTextBlock->Text = outputText;
    }
}

void Scenario6::OpenFromListButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    rootPage->ResetScenarioOutput(OutputTextBlock);
    StorageFile^ file = rootPage->SampleFile;
    if (file != nullptr)
    {
        if (MRURadioButton->IsChecked->Value)
        {
            if (rootPage->MruToken != nullptr)
            {
                // Open the file via the token that was stored when adding this file into the MRU list
                create_task(StorageApplicationPermissions::MostRecentlyUsedList->GetFileAsync(rootPage->MruToken)).then([this](task<StorageFile^> task)
                {
                    try
                    {
                        StorageFile^ file = task.get();
                        // Read the file
                        create_task(FileIO::ReadTextAsync(file)).then([this, file](String^ fileContent)
                        {
                            OutputTextBlock->Text = "The file '" + file->Name + "' was opened by a stored token from the MRU list, it contains the following text:\n\n" + fileContent;
                        });
                    }
                    catch(COMException^ ex)
                    {
                        rootPage->HandleFileNotFoundException(ex);
                    }
                });
            }
            else
            {
                OutputTextBlock->Text = "The MRU list is empty, please select 'Most Recently Used' list and click 'Add to List' to add a file to the MRU list.";
            }
        }
        else if (FALRadioButton->IsChecked->Value)
        {
            if (rootPage->FalToken != nullptr)
            {
                // Open the file via the token that was stored when adding this file into the FAL list
                create_task(StorageApplicationPermissions::FutureAccessList->GetFileAsync(rootPage->FalToken)).then([this](task<StorageFile^> task)
                {
                    try
                    {
                        StorageFile^ file = task.get();
                        // Read the file
                        create_task(FileIO::ReadTextAsync(file)).then([this, file](String^ fileContent)
                        {
                            OutputTextBlock->Text = "The file '" + file->Name + "' was opened by a stored token from the FAL list, it contains the following text:\n\n" + fileContent;
                        });
                    }
                    catch(COMException^ ex)
                    {
                        rootPage->HandleFileNotFoundException(ex);
                    }
                });
            }
            else
            {
                OutputTextBlock->Text = "The FAL list is empty, please select 'Future Access List' list and click 'Add to List' to add a file to the FAL list.";
            }
        }
    }
}
