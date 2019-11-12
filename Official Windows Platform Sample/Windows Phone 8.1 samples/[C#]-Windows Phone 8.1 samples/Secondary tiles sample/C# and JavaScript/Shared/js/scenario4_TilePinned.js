//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario4_TilePinned.html", {
        ready: function (element, options) {
            document.getElementById("tileExists").addEventListener("click", doesTileExist, false);
        }
    });

    function doesTileExist() {
        var Scenario1TileId = "SecondaryTile.Logo";
        // In this scenario we're going to explicitly check if the tile we pinned in Scenario 1 is currently pinned.
        // This is useful for all sorts of reasons. We'll see two explicit cases where this knowledge is useful
        // in Scenario pushNotification and pinFromAppbar.

        if (Windows.UI.StartScreen.SecondaryTile.exists(Scenario1TileId)) {
            WinJS.log && WinJS.log("The secondary tile from Scenario 1 is pinned.", "sample", "status");
        } else {
            WinJS.log && WinJS.log("The secondary tile from Scenario 1 is not pinned.", "sample", "error");
        }
    }
})();
