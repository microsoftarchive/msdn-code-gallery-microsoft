//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario3_Write.xaml.cpp
// Implementation of the CdcAcmWrite class
//

#include "pch.h"
#include "Scenario3_Write.xaml.h"

using namespace SDKSample;
using namespace SDKSample::UsbCdcControl;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

const DependencyProperty^ CdcAcmWrite::RawBinaryDataProperty = DependencyProperty::Register(
    "RawBinaryData",
    Windows::UI::Xaml::Interop::TypeName(Windows::Storage::Streams::IBuffer::typeid),
    Windows::UI::Xaml::Interop::TypeName(TextBox::typeid), nullptr
    );

CdcAcmWrite::CdcAcmWrite()
{
    InitializeComponent();

    auto controls = ref new BinaryDataControls();
    controls->Button = this->buttonLoadBinaryData1;
    controls->TextBox = this->textBoxBinaryData1;
    this->binaryDataControls.push_back(controls);

    controls = ref new BinaryDataControls();
    controls->Button = this->buttonLoadBinaryData2;
    controls->TextBox = this->textBoxBinaryData2;
    this->binaryDataControls.push_back(controls);
}

/// <summary>
/// Populates the page with content passed during navigation.  Any saved state is also
/// provided when recreating a page from a prior session.
/// </summary>
/// <param name="navigationParameter">The parameter value passed to
/// <see cref="Frame::Navigate(Type, Object)"/> when this page was initially requested.
/// </param>
/// <param name="pageState">A map of state preserved by this page during an earlier
/// session.  This will be null the first time a page is visited.</param>
void CdcAcmWrite::LoadState(Object^ navigationParameter, IMap<String^, Object^>^ pageState)
{
    (void) navigationParameter;    // Unused parameter

    if (pageState != nullptr && pageState->Size > 0)
    {
        auto iter = pageState->First();
        do
        {
            auto pair = iter->Current;
            auto control = FindControl(this, pair->Key);
            auto textBox = dynamic_cast<Windows::UI::Xaml::Controls::TextBox^>(control);
            if (textBox != nullptr)
            {
                textBox->Text = dynamic_cast<String^>(pair->Value);
                continue;
            }
            auto comboBox = dynamic_cast<Windows::UI::Xaml::Controls::ComboBox^>(control);
            if (comboBox != nullptr)
            {
                for (unsigned int j = 0; j < comboBox->Items->Size; j ++)
                {
                    auto item = (Windows::UI::Xaml::Controls::ComboBoxItem^)comboBox->Items->GetAt(j);
                    if ((String^)item->Content == dynamic_cast<String^>(pair->Value))
                    {
                        comboBox->SelectedIndex = j;
                        break;
                    }
                }
                continue;
            }
        }
        while (iter->MoveNext());
    }

    if (this->SerialPortInfo != nullptr)
    {
        this->textBlockDeviceInUse->Text = this->SerialPortInfo->Name;
    }
    else
    {
        this->buttonWriteBulkOut->IsEnabled = false;
        this->buttonSendBreak->IsEnabled = false;
        this->buttonWriteBinary1->IsEnabled = false;
        this->buttonWriteBinary2->IsEnabled = false;
        this->textBlockDeviceInUse->Text = "No device selected.";
        this->textBlockDeviceInUse->Foreground = ref new SolidColorBrush(Windows::UI::Colors::OrangeRed);
    }

    this->onDeviceAddedRegToken = UsbDeviceList::Singleton->DeviceAdded::add(
        ref new Windows::Foundation::EventHandler<UsbDeviceInfo^>(this, &CdcAcmWrite::OnDeviceAdded));

    this->onDeviceRemovedRegToken = UsbDeviceList::Singleton->DeviceRemoved::add(
        ref new UsbDeviceList::DeviceRemovedHandler(this, &CdcAcmWrite::OnDeviceRemoved));
}

/// <summary>
/// Preserves state associated with this page in case the application is suspended or the
/// page is discarded from the navigation cache.  Values must conform to the serialization
/// requirements of <see cref="SuspensionManager::SessionState"/>.
/// </summary>
/// <param name="pageState">An empty map to be populated with serializable state.</param>
void CdcAcmWrite::SaveState(IMap<String^, Object^>^ pageState)
{
    UsbDeviceList::Singleton->DeviceAdded::remove(this->onDeviceAddedRegToken);
    UsbDeviceList::Singleton->DeviceRemoved::remove(this->onDeviceRemovedRegToken);
}

void CdcAcmWrite::OnDeviceAdded(Platform::Object^ sender, UsbDeviceInfo^ info)
{

}

Windows::Foundation::IAsyncAction^ CdcAcmWrite::OnDeviceRemoved(Platform::Object^ sender, UsbDeviceInfo^ info)
{
    return this->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this, info]
    {
        if (this->SerialPortInfo != nullptr)
        {
            if (this->SerialPortInfo->DeviceId == info->Id)
            {
                (ref new Windows::UI::Popups::MessageDialog(info->Name + " has been removed."))->ShowAsync();
                this->buttonWriteBulkOut->IsEnabled = false;
                this->buttonSendBreak->IsEnabled = false;
                this->buttonWriteBinary1->IsEnabled = false;
                this->buttonWriteBinary2->IsEnabled = false;
                this->textBlockDeviceInUse->Text = "No device selected.";
                this->textBlockDeviceInUse->Foreground = ref new SolidColorBrush(Windows::UI::Colors::OrangeRed);
            }
        }
    }));
}

