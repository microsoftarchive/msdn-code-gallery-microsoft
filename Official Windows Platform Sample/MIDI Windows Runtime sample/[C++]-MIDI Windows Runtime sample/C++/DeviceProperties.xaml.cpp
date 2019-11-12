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
// DeviceProperties.xaml.cpp
// Implementation of the DeviceProperties class
//

#include "pch.h"
#include "DeviceProperties.xaml.h"
#include "ppl.h"

using namespace Platform::Collections;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml::Navigation;
using namespace concurrency;

using namespace WindowsPreview::Devices::Midi;
using namespace SDKSample::MIDI;

DeviceProperties::DeviceProperties()
 :  _rootPage(nullptr),
    _inPortsListBox(nullptr),
    _outPortsListBox(nullptr),
    _inPropertiesListBox(nullptr),
    _outPropertiesListBox(nullptr),
    _enumeratePortsButton(nullptr),
    _enumWithWatcher(false),
    _midiInDeviceWatcher(nullptr),
    _midiOutDeviceWatcher(nullptr)
{
    InitializeComponent();
    ScenarioInit();

    // start the MIDI In and Out device watcher
    _midiInDeviceWatcher = ref new MidiDeviceWatcher(MidiInPort::GetDeviceSelector(), Dispatcher, _inPortsListBox);
    _midiOutDeviceWatcher = ref new MidiDeviceWatcher(MidiOutPort::GetDeviceSelector(), Dispatcher, _outPortsListBox);

    if (_enumWithWatcher)
    {
        _midiInDeviceWatcher->Start();
        _midiOutDeviceWatcher->Start();

        _enumeratePortsButton->IsEnabled = false;
    }
    else
    {
        _enumeratePortsButton->IsEnabled = true;
    }
}

void DeviceProperties::ScenarioInit()
{
    if (_inPortsListBox == nullptr)
    {
        _inPortsListBox = safe_cast<ListBox^>(static_cast<IFrameworkElement^>(this)->FindName("InPortsList"));
    }
    if (_outPortsListBox == nullptr)
    {
        _outPortsListBox = safe_cast<ListBox^>(static_cast<IFrameworkElement^>(this)->FindName("OutPortsList"));
    }
    if (_inPropertiesListBox == nullptr)
    {
        _inPropertiesListBox = safe_cast<ListBox^>(static_cast<IFrameworkElement^>(this)->FindName("InPropertiesList"));
    }
    if (_outPropertiesListBox == nullptr)
    {
        _outPropertiesListBox = safe_cast<ListBox^>(static_cast<IFrameworkElement^>(this)->FindName("OutPropertiesList"));
    }
    if (_enumeratePortsButton == nullptr)
    {
        _enumeratePortsButton = safe_cast<Button ^>(static_cast<IFrameworkElement ^>(this)->FindName("btnEnumeratePorts"));
    }

    DisplayEmptyInPropertiesBox();
    DisplayEmptyOutPropertiesBox();
    DisplayEmptyInPortsBox();
    DisplayEmptyOutPortsBox();
}

void DeviceProperties::ScenarioClose()
{
    if (_enumWithWatcher)
    {
        _midiInDeviceWatcher->Stop();
        _midiOutDeviceWatcher->Stop();
    }
}

void DeviceProperties::ShowStatusMessage(String^ text)
{
    _rootPage->NotifyUser(text, NotifyType::StatusMessage);
}

void DeviceProperties::ShowExceptionMessage(Exception^ ex)
{
    _rootPage->NotifyUser(ex->Message, NotifyType::ErrorMessage);
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void DeviceProperties::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    _rootPage = MainPage::Current;
}

void DeviceProperties::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()

    ScenarioClose();
}

void DeviceProperties::DisplayEmptyInPropertiesBox()
{
    _inPropertiesListBox->Items->Clear();
    _inPropertiesListBox->Items->Append(L"Select a MIDI in port to view its properties");
    _inPropertiesListBox->IsEnabled = false;
}

void DeviceProperties::DisplayEmptyOutPropertiesBox()
{
    _outPropertiesListBox->Items->Clear();
    _outPropertiesListBox->Items->Append(L"Select a MIDI out port to view its properties");
    _outPropertiesListBox->IsEnabled = false;
}

void DeviceProperties::DisplayEmptyInPortsBox()
{
    _inPortsListBox->Items->Append(L"Click \"Enumerate Ports\" to display MIDI in ports");
    _inPortsListBox->IsEnabled = false;
}

void DeviceProperties::DisplayEmptyOutPortsBox()
{
    _outPortsListBox->Items->Append(L"Click \"Enumerate Ports\" to display MIDI out ports");
    _outPortsListBox->IsEnabled = false;
}

