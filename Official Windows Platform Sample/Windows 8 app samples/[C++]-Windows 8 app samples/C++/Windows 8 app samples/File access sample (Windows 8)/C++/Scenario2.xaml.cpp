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
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"

using namespace SDKSample::FileAccess;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario2::Scenario2()
{
    InitializeComponent();
    rootPage = MainPage::Current;
    WriteTextButton->Click += ref new RoutedEventHandler(this, &Scenario2::WriteTextButton_Click);
    ReadTextButton->Click  += ref new RoutedEventHandler(this, &Scenario2::ReadTextButton_Click);
}

void Scenario2::WriteTextButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    rootPage->ResetScenarioOutput(OutputTextBlock);
    StorageFile^ file = rootPage->SampleFile;
    if (file != nullptr)
    {
        String^ userContent = InputTextBox->Text;
        if (userContent != nullptr && !userContent->IsEmpty())
        {
            create_task(FileIO::WriteTextAsync(file, userContent)).then([this, file, userContent](task<void> task)
            {
                try
                {
                    task.get();
                    OutputTextBlock->Text = "The following text was written to '" + file->Name + "':\n\n" + userContent;
                }
                catch(COMException^ ex)
                {
                    rootPage->HandleFileNotFoundException(ex);
                }
            });
        }
        else
        {
            OutputTextBlock->Text = "The text box is empty, please write something and then click 'Write' again.";
        }
    }
}

void Scenario2::ReadTextButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    rootPage->ResetScenarioOutput(OutputTextBlock);
    StorageFile^ file = rootPage->SampleFile;
    if (file != nullptr)
    {
        create_task(FileIO::ReadTextAsync(file)).then([this, file](task<String^> task)
        {
            try
            {
                String^ fileContent = task.get();
                OutputTextBlock->Text = "The following text was read from '" + file->Name + "':\n\n" + fileContent;
            }
            catch(COMException^ ex)
            {
                rootPage->HandleFileNotFoundException(ex);
            }
        });
    }
}
