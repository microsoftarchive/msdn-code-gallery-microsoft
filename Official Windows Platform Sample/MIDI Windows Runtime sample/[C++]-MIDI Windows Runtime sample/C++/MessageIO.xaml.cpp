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
// MessageIO.xaml.cpp
// Implementation of the MessageIO class
//

#include "pch.h"
#include "MessageIO.xaml.h"
#include "ppl.h"

using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Platform;
using namespace Windows::UI;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Storage::Streams;
using namespace WindowsPreview::Devices::Midi;

using namespace SDKSample::MIDI;
using namespace concurrency;


MessageIO::MessageIO()
 :  _rootPage(nullptr),
    _messageTypeComboBox(nullptr),
    _field1ComboBox(nullptr),
    _field2ComboBox(nullptr),
    _field3ComboBox(nullptr),
    _messageTypeTextBlock(nullptr),
    _field1TextBlock(nullptr),
    _field2TextBlock(nullptr),
    _field3TextBlock(nullptr),
    _rawBufferTextBlock(nullptr),
    _inPortsListBox(nullptr),
    _outPortsListBox(nullptr),
    _messageOutputListBox(nullptr),
    _sendButton(nullptr),
    _resetButton(nullptr),
    _sendMessageTextBox(nullptr),
    _messageTypeMap(nullptr),
    _messageType(MidiMessageType::None),
    _field1IntValue(-1),
    _field2IntValue(-1),
    _field3IntValue(-1),
    _midiInDeviceWatcher(nullptr),
    _midiOutDeviceWatcher(nullptr)
{
    _messageTypeMap = ref new Map<MidiMessageType, String ^>();
    _messageTypeMap->Insert(MidiMessageType::ActiveSensing, L"Active Sensing");
    _messageTypeMap->Insert(MidiMessageType::ChannelPressure, L"Channel Pressure");
    _messageTypeMap->Insert(MidiMessageType::Continue, L"Continue");
    _messageTypeMap->Insert(MidiMessageType::ControlChange, L"Control Change");
    _messageTypeMap->Insert(MidiMessageType::MidiTimeCode, L"MIDI Time Code");
    //_messageTypeMap->Insert(MidiMessageType::None, L"None");
    _messageTypeMap->Insert(MidiMessageType::NoteOff, L"Note Off");
    _messageTypeMap->Insert(MidiMessageType::NoteOn, L"Note On");
    _messageTypeMap->Insert(MidiMessageType::PitchBendChange, L"Pitch Bend Change");
    _messageTypeMap->Insert(MidiMessageType::PolyphonicKeyPressure, L"Polyphonic Key Pressure");
    _messageTypeMap->Insert(MidiMessageType::ProgramChange, L"Program Change");
    _messageTypeMap->Insert(MidiMessageType::SongPositionPointer, L"Song Position Pointer");
    _messageTypeMap->Insert(MidiMessageType::SongSelect, L"Song Select");
    _messageTypeMap->Insert(MidiMessageType::Start, L"Start");
    _messageTypeMap->Insert(MidiMessageType::Stop, L"Stop");
    _messageTypeMap->Insert(MidiMessageType::SystemExclusive, L"System Exclusive");
    _messageTypeMap->Insert(MidiMessageType::SystemReset, L"System Reset");
    _messageTypeMap->Insert(MidiMessageType::TimingClock, L"Timing Clock");
    _messageTypeMap->Insert(MidiMessageType::TuneRequest, L"Tune Request");

    InitializeComponent();
    ScenarioInit();

    // start the MIDI In and Out device watcher
    _midiInDeviceWatcher = ref new MidiDeviceWatcher(MidiInPort::GetDeviceSelector(), Dispatcher, _inPortsListBox);
    _midiOutDeviceWatcher = ref new MidiDeviceWatcher(MidiOutPort::GetDeviceSelector(), Dispatcher, _outPortsListBox);

    _midiInDeviceWatcher->Start();
    _midiOutDeviceWatcher->Start();
}

MessageIO::~MessageIO()
{
    _midiInDeviceWatcher->Stop();
    _midiOutDeviceWatcher->Stop();
}