void DeviceProperties::DisplayPropertiesList(DeviceInformation^ devInfo, ListBox^ listBox)
{
    String^ devContainer = devInfo->Properties->Lookup("System.Devices.ContainerID")->ToString();
    String^ devInstance = devInfo->Properties->Lookup("System.Devices.DeviceInstanceId")->ToString();

    listBox->Items->Clear();

    listBox->Items->Append("Id: " + devInfo->Id);
    listBox->Items->Append("Name: " + devInfo->Name);
    listBox->Items->Append("IsDefault: " + devInfo->IsDefault);
    listBox->Items->Append("IsEnabled: " + devInfo->IsEnabled);
    listBox->Items->Append("EnclosureLocation: " + devInfo->EnclosureLocation);

    listBox->Items->Append("--- Device Interface ---");
    // display the device interface information
    std::for_each(begin(devInfo->Properties), end(devInfo->Properties),
        [listBox](IKeyValuePair<String^, Object^>^ prop)
    {
        listBox->Items->Append(prop->Key + ": " + prop->Value->ToString());
    });

    // now, go from device interface to device container

    auto properties = ref new Vector<String^>();
    properties->Append("System.ItemNameDisplay");
    properties->Append("System.Devices.DiscoveryMethod");
    properties->Append("System.Devices.Connected");
    properties->Append("System.Devices.Paired");
    properties->Append("System.Devices.Icon");
    properties->Append("System.Devices.LocalMachine");
    properties->Append("System.Devices.MetadataPath");
    properties->Append("System.Devices.LaunchDeviceStageFromExplorer");
    properties->Append("System.Devices.DeviceDescription1");
    properties->Append("System.Devices.DeviceDescription2");
    properties->Append("System.Devices.NotWorkingProperly");
    properties->Append("System.Devices.IsShared");
    properties->Append("System.Devices.IsNetworkConnected");
    properties->Append("System.Devices.IsDefault");
    properties->Append("System.Devices.Category");
    properties->Append("System.Devices.CategoryPlural");
    properties->Append("System.Devices.CategoryGroup");
    properties->Append("System.Devices.FriendlyName");
    properties->Append("System.Devices.Manufacturer");
    properties->Append("System.Devices.ModelName");
    properties->Append("System.Devices.ModelNumber");
    properties->Append("System.Devices.HardwareIds");

    create_task(Pnp::PnpObject::CreateFromIdAsync(Pnp::PnpObjectType::DeviceContainer, devContainer, properties)).then(
        [listBox](Pnp::PnpObject^ pnpObject)
    {
        listBox->Items->Append("--- Device Container ---");
        IBoxArray<String ^>^ boxedHwIdArray = dynamic_cast<IBoxArray<String^>^>(pnpObject->Properties->Lookup("System.Devices.HardwareIds"));
        if (nullptr != boxedHwIdArray)
        {
            Vector<String ^>^ itemString = ref new Vector<String ^>(boxedHwIdArray->Value);
            for (unsigned int i = 0; i < itemString->Size; i++)
            {
                listBox->Items->Append("System.Devices.HardwareIds[" + i + "]: " + itemString->GetAt(i));
            }
        }

        std::for_each(begin(pnpObject->Properties), end(pnpObject->Properties),
            [listBox](IKeyValuePair<String^, Object^>^ prop)
        {
            if (!prop->Key->Equals("System.Devices.HardwareIds"))
            {
                listBox->Items->Append(prop->Key + ": " + prop->Value->ToString());
            }
        });
    });

    auto devproperties = ref new Vector<String^>();
    devproperties->Append("System.Devices.Children");
    devproperties->Append("System.Devices.CompatibleIds");
    devproperties->Append("System.Devices.ContainerId");
    devproperties->Append("System.Devices.DeviceCapabilities");
    devproperties->Append("System.Devices.DeviceCharacteristics");
    devproperties->Append("System.Devices.DeviceHasProblem");
    devproperties->Append("System.Devices.DeviceInstanceId");
    devproperties->Append("System.Devices.HardwareIds");
    devproperties->Append("System.Devices.InLocalMachineContainer");
    devproperties->Append("System.ItemNameDisplay");

    create_task(Pnp::PnpObject::CreateFromIdAsync(Pnp::PnpObjectType::Device, devInstance, devproperties)).then(
        [listBox](Pnp::PnpObject^ pnpObject)
    {
        listBox->Items->Append("--- Device ---");
        IBoxArray<String ^>^ boxedHwIdArray = dynamic_cast<IBoxArray<String^>^>(pnpObject->Properties->Lookup("System.Devices.HardwareIds"));
        if (nullptr != boxedHwIdArray)
        {
            Vector<String ^>^ itemString = ref new Vector<String ^>(boxedHwIdArray->Value);
            for (unsigned int i = 0; i < itemString->Size; i++)
            {
                listBox->Items->Append("System.Devices.HardwareIds[" + i + "]: " + itemString->GetAt(i));
            }
        }

        IBoxArray<String ^>^ boxedCompatIdArray = dynamic_cast<IBoxArray<String^>^>(pnpObject->Properties->Lookup("System.Devices.CompatibleIds"));
        if (nullptr != boxedCompatIdArray)
        {
            Vector<String ^>^ itemString = ref new Vector<String ^>(boxedCompatIdArray->Value);
            for (unsigned int i = 0; i < itemString->Size; i++)
            {
                listBox->Items->Append("System.Devices.CompatibleIds[" + i + "]: " + itemString->GetAt(i));
            }
        }

        std::for_each(begin(pnpObject->Properties), end(pnpObject->Properties),
            [listBox](IKeyValuePair<String^, Object^>^ prop)
        {
            if (!prop->Key->Equals("System.Devices.CompatibleIds") && !prop->Key->Equals("System.Devices.HardwareIds"))
            {
                listBox->Items->Append(prop->Key + ": " + prop->Value->ToString());
            }
        });
    });

    listBox->IsEnabled = true;
}

