// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"
#include "Scenario4_WriteAndReadBytesInAFile.xaml.h"

using namespace SDKSample::FileAccess;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

Scenario4::Scenario4() : rootPage(MainPage::Current)
{
    InitializeComponent();
    rootPage->Initialize();
    rootPage->ValidateFile();
    WriteBytesButton->Click += ref new RoutedEventHandler(this, &Scenario4::WriteBytesButton_Click);
    ReadBytesButton->Click  += ref new RoutedEventHandler(this, &Scenario4::ReadBytesButton_Click);
}

IBuffer^ Scenario4::GetBufferFromString(String^ str)
{
    InMemoryRandomAccessStream^ memoryStream = ref new InMemoryRandomAccessStream();
    DataWriter^ dataWriter = ref new DataWriter(memoryStream);
    dataWriter->WriteString(str);
    return dataWriter->DetachBuffer();
}

void Scenario4::WriteBytesButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    StorageFile^ file = rootPage->SampleFile;
    if (file != nullptr)
    {
        String^ userContent = InputTextBox->Text;
        if (userContent != nullptr && !userContent->IsEmpty())
        {
            IBuffer^ buffer = GetBufferFromString(userContent);
            create_task(FileIO::WriteBufferAsync(file, buffer)).then([this, file, buffer, userContent](task<void> task)
            {
                try
                {
                    task.get();
                    rootPage->NotifyUser("The following " + buffer->Length.ToString() + " bytes of text were written to '" + file->Name + "':\n" + userContent, NotifyType::StatusMessage);
                }
                catch (COMException^ ex)
                {
                    rootPage->HandleFileNotFoundException(ex);
                }
            });
        }
        else
        {
            rootPage->NotifyUser("The text box is empty, please write something and then click 'Write' again.", NotifyType::ErrorMessage);
        }
    }
    else
    {
        rootPage->NotifyUserFileNotExist();
    }
}

void Scenario4::ReadBytesButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    StorageFile^ file = rootPage->SampleFile;
    if (file != nullptr)
    {
        create_task(FileIO::ReadBufferAsync(file)).then([this, file](task<IBuffer^> task)
        {
            try
            {
                IBuffer^ buffer = task.get();
                DataReader^ dataReader = DataReader::FromBuffer(buffer);
                String^ fileContent = dataReader->ReadString(buffer->Length);
                rootPage->NotifyUser("The following " + buffer->Length.ToString() + " bytes of text were read from '" + file->Name + "':\n" + fileContent, NotifyType::StatusMessage);
            }
            catch (COMException^ ex)
            {
                rootPage->HandleFileNotFoundException(ex);
            }
        });
    }
    else
    {
        rootPage->NotifyUserFileNotExist();
    }
}
