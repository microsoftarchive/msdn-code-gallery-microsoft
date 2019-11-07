//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/PortraitFlipped.html", {
        ready: function (element, options) {
            document.getElementById("setPortraitFlipped").addEventListener("click", setPortraitFlipped_click, false);
        }
    });

    function setPortraitFlipped_click() {
        Windows.Graphics.Display.DisplayInformation.autoRotationPreferences = Windows.Graphics.Display.DisplayOrientations.portraitFlipped;

        if (Windows.Graphics.Display.DisplayInformation.autoRotationPreferences === Windows.Graphics.Display.DisplayOrientations.portraitFlipped) {
            WinJS.log && WinJS.log("Succeeded: Preference set to Portrait Flipped.", "sample", "status");
        } else {
            WinJS.log && WinJS.log("Error: Failed to set the preference.", "sample", "error");
        }
    }
})();
