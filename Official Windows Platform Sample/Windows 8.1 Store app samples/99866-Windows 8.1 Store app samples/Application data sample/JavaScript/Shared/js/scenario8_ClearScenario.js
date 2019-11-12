//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario8_ClearScenario.html", {
        ready: function (element, options) {
            document.getElementById("clear").addEventListener("click", clear, false);
        }
    });

    function clear() {
        Windows.Storage.ApplicationData.current.clearAsync().done(function () {
            document.getElementById("clearOutput").innerText = "ApplicationData has been cleared.  Visit the other scenarios to see that their data has been cleared.";
        }, function (err) {
            document.getElementById("clearOutput").innerText = "Unable to clear settings, make sure all files are closed.";
        });
    }
})();
