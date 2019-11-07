//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

var socketsSample = {};

(function () {
    "use strict";

    socketsSample.listener = null;
    socketsSample.listenerOutputStream = null;
    socketsSample.listenerPeerAddress = null;
    socketsSample.listenerPeerPort = null;
    socketsSample.clientSocket = null;
    socketsSample.clientDataWriter = null;
    socketsSample.connected = false;
    socketsSample.closing = false;
    socketsSample.bindingToService = false;

    socketsSample.serviceNameAccept = "22112";
    socketsSample.hostNameConnect = "localhost";
    socketsSample.serviceNameConnect = "22112";

    socketsSample.close = function () {

        socketsSample.closing = true;

        if (socketsSample.listener) {
            socketsSample.listener.close();
        }

        if (socketsSample.clientSocket) {
            socketsSample.clientSocket.close();
        }

        if (socketsSample.clientDataWriter) {
            socketsSample.clientDataWriter.close();
        }

        if (socketsSample.listenerOutputStream) {
            socketsSample.listenerOutputStream.close();
        }

        socketsSample.listener = null;
        socketsSample.listenerOutputStream = null;
        socketsSample.listenerPeerAddress = null;
        socketsSample.listenerPeerPort = null;
        socketsSample.clientSocket = null;
        socketsSample.clientDataWriter = null;
        socketsSample.connected = false;
    };

    socketsSample.setValues = function () {
        var serviceNameAcceptInput = document.getElementById("serviceNameAccept");
        var hostNameConnectInput = document.getElementById("hostNameConnect");
        var serviceNameConnectInput = document.getElementById("serviceNameConnect");

        if (serviceNameAcceptInput) {
            serviceNameAcceptInput.value = socketsSample.serviceNameAccept;
        }
        if (hostNameConnectInput) {
            hostNameConnectInput.value = socketsSample.hostNameConnect;
        }
        if (serviceNameConnectInput) {
            serviceNameConnectInput.value = socketsSample.serviceNameConnect;
        }
    };

    socketsSample.getValues = function (evt) {
        switch (evt.target.id) {
            case "serviceNameAccept":
                socketsSample.serviceNameAccept = evt.target.value;
                break;
            case "hostNameConnect":
                socketsSample.hostNameConnect = evt.target.value;
                break;
            case "serviceNameConnect":
                socketsSample.serviceNameConnect = evt.target.value;
                break;
        }
    };
})();