void  MessageIO::ScenarioInit()
{
    if (nullptr == _messageTypeComboBox)
    {
        _messageTypeComboBox = safe_cast<ComboBox^>(static_cast<IFrameworkElement^>(this)->FindName("cmbMessageType"));
    }
    if (nullptr == _field1ComboBox)
    {
        _field1ComboBox = safe_cast<ComboBox^>(static_cast<FrameworkElement^>(this)->FindName("cmbField1"));
    }
    if (nullptr == _field2ComboBox)
    {
        _field2ComboBox = safe_cast<ComboBox^>(static_cast<FrameworkElement^>(this)->FindName("cmbField2"));
    }
    if (nullptr == _field3ComboBox)
    {
        _field3ComboBox = safe_cast<ComboBox^>(static_cast<FrameworkElement^>(this)->FindName("cmbField3"));
    }
    if (nullptr == _messageTypeTextBlock)
    {
        _messageTypeTextBlock = safe_cast<TextBlock^>(static_cast<IFrameworkElement^>(this)->FindName("lblMessageType"));
    }
    if (nullptr == _field1TextBlock)
    {
        _field1TextBlock = safe_cast<TextBlock^>(static_cast<FrameworkElement^>(this)->FindName("lblField1"));
    }
    if (nullptr == _field2TextBlock)
    {
        _field2TextBlock = safe_cast<TextBlock^>(static_cast<FrameworkElement^>(this)->FindName("lblField2"));
    }
    if (nullptr == _field3TextBlock)
    {
        _field3TextBlock = safe_cast<TextBlock^>(static_cast<FrameworkElement^>(this)->FindName("lblField3"));
    }
    if (nullptr == _rawBufferTextBlock)
    {
        _rawBufferTextBlock = safe_cast<TextBlock^>(static_cast<FrameworkElement^>(this)->FindName("lblRawBuffer"));
    }
    if (nullptr == _inPortsListBox)
    {
        _inPortsListBox = safe_cast<ListBox^>(static_cast<IFrameworkElement^>(this)->FindName("InPortsList"));
    }
    if (nullptr == _outPortsListBox)
    {
        _outPortsListBox = safe_cast<ListBox^>(static_cast<IFrameworkElement^>(this)->FindName("OutPortsList"));
    }
    if (nullptr == _messageOutputListBox)
    {
        _messageOutputListBox = safe_cast<ListBox^>(static_cast<FrameworkElement^>(this)->FindName("listMessageOutput"));
    }
    if (nullptr == _sendButton)
    {
        _sendButton = safe_cast<Button^>(static_cast<FrameworkElement^>(this)->FindName("btnSend"));
    }
    if (nullptr == _resetButton)
    {
        _resetButton = safe_cast<Button^>(static_cast<FrameworkElement^>(this)->FindName("btnReset"));
    }
    if (nullptr == _sendMessageTextBox)
    {
        _sendMessageTextBox = safe_cast<TextBox^>(static_cast<FrameworkElement^>(this)->FindName("SendMessageTextBox"));
    }

    std::for_each(begin(_messageTypeMap), end(_messageTypeMap),
        [this](Windows::Foundation::Collections::IKeyValuePair<MidiMessageType, String ^>^ messageType)
    {
        _messageTypeComboBox->Items->Append(messageType->Value);
    });
}

void  MessageIO::ScenarioClose()
{
}

void MessageIO::ShowStatusMessage(String^ text)
{
    _rootPage->NotifyUser(text, NotifyType::StatusMessage);
}

void MessageIO::ShowExceptionMessage(Exception^ ex)
{
    _rootPage->NotifyUser(ex->Message, NotifyType::ErrorMessage);
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void MessageIO::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    _rootPage = MainPage::Current;
}

void MessageIO::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()

    ScenarioClose();
}

void MessageIO::EnableMessageBuilding()
{
    UpdateMessageTypeUI();
}

void MessageIO::ResetMessageBuilding(bool resetMessageType)
{
    if (resetMessageType)
    {
        _messageType = MidiMessageType::None;
        _messageTypeComboBox->SelectedIndex = -1;
        UpdateMessageTypeUI();
    }

    _field1IntValue = -1;
    _field2IntValue = -1;
    _field3IntValue = -1;

    UpdateField1UI();
    UpdateField2UI();
    UpdateField3UI();

    _sendButton->IsEnabled = false;
    _rawBufferTextBlock->Visibility = Xaml::Visibility::Collapsed;
    _sendMessageTextBox->Text = L"";
}

MidiMessageType MessageIO::GetMessageTypeFromIndex(int index)
{
    int count = 0;
    MidiMessageType retValue = MidiMessageType::None;

    std::for_each(begin(_messageTypeMap), end(_messageTypeMap),
        [index, &retValue, &count](Windows::Foundation::Collections::IKeyValuePair<MidiMessageType, String ^>^ messageType)
    {
        if (count == index)
        {
            retValue = messageType->Key;
        }

        count++;
    });

    return retValue;
}

