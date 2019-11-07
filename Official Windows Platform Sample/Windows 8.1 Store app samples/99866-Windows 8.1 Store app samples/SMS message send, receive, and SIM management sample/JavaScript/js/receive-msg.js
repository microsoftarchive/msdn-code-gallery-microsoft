//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/receive-msg.html", {
        ready: function (element, options) {
            document.getElementById("receiveButton").addEventListener("click", receiveSMS, false);
        }
    });
    var smsDevice = null;
    var listening = false;
    function receiveSMS() {
        try {
            if (smsDevice === null) {
                var smsDeviceOperation = Windows.Devices.Sms.SmsDevice.getDefaultAsync();
                smsDeviceOperation.done(smsDeviceReceived, errorCallback);
            } else {
                smsDeviceReceive();
            }

        } catch (err) {
            WinJS.log && WinJS.log("Failed to find SMS device: " + err.name + " : " + err.description, "sample", "error");
        }
    }

    function smsDeviceReceived(smsDeviceResult) {
        smsDevice = smsDeviceResult;
        smsDeviceReceive();
    }

    function smsReceived(smsMessage) {
        var textMessage = smsMessage.textMessage;
        document.getElementById("fromNumber").innerText = textMessage.from;
        document.getElementById("newMessage").innerText = textMessage.body;

        WinJS.log && WinJS.log("Message Received!", "sample", "status");
    }

    function smsDeviceReceive() {
        try {
            if (smsDevice !== null) {
                if (!listening) {
                    // Sets up listener
                    WinJS.log && WinJS.log("Waiting for message ...", "sample", "status");
                    listening = true;
                    smsDevice.onsmsmessagereceived = smsReceived;
                }
            } else {
                WinJS.log && WinJS.log("No SMS device found", "sample", "error");
            }
        } catch (err) {
            WinJS.log && WinJS.log(err.name + " : " + err.description, "sample", "error");
        }
    }

    function errorCallback(error) {
        WinJS.log && WinJS.log(error.name + " : " + error.description, "sample", "error");
    }
})();