void DeviceProperties::EnumeratePorts(Object^ sender, RoutedEventArgs^ e)
{
    _midiInDeviceWatcher->UpdatePorts();
    _midiOutDeviceWatcher->UpdatePorts();
}

void DeviceProperties::InPortsListSelectionChanged(Platform::Object^ sender, SelectionChangedEventArgs^ e)
{
    int listBoxIndex = ((ListBox ^)sender)->SelectedIndex;

    if (listBoxIndex >= 0)
    {
        DeviceInformationCollection^ devInfoCollection = _midiInDeviceWatcher->GetDeviceInformationCollection();
        if (nullptr != devInfoCollection)
        {
            DeviceInformation^ devInfo = devInfoCollection->GetAt(listBoxIndex);
            if (nullptr != devInfo)
            {
                // Update the properties page with the information for this port
                DisplayPropertiesList(devInfo, _inPropertiesListBox);
            }
            else
            {
                _inPropertiesListBox->Items->Clear();
                _inPropertiesListBox->Items->Append(L"Device Information not found for in port");
                _inPropertiesListBox->IsEnabled = false;
            }
        }
        else
        {
            _inPropertiesListBox->Items->Clear();
            _inPropertiesListBox->Items->Append(L"Device Information Collection for MIDI in ports not found");
            _inPropertiesListBox->IsEnabled = false;
        }
    }
    else
    {
        // Otherwise, clear the properties pane
        DisplayEmptyInPropertiesBox();
    }
}

void DeviceProperties::OutPortsListSelectionChanged(Platform::Object^ sender, SelectionChangedEventArgs^ e)
{
    int listBoxIndex = ((ListBox ^)sender)->SelectedIndex;

    if (listBoxIndex >= 0)
    {
        DeviceInformationCollection^ devInfoCollection = _midiOutDeviceWatcher->GetDeviceInformationCollection();
        if (nullptr != devInfoCollection)
        {
            DeviceInformation^ devInfo = devInfoCollection->GetAt(listBoxIndex);
            if (nullptr != devInfo)
            {
                // Update the properties page with the information for this port
                DisplayPropertiesList(devInfo, _outPropertiesListBox);
            }
            else
            {
                _outPropertiesListBox->Items->Clear();
                _outPropertiesListBox->Items->Append(L"Device Information not found for out port");
                _outPropertiesListBox->IsEnabled = false;
            }
        }
        else
        {
            _outPropertiesListBox->Items->Clear();
            _outPropertiesListBox->Items->Append(L"Device Information Collection for MIDI out ports not found");
            _outPropertiesListBox->IsEnabled = false;
        }
    }
    else
    {
        // Otherwise, clear the properties pane
        DisplayEmptyOutPropertiesBox();
    }
}

void DeviceProperties::TogglePortMonitor(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    _enumWithWatcher = !_enumWithWatcher;

    if (_enumWithWatcher)
    {
        if (nullptr != _midiInDeviceWatcher)
        {
            _midiInDeviceWatcher->Start();
        }
        if (nullptr != _midiOutDeviceWatcher)
        {
            _midiOutDeviceWatcher->Start();
        }
        if (nullptr != _enumeratePortsButton)
        {
            _enumeratePortsButton->IsEnabled = false;
        }
    }
    else
    {
        if (nullptr != _midiInDeviceWatcher)
        {
            _midiInDeviceWatcher->Stop();
        }
        if (nullptr != _midiOutDeviceWatcher)
        {
            _midiOutDeviceWatcher->Stop();
        }
        if (nullptr != _enumeratePortsButton)
        {
            _enumeratePortsButton->IsEnabled = true;
        }
    }
}

