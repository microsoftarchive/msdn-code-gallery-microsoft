//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var Notifications = Windows.UI.Notifications;
    var Start = Windows.UI.StartScreen;
    var badgeTileId = "ST_BADGE";
    var textTileId = "ST_BADGE_AND_TEXT";

    var page = WinJS.UI.Pages.define("/html/scenario3.html", {
        ready: function (element, options) {
            document.getElementById("scenario3Badge").addEventListener("click", scenario3Badge, false);
            document.getElementById("scenario3BadgeAndText").addEventListener("click", scenario3BadgeAndText, false);
        }
    });

    function scenario3Badge(e) {
        if (!Start.SecondaryTile.exists(badgeTileId)) {
            // Construct the tile, set the lock screen properties
            var secondTile = new Start.SecondaryTile(
                badgeTileId,
                "LockScreen JS - Badge only",
                "BADGE_ARGS",
                new Windows.Foundation.Uri("ms-appx:///images/squareTile-sdk.png"),
                Start.TileSize.square150x150
                );
            secondTile.lockScreenBadgeLogo = new Windows.Foundation.Uri("ms-appx:///images/badgelogo-sdk.png");

            // Find the location of the button that was clicked on, use it to play the popup window
            var buttonRect = e.srcElement.getBoundingClientRect();

            secondTile.requestCreateForSelectionAsync({ x: buttonRect.left, y: buttonRect.top, width: buttonRect.width, height: buttonRect.height }, Windows.UI.Popups.Placement.above).done(function (isPinned) {
                if (isPinned) {
                    var badgeContent = new NotificationsExtensions.BadgeContent.BadgeNumericNotificationContent(2);
                    Notifications.BadgeUpdateManager.createBadgeUpdaterForSecondaryTile(badgeTileId).update(badgeContent.createNotification());
                    WinJS.log && WinJS.log("Secondary tile created and badge updated. Go to PC settings to add it to the lock screen.", "sample", "status");
                } else {
                    WinJS.log && WinJS.log("Tile not created.", "sample", "error");
                }
            });
        } else {
            WinJS.log && WinJS.log("Badge secondary tile already exists.", "sample", "error");
        }
    }

    function scenario3BadgeAndText(e) {
        if (!Start.SecondaryTile.exists(textTileId)) {
            // Construct the tile, set the lock screen properties
            // In this case, a wide tile property will also be set
            var secondTile = new Start.SecondaryTile(
                textTileId,
                "LockScreen JS - Badge and tile text",
                "TEXT_ARGS",
                new Windows.Foundation.Uri("ms-appx:///images/squareTile-sdk.png"),
                Start.TileSize.wide310x150
                );
            secondTile.visualElements.wide310x150Logo = new Windows.Foundation.Uri("ms-appx:///images/tile-sdk.png");
            secondTile.lockScreenBadgeLogo = new Windows.Foundation.Uri("ms-appx:///images/badgelogo-sdk.png");
            secondTile.lockScreenDisplayBadgeAndTileText = true;

            // Find the location of the button that was clicked on, use it to play the popup window
            var buttonRect = e.srcElement.getBoundingClientRect();

            secondTile.requestCreateForSelectionAsync({ x: buttonRect.left, y: buttonRect.top, width: buttonRect.width, height: buttonRect.height }, Windows.UI.Popups.Placement.above).done(function (isPinned) {
                if (isPinned) {
                    var tileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWide310x150Text03();
                    tileContent.textHeadingWrap.text = "Text for the lock screen";
                    tileContent.requireSquare150x150Content = false;
                    Notifications.TileUpdateManager.createTileUpdaterForSecondaryTile(textTileId).update(tileContent.createNotification());
                    WinJS.log && WinJS.log("Secondary tile created and updated. Go to PC settings to add it to the lock screen.", "sample", "status");
                } else {
                    WinJS.log && WinJS.log("Tile not created.", "sample", "error");
                }
            });
        } else {
            WinJS.log && WinJS.log("Badge and text secondary tile already exists.", "sample", "error");
        }
    }
})();