void MessageIO::InPortsListSelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
    // see if we should open ports
    IVector<Object ^>^ addedPorts = e->AddedItems;

    std::for_each(begin(e->AddedItems), end(e->AddedItems),
        [this](Object^ portName)
    {
        DeviceInformation^ portToOpen = GetDeviceInformationForInPort(portName->ToString());
        if (nullptr != portToOpen)
        {
            create_task(MidiInPort::FromIdAsync(portToOpen->Id)).then(
                [this](MidiInPort^ inPort)
            {
                if (nullptr != inPort)
                {
                    inPort->MessageReceived += ref new Windows::Foundation::TypedEventHandler<MidiInPort ^,
                        MidiMessageReceivedEventArgs ^>(this, &MessageIO::OnMessageReceived);

                    _midiInPortArray.Append(inPort);
                }
            });
        }
    });

    std::for_each(begin(e->RemovedItems), end(e->RemovedItems),
        [this](Object^ portName)
    {
        DeviceInformation^ portToClose = GetDeviceInformationForInPort(portName->ToString());
        if (nullptr != portToClose)
        {
            CloseInPort(portToClose->Id);
        }
    });
}

void MessageIO::OutPortsListSelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
    // see if we should open ports
    bool updateMessageBuildingUI = false;

    // only update the message type combo box for certain conditions
    //  1. no ports are open and we are going to open some now
    //  2. we are closing all the open ports (AND not opening more ports)
    if ((_midiOutPortArray.Size == 0 && e->AddedItems->Size > 0) ||
        (e->RemovedItems->Size == _midiOutPortArray.Size && e->AddedItems->Size == 0))
    {
        updateMessageBuildingUI = true;
    }

    std::for_each(begin(e->AddedItems), end(e->AddedItems),
        [this, updateMessageBuildingUI](Object^ portName)
    {
        DeviceInformation^ portToOpen = GetDeviceInformationForOutPort(portName->ToString());
        if (nullptr != portToOpen)
        {
            create_task(MidiOutPort::FromIdAsync(portToOpen->Id)).then(
                [this, updateMessageBuildingUI](MidiOutPort^ outPort)
            {
                if (nullptr != outPort)
                {
                    _midiOutPortArray.Append(outPort);
                }

                if (updateMessageBuildingUI)
                {
                    EnableMessageBuilding();
                }
            });
        }
    });

    std::for_each(begin(e->RemovedItems), end(e->RemovedItems),
        [this, updateMessageBuildingUI](Object^ portName)
    {
        DeviceInformation^ portToClose = GetDeviceInformationForOutPort(portName->ToString());
        if (nullptr != portToClose)
        {
            CloseOutPort(portToClose->Id);
        }
    });

    if (updateMessageBuildingUI  && e->RemovedItems->Size > 0 )
    {
        ResetMessageBuilding(true);
    }
}

void MessageIO::MessageTypeSelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
    int typeIndex = _messageTypeComboBox->SelectedIndex;

    if (-1 == typeIndex)
    {
        // field is being reset
        return;
    }

    ResetMessageBuilding(false);

    _messageType = GetMessageTypeFromIndex(typeIndex);

    switch (_messageType)
    {
        case MidiMessageType::SystemExclusive:

            _rawBufferTextBlock->Visibility = Xaml::Visibility::Visible;

            // provide the base "Start of SysEx" (0xF0) and
            // "End of SysEx" (0xF7) values.
            _sendMessageTextBox->Text = L"F0 F7";

        case MidiMessageType::TuneRequest:
        case MidiMessageType::TimingClock:
        case MidiMessageType::Start:
        case MidiMessageType::Continue:
        case MidiMessageType::Stop:
        case MidiMessageType::ActiveSensing:
        case MidiMessageType::SystemReset:

            _sendButton->IsEnabled = true;
            break;

        default:

            _sendButton->IsEnabled = false;
            break;

    }

    UpdateField1UI();
}

void MessageIO::Field1ComboBoxSelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
    _field1IntValue = _field1ComboBox->SelectedIndex;

    switch (_messageType)
    {
        case MidiMessageType::SongPositionPointer:
        case MidiMessageType::SongSelect:

            if (-1 != _field1IntValue)
            {
                _sendButton->IsEnabled = true;
            }
            break;

        default:

            _sendButton->IsEnabled = false;
            break;
    }

    UpdateField2UI();
}

void MessageIO::Field2ComboBoxSelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
    _field2IntValue = _field2ComboBox->SelectedIndex;

    switch (_messageType)
    {
        case MidiMessageType::ProgramChange:
        case MidiMessageType::ChannelPressure:
        case MidiMessageType::PitchBendChange:
        case MidiMessageType::MidiTimeCode:

            if (-1 != _field2IntValue)
            {
                _sendButton->IsEnabled = true;
            }
            break;

        default:

            _sendButton->IsEnabled = false;
            break;
    }

    UpdateField3UI();
}

void MessageIO::Field3ComboBoxSelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
    _field3IntValue = _field3ComboBox->SelectedIndex;

    switch (_messageType)
    {
        case MidiMessageType::NoteOff:
        case MidiMessageType::NoteOn:
        case MidiMessageType::PolyphonicKeyPressure:
        case MidiMessageType::ControlChange:

            if (-1 != _field3IntValue)
            {
                _sendButton->IsEnabled = true;
            }
            break;

        default:

            _sendButton->IsEnabled = false;
            break;
    }
}

