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
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"

using namespace SDKSample::FileAccess;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario1::Scenario1()
{
    InitializeComponent();
    rootPage = MainPage::Current;
    CreateFileButton->Click += ref new RoutedEventHandler(this, &Scenario1::CreateFileButton_Click);
}

void Scenario1::CreateFileButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    create_task(KnownFolders::DocumentsLibrary->CreateFileAsync(rootPage->Filename, CreationCollisionOption::ReplaceExisting)).then([this](StorageFile^ file)
    {
        rootPage->SampleFile = file;
        OutputTextBlock->Text = "The file '" + file->Name + "' was created.";
    });
}
