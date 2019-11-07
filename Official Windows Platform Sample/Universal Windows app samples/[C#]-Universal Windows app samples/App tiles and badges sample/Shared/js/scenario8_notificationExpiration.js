//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";
    var page = WinJS.UI.Pages.define("/html/scenario8_notificationExpiration.html", {
        ready: function (element, options) {
            document.getElementById("useTileNotificationExpiration").addEventListener("click", useTileNotificationExpiration, false);
        }
    });

    function useTileNotificationExpiration() {
        var currentTime = new Date();
        var seconds = document.getElementById("seconds").value;
        var numericExpression = /^[0-9]+$/;
        if (!(seconds.length > 0 && seconds.length <= 3 && seconds.match(numericExpression))) {
            seconds = 10;
        }

        // Create a notification for the Square310x310 tile using one of the available templates for the size.
        var square310x310TileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileSquare310x310Text09();
        square310x310TileContent.textHeadingWrap.text = "This notification will expire at " + new Date(currentTime.getTime() + seconds * 1000);
        square310x310TileContent.branding = NotificationsExtensions.TileContent.TileBranding.none;

        // Create a notification for the Wide310x150 tile using one of the available templates for the size.
        var wide310x150TileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileWide310x150Text04();
        wide310x150TileContent.textBodyWrap.text = "This notification will expire at " + new Date(currentTime.getTime() + seconds * 1000);
        wide310x150TileContent.branding = NotificationsExtensions.TileContent.TileBranding.none;

        // Create a notification for the Square150x150 tile using one of the available templates for the size.
        var square150x150TileContent = NotificationsExtensions.TileContent.TileContentFactory.createTileSquare150x150Text04();
        square150x150TileContent.textBodyWrap.text = "This notification will expire at " + new Date(currentTime.getTime() + seconds * 1000);
        square150x150TileContent.branding = NotificationsExtensions.TileContent.TileBranding.none;

        // Attach the Square150x150 template to the Wide310x150 template.
        wide310x150TileContent.square150x150Content = square150x150TileContent;

        // Attach the Wide310x150 template to the Square310x310 template.
        square310x310TileContent.wide310x150Content = wide310x150TileContent;

        var tileNotification = square310x310TileContent.createNotification();

        var expiryTime = new Date(currentTime.getTime() + seconds * 1000);

        // Set the expiration time and update the tile.
        tileNotification.expirationTime = expiryTime;
        Windows.UI.Notifications.TileUpdateManager.createTileUpdaterForApplication().update(tileNotification);

        WinJS.log && WinJS.log("Tile notification sent. It will expire at " + expiryTime, "sample", "status");
    }
})();