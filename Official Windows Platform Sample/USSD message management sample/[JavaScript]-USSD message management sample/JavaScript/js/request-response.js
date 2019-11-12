//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/request-response.html", {
        ready: function (element, options) {
            document.getElementById("button1").addEventListener("click", sendUssdMessage, false);
        }
    });

    function sendUssdMessage() {
        try {
            // Get the USSD message text.
            var messageText = document.getElementById("scenarioInput").value;
            if (messageText === "") {
                WinJS.log && WinJS.log("Message cannot be empty", "sample", "error");
                return;
            }

            // Get the network account ID.
            var networkAccIds = Windows.Networking.NetworkOperators.MobileBroadbandAccount.availableNetworkAccountIds;
            if (networkAccIds.size === 0) {
                WinJS.log && WinJS.log("No network account ID found", "sample", "error");
                return;
            }
            // For the sake of simplicity, assume we want to use the first account.
            // Refer to the MobileBroadbandAccount API's for how to select a specific account ID.
            var networkAccountId = networkAccIds[0];

            WinJS.log && WinJS.log("Sending USSD request", "sample", "status");

            // Create a USSD session for the specified network acccount ID.
            var session = Windows.Networking.NetworkOperators.UssdSession.createFromNetworkAccountId(networkAccountId);

            // Create a USSD message to be sent to the network. This message is specific to
            // the network operator.
            var message = new Windows.Networking.NetworkOperators.UssdMessage(messageText);

            // Send a message to the network and wait for the reply.
            session.sendMessageAndGetReplyAsync(message).done(function (reply) {
                // Display the network reply. The reply always contains a resultCode.
                var code = reply.resultCode;
                if (code === Windows.Networking.NetworkOperators.UssdResultCode.actionRequired ||
                    code === Windows.Networking.NetworkOperators.UssdResultCode.noActionRequired) {
                    // If the actionRequired or noActionRequired resultCode is returned, the reply contains
                    // a message from the network.
                    var replyMessage = reply.message;
                    var payloadAsText = replyMessage.payloadAsText;
                    if (payloadAsText !== "") {
                        // The message may be sent using various encodings. If Windows supports
                        // the encoding, the message can be accessed as text and will not be empty.
                        // Therefore, the test for an empty string is sufficient.
                        WinJS.log && WinJS.log("Response: " + payloadAsText, "sample", "status");
                    }
                    else {
                        // If Windows does not support the encoding, the application may check
                        // the DataCodingScheme used for encoding and access the raw message
                        // through replyMessage.getPayload()
                        WinJS.log && WinJS.log("Unsupported data coding scheme 0x" + replyMessage.dataCodingScheme.toString(16),
                            "sample", "status");
                    }
                }
                else {
                    WinJS.log && WinJS.log("Request failed", "sample", "error");
                }
                if (code === Windows.Networking.NetworkOperators.UssdResultCode.actionRequired) {
                    session.close(); // Close the session from our end
                }
            }, function (error) {
                WinJS.log && WinJS.log(error.description + ", Error number: " + " 0x" + (0xFFFFFFFF + error.number + 1).toString(16),
                    "sample", "error");
            });
        }
        catch (e) {
            WinJS.log && WinJS.log("An unexpected exception occured: " + e.name + ": " + e.message, "sample", "error");
        }
    }
})();
