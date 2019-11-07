//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/GetDomainName.html", {
        ready: function (element, options) {
            document.getElementById("GetDomain").addEventListener("click", GetDomain, false);
        }
    });

    function GetDomain() {
        Windows.System.UserProfile.UserInformation.getDomainNameAsync().done(function (result) {
            if (result) {
                WinJS.log && WinJS.log("DomainName = " + result, "sample", "status");
            } else {
                WinJS.log && WinJS.log("No domain name returned for the current user", "sample", "status");
            }
        }, function (error) {
            // The API will fail if the app does not have enterpriseAuthentication capability.
            // This error handling code can be removed once the capability is added to
            // the manifest.
            WinJS.log && WinJS.log("Error = " + error, "sample", "error");
        });
    }

})();
