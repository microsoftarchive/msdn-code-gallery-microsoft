//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/S4_Revoke.html", {
        ready: function (element, options) {
            document.getElementById("Revoke").addEventListener("click", doRevoke, false);
            SdkSample.validateFileExistence();
        }
    });

    function doRevoke() {
        if ("" === EnterpiseID.value)
        {
            WinJS.log && WinJS.log("Please enter an Enterpise ID that you want to use.", "sample", "error");
            return;
        }

        try{
            Windows.Security.EnterpriseData.FileRevocationManager.revoke(EnterpiseID.value);
            WinJS.log && WinJS.log("The Enterprise ID " + EnterpiseID.value + " was revoked. The files protected by it will not be accessible anymore.", "sample", "status");
        }
        catch ( e ){

            //
            // NOTE: Generally you should not rely on exception handling
            // to validate an Enterprise ID string. In real-world
            // applications, the domain name of the enterprise might be
            // parsed out of an email address or a URL, and may even be
            // entered by a user. Your app-specific code to extract the
            // Enterprise ID should validate the Enterprise ID string is an
            // internationalized domain name before passing it to
            // Revoke.
            //

            if (e.number === -2147024809) { //InvalidArgumentException
                WinJS.log && WinJS.log("Given Enterprise ID string is invalid.\n" +
                                       "Please try again using a properly formatted Internationalized Domain Name as the Enterprise ID string.", "sample", "error");
            }
            else {
                WinJS.log && WinJS.log("Revoke failed exception " + e, "sample", "error");
            }
        }
    }
})();
