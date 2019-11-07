//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <dictionary>
///    rtt
/// </dictionary>

(function () {
    "use strict";

    var myClientControlObject = null;
    var connectButton = null;
    var serverName = null;
    var desktopWidth = 1024;
    var desktopHeight = 768;
    var eventsDisplayRegion = null;

    var page = WinJS.UI.Pages.define("/html/handle-events.html", {
        ready: function (element, options) {
            connectButton = document.getElementById("connectButton");
            connectButton.addEventListener("click", handleConnectButton, false);
        }
    });

    function handleConnectButton () {
        WinJS.log && WinJS.log("", "sample", "error");

        serverName = document.getElementById("serverName").value;
        if (!serverName) {
            WinJS.log && WinJS.log("Enter PC name to connect to.", "sample", "error");
            return;
        }

        eventsDisplayRegion = document.getElementById("eventsDisplayRegion");
        eventsDisplayRegion.innerText = "";

        // Toggle the connect button behavior.
        var connect = (connectButton.innerText === "Connect");
        connectButton.innerText = connect ? "Disconnect" : "Connect";

        if (connect) {
            // Create the client control, set connection properties and connect.
            myClientControlObject = Microsoft.Sample.RDPClient.createClientControl(desktopWidth, desktopHeight);
            Microsoft.Sample.RDPClient.setMinimalProperties(myClientControlObject, serverName, desktopWidth, desktopHeight);

            // Add event listeners 
            attachEventListeners();

            Microsoft.Sample.RDPClient.connectToRemotePC(myClientControlObject);
        } else {
            // Disconnect and release the client control.
            Microsoft.Sample.RDPClient.disconnectFromRemotePC(myClientControlObject);
            Microsoft.Sample.RDPClient.removeClientControl(myClientControlObject);
            myClientControlObject = null;
        }
    }

    function attachEventListeners () {
        myClientControlObject.attachEvent("OnConnecting", function () {
            displayEventNotification("OnConnecting: Attempting to establish connection to the remote PC.", true);
        });

        myClientControlObject.attachEvent("OnConnected", function () {
            displayEventNotification("OnConnected: Client control has connected to the remote PC.", true);
        });

        myClientControlObject.attachEvent("OnLoginCompleted", function () {
            displayEventNotification("OnLoginCompleted: Successfully logged on to the remote PC.", true);
        });

        myClientControlObject.attachEvent("OnDialogDisplaying", function () {
            displayEventNotification("OnDialogDisplaying: A dialog message is being displayed by the client control.", true);
        });

        myClientControlObject.attachEvent("OnDialogDismissed", function () {
            displayEventNotification("OnDialogDismissed: Dialog message shown by the client control is dismissed.", true);
        });

        myClientControlObject.attachEvent("OnDisconnected", function (disconnectReason, extendedDisconnectReason, disconnectErrorMessage) {
            displayEventNotification("OnDisconnected: Client control has been disconnected from the remote PC.", true);
            displayEventNotification("Disconnect reason: " + disconnectReason);
            displayEventNotification("Extended disconnect reason: " + extendedDisconnectReason);
            displayEventNotification("Disconnect error message: " + disconnectErrorMessage);

            // Remove the control and reset the connection button.
            Microsoft.Sample.RDPClient.removeClientControl(myClientControlObject);
            connectButton.innerText = "Connect";
        });

        myClientControlObject.attachEvent("OnNetworkBandwidthChanged", function (qualityInfo, bandwidth, rtt) {
            displayEventNotification("OnNetworkBandwidthChanged: Network bandwidth information has been changed.", true);
            displayEventNotification("Network bandwidth quality: " + translateQualityInfoCode(qualityInfo));
        });

        myClientControlObject.attachEvent("OnStatusChanged", function (statusCode, statusMessage) {
            displayEventNotification("OnStatusChanged: Connection status has been changed.", true);
            displayEventNotification("Status message: " + statusMessage);
        });

        myClientControlObject.attachEvent("OnRemoteDesktopSizeChanged", function (width, height) {
            displayEventNotification("OnRemoteDesktopSizeChanged: Remote PC's desktop size has been changed.", true);
            displayEventNotification("Width: " + width + ", Height: " + height);
        });
    }
    
    function translateQualityInfoCode (qualityInfo) {
        var qualityString = "Unknown";

        switch (qualityInfo) {
            case 1:
                qualityString = "Poor";
                break;
            case 2:
                qualityString = "Good";
                break;
            case 3:
                qualityString = "Good";
                break;
            case 4:
                qualityString = "Excellent";
                break;
        }

        return qualityString;
    }

    function displayEventNotification(message, extraLine) {
        var lineSeparator = (eventsDisplayRegion.innerText === "") ? "" : "\n";
        lineSeparator += (extraLine && lineSeparator !== "") ? "\n" : "";

        eventsDisplayRegion.innerText += lineSeparator + message;
    }

})();
