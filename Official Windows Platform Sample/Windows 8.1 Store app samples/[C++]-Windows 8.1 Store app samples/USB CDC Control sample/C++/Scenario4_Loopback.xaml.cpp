//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario4_Loopback.xaml.cpp
// Implementation of the CdcAcmLoopback class
//

#include "pch.h"
#include "Scenario4_Loopback.xaml.h"

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


CdcAcmLoopback::CdcAcmLoopback()
{
    InitializeComponent();

    this->opRead = nullptr;
    this->comboBoxDevicesArray = ref new Platform::Array<ComboBox^>(2);
    this->comboBoxDevicesArray[0] = this->comboBoxDevices1;
    this->comboBoxDevicesArray[1] = this->comboBoxDevices2;
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
void CdcAcmLoopback::LoadState(Object^ navigationParameter, IMap<String^, Object^>^ pageState)
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
                if (pair->Key == this->comboBoxDevices1->Name)
                {
                    this->previousSelectedDeviceId1 = dynamic_cast<String^>(pair->Value);
                }
                else if (pair->Key == this->comboBoxDevices2->Name)
                {
                    this->previousSelectedDeviceId2 = dynamic_cast<String^>(pair->Value);
                }
                else
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

    // Dispose all devices which are used in Scenario1 to 3.
    UsbDeviceList::Singleton->DisposeAll();

    std::for_each(begin(DeviceList::Instances), end(DeviceList::Instances), [this](DeviceList^ deviceList)
    {
        for (unsigned int i = 0; i < deviceList->Devices->Size; i++)
        {
            auto info = deviceList->Devices->GetAt(i);
            this->OnDeviceAdded(this, ref new UsbDeviceInfo(info));
        }
    });

    this->onDeviceAddedRegToken = UsbDeviceList::Singleton->DeviceAdded::add(
        ref new Windows::Foundation::EventHandler<UsbDeviceInfo^>(this, &CdcAcmLoopback::OnDeviceAdded));

    this->onDeviceRemovedRegToken = UsbDeviceList::Singleton->DeviceRemoved::add(
        ref new UsbDeviceList::DeviceRemovedHandler(this, &CdcAcmLoopback::OnDeviceRemoved));

    this->buttonLoopbackTest->IsEnabled = false;
    this->buttonStopLoopback->IsEnabled = false;
}

/// <summary>
/// Preserves state associated with this page in case the application is suspended or the
/// page is discarded from the navigation cache.  Values must conform to the serialization
/// requirements of <see cref="SuspensionManager::SessionState"/>.
/// </summary>
/// <param name="pageState">An empty map to be populated with serializable state.</param>
void CdcAcmLoopback::SaveState(IMap<String^, Object^>^ pageState)
{
    UsbDeviceList::Singleton->DeviceAdded::remove(this->onDeviceAddedRegToken);
    UsbDeviceList::Singleton->DeviceRemoved::remove(this->onDeviceRemovedRegToken);

    // Cancel, if a read op is processing.
    this->buttonStopLoopback_Click(this, nullptr);

    // Dispose all devices (prior to going to Scenario1 to 3).
    UsbDeviceList::Singleton->DisposeAll();

    if (pageState != nullptr)
    {
        if (this->comboBoxDevices1->SelectedValue != nullptr)
        {
            pageState->Insert(this->comboBoxDevices1->Name, ((UsbDeviceComboBoxItem^)this->comboBoxDevices1->SelectedValue)->Id);
        }
        if (this->comboBoxDevices2->SelectedValue != nullptr)
        {
            pageState->Insert(this->comboBoxDevices2->Name, ((UsbDeviceComboBoxItem^)this->comboBoxDevices2->SelectedValue)->Id);
        }
        pageState->Insert(this->textBoxDTERate->Name, textBoxDTERate->Text);
        pageState->Insert(this->comboBoxCharFormat->Name, ((Windows::UI::Xaml::Controls::ComboBoxItem^)this->comboBoxCharFormat->SelectedValue)->Content);
        pageState->Insert(this->comboBoxParityType->Name, ((Windows::UI::Xaml::Controls::ComboBoxItem^)this->comboBoxParityType->SelectedValue)->Content);
        pageState->Insert(this->comboBoxDataBits->Name, ((Windows::UI::Xaml::Controls::ComboBoxItem^)this->comboBoxDataBits->SelectedValue)->Content);
        pageState->Insert(this->comboBoxRTS->Name, ((Windows::UI::Xaml::Controls::ComboBoxItem^)this->comboBoxRTS->SelectedValue)->Content);
        pageState->Insert(this->comboBoxDTR->Name, ((Windows::UI::Xaml::Controls::ComboBoxItem^)this->comboBoxDTR->SelectedValue)->Content);
    }
}

