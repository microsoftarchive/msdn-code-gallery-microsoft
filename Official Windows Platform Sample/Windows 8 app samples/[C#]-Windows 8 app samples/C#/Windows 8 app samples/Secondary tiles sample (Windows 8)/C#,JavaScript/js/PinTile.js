//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/PinTile.html", {
        ready: function (element, options) {
            document.getElementById("pinButton").addEventListener("click", pinSecondaryTile, false);
        }
    });

    function pinSecondaryTile() {
        var Scenario1TileId = "SecondaryTile.Logo";
        // We're pinning a secondary tile, so first we need to build up the tile.Pin
        // Start by assigning a logo and small logo for the tile.
        var uriLogo = new Windows.Foundation.Uri("ms-appx:///images/SecondaryTileDefault-sdk.png");
        var uriSmallLogo = new Windows.Foundation.Uri("ms-appx:///images/smallLogoSecondaryTile-sdk.png");

        // If a user launches the sample through the pinned secondary tile, the app will want to respond appropriately.
        // In this case, we'll use activation arguments to tell that the app was launched from that secondary tile.
        // Responding appropriately, for the purpose of this sample, will mean displaying the activation arguments passed.
        // We'll set the time the tile was pinned as the activation argument.

        // Get the current time...
        var currentTime = new Date();

        // Now, to make this explicit and highly readable, assign current time plus some name to a variable called tileActivationArguments
        var newTileActivationArguments = Scenario1TileId  + " WasPinnedAt=" + currentTime;

        // And we're ready to construct our tile!
        // Pass in all the details we specified above into the constructor...
        var tile = new Windows.UI.StartScreen.SecondaryTile(Scenario1TileId,
            "Title text shown on the tile",
            "Name of the tile the user sees when searching for the tile",
            newTileActivationArguments,
            Windows.UI.StartScreen.TileOptions.showNameOnLogo,
            uriLogo);

        // Now that our tile is created, let's set the the foreground text value in order to make the text stand out from the tile background.
        tile.foregroundText = Windows.UI.StartScreen.ForegroundText.dark;

        // Let's also set the small logo, since it's not possible to set that in the constructor
        tile.smallLogo = uriSmallLogo;


        // Now, we're almost ready to try and pin the tile.
        // when we make the pin request, Windows will show a flyout asking for user confirmation.
        // To make this predictable and meet user expectations, we want to consistently place the flyout near the button where the flyout was invoked.
        // Grab the bounding rectangle of the element that was passed in...
        var selectionRect = document.getElementById("pinButton").getBoundingClientRect();

        // Using the bounding rectangle of the element that was used to invoke the flyout, we will be able to derive coordinates to show the flyout over.
        // We now are ready to try and pin the tile.
        tile.requestCreateForSelectionAsync({ x: selectionRect.left, y: selectionRect.top, width: selectionRect.width, height: selectionRect.height }, Windows.UI.Popups.Placement.below).done(function (isCreated) {
            if (isCreated) {
                // The tile was successfully created, so we'll display a status message
                WinJS.log && WinJS.log("Secondary tile was successfully pinned.", "sample", "status");
            } else {
                // something happened - either the user cancelled, or an error was encountered.
                // Either way, the flyout is now gone, and we'll react the same way.
                WinJS.log && WinJS.log("Secondary tile was not pinned.", "sample", "error");
            }
        });
    }
})();
