//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "Microsoft account Authentication Sample";

    var scenarios = [
        { url: "/html/scenario01_delegation.html", title: "Delegation Ticket" },
        { url: "/html/scenario02_jwt.html", title: "JWT Ticket" },
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
        scenarios: new WinJS.Binding.List(scenarios),
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
})();