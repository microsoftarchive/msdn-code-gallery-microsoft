//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/None.html", {
        ready: function (element, options) {
            document.getElementById("clearPreference").addEventListener("click", clearPreference_click, false);
        }
    });

    function clearPreference_click() {
        Windows.Graphics.Display.DisplayInformation.autoRotationPreferences = Windows.Graphics.Display.DisplayOrientations.none;

        if (Windows.Graphics.Display.DisplayInformation.autoRotationPreferences === Windows.Graphics.Display.DisplayOrientations.none) {
            WinJS.log && WinJS.log("Succeeded: All preferences cleared.", "sample", "status");
        } else {
            WinJS.log && WinJS.log("Error: Failed to set the preference.", "sample", "error");
        }
    }
})();