void CdcAcmLoopback::OnDeviceAdded(Platform::Object^ sender, UsbDeviceInfo^ info)
{
    const auto dispatcher = this->Dispatcher;
    dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this, info]
    {
        this->comboBoxDevices1->Items->Append(ref new UsbDeviceComboBoxItem(info));
        this->comboBoxDevices2->Items->Append(ref new UsbDeviceComboBoxItem(info));
        if (this->comboBoxDevices1->SelectedIndex == -1)
        {
            if (this->previousSelectedDeviceId1 == info->Id || this->previousSelectedDeviceId1 == L"")
            {
                this->comboBoxDevices1->SelectedIndex = 0;
            }
        }
        if (this->comboBoxDevices2->SelectedIndex == -1)
        {
            if (this->previousSelectedDeviceId2 == info->Id || this->previousSelectedDeviceId2 == L"")
            {
                this->comboBoxDevices2->SelectedIndex = 0;
            }
        }
    }));
}

Windows::Foundation::IAsyncAction^ CdcAcmLoopback::OnDeviceRemoved(Platform::Object^ sender, UsbDeviceInfo^ info)
{
    return this->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this, info]
    {
        auto showMessageDialog = false;
        for (unsigned int combo = 0; combo < this->comboBoxDevicesArray->Length; combo ++)
        {
            ComboBox^ comboBoxDevices = this->comboBoxDevicesArray[combo];
            for (unsigned int i = 0; i < comboBoxDevices->Items->Size; i ++)
            {
                auto item = (UsbDeviceComboBoxItem^)comboBoxDevices->Items->GetAt(i);
                if (item->Id == info->Id)
                {
                    if (this->buttonInitialize->IsEnabled == false && comboBoxDevices->SelectedIndex == i)
                    {
                        showMessageDialog = true;
                        if (this->opRead != nullptr)
                        {
                            this->buttonStopLoopback_Click(this, nullptr); // cancel read op if possible.
                        }
                    }
                    comboBoxDevices->Items->RemoveAt(i);
                    if (comboBoxDevices->SelectedIndex == -1 && comboBoxDevices->Items->Size > 0)
                    {
                        comboBoxDevices->SelectedIndex = 0;
                    }
                    break;
                }
            }
        }

        if (showMessageDialog)
        {
            (ref new Windows::UI::Popups::MessageDialog(info->Name + " has been removed."))->ShowAsync();
        }
    }));
}

Windows::Foundation::IAsyncOperation<int>^ CdcAcmLoopback::Read(UsbSerialPort^ port, Windows::Storage::Streams::Buffer^ buffer, int timeout)
{
    port->ReadTimeout = timeout;
    return this->opRead = port->Read(buffer, 0, buffer->Length);
}

void CdcAcmLoopback::comboBoxDevices_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^)
{
    // Restore UI state.
    this->buttonStopLoopback_Click(this, nullptr); // cancel read op if possible.
    this->buttonLoopbackTest->IsEnabled = false;
    this->buttonStopLoopback->IsEnabled = false;
    this->buttonInitialize->IsEnabled = true;

    // Dispose all devices.
    UsbDeviceList::Singleton->DisposeAll();

    auto comboBoxDevices = (ComboBox^)sender;
    if (comboBoxDevices->SelectedItem == nullptr)
    {
        return;
    }

    for (unsigned int combo = 0; combo < this->comboBoxDevicesArray->Length; combo ++)
    {
        ComboBox^ comboBox = this->comboBoxDevicesArray[combo];
        if (comboBox->SelectedItem == nullptr)
        {
            return;
        }
    }

    auto item1 = (UsbDeviceComboBoxItem^)this->comboBoxDevices1->SelectedItem;
    auto item2 = (UsbDeviceComboBoxItem^)this->comboBoxDevices2->SelectedItem;

    if (item1->Id == item2->Id)
    {
        // Both comboBoxes are selecting a same device.
        auto currIndex = this->comboBoxDevices2->SelectedIndex;
        for (unsigned int index = 0; index < this->comboBoxDevices2->Items->Size; index++)
        {
            if (index != currIndex)
            {
                this->comboBoxDevices2->SelectedIndex = index;
                break;
            }
        }
        if (currIndex == this->comboBoxDevices2->SelectedIndex)
        {
            this->comboBoxDevices2->SelectedIndex = -1;
            return;
        }
    }
}

