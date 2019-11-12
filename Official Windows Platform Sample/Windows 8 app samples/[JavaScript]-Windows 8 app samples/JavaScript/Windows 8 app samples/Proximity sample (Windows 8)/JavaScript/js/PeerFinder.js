//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var ProxNS = Windows.Networking.Proximity;
    var discoveredPeers;      // List of peerinformation objects returned by findAllPeersAsync
    var socket;               // Socket is stored here so that the send button event handler can access it
    var socketClosed = true;
    var requestingPeer;       // Peer that requested the connection
    var dataWriter;           // Data writer for the output stream of the socket
    var triggeredConnectSupported = false;
    var browseConnectSupported = false;
    var peerFinderStarted = false;
    
    //connection states
    var rgConnectState = new Array();
    rgConnectState[ProxNS.TriggeredConnectState.peerFound] = "PeerFound";
    rgConnectState[ProxNS.TriggeredConnectState.listening] = "Listening";
    rgConnectState[ProxNS.TriggeredConnectState.connecting] = "Connecting";
    rgConnectState[ProxNS.TriggeredConnectState.completed] = "Completed";
    rgConnectState[ProxNS.TriggeredConnectState.canceled] = "Canceled";
    rgConnectState[ProxNS.TriggeredConnectState.failed] = "Failed";

    function triggeredConnectionStateChangedEventHandler(e) {
        ProximityHelpers.logInfo(ProximityHelpers.id("peerFinderOutput"), "triggeredConnectionStateChangedEventHandler - " +
                    rgConnectState[e.state]);

        if (e.state === ProxNS.TriggeredConnectState.peerFound) {
            ProximityHelpers.clearLastError();
            // Use this state to indicate to users that the tap is complete and
            // they can pull there devices away.
            ProximityHelpers.displayError("Tap complete, socket connection starting!");
        }

        if (e.state === ProxNS.TriggeredConnectState.completed) {
            ProximityHelpers.displayStatus("Socket connect success!");
            // Grab and use the socket that just connected.
            startSendReceive(e.socket);
        }

        if (e.state === ProxNS.TriggeredConnectState.failed) {
            ProximityHelpers.displayError("Socket connect failed!");
        }
    }

    function connectionRequestedEventHandler(e) {
        requestingPeer = e.peerInformation;
        ProximityHelpers.displayStatus("Connection Requested from peer: " + requestingPeer.displayName);
        ProximityHelpers.id("peerFinder_AcceptRequest").style.display = "inline";
        ProximityHelpers.id("peerFinder_Send").style.display = "none";
        ProximityHelpers.id("peerFinder_Message").style.display = "none";
    }

    function peerFinder_AcceptRequest() {
        // Accept the connection if the user clicks okay.
        ProximityHelpers.displayStatus("Connecting to " + requestingPeer.displayName + " ...");
        ProximityHelpers.id("peerFinder_AcceptRequest").style.display = "none";
        ProxNS.PeerFinder.connectAsync(requestingPeer).done(
            function (proximitySocket) {
                ProximityHelpers.displayStatus("Connect to " + requestingPeer.displayName + " succeeded");
                startSendReceive(proximitySocket);
            },
            function (err) {
                ProximityHelpers.displayError("Connect to " + requestingPeer.displayName + " failed with " + err);
                ProximityHelpers.id("peerFinder_Connect").style.display = "none";
            });
    }

    function socketError(errMessage) {
        ProximityHelpers.displayError(errMessage);
        ProximityHelpers.id("peerFinder_StartFindingPeers").style.display = "inline";
        if (browseConnectSupported) {
            ProximityHelpers.id("peerFinder_BrowsePeers").style.display = "inline";
        }
        ProximityHelpers.id("peerFinder_Send").style.display = "none";
        ProximityHelpers.id("peerFinder_Message").style.display = "none";
        closeSocket();
    }

    function startReader(socketReader) {
        // Read out and print the received message from the socket
        var initialLength = 4; // size of integer
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
                                startReader(socketReader);
                            } else {
                                // Successfully read 0 bytes, socket must have been closed.
                                socketError("Remote side closed the socket");
                                socketReader.close();
                            }
                        },
                        function (err) {
                            if (!socketClosed) {
                                socketError("Reading of message failed " + err);
                            }
                            socketReader.close();
                        });
                } else {
                    // Successfully read 0 bytes, socket must have been closed.
                    socketError("Remote side closed the socket");
                    socketReader.close();
                }
            },
            function (err) {
                if (!socketClosed) {
                    socketError("Reading of message length failed " + err);
                }
                socketReader.close();
            });
    }


    function peerFinder_StartFindingPeers() {
        if (!peerFinderStarted) {
            // Then start listening for proximate peers
            ProxNS.PeerFinder.start();
            peerFinderStarted = true;
        }

        if (browseConnectSupported && triggeredConnectSupported) {
            ProximityHelpers.displayStatus("Tap another device to connect to a peer or click Browse for Peers button.");
            ProximityHelpers.id("peerFinder_BrowsePeers").style.display = "inline";
        } else if (triggeredConnectSupported) {
            ProximityHelpers.displayStatus("Tap another device to connect to a peer.");
        } else if (browseConnectSupported) {
            ProximityHelpers.id("peerFinder_BrowsePeers").style.display = "inline";
            ProximityHelpers.displayStatus("Click browse for peers button");
        }
    }

    function peerFinder_BrowsePeers() {
        ProximityHelpers.displayStatus("Finding Peers ...");
        var foundPeersList = ProximityHelpers.id("peerFinder_FoundPeersList");

        // Empty the current option list
        for (var i = foundPeersList.options.length; i >= 0; i--) {
            foundPeersList.options[i] = null;
        }

        ProxNS.PeerFinder.findAllPeersAsync().done(
            function (peerInfoCollection) {
                var statusMessage = "Found " + peerInfoCollection.size + " peers";
                ProximityHelpers.displayStatus();
                var optionElement;
                var textElement;
                // add newly found peers into the drop down list.
                for (i = 0; i < peerInfoCollection.size; i++) {
                    var peerInformation = peerInfoCollection[i];
                    optionElement = document.createElement("option");
                    optionElement.setAttribute("value", i + 1);
                    textElement = document.createTextNode(peerInformation.displayName);
                    optionElement.appendChild(textElement);
                    foundPeersList.appendChild(optionElement);
                    statusMessage += "\n" + peerInformation.displayName;
                }
                ProximityHelpers.id("peerFinder_FoundPeersList").style.display = "inline";
                if (peerInfoCollection.size === 0) {
                    optionElement = document.createElement("option");
                    optionElement.setAttribute("value", 1);
                    textElement = document.createTextNode("None Found");
                    optionElement.appendChild(textElement);
                    foundPeersList.appendChild(optionElement);
                    ProximityHelpers.id("peerFinder_Connect").style.display = "none";
                } else {
                    ProximityHelpers.id("peerFinder_Connect").style.display = "inline";
                }

                ProximityHelpers.displayStatus(statusMessage);
                discoveredPeers = peerInfoCollection;
            },
            function (err) {
                ProximityHelpers.displayError(err);
            });
    }

    function startSendReceive(proximitySocket) {
        socket = proximitySocket;
        // If the scenario was switched just as the socket connection completed, just close the socket.
        if (!peerFinderStarted) {
            closeSocket();
            return;
        }
        socketClosed = false;
        dataWriter = new Windows.Storage.Streams.DataWriter(socket.outputStream);

        // show the send button
        ProximityHelpers.id("peerFinder_Send").style.display = "inline";
        ProximityHelpers.id("peerFinder_Message").style.display = "inline";

        // Hide the connect button
        ProximityHelpers.id("peerFinder_Connect").style.display = "none";
        ProximityHelpers.id("peerFinder_FoundPeersList").style.display = "none";
        ProximityHelpers.id("peerFinder_AcceptRequest").style.display = "none";
        ProximityHelpers.id("peerFinder_BrowsePeers").style.display = "none";
        ProximityHelpers.id("peerFinder_StartFindingPeers").style.display = "none";

        var dataReader = new Windows.Storage.Streams.DataReader(proximitySocket.inputStream);
        startReader(dataReader);
    }

    function peerFinder_Connect() {
        var foundPeersList = ProximityHelpers.id("peerFinder_FoundPeersList");
        var peerToConnect = discoveredPeers[foundPeersList.selectedIndex];
        ProximityHelpers.displayStatus("Connecting to " + peerToConnect.displayName + " ...");
        ProxNS.PeerFinder.connectAsync(peerToConnect).done(
            function (proximitySocket) {
                ProximityHelpers.displayStatus("Connect succeeded");
                startSendReceive(proximitySocket);
            },
            function (err) {
                ProximityHelpers.id("peerFinder_Connect").style.display = "none";
                ProximityHelpers.id("peerFinder_FoundPeersList").style.display = "none";
                ProximityHelpers.displayError("Connect failed with " + err);
            });
    }

    function peerFinder_Send() {
        ProximityHelpers.displayError("");
        var msg = ProximityHelpers.id("peerFinder_Message");

        var string = msg.value;
        msg.value = ""; // clear the message box after sending it.
        string.trim();
        if (!socketClosed) {
            if (string.length > 0) {
                var strLength = dataWriter.measureString(string);
                dataWriter.writeInt32(strLength);
                dataWriter.writeString(string);
                dataWriter.storeAsync().done(
                    function (byteCount) {
                        if (byteCount > 0) {
                            ProximityHelpers.displayStatus("Sent message: " + string + ", number of bytes written: " + byteCount);
                        } else {
                            socketError("Remote side closed the socket");
                        }
                    },
                    function (err) {
                        if (!socketClosed) {
                            socketError("Send completed error " + err.message);
                        }
                    });
            } else {
                ProximityHelpers.displayError("Please enter a message");
            }
        } else {
            socketError("Remote side closed the socket");
        }
    }

    function closeSocket() {
        if (socket) {
            socketClosed = true;
            socket.close();
            socket = null;
        }

        if (dataWriter) {
            dataWriter.close();
            dataWriter = null;
        }
    }

    var page = WinJS.UI.Pages.define("/html/PeerFinder.html", {
        ready: function (element, options) {
            var peerFinderErrorMsg = null;
            var supportedDiscoveryTypes = ProxNS.PeerFinder.supportedDiscoveryTypes;

            // First attach the event handlers (there can only be one handler for each event).
            ProxNS.PeerFinder.ontriggeredconnectionstatechanged = triggeredConnectionStateChangedEventHandler;
            ProxNS.PeerFinder.onconnectionrequested = connectionRequestedEventHandler;

            // Enable triggered related buttons only if the hardware support is present
            if (supportedDiscoveryTypes & ProxNS.PeerDiscoveryTypes.triggered) {
                triggeredConnectSupported = true;
            } else {
                peerFinderErrorMsg = "Tap based discovery of peers not supported \n";
            }

            // Enable browse related buttons only if the hardware support is present
            if (supportedDiscoveryTypes & ProxNS.PeerDiscoveryTypes.browse) {
                browseConnectSupported = true;
                ProximityHelpers.id("peerFinder_AcceptRequest").addEventListener("click", peerFinder_AcceptRequest, false);
                ProximityHelpers.id("peerFinder_BrowsePeers").addEventListener("click", peerFinder_BrowsePeers, false);
                ProximityHelpers.id("peerFinder_Connect").addEventListener("click", peerFinder_Connect, false);
            } else {
                if (peerFinderErrorMsg) {
                    peerFinderErrorMsg += "Browsing for peers not supported";
                } else {
                    peerFinderErrorMsg = "Browsing for peers not supported";
                }
            }

            if (triggeredConnectSupported || browseConnectSupported) {
                ProximityHelpers.id("peerFinder_StartFindingPeers").addEventListener("click", peerFinder_StartFindingPeers, false);
                ProximityHelpers.id("peerFinder_StartFindingPeers").style.display = "inline";
                ProximityHelpers.id("peerFinder_Send").addEventListener("click", peerFinder_Send, false);
            }

            if (peerFinderErrorMsg) {
                ProximityHelpers.displayError(peerFinderErrorMsg);
            }

            if (options === true) {
                // If this sample app was launched by a proximity tap, start the peerfinder
                // right away to get connected.
                peerFinder_StartFindingPeers();
            }
        },

        unload: function () {
            if (peerFinderStarted) {
                ProxNS.PeerFinder.stop();
                closeSocket();
                peerFinderStarted = false;
            }
        }
    });


})();
