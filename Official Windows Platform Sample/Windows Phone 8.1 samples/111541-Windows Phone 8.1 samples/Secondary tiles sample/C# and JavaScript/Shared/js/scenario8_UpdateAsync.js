//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario8_UpdateAsync.html", {
        ready: function (element, options) {
            document.getElementById("updateDefaultLogo").addEventListener("click", updateDefaultLogo, false);
        }
    });

    function updateDefaultLogo() {
        var Scenario1TileId = "SecondaryTile.Logo";

        // An application can change the default logo of any Secondary Tile it has already pinned.
        // In order to change the default logo, the tile must of course exist.

        // We'll check if the tile exists using the same logic we used in Scenario 4, only this time we'll do something more interesting.

        if (Windows.UI.StartScreen.SecondaryTile.exists(Scenario1TileId)) {
            var tileToUpdate = new Windows.UI.StartScreen.SecondaryTile(Scenario1TileId);
            var uriUpdatedLogo = new Windows.Foundation.Uri("ms-appx:///images/squareTileLogoUpdate-sdk.png");
            tileToUpdate.visualElements.square150x150Logo = uriUpdatedLogo;
            tileToUpdate.updateAsync().done(function (Updated) {
                if (Updated) {
                    WinJS.log && WinJS.log("Secondary tile default logo updated.", "sample", "status");
                } else {
                    WinJS.log && WinJS.log("Secondary tile default logo was not updated.", "sample", "error");
                }
            });
        } else {
            WinJS.log && WinJS.log("To complete this scenario, first pin the secondary tile in Scenario 1.", "sample", "error");
        }
    }
})();
