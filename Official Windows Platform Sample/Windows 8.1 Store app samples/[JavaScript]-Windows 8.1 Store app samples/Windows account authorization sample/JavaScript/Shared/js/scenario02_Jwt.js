//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario02_jwt.html", {
        ready: function (element, options) {
            document.getElementById("signInButton").addEventListener("click", signInAndPomptIfNeeded, false);

            SdkSample.displayStatus("JWT page loaded");
            SdkSample.displayAccountStatus();

            if (SdkSample.accountStatus.jwtTicket === "") {
                signInAndPomptIfNeeded();
            }
        }
    });

    function signInAndPomptIfNeeded() {
        SdkSample.displayStatus("Signing in...");

        // Ticket target to be used
        var serviceTicketRequest = new Windows.Security.Authentication.OnlineId.OnlineIdServiceTicketRequest("jsonwebtokensample2.com", "JWT");


        SdkSample.authenticator.authenticateUserAsync(serviceTicketRequest).done(function (authResult) {
                if (authResult.tickets[0].value !== "") {
                    SdkSample.accountStatus.isSignedIn = true;
                    SdkSample.accountStatus.jwtTicket = authResult.tickets[0].value;
                    SdkSample.displayStatus("Got JWT ticket.");
                    SdkSample.displayAccountStatus();
                }
            }, function (authStatus) {
                if (authStatus.name === "Canceled") {
                    SdkSample.displayStatus("Canceled");
                } else {
                    SdkSample.displayError("Authorization failed: " + authStatus.message);
                }

                SdkSample.accountStatus.isSignedIn = false;
                SdkSample.displayAccountStatus();
            });
    }
})();