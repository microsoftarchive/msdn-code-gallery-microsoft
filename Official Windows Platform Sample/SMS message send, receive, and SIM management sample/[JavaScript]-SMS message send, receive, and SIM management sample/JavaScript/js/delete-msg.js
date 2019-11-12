//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/delete-msg.html", {
        ready: function (element, options) {
            document.getElementById("deleteButton").addEventListener("click", deleteSMSButtonClick, false);
            document.getElementById("deleteAllButton").addEventListener("click", deleteAllSMSButtonClick, false);
        }
    });
    var smsDevice = null;

    // Callback function for delete all button click
    function deleteAllSMSButtonClick() {
        try {
            if (smsDevice === null) {
                var smsDeviceOperation = Windows.Devices.Sms.SmsDevice.getDefaultAsync();
                smsDeviceOperation.done(function (smsDeviceResult) {
                    smsDevice = smsDeviceResult;
                    smsDeviceDeleteAll();
                }, errorCallback);
            } else {
                smsDeviceDeleteAll();
            }
        } catch (err) {
            WinJS.log && WinJS.log("SMS did not set up: " + err.name + " : " + err.description, "sample", "error");
        }
    }

    function smsDeviceDeleteAll() {
        try {
            if (smsDevice !== null) {

                // Delete all messages asynchronously.
                var messageStore = smsDevice.messageStore;
                var deleteSmsOperation = messageStore.deleteMessagesAsync(Windows.Devices.Sms.SmsMessageFilter.all);

                WinJS.log && WinJS.log("Deleting all messages ...", "sample", "status");

                deleteSmsOperation.done(function (reply) {
                    WinJS.log && WinJS.log("All messages deleted", "sample", "status");
                }, function (err) {
                    WinJS.log && WinJS.log("Error deleting messages.", "sample", "error");
                });
            } else {
                WinJS.log && WinJS.log("No SMS device found", "sample", "error");
            }
        } catch (err) {
            WinJS.log && WinJS.log("SMS message was not deleted: " + err.name + " : " + err.description, "sample", "error");
            // On failure, release the device. If the user revoked access or the device
            // is removed, a new device object must be obtained.
            smsDevice = null;
        }
    }


    function deleteSMSButtonClick() {
        try {
            if (smsDevice === null) {
                var smsDeviceOperation = Windows.Devices.Sms.SmsDevice.getDefaultAsync();
                smsDeviceOperation.done(function (smsDeviceResult) {
                    smsDevice = smsDeviceResult;
                    smsDeviceDelete();
                }, errorCallback);
            } else {
                smsDeviceDelete();
            }
        } catch (err) {
            WinJS.log && WinJS.log("SMS did not set up: " + err.name + " : " + err.description, "sample", "error");
        }
    }


    function smsDeviceDelete() {
        try {
            if (smsDevice !== null) {

                var messageStore = smsDevice.messageStore;
                var messageID = parseInt(document.getElementById("messageId").value);

                if (isNaN(messageID) || messageID < 1 || messageID > messageStore.maxMessages) {
                    WinJS.log && WinJS.log("Invalid message ID entered", "sample", "error");
                    return;
                }

                var deleteSmsOperation = messageStore.deleteMessageAsync(messageID);
                WinJS.log && WinJS.log("Deleting message...", "sample", "status");

                deleteSmsOperation.done(function (reply) {
                    WinJS.log && WinJS.log("Text message deleted.", "sample", "status");
                }, function (err) {
                    WinJS.log && WinJS.log("Error deleting message.", "sample", "error");
                });
            } else {
                WinJS.log && WinJS.log("No SMS device found", "sample", "error");
            }
        } catch (err) {
            WinJS.log && WinJS.log("SMS message was not deleted: " + err.name + " : " + err.description, "sample", "error");
            // On failure, release the device. If the user revoked access or the device
            // is removed, a new device object must be obtained.
            smsDevice = null;
        }
    }

    function errorCallback(error) {
        WinJS.log && WinJS.log(error.name + " : " + error.description, "sample", "error");
    }

})();
