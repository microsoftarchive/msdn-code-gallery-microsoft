//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/test-pin.html", {
        ready: function (element, options) {
            document.getElementById("getPinStateButton").addEventListener("click", getPinState, false);
            document.getElementById("enterPinButton").addEventListener("click", enterPin, false);
            document.getElementById("enterPinInput").style.display = "none";
            tryGetMbDevice();
        }
    });

    function printMsg(pinInfo) {
        if (pinInfo.isPinEnabled === true) {
            document.getElementById("enterPinInput").style.display = "";
        }
        else {
            document.getElementById("enterPinInput").style.display = "none";
        }
        WinJS.log && WinJS.log(pinInfo.msg, "sample", "status");
    }

    function tryGetMbDevice() {
        try {
            // Get the pin wrapper
            var pinWrapper = new MbnComWrapper.PinWrapper(printMsg);
        } catch (e) {
            // Parse the exception
            mbComApiUtil.parseExceptionCodeAndPrint(e);
            document.getElementById("getPinStateButton").disabled = true;
            document.getElementById("enterPinInput").style.display = "none";
        }
    }

    function getPinState() {
        try {
            // Create pin wrapper
            var pinWrapper = createPinWrapper();

            // Get the pin state
            pinWrapper.getPinState();
        } catch (e) {
            // Parse the exception
            mbComApiUtil.parseExceptionCodeAndPrint(e);
            document.getElementById("getPinStateButton").disabled = true;
            document.getElementById("enterPinInput").style.display = "none";
        }
    }

    function enterPin() {
        try {
            // Create pin wrapper
            var pinWrapper = createPinWrapper();
            var pinInput = document.getElementById("pinText").value;
            if (pinInput === "") {
                WinJS.log && WinJS.log("Pin cannot be empty", "sample", "error");
            }
            else {
                // Enter the pin
                pinWrapper.enterPin(pinInput);
            }
        } catch (e) {
            // Parse the exception
            mbComApiUtil.parseExceptionCodeAndPrint(e);
            document.getElementById("getPinStateButton").disabled = true;
            document.getElementById("enterPinInput").style.display = "none";
        }
    }

    function createPinWrapper() {
        var pinWrapper;

        // Get the pin wrapper
        pinWrapper = new MbnComWrapper.PinWrapper(printMsg);

        // Add event listeners
        pinWrapper.addEventListener(
            "raiseongetpinstatecompleteevent",
            printMsg,
            false);

        pinWrapper.addEventListener(
            "raiseonpinentercompleteevent",
            printMsg,
            false);

        return pinWrapper;
    }
})();