void MessageIO::RawMessageTextBlockTapped(Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e)
{
    FrameworkElement^ element = static_cast<FrameworkElement^>(sender);

    if (element != nullptr && MidiMessageType::SystemExclusive == _messageType)
    {
        Windows::UI::Xaml::Controls::Primitives::FlyoutBase::ShowAttachedFlyout(element);
    }
}

void MessageIO::ResetButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ResetMessageBuilding(true);
}

void MessageIO::SendButtonClick(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    IMidiMessage^ midiMessage;

    switch (_messageType)
    {
        case MidiMessageType::NoteOff:

            midiMessage = ref new MidiNoteOffMessage(_field1IntValue, _field2IntValue, _field3IntValue);
            break;

        case MidiMessageType::NoteOn:

            midiMessage = ref new MidiNoteOnMessage(_field1IntValue, _field2IntValue, _field3IntValue);
            break;

        case MidiMessageType::PolyphonicKeyPressure:

            midiMessage = ref new MidiPolyphonicKeyPressureMessage(_field1IntValue, _field2IntValue, _field3IntValue);
            break;

        case MidiMessageType::ControlChange:

            midiMessage = ref new MidiControlChangeMessage(_field1IntValue, _field2IntValue, _field3IntValue);
            break;

        case MidiMessageType::ProgramChange:

            midiMessage = ref new MidiProgramChangeMessage(_field1IntValue, _field2IntValue);
            break;

        case MidiMessageType::ChannelPressure:

            midiMessage = ref new MidiChannelPressureMessage(_field1IntValue, _field2IntValue);
            break;

        case MidiMessageType::PitchBendChange:

            midiMessage = ref new MidiPitchBendChangeMessage(_field1IntValue, _field2IntValue);
            break;

        case MidiMessageType::SystemExclusive:
        {
            DataWriter^ dataWriter = ref new DataWriter();

            // expecting a string of format "NN NN NN NN...." where NN is a byte in hex
            int len = _sendMessageTextBox->Text->Length();
            if (0 == len)
            {
                return;
            }

            const wchar_t* startPointer = _sendMessageTextBox->Text->Data();
            wchar_t* endPointer = nullptr;

            do
            {
                byte midiByte = (byte)wcstoul(startPointer, &endPointer, 16);
                if (endPointer == startPointer)
                {
                    // conversion failed, bail out
                    break;
                }

                dataWriter->WriteByte(midiByte);

                // prep for next iteration
                startPointer = endPointer;
            } while (nullptr != endPointer);

            midiMessage = ref new MidiSystemExclusiveMessage(dataWriter->DetachBuffer());
            break;
        }
        case MidiMessageType::MidiTimeCode:

            midiMessage = ref new MidiTimeCodeMessage(_field1IntValue, _field2IntValue);
            break;

        case MidiMessageType::SongPositionPointer:

            midiMessage = ref new MidiSongPositionPointerMessage(_field1IntValue);
            break;

        case MidiMessageType::SongSelect:

            midiMessage = ref new MidiSongSelectMessage(_field1IntValue);
            break;

        case MidiMessageType::TuneRequest:

            midiMessage = ref new MidiTuneRequestMessage();
            break;

        case MidiMessageType::TimingClock:

            midiMessage = ref new MidiTimingClockMessage();
            break;

        case MidiMessageType::Start:

            midiMessage = ref new MidiStartMessage();
            break;

        case MidiMessageType::Continue:

            midiMessage = ref new MidiContinueMessage();
            break;

        case MidiMessageType::Stop:

            midiMessage = ref new MidiStopMessage();
            break;

        case MidiMessageType::ActiveSensing:

            midiMessage = ref new MidiActiveSensingMessage();
            break;

        case MidiMessageType::SystemReset:

            midiMessage = ref new MidiSystemResetMessage();
            break;

        case MidiMessageType::None:
        default:
            return;
    }

    std::for_each(begin(_midiOutPortArray.GetView()), end(_midiOutPortArray.GetView()),
        [midiMessage](MidiOutPort^ midiOutPort)
    {
        midiOutPort->SendMessage(midiMessage);
    });
}

void MessageIO::UpdateMessageTypeUI()
{
    if (_midiOutPortArray.Size > 0)
    {
        _messageTypeComboBox->IsEnabled = true;
        _messageTypeTextBlock->Text = L"Message Type";

        _resetButton->IsEnabled = true;
    }
    else
    {
        _messageTypeComboBox->IsEnabled = false;
        _messageTypeTextBlock->Text = L"";

        _resetButton->IsEnabled = false;
    }
}

