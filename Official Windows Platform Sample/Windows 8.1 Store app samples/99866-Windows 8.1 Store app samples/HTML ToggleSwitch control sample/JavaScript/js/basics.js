//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved


(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/basics.html", {
        ready: function (element, options) {

            //check which stylesheet is currently loaded
            initStyleSheetRadioButton();
        }
    });
})();

function toggleWifi() {
    var obj = document.getElementById("wifiToggle").winControl;
    console.log("Wifi toggled. Current status: " + (obj.checked ? "on" : "off"));
}

// To protect against untrusted code execution, all functions are required to be marked as supported for processing before they can be used inside a data-win-options attribute in HTML markup.
WinJS.Utilities.markSupportedForProcessing(toggleWifi);

