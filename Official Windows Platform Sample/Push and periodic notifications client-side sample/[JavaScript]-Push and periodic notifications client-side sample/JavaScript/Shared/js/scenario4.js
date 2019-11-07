//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var notifications = Windows.UI.Notifications;

    var page = WinJS.UI.Pages.define("/html/scenario4.html", {
        ready: function (element, options) {
            document.getElementById("startTilePolling").addEventListener("click", startTilePolling, false);
            document.getElementById("stopTilePolling").addEventListener("click", stopTilePolling, false);

            // IMPORTANT NOTE: call this only if you plan on polling several different URLs, and only
            // once after the user installs the app or creates a secondary tile
            notifications.TileUpdateManager.createTileUpdaterForApplication().enableNotificationQueue(true);
        }
    });

    function startTilePolling() {
        var inputBoxes = document.querySelectorAll(".batch-url");

        var urisToPoll = [];
        for (var i = 0, len = inputBoxes.length; i < len; i++) {
            var polledUrl = inputBoxes[i].value;

            // The default value of this text box is "http://".
            // Make sure the user entered some data.
            if (polledUrl !== "http://" && polledUrl !== "") {
                urisToPoll.push(new Windows.Foundation.Uri(polledUrl));
            }
        }

        var recurrence = notifications.PeriodicUpdateRecurrence[document.getElementById("tilePeriodicRecurrence").value];

        if (urisToPoll.length === 1) {
            notifications.TileUpdateManager.createTileUpdaterForApplication().startPeriodicUpdate(urisToPoll[0], recurrence);
            WinJS.log && WinJS.log("Started polling " + urisToPoll[0].displayUri + ". Look at the application’s tile on the Start menu to see the latest update.", "sample", "status");
        } else if (urisToPoll.length > 1) {
            notifications.TileUpdateManager.createTileUpdaterForApplication().startPeriodicUpdateBatch(urisToPoll, recurrence);
            WinJS.log && WinJS.log("Started polling the specified URLs. Look at the application’s tile on the Start menu to see the latest update.", "sample", "status");
        } else {
            WinJS.log && WinJS.log("Specify a URL that returns tile XML to begin tile polling.", "sample", "error");
        }
    }

    function stopTilePolling() {
        notifications.TileUpdateManager.createTileUpdaterForApplication().stopPeriodicUpdate();

        WinJS.log && WinJS.log("Stopped polling.", "sample", "status");
    }

})();
