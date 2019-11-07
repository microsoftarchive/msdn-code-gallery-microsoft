//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/GetSIPUri.html", {
        ready: function (element, options) {
            document.getElementById("GetUri").addEventListener("click", GetUri, false);
        }
    });

    function GetUri() {
        Windows.System.UserProfile.UserInformation.getSessionInitiationProtocolUriAsync().done(function (result) {
            if (result) {
                WinJS.log && WinJS.log("Raw SIPUri = " + result.rawUri + "\nSIP email = " + result.userName + "@" + result.domain, "sample", "status");
            } else {
                WinJS.log && WinJS.log("No SIP Uri returned for current user", "sample", "status");
            }
        }, function (error) {
            // The API will fail if the app does not have enterpriseAuthentication capability.
            // This error handling code can be removed once the capability is added to
            // the manifest.
            WinJS.log && WinJS.log("Error = " + error, "sample", "error");
        });
    }
})();
