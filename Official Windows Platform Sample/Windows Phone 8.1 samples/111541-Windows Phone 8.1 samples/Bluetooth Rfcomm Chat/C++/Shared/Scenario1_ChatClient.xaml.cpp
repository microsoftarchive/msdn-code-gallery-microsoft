//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1_ChatClient.xaml.cpp
// Implementation of the Scenario1_ChatClient class
//

#include "pch.h"
#include "Scenario1_ChatClient.xaml.h"

using namespace concurrency;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation::Collections;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::Devices::Bluetooth::Rfcomm;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Networking::Sockets;
using namespace Windows::Storage::Streams;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

using namespace SDKSample;
using namespace SDKSample::BluetoothRfcommChat;

// The Chat Service's custom service Uuid {34B1CF4D-1069-4AD6-89B6-E161D79BE4D8}
static const GUID RFCOMM_CHAT_SERVICE_UUID = {0x34B1CF4D, 0x1069, 0x4AD6, 0x89, 0xB6, 0xE1, 0x61, 0xD7, 0x9B, 0xE4, 0xD8};

// The Id of the Service Name SDP attribute
static const uint16 SDP_SERVICE_NAME_ATTRIBUTE_ID = 0x100;

// The SDP Type of the Service Name SDP attribute.
// The first byte in the SDP Attribute encodes the SDP Attribute Type as follows :
//    -  the Attribute Type size in the least significant 3 bits,
//    -  the SDP Attribute Type value in the most significant 5 bits.
static const byte SDP_SERVICE_NAME_ATTRIBUTE_TYPE = (4 << 3) | 5;

Scenario1_ChatClient::Scenario1_ChatClient()
{
    InitializeComponent();

    chatServiceInfoCollection = nullptr;
    chatService = nullptr;
    chatSocket = nullptr;
    chatWriter = nullptr;

    App::Current->Suspending += ref new SuspendingEventHandler(this, &Scenario1_ChatClient::App_Suspending);
}

/// <summary>
/// Invoked on App suspension: Closes the Chat service and socket.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void Scenario1_ChatClient::App_Suspending(Object^ sender, Windows::ApplicationModel::SuspendingEventArgs^ e)
{
    Disconnect();

    MainPage::Current->NotifyUser("Disconnected due to suspension", NotifyType::StatusMessage);
}

/// <summary>
/// Starts running the scenario by displaying a list of Rfcomm chat services.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void Scenario1_ChatClient::RunButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    MainPage::Current->NotifyUser("", NotifyType::StatusMessage);

    // Find all paired instances of the Rfcomm chat service
    create_task(DeviceInformation::FindAllAsync(
        RfcommDeviceService::GetDeviceSelector(RfcommServiceId::FromUuid(RFCOMM_CHAT_SERVICE_UUID))))
        .then([this] (DeviceInformationCollection^ deviceInfoCollection)
    {
        chatServiceInfoCollection = deviceInfoCollection;
        if (chatServiceInfoCollection->Size > 0)
        {
            auto items = ref new Vector<String^>();
            std::for_each(begin(chatServiceInfoCollection), end(chatServiceInfoCollection), [items](IDeviceInformation^ chatServiceInfo)
            {
                items->Append(chatServiceInfo->Name);
            });
            cvs->Source = items;
            ServiceSelector->Visibility = Windows::UI::Xaml::Visibility::Visible;
        }
        else
        {
            MainPage::Current->NotifyUser(
                "No chat services were found. Please pair Windows with a device that is advertising the chat service",
                NotifyType::ErrorMessage);
        }
    });
}

/// <summary>
/// Invoked when a service is selected: Retrieves and displays the Service Name attribute of the remote Chat service
/// and then connects to that service so that messages can be sent and received.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void Scenario1_ChatClient::ServiceList_Tapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e)
{
    RunButton->IsEnabled = false;
    ServiceSelector->Visibility = Windows::UI::Xaml::Visibility::Collapsed;

    auto chatServiceInfo = chatServiceInfoCollection->GetAt(ServiceList->SelectedIndex);
    create_task(RfcommDeviceService::FromIdAsync(chatServiceInfo->Id))
        .then([this] (RfcommDeviceService^ service)
    {
        if (service == nullptr)
        {
            MainPage::Current->NotifyUser(
                "Access to the device is denied because the application was not granted access",
                NotifyType::StatusMessage);
            return task<void>();
        }

        chatService = service;

        return create_task(chatService->GetSdpRawAttributesAsync())
            .then([this] (task<IMapView<unsigned int, IBuffer^>^> attributesTask)
        {
            auto attributes = attributesTask.get();
            auto buffer = attributes->Lookup(SDP_SERVICE_NAME_ATTRIBUTE_ID);
            if (buffer == nullptr)
            {
                MainPage::Current->NotifyUser(
                    "The Chat service is not advertising the Service Name attribute (attribute id=0x100). " +
                    "Please verify that you are running the BluetoothRfcommChat server.", 
                    NotifyType::ErrorMessage);
                return task<void>();
            }

            auto attributeReader = DataReader::FromBuffer(buffer);
            auto attributeType = attributeReader->ReadByte();
            if (attributeType != SDP_SERVICE_NAME_ATTRIBUTE_TYPE)
            {
                MainPage::Current->NotifyUser(
                    "The Chat service is using an unexpected format for the Service Name attribute. " +
                    "Please verify that you are running the BluetoothRfcommChat server.", 
                    NotifyType::ErrorMessage);
                return task<void>();
            }

            auto serviceNameLength = attributeReader->ReadByte();
                    
            // The Service Name attribute requires UTF-8 encoding.
            attributeReader->UnicodeEncoding = UnicodeEncoding::Utf8;
            ServiceName->Text = "Service Name: \"" + attributeReader->ReadString(serviceNameLength) + "\"";

            {
                reader_writer_lock::scoped_lock lock(chatSocketLock);
                chatSocket = ref new StreamSocket();
            }

            return create_task(chatSocket->ConnectAsync(
                chatService->ConnectionHostName, chatService->ConnectionServiceName))
                .then([this] ()
            {
                chatWriter = ref new DataWriter(chatSocket->OutputStream);
                ChatBox->Visibility = Windows::UI::Xaml::Visibility::Visible;

                DataReader^ chatReader = ref new DataReader(chatSocket->InputStream);
                ReceiveStringLoop(chatReader);
            });
        });
    }).then([this] (task<void> finalTask)
    {
        try 
        {
            // Capture any errors and exceptions that occured
            finalTask.get();
        }
        catch (COMException^ e)
        {
            RunButton->IsEnabled = true;
            MainPage::Current->NotifyUser(
                "Error: " + e->HResult.ToString() + " - " + e->Message, 
                NotifyType::ErrorMessage);
        }
    });
}