void MessageIO::UpdateField1UI()
{
    switch (_messageType)
    {
        case MidiMessageType::NoteOff:
        case MidiMessageType::NoteOn:
        case MidiMessageType::PolyphonicKeyPressure:
        case MidiMessageType::ControlChange:
        case MidiMessageType::ProgramChange:
        case MidiMessageType::ChannelPressure:
        case MidiMessageType::PitchBendChange:

            _field1ComboBox->Items->Clear();

            for (int i = 0; i < 16; i++)
            {
                _field1ComboBox->Items->Append(i);
            }

            _field1ComboBox->Visibility = Xaml::Visibility::Visible;
            _field1ComboBox->IsEnabled = true;

            _field1TextBlock->Text = L"Channel";
            break;

        case MidiMessageType::MidiTimeCode:

            _field1ComboBox->Items->Clear();

            for (int i = 0; i < 8; i++)
            {
                _field1ComboBox->Items->Append(i);
            }

            _field1ComboBox->Visibility = Xaml::Visibility::Visible;
            _field1ComboBox->IsEnabled = true;

            _field1TextBlock->Text = L"Message Type";
            break;

        case MidiMessageType::SongPositionPointer:

            _field1ComboBox->Items->Clear();

            for (int i = 0; i < 16384; i++)
            {
                _field1ComboBox->Items->Append(i);
            }

            _field1ComboBox->Visibility = Xaml::Visibility::Visible;
            _field1ComboBox->IsEnabled = true;

            _field1TextBlock->Text = L"Beats";
            break;

        case MidiMessageType::SongSelect:

            _field1ComboBox->Items->Clear();

            for (int i = 0; i < 128; i++)
            {
                _field1ComboBox->Items->Append(i);
            }

            _field1ComboBox->Visibility = Xaml::Visibility::Visible;
            _field1ComboBox->IsEnabled = true;

            _field1TextBlock->Text = L"Song";
            break;

        case MidiMessageType::SystemExclusive:

            _field1ComboBox->Items->Clear();

            _field1ComboBox->Visibility = Xaml::Visibility::Collapsed;
            _field1ComboBox->IsEnabled = false;

            _field1TextBlock->Text = L"Please edit the message in the 'Raw SysEx Message Buffer' flyout by clicking on 'F0 F7'.";
            break;

        default:

            _field1ComboBox->Items->Clear();
            _field1ComboBox->IsEnabled = false;
            _field1ComboBox->Visibility = Xaml::Visibility::Collapsed;

            _field1TextBlock->Text = L"";
            break;
    }
}

void MessageIO::UpdateField2UI()
{
    // the field is being "reset"
    if (-1 == _field1IntValue)
    {
        _field2ComboBox->Items->Clear();
        _field2ComboBox->IsEnabled = false;
        _field2ComboBox->Visibility = Xaml::Visibility::Collapsed;

        _field2TextBlock->Text = L"";

        return;
    }

    switch (_messageType)
    {
        case MidiMessageType::NoteOff:
        case MidiMessageType::NoteOn:
        case MidiMessageType::PolyphonicKeyPressure:

            _field2ComboBox->Items->Clear();

            for (int i = 0; i < 128; i++)
            {
                _field2ComboBox->Items->Append(i);
            }

            _field2ComboBox->Visibility = Xaml::Visibility::Visible;
            _field2ComboBox->IsEnabled = true;

            _field2TextBlock->Text = L"Note";
            break;

        case MidiMessageType::ControlChange:

            _field2ComboBox->Items->Clear();

            for (int i = 0; i < 128; i++)
            {
                _field2ComboBox->Items->Append(i);
            }

            _field2ComboBox->Visibility = Xaml::Visibility::Visible;
            _field2ComboBox->IsEnabled = true;

            _field2TextBlock->Text = L"Controller";
            break;

        case MidiMessageType::ProgramChange:

            _field2ComboBox->Items->Clear();

            for (int i = 0; i < 128; i++)
            {
                _field2ComboBox->Items->Append(i);
            }

            _field2ComboBox->Visibility = Xaml::Visibility::Visible;
            _field2ComboBox->IsEnabled = true;

            _field2TextBlock->Text = L"Program Number";
            break;

        case MidiMessageType::ChannelPressure:

            _field2ComboBox->Items->Clear();

            for (int i = 0; i < 128; i++)
            {
                _field2ComboBox->Items->Append(i);
            }

            _field2ComboBox->Visibility = Xaml::Visibility::Visible;
            _field2ComboBox->IsEnabled = true;

            _field2TextBlock->Text = L"Pressure Value";
            break;

        case MidiMessageType::PitchBendChange:

            _field2ComboBox->Items->Clear();

            for (int i = 0; i < 16384; i++)
            {
                _field2ComboBox->Items->Append(i);
            }

            _field2ComboBox->Visibility = Xaml::Visibility::Visible;
            _field2ComboBox->IsEnabled = true;

            _field2TextBlock->Text = L"Pitch Bend Value";
            break;

        case MidiMessageType::MidiTimeCode:

            _field2ComboBox->Items->Clear();

            for (int i = 0; i < 16; i++)
            {
                _field2ComboBox->Items->Append(i);
            }

            _field2ComboBox->Visibility = Xaml::Visibility::Visible;
            _field2ComboBox->IsEnabled = true;

            _field2TextBlock->Text = L"Value";
            break;

        default:

            _field2ComboBox->Items->Clear();
            _field2ComboBox->IsEnabled = false;
            _field2ComboBox->Visibility = Xaml::Visibility::Collapsed;

            _field2TextBlock->Text = L"";
            break;
    }
}

