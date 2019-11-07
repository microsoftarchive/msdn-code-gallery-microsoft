//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2_Read.xaml.cpp
// Implementation of the CdcAcmRead class.
//

#include "pch.h"
#include "Scenario2_Read.xaml.h"

using namespace SDKSample;
using namespace SDKSample::UsbCdcControl;

using namespace Platform;
using namespace Windows::Devices::Usb;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;


CdcAcmRead::CdcAcmRead()
{
    InitializeComponent();

    this->opRead = nullptr;
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
void CdcAcmRead::LoadState(Object^ navigationParameter, IMap<String^, Object^>^ pageState)
{
    (void) navigationParameter;    // Unused parameter

    if (pageState != nullptr && pageState->Size > 0)
    {
        auto iter = pageState->First();
        do
        {
            auto pair = iter->Current;
            auto control = FindControl(this, pair->Key);
            auto textBox = dynamic_cast<TextBox^>(control);
            if (textBox != nullptr)
            {
                textBox->Text = dynamic_cast<String^>(pair->Value);
                continue;
            }
            auto comboBox = dynamic_cast<ComboBox^>(control);
            if (comboBox != nullptr)
            {
                for (unsigned int j = 0; j < comboBox->Items->Size; j ++)
                {
                    auto item = (ComboBoxItem^)comboBox->Items->GetAt(j);
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
        this->buttonReadBulkIn->IsEnabled = false;
        this->buttonWatchBulkIn->IsEnabled = false;
        this->buttonStopWatching->IsEnabled = false;
        this->textBlockDeviceInUse->Text = "No device selected.";
        this->textBlockDeviceInUse->Foreground = ref new SolidColorBrush(Windows::UI::Colors::OrangeRed);
    }

    this->onDeviceAddedRegToken = UsbDeviceList::Singleton->DeviceAdded::add(
        ref new Windows::Foundation::EventHandler<UsbDeviceInfo^>(this, &CdcAcmRead::OnDeviceAdded));

    this->onDeviceRemovedRegToken = UsbDeviceList::Singleton->DeviceRemoved::add(
        ref new UsbDeviceList::DeviceRemovedHandler(this, &CdcAcmRead::OnDeviceRemoved));
}

/// <summary>
/// Preserves state associated with this page in case the application is suspended or the
/// page is discarded from the navigation cache.  Values must conform to the serialization
/// requirements of <see cref="SuspensionManager::SessionState"/>.
/// </summary>
/// <param name="pageState">An empty map to be populated with serializable state.</param>
void CdcAcmRead::SaveState(IMap<String^, Object^>^ pageState)
{
    UsbDeviceList::Singleton->DeviceAdded::remove(this->onDeviceAddedRegToken);
    UsbDeviceList::Singleton->DeviceRemoved::remove(this->onDeviceRemovedRegToken);

    if (pageState != nullptr)
    {
        pageState->Insert(this->textBoxBytesToRead->Name, this->textBoxBytesToRead->Text);
        pageState->Insert(this->textBoxReadTimeout->Name, this->textBoxReadTimeout->Text);
    }
}

void CdcAcmRead::OnDeviceAdded(Platform::Object^ sender, UsbDeviceInfo^ info)
{

}

Windows::Foundation::IAsyncAction^ CdcAcmRead::OnDeviceRemoved(Platform::Object^ sender, UsbDeviceInfo^ info)
{
    return this->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this, info]
    {
        if (this->SerialPortInfo != nullptr && this->SerialPortInfo->DeviceId == info->Id)
        {
            (ref new Windows::UI::Popups::MessageDialog(info->Name + " has been removed."))->ShowAsync();
            if (this->opRead != nullptr)
            {
                this->buttonStopWatching_Click(this, nullptr); // cancel read op if possible.
            }
            this->buttonReadBulkIn->IsEnabled = false;
            this->buttonWatchBulkIn->IsEnabled = false;
            this->buttonStopWatching->IsEnabled = false;
            this->textBlockDeviceInUse->Text = "No device selected.";
            this->textBlockDeviceInUse->Foreground = ref new SolidColorBrush(Windows::UI::Colors::OrangeRed);
        }
    }));
}

void CdcAcmRead::ReadByteOneByOne()
{
    auto buffer = ref new Windows::Storage::Streams::Buffer(1);
    buffer->Length = 1;

    concurrency::create_task(Read(buffer, -1)).then([this, buffer](concurrency::task<int> task)
    {
        this->opRead = nullptr;
        int count = 0;
        try
        {
            count = task.get();
        }
        catch (const concurrency::task_canceled&)
        {
            // buttonStopWatching looks clicked.
            return;
        }

        if (count > 0)
        {
            auto isAscii = this->radioButtonAscii->IsChecked->Value == true;
            auto temp = this->textBoxReadBulkInLogger->Text;
            temp += isAscii ? ::AsciiBufferToAsciiString(buffer) : ::BinaryBufferToBinaryString(buffer);
            this->textBoxReadBulkInLogger->Text = temp;
        }
        this->ReadByteOneByOne();
    });
}

Windows::Foundation::IAsyncOperation<int>^ CdcAcmRead::Read(Windows::Storage::Streams::Buffer^ buffer, int timeout)
{
    this->SerialPortInfo->Port->ReadTimeout = timeout;
    return this->opRead = this->SerialPortInfo->Port->Read(buffer, 0, buffer->Length);
}

void CdcAcmRead::buttonReadBulkIn_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (this->buttonReadBulkIn->Content->ToString() == "Read")
    {
        const int bytesToRead = _wtoi(this->textBoxBytesToRead->Text->Data());
        if (bytesToRead > 0)
        {
            // UI status.
            this->buttonReadBulkIn->Content = "Stop Read";
            this->buttonWatchBulkIn->IsEnabled = false;
            SDKSample::MainPage::Current->NotifyUser("Reading", SDKSample::NotifyType::StatusMessage);

            auto buffer = ref new Windows::Storage::Streams::Buffer(bytesToRead);
            buffer->Length = bytesToRead;
            const int timeout = _wtoi(this->textBoxReadTimeout->Text->Data());
            concurrency::create_task(this->Read(buffer, timeout)).then([this, bytesToRead, buffer](concurrency::task<int> task)
            {
                this->buttonReadBulkIn->Content = "Read";
                this->buttonWatchBulkIn->IsEnabled = true;
                this->opRead = nullptr;

                bool canceled = false;
                int count = 0;
                try
                {
                    count = task.get();
                }
                catch (const concurrency::task_canceled&)
                {
                    // canceled.
                    canceled = true;
                    SDKSample::MainPage::Current->NotifyUser("Canceled", SDKSample::NotifyType::ErrorMessage);
                }

                if (canceled == false)
                {
                    if (count < bytesToRead)
                    {
                        // This would be timeout.
                        SDKSample::MainPage::Current->NotifyUser("Timeout: read " + default::int32(count).ToString() + " byte(s)", SDKSample::NotifyType::ErrorMessage);
                    }
                    else
                    {
                        SDKSample::MainPage::Current->NotifyUser("Completed", SDKSample::NotifyType::StatusMessage);
                    }
                }

                if (count > 0)
                {
                    auto isAscii = this->radioButtonAscii->IsChecked->Value == true;
                    auto temp = this->textBoxReadBulkInLogger->Text;
                    temp += isAscii ? ::AsciiBufferToAsciiString(buffer) : ::BinaryBufferToBinaryString(buffer);
                    this->textBoxReadBulkInLogger->Text = temp;
                }
            });
        }
    }
    else
    {
        this->buttonStopWatching_Click(this, nullptr);
        this->buttonReadBulkIn->Content = "Read";
    }
}

