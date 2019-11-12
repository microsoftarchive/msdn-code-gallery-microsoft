//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/Landscape.html", {
        ready: function (element, options) {
            document.getElementById("setLandscape").addEventListener("click", setLandscape_click, false);
        }
    });

    function setLandscape_click() {
        Windows.Graphics.Display.DisplayInformation.autoRotationPreferences = Windows.Graphics.Display.DisplayOrientations.landscape;

        if (Windows.Graphics.Display.DisplayInformation.autoRotationPreferences === Windows.Graphics.Display.DisplayOrientations.landscape) {
            WinJS.log && WinJS.log("Succeeded: Preference set to Landscape.", "sample", "status");
        } else {
            WinJS.log && WinJS.log("Error: Failed to set the preference.", "sample", "error");
        }
    }
})();
