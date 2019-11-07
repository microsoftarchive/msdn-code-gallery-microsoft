//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/jwt.html", {
        ready: function (element, options) {
            document.getElementById("signInButton").addEventListener("click", signInAndPomptIfNeeded, false);
            document.getElementById("refreshButton").addEventListener("click", sendJwtTicketToServer, false);

            SdkSample.displayStatus("Delegation page loaded");
            SdkSample.displayAccountStatus();

            if (SdkSample.accountStatus.jwtTicket !== "") {
                showServerResponse();
            } else {
                signInAndPomptIfNeeded();
            }
        }
    });

    function showServerResponse() {
        document.getElementById("serverResponseResponse").innerText = SdkSample.accountStatus.jwtServerResponse;
    }

    function sendJwtTicketToServer() {
        SdkSample.displayStatus("Sending JWT ticket to server");

        // look in the project's .\Server folder how to deploy test site;
        // if there is a HTTP proxy, make sure the sample is configured properly;
        // in production, you should use your own web host with https:// instead of http://localhost
        var url = "http://localhost/JsonWebTokenSample/Default.aspx?access_token=" + SdkSample.accountStatus.delegationTicket;

        var httpRequest = new XMLHttpRequest();

        httpRequest.onreadystatechange = function () {
            if (httpRequest.readyState === 4 /* complete */) {
                if (httpRequest.status === 200) {
                    SdkSample.accountStatus.jwtServerResponse = httpRequest.responseText;
                    showServerResponse();
                } else {
                    SdkSample.displayError("HTTP error:" + httpRequest.status);
                }
            }
        };

        httpRequest.open("GET", url, true);
        httpRequest.send();
    }

    function signInAndPomptIfNeeded() {
        SdkSample.displayStatus("Signing in...");

        // Ticket target to be used
        var serviceTicketRequest = new Windows.Security.Authentication.OnlineId.OnlineIdServiceTicketRequest("jsonwebtokensample.com", "JWT");

        SdkSample.authenticator.authenticateUserAsync(serviceTicketRequest).done(function (authResult) {
                if (authResult.tickets[0].value !== "") {
                    SdkSample.accountStatus.isSignedIn = true;
                    SdkSample.accountStatus.jwtTicket = authResult.tickets[0].value;
                    SdkSample.displayStatus("Got JWT ticket.");
                    SdkSample.displayAccountStatus();
                    if (SdkSample.accountStatus.jwtTicket !== "") {
                        sendJwtTicketToServer();
                    }
                }
            }, function (authStatus) {
                if (authStatus.name === "Canceled") {
                    SdkSample.displayStatus("Canceled");
                } else {
                    SdkSample.displayError("Autorization failed: " + authStatus.message);
                }

                SdkSample.accountStatus.isSignedIn = false;
                SdkSample.displayAccountStatus();
            });
    }
})();
