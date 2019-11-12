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
// Scenario7.xaml.cpp
// Implementation of the Scenario7 class
//

#include "pch.h"
#include "Scenario7.xaml.h"

using namespace SDKSample::FileAccess;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario7::Scenario7()
{
    InitializeComponent();
    rootPage = MainPage::Current;
    CopyFileButton->Click += ref new RoutedEventHandler(this, &Scenario7::CopyFileButton_Click);
}

void Scenario7::CopyFileButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    rootPage->ResetScenarioOutput(OutputTextBlock);
    StorageFile^ file = rootPage->SampleFile;
    if (file != nullptr)
    {
        // Get the returned file and copy it
        StorageFolder^ documentsFolder = KnownFolders::DocumentsLibrary;
        create_task(file->CopyAsync(documentsFolder, "sample - Copy.dat", NameCollisionOption::ReplaceExisting)).then([this, file](task<StorageFile^> task)
        {
            try
            {
                StorageFile^ sampleFileCopy = task.get();
                OutputTextBlock->Text = "The file '" + file->Name + "' was copied and the new file was named '" + sampleFileCopy->Name + "'.";
            }
            catch(COMException^ ex)
            {
                rootPage->HandleFileNotFoundException(ex);
            }
        });
    }
}
