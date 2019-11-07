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
// PeerFinderScenario.xaml.cpp
// Implementation of the PeerFinderScenario class
//



#include "pch.h"
#include "PeerFinderScenario.xaml.h"

using namespace ProximityCPP;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Networking::Proximity;
using namespace Windows::Networking::Sockets;
using namespace Windows::Storage::Streams;
using namespace Platform;


PeerFinderScenario::PeerFinderScenario()
{
    InitializeComponent();
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    m_rootPage = MainPage::Current;

    m_triggeredConnectSupported = (PeerFinder::SupportedDiscoveryTypes & PeerDiscoveryTypes::Triggered) ==
        PeerDiscoveryTypes::Triggered;
    m_browseConnectSupported = (PeerFinder::SupportedDiscoveryTypes & PeerDiscoveryTypes::Browse) ==
        PeerDiscoveryTypes::Browse;

    //connection states
    m_ConnectStateArray = ref new Platform::Collections::Vector<String^>();
    m_ConnectStateArray->InsertAt((int)TriggeredConnectState::PeerFound, "PeerFound");
    m_ConnectStateArray->InsertAt((int)TriggeredConnectState::Listening, "Listening");
    m_ConnectStateArray->InsertAt((int)TriggeredConnectState::Connecting, "Connecting");
    m_ConnectStateArray->InsertAt((int)TriggeredConnectState::Completed, "Completed");
    m_ConnectStateArray->InsertAt((int)TriggeredConnectState::Canceled, "Canceled");
    m_ConnectStateArray->InsertAt((int)TriggeredConnectState::Failed, "Failed");

    if (m_triggeredConnectSupported || m_browseConnectSupported)
    {
        PeerFinder_StartFindingPeersButton->Click += ref new RoutedEventHandler(this, &PeerFinderScenario::PeerFinder_StartFindingPeers);
        PeerFinder_BrowsePeersButton->Click += ref new RoutedEventHandler(this, &PeerFinderScenario::PeerFinder_BrowsePeers);
        PeerFinder_ConnectButton->Click += ref new RoutedEventHandler(this, &PeerFinderScenario::PeerFinder_Connect);
        PeerFinder_AcceptButton->Click += ref new RoutedEventHandler(this, &PeerFinderScenario::PeerFinder_Accept);
        PeerFinder_SendButton->Click += ref new RoutedEventHandler(this, &PeerFinderScenario::PeerFinder_Send);
        PeerFinder_StartFindingPeersButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
    }
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void PeerFinderScenario::OnNavigatedTo(NavigationEventArgs^ e)
{
    if (m_triggeredConnectSupported || m_browseConnectSupported)
    {
        PeerFinder_StartFindingPeersButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
        PeerFinder_BrowsePeersButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        PeerFinder_ConnectButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        PeerFinder_FoundPeersList->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        PeerFinder_SendButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        PeerFinder_AcceptButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        PeerFinder_MessageBox->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
        PeerFinder_MessageBox->Text = "Hello World";
        m_rootPage->ClearLog(PeerFinderOutputText);
        if (m_rootPage->IsLaunchedByTap())
        {
            m_rootPage->NotifyUser("Launched by tap", NotifyType::StatusMessage);
            PeerFinder_StartFindingPeers(nullptr, nullptr);
        }
        else
        {
            if (!m_triggeredConnectSupported)
            {
                m_rootPage->NotifyUser("Tap based discovery of peers not supported", NotifyType::ErrorMessage);
            }
            else if (!m_browseConnectSupported)
            {
                m_rootPage->NotifyUser("Browsing for peers not supported", NotifyType::ErrorMessage);
            }
        }
    }
    else
    {
        m_rootPage->NotifyUser("Tap based discovery of peers not supported \nBrowsing for peers not supported", NotifyType::ErrorMessage);
    }
}

void PeerFinderScenario::OnNavigatingFrom(NavigatingCancelEventArgs^ e)
{
    m_startPeerFinderImmediately = false;
    if (m_peerFinderStarted)
    {
        // unregister the events when the window goes out of scope
        PeerFinder::TriggeredConnectionStateChanged -= m_triggerToken;
        PeerFinder::ConnectionRequested -= m_connectionRequestedToken;
        m_peerFinderStarted = false;
        CloseSocket();
        PeerFinder::Stop();
    }
}

void PeerFinderScenario::ConnectionRequestedEventHandler(Object^ sender, ConnectionRequestedEventArgs^ connectionEventArgs) 
{
    m_rootPage->NotifyUser("Connection requested from peer " + connectionEventArgs->PeerInformation->DisplayName, NotifyType::StatusMessage);
    m_requestingPeer = connectionEventArgs->PeerInformation;
    PeerFinder_AcceptButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
    PeerFinder_SendButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    PeerFinder_MessageBox->Visibility = Windows::UI::Xaml::Visibility::Collapsed;    
}

void PeerFinderScenario::TriggeredConnectionStateChangedEventHandler(Object^ sender, TriggeredConnectionStateChangedEventArgs^ args) 
{
    m_rootPage->LogInfo("TriggeredConnectionStateChangedEventHandler - " +
        m_ConnectStateArray->GetAt((int)args->State), PeerFinderOutputText);

    if (args->State == TriggeredConnectState::PeerFound)
    {
        // Use this state to indicate to users that the tap is complete and
        // they can pull there devices away.
        m_rootPage->NotifyUser("Tap complete, socket connection starting!", NotifyType::StatusMessage);
    }

    if (args->State == TriggeredConnectState::Completed) 
    {
        m_rootPage->NotifyUser("Socket connect success!", NotifyType::StatusMessage);
        // Start using the socket that just connected.
        PeerFinder_StartSendReceive(args->Socket);
    }

    if (args->State == TriggeredConnectState::Failed)
    {
        m_rootPage->NotifyUser("Socket connect failed!", NotifyType::ErrorMessage);
    }
}

void PeerFinderScenario::PeerFinder_StartFindingPeers(Object^ sender, RoutedEventArgs^ e) 
{
    // Then start listening for proximate peers
    m_rootPage->NotifyUser("", NotifyType::ErrorMessage);
    if (m_peerFinderStarted == false)
    {
        PeerFinder::Start();
        m_peerFinderStarted = true;
        // First attach the callback handler
        m_triggerToken = PeerFinder::TriggeredConnectionStateChanged += ref new TypedEventHandler<Object^, TriggeredConnectionStateChangedEventArgs^>(this,
            &PeerFinderScenario::TriggeredConnectionStateChangedEventHandler, CallbackContext::Same);
        m_connectionRequestedToken = PeerFinder::ConnectionRequested += ref new TypedEventHandler<Object^, Windows::Networking::Proximity::ConnectionRequestedEventArgs^>(this, 
            &PeerFinderScenario::ConnectionRequestedEventHandler, CallbackContext::Same);
        if (m_browseConnectSupported && m_triggeredConnectSupported)
        {
            PeerFinder_BrowsePeersButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
            m_rootPage->NotifyUser("Tap another device to connect to a peer or click Browse for Peers button.", NotifyType::StatusMessage);
        }
        else if (m_triggeredConnectSupported)
        {
            m_rootPage->NotifyUser("Tap another device to connect to a peer.", NotifyType::StatusMessage);
        }
        else
        {
            PeerFinder_BrowsePeersButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
            m_rootPage->NotifyUser("Click Browse for Peers button.", NotifyType::StatusMessage);
        }
    }
}

void PeerFinderScenario::PeerFinder_BrowsePeers(Object^ sender, RoutedEventArgs^ e) 
{
    m_rootPage->NotifyUser("Finding Peers...", NotifyType::StatusMessage);
    auto op = PeerFinder::FindAllPeersAsync();
    concurrency::task<IVectorView<PeerInformation^>^> findAllPeersTask(op);

    findAllPeersTask.then([this](concurrency::task<IVectorView<PeerInformation^>^> resultTask)
    {
        try
        {
            m_peerInformationList = resultTask.get();
            if (m_peerInformationList->Size > 0)
            {
                PeerFinder_FoundPeersList->Items->Clear();
                for (unsigned int i = 0; i < m_peerInformationList->Size; i++)
                {                    
                    ListBoxItem^ item = ref new ListBoxItem();
                    item->Content = m_peerInformationList->GetAt(i)->DisplayName;
                    PeerFinder_FoundPeersList->Items->Append(item);
                }
                PeerFinder_ConnectButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
                PeerFinder_FoundPeersList->Visibility = Windows::UI::Xaml::Visibility::Visible;
                m_rootPage->NotifyUser("Finding Peers Done", NotifyType::StatusMessage);
            }
            else
            {
                m_rootPage->NotifyUser("No peers found", NotifyType::StatusMessage);
                PeerFinder_ConnectButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
                PeerFinder_FoundPeersList->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
            }
        }
        catch (Exception^ e)
        {
            m_rootPage->NotifyUser("Exception occurred while finding peer: " + e->Message, NotifyType::ErrorMessage);
        }
    });
}

void PeerFinderScenario::PeerFinder_Accept(Object^ sender, RoutedEventArgs^ e)
{
    m_rootPage->NotifyUser("Connecting to " + m_requestingPeer->DisplayName + "....", NotifyType::StatusMessage);
    PeerFinder_AcceptButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;

    auto op = PeerFinder::ConnectAsync(m_requestingPeer);
    concurrency::task<StreamSocket^> connectTask(op);
    connectTask.then([this](concurrency::task<StreamSocket^> resultTask)
    {
        try
        {
            m_rootPage->NotifyUser("Connection succeeded", NotifyType::StatusMessage);
            PeerFinder_StartSendReceive(resultTask.get());
        }
        catch(Exception^ e)
        {
            m_rootPage->NotifyUser("Connection to " + m_requestingPeer->DisplayName + " failed: " + e->Message, NotifyType::ErrorMessage);
        }
    });
}

void PeerFinderScenario::PeerFinder_Connect(Object^ sender, RoutedEventArgs^ e)
{
    m_rootPage->NotifyUser("", NotifyType::ErrorMessage);
    PeerInformation^ peerToConnect = nullptr;

    // If nothing is selected, select the first peer
    if (PeerFinder_FoundPeersList->SelectedIndex == -1)
    {
        peerToConnect = m_peerInformationList->GetAt(0);
    }
    else
    {
        peerToConnect = m_peerInformationList->GetAt(PeerFinder_FoundPeersList->SelectedIndex);
    }

    m_rootPage->NotifyUser("Connecting to " + peerToConnect->DisplayName + "....", NotifyType::StatusMessage);
    auto op = PeerFinder::ConnectAsync(peerToConnect);

    concurrency::task<StreamSocket^> connectTask(op);
    connectTask.then([this, peerToConnect](concurrency::task<StreamSocket^> resultTask)
    {
        try
        {
            m_rootPage->NotifyUser("Connection succeeded", NotifyType::StatusMessage);
            PeerFinder_StartSendReceive(resultTask.get()); 
        }
        catch(Exception ^e)
        {
            m_rootPage->NotifyUser("Connection to " + peerToConnect->DisplayName + " failed: " + e->Message, NotifyType::ErrorMessage);			
        }
    });
}

void PeerFinderScenario::SocketError(String^ errMessage)
{
    m_rootPage->NotifyUser(errMessage, NotifyType::ErrorMessage);
    PeerFinder_StartFindingPeersButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
    if (m_browseConnectSupported)
    {
        PeerFinder_BrowsePeersButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
    }
    PeerFinder_SendButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    PeerFinder_MessageBox->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    CloseSocket();
}

void PeerFinderScenario::CloseSocket()
{
    if (m_socket != nullptr)
    {
        m_socketClosed = true;
        delete m_socket;
        m_socket = nullptr;
    }

    if (m_dataWriter != nullptr)
    {
        delete m_dataWriter;
        m_dataWriter = nullptr;
    }
}

void PeerFinderScenario::PeerFinder_Send(Object^ sender, RoutedEventArgs^ e)
{
    m_rootPage->NotifyUser("", NotifyType::ErrorMessage);
    String^ message = PeerFinder_MessageBox->Text;
    PeerFinder_MessageBox->Text = ""; // clear the input now that the message is being sent.
    if (!m_socketClosed)
    {
        if (message->Length() > 0)
        {
            m_dataWriter->WriteUInt32(m_dataWriter->MeasureString(message));
            m_dataWriter->WriteString(message);
            concurrency::task<unsigned int> storeTask(m_dataWriter->StoreAsync());
            storeTask.then([this, message](concurrency::task<unsigned int> resultTask)
            {
                try
                {
                    unsigned int numBytesWritten = resultTask.get();
                    if (numBytesWritten > 0)
                    {
                        m_rootPage->NotifyUser("Sent message: " + message + ", number of bytes written: " + numBytesWritten.ToString(), NotifyType::StatusMessage);
                    }
                    else
                    {
                        SocketError("The remote side closed the socket");
                    }
                }
                catch (Exception^ e)
                {
                    if (!m_socketClosed)
                    {
                        SocketError("Failed to send message with error: " + e->Message);
                    }
                }
            });
        }
        else
        {
            m_rootPage->NotifyUser("Please type a message", NotifyType::ErrorMessage);
        }
    }
    else
    {
        SocketError("The remote side closed the socket");
    }
}

// Start the send receive operations
void PeerFinderScenario::PeerFinder_StartSendReceive(StreamSocket^ socket)
{
    m_socket = socket;
    // If the scenario was switched just as the socket connection completed, just close the socket.
    if (!m_peerFinderStarted)
    {
        CloseSocket();
        return;
    }

    PeerFinder_SendButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
    PeerFinder_MessageBox->Visibility = Windows::UI::Xaml::Visibility::Visible;

    // Hide the controls related to setting up a connection
    PeerFinder_ConnectButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    PeerFinder_AcceptButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    PeerFinder_FoundPeersList->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    PeerFinder_BrowsePeersButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    PeerFinder_StartFindingPeersButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    m_dataWriter = ref new DataWriter(m_socket->OutputStream);
    m_socketClosed = false;
    PeerFinder_StartReader(ref new DataReader(m_socket->InputStream));
}

void PeerFinderScenario::PeerFinder_StartReader(DataReader^ socketReader)
{
    concurrency::task<unsigned int> loadTask(socketReader->LoadAsync(sizeof(unsigned int)));
    loadTask.then([this, socketReader](concurrency::task<unsigned int> stringBytesTask)
    {
        try
        {
            unsigned int bytesRead = stringBytesTask.get();
            if (bytesRead > 0)
            {
                unsigned int strLength = (unsigned int)socketReader->ReadUInt32();
                concurrency::task<unsigned int> loadStringTask(socketReader->LoadAsync(strLength));
                loadStringTask.then([this, strLength, socketReader](concurrency::task<unsigned int> resultTask)
                {
                    try
                    {
                        unsigned int bytesRead = resultTask.get();
                        if (bytesRead > 0)
                        {
                            String^ message = socketReader->ReadString(strLength);
                            m_rootPage->NotifyUser("Got message: " + message, NotifyType::StatusMessage);
                            PeerFinder_StartReader(socketReader);
                        }
                        else
                        {
                            SocketError("The remote side closed the socket");
                            delete socketReader;
                        }
                    }
                    catch (Exception^ e)
                    {
                        if (!m_socketClosed)
                        {
                            SocketError("Failed to read from socket: " + e->Message);
                        }
                        delete socketReader;
                    }
                });
            }
            else
            {
                SocketError("The remote side closed the socket");
                delete socketReader;
            }
        }
        catch (Exception^ e)
        {
            if (!m_socketClosed)
            {
                SocketError("Failed to read from socket: " + e->Message);
            }
            delete socketReader;
        }
    });
}