void CdcAcmLoopback::buttonInitialize_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (this->comboBoxDevices1->SelectedIndex == -1 || this->comboBoxDevices2->SelectedIndex == -1)
    {
        return;
    }

    auto dispatcher = this->Dispatcher;
    const auto deviceId1 = ((UsbDeviceComboBoxItem^)this->comboBoxDevices1->SelectedItem)->Id;
    const auto deviceId2 = ((UsbDeviceComboBoxItem^)this->comboBoxDevices2->SelectedItem)->Id;
    const auto deviceName1 = (String^)((UsbDeviceComboBoxItem^)this->comboBoxDevices1->SelectedItem)->Content;
    const auto deviceName2 = (String^)((UsbDeviceComboBoxItem^)this->comboBoxDevices2->SelectedItem)->Content;
    const auto dteRate = _wtoi(this->textBoxDTERate->Text->Data());
    const auto parity = (Parity)this->comboBoxParityType->SelectedIndex;
    const auto dataBits = _wtoi((((ComboBoxItem^)this->comboBoxDataBits->SelectedItem)->Content->ToString())->Data());
    const auto charFormat = (StopBits)this->comboBoxCharFormat->SelectedIndex;
    const auto dtr = this->comboBoxDTR->SelectedIndex != 0;
    const auto rts = this->comboBoxRTS->SelectedIndex != 0;

    std::vector<concurrency::task<void>> createSerialPortTasks;

    std::array<UsbDeviceInfo^, 2> deviceInfos = { ref new UsbDeviceInfo(deviceId1, deviceName1), ref new UsbDeviceInfo(deviceId2, deviceName2) };
    std::for_each(begin(deviceInfos), end(deviceInfos), [this, dispatcher, dteRate, parity, dataBits, charFormat, dtr, rts, &createSerialPortTasks](UsbDeviceInfo^ deviceInfo)
    {
        createSerialPortTasks.push_back(concurrency::create_task(UsbDevice::FromIdAsync(deviceInfo->Id)).then([this, dispatcher, dteRate, parity, dataBits, charFormat, dtr, rts, deviceInfo](UsbDevice^ usbDevice)
        {
            return concurrency::create_task(UsbSerialPort::Create(usbDevice)).then([this, dispatcher, dteRate, parity, dataBits, charFormat, dtr, rts, deviceInfo, usbDevice](UsbSerialPort^ port)
            {
                if (!port)
                {
                    if (!!usbDevice)
                    {
                        delete usbDevice;
                    }

                    // Return a dummy task.
                    return concurrency::create_task([](){});
                }

                auto addToListTask = dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([port, deviceInfo]()
                {
                    UsbDeviceList::Singleton->List->Append(ref new UsbSerialPortInfo(port, deviceInfo->Id, deviceInfo->Name));
                }));

                return concurrency::create_task(port->Open(dteRate, parity, dataBits, charFormat)).then([port, dtr](Windows::Foundation::HResult hr)
                {
                    if (hr.Value != S_OK)
                    {
                        concurrency::cancel_current_task();
                    }
                    // DtrEnable
                    return port->DtrEnable_set(dtr);
                }
                ).then([port, rts]()
                {
                    // RtsEnable
                    return port->RtsEnable_set(rts);
                }
                ).then([addToListTask]()
                {
                    return addToListTask;
                });
            });
        }));
    });

    concurrency::when_all(begin(createSerialPortTasks), end(createSerialPortTasks)).then([this, dispatcher]()
    {
        dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this]()
        {
            if (this->SerialPortInfo1 != nullptr && this->SerialPortInfo2 != nullptr)
            {
                this->buttonLoopbackTest->IsEnabled = true;
                this->buttonInitialize->IsEnabled = false;
                SDKSample::MainPage::Current->NotifyUser("Initialized.", SDKSample::NotifyType::StatusMessage);
            }
            else
            {
                String^ deviceNumber = nullptr;

                if (!this->SerialPortInfo1)
                {
                    if (!this->SerialPortInfo2)
                    {
                        deviceNumber = "Both devices";
                    }
                    else
                    {
                        deviceNumber = "Device 1";
                    }
                }
                else
                {
                    deviceNumber = "Device 2";
                }

                SDKSample::MainPage::Current->NotifyUser(deviceNumber + " failed to be initialized.", SDKSample::NotifyType::ErrorMessage);

                // Close all devices because one of the devices failed to be opened
                UsbDeviceList::Singleton->DisposeAll();
            }
        }));
    });
}