void CdcAcmRead::buttonWatchBulkIn_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    this->buttonReadBulkIn->IsEnabled = false;
    this->buttonWatchBulkIn->IsEnabled = false;
    this->buttonStopWatching->IsEnabled = true;
    SDKSample::MainPage::Current->NotifyUser("", SDKSample::NotifyType::StatusMessage);

    this->ReadByteOneByOne();
}

void CdcAcmRead::buttonStopWatching_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (this->opRead)
    {
        this->opRead->Cancel();
        this->opRead = nullptr;
    }
    this->buttonReadBulkIn->IsEnabled = true;
    this->buttonWatchBulkIn->IsEnabled = true;
    this->buttonStopWatching->IsEnabled = false;
}

void CdcAcmRead::textBoxLogger_TextChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::TextChangedEventArgs^ e)
{
    GotoEndPosTextBox(dynamic_cast<TextBox^>(sender));
}

void CdcAcmRead::radioButtonDataFormat_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (sender->Equals(this->radioButtonAscii))
    {
        auto binary = this->textBoxReadBulkInLogger->Text;

        // Binary to Unicode.
        std::vector<std::wstring> list;
        std::wstring wstr(binary->Data());
        std::wstring::size_type curr = 0;
        for ( ; ; )
        {
            auto find = wstr.find(L" ", curr);
            if (find != std::wstring::npos)
            { 
                list.push_back(wstr.substr(curr, find - curr));
                curr = find + 1;
            }
            else
            {
                break;
            }
        }

        auto temp = ref new Platform::String();
        for (std::vector<std::wstring>::size_type i = 0; i < list.size(); i ++)
        {
            wchar_t *endptr;
            auto val = wcstol(list[i].c_str(), &endptr, 16);
            char ch[2];
            ((BYTE*)ch)[0] = (char)val;
            ch[1] = '\0';
            size_t wstrlen = 0;
            wchar_t wstr[2];
            mbstowcs_s(&wstrlen, wstr, 2, ch, _TRUNCATE);

            temp += ref new Platform::String(wstr);
        }

        this->textBoxReadBulkInLogger->Text = temp;
    }
    else if (sender->Equals(this->radioButtonBinary))
    {
        auto ascii = this->textBoxReadBulkInLogger->Text;

        // Unicode to ASCII.
        size_t mbLen = 0;
        char* szBuffer = new char[ascii->Length() + 1];
        wcstombs_s(&mbLen, szBuffer, ascii->Length() + 1, ascii->Data(), _TRUNCATE);

        auto temp = ref new Platform::String();
        for (size_t i = 0; i < mbLen; i ++)
        {
            if (szBuffer[i] == '\0')
                break;

            wchar_t buffer[4];
            swprintf_s(buffer, 4, L"%02X ", ((BYTE*)szBuffer)[i]);
            temp += ref new Platform::String(buffer);
        }

        delete [] szBuffer;

        this->textBoxReadBulkInLogger->Text = temp;
    }
}
