//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var liveTileId = "SecondaryTile.LiveTile";
    var page = WinJS.UI.Pages.define("/html/SecondaryTileNotification.html", {
        ready: function (element, options) {
            document.getElementById("pinLiveTile").addEventListener("click", pinLiveTile, false);
            document.getElementById("sendTileNotification").addEventListener("click", sendTileNotification, false);
            document.getElementById("sendBadgeNotification").addEventListener("click", sendBadgeNotification, false);
            document.getElementById("sendTileNotificationString").addEventListener("click", sendTileNotificationWithStringManipulation, false);
            document.getElementById("sendBadgeNotificationString").addEventListener("click", sendBadgeNotificationWithStringManipulation, false);
        }
    });

    function pinLiveTile() {
        var uriLogo = new Windows.Foundation.Uri("ms-appx:///images/SecondaryTileDefault-sdk.png");
        var uriWideLogo = new Windows.Foundation.Uri("ms-appx:///images/tile-sdk.png");
        var tileActivationArguments = liveTileId + " WasPinnedAt=" + new Date();

        // Create the secondary tile just like we did in pinTile scenario.
        // Provide a wideLogo since wide tiles have a few more templates available for notifications
        var tile = new Windows.UI.StartScreen.SecondaryTile(liveTileId,
                                                            "A Live Secondary Tile",
                                                            "Secondary Tile Sample Live Secondary Tile",
                                                            tileActivationArguments,
                                                            Windows.UI.StartScreen.TileOptions.showNameOnLogo,
                                                            uriLogo,
                                                            uriWideLogo);

        tile.foregroundText = Windows.UI.StartScreen.ForegroundText.light;
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
            var tileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWideText04();
            tileContent.textBodyWrap.text = "Sent to a secondary tile from NotificationsExtensions";

            var squareTileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileSquareText04();
            squareTileContent.textBodyWrap.text = "Sent to a secondary tile from NotificationsExtensions";
            tileContent.squareContent = squareTileContent;

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
                              + "<visual>"
                              + "<binding template='TileWideText04'>"
                              + "<text id='1'>Send to a secondary tile from strings</text>"
                              + "</binding>"
                              + "<binding template='TileSquareText04'>"
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