DeviceProperties::MidiDeviceWatcher::MidiDeviceWatcher(String ^ midiSelector, CoreDispatcher ^ dispatcher,
    ListBox ^ portListBox)
 :  _midiSelectorString(midiSelector),
    _coreDispatcher(dispatcher),
    _portListBox(portListBox),
    _deviceWatcher(nullptr),
    _devInfoCollection(nullptr),
    _enumCompleted(false)
{
    _deviceWatcher = DeviceInformation::CreateWatcher(midiSelector);

    _portAddedToken = _deviceWatcher->Added += ref new TypedEventHandler<DeviceWatcher ^, DeviceInformation ^>(this, &DeviceProperties::MidiDeviceWatcher::OnPortAdded);
    _portRemovedToken = _deviceWatcher->Removed += ref new TypedEventHandler<DeviceWatcher ^, DeviceInformationUpdate ^>(this, &DeviceProperties::MidiDeviceWatcher::OnPortRemoved);
    _portUpdatedToken = _deviceWatcher->Updated += ref new TypedEventHandler<DeviceWatcher ^, DeviceInformationUpdate ^>(this, &DeviceProperties::MidiDeviceWatcher::OnPortUpdated);
    _portEnumCompleteToken = _deviceWatcher->EnumerationCompleted += ref new TypedEventHandler<DeviceWatcher ^, Object ^>(this, &DeviceProperties::MidiDeviceWatcher::OnPortEnumCompleted);
}

DeviceProperties::MidiDeviceWatcher::~MidiDeviceWatcher()
{
    _deviceWatcher->Added -= _portAddedToken;
    _deviceWatcher->Removed -= _portRemovedToken;
    _deviceWatcher->Updated -= _portUpdatedToken;
    _deviceWatcher->EnumerationCompleted -= _portEnumCompleteToken;
}

void DeviceProperties::MidiDeviceWatcher::Start()
{
    _deviceWatcher->Start();
}

void DeviceProperties::MidiDeviceWatcher::Stop()
{
    _deviceWatcher->Stop();
}

DeviceInformationCollection^ DeviceProperties::MidiDeviceWatcher::GetDeviceInformationCollection()
{
    return _devInfoCollection;
}

void DeviceProperties::MidiDeviceWatcher::UpdatePorts()
{
    auto properties = ref new Vector<String^>();
    properties->Append("System.Devices.ContainerId");
    properties->Append("System.Devices.DeviceInstanceId");
    properties->Append("System.Devices.InterfaceClassGuid");
    properties->Append("System.Devices.InterfaceEnabled");
    properties->Append("System.ItemNameDisplay");

    create_task(DeviceInformation::FindAllAsync(_midiSelectorString, properties)).then(
        [this](DeviceInformationCollection^ DeviceInfoCollection)
    {
        _portListBox->Items->Clear();

        if ((DeviceInfoCollection == nullptr) || (DeviceInfoCollection->Size == 0))
        {
            _portListBox->Items->Append("No MIDI ports");
            _portListBox->IsEnabled = false;
        }
        else
        {
            _devInfoCollection = DeviceInfoCollection;

            // Enumerate through the ports and the custom properties
            for (unsigned int i = 0; i < DeviceInfoCollection->Size; i++)
            {
                _portListBox->Items->Append(DeviceInfoCollection->GetAt(i)->Name);
            }

            _portListBox->IsEnabled = true;
        }
    });
}

void DeviceProperties::MidiDeviceWatcher::OnPortAdded(DeviceWatcher^ deviceWatcher, DeviceInformation^ devInfo)
{
    if (_enumCompleted)
    {
        create_task(_coreDispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::High,
            ref new Windows::UI::Core::DispatchedHandler([this]()
        {
            UpdatePorts();
        })));
    }
}

void DeviceProperties::MidiDeviceWatcher::OnPortRemoved(DeviceWatcher^ deviceWatcher, DeviceInformationUpdate^ devInfoUpdate)
{
    if (_enumCompleted)
    {
        create_task(_coreDispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::High,
            ref new Windows::UI::Core::DispatchedHandler([this]()
        {
            UpdatePorts();
        })));
    }
}

void DeviceProperties::MidiDeviceWatcher::OnPortUpdated(DeviceWatcher^ deviceWatcher, DeviceInformationUpdate^ devInfoUpdate)
{
    if (_enumCompleted)
    {
        create_task(_coreDispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::High,
            ref new Windows::UI::Core::DispatchedHandler([this]()
        {
            UpdatePorts();
        })));
    }
}

void DeviceProperties::MidiDeviceWatcher::OnPortEnumCompleted(DeviceWatcher^ deviceWatcher, Object^ obj)
{
    _enumCompleted = true;

    create_task(_coreDispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::High,
            ref new Windows::UI::Core::DispatchedHandler([this]()
    {
        UpdatePorts();
    })));
}