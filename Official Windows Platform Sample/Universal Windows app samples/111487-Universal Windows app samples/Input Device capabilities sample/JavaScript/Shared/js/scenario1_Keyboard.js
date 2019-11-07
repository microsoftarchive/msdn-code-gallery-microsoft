//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1_Keyboard.html", {
        ready: function (element, options) {
            id("button1").addEventListener("click", getKeyboardCapabilities, false);
        }
    });

    function id(elementId) {
        return document.getElementById(elementId);
    }

    function getKeyboardCapabilities() {
        var keyboardCapabilities = new Windows.Devices.Input.KeyboardCapabilities();
        id("keyboardPresent").innerHTML = /*@static_cast(String)*/keyboardCapabilities.keyboardPresent;
    }
})();
