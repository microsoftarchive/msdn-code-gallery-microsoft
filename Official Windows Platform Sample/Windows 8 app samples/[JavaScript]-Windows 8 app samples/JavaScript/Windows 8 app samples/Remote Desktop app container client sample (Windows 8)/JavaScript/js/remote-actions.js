//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <dictionary>
///    appbar
/// </dictionary>

(function () {
    "use strict";

    var myClientControlObject = null;
    var connectButton = null;
    var charmsButton = null;
    var startButton = null;
    var serverName = null;
    var desktopWidth = 1024;
    var desktopHeight = 768;

    // Define possible action types to the ExecuteRemoteAction API.
    var remoteActions = {
        charms: 0,
        appbar: 1,
        snap: 2,
        start: 3,
        switch: 4
    };

    var page = WinJS.UI.Pages.define("/html/remote-actions.html", {
        ready: function (element, options) {
            connectButton = document.getElementById("connectButton");
            connectButton.addEventListener("click", handleConnectButton, false);

            charmsButton = document.getElementById("charmsButton");
            charmsButton.addEventListener("click", function () {
                sendRemoteAction(remoteActions.charms);
            }, false);

            startButton = document.getElementById("startButton");
            startButton.addEventListener("click", function () {
                sendRemoteAction(remoteActions.start);
            }, false);

        }
    });

    function handleConnectButton () {
        WinJS.log && WinJS.log("", "sample", "error");

        serverName = document.getElementById("serverName").value;
        if (!serverName) {
            WinJS.log && WinJS.log("Enter PC name to connect to.", "sample", "error");
            return;
        }

        // Toggle the connect button behavior.
        var connect = (connectButton.innerText === "Connect");
        connectButton.innerText = connect ? "Disconnect" : "Connect";

        if (connect) {
            // Create the client control, set connection properties and connect.
            myClientControlObject = Microsoft.Sample.RDPClient.createClientControl(desktopWidth, desktopHeight);
            Microsoft.Sample.RDPClient.setMinimalProperties(myClientControlObject, serverName, desktopWidth, desktopHeight);
            Microsoft.Sample.RDPClient.connectToRemotePC(myClientControlObject);
        } else {
            // Disconnect and release the client control.
            Microsoft.Sample.RDPClient.disconnectFromRemotePC(myClientControlObject);
            Microsoft.Sample.RDPClient.removeClientControl(myClientControlObject);
            myClientControlObject = null;
        }
    }

    function sendRemoteAction(actionType) {
        WinJS.log && WinJS.log("", "sample", "error");

        if (myClientControlObject) {
            try {
                myClientControlObject.actions.ExecuteRemoteAction(actionType);
            } catch (e) {
                WinJS.log && WinJS.log("Failed to send the remote action. Error: " + e.number + " " + e.description, "sample", "error");
            }
        } else {
            WinJS.log && WinJS.log("Connect to the remote PC before sending remote action.", "sample", "error");
        }
    }

})();
