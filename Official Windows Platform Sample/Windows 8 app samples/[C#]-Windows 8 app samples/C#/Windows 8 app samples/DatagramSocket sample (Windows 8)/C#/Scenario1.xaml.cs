//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using SDKTemplate;
using System;
using Windows.ApplicationModel.Core;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DatagramSocketSample
{
    /// <summary>
    /// A page for first scenario.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario1()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// This is the click handler for the 'StartListener' button.
        /// </summary>
        /// <param name="sender">Object for which the event was generated.</param>
        /// <param name="e">Event's parameters.</param>
        private async void StartListener_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(ServiceNameForListener.Text))
            {
                rootPage.NotifyUser("Please provide a service name.", NotifyType.ErrorMessage);
                return;
            }

            if (CoreApplication.Properties.ContainsKey("listener"))
            {
                rootPage.NotifyUser("This step has already been executed. Please move to the next one.", NotifyType.ErrorMessage);
                return;
            }

            DatagramSocket listener = new DatagramSocket();
            listener.MessageReceived += MessageReceived;

            // Save the socket, so subsequent steps can use it.
            CoreApplication.Properties.Add("listener", listener);

            // Start listen operation.
            try
            {
                await listener.BindServiceNameAsync(ServiceNameForListener.Text);
                rootPage.NotifyUser("Listening", NotifyType.StatusMessage);
            }
            catch (Exception exception)
            {
                CoreApplication.Properties.Remove("listener");
                
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                rootPage.NotifyUser("Start listening failed with error: " + exception.Message, NotifyType.ErrorMessage);
            }
        }

        async void MessageReceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs eventArguments)
        {
            object outObj;
            if (CoreApplication.Properties.TryGetValue("remotePeer", out outObj))
            {
                EchoMessage((RemotePeer)outObj, eventArguments);
                return;
            }

            // We do not have an output stream yet so create one.
            try
            {
                IOutputStream outputStream = await socket.GetOutputStreamAsync(eventArguments.RemoteAddress, eventArguments.RemotePort);

                // It might happen that the OnMessage was invoked more than once before the GetOutputStreamAsync completed.
                // In this case we will end up with multiple streams - make sure we have just one of it.
                RemotePeer peer;
                lock (this)
                {
                    if (CoreApplication.Properties.TryGetValue("remotePeer", out outObj))
                    {
                        peer = (RemotePeer)outObj;
                    }
                    else
                    {
                        peer = new RemotePeer(outputStream, eventArguments.RemoteAddress, eventArguments.RemotePort);
                        CoreApplication.Properties.Add("remotePeer", peer);
                    }
                }
                
                EchoMessage(peer, eventArguments);
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                NotifyUserFromAsyncThread("Connect failed with error: " + exception.Message, NotifyType.ErrorMessage);
            }
        }

        async void EchoMessage(RemotePeer peer, DatagramSocketMessageReceivedEventArgs eventArguments)
        {
            if (!peer.IsMatching(eventArguments.RemoteAddress, eventArguments.RemotePort))
            {
                // In the sample we are communicating with just one peer. To communicate with multiple peers application
                // should cache output streams (i.e. by using a hash map), because creating an output stream for each
                //  received datagram is costly. Keep in mind though, that every cache requires logic to remove old
                // or unused elements; otherwise cache turns into a memory leaking structure.
                NotifyUserFromAsyncThread(String.Format("Got datagram from {0}:{1}, but already 'connected' to {3}", eventArguments.RemoteAddress, eventArguments.RemotePort, peer), NotifyType.ErrorMessage);
            }

            try
            {
                await peer.OutputStream.WriteAsync(eventArguments.GetDataReader().DetachBuffer());
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                NotifyUserFromAsyncThread("Send failed with error: " + exception.Message, NotifyType.ErrorMessage);
            }
        }

        private void NotifyUserFromAsyncThread(string strMessage, NotifyType type)
        {
            var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => rootPage.NotifyUser(strMessage, type));
        }
    }

    class RemotePeer
    {
        IOutputStream outputStream;
        HostName hostName;
        String port;

        public RemotePeer(IOutputStream outputStream, HostName hostName, String port)
        {
            this.outputStream = outputStream;
            this.hostName = hostName;
            this.port = port;
        }

        public bool IsMatching(HostName hostName, String port)
        {
            return (this.hostName == hostName && this.port == port);
        }

        public IOutputStream OutputStream
        {
            get { return outputStream; }
        }

        public override String ToString()
        {
            return hostName + port;
        }
    }
}
