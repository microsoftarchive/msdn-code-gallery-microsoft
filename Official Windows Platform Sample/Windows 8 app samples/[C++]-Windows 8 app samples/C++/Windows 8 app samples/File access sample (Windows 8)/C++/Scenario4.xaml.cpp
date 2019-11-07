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
// Scenario4.xaml.cpp
// Implementation of the Scenario4 class
//

#include "pch.h"
#include "Scenario4.xaml.h"

using namespace SDKSample::FileAccess;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario4::Scenario4()
{
    InitializeComponent();
    rootPage = MainPage::Current;
    WriteToStreamButton->Click  += ref new RoutedEventHandler(this, &Scenario4::WriteToStreamButton_Click);
    ReadFromStreamButton->Click += ref new RoutedEventHandler(this, &Scenario4::ReadFromStreamButton_Click);
}

void Scenario4::WriteToStreamButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    rootPage->ResetScenarioOutput(OutputTextBlock);
    StorageFile^ file = rootPage->SampleFile;
    if (file != nullptr)
    {
        String^ userContent = InputTextBox->Text;
        if (userContent != nullptr && !userContent->IsEmpty())
        {
            create_task(file->OpenTransactedWriteAsync()).then([this, file, userContent](task<StorageStreamTransaction^> task)
            {
                try
                {
                    StorageStreamTransaction^ transaction = task.get();
                    DataWriter^ dataWriter = ref new DataWriter(transaction->Stream);
                    dataWriter->WriteString(userContent);
                    create_task(dataWriter->StoreAsync()).then([this, file, dataWriter, transaction, userContent](unsigned int bytesWritten)
                    {
                        transaction->Stream->Size = bytesWritten; // reset stream size to override the file
                        create_task(transaction->CommitAsync()).then([this, file, userContent]()
                        {
                            OutputTextBlock->Text = "The following text was written to '" + file->Name + "' using a stream:\n\n" + userContent;
                        });
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
            OutputTextBlock->Text = "The text box is empty, please write something and then click 'Write' again.";
        }
    }
}

void Scenario4::ReadFromStreamButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    rootPage->ResetScenarioOutput(OutputTextBlock);
    StorageFile^ file = rootPage->SampleFile;
    if (file != nullptr)
    {
        create_task(file->OpenAsync(FileAccessMode::Read)).then([this, file](task<IRandomAccessStream^> task)
        {
            try
            {
                IRandomAccessStream^ readStream = task.get();
                UINT64 const size = readStream->Size;
                if (size <= MAXUINT32)
                {
                    DataReader^ dataReader = ref new DataReader(readStream);
                    create_task(dataReader->LoadAsync(static_cast<UINT32>(size))).then([this, file, dataReader](unsigned int numBytesLoaded)
                    {
                        String^ fileContent = dataReader->ReadString(numBytesLoaded);
                        delete dataReader; // As a best practice, explicitly close the dataReader resource as soon as it is no longer needed.
                        OutputTextBlock->Text = "The following text was read from '" + file->Name + "' using a stream:\n\n" + fileContent;
                    });
                }
                else
                {
                    delete readStream; // As a best practice, explicitly close the readStream resource as soon as it is no longer needed.
                    OutputTextBlock->Text = "File " + file->Name + " is too big for LoadAsync to load in a single chunk. Files larger than 4GB need to be broken into multiple chunks to be loaded by LoadAsync.";
                }
            }
            catch(COMException^ ex)
            {
                rootPage->HandleFileNotFoundException(ex);
            }
        });
    }
}
