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
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"

using namespace DatagramSocketSample;

using namespace Concurrency;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::Foundation;
using namespace Windows::Networking::Sockets;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario1::Scenario1()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void Scenario1::StartListener_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (ServiceNameForListener->Text == nullptr)
    {
        rootPage->NotifyUser("Please provide a service name.", NotifyType::ErrorMessage);
        return;
    }

    if (CoreApplication::Properties->HasKey("listener"))
    {
        rootPage->NotifyUser("This step has already been executed. Please move to the next one.", NotifyType::ErrorMessage);
        return;
    }

    DatagramSocket^ listener = ref new DatagramSocket();
    ListenerContext^ listenerContext = ref new ListenerContext(rootPage, listener);
    listener->MessageReceived += ref new TypedEventHandler<DatagramSocket^, DatagramSocketMessageReceivedEventArgs^>(listenerContext, &ListenerContext::OnMessage);

    // Events cannot be hooked up directly to the ScenarioInput1 object, as the object can fall out-of-scope and be deleted. This would render
    // any event hooked up to the object ineffective. The ListenerContext guarantees that both the socket and object that serves it's events have the same life time.
    CoreApplication::Properties->Insert("listener", listenerContext);

    // Start listen operation.
    task<void>(listener->BindServiceNameAsync(ServiceNameForListener->Text)).then([this] (task<void> previousTask)
    {
        try
        {
            // Try getting an exception.
            previousTask.get();
            rootPage->NotifyUser("Listening", NotifyType::StatusMessage);
        }
        catch (Exception^ exception)
        {
            CoreApplication::Properties->Remove("listener");
            rootPage->NotifyUser("Start listening failed with error: " + exception->Message, NotifyType::ErrorMessage);
        }
    });
}

void ListenerContext::OnMessage(DatagramSocket^ socket, DatagramSocketMessageReceivedEventArgs^ eventArguments)
{
    if (outputStream != nullptr)
    {
        EchoMessage(eventArguments);
        return;
    }

    // We do not have an output stream yet so create one.
    task<IOutputStream^>(socket->GetOutputStreamAsync(eventArguments->RemoteAddress, eventArguments->RemotePort)).then([this, socket, eventArguments] (IOutputStream^ stream)
    {
        // It might happen that the OnMessage was invoked more than once before the GetOutputStreamAsync completed.
        // In this case we will end up with multiple streams - make sure we have just one of it.
        EnterCriticalSection(&lock);

        if (outputStream == nullptr)
        {
            outputStream = stream;
            hostName = eventArguments->RemoteAddress;
            port = eventArguments->RemotePort;
        }

        LeaveCriticalSection(&lock);

        EchoMessage(eventArguments);
    }).then([this, socket, eventArguments] (task<void> previousTask)
    {
        try
        {
            // Try getting all exceptions from the continuation chain above this point.
            previousTask.get();
        }
        catch (Exception^ exception)
        {
            NotifyUserFromAsyncThread("Getting an output stream failed with error: " + exception->Message, NotifyType::ErrorMessage);
        }
    });
}

ListenerContext::ListenerContext(MainPage^ rootPage, DatagramSocket^ listener)
{
    this->rootPage = rootPage;
    this->listener = listener;
    InitializeCriticalSectionEx(&lock, 0, 0);
}

ListenerContext::~ListenerContext()
{
    // The listener can be closed in two ways:
    //  - explicit: by using delete operator (the listener is closed even if there are outstanding references to it).
    //  - implicit: by removing last reference to it (i.e. falling out-of-scope).
    // In this case this is the last reference to the listener so both will yield the same result.
    delete listener;
    listener = nullptr;
    
    DeleteCriticalSection(&lock);
}

void ListenerContext::EchoMessage(DatagramSocketMessageReceivedEventArgs^ eventArguments)
{
    if (!IsMatching(eventArguments->RemoteAddress, eventArguments->RemotePort))
    {
        // In the sample we are communicating with just one peer. To communicate with multiple peers application
        // should cache output streams (i.e. by using a hash map), because creating an output stream for each
        // received datagram is costly. Keep in mind though, that every cache requires logic to remove old
        // or unused elements; otherwise cache turns into a memory leaking structure.
        NotifyUserFromAsyncThread("Got datagram from " + eventArguments->RemoteAddress->DisplayName + ":" +
            eventArguments->RemotePort + ", but already 'connected' to " + hostName->DisplayName + ":" +
            port, NotifyType::ErrorMessage);
    }

    task<unsigned int>(outputStream->WriteAsync(eventArguments->GetDataReader()->DetachBuffer())).then([this] (task<unsigned int> writeTask)
    {
        try
        {
            // Try getting an exception.
            writeTask.get();
        }
        catch (Exception^ exception)
        {
            NotifyUserFromAsyncThread("Echoing a message failed with error: " + exception->Message, NotifyType::ErrorMessage);
        }
    });
}

bool ListenerContext::IsMatching(HostName^ hostName, String^ port)
{
    return (this->hostName == hostName && this->port == port);
}

void ListenerContext::NotifyUserFromAsyncThread(String^ message, NotifyType type)
{
    rootPage->Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, message, type] ()
    {
        rootPage->NotifyUser(message, type);
    }));
}
