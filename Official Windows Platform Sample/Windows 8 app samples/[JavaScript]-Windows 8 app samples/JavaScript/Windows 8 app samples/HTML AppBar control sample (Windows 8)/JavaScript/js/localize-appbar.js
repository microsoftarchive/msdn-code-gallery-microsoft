//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/localize-appbar.html", {
        ready: function (element, options) {
            // Load resources and enable listener so they get updated when user changes language selection
            loadResources();
            WinJS.Resources.addEventListener("contextchanged", loadResources);

            document.getElementById("cmdCommand1").addEventListener("click", doClickCommand1, false);
            document.getElementById("cmdCommand2").addEventListener("click", doClickCommand2, false);
            WinJS.log && WinJS.log("To show the bar, swipe up from the bottom of the screen, right-click, or press Windows Logo + z. To dismiss the bar, tap in the application, swipe, right-click, or press Windows Logo + z again.", "sample", "status");
        },
        unload: function () {
            // Remove listener, no other documents have resources
            WinJS.Resources.removeEventListener("contextchanged", loadResources);

            AppBarSampleUtils.removeAppBars();
        }
    });

    // Handles loading of resources
    function loadResources() {
        WinJS.Resources.processAll();
    }

    // Command button functions
    function doClickCommand1() {
        WinJS.log && WinJS.log("Localized Command1 button pressed", "sample", "status");
    }

    function doClickCommand2() {
        WinJS.log && WinJS.log("Localized Command2 button pressed", "sample", "status");
    }

})();
