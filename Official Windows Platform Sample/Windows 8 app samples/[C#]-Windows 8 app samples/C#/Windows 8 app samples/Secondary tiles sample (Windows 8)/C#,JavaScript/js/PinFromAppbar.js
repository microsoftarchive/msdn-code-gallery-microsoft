//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var appbarTileId = "SecondaryTile.AppBar";
    var page = WinJS.UI.Pages.define("/html/PinFromAppbar.html", {
        ready: function (element, options) {
            // enableAppBar is a helper that sets up the app bar in this scenario.
            enableAppbar();
            document.getElementById("commandButton").addEventListener("click", appbarButtonClicked, false);
            WinJS.log && WinJS.log("To show the bar swipe up from the bottom of the screen, right-click, or press Windows Logo + z. To dismiss the bar, swipe, right-click, or press Windows Logo + z again.", "sample", "status");
        },
        unload: function () {
            // Remove app bar from previous scenario
            var otherAppBars = document.querySelectorAll('div[data-win-control="WinJS.UI.AppBar"]');
            for (var i = 0, len = otherAppBars.length; i < len; i++) {
                var otherScenarioAppBar = otherAppBars[i];
                otherScenarioAppBar.parentNode.removeChild(otherScenarioAppBar);
            }
        }
    });

    function pinByElementAsync(element, newTileID, newTileShortName, newTileDisplayName) {

        // Sometimes it's convenient to create our tile and pin it, all in a single asynchronous call.

        // We're pinning a secondary tile, so let's build up the tile just like we did in pinByElement
        var uriLogo = new Windows.Foundation.Uri("ms-appx:///images/SecondaryTileDefault-sdk.png");
        var uriSmallLogo = new Windows.Foundation.Uri("ms-appx:///images/smallLogoSecondaryTile-sdk.png");
        var currentTime = new Date();
        var TileActivationArguments = newTileID + " WasPinnedAt=" + currentTime;
        var tile = new Windows.UI.StartScreen.SecondaryTile(newTileID, newTileShortName, newTileDisplayName, TileActivationArguments, Windows.UI.StartScreen.TileOptions.showNameOnLogo, uriLogo);
        tile.foregroundText = Windows.UI.StartScreen.ForegroundText.light;
        tile.smallLogo = uriSmallLogo;


        // Let's place the focus rectangle near the button, just like we did in pinByElement
        var selectionRect = element.getBoundingClientRect();

        // Now let's try to pin the tile.
        // We'll make the same fundamental call as we did in pinByElement, but this time we'll return a promise.
        return new WinJS.Promise(function (complete, error, progress) {
            tile.requestCreateForSelectionAsync({ x: selectionRect.left, y: selectionRect.top, width: selectionRect.width, height: selectionRect.height }, Windows.UI.Popups.Placement.above).done(function (isCreated) {
                if (isCreated) {
                    complete(true);
                } else {
                    complete(false);
                }
            });
        });
    }

    function unpinByElementAsync(element, unwantedTileID) {

        // Sometimes it's convenient to get ready to delete our tile and then try to unpin it, all in a single asynchronous call.

        // element is the html element on the page where the unpin request was initiated.
        // We want to grab the coordinates of that element in order to properly position the confirmation flyout.
        // We'll use the bounding client rectangle of the element to get those coordinates.
        var selectionRect = element.getBoundingClientRect();

        // In this example, we're going to delete a tile named SecondaryTile.01. In order to tell the Unpin
        // API which tile we want to delete, we need to construct a SecondaryTile with that name.
        var tileToGetDeleted = new Windows.UI.StartScreen.SecondaryTile(unwantedTileID);

        // Now we make the delete request, passing a point built from the bounding client rectangle.
        // Note that this is an async call, and we'll return a promise so we can do additional work when it's complete.

        return new WinJS.Promise(function (complete, error, progress) {
            tileToGetDeleted.requestDeleteForSelectionAsync({ x: selectionRect.left, y: selectionRect.top, width: selectionRect.width, height: selectionRect.height }, Windows.UI.Popups.Placement.above).done(function (isDeleted) {
                if (isDeleted) {
                    complete(true);
                } else {
                    complete(false);
                }
            });
        });
    }

    function setAppbarButton() {

        // We want the command bar to show either the pin or unpin command on the app bar, depending on whether the tile exists.
        // To do this, we'll check if the tile exists, and then we'll set the app bar command and icon appropriately.

        // Step 1: Let's apply skills learned in "IsTiledPinned" scenario and check if the tile exists
        if (Windows.UI.StartScreen.SecondaryTile.exists(appbarTileId)) {
            // Step 2: the tile exists, so set the command label to unpin and the appropriate icon:
            document.getElementById("commandButton").winControl.label = "Unpin from Start";
            document.getElementById("commandButton").winControl.icon = "unpin";
            document.getElementById("commandButton").winControl.tooltip = "Unpin from Start";
        } else {
            // Step 2b: the tile does not exist, so set the command label to pin and the appropriate icon:
            document.getElementById("commandButton").winControl.label = "Pin to Start";
            document.getElementById("commandButton").winControl.icon = "pin";
            document.getElementById("commandButton").winControl.tooltip = "Pin to Start";
        }

        document.getElementById("pinUnpinFromAppbar").winControl.sticky = false;
    }

    function enableAppbar() {
        WinJS.log && WinJS.log("To show the app bar, swipe up from the bottom of the screen, right-click, or press Windows key+Z. To dismiss the bar, tap in the application, swipe, right-click, or press Windows key+Z." ,
                               "sample", "status");


        // Enable the app bar so it will respond to edge swipes
        document.getElementById("pinUnpinFromAppbar").disabled = false;

        // Set the buttons on the command bar
        setAppbarButton();

        // And, to make things clear for this sample, let's show the app bar by default when switching to the scenario
        document.getElementById("pinUnpinFromAppbar").winControl.sticky = true;
    }

    function appbarButtonClicked() {
        document.getElementById("pinUnpinFromAppbar").winControl.sticky = true;
        // Check which appBar command button we received...
        if (WinJS.UI.AppBarIcon.unpin === document.getElementById("commandButton").winControl.icon) {
            unpinByElementAsync(document.getElementById("commandButton"), appbarTileId).done(function (isDeleted) {
                if (isDeleted) {
                    WinJS.log && WinJS.log("Secondary tile was successfully unpinned.", "sample", "status");
                    setAppbarButton();
                } else {
                    WinJS.log && WinJS.log("Secondary tile was not unpinned.", "sample", "error");
                    setAppbarButton();
                }
            });
        } else {
            pinByElementAsync(document.getElementById("commandButton"), appbarTileId, "Appbar pinned secondary tile", "A secondary tile that was pinned by the user from the Appbar").done(function (isCreated) {
                if (isCreated) {
                    WinJS.log && WinJS.log("Secondary tile was successfully pinned.", "sample", "status");
                    setAppbarButton();
                } else {
                    WinJS.log && WinJS.log("Secondary tile was not pinned.", "sample", "error");
                    setAppbarButton();
                }
            });
        }
    }
})();
