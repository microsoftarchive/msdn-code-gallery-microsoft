using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Data;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SDKTemplate
{
    public class BufferConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            String metadata = String.Empty;
            IBuffer buffer = value as IBuffer;
            if (buffer != null)
            {
                using (var metadataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
                {
                    metadata = metadataReader.ReadString(buffer.Length);
                }
                metadata = String.Format("({0})", metadata);
            }
            return metadata;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    // This class encapsulates a Peer.
    public class ConnectedPeer
    {
        public StreamSocket _socket;
        public bool _socketClosed;
        public DataWriter _dataWriter;

        public ConnectedPeer(StreamSocket socket, bool socketClosed, DataWriter dataWriter)
        {
            _socket = socket;
            _socketClosed = socketClosed;
            _dataWriter = dataWriter;
        }
    }

    public class SocketEventArgs : EventArgs
    {
        public SocketEventArgs(string s)
        {
            msg = s;
        }
        private string msg;
        public string Message
        {
            get { return msg; }
        }
    }

    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(string s)
        {
            msg = s;
        }
        private string msg;
        public string Message
        {
            get { return msg; }
        }
    }

    public enum ConnectState
    {
        PeerFound,
        Listening,
        Connecting,
        Completed,
        Canceled,
        Failed
    };

    class SocketHelper
    {
        List<ConnectedPeer> _connectedPeers = new List<ConnectedPeer>();

        public event EventHandler<SocketEventArgs> RaiseSocketErrorEvent;
        public event EventHandler<MessageEventArgs> RaiseMessageEvent;

        public ReadOnlyCollection<ConnectedPeer> ConnectedPeers
        {
            get { return new ReadOnlyCollection<ConnectedPeer>(_connectedPeers); }
        }

        public void Add(ConnectedPeer p)
        {
            _connectedPeers.Add(p);
        }

        // Send a message through a specific dataWriter
        public async void SendMessageToPeer(String message, ConnectedPeer connectedPeer)
        {
            try
            {
                if (!connectedPeer._socketClosed)
                {
                    DataWriter dataWriter = connectedPeer._dataWriter;

                    uint msgLength = dataWriter.MeasureString(message);
                    dataWriter.WriteInt32((int)msgLength);
                    dataWriter.WriteString(message);

                    uint numBytesWritten = await dataWriter.StoreAsync();
                    if (numBytesWritten > 0)
                    {
                        OnRaiseMessageEvent(new MessageEventArgs("Sent message: " + message + ", number of bytes written: " + numBytesWritten));
                    }
                    else
                    {
                        OnRaiseSocketErrorEvent(new SocketEventArgs("The remote side closed the socket"));
                    }
                }
            }
            catch (Exception err)
            {
                if (!connectedPeer._socketClosed)
                {
                    OnRaiseSocketErrorEvent(new SocketEventArgs("Failed to send message with error: " + err.Message));
                }
            }
        }

        public async void StartReader(ConnectedPeer connectedPeer)
        {
            try
            {
                using (var socketReader = new Windows.Storage.Streams.DataReader(connectedPeer._socket.InputStream))
                {
                    // Read the message sent by the remote peer
                    uint bytesRead = await socketReader.LoadAsync(sizeof(uint));
                    if (bytesRead > 0)
                    {
                        uint strLength = (uint)socketReader.ReadUInt32();
                        bytesRead = await socketReader.LoadAsync(strLength);
                        if (bytesRead > 0)
                        {
                            String message = socketReader.ReadString(strLength);
                            OnRaiseMessageEvent(new MessageEventArgs("Got message: " + message));
                            StartReader(connectedPeer); // Start another reader
                        }
                        else
                        {
                            OnRaiseSocketErrorEvent(new SocketEventArgs("The remote side closed the socket"));
                        }
                    }
                    else
                    {
                        OnRaiseSocketErrorEvent(new SocketEventArgs("The remote side closed the socket"));
                    }

                    socketReader.DetachStream();
                }

            }
            catch (Exception e)
            {
                if (!connectedPeer._socketClosed)
                {
                    OnRaiseSocketErrorEvent(new SocketEventArgs("Reading from socket failed: " + e.Message));
                }
            }
        }

        public void CloseSocket()
        {
            // Close all the established sockets.
            foreach (ConnectedPeer obj in _connectedPeers)
            {
                if (obj._socket != null)
                {
                    obj._socketClosed = true;
                    obj._socket.Dispose();
                    obj._socket = null;
                }

                if (obj._dataWriter != null)
                {
                    obj._dataWriter.Dispose();
                    obj._dataWriter = null;
                }
            }

            _connectedPeers.Clear();
        }

        protected virtual void OnRaiseSocketErrorEvent(SocketEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of 
            // a race condition if the last subscriber unsubscribes 
            // immediately after the null check and before the event is raised.
            EventHandler<SocketEventArgs> handler = RaiseSocketErrorEvent;

            // Event will be null if there are no subscribers 
            if (handler != null)
            {
                // Use the () operator to raise the event.
                handler(this, e);
            }
        }

        protected virtual void OnRaiseMessageEvent(MessageEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of 
            // a race condition if the last subscriber unsubscribes 
            // immediately after the null check and before the event is raised.
            EventHandler<MessageEventArgs> handler = RaiseMessageEvent;

            // Event will be null if there are no subscribers 
            if (handler != null)
            {
                // Use the () operator to raise the event.
                handler(this, e);
            }
        }

    }
}
