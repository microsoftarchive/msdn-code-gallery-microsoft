//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var ProxNS = Windows.Networking.Proximity;

    var BindablePeer = WinJS.Class.define(
        function (peerInfo) {
            this.peerInfo = peerInfo;
            // The following are necessary for the binding to work. We can't define _getObservable on a peerInformation WinRT object
            this.id = peerInfo.id;
            this.displayName = peerInfo.displayName;
            this.discoveryData = { buffer: peerInfo.discoveryData };
        },
        {}
    );

    var SocketHelper = WinJS.Class.define(
        function (socketError) {
            this.connectedPeers = new Array();
            this.socketError = socketError;
        },
        {
            sendMessageToPeer: function (message, connectedPeer) {
                if (!connectedPeer.socketClosed) {
                    var dataWriter = connectedPeer.dataWriter;

                    var msgLength = dataWriter.measureString(message);
                    dataWriter.writeInt32(msgLength);
                    dataWriter.writeString(message);
                    var that = this;

                    dataWriter.storeAsync().done(
                        function (byteCount) {
                            if (byteCount > 0) {
                                ProximityHelpers.displayStatus("Sent message: " + message + ", number of bytes written: " + byteCount);
                            } else {
                                that.socketError("Remote side closed the socket");
                            }
                        },
                        function (err) {
                            if (!connectedPeer.socketClosed) {
                                that.socketError("Send completed error " + err.message);
                            }
                        });
                } else {
                    that.socketError("Remote side closed the socket");
                }
            },

            startReader: function (connectedPeer) {
                var socketReader = new Windows.Storage.Streams.DataReader(connectedPeer.socket.inputStream);

                // Read out and print the received message from the socket
                var initialLength = 4; // size of integer
                var that = this;

                // Read the message sent by the remote peer
                socketReader.loadAsync(initialLength).done(
                    function (lengthByteCount) {
                        if (lengthByteCount > 0) {
                            var strLength = socketReader.readInt32();
                            socketReader.loadAsync(strLength).done(
                                function (stringByteCount) {
                                    if (stringByteCount > 0) {
                                        var message;
                                        message = socketReader.readString(strLength);
                                        ProximityHelpers.displayStatus("Got message: " + message);
                                        that.startReader(connectedPeer);
                                    } else {
                                        // Successfully read 0 bytes, socket must have been closed.
                                        that.socketError("Remote side closed the socket");
                                        socketReader.close();
                                    }
                                },
                                function (err) {
                                    if (!connectedPeer.socketClosed) {
                                        this.socketError("Reading of message failed " + err);
                                    }
                                    socketReader.close();
                                });
                        } else {
                            // Successfully read 0 bytes, socket must have been closed.
                            that.socketError("Remote side closed the socket");
                            socketReader.close();
                        }
                    },
                    function (err) {
                        if (!connectedPeer.socketClosed) {
                            that.socketError("Reading of message length failed " + err);
                        }
                        socketReader.close();
                    });
            },

            closeSocket: function () {
                this.connectedPeers.forEach(function (connectedPeer) {
                    if (connectedPeer.socket) {
                        connectedPeer.socketClosed = true;
                        connectedPeer.socket.close();
                        connectedPeer.socket = null;
                    }

                    if (connectedPeer.dataWriter) {
                        connectedPeer.dataWriter.close();
                        connectedPeer.dataWriter = null;
                    }
                });
                this.connectedPeers = [];
            }
        });

    var rgConnectState = new Array();
    rgConnectState[ProxNS.TriggeredConnectState.peerFound] = "PeerFound";
    rgConnectState[ProxNS.TriggeredConnectState.listening] = "Listening";
    rgConnectState[ProxNS.TriggeredConnectState.connecting] = "Connecting";
    rgConnectState[ProxNS.TriggeredConnectState.completed] = "Completed";
    rgConnectState[ProxNS.TriggeredConnectState.canceled] = "Canceled";
    rgConnectState[ProxNS.TriggeredConnectState.failed] = "Failed";

    WinJS.Namespace.define("ProximitySockets", {
        connectedPeer: function ConnectedPeer(socket, socketClosed, dataWriter) {
            this.socket = socket;
            this.socketClosed = socketClosed;
            this.dataWriter = dataWriter;
        },
        
        bindablePeer: BindablePeer,

        bufferToString: WinJS.Binding.converter(function (data) {
            if (!data || !data.buffer) {
                return "";
            }
            var discoveryDataReader = Windows.Storage.Streams.DataReader.fromBuffer(data.buffer);
            var discoveryData = "(" + discoveryDataReader.readString(data.buffer.length) + ")";
            return discoveryData;
        }),

        getConnectState: function (state) {
            return rgConnectState[state];
        },

        socketHelper: SocketHelper
    });

})();
