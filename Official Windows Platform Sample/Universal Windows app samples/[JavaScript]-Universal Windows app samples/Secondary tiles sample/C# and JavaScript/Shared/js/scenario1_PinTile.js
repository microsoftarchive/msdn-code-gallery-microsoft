//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario1_PinTile.html", {
        ready: function (element, options) {
            document.getElementById("pinButton").addEventListener("click", pinSecondaryTile, false);
        }
    });

    function pinSecondaryTile() {
        var Scenario1TileId = "SecondaryTile.Logo";

        // Prepare package images for all four tile sizes in our tile to be pinned as well as for the square30x30 logo used in the Apps view.
        var square70x70Logo = new Windows.Foundation.Uri("ms-appx:///Images/square70x70Tile-sdk.png");
        var square150x150Logo = new Windows.Foundation.Uri("ms-appx:///Images/square150x150Tile-sdk.png");
        var wide310x150Logo = new Windows.Foundation.Uri("ms-appx:///Images/wide310x150Tile-sdk.png");
        var square310x310Logo = new Windows.Foundation.Uri("ms-appx:///Images/square310x310Tile-sdk.png");
        var square30x30Logo = new Windows.Foundation.Uri("ms-appx:///Images/square30x30Tile-sdk.png");

        // If a user launches the sample through the pinned secondary tile, the app will want to respond appropriately.
        // In this case, we'll use activation arguments to tell that the app was launched from that secondary tile.
        // Responding appropriately, for the purpose of this sample, will mean displaying the activation arguments passed.
        // We'll set the time the tile was pinned as the activation argument.

        // Get the current time...
        var currentTime = new Date();

        // Now, to make this explicit and highly readable, assign current time plus some name to a variable called tileActivationArguments
        var newTileActivationArguments = Scenario1TileId  + " WasPinnedAt=" + currentTime;

        // Create a Secondary tile with all the required arguments.
        // Note the last argument specifies what size the Secondary tile should show up as by default in the Pin to start fly out.
        // It can be set to TileSize.Square150x150, TileSize.Wide310x150, or TileSize.Default.  
        // If set to TileSize.Wide310x150, then the asset for the wide size must be supplied as well.
        // TileSize.Default will default to the wide size if a wide size is provided, and to the medium size otherwise.
        var tile = new Windows.UI.StartScreen.SecondaryTile(Scenario1TileId,
            "Title text shown on the tile",
            newTileActivationArguments,
            square150x150Logo,
            Windows.UI.StartScreen.TileSize.Square150x150);

        // Only support of the small and medium tile sizes is mandatory.
        // To have the larger tile sizes available the assets must be provided.
        tile.visualElements.wide310x150Logo = wide310x150Logo;
        tile.visualElements.square310x310Logo = square310x310Logo;

        // If the asset for the small tile size is not provided, it will be created by scaling down the medium tile size asset.
        // Thus, providing the asset for the small tile size is not mandatory, though is recommended to avoid scaling artifacts and can be overridden as shown below.
        tile.visualElements.square70x70Logo = square70x70Logo;

        // Like the background color, the square30x30 logo is inherited from the parent application tile by default.
        // Let's override it, just to see how that's done.
        tile.visualElements.square30x30Logo = square30x30Logo;

        // The display of the secondary tile name can be controlled for each tile size.
        // The default is false.
        tile.visualElements.showNameOnSquare150x150Logo = true;
        tile.visualElements.showNameOnWide310x150Logo = true;
        tile.visualElements.showNameOnSquare310x310Logo = true;

        // Specify a foreground text value.
        // The tile background color is inherited from the parent unless a separate value is specified.
        tile.visualElements.foregroundText = Windows.UI.StartScreen.ForegroundText.dark;

        // Set this to false if roaming doesn't make sense for the secondary tile.
        // The default is true;
        tile.roamingEnabled = false;

        // Now, we're almost ready to try and pin the tile.
        // when we make the pin request, Windows will show a flyout asking for user confirmation.
        // To make this predictable and meet user expectations, we want to consistently place the flyout near the button where the flyout was invoked.
        // Grab the bounding rectangle of the pin button.
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
