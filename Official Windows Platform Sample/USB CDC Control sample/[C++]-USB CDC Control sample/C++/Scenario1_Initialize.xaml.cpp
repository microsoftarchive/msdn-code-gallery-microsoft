//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1_Initialize.xaml.cpp
// Implementation of the CdcAcmInitialize class.
//

#include "pch.h"
#include "Scenario1_Initialize.xaml.h"

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


CdcAcmInitialize::CdcAcmInitialize()
{
    InitializeComponent();

    this->previousSelectedDeviceId = ref new String();
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
void CdcAcmInitialize::LoadState(Object^ navigationParameter, IMap<String^, Object^>^ pageState)
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
                if (pair->Key == "comboBoxDevices")
                {
                    this->previousSelectedDeviceId = dynamic_cast<String^>(pair->Value);
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

    std::for_each(begin(DeviceList::Instances), end(DeviceList::Instances), [this](DeviceList^ deviceList)
    {
        for (unsigned int i = 0; i < deviceList->Devices->Size; i++)
        {
            auto info = deviceList->Devices->GetAt(i);
            this->OnDeviceAdded(this, ref new UsbDeviceInfo(info));
        }
    });

    if (UsbDeviceList::Singleton->List->Size > 0)
    {
        this->comboBoxDevices->IsEnabled = false;
        this->buttonDeviceSelect->IsEnabled = false;
        this->buttonDeviceDeselect->IsEnabled = true;
    }
    else
    {
        this->buttonInitialize->IsEnabled = false;
        this->buttonDeviceDeselect->IsEnabled = false;
    }

    this->onDeviceAddedRegToken = UsbDeviceList::Singleton->DeviceAdded::add(
        ref new Windows::Foundation::EventHandler<UsbDeviceInfo^>(this, &CdcAcmInitialize::OnDeviceAdded));

    this->onDeviceRemovedRegToken = UsbDeviceList::Singleton->DeviceRemoved::add(
        ref new UsbDeviceList::DeviceRemovedHandler(this, &CdcAcmInitialize::OnDeviceRemoved));

    UsbDeviceList::Singleton->StartWatcher();
}

/// <summary>
/// Preserves state associated with this page in case the application is suspended or the
/// page is discarded from the navigation cache.  Values must conform to the serialization
/// requirements of <see cref="SuspensionManager::SessionState"/>.
/// </summary>
/// <param name="pageState">An empty map to be populated with serializable state.</param>
void CdcAcmInitialize::SaveState(IMap<String^, Object^>^ pageState)
{
    UsbDeviceList::Singleton->DeviceAdded::remove(this->onDeviceAddedRegToken);
    UsbDeviceList::Singleton->DeviceRemoved::remove(this->onDeviceRemovedRegToken);

    if (pageState != nullptr)
    {
        if (this->comboBoxDevices->SelectedValue != nullptr)
        {
            pageState->Insert(this->comboBoxDevices->Name, ((UsbDeviceComboBoxItem^)this->comboBoxDevices->SelectedValue)->Id);
        }
        pageState->Insert(this->textBoxDTERate->Name, this->textBoxDTERate->Text);
        pageState->Insert(this->comboBoxCharFormat->Name, ((ComboBoxItem^)this->comboBoxCharFormat->SelectedValue)->Content);
        pageState->Insert(this->comboBoxParityType->Name, ((ComboBoxItem^)this->comboBoxParityType->SelectedValue)->Content);
        pageState->Insert(this->comboBoxDataBits->Name, ((ComboBoxItem^)this->comboBoxDataBits->SelectedValue)->Content);
        pageState->Insert(this->comboBoxRTS->Name, ((ComboBoxItem^)this->comboBoxRTS->SelectedValue)->Content);
        pageState->Insert(this->comboBoxDTR->Name, ((ComboBoxItem^)this->comboBoxDTR->SelectedValue)->Content);
    }
}

void CdcAcmInitialize::OnDeviceAdded(Platform::Object^ sender, UsbDeviceInfo^ info)
{
    const auto dispatcher = this->Dispatcher;
    if (dispatcher->HasThreadAccess)
    {
        this->AddDeviceToComboBox(info);
    }
    else
    dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this, info]
    {
        this->AddDeviceToComboBox(info);
    }));
}

void CdcAcmInitialize::AddDeviceToComboBox(UsbDeviceInfo^ info)
{
    this->comboBoxDevices->Items->InsertAt(0, ref new UsbDeviceComboBoxItem(info));
    if (this->SerialPortInfo != nullptr && this->SerialPortInfo->DeviceId == info->Id)
    {
        this->comboBoxDevices->SelectedIndex = 0;
    }
    else if (this->comboBoxDevices->SelectedIndex == -1)
    {
        if (this->previousSelectedDeviceId == info->Id || this->previousSelectedDeviceId == L"")
        {
            this->comboBoxDevices->SelectedIndex = 0;
        }
    }
}

Windows::Foundation::IAsyncAction^ CdcAcmInitialize::OnDeviceRemoved(Platform::Object^ sender, UsbDeviceInfo^ info)
{
    return this->Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this, info]
    {
        for (unsigned int i = 0; i < this->comboBoxDevices->Items->Size; i ++)
        {
            auto item = (UsbDeviceComboBoxItem^)this->comboBoxDevices->Items->GetAt(i);
            if (item->Id == info->Id)
            {
                const auto isEnabled = this->comboBoxDevices->IsEnabled;
                this->comboBoxDevices->IsEnabled = true;
                this->comboBoxDevices->Items->RemoveAt(i);
                if (this->SerialPortInfo != nullptr && this->SerialPortInfo->DeviceId == info->Id)
                {
                    (ref new Windows::UI::Popups::MessageDialog(info->Name + " has been removed."))->ShowAsync();
                    this->buttonDeviceDeselect_Click(nullptr, nullptr);
                    if (this->comboBoxDevices->Items->Size > 0)
                    {
                        this->comboBoxDevices->SelectedIndex = 0;
                    }
                }
                else
                {
                    if (this->comboBoxDevices->SelectedIndex == -1)
                    {
                        this->comboBoxDevices->SelectedIndex = 0;
                    }
                    this->comboBoxDevices->IsEnabled = isEnabled;
                }
                return;
            }
        }
    }));
}

