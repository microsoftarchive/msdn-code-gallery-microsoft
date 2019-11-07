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
// Scenario8.xaml.cpp
// Implementation of the Scenario8 class
//

#include "pch.h"
#include "Scenario8.xaml.h"

using namespace SDKSample::FileAccess;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario8::Scenario8()
{
    InitializeComponent();
    rootPage = MainPage::Current;
    DeleteFileButton->Click += ref new RoutedEventHandler(this, &Scenario8::DeleteFileButton_Click);
}

void Scenario8::DeleteFileButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    rootPage->ResetScenarioOutput(OutputTextBlock);
    StorageFile^ sampleFile = rootPage->SampleFile;
    if (sampleFile != nullptr)
    {
        String^ filename = sampleFile->Name;
        // Get the returned file and displays basic file properties
        create_task(sampleFile->DeleteAsync()).then([this, filename](task<void> task)
        {
            try
            {
                task.get();
                rootPage->SampleFile = nullptr;
                OutputTextBlock->Text = "The file '" + filename + "' was deleted";
            }
            catch(COMException^ ex)
            {
                rootPage->HandleFileNotFoundException(ex);
            }
        });
    }
}
