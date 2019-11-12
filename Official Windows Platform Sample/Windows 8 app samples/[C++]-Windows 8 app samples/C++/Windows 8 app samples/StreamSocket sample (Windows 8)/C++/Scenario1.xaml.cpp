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

using namespace StreamSocketSample;

using namespace Concurrency;
using namespace Platform;
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
    // Overriding the listener here is safe as it will be deleted once all references to it are gone. However, in many cases this
    // is a dangerous pattern to override data semi-randomly (each time user clicked the button) so we block it here.
    if (CoreApplication::Properties->HasKey("listener"))
    {
        rootPage->NotifyUser("This step has already been executed. Please move to the next one.", NotifyType::ErrorMessage);
        return;
    }

    if (ServiceNameForListener->Text == nullptr)
    {
        rootPage->NotifyUser("Please provide a service name.", NotifyType::ErrorMessage);
        return;
    }

    StreamSocketListener^ listener = ref new StreamSocketListener();
    ListenerContext^ listenerContext = ref new ListenerContext(rootPage, listener);
    listener->ConnectionReceived += ref new TypedEventHandler<StreamSocketListener^, StreamSocketListenerConnectionReceivedEventArgs^>(listenerContext, &ListenerContext::OnConnection);

    // Events cannot be hooked up directly to the ScenarioInput1 object, as the object can fall out-of-scope and be deleted. This would render
    // any event hooked up to the object ineffective. The ListenerContext guarantees that both the listener and object that serves it's events have the same life time.
    CoreApplication::Properties->Insert("listener", listenerContext);

    // Start listen operation.
    task<void>(listener->BindServiceNameAsync(ServiceNameForListener->Text)).then([this] (task<void> previousTask)
    {
        try
        {
            // Try getting all exceptions from the continuation chain above this point.
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

ListenerContext::ListenerContext(MainPage^ rootPage, StreamSocketListener^ listener)
{
    this->rootPage = rootPage;
    this->listener = listener;
}

ListenerContext::~ListenerContext()
{
    // The listener can be closed in two ways:
    //  - explicit: by using delete operator (the listener is closed even if there are outstanding references to it).
    //  - implicit: by removing last reference to it (i.e. falling out-of-scope).
    // In this case this is the last reference to the listener so both will yield the same result.
    delete listener;
    listener = nullptr;
}

void ListenerContext::OnConnection(StreamSocketListener^ listener, StreamSocketListenerConnectionReceivedEventArgs^ object)
{
    DataReader^ reader = ref new DataReader(object->Socket->InputStream);

    // Start a receive loop.
    ReceiveStringLoop(reader, object->Socket);
}

void ListenerContext::ReceiveStringLoop(DataReader^ reader, StreamSocket^ socket)
{
    // Read first 4 bytes (length of the subsequent string).
    task<unsigned int>(reader->LoadAsync(sizeof(UINT32))).then([this, reader, socket] (unsigned int size)
    {
        if (size < sizeof(UINT32))
        {
            // The underlying socket was closed before we were able to read the whole data.
            cancel_current_task();
        }

        unsigned int stringLength = reader->ReadUInt32();
        return task<unsigned int>(reader->LoadAsync(stringLength)).then([this, reader, stringLength] (unsigned int actualStringLength)
        {
            if (actualStringLength != stringLength)
            {
                // The underlying socket was closed before we were able to read the whole data.
                cancel_current_task();
            }
            
            // Display the string on the screen. This thread is invoked on non-UI thread, so we need to marshal the call back to the UI thread.
            NotifyUserFromAsyncThread("Received data: \"" + reader->ReadString(actualStringLength) + "\"", NotifyType::StatusMessage);
        });
    }).then([this, reader, socket] (task<void> previousTask)
    {
        try
        {
            // Try getting all exceptions from the continuation chain above this point.
            previousTask.get();

            // Everything went ok, so try to receive another string. The receive will continue until the stream is broken (i.e. peer closed closed the socket).
            ReceiveStringLoop(reader, socket);
        }
        catch (Exception^ exception)
        {
            NotifyUserFromAsyncThread("Read stream failed with error: " + exception->Message, NotifyType::ErrorMessage);

            // Explicitly close the socket.
            delete socket;
        }
        catch (task_canceled&)
        {
            // Do not print anything here - this will usually happen because user closed the client socket.

            // Explicitly close the socket.
            delete socket;
        }
    });
}

void ListenerContext::NotifyUserFromAsyncThread(String^ message, NotifyType type)
{
    rootPage->Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, message, type] ()
    {
        rootPage->NotifyUser(message, type);
    }));
}
