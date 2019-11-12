//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario3_EnumerateTiles.html", {
        ready: function (element, options) {
            document.getElementById("enumerateTiles").addEventListener("click", enumerateTiles, false);
        }
    });

    function enumerateTiles() {
        // This scenario illustrates how to retrieve a list of all tiles pinned by the application.
        //
        // Tile enumeration is important whenever we're working with Secondary Tiles. One case where this is specifically
        // important is when renewing channels URIs for tile push notifications. Notification channels expire after 30 days.
        // During the lifetime of an application, the app will need to periodically enumerate its tiles and renew notification
        // channels for every secondary tile it wants to send push notifications to. Alternatively, the app will want to
        // call startPeriodicUpdate to update its tiles.
        //
        // We'll start by assuming no tiles are pinned, and then update our status later if we're wrong.
        WinJS.log && WinJS.log("No secondary tiles detected...", "sample", "error");

        // Get secondary tile IDs for all apps in the package and list them:
        Windows.UI.StartScreen.SecondaryTile.findAllAsync().done(function (tiles) {
            if (tiles) {
                tiles.forEach(function (tile) {
                    var myTileId = tile.tileId;
                    document.getElementById("enumerateTilesOutput").innerHTML += "<p>" + myTileId + "</p>";
                });
                WinJS.log && WinJS.log("List of secondary tiles by ID:", "sample", "status");
            }
        });
    }
})();
