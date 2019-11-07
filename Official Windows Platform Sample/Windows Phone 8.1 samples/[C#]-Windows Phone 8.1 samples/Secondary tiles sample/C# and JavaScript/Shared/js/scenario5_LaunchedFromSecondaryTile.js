//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario5_LaunchedFromSecondaryTile.html", {
        // During an initial activation this event is called before the system splash screen is torn down.
        // Do any initialization work that is necessary to set up the initial UI.
        processed: function (element, options) {
            if (options) {
                document.getElementById("launchedFromSecondaryTileOutput").innerHTML += "<p>" + "App was activated from a secondary tile with the following activation arguments : " + options + "</p>";
            }
        },
    // During an initial activation this event is called after the system splash screen is torn down.
    // Do any initialization work that is not related to getting the initial UI set up.
        ready: function (element, options) {
        }
    });
})();