void MessageIO::UpdateField3UI()
{
    // the field is being "reset"
    if (-1 == _field2IntValue)
    {
        _field3ComboBox->Items->Clear();
        _field3ComboBox->IsEnabled = false;
        _field3ComboBox->Visibility = Xaml::Visibility::Collapsed;

        _field3TextBlock->Text = L"";

        return;
    }

    switch (_messageType)
    {
        case MidiMessageType::NoteOff:
        case MidiMessageType::NoteOn:

            _field3ComboBox->Items->Clear();

            for (int i = 0; i < 128; i++)
            {
                _field3ComboBox->Items->Append(i);
            }

            _field3ComboBox->Visibility = Xaml::Visibility::Visible;
            _field3ComboBox->IsEnabled = true;

            _field3TextBlock->Text = L"Velocity";

            break;

        case MidiMessageType::PolyphonicKeyPressure:

            _field3ComboBox->Items->Clear();

            for (int i = 0; i < 128; i++)
            {
                _field3ComboBox->Items->Append(i);
            }

            _field3ComboBox->Visibility = Xaml::Visibility::Visible;
            _field3ComboBox->IsEnabled = true;

            _field3TextBlock->Text = L"Pressure";

            break;

        case MidiMessageType::ControlChange:

            _field3ComboBox->Items->Clear();

            for (int i = 0; i < 128; i++)
            {
                _field3ComboBox->Items->Append(i);
            }

            _field3ComboBox->Visibility = Xaml::Visibility::Visible;
            _field3ComboBox->IsEnabled = true;

            _field3TextBlock->Text = L"Value";

            break;

        default:

            _field3ComboBox->Items->Clear();
            _field3ComboBox->IsEnabled = false;
            _field3ComboBox->Visibility = Xaml::Visibility::Collapsed;

            _field3TextBlock->Text = L"";

            break;
    }
}

