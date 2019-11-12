//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/GetPrincipalName.html", {
        ready: function (element, options) {
            document.getElementById("GetPrincipal").addEventListener("click", GetPrincipal, false);
        }
    });

    function GetPrincipal() {
        Windows.System.UserProfile.UserInformation.getPrincipalNameAsync().done(function (result) {
            if (result) {
                WinJS.log && WinJS.log("PrincipalName = " + result, "sample", "status");
            } else {
                WinJS.log && WinJS.log("No principal name returned for current user", "sample", "status");
            }
        }, function (error) {
            // The API will fail if the app does not have enterpriseAuthentication capability.
            // This error handling code can be removed once the capability is added to
            // the manifest.
            WinJS.log && WinJS.log("Error = " + error, "sample", "error");
        });
    }
})();
