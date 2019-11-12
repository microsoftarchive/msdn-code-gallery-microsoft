//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var ProxNS = Windows.Networking.Proximity;
    var peerWatcher;
    var peerWatcherIsRunning = false;
    var peerListBinding;      // Data source for binding to the list of peers found
    var connectedPeers = new Array(); // List of connected peers
    var requestingPeer;
    var triggeredConnectSupported = false;
    var browseConnectSupported = false;
    var peerFinderStarted = false;
    var role = ProxNS.PeerRole.peer;

    // Helpers to update the UI state

    function hideAllControls() {
        ProximityHelpers.id("peerFinder_StartAdvertising").style.display = "none";
        ProximityHelpers.id("peerFinder_StopAdvertising").style.display = "none";

        ProximityHelpers.id("peerFinder_BrowseDiv").style.display = "none";
        ProximityHelpers.id("peerFinder_StopPeerWatcher").style.display = "none";
        ProximityHelpers.id("peerFinder_StartPeerWatcher").style.display = "none";
        ProximityHelpers.id("peerFinder_Connect").style.display = "none";

        ProximityHelpers.id("peerFinder_ConnectionDiv").style.display = "none";
        ProximityHelpers.id("peerFinder_AcceptRequest").style.display = "none";
        ProximityHelpers.id("peerFinder_Send").style.display = "none";
        ProximityHelpers.id("peerFinder_Message").style.display = "none";
        ProximityHelpers.id("peerFinder_SendToPeerList").style.display = "none";
        ProximityHelpers.id("peerFinder_PeerListNoPeers").style.display = "none";

        ProximityHelpers.id("peerFinder_SelectRole").style.display = "none";
        ProximityHelpers.id("peerFinder_DiscoveryData").style.display = "none";
    }

    function hideAllControlGroups() {
        ProximityHelpers.id("peerFinder_BrowseDiv").style.display = "none";
        ProximityHelpers.id("peerFinder_ConnectionDiv").style.display = "none";
        ProximityHelpers.id("peerFinder_AdvertiseDiv").style.display = "none";
    }

    function showSendOrAcceptControls(send) {
        var sendDisplay = (send) ? "inline" : "none";
        var acceptDisplay = (!send) ? "inline" : "none";

        ProximityHelpers.id("peerFinder_ConnectionDiv").style.display = "block";

        ProximityHelpers.id("peerFinder_Send").style.display = sendDisplay;
        ProximityHelpers.id("peerFinder_Message").style.display = sendDisplay;
        ProximityHelpers.id("peerFinder_SendToPeerList").style.display = sendDisplay;

        ProximityHelpers.id("peerFinder_AcceptRequest").style.display = acceptDisplay;
    }

    function showAdvertiseControls() {
        ProximityHelpers.id("peerFinder_AdvertiseDiv").style.display = "block";
        ProximityHelpers.id("peerFinder_StartAdvertising").style.display = "inline";
        ProximityHelpers.id("peerFinder_StopAdvertising").style.display = "none";

        ProximityHelpers.id("peerFinder_SelectRole").style.display = "inline";

        if (browseConnectSupported) {
            ProximityHelpers.id("peerFinder_DiscoveryData").style.display = "inline";
            ProximityHelpers.id("peerFinder_StartPeerWatcher").style.display = "inline";
        }

        ProximityHelpers.id("peerFinder_ConnectionDiv").style.display = "none";
    }

    function toggleWatcherControls(show) {
        var display = (show) ? "inline" : "none";
        var displayBlock = (show) ? "block" : "none";
        var displayAdvertise = (!show) ? "inline" : "none";

        ProximityHelpers.id("peerFinder_StartAdvertising").style.display = displayAdvertise;
        ProximityHelpers.id("peerFinder_StopAdvertising").style.display = display;
        if (browseConnectSupported) {
            ProximityHelpers.id("peerFinder_BrowseDiv").style.display = displayBlock;
        }
    }

    function showPeerAddedControls() {
        ProximityHelpers.id("peerFinder_Connect").style.display = "inline";
        ProximityHelpers.id("peerFinder_PeerListNoPeers").style.display = "none";
    }

    function togglePeerWatcherStartControls(running) {
        ProximityHelpers.id("peerFinder_StopPeerWatcher").style.display = (running) ? "inline" : "none";
        ProximityHelpers.id("peerFinder_StartPeerWatcher").style.display = (running) ? "none" : "inline";
    }

    function showStartPeerWatcherControls() {
        ProximityHelpers.id("peerFinder_PeerListNoPeers").style.display = "none";
        ProximityHelpers.id("peerFinder_Connect").style.display = "none";
        togglePeerWatcherStartControls(true);
    }

    // Handles PeerFinder.ontriggeredconnectionstatechanged
    function triggeredConnectionStateChangedEventHandler(e) {
        ProximityHelpers.logInfo(ProximityHelpers.id("peerFinderOutput"), "triggeredConnectionStateChangedEventHandler - " +
                    ProximitySockets.getConnectState(e.state));

        if (e.state === ProxNS.TriggeredConnectState.peerFound) {
            // Use this state to indicate to users that the tap is complete and
            // they can pull there devices away.
            ProximityHelpers.displayError("Tap complete, socket connection starting!");
        }

        if (e.state === ProxNS.TriggeredConnectState.completed) {
            ProximityHelpers.displayStatus("Socket connect success!");
            // Grab and use the socket that just connected.
            startSendReceive(e.socket, null);
        }

        // The socket conenction failed
        if (e.state === ProxNS.TriggeredConnectState.failed) {
            ProximityHelpers.displayError("Socket connect failed!");
        }
    }

    function socketError(errMessage) {
        ProximityHelpers.displayError(errMessage);
        
        showAdvertiseControls();

        // Clear the SendToPeerList
        ProximityHelpers.id("peerFinder_SendToPeerList").style.display = "none";
        var sendToPeerList = ProximityHelpers.id("peerFinder_SendToPeerList");
        for (var i = sendToPeerList.options.length; i > 0; i--) {
            sendToPeerList.options[i] = null;
        }

        socketHelper.closeSocket();
    }

    var socketHelper = new ProximitySockets.socketHelper(socketError);

    // This gets called when we receive a connect request from a Peer
    function connectionRequestedEventHandler(e) {
        requestingPeer = e.peerInformation;
        ProximityHelpers.displayStatus("Connection Requested from peer: " + requestingPeer.displayName);

        hideAllControlGroups();
        showSendOrAcceptControls(false);
    }

    function peerFinder_AcceptRequest() {
        // Accept the connection if the user clicks okay.
        ProximityHelpers.displayStatus("Connecting to " + requestingPeer.displayName + " ...");
        ProximityHelpers.id("peerFinder_AcceptRequest").style.display = "none";

        // Connect to the incoming peer
        ProxNS.PeerFinder.connectAsync(requestingPeer).done(
            function (proximitySocket) {
                ProximityHelpers.displayStatus("Connect to " + requestingPeer.displayName + " succeeded");
                startSendReceive(proximitySocket, requestingPeer);
            },
            function (err) {
                ProximityHelpers.displayError("Connect to " + requestingPeer.displayName + " failed with " + err);
            });
    }

    function peerFinder_StartAdvertising() {
        // If PeerFinder is started, stop it, so that new properties
        // selected by the user (Role/DiscoveryData) can be updated.
        peerFinder_StopAdvertising();

        if (!peerFinderStarted) {
            // Set the PeerFinder.role property
            // NOTE: this has no effect on the Phone platform

            switch (ProximityHelpers.id("peerFinder_SelectRole").value) {
                case "peer":
                    ProxNS.PeerFinder.role = ProxNS.PeerRole.peer;
                    break;
                case "host":
                    ProxNS.PeerFinder.role = ProxNS.PeerRole.host;
                    break;
                case "client":
                    ProxNS.PeerFinder.role = ProxNS.PeerRole.client;
                    break;
            }

            // Set discoveryData property if the user entered some text
            // NOTE: this has no effect on the Phone platform
            if (ProximityHelpers.id("peerFinder_DiscoveryData").value !== "What's happening today?") {
                var discoveryDataWriter = Windows.Storage.Streams.DataWriter(new Windows.Storage.Streams.InMemoryRandomAccessStream());
                discoveryDataWriter.writeString(ProximityHelpers.id("peerFinder_DiscoveryData").value);
                ProxNS.PeerFinder.discoveryData = discoveryDataWriter.detachBuffer();
            }

            // Then start listening for proximate peers
            ProxNS.PeerFinder.start();
            peerFinderStarted = true;
            ProximityHelpers.id("peerFinder_StopAdvertising").style.display = "inline";
        }

        toggleWatcherControls(true);
        if (browseConnectSupported && triggeredConnectSupported) {
            ProximityHelpers.displayStatus("Click Browse for Peers button or tap another device to connect to a peer.");
        } else if (triggeredConnectSupported) {
            ProximityHelpers.displayStatus("Tap another device to connect to a peer.");
        } else if (browseConnectSupported) {
            ProximityHelpers.displayStatus("Click browse for peers button");
        }
    }

    function peerFinder_StopAdvertising() {
        if (peerFinderStarted) {
            ProxNS.PeerFinder.stop();

            peerFinderStarted = false;
            ProximityHelpers.displayStatus("Stopped Advertising");
            toggleWatcherControls(false);
        }
    }

    function peerFinder_StartPeerWatcher() {
        if (peerWatcherIsRunning) {
            ProximityHelpers.displayStatus("Can't start PeerWatcher while it is running!");
            return;
        }

        ProximityHelpers.displayStatus("Starting PeerWatcher...");
        try {
            if (!peerWatcher) {
                peerWatcher = ProxNS.PeerFinder.createWatcher();
                // Hook up events
                peerWatcher.addEventListener("added", peerWatcher_Added);
                peerWatcher.addEventListener("removed", peerWatcher_Removed);
                peerWatcher.addEventListener("updated",  peerWatcher_Updated);
                peerWatcher.addEventListener("enumerationcompleted", peerWatcher_EnumerationCompleted);
                peerWatcher.addEventListener("stopped", peerWatcher_Stopped);
            }

            // Empty the current list
            while (peerListBinding.length > 0) {
                peerListBinding.pop();
            }

            peerWatcher.start();
            peerWatcherIsRunning = true;
            ProximityHelpers.displayStatus("PeerWatcher is running");

            ProximityHelpers.logInfo(ProximityHelpers.id("peerFinderOutput"), "PeerWatcher is running!");
            showStartPeerWatcherControls();
        } catch (e) {
            ProximityHelpers.displayError("PeerWatcher.Start throws an exception: " + e.message);
        }
    }

    function peerFinder_StopPeerWatcher() {
        ProximityHelpers.logInfo(ProximityHelpers.id("peerFinderOutput"), "Stopping PeerWatcher... wait for stopped event");
        try {
            peerWatcher.stop();
        } catch (e) {
            ProximityHelpers.displayError("PeerWatcher.Stop throws an exception: " + e.message);
        }
    }

    function getTruncatedPeerId(id) {
        var truncated = id;
        if (truncated.length > 10) {
            truncated = id.substr(0, 5) + "..." + id.substr(id.length - 5);
        }
        return truncated;
    }

    // PeerWatcher events

    function peerWatcher_Added(peerInfo) {
        ProximityHelpers.logInfo(ProximityHelpers.id("peerFinderOutput"), "Peer added: " + getTruncatedPeerId(peerInfo.id) + ", name: " + peerInfo.displayName);
        showPeerAddedControls();

        peerListBinding.push(new ProximitySockets.bindablePeer(peerInfo));
    }

    function peerWatcher_Removed(peerInfo) {
        ProximityHelpers.logInfo(ProximityHelpers.id("peerFinderOutput"), "Peer removed: " + getTruncatedPeerId(peerInfo.id) + ", name: " + peerInfo.displayName);

        var match = null;
        var index;
        peerListBinding.forEach(function (e, i) { if (e.id === peerInfo.id) { match = e; index = i; } });
        if (match) {
            peerListBinding.splice(index, 1);
        }
    }

    function peerWatcher_Updated(peerInfo) {
        ProximityHelpers.logInfo(ProximityHelpers.id("peerFinderOutput"), "Peer updated: " + getTruncatedPeerId(peerInfo.id) + ", name: " + peerInfo.displayName);

        var match = null;
        var index;
        peerListBinding.forEach(function (e, i) { if (e.id === peerInfo.id) { match = e; index = i; } });
        if (match) {
            peerListBinding.setAt(index, new ProximitySockets.bindablePeer(peerInfo));
        }
    }

    function peerWatcher_EnumerationCompleted(o) {
        ProximityHelpers.logInfo(ProximityHelpers.id("peerFinderOutput"), "PeerWatcher Enumeration Completed");
        // All peers that were visible at the start of the scan have been found
        // Stopping PeerWatcher here is similar to FindAllPeersAsync

        // Notify the user that no peers were found after we have done an initial scan
        if (peerListBinding.length === 0) {
            ProximityHelpers.id("peerFinder_PeerListNoPeers").style.display = "block";
        }
    }

    function peerWatcher_Stopped(o) {
        // This indicates that the PeerWatcher was stopped explicitly through PeerWatcher.Stop, or it was aborted
        // The Status property indicates the cause of the event
        ProximityHelpers.logInfo(ProximityHelpers.id("peerFinderOutput"), "PeerWatcher Stopped. Status: " + peerWatcher.status);
        ProximityHelpers.displayStatus("PeerWatcher Stopped");
        // PeerWatcher is now actually stopped and we can start it again, update the UI button state accordingly
        peerWatcherIsRunning = false;
        togglePeerWatcherStartControls(false);
    }

    function startSendReceive(proximitySocket, peerInformation) {
        var index = socketHelper.connectedPeers.length;
        socketHelper.connectedPeers[index] = new ProximitySockets.connectedPeer(proximitySocket, false, new Windows.Storage.Streams.DataWriter(proximitySocket.outputStream));

        if (!peerFinderStarted) {
            socketHelper.closeSocket();
            return;
        }

        // show the send button and the list of peers to send to
        hideAllControlGroups();
        showSendOrAcceptControls(true);

        if (peerInformation) {
            // Add a new peer to the list of peers.
            var optionElement = document.createElement("option");
            optionElement.setAttribute("value", index);
            var textElement = document.createTextNode(peerInformation.displayName);
            optionElement.appendChild(textElement);
            ProximityHelpers.id("peerFinder_SendToPeerList").appendChild(optionElement);
        }

        socketHelper.startReader(socketHelper.connectedPeers[index]);
    }

    function peerFinder_Connect() {
        var foundPeersList = ProximityHelpers.id("peerWatcher_FoundPeersList").winControl;
        
        if (peerListBinding.length === 0) {
            ProximityHelpers.displayError("Cannot connect, there were no peers found!");
        } else {
            var peerToConnect;
            // Get selected item, assume we can only have one item selected or none
            if (foundPeersList.selection.count() > 0) {
                foundPeersList.selection.getItems().done(function (items) {
                    peerToConnect = items[0].data.peerInfo;
                });
            } else {
                peerToConnect = peerListBinding.getAt(0).peerInfo;
            }
            
            ProximityHelpers.displayStatus("Connecting to " + peerToConnect.displayName + " ...");
            ProxNS.PeerFinder.connectAsync(peerToConnect).done(
                function (proximitySocket) {
                    ProximityHelpers.displayStatus("Connect succeeded");
                    startSendReceive(proximitySocket, peerToConnect);
                },
                function (err) {
                    ProximityHelpers.displayError("Connect failed with " + err);
                });
        }
    }

    // Send message to the selected peer(s)
    function peerFinder_Send() {
        ProximityHelpers.displayError("");
        var msg = ProximityHelpers.id("peerFinder_Message");
        var sendToPeerList = ProximityHelpers.id("peerFinder_SendToPeerList");

        var string = msg.value;
        msg.value = ""; // clear the message box after sending it.
        string.trim();

        if (string.length > 0) {
            // Send message to all peers
            if (sendToPeerList.value === "all") {
                for (var i in socketHelper.connectedPeers) {
                    socketHelper.sendMessageToPeer(string, socketHelper.connectedPeers[i]);
                }
            } else if (sendToPeerList.value >= 0 && sendToPeerList.value < socketHelper.connectedPeers.length) {
                // Send message to selected peer
                socketHelper.sendMessageToPeer(string, socketHelper.connectedPeers[sendToPeerList.value]);
            }
        } else {
            ProximityHelpers.displayError("Please enter a message");
        }
    }

    var page = WinJS.UI.Pages.define("/html/Scenario2_PeerWatcher.html", {
        ready: function (element, options) {
            var peerFinderErrorMsg = null;
            var supportedDiscoveryTypes = ProxNS.PeerFinder.supportedDiscoveryTypes;

            // First attach the event handlers (there can only be one handler for each event).
            ProxNS.PeerFinder.ontriggeredconnectionstatechanged = triggeredConnectionStateChangedEventHandler;
            ProxNS.PeerFinder.onconnectionrequested = connectionRequestedEventHandler;

            // Enable triggered related buttons only if the hardware support is present
            if (supportedDiscoveryTypes & ProxNS.PeerDiscoveryTypes.triggered) {
                triggeredConnectSupported = true;
            }

            // Enable browse related buttons only if the hardware support is present
            if (supportedDiscoveryTypes & ProxNS.PeerDiscoveryTypes.browse) {
                browseConnectSupported = true;
                ProximityHelpers.id("peerFinder_AcceptRequest").addEventListener("click", peerFinder_AcceptRequest, false);
                ProximityHelpers.id("peerFinder_StartPeerWatcher").addEventListener("click", peerFinder_StartPeerWatcher, false);
                ProximityHelpers.id("peerFinder_StopPeerWatcher").addEventListener("click", peerFinder_StopPeerWatcher, false);
                ProximityHelpers.id("peerFinder_Connect").addEventListener("click", peerFinder_Connect, false);
            } else {
                peerFinderErrorMsg = "Browsing for peers not supported";
            }

            hideAllControls();

            if (triggeredConnectSupported || browseConnectSupported) {
                ProximityHelpers.id("peerFinder_StartAdvertising").addEventListener("click", peerFinder_StartAdvertising, false);
                ProximityHelpers.id("peerFinder_StopAdvertising").addEventListener("click", peerFinder_StopAdvertising, false);
                ProximityHelpers.id("peerFinder_Send").addEventListener("click", peerFinder_Send, false);
                
                showAdvertiseControls();

                var peerList = ProximityHelpers.id("peerWatcher_FoundPeersList").winControl;
                peerListBinding = new WinJS.Binding.List([]);

                peerList.itemDataSource = peerListBinding.dataSource;
                peerList.forceLayout();
            }

            if (peerFinderErrorMsg) {
                ProximityHelpers.displayError(peerFinderErrorMsg);
            }
        },

        unload: function () {
            if (peerFinderStarted) {
                ProxNS.PeerFinder.ontriggeredconnectionstatechanged = null;
                ProxNS.PeerFinder.onconnectionrequested = null;
                ProxNS.PeerFinder.stop();
                socketHelper.closeSocket();
                peerFinderStarted = false;
            }
            if (peerWatcher) {
                peerWatcher.removeEventListener("added", peerWatcher_Added);
                peerWatcher.removeEventListener("removed", peerWatcher_Removed);
                peerWatcher.removeEventListener("updated", peerWatcher_Updated);
                peerWatcher.removeEventListener("enumerationCompleted", peerWatcher_EnumerationCompleted);
                peerWatcher.removeEventListener("stopped", peerWatcher_Stopped);

                peerWatcher = null;
            }
        }
    });


})();
