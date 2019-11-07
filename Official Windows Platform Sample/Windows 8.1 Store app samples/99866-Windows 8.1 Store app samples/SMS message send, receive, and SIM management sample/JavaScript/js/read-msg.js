//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/read-msg.html", {
        ready: function (element, options) {
            document.getElementById("readButton").addEventListener("click", readSMS, false);
        }
    });

   var smsDevice = null;

   function readSMS() {
        try {
            if (smsDevice === null) {
                var smsDeviceOperation = Windows.Devices.Sms.SmsDevice.getDefaultAsync();
                smsDeviceOperation.done(smsDeviceReceived, errorCallback);
            } else {
                smsDeviceRead();
            }
        } catch (err) {
            WinJS.log && WinJS.log("SMS did not set up: " + err.name + " : " + err.description, "sample", "error");
        }
    }

    function smsDeviceReceived(smsDeviceResult) {
        smsDevice = smsDeviceResult;
        smsDeviceRead();
    }

    function smsDeviceRead() {
        try {
            if (smsDevice !== null) {
                var messageStore = smsDevice.messageStore;
                var messageID = parseInt(document.getElementById("messageIdRead").value);

                // Check for a valid ID number
                if (isNaN(messageID) || messageID < 1 || messageID > messageStore.maxMessages) {
                    WinJS.log && WinJS.log("Invalid ID number", "sample", "error");
                    return;
                }

                var getSmsMessageOperation = messageStore.getMessageAsync(messageID);

                // Display message when get is completed
                getSmsMessageOperation.done(smsMessageReadSuccess, function (error) {
                    WinJS.log && WinJS.log("Error reading message (try different ID)", "sample", "error");
                });
            } else {
                WinJS.log && WinJS.log("No SMS Device Found", "sample", "error");
            }
        }
        catch (err) {
            WinJS.log && WinJS.log("SMS did not set up: " + err, "sample", "error");
            // On failure, release the device. If the user revoked access or the device
            // is removed, a new device object must be obtained.
            smsDevice = null;
        }
    }

    function smsMessageReadSuccess(smsMessage) {
        try {
            var textMessage = new Windows.Devices.Sms.SmsTextMessage.fromBinaryMessage(smsMessage);
            document.getElementById("date").innerText = textMessage.timestamp;
            document.getElementById("fromNumberRead").innerText = textMessage.from;
            document.getElementById("messageRead").innerText = textMessage.body;

            WinJS.log && WinJS.log("Message read.", "sample", "status");
        }
        catch (err) {
            WinJS.log && WinJS.log("SMS did not set up: " + err, "sample", "error");
            // On failure, release the device. If the user revoked access or the device
            // is removed, a new device object must be obtained.
            smsDevice = null;
        }
    }

    function errorCallback(error) {
        WinJS.log && WinJS.log(error.name + " : " + error.description, "sample", "error");
    }
})();
