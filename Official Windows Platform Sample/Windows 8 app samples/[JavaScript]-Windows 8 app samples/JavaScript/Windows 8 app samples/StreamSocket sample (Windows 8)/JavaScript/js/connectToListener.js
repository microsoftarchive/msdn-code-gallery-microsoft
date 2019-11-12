//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/connectToListener.html", {
        ready: function (element, options) {
            document.getElementById("buttonOpen").addEventListener("click", openClient, false);
            document.getElementById("hostNameConnect").addEventListener("change", socketsSample.getValues, false);
            document.getElementById("serviceNameConnect").addEventListener("change", socketsSample.getValues, false);
            socketsSample.setValues();
        }
    });

    function openClient() {
        if (socketsSample.clientSocket) {
            socketsSample.displayStatus("Already have a client; call close to close the listener and the client.");
            return;
        }

        var serviceName = document.getElementById("serviceNameConnect").value;
        if (serviceName === "") {
            socketsSample.displayStatus("Please provide a service name.");
            return;
        }

        // By default 'hostNameConnect' is disabled and host name validation is not required. When enabling the text
        // box validating the host name is required since it was received from an untrusted source (user input).
        // The host name is validated by catching exceptions thrown by the HostName constructor.
        // Note that when enabling the text box users may provide names for hosts on the intErnet that require the
        // "Internet (Client)" capability.
        var hostName;
        try {
            hostName = new Windows.Networking.HostName(document.getElementById("hostNameConnect").value);
        } catch (error) {
            socketsSample.displayStatus("Error: Invalid host name.");
            return;
        }
        
        socketsSample.closing = false;
        socketsSample.clientSocket = new Windows.Networking.Sockets.StreamSocket();
        socketsSample.displayStatus("Client: connection started.");
        socketsSample.clientSocket.connectAsync(hostName, serviceName).done(function () {
            socketsSample.displayStatus("Client: connection completed.");
            socketsSample.connected = true;
        }, onError);
    }

    function onError(reason) {
        socketsSample.clientSocket = null;

        // When we close a socket, outstanding async operations will be canceled and the
        // error callbacks called.  There's no point in displaying those errors.
        if (!socketsSample.closing) {
            socketsSample.displayStatus(reason);
        }
    }
})();
