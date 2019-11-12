//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario2_UnpinTile.html", {
        ready: function (element, options) {
            document.getElementById("unpinButton").addEventListener("click", unpinTile, false);
        }
    });

    function unpinTile() {
        var Scenario1TileId = "SecondaryTile.Logo";

        if (Windows.UI.StartScreen.SecondaryTile.exists(Scenario1TileId)) {

            // Unpinning a tile is very straightforward. All we need is the ID of the tile we're unpinning.
            // In this case, the tile is SecondaryTile.tilePinnedInScenario1, the tile we pinned in scenario 1,
            // so let's go ahead and create that.
            var tileToDelete = new Windows.UI.StartScreen.SecondaryTile(Scenario1TileId);

            // Now, we're almost ready to try and unpin the tile.
            // when we make the upin request, Windows will show a flyout asking for user confirmation.
            // To make this predictable and meet user expectations, we want to consistently place the flyout near the button where the flyout was invoked.
            // Grab the bounding rectangle of the element that was passed in...
            // Windows Phone: the selection rect will have no effect on the phone but the same code may be used.
            var selectionRect = document.getElementById("unpinButton").getBoundingClientRect();

            // Now we make the delete request, passing a point built from the bounding client rectangle.
            tileToDelete.requestDeleteForSelectionAsync({ x: selectionRect.left, y: selectionRect.top, width: selectionRect.width, height: selectionRect.height }, Windows.UI.Popups.Placement.below).done(function (isDeleted) {
                if (isDeleted) {
                    // The tile was successfully deleted, so we'll display a status message
                    WinJS.log && WinJS.log("Secondary tile was successfully unpinned.", "sample", "status");
                } else {
                    // something happened - either the user cancelled, or an error was encountered.
                    // Either way, the flyout is now gone, and we'll react the same way.
                    WinJS.log && WinJS.log("Secondary tile was not unpinned.", "sample", "error");
                }
            });
        }
        else {
            WinJS.log && WinJS.log("Tile not found, please use Secnario 1 to pin a tile.", "sample", "error");
        }
    }
})();
