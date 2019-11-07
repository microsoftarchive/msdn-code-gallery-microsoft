//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/sendData.html", {
        ready: function (element, options) {
            document.getElementById("buttonSend").addEventListener("click", sendHello, false);
        }
    });

    function sendHello() {
        if (!socketsSample.connected) {
            socketsSample.displayStatus("Client: you must connect the client before using it.");
            return;
        }

        if (!socketsSample.clientDataWriter) {
            socketsSample.clientDataWriter = new Windows.Storage.Streams.DataWriter(socketsSample.clientSocket.outputStream);
        }

        var string = "Hello World";
        socketsSample.clientDataWriter.writeString(string);

        socketsSample.displayStatus("Client sending: " + string + ".");
        socketsSample.clientDataWriter.storeAsync().done(function () {
            socketsSample.displayStatus("Client sent: " + string + ".");
        }, onError);
    }

    function onError(reason) {
        // When we close a socket, outstanding async operations will be canceled and the
        // error callbacks called.  There's no point in displaying those errors.
        if (!socketsSample.closing) {
            socketsSample.displayStatus(reason);
        }
    }
})();