void CdcAcmWrite::textBoxLogger_TextChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::TextChangedEventArgs^ e)
{
    GotoEndPosTextBox(dynamic_cast<TextBox^>(sender));
}

void CdcAcmWrite::buttonWriteBulkOut_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (this->SerialPortInfo != nullptr)
    {
        auto dataToWrite = this->textBoxDataToWrite->Text;

        // Unicode to ASCII.
        size_t mbLen = 0;
        char* szBuffer = new char[dataToWrite->Length() + 1];
        wcstombs_s(&mbLen, szBuffer, dataToWrite->Length() + 1, dataToWrite->Data(), _TRUNCATE);

        auto buffer = ref new Windows::Storage::Streams::Buffer(mbLen);
        Microsoft::WRL::ComPtr<Windows::Storage::Streams::IBufferByteAccess> pBufferByteAccess; 
        Microsoft::WRL::ComPtr<IUnknown> pBuffer((IUnknown*)buffer); 
        pBuffer.As(&pBufferByteAccess);
        BYTE* rawBuffer;
        pBufferByteAccess->Buffer(&rawBuffer);
        CopyMemory(rawBuffer, szBuffer, mbLen);
        buffer->Length = mbLen;
        if (this->checkBoxSendNullTerminateCharToBulkOut->IsChecked->Value == false)
        {
            if (szBuffer[mbLen - 1] == '\0')
            {
                buffer->Length --;
            }
        }
        delete [] szBuffer;

        concurrency::create_task(this->SerialPortInfo->Port->Write(buffer, 0, buffer->Length)).then([this, dataToWrite, buffer](Windows::Foundation::HResult hr)
        {
            default::int32 num = buffer->Length;
            auto temp = this->textBoxWriteLog->Text;
            temp += "Write completed: \"" + dataToWrite + "\" (" + num.ToString() + " bytes)\n";
            this->textBoxWriteLog->Text = temp;

            this->textBoxDataToWrite->Text = "";
        });
    }
}

/// <summary>
/// Generates a RS-232C break signal during a time length.
/// </summary>
void CdcAcmWrite::buttonSendBreak_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (this->SerialPortInfo != nullptr)
    {
        this->SerialPortInfo->Port->SetControlRequest(RequestCode::SendBreak, _wtoi(textBoxDurationOfBreak->Text->Data()), nullptr);
    }   
}

void CdcAcmWrite::buttonWriteBinary_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    TextBox^ textBox = nullptr;
    if (sender->Equals(this->buttonWriteBinary1))
    {
        textBox = this->textBoxBinaryData1;
    }
    else if (sender->Equals(this->buttonWriteBinary2))
    {
        textBox = this->textBoxBinaryData2;
    }

    if (!!textBox)
    {
        auto value = textBox->GetValue(const_cast<DependencyProperty^>(RawBinaryDataProperty) );
        if (!!value)
        {
            auto buffer = dynamic_cast<Windows::Storage::Streams::IBuffer^>(value) ;
            concurrency::create_task(this->SerialPortInfo->Port->Write(buffer, 0, buffer->Length)).then([this, buffer](Windows::Foundation::HResult hr)
            {
                default::int32 num = buffer->Length;
                auto temp = this->textBoxWriteLog->Text;
                temp += "Write completed: \"" + BinaryBufferToBinaryString(buffer) + "\" (" + num.ToString() + " bytes)\n";
                this->textBoxWriteLog->Text = temp;
            });
        }
    }
}

void CdcAcmWrite::buttonLoadBinaryData_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto picker = ref new Windows::Storage::Pickers::FileOpenPicker();
    picker->SuggestedStartLocation = Windows::Storage::Pickers::PickerLocationId::DocumentsLibrary;
    picker->FileTypeFilter->Append(".bin");
    Concurrency::create_task(picker->PickSingleFileAsync()).then([](Concurrency::task<Windows::Storage::StorageFile^> task)
    {
        Windows::Storage::StorageFile^ file = task.get();
        if (file == nullptr)
        {
            Concurrency::cancel_current_task();
        }
        return file->OpenAsync(Windows::Storage::FileAccessMode::Read);
    }
    ).then([this, sender](Windows::Storage::Streams::IRandomAccessStream^ stream)
    {
        auto reader = ref new Windows::Storage::Streams::DataReader(stream->GetInputStreamAt(0));
        Concurrency::create_task(reader->LoadAsync((unsigned int)stream->Size)).then([this, sender, reader, stream](unsigned int count)
        {
            auto buffer = reader->ReadBuffer(count);
            auto str = BinaryBufferToBinaryString(buffer);
            TextBox^ textBox = nullptr;
            for (size_t i = 0; i < this->binaryDataControls.size(); i ++)
            {
                if (sender->Equals(this->binaryDataControls[i]->Button))
                {
                    textBox = this->binaryDataControls[i]->TextBox;
                    break;
                }
            }
            if (textBox != nullptr)
            {
                textBox->Text = str;
                textBox->SetValue(const_cast<DependencyProperty^>(RawBinaryDataProperty), buffer);
            }
        });
    });
}