void MessageIO::OnMessageReceived(MidiInPort ^sender, MidiMessageReceivedEventArgs ^args)
{
    IMidiMessage^ midiMessage = args->Message;

    create_task(Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::High,
        ref new Windows::UI::Core::DispatchedHandler([this,sender, midiMessage]()
    {
        String^ outputString = midiMessage->Timestamp.Duration +", " + _messageTypeMap->Lookup(midiMessage->Type);

        switch (midiMessage->Type)
        {
            case MidiMessageType::NoteOff:
            {
                MidiNoteOffMessage^ noteOff = static_cast<MidiNoteOffMessage ^>(midiMessage);

                outputString += ", Channel:" + noteOff->Channel + ", Note:" + noteOff->Note + ", Velocity:" + noteOff->Velocity;
                break;
            }
            case MidiMessageType::NoteOn:
            {
                MidiNoteOnMessage^ noteOn = static_cast<MidiNoteOnMessage ^>(midiMessage);

                outputString += ", Channel:" + noteOn->Channel + ", Note:" + noteOn->Note + ", Velocity:" + noteOn->Velocity;
                break;
            }
            case MidiMessageType::PolyphonicKeyPressure:
            {
                MidiPolyphonicKeyPressureMessage^ polyKeyMessage = static_cast<MidiPolyphonicKeyPressureMessage ^>(midiMessage);

                outputString += ", Channel:" + polyKeyMessage->Channel + ", Note:" + polyKeyMessage->Note + ", Pressure:" + polyKeyMessage->Pressure;
                break;

            }
            case MidiMessageType::ControlChange:
            {
                MidiControlChangeMessage^ controlMessage = static_cast<MidiControlChangeMessage ^>(midiMessage);

                outputString += ", Channel:" + controlMessage->Channel + ", Controller:" + controlMessage->Controller + ", Value:" + controlMessage->ControlValue;
                break;

            }
            case MidiMessageType::ProgramChange:
            {

                MidiProgramChangeMessage^ programMessage = static_cast<MidiProgramChangeMessage ^>(midiMessage);

                outputString += ", Channel:" + programMessage->Channel + ", Program:" + programMessage->Program;
                break;

            }
            case MidiMessageType::ChannelPressure:
            {
                MidiChannelPressureMessage^ channelPressureMessage = static_cast<MidiChannelPressureMessage ^>(midiMessage);

                outputString += ", Channel:" + channelPressureMessage->Channel + ", Pressure:" + channelPressureMessage->Pressure;
                break;

            }
            case MidiMessageType::PitchBendChange:
            {
                MidiPitchBendChangeMessage^ pitchBendMessage = static_cast<MidiPitchBendChangeMessage ^>(midiMessage);

                outputString += ", Channel:" + pitchBendMessage->Channel + ", Bend:" + pitchBendMessage->Bend;
                break;

            }
            case MidiMessageType::SystemExclusive:
            {
                MidiSystemExclusiveMessage^ sysExMessage = static_cast<MidiSystemExclusiveMessage ^>(midiMessage);
                DataReader^ sysExReader = DataReader::FromBuffer(sysExMessage->RawData);

                outputString += ", ";

                while (sysExReader->UnconsumedBufferLength > 0)
                {
                    // prepare SysEx message for printing to the screen.
                    // convert bytes to hex strings.
                    Array<wchar_t>^ byteInHex = ref new Array<wchar_t>(3);
                    byte byteRead = sysExReader->ReadByte();

                    swprintf_s(byteInHex->Data, sizeof(byteInHex->Length), L"%02X", byteRead);
                    String^ hexString = ref new String(byteInHex->Data);

                    outputString += hexString + " ";
                }
                break;

            }
            case MidiMessageType::MidiTimeCode:
            {
                MidiTimeCodeMessage^ mtcMessage = static_cast<MidiTimeCodeMessage ^>(midiMessage);

                outputString += ", Type:" + mtcMessage->FrameType + ", Values:" + mtcMessage->Values;
                break;

            }
            case MidiMessageType::SongPositionPointer:
            {
                MidiSongPositionPointerMessage^ songPositionMessage = static_cast<MidiSongPositionPointerMessage ^>(midiMessage);

                outputString += ", Beats:" + songPositionMessage->Beats;
                break;

            }
            case MidiMessageType::SongSelect:
            {
                MidiSongSelectMessage^ songSelectMessage = static_cast<MidiSongSelectMessage ^>(midiMessage);

                outputString += ", Song:" + songSelectMessage->Song;
                break;

            }
            case MidiMessageType::TuneRequest:
            {
                MidiTuneRequestMessage^ tuneRequestMessage = static_cast<MidiTuneRequestMessage ^>(midiMessage);

                break;

            }
            case MidiMessageType::TimingClock:
            {
                MidiTimingClockMessage^ timingClockMessage = static_cast<MidiTimingClockMessage ^>(midiMessage);

                break;

            }
            case MidiMessageType::Start:
            {
                MidiStartMessage^ startMessage = static_cast<MidiStartMessage ^>(midiMessage);

                break;

            }
            case MidiMessageType::Continue:
            {
                MidiContinueMessage^ continueMessage = static_cast<MidiContinueMessage ^>(midiMessage);

                break;

            }
            case MidiMessageType::Stop:
            {
                MidiStopMessage^ stopMessage = static_cast<MidiStopMessage ^>(midiMessage);

                break;

            }
            case MidiMessageType::ActiveSensing:
            {
                MidiActiveSensingMessage^ activeSensingMessage = static_cast<MidiActiveSensingMessage ^>(midiMessage);

                break;

            }
            case MidiMessageType::SystemReset:
            {
                MidiSystemResetMessage^ systemResetMessage = static_cast<MidiSystemResetMessage ^>(midiMessage);

                break;

            }
            case MidiMessageType::None:
            default:
                break;
        }

        _messageOutputListBox->Items->Append(outputString);
    })));
}

DeviceInformation^ MessageIO::GetDeviceInformationForInPort(String^ friendlyName)
{
    DeviceInformation^ retValue = nullptr;
    DeviceInformationCollection^ inputCollection = _midiInDeviceWatcher->GetDeviceInformationCollection();

    std::for_each(begin(inputCollection), end(inputCollection),
        [&friendlyName, &retValue](DeviceInformation^ devInfo)
    {
        if (0 == String::CompareOrdinal(devInfo->Name, friendlyName))
        {
             retValue = devInfo;
        }
    });

    return retValue;
}

DeviceInformation^ MessageIO::GetDeviceInformationForOutPort(String^ friendlyName)
{
    DeviceInformation^ retValue = nullptr;
    DeviceInformationCollection^ inputCollection = _midiOutDeviceWatcher->GetDeviceInformationCollection();

    std::for_each(begin(inputCollection), end(inputCollection),
        [&friendlyName, &retValue](DeviceInformation^ devInfo)
    {
        if (0 == String::CompareOrdinal(devInfo->Name, friendlyName))
        {
            retValue = devInfo;
        }
    });

    return retValue;
}

