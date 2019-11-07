//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/provide-information.html", {
        ready: function (element, options) {
            document.getElementById("moreInfoLink").addEventListener("click", showMoreInfoFlyout, false);
            document.getElementById("moreInfoFlyout").addEventListener("afterhide", onDismiss, false);
        }
    });

    // Show the flyout
    function showMoreInfoFlyout() {
        WinJS.log && WinJS.log("The flyout was shown.", "sample", "status");

        var moreInfoLink = document.getElementById("moreInfoLink");
        document.getElementById("moreInfoFlyout").winControl.show(moreInfoLink);
    }

    // This function runs when the flyout is dismissed
    function onDismiss() {
        WinJS.log && WinJS.log("The flyout was dismissed.", "sample", "status");
    }

})();
