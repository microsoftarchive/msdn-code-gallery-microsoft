//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "SocketHelper.h"

using namespace SDKSample;
using namespace Windows::Storage::Streams;
using namespace Platform;

// Send a message through a specific dataWriter
void SocketHelper::SendMessageToPeer(String^ message, ConnectedPeer^ connectedPeer)
{
    if (!(connectedPeer->IsSocketClosed()))
    {
        Windows::Storage::Streams::DataWriter^ dataWriter = connectedPeer->GetWriter();
        dataWriter->WriteUInt32(dataWriter->MeasureString(message));
        dataWriter->WriteString(message);
        concurrency::task<unsigned int> storeTask(dataWriter->StoreAsync());
        storeTask.then([this, message](concurrency::task<unsigned int> resultTask)
        {
            try
            {
                unsigned int numBytesWritten = resultTask.get();
                if (numBytesWritten > 0)
                {
                    MessageSent(this, "Sent message: " + message + ", number of bytes written: " + numBytesWritten.ToString());
                }
                else
                {
                    SocketError(this, "The remote side closed the socket");
                }
            }
            catch (Exception^ e)
            {
                SocketError(this, "Failed to send message with error: " + e->Message);
            }
        });
    }
    else
    {
        SocketError(this, "The remote side closed the socket");
    }
}

void SocketHelper::StartReader(DataReader^ socketReader, bool fSocketClosed)
{
    // Read the message sent by the remote peer
    concurrency::task<unsigned int> loadTask(socketReader->LoadAsync(sizeof(unsigned int)));
    loadTask.then([this, socketReader, fSocketClosed](concurrency::task<unsigned int> stringBytesTask)
    {
        try
        {
            unsigned int bytesRead = stringBytesTask.get();
            if (bytesRead > 0)
            {
                unsigned int strLength = (unsigned int)socketReader->ReadUInt32();
                concurrency::task<unsigned int> loadStringTask(socketReader->LoadAsync(strLength));
                loadStringTask.then([this, strLength, socketReader, fSocketClosed](concurrency::task<unsigned int> resultTask)
                {
                    try
                    {
                        unsigned int bytesRead = resultTask.get();
                        if (bytesRead > 0)
                        {
                            String^ message = socketReader->ReadString(strLength);
                            MessageSent(this, "Got message: " + message);
                            StartReader(socketReader, fSocketClosed);
                        }
                        else
                        {
                            SocketError(this, "The remote side closed the socket");
                            delete socketReader;
                        }
                    }
                    catch (Exception^ e)
                    {
                        if (!fSocketClosed )
                        {
                            SocketError(this, "Failed to read from socket: " + e->Message);
                        }
                        delete socketReader;
                    }
                });
            }
            else
            {
                SocketError(this, "The remote side closed the socket");
                delete socketReader;
            }
        }
        catch (Exception^ e)
        {
            if (!fSocketClosed)
            {
                SocketError(this, "Failed to read from socket: " + e->Message);
            }
            delete socketReader;
        }
    });
}

void SocketHelper::CloseSocket()
{
    // Close all the established sockets.
    for each (ConnectedPeer^ obj in m_connectedPeers)
    {
        if (obj->GetSocket() != nullptr)
        {
            obj->SetSocketClosedState(true);
            delete obj->GetSocket();
            obj->SetSocket(nullptr);
        }

        if (obj->GetWriter() != nullptr)
        {
            delete obj->GetWriter();
            obj->SetWriter(nullptr);
        }
    }

    m_connectedPeers->Clear();
}