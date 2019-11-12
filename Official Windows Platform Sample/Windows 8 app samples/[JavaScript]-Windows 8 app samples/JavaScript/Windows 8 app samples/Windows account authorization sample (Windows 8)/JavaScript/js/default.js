//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Microsoft Account Authentication Sample";

    var scenarios = [
        { url: "/html/delegation.html", title: "Delegation Ticket" },
        { url: "/html/Jwt.html", title: "JWT Ticket" },
    ];

    var accountStatus = {
        isSignedIn: false,
        delegationTicket: "",
        delegationJsonUserInfo: null,
        delegationJsonUserContacts: null,
        delegationJsonUserActivity: null,
        jwtTicket: "",
        jwtServerResponse: ""
    };

    var authenticator = new Windows.Security.Authentication.OnlineId.OnlineIdAuthenticator();

    function activated(eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            // Use setPromise to indicate to the system that the splash screen must not be torn down
            // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                // Navigate to either the first scenario or to the last running scenario
                // before suspension or termination.
                var url = WinJS.Application.sessionState.lastUrl || scenarios[0].url;
                return WinJS.Navigation.navigate(url);
            }));
        }
    }

    WinJS.Navigation.addEventListener("navigated", function (eventObject) {
        var url = eventObject.detail.location;
        var host = document.getElementById("contentHost");
        // Call unload method on current scenario, if there is one
        host.winControl && host.winControl.unload && host.winControl.unload();
        WinJS.Utilities.empty(host);
        eventObject.detail.setPromise(WinJS.UI.Pages.render(url, host, eventObject.detail.state).then(function () {
            WinJS.Application.sessionState.lastUrl = url;
        }));
    });

    function displayStatus(message) {
        WinJS.log && WinJS.log(message, "sample", "status");
    }

    function displayError(message) {
        WinJS.log && WinJS.log(message, "sample", "error");
    }

    function resetOutput() {
        var items = document.querySelectorAll("#outputTiles .item");
        for (var i = 0, len = items.length; i < len; i++) {
            items[i].innerText = "";
        }
    }

    function displayAccountStatus() {
        var styleDisplay;

        var logo = document.getElementById("userPicture");
        if (logo) {
            logo.width = 64;
            logo.height = 64;
            if (SdkSample.accountStatus.delegationTicket !== "") {
                logo.src = "https://apis.live.net/v5.0/me/picture?access_token=" + SdkSample.accountStatus.delegationTicket;
            } else {
                logo.src = "/images/user.png";
            }
        }

        if (SdkSample.accountStatus.isSignedIn) {
            styleDisplay = "none";
        } else {
            styleDisplay = "block";
            resetOutput();
        }

        var signInButton = document.getElementById("signInButton");
        if (signInButton) {
            signInButton.style.display = styleDisplay;
        }
    }

    function signOut() {
        if (SdkSample.authenticator.canSignOut) {
            SdkSample.displayStatus("Signing out...");
            SdkSample.authenticator.signOutUserAsync().done(function () {
                SdkSample.accountStatus.delegationTicket = "";
                SdkSample.accountStatus.isSignedIn = false;
                SdkSample.displayAccountStatus();
                SdkSample.displayStatus("Signed out.");
            });
        }
    }

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: scenarios,
        authenticator: authenticator,
        accountStatus: accountStatus,
        displayAccountStatus: displayAccountStatus,
        displayStatus: displayStatus,
        displayError: displayError,
        signOut: signOut
    });

    var page = WinJS.UI.Pages.define("/html/AppSettings.html", {
        ready: function (element, options) {
            var i;
            var items;
            var len;

            if (SdkSample.accountStatus.isSignedIn && SdkSample.authenticator.canSignOut) {
                items = document.querySelectorAll("#signOut .item");
                for (i = 0, len = items.length; i < len; i++) {
                    items[i].style.display = "block";
                }
                items = document.querySelectorAll("#cannotSignOut .item");
                for (i = 0, len = items.length; i < len; i++) {
                    items[i].style.display = "none";
                }
            } else {
                items = document.querySelectorAll("#signOut .item");
                for (i = 0, len = items.length; i < len; i++) {
                    items[i].style.display = "none";
                }
                items = document.querySelectorAll("#cannotSignOut .item");
                for (i = 0, len = items.length; i < len; i++) {
                    items[i].style.display = "block";
                }
            }
        }
    });

    // Add Settings handler
    WinJS.Application.onsettings = function (e) {
        e.detail.applicationcommands = { "settingsDiv": { title: "Account Settings", href: "/html/AppSettings.html" } };
        WinJS.UI.SettingsFlyout.populateSettings(e);
    };

    WinJS.Application.addEventListener("activated", activated, false);
    WinJS.Application.start();
})();
