//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

/// <dictionary>
///    png
/// </dictionary>

(function () {
    "use strict";

    var myClientControlObject = null;
    var connectButton = null;
    var snapshotButton = null;
    var snapshotImageElement = null;
    var serverName = null;
    var desktopWidth = 1024;
    var desktopHeight = 768;

    // Define possible encoding types to the GetSnapshot API.
    var snapshotEncodingType = {
        dataUri: 0,
    };

    // Define possible snapshot formats to the GetSnapshot API.
    var snapshotFormatType = {
        png: 0,
        jpeg: 1,
        bmp: 2,
    };

    var page = WinJS.UI.Pages.define("/html/take-snapshot.html", {
        ready: function (element, options) {
            connectButton = document.getElementById("connectButton");
            connectButton.addEventListener("click", handleConnectButton, false);

            snapshotButton = document.getElementById("snapshotButton");
            snapshotButton.addEventListener("click", takeSnapshot, false);
        }
    });

    function handleConnectButton () {
        WinJS.log && WinJS.log("", "sample", "error");

        serverName = document.getElementById("serverName").value;
        if (!serverName) {
            WinJS.log && WinJS.log("Enter PC name to connect to.", "sample", "error");
            return;
        }
        snapshotImageElement = document.getElementById("snapshotImage");

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
            snapshotImageElement.src = "";
            myClientControlObject = null;
        }
    }

    function takeSnapshot() {
        WinJS.log && WinJS.log("", "sample", "error");

        if (myClientControlObject) {
            // Define the desired snapshot dimension. In this sample the aspect ratio is set to match a 1024x768 display.
            var snapshotDimensions = {
                width: 250,
                height: 188,
            };

            try {
                // Get a snapshot
                var snapshotUri = myClientControlObject.Actions.GetSnapshot(snapshotEncodingType.dataUri, snapshotFormatType.png,
                        snapshotDimensions.width, snapshotDimensions.height);

                // Set the image element's src to the URI received from the client control
                snapshotImageElement.src = snapshotUri;
            } catch (e) {
                WinJS.log && WinJS.log("Failed to get the Snapshot. Error: " + e.number + " " + e.description, "sample", "error");
            }
        } else {
            WinJS.log && WinJS.log("Connect to the remote PC before getting the snapshot.", "sample", "error");
        }
    }

})();