/// <summary>
/// Sends a Chat message to remote Rfcomm service.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void Scenario1_ChatClient::SendButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        chatWriter->WriteUInt32(MessageTextBox->Text->Length());
        chatWriter->WriteString(MessageTextBox->Text);

        chatWriter->StoreAsync();
        ConversationList->Items->Append("Sent: " + MessageTextBox->Text);
        
        MessageTextBox->Text = "";
    }
    catch (Exception^ e)
    {
        MainPage::Current->NotifyUser("Error: " + e->HResult.ToString() + " - " + e->Message, 
            NotifyType::ErrorMessage);
    }
}

/// <summary>
/// Closes the Chat service and socket.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void Scenario1_ChatClient::DisconnectButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Disconnect();

    MainPage::Current->NotifyUser("Disconnected", NotifyType::StatusMessage);
}

/// <summary>
/// Reads the next message from the socket
/// </summary>
void Scenario1_ChatClient::ReceiveStringLoop(DataReader^ chatReader)
{
    // Read first 4 bytes (length of the subsequent string).
    create_task(chatReader->LoadAsync(sizeof(UINT32)))
        .then([this, chatReader] (unsigned int size)
    {
        if (size < sizeof(UINT32))
        {
            // The underlying socket was closed before we were able to read the whole data.
            cancel_current_task();
        }

        unsigned int stringLength = chatReader->ReadUInt32();
        return create_task(chatReader->LoadAsync(stringLength))
            .then([this, chatReader, stringLength] (unsigned int actualStringLength)
        {
            if (actualStringLength != stringLength)
            {
                // The underlying socket was closed before we were able to read the whole data.
                cancel_current_task();
            }
            
            // Display the string on the screen. This thread is invoked on non-UI thread, so we need to marshal the
            // call back to the UI thread.
            MainPage::Current->Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler(
                [this, chatReader, stringLength] ()
            {
                ConversationList->Items->Append("Received: \"" + chatReader->ReadString(stringLength) + "\"");
            }));
        });
    }).then([this, chatReader] (task<void> finalTask)
    {
        try
        {
            // Try getting all exceptions from the continuation chain above this point.
            finalTask.get();

            // Everything went ok, so try to receive another string. The receive will continue until the stream is broken (i.e. the socket is closed).
            ReceiveStringLoop(chatReader);
        }
        catch (Exception^ exception)
        {
            reader_writer_lock::scoped_lock_read lock(chatSocketLock);
            if (chatSocket == nullptr)
            {
                // Do not print anything here - the user closed the client socket.
            }
            else
            {
                NotifyUserFromAsyncThread("Read stream failed with error: " + exception->Message, NotifyType::ErrorMessage);
                Disconnect();
            }
        }
        catch (task_canceled&)
        {
            Disconnect();
            NotifyUserFromAsyncThread("Client disconnected.", NotifyType::StatusMessage);
        }
    });
}

/// <summary>
/// Closes the Chat service and socket.
/// </summary>
void Scenario1_ChatClient::Disconnect()
{
    if (chatWriter != nullptr)
    {
        chatWriter->DetachStream();
        chatWriter = nullptr;
    }

    {
        reader_writer_lock::scoped_lock lock(chatSocketLock);
        if (chatSocket != nullptr)
        {
            delete chatSocket;
            chatSocket = nullptr;
        }
    }

    RunButton->IsEnabled = true;
    ServiceSelector->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    ChatBox->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    ConversationList->Items->Clear();
}

void Scenario1_ChatClient::NotifyUserFromAsyncThread(String^ message, NotifyType type)
{
    MainPage::Current->Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([message, type] ()
    {
        MainPage::Current->NotifyUser(message, type);
    }));
}