void CdcAcmLoopback::buttonLoopbackTest_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto dispatcher = this->Dispatcher;

    // Unicode to ASCII.
    auto textToSend = this->textBoxForLoopback->Text;                    
    size_t mbLen = 0;
    char* szBuffer = new char[textToSend->Length() + 1];
    wcstombs_s(&mbLen, szBuffer, textToSend->Length() + 1, textToSend->Data(), _TRUNCATE);

    auto buffer = ref new Windows::Storage::Streams::Buffer(mbLen);
    Microsoft::WRL::ComPtr<Windows::Storage::Streams::IBufferByteAccess> pBufferByteAccess; 
    Microsoft::WRL::ComPtr<IUnknown> pBuffer((IUnknown*)buffer); 
    pBuffer.As(&pBufferByteAccess);
    BYTE* rawBuffer;
    pBufferByteAccess->Buffer(&rawBuffer);
    CopyMemory(rawBuffer, szBuffer, mbLen);
    buffer->Length = mbLen;
    delete [] szBuffer;

    auto readBuffer = ref new Windows::Storage::Streams::Buffer(buffer->Length);
    readBuffer->Length = buffer->Length;

    this->buttonLoopbackTest->IsEnabled = false;
    this->buttonStopLoopback->IsEnabled = true;
    SDKSample::MainPage::Current->NotifyUser("", SDKSample::NotifyType::StatusMessage);

    // _serialport1 to _serialport2
    auto writeTask = concurrency::create_task(this->SerialPortInfo1->Port->Write(buffer, 0, buffer->Length)).then([](Windows::Foundation::HResult hr){ return (int)0; });
    auto readTask = concurrency::create_task(this->Read(this->SerialPortInfo2->Port, readBuffer, -1));
    (writeTask && readTask).then([this, dispatcher, textToSend, buffer, readBuffer, readTask](concurrency::task<std::vector<int>> task)
    {
        int count = 0;
        try
        {
            count = readTask.get();
        }
        catch (const concurrency::task_canceled&)
        {
            this->opRead = nullptr;
            dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this]()
            {
                SDKSample::MainPage::Current->NotifyUser("Canceled", SDKSample::NotifyType::ErrorMessage);
                this->buttonLoopbackTest->IsEnabled = true;
                this->buttonStopLoopback->IsEnabled = false;
            }));
            concurrency::cancel_current_task();
        }
        
        this->opRead = nullptr;
        readBuffer->Length = count;

        this->statusMessage = "";

        bool isSame = false;
        HRESULT hr = IsSameData(buffer, readBuffer, &isSame);
        if (hr != S_OK)
        {
            concurrency::cancel_current_task();
        }
        else if (isSame)
        {
            this->statusMessage += "CDC device 2 received \"" + textToSend + "\" from CDC device 1. ";
        }
        else
        {
            this->statusMessage += "Loopback failed: CDC device 1 to CDC device 2. ";
        }

        // _serialport2 to _serialport1
        auto writeTask2 = concurrency::create_task(this->SerialPortInfo2->Port->Write(buffer, 0, buffer->Length)).then([](Windows::Foundation::HResult hr){ return (int) 0; });
        auto readTask2 = concurrency::create_task(this->Read(this->SerialPortInfo1->Port, readBuffer, -1));
        return (writeTask2 && readTask2).then([this, dispatcher, textToSend, buffer, readBuffer, readTask2](concurrency::task<std::vector<int>> task)
        {
            int count = 0;
            try
            {
                count = readTask2.get();
            }
            catch (const concurrency::task_canceled&)
            {
                this->opRead = nullptr;
                dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this]()
                {
                    SDKSample::MainPage::Current->NotifyUser("Canceled", SDKSample::NotifyType::ErrorMessage);
                    this->buttonLoopbackTest->IsEnabled = true;
                    this->buttonStopLoopback->IsEnabled = false;
                }));
                concurrency::cancel_current_task();
            }

            this->opRead = nullptr;
            readBuffer->Length = count;

            bool isSame = false;
            HRESULT hr = IsSameData(buffer, readBuffer, &isSame);
            if (hr != S_OK)
            {
                concurrency::cancel_current_task();
            }
            else if (isSame)
            {
                this->statusMessage += "CDC device 1 received \"" + textToSend + "\" from CDC device 2. ";
            }
            else
            {
                this->statusMessage += "Loopback failed: CDC device 2 to CDC device 1. ";
            }

            dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this]()
            {
                this->buttonLoopbackTest->IsEnabled = true;
                this->buttonStopLoopback->IsEnabled = false;
                SDKSample::MainPage::Current->NotifyUser(this->statusMessage, SDKSample::NotifyType::StatusMessage);
            }));
        });
    });
}

void CdcAcmLoopback::buttonStopLoopback_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (!!this->opRead)
    {
        this->buttonStopLoopback->IsEnabled = false;
        this->opRead->Cancel();
        this->opRead = nullptr;
    }
}

void CdcAcmLoopback::textBoxLogger_TextChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::TextChangedEventArgs^ e)
{
    GotoEndPosTextBox(dynamic_cast<TextBox^>(sender));
}