void MessageIO::CloseInPort(String^ portId)
{
    for (unsigned int index = 0; index < _midiInPortArray.Size; index++)
    {
        if (0 == String::CompareOrdinal(_midiInPortArray.GetAt(index)->DeviceId, portId))
        {
            _midiInPortArray.RemoveAt(index);

            return;
        }
    }
}

void MessageIO::CloseOutPort(String^ portId)
{
    for (unsigned int index = 0; index < _midiOutPortArray.Size; index++)
    {
        if (0 == String::CompareOrdinal(_midiOutPortArray.GetAt(index)->DeviceId, portId))
        {
            _midiOutPortArray.RemoveAt(index);

            return;
        }
    }
}

MessageIO::MidiDeviceWatcher::MidiDeviceWatcher(String ^ midiSelector, CoreDispatcher ^ dispatcher, ListBox ^ portListBox)
 :  _midiSelectorString(midiSelector),
    _coreDispatcher(dispatcher),
    _portListBox(portListBox),
    _deviceWatcher(nullptr),
    _devInfoCollection(nullptr),
    _enumCompleted(false)
{
    _deviceWatcher = DeviceInformation::CreateWatcher(midiSelector);

    _portAddedToken = _deviceWatcher->Added += ref new TypedEventHandler<DeviceWatcher ^, DeviceInformation ^>(this, &MessageIO::MidiDeviceWatcher::OnPortAdded);
    _portRemovedToken = _deviceWatcher->Removed += ref new TypedEventHandler<DeviceWatcher ^, DeviceInformationUpdate ^>(this, &MessageIO::MidiDeviceWatcher::OnPortRemoved);
    _portUpdatedToken = _deviceWatcher->Updated += ref new TypedEventHandler<DeviceWatcher ^, DeviceInformationUpdate ^>(this, &MessageIO::MidiDeviceWatcher::OnPortUpdated);
    _portEnumCompleteToken = _deviceWatcher->EnumerationCompleted += ref new TypedEventHandler<DeviceWatcher ^, Object ^>(this, &MessageIO::MidiDeviceWatcher::OnPortEnumCompleted);
}

MessageIO::MidiDeviceWatcher::~MidiDeviceWatcher()
{
    _deviceWatcher->Added -= _portAddedToken;
    _deviceWatcher->Removed -= _portRemovedToken;
    _deviceWatcher->Updated -= _portUpdatedToken;
    _deviceWatcher->EnumerationCompleted -= _portEnumCompleteToken;
}

void MessageIO::MidiDeviceWatcher::Start()
{
    _deviceWatcher->Start();
}

void MessageIO::MidiDeviceWatcher::Stop()
{
    _deviceWatcher->Stop();
}

DeviceInformationCollection^ MessageIO::MidiDeviceWatcher::GetDeviceInformationCollection()
{
    return _devInfoCollection;
}

void MessageIO::MidiDeviceWatcher::UpdatePorts()
{
    create_task(DeviceInformation::FindAllAsync(_midiSelectorString)).then(
        [this](DeviceInformationCollection^ deviceInfoCollection)
    {
        _portListBox->Items->Clear();

        if ((deviceInfoCollection == nullptr) || (deviceInfoCollection->Size == 0))
        {
            _portListBox->Items->Append("No MIDI ports");
            _portListBox->IsEnabled = false;
        }
        else
        {
            _devInfoCollection = deviceInfoCollection;

            // Enumerate through the ports and the custom properties
            for (unsigned int i = 0; i < deviceInfoCollection->Size; i++)
            {
                _portListBox->Items->Append(deviceInfoCollection->GetAt(i)->Name);
            }

            _portListBox->IsEnabled = true;
        }
    });
}

void MessageIO::MidiDeviceWatcher::OnPortAdded(DeviceWatcher^ deviceWatcher, DeviceInformation^ devInfo)
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

void MessageIO::MidiDeviceWatcher::OnPortRemoved(DeviceWatcher^ deviceWatcher, DeviceInformationUpdate^ devInfoUpdate)
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

void MessageIO::MidiDeviceWatcher::OnPortUpdated(DeviceWatcher^ deviceWatcher, DeviceInformationUpdate^ devInfoUpdate)
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

void MessageIO::MidiDeviceWatcher::OnPortEnumCompleted(DeviceWatcher^ deviceWatcher, Object^ obj)
{
    _enumCompleted = true;

    create_task(_coreDispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::High,
        ref new Windows::UI::Core::DispatchedHandler([this]()
    {
        UpdatePorts();
    })));
}