//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/4-SettingsFlyout-Settings.html", {

        ready: function (element, options) {
            // Register the handlers for dismissal
            document.getElementById("defaultSettingsFlyout").addEventListener("keydown", handleKeys);
        },

        unload: function () {
            // Remove the handlers for dismissal
            document.getElementById("defaultSettingsFlyout").removeEventListener("keydown", handleKeys);
            document.getElementById("backButton").removeEventListener("click", handleBackButton);
        },
    });

    function handleKeys(evt) {
        // Handles Alt+Left and backspace key in the control and dismisses it
        if ((evt.altKey && evt.key === 'Left') || (evt.key === 'Backspace')) {
            WinJS.UI.SettingsFlyout.show();
        }
    };
})();
