//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/startListener.html", {
        ready: function (element, options) {
            document.getElementById("buttonStartListener").addEventListener("click", startListener, false);
            document.getElementById("serviceNameAccept").addEventListener("change", socketsSample.getValues, false);
            socketsSample.setValues();
        }
    });

    function startListener() {
        var serviceName = document.getElementById("serviceNameAccept").value;
        if (serviceName === "") {
            socketsSample.displayStatus("Please provide a service name.");
            return;
        }
            
        if (socketsSample.listener) {
            socketsSample.displayStatus("Already have a listener; call close to close the listener.");
            return;
        }

        socketsSample.closing = false;
        socketsSample.bindingToService = true;
        socketsSample.listener = new Windows.Networking.Sockets.DatagramSocket();
        socketsSample.listener.addEventListener("messagereceived", onServerMessageReceived);
        socketsSample.displayStatus("Server: listener creation started.");
        socketsSample.listener.bindServiceNameAsync(serviceName).done(function () {
            socketsSample.displayStatus("Server: listener creation completed.");
            socketsSample.bindingToService = false;
        }, onError);
    }

    function onServerMessageReceived(eventArgument) {
        if (socketsSample.listenerOutputStream) {
            echoMessage(socketsSample.listenerOutputStream, eventArgument);
            return;
        }
            
        socketsSample.listener.getOutputStreamAsync(eventArgument.remoteAddress, eventArgument.remotePort).done(function (outputStream) {
            if (!socketsSample.listenerOutputStream) {
                socketsSample.listenerOutputStream = outputStream;
                socketsSample.listenerPeerAddress = eventArgument.remoteAddress;
                socketsSample.listenerPeerPort = eventArgument.remotePort;
            }

            echoMessage(socketsSample.listenerOutputStream, eventArgument);
        });
    }

    function echoMessage(outputStream, eventArgument) {
        if (socketsSample.listenerPeerAddress !== eventArgument.remoteAddress ||
            socketsSample.listenerPeerPort !== eventArgument.remotePort) {
            socketsSample.displayStatus("Got datagram from " + eventArguments.remoteAddress + ":" + eventArguments.remotePort +
                ", but already 'connected' to " + socketsSample.listenerPeerAddress + ":" + socketsSample.listenerPeerPort);
            return;
        }

        outputStream.writeAsync(eventArgument.getDataReader().detachBuffer()).done(function () {
            // Do nothing - client will print out a message when data is received.
        });
    }

    function onError(reason) {
        // Clean up a listener if we failed to bind to a port.
        if (socketsSample.bindingToService) {
            socketsSample.listener = null;
            socketsSample.bindingToService = false;
        }
        
        // When we close a socket, outstanding async operations will be canceled and the
        // error callbacks called.  There's no point in displaying those errors.
        if (!socketsSample.closing) {
            socketsSample.displayStatus(reason);
        }
    }
})();
