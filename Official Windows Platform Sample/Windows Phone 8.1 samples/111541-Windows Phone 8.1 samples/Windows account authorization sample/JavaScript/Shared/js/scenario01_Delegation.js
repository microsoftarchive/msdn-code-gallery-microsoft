//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var page = WinJS.UI.Pages.define("/html/scenario01_delegation.html", {
        ready: function (element, options) {
            document.getElementById("signInButton").addEventListener("click", signInAndPomptIfNeeded, false);
            SdkSample.displayStatus("Delegation page loaded");
            SdkSample.displayAccountStatus();
            if (SdkSample.accountStatus.delegationTicket !== "") {
                showUserInfo();
                showUserContacts();
                showUserEvents();
            } else {
                signInAndPomptIfNeeded();
            }
        }
    });

    var URI_API_LIVE = "https://apis.live.net/v5.0/me";

    function signInAndPomptIfNeeded() {
        SdkSample.displayStatus("Signing in...");

        // Ticket target to be used
        var serviceTicketRequest = new Windows.Security.Authentication.OnlineId.OnlineIdServiceTicketRequest("wl.basic wl.contacts_photos wl.calendars", "DELEGATION");

        SdkSample.authenticator.authenticateUserAsync(serviceTicketRequest).done(function (authResult) {
                if (authResult.tickets[0].value !== "") {
                    SdkSample.accountStatus.isSignedIn = true;
                    SdkSample.accountStatus.delegationTicket = authResult.tickets[0].value;
                    SdkSample.displayStatus("Got Delegation ticket.");
                    SdkSample.displayAccountStatus();
                    if (SdkSample.accountStatus.delegationTicket !== "") {
                        retrieveUserInfo();
                        retrieveContacts();
                        retrieveActivity();
                    }
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

    function showTileItems(tile) {
        var items = document.querySelectorAll(tile + " .item");
        for (var i = 0, len = items.length; i < len; i++) {
            items[i].className = "item shown";
        }
    }

    function showUserInfo() {
        var me = SdkSample.accountStatus.delegationJsonUserInfo;
        if (me !== null) {
            document.getElementById("userName").innerText = me.name;
        } else {
            document.getElementById("userName").innerText = "";
        }
    }

    function showUserContacts() {
        var contacts = SdkSample.accountStatus.delegationJsonUserContacts;
        if (contacts !== null) {
            var contactsOutput = document.getElementById("contactsOutput");
            for (var i = 0, len = contacts.data.length; i < len; i++) {
                var contact = contacts.data[i];
                var contactText = document.createElement("div");
                contactText.innerText = contact.name;
                contactsOutput.appendChild(contactText);
            }
            showTileItems("#contactsTile");
        }
    }

    function showUserEvents() {
        var events = SdkSample.accountStatus.delegationJsonUserActivity;
        if (events !== null) {
            var eventsOutput = document.getElementById("eventsOutput");
            var eventData;
            var eventText;

            for (var i = 0, len = events.data.length; i < len; i++) {
                eventData = events.data[i];
                eventText = document.createElement("div");

                eventText.innerText = eventData.name + " - " + eventData.start_time;
                eventsOutput.appendChild(eventText);
            }
            showTileItems("#eventsTile");
        }
    }

    function retrieveUserInfo() {
        var url = URI_API_LIVE + "?access_token=" + SdkSample.accountStatus.delegationTicket;

        var httpRequest = new XMLHttpRequest();

        httpRequest.onreadystatechange = function () {
            if (httpRequest.readyState === 4 /* complete */) {
                if (httpRequest.status === 200) {
                    SdkSample.accountStatus.delegationJsonUserInfo = JSON.parse(httpRequest.responseText);
                    showUserInfo();
                } else {
                    SdkSample.displayError("HTTP error:" + httpRequest.status);
                }
            }
        };

        httpRequest.open("GET", url, true);
        httpRequest.send();
    }

    function retrieveContacts() {
        var url = URI_API_LIVE  + "/contacts?access_token=" + SdkSample.accountStatus.delegationTicket;
        var httpRequest = new XMLHttpRequest();
        httpRequest.onreadystatechange = function () {
            if (httpRequest.readyState === 4 /* complete */) {
                if (httpRequest.status === 200) {
                    SdkSample.accountStatus.delegationJsonUserContacts = JSON.parse(httpRequest.responseText);
                    showUserContacts();
                } else {
                    SdkSample.displayError("HTTP error:" + httpRequest.status);
                }
            }
        };

        httpRequest.open("GET", url, true);
        httpRequest.send();
    }

    function retrieveActivity() {
        var url = URI_API_LIVE + "/events?access_token=" + SdkSample.accountStatus.delegationTicket;
        var httpRequest = new XMLHttpRequest();
        httpRequest.onreadystatechange = function () {
            if (httpRequest.readyState === 4 /* complete */) {
                if (httpRequest.status === 200) {
                    SdkSample.accountStatus.delegationJsonUserActivity = JSON.parse(httpRequest.responseText);
                    showUserEvents();
                } else {
                    SdkSample.displayError("HTTP error:" + httpRequest.status);
                }
            }
        };

        httpRequest.open("GET", url, true);
        httpRequest.send();
    }
})();