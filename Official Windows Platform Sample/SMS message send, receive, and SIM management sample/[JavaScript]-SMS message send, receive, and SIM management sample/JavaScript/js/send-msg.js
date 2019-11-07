//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/send-msg.html", {
        ready: function (element, options) {
            document.getElementById("sendButton").addEventListener("click", sendSMS, false);
        }
    });
    var smsDevice = null;

    function sendSMS() {
        try {
            if (smsDevice === null) {
                var smsDeviceOperation = Windows.Devices.Sms.SmsDevice.getDefaultAsync();
                smsDeviceOperation.done(smsDeviceReceived, errorCallback);
            } else {
                // We already have the smsDevice
                smsDeviceSend();
            }
        } catch (err) {
            WinJS.log && WinJS.log("SMS did not set up: " + err.name + " : " + err.description, "sample", "error");
        }
    }

    function smsDeviceReceived(smsDeviceResult) {
        smsDevice = smsDeviceResult;
        smsDeviceSend();
    }

    function smsDeviceSend() {
        try {
            if (smsDevice !== null) {
                // Defines a text message
                var smsMessage = new Windows.Devices.Sms.SmsTextMessage();
                smsMessage.to = document.getElementById("phoneNumber").value;
                smsMessage.body = document.getElementById("messageText").value;
                var sendSmsMessageOperation = smsDevice.sendMessageAsync(smsMessage);
                WinJS.log && WinJS.log("Sending message...", "sample", "status");
                sendSmsMessageOperation.done(function (reply) {
                    WinJS.log && WinJS.log("Text message sent.", "sample", "status");
                }, errorCallback);
            } else {
                WinJS.log && WinJS.log("No SMS device found", "sample", "error");
            }
        } catch (err) {
            WinJS.log && WinJS.log("SMS message was not sent: " + err.name + " : " + err.description, "sample", "error");
        }
    }

    function errorCallback(error) {
        WinJS.log && WinJS.log(error.name + " : " + error.description, "sample", "error");
    }

})();
