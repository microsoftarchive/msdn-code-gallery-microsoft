//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/send-pdu-msg.html", {
        ready: function (element, options) {
            document.getElementById("sendPDUButton").addEventListener("click", sendPDUSMS, false);
         }
    });

    var smsDevice = null;

     function sendPDUSMS() {
        try {
            if (smsDevice === null) {
                var smsDeviceOperation = Windows.Devices.Sms.SmsDevice.getDefaultAsync();
                smsDeviceOperation.done(function (smsDeviceResult) {
                    smsDevice = smsDeviceResult;
                    smsDevicePDUSend();
                }, errorCallback);
            } else {
                smsDevicePDUSend();
            }
        } catch (err) {
            WinJS.log && WinJS.log("SMS did not set up: " + err.name + " : " + err.description, "sample", "error");
        }
    }

    function smsDeviceReceived(smsDeviceResult) {
        smsDevice = smsDeviceResult;
        smsDevicePDUSend();
    }

    function smsDevicePDUSend() {
        try {
            if (smsDevice !== null) {
                // Defines a binary message
                var smsMessage = new Windows.Devices.Sms.SmsBinaryMessage();
                var messsagePdu = document.getElementById("messagePDU").value;
                var messagePduByteArray = hexToByteArray(messsagePdu);

                if (messagePduByteArray === null) {
                    return;
                }

                smsMessage.setData(messagePduByteArray);
                if (smsDevice.cellularClass === Windows.Devices.Sms.CellularClass.gsm) {
                    smsMessage.format = Windows.Devices.Sms.SmsDataFormat.gsmSubmit;
                } else {
                    smsMessage.format = Windows.Devices.Sms.SmsDataFormat.cdmaSubmit;
                }

                var sendSmsMessageOperation = smsDevice.sendMessageAsync(smsMessage);

                WinJS.log && WinJS.log("Sending message ...", "sample", "status");
                sendSmsMessageOperation.done(function (reply) {
                    WinJS.log && WinJS.log("Sent message in PDU format", "sample", "status");
                }, errorCallback);
            } else {
                WinJS.log && WinJS.log("No SMS Device Found", "sample", "error");
            }
        } catch (err) {
            WinJS.log && WinJS.log("SMS did not set up: " + err, "sample", "error");
            // On failure, release the device. If the user revoked access or the device
            // is removed, a new device object must be obtained.
            smsDevice = null;
        }
    }

    // Used to convert hex PDU to byte array for sending SMS using PDU mode
    function hexToByteArray(hexString) {
        var result = [];
        var hexByte = "";
        var decByte = 0;

        if (hexString.length > 0 && (hexString.length & 1) === 0) {
            for (var i = 0; i < hexString.length; i = i + 2) {
                hexByte = hexString.substring(i, i + 2);
                decByte = parseInt(hexByte, 16);
                
                if (isNaN(decByte)) {
                    WinJS.log && WinJS.log("Input string was not in a correct format.", "sample", "error");
                    return null;
                }

                result.push(decByte);
            }
            return result;
        } else {
            WinJS.log && WinJS.log("Input string must have an even number of hex digits.", "sample", "error");
            return null;
        }
    }

    function errorCallback(error) {
        WinJS.log && WinJS.log(error.name + " : " + error.description, "sample", "error");
    }

})();
