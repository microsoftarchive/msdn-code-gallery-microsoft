//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario9_PinTileAlternateVisualElements.html", {
        ready: function (element, options) {
            document.getElementById("pinButton").addEventListener("click", pinSecondaryTile, false);
        }
    });

    function pinSecondaryTile(sender) {
        var Scenario1TileId = "SecondaryTile.Logo";

        // We're pinning a secondary tile, so first we need to build up the tile.Pin
        // Start by creating the Uri assests.
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

        // Create a Secondary tile with all the required arguments and set the preferred size to be the medium tile size.
        var tile = new Windows.UI.StartScreen.SecondaryTile(Scenario1TileId,
            "Title text shown on the tile",
            newTileActivationArguments,
            square150x150Logo,
            Windows.UI.StartScreen.TileSize.square150x150);

        // Only support of the small and medium tile sizes is mandatory.
        // To have the larger tile sizes available the assets must be provided.
        tile.visualElements.wide310x150Logo = wide310x150Logo;
        tile.visualElements.square310x310Logo = square310x310Logo;

        // If the asset for the small tile size is not provided, it will be created by scaling down the medium tile size asset.
        // Thus, providing the asset for the small tile size is not mandatory, though is recommended to avoid scaling artifacts and can be overridden as shown below. 
        // Note that the asset for the small tile size must be explicitly provided if alternates for the small tile size are also explicitly provided.
        tile.visualElements.square70x70Logo = square70x70Logo;

        // Like the background color, the square30x30 logo is inherited from the parent application tile by default. 
        // Let's override it, just to see how that's done.
        tile.visualElements.square30x30Logo = square30x30Logo;

        // The display of the secondary tile name can be controlled for each tile size.
        // The default is false.
        tile.visualElements.showNameOnSquare150x150Logo = false;
        tile.visualElements.showNameOnWide310x150Logo = true;
        tile.visualElements.showNameOnSquare310x310Logo = true;

        // This is needed to add alternate tile options for a user to choose from for the supported tile sizes.
        tile.onvisualelementsrequested = visualElementsRequestedHandler;

        // Specify a foreground text value.
        // The tile background color is inherited from the parent unless a separate value is specified.
        tile.visualElements.foregroundText = Windows.UI.StartScreen.ForegroundText.dark;

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

    // Alternate tile options for the supported tile sizes are set in the handler.
    function visualElementsRequestedHandler(args)
    {
        // Prepare the images for the alternate tile options.
        var alternate1Square70x70Logo = new Windows.Foundation.Uri("ms-appx:///Images/alternate1Square70x70Tile-sdk.png");
        var alternate1Square150x150Logo = new Windows.Foundation.Uri("ms-appx:///Images/alternate1Square150x150Tile-sdk.png");
        var alternate1Wide310x150Logo = new Windows.Foundation.Uri("ms-appx:///Images/alternate1Wide310x150Tile-sdk.png");
        var alternate1Square310x310Logo = new Windows.Foundation.Uri("ms-appx:///Images/alternate1Square310x310Tile-sdk.png");

        var alternate2Square70x70Logo = new Windows.Foundation.Uri("ms-appx:///Images/alternate2Square70x70Tile-sdk.png");
        var alternate2Square150x150Logo = new Windows.Foundation.Uri("ms-appx:///Images/alternate2Square150x150Tile-sdk.png");
        var alternate2Wide310x150Logo = new Windows.Foundation.Uri("ms-appx:///Images/alternate2Wide310x150Tile-sdk.png");
        var alternate2Square310x310Logo = new Windows.Foundation.Uri("ms-appx:///Images/alternate2Square310x310Tile-sdk.png");

        var alternate3Square70x70Logo = new Windows.Foundation.Uri("ms-appx:///Images/alternate3Square70x70Tile-sdk.png");
        var alternate3Square150x150Logo = new Windows.Foundation.Uri("ms-appx:///Images/alternate3Square150x150Tile-sdk.png");
        var alternate3Wide310x150Logo = new Windows.Foundation.Uri("ms-appx:///Images/alternate3Wide310x150Tile-sdk.png");
        var alternate3Square310x310Logo = new Windows.Foundation.Uri("ms-appx:///Images/alternate3Square310x310Tile-sdk.png");

        // Set the assets for the alternate tile options.
        // Note that up to 3 sets of alternate options can be provided.
        // Each tile size in a given set will map to each other (for example, the second alternate medium tile option will map to the second alternate wide tile option).
        // Note that if alternates are provided for the small tile size (square70x70 logo), the default option for the small tile size must be overridden as shown earlier.
        // If neither the default option nor the alternates for the small tile size are provided, then all the medium tile size options will be scaled down to their corresponding small tile size options.
        args.request.alternateVisualElements[0].square70x70Logo = alternate1Square70x70Logo;
        args.request.alternateVisualElements[0].square150x150Logo = alternate1Square150x150Logo;
        args.request.alternateVisualElements[0].wide310x150Logo = alternate1Wide310x150Logo;
        args.request.alternateVisualElements[0].square310x310Logo = alternate1Square310x310Logo;

        // Set the BackgoundColor, ForegroundText, and ShowName propreties for the alternate set.
        args.request.alternateVisualElements[0].backgroundColor = Windows.UI.Colors.green;
        args.request.alternateVisualElements[0].foregroundText = Windows.UI.StartScreen.ForegroundText.dark;
        args.request.alternateVisualElements[0].showNameOnSquare310x310Logo = true;

        args.request.alternateVisualElements[1].square70x70Logo = alternate2Square70x70Logo;
        args.request.alternateVisualElements[1].square150x150Logo = alternate2Square150x150Logo;
        args.request.alternateVisualElements[1].wide310x150Logo = alternate2Wide310x150Logo;
        args.request.alternateVisualElements[1].square310x310Logo = alternate2Square310x310Logo;

        args.request.alternateVisualElements[1].backgroundColor = Windows.UI.Colors.darkViolet;
        args.request.alternateVisualElements[1].foregroundText = Windows.UI.StartScreen.ForegroundText.dark;
        args.request.alternateVisualElements[1].showNameOnWide310x150Logo = true;

        args.request.alternateVisualElements[2].square70x70Logo = alternate3Square70x70Logo;
        args.request.alternateVisualElements[2].square150x150Logo = alternate3Square150x150Logo;
        args.request.alternateVisualElements[2].wide310x150Logo = alternate3Wide310x150Logo;
        args.request.alternateVisualElements[2].square310x310Logo = alternate3Square310x310Logo;

        args.request.alternateVisualElements[2].backgroundColor = Windows.UI.Colors.red;
        args.request.alternateVisualElements[2].foregroundText = Windows.UI.StartScreen.ForegroundText.light;
        args.request.alternateVisualElements[2].showNameOnSquare150x150Logo = true;
    }
})();
