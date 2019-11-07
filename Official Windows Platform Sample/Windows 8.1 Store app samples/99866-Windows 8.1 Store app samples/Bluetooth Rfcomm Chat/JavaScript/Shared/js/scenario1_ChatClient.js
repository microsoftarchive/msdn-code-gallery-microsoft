//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario1_ChatClient.html", {
        ready: function (element, options) {
            serviceList.onchange = onServiceListSelect;
            document.getElementById("runButton").addEventListener("click", onRunButtonClick, false);
            document.getElementById("sendButton").addEventListener("click", onSendButtonClick, false);
            document.getElementById("disconnectButton").addEventListener("click", onDisconnectButtonClick, false);
        }
    });

    // The Chat Service's custom service Uuid {34B1CF4D-1069-4AD6-89B6-E161D79BE4D8}
    var RFCOMM_CHAT_SERVICE_UUID = "{34b1cf4d-1069-4ad6-89b6-e161d79be4d8}";

    // The Id of the Service Name SDP attribute
    var SDP_SERVICE_NAME_ATTRIBUTE_ID = 0x100;

    // The SDP Type of the Service Name SDP attribute.
    // The first byte in the SDP Attribute encodes the SDP Attribute Type as follows :
    //    -  the Attribute Type size in the least significant 3 bits,
    //    -  the SDP Attribute Type value in the most significant 5 bits.
    var SDP_SERVICE_NAME_ATTRIBUTE_TYPE = (4 << 3) | 5;

    var app = WinJS.Application;
    var rfcomm = Windows.Devices.Bluetooth.Rfcomm;
    var sockets = Windows.Networking.Sockets;
    var streams = Windows.Storage.Streams;

    app.addEventListener("oncheckpoint", function (args) {
        // This application is about to be suspended
        disconnect();
        WinJS.log && WinJS.log("Disconnected due to suspension");
    });

    var chatService;
    var chatSocket;
    var chatServices;
    var chatWriter;

    function onRunButtonClick() {
        Windows.Devices.Enumeration.DeviceInformation.findAllAsync(
            rfcomm.RfcommDeviceService.getDeviceSelector(rfcomm.RfcommServiceId.fromUuid(RFCOMM_CHAT_SERVICE_UUID)),
            null).done(function (services) {
                if (services.length > 0) {
                    chatServices = services;

                    while (serviceList.firstChild) {
                        serviceList.removeChild(serviceList.firstChild);
                    }

                    for (var i = 0; i < services.length; i++) {
                        var service = services[i];
                        var serviceName = document.createElement("option");
                        serviceName.innerText = service.name;
                        serviceList.appendChild(serviceName);
                    }

                    serviceList.selectedIndex = -1;
                    serviceSelector.style.display = "";
                } else {
                    WinJS.log && WinJS.log("No chat services were found. Please pair Windows with a device that is " +
                        "advertising the chat service", "sample", "status");
                }
            });
    }

    function onServiceListSelect() {
        serviceSelector.style.display = "none";

        // Initialize the target Rfcomm service
        rfcomm.RfcommDeviceService.fromIdAsync(chatServices[serviceList.selectedIndex].id).done(
            function (service) {
                if (service === null) {
                    WinJS.log && WinJS.log(
                        "Access to the device is denied because the application was not granted access",
                        "sample", "status");
                    return;
                }

                chatService = service;

                chatService.getSdpRawAttributesAsync(Windows.Devices.Bluetooth.BluetoothCacheMode.uncached).done(
                    function (attributes) {
                        var buffer = attributes.lookup(SDP_SERVICE_NAME_ATTRIBUTE_ID);
                        if (buffer === null) {
                            WinJS.log && WinJS.log(
                                "The Chat service is not advertising the Service Name attribute (attribute " +
                                "id=0x100). Please verify that you are running the BluetoothRfcommChat server.",
                                "sample", "error");
                            return;
                        }

                        var attributeReader = streams.DataReader.fromBuffer(buffer);
                        var attributeType = attributeReader.readByte();
                        if (attributeType !== SDP_SERVICE_NAME_ATTRIBUTE_TYPE) {
                            WinJS.log && WinJS.log(
                                "The Chat service is using an expected format for the Service Name attribute. " +
                                "Please verify that you are running the BluetoothRfcommChat server.",
                                "sample", "error");
                            return;
                        }

                        var serviceNameLength = attributeReader.readByte();

                        // The Service Name attribute requires UTF-8 encoding.
                        attributeReader.unicodeEncoding = streams.UnicodeEncoding.utf8;
                        serviceName.innerText =
                            "Service Name: \"" + attributeReader.readString(serviceNameLength) + "\"";

                        chatSocket = new sockets.StreamSocket();
                        chatSocket.connectAsync(
                            chatService.connectionHostName,
                            chatService.connectionServiceName,
                            sockets.SocketProtectionLevel.plainSocket).done(function () {
                                chatWriter = new streams.DataWriter(chatSocket.outputStream);
                                chatBox.style.display = "";

                                receiveStringLoop(new streams.DataReader(chatSocket.inputStream));
                            }, function (error) {
                                WinJS.log && WinJS.log("Failed to connect to server, with error: " + error, "sample", "error");
                            });
                    }, function (error) {
                        WinJS.log && WinJS.log("Failed to retrieve SDP attributes, with error: " + error, "sample", "error");
                    });
            }, function (error) {
                WinJS.log && WinJS.log("Failed to connect to server, with error: " + error, "sample", "error");
            });
        }

    function receiveStringLoop(reader) {
        reader.loadAsync(4).done(function (size) {
            if (size < 4) {
                disconnect();
                WinJS.log && WinJS.log("Client disconnected.", "sample", "status");
                return;
            }

            var stringLength = reader.readUInt32();
            reader.loadAsync(stringLength).done(function (actualStringLength) {
                if (actualStringLength < stringLength) {
                    disconnect();
                    WinJS.log && WinJS.log("Client disconnected.", "sample", "status");
                    return;
                }

                var currentMessage = document.createElement("option");
                currentMessage.innerText = "Received: " + reader.readString(actualStringLength);
                conversationSelect.appendChild(currentMessage);

                // Restart the read for more bytes. We could just call receiveStringLoop() but in the case subsequent
                // read operations complete synchronously we start building up the stack and potentially crash. We use
                // WinJS.Promise.timeout() invoke this function after the stack for current call unwinds.
                WinJS.Promise.timeout().done(function () { return receiveStringLoop(reader); });
            }, function (error) {
                WinJS.log && WinJS.log("Failed to read the message, with error: " + error, "sample", "error");
            });
        }, function (error) {
            WinJS.log && WinJS.log("Failed to read the message size, with error: " + error, "sample", "error");
        });
    }

    function onSendButtonClick() {
        try {
            chatWriter.writeUInt32(messageInput.value.length);
            chatWriter.writeString(messageInput.value);

            chatWriter.storeAsync().done(function () {
                var sentMessage = document.createElement("option");
                sentMessage.innerText = "Sent: " + messageInput.value;
                conversationSelect.appendChild(sentMessage);
                messageInput.value = "";
            }, function (error) {
                WinJS.log && WinJS.log("Failed to send the message to the server, error: " + error,
                    "sample", "error");
            });
        } catch (error) {
            WinJS.log && WinJS.log("Sending message failed with error: " + error);
        }
    }
    
    function onDisconnectButtonClick() {
        disconnect();
        WinJS.log && WinJS.log("Disconnected", "sample", "status");
    }

    function disconnect() {
        if (chatWriter) {
            chatWriter.detachStream();
            chatWriter = null;
        }

        if (chatSocket) {
            chatSocket.close();
            chatSocket = null;
        }

        runButton.disabled = false;
        serviceSelector.style.display = "none";
        chatBox.style.display = "none";
        conversationSelect.innerHTML = "";
    }
})();
