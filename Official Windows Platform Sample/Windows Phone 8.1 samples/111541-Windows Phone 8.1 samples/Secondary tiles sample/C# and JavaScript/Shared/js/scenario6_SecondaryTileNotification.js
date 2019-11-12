//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var liveTileId = "SecondaryTile.LiveTile";
    var page = WinJS.UI.Pages.define("/html/scenario6_SecondaryTileNotification.html", {
        ready: function (element, options) {
            document.getElementById("pinLiveTile").addEventListener("click", pinLiveTile, false);
            document.getElementById("sendTileNotification").addEventListener("click", sendTileNotification, false);
            document.getElementById("sendBadgeNotification").addEventListener("click", sendBadgeNotification, false);
            document.getElementById("sendTileNotificationString").addEventListener("click", sendTileNotificationWithStringManipulation, false);
            document.getElementById("sendBadgeNotificationString").addEventListener("click", sendBadgeNotificationWithStringManipulation, false);
        }
    });

    function pinLiveTile() {
        var square150x150Logo = new Windows.Foundation.Uri("ms-appx:///Images/square150x150Tile-sdk.png");
        var wide310x150Logo = new Windows.Foundation.Uri("ms-appx:///Images/wide310x150Tile-sdk.png");
        var tileActivationArguments = liveTileId + " WasPinnedAt=" + new Date();

        // Create the secondary tile just like we did in pinTile scenario.
        // Provide a wideLogo since wide tiles have a few more templates available for notifications
        var tile = new Windows.UI.StartScreen.SecondaryTile(liveTileId,
                                                            "A Live Secondary Tile",
                                                            tileActivationArguments,
                                                            square150x150Logo,
                                                            Windows.UI.StartScreen.TileSize.wide310x150);

        // Adding the wide tile logo.
        tile.visualElements.wide310x150Logo = wide310x150Logo;

        // The display of the app name can be controlled for each tile size.
        // The default is false.
        tile.visualElements.showNameOnSquare150x150Logo = true;
        tile.visualElements.showNameOnWide310x150Logo = true;

        // Specify a foreground text value.
        // The tile background color is inherited from the parent unless a separate value is specified.
        tile.visualElements.foregroundText = Windows.UI.StartScreen.ForegroundText.light;

        var selectionRect = document.getElementById("pinLiveTile").getBoundingClientRect();
        tile.requestCreateForSelectionAsync({ x: selectionRect.left, y: selectionRect.top, width: selectionRect.width, height: selectionRect.height }, Windows.UI.Popups.Placement.below).done(function (isCreated) {
            if (isCreated) {
                WinJS.log && WinJS.log("Secondary tile was successfully pinned.", "sample", "status");
            } else {
                WinJS.log && WinJS.log("Secondary tile was not pinned.", "sample", "error");
            }
        });
    }

    function sendTileNotification() {
        // We can only send a notification for a tile that is pinned. So let's make sure the tile is pinned before we try to send the notification.
        if (Windows.UI.StartScreen.SecondaryTile.exists(liveTileId)) {
            // Note: This sample contains an additional reference, NotificationsExtensions, which you can use in your own apps
            var tileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWide310x150Text04();
            tileContent.textBodyWrap.text = "Sent to a secondary tile from NotificationsExtensions";

            var squareTileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileSquare150x150Text04();
            squareTileContent.textBodyWrap.text = "Sent to a secondary tile from NotificationsExtensions";
            tileContent.square150x150Content = squareTileContent;

            // instead of creating a tileUpdater for the application, create one for the secondary tile and pass in the secondary tileId
            Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForSecondaryTile("SecondaryTile.LiveTile").update(tileContent.createNotification());

            WinJS.log && WinJS.log("Tile notification sent to SecondaryTile.LiveTile", "sample", "status");
        } else {
            WinJS.log && WinJS.log("SecondaryTile.LiveTile is not pinned to Start, so it's not possible to send it a notification.", "sample", "error");
        }
    }

    function sendBadgeNotification() {
        if (Windows.UI.StartScreen.SecondaryTile.exists(liveTileId)) {
            // Note: This sample contains an additional reference, NotificationsExtensions, which you can use in your own apps
            var badgeContent = new NotificationsExtensions.BadgeContent.BadgeNumericNotificationContent(6);

            // instead of creating a badgeUpdater for the application, create one for the secondary tile and pass in the secondary tileId
            Windows.UI.Notifications.BadgeUpdateManager.createBadgeUpdaterForSecondaryTile("SecondaryTile.LiveTile").update(badgeContent.createNotification());

            WinJS.log && WinJS.log("Badge notification sent to SecondaryTile.LiveTile", "sample", "status");
        } else {
            WinJS.log && WinJS.log("SecondaryTile.LiveTile is not pinned to Start, so it's not possible to send it a notification.", "sample", "error");
        }
    }

    // Creating a tile with a string and loading it into a DOM
    function sendTileNotificationWithStringManipulation() {
        // We can only send a notification for a tile that is pinned. So let's make sure the tile is pinned before we try to send the notification.
        if (Windows.UI.StartScreen.SecondaryTile.exists("SecondaryTile.LiveTile")) {
            var tileXmlString = "<tile>"
                              + "<visual version='2'>"
                              + "<binding template='TileWide310x150Text04' fallback='TileWideText04'>"
                              + "<text id='1'>Send to a secondary tile from strings</text>"
                              + "</binding>"
                              + "<binding template='TileSquare150x150Text04' fallback='TileSquareText04'>"
                              + "<text id='1'>Send to a secondary tile from strings</text>"
                              + "</binding>"
                              + "</visual>"
                              + "</tile>";

            var tileDOM = new Windows.Data.Xml.Dom.XmlDocument();
            tileDOM.loadXml(tileXmlString);
            var tile = new Windows.UI.Notifications.TileNotification(tileDOM);

            // instead of creating a tileUpdater for the application, create one for the secondary tile and pass in the secondary tileId
            Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForSecondaryTile("SecondaryTile.LiveTile").update(tile);

            WinJS.log && WinJS.log("Tile notification sent to SecondaryTile.LiveTile", "sample", "status");
        } else {
            WinJS.log && WinJS.log("SecondaryTile.LiveTile is not pinned to Start, so it's not possible to send it a notification.", "sample", "error");
        }
    }

    // Creating a badge with a string and loading it into a DOM
    function sendBadgeNotificationWithStringManipulation() {
        if (Windows.UI.StartScreen.SecondaryTile.exists("SecondaryTile.LiveTile")) {
            var badgeXmlString = "<badge value='9'/>";
            var badgeDOM = new Windows.Data.Xml.Dom.XmlDocument();
            badgeDOM.loadXml(badgeXmlString);
            var badge = new Windows.UI.Notifications.BadgeNotification(badgeDOM);

            // instead of creating a badgeUpdater for the application, create one for the secondary tile and pass in the secondary tileId
            Windows.UI.Notifications.BadgeUpdateManager.createBadgeUpdaterForSecondaryTile("SecondaryTile.LiveTile").update(badge);

            WinJS.log && WinJS.log("Badge notification sent to SecondaryTile.LiveTile", "sample", "status");
        } else {
            WinJS.log && WinJS.log("SecondaryTile.LiveTile is not pinned to Start, so it's not possible to send it a notification.", "sample", "error");
        }
    }
})();
