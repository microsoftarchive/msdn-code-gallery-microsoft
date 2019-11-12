//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var proximityDevice = null;


    var page = WinJS.UI.Pages.define("/html/Scenario3_ProximityDevice.html", {
        ready: function (element, options) {
            proximityDevice = ProximityHelpers.initializeProximityDevice();
            if (proximityDevice) {

                // Don't bother listening for button clicks if there is no proximity device available.
                ProximityHelpers.id("proximityDevice_PublishMessageButton").addEventListener("click", proximityDevice_PublishMessage, false);
                ProximityHelpers.id("proximityDevice_SubscribeForMessageButton").addEventListener("click", proximityDevice_SubscribeForMessage, false);
                ProximityHelpers.id("proximityDevice_PublishMessageButton").style.display = "inline";
                ProximityHelpers.id("proximityDevice_SubscribeForMessageButton").style.display = "inline";
                ProximityHelpers.id("proximityDevice_PublishText").style.display = "inline";
                ProximityHelpers.id("proximityDevice_PublishText").value = "Hello World";
            }
        },

        unload: function () {
            // Cleanup any publications or subscriptions
            if (proximityDevice) {
                    proximityDevice.stopPublishingMessage(publishedMessageId);
                    publishedMessageId = -1;
                    proximityDevice.stopSubscribingForMessage(subscribedMessageId);
                    subscribedMessageId = -1;
                
            } 
        }

    });

    var publishedMessageId = -1;
    function proximityDevice_PublishMessage() {
        if (publishedMessageId === -1) {
            ProximityHelpers.displayError("");
            var publishText = ProximityHelpers.id("proximityDevice_PublishText").value;
            ProximityHelpers.id("proximityDevice_PublishText").value = ""; // clear the input after publishing.
            if (publishText.length > 0) {
                publishedMessageId = proximityDevice.publishMessage("Windows.SampleMessageType",
                                                                        publishText);
                ProximityHelpers.displayStatus("Message published, tap another device to transmit.");
                ProximityHelpers.logInfo(ProximityHelpers.id("proximityDeviceOutput"), "Published: " + publishText);
            } else {
                ProximityHelpers.displayError("Please type a message");
            }
        } else {
            ProximityHelpers.displayError("This sample only supports publishing one message at a time.");
        }
    }

    function messageReceived(receivingDevice, message) {
        ProximityHelpers.logInfo(ProximityHelpers.id("proximityDeviceOutput"), "Message received: " + message.dataAsString);
    }

    var subscribedMessageId = -1;

    function proximityDevice_SubscribeForMessage() {
        if (subscribedMessageId === -1) {
            subscribedMessageId = proximityDevice.subscribeForMessage("Windows.SampleMessageType", messageReceived);
            ProximityHelpers.displayStatus("Subscribed for proximity message, enter proximity to receive.");
        } else {
            ProximityHelpers.displayError("This sample only supports subscribing for one message at a time.");
        }
    }

})();
