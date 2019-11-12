//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var output;

    var page = WinJS.UI.Pages.define("/html/scenario6.html", {
        ready: function (element, options) {
            output = document.getElementById('output');
            WinJS.Resources.processAll(output);
            WinJS.Resources.addEventListener("contextchanged", refresh, false);
        },
        unload: function () {
            WinJS.Resources.removeEventListener("contextchanged", refresh, false);
        }
    });

    function refresh() {
        WinJS.Resources.processAll(output); // Refetch string resources
    }
})();
