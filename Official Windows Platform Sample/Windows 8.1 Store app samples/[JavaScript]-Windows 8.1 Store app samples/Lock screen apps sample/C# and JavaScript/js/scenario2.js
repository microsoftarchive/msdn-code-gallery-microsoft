//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var Notifications = Windows.UI.Notifications;

    var page = WinJS.UI.Pages.define("/html/scenario2.html", {
        ready: function (element, options) {
            document.getElementById("scenario2SendBadge").addEventListener("click", scenario2SendBadge, false);
            document.getElementById("scenario2SendBadgeString").addEventListener("click", scenario2SendBadgeWithStringManipulation, false);
            document.getElementById("scenario2ClearBadge").addEventListener("click", scenario2ClearBadge, false);
            document.getElementById("scenario2SendTile").addEventListener("click", scenario2SendTile, false);
            document.getElementById("scenario2SendTileString").addEventListener("click", scenario2SendTileWithStringManipulation, false);
            document.getElementById("scenario2ClearTile").addEventListener("click", scenario2ClearTile, false);
        }
    });

    function scenario2SendBadge() {
        var badgeContent = new NotificationsExtensions.BadgeContent.BadgeNumericNotificationContent(6);
        Notifications.BadgeUpdateManager.createBadgeUpdaterForApplication().update(badgeContent.createNotification());
        WinJS.log && WinJS.log(badgeContent.getContent(), "sample", "status");
    }

    function scenario2SendBadgeWithStringManipulation() {
        var badgeXmlString = "<badge value='6'/>";
        var badgeDOM = new Windows.Data.Xml.Dom.XmlDocument();
        badgeDOM.loadXml(badgeXmlString);
        var badge = new Notifications.BadgeNotification(badgeDOM);
        Notifications.BadgeUpdateManager.createBadgeUpdaterForApplication().update(badge);
        WinJS.log && WinJS.log("Badge notification sent", "sample", "status");
    }

    function scenario2ClearBadge() {
        // The same BadgeUpdateManager can be used to clear the tile since we're sending badge notifications to the application's default tile
        Notifications.BadgeUpdateManager.createBadgeUpdaterForApplication().clear();
        WinJS.log && WinJS.log("Badge notification cleared", "sample", "status");
    }

    function scenario2SendTile() {
        var tileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWide310x150SmallImageAndText03();
        tileContent.textBodyWrap.text = "This tile notification has an image, but it won't be displayed on the lock screen";
        tileContent.image.src = "ms-appx:///images/tile-sdk.png";
        tileContent.requireSquare150x150Content = false;
        Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileContent.createNotification());
        WinJS.log && WinJS.log("Tile notification sent", "sample", "status");
    }

    function scenario2SendTileWithStringManipulation() {
        var tileXmlString = "<tile>"
                          + "<visual version='2'>"
                          + "<binding template='TileWide310x150SmallImageAndText03' fallback='TileWideSmallImageAndText03'>"
                          + "<image id='1' src='ms-appx:///images/tile-sdk.png'/>"
                          + "<text id='1'>This tile notification has an image, but it won't be displayed on the lock screen</text>"
                          + "</binding>"
                          + "</visual>"
                          + "</tile>";

        var tileDOM = new Windows.Data.Xml.Dom.XmlDocument();
        tileDOM.loadXml(tileXmlString);
        var tile = new Notifications.TileNotification(tileDOM);
        Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tile);
        WinJS.log && WinJS.log("Tile notification sent", "sample", "status");
    }

    function scenario2ClearTile() {
        Notifications.TileUpdateManager.createTileUpdaterForApplication().clear();
        WinJS.log && WinJS.log("Tile notification cleared", "sample", "status");
    }
})();
