//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    WinJS.Namespace.define("AppBarSampleUtils", {
        removeAppBars: function () {
            // Remove AppBar from previous scenario
            var otherAppBars = document.querySelectorAll('div[data-win-control="WinJS.UI.AppBar"]');
            var len = otherAppBars.length;
            for (var i = 0; i < len; i++) {
                var otherScenarioAppBar = otherAppBars[i];
                otherScenarioAppBar.parentNode.removeChild(otherScenarioAppBar);
            }
        }
    });

})();
