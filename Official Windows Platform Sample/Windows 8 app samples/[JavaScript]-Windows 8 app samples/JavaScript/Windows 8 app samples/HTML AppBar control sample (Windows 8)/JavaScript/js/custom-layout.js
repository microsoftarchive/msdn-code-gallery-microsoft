//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/custom-layout.html", {
        ready: function (element, options) {
            document.getElementById("cmdBack").addEventListener("click", doClickBack, false);
            WinJS.log && WinJS.log("To show the bar, swipe up from the bottom of the screen, right-click, or press Windows Logo + z. To dismiss the bar, tap in the application, swipe, right-click, or press Windows Logo + z again.", "sample", "status");
        },
        unload: function () {
            AppBarSampleUtils.removeAppBars();
        }
    });

    function doClickBack() {
        WinJS.log && WinJS.log("Back button pressed", "sample", "status");
    }

})();
