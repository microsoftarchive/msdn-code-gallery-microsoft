//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/PhoneCall.html", {
        ready: function (element, options) {
            document.getElementById("buttonInitialize").addEventListener("click", initDevice, false);
            document.getElementById("buttonIncomingCall").addEventListener("click", newIncomingCall, false);
            document.getElementById("buttonHangUp").addEventListener("click", hangupButton, false);
        }
    });

    // Initialize the call control object here using the default bluetooth communications device
    var callControls = null;
    var callToken;
    var audiotag;

    function displayStatus(message) {
        WinJS.log && WinJS.log(getTimeStampedMessage(message), "Call Control", "status");
    }

    function displayError(message) {
        WinJS.log && WinJS.log(getTimeStampedMessage(message), "Call Control", "error");
    }

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function initDevice() {
        if (!callControls) {
            callControls = Windows.Media.Devices.CallControl.getDefault();

            if (callControls) {
                // Add the event listener to listen for the various button presses
                callControls.addEventListener("answerrequested", answerButton, false);
                callControls.addEventListener("hanguprequested", hangupButton, false);
                callControls.addEventListener("audiotransferrequested", audiotransferButton, false);
                callControls.addEventListener("redialrequested", redialButton, false);
                callControls.addEventListener("dialrequested", dialButton, false);

                displayStatus("Call Controls Initialized");
                id("buttonIncomingCall").disabled = false;
                id("buttonHangUp").disabled = true;
                id("buttonInitialize").disabled = true;
            } else {
                displayError("No Bluetooth device detected.");
            }
        }
    }

    function newIncomingCall() {
        // Indicate a new incoming call and ring the headset.
        callToken = callControls.indicateNewIncomingCall(true, "5555555555");
        displayStatus("Call Token: " + callToken);
        id("buttonIncomingCall").disabled = true;
    }

    function answerButton() {
        // When the answer button is pressed indicate to the device that the call was answered
        // and start a song on the headset (this is done by streaming music to the bluetooth
        // device in this sample)
        displayStatus("Answer requested: " + callToken);
        callControls.indicateActiveCall(callToken);
        audiotag = document.getElementById("audiotag");
        audiotag.play();
        id("buttonHangUp").disabled = false;
    }

    function hangupButton() {
        // Hang up request received.  The application should end the active call and stop
        // streaming to the headset
        displayStatus("Hangup requested");
        callControls.endCall(callToken);
        audiotag = document.getElementById("audiotag");
        audiotag.pause();
        id("buttonIncomingCall").disabled = false;
        id("buttonHangUp").disabled = true;
        callToken = 0;
    }

    function audiotransferButton() {
        // Handle the audio transfer request here
        displayStatus("Audio Transfer requested");
    }

    function redialButton(redialRequestedEventArgs) {
        // Handle the redial request here.  Indicate to the device that the request was handled.
        displayStatus("Redial requested");
        redialRequestedEventArgs.handled();
    }

    function dialButton(dialRequestedEventArgs) {
        // A device may send a dial request by either sending a URI or if it is a speed dial,
        // an integer with the number to dial.
        if (typeof (dialRequestedEventArgs.contact) === "number") {
            displayStatus("Dial requested: " + dialRequestedEventArgs.contact);
            dialRequestedEventArgs.handled();
        }
        else {
            displayStatus("Dial requested: " + dialRequestedEventArgs.contact.schemeName + ":" +
            dialRequestedEventArgs.contact.path);
            dialRequestedEventArgs.handled();
        }
    }

    function getTimeStampedMessage(eventCalled) {
        var timeformat = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longtime");
        var time = timeformat.format(new Date());
        var message = eventCalled + "\t\t" + time;

        return message;

    }

})();
