//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;

namespace BluetoothRfcommChat
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2_ChatServer : Page
    {
        // The Chat Server's custom service Uuid: 34B1CF4D-1069-4AD6-89B6-E161D79BE4D8
        private static readonly Guid RfcommChatServiceUuid = Guid.Parse("34B1CF4D-1069-4AD6-89B6-E161D79BE4D8");

        // The Id of the Service Name SDP attribute
        private const UInt16 SdpServiceNameAttributeId = 0x100;

        // The SDP Type of the Service Name SDP attribute.
        // The first byte in the SDP Attribute encodes the SDP Attribute Type as follows :
        //    -  the Attribute Type size in the least significant 3 bits,
        //    -  the SDP Attribute Type value in the most significant 5 bits.
        private const byte SdpServiceNameAttributeType = (4 << 3) | 5;

        // The value of the Service Name SDP attribute
        private const string SdpServiceName = "Bluetooth Rfcomm Chat Service";

        private StreamSocket socket;
        private DataWriter writer;
        private RfcommServiceProvider rfcommProvider;
        private StreamSocketListener socketListener;

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario2_ChatServer()
        {
            this.InitializeComponent();

            App.Current.Suspending += App_Suspending;
        }

        void App_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            // Make sure we cleanup resources on suspend
            Disconnect();
        }
       
        /// <summary>
        /// Initialize a server socket listening for incoming Bluetooth Rfcomm connections
        /// </summary>
        async void InitializeRfcommServer()
        {
            try
            {
                ListenButton.IsEnabled = false;
                DisconnectButton.IsEnabled = true;

                rfcommProvider = await RfcommServiceProvider.CreateAsync(
                    RfcommServiceId.FromUuid(RfcommChatServiceUuid));

                // Create a listener for this service and start listening
                socketListener = new StreamSocketListener();
                socketListener.ConnectionReceived += OnConnectionReceived;

                await socketListener.BindServiceNameAsync(rfcommProvider.ServiceId.AsString(),
                    SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);

                // Set the SDP attributes and start Bluetooth advertising
                InitializeServiceSdpAttributes(rfcommProvider);
                rfcommProvider.StartAdvertising(socketListener);

                NotifyStatus("Listening for incoming connections");
            }
            catch (Exception e)
            {
                NotifyError(e);
            }
        }

        /// <summary>
        /// Initialize the Rfcomm service's SDP attributes.
        /// </summary>
        /// <param name="rfcommProvider">The Rfcomm service provider to initialize.</param>
        private void InitializeServiceSdpAttributes(RfcommServiceProvider rfcommProvider)
        {
            var sdpWriter = new DataWriter();

            // Write the Service Name Attribute.

            sdpWriter.WriteByte(SdpServiceNameAttributeType);

            // The length of the UTF-8 encoded Service Name SDP Attribute.
            sdpWriter.WriteByte((byte)SdpServiceName.Length);

            // The UTF-8 encoded Service Name value.
            sdpWriter.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
            sdpWriter.WriteString(SdpServiceName);

            // Set the SDP Attribute on the RFCOMM Service Provider.
            rfcommProvider.SdpRawAttributes.Add(SdpServiceNameAttributeId, sdpWriter.DetachBuffer());
        }

        /// <summary>
        /// Invoked when the socket listener accepted an incoming Bluetooth connection.
        /// </summary>
        /// <param name="sender">The socket listener that accecpted the connection.</param>
        /// <param name="args">The connection accept parameters, which contain the connected socket.</param>
        private async void OnConnectionReceived(
            StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                NotifyStatus("Client Connected");

                // Don't need the listener anymore
                socketListener.Dispose();
                socketListener = null;

                socket = args.Socket;

                writer = new DataWriter(socket.OutputStream);

                var reader = new DataReader(socket.InputStream);
                bool remoteDisconnection = false;
                while (true)
                {
                    uint readLength = await reader.LoadAsync(sizeof(uint));
                    if (readLength < sizeof(uint))
                    {
                        remoteDisconnection = true;
                        break;
                    }
                    uint currentLength = reader.ReadUInt32();

                    readLength = await reader.LoadAsync(currentLength);
                    if (readLength < currentLength)
                    {
                        remoteDisconnection = true;
                        break;
                    }
                    string message = reader.ReadString(currentLength);

                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        ConversationListBox.Items.Add("Received: " + message);
                    });
                }

                reader.DetachStream();
                if (remoteDisconnection)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Disconnect();
                        NotifyStatus("Client disconnected.");
                    });
                }
            }
            catch (Exception e)
            {
                NotifyError(e);
            }
        }

        /// <summary>
        /// Send message over the Bluetooth socket.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (socket != null)
                {
                    string message = MessageTextBox.Text;
                    writer.WriteUInt32((uint)message.Length);
                    writer.WriteString(message);

                    await writer.StoreAsync();
                    ConversationListBox.Items.Add("Sent: " + message);

                    // Clear the messageTextBox for a new message
                    MessageTextBox.Text = "";
                }
                else
                {
                    NotifyStatus("No clients connected, please wait for a client to connect before attempting to send a message");
                }
            }
            catch (Exception ex)
            {
                NotifyError(ex);
            }
        }

        /// <summary>
        /// Start the Bluetooth server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListenButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeRfcommServer();
        }

        /// <summary>
        /// Stop Bluetooth server and cleanup any outstanding connections.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();
            NotifyStatus("Disconnected.");
        }
         
        /// <summary>
        /// Cleanup Bluetooth resources
        /// </summary>
        private void Disconnect()
        {
            if (rfcommProvider != null)
            {
                rfcommProvider.StopAdvertising();
                rfcommProvider = null;
            }

            if (socketListener != null)
            {
                socketListener.Dispose();
                socketListener = null;
            }

            if (writer != null)
            {
                writer.DetachStream();
                writer = null;
            }

            if (socket != null)
            {
                socket.Dispose();
                socket = null;
            }

            ListenButton.IsEnabled = true;
            DisconnectButton.IsEnabled = false;
            ConversationListBox.Items.Clear();
        }

        private async void NotifyStatus(string message)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                MainPage.Current.NotifyUser(message, NotifyType.StatusMessage);
            });
        }

        private async void NotifyError(Exception e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                MainPage.Current.NotifyUser("ERROR: " + String.Format("0x{0:X8}", e.HResult) + " - " + e.Message,
                    NotifyType.ErrorMessage);
            });
        }
    }
}
