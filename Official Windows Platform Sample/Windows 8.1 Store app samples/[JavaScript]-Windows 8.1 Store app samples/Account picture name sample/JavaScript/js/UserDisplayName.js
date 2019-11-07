//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/UserDisplayName.html", {
        ready: function (element, options) {
            document.getElementById("getDisplayName").addEventListener("click", getDisplayName, false);
        }
    });

    function getDisplayName() {
        Windows.System.UserProfile.UserInformation.getDisplayNameAsync().done(function (result) {
            if (result) {
                WinJS.log && WinJS.log("Display Name = " + result, "sample", "status");
            } else {
                WinJS.log && WinJS.log("No Display Name was returned.", "sample", "status");
            }
        });
    }
})();
