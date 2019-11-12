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
        if (socketsSample.listener) {
            socketsSample.displayStatus("Already have a listener; call close to close the listener.");
            return;
        }

        var serviceName = document.getElementById("serviceNameAccept").value;
        if (serviceName === "") {
            socketsSample.displayStatus("Please provide a service name.");
            return;
        }

        socketsSample.closing = false;
        socketsSample.bindingToService = true;
        socketsSample.listener = new Windows.Networking.Sockets.StreamSocketListener(serviceName);
        socketsSample.listener.addEventListener("connectionreceived", onServerAccept);
        socketsSample.displayStatus("Server: listener creation started.");
        socketsSample.listener.bindServiceNameAsync(serviceName).done(function () {
            socketsSample.displayStatus("Server: listener creation completed.");
            socketsSample.bindingToService = false;
        }, onError);
    }

    // This has to be a real function ; it will "loop" back on itself with the
    // call to acceptAsync at the very end.
    function onServerAccept(eventArgument) {
        socketsSample.displayStatus("Server: connection accepted.");
        socketsSample.serverSocket = eventArgument.socket;
        socketsSample.serverReader = new Windows.Storage.Streams.DataReader(socketsSample.serverSocket.inputStream);
        startServerRead();
    }

    // The protocol here is simple: a four-byte 'network byte order' (big-endian) integer
    // that says how long a string is, and then a string that is that long.
    // We wait for exactly 4 bytes, read in the count value, and then wait for
    // count bytes, and then display them.
    function startServerRead() {
        socketsSample.serverReader.loadAsync(4).done(function (sizeBytesRead) {
            // Make sure 4 bytes were read.
            if (sizeBytesRead !== 4) {
                socketsSample.displayStatus("Server: connection lost.");
                return;
            }

            // Read in the 4 bytes count and then read in that many bytes.
            var count = socketsSample.serverReader.readInt32();
            return socketsSample.serverReader.loadAsync(count).then(function (stringBytesRead) {
                // Make sure the whole string was read.
                if (stringBytesRead !== count) {
                    socketsSample.displayStatus("Server: connection lost.");
                    return;
                }
                // Read in the string.
                var string = socketsSample.serverReader.readString(count);
                socketsSample.displayOutput("Server read: " + string);

                // Restart the read for more bytes. We could just call startServerRead() but in
                // the case subsequent read operations complete synchronously we start building
                // up the stack and potentially crash. We use WinJS.Promise.timeout() invoke
                // this function after the stack for current call unwinds.
                WinJS.Promise.timeout().done(function () { return startServerRead(); });
            }); // End of "read in rest of string" function.
        }, onError);
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