void CdcAcmInitialize::buttonDeviceSelect_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // No device selected.
    if (this->comboBoxDevices->SelectedIndex == -1)
    {
        return;
    }

    auto dispatcher = this->Dispatcher;
    const auto deviceId = ((UsbDeviceComboBoxItem^)this->comboBoxDevices->SelectedItem)->Id;
    const auto deviceName = (Platform::String^)((UsbDeviceComboBoxItem^)this->comboBoxDevices->SelectedItem)->Content;

    Concurrency::create_task(UsbDevice::FromIdAsync(deviceId)).then([this, dispatcher, deviceId, deviceName](UsbDevice^ usbDevice)
    {
        return Concurrency::create_task(UsbSerialPort::Create(usbDevice)).then([this, dispatcher, deviceId, deviceName, usbDevice](Concurrency::task<UsbSerialPort^> task)
        {
            try
            {
                UsbSerialPort^ serialport = task.get();
                if (serialport == nullptr)
                {
                    dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([deviceName]()
                    {
                        SDKSample::MainPage::Current->NotifyUser(deviceName + " is not compatible with CDC ACM.", SDKSample::NotifyType::ErrorMessage);
                    }));
                    if (!!usbDevice)
                    {
                        delete usbDevice;
                    }
                    return;
                }

                UsbDeviceList::Singleton->List->Append(ref new UsbSerialPortInfo(serialport, deviceId, deviceName));
                dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([this]()
                {
                    this->comboBoxDevices->IsEnabled = false;
                    this->buttonDeviceSelect->IsEnabled = false;
                    this->buttonInitialize->IsEnabled = true;
                    this->buttonDeviceDeselect->IsEnabled = true;
                    SDKSample::MainPage::Current->NotifyUser("", SDKSample::NotifyType::StatusMessage);
                }));
            }
            catch (Platform::Exception^ exception)
            {
                dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, ref new Windows::UI::Core::DispatchedHandler([exception]()
                {
                    SDKSample::MainPage::Current->NotifyUser(exception->Message, SDKSample::NotifyType::ErrorMessage);
                }));
            }
        });
    });
}

void CdcAcmInitialize::buttonDeviceDeselect_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Platform::String^ prevSelectedDeviceId = nullptr;
    if (this->SerialPortInfo != nullptr)
    {
        prevSelectedDeviceId = this->SerialPortInfo->DeviceId;
    }
    this->comboBoxDevices->IsEnabled = true;
    UsbDeviceList::Singleton->DisposeAll();
    std::for_each(begin(DeviceList::Instances), end(DeviceList::Instances), [this, prevSelectedDeviceId](DeviceList^ deviceList)
    {
        for (unsigned int i = 0; i < deviceList->Devices->Size; i++)
        {
            auto info = deviceList->Devices->GetAt(i);
            int foundIndex = -1;
            for (unsigned int j = 0; j < this->comboBoxDevices->Items->Size; j++)
            {
                if (((UsbDeviceComboBoxItem^) this->comboBoxDevices->Items->GetAt(j))->Id == info->Id)
                {
                    foundIndex = j;
                    break;
                }
            }
            if (foundIndex == -1)
            {
                this->comboBoxDevices->Items->InsertAt(0, ref new UsbDeviceComboBoxItem(info));
                foundIndex = 0;
            }

            if (prevSelectedDeviceId == nullptr || prevSelectedDeviceId == info->Id)
            {
                this->comboBoxDevices->SelectedIndex = foundIndex;
            }
        }
    });
    this->buttonDeviceSelect->IsEnabled = true;
    this->buttonInitialize->IsEnabled = false;
    this->buttonDeviceDeselect->IsEnabled = false;
}

void CdcAcmInitialize::buttonInitialize_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (this->SerialPortInfo == nullptr)
    {
        return;
    }

    auto dispatcher = this->Dispatcher;
    const auto dteRate = _wtoi(this->textBoxDTERate->Text->Data());
    const auto parity = (Parity)this->comboBoxParityType->SelectedIndex;
    const auto dataBits = _wtoi((((ComboBoxItem^)this->comboBoxDataBits->SelectedItem)->Content->ToString())->Data());
    const auto charFormat = (StopBits)this->comboBoxCharFormat->SelectedIndex;
    const auto dtr = this->comboBoxDTR->SelectedIndex != 0;
    const auto rts = this->comboBoxRTS->SelectedIndex != 0;
    auto port = this->SerialPortInfo->Port;

    concurrency::create_task(port->Open(
        dteRate,
        parity,
        dataBits,
        charFormat
        )
    ).then([port, dtr](Windows::Foundation::HResult hr)
    {
        if (hr.Value != S_OK)
        {
            SDKSample::MainPage::Current->NotifyUser("SerialPort cannot be opened.", SDKSample::NotifyType::ErrorMessage);
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
    ).then([]()
    {
        SDKSample::MainPage::Current->NotifyUser("Initialized.", SDKSample::NotifyType::StatusMessage);
    });
}
