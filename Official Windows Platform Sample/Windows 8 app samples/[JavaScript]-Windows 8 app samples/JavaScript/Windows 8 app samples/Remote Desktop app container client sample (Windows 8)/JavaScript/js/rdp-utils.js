//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    WinJS.Namespace.define("Microsoft.Sample.RDPClient", {

        createClientControl: function (desktopWidth, desktopHeight) {
            // Initialize RDP client control.
            // Create object element.
            var clientControlObject = document.createElement("object");

            // Register for ActiveX events.
            clientControlObject.addEventListener("readystatechange", function (e) {
                if (clientControlObject.readyState !== 4) {
                    WinJS.log && WinJS.log("Error: ActiveX control readyState is not 4. ReadyState: " + clientControlObject.readyState, "sample", "error");
                }
            }, false);

            clientControlObject.addEventListener("error", function (e) {
                WinJS.log && WinJS.log("Error in loading the ActiveX control", "sample", "error");
            }, false);

            // Set object's classid to RDP client control's s CLSID.
            clientControlObject.classid = "CLSID:EAB16C5D-EED1-4E95-868B-0FBA1B42C092";
            
            clientControlObject.id = "clientControl";

            // Set object's height and width.
            clientControlObject.width = desktopWidth;
            clientControlObject.height = desktopHeight;

            // Add the element to  DOM.
            var clientControlRegion = document.getElementById("clientControlRegion");
            clientControlRegion.appendChild(clientControlObject);

            return clientControlObject;
        },

        removeClientControl: function (clientControlObject) {
            var clientControlRegion = document.getElementById("clientControlRegion");
            try {
                clientControlObject && clientControlRegion && clientControlRegion.removeChild(clientControlObject);
            } catch (e) {
                WinJS.log && WinJS.log("Error while removing the client control. Error: " + e.number + " " + e.description, "sample", "error");
            }
        },

        connectToRemotePC: function (clientControlObject) {
            try {
                clientControlObject.Connect();
            } catch (e) {
                WinJS.log && WinJS.log("Connect call failed. Error: " + e.number + " " + e.description, "sample", "error");
            }
        },

        disconnectFromRemotePC: function (clientControlObject) {
            try {
                clientControlObject.Disconnect();
            } catch (e) {
                WinJS.log && WinJS.log("Disconnect call failed. Error: " + e.number + " " + e.description, "sample", "error");
            }
        },

        setMinimalProperties: function (clientControlObject, serverName, desktopWidth, desktopHeight) {
            try {
                // Set the server to connect to.
                clientControlObject.Settings.SetRdpProperty("Full Address", serverName);

                // Set the desktop resolution.
                clientControlObject.Settings.SetRdpProperty("DesktopWidth", desktopWidth);
                clientControlObject.Settings.SetRdpProperty("DesktopHeight", desktopHeight);

                // Do not show the publisher warning dialog.
                clientControlObject.Settings.setRdpProperty("ShowPublisherWarningDialog", false);
            } catch (e) {
                WinJS.log && WinJS.log("Setting properties failed. Error: " + e.number + " " + e.description, "sample", "error");
            }
        },

    });
})();
