//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var notifications = Windows.UI.Notifications;

    var page = WinJS.UI.Pages.define("/html/scenario5.html", {
        ready: function (element, options) {
            document.getElementById("startBadgePolling").addEventListener("click", startBadgePolling, false);
            document.getElementById("stopBadgePolling").addEventListener("click", stopBadgePolling, false);
        }
    });

    function startBadgePolling() {
        var polledUrl = document.getElementById("badgePollingURL").value;

        // The default value of this text box is "http://".
        // Make sure the user entered some data.
        if (polledUrl !== "http://") {
            var badgeRecurrence = notifications.PeriodicUpdateRecurrence[document.getElementById("badgePeriodicRecurrence").value];

            notifications.BadgeUpdateManager.createBadgeUpdaterForApplication().startPeriodicUpdate(new Windows.Foundation.Uri(polledUrl), badgeRecurrence);

            WinJS.log && WinJS.log("Started polling " + polledUrl + ". Look at the application’s tile on the Start menu to see the latest update.", "sample", "status");
        } else {
            WinJS.log && WinJS.log("Specify a URL that returns badge XML to begin badge polling.", "sample", "error");
        }
    }

    function stopBadgePolling() {
        notifications.BadgeUpdateManager.createBadgeUpdaterForApplication().stopPeriodicUpdate();

        WinJS.log && WinJS.log("Stopped polling.", "sample", "status");
    }
})